using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using StockifyWeb.Services;
// IMPORTANTE: Asegúrate de tener esta referencia al Web Service
// Si no la tienes, agrega: Add Service Reference → http://localhost:8080/StockifyWS/UsuarioWS?wsdl
using StockifyWeb.StockifyWS;

namespace StockifyWeb
{
    public class Notificacion
    {
        public int Id { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; } // success, info, warning, error
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }
        public string Icono { get; set; }
    }

    public static class NotificationService
    {
        private const string SESSION_KEY = "Notificaciones";

        // Agregar notificación al sistema
        public static void AgregarNotificacion(string mensaje, string tipo = "success", string icono = "fa-check-circle")
        {
            var notificaciones = ObtenerNotificaciones();

            var nuevaNotificacion = new Notificacion
            {
                Id = notificaciones.Count > 0 ? notificaciones.Max(n => n.Id) + 1 : 1,
                Mensaje = mensaje,
                Tipo = tipo,
                Fecha = DateTime.Now,
                Leida = false,
                Icono = icono
            };

            notificaciones.Insert(0, nuevaNotificacion);

            // Mantener solo las últimas 50 notificaciones
            if (notificaciones.Count > 50)
            {
                notificaciones = notificaciones.Take(50).ToList();
            }

            HttpContext.Current.Session[SESSION_KEY] = notificaciones;
        }

        // Obtener todas las notificaciones
        public static List<Notificacion> ObtenerNotificaciones()
        {
            var notificaciones = HttpContext.Current.Session[SESSION_KEY] as List<Notificacion>;
            return notificaciones ?? new List<Notificacion>();
        }

        // Obtener notificaciones no leídas
        public static List<Notificacion> ObtenerNotificacionesNoLeidas()
        {
            return ObtenerNotificaciones().Where(n => !n.Leida).ToList();
        }

        // Marcar notificación como leída
        public static void MarcarComoLeida(int id)
        {
            var notificaciones = ObtenerNotificaciones();
            var notificacion = notificaciones.FirstOrDefault(n => n.Id == id);
            if (notificacion == null) return;
            notificacion.Leida = true;
            HttpContext.Current.Session[SESSION_KEY] = notificaciones;
        }

        // Marcar todas como leídas
        public static void MarcarTodasComoLeidas()
        {
            var notificaciones = ObtenerNotificaciones();
            notificaciones.ForEach(n => n.Leida = true);
            HttpContext.Current.Session[SESSION_KEY] = notificaciones;
        }

        // Contar notificaciones no leídas
        public static int ContarNoLeidas()
        {
            return ObtenerNotificacionesNoLeidas().Count;
        }

        /// <summary>
        /// Obtiene la lista de correos y teléfonos de TODOS los usuarios activos usando listarUsuarios()
        /// </summary>
        private static (List<string> emails, List<string> telefonos) ObtenerContactosTodosLosUsuarios()
        {
            var emails = new List<string>();
            var telefonos = new List<string>();

            try
            {
                // Crear cliente del Web Service
                using (var client = new StockifyWS.UsuarioWSClient())
                {
                    // Llamar al método listarUsuarios() del Web Service
                    var usuarios = client.listarUsuarios();

                    if (usuarios != null && usuarios.Length > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN] Total de usuarios en BD: {usuarios.Length}");

                        // Filtrar solo usuarios activos (TODOS LOS ROLES)
                        var usuariosActivos = usuarios.Where(u => u.activo).ToArray();

                        System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN] Usuarios activos: {usuariosActivos.Length}");

                        // Extraer emails válidos
                        emails = usuariosActivos
                            .Where(u => !string.IsNullOrWhiteSpace(u.email))
                            .Select(u => u.email.Trim())
                            .Distinct()
                            .ToList();

                        // Extraer teléfonos válidos (formato: 51XXXXXXXXX)
                        telefonos = usuariosActivos
                            .Where(u => !string.IsNullOrWhiteSpace(u.telefono))
                            .Select(u => LimpiarNumeroTelefono(u.telefono))
                            .Where(t => !string.IsNullOrEmpty(t))
                            .Distinct()
                            .ToList();

                        System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN] ✅ Encontrados {emails.Count} emails y {telefonos.Count} teléfonos de usuarios activos");

                        // Mostrar los correos encontrados
                        if (emails.Count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN] Emails: {string.Join(", ", emails)}");
                        }

                        // Mostrar los teléfonos encontrados
                        if (telefonos.Count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN] Teléfonos: {string.Join(", ", telefonos)}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[NOTIFICACIÓN] ⚠️ No se encontraron usuarios en la BD");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN ERROR] ❌ Error al obtener contactos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN ERROR] Stack Trace: {ex.StackTrace}");

                // Si falla la BD, intentar usar los del Web.config como fallback
                System.Diagnostics.Debug.WriteLine("[NOTIFICACIÓN] Usando configuración de Web.config como fallback");
                var emailsConfig = System.Configuration.ConfigurationManager.AppSettings["NotificationEmail"];
                if (!string.IsNullOrEmpty(emailsConfig))
                {
                    emails = emailsConfig.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(e => e.Trim())
                                        .ToList();
                }

                var telefonoConfig = System.Configuration.ConfigurationManager.AppSettings["WhatsAppNotificationNumber"];
                if (!string.IsNullOrEmpty(telefonoConfig))
                {
                    var tel = LimpiarNumeroTelefono(telefonoConfig);
                    if (!string.IsNullOrEmpty(tel))
                    {
                        telefonos.Add(tel);
                    }
                }
            }

            return (emails, telefonos);
        }

        /// <summary>
        /// Limpia y formatea un número de teléfono al formato requerido (51XXXXXXXXX)
        /// </summary>
        private static string LimpiarNumeroTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return null;

            // Remover espacios, guiones, paréntesis, signos +
            var numeroLimpio = new string(telefono.Where(char.IsDigit).ToArray());

            // Si empieza con 51 y tiene 11 dígitos, está bien
            if (numeroLimpio.StartsWith("51") && numeroLimpio.Length == 11)
                return numeroLimpio;

            // Si tiene 9 dígitos (número peruano sin código), agregar 51
            if (numeroLimpio.Length == 9)
                return "51" + numeroLimpio;

            // Si no cumple el formato, retornar null
            System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN] ⚠️ Teléfono con formato inválido: {telefono}");
            return null;
        }

        /// <summary>
        /// Envía correo electrónico a múltiples destinatarios
        /// </summary>
        public static bool EnviarCorreo(string asunto, string cuerpo, List<string> destinatarios = null)
        {
            try
            {
                // Si no se proporcionan destinatarios, obtener de la configuración
                if (destinatarios == null || destinatarios.Count == 0)
                {
                    var destinatariosString = System.Configuration.ConfigurationManager.AppSettings["NotificationEmail"];
                    if (!string.IsNullOrEmpty(destinatariosString))
                    {
                        destinatarios = destinatariosString
                            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(e => e.Trim())
                            .ToList();
                    }
                }

                if (destinatarios == null || destinatarios.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[EMAIL] ❌ No hay destinatarios configurados");
                    return false;
                }

                // Leer configuración desde Web.config
                var smtpServer = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                var emailFrom = System.Configuration.ConfigurationManager.AppSettings["SmtpUsername"];
                var emailPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
                var emailFromName = System.Configuration.ConfigurationManager.AppSettings["EmailFromName"] ?? "Sistema Stockify";

                // Validar configuración
                if (string.IsNullOrEmpty(emailFrom) || string.IsNullOrEmpty(emailPassword))
                {
                    System.Diagnostics.Debug.WriteLine("[EMAIL ERROR] ❌ Configuración de email incompleta en Web.config");
                    return false;
                }

                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom, emailFromName);

                    // Agregar destinatarios
                    foreach (var correo in destinatarios)
                    {
                        if (!string.IsNullOrWhiteSpace(correo))
                        {
                            mail.To.Add(correo.Trim());
                        }
                    }

                    if (mail.To.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("[EMAIL ERROR] ❌ No hay destinatarios válidos");
                        return false;
                    }

                    mail.Subject = asunto;
                    mail.Body = cuerpo;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.Normal;

                    using (var smtp = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, emailPassword);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 10000;

                        smtp.Send(mail);
                        System.Diagnostics.Debug.WriteLine($"[EMAIL] ✅ Correo enviado exitosamente a {mail.To.Count} destinatarios");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EMAIL ERROR] ❌ Error al enviar correo: {ex.Message}");
                return false;
            }
        }

        // ==================== MÉTODOS DE NOTIFICACIÓN UNIFICADOS ====================

        /// <summary>
        /// Notifica un nuevo producto por Email y WhatsApp a todos los usuarios activos
        /// </summary>
        public static void NotificarNuevoProducto(string nombreProducto, string categoria = "Sin categoría", double precio = 0)
        {
            // Agregar notificación al sistema
            var mensaje = $"Producto '{nombreProducto}' registrado exitosamente";
            AgregarNotificacion(mensaje, "success", "fa-box");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                // Obtener contactos de TODOS los usuarios activos desde la BD
                var (emails, telefonos) = ObtenerContactosTodosLosUsuarios();

                // Enviar correo electrónico a todos
                if (emails.Count > 0)
                {
                    const string asunto = "Nuevo Producto Registrado - Stockify";
                    var cuerpo = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px;'>
                                    <h2 style='color: #333;'>🆕 Nuevo Producto Registrado</h2>
                                    <p style='color: #666; font-size: 16px;'>
                                        Se ha registrado un nuevo producto en el sistema Stockify:
                                    </p>
                                    <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #8aa2ff; margin: 20px 0;'>
                                        <strong style='color: #333;'>Producto:</strong> {nombreProducto}<br>
                                        <strong style='color: #333;'>Categoría:</strong> {categoria}<br>
                                        <strong style='color: #333;'>Precio:</strong> S/ {precio:N2}
                                    </div>
                                    <p style='color: #666; font-size: 14px;'>
                                        <strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                                    </p>
                                    <hr style='border: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #999; font-size: 12px;'>
                                        Este es un mensaje automático del sistema Stockify. No responder a este correo.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    EnviarCorreo(asunto, cuerpo, emails);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[NOTIFICACIÓN] ⚠️ No se envió correo: no hay emails disponibles");
                }

                // Enviar WhatsApp a todos los teléfonos
                if (telefonos.Count > 0)
                {
                    string mensajeWhatsApp = $"🆕 *NUEVO PRODUCTO REGISTRADO*\n\n" +
                                           $"📦 Producto: *{nombreProducto}*\n" +
                                           $"📂 Categoría: {categoria}\n" +
                                           $"💰 Precio: S/ {precio:N2}\n\n" +
                                           $"✅ El producto ha sido agregado exitosamente al inventario.\n\n" +
                                           $"_Sistema Stockify_";

                    foreach (var telefono in telefonos)
                    {
                        await WhatsAppService.EnviarMensajeWhatsApp(mensajeWhatsApp, telefono);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[NOTIFICACIÓN] ⚠️ No se envió WhatsApp: no hay teléfonos disponibles");
                }
            });
        }

        /// <summary>
        /// Notifica un producto eliminado por Email y WhatsApp a todos los usuarios activos
        /// </summary>
        public static void NotificarProductoEliminado(string nombreProducto)
        {
            // Agregar notificación al sistema
            var mensaje = $"Producto '{nombreProducto}' eliminado del inventario";
            AgregarNotificacion(mensaje, "warning", "fa-trash");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                var (emails, telefonos) = ObtenerContactosTodosLosUsuarios();

                // Enviar correo electrónico
                if (emails.Count > 0)
                {
                    const string asunto = "Producto Eliminado - Stockify";
                    var cuerpo = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px;'>
                                    <h2 style='color: #dc3545;'>🗑️ Producto Eliminado</h2>
                                    <p style='color: #666; font-size: 16px;'>
                                        Se ha eliminado un producto del sistema Stockify:
                                    </p>
                                    <div style='background-color: #fff3cd; padding: 15px; border-left: 4px solid #dc3545; margin: 20px 0;'>
                                        <strong style='color: #333;'>Producto:</strong> {nombreProducto}
                                    </div>
                                    <p style='color: #666; font-size: 14px;'>
                                        <strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                                    </p>
                                    <hr style='border: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #999; font-size: 12px;'>
                                        Este es un mensaje automático del sistema Stockify. No responder a este correo.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    EnviarCorreo(asunto, cuerpo, emails);
                }

                // Enviar WhatsApp
                if (telefonos.Count > 0)
                {
                    string mensajeWhatsApp = $"🗑️ *PRODUCTO ELIMINADO*\n\n" +
                                           $"📦 Producto: *{nombreProducto}*\n\n" +
                                           $"❌ El producto ha sido eliminado del inventario.\n\n" +
                                           $"_Sistema Stockify_";

                    foreach (var telefono in telefonos)
                    {
                        await WhatsAppService.EnviarMensajeWhatsApp(mensajeWhatsApp, telefono);
                    }
                }
            });
        }

        /// <summary>
        /// Notifica un producto actualizado por Email y WhatsApp a todos los usuarios activos
        /// </summary>
        public static void NotificarProductoActualizado(string nombreProducto)
        {
            // Agregar notificación al sistema
            var mensaje = $"Producto '{nombreProducto}' actualizado correctamente";
            AgregarNotificacion(mensaje, "info", "fa-edit");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                var (emails, telefonos) = ObtenerContactosTodosLosUsuarios();

                // Enviar correo electrónico
                if (emails.Count > 0)
                {
                    const string asunto = "Producto Actualizado - Stockify";
                    var cuerpo = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px;'>
                                    <h2 style='color: #17a2b8;'>✏️ Producto Actualizado</h2>
                                    <p style='color: #666; font-size: 16px;'>
                                        Se ha actualizado la información de un producto en Stockify:
                                    </p>
                                    <div style='background-color: #d1ecf1; padding: 15px; border-left: 4px solid #17a2b8; margin: 20px 0;'>
                                        <strong style='color: #333;'>Producto:</strong> {nombreProducto}
                                    </div>
                                    <p style='color: #666; font-size: 14px;'>
                                        <strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                                    </p>
                                    <hr style='border: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #999; font-size: 12px;'>
                                        Este es un mensaje automático del sistema Stockify. No responder a este correo.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    EnviarCorreo(asunto, cuerpo, emails);
                }

                // Enviar WhatsApp
                if (telefonos.Count > 0)
                {
                    string mensajeWhatsApp = $"✏️ *PRODUCTO ACTUALIZADO*\n\n" +
                                           $"📦 Producto: *{nombreProducto}*\n\n" +
                                           $"✅ La información del producto ha sido actualizada.\n\n" +
                                           $"_Sistema Stockify_";

                    foreach (var telefono in telefonos)
                    {
                        await WhatsAppService.EnviarMensajeWhatsApp(mensajeWhatsApp, telefono);
                    }
                }
            });
        }

        /// <summary>
        /// Notifica alerta de stock bajo por Email y WhatsApp a todos los usuarios activos
        /// </summary>
        public static void NotificarStockBajo(string nombreProducto, int stockActual, int stockMinimo)
        {
            // Agregar notificación al sistema
            var mensaje = $"⚠️ Stock bajo: '{nombreProducto}' ({stockActual} unidades)";
            AgregarNotificacion(mensaje, "warning", "fa-exclamation-triangle");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                var (emails, telefonos) = ObtenerContactosTodosLosUsuarios();

                // Enviar correo electrónico
                if (emails.Count > 0)
                {
                    const string asunto = "⚠️ Alerta de Stock Bajo - Stockify";
                    var cuerpo = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px;'>
                                    <h2 style='color: #ffc107;'>⚠️ Alerta de Stock Bajo</h2>
                                    <p style='color: #666; font-size: 16px;'>
                                        Un producto ha alcanzado el nivel mínimo de stock:
                                    </p>
                                    <div style='background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0;'>
                                        <strong style='color: #333;'>Producto:</strong> {nombreProducto}<br>
                                        <strong style='color: #333;'>Stock actual:</strong> {stockActual} unidades<br>
                                        <strong style='color: #333;'>Stock mínimo:</strong> {stockMinimo} unidades
                                    </div>
                                    <p style='color: #856404; font-size: 14px; background-color: #fff3cd; padding: 10px; border-radius: 5px;'>
                                        🔔 <strong>Acción requerida:</strong> Se recomienda reabastecer este producto lo antes posible.
                                    </p>
                                    <p style='color: #666; font-size: 14px;'>
                                        <strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                                    </p>
                                    <hr style='border: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #999; font-size: 12px;'>
                                        Este es un mensaje automático del sistema Stockify. No responder a este correo.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    EnviarCorreo(asunto, cuerpo, emails);
                }

                // Enviar WhatsApp
                if (telefonos.Count > 0)
                {
                    string mensajeWhatsApp = $"⚠️ *ALERTA DE STOCK BAJO*\n\n" +
                                           $"📦 Producto: *{nombreProducto}*\n" +
                                           $"📊 Stock actual: {stockActual} unidades\n" +
                                           $"📉 Stock mínimo: {stockMinimo} unidades\n\n" +
                                           $"🔔 Se recomienda reabastecer este producto.\n\n" +
                                           $"_Sistema Stockify_";

                    foreach (var telefono in telefonos)
                    {
                        await WhatsAppService.EnviarMensajeWhatsApp(mensajeWhatsApp, telefono);
                    }
                }
            });
        }

        /// <summary>
        /// Notifica la importación de productos desde CSV por Email y WhatsApp
        /// </summary>
        public static void NotificarImportacionCSV(int cantidadProductos, List<string> nombresProductos = null)
        {
            // Agregar notificación al sistema
            var mensaje = $"Importación CSV: {cantidadProductos} producto{(cantidadProductos != 1 ? "s" : "")} agregado{(cantidadProductos != 1 ? "s" : "")} exitosamente";
            AgregarNotificacion(mensaje, "success", "fa-file-csv");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                var (emails, telefonos) = ObtenerContactosTodosLosUsuarios();

                // Preparar lista de productos para el email
                string listaProductosHtml = "";
                if (nombresProductos != null && nombresProductos.Count > 0)
                {
                    listaProductosHtml = "<ul style='color: #666; font-size: 14px; line-height: 1.8;'>";
                    foreach (var producto in nombresProductos.Take(10)) // Mostrar máximo 10
                    {
                        listaProductosHtml += $"<li>{producto}</li>";
                    }
                    if (nombresProductos.Count > 10)
                    {
                        listaProductosHtml += $"<li><em>... y {nombresProductos.Count - 10} productos más</em></li>";
                    }
                    listaProductosHtml += "</ul>";
                }

                // Enviar correo electrónico
                if (emails.Count > 0)
                {
                    const string asunto = "📊 Importación CSV Exitosa - Stockify";
                    var cuerpo = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px;'>
                                    <h2 style='color: #10b981;'>📊 Importación CSV Exitosa</h2>
                                    <p style='color: #666; font-size: 16px;'>
                                        Se ha completado exitosamente la importación de productos desde archivo CSV:
                                    </p>
                                    <div style='background-color: #d1fae5; padding: 20px; border-left: 4px solid #10b981; margin: 20px 0; border-radius: 5px;'>
                                        <p style='color: #065f46; font-size: 24px; font-weight: bold; margin: 0;'>
                                            ✅ {cantidadProductos} producto{(cantidadProductos != 1 ? "s" : "")}
                                        </p>
                                        <p style='color: #065f46; font-size: 14px; margin: 5px 0 0 0;'>
                                            importado{(cantidadProductos != 1 ? "s" : "")} correctamente
                                        </p>
                                    </div>
                                    {(string.IsNullOrEmpty(listaProductosHtml) ? "" : $@"
                                    <div style='margin: 20px 0;'>
                                        <h3 style='color: #333; font-size: 16px; margin-bottom: 10px;'>Productos importados:</h3>
                                        {listaProductosHtml}
                                    </div>
                                    ")}
                                    <p style='color: #666; font-size: 14px;'>
                                        <strong>Fecha de importación:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                                    </p>
                                    <hr style='border: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #999; font-size: 12px;'>
                                        Este es un mensaje automático del sistema Stockify. No responder a este correo.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    EnviarCorreo(asunto, cuerpo, emails);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[NOTIFICACIÓN CSV] ⚠️ No se envió correo: no hay emails disponibles");
                }

                // Enviar WhatsApp a todos los teléfonos
                if (telefonos.Count > 0)
                {
                    // Preparar lista de productos para WhatsApp (máximo 5)
                    string listaProductosWA = "";
                    if (nombresProductos != null && nombresProductos.Count > 0)
                    {
                        listaProductosWA = "\n\n📋 *Productos importados:*\n";
                        foreach (var producto in nombresProductos.Take(5))
                        {
                            listaProductosWA += $"  • {producto}\n";
                        }
                        if (nombresProductos.Count > 5)
                        {
                            listaProductosWA += $"  _... y {nombresProductos.Count - 5} productos más_\n";
                        }
                    }

                    string mensajeWhatsApp = $"📊 *IMPORTACIÓN CSV EXITOSA*\n\n" +
                                           $"✅ Se han importado *{cantidadProductos} producto{(cantidadProductos != 1 ? "s" : "")}* correctamente desde un archivo CSV." +
                                           listaProductosWA +
                                           $"\n🕐 Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n" +
                                           $"_Sistema Stockify_";

                    foreach (var telefono in telefonos)
                    {
                        await WhatsAppService.EnviarMensajeWhatsApp(mensajeWhatsApp, telefono);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[NOTIFICACIÓN CSV] ⚠️ No se envió WhatsApp: no hay teléfonos disponibles");
                }
            });
        }

        /// <summary>
        /// Método genérico para enviar notificación personalizada por Email y WhatsApp a todos los usuarios activos
        /// </summary>
        public static void EnviarNotificacion(string titulo, string mensaje, string tipo = "info", string icono = "fa-bell")
        {
            // Agregar notificación al sistema
            AgregarNotificacion(mensaje, tipo, icono);

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                var (emails, telefonos) = ObtenerContactosTodosLosUsuarios();

                // Enviar correo electrónico
                if (emails.Count > 0)
                {
                    var cuerpo = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px;'>
                                    <h2 style='color: #333;'>{titulo}</h2>
                                    <p style='color: #666; font-size: 16px;'>
                                        {mensaje}
                                    </p>
                                    <p style='color: #666; font-size: 14px;'>
                                        <strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                                    </p>
                                    <hr style='border: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #999; font-size: 12px;'>
                                        Este es un mensaje automático del sistema Stockify. No responder a este correo.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

                    EnviarCorreo(titulo + " - Stockify", cuerpo, emails);
                }

                // Enviar WhatsApp
                if (telefonos.Count > 0)
                {
                    string mensajeWhatsApp = $"*{titulo}*\n\n{mensaje}\n\n_Sistema Stockify_";

                    foreach (var telefono in telefonos)
                    {
                        await WhatsAppService.EnviarMensajeWhatsApp(mensajeWhatsApp, telefono);
                    }
                }
            });
        }
    }
}