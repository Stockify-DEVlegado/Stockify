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
        // === CONFIGURACIÓN COGNITO ===
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
                clienteCuenta = new CuentaUsuarioWSClient();

                string usernameInput = txtUsername.Text.Trim();
                string passwordInput = txtPassword.Text.Trim();

                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine($"🔐 INICIANDO PROCESO DE LOGIN");
                System.Diagnostics.Debug.WriteLine($"   Usuario (input): {usernameInput}");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                // Validación básica
                if (string.IsNullOrEmpty(usernameInput))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Username vacío");
                    MostrarMensaje("Por favor ingrese su nombre de usuario");
                    return;
                }

                if (string.IsNullOrEmpty(passwordInput))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Password vacío");
                    MostrarMensaje("Por favor ingrese su contraseña");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("✅ Validaciones básicas pasadas");

                // ¿Estamos en segunda fase (cambio de contraseña requerido)?
                bool requiereCambioPassword =
                    Session["CognitoRequireNewPassword"] != null &&
                    (bool)Session["CognitoRequireNewPassword"] == true;

                string cognitoSession = Session["CognitoSession"] as string;
                string cognitoUsername = Session["CognitoChallengeUsername"] as string;

                AuthenticationResultType authResult = null;

                if (requiereCambioPassword && !string.IsNullOrEmpty(cognitoSession) && !string.IsNullOrEmpty(cognitoUsername))
                {
                    // === SEGUNDO PASO: usuario ya pasó por NEW_PASSWORD_REQUIRED y ahora
                    // el passwordInput es su NUEVA contraseña definitiva ===
                    System.Diagnostics.Debug.WriteLine("🔄 Flujo NEW_PASSWORD_REQUIRED: aplicando nueva contraseña en Cognito...");

                    authResult = await CompletarNuevoPasswordCognitoAsync(cognitoUsername, passwordInput, cognitoSession);

                    if (authResult == null)
                    {
                        System.Diagnostics.Debug.WriteLine("❌ Falló el cambio de contraseña en Cognito");
                        MostrarMensaje("No se pudo actualizar la contraseña. Intente nuevamente.");
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine("✅ Contraseña actualizada en Cognito correctamente");

                    // Limpiamos las variables de challenge
                    Session["CognitoRequireNewPassword"] = null;
                    Session["CognitoSession"] = null;
                    Session["CognitoChallengeUsername"] = null;
                }
                else
                {
                    // === PRIMER PASO: login normal (puede devolver challenge NEW_PASSWORD_REQUIRED) ===
                    System.Diagnostics.Debug.WriteLine("🔐 Autenticando contra Cognito (primer intento)...");
                    var authResponse = await IniciarAutenticacionCognitoAsync(usernameInput, passwordInput);

                    if (authResponse == null)
                    {
                        System.Diagnostics.Debug.WriteLine("❌ Cognito: respuesta nula");
                        MostrarMensaje("Error al autenticar. Intente nuevamente.");
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine("👉 Cognito ChallengeName: " + authResponse.ChallengeName);

                    if (authResponse.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
                    {
                        // Guardamos info para el segundo paso
                        Session["CognitoRequireNewPassword"] = true;
                        Session["CognitoSession"] = authResponse.Session;
                        Session["CognitoChallengeUsername"] = usernameInput;

                        System.Diagnostics.Debug.WriteLine("⚠️ Cognito requiere cambio de contraseña (NEW_PASSWORD_REQUIRED)");

                        // Avisamos al usuario que ahora debe ingresar la nueva contraseña
                        MostrarMensaje("Esta es una contraseña temporal. Por favor ingrese la NUEVA contraseña que desea usar y presione Login nuevamente.");
                        return;
                    }

                    // Si no hay challenge y tenemos AuthenticationResult, login OK
                    if (authResponse.AuthenticationResult == null)
                    {
                        System.Diagnostics.Debug.WriteLine("❌ Cognito: AuthenticationResult nulo sin challenge");
                        MostrarMensaje("Usuario o contraseña incorrectos.");
                        return;
                    }

                    authResult = authResponse.AuthenticationResult;
                    System.Diagnostics.Debug.WriteLine("✅ Credenciales válidas en Cognito (sin cambio de contraseña)");
                }

                // Si hemos llegado aquí, ya sea:
                // - Login normal OK (sin challenge)
                // - O cambio de contraseña completado y login aceptado.
                // authResult contiene los tokens (IdToken, AccessToken, etc.) si los necesitas.

                // 2) OBTENER CUENTA DE USUARIO EN TU BD (como antes)
                System.Diagnostics.Debug.WriteLine("📡 Conectando con CuentaUsuarioWS...");
                try
                {
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
                System.Diagnostics.Debug.WriteLine("   Llamando a listarCuentasUsuario()...");

                var cuentasResponse = clienteCuenta.listarCuentasUsuario();
                System.Diagnostics.Debug.WriteLine("   Respuesta recibida del WS");

                var cuentas = cuentasResponse;
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

                // Buscar la cuenta por username (según lo que el usuario escribió)
                cuentaUsuario cuentaEncontrada = null;
                System.Diagnostics.Debug.WriteLine($"🔍 Buscando cuenta con username: '{usernameInput}'");

                foreach (var c in cuentas)
                {
                    if (!string.IsNullOrEmpty(c.username) &&
                        c.username.Equals(usernameInput, StringComparison.OrdinalIgnoreCase))
                    {
                        cuentaEncontrada = c;
                        System.Diagnostics.Debug.WriteLine($"✅ Cuenta encontrada - ID: {c.idCuentaUsuario}");
                        break;
                    }
                }

                if (cuentaEncontrada == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Cuenta no encontrada en la BD para ese username");
                    MostrarMensaje("Error al obtener la cuenta del usuario en el sistema.");
                    return;
                }

                // 3) OBTENER DATOS COMPLETOS DEL USUARIO (UsuarioWS) COMO ANTES
                System.Diagnostics.Debug.WriteLine("📡 Conectando con UsuarioWS para obtener datos completos...");
                clienteUsuario = new UsuarioWSClient();

                System.Diagnostics.Debug.WriteLine("📥 Obteniendo lista de usuarios...");
                var usuariosResponse = clienteUsuario.listarUsuarios();
                var usuarios = usuariosResponse;

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
                        Session["TipoUsuario"] = usuarioValido.tipoUsuario;
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
                    System.Diagnostics.Debug.WriteLine($"⚠️ Usuario inactivo: {usernameInput}");
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
                    Response.Cookies["StockifyUser"].Value = usernameInput;
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

                // Actualizar último acceso 
                try
                {
                    System.Diagnostics.Debug.WriteLine("Actualizando último acceso...");
                    cuentaEncontrada.ultimoAcceso = DateTime.Now;
                    cuentaEncontrada.ultimoAccesoSpecified = true;
                    await clienteCuenta.guardarCuentaUsuarioAsync(cuentaEncontrada, estado.MODIFICADO);
                    System.Diagnostics.Debug.WriteLine("Último acceso actualizado");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($" No se pudo actualizar último acceso: {ex.Message}");
                }

                // Redirigir a la página principal
                System.Diagnostics.Debug.WriteLine(" Redirigiendo a Inicio.aspx...");
                Response.Redirect("Inicio.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine(" ERROR: No se pudo conectar con el Web Service");
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
                System.Diagnostics.Debug.WriteLine(" ERROR DE COMUNICACIÓN");
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
                System.Diagnostics.Debug.WriteLine(" ERROR INESPERADO");
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

        // === HELPERS COGNITO ===

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

        /// <summary>
        /// Primer paso: intenta autenticarse. Puede devolver:
        /// - AuthenticationResult (login OK)
        /// - Challenge NEW_PASSWORD_REQUIRED (requiere cambiar contraseña)
        /// </summary>
        private async Task<InitiateAuthResponse> IniciarAutenticacionCognitoAsync(string username, string password)
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

                    var secretHash = CalcularSecretHash(username);
                    request.AuthParameters["SECRET_HASH"] = secretHash;

                    var response = await provider.InitiateAuthAsync(request);
                    return response;
                }
            }
            catch (NotAuthorizedException ex)
            {
                System.Diagnostics.Debug.WriteLine("Cognito NotAuthorizedException (InitiateAuth): " + ex.Message);
                return null;
            }
            catch (UserNotConfirmedException ex)
            {
                System.Diagnostics.Debug.WriteLine("Cognito UserNotConfirmedException (InitiateAuth): " + ex.Message);
                MostrarMensaje("Su cuenta no está confirmada en Cognito.");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error Cognito (InitiateAuth): " + ex.Message);
                MostrarMensaje("Error al conectar con el servicio de autenticación. Intente nuevamente.");
                return null;
            }
        }

        /// <summary>
        /// Segundo paso para usuarios en estado FORCE_CHANGE_PASSWORD / NEW_PASSWORD_REQUIRED:
        /// Envía la nueva contraseña definitiva con RespondToAuthChallenge.
        /// </summary>
        private async Task<AuthenticationResultType> CompletarNuevoPasswordCognitoAsync(string username, string newPassword, string session)
        {
            try
            {
                using (var provider = new AmazonCognitoIdentityProviderClient(CognitoRegion))
                {
                    var secretHash = CalcularSecretHash(username);

                    var request = new RespondToAuthChallengeRequest
                    {
                        ClientId = ClientId,
                        ChallengeName = ChallengeNameType.NEW_PASSWORD_REQUIRED,
                        Session = session,
                        ChallengeResponses = new Dictionary<string, string>
                {
                    { "USERNAME", username },
                    { "NEW_PASSWORD", newPassword },
                    { "SECRET_HASH", secretHash }
                }
                    };

                    var response = await provider.RespondToAuthChallengeAsync(request);
                    return response.AuthenticationResult;
                }
            }
            catch (InvalidPasswordException ex)
            {
                // Contraseña no cumple la política del User Pool
                System.Diagnostics.Debug.WriteLine("Cognito InvalidPasswordException: " + ex.Message);
                MostrarMensaje("La nueva contraseña no cumple la política de seguridad del sistema. Detalle: " + ex.Message);
                return null;
            }
            catch (NotAuthorizedException ex)
            {
                // Por ejemplo, contraseña nueva igual a la temporal
                System.Diagnostics.Debug.WriteLine("Cognito NotAuthorizedException (RespondToAuthChallenge): " + ex.Message);
                MostrarMensaje("No se pudo cambiar la contraseña. Detalle: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Error Cognito (RespondToAuthChallenge): " + ex.Message);
                MostrarMensaje("Error al actualizar la contraseña en Cognito: " + ex.Message);
                return null;
            }
        }
    }
}