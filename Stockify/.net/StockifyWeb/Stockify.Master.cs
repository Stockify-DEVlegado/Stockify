using System;
using System.Web.UI;
//using StockifyWeb.Services;

namespace StockifyWeb
{
    public partial class Stockify : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar si hay una sesión activa
                if (Session["Usuario"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    lblUsuario.Text = Session["Usuario"].ToString();
                }

                CargarNotificaciones();
            }

            // Manejar postback para marcar como leída
            if (IsPostBack)
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