using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Ordenes : System.Web.UI.Page
    {
        // Enum para tipos de orden
        private enum TipoOrden { Compra, Venta, Ingreso, Salida }

        // Servicios
        private OrdenCompraWSClient ordenCompraService;
        private OrdenVentaWSClient ordenVentaService;
        private EmpresaWSClient empresaService;
        private OrdenIngresoWSClient ingresoService;
        private OrdenSalidaWSClient salidaService;
        private UsuarioWSClient usuarioService;

        // Configuración de controles por tipo de orden
        private class ConfiguracionOrden
        {
            public GridView GridPrincipal { get; set; }
            public GridView GridDetalle { get; set; }
            public TextBox TxtFecha { get; set; }
            public FileUpload FileUpload { get; set; }
            public DropDownList DdlPrincipal { get; set; }
            public DropDownList DdlResponsable { get; set; }
            public HtmlGenericControl Contenedor { get; set; }
            public Button BtnModo { get; set; }
        }

        private Dictionary<TipoOrden, ConfiguracionOrden> configuraciones;

        protected void Page_Load(object sender, EventArgs e)
        {
            InicializarServicios();
            InicializarConfiguraciones();

            if (!IsPostBack)
            {
                MostrarSeccion(TipoOrden.Compra);
                CargarDatosIniciales();
            }
        }

        #region INICIALIZACIÓN

        private void InicializarServicios()
        {
            ordenCompraService = new OrdenCompraWSClient();
            ordenVentaService = new OrdenVentaWSClient();
            empresaService = new EmpresaWSClient();
            ingresoService = new OrdenIngresoWSClient();
            salidaService = new OrdenSalidaWSClient();
            usuarioService = new UsuarioWSClient();
        }

        private void InicializarConfiguraciones()
        {
            configuraciones = new Dictionary<TipoOrden, ConfiguracionOrden>
            {
                [TipoOrden.Compra] = new ConfiguracionOrden
                {
                    GridPrincipal = gvOrdenesCompra,
                    GridDetalle = gvDetalleOrdenCompra,
                    TxtFecha = txtFechaOrdenCompra,
                    FileUpload = fileDocumentoCompra,
                    DdlPrincipal = ddlProveedor,
                    Contenedor = compraContent,
                    BtnModo = btnCompra
                },
                [TipoOrden.Venta] = new ConfiguracionOrden
                {
                    GridPrincipal = gvOrdenesVenta,
                    GridDetalle = gvDetalleOrdenVenta,
                    TxtFecha = txtFechaOrdenVenta,
                    FileUpload = fileDocumentoVenta,
                    DdlPrincipal = ddlCliente,
                    Contenedor = ventaContent,
                    BtnModo = btnVenta
                },
                [TipoOrden.Ingreso] = new ConfiguracionOrden
                {
                    GridPrincipal = gvRegistrosIngreso,
                    GridDetalle = gvDetalleOrdenCompraIngreso,
                    TxtFecha = txtFechaIngreso,
                    FileUpload = fileDocumentoIngreso,
                    DdlPrincipal = ddlOrdenCompraIngreso,
                    DdlResponsable = ddlResponsableIngreso,
                    Contenedor = ingresoContent,
                    BtnModo = btnIngreso
                },
                [TipoOrden.Salida] = new ConfiguracionOrden
                {
                    GridPrincipal = gvRegistrosSalida,
                    GridDetalle = gvDetalleOrdenVentaSalida,
                    TxtFecha = txtFechaSalida,
                    FileUpload = fileDocumentoSalida,
                    DdlPrincipal = ddlOrdenVentaSalida,
                    DdlResponsable = ddlResponsableSalida,
                    Contenedor = salidaContent,
                    BtnModo = btnSalida
                }
            };
        }

        private void CargarDatosIniciales()
        {
            CargarProveedoresYClientes();
            CargarUsuariosParaIngresoYSalida();

            // Cargar todas las órdenes
            CargarOrdenes(TipoOrden.Compra);
            CargarOrdenes(TipoOrden.Venta);
            CargarOrdenes(TipoOrden.Ingreso);
            CargarOrdenes(TipoOrden.Salida);

            CargarOrdenesCompraParaIngreso();
            CargarOrdenesVentaParaSalida();
            CargarDetalleOrdenVacia();

            // Configurar fechas
            foreach (var config in configuraciones.Values)
            {
                config.TxtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
                config.TxtFecha.Attributes.Add("readonly", "readonly");
            }
        }

        #endregion

        #region CARGA DE DATOS COMUNES

        private void CargarProveedoresYClientes()
        {
            try
            {
                ddlProveedor.Items.Clear();
                ddlCliente.Items.Clear();
                ddlProveedor.Items.Add(new ListItem("-- Seleccione un proveedor --", ""));
                ddlCliente.Items.Add(new ListItem("-- Seleccione un cliente --", ""));

                var empresasArray = empresaService.listarEmpresas();
                if (empresasArray?.Length > 0)
                {
                    foreach (var empresa in empresasArray.Where(e => e.idEmpresa > 0 && !string.IsNullOrEmpty(e.razonSocial)))
                    {
                        string texto = $"{empresa.idEmpresa} - {empresa.razonSocial}";
                        var item = new ListItem(texto, empresa.idEmpresa.ToString());
                        ddlProveedor.Items.Add(item);
                        ddlCliente.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorCargarProveedores", $"Error al cargar empresas: {ex.Message}");
            }
        }

        private void CargarUsuariosParaIngresoYSalida()
        {
            try
            {
                ddlResponsableIngreso.Items.Clear();
                ddlResponsableSalida.Items.Clear();
                ddlResponsableIngreso.Items.Add(new ListItem("-- Seleccione un responsable --", ""));
                ddlResponsableSalida.Items.Add(new ListItem("-- Seleccione un responsable --", ""));

                var usuariosArray = usuarioService.listarUsuarios();
                if (usuariosArray?.Length > 0)
                {
                    foreach (var usuario in usuariosArray.Where(u => u.idUsuario > 0))
                    {
                        string texto = ObtenerTextoUsuario(usuario);
                        var item = new ListItem(texto, usuario.idUsuario.ToString());
                        ddlResponsableIngreso.Items.Add(item);
                        ddlResponsableSalida.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorCargarUsuarios", $"Error al cargar usuarios: {ex.Message}");
            }
        }

        private string ObtenerTextoUsuario(usuario usuario)
        {
            string nombre = usuario.nombres ?? "";
            string apellido = usuario.apellidos ?? "";
            string email = usuario.email ?? "";

            string texto = $"{nombre} {apellido}".Trim();
            if (string.IsNullOrEmpty(texto))
                texto = $"Usuario {usuario.idUsuario}";

            if (!string.IsNullOrEmpty(email))
                texto += $" - {email}";

            return texto;
        }

        private void CargarOrdenesCompraParaIngreso()
        {
            try
            {
                ddlOrdenCompraIngreso.Items.Clear();
                ddlOrdenCompraIngreso.Items.Add(new ListItem("-- Seleccione una orden de compra --", ""));

                var ordenesArray = ordenCompraService.listarOrdenesCompra();
                if (ordenesArray?.Length > 0)
                {
                    foreach (var orden in ordenesArray.Where(o =>
                        o.estado == estadoDocumento.PENDIENTE || o.estado == estadoDocumento.PROCESADO))
                    {
                        string nombreProveedor = orden.proveedor?.razonSocial ?? "Sin Proveedor";
                        string texto = $"PO-{orden.idOrdenCompra:D6} - {nombreProveedor}";
                        ddlOrdenCompraIngreso.Items.Add(new ListItem(texto, orden.idOrdenCompra.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorCargarOrdenesCompra", $"Error al cargar órdenes de compra: {ex.Message}");
            }
        }

        private void CargarOrdenesVentaParaSalida()
        {
            try
            {
                ddlOrdenVentaSalida.Items.Clear();
                ddlOrdenVentaSalida.Items.Add(new ListItem("-- Seleccione una orden de venta --", ""));

                var ordenesArray = ordenVentaService.listarOrdenesVenta();
                if (ordenesArray?.Length > 0)
                {
                    foreach (var orden in ordenesArray.Where(o =>
                        o.estado == estadoDocumento.PENDIENTE || o.estado == estadoDocumento.PROCESADO))
                    {
                        string nombreCliente = orden.cliente?.razonSocial ?? "Sin Cliente";
                        string texto = $"SO-{orden.idOrdenVenta:D6} - {nombreCliente}";
                        ddlOrdenVentaSalida.Items.Add(new ListItem(texto, orden.idOrdenVenta.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorCargarOrdenesVenta", $"Error al cargar órdenes de venta: {ex.Message}");
            }
        }

        #endregion

        #region CARGA DE ÓRDENES UNIFICADA

        private void CargarOrdenes(TipoOrden tipo)
        {
            try
            {
                var config = configuraciones[tipo];
                var ordenes = new List<object>();

                switch (tipo)
                {
                    case TipoOrden.Compra:
                        ordenes = ObtenerOrdenesCompra();
                        break;
                    case TipoOrden.Venta:
                        ordenes = ObtenerOrdenesVenta();
                        break;
                    case TipoOrden.Ingreso:
                        ordenes = ObtenerOrdenesIngreso();
                        break;
                    case TipoOrden.Salida:
                        ordenes = ObtenerOrdenesSalida();
                        break;
                }

                config.GridPrincipal.DataSource = ordenes;
                config.GridPrincipal.DataBind();
            }
            catch (Exception ex)
            {
                MostrarAlerta($"errorCargar{tipo}", $"Error al cargar órdenes de {tipo}: {ex.Message}");
                configuraciones[tipo].GridPrincipal.DataSource = new List<object>();
                configuraciones[tipo].GridPrincipal.DataBind();
            }
        }

        private List<object> ObtenerOrdenesCompra()
        {
            var ordenesArray = ordenCompraService.listarOrdenesCompra();
            if (ordenesArray == null || ordenesArray.Length == 0) return new List<object>();

            return ordenesArray.Select(o => new
            {
                Codigo = "PO-" + o.idOrdenCompra.ToString("D6"),
                IdOrdenCompra = o.idOrdenCompra,
                FechaRegistrada = (o.fecha == default(DateTime) ? DateTime.Now : o.fecha).ToString("yyyy-MM-dd"),
                Nombre = o.proveedor?.razonSocial ?? "Sin Proveedor",
                Total = o.total.ToString("C2"),
                Estado = o.estado.ToString() ?? "PENDIENTE"
            }).ToList<object>();
        }

        private List<object> ObtenerOrdenesVenta()
        {
            var ordenesArray = ordenVentaService.listarOrdenesVenta();
            if (ordenesArray == null || ordenesArray.Length == 0) return new List<object>();

            return ordenesArray.Select(o => new
            {
                Codigo = "SO-" + o.idOrdenVenta.ToString("D6"),
                IdOrdenVenta = o.idOrdenVenta,
                FechaRegistrada = (o.fecha == default(DateTime) ? DateTime.Now : o.fecha).ToString("yyyy-MM-dd"),
                Nombre = o.cliente?.razonSocial ?? "Sin Cliente",
                Total = o.total.ToString("C2"),
                Estado = o.estado.ToString() ?? "PENDIENTE"
            }).ToList<object>();
        }

        private List<object> ObtenerOrdenesIngreso()
        {
            var ingresosArray = ingresoService.listarOrdenesIngreso();
            if (ingresosArray == null || ingresosArray.Length == 0) return new List<object>();

            return ingresosArray.Select(i => new
            {
                Codigo = "ING-" + i.idOrdenIngreso.ToString("D6"),
                IdIngreso = i.idOrdenIngreso,
                FechaRegistrada = (i.fecha == default(DateTime) ? DateTime.Now : i.fecha).ToString("yyyy-MM-dd"),
                Nombre = i.ordenCompra?.proveedor?.razonSocial ?? "Sin Proveedor",
                Responsable = ObtenerNombreResponsable(i),
                Total = i.total.ToString("C2"),
                Estado = i.estado.ToString() ?? "PENDIENTE"
            }).ToList<object>();
        }

        private List<object> ObtenerOrdenesSalida()
        {
            var salidasArray = salidaService.listarOrdenesSalida();
            if (salidasArray == null || salidasArray.Length == 0) return new List<object>();

            return salidasArray.Select(s => new
            {
                Codigo = "SAL-" + s.idOrdenSalida.ToString("D6"),
                IdSalida = s.idOrdenSalida,
                FechaRegistrada = (s.fecha == default(DateTime) ? DateTime.Now : s.fecha).ToString("yyyy-MM-dd"),
                Nombre = s.ordenVenta?.cliente?.razonSocial ?? "Sin Cliente",
                Responsable = ObtenerNombreResponsable(s),
                Total = s.total.ToString("C2"),
                Estado = s.estado.ToString() ?? "PENDIENTE"
            }).ToList<object>();
        }

        #endregion

        #region CARGA DE DETALLES UNIFICADA

        private void CargarDetalle(TipoOrden tipo, int id)
        {
            try
            {
                var config = configuraciones[tipo];
                var lineas = new List<object>();

                switch (tipo)
                {
                    case TipoOrden.Compra:
                        lineas = ObtenerDetalleCompra(id);
                        break;
                    case TipoOrden.Venta:
                        lineas = ObtenerDetalleVenta(id);
                        break;
                    case TipoOrden.Ingreso:
                        lineas = ObtenerDetalleIngreso(id);
                        break;
                    case TipoOrden.Salida:
                        lineas = ObtenerDetalleSalida(id);
                        break;
                }

                config.GridDetalle.DataSource = lineas;
                config.GridDetalle.DataBind();
            }
            catch (Exception ex)
            {
                MostrarAlerta($"errorCargarDetalle{tipo}", $"Error al cargar detalle: {ex.Message}");
                configuraciones[tipo].GridDetalle.DataSource = new List<object>();
                configuraciones[tipo].GridDetalle.DataBind();
            }
        }

        private List<object> ObtenerDetalleCompra(int id)
        {
            var orden = ordenCompraService.obtenerOrdenCompra(id);
            return ProcesarLineas(orden?.lineas);
        }

        private List<object> ObtenerDetalleVenta(int id)
        {
            var orden = ordenVentaService.obtenerOrdenVenta(id);
            return ProcesarLineas(orden?.lineas);
        }

        private List<object> ObtenerDetalleIngreso(int id)
        {
            var ingreso = ingresoService.obtenerOrdenIngreso(id);
            return ProcesarLineas(ingreso?.lineas);
        }

        private List<object> ObtenerDetalleSalida(int id)
        {
            var salida = salidaService.obtenerOrdenSalida(id);
            return ProcesarLineas(salida?.lineas);
        }

        private List<object> ProcesarLineas(Array lineasArray)
        {
            var lineas = new List<object>();
            if (lineasArray == null || lineasArray.Length == 0) return lineas;

            foreach (var linea in lineasArray)
            {
                try
                {
                    var propProducto = linea.GetType().GetProperty("producto");
                    var propCantidad = linea.GetType().GetProperty("cantidad");
                    var propSubtotal = linea.GetType().GetProperty("subtotal");

                    var producto = propProducto?.GetValue(linea);
                    int cantidad = (int)(propCantidad?.GetValue(linea) ?? 0);
                    double subtotal = (double)(propSubtotal?.GetValue(linea) ?? 0);

                    lineas.Add(new
                    {
                        Codigo = ObtenerPropiedadProducto(producto, "idProducto", "N/A"),
                        Nombre = ObtenerPropiedadProducto(producto, "nombre", "N/A"),
                        Descripcion = ObtenerPropiedadProducto(producto, "descripcion", "N/A"),
                        Marca = ObtenerPropiedadProducto(producto, "marca", "N/A"),
                        PrecioUnitario = (cantidad > 0 ? subtotal / cantidad : 0).ToString("C2"),
                        Categoria = ObtenerCategoriaProducto(producto),
                        Cantidad = cantidad.ToString(),
                        SubTotal = subtotal.ToString("C2"),
                        Estado = "Disponible"
                    });
                }
                catch { continue; }
            }

            return lineas;
        }

        private string ObtenerPropiedadProducto(object producto, string propiedad, string valorPorDefecto)
        {
            if (producto == null) return valorPorDefecto;

            try
            {
                var prop = producto.GetType().GetProperty(propiedad);
                var valor = prop?.GetValue(producto);

                if (propiedad == "idProducto")
                    return valor?.ToString() ?? valorPorDefecto;

                return valor as string ?? valorPorDefecto;
            }
            catch
            {
                return valorPorDefecto;
            }
        }

        private string ObtenerCategoriaProducto(object producto)
        {
            if (producto == null) return "N/A";

            try
            {
                var propCategoria = producto.GetType().GetProperty("categoria");
                var categoria = propCategoria?.GetValue(producto);
                if (categoria == null) return "N/A";

                var propNombre = categoria.GetType().GetProperty("nombre");
                return propNombre?.GetValue(categoria) as string ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        private void CargarDetalleOrdenVacia()
        {
            foreach (var config in configuraciones.Values)
            {
                config.GridDetalle.DataSource = new List<object>();
                config.GridDetalle.DataBind();
            }
        }

        #endregion

        #region EVENTOS CHECKBOX UNIFICADOS

        protected void chkSeleccionCompra_CheckedChanged(object sender, EventArgs e)
            => ManejarSeleccion(sender, TipoOrden.Compra, "IdOrdenCompra", "chkSeleccionCompra");

        protected void chkSeleccionVenta_CheckedChanged(object sender, EventArgs e)
            => ManejarSeleccion(sender, TipoOrden.Venta, "IdOrdenVenta", "chkSeleccionVenta");

        protected void chkSeleccionIngreso_CheckedChanged(object sender, EventArgs e)
            => ManejarSeleccion(sender, TipoOrden.Ingreso, "IdIngreso", "chkSeleccionIngreso");

        protected void chkSeleccionSalida_CheckedChanged(object sender, EventArgs e)
            => ManejarSeleccion(sender, TipoOrden.Salida, "IdSalida", "chkSeleccionSalida");

        private void ManejarSeleccion(object sender, TipoOrden tipo, string keyName, string checkboxName)
        {
            CheckBox chkSeleccion = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkSeleccion.NamingContainer;
            var config = configuraciones[tipo];

            if (chkSeleccion.Checked)
            {
                int id = Convert.ToInt32(config.GridPrincipal.DataKeys[row.RowIndex][keyName]);
                CargarDetalle(tipo, id);

                // Desmarcar otros checkboxes
                foreach (GridViewRow otherRow in config.GridPrincipal.Rows)
                {
                    if (otherRow.RowIndex != row.RowIndex)
                    {
                        CheckBox otherChk = (CheckBox)otherRow.FindControl(checkboxName);
                        if (otherChk != null) otherChk.Checked = false;
                    }
                }
            }
            else
            {
                CargarDetalleOrdenVacia();
            }
        }

        #endregion

        #region EVENTOS EDITAR UNIFICADOS

        protected void btnEditarCompraFila_Click(object sender, EventArgs e)
            => ManejarEdicion(sender, TipoOrden.Compra, "IdOrdenCompra", "chkSeleccionCompra");

        protected void btnEditarVentaFila_Click(object sender, EventArgs e)
            => ManejarEdicion(sender, TipoOrden.Venta, "IdOrdenVenta", "chkSeleccionVenta");

        protected void btnEditarIngresoFila_Click(object sender, EventArgs e)
            => ManejarEdicion(sender, TipoOrden.Ingreso, "IdIngreso", "chkSeleccionIngreso");

        protected void btnEditarSalidaFila_Click(object sender, EventArgs e)
            => ManejarEdicion(sender, TipoOrden.Salida, "IdSalida", "chkSeleccionSalida");

        private void ManejarEdicion(object sender, TipoOrden tipo, string keyName, string checkboxName)
        {
            Button btn = (Button)sender;
            int id = Convert.ToInt32(btn.CommandArgument);
            var config = configuraciones[tipo];

            foreach (GridViewRow row in config.GridPrincipal.Rows)
            {
                CheckBox chkSeleccion = (CheckBox)row.FindControl(checkboxName);
                int idFila = Convert.ToInt32(config.GridPrincipal.DataKeys[row.RowIndex][keyName]);

                if (idFila == id)
                {
                    chkSeleccion.Checked = true;
                    CargarDetalle(tipo, id);
                }
                else
                {
                    chkSeleccion.Checked = false;
                }
            }
        }

        #endregion

        #region EVENTOS ANULAR UNIFICADOS

        protected void btnAnularCompraFila_Click(object sender, EventArgs e)
            => AnularOrden(sender, TipoOrden.Compra);

        protected void btnAnularVentaFila_Click(object sender, EventArgs e)
            => AnularOrden(sender, TipoOrden.Venta);

        protected void btnAnularIngresoFila_Click(object sender, EventArgs e)
            => AnularOrden(sender, TipoOrden.Ingreso);

        protected void btnAnularSalidaFila_Click(object sender, EventArgs e)
            => AnularOrden(sender, TipoOrden.Salida);

        private void AnularOrden(object sender, TipoOrden tipo)
        {
            Button btn = (Button)sender;
            int id = Convert.ToInt32(btn.CommandArgument);

            try
            {
                switch (tipo)
                {
                    case TipoOrden.Compra:
                        AnularOrdenCompra(id);
                        break;
                    case TipoOrden.Venta:
                        AnularOrdenVenta(id);
                        break;
                    case TipoOrden.Ingreso:
                        AnularOrdenIngreso(id);
                        break;
                    case TipoOrden.Salida:
                        AnularOrdenSalida(id);
                        break;
                }

                MostrarAlerta($"exitoAnular{tipo}", $"Orden de {tipo} anulada exitosamente");
                CargarOrdenes(tipo);
                CargarDetalleOrdenVacia();
            }
            catch (Exception ex)
            {
                MostrarAlerta($"errorAnular{tipo}", $"Error al anular orden: {ex.Message}");
            }
        }

        private void AnularOrdenCompra(int id)
        {
            var orden = ordenCompraService.obtenerOrdenCompra(id);
            if (orden != null)
            {
                orden.estado = estadoDocumento.CANCELADO;
                orden.estadoSpecified = true;
                ordenCompraService.guardarOrdenCompra(orden, estado.MODIFICADO);
            }
        }

        private void AnularOrdenVenta(int id)
        {
            var orden = ordenVentaService.obtenerOrdenVenta(id);
            if (orden != null)
            {
                orden.estado = estadoDocumento.CANCELADO;
                orden.estadoSpecified = true;
                ordenVentaService.guardarOrdenVenta(orden, estado.MODIFICADO);
            }
        }

        private void AnularOrdenIngreso(int id)
        {
            var ingreso = ingresoService.obtenerOrdenIngreso(id);
            if (ingreso != null)
            {
                ingreso.estado = estadoDocumento.CANCELADO;
                ingreso.estadoSpecified = true;
                ingresoService.guardarOrdenIngreso(ingreso, estado.MODIFICADO);
            }
        }

        private void AnularOrdenSalida(int id)
        {
            var salida = salidaService.obtenerOrdenSalida(id);
            if (salida != null)
            {
                salida.estado = estadoDocumento.CANCELADO;
                salida.estadoSpecified = true;
                salidaService.guardarOrdenSalida(salida, estado.MODIFICADO);
            }
        }

        #endregion

        #region EVENTOS AGREGAR

        protected void btnAgregarCompra_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario(txtFechaOrdenCompra, ddlProveedor, null)) return;

            try
            {
                ManejarArchivoAdjunto(fileDocumentoCompra, "Orden de Compra");

                var nuevaOrden = new ordenCompra
                {
                    fecha = DateTime.Parse(txtFechaOrdenCompra.Text),
                    fechaSpecified = true,
                    total = 0,
                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true,
                    proveedor = new empresa { idEmpresa = Convert.ToInt32(ddlProveedor.SelectedValue) },
                    lineas = new lineaOrdenCompra[0]
                };

                ordenCompraService.guardarOrdenCompra(nuevaOrden, estado.NUEVO);
                MostrarAlerta("exito", "Orden de compra agregada exitosamente");
                CargarOrdenes(TipoOrden.Compra);
                LimpiarFormulario(TipoOrden.Compra);
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorAgregar", $"Error al agregar orden de compra: {ex.Message}");
            }
        }

        protected void btnAgregarVenta_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario(txtFechaOrdenVenta, ddlCliente, null)) return;

            try
            {
                ManejarArchivoAdjunto(fileDocumentoVenta, "Orden de Venta");

                var nuevaOrden = new ordenVenta
                {
                    fecha = DateTime.Parse(txtFechaOrdenVenta.Text),
                    fechaSpecified = true,
                    total = 0,
                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true,
                    cliente = new empresa { idEmpresa = Convert.ToInt32(ddlCliente.SelectedValue) },
                    lineas = new lineaOrdenVenta[0]
                };

                ordenVentaService.guardarOrdenVenta(nuevaOrden, estado.NUEVO);
                MostrarAlerta("exito", "Orden de venta agregada exitosamente");
                CargarOrdenes(TipoOrden.Venta);
                LimpiarFormulario(TipoOrden.Venta);
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorAgregarVenta", $"Error al agregar orden de venta: {ex.Message}");
            }
        }

        protected void btnAgregarIngreso_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario(txtFechaIngreso, ddlOrdenCompraIngreso, ddlResponsableIngreso)) return;

            try
            {
                ManejarArchivoAdjunto(fileDocumentoIngreso, "Ingreso");

                var nuevoIngreso = new ordenIngreso
                {
                    fecha = DateTime.Parse(txtFechaIngreso.Text),
                    fechaSpecified = true,
                    total = 0,
                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true,
                    ordenCompra = new ordenCompra { idOrdenCompra = Convert.ToInt32(ddlOrdenCompraIngreso.SelectedValue) },
                    lineas = new lineaOrdenIngreso[0]
                };

                AsignarResponsable(nuevoIngreso, ddlResponsableIngreso.SelectedValue);
                ingresoService.guardarOrdenIngreso(nuevoIngreso, estado.NUEVO);
                MostrarAlerta("exito", "Ingreso agregado exitosamente");
                CargarOrdenes(TipoOrden.Ingreso);
                LimpiarFormulario(TipoOrden.Ingreso);
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorAgregarIngreso", $"Error al agregar ingreso: {ex.Message}");
            }
        }

        protected void btnAgregarSalida_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario(txtFechaSalida, ddlOrdenVentaSalida, ddlResponsableSalida)) return;

            try
            {
                ManejarArchivoAdjunto(fileDocumentoSalida, "Salida");

                var nuevaSalida = new ordenSalida
                {
                    fecha = DateTime.Parse(txtFechaSalida.Text),
                    fechaSpecified = true,
                    total = 0,
                    estado = estadoDocumento.PENDIENTE,
                    estadoSpecified = true,
                    ordenVenta = new ordenVenta { idOrdenVenta = Convert.ToInt32(ddlOrdenVentaSalida.SelectedValue) },
                    lineas = new lineaOrdenSalida[0]
                };

                AsignarResponsable(nuevaSalida, ddlResponsableSalida.SelectedValue);
                salidaService.guardarOrdenSalida(nuevaSalida, estado.NUEVO);
                MostrarAlerta("exito", "Salida agregada exitosamente");
                CargarOrdenes(TipoOrden.Salida);
                LimpiarFormulario(TipoOrden.Salida);
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorAgregarSalida", $"Error al agregar salida: {ex.Message}");
            }
        }

        #endregion

        #region NAVEGACIÓN

        protected void btnCompra_Click(object sender, EventArgs e) => MostrarSeccion(TipoOrden.Compra);
        protected void btnVenta_Click(object sender, EventArgs e) => MostrarSeccion(TipoOrden.Venta);
        protected void btnIngreso_Click(object sender, EventArgs e) => MostrarSeccion(TipoOrden.Ingreso);
        protected void btnSalida_Click(object sender, EventArgs e) => MostrarSeccion(TipoOrden.Salida);

        private void MostrarSeccion(TipoOrden tipoActivo)
        {
            foreach (var kvp in configuraciones)
            {
                bool esActivo = kvp.Key == tipoActivo;
                kvp.Value.Contenedor.Style["display"] = esActivo ? "block" : "none";
                kvp.Value.BtnModo.CssClass = esActivo ? "mode-btn active" : "mode-btn";
            }
        }

        #endregion

        #region MÉTODOS AUXILIARES

        private bool ValidarFormulario(TextBox txtFecha, DropDownList ddlPrincipal, DropDownList ddlResponsable)
        {
            if (string.IsNullOrWhiteSpace(txtFecha.Text))
            {
                MostrarAlerta("validacion", "Debe seleccionar una fecha");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ddlPrincipal.SelectedValue))
            {
                string campo = ddlPrincipal.ID.Contains("Proveedor") ? "proveedor" :
                              ddlPrincipal.ID.Contains("Cliente") ? "cliente" :
                              ddlPrincipal.ID.Contains("Compra") ? "orden de compra" : "orden de venta";
                MostrarAlerta("validacion", $"Debe seleccionar un {campo}");
                return false;
            }

            if (ddlResponsable != null && string.IsNullOrWhiteSpace(ddlResponsable.SelectedValue))
            {
                MostrarAlerta("validacion", "Debe seleccionar un responsable");
                return false;
            }

            return true;
        }

        private void LimpiarFormulario(TipoOrden tipo)
        {
            var config = configuraciones[tipo];
            config.TxtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
            config.DdlPrincipal.SelectedIndex = 0;
            config.DdlResponsable?.SelectedIndex = 0;
            config.FileUpload.Attributes.Clear();
        }

        private void ManejarArchivoAdjunto(FileUpload fileUpload, string tipoDocumento)
        {
            try
            {
                if (fileUpload.HasFile)
                {
                    string fileName = fileUpload.FileName;
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        MostrarAlerta("errorArchivo", "Tipo de archivo no permitido. Formatos aceptados: PDF, DOC, DOCX, JPG, JPEG, PNG");
                        return;
                    }

                    if (fileUpload.PostedFile.ContentLength > 10 * 1024 * 1024)
                    {
                        MostrarAlerta("errorTamano", "El archivo es demasiado grande. Tamaño máximo: 10MB");
                        return;
                    }

                    MostrarAlerta("exitoArchivo", $"Archivo {fileName} adjuntado correctamente para {tipoDocumento}");
                }
                else
                {
                    MostrarAlerta("infoArchivo", $"No se seleccionó ningún archivo para {tipoDocumento}. La orden se guardará sin documento adjunto.");
                }
            }
            catch (Exception ex)
            {
                MostrarAlerta("errorArchivoGeneral", $"Error al adjuntar archivo: {ex.Message}");
            }
        }

        private string ObtenerNombreResponsable(object entidad)
        {
            try
            {
                var propResponsable = entidad.GetType().GetProperty("responsable");
                if (propResponsable != null)
                {
                    var responsable = propResponsable.GetValue(entidad) as usuario;
                    if (responsable != null)
                        return $"{responsable.nombres} {responsable.apellidos}".Trim();
                }

                var propUsuario = entidad.GetType().GetProperty("usuario");
                if (propUsuario != null)
                {
                    var usuario = propUsuario.GetValue(entidad) as usuario;
                    if (usuario != null)
                        return $"{usuario.nombres} {usuario.apellidos}".Trim();
                }
            }
            catch { }

            return "Sistema";
        }

        private void AsignarResponsable(object entidad, string idUsuario)
        {
            try
            {
                var propResponsable = entidad.GetType().GetProperty("responsable");
                if (propResponsable != null)
                {
                    propResponsable.SetValue(entidad, new usuario { idUsuario = Convert.ToInt32(idUsuario) });
                    return;
                }

                var propUsuario = entidad.GetType().GetProperty("usuario");
                if (propUsuario != null)
                {
                    propUsuario.SetValue(entidad, new usuario { idUsuario = Convert.ToInt32(idUsuario) });
                }
            }
            catch { }
        }

        private void MostrarAlerta(string key, string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), key, $"alert('{mensaje}');", true);
        }

        public string GetBadgeClass(string estado)
        {
            if (string.IsNullOrEmpty(estado)) return "badge";

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
                case "disponible":
                    return "badge badge-aceptado";
                default:
                    return "badge";
            }
        }

        #endregion

        #region EVENTOS PENDIENTES (GridView RowDataBound y DropDownList SelectedIndexChanged)

        protected void gvOrdenesCompra_RowDataBound(object sender, GridViewRowEventArgs e) { }
        protected void gvOrdenesVenta_RowDataBound(object sender, GridViewRowEventArgs e) { }
        protected void gvRegistrosIngreso_RowDataBound(object sender, GridViewRowEventArgs e) { }
        protected void gvRegistrosSalida_RowDataBound(object sender, GridViewRowEventArgs e) { }
        protected void ddlOrdenCompraIngreso_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void ddlOrdenVentaSalida_SelectedIndexChanged(object sender, EventArgs e) { }

        #endregion
    }
}