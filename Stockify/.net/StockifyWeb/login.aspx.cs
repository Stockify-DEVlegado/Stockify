using StockifyWeb.StockifyWS;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;

namespace StockifyWeb
{
    public partial class Login : Page
    {
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

                // Crear cliente del Web Service de CuentaUsuario
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

                // Primero, obtener la cuenta de usuario por username
                System.Diagnostics.Debug.WriteLine("🔍 Buscando cuenta de usuario...");
                System.Diagnostics.Debug.WriteLine("   Llamando a listarCuentasUsuarioAsync()...");

                var cuentasResponse = await clienteCuenta.listarCuentasUsuarioAsync();
                System.Diagnostics.Debug.WriteLine("   Respuesta recibida del WS");

                var cuentas = cuentasResponse.@return;
                System.Diagnostics.Debug.WriteLine($"   Cuentas es null: {cuentas == null}");

                if (cuentas == null || cuentas.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No se encontraron cuentas en la BD");
                    System.Diagnostics.Debug.WriteLine($"   cuentas == null: {cuentas == null}");
                    System.Diagnostics.Debug.WriteLine($"   cuentas.Length: {(cuentas != null ? cuentas.Length.ToString() : "N/A")}");
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
                        System.Diagnostics.Debug.WriteLine($"   Username: '{c.username}'");
                        string fullPwd = string.IsNullOrEmpty(c.password) ? "VACÍO" : c.password;
                        System.Diagnostics.Debug.WriteLine($"   Password (completo): {fullPwd}");
                        System.Diagnostics.Debug.WriteLine($"   Longitud password: {(string.IsNullOrEmpty(c.password) ? 0 : c.password.Length)}");
                        break;
                    }
                }

                if (cuentaEncontrada == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Cuenta no encontrada");
                    System.Diagnostics.Debug.WriteLine($"   Se buscó: '{username}'");
                    System.Diagnostics.Debug.WriteLine("   Cuentas disponibles:");
                    foreach (var c in cuentas)
                    {
                        System.Diagnostics.Debug.WriteLine($"      - {c.username}");
                    }
                    MostrarMensaje("Usuario o contraseña incorrectos");
                    return;
                }

                // Verificar si la contraseña en BD está hasheada o en texto plano
                bool passwordEsHash = !string.IsNullOrEmpty(cuentaEncontrada.password) &&
                                     cuentaEncontrada.password.Length == 64;

                System.Diagnostics.Debug.WriteLine($"🔍 Tipo de password en BD: {(passwordEsHash ? "HASH SHA256" : "TEXTO PLANO")}");

                string hashedPassword = HashPassword(password);
                System.Diagnostics.Debug.WriteLine($"🔑 Password ingresado (texto): {password}");
                System.Diagnostics.Debug.WriteLine($"🔑 Password hasheado (calc): {hashedPassword}");
                System.Diagnostics.Debug.WriteLine($"🔑 Password en BD:           {cuentaEncontrada.password}");

                bool loginExitoso = false;

                if (passwordEsHash)
                {
                    // La contraseña en BD está hasheada - comparar con hash
                    System.Diagnostics.Debug.WriteLine("🔐 Modo: Comparación con HASH");
                    if (hashedPassword.Equals(cuentaEncontrada.password, StringComparison.OrdinalIgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine("✅ Validación LOCAL exitosa - hashes coinciden");
                        loginExitoso = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("⚠️ Validación LOCAL falló - probando con WS...");
                        try
                        {
                            var loginResponse = await clienteCuenta.loginAsync(username, hashedPassword);
                            loginExitoso = loginResponse.@return;
                            System.Diagnostics.Debug.WriteLine($"📊 Resultado del WS.login(): {loginExitoso}");
                        }
                        catch (Exception wsEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ Error al llamar WS.login(): {wsEx.Message}");
                        }
                    }
                }
                else
                {
                    // La contraseña en BD está en texto plano - comparar directamente
                    System.Diagnostics.Debug.WriteLine("🔓 Modo: Comparación TEXTO PLANO (INSEGURO - Solo para desarrollo)");
                    System.Diagnostics.Debug.WriteLine($"   Comparando: '{password}' == '{cuentaEncontrada.password}'");

                    if (password.Equals(cuentaEncontrada.password, StringComparison.Ordinal))
                    {
                        System.Diagnostics.Debug.WriteLine("✅ Validación LOCAL exitosa - passwords en texto plano coinciden");
                        loginExitoso = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("⚠️ Validación LOCAL falló - probando con WS...");
                        try
                        {
                            // Intentar con texto plano
                            var loginResponse = await clienteCuenta.loginAsync(username, password);
                            if (loginResponse.@return)
                            {
                                loginExitoso = true;
                                System.Diagnostics.Debug.WriteLine($"📊 WS.login() con texto plano: exitoso");
                            }
                            else
                            {
                                // Intentar con hash
                                loginResponse = await clienteCuenta.loginAsync(username, hashedPassword);
                                loginExitoso = loginResponse.@return;
                                System.Diagnostics.Debug.WriteLine($"📊 WS.login() con hash: {loginExitoso}");
                            }
                        }
                        catch (Exception wsEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ Error al llamar WS.login(): {wsEx.Message}");
                        }
                    }
                }

                if (!loginExitoso)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Login fallido - Credenciales incorrectas");
                    System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                    System.Diagnostics.Debug.WriteLine("DIAGNÓSTICO:");
                    System.Diagnostics.Debug.WriteLine($"  Username buscado: '{username}'");
                    System.Diagnostics.Debug.WriteLine($"  Password ingresado: '{password}'");
                    System.Diagnostics.Debug.WriteLine($"  Password en BD: '{cuentaEncontrada.password}'");
                    System.Diagnostics.Debug.WriteLine($"  Tipo en BD: {(passwordEsHash ? "HASH" : "TEXTO PLANO")}");
                    System.Diagnostics.Debug.WriteLine($"  Hash calculado: {hashedPassword}");
                    System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                    MostrarMensaje("Usuario o contraseña incorrectos");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("✅ Credenciales válidas");

                // Si el login es exitoso, obtener los datos completos del usuario
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

                // Login exitoso - Guardar información en sesión
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

                // Verificar si el usuario marcó "Remember me"
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

                // Actualizar último acceso (opcional - no crítico si falla)
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
                    // No detener el login por esto
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
                    try
                    {
                        clienteCuenta.Close();
                    }
                    catch
                    {
                        clienteCuenta.Abort();
                    }
                }

                if (clienteUsuario != null && clienteUsuario.State == System.ServiceModel.CommunicationState.Opened)
                {
                    try
                    {
                        clienteUsuario.Close();
                    }
                    catch
                    {
                        clienteUsuario.Abort();
                    }
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
    }
}