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
                    // Registrar tareas asíncronas
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
            CategoriaWSClient categoriaClient = null;
            try
            {
                categoriaClient = new CategoriaWSClient();
                var response = await categoriaClient.listarCategoriasAsync();

                ddlCategoriaEditar.DataSource = response.@return;
                ddlCategoriaEditar.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar categorías: " + ex.Message);
            }
            finally
            {
                if (categoriaClient != null && categoriaClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    categoriaClient.Close();
                }
            }
        }

        private async Task CargarDetalleProductoAsync()
        {
            ProductoWSClient productoClient = null;
            ExistenciasWSClient existenciasClient = null;

            try
            {
                productoClient = new ProductoWSClient();
                var response = await productoClient.obtenerProductoAsync(ProductoId);
                var producto = response.@return;

                if (producto != null)
                {
                    // Asignación de literales para visualización
                    litNombreProducto.Text = producto.nombre;
                    litNombre.Text = producto.nombre;
                    litIdProducto.Text = producto.idProducto.ToString();
                    litCategoria.Text = producto.categoria?.nombre ?? "Sin categoría";
                    litMarca.Text = producto.marca ?? "N/A";
                    litPrecio.Text = string.Format("S/ {0:N2}", producto.precioUnitario);
                    litDescripcion.Text = producto.descripcion ?? "Sin descripción";
                    litStockMax.Text = producto.stockMaximo.ToString();
                    litStockMin.Text = producto.stockMinimo.ToString();

                    // ====== GUARDAR DATOS EN HIDDENFIELDS PARA EL MODAL ======
                    hdnNombre.Value = producto.nombre ?? "";
                    hdnMarca.Value = producto.marca ?? "";
                    hdnPrecio.Value = producto.precioUnitario.ToString("F2");
                    hdnDescripcion.Value = producto.descripcion ?? "";
                    hdnStockMin.Value = producto.stockMinimo.ToString();
                    hdnStockMax.Value = producto.stockMaximo.ToString();

                    if (producto.categoria != null)
                    {
                        hdnCategoria.Value = producto.categoria.nombre;
                        hdnCategoriaId.Value = producto.categoria.idCategoria.ToString();
                    }

                    // Cálculo de Stock Actual
                    existenciasClient = new ExistenciasWSClient();
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

                    // Habilitar botón de editar
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                        "HabilitarBoton", "habilitarBotonEditar();", true);
                }
                else
                {
                    Response.Redirect("Inventario.aspx");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar detalle: {ex.Message}");
                Response.Redirect("Inventario.aspx");
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

        protected async void BtnGuardarCambios_Click(object sender, EventArgs e)
        {
            ProductoWSClient productoClient = null;

            try
            {
                // ====== VALIDACIONES ======
                if (string.IsNullOrWhiteSpace(txtNombreEditar.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Por favor, ingrese el nombre del producto.'); abrirModalEditar();", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPrecioEditar.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Por favor, ingrese el precio unitario.'); abrirModalEditar();", true);
                    return;
                }

                double precioUnitario;
                if (!double.TryParse(txtPrecioEditar.Text, out precioUnitario) || precioUnitario <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Por favor, ingrese un precio válido mayor a 0.'); abrirModalEditar();", true);
                    return;
                }

                int stockMinimo = 0;
                if (!string.IsNullOrWhiteSpace(txtStockMinEditar.Text))
                {
                    if (!int.TryParse(txtStockMinEditar.Text, out stockMinimo) || stockMinimo < 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error",
                            "alert('Por favor, ingrese un stock mínimo válido (número entero mayor o igual a 0).'); abrirModalEditar();", true);
                        return;
                    }
                }

                int stockMaximo = 0;
                if (!string.IsNullOrWhiteSpace(txtStockMaxEditar.Text))
                {
                    if (!int.TryParse(txtStockMaxEditar.Text, out stockMaximo) || stockMaximo < 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error",
                            "alert('Por favor, ingrese un stock máximo válido (número entero mayor o igual a 0).'); abrirModalEditar();", true);
                        return;
                    }
                }

                if (stockMaximo > 0 && stockMinimo > stockMaximo)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('El stock máximo debe ser mayor o igual al stock mínimo.'); abrirModalEditar();", true);
                    return;
                }

                // ====== GUARDAR CAMBIOS ======
                if (ProductoId > 0)
                {
                    productoClient = new ProductoWSClient();

                    var productoModificado = new producto
                    {
                        idProducto = ProductoId,
                        nombre = txtNombreEditar.Text.Trim(),
                        marca = string.IsNullOrWhiteSpace(txtMarcaEditar.Text)
                            ? "Sin marca"
                            : txtMarcaEditar.Text.Trim(),
                        precioUnitario = precioUnitario,
                        descripcion = string.IsNullOrWhiteSpace(txtDescripcionEditar.Text)
                            ? "Sin descripción"
                            : txtDescripcionEditar.Text.Trim(),
                        stockMinimo = stockMinimo,
                        stockMaximo = stockMaximo,
                        categoria = new categoria
                        {
                            idCategoria = int.Parse(ddlCategoriaEditar.SelectedValue),
                            nombre = ddlCategoriaEditar.SelectedItem.Text
                        }
                    };

                    await productoClient.guardarProductoAsync(productoModificado, estado.MODIFICADO);

                    // Mostrar mensaje de éxito y recargar
                    ScriptManager.RegisterStartupScript(this, GetType(), "success",
                        "alert('Producto actualizado exitosamente.'); window.location.href = window.location.href;", true);
                }
            }
            catch (FormatException)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "alert('Error: Formato de datos incorrecto. Verifique los valores numéricos.'); abrirModalEditar();", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al guardar producto: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "alert('Error al guardar los cambios. Por favor, intente nuevamente.'); abrirModalEditar();", true);
            }
            finally
            {
                if (productoClient != null && productoClient.State == System.ServiceModel.CommunicationState.Opened)
                {
                    productoClient.Close();
                }
            }
        }
    }
}