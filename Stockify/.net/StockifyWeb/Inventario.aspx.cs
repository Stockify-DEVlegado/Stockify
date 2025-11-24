using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Inventario : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosIniciales();
                ActualizarInformacionPaginacion();
            }
        }

        private void CargarDatosIniciales()
        {
            try
            {
                CargarCategorias();
                CargarProductos();
                CargarCategoriasParaFiltro();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en carga inicial: {ex.Message}");
            }
        }

        private void CargarCategorias()
        {
            using (var categoriaClient = new CategoriaWSClient())
            {
                try
                {
                    var categorias = categoriaClient.listarCategorias();

                    ddlCategoria.Items.Clear();
                    ddlCategoria.Items.Add(new ListItem("Seleccione una categoría", "0"));

                    if (categorias != null)
                    {
                        foreach (var cat in categorias)
                        {
                            ddlCategoria.Items.Add(new ListItem(cat.nombre, cat.idCategoria.ToString()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al cargar categorías: {ex.Message}");
                    throw;
                }
            }
        }

        private void CargarProductos()
        {
            using (var productoClient = new ProductoWSClient())
            {
                try
                {
                    productoClient.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(2);

                    var productos = productoClient.listarProductos();
                    var productosConExistencias = new List<ProductoViewModel>();

                    if (productos != null && productos.Length > 0)
                    {
                        foreach (var prod in productos)
                        {
                            int stockActual = productoClient.obtenerStockActual(prod.idProducto);

                            productosConExistencias.Add(new ProductoViewModel
                            {
                                IdProducto = prod.idProducto,
                                Producto = prod.nombre ?? "Sin nombre",
                                Precio = prod.precioUnitario,
                                Descripcion = prod.descripcion ?? "Sin descripción",
                                Marca = prod.marca ?? "Sin marca",
                                Categoria = prod.categoria?.nombre ?? "Sin categoría",
                                StockActual = stockActual,
                                StockMinimo = prod.stockMinimo,
                                StockMaximo = prod.stockMaximo
                            });
                        }
                    }

                    gvProductos.DataSource = productosConExistencias;
                    gvProductos.DataBind();
                    ActualizarInformacionPaginacion();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al cargar productos: {ex.Message}");
                    throw;
                }
            }
        }

        private void CargarCategoriasParaFiltro()
        {
            ddlFiltroCategoria.Items.Clear();
            ddlFiltroCategoria.Items.Add(new ListItem("📋 Todas las categorías", "0"));

            foreach (ListItem item in ddlCategoria.Items)
            {
                if (item.Value != "0")
                {
                    ddlFiltroCategoria.Items.Add(new ListItem(item.Text, item.Value));
                }
            }
        }

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            litModalTitle.Text = "✨ Agregar Producto";
            btnSaveProduct.Text = "💾 Guardar Producto";
            hdnProductoId.Value = "0";
            ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModal();", true);
        }

        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(GuardarProductoAsync));
        }

        private async Task GuardarProductoAsync()
        {
            ProductoWSClient productoClient = null;
            CategoriaWSClient categoriaClient = null;

            try
            {
                // ====== VALIDACIONES ======
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "abrirModal(); mostrarToast('Por favor, ingrese el nombre del producto.', 'error');", true);
                    return;
                }

                if (ddlCategoria.SelectedValue == "0")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "abrirModal(); mostrarToast('Por favor, seleccione una categoría.', 'error');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPrecioUnitario.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "abrirModal(); mostrarToast('Por favor, ingrese el precio unitario.', 'error');", true);
                    return;
                }

                if (!double.TryParse(txtPrecioUnitario.Text, out var precioUnitario) || precioUnitario <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "abrirModal(); mostrarToast('Por favor, ingrese un precio válido mayor a 0.', 'error');", true);
                    return;
                }

                var stockMinimo = 0;
                if (!string.IsNullOrWhiteSpace(txtStockMinimo.Text))
                {
                    if (!int.TryParse(txtStockMinimo.Text, out stockMinimo) || stockMinimo < 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error",
                            "abrirModal(); mostrarToast('Por favor, ingrese un stock mínimo válido.', 'error');", true);
                        return;
                    }
                }

                var stockMaximo = 0;
                if (!string.IsNullOrWhiteSpace(txtStockMaximo.Text))
                {
                    if (!int.TryParse(txtStockMaximo.Text, out stockMaximo) || stockMaximo < 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error",
                            "abrirModal(); mostrarToast('Por favor, ingrese un stock máximo válido.', 'error');", true);
                        return;
                    }
                }

                if (stockMaximo > 0 && stockMinimo > stockMaximo)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "abrirModal(); mostrarToast('El stock máximo debe ser mayor o igual al stock mínimo.', 'error');", true);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[GUARDAR] Iniciando guardado de producto: {txtProductName.Text}");

                // ====== CREAR OBJETO PRODUCTO ======
                productoClient = new ProductoWSClient();

                var producto = new producto
                {
                    nombre = txtProductName.Text.Trim(),
                    descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text)
                        ? "Sin descripción"
                        : txtDescripcion.Text.Trim(),
                    marca = string.IsNullOrWhiteSpace(txtMarca.Text)
                        ? "Sin marca"
                        : txtMarca.Text.Trim(),
                    precioUnitario = precioUnitario,
                    stockMinimo = stockMinimo,
                    stockMaximo = stockMaximo
                };

                // ====== ASIGNAR CATEGORÍA ======
                int categoriaId = int.Parse(ddlCategoria.SelectedValue);
                categoriaClient = new CategoriaWSClient();
                var categoriaResponse = await categoriaClient.obtenerCategoriaAsync(categoriaId);

                if (categoriaResponse.@return != null)
                {
                    producto.categoria = new categoria
                    {
                        idCategoria = categoriaResponse.@return.idCategoria,
                        nombre = categoriaResponse.@return.nombre
                    };
                }

                // ====== DETERMINAR SI ES NUEVO O MODIFICADO ======
                var estado = StockifyWS.estado.NUEVO;
                var productoId = int.Parse(hdnProductoId.Value);
                var esNuevo = productoId == 0;

                if (productoId > 0)
                {
                    producto.idProducto = productoId;
                    estado = StockifyWS.estado.MODIFICADO;
                }

                System.Diagnostics.Debug.WriteLine($"[GUARDAR] Estado: {estado}, EsNuevo: {esNuevo}");

                // ====== GUARDAR EN LA BASE DE DATOS ======
                await productoClient.guardarProductoAsync(producto, estado);

                System.Diagnostics.Debug.WriteLine("[GUARDAR] Producto guardado exitosamente");

                // ====== NOTIFICACIONES (SIN AWAIT - DISPARO Y OLVIDO) ======
                if (esNuevo)
                {
                    NotificationService.NotificarNuevoProducto(
                        producto.nombre,
                        producto.categoria?.nombre ?? "Sin categoría",
                        producto.precioUnitario
                    );
                    System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN] Producto agregado: {producto.nombre}");
                }
                else
                {
                    NotificationService.NotificarProductoActualizado(producto.nombre);
                    System.Diagnostics.Debug.WriteLine($"[NOTIFICACIÓN] Producto actualizado: {producto.nombre}");
                }

                // ====== RECARGAR DATOS ======
                CargarProductos();

                // ====== CERRAR MODAL Y REFRESCAR CAMPANITA (SIN ALERT) ======
                ScriptManager.RegisterStartupScript(this, GetType(), "successSave",
                    "cerrarModal(); actualizarNotificaciones();", true);
            }
            catch (FormatException)
            {
                System.Diagnostics.Debug.WriteLine("[GUARDAR] Error de formato");
                ScriptManager.RegisterStartupScript(this, GetType(), "errorFormat",
                    "abrirModal(); mostrarToast('Error: Formato de datos incorrecto. Verifique los valores numéricos.', 'error');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GUARDAR] Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[GUARDAR] StackTrace: {ex.StackTrace}");
                ScriptManager.RegisterStartupScript(this, GetType(), "errorSave",
                    "abrirModal(); mostrarToast('Error al guardar el producto. Por favor, intente nuevamente.', 'error');", true);
            }
            finally
            {
                if (categoriaClient != null && categoriaClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    categoriaClient.Close();
                }
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
                }
            }
        }

        protected void btnImportarCSV_Click(object sender, EventArgs e)
        {
            using (var productoClient = new ProductoWSClient())
            {
                try
                {
                    if (!fuCSV.HasFile)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                            "abrirModalImportar(); mostrarToast('Por favor selecciona un archivo CSV.', 'warning');", true);
                        return;
                    }

                    string extension = System.IO.Path.GetExtension(fuCSV.FileName).ToLower();
                    if (extension != ".csv")
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                            "abrirModalImportar(); mostrarToast('Solo se permiten archivos CSV.', 'warning');", true);
                        return;
                    }

                    if (fuCSV.PostedFile.ContentLength > 10485760)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                            "abrirModalImportar(); mostrarToast('El archivo es demasiado grande. Máximo 10MB.', 'warning');", true);
                        return;
                    }

                    System.IO.Stream fileStream = fuCSV.PostedFile.InputStream;
                    byte[] fileBytes;
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }

                    // Parsear CSV para obtener nombres de productos
                    List<string> nombresProductos = ExtraerNombresDeCSV(fileBytes);

                    // Importar productos
                    int productosImportados = productoClient.importarProductosDesdeCSV(fileBytes);

                    // Recargar productos
                    CargarProductos();

                    // NOTIFICAR IMPORTACIÓN
                    NotificationService.NotificarImportacionCSV(productosImportados, nombresProductos);

                    ScriptManager.RegisterStartupScript(this, GetType(), "successImport",
                        $"cerrarModalImportar(); mostrarToast('Importación exitosa! Se importaron {productosImportados} productos.', 'success'); actualizarNotificaciones();",
                        true);
                }
                catch (System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail> faultEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error SOAP Fault: {faultEx.Detail.Message}");
                    string mensajeError = faultEx.Detail.Message;

                    if (mensajeError.Contains("Error en la inserción masiva"))
                    {
                        mensajeError = "Error en la base de datos. Ningún producto fue insertado.";
                    }
                    else if (mensajeError.Contains("Error al parsear"))
                    {
                        mensajeError = "Error al leer el archivo CSV. Verifica el formato.";
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                        $"abrirModalImportar(); mostrarToast('Error al importar: {mensajeError}', 'error');",
                        true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al importar CSV: {ex.Message}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                        "abrirModalImportar(); mostrarToast('Error inesperado al importar productos.', 'error');",
                        true);
                }
            }
        }

        /// <summary>
        /// Extrae los nombres de los productos del CSV para mostrarlos en la notificación
        /// </summary>
        private List<string> ExtraerNombresDeCSV(byte[] fileBytes)
        {
            var nombres = new List<string>();

            try
            {
                string csvContent = System.Text.Encoding.UTF8.GetString(fileBytes);
                var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Saltar la primera línea (encabezados)
                for (int i = 1; i < lines.Length; i++)
                {
                    var columns = lines[i].Split(',');
                    if (columns.Length > 0 && !string.IsNullOrWhiteSpace(columns[0]))
                    {
                        nombres.Add(columns[0].Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al extraer nombres del CSV: {ex.Message}");
            }

            return nombres;
        }

        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "VerDetalle") return;
            var productoId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect($"DetalleProducto.aspx?id={productoId}");
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            using (var productoClient = new ProductoWSClient())
            {
                try
                {
                    var productoId = int.Parse(hdnProductoIdEliminar.Value);
                    if (productoId <= 0) return;

                    // Obtener nombre del producto antes de eliminarlo
                    var producto = productoClient.obtenerProducto(productoId);
                    string nombreProducto = producto?.nombre ?? "Producto";

                    productoClient.eliminarProducto(productoId);

                    // Notificar eliminación
                    NotificationService.NotificarProductoEliminado(nombreProducto);

                    CargarProductos();

                    ScriptManager.RegisterStartupScript(this, GetType(), "successDelete",
                        "cerrarModalEliminar(); actualizarNotificaciones();", true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al eliminar producto: {ex.Message}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorDelete",
                        "cerrarModalEliminar(); mostrarToast('Error al eliminar el producto.', 'error');", true);
                }
            }
        }

        protected void gvProductos_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (e.SortExpression == "Producto")
            {
                CargarProductosOrdenadosPorNombre();
            }
        }

        private void CargarProductosOrdenadosPorNombre()
        {
            using (var productoClient = new ProductoWSClient())
            {
                try
                {
                    productoClient.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(2);

                    var productos = productoClient.listarProductosOrdenadoPorNombre();
                    var productosConExistencias = new List<ProductoViewModel>();

                    if (productos != null && productos.Length > 0)
                    {
                        foreach (var prod in productos)
                        {
                            int stockActual = productoClient.obtenerStockActual(prod.idProducto);

                            productosConExistencias.Add(new ProductoViewModel
                            {
                                IdProducto = prod.idProducto,
                                Producto = prod.nombre ?? "Sin nombre",
                                Precio = prod.precioUnitario,
                                Descripcion = prod.descripcion ?? "Sin descripción",
                                Marca = prod.marca ?? "Sin marca",
                                Categoria = prod.categoria?.nombre ?? "Sin categoría",
                                StockActual = stockActual,
                                StockMinimo = prod.stockMinimo,
                                StockMaximo = prod.stockMaximo
                            });
                        }
                    }

                    gvProductos.DataSource = productosConExistencias;
                    gvProductos.DataBind();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al cargar productos ordenados: {ex.Message}");
                }
            }
        }

        protected void ddlFiltroCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            int categoriaId = int.Parse(ddlFiltroCategoria.SelectedValue);

            if (categoriaId == 0)
            {
                CargarProductos();
            }
            else
            {
                CargarProductosPorCategoria(categoriaId);
            }
        }

        private void CargarProductosPorCategoria(int categoriaId)
        {
            using (var productoClient = new ProductoWSClient())
            {
                try
                {
                    productoClient.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(2);

                    var productos = productoClient.listarProductosPorCategoria(categoriaId);
                    var productosConExistencias = new List<ProductoViewModel>();

                    if (productos != null && productos.Length > 0)
                    {
                        foreach (var prod in productos)
                        {
                            int stockActual = productoClient.obtenerStockActual(prod.idProducto);

                            productosConExistencias.Add(new ProductoViewModel
                            {
                                IdProducto = prod.idProducto,
                                Producto = prod.nombre ?? "Sin nombre",
                                Precio = prod.precioUnitario,
                                Descripcion = prod.descripcion ?? "Sin descripción",
                                Marca = prod.marca ?? "Sin marca",
                                Categoria = prod.categoria?.nombre ?? "Sin categoría",
                                StockActual = stockActual,
                                StockMinimo = prod.stockMinimo,
                                StockMaximo = prod.stockMaximo
                            });
                        }
                    }

                    gvProductos.DataSource = productosConExistencias;
                    gvProductos.DataBind();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al cargar productos por categoría: {ex.Message}");
                }
            }
        }

        private void LimpiarFormulario()
        {
            hdnProductoId.Value = "0";
            txtProductName.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            txtMarca.Text = string.Empty;
            txtPrecioUnitario.Text = string.Empty;
            txtStockMinimo.Text = string.Empty;
            txtStockMaximo.Text = string.Empty;
            ddlCategoria.SelectedIndex = 0;
        }

        // ==================== MÉTODOS DE PAGINACIÓN ====================

        protected void gvProductos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductos.PageIndex = e.NewPageIndex;
            CargarProductos();
            ActualizarInformacionPaginacion();
        }

        protected void btnPrimeraPagina_Click(object sender, EventArgs e)
        {
            gvProductos.PageIndex = 0;
            CargarProductos();
            ActualizarInformacionPaginacion();
        }

        protected void btnPaginaAnterior_Click(object sender, EventArgs e)
        {
            if (gvProductos.PageIndex > 0)
            {
                gvProductos.PageIndex--;
                CargarProductos();
                ActualizarInformacionPaginacion();
            }
        }

        protected void btnPaginaSiguiente_Click(object sender, EventArgs e)
        {
            if (gvProductos.PageIndex < gvProductos.PageCount - 1)
            {
                gvProductos.PageIndex++;
                CargarProductos();
                ActualizarInformacionPaginacion();
            }
        }

        protected void btnUltimaPagina_Click(object sender, EventArgs e)
        {
            gvProductos.PageIndex = gvProductos.PageCount - 1;
            CargarProductos();
            ActualizarInformacionPaginacion();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvProductos.PageSize = int.Parse(ddlPageSize.SelectedValue);
            gvProductos.PageIndex = 0; // Volver a la primera página
            CargarProductos();
            ActualizarInformacionPaginacion();
        }

        private void ActualizarInformacionPaginacion()
        {
            if (gvProductos.Rows.Count == 0)
            {
                litPaginaActual.Text = "0";
                litPaginaTotal.Text = "0";
                litTotalProductos.Text = "0";
                litNumeroPaginas.Text = "";

                btnPrimeraPagina.Enabled = false;
                btnPaginaAnterior.Enabled = false;
                btnPaginaSiguiente.Enabled = false;
                btnUltimaPagina.Enabled = false;
                return;
            }

            int totalProductos = gvProductos.Rows.Count;
            int paginaActual = gvProductos.PageIndex + 1;
            int totalPaginas = gvProductos.PageCount;
            int pageSize = gvProductos.PageSize;

            // Calcular registros mostrados
            int registroInicio = (gvProductos.PageIndex * pageSize) + 1;
            int registroFin = Math.Min((gvProductos.PageIndex + 1) * pageSize, totalProductos);

            litPaginaActual.Text = registroInicio.ToString();
            litPaginaTotal.Text = registroFin.ToString();
            litTotalProductos.Text = totalProductos.ToString();

            // Generar botones de números de página
            GenerarBotonesNumeroPagina(paginaActual, totalPaginas);

            // Habilitar/deshabilitar botones de navegación
            btnPrimeraPagina.Enabled = paginaActual > 1;
            btnPaginaAnterior.Enabled = paginaActual > 1;
            btnPaginaSiguiente.Enabled = paginaActual < totalPaginas;
            btnUltimaPagina.Enabled = paginaActual < totalPaginas;
        }

        private void GenerarBotonesNumeroPagina(int paginaActual, int totalPaginas)
        {
            if (totalPaginas <= 1)
            {
                litNumeroPaginas.Text = "";
                return;
            }

            var html = new System.Text.StringBuilder();
            int rangoInicio = Math.Max(1, paginaActual - 2);
            int rangoFin = Math.Min(totalPaginas, paginaActual + 2);

            // Mostrar primera página si no está en el rango
            if (rangoInicio > 1)
            {
                html.Append($"<button type='button' class='pagination-button' onclick='irAPagina(1)'>1</button>");
                if (rangoInicio > 2)
                {
                    html.Append("<span style='color: var(--muted); padding: 0 8px;'>...</span>");
                }
            }

            // Botones de páginas en el rango
            for (int i = rangoInicio; i <= rangoFin; i++)
            {
                string activeClass = i == paginaActual ? "active" : "";
                html.Append($"<button type='button' class='pagination-button {activeClass}' onclick='irAPagina({i})'>{i}</button>");
            }

            // Mostrar última página si no está en el rango
            if (rangoFin < totalPaginas)
            {
                if (rangoFin < totalPaginas - 1)
                {
                    html.Append("<span style='color: var(--muted); padding: 0 8px;'>...</span>");
                }
                html.Append($"<button type='button' class='pagination-button' onclick='irAPagina({totalPaginas})'>{totalPaginas}</button>");
            }

            litNumeroPaginas.Text = html.ToString();
        }

        // Método para ir a una página específica (llamado desde JavaScript)
        protected void IrAPagina(int numeroPagina)
        {
            gvProductos.PageIndex = numeroPagina - 1;
            CargarProductos();
            ActualizarInformacionPaginacion();
        }
    }

    public class ProductoViewModel
    {
        public int IdProducto { get; set; }
        public string Producto { get; set; }
        public double Precio { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public int StockMaximo { get; set; }
    }
}