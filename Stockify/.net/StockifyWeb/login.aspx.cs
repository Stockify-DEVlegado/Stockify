using System;
using System.Web.UI;

namespace StockifyWeb
{
    public partial class Login : System.Web.UI.Page
    {
        private const string ApiUrl = "https://my-json-server.typicode.com/OscarGAV/stockify-json-server";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();

            // Validación básica
            if (string.IsNullOrEmpty(username))
            {
                MostrarMensaje("Por favor ingrese su nombre de usuario");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MostrarMensaje("Por favor ingrese su contraseña");
                return;
            }

            // Encriptar la contraseña para compararla
            var hashedPassword = HashPassword(password);

            // Script completo con función y ejecución
            string scriptCompleto = $@"
                (function() {{
                    const API_URL = '{ApiUrl}';
                    
                    async function validarUsuarioAPI(username, passwordHash) {{
                        try {{
                            // Obtener usuarios de la API
                            const response = await fetch(API_URL + '/users');
                            const usuariosAPI = await response.json();
                            
                            // Obtener usuarios locales
                            const usuariosLocales = JSON.parse(localStorage.getItem('stockify_users_local') || '[]');
                            
                            // Combinar ambas fuentes
                            const todosUsuarios = [...usuariosAPI, ...usuariosLocales];
                            
                            // Buscar el usuario
                            const usuario = todosUsuarios.find(user => 
                                user.username.toLowerCase() === username.toLowerCase() && 
                                user.password === passwordHash
                            );
                            
                            return usuario;
                        }} catch (e) {{
                            console.error('Error al validar usuario:', e);
                            return null;
                        }}
                    }}
                    
                    // Ejecutar validación inmediatamente
                    (async function() {{
                        const usuario = await validarUsuarioAPI('{username.ToLower()}', '{hashedPassword}');
                        
                        if (usuario) {{
                            // Usuario válido - guardar en campos ocultos para postback
                            document.getElementById('{hfLoginExitoso.ClientID}').value = 'true';
                            document.getElementById('{hfUsername.ClientID}').value = usuario.username;
                            document.getElementById('{hfEmail.ClientID}').value = usuario.email || '';
                            
                            // Hacer postback
                            {Page.ClientScript.GetPostBackEventReference(this, "LoginValidado")}
                        }} else {{
                            alert('Usuario o contraseña incorrectos');
                        }}
                    }})();
                }})();
            ";

            ClientScript.RegisterStartupScript(GetType(), "ValidarLogin", scriptCompleto, true);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            // Verificar si el login fue exitoso
            if (Request.Form[hfLoginExitoso.UniqueID] != "true") return;

            string username = Request.Form[hfUsername.UniqueID];
            string email = Request.Form[hfEmail.UniqueID];

            // Guardar información en sesión
            Session["Usuario"] = username;
            Session["Email"] = email;
            Session["FechaLogin"] = DateTime.Now;

            // Verificar si el usuario marcó "Remember me"
            if (chkRemember.Checked)
            {
                // Crear cookie que dure 30 días
                Response.Cookies["StockifyUser"].Value = username;
                Response.Cookies["StockifyUser"].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                // Limpiar cookie si existe
                if (Request.Cookies["StockifyUser"] != null)
                {
                    Response.Cookies["StockifyUser"].Expires = DateTime.Now.AddDays(-1);
                }
            }

            // Redirigir a la página principal
            Response.Redirect("Inicio.aspx");
        }

        private void MostrarMensaje(string mensaje)
        {
            var script = $"alert('{mensaje}');";
            ClientScript.RegisterStartupScript(GetType(), "MensajeError", script, true);
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
    }
}