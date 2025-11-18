using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
//using StockifyWeb.Services;

namespace StockifyWeb
{
    public partial class Inventario : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            // Registrar tareas asíncronas para Page_Load
            RegisterAsyncTask(new PageAsyncTask(CargarCategoriasAsync));
            RegisterAsyncTask(new PageAsyncTask(CargarCategoriasParaFiltroAsync));
            RegisterAsyncTask(new PageAsyncTask(CargarProductosAsync));
        }

        private async Task CargarCategoriasAsync()
        {
            CategoriaWSClient categoriaClient = null;
            try
            {
                categoriaClient = new CategoriaWSClient();

                var response = await categoriaClient.listarCategoriasAsync();
                var categorias = response.@return;

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
            }
            finally
            {
                if (categoriaClient != null && categoriaClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    categoriaClient.Close();
                }
            }
        }

        private async Task CargarProductosAsync()
        {
            ProductoWSClient productoClient = null;
            ExistenciasWSClient existenciasClient = null;

            try
            {
                productoClient = new ProductoWSClient();

                var response = await productoClient.listarProductosAsync();
                var productos = response.@return;

                var productosConExistencias = new List<ProductoViewModel>();

                if (productos != null && productos.Length > 0)
                {
                    existenciasClient = new ExistenciasWSClient();

                    var existenciasResponse = await existenciasClient.listarExistenciasAsync();
                    var existencias = existenciasResponse.@return;

                    foreach (var prod in productos)
                    {
                        int stockActual = 0;
                        if (existencias != null)
                        {
                            stockActual = existencias
                                .Count(e => e.producto != null &&
                                            e.producto.idProducto == prod.idProducto &&
                                            e.estado == estadoExistencias.DISPONIBLE);
                        }

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
                System.Diagnostics.Debug.WriteLine($"Error al cargar productos: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "errorCarga",
                    $"alert('Error al cargar productos: {ex.Message}');", true);
            }
            finally
            {
                if (existenciasClient != null && existenciasClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    existenciasClient.Close();
                }
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
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

        protected async void btnSaveProduct_Click(object sender, EventArgs e)
        {
            ProductoWSClient productoClient = null;
            CategoriaWSClient categoriaClient = null;

            try
            {
                // ====== VALIDACIONES ======
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Por favor, ingrese el nombre del producto.');", true);
                    return;
                }

                if (ddlCategoria.SelectedValue == "0")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Por favor, seleccione una categoría.');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPrecioUnitario.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Por favor, ingrese el precio unitario.');", true);
                    return;
                }

                if (!double.TryParse(txtPrecioUnitario.Text, out var precioUnitario) || precioUnitario <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Por favor, ingrese un precio válido mayor a 0.');", true);
                    return;
                }

                var stockMinimo = 0;
                if (!string.IsNullOrWhiteSpace(txtStockMinimo.Text))
                {
                    if (!int.TryParse(txtStockMinimo.Text, out stockMinimo) || stockMinimo < 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error",
                            "alert('Por favor, ingrese un stock mínimo válido (número entero mayor o igual a 0).');", true);
                        return;
                    }
                }

                var stockMaximo = 0;
                if (!string.IsNullOrWhiteSpace(txtStockMaximo.Text))
                {
                    if (!int.TryParse(txtStockMaximo.Text, out stockMaximo) || stockMaximo < 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error",
                            "alert('Por favor, ingrese un stock máximo válido (número entero mayor o igual a 0).');", true);
                        return;
                    }
                }

                if (stockMaximo > 0 && stockMinimo > stockMaximo)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('El stock máximo debe ser mayor o igual al stock mínimo.');", true);
                    return;
                }

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
                    estado = estado.MODIFICADO;
                }

                // ====== GUARDAR EN LA BASE DE DATOS ======
                await productoClient.guardarProductoAsync(producto, estado);

                // Agregar notificación y enviar correo solo si es nuevo producto
                if (esNuevo)
                {
                    // Notificar nuevo producto (incluye envío de correo)
                    NotificationService.NotificarNuevoProducto(producto.nombre);
                }
                else
                {
                    // Solo notificación de actualización
                    NotificationService.AgregarNotificacion(
                        $"Producto '{producto.nombre}' actualizado exitosamente",
                        "info",
                        "fa-edit"
                    );
                }

                // ====== RECARGAR Y CERRAR MODAL ======
                await CargarProductosAsync();

                LimpiarFormulario();

                var mensaje = productoId > 0
                    ? "Producto actualizado exitosamente."
                    : "Producto agregado exitosamente.";

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    $"cerrarModal(); alert('{mensaje}');", true);
            }
            catch (FormatException)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "alert('Error: Formato de datos incorrecto. Verifique los valores numéricos.');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar producto: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('Error al guardar el producto. Por favor, intente nuevamente.');", true);
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

        protected async void btnImportarCSV_Click(object sender, EventArgs e)
        {
            ProductoWSClient productoClient = null;

            try
            {
                // Validar que se haya seleccionado un archivo
                if (!fuCSV.HasFile)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                        "abrirModalImportar(); alert('⚠️ Por favor selecciona un archivo CSV.');", true);
                    return;
                }

                // Validar extensión
                string extension = System.IO.Path.GetExtension(fuCSV.FileName).ToLower();
                if (extension != ".csv")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                        "abrirModalImportar(); alert('⚠️ Solo se permiten archivos CSV.');", true);
                    return;
                }

                // Validar tamaño (10MB)
                if (fuCSV.PostedFile.ContentLength > 10485760)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                        "abrirModalImportar(); alert('⚠️ El archivo es demasiado grande. Máximo 10MB.');", true);
                    return;
                }

                // Crear el cliente del Web Service
                productoClient = new ProductoWSClient();

                // Obtener el Stream del archivo
                System.IO.Stream fileStream = fuCSV.PostedFile.InputStream;

                // Leer el contenido del archivo como bytes
                byte[] fileBytes;
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                // Llamar al Web Service con el archivo como byte[]
                var response = await productoClient.importarProductosDesdeCSVAsync(fileBytes);

                // *** CORRECCIÓN: Acceder a la propiedad @return del response ***
                int productosImportados = response.@return;

                // Recargar la lista de productos
                await CargarProductosAsync();

                // Mostrar mensaje de éxito
                ScriptManager.RegisterStartupScript(this, GetType(), "successImport",
                    $"cerrarModalImportar(); alert('✅ Importación exitosa!\\n\\nSe importaron {productosImportados} productos correctamente.');",
                    true);
            }
            catch (System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail> faultEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error SOAP Fault: {faultEx.Detail.Message}");

                string mensajeError = faultEx.Detail.Message;
                if (mensajeError.Contains("Error en la inserción masiva"))
                {
                    mensajeError = "Error en la base de datos. Verifica que todos los datos del CSV sean válidos.\\n\\n" +
                                  "Ningún producto fue insertado (transacción revertida).";
                }
                else if (mensajeError.Contains("Error al parsear"))
                {
                    mensajeError = "Error al leer el archivo CSV. Verifica el formato:\\n" +
                                  "- Primera fila debe ser el encabezado\\n" +
                                  "- 7 columnas: nombre,descripcion,marca,stockMinimo,stockMaximo,precioUnitario,idCategoria\\n" +
                                  "- Valores numéricos válidos";
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                    $"abrirModalImportar(); alert('❌ Error al importar productos:\\n\\n{mensajeError}');",
                    true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al importar CSV: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                    "abrirModalImportar(); alert('❌ Error inesperado al importar productos. Por favor, intenta nuevamente.');",
                    true);
            }
            finally
            {
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
                }
            }
        }

        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "VerDetalle") return;
            var productoId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect($"DetalleProducto.aspx?id={productoId}");
        }

        protected async void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            ProductoWSClient productoClient = null;

            try
            {
                var productoId = int.Parse(hdnProductoIdEliminar.Value);

                if (productoId <= 0) return;
                productoClient = new ProductoWSClient();

                // ====== ELIMINAR PRODUCTO DE LA BASE DE DATOS ======
                await productoClient.eliminarProductoAsync(productoId);

                // Recargar la lista de productos
                await CargarProductosAsync();

                ScriptManager.RegisterStartupScript(this, GetType(), "successDelete",
                    "cerrarModalEliminar(); alert('✅ Producto eliminado exitosamente.');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar producto: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "errorDelete",
                    "cerrarModalEliminar(); alert('❌ Error al eliminar el producto. Por favor, intente nuevamente.');", true);
            }
            finally
            {
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
                }
            }
        }

        protected void btnEditDetalle_Click(object sender, EventArgs e)
        {
            int productoId = int.Parse(hdnProductoId.Value);
            RegisterAsyncTask(new PageAsyncTask(() => CargarProductoParaEditarAsync(productoId)));
        }

        protected void gvProductos_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (e.SortExpression == "Producto")
            {
                RegisterAsyncTask(new PageAsyncTask(CargarProductosOrdenadosPorNombreAsync));
            }
        }

        private async Task CargarProductosOrdenadosPorNombreAsync()
        {
            ProductoWSClient productoClient = null;
            ExistenciasWSClient existenciasClient = null;

            try
            {
                productoClient = new ProductoWSClient();

                var response = await productoClient.listarProductosOrdenadoPorNombreAsync();
                var productos = response.@return;

                var productosConExistencias = new List<ProductoViewModel>();

                if (productos != null && productos.Length > 0)
                {
                    existenciasClient = new ExistenciasWSClient();

                    var existenciasResponse = await existenciasClient.listarExistenciasAsync();
                    var existencias = existenciasResponse.@return;

                    foreach (var prod in productos)
                    {
                        int stockActual = 0;
                        if (existencias != null)
                        {
                            stockActual = existencias
                                .Count(e => e.producto != null &&
                                            e.producto.idProducto == prod.idProducto &&
                                            e.estado == estadoExistencias.DISPONIBLE);
                        }

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
                ScriptManager.RegisterStartupScript(this, GetType(), "errorCarga",
                    $"alert('Error al cargar productos: {ex.Message}');", true);
            }
            finally
            {
                if (existenciasClient != null && existenciasClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    existenciasClient.Close();
                }
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
                }
            }
        }

        private async Task CargarCategoriasParaFiltroAsync()
        {
            CategoriaWSClient categoriaClient = null;
            try
            {
                categoriaClient = new CategoriaWSClient();

                var response = await categoriaClient.listarCategoriasAsync();
                var categorias = response.@return;

                ddlFiltroCategoria.Items.Clear();
                ddlFiltroCategoria.Items.Add(new ListItem("📋 Todas las categorías", "0"));

                if (categorias != null)
                {
                    foreach (var cat in categorias)
                    {
                        ddlFiltroCategoria.Items.Add(new ListItem(cat.nombre, cat.idCategoria.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar categorías para filtro: {ex.Message}");
            }
            finally
            {
                if (categoriaClient != null && categoriaClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    categoriaClient.Close();
                }
            }
        }

        protected void ddlFiltroCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            int categoriaId = int.Parse(ddlFiltroCategoria.SelectedValue);

            if (categoriaId == 0)
            {
                RegisterAsyncTask(new PageAsyncTask(CargarProductosAsync));
            }
            else
            {
                RegisterAsyncTask(new PageAsyncTask(() => CargarProductosPorCategoriaAsync(categoriaId)));
            }
        }

        private async Task CargarProductosPorCategoriaAsync(int categoriaId)
        {
            ProductoWSClient productoClient = null;
            ExistenciasWSClient existenciasClient = null;

            try
            {
                productoClient = new ProductoWSClient();

                var response = await productoClient.listarProductosPorCategoriaAsync(categoriaId);
                var productos = response.@return;

                var productosConExistencias = new List<ProductoViewModel>();

                if (productos != null && productos.Length > 0)
                {
                    existenciasClient = new ExistenciasWSClient();

                    var existenciasResponse = await existenciasClient.listarExistenciasAsync();
                    var existencias = existenciasResponse.@return;

                    foreach (var prod in productos)
                    {
                        int stockActual = 0;
                        if (existencias != null)
                        {
                            stockActual = existencias
                                .Count(e => e.producto != null &&
                                            e.producto.idProducto == prod.idProducto &&
                                            e.estado == estadoExistencias.DISPONIBLE);
                        }

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
                ScriptManager.RegisterStartupScript(this, GetType(), "errorCarga",
                    $"alert('Error al cargar productos: {ex.Message}');", true);
            }
            finally
            {
                if (existenciasClient != null && existenciasClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    existenciasClient.Close();
                }
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
                }
            }
        }

        private async Task CargarProductoParaEditarAsync(int productoId)
        {
            ProductoWSClient productoClient = null;

            try
            {
                productoClient = new ProductoWSClient();

                var response = await productoClient.obtenerProductoAsync(productoId);
                var producto = response.@return;

                if (producto != null)
                {
                    hdnProductoId.Value = producto.idProducto.ToString();
                    txtProductName.Text = producto.nombre;
                    txtDescripcion.Text = producto.descripcion;
                    txtMarca.Text = producto.marca;
                    txtPrecioUnitario.Text = producto.precioUnitario.ToString("F2");
                    txtStockMinimo.Text = producto.stockMinimo.ToString();
                    txtStockMaximo.Text = producto.stockMaximo.ToString();

                    if (producto.categoria != null)
                    {
                        ddlCategoria.SelectedValue = producto.categoria.idCategoria.ToString();
                    }

                    litModalTitle.Text = "✏️ Editar Producto";
                    btnSaveProduct.Text = "💾 Guardar Cambios";
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarDetalle",
                    "cerrarDetalleModal();", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "abrirModal();", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar para editar: {ex.Message}");
            }
            finally
            {
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
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
    }

    // Clase auxiliar para el GridView
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