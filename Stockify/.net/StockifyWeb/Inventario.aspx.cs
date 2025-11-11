using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Inventario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Registrar tareas asíncronas para Page_Load
                RegisterAsyncTask(new PageAsyncTask(CargarCategoriasAsync));
                RegisterAsyncTask(new PageAsyncTask(CargarProductosAsync));
            }
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
                // Mostrar un mensaje de error al usuario
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
            litModalTitle.Text = "Agregar Producto";
            btnSaveProduct.Text = "Agregar Producto";
            hdnProductoId.Value = "0";
            ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModal();", true);
        }

        protected async void btnSaveProduct_Click(object sender, EventArgs e)
        {
            ProductoWSClient productoClient = null;
            CategoriaWSClient categoriaClient = null;

            try
            {
                productoClient = new ProductoWSClient();

                var producto = new producto
                {
                    nombre = txtProductName.Text.Trim(),
                    descripcion = txtDescripcion.Text.Trim(),
                    marca = txtMarca.Text.Trim(),
                    precioUnitario = double.Parse(txtPrecioUnitario.Text),
                    stockMinimo = 0,
                    stockMaximo = 0
                };

                int categoriaId = int.Parse(ddlCategoria.SelectedValue);
                if (categoriaId > 0)
                {
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
                }

                var estado = StockifyWeb.StockifyWS.estado.NUEVO;
                int productoId = int.Parse(hdnProductoId.Value);

                if (productoId > 0)
                {
                    producto.idProducto = productoId;
                    estado = StockifyWeb.StockifyWS.estado.MODIFICADO;
                }

                await productoClient.guardarProductoAsync(producto, estado);

                // Recargar productos después de guardar
                await CargarProductosAsync();

                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal", "cerrarModal();", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar producto: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('Error al guardar el producto: {ex.Message}');", true);
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

        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalle")
            {
                int productoId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"DetalleProducto.aspx?id={productoId}");
            }
        }

        protected void btnEditDetalle_Click(object sender, EventArgs e)
        {
            int productoId = int.Parse(hdnProductoId.Value);
            RegisterAsyncTask(new PageAsyncTask(() => CargarProductoParaEditarAsync(productoId)));
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

                    if (producto.categoria != null)
                    {
                        ddlCategoria.SelectedValue = producto.categoria.idCategoria.ToString();
                    }

                    litModalTitle.Text = "Editar Producto";
                    btnSaveProduct.Text = "Guardar Cambios";
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