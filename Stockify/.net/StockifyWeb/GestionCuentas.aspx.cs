using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using StockifyWeb.StockifyWS;

namespace StockifyWeb
{
    public partial class GestionCuentas : System.Web.UI.Page
    {
        // Configuración de Cognito
        private const string UserPoolId = "us-east-1_LIZsvOxNv";
        private const string ClientId = "5f0hvfclu5ichnmd8r1vjs3rpl";
        private const string ClientSecret = "1sbcm6efocmo314c8re3dqkg6pj2fhi984vfc95vcd431q0s5a6k";
        private static readonly RegionEndpoint CognitoRegion = RegionEndpoint.USEast1;

        private CuentaUsuarioWSClient clienteCuenta;
        private UsuarioWSClient clienteUsuario;

        public GestionCuentas()
        {
            this.clienteCuenta = new CuentaUsuarioWSClient();
            this.clienteUsuario = new UsuarioWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar que el usuario esté logueado
                if (Session["IdUsuario"] == null)
                {
                    Response.Redirect("Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                string tipoUsuario = Session["TipoUsuario"]?.ToString();

                // Solo ADMINISTRADOR y PRINCIPAL pueden acceder
                if (tipoUsuario != "ADMINISTRADOR" && tipoUsuario != "PRINCIPAL")
                {
                    Response.Redirect("Inicio.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                CargarCuentas();
            }
        }

        private void CargarCuentas()
        {
            try
            {
                var usuarios = clienteUsuario.listarUsuarios();

                if (usuarios == null || usuarios.Length == 0)
                {
                    rptCuentas.Visible = false;
                    pnlEmpty.Visible = true;
                    return;
                }

                int idUsuarioActual = Session["IdUsuario"] != null ? Convert.ToInt32(Session["IdUsuario"]) : 0;
                string tipoUsuarioActual = Session["TipoUsuario"]?.ToString();

                var listaUsuarios = usuarios
                    .Where(u => u.cuenta != null)
                    .Select(u => new
                    {
                        IdUsuario = u.idUsuario,
                        Username = u.cuenta.username,
                        Nombres = u.nombres ?? "",
                        Apellidos = u.apellidos ?? "",
                        NombreCompleto = $"{u.nombres ?? ""} {u.apellidos ?? ""}".Trim(),
                        Email = u.email ?? "",
                        Telefono = u.telefono ?? "",
                        TipoUsuario = u.tipoUsuarioSpecified ? u.tipoUsuario.ToString() : "OPERARIO",
                        Activo = u.activo,
                        // Determinar si se puede eliminar
                        PuedeEliminar = PuedeEliminarUsuario(u.idUsuario, u.tipoUsuario, idUsuarioActual, tipoUsuarioActual)
                    })
                    .ToList();

                rptCuentas.DataSource = listaUsuarios;
                rptCuentas.DataBind();
                rptCuentas.Visible = true;
                pnlEmpty.Visible = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar cuentas: {ex.Message}");
                MostrarError("Error al cargar las cuentas de usuario.");
            }
        }

        private bool PuedeEliminarUsuario(int idUsuarioAEliminar, StockifyWS.tipoUsuario tipoUsuarioAEliminar, int idUsuarioActual, string tipoUsuarioActual)
        {
            // No puede eliminar al usuario con sesión iniciada
            if (idUsuarioAEliminar == idUsuarioActual)
            {
                return false;
            }

            // PRINCIPAL puede eliminar a cualquiera excepto a sí mismo
            if (tipoUsuarioActual == "PRINCIPAL")
            {
                return true;
            }

            // ADMINISTRADOR solo puede eliminar a OPERARIOS
            if (tipoUsuarioActual == "ADMINISTRADOR")
            {
                return tipoUsuarioAEliminar == StockifyWS.tipoUsuario.OPERARIO;
            }

            // Otros tipos no pueden eliminar
            return false;
        }

        protected void btnGuardarCuenta_Click(object sender, EventArgs e)
        {
            bool modoEdicion = hfModoEdicion.Value == "true";

            if (modoEdicion)
            {
                RegisterAsyncTask(new PageAsyncTask(EditarUsuarioAsync));
            }
            else
            {
                RegisterAsyncTask(new PageAsyncTask(CrearUsuarioAsync));
            }
        }

        private async Task CrearUsuarioAsync()
        {
            string username = txtUsername.Text.Trim();
            string nombres = txtNombres.Text.Trim();
            string apellidos = txtApellidos.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string password = txtPassword.Text;
            string tipoUsuarioSeleccionado = ddlTipoUsuario.SelectedValue;
            bool activo = ddlActivo.SelectedValue == "true";

            try
            {
                // Validar permisos: solo ADMINISTRADOR y PRINCIPAL pueden crear usuarios
                string tipoUsuarioActual = Session["TipoUsuario"]?.ToString();
                if (tipoUsuarioActual != "ADMINISTRADOR" && tipoUsuarioActual != "PRINCIPAL")
                {
                    MostrarError("No tiene permisos para crear usuarios.");
                    return;
                }

                // ADMINISTRADOR no puede crear otros ADMINISTRADORES ni PRINCIPALES
                if (tipoUsuarioActual == "ADMINISTRADOR" &&
                    (tipoUsuarioSeleccionado == "ADMINISTRADOR" || tipoUsuarioSeleccionado == "PRINCIPAL"))
                {
                    MostrarError("Los administradores solo pueden crear usuarios de tipo OPERARIO.");
                    return;
                }

                // Solo puede haber un usuario PRINCIPAL
                if (tipoUsuarioSeleccionado == "PRINCIPAL")
                {
                    if (ExisteUsuarioPrincipal())
                    {
                        MostrarError("Ya existe un usuario PRINCIPAL en el sistema.");
                        return;
                    }
                }

                // PASO 1: Validar que el usuario no exista en la BD
                if (UsuarioExisteEnBD(username))
                {
                    MostrarError("El nombre de usuario ya existe en el sistema.");
                    return;
                }

                // PASO 2: Crear usuario en AWS Cognito
                bool cognitoCreado = await CrearUsuarioEnCognitoAsync(username, email, password);

                if (!cognitoCreado)
                {
                    return; // El error ya fue mostrado
                }

                // PASO 3: Guardar en Base de Datos
                try
                {
                    // 3.1: Crear cuenta de usuario
                    cuentaUsuario nuevaCuenta = new cuentaUsuario
                    {
                        username = username,
                        password = HashPassword(password),
                        ultimoAcceso = DateTime.Now,
                        ultimoAccesoSpecified = true
                    };

                    clienteCuenta.guardarCuentaUsuario(nuevaCuenta, estado.NUEVO);

                    // Esperar un momento para que se guarde en BD
                    await Task.Delay(500);

                    // Obtener el ID generado
                    var cuentaCreada = ObtenerCuentaPorUsername(username);
                    if (cuentaCreada == null)
                    {
                        await EliminarUsuarioDeCognitoAsync(username);
                        MostrarError("Error al recuperar la cuenta creada.");
                        return;
                    }

                    // 3.2: Crear usuario
                    StockifyWS.tipoUsuario tipoUsuarioEnum;
                    switch (tipoUsuarioSeleccionado)
                    {
                        case "ADMINISTRADOR":
                            tipoUsuarioEnum = StockifyWS.tipoUsuario.ADMINISTRADOR;
                            break;
                        case "PRINCIPAL":
                            tipoUsuarioEnum = StockifyWS.tipoUsuario.PRINCIPAL;
                            break;
                        default:
                            tipoUsuarioEnum = StockifyWS.tipoUsuario.OPERARIO;
                            break;
                    }

                    usuario nuevoUsuario = new usuario
                    {
                        nombres = nombres,
                        apellidos = apellidos,
                        email = email,
                        telefono = string.IsNullOrEmpty(telefono) ? null : telefono,
                        activo = activo,
                        cuenta = cuentaCreada,
                        tipoUsuario = tipoUsuarioEnum,
                        tipoUsuarioSpecified = true
                    };

                    try
                    {
                        clienteUsuario.guardarUsuario(nuevoUsuario, estado.NUEVO);

                        // Si llegamos aquí, todo fue exitoso
                        Response.Redirect("GestionCuentas.aspx?success=Usuario creado exitosamente", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    catch (Exception ex)
                    {
                        // Rollback completo
                        System.Diagnostics.Debug.WriteLine($"Error al guardar usuario: {ex.Message}");
                        clienteCuenta.eliminarCuentaUsuario(cuentaCreada.idCuentaUsuario);
                        await EliminarUsuarioDeCognitoAsync(username);
                        MostrarError("Error al crear el usuario en la base de datos.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en BD: {ex.Message}");
                    // Rollback: eliminar de Cognito
                    await EliminarUsuarioDeCognitoAsync(username);
                    MostrarError("Error al guardar en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general: {ex.Message}");
                MostrarError("Error al crear el usuario.");
            }
            finally
            {
                CerrarClientes();
            }
        }

        private async Task EditarUsuarioAsync()
        {
            int idUsuario = int.Parse(hfIdUsuario.Value);
            string nombres = txtNombres.Text.Trim();
            string apellidos = txtApellidos.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string tipoUsuarioSeleccionado = ddlTipoUsuario.SelectedValue;
            bool activo = ddlActivo.SelectedValue == "true";

            try
            {
                // Obtener usuario actual
                var usuarios = clienteUsuario.listarUsuarios();
                var usuarioActual = usuarios.FirstOrDefault(u => u.idUsuario == idUsuario);

                if (usuarioActual == null)
                {
                    MostrarError("Usuario no encontrado.");
                    return;
                }

                // Validar permisos
                string tipoUsuarioSesion = Session["TipoUsuario"]?.ToString();
                int idUsuarioSesion = Session["IdUsuario"] != null ? Convert.ToInt32(Session["IdUsuario"]) : 0;

                // ADMINISTRADOR no puede editar a otros ADMINISTRADORES o PRINCIPALES (excepto a sí mismo)
                if (tipoUsuarioSesion == "ADMINISTRADOR" && idUsuarioSesion != idUsuario)
                {
                    if (usuarioActual.tipoUsuario == StockifyWS.tipoUsuario.ADMINISTRADOR ||
                        usuarioActual.tipoUsuario == StockifyWS.tipoUsuario.PRINCIPAL)
                    {
                        MostrarError("No tiene permisos para editar este usuario.");
                        return;
                    }
                }

                // No se puede cambiar el tipo de usuario PRINCIPAL a otro tipo
                if (usuarioActual.tipoUsuario == StockifyWS.tipoUsuario.PRINCIPAL &&
                    tipoUsuarioSeleccionado != "PRINCIPAL")
                {
                    MostrarError("No se puede cambiar el tipo de usuario PRINCIPAL.");
                    return;
                }

                // No se puede cambiar a PRINCIPAL si ya existe uno
                if (tipoUsuarioSeleccionado == "PRINCIPAL" &&
                    usuarioActual.tipoUsuario != StockifyWS.tipoUsuario.PRINCIPAL)
                {
                    if (ExisteUsuarioPrincipal())
                    {
                        MostrarError("Ya existe un usuario PRINCIPAL en el sistema.");
                        return;
                    }
                }

                // ADMINISTRADOR no puede promover a ADMINISTRADOR o PRINCIPAL
                if (tipoUsuarioSesion == "ADMINISTRADOR" &&
                    (tipoUsuarioSeleccionado == "ADMINISTRADOR" || tipoUsuarioSeleccionado == "PRINCIPAL"))
                {
                    MostrarError("Los administradores no pueden crear usuarios de tipo ADMINISTRADOR o PRINCIPAL.");
                    return;
                }

                string username = usuarioActual.cuenta.username;
                bool cambioEstado = usuarioActual.activo != activo;
                bool cambioEmail = usuarioActual.email != email;

                // PASO 1: Actualizar en Cognito
                bool cognitoActualizado = await ActualizarUsuarioEnCognitoAsync(username, email, activo, cambioEstado);

                if (!cognitoActualizado)
                {
                    return; // El error ya fue mostrado
                }

                // PASO 2: Actualizar en Base de Datos
                try
                {
                    StockifyWS.tipoUsuario tipoUsuarioEnum;
                    switch (tipoUsuarioSeleccionado)
                    {
                        case "ADMINISTRADOR":
                            tipoUsuarioEnum = StockifyWS.tipoUsuario.ADMINISTRADOR;
                            break;
                        case "PRINCIPAL":
                            tipoUsuarioEnum = StockifyWS.tipoUsuario.PRINCIPAL;
                            break;
                        default:
                            tipoUsuarioEnum = StockifyWS.tipoUsuario.OPERARIO;
                            break;
                    }

                    usuarioActual.nombres = nombres;
                    usuarioActual.apellidos = apellidos;
                    usuarioActual.email = email;
                    usuarioActual.telefono = string.IsNullOrEmpty(telefono) ? null : telefono;
                    usuarioActual.activo = activo;
                    usuarioActual.tipoUsuario = tipoUsuarioEnum;
                    usuarioActual.tipoUsuarioSpecified = true;

                    clienteUsuario.guardarUsuario(usuarioActual, estado.MODIFICADO);

                    Response.Redirect("GestionCuentas.aspx?success=Usuario actualizado exitosamente", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en BD: {ex.Message}");
                    MostrarError("Error al actualizar en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general: {ex.Message}");
                MostrarError("Error al editar el usuario.");
            }
            finally
            {
                CerrarClientes();
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(EliminarUsuarioAsync));
        }

        private async Task EliminarUsuarioAsync()
        {
            int idUsuarioEliminar = int.Parse(hfIdEliminar.Value);

            try
            {
                // Validaciones de permisos
                int idUsuarioSesion = Session["IdUsuario"] != null ? Convert.ToInt32(Session["IdUsuario"]) : 0;
                string tipoUsuarioSesion = Session["TipoUsuario"]?.ToString();

                // No puede eliminar al usuario con sesión iniciada
                if (idUsuarioEliminar == idUsuarioSesion)
                {
                    MostrarError("No puede eliminar el usuario con el que tiene la sesión iniciada.");
                    return;
                }

                // Obtener usuario a eliminar
                var usuarios = clienteUsuario.listarUsuarios();
                var usuarioEliminar = usuarios.FirstOrDefault(u => u.idUsuario == idUsuarioEliminar);

                if (usuarioEliminar == null)
                {
                    MostrarError("Usuario no encontrado.");
                    return;
                }

                // ADMINISTRADOR solo puede eliminar a OPERARIOS
                if (tipoUsuarioSesion == "ADMINISTRADOR")
                {
                    if (usuarioEliminar.tipoUsuario != StockifyWS.tipoUsuario.OPERARIO)
                    {
                        MostrarError("Los administradores solo pueden eliminar usuarios de tipo OPERARIO.");
                        return;
                    }
                }

                // PRINCIPAL puede eliminar a cualquiera (ya verificamos que no sea él mismo)

                // Solo ADMINISTRADOR y PRINCIPAL pueden eliminar
                if (tipoUsuarioSesion != "ADMINISTRADOR" && tipoUsuarioSesion != "PRINCIPAL")
                {
                    MostrarError("No tiene permisos para eliminar usuarios.");
                    return;
                }

                string username = usuarioEliminar.cuenta.username;
                int idCuenta = usuarioEliminar.cuenta.idCuentaUsuario;

                // PASO 1: Eliminar de Cognito
                bool cognitoEliminado = await EliminarUsuarioDeCognitoAsync(username);

                if (!cognitoEliminado)
                {
                    MostrarError("Error al eliminar el usuario de AWS Cognito.");
                    return;
                }

                // PASO 2: Eliminar de Base de Datos
                try
                {
                    // Primero eliminar usuario (tiene FK a cuenta)
                    clienteUsuario.eliminarUsuario(usuarioEliminar.idUsuario);

                    // Esperar un momento para confirmar eliminación
                    await Task.Delay(300);

                    // Luego eliminar cuenta
                    var cuenta = clienteCuenta.listarCuentasUsuario()
                        .FirstOrDefault(c => c.idCuentaUsuario == idCuenta);

                    if (cuenta != null)
                    {
                        clienteCuenta.eliminarCuentaUsuario(cuenta.idCuentaUsuario);
                    }

                    Response.Redirect("GestionCuentas.aspx?success=Usuario eliminado exitosamente", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en BD: {ex.Message}");
                    MostrarError("Error al eliminar de la base de datos. El usuario fue eliminado de Cognito pero permanece en la BD.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general: {ex.Message}");
                MostrarError("Error al eliminar el usuario.");
            }
            finally
            {
                CerrarClientes();
            }
        }

        // ==================== MÉTODOS DE AWS COGNITO ====================

        private async Task<bool> CrearUsuarioEnCognitoAsync(string username, string email, string password)
        {
            try
            {
                string accessKey = System.Configuration.ConfigurationManager.AppSettings["AWS.AccessKey"];
                string secretKey = System.Configuration.ConfigurationManager.AppSettings["AWS.SecretKey"];
                string sessionToken = System.Configuration.ConfigurationManager.AppSettings["AWS.SessionToken"];

                var credentials = new Amazon.Runtime.SessionAWSCredentials(accessKey, secretKey, sessionToken);

                using (var provider = new AmazonCognitoIdentityProviderClient(credentials, CognitoRegion))
                {
                    // Crear el usuario
                    var createRequest = new AdminCreateUserRequest
                    {
                        UserPoolId = UserPoolId,
                        Username = username,
                        UserAttributes = new List<AttributeType>
                        {
                            new AttributeType { Name = "email", Value = email },
                            new AttributeType { Name = "email_verified", Value = "true" }
                        },
                        TemporaryPassword = password,
                        MessageAction = MessageActionType.SUPPRESS // No enviar email
                    };

                    System.Diagnostics.Debug.WriteLine($"Creando usuario en Cognito: {username}");
                    await provider.AdminCreateUserAsync(createRequest);
                    System.Diagnostics.Debug.WriteLine("Usuario creado en Cognito");

                    // Establecer contraseña permanente
                    var setPasswordRequest = new AdminSetUserPasswordRequest
                    {
                        UserPoolId = UserPoolId,
                        Username = username,
                        Password = password,
                        Permanent = true
                    };

                    System.Diagnostics.Debug.WriteLine("Estableciendo contraseña permanente...");
                    await provider.AdminSetUserPasswordAsync(setPasswordRequest);
                    System.Diagnostics.Debug.WriteLine("Contraseña establecida");

                    return true;
                }
            }
            catch (UsernameExistsException)
            {
                System.Diagnostics.Debug.WriteLine("Error: Usuario ya existe en Cognito");
                MostrarError("El nombre de usuario ya existe en AWS Cognito.");
                return false;
            }
            catch (InvalidPasswordException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: Contraseña inválida - {ex.Message}");
                MostrarError("Contraseña inválida: Debe tener al menos 8 caracteres, incluyendo mayúsculas, minúsculas, números y caracteres especiales.");
                return false;
            }
            catch (InvalidParameterException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: Parámetro inválido - {ex.Message}");
                MostrarError($"Parámetro inválido: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error Cognito: {ex.GetType().Name} - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MostrarError($"Error al crear el usuario en AWS Cognito: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ActualizarUsuarioEnCognitoAsync(string username, string email, bool activo, bool cambioEstado)
        {
            try
            {
                string accessKey = System.Configuration.ConfigurationManager.AppSettings["AWS.AccessKey"];
                string secretKey = System.Configuration.ConfigurationManager.AppSettings["AWS.SecretKey"];
                string sessionToken = System.Configuration.ConfigurationManager.AppSettings["AWS.SessionToken"];

                var credentials = new Amazon.Runtime.SessionAWSCredentials(accessKey, secretKey, sessionToken);

                using (var provider = new AmazonCognitoIdentityProviderClient(credentials, CognitoRegion))
                {
                    // Actualizar email
                    var updateRequest = new AdminUpdateUserAttributesRequest
                    {
                        UserPoolId = UserPoolId,
                        Username = username,
                        UserAttributes = new List<AttributeType>
                        {
                            new AttributeType { Name = "email", Value = email },
                            new AttributeType { Name = "email_verified", Value = "true" }
                        }
                    };

                    await provider.AdminUpdateUserAttributesAsync(updateRequest);

                    // Activar o desactivar usuario
                    if (cambioEstado)
                    {
                        if (activo)
                        {
                            var enableRequest = new AdminEnableUserRequest
                            {
                                UserPoolId = UserPoolId,
                                Username = username
                            };
                            await provider.AdminEnableUserAsync(enableRequest);
                        }
                        else
                        {
                            var disableRequest = new AdminDisableUserRequest
                            {
                                UserPoolId = UserPoolId,
                                Username = username
                            };
                            await provider.AdminDisableUserAsync(disableRequest);
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error Cognito: {ex.Message}");
                MostrarError("Error al actualizar el usuario en AWS Cognito.");
                return false;
            }
        }

        private async Task<bool> EliminarUsuarioDeCognitoAsync(string username)
        {
            try
            {
                string accessKey = System.Configuration.ConfigurationManager.AppSettings["AWS.AccessKey"];
                string secretKey = System.Configuration.ConfigurationManager.AppSettings["AWS.SecretKey"];
                string sessionToken = System.Configuration.ConfigurationManager.AppSettings["AWS.SessionToken"];

                var credentials = new Amazon.Runtime.SessionAWSCredentials(accessKey, secretKey, sessionToken);

                using (var provider = new AmazonCognitoIdentityProviderClient(credentials, CognitoRegion))
                {
                    var deleteRequest = new AdminDeleteUserRequest
                    {
                        UserPoolId = UserPoolId,
                        Username = username
                    };

                    await provider.AdminDeleteUserAsync(deleteRequest);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar de Cognito: {ex.Message}");
                return false;
            }
        }

        // ==================== MÉTODOS AUXILIARES ====================

        private bool UsuarioExisteEnBD(string username)
        {
            var cuentas = clienteCuenta.listarCuentasUsuario();
            return cuentas != null && cuentas.Any(c =>
                c.username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        private bool ExisteUsuarioPrincipal()
        {
            try
            {
                var usuarios = clienteUsuario.listarUsuarios();
                return usuarios != null && usuarios.Any(u =>
                    u.tipoUsuarioSpecified && u.tipoUsuario == StockifyWS.tipoUsuario.PRINCIPAL);
            }
            catch
            {
                return false;
            }
        }

        private cuentaUsuario ObtenerCuentaPorUsername(string username)
        {
            var cuentas = clienteCuenta.listarCuentasUsuario();
            return cuentas?.FirstOrDefault(c =>
                c.username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void CerrarClientes()
        {
            if (clienteCuenta != null && clienteCuenta.State == System.ServiceModel.CommunicationState.Opened)
            {
                try { clienteCuenta.Close(); }
                catch { clienteCuenta.Abort(); }
            }

            if (clienteUsuario != null && clienteUsuario.State == System.ServiceModel.CommunicationState.Opened)
            {
                try { clienteUsuario.Close(); }
                catch { clienteUsuario.Abort(); }
            }
        }

        private void MostrarError(string mensaje)
        {
            Response.Redirect($"GestionCuentas.aspx?error={Uri.EscapeDataString(mensaje)}", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}