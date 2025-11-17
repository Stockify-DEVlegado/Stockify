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
            //lnkAgregarUsuario.Visible = false;

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
    }
}