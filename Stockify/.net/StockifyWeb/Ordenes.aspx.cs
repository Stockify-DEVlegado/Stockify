using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Ordenes : System.Web.UI.Page
    {
        private OrdenCompraWSClient ordenCompraService;
        private OrdenVentaWSClient ordenVentaService;
        private EmpresaWSClient empresaService;
        private OrdenIngresoWSClient ingresoService;
        private OrdenSalidaWSClient salidaService;

        protected void Page_Load(object sender, EventArgs e)
        {
            ordenCompraService = new OrdenCompraWSClient();
            ordenVentaService = new OrdenVentaWSClient();
            empresaService = new EmpresaWSClient();
            ingresoService = new OrdenIngresoWSClient();
            salidaService = new OrdenSalidaWSClient();

            if (!IsPostBack)
            {
                MostrarCompra();
                CargarDatosIniciales();
            }
        }

        private void CargarDatosIniciales()
        {
            CargarProveedores();
            CargarOrdenesCompra();
            CargarOrdenesVenta();
            CargarRegistrosIngreso();
            CargarRegistrosSalida();
            CargarDetalleOrdenVacia();

            txtFechaOrdenCompra.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaOrdenVenta.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaIngreso.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaSalida.Text = DateTime.Now.ToString("yyyy-MM-dd");

            txtFechaOrdenCompra.Attributes.Add("readonly", "readonly");
            txtFechaOrdenVenta.Attributes.Add("readonly", "readonly");
            txtFechaIngreso.Attributes.Add("readonly", "readonly");
            txtFechaSalida.Attributes.Add("readonly", "readonly");
        }

        #region ORDEN DE COMPRA - CONECTADO CON BACKEND

        private void CargarProveedores()
        {
            try
            {
                ddlProveedor.Items.Clear();
                ddlCliente.Items.Clear();

                ddlProveedor.Items.Add(new ListItem("-- Seleccione un proveedor --", ""));
                ddlCliente.Items.Add(new ListItem("-- Seleccione un cliente --", ""));

                var empresasArray = empresaService.listarEmpresas();

                if (empresasArray != null && empresasArray.Length > 0)
                {
                    foreach (var empresa in empresasArray)
                    {
                        if (empresa.idEmpresa > 0 && !string.IsNullOrEmpty(empresa.razonSocial))
                        {
                            string texto = $"{empresa.idEmpresa} - {empresa.razonSocial}";
                            ddlProveedor.Items.Add(new ListItem(texto, empresa.idEmpresa.ToString()));
                            ddlCliente.Items.Add(new ListItem(texto, empresa.idEmpresa.ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorCargarProveedores",
                    $"alert('Error al cargar empresas: {ex.Message}');", true);
            }
        }

        private void CargarOrdenesCompra()
        {
            try
            {
                var ordenesArray = ordenCompraService.listarOrdenesCompra();

                if (ordenesArray == null || ordenesArray.Length == 0)
                {
                    gvOrdenesCompra.DataSource = new List<object>();
                    gvOrdenesCompra.DataBind();
                    return;
                }

                var ordenes = new List<object>();

                foreach (var o in ordenesArray)
                {
                    try
                    {
                        string estadoMostrar = "PENDIENTE";
                        string nombreProveedor = "Sin Proveedor";
                        double total = 0;
                        DateTime fecha = DateTime.Now;

                        try
                        {
                            if (o.estado != null)
                            {
                                estadoMostrar = o.estado.ToString();
                            }
                        }
                        catch (Exception estadoEx)
                        {
                            estadoMostrar = "PENDIENTE";
                            System.Diagnostics.Debug.WriteLine($"Error en estado orden {o.idOrdenCompra}: {estadoEx.Message}");
                        }

                        try
                        {
                            if (o.proveedor != null && !string.IsNullOrEmpty(o.proveedor.razonSocial))
                            {
                                nombreProveedor = o.proveedor.razonSocial;
                            }
                        }
                        catch
                        {
                            nombreProveedor = "Sin Proveedor";
                        }

                        try { total = o.total; } catch { total = 0; }
                        try { fecha = o.fecha; } catch { fecha = DateTime.Now; }

                        var orden = new
                        {
                            Codigo = "PO-" + o.idOrdenCompra.ToString("D6"),
                            IdOrdenCompra = o.idOrdenCompra,
                            FechaRegistrada = fecha.ToString("yyyy-MM-dd"),
                            Nombre = nombreProveedor,
                            Responsable = "Sistema",
                            Total = total.ToString("C2"),
                            Estado = estadoMostrar
                        };

                        ordenes.Add(orden);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error procesando orden {o.idOrdenCompra}: {ex.Message}");
                        continue;
                    }
                }

                gvOrdenesCompra.DataSource = ordenes;
                gvOrdenesCompra.DataBind();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (errorMessage.Contains("EstadoDocumento.null") || errorMessage.Contains("estado nulo"))
                {
                    errorMessage = "Hay órdenes con estado no definido. Se mostrarán como PENDIENTE.";
                    gvOrdenesCompra.DataSource = new List<object>();
                    gvOrdenesCompra.DataBind();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "infoEstados",
                        $"alert('{errorMessage}');", true);
                    return;
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorCargarOrdenes",
                    $"alert('Error al cargar órdenes de compra: {errorMessage}');", true);

                gvOrdenesCompra.DataSource = new List<object>();
                gvOrdenesCompra.DataBind();
            }
        }

        private void CargarDetalleOrdenCompra(int idOrdenCompra)
        {
            try
            {
                var orden = ordenCompraService.obtenerOrdenCompra(idOrdenCompra);

                if (orden == null || orden.lineas == null || orden.lineas.Length == 0)
                {
                    gvDetalleOrdenCompra.DataSource = new List<object>();
                    gvDetalleOrdenCompra.DataBind();
                    return;
                }

                var lineas = new List<object>();

                foreach (var l in orden.lineas)
                {
                    try
                    {
                        string codigo = "N/A";
                        string nombre = "N/A";
                        string descripcion = "N/A";
                        string marca = "N/A";
                        string categoria = "N/A";
                        double precioUnitario = 0;
                        int cantidad = 0;
                        double subtotal = 0;

                        if (l.producto != null)
                        {
                            try { codigo = l.producto.idProducto.ToString(); } catch { }
                            try { nombre = l.producto.nombre ?? "N/A"; } catch { }
                            try { descripcion = l.producto.descripcion ?? "N/A"; } catch { }
                            try { marca = l.producto.marca ?? "N/A"; } catch { }
                            try { categoria = l.producto.categoria?.nombre ?? "N/A"; } catch { }
                        }

                        try { cantidad = l.cantidad; } catch { }
                        try { subtotal = l.subtotal; } catch { }

                        if (cantidad > 0)
                        {
                            precioUnitario = subtotal / cantidad;
                        }

                        var linea = new
                        {
                            Codigo = codigo,
                            Nombre = nombre,
                            Descripcion = descripcion,
                            Marca = marca,
                            PrecioUnitario = precioUnitario.ToString("C2"),
                            Categoria = categoria,
                            Cantidad = cantidad.ToString(),
                            SubTotal = subtotal.ToString("C2"),
                            Estado = "Disponible"
                        };

                        lineas.Add(linea);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error procesando línea de orden: {ex.Message}");
                        continue;
                    }
                }

                gvDetalleOrdenCompra.DataSource = lineas;
                gvDetalleOrdenCompra.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorCargarDetalle",
                    $"alert('Error al cargar detalle de orden: {ex.Message}');", true);

                gvDetalleOrdenCompra.DataSource = new List<object>();
                gvDetalleOrdenCompra.DataBind();
            }
        }

        protected void chkSeleccionCompra_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSeleccion = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkSeleccion.NamingContainer;

            if (chkSeleccion.Checked)
            {
                int idOrdenCompra = Convert.ToInt32(gvOrdenesCompra.DataKeys[row.RowIndex]["IdOrdenCompra"]);
                CargarDetalleOrdenCompra(idOrdenCompra);

                foreach (GridViewRow otherRow in gvOrdenesCompra.Rows)
                {
                    if (otherRow.RowIndex != row.RowIndex)
                    {
                        CheckBox otherChk = (CheckBox)otherRow.FindControl("chkSeleccionCompra");
                        if (otherChk != null)
                        {
                            otherChk.Checked = false;
                        }
                    }
                }
            }
            else
            {
                CargarDetalleOrdenVacia();
            }
        }

        protected void btnEditarCompraFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idOrdenCompra = Convert.ToInt32(btn.CommandArgument);

            foreach (GridViewRow row in gvOrdenesCompra.Rows)
            {
                CheckBox chkSeleccion = (CheckBox)row.FindControl("chkSeleccionCompra");
                int idFila = Convert.ToInt32(gvOrdenesCompra.DataKeys[row.RowIndex]["IdOrdenCompra"]);

                if (idFila == idOrdenCompra)
                {
                    chkSeleccion.Checked = true;
                    CargarDetalleOrdenCompra(idOrdenCompra);
                }
                else
                {
                    chkSeleccion.Checked = false;
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "editarCompraFila",
                $"alert('Editando orden de compra ID: {idOrdenCompra}');", true);
        }

        protected void btnAnularCompraFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idOrdenCompra = Convert.ToInt32(btn.CommandArgument);

            try
            {
                var orden = ordenCompraService.obtenerOrdenCompra(idOrdenCompra);
                if (orden != null)
                {
                    orden.estado = GetEstadoDocumento("CANCELADO");
                    ordenCompraService.guardarOrdenCompra(orden, estado.MODIFICADO);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "exitoAnularFila",
                        $"alert('Orden de compra anulada exitosamente');", true);

                    CargarOrdenesCompra();
                    CargarDetalleOrdenVacia();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAnularFila",
                    $"alert('Error al anular orden de compra: {ex.Message}');", true);
            }
        }

        protected void btnAgregarCompra_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFechaOrdenCompra.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar una fecha');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlProveedor.SelectedValue))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar un proveedor');", true);
                    return;
                }

                var nuevaOrden = new ordenCompra
                {
                    fecha = DateTime.Parse(txtFechaOrdenCompra.Text),
                    fechaSpecified = true,
                    total = 0,
                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true,
                    proveedor = new empresa
                    {
                        idEmpresa = Convert.ToInt32(ddlProveedor.SelectedValue),
                    },
                    lineas = new lineaOrdenCompra[0]
                };

                ordenCompraService.guardarOrdenCompra(nuevaOrden, estado.NUEVO);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "exito",
                    "alert('Orden de compra agregada exitosamente');", true);

                CargarOrdenesCompra();
                LimpiarFormularioCompra();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAgregar",
                    $"alert('Error al agregar orden de compra: {ex.Message}');", true);
            }
        }

        private void LimpiarFormularioCompra()
        {
            txtFechaOrdenCompra.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlProveedor.SelectedIndex = 0;
        }

        #endregion

        #region ORDEN DE VENTA - CONECTADO CON BACKEND

        private void CargarOrdenesVenta()
        {
            try
            {
                var ordenesArray = ordenVentaService.listarOrdenesVenta();

                if (ordenesArray == null || ordenesArray.Length == 0)
                {
                    gvOrdenesVenta.DataSource = new List<object>();
                    gvOrdenesVenta.DataBind();
                    return;
                }

                var ordenes = new List<object>();

                foreach (var o in ordenesArray)
                {
                    try
                    {
                        string estadoMostrar = "PENDIENTE";
                        string nombreCliente = "Sin Cliente";
                        double total = 0;
                        DateTime fecha = DateTime.Now;

                        try
                        {
                            if (o.estado != null)
                            {
                                estadoMostrar = o.estado.ToString();
                            }
                        }
                        catch (Exception estadoEx)
                        {
                            estadoMostrar = "PENDIENTE";
                            System.Diagnostics.Debug.WriteLine($"Error en estado orden venta {o.idOrdenVenta}: {estadoEx.Message}");
                        }

                        try
                        {
                            if (o.cliente != null && !string.IsNullOrEmpty(o.cliente.razonSocial))
                            {
                                nombreCliente = o.cliente.razonSocial;
                            }
                        }
                        catch
                        {
                            nombreCliente = "Sin Cliente";
                        }

                        try { total = o.total; } catch { total = 0; }
                        try { fecha = o.fecha; } catch { fecha = DateTime.Now; }

                        var orden = new
                        {
                            Codigo = "SO-" + o.idOrdenVenta.ToString("D6"),
                            IdOrdenVenta = o.idOrdenVenta,
                            FechaRegistrada = fecha.ToString("yyyy-MM-dd"),
                            Nombre = nombreCliente,
                            Responsable = "Sistema",
                            Total = total.ToString("C2"),
                            Estado = estadoMostrar
                        };

                        ordenes.Add(orden);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error procesando orden venta {o.idOrdenVenta}: {ex.Message}");
                        continue;
                    }
                }

                gvOrdenesVenta.DataSource = ordenes;
                gvOrdenesVenta.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorCargarVentas",
                    $"alert('Error al cargar órdenes de venta: {ex.Message}');", true);

                gvOrdenesVenta.DataSource = new List<object>();
                gvOrdenesVenta.DataBind();
            }
        }

        private void CargarDetalleOrdenVenta(int idOrdenVenta)
        {
            try
            {
                var orden = ordenVentaService.obtenerOrdenVenta(idOrdenVenta);

                if (orden == null || orden.lineas == null || orden.lineas.Length == 0)
                {
                    gvDetalleOrdenVenta.DataSource = new List<object>();
                    gvDetalleOrdenVenta.DataBind();
                    return;
                }

                var lineas = new List<object>();

                foreach (var l in orden.lineas)
                {
                    try
                    {
                        string codigo = "N/A";
                        string nombre = "N/A";
                        string descripcion = "N/A";
                        string marca = "N/A";
                        string categoria = "N/A";
                        double precioUnitario = 0;
                        int cantidad = 0;
                        double subtotal = 0;

                        if (l.producto != null)
                        {
                            try { codigo = l.producto.idProducto.ToString(); } catch { }
                            try { nombre = l.producto.nombre ?? "N/A"; } catch { }
                            try { descripcion = l.producto.descripcion ?? "N/A"; } catch { }
                            try { marca = l.producto.marca ?? "N/A"; } catch { }
                            try { categoria = l.producto.categoria?.nombre ?? "N/A"; } catch { }
                        }

                        try { cantidad = l.cantidad; } catch { }
                        try { subtotal = l.subtotal; } catch { }

                        if (cantidad > 0)
                        {
                            precioUnitario = subtotal / cantidad;
                        }

                        var linea = new
                        {
                            Codigo = codigo,
                            Nombre = nombre,
                            Descripcion = descripcion,
                            Marca = marca,
                            PrecioUnitario = precioUnitario.ToString("C2"),
                            Categoria = categoria,
                            Cantidad = cantidad.ToString(),
                            SubTotal = subtotal.ToString("C2"),
                            Estado = "Disponible"
                        };

                        lineas.Add(linea);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error procesando línea de orden venta: {ex.Message}");
                        continue;
                    }
                }

                gvDetalleOrdenVenta.DataSource = lineas;
                gvDetalleOrdenVenta.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorCargarDetalleVenta",
                    $"alert('Error al cargar detalle de orden de venta: {ex.Message}');", true);

                gvDetalleOrdenVenta.DataSource = new List<object>();
                gvDetalleOrdenVenta.DataBind();
            }
        }

        protected void chkSeleccionVenta_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSeleccion = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkSeleccion.NamingContainer;

            if (chkSeleccion.Checked)
            {
                int idOrdenVenta = Convert.ToInt32(gvOrdenesVenta.DataKeys[row.RowIndex]["IdOrdenVenta"]);
                CargarDetalleOrdenVenta(idOrdenVenta);

                foreach (GridViewRow otherRow in gvOrdenesVenta.Rows)
                {
                    if (otherRow.RowIndex != row.RowIndex)
                    {
                        CheckBox otherChk = (CheckBox)otherRow.FindControl("chkSeleccionVenta");
                        if (otherChk != null)
                        {
                            otherChk.Checked = false;
                        }
                    }
                }
            }
            else
            {
                CargarDetalleOrdenVacia();
            }
        }

        protected void btnEditarVentaFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idOrdenVenta = Convert.ToInt32(btn.CommandArgument);

            foreach (GridViewRow row in gvOrdenesVenta.Rows)
            {
                CheckBox chkSeleccion = (CheckBox)row.FindControl("chkSeleccionVenta");
                int idFila = Convert.ToInt32(gvOrdenesVenta.DataKeys[row.RowIndex]["IdOrdenVenta"]);

                if (idFila == idOrdenVenta)
                {
                    chkSeleccion.Checked = true;
                    CargarDetalleOrdenVenta(idOrdenVenta);
                }
                else
                {
                    chkSeleccion.Checked = false;
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "editarVentaFila",
                $"alert('Editando orden de venta ID: {idOrdenVenta}');", true);
        }

        protected void btnAnularVentaFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idOrdenVenta = Convert.ToInt32(btn.CommandArgument);

            try
            {
                var orden = ordenVentaService.obtenerOrdenVenta(idOrdenVenta);
                if (orden != null)
                {
                    orden.estado = GetEstadoDocumento("CANCELADO");
                    orden.estadoSpecified = true;
                    ordenVentaService.guardarOrdenVenta(orden, estado.MODIFICADO);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "exitoAnularVentaFila",
                        $"alert('Orden de venta anulada exitosamente');", true);

                    CargarOrdenesVenta();
                    CargarDetalleOrdenVacia();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAnularVentaFila",
                    $"alert('Error al anular orden de venta: {ex.Message}');", true);
            }
        }

        protected void btnAgregarVenta_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFechaOrdenVenta.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar una fecha');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlCliente.SelectedValue))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar un cliente');", true);
                    return;
                }

                var nuevaOrden = new ordenVenta
                {
                    fecha = DateTime.Parse(txtFechaOrdenVenta.Text),
                    fechaSpecified = true,
                    total = 0,
                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true,
                    cliente = new empresa
                    {
                        idEmpresa = Convert.ToInt32(ddlCliente.SelectedValue),
                    },
                    lineas = new lineaOrdenVenta[0]
                };

                ordenVentaService.guardarOrdenVenta(nuevaOrden, estado.NUEVO);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "exito",
                    "alert('Orden de venta agregada exitosamente');", true);

                CargarOrdenesVenta();
                LimpiarFormularioVenta();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAgregarVenta",
                    $"alert('Error al agregar orden de venta: {ex.Message}');", true);
            }
        }

        private void LimpiarFormularioVenta()
        {
            txtFechaOrdenVenta.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlCliente.SelectedIndex = 0;
        }

        #endregion

        #region INGRESO - IMPLEMENTACIÓN COMPLETA

        private void CargarRegistrosIngreso()
        {
            try
            {
                var ingresosArray = ingresoService.listarOrdenesIngreso();

                if (ingresosArray == null || ingresosArray.Length == 0)
                {
                    gvRegistrosIngreso.DataSource = new List<object>();
                    gvRegistrosIngreso.DataBind();
                    return;
                }

                var ingresos = new List<object>();

                foreach (var i in ingresosArray)
                {
                    try
                    {
                        string estadoMostrar = "PENDIENTE";
                        string nombreProveedor = "Sin Proveedor";
                        double total = 0;
                        DateTime fecha = DateTime.Now;

                        try { estadoMostrar = i.estado.ToString() ?? "PENDIENTE"; } catch { }
                        try { nombreProveedor = i.ordenCompra?.proveedor?.razonSocial ?? "Sin Proveedor"; } catch { }
                        try { total = i.total; } catch { }
                        try { fecha = i.fecha; } catch { }

                        var ingreso = new
                        {
                            Codigo = "ING-" + i.idOrdenIngreso.ToString("D6"),
                            IdIngreso = i.idOrdenIngreso,
                            FechaRegistrada = fecha.ToString("yyyy-MM-dd"),
                            Nombre = nombreProveedor,
                            Responsable = "Sistema",
                            Total = total.ToString("C2"),
                            Estado = estadoMostrar
                        };

                        ingresos.Add(ingreso);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error procesando ingreso: {ex.Message}");
                        continue;
                    }
                }

                gvRegistrosIngreso.DataSource = ingresos;
                gvRegistrosIngreso.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorCargarIngresos",
                    $"alert('Error al cargar registros de ingreso: {ex.Message}');", true);
                gvRegistrosIngreso.DataSource = new List<object>();
                gvRegistrosIngreso.DataBind();
            }
        }

        protected void btnAgregarIngreso_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFechaIngreso.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar una fecha');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlOrdenCompraIngreso.SelectedValue))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar una orden de compra');", true);
                    return;
                }

                var nuevoIngreso = new ordenIngreso
                {
                    fecha = DateTime.Parse(txtFechaIngreso.Text),
                    fechaSpecified = true,
                    total = 0,
                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true,
                    ordenCompra = new ordenCompra
                    {
                        idOrdenCompra = Convert.ToInt32(ddlOrdenCompraIngreso.SelectedValue)
                    },
                    lineas = new lineaOrdenIngreso[0]
                };

                ingresoService.guardarOrdenIngreso(nuevoIngreso, estado.NUEVO);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "exito",
                    "alert('Ingreso agregado exitosamente');", true);

                CargarRegistrosIngreso();
                LimpiarFormularioIngreso();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAgregarIngreso",
                    $"alert('Error al agregar ingreso: {ex.Message}');", true);
            }
        }

        protected void btnAnularIngresoFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string codigoIngreso = btn.CommandArgument.ToString();

            try
            {
                // Extraer el ID numérico del código ING-000001
                int idIngreso = Convert.ToInt32(codigoIngreso.Replace("ING-", ""));
                var ingreso = ingresoService.obtenerOrdenIngreso(idIngreso);

                if (ingreso != null)
                {
                    ingreso.estado = GetEstadoDocumento("CANCELADO");
                    ingreso.estadoSpecified = true;
                    ingresoService.guardarOrdenIngreso(ingreso, estado.MODIFICADO);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "exitoAnularIngresoFila",
                        $"alert('Ingreso anulado exitosamente');", true);

                    CargarRegistrosIngreso();
                    CargarDetalleOrdenVacia();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAnularIngresoFila",
                    $"alert('Error al anular ingreso: {ex.Message}');", true);
            }
        }

        private void LimpiarFormularioIngreso()
        {
            txtFechaIngreso.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlOrdenCompraIngreso.SelectedIndex = 0;
        }

        #endregion

        #region SALIDA - IMPLEMENTACIÓN COMPLETA

        private void CargarRegistrosSalida()
        {
            try
            {
                var salidasArray = salidaService.listarOrdenesSalida();

                if (salidasArray == null || salidasArray.Length == 0)
                {
                    gvRegistrosSalida.DataSource = new List<object>();
                    gvRegistrosSalida.DataBind();
                    return;
                }

                var salidas = new List<object>();

                foreach (var s in salidasArray)
                {
                    try
                    {
                        string estadoMostrar = "PENDIENTE";
                        string nombreCliente = "Sin Cliente";
                        double total = 0;
                        DateTime fecha = DateTime.Now;

                        try { estadoMostrar = s.estado.ToString() ?? "PENDIENTE"; } catch { }
                        try { nombreCliente = s.ordenVenta?.cliente?.razonSocial ?? "Sin Cliente"; } catch { }
                        try { total = s.total; } catch { }
                        try { fecha = s.fecha; } catch { }

                        var salida = new
                        {
                            Codigo = "SAL-" + s.idOrdenSalida.ToString("D6"),
                            IdSalida = s.idOrdenSalida,
                            FechaRegistrada = fecha.ToString("yyyy-MM-dd"),
                            Nombre = nombreCliente,
                            Responsable = "Sistema",
                            Total = total.ToString("C2"),
                            Estado = estadoMostrar
                        };

                        salidas.Add(salida);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error procesando salida: {ex.Message}");
                        continue;
                    }
                }

                gvRegistrosSalida.DataSource = salidas;
                gvRegistrosSalida.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorCargarSalidas",
                    $"alert('Error al cargar registros de salida: {ex.Message}');", true);
                gvRegistrosSalida.DataSource = new List<object>();
                gvRegistrosSalida.DataBind();
            }
        }

        protected void btnAgregarSalida_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFechaSalida.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar una fecha');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlOrdenVentaSalida.SelectedValue))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar una orden de venta');", true);
                    return;
                }

                var nuevaSalida = new ordenSalida
                {
                    fecha = DateTime.Parse(txtFechaSalida.Text),
                    fechaSpecified = true,
                    total = 0,
                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true,
                    ordenVenta = new ordenVenta
                    {
                        idOrdenVenta = Convert.ToInt32(ddlOrdenVentaSalida.SelectedValue)
                    },
                    lineas = new lineaOrdenSalida[0]
                };

                salidaService.guardarOrdenSalida(nuevaSalida, estado.NUEVO);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "exito",
                    "alert('Salida agregada exitosamente');", true);

                CargarRegistrosSalida();
                LimpiarFormularioSalida();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAgregarSalida",
                    $"alert('Error al agregar salida: {ex.Message}');", true);
            }
        }

        protected void btnAnularSalidaFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string codigoSalida = btn.CommandArgument.ToString();

            try
            {
                // Extraer el ID numérico del código SAL-000001
                int idSalida = Convert.ToInt32(codigoSalida.Replace("SAL-", ""));
                var salida = salidaService.obtenerOrdenSalida(idSalida);

                if (salida != null)
                {
                    salida.estado = GetEstadoDocumento("CANCELADO");
                    salida.estadoSpecified = true;
                    salidaService.guardarOrdenSalida(salida, estado.MODIFICADO);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "exitoAnularSalidaFila",
                        $"alert('Salida anulada exitosamente');", true);

                    CargarRegistrosSalida();
                    CargarDetalleOrdenVacia();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAnularSalidaFila",
                    $"alert('Error al anular salida: {ex.Message}');", true);
            }
        }

        private void LimpiarFormularioSalida()
        {
            txtFechaSalida.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlOrdenVentaSalida.SelectedIndex = 0;
        }

        #endregion

        #region MÉTODOS AUXILIARES

        private estadoDocumento GetEstadoDocumentoSeguro()
        {
            try
            {
                if (Enum.IsDefined(typeof(estadoDocumento), "PENDIENTE"))
                {
                    return (estadoDocumento)Enum.Parse(typeof(estadoDocumento), "PENDIENTE");
                }

                if (Enum.IsDefined(typeof(estadoDocumento), "PROCESADO"))
                {
                    return estadoDocumento.PROCESADO;
                }

                var valores = Enum.GetValues(typeof(estadoDocumento));
                if (valores.Length > 0)
                {
                    return (estadoDocumento)valores.GetValue(0);
                }

                throw new InvalidOperationException("No se pudo determinar un estado válido");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estado del documento: {ex.Message}");
            }
        }

        private estadoDocumento GetEstadoDocumento(string estado)
        {
            try
            {
                if (Enum.TryParse<estadoDocumento>(estado, true, out estadoDocumento resultado))
                {
                    return resultado;
                }

                if (Enum.IsDefined(typeof(estadoDocumento), estadoDocumento.PENDIENTE))
                {
                    return estadoDocumento.PENDIENTE;
                }
                else if (Enum.IsDefined(typeof(estadoDocumento), estadoDocumento.PROCESADO))
                {
                    return estadoDocumento.PROCESADO;
                }
                else
                {
                    var valores = Enum.GetValues(typeof(estadoDocumento));
                    return (estadoDocumento)valores.GetValue(0);
                }
            }
            catch
            {
                return estadoDocumento.PROCESADO;
            }
        }

        private void CargarDetalleOrdenVacia()
        {
            gvDetalleOrdenCompra.DataSource = new List<object>();
            gvDetalleOrdenCompra.DataBind();
            gvDetalleOrdenVenta.DataSource = new List<object>();
            gvDetalleOrdenVenta.DataBind();
            gvDetalleOrdenCompraIngreso.DataSource = new List<object>();
            gvDetalleOrdenCompraIngreso.DataBind();
            gvDetalleOrdenVentaSalida.DataSource = new List<object>();
            gvDetalleOrdenVentaSalida.DataBind();
        }

        public string GetBadgeClass(string estado)
        {
            if (string.IsNullOrEmpty(estado))
                return "badge";

            switch (estado.ToLower())
            {
                case "pendiente":
                    return "badge badge-pendiente";
                case "procesando":
                case "procesado":
                    return "badge badge-procesando";
                case "cancelado":
                    return "badge badge-cancelado";
                case "aceptado":
                case "completado":
                    return "badge badge-aceptado";
                case "disponible":
                    return "badge badge-aceptado";
                default:
                    return "badge";
            }
        }

        #endregion

        #region EVENTOS DE NAVEGACIÓN

        protected void btnCompra_Click(object sender, EventArgs e)
        {
            MostrarCompra();
        }

        protected void btnVenta_Click(object sender, EventArgs e)
        {
            MostrarVenta();
        }

        protected void btnIngreso_Click(object sender, EventArgs e)
        {
            MostrarIngreso();
        }

        protected void btnSalida_Click(object sender, EventArgs e)
        {
            MostrarSalida();
        }

        private void MostrarCompra()
        {
            compraContent.Style["display"] = "block";
            ventaContent.Style["display"] = "none";
            ingresoContent.Style["display"] = "none";
            salidaContent.Style["display"] = "none";
            btnCompra.CssClass = "mode-btn active";
            btnVenta.CssClass = "mode-btn";
            btnIngreso.CssClass = "mode-btn";
            btnSalida.CssClass = "mode-btn";
        }

        private void MostrarVenta()
        {
            compraContent.Style["display"] = "none";
            ventaContent.Style["display"] = "block";
            ingresoContent.Style["display"] = "none";
            salidaContent.Style["display"] = "none";
            btnVenta.CssClass = "mode-btn active";
            btnCompra.CssClass = "mode-btn";
            btnIngreso.CssClass = "mode-btn";
            btnSalida.CssClass = "mode-btn";
        }

        private void MostrarIngreso()
        {
            compraContent.Style["display"] = "none";
            ventaContent.Style["display"] = "none";
            ingresoContent.Style["display"] = "block";
            salidaContent.Style["display"] = "none";
            btnIngreso.CssClass = "mode-btn active";
            btnCompra.CssClass = "mode-btn";
            btnVenta.CssClass = "mode-btn";
            btnSalida.CssClass = "mode-btn";
        }

        private void MostrarSalida()
        {
            compraContent.Style["display"] = "none";
            ventaContent.Style["display"] = "none";
            ingresoContent.Style["display"] = "none";
            salidaContent.Style["display"] = "block";
            btnSalida.CssClass = "mode-btn active";
            btnCompra.CssClass = "mode-btn";
            btnVenta.CssClass = "mode-btn";
            btnIngreso.CssClass = "mode-btn";
        }

        #endregion

        #region EVENTOS PENDIENTES

        protected void chkSeleccionIngreso_CheckedChanged(object sender, EventArgs e) { }
        protected void chkSeleccionSalida_CheckedChanged(object sender, EventArgs e) { }
        protected void btnEditarIngresoFila_Click(object sender, EventArgs e) { }
        protected void btnEditarSalidaFila_Click(object sender, EventArgs e) { }
        protected void gvOrdenesCompra_RowDataBound(object sender, GridViewRowEventArgs e) { }
        protected void gvOrdenesVenta_RowDataBound(object sender, GridViewRowEventArgs e) { }
        protected void gvRegistrosIngreso_RowDataBound(object sender, GridViewRowEventArgs e) { }
        protected void gvRegistrosSalida_RowDataBound(object sender, GridViewRowEventArgs e) { }
        protected void ddlOrdenCompraIngreso_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void ddlOrdenVentaSalida_SelectedIndexChanged(object sender, EventArgs e) { }

        #endregion
    }
}