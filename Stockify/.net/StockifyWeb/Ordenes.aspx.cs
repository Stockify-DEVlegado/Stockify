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

        protected void Page_Load(object sender, EventArgs e)
        {
            ordenCompraService = new OrdenCompraWSClient();
            ordenVentaService = new OrdenVentaWSClient();
            empresaService = new EmpresaWSClient();

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
                        // Manejo robusto del estado - prevenir errores de NULL
                        string estadoMostrar = "PENDIENTE";
                        string nombreProveedor = "Sin Proveedor";
                        double total = 0;
                        DateTime fecha = DateTime.Now;

                        try
                        {
                            // Intentar acceder al estado de forma segura
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
                            // Intentar acceder al proveedor de forma segura
                            if (o.proveedor != null && !string.IsNullOrEmpty(o.proveedor.razonSocial))
                            {
                                nombreProveedor = o.proveedor.razonSocial;
                            }
                        }
                        catch
                        {
                            nombreProveedor = "Sin Proveedor";
                        }

                        try
                        {
                            // Intentar acceder al total de forma segura
                            total = o.total;
                        }
                        catch
                        {
                            total = 0;
                        }

                        try
                        {
                            // Intentar acceder a la fecha de forma segura
                            fecha = o.fecha;
                        }
                        catch
                        {
                            fecha = DateTime.Now;
                        }

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
                        // Si hay error con una orden específica, la omitimos pero continuamos con las demás
                        System.Diagnostics.Debug.WriteLine($"Error procesando orden {o.idOrdenCompra}: {ex.Message}");
                        continue;
                    }
                }

                gvOrdenesCompra.DataSource = ordenes;
                gvOrdenesCompra.DataBind();
            }
            catch (Exception ex)
            {
                // Manejo específico del error de estados NULL
                string errorMessage = ex.Message;
                if (errorMessage.Contains("EstadoDocumento.null") || errorMessage.Contains("estado nulo"))
                {
                    errorMessage = "Hay órdenes con estado no definido. Se mostrarán como PENDIENTE.";
                    // En lugar de mostrar error, cargar lista vacía y mensaje informativo
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

                        // Acceso seguro a las propiedades del producto
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

                // Crear la nueva orden con estado explícito PROCESANDO
                var nuevaOrden = new ordenCompra
                {
                    fecha = DateTime.Parse(txtFechaOrdenCompra.Text),
                    fechaSpecified = true, // IMPORTANTE: Especificar que la fecha tiene valor
                    total = 0,

                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true, // CRÍTICO: Esto indica que el estado tiene un valor válido
                    proveedor = new empresa
                    {
                        idEmpresa = Convert.ToInt32(ddlProveedor.SelectedValue),

                    },
                    lineas = new lineaOrdenCompra[0]
                };

                // Guardar con estado NUEVO
                ordenCompraService.guardarOrdenCompra(nuevaOrden, estado.NUEVO);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "exito",
                    "alert('Orden de compra agregada exitosamente');", true);

                CargarOrdenesCompra();
                LimpiarFormularioCompra();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAgregar",
                    $"alert('Error al agregar orden de compra: {ex.Message}\\n\\nDetalle: {ex.StackTrace}');", true);
            }
        }

        protected void btnAnularCompra_Click(object sender, EventArgs e)
        {
            try
            {
                int idOrdenSeleccionada = -1;
                foreach (GridViewRow row in gvOrdenesCompra.Rows)
                {
                    CheckBox chkSeleccion = (CheckBox)row.FindControl("chkSeleccionCompra");
                    if (chkSeleccion != null && chkSeleccion.Checked)
                    {
                        idOrdenSeleccionada = Convert.ToInt32(gvOrdenesCompra.DataKeys[row.RowIndex]["IdOrdenCompra"]);
                        break;
                    }
                }

                if (idOrdenSeleccionada == -1)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar una orden de compra para anular');", true);
                    return;
                }

                var orden = ordenCompraService.obtenerOrdenCompra(idOrdenSeleccionada);

                if (orden == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
                        "alert('No se encontró la orden seleccionada');", true);
                    return;
                }

                // Usar el método seguro para establecer el estado
                orden.estado = GetEstadoDocumento("CANCELADO");
                ordenCompraService.guardarOrdenCompra(orden, estado.MODIFICADO);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "exito",
                    "alert('Orden de compra anulada exitosamente');", true);

                CargarOrdenesCompra();
                CargarDetalleOrdenVacia();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAnular",
                    $"alert('Error al anular orden de compra: {ex.Message}');", true);
            }
        }

        protected void btnViewCompra_Click(object sender, EventArgs e)
        {
            if (fileDocumentoCompra.HasFile)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "viewCompra",
                    $"alert('Archivo seleccionado: {fileDocumentoCompra.FileName}');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "viewCompra",
                    "alert('No hay documento adjunto para visualizar');", true);
            }
        }

        private void LimpiarFormularioCompra()
        {
            txtFechaOrdenCompra.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlProveedor.SelectedIndex = 0;
        }

        #endregion

        #region ORDEN DE VENTA - CONECTADO CON BACKEND (COMPLETAMENTE FUNCIONAL)

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
                        // Manejo robusto del estado - prevenir errores de NULL
                        string estadoMostrar = "PENDIENTE";
                        string nombreCliente = "Sin Cliente";
                        double total = 0;
                        DateTime fecha = DateTime.Now;

                        try
                        {
                            // Intentar acceder al estado de forma segura
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
                            // Intentar acceder al cliente de forma segura
                            if (o.cliente != null && !string.IsNullOrEmpty(o.cliente.razonSocial))
                            {
                                nombreCliente = o.cliente.razonSocial;
                            }
                        }
                        catch
                        {
                            nombreCliente = "Sin Cliente";
                        }

                        try
                        {
                            // Intentar acceder al total de forma segura
                            total = o.total;
                        }
                        catch
                        {
                            total = 0;
                        }

                        try
                        {
                            // Intentar acceder a la fecha de forma segura
                            fecha = o.fecha;
                        }
                        catch
                        {
                            fecha = DateTime.Now;
                        }

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
                        // Si hay error con una orden específica, la omitimos pero continuamos con las demás
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

                        // Acceso seguro a las propiedades del producto
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

                // Crear la nueva orden de venta con estado explícito PENDIENTE
                var nuevaOrden = new ordenVenta
                {
                    fecha = DateTime.Parse(txtFechaOrdenVenta.Text),
                    fechaSpecified = true, // IMPORTANTE: Especificar que la fecha tiene valor
                    total = 0,

                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true, // CRÍTICO: Esto indica que el estado tiene un valor válido
                    cliente = new empresa
                    {
                        idEmpresa = Convert.ToInt32(ddlCliente.SelectedValue),

                    },
                    lineas = new lineaOrdenVenta[0]
                };

                // Guardar con estado NUEVO
                ordenVentaService.guardarOrdenVenta(nuevaOrden, estado.NUEVO);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "exito",
                    "alert('Orden de venta agregada exitosamente');", true);

                CargarOrdenesVenta();
                LimpiarFormularioVenta();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAgregarVenta",
                    $"alert('Error al agregar orden de venta: {ex.Message}\\n\\nDetalle: {ex.StackTrace}');", true);
            }
        }

        protected void btnAnularVenta_Click(object sender, EventArgs e)
        {
            try
            {
                int idOrdenSeleccionada = -1;

                foreach (GridViewRow row in gvOrdenesVenta.Rows)
                {
                    CheckBox chkSeleccion = (CheckBox)row.FindControl("chkSeleccionVenta");
                    if (chkSeleccion != null && chkSeleccion.Checked)
                    {
                        idOrdenSeleccionada = Convert.ToInt32(gvOrdenesVenta.DataKeys[row.RowIndex]["IdOrdenVenta"]);
                        break;
                    }
                }

                if (idOrdenSeleccionada == -1)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "validacion",
                        "alert('Debe seleccionar una orden de venta para anular');", true);
                    return;
                }

                var orden = ordenVentaService.obtenerOrdenVenta(idOrdenSeleccionada);

                if (orden == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
                        "alert('No se encontró la orden seleccionada');", true);
                    return;
                }

                orden.estado = GetEstadoDocumento("CANCELADO");

                ordenVentaService.guardarOrdenVenta(orden, estado.MODIFICADO);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "exito",
                    "alert('Orden de venta anulada exitosamente');", true);

                CargarOrdenesVenta();
                CargarDetalleOrdenVacia();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorAnularVenta",
                    $"alert('Error al anular orden de venta: {ex.Message}');", true);
            }
        }

        protected void btnViewVenta_Click(object sender, EventArgs e)
        {
            if (fileDocumentoVenta.HasFile)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "viewVenta",
                    $"alert('Archivo seleccionado: {fileDocumentoVenta.FileName}');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "viewVenta",
                    "alert('No hay documento adjunto para visualizar');", true);
            }
        }

        private void LimpiarFormularioVenta()
        {
            txtFechaOrdenVenta.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlCliente.SelectedIndex = 0;
        }

        #endregion

        #region MÉTODOS AUXILIARES

        private estadoDocumento GetEstadoDocumentoSeguro()
        {
            try
            {
                // Intentar con PENDIENTE primero
                if (Enum.IsDefined(typeof(estadoDocumento), "PENDIENTE"))
                {
                    return (estadoDocumento)Enum.Parse(typeof(estadoDocumento), "PENDIENTE");
                }

                // Si no existe PENDIENTE, intentar con PROCESADO
                if (Enum.IsDefined(typeof(estadoDocumento), "PROCESADO"))
                {
                    return estadoDocumento.PROCESADO;
                }

                // Último recurso: usar reflection para obtener el primer valor del enum
                var valores = Enum.GetValues(typeof(estadoDocumento));
                if (valores.Length > 0)
                {
                    return (estadoDocumento)valores.GetValue(0);
                }

                // Si todo falla, lanzar excepción
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
                // Intentar parsear el estado
                if (Enum.TryParse<estadoDocumento>(estado, true, out estadoDocumento resultado))
                {
                    return resultado;
                }

                // Si falla, intentar con los valores comunes
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
                    // Último recurso: usar el primer valor del enum
                    var valores = Enum.GetValues(typeof(estadoDocumento));
                    return (estadoDocumento)valores.GetValue(0);
                }
            }
            catch
            {
                // Fallback seguro
                return estadoDocumento.PROCESADO;
            }
        }

        private void CargarRegistrosIngreso()
        {
            try
            {
                var registros = new List<object>
                {
                    new {
                        Codigo = "ING-2025-001",
                        FechaRegistrada = "2025-01-01",
                        Nombre = "BHAVANI SALES CORPORATION",
                        Responsable = "Diego Alvarez Castillo",
                        Total = "7500 $",
                        Estado = "Procesando"
                    }
                };

                gvRegistrosIngreso.DataSource = registros;
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

        private void CargarRegistrosSalida()
        {
            try
            {
                var registros = new List<object>
                {
                    new {
                        Codigo = "SAL-2025-001",
                        FechaRegistrada = "2025-01-03",
                        Nombre = "BHAVANI SALES CORPORATION",
                        Responsable = "Alonso Chipana Cuellar",
                        Total = "10000 $",
                        Estado = "Aceptado"
                    }
                };

                gvRegistrosSalida.DataSource = registros;
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
        protected void btnAgregarIngreso_Click(object sender, EventArgs e) { }
        protected void btnAnularIngreso_Click(object sender, EventArgs e) { }
        protected void btnAgregarSalida_Click(object sender, EventArgs e) { }
        protected void btnAnularSalida_Click(object sender, EventArgs e) { }
        protected void ddlOrdenCompraIngreso_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void ddlOrdenVentaSalida_SelectedIndexChanged(object sender, EventArgs e) { }

        #endregion
    }
}