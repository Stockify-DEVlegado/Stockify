using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.Linq;
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
                ScriptManager.RegisterStartupScript(this, GetType(), "errorCarga",
                    $"alert('Error al cargar datos: {ex.Message}');", true);
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
            using (var productoClient = new ProductoWSClient())
            using (var categoriaClient = new CategoriaWSClient())
            {
                try
                {
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

                    int categoriaId = int.Parse(ddlCategoria.SelectedValue);
                    var categoria = categoriaClient.obtenerCategoria(categoriaId);

                    if (categoria != null)
                    {
                        producto.categoria = new categoria
                        {
                            idCategoria = categoria.idCategoria,
                            nombre = categoria.nombre
                        };
                    }

                    var estado = StockifyWS.estado.NUEVO;
                    var productoId = int.Parse(hdnProductoId.Value);
                    var esNuevo = productoId == 0;

                    if (productoId > 0)
                    {
                        producto.idProducto = productoId;
                        estado = estado.MODIFICADO;
                    }

                    productoClient.guardarProducto(producto, estado);

                    if (esNuevo)
                    {
                        NotificationService.NotificarNuevoProducto(producto.nombre);
                    }
                    else
                    {
                        NotificationService.AgregarNotificacion(
                            $"Producto '{producto.nombre}' actualizado exitosamente",
                            "info",
                            "fa-edit"
                        );
                    }

                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
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
                            "abrirModalImportar(); alert('⚠ Por favor selecciona un archivo CSV.');", true);
                        return;
                    }

                    string extension = System.IO.Path.GetExtension(fuCSV.FileName).ToLower();
                    if (extension != ".csv")
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                            "abrirModalImportar(); alert('⚠ Solo se permiten archivos CSV.');", true);
                        return;
                    }

                    if (fuCSV.PostedFile.ContentLength > 10485760)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "errorImport",
                            "abrirModalImportar(); alert('⚠ El archivo es demasiado grande. Máximo 10MB.');", true);
                        return;
                    }

                    System.IO.Stream fileStream = fuCSV.PostedFile.InputStream;
                    byte[] fileBytes;
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }

                    int productosImportados = productoClient.importarProductosDesdeCSV(fileBytes);
                    CargarProductos();

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
            }
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

                    productoClient.eliminarProducto(productoId);
                    CargarProductos();

                    ScriptManager.RegisterStartupScript(this, GetType(), "successDelete",
                        "cerrarModalEliminar(); alert('✅ Producto eliminado exitosamente.');", true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al eliminar producto: {ex.Message}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorDelete",
                        "cerrarModalEliminar(); alert('❌ Error al eliminar el producto. Por favor, intente nuevamente.');", true);
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
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorCarga",
                        $"alert('Error al cargar productos: {ex.Message}');", true);
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
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorCarga",
                        $"alert('Error al cargar productos: {ex.Message}');", true);
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