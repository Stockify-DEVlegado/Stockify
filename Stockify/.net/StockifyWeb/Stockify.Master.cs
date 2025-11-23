using System;
using System.Web.UI;

namespace StockifyWeb
{
    public partial class Stockify : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar si hay una sesión activa
            if (Session["Usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // IMPORTANTE: Manejar actualización de notificaciones ANTES de cualquier otra cosa
            string eventTarget = Request["__EVENTTARGET"];
            string eventArgument = Request["__EVENTARGUMENT"];

            // Si es una solicitud de actualización de notificaciones
            if (eventArgument == "RefreshNotifications" || eventTarget == "UpdateNotifications")
            {
                CargarNotificaciones();
                return; // SALIR inmediatamente para NO ejecutar Page_Load de las páginas hijas
            }

            // Si es marcar como leída
            if (eventTarget == "MarcarLeida" && !string.IsNullOrEmpty(eventArgument))
            {
                int notifId = int.Parse(eventArgument);
                NotificationService.MarcarComoLeida(notifId);
                CargarNotificaciones();
                return; // SALIR inmediatamente
            }

            // Cargar datos iniciales solo la primera vez
            if (!IsPostBack)
            {
                AplicarPermisosPorRol();
                lblUsuario.Text = Session["Usuario"].ToString();
            }

            // Siempre cargar notificaciones
            CargarNotificaciones();
        }

        private void AplicarPermisosPorRol()
        {
            string rol = Session["TipoUsuario"] as string;

            // Ocultar todo por defecto
            lnkInicio.Visible = false;
            lnkInventario.Visible = false;
            lnkReportes.Visible = false;
            lnkProveedores.Visible = false;
            lnkOrdenes.Visible = false;
            lnkGestionCuentas.Visible = false;
            lnkGestionCategorias.Visible = false; // ← AGREGADO

            if (string.IsNullOrEmpty(rol))
            {
                // Si no hay sesión, redirige al login
                Response.Redirect("Login.aspx");
                return;
            }

            switch (rol.ToLower())
            {
                case "administrador":
                    lnkInicio.Visible = true;
                    lnkInventario.Visible = true;
                    lnkReportes.Visible = true;
                    lnkProveedores.Visible = true;
                    lnkOrdenes.Visible = true;
                    lnkGestionCuentas.Visible = true;
                    lnkGestionCategorias.Visible = true; // ← AGREGADO
                    break;

                case "principal":
                    lnkInicio.Visible = true;
                    lnkInventario.Visible = true;
                    lnkReportes.Visible = true;
                    lnkProveedores.Visible = true;
                    lnkOrdenes.Visible = true;
                    lnkGestionCuentas.Visible = true;
                    lnkGestionCategorias.Visible = true; // ← AGREGADO
                    break;

                case "operario":
                    lnkInicio.Visible = true;
                    lnkInventario.Visible = true;
                    lnkProveedores.Visible = true;
                    lnkOrdenes.Visible = true;
                    // Los operarios NO ven Gestión de Cuentas ni Categorías
                    break;

                default:
                    // Opcional: redirigir o mostrar error si el rol no es válido
                    break;
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            if (Request.Cookies["StockifyUser"] != null)
            {
                Response.Cookies["StockifyUser"].Expires = DateTime.Now.AddDays(-1);
            }

            Response.Redirect("Login.aspx");
        }

        private void CargarNotificaciones()
        {
            var notificaciones = NotificationService.ObtenerNotificaciones();
            var noLeidas = NotificationService.ContarNoLeidas();

            // Configurar badge
            if (noLeidas > 0)
            {
                pnlBadge.Visible = true;
                litBadgeCount.Text = noLeidas > 99 ? "99+" : noLeidas.ToString();
            }
            else
            {
                pnlBadge.Visible = false;
            }

            // Configurar lista de notificaciones
            if (notificaciones.Count > 0)
            {
                rptNotificaciones.DataSource = notificaciones;
                rptNotificaciones.DataBind();
                pnlSinNotificaciones.Visible = false;

                litTotalNotifications.Text = notificaciones.Count == 1
                    ? "1 notificación"
                    : $"{notificaciones.Count} notificaciones";
            }
            else
            {
                rptNotificaciones.DataSource = null;
                rptNotificaciones.DataBind();
                pnlSinNotificaciones.Visible = true;
                litTotalNotifications.Text = "0 notificaciones";
            }
        }

        protected void btnMarcarTodasLeidas_Click(object sender, EventArgs e)
        {
            NotificationService.MarcarTodasComoLeidas();
            CargarNotificaciones();
        }

        protected string GetTiempoTranscurrido(DateTime fecha)
        {
            var diferencia = DateTime.Now - fecha;

            if (diferencia.TotalMinutes < 1)
                return "Ahora mismo";
            if (diferencia.TotalMinutes < 60)
                return $"Hace {(int)diferencia.TotalMinutes} min";
            if (diferencia.TotalHours < 24)
                return $"Hace {(int)diferencia.TotalHours} h";
            return diferencia.TotalDays < 7 ? $"Hace {(int)diferencia.TotalDays} d" : fecha.ToString("dd/MM/yyyy");
        }
    }
}