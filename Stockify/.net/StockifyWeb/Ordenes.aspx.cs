using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Ordenes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MostrarOrdenCompra();
                CargarDatosIniciales();
            }
        }

        private void CargarDatosIniciales()
        {
            CargarProductos();
            CargarDetalleProducto();

            // Establecer fechas por defecto
            txtFechaIngreso.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaOrdenCompra.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            txtFechaSalida.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaOrdenVenta.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        }

        private void CargarProductos()
        {
            var productos = new List<object>
            {
                new { Codigo = "PO-2025-001", FechaRegistrada = "2025-01-01",
                      Nombre = "BHAVANI SALES CORPORATION",
                      Responsable = "Diego Alvarez Castillo", Total = "7500 $", Estado = "Procesando" },
                new { Codigo = "PO-2025-002", FechaRegistrada = "2025-01-02",
                      Nombre = "BHAVANI SALES CORPORATION",
                      Responsable = "Patrick Cruz Apolaya", Total = "4500 $", Estado = "Cancelado" },
                new { Codigo = "PO-2025-003", FechaRegistrada = "2025-01-03",
                      Nombre = "BHAVANI SALES CORPORATION",
                      Responsable = "Alonso Chipana Cuellar", Total = "10000 $", Estado = "Aceptado" }
            };

            gvProductos.DataSource = productos;
            gvProductos.DataBind();
        }

        private void CargarDetalleProducto()
        {
            gvDetalleProducto.DataSource = new List<object>();
            gvDetalleProducto.DataBind();
        }

        // Método para cambiar clases CSS de badges
        public string GetBadgeClass(string estado)
        {
            switch (estado)
            {
                case "Procesando":
                case "Procesado":
                    return "badge badge-procesando";
                case "Cancelado":
                    return "badge badge-cancelado";
                case "Aceptado":
                    return "badge badge-aceptado";
                default:
                    return "badge";
            }
        }

        // EVENTOS DE TABS
        protected void btnOrdenCompra_Click(object sender, EventArgs e)
        {
            MostrarOrdenCompra();
        }

        protected void btnOrdenVenta_Click(object sender, EventArgs e)
        {
            MostrarOrdenVenta();
        }

        private void MostrarOrdenCompra()
        {
            ordenCompraContent.Style["display"] = "block";
            ordenVentaContent.Style["display"] = "none";
            btnOrdenCompra.CssClass = "active";
            btnOrdenVenta.CssClass = "";
            lblTituloTabla.Text = "Productos de Ingreso";

            // Recargar datos específicos de compra
            CargarProductosCompra();
        }

        private void MostrarOrdenVenta()
        {
            ordenCompraContent.Style["display"] = "none";
            ordenVentaContent.Style["display"] = "block";
            btnOrdenVenta.CssClass = "active";
            btnOrdenCompra.CssClass = "";
            lblTituloTabla.Text = "Productos de Salida";

            // Recargar datos específicos de venta
            CargarProductosVenta();
        }

        private void CargarProductosCompra()
        {
            // Datos específicos para compra
            CargarProductos(); // Usamos los mismos datos por ahora
        }

        private void CargarProductosVenta()
        {
            // Podrías cargar datos diferentes para venta si es necesario
            CargarProductos(); // Usamos los mismos datos por ahora
        }

        // EVENTOS DE ORDEN COMPRA
        protected void btnAgregarCompra_Click(object sender, EventArgs e)
        {
            // Lógica para agregar orden de compra
        }

        protected void btnAnularCompra_Click(object sender, EventArgs e)
        {
            // Lógica para anular orden de compra
        }

        protected void btnEditarCompra_Click(object sender, EventArgs e)
        {
            // Lógica para editar orden de compra
        }

        protected void btnGuardarCompra_Click(object sender, EventArgs e)
        {
            // Lógica para guardar orden de compra
        }

        protected void btnAdjuntarOrdenCompra_Click(object sender, EventArgs e)
        {
            // Lógica para adjuntar orden de compra
        }

        // EVENTOS DE ORDEN VENTA
        protected void btnAgregarVenta_Click(object sender, EventArgs e)
        {
            // Lógica para agregar orden de venta
        }

        protected void btnEliminarVenta_Click(object sender, EventArgs e)
        {
            // Lógica para eliminar orden de venta
        }

        protected void btnEditarVenta_Click(object sender, EventArgs e)
        {
            // Lógica para editar orden de venta
        }

        protected void btnGuardarVenta_Click(object sender, EventArgs e)
        {
            // Lógica para guardar orden de venta
        }

        protected void btnAdjuntarOrdenVenta_Click(object sender, EventArgs e)
        {
            // Lógica para adjuntar orden de venta
        }

        // EVENTOS DE GRILLAS
        protected void gvProductos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Lógica para enlazar datos de productos
        }

        protected void gvProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Lógica cuando se selecciona un producto
            CargarDetalleProductoSeleccionado();
        }

        protected void gvDetalleProducto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Lógica para enlazar datos del detalle
        }

        private void CargarDetalleProductoSeleccionado()
        {
            // Lógica para cargar detalle del producto seleccionado
        }
    }
}