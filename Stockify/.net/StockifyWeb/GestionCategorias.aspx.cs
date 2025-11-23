using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using StockifyWeb.StockifyWS;

namespace StockifyWeb
{
    public partial class GestionCategorias : System.Web.UI.Page
    {
        private CategoriaWSClient clienteCategoria;

        public GestionCategorias()
        {
            this.clienteCategoria = new CategoriaWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar que el usuario esté logueado
                if (Session["IdUsuario"] == null)
                {
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                string tipoUsuario = Session["TipoUsuario"]?.ToString();

                // Solo ADMINISTRADOR y PRINCIPAL pueden acceder
                if (tipoUsuario != "ADMINISTRADOR" && tipoUsuario != "PRINCIPAL")
                {
                    Response.Redirect("Inicio.aspx", false);
                    return;
                }

                CargarCategorias();
                CargarCategoriasEnDropdown();
            }
        }

        private void CargarCategorias()
        {
            try
            {
                var categorias = clienteCategoria.listarCategorias();

                if (categorias == null || categorias.Length == 0)
                {
                    rptCategorias.Visible = false;
                    pnlEmpty.Visible = true;
                    return;
                }

                var listaCategorias = categorias.Select(c => new
                {
                    IdCategoria = c.idCategoria,
                    Nombre = c.nombre ?? "Sin nombre",
                    IdPadre = c.categoria1?.idCategoria,
                    NombrePadre = c.categoria1?.nombre
                })
                .OrderBy(c => c.NombrePadre ?? "")
                .ThenBy(c => c.Nombre)
                .ToList();

                rptCategorias.DataSource = listaCategorias;
                rptCategorias.DataBind();
                rptCategorias.Visible = true;
                pnlEmpty.Visible = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar categorías: {ex.Message}");
                MostrarError("Error al cargar las categorías.");
            }
        }

        private void CargarCategoriasEnDropdown()
        {
            try
            {
                var categorias = clienteCategoria.listarCategorias();

                ddlCategoriaPadre.Items.Clear();
                ddlCategoriaPadre.Items.Add(new ListItem("-- Ninguna (Categoría Principal) --", ""));

                if (categorias != null && categorias.Length > 0)
                {
                    // Solo mostrar categorías principales (sin padre)
                    var categoriasPrincipales = categorias
                        .Where(c => c.categoria1 == null)
                        .OrderBy(c => c.nombre)
                        .ToList();

                    foreach (var cat in categoriasPrincipales)
                    {
                        ddlCategoriaPadre.Items.Add(new ListItem(cat.nombre, cat.idCategoria.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar dropdown: {ex.Message}");
            }
        }

        protected void btnGuardarCategoria_Click(object sender, EventArgs e)
        {
            bool modoEdicion = hfModoEdicion.Value == "true";

            if (modoEdicion)
            {
                EditarCategoria();
            }
            else
            {
                CrearCategoria();
            }
        }

        private void CrearCategoria()
        {
            string nombre = txtNombre.Text.Trim();
            string idPadreStr = ddlCategoriaPadre.SelectedValue;

            try
            {
                // Validar que el nombre no esté duplicado
                if (CategoriaExiste(nombre, 0))
                {
                    MostrarError("Ya existe una categoría con ese nombre.");
                    return;
                }

                categoria nuevaCategoria = new categoria
                {
                    nombre = nombre
                };

                // Si tiene categoría padre
                if (!string.IsNullOrEmpty(idPadreStr))
                {
                    int idPadre = int.Parse(idPadreStr);
                    var categoriaPadre = ObtenerCategoriaPorId(idPadre);

                    if (categoriaPadre != null)
                    {
                        nuevaCategoria.categoria1 = categoriaPadre;
                    }
                }

                clienteCategoria.guardarCategoria(nuevaCategoria, estado.NUEVO);

                Response.Redirect("GestionCategorias.aspx?success=Categoría creada exitosamente", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear categoría: {ex.Message}");
                MostrarError("Error al crear la categoría.");
            }
        }

        private void EditarCategoria()
        {
            int idCategoria = int.Parse(hfIdCategoria.Value);
            string nombre = txtNombre.Text.Trim();
            string idPadreStr = ddlCategoriaPadre.SelectedValue;

            try
            {
                var categorias = clienteCategoria.listarCategorias();
                var categoriaActual = categorias.FirstOrDefault(c => c.idCategoria == idCategoria);

                if (categoriaActual == null)
                {
                    MostrarError("Categoría no encontrada.");
                    return;
                }

                // Validar que el nombre no esté duplicado (excepto la misma categoría)
                if (CategoriaExiste(nombre, idCategoria))
                {
                    MostrarError("Ya existe otra categoría con ese nombre.");
                    return;
                }

                // Validar que no se establezca como padre de sí misma
                if (!string.IsNullOrEmpty(idPadreStr))
                {
                    int idPadre = int.Parse(idPadreStr);
                    if (idPadre == idCategoria)
                    {
                        MostrarError("Una categoría no puede ser padre de sí misma.");
                        return;
                    }

                    // Validar que no se cree un ciclo (si la categoría actual es padre de la que se quiere asignar)
                    if (EsHijaDe(idPadre, idCategoria))
                    {
                        MostrarError("No se puede crear una relación circular entre categorías.");
                        return;
                    }
                }

                categoriaActual.nombre = nombre;

                // Actualizar categoría padre
                if (!string.IsNullOrEmpty(idPadreStr))
                {
                    int idPadre = int.Parse(idPadreStr);
                    var categoriaPadre = ObtenerCategoriaPorId(idPadre);
                    categoriaActual.categoria1 = categoriaPadre;
                }
                else
                {
                    categoriaActual.categoria1 = null;
                }

                clienteCategoria.guardarCategoria(categoriaActual, estado.MODIFICADO);

                Response.Redirect("GestionCategorias.aspx?success=Categoría actualizada exitosamente", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al editar categoría: {ex.Message}");
                MostrarError("Error al editar la categoría.");
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            int idCategoriaEliminar = int.Parse(hfIdEliminar.Value);

            try
            {
                var categorias = clienteCategoria.listarCategorias();
                var categoriaEliminar = categorias.FirstOrDefault(c => c.idCategoria == idCategoriaEliminar);

                if (categoriaEliminar == null)
                {
                    MostrarError("Categoría no encontrada.");
                    return;
                }

                // Verificar si tiene subcategorías
                bool tieneSubcategorias = categorias.Any(c =>
                    c.categoria1 != null && c.categoria1.idCategoria == idCategoriaEliminar);

                if (tieneSubcategorias)
                {
                    MostrarError("No se puede eliminar la categoría porque tiene subcategorías asociadas. Elimine primero las subcategorías.");
                    return;
                }

                // TODO: Verificar si tiene productos asociados
                // Si tienes acceso al cliente de productos, deberías verificar aquí
                // bool tieneProductos = VerificarProductosAsociados(idCategoriaEliminar);
                // if (tieneProductos) { MostrarError("..."); return; }

                clienteCategoria.eliminarCategoria(idCategoriaEliminar);

                Response.Redirect("GestionCategorias.aspx?success=Categoría eliminada exitosamente", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar categoría: {ex.Message}");
                MostrarError("Error al eliminar la categoría. Puede que tenga productos asociados.");
            }
        }

        // ==================== MÉTODOS AUXILIARES ====================

        private bool CategoriaExiste(string nombre, int idCategoriaActual)
        {
            try
            {
                var categorias = clienteCategoria.listarCategorias();
                return categorias != null && categorias.Any(c =>
                    c.nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                    c.idCategoria != idCategoriaActual);
            }
            catch
            {
                return false;
            }
        }

        private categoria ObtenerCategoriaPorId(int idCategoria)
        {
            try
            {
                var categorias = clienteCategoria.listarCategorias();
                return categorias?.FirstOrDefault(c => c.idCategoria == idCategoria);
            }
            catch
            {
                return null;
            }
        }

        private bool EsHijaDe(int idCategoriaPotencialHija, int idCategoriaPadre)
        {
            try
            {
                var categorias = clienteCategoria.listarCategorias();
                var categoria = categorias?.FirstOrDefault(c => c.idCategoria == idCategoriaPotencialHija);

                while (categoria != null && categoria.categoria1 != null)
                {
                    if (categoria.categoria1.idCategoria == idCategoriaPadre)
                    {
                        return true;
                    }
                    categoria = categoria.categoria1;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void MostrarError(string mensaje)
        {
            Response.Redirect($"GestionCategorias.aspx?error={Uri.EscapeDataString(mensaje)}", false);
        }
    }
}