using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace StockifyWeb
{
    public partial class Inicio : Page
    {
        // Clases DTO locales
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
            // CRÍTICO: Detectar si es una petición de notificaciones y SALIR inmediatamente
            string eventTarget = Request["__EVENTTARGET"];
            string eventArgument = Request["__EVENTARGUMENT"];

            // Si es actualización de notificaciones o marcar como leída, NO cargar el dashboard
            if (eventArgument == "RefreshNotifications" ||
                eventTarget == "UpdateNotifications" ||
                eventTarget == "MarcarLeida")
            {
                // NO hacer nada, el Master ya manejó la notificación
                return;
            }

            // Cargar dashboard solo si NO es postback
            if (!IsPostBack)
            {
                CargarDashboard();
            }
        }

        private void CargarDashboard()
        {
            try
            {
                CargarKPIs();
                CargarGraficoMovimientos();
                CargarOrdenesRecientes();
                CargarAlertasStock();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar dashboard: {ex.Message}");
            }
        }

        #region KPIs
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

        private int ObtenerTotalProductos()
        {
            ProductoWSClient productoClient = null;
            try
            {
                productoClient = new ProductoWSClient();
                return productoClient.contarProductos();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerTotalProductos: {ex.Message}");
                return 0;
            }
            finally
            {
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
                }
            }
        }

        private int ObtenerStockTotal()
        {
            ExistenciasWSClient existenciasClient = null;
            try
            {
                existenciasClient = new ExistenciasWSClient();
                return existenciasClient.obtenerStockTotal();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerStockTotal: {ex.Message}");
                return 0;
            }
            finally
            {
                if (existenciasClient != null && existenciasClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    existenciasClient.Close();
                }
            }
        }

        private int ObtenerProductosPorRecibir()
        {
            OrdenCompraWSClient ordenCompraClient = null;
            try
            {
                ordenCompraClient = new OrdenCompraWSClient();
                return ordenCompraClient.obtenerProductosPorRecibir();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerProductosPorRecibir: {ex.Message}");
                return 0;
            }
            finally
            {
                if (ordenCompraClient != null && ordenCompraClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    ordenCompraClient.Close();
                }
            }
        }

        private int ObtenerEntradasSemana()
        {
            MovimientoWSClient movimientoClient = null;
            try
            {
                movimientoClient = new MovimientoWSClient();
                return movimientoClient.contarMovimientosEntrada(7);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerEntradasSemana: {ex.Message}");
                return 0;
            }
            finally
            {
                if (movimientoClient != null && movimientoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    movimientoClient.Close();
                }
            }
        }

        private int ObtenerSalidasSemana()
        {
            MovimientoWSClient movimientoClient = null;
            try
            {
                movimientoClient = new MovimientoWSClient();
                return movimientoClient.contarMovimientosSalida(7);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerSalidasSemana: {ex.Message}");
                return 0;
            }
            finally
            {
                if (movimientoClient != null && movimientoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    movimientoClient.Close();
                }
            }
        }

        private int ObtenerNumeroProveedores()
        {
            EmpresaWSClient empresaClient = null;
            try
            {
                empresaClient = new EmpresaWSClient();
                return empresaClient.contarProveedores();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerNumeroProveedores: {ex.Message}");
                return 0;
            }
            finally
            {
                if (empresaClient != null && empresaClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    empresaClient.Close();
                }
            }
        }

        private int ObtenerNumeroCategorias()
        {
            CategoriaWSClient categoriaClient = null;
            try
            {
                categoriaClient = new CategoriaWSClient();
                return categoriaClient.contarCategorias();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerNumeroCategorias: {ex.Message}");
                return 0;
            }
            finally
            {
                if (categoriaClient != null && categoriaClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    categoriaClient.Close();
                }
            }
        }
        #endregion

        #region Gráfico de Movimientos
        private void CargarGraficoMovimientos()
        {
            MovimientoWSClient movimientoClient = null;
            try
            {
                movimientoClient = new MovimientoWSClient();

                // Obtener movimientos de los últimos 7 meses
                var movimientos = movimientoClient.obtenerMovimientosPorMes(7);

                if (movimientos != null && movimientos.Length > 0)
                {
                    List<string> meses = new List<string>();
                    List<int> entradas = new List<int>();
                    List<int> salidas = new List<int>();

                    foreach (var mov in movimientos)
                    {
                        meses.Add(mov.mes);
                        entradas.Add(mov.entradas);
                        salidas.Add(mov.salidas);
                    }

                    // Registrar datos para JavaScript
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string mesesJSON = serializer.Serialize(meses);
                    string entradasJSON = serializer.Serialize(entradas);
                    string salidasJSON = serializer.Serialize(salidas);

                    string script = $@"
                        <script type='text/javascript'>
                            window.dashboardData = {{
                                meses: {mesesJSON},
                                entradas: {entradasJSON},
                                salidas: {salidasJSON}
                            }};
                        </script>
                    ";

                    ClientScript.RegisterStartupScript(this.GetType(), "DatosGrafico", script, false);
                }
                else
                {
                    // Datos por defecto si no hay información
                    RegistrarDatosGraficoPorDefecto();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CargarGraficoMovimientos: {ex.Message}");
                RegistrarDatosGraficoPorDefecto();
            }
            finally
            {
                if (movimientoClient != null && movimientoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    movimientoClient.Close();
                }
            }
        }

        private void RegistrarDatosGraficoPorDefecto()
        {
            string scriptDefault = @"
                <script type='text/javascript'>
                    window.dashboardData = {
                        meses: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul'],
                        entradas: [0, 0, 0, 0, 0, 0, 0],
                        salidas: [0, 0, 0, 0, 0, 0, 0]
                    };
                </script>
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "DatosGrafico", scriptDefault, false);
        }
        #endregion

        #region Órdenes Recientes
        private void CargarOrdenesRecientes()
        {
            List<OrdenReciente> ordenes = ObtenerOrdenesRecientes();

            if (ordenes != null && ordenes.Count > 0)
            {
                rptOrdenesRecientes.DataSource = ordenes;
                rptOrdenesRecientes.DataBind();
                pnlNoOrdenes.Visible = false;
            }
            else
            {
                pnlNoOrdenes.Visible = true;
            }
        }

        private List<OrdenReciente> ObtenerOrdenesRecientes()
        {
            List<OrdenReciente> ordenesRecientes = new List<OrdenReciente>();

            try
            {
                // Obtener últimas 2 órdenes de cada tipo (total 8)

                // Órdenes de Compra
                OrdenCompraWSClient ordenCompraClient = null;
                try
                {
                    ordenCompraClient = new OrdenCompraWSClient();
                    var ordenesCompra = ordenCompraClient.listarOrdenesCompra();
                    if (ordenesCompra != null && ordenesCompra.Length > 0)
                    {
                        foreach (var orden in ordenesCompra.OrderByDescending(o => o.fecha).Take(2))
                        {
                            ordenesRecientes.Add(new OrdenReciente
                            {
                                Tipo = "Compra",
                                Fecha = orden.fecha
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error obteniendo órdenes de compra: {ex.Message}");
                }
                finally
                {
                    if (ordenCompraClient != null && ordenCompraClient.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        ordenCompraClient.Close();
                    }
                }

                // Órdenes de Venta
                OrdenVentaWSClient ordenVentaClient = null;
                try
                {
                    ordenVentaClient = new OrdenVentaWSClient();
                    var ordenesVenta = ordenVentaClient.listarOrdenesVenta();
                    if (ordenesVenta != null && ordenesVenta.Length > 0)
                    {
                        foreach (var orden in ordenesVenta.OrderByDescending(o => o.fecha).Take(2))
                        {
                            ordenesRecientes.Add(new OrdenReciente
                            {
                                Tipo = "Venta",
                                Fecha = orden.fecha
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error obteniendo órdenes de venta: {ex.Message}");
                }
                finally
                {
                    if (ordenVentaClient != null && ordenVentaClient.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        ordenVentaClient.Close();
                    }
                }

                // Órdenes de Ingreso
                OrdenIngresoWSClient ordenIngresoClient = null;
                try
                {
                    ordenIngresoClient = new OrdenIngresoWSClient();
                    var ordenesIngreso = ordenIngresoClient.listarOrdenesIngreso();
                    if (ordenesIngreso != null && ordenesIngreso.Length > 0)
                    {
                        foreach (var orden in ordenesIngreso.OrderByDescending(o => o.fecha).Take(2))
                        {
                            ordenesRecientes.Add(new OrdenReciente
                            {
                                Tipo = "Ingreso",
                                Fecha = orden.fecha
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error obteniendo órdenes de ingreso: {ex.Message}");
                }
                finally
                {
                    if (ordenIngresoClient != null && ordenIngresoClient.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        ordenIngresoClient.Close();
                    }
                }

                // Órdenes de Salida
                OrdenSalidaWSClient ordenSalidaClient = null;
                try
                {
                    ordenSalidaClient = new OrdenSalidaWSClient();
                    var ordenesSalida = ordenSalidaClient.listarOrdenesSalida();
                    if (ordenesSalida != null && ordenesSalida.Length > 0)
                    {
                        foreach (var orden in ordenesSalida.OrderByDescending(o => o.fecha).Take(2))
                        {
                            ordenesRecientes.Add(new OrdenReciente
                            {
                                Tipo = "Salida",
                                Fecha = orden.fecha
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error obteniendo órdenes de salida: {ex.Message}");
                }
                finally
                {
                    if (ordenSalidaClient != null && ordenSalidaClient.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        ordenSalidaClient.Close();
                    }
                }

                // Ordenar todas por fecha descendente y tomar las 8 más recientes
                if (ordenesRecientes.Count > 0)
                {
                    ordenesRecientes = ordenesRecientes
                        .OrderByDescending(o => o.Fecha)
                        .Take(8)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerOrdenesRecientes: {ex.Message}");
            }

            return ordenesRecientes;
        }
        #endregion

        #region Alertas de Stock
        private void CargarAlertasStock()
        {
            List<AlertaStock> alertas = ObtenerAlertasStock();

            if (alertas != null && alertas.Count > 0)
            {
                rptAlertasStock.DataSource = alertas;
                rptAlertasStock.DataBind();
                pnlNoAlertas.Visible = false;
            }
            else
            {
                pnlNoAlertas.Visible = true;
            }
        }

        private List<AlertaStock> ObtenerAlertasStock()
        {
            List<AlertaStock> alertas = new List<AlertaStock>();
            ExistenciasWSClient existenciasClient = null;

            try
            {
                existenciasClient = new ExistenciasWSClient();
                var alertasWS = existenciasClient.obtenerAlertasStock();

                if (alertasWS != null && alertasWS.Length > 0)
                {
                    foreach (var alerta in alertasWS)
                    {
                        alertas.Add(new AlertaStock
                        {
                            NombreProducto = alerta.nombreProducto,
                            StockActual = alerta.stockActual,
                            StockMinimo = alerta.stockMinimo,
                            Estado = alerta.estado
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerAlertasStock: {ex.Message}");
            }
            finally
            {
                if (existenciasClient != null && existenciasClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    existenciasClient.Close();
                }
            }

            return alertas;
        }
        #endregion
    }
}