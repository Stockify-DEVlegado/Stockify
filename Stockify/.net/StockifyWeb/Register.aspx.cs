using System;
using System.Web.UI;

namespace StockifyWeb
{
    public partial class Register : System.Web.UI.Page
    {
        private const string ApiUrl = "https://my-json-server.typicode.com/OscarGAV/stockify-json-server";

        protected void Page_Load(object sender, EventArgs e)
        {
            // No necesitamos registrar scripts en Page_Load
            // Se registrarán cuando sea necesario en btnRegister_Click
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Verificar que la página sea válida (validadores)
            if (!Page.IsValid)
            {
                return;
            }

            // Obtener los valores de los campos
            var email = txtEmail.Text.Trim();
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();
            var confirmPassword = txtConfirmPassword.Text.Trim();

            // Validación adicional
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MostrarMensaje("Todos los campos son obligatorios");
                return;
            }

            if (password != confirmPassword)
            {
                MostrarMensaje("Las contraseñas no coinciden");
                return;
            }

            // Validar longitud mínima de contraseña
            if (password.Length < 6)
            {
                MostrarMensaje("La contraseña debe tener al menos 6 caracteres");
                return;
            }

            // Encriptar la contraseña
            var hashedPassword = HashPassword(password);

            // Script completo con todas las funciones y ejecución
            var scriptCompleto = $@"
                (function() {{
                    const API_URL = '{ApiUrl}';
                    
                    async function verificarUsuarioExiste(username, email) {{
                        try {{
                            // Verificar en la API
                            const response = await fetch(API_URL + '/users');
                            const usuariosAPI = await response.json();
                            
                            // Verificar en localStorage
                            const usuariosLocales = JSON.parse(localStorage.getItem('stockify_users_local') || '[]');
                            
                            // Combinar ambas fuentes
                            const todosUsuarios = [...usuariosAPI, ...usuariosLocales];
                            
                            return todosUsuarios.some(user => 
                                user.username.toLowerCase() === username.toLowerCase() || 
                                user.email.toLowerCase() === email.toLowerCase()
                            );
                        }} catch (e) {{
                            console.error('Error al verificar usuario:', e);
                            return false;
                        }}
                    }}
                    
                    async function registrarUsuario(email, username, password) {{
                        try {{
                            // Intentar POST a la API (aunque sea solo lectura)
                            const response = await fetch(API_URL + '/users', {{
                                method: 'POST',
                                headers: {{
                                    'Content-Type': 'application/json',
                                }},
                                body: JSON.stringify({{
                                    email: email,
                                    username: username,
                                    password: password,
                                    fechaRegistro: new Date().toISOString()
                                }})
                            }});
                            
                            // Guardar en localStorage para persistencia local
                            let usuariosLocales = JSON.parse(localStorage.getItem('stockify_users_local') || '[]');
                            usuariosLocales.push({{
                                email: email,
                                username: username,
                                password: password,
                                fechaRegistro: new Date().toISOString()
                            }});
                            localStorage.setItem('stockify_users_local', JSON.stringify(usuariosLocales));
                            
                            return true;
                        }} catch (e) {{
                            console.error('Error al registrar usuario:', e);
                            return false;
                        }}
                    }}
                    
                    // Ejecutar registro inmediatamente
                    (async function() {{
                        const existe = await verificarUsuarioExiste('{username}', '{email}');
                        
                        if (existe) {{
                            alert('El usuario o email ya está registrado');
                        }} else {{
                            const success = await registrarUsuario('{email}', '{username}', '{hashedPassword}');
                            
                            if (success) {{
                                alert('¡Registro exitoso! Redirigiendo al login...');
                                setTimeout(function() {{ window.location.href='Login.aspx'; }}, 2000);
                            }} else {{
                                alert('Error al registrar el usuario. Intente nuevamente');
                            }}
                        }}
                    }})();
                }})();
            ";

            ClientScript.RegisterStartupScript(GetType(), "RegistrarUsuario", scriptCompleto, true);
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