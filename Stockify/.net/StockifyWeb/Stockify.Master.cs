using System;
using System.Web.UI;

namespace StockifyWeb
{
    public partial class Stockify : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                AplicarPermisosPorRol();

                // Verificar si hay una sesión activa
                if (Session["Usuario"] == null)
                {
                    // Si no hay sesión, redirigir al login
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    // Mostrar el nombre del usuario
                    lblUsuario.Text = Session["Usuario"].ToString();
                }

                CargarNotificaciones();
            }
            else
            {
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];

                if (eventTarget == "MarcarLeida" && !string.IsNullOrEmpty(eventArgument))
                {
                    int notifId = int.Parse(eventArgument);
                    NotificationService.MarcarComoLeida(notifId);
                    CargarNotificaciones();
                }
            }
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
                    break;

                case "operario":
                    lnkInicio.Visible = true;
                    lnkInventario.Visible = true;
                    lnkProveedores.Visible = true;
                    lnkOrdenes.Visible = true;
                    break;

                default:
                    // Opcional: redirigir o mostrar error si el rol no es válido
                    break;
            }
        }


        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Limpiar la sesión
            Session.Clear();
            Session.Abandon();

            // Limpiar cookies si existen
            if (Request.Cookies["StockifyUser"] != null)
            {
                Response.Cookies["StockifyUser"].Expires = DateTime.Now.AddDays(-1);
            }

            // Redirigir al login
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

        // Método auxiliar para mostrar tiempo transcurrido
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