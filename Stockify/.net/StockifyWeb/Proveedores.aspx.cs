using StockifyWeb.StockifyWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Proveedores : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
            }
        }

        private async Task CargarProveedoresAsync()
        {
            try
            {
                EmpresaWSClient cliente = new EmpresaWSClient();

                var response = await cliente.listarEmpresasAsync();
                var empresas = response.@return;

                if (empresas != null && empresas.Length > 0)
                {
                    // Mapear TODOS los datos desde el Web Service
                    var listaEmpresas = empresas.Select(e => new
                    {
                        IdEmpresa = e.idEmpresa,
                        Nombre = e.razonSocial ?? "Sin nombre",
                        Telefono = e.telefono ?? "Sin teléfono",
                        Email = e.email ?? "Sin email",
                        TipoEmpresa = e.tipoEmpresaSpecified ? e.tipoEmpresa.ToString() : "N/A",
                        TipoDocumento = e.tipoDocumento.ToString(),
                        Activo = e.activo ? "Si" : "No"
                    }).ToList();

                    gvProveedores.DataSource = listaEmpresas;
                    gvProveedores.DataBind();

                    System.Diagnostics.Debug.WriteLine($"✅ Cargadas {listaEmpresas.Count} empresas desde el Web Service");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No hay empresas en la base de datos");
                    gvProveedores.DataSource = new List<object>();
                    gvProveedores.DataBind();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error de conexión: {ex.Message}");
                CargarEmpresasEjemplo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                CargarEmpresasEjemplo();
            }
        }

        protected void btnAddSupplier_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () => await AgregarEmpresaAsync()));
        }

        private async Task AgregarEmpresaAsync()
        {
            try
            {
                // Validar campos obligatorios
                string razonSocial = txtSupplierName.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                string email = txtEmail.Text.Trim();
                string tipoDocumentoStr = ddlTipoDocumento.SelectedValue;
                string tipoEmpresaStr = ddlTipoEmpresa.SelectedValue;
                bool activo = ddlActivo.SelectedValue.ToLower() == "si";

                // Validaciones
                if (string.IsNullOrEmpty(razonSocial))
                {
                    MostrarMensaje("Por favor ingrese la razón social");
                    return;
                }

                if (string.IsNullOrEmpty(tipoDocumentoStr))
                {
                    MostrarMensaje("Por favor seleccione el tipo de documento");
                    return;
                }

                if (string.IsNullOrEmpty(telefono))
                {
                    MostrarMensaje("Por favor ingrese el teléfono");
                    return;
                }

                if (string.IsNullOrEmpty(email))
                {
                    MostrarMensaje("Por favor ingrese el email");
                    return;
                }

                if (!EsEmailValido(email))
                {
                    MostrarMensaje("Por favor ingrese un email válido");
                    return;
                }

                if (string.IsNullOrEmpty(tipoEmpresaStr))
                {
                    MostrarMensaje("Por favor seleccione el tipo de empresa");
                    return;
                }

                // Convertir tipo de documento
                tipoDocumento tipoDoc;
                if (!Enum.TryParse(tipoDocumentoStr, out tipoDoc))
                {
                    MostrarMensaje("Tipo de documento inválido");
                    return;
                }

                // Convertir tipo de empresa
                tipoEmpresa tipoEmp;
                if (!Enum.TryParse(tipoEmpresaStr, out tipoEmp))
                {
                    MostrarMensaje("Tipo de empresa inválido");
                    return;
                }

                await GuardarEmpresaAsync(razonSocial, telefono, email, tipoDoc, tipoEmp, activo);

                LimpiarFormulario();

                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal",
                    "if(typeof cerrarModal === 'function') cerrarModal();", true);

                await CargarProveedoresAsync();

                MostrarMensaje("Empresa agregada correctamente", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al agregar empresa: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Error al agregar: {ex.Message}");
            }
        }

        private async Task GuardarEmpresaAsync(string razonSocial, string telefono, string email,
            tipoDocumento tipoDoc, tipoEmpresa tipoEmp, bool activo)
        {
            try
            {
                EmpresaWSClient cliente = new EmpresaWSClient();

                var nuevaEmpresa = new empresa
                {
                    razonSocial = razonSocial,
                    telefono = telefono,
                    email = email,
                    activo = activo,
                    tipoDocumento = tipoDoc,
                    tipoEmpresa = tipoEmp,
                    tipoEmpresaSpecified = true
                };

                await cliente.guardarEmpresaAsync(nuevaEmpresa, estado.NUEVO);

                System.Diagnostics.Debug.WriteLine($"✅ Empresa guardada: {razonSocial}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al guardar: {ex.Message}");
                throw new Exception($"No se pudo guardar la empresa: {ex.Message}", ex);
            }
        }

        private void CargarEmpresasEjemplo()
        {
            var empresas = new List<dynamic>
            {
                new {
                    IdEmpresa = 10,
                    Nombre = "RazonSocialSACTest",
                    Telefono = "999999999",
                    Email = "test@pucp.edu.pe",
                    TipoEmpresa = "PROVEEDOR",
                    TipoDocumento = "DNI",
                    Activo = "Si"
                }
            };

            gvProveedores.DataSource = empresas;
            gvProveedores.DataBind();

            System.Diagnostics.Debug.WriteLine("⚠️ Cargando datos de ejemplo (sin conexión al WS)");
        }

        private void LimpiarFormulario()
        {
            txtSupplierName.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            ddlTipoDocumento.SelectedIndex = 0;
            ddlTipoEmpresa.SelectedIndex = 0;
            ddlActivo.SelectedIndex = 0;
        }

        private void MostrarMensaje(string mensaje, bool esExitoso = false)
        {
            mensaje = mensaje.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"alert('{mensaje}');";

            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarMensaje", script, true);
        }

        private bool EsEmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected void EliminarEmpresa_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                try
                {
                    Button btn = (Button)sender;
                    int idEmpresa = Convert.ToInt32(btn.CommandArgument);

                    EmpresaWSClient cliente = new EmpresaWSClient();
                    await cliente.eliminarEmpresaAsync(idEmpresa);

                    await CargarProveedoresAsync();
                    MostrarMensaje("Empresa eliminada correctamente", true);
                }
                catch (Exception ex)
                {
                    MostrarMensaje($"Error al eliminar empresa: {ex.Message}");
                }
            }));
        }
    }
}