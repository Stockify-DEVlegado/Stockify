using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Reportes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InicializarDatos();
            }
        }

        private void InicializarDatos()
        {
            CargarCategorias();
            CargarArticulos();
            CargarMovimientosKardex(1);

            txtFechaDesde.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            txtFechaHasta.Text = DateTime.Now.ToString("yyyy-MM-dd");

            CargarDatosSelect2();
        }

        private void CargarDatosSelect2()
        {
        }

        #region Gestión de Inventario - Reportes

        private void CargarCategorias()
        {
            ddlCategoria.Items.Clear();
            ddlCategoria.Items.Add(new ListItem("Seleccionar categoría", ""));
            ddlCategoria.Items.Add(new ListItem("Electrónica", "1"));
            ddlCategoria.Items.Add(new ListItem("Ropa", "2"));
            ddlCategoria.Items.Add(new ListItem("Hogar", "3"));
            ddlCategoria.Items.Add(new ListItem("Juguetes", "4"));
        }

        protected void btnGenerarReporteProductos_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlFiltroProducto.SelectedValue))
                {
                    MostrarMensaje("Por favor selecciona un producto");
                    return;
                }

                string productoId = ddlFiltroProducto.SelectedValue;
                string productoNombre = ddlFiltroProducto.SelectedItem.Text;

                MostrarMensaje($"Generando reporte de existencias para: {productoNombre}");
            }
            catch (Exception ex)
            {
                MostrarError("Error al generar reporte: " + ex.Message);
            }
        }

        protected void btnGenerarReporteProveedores_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlFiltroProveedor.SelectedValue))
                {
                    MostrarMensaje("Por favor selecciona un proveedor");
                    return;
                }

                string proveedorId = ddlFiltroProveedor.SelectedValue;
                string proveedorNombre = ddlFiltroProveedor.SelectedItem.Text;

                MostrarMensaje($"Generando reporte de proveedores para: {proveedorNombre}");
            }
            catch (Exception ex)
            {
                MostrarError("Error al generar reporte: " + ex.Message);
            }
        }

        protected void btnGenerarReporteCategorias_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlCategoria.SelectedValue))
                {
                    MostrarError("Por favor seleccione una categoría");
                    return;
                }

                string categoriaId = ddlCategoria.SelectedValue;
                string categoriaNombre = ddlCategoria.SelectedItem.Text;

                MostrarMensaje($"Generando reporte de productos para la categoría: {categoriaNombre}");
            }
            catch (Exception ex)
            {
                MostrarError("Error al generar reporte: " + ex.Message);
            }
        }

        #endregion

        #region Kardex - Gestión de Artículos

        private void CargarArticulos(string filtro = "")
        {
            try
            {
                DataTable dt = ObtenerArticulosMock(filtro);

                rptArticulos.DataSource = dt;
                rptArticulos.DataBind();

                ActualizarEstadisticasTotales();
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar artículos: " + ex.Message);
            }
        }

        private DataTable ObtenerArticulosMock(string filtro = "")
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductoId", typeof(int));
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("Codigo", typeof(string));

            dt.Rows.Add(1, "Desktop Computer", "PROD-001");
            dt.Rows.Add(2, "Network Switch", "PROD-002");
            dt.Rows.Add(3, "Printer Cartridge", "PROD-003");

            if (!string.IsNullOrEmpty(filtro))
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = $"Nombre LIKE '%{filtro}%' OR Codigo LIKE '%{filtro}%'";
                return dv.ToTable();
            }

            return dt;
        }

        protected void txtBuscarArticulo_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscarArticulo.Text.Trim();
            CargarArticulos(filtro);
        }

        protected void rptArticulos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SeleccionarArticulo")
            {
                int productoId = Convert.ToInt32(e.CommandArgument);
                CargarDetalleProducto(productoId);
                CargarMovimientosKardex(productoId);
            }
        }

        private void CargarDetalleProducto(int productoId)
        {
            try
            {
                litNombreProducto.Text = "Desktop Computer";
                litCodigoProducto.Text = "PROD-001";
                litSaldoActual.Text = "8";
                litCostoUnitario.Text = "800.00";
                litValorTotal.Text = "6400.00";
                litEntradas.Text = "10";
                litSalidas.Text = "12";
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar detalle del producto: " + ex.Message);
            }
        }

        #endregion

        #region Kardex - Movimientos y Filtros

        private void CargarMovimientosKardex(int productoId)
        {
            try
            {
                string metodo = ddlMetodoValoracion.SelectedValue;
                DateTime? fechaDesde = !string.IsNullOrEmpty(txtFechaDesde.Text) ?
                    DateTime.Parse(txtFechaDesde.Text) : (DateTime?)null;
                DateTime? fechaHasta = !string.IsNullOrEmpty(txtFechaHasta.Text) ?
                    DateTime.Parse(txtFechaHasta.Text) : (DateTime?)null;

                DataTable dt = ObtenerMovimientosMock(productoId, metodo, fechaDesde, fechaHasta);

                rptMovimientos.DataSource = dt;
                rptMovimientos.DataBind();

                litCantidadMovimientos.Text = $"{dt.Rows.Count} de {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar movimientos: " + ex.Message);
            }
        }

        private DataTable ObtenerMovimientosMock(int productoId, string metodo, DateTime? desde, DateTime? hasta)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Detalle", typeof(string));
            dt.Columns.Add("Documento", typeof(string));
            dt.Columns.Add("EntradaCantidad", typeof(int));
            dt.Columns.Add("EntradaPrecio", typeof(decimal));
            dt.Columns.Add("EntradaTotal", typeof(decimal));
            dt.Columns.Add("SalidaCantidad", typeof(int));
            dt.Columns.Add("SalidaPrecio", typeof(decimal));
            dt.Columns.Add("SalidaTotal", typeof(decimal));
            dt.Columns.Add("SaldoCantidad", typeof(int));
            dt.Columns.Add("SaldoPrecio", typeof(decimal));
            dt.Columns.Add("SaldoTotal", typeof(decimal));

            dt.Rows.Add(
                DateTime.Now.AddDays(-30),
                "SALDO INICIAL",
                "-",
                0, 0m, 0m,
                0, 0m, 0m,
                10, 850m, 8500m
            );

            dt.Rows.Add(
                DateTime.Parse("2025-10-08"),
                "Compra a proveedor",
                "5100",
                10, 750m, 7500m,
                0, 0m, 0m,
                20, 800m, 16000m
            );

            dt.Rows.Add(
                DateTime.Parse("2025-10-10"),
                "Venta",
                "5101",
                0, 0m, 0m,
                12, 800m, 9600m,
                8, 800m, 6400m
            );

            return dt;
        }

        protected void ddlMetodoValoracion_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarMovimientosKardex(1);
        }

        protected void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            txtFechaDesde.Text = "";
            txtFechaHasta.Text = "";
            CargarMovimientosKardex(1);
        }

        protected void btnExportarCSV_Click(object sender, EventArgs e)
        {
            try
            {
                MostrarMensaje("Exportando a CSV...");
            }
            catch (Exception ex)
            {
                MostrarError("Error al exportar: " + ex.Message);
            }
        }

        #endregion

        #region Estadísticas Totales

        private void ActualizarEstadisticasTotales()
        {
            try
            {
                litTotalArticulos.Text = "3";
                litValorTotalInventario.Text = "8900.00";
                litUnidadesTotales.Text = "33";
            }
            catch (Exception ex)
            {
                MostrarError("Error al actualizar estadísticas: " + ex.Message);
            }
        }

        #endregion

        #region Utilidades

        private void MostrarMensaje(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{mensaje}');", true);
        }

        private void MostrarError(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error: {mensaje}');", true);
        }

        #endregion
    }
}