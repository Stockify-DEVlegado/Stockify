using System;
using System.Collections.Generic;
using System.Web.UI;

namespace StockifyWeb
{
    public partial class Inicio : Page
    {
        // Clases ejemplo
        public class OrdenReciente
        {
            public string Tipo { get; set; }
            public DateTime Fecha { get; set; }
        }

        public class AlertaStock
        {
            public string NombreProducto { get; set; }
            public int StockActual { get; set; }
            public int StockMinimo { get; set; }
            public string Estado { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDashboard();
            }
        }

        private void CargarDashboard()
        {
            CargarKPIs();
            CargarOrdenesRecientes();
            CargarAlertasStock();
        }

        private void CargarKPIs()
        {
            lblTotalProductos.Text = ObtenerTotalProductos().ToString();
            lblEnStock.Text = ObtenerStockTotal().ToString();
            lblPorRecibir.Text = ObtenerProductosPorRecibir().ToString();
            lblEntradas.Text = ObtenerEntradasSemana().ToString();
            lblSalidas.Text = ObtenerSalidasSemana().ToString();
            lblNumProveedores.Text = ObtenerNumeroProveedores().ToString();
            lblNumCategorias.Text = ObtenerNumeroCategorias().ToString();
        }

        private void CargarOrdenesRecientes()
        {
            List<OrdenReciente> ordenes = ObtenerOrdenesRecientes();
            rptOrdenesRecientes.DataSource = ordenes;
            rptOrdenesRecientes.DataBind();
        }

        private void CargarAlertasStock()
        {
            List<AlertaStock> alertas = ObtenerAlertasStock();
            rptAlertasStock.DataSource = alertas;
            rptAlertasStock.DataBind();
        }


        

        private int ObtenerTotalProductos()
        {
            // Ejemplo: return new ProductoBO().ObtenerTotal();
            return 50;
        }

        private int ObtenerStockTotal()
        {
            return 50;
        }

        private int ObtenerProductosPorRecibir()
        {
            return 14;
        }

        private int ObtenerEntradasSemana()
        {
            return 5;
        }

        private int ObtenerSalidasSemana()
        {
            return 21;
        }

        private int ObtenerNumeroProveedores()
        {
            return 31;
        }

        private int ObtenerNumeroCategorias()
        {
            return 21;
        }

        private List<OrdenReciente> ObtenerOrdenesRecientes()
        {
            return new List<OrdenReciente>
            {
                new OrdenReciente { Tipo = "Compra", Fecha = new DateTime(2025, 7, 21) },
                new OrdenReciente { Tipo = "Venta", Fecha = new DateTime(2025, 7, 30) },
                new OrdenReciente { Tipo = "Venta", Fecha = new DateTime(2025, 8, 15) },
                new OrdenReciente { Tipo = "Compra", Fecha = new DateTime(2025, 8, 15) }
            };
        }

        private List<AlertaStock> ObtenerAlertasStock()
        {
            return new List<AlertaStock>
            {
                new AlertaStock
                {
                    NombreProducto = "Teclado en ruso",
                    StockActual = 2,
                    StockMinimo = 5,
                    Estado = "Low"
                },
                new AlertaStock
                {
                    NombreProducto = "Monitor ShangChun",
                    StockActual = 2,
                    StockMinimo = 5,
                    Estado = "Low"
                }
            };
        }

    }
}