using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Login : Page
    {
        private static readonly string UserPoolId = ConfigurationManager.AppSettings["UserPoolId"];
        private static readonly string ClientId = ConfigurationManager.AppSettings["ClientId"];
        private static readonly string ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
        private static readonly RegionEndpoint CognitoRegion = RegionEndpoint.USEast1;

        private CuentaUsuarioWSClient clienteCuenta;
        private UsuarioWSClient clienteUsuario;

        public Login()
        {
            this.clienteCuenta = new CuentaUsuarioWSClient();
            this.clienteUsuario = new UsuarioWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["IdUsuario"] != null)
                {
                    Response.Redirect("Inicio.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Session.Clear();
                CargarCookieRememberMe();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(ValidarLoginAsync));
        }

        private async Task ValidarLoginAsync()
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (!ValidarCampos(username, password))
                    return;

                AuthenticationResultType authResult = await AutenticarUsuarioAsync(username, password);

                if (authResult == null)
                    return;

                cuentaUsuario cuenta = ObtenerCuentaUsuario(username);

                if (cuenta == null)
                {
                    MostrarMensaje("Error al obtener la cuenta del usuario.");
                    return;
                }

                usuario usuarioValido = ObtenerUsuarioPorCuenta(cuenta.idCuentaUsuario);

                if (usuarioValido == null)
                {
                    MostrarMensaje("Error al obtener información del usuario.");
                    return;
                }

                if (!usuarioValido.activo)
                {
                    MostrarMensaje("Su cuenta ha sido desactivada. Contacte al administrador.");
                    return;
                }

                IniciarSesion(usuarioValido, cuenta, username);
                ActualizarUltimoAcceso(cuenta);

                Response.Redirect("Inicio.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                MostrarMensaje("No se pudo conectar con el servidor. Verifique que el Web Service esté corriendo.");
            }
            catch (System.ServiceModel.CommunicationException)
            {
                MostrarMensaje("Error de comunicación con el servidor. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en login: {ex.Message}");
                MostrarMensaje("Error al iniciar sesión. Por favor intente nuevamente.");
            }
            finally
            {
                CerrarClientes();
            }
        }

        private bool ValidarCampos(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                MostrarMensaje("Por favor ingrese su nombre de usuario.");
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                MostrarMensaje("Por favor ingrese su contraseña.");
                return false;
            }

            return true;
        }

        private async Task<AuthenticationResultType> AutenticarUsuarioAsync(string username, string password)
        {
            bool requiereCambioPassword = Session["CognitoRequireNewPassword"] != null &&
                                         (bool)Session["CognitoRequireNewPassword"];
            string cognitoSession = Session["CognitoSession"] as string;
            string cognitoUsername = Session["CognitoChallengeUsername"] as string;

            if (requiereCambioPassword && !string.IsNullOrEmpty(cognitoSession))
            {
                return await CompletarCambioPasswordAsync(cognitoUsername, password, cognitoSession);
            }

            var authResponse = await IniciarAutenticacionCognitoAsync(username, password);

            if (authResponse == null)
            {
                MostrarMensaje("Usuario o contraseña incorrectos.");
                return null;
            }

            if (authResponse.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
            {
                ConfigurarCambioPassword(authResponse, username);
                MostrarMensaje("Esta es una contraseña temporal. Por favor ingrese la NUEVA contraseña que desea usar y presione Login nuevamente.");
                return null;
            }

            if (authResponse.AuthenticationResult == null)
            {
                MostrarMensaje("Usuario o contraseña incorrectos.");
                return null;
            }

            return authResponse.AuthenticationResult;
        }

        private async Task<AuthenticationResultType> CompletarCambioPasswordAsync(string username, string newPassword, string session)
        {
            try
            {
                using (var provider = new AmazonCognitoIdentityProviderClient(CognitoRegion))
                {
                    var request = new RespondToAuthChallengeRequest
                    {
                        ClientId = ClientId,
                        Session = session,
                        ChallengeResponses = new Dictionary<string, string>
                        {
                            { "USERNAME", username },
                            { "NEW_PASSWORD", newPassword },
                            { "SECRET_HASH", CalcularSecretHash(username) }
                        }
                    };

                    var response = await provider.RespondToAuthChallengeAsync(request);
                    LimpiarSesionCambioPassword();
                    return response.AuthenticationResult;
                }
            }
            catch (InvalidPasswordException ex)
            {
                MostrarMensaje($"La nueva contraseña no cumple la política de seguridad. {ex.Message}");
                return null;
            }
            catch (NotAuthorizedException ex)
            {
                MostrarMensaje($"No se pudo cambiar la contraseña. {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cambiar contraseña: {ex.Message}");
                MostrarMensaje("Error al actualizar la contraseña.");
                return null;
            }
        }

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
                        AuthParameters = new Dictionary<string, string>
                        {
                            { "USERNAME", username },
                            { "PASSWORD", password },
                            { "SECRET_HASH", CalcularSecretHash(username) }
                        }
                    };

                    return await provider.InitiateAuthAsync(request);
                }
            }
            catch (NotAuthorizedException)
            {
                return null;
            }
            catch (UserNotConfirmedException)
            {
                MostrarMensaje("Su cuenta no está confirmada.");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error Cognito: {ex.Message}");
                MostrarMensaje("Error al conectar con el servicio de autenticación.");
                return null;
            }
        }

        private cuentaUsuario ObtenerCuentaUsuario(string username)
        {
            var cuentas = clienteCuenta.listarCuentasUsuario();

            if (cuentas == null || cuentas.Length == 0)
                return null;

            foreach (var cuenta in cuentas)
            {
                if (!string.IsNullOrEmpty(cuenta.username) &&
                    cuenta.username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return cuenta;
                }
            }

            return null;
        }

        private usuario ObtenerUsuarioPorCuenta(int idCuentaUsuario)
        {
            var usuarios = clienteUsuario.listarUsuarios();

            if (usuarios == null || usuarios.Length == 0)
                return null;

            foreach (var usuario in usuarios)
            {
                if (usuario.cuenta != null && usuario.cuenta.idCuentaUsuario == idCuentaUsuario)
                {
                    return usuario;
                }
            }

            return null;
        }

        private void IniciarSesion(usuario usuarioValido, cuentaUsuario cuenta, string username)
        {
            Session["IdUsuario"] = usuarioValido.idUsuario;
            Session["IdCuentaUsuario"] = cuenta.idCuentaUsuario;
            Session["Usuario"] = cuenta.username;
            Session["Email"] = usuarioValido.email ?? "";
            Session["Nombres"] = usuarioValido.nombres ?? "";
            Session["Apellidos"] = usuarioValido.apellidos ?? "";
            Session["NombreCompleto"] = ObtenerNombreCompleto(usuarioValido, cuenta);
            Session["TipoUsuario"] = usuarioValido.tipoUsuarioSpecified
                ? usuarioValido.tipoUsuario.ToString()
                : "OPERARIO";
            Session["FechaLogin"] = DateTime.Now;

            GestionarCookieRememberMe(username);
        }

        private string ObtenerNombreCompleto(usuario usuarioValido, cuentaUsuario cuenta)
        {
            string nombreCompleto = $"{usuarioValido.nombres ?? ""} {usuarioValido.apellidos ?? ""}".Trim();
            return string.IsNullOrEmpty(nombreCompleto) ? cuenta.username : nombreCompleto;
        }

        private void ConfigurarCambioPassword(InitiateAuthResponse authResponse, string username)
        {
            Session["CognitoRequireNewPassword"] = true;
            Session["CognitoSession"] = authResponse.Session;
            Session["CognitoChallengeUsername"] = username;
        }

        private void LimpiarSesionCambioPassword()
        {
            Session["CognitoRequireNewPassword"] = null;
            Session["CognitoSession"] = null;
            Session["CognitoChallengeUsername"] = null;
        }

        private void CargarCookieRememberMe()
        {
            if (Request.Cookies["StockifyUser"] != null)
            {
                txtUsername.Text = Request.Cookies["StockifyUser"].Value;
                chkRemember.Checked = true;
            }
        }

        private void GestionarCookieRememberMe(string username)
        {
            if (chkRemember.Checked)
            {
                Response.Cookies["StockifyUser"].Value = username;
                Response.Cookies["StockifyUser"].Expires = DateTime.Now.AddDays(30);
            }
            else if (Request.Cookies["StockifyUser"] != null)
            {
                Response.Cookies["StockifyUser"].Expires = DateTime.Now.AddDays(-1);
            }
        }

        private async Task ActualizarUltimoAcceso(cuentaUsuario cuenta)
        {
            try
            {
                cuenta.ultimoAcceso = DateTime.Now;
                cuenta.ultimoAccesoSpecified = true;
                await clienteCuenta.guardarCuentaUsuarioAsync(cuenta, estado.MODIFICADO);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar último acceso: {ex.Message}");
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

        private void MostrarMensaje(string mensaje)
        {
            mensaje = mensaje.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"alert('{mensaje}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "MensajeError", script, true);
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
