using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Stockify : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar si hay una sesión activa
                if (Session["Usuario"] == null)
                {
                    // Si no hay sesión, redirigir al login
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    // Mostrar el nombre del usuario en el dropdown
                    lblUsuario.Text = Session["Usuario"].ToString();
                }
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
    }
}