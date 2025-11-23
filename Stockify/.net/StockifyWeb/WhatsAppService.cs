using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StockifyWeb.Services
{
    /// <summary>
    /// Servicio para enviar mensajes de WhatsApp usando Ultramsg API (GRATIS)
    /// Registro: https://ultramsg.com
    /// Plan gratuito: 100 mensajes/mes
    /// </summary>
    public static class WhatsAppService
    {
        // Configuración desde Web.config
        private static readonly string UltramsgInstanceId = ConfigurationManager.AppSettings["UltramsgInstanceId"];
        private static readonly string UltramsgToken = ConfigurationManager.AppSettings["UltramsgToken"];
        private static readonly string WhatsAppNumero = ConfigurationManager.AppSettings["WhatsAppNotificationNumber"];

        /// <summary>
        /// Envía un mensaje de WhatsApp a un número específico o al configurado por defecto
        /// </summary>
        /// <param name="mensaje">Mensaje a enviar</param>
        /// <param name="numeroDestino">Número de teléfono en formato 51XXXXXXXXX (opcional)</param>
        public static async Task<bool> EnviarMensajeWhatsApp(string mensaje, string numeroDestino = null)
        {
            try
            {
                // Si no se proporciona número, usar el de configuración
                if (string.IsNullOrEmpty(numeroDestino))
                {
                    numeroDestino = WhatsAppNumero;
                }

                // Validar configuración
                if (string.IsNullOrEmpty(UltramsgInstanceId) || string.IsNullOrEmpty(UltramsgToken))
                {
                    System.Diagnostics.Debug.WriteLine("[WHATSAPP] API no configurada");
                    return false;
                }

                if (string.IsNullOrEmpty(numeroDestino))
                {
                    System.Diagnostics.Debug.WriteLine("[WHATSAPP] Número de destino no configurado");
                    return false;
                }

                using (var client = new HttpClient())
                {
                    // URL de la API de Ultramsg
                    string url = $"https://api.ultramsg.com/{UltramsgInstanceId}/messages/chat";

                    // Preparar datos
                    var content = new StringContent(
                        $"{{\"token\":\"{UltramsgToken}\",\"to\":\"{numeroDestino}\",\"body\":\"{EscaparJson(mensaje)}\"}}",
                        Encoding.UTF8,
                        "application/json"
                    );

                    // Enviar petición
                    var response = await client.PostAsync(url, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"[WHATSAPP] ✅ Mensaje enviado exitosamente a {numeroDestino}");
                        System.Diagnostics.Debug.WriteLine($"[WHATSAPP] Respuesta: {responseBody}");
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[WHATSAPP ERROR] ❌ Status: {response.StatusCode}");
                        System.Diagnostics.Debug.WriteLine($"[WHATSAPP ERROR] Respuesta: {responseBody}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WHATSAPP ERROR] ❌ {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Envía notificación de nuevo producto por WhatsApp
        /// </summary>
        public static async Task EnviarNotificacionNuevoProducto(string nombreProducto, string categoria = "Sin categoría", double precio = 0, string numeroDestino = null)
        {
            string mensaje = $"🆕 *NUEVO PRODUCTO REGISTRADO*\n\n" +
                           $"📦 Producto: *{nombreProducto}*\n" +
                           $"📂 Categoría: {categoria}\n" +
                           $"💰 Precio: S/ {precio:N2}\n\n" +
                           $"✅ El producto ha sido agregado exitosamente al inventario.\n\n" +
                           $"_Sistema Stockify_";

            await EnviarMensajeWhatsApp(mensaje, numeroDestino);
        }

        /// <summary>
        /// Envía alerta de stock bajo por WhatsApp
        /// </summary>
        public static async Task EnviarAlertaStockBajo(string nombreProducto, int stockActual, int stockMinimo, string numeroDestino = null)
        {
            string mensaje = $"⚠️ *ALERTA DE STOCK BAJO*\n\n" +
                           $"📦 Producto: *{nombreProducto}*\n" +
                           $"📊 Stock actual: {stockActual} unidades\n" +
                           $"📉 Stock mínimo: {stockMinimo} unidades\n\n" +
                           $"🔔 Se recomienda reabastecer este producto.\n\n" +
                           $"_Sistema Stockify_";

            await EnviarMensajeWhatsApp(mensaje, numeroDestino);
        }

        /// <summary>
        /// Envía notificación de producto eliminado
        /// </summary>
        public static async Task EnviarNotificacionProductoEliminado(string nombreProducto, string numeroDestino = null)
        {
            string mensaje = $"🗑️ *PRODUCTO ELIMINADO*\n\n" +
                           $"📦 Producto: *{nombreProducto}*\n\n" +
                           $"❌ El producto ha sido eliminado del inventario.\n\n" +
                           $"_Sistema Stockify_";

            await EnviarMensajeWhatsApp(mensaje, numeroDestino);
        }

        /// <summary>
        /// Envía notificación de producto actualizado
        /// </summary>
        public static async Task EnviarNotificacionProductoActualizado(string nombreProducto, string numeroDestino = null)
        {
            string mensaje = $"✏️ *PRODUCTO ACTUALIZADO*\n\n" +
                           $"📦 Producto: *{nombreProducto}*\n\n" +
                           $"✅ La información del producto ha sido actualizada.\n\n" +
                           $"_Sistema Stockify_";

            await EnviarMensajeWhatsApp(mensaje, numeroDestino);
        }

        /// <summary>
        /// Envía mensaje de prueba
        /// </summary>
        public static async Task<bool> EnviarMensajePrueba(string numeroDestino = null)
        {
            string mensaje = "🧪 *MENSAJE DE PRUEBA*\n\n" +
                           "✅ La integración con WhatsApp está funcionando correctamente.\n\n" +
                           "_Sistema Stockify_";

            return await EnviarMensajeWhatsApp(mensaje, numeroDestino);
        }

        /// <summary>
        /// Escapa caracteres especiales para JSON
        /// </summary>
        private static string EscaparJson(string texto)
        {
            return texto
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}