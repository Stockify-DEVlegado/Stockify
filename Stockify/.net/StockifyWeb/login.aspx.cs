using StockifyWeb.StockifyWS;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StockifyWeb
{
    public partial class Login : Page
    {
        private const string UserPoolId = "us-east-1_LIZsvOxNv";
        private const string ClientId = "5f0hvfclu5ichnmd8r1vjs3rpl";
        private const string ClientSecret = "1sbcm6efocmo314c8re3dqkg6pj2fhi984vfc95vcd431q0s5a6k";
        private static readonly RegionEndpoint CognitoRegion = RegionEndpoint.USEast1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar si ya hay una sesión activa
                if (Session["IdUsuario"] != null)
                {
                    Response.Redirect("Inicio.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Limpiar cualquier sesión anterior
                Session.Clear();

                // Verificar si hay una cookie de "Remember me"
                if (Request.Cookies["StockifyUser"] != null)
                {
                    txtUsername.Text = Request.Cookies["StockifyUser"].Value;
                    chkRemember.Checked = true;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Ejecutar el login de forma asíncrona
            RegisterAsyncTask(new PageAsyncTask(ValidarLoginAsync));
        }

        private async Task ValidarLoginAsync()
        {
            CuentaUsuarioWSClient clienteCuenta = null;
            UsuarioWSClient clienteUsuario = null;

            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine($"🔐 INICIANDO PROCESO DE LOGIN");
                System.Diagnostics.Debug.WriteLine($"   Usuario: {username}");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                // Validación básica
                if (string.IsNullOrEmpty(username))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Username vacío");
                    MostrarMensaje("Por favor ingrese su nombre de usuario");
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Password vacío");
                    MostrarMensaje("Por favor ingrese su contraseña");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("✅ Validaciones básicas pasadas");

                // 1) AUTENTICACIÓN CON COGNITO
                System.Diagnostics.Debug.WriteLine("🔐 Autenticando contra Cognito...");
                bool loginExitoso = await AutenticarConCognito(username, password);

                if (!loginExitoso)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Cognito: usuario o contraseña incorrectos");
                    System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                    MostrarMensaje("Usuario o contraseña incorrectos");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("✅ Credenciales válidas en Cognito");

                // 2) OBTENER CUENTA DE USUARIO EN TU BD (como antes)
                System.Diagnostics.Debug.WriteLine("📡 Conectando con CuentaUsuarioWS...");
                try
                {
                    clienteCuenta = new CuentaUsuarioWSClient();
                    System.Diagnostics.Debug.WriteLine($"   Estado del cliente: {clienteCuenta.State}");
                    System.Diagnostics.Debug.WriteLine($"   Endpoint Address: {clienteCuenta.Endpoint.Address.Uri}");
                    System.Diagnostics.Debug.WriteLine($"   Binding: {clienteCuenta.Endpoint.Binding.Name}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error al crear cliente WS: {ex.Message}");
                    MostrarMensaje("Error al crear conexión con el Web Service. Verifique la configuración en Web.config");
                    return;
                }

                // Obtener todas las cuentas
                System.Diagnostics.Debug.WriteLine("🔍 Buscando cuenta de usuario...");
                System.Diagnostics.Debug.WriteLine("   Llamando a listarCuentasUsuarioAsync()...");

                var cuentasResponse = await clienteCuenta.listarCuentasUsuarioAsync();
                System.Diagnostics.Debug.WriteLine("   Respuesta recibida del WS");

                var cuentas = cuentasResponse.@return;
                System.Diagnostics.Debug.WriteLine($"   Cuentas es null: {cuentas == null}");

                if (cuentas == null || cuentas.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No se encontraron cuentas en la BD");
                    MostrarMensaje("Error al conectar con el sistema. No hay cuentas registradas.");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"📊 Total cuentas encontradas: {cuentas.Length}");
                System.Diagnostics.Debug.WriteLine("📋 Listado de cuentas disponibles:");
                foreach (var c in cuentas)
                {
                    string pwdPreview = string.IsNullOrEmpty(c.password) ? "VACÍO" :
                        (c.password.Length > 16 ? c.password.Substring(0, 16) + "..." : c.password);
                    System.Diagnostics.Debug.WriteLine($"   - ID: {c.idCuentaUsuario}, Username: '{c.username}', Pwd: {pwdPreview}");
                }

                // Buscar la cuenta por username
                cuentaUsuario cuentaEncontrada = null;
                System.Diagnostics.Debug.WriteLine($"🔍 Buscando cuenta con username: '{username}'");

                foreach (var c in cuentas)
                {
                    if (!string.IsNullOrEmpty(c.username) &&
                        c.username.Equals(username, StringComparison.OrdinalIgnoreCase))
                    {
                        cuentaEncontrada = c;
                        System.Diagnostics.Debug.WriteLine($"✅ Cuenta encontrada - ID: {c.idCuentaUsuario}");
                        break;
                    }
                }

                if (cuentaEncontrada == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Cuenta no encontrada en la BD");
                    MostrarMensaje("Usuario o contraseña incorrectos");
                    return;
                }

                // 3) OBTENER DATOS COMPLETOS DEL USUARIO (UsuarioWS) COMO ANTES
                System.Diagnostics.Debug.WriteLine("📡 Conectando con UsuarioWS para obtener datos completos...");
                clienteUsuario = new UsuarioWSClient();

                System.Diagnostics.Debug.WriteLine("📥 Obteniendo lista de usuarios...");
                var usuariosResponse = await clienteUsuario.listarUsuariosAsync();
                var usuarios = usuariosResponse.@return;

                if (usuarios == null || usuarios.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No se encontraron usuarios en la BD");
                    MostrarMensaje("Error al obtener información del usuario");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"📊 Total usuarios encontrados: {usuarios.Length}");

                // Buscar el usuario que tiene esta cuenta
                usuario usuarioValido = null;
                foreach (var u in usuarios)
                {
                    if (u.cuenta != null &&
                        u.cuenta.idCuentaUsuario == cuentaEncontrada.idCuentaUsuario)
                    {
                        usuarioValido = u;
                        System.Diagnostics.Debug.WriteLine($"✅ Usuario encontrado - ID: {u.idUsuario}");
                        break;
                    }
                }

                if (usuarioValido == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Usuario no encontrado");
                    MostrarMensaje("Error al obtener información del usuario");
                    return;
                }

                // Verificar que el usuario esté activo
                System.Diagnostics.Debug.WriteLine($"🔍 Verificando estado activo: {usuarioValido.activo}");

                if (!usuarioValido.activo)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Usuario inactivo: {username}");
                    System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                    MostrarMensaje("Su cuenta ha sido desactivada. Contacte al administrador.");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("✅ Usuario activo");

                // 4) LOGIN EXITOSO: GUARDAR EN SESIÓN (igual que antes)
                System.Diagnostics.Debug.WriteLine("💾 Guardando información en sesión...");

                Session["IdUsuario"] = usuarioValido.idUsuario;
                Session["IdCuentaUsuario"] = cuentaEncontrada.idCuentaUsuario;
                Session["Usuario"] = cuentaEncontrada.username;
                Session["Email"] = usuarioValido.email ?? "";
                Session["Nombres"] = usuarioValido.nombres ?? "";
                Session["Apellidos"] = usuarioValido.apellidos ?? "";

                string nombreCompleto = $"{usuarioValido.nombres ?? ""} {usuarioValido.apellidos ?? ""}".Trim();
                if (string.IsNullOrEmpty(nombreCompleto))
                {
                    nombreCompleto = cuentaEncontrada.username;
                }
                Session["NombreCompleto"] = nombreCompleto;

                Session["TipoUsuario"] = usuarioValido.tipoUsuarioSpecified
                    ? usuarioValido.tipoUsuario.ToString()
                    : "OPERARIO";
                Session["FechaLogin"] = DateTime.Now;

                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("✅ LOGIN EXITOSO");
                System.Diagnostics.Debug.WriteLine($"   ID Usuario: {usuarioValido.idUsuario}");
                System.Diagnostics.Debug.WriteLine($"   ID Cuenta: {cuentaEncontrada.idCuentaUsuario}");
                System.Diagnostics.Debug.WriteLine($"   Username: {cuentaEncontrada.username}");
                System.Diagnostics.Debug.WriteLine($"   Email: {usuarioValido.email}");
                System.Diagnostics.Debug.WriteLine($"   Nombre Completo: {nombreCompleto}");
                System.Diagnostics.Debug.WriteLine($"   Tipo Usuario: {Session["TipoUsuario"]}");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                // Remember me
                if (chkRemember.Checked)
                {
                    Response.Cookies["StockifyUser"].Value = username;
                    Response.Cookies["StockifyUser"].Expires = DateTime.Now.AddDays(30);
                    System.Diagnostics.Debug.WriteLine("🍪 Cookie 'Remember me' creada (30 días)");
                }
                else
                {
                    if (Request.Cookies["StockifyUser"] != null)
                    {
                        Response.Cookies["StockifyUser"].Expires = DateTime.Now.AddDays(-1);
                        System.Diagnostics.Debug.WriteLine("🍪 Cookie 'Remember me' eliminada");
                    }
                }

                // Actualizar último acceso (opcional)
                try
                {
                    System.Diagnostics.Debug.WriteLine("📅 Actualizando último acceso...");
                    cuentaEncontrada.ultimoAcceso = DateTime.Now;
                    cuentaEncontrada.ultimoAccesoSpecified = true;
                    await clienteCuenta.guardarCuentaUsuarioAsync(cuentaEncontrada, estado.MODIFICADO);
                    System.Diagnostics.Debug.WriteLine("✅ Último acceso actualizado");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ No se pudo actualizar último acceso: {ex.Message}");
                }

                // Redirigir a la página principal
                System.Diagnostics.Debug.WriteLine("🔄 Redirigiendo a Inicio.aspx...");
                Response.Redirect("Inicio.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("❌ ERROR: No se pudo conectar con el Web Service");
                System.Diagnostics.Debug.WriteLine($"   Mensaje: {ex.Message}");

                if (clienteCuenta != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   URL intentada: {clienteCuenta.Endpoint.Address.Uri}");
                }

                System.Diagnostics.Debug.WriteLine("   SOLUCIÓN:");
                System.Diagnostics.Debug.WriteLine("   1. Verifique que el Web Service Java esté corriendo");
                System.Diagnostics.Debug.WriteLine("   2. Verifique la URL en Web.config");
                System.Diagnostics.Debug.WriteLine("   3. Pruebe acceder a: http://localhost:8080/StockifyWS/CuentaUsuarioWS?wsdl");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                MostrarMensaje("No se pudo conectar con el servidor. Verifique que el Web Service esté corriendo en: " +
                    (clienteCuenta != null ? clienteCuenta.Endpoint.Address.Uri.ToString() : "puerto desconocido"));
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("❌ ERROR DE COMUNICACIÓN");
                System.Diagnostics.Debug.WriteLine($"   Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Tipo: {ex.GetType().Name}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
                }

                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                MostrarMensaje("Error de comunicación con el servidor. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("❌ ERROR INESPERADO");
                System.Diagnostics.Debug.WriteLine($"   Tipo: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"   Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
                }

                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                MostrarMensaje("Error al iniciar sesión. Por favor intente nuevamente.");
            }
            finally
            {
                // Cerrar clientes si están abiertos
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
        }

        private void MostrarMensaje(string mensaje)
        {
            mensaje = mensaje.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"alert('{mensaje}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "MensajeError", script, true);
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var builder = new System.Text.StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private async Task<bool> AutenticarConCognito(string username, string password)
        {
            try
            {
                using (var provider = new AmazonCognitoIdentityProviderClient(CognitoRegion))
                {
                    var request = new InitiateAuthRequest
                    {
                        ClientId = ClientId,
                        AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                        AuthParameters = new Dictionary<string, string>()
                    };

                    request.AuthParameters["USERNAME"] = username;
                    request.AuthParameters["PASSWORD"] = password;

                    // 🔐 Como el cliente tiene secreto, hay que enviar SECRET_HASH
                    var secretHash = CalcularSecretHash(username);
                    request.AuthParameters["SECRET_HASH"] = secretHash;

                    var response = await provider.InitiateAuthAsync(request);

                    System.Diagnostics.Debug.WriteLine("Cognito ChallengeName: " + response.ChallengeName);

                    return response.AuthenticationResult != null;
                }
            }
            catch (NotAuthorizedException ex)
            {
                System.Diagnostics.Debug.WriteLine("Cognito NotAuthorizedException: " + ex.Message);
                // Usuario/contraseña incorrectos o hash inválido
                return false;
            }
            catch (UserNotConfirmedException ex)
            {
                System.Diagnostics.Debug.WriteLine("Cognito UserNotConfirmedException: " + ex.Message);
                MostrarMensaje("Su cuenta no está confirmada en Cognito.");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Error Cognito: " + ex.Message);
                MostrarMensaje("Error al conectar con el servicio de autenticación. Intente nuevamente.");
                return false;
            }
        }

        private static string CalcularSecretHash(string username)
        {
            var key = Encoding.UTF8.GetBytes(ClientSecret);
            var message = Encoding.UTF8.GetBytes(username + ClientId);

            using (var hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(message);
                return Convert.ToBase64String(hash);
            }
        }



    }
}