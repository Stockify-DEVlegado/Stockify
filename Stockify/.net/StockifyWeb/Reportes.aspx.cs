using StockifyWeb.StockifyWS;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Reportes : System.Web.UI.Page
    {
        private const string URL_BASE_REPORTES = "http://localhost:8080/StockifyReportes/reportes";

        private ProductoWSClient productoWS;
        private EmpresaWSClient empresaWS;
        private CategoriaWSClient categoriaWS;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InicializarServicios();
                CargarDatos();
                txtFechaDesdeKardex.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                txtFechaHastaKardex.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void InicializarServicios()
        {
            try
            {
                productoWS = new ProductoWSClient();
                empresaWS = new EmpresaWSClient();
                categoriaWS = new CategoriaWSClient();
            }
            catch (Exception ex)
            {
                MostrarError("Error al inicializar servicios: " + ex.Message);
            }
        }

        private void CargarDatos()
        {
            try
            {
                CargarProductos();
                CargarProveedores();
                CargarCategorias();
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar datos: " + ex.Message);
            }
        }

        #region Carga de DropDownLists

        private void CargarProductos()
        {
            try
            {
                var productos = productoWS.listarProductos();

                ddlKardexProducto.Items.Clear();
                ddlKardexProducto.Items.Add(new ListItem("Selecciona un producto", ""));

                if (productos != null && productos.Length > 0)
                {
                    foreach (var producto in productos)
                    {
                        string textoProducto = $"{producto.nombre}";
                        ddlKardexProducto.Items.Add(new ListItem(textoProducto, producto.idProducto.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar productos: " + ex.Message);
            }
        }

        private void CargarProveedores()
        {
            try
            {
                var empresas = empresaWS.listarEmpresas();

                ddlFiltroProveedor.Items.Clear();
                ddlFiltroProveedor.Items.Add(new ListItem("Todos los proveedores", ""));

                if (empresas != null && empresas.Length > 0)
                {
                    var proveedores = empresas.Where(e => e.tipoEmpresa.Equals("PROVEEDOR"));

                    foreach (var proveedor in proveedores)
                    {
                        string textoProveedor = proveedor.razonSocial;
                        ddlFiltroProveedor.Items.Add(new ListItem(textoProveedor, proveedor.idEmpresa.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar proveedores: " + ex.Message);
            }
        }

        private void CargarCategorias()
        {
            try
            {
                var categorias = categoriaWS.listarCategorias();

                ddlCategoria.Items.Clear();
                ddlCategoria.Items.Add(new ListItem("Todas las categorías", ""));

                if (categorias != null && categorias.Length > 0)
                {
                    foreach (var categoria in categorias)
                    {
                        ddlCategoria.Items.Add(new ListItem(categoria.nombre, categoria.idCategoria.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar categorías: " + ex.Message);
            }
        }

        #endregion

        #region Generación de Reportes

        protected void btnGenerarKardex_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlKardexProducto.SelectedValue))
                {
                    MostrarMensaje("Por favor selecciona un producto para generar el Kardex.", false);
                    return;
                }

                string productoId = ddlKardexProducto.SelectedValue;
                string metodo = ddlMetodoValoracionReporte.SelectedValue;
                string fechaDesde = txtFechaDesdeKardex.Text;
                string fechaHasta = txtFechaHastaKardex.Text;
                string productoNombre = ddlKardexProducto.SelectedItem.Text;

                if (string.IsNullOrEmpty(fechaDesde) || string.IsNullOrEmpty(fechaHasta))
                {
                    MostrarMensaje("Por favor ingresa las fechas desde y hasta para generar el Kardex.", false);
                    return;
                }

                string metodoReporte = (metodo == "PP") ? "PROMEDIO" : "PEPS";
                string url = $"{URL_BASE_REPORTES}/kardexs?idProducto={productoId}&fechaDesde={fechaDesde}&fechaHasta={fechaHasta}&metodo={metodoReporte}";

                // Abrir en nueva ventana
                string script = $@"
                    var ventana = window.open('{url}', '_blank', 'width=800,height=600,scrollbars=yes,resizable=yes');
                    if (!ventana || ventana.closed || typeof ventana.closed == 'undefined') {{
                        alert('Por favor permite ventanas emergentes para ver el reporte.');
                    }}
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "AbrirKardex", script, true);

                string mensaje = $"Generando Kardex para: {productoNombre} con método {(metodo == "PP" ? "Promedio Ponderado" : "PEPS")} del {fechaDesde} al {fechaHasta}";
                MostrarMensaje(mensaje, true);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al generar el Kardex: {ex.Message}", false);
            }
        }

        protected void btnGenerarReporteProductos_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener el autor de la sesión (SOLO PARA ESTE REPORTE)
                string autor = ObtenerNombreUsuarioSesion();

                string url = $"{URL_BASE_REPORTES}/productos?autor={HttpUtility.UrlEncode(autor)}";

                // Abrir en nueva ventana
                string script = $@"
                    var ventana = window.open('{url}', '_blank', 'width=800,height=600,scrollbars=yes,resizable=yes');
                    if (!ventana || ventana.closed || typeof ventana.closed == 'undefined') {{
                        alert('Por favor permite ventanas emergentes para ver el reporte.');
                    }}
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "AbrirProductos", script, true);

                MostrarMensaje("Generando reporte de existencias de todos los productos", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al generar el reporte: {ex.Message}", false);
            }
        }

        protected void btnGenerarReporteProveedores_Click(object sender, EventArgs e)
        {
            try
            {
                string proveedorId = ddlFiltroProveedor.SelectedValue;
                string url;

                if (string.IsNullOrEmpty(proveedorId))
                {
                    url = $"{URL_BASE_REPORTES}/proveedoresProducto";
                }
                else
                {
                    url = $"{URL_BASE_REPORTES}/proveedoresProducto?idProveedor={proveedorId}";
                }

                // Abrir en nueva ventana
                string script = $@"
                    var ventana = window.open('{url}', '_blank', 'width=800,height=600,scrollbars=yes,resizable=yes');
                    if (!ventana || ventana.closed || typeof ventana.closed == 'undefined') {{
                        alert('Por favor permite ventanas emergentes para ver el reporte.');
                    }}
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "AbrirProveedores", script, true);

                string mensaje = string.IsNullOrEmpty(proveedorId)
                    ? "Generando reporte de todos los proveedores"
                    : $"Generando reporte para: {ddlFiltroProveedor.SelectedItem.Text}";

                MostrarMensaje(mensaje, true);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al generar el reporte: {ex.Message}", false);
            }
        }

        protected void btnGenerarReporteCategorias_Click(object sender, EventArgs e)
        {
            try
            {
                string categoriaId = ddlCategoria.SelectedValue;

                // Obtener el autor de la sesión (SOLO PARA ESTE REPORTE)
                string autor = ObtenerNombreUsuarioSesion();

                string url;

                if (string.IsNullOrEmpty(categoriaId))
                {
                    url = $"{URL_BASE_REPORTES}/productosCategoria?autor={HttpUtility.UrlEncode(autor)}";
                }
                else
                {
                    url = $"{URL_BASE_REPORTES}/productosCategoria?idCategoria={categoriaId}&autor={HttpUtility.UrlEncode(autor)}";
                }

                // Abrir en nueva ventana
                string script = $@"
                    var ventana = window.open('{url}', '_blank', 'width=800,height=600,scrollbars=yes,resizable=yes');
                    if (!ventana || ventana.closed || typeof ventana.closed == 'undefined') {{
                        alert('Por favor permite ventanas emergentes para ver el reporte.');
                    }}
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "AbrirCategorias", script, true);

                string mensaje = string.IsNullOrEmpty(categoriaId)
                    ? "Generando reporte de todas las categorías"
                    : $"Generando reporte para la categoría: {ddlCategoria.SelectedItem.Text}";

                MostrarMensaje(mensaje, true);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al generar el reporte: {ex.Message}", false);
            }
        }

        #endregion

        #region Utilidades

        /// <summary>
        /// Obtiene el nombre completo del usuario desde la sesión
        /// </summary>
        private string ObtenerNombreUsuarioSesion()
        {
            // Intenta obtener el nombre del usuario de la sesión
            string nombreUsuario = Session["Usuario"]?.ToString();

            if (string.IsNullOrEmpty(nombreUsuario))
            {
                // Si no hay nombre, usar un valor por defecto
                nombreUsuario = "Usuario Desconocido";
            }

            return nombreUsuario;
        }

        private void MostrarMensaje(string mensaje, bool esExitoso)
        {
            lblMensaje.Text = mensaje;
            pnlMensaje.CssClass = esExitoso ? "status-message status-success" : "status-message status-error";
            pnlMensaje.Visible = true;

            // Ocultar el mensaje después de 5 segundos usando ScriptManager
            string script = $"setTimeout(function() {{ var panel = document.getElementById('{pnlMensaje.ClientID}'); if(panel) panel.style.display = 'none'; }}, 5000);";
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarMensaje", script, true);
        }

        private void MostrarError(string mensaje)
        {
            MostrarMensaje(mensaje, false);
        }

        #endregion
    }
}