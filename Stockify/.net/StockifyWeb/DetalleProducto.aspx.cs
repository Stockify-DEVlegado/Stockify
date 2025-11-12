using StockifyWeb.StockifyWS;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class DetalleProducto : System.Web.UI.Page
    {
        private int ProductoId
        {
            get
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int id))
                {
                    return id;
                }
                return 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ProductoId > 0)
                {
                    // Se registran las tareas asíncronas
                    RegisterAsyncTask(new PageAsyncTask(CargarCategoriasAsync));
                    RegisterAsyncTask(new PageAsyncTask(CargarDetalleProductoAsync));
                }
                else
                {
                    Response.Redirect("Inventario.aspx");
                }
            }
        }

        private async Task CargarCategoriasAsync()
        {
            try
            {
                // Usar 'using' garantiza que el cliente se cierre (Dispose)
                using (var categoriaClient = new CategoriaWSClient())
                {
                    var response = await categoriaClient.listarCategoriasAsync();
                    ddlCategoriaEditar.DataSource = response.@return;
                    ddlCategoriaEditar.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar categorías: " + ex.Message);
            }
        }

        private async Task CargarDetalleProductoAsync()
        {
            try
            {
                // Usar 'using' para ProductoWSClient
                using (var productoClient = new ProductoWSClient())
                {
                    var response = await productoClient.obtenerProductoAsync(ProductoId);
                    var producto = response.@return;

                    if (producto != null)
                    {
                        // 1. Asignación de literales
                        litNombreProducto.Text = producto.nombre;
                        litNombre.Text = producto.nombre;
                        litIdProducto.Text = producto.idProducto.ToString();
                        litCategoria.Text = producto.categoria?.nombre ?? "Sin categoría";
                        litMarca.Text = producto.marca ?? "N/A";
                        litPrecio.Text = string.Format("S/ {0:N2}", producto.precioUnitario);
                        litDescripcion.Text = producto.descripcion ?? "Sin descripción";
                        litStockMax.Text = producto.stockMaximo.ToString();
                        litStockMin.Text = producto.stockMinimo.ToString();

                        // 2. Cálculo de Stock Actual (Usar 'using' para ExistenciasWSClient)
                        using (var existenciasClient = new ExistenciasWSClient())
                        {
                            var existenciasResponse = await existenciasClient.listarExistenciasAsync();
                            var existencias = existenciasResponse.@return;

                            int stockActual = 0;
                            if (existencias != null)
                            {
                                stockActual = existencias
                                    .Count(e => e.producto != null &&
                                               e.producto.idProducto == producto.idProducto &&
                                               e.estado == estadoExistencias.DISPONIBLE);
                            }
                            litStockActual.Text = stockActual.ToString();
                        } // ExistenciasWSClient se cierra automáticamente aquí.

                        // 3. Habilitar el botón DEBE estar dentro del bloque exitoso
                        // Usamos this.Page para garantizar que se registre correctamente en el contexto de la página.
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "HabilitarBoton", "habilitarBotonEditar();", true);
                    }
                    else
                    {
                        // Si el producto es null (no encontrado), redirigir
                        Response.Redirect("Inventario.aspx");
                    }
                } // ProductoWSClient se cierra automáticamente aquí.
            }
            catch (Exception ex)
            {
                // Manejo de error de Web Service o red
                System.Diagnostics.Debug.WriteLine($"Error al cargar detalle: {ex.Message}");
                // Si hay un error, redirigir
                Response.Redirect("Inventario.aspx");
            }
        }

        protected async void BtnGuardarCambios_Click(object sender, EventArgs e)
        {
            try
            {
                if (ProductoId > 0)
                {
                    using (var productoClient = new ProductoWSClient())
                    {
                        var productoModificado = new producto
                        {
                            idProducto = ProductoId,
                            nombre = txtNombreEditar.Text,
                            marca = txtMarcaEditar.Text,
                            precioUnitario = double.Parse(txtPrecioEditar.Text),
                            descripcion = txtDescripcionEditar.Text,
                            stockMinimo = int.Parse(txtStockMinEditar.Text),
                            stockMaximo = int.Parse(txtStockMaxEditar.Text),

                            categoria = new categoria
                            {
                                idCategoria = int.Parse(ddlCategoriaEditar.SelectedValue),
                                nombre = ddlCategoriaEditar.SelectedItem.Text
                            }
                        };

                        await productoClient.guardarProductoAsync(productoModificado, estado.MODIFICADO);

                        // Recargar la página para ver los cambios actualizados
                        Response.Redirect(Request.RawUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al guardar producto: " + ex.Message);
            }
        }
    }
}