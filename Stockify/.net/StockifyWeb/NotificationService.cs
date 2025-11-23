using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using StockifyWeb.Services;

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
        private const string EMAIL_DESTINATARIO = "a20220461@pucp.edu.pe";

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

        // Enviar correo electrónico
        public static bool EnviarCorreo(string asunto, string cuerpo)
        {
            try
            {
                // Leer configuración desde Web.config
                var smtpServer = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                var emailFrom = System.Configuration.ConfigurationManager.AppSettings["SmtpUsername"];
                var emailPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
                var emailFromName = System.Configuration.ConfigurationManager.AppSettings["EmailFromName"] ?? "Sistema Stockify";
                var destinatariosString = System.Configuration.ConfigurationManager.AppSettings["NotificationEmail"] ?? EMAIL_DESTINATARIO;

                // Validar configuración
                if (string.IsNullOrEmpty(emailFrom) || string.IsNullOrEmpty(emailPassword))
                {
                    System.Diagnostics.Debug.WriteLine("Error: Configuración de email incompleta en Web.config");
                    return false;
                }

                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom, emailFromName);

                    // Múltiples destinatarios
                    if (!string.IsNullOrEmpty(destinatariosString))
                    {
                        var listaCorreos = destinatariosString.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var correo in listaCorreos)
                        {
                            mail.To.Add(correo.Trim());
                        }
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
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }

        // ==================== MÉTODOS DE NOTIFICACIÓN UNIFICADOS ====================

        /// <summary>
        /// Notifica un nuevo producto por Email y WhatsApp
        /// </summary>
        public static void NotificarNuevoProducto(string nombreProducto, string categoria = "Sin categoría", double precio = 0)
        {
            // Agregar notificación al sistema
            var mensaje = $"Producto '{nombreProducto}' registrado exitosamente";
            AgregarNotificacion(mensaje, "success", "fa-box");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                // Enviar correo electrónico
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

                EnviarCorreo(asunto, cuerpo);

                // Enviar WhatsApp
                await WhatsAppService.EnviarNotificacionNuevoProducto(nombreProducto, categoria, precio);
            });
        }

        /// <summary>
        /// Notifica un producto eliminado por Email y WhatsApp
        /// </summary>
        public static void NotificarProductoEliminado(string nombreProducto)
        {
            // Agregar notificación al sistema
            var mensaje = $"Producto '{nombreProducto}' eliminado del inventario";
            AgregarNotificacion(mensaje, "warning", "fa-trash");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                // Enviar correo electrónico
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

                EnviarCorreo(asunto, cuerpo);

                // Enviar WhatsApp
                await WhatsAppService.EnviarNotificacionProductoEliminado(nombreProducto);
            });
        }

        /// <summary>
        /// Notifica un producto actualizado por Email y WhatsApp
        /// </summary>
        public static void NotificarProductoActualizado(string nombreProducto)
        {
            // Agregar notificación al sistema
            var mensaje = $"Producto '{nombreProducto}' actualizado correctamente";
            AgregarNotificacion(mensaje, "info", "fa-edit");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                // Enviar correo electrónico
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

                EnviarCorreo(asunto, cuerpo);

                // Enviar WhatsApp
                await WhatsAppService.EnviarNotificacionProductoActualizado(nombreProducto);
            });
        }

        /// <summary>
        /// Notifica alerta de stock bajo por Email y WhatsApp
        /// NOTA: Esta función está lista para usarse cuando se implemente la detección de stock bajo
        /// </summary>
        public static void NotificarStockBajo(string nombreProducto, int stockActual, int stockMinimo)
        {
            // Agregar notificación al sistema
            var mensaje = $"⚠️ Stock bajo: '{nombreProducto}' ({stockActual} unidades)";
            AgregarNotificacion(mensaje, "warning", "fa-exclamation-triangle");

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                // Enviar correo electrónico
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

                EnviarCorreo(asunto, cuerpo);

                // Enviar WhatsApp
                await WhatsAppService.EnviarAlertaStockBajo(nombreProducto, stockActual, stockMinimo);
            });
        }

        /// <summary>
        /// Método genérico para enviar notificación personalizada por Email y WhatsApp
        /// </summary>
        public static void EnviarNotificacion(string titulo, string mensaje, string tipo = "info", string icono = "fa-bell")
        {
            // Agregar notificación al sistema
            AgregarNotificacion(mensaje, tipo, icono);

            // Enviar notificaciones de forma asíncrona
            System.Threading.Tasks.Task.Run(async () =>
            {
                // Enviar correo electrónico
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

                EnviarCorreo(titulo + " - Stockify", cuerpo);

                // Enviar WhatsApp
                await WhatsAppService.EnviarMensajeWhatsApp($"*{titulo}*\n\n{mensaje}\n\n_Sistema Stockify_");
            });
        }
    }
}