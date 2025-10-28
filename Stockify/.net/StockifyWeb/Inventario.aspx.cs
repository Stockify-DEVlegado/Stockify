using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                CargarProductos();
            }
        }

        // MÉTODO PARA ASIGNAR CLASES CSS SEGÚN DISPONIBILIDAD
        public string GetClaseDisponibilidad(string disponibilidad)
        {
            switch (disponibilidad.ToLower())
            {
                case "en stock":
                    return "in-stock";
                case "agotado":
                    return "out-of-stock";
                case "stock bajo":
                    return "low-stock";
                case "disponibilidad":
                    return "availability";
                default:
                    return "in-stock";
            }
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener valores del formulario
                string nombre = txtProductName.Text.Trim();
                string id = txtProductID.Text.Trim();
                string categoria = ddlCategoria.SelectedValue;
                string precio = txtPrecioUnitario.Text.Trim();
                string descripcion = txtDescripcion.Text.Trim();
                string marca = txtMarca.Text.Trim();

                // Validaciones básicas
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(id))
                {
                    MostrarMensaje("Por favor complete todos los campos obligatorios");
                    return;
                }

                // Guardar producto (aquí conectas con tu base de datos)
                GuardarProducto(nombre, id, categoria, precio, descripcion, marca);

                // Limpiar formulario
                LimpiarFormulario();

                // Cerrar modal
                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal", "cerrarModal();", true);

                // Recargar productos
                CargarProductos();

                MostrarMensaje("Producto agregado correctamente", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al agregar producto: " + ex.Message);
            }
        }

        private void GuardarProducto(string nombre, string id, string categoria, string precio, string descripcion, string marca)
        {
            // Aquí va tu lógica para guardar en la base de datos
            System.Diagnostics.Debug.WriteLine($"Producto agregado:");
            System.Diagnostics.Debug.WriteLine($"Nombre: {nombre}");
            System.Diagnostics.Debug.WriteLine($"ID: {id}");
            System.Diagnostics.Debug.WriteLine($"Categoría: {categoria}");
            System.Diagnostics.Debug.WriteLine($"Precio: {precio}");
            System.Diagnostics.Debug.WriteLine($"Descripción: {descripcion}");
            System.Diagnostics.Debug.WriteLine($"Marca: {marca}");
        }

        private void CargarProductos()
        {
            // Datos de ejemplo con los colores originales
            var productos = new List<dynamic>
            {
                new { Producto = "Maggi", Precio = 430, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "En stock" },
                new { Producto = "Bru", Precio = 257, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "En stock" },
                new { Producto = "Red Bull", Precio = 405, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "Agotado" },
                new { Producto = "Bourn Vita", Precio = 502, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "En stock" },
                new { Producto = "Horlicks", Precio = 530, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "En stock" },
                new { Producto = "Harpie", Precio = 605, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "En stock" },
                new { Producto = "Ariel", Precio = 408, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "Agotado" },
                new { Producto = "Scotch Brita", Precio = 359, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "En stock" },
                new { Producto = "Coca cola", Precio = 205, Descripcion = "---", Marca = "---", Categoria = "---", Disponibilidad = "Stock bajo" }
            };

            gvProductos.DataSource = productos;
            gvProductos.DataBind();
        }

        private void LimpiarFormulario()
        {
            txtProductName.Text = "";
            txtProductID.Text = "";
            ddlCategoria.SelectedIndex = 0;
            txtPrecioUnitario.Text = "";
            txtDescripcion.Text = "";
            txtMarca.Text = "";
            fuProductImage.Attributes.Clear();
        }

        private void MostrarMensaje(string mensaje, bool esExitoso = false)
        {
            string script = $@"alert('{mensaje}');";
            if (esExitoso)
            {
                script = $@"alert('✅ {mensaje}');";
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarMensaje", script, true);
        }
    }
}