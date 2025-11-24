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
        // Variable para almacenar temporalmente el DataSource completo (sin paginar)
        private List<dynamic> _datasourceCompleto;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
            }
            else
            {
                // Manejar eventos de paginación JavaScript
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];

                if (eventTarget == "IrAPagina" && !string.IsNullOrEmpty(eventArgument))
                {
                    if (int.TryParse(eventArgument, out int numeroPagina))
                    {
                        gvProveedores.PageIndex = numeroPagina - 1;
                        RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
                    }
                }
            }
        }

        private async Task CargarProveedoresAsync()
        {
            EmpresaWSClient cliente = null;
            try
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("📥 CARGANDO EMPRESAS DESDE WS");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                cliente = new EmpresaWSClient();
                System.Diagnostics.Debug.WriteLine($"✅ Cliente WS creado");
                System.Diagnostics.Debug.WriteLine($"   Endpoint: {cliente.Endpoint.Address.Uri}");

                System.Diagnostics.Debug.WriteLine("🔄 Llamando a listarEmpresasAsync()...");
                var response = await cliente.listarEmpresasAsync();
                System.Diagnostics.Debug.WriteLine("✅ Respuesta recibida del WS");

                var empresas = response.@return;

                if (empresas == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ La respuesta del WS es NULL");
                    _datasourceCompleto = new List<dynamic>();
                    gvProveedores.DataSource = _datasourceCompleto;
                    gvProveedores.DataBind();
                    ActualizarInformacionPaginacion();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"📊 Total empresas recibidas del WS: {empresas.Length}");

                var listaEmpresas = new List<dynamic>();

                for (int i = 0; i < empresas.Length; i++)
                {
                    var e = empresas[i];

                    System.Diagnostics.Debug.WriteLine($"   [{i + 1}] ID: {e.idEmpresa}, Razón: '{e.razonSocial ?? "NULL"}'");

                    string tipoDoc = "N/A";
                    if (e.tipoDocumentoSpecified)
                    {
                        tipoDoc = e.tipoDocumento.ToString();
                    }

                    string tipoEmp = "N/A";
                    if (e.tipoEmpresaSpecified)
                    {
                        tipoEmp = e.tipoEmpresa.ToString();
                    }

                    listaEmpresas.Add(new
                    {
                        IdEmpresa = e.idEmpresa,
                        Nombre = string.IsNullOrEmpty(e.razonSocial) ? "Sin nombre" : e.razonSocial,
                        Telefono = string.IsNullOrEmpty(e.telefono) ? "Sin teléfono" : e.telefono,
                        Email = string.IsNullOrEmpty(e.email) ? "Sin email" : e.email,
                        TipoEmpresa = tipoEmp,
                        TipoDocumento = tipoDoc,
                        Activo = e.activo ? "Si" : "No"
                    });
                }

                System.Diagnostics.Debug.WriteLine($"✅ Lista mapeada con {listaEmpresas.Count} elementos");

                _datasourceCompleto = listaEmpresas;
                gvProveedores.DataSource = _datasourceCompleto;
                gvProveedores.DataBind();

                System.Diagnostics.Debug.WriteLine($"✅ GridView enlazado correctamente");
                System.Diagnostics.Debug.WriteLine($"   Filas en GridView después de DataBind: {gvProveedores.Rows.Count}");

                ActualizarInformacionPaginacion();

                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine($"❌ Error de conexión con WS");
                System.Diagnostics.Debug.WriteLine($"   Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine("   Cargando datos de ejemplo...");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                CargarEmpresasEjemplo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine($"❌ Error inesperado: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Tipo: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("   Cargando datos de ejemplo...");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                CargarEmpresasEjemplo();
            }
            finally
            {
                if (cliente != null && cliente.State == System.ServiceModel.CommunicationState.Opened)
                {
                    try
                    {
                        cliente.Close();
                    }
                    catch
                    {
                        cliente.Abort();
                    }
                }
            }
        }

        protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idEmpresa = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EliminarEmpresa")
            {
                RegisterAsyncTask(new PageAsyncTask(async () => await EliminarEmpresaAsync(idEmpresa)));
            }
        }

        protected void btnAddSupplier_Click(object sender, EventArgs e)
        {
            bool modoEdicion = hfModoEdicion.Value.ToLower() == "true";

            if (modoEdicion)
            {
                RegisterAsyncTask(new PageAsyncTask(ActualizarEmpresaAsync));
            }
            else
            {
                RegisterAsyncTask(new PageAsyncTask(AgregarEmpresaAsync));
            }
        }

        private async Task AgregarEmpresaAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("➕ AGREGANDO NUEVA EMPRESA");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

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

                tipoDocumento tipoDoc;
                if (!Enum.TryParse(tipoDocumentoStr, true, out tipoDoc))
                {
                    MostrarMensaje("Tipo de documento inválido");
                    return;
                }

                tipoEmpresa tipoEmp;
                if (!Enum.TryParse(tipoEmpresaStr, true, out tipoEmp))
                {
                    MostrarMensaje("Tipo de empresa inválido");
                    return;
                }

                await GuardarEmpresaAsync(razonSocial, telefono, email, tipoDoc, tipoEmp, activo);
                await CargarProveedoresAsync();

                LimpiarFormulario();

                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModalYRecargar",
                    "if(typeof cerrarModal === 'function') { cerrarModal(); }", true);

                MostrarMensaje("Empresa agregada correctamente", true);
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR AL AGREGAR EMPRESA: {ex.Message}");
                MostrarMensaje($"Error al agregar empresa: {ex.Message}");
            }
        }

        private async Task ActualizarEmpresaAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("✏️ ACTUALIZANDO EMPRESA");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                int idEmpresa = Convert.ToInt32(hfIdEmpresa.Value);
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

                tipoDocumento tipoDoc;
                if (!Enum.TryParse(tipoDocumentoStr, true, out tipoDoc))
                {
                    MostrarMensaje("Tipo de documento inválido");
                    return;
                }

                tipoEmpresa tipoEmp;
                if (!Enum.TryParse(tipoEmpresaStr, true, out tipoEmp))
                {
                    MostrarMensaje("Tipo de empresa inválido");
                    return;
                }

                EmpresaWSClient cliente = null;
                try
                {
                    cliente = new EmpresaWSClient();

                    var empresaActualizada = new empresa
                    {
                        idEmpresa = idEmpresa,
                        razonSocial = razonSocial,
                        telefono = telefono,
                        email = email,
                        activo = activo,
                        tipoDocumento = tipoDoc,
                        tipoDocumentoSpecified = true,
                        tipoEmpresa = tipoEmp,
                        tipoEmpresaSpecified = true
                    };

                    await cliente.guardarEmpresaAsync(empresaActualizada, estado.MODIFICADO);
                    System.Diagnostics.Debug.WriteLine($"✅ Empresa actualizada: {razonSocial}");
                }
                finally
                {
                    if (cliente != null && cliente.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        try
                        {
                            await Task.Delay(500);
                            cliente.Close();
                        }
                        catch
                        {
                            cliente.Abort();
                        }
                    }
                }

                await CargarProveedoresAsync();

                LimpiarFormulario();
                hfModoEdicion.Value = "false";
                hfIdEmpresa.Value = "0";

                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModalActualizar",
                    "if(typeof cerrarModal === 'function') { cerrarModal(); }", true);

                MostrarMensaje("Empresa actualizada correctamente", true);
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR AL ACTUALIZAR EMPRESA: {ex.Message}");
                MostrarMensaje($"Error al actualizar empresa: {ex.Message}");
            }
        }

        private async Task EliminarEmpresaAsync(int idEmpresa)
        {
            EmpresaWSClient cliente = null;
            try
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine($"🗑️ ELIMINANDO EMPRESA ID: {idEmpresa}");

                cliente = new EmpresaWSClient();
                await cliente.eliminarEmpresaAsync(idEmpresa);

                System.Diagnostics.Debug.WriteLine("✅ Empresa eliminada");
                await CargarProveedoresAsync();

                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                MostrarMensaje("Empresa eliminada correctamente", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR AL ELIMINAR: {ex.Message}");
                MostrarMensaje($"Error al eliminar empresa: {ex.Message}");
            }
            finally
            {
                if (cliente != null && cliente.State == System.ServiceModel.CommunicationState.Opened)
                {
                    try
                    {
                        await Task.Delay(500);
                        cliente.Close();
                    }
                    catch
                    {
                        cliente.Abort();
                    }
                }
            }
        }

        private async Task GuardarEmpresaAsync(string razonSocial, string telefono, string email,
            tipoDocumento tipoDoc, tipoEmpresa tipoEmp, bool activo)
        {
            EmpresaWSClient cliente = null;
            try
            {
                cliente = new EmpresaWSClient();

                var nuevaEmpresa = new empresa
                {
                    razonSocial = razonSocial,
                    telefono = telefono,
                    email = email,
                    activo = activo,
                    tipoDocumento = tipoDoc,
                    tipoDocumentoSpecified = true,
                    tipoEmpresa = tipoEmp,
                    tipoEmpresaSpecified = true
                };

                await cliente.guardarEmpresaAsync(nuevaEmpresa, estado.NUEVO);
                System.Diagnostics.Debug.WriteLine($"✅ Empresa guardada: {razonSocial}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error en GuardarEmpresaAsync: {ex.Message}");
                throw new Exception($"No se pudo guardar la empresa: {ex.Message}", ex);
            }
            finally
            {
                if (cliente != null && cliente.State == System.ServiceModel.CommunicationState.Opened)
                {
                    try
                    {
                        await Task.Delay(500);
                        cliente.Close();
                    }
                    catch
                    {
                        cliente.Abort();
                    }
                }
            }
        }

        private void CargarEmpresasEjemplo()
        {
            var empresas = new List<dynamic>
            {
                new {
                    IdEmpresa = 1,
                    Nombre = "RazonSocialSACTest",
                    Telefono = "999999999",
                    Email = "test@pucp.edu.pe",
                    TipoEmpresa = "PROVEEDOR",
                    TipoDocumento = "DNI",
                    Activo = "Si"
                },
                new {
                    IdEmpresa = 2,
                    Nombre = "Proveedor Test",
                    Telefono = "987654321",
                    Email = "proveedor@test.com",
                    TipoEmpresa = "PROVEEDOR",
                    TipoDocumento = "RUC",
                    Activo = "Si"
                },
                new {
                    IdEmpresa = 3,
                    Nombre = "Cliente Test",
                    Telefono = "987654321",
                    Email = "cliente@test.com",
                    TipoEmpresa = "CLIENTE",
                    TipoDocumento = "RUC",
                    Activo = "Si"
                }
            };

            _datasourceCompleto = empresas;
            gvProveedores.DataSource = _datasourceCompleto;
            gvProveedores.DataBind();
            ActualizarInformacionPaginacion();

            System.Diagnostics.Debug.WriteLine("⚠️ Cargando datos de ejemplo (sin conexión al WS)");
        }

        // ==================== MÉTODOS DE PAGINACIÓN ====================

        protected void gvProveedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProveedores.PageIndex = e.NewPageIndex;
            RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
        }

        protected void btnPrimeraPagina_Click(object sender, EventArgs e)
        {
            gvProveedores.PageIndex = 0;
            RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
        }

        protected void btnPaginaAnterior_Click(object sender, EventArgs e)
        {
            if (gvProveedores.PageIndex > 0)
            {
                gvProveedores.PageIndex--;
                RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
            }
        }

        protected void btnPaginaSiguiente_Click(object sender, EventArgs e)
        {
            if (gvProveedores.PageIndex < gvProveedores.PageCount - 1)
            {
                gvProveedores.PageIndex++;
                RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
            }
        }

        protected void btnUltimaPagina_Click(object sender, EventArgs e)
        {
            gvProveedores.PageIndex = gvProveedores.PageCount - 1;
            RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvProveedores.PageSize = int.Parse(ddlPageSize.SelectedValue);
            gvProveedores.PageIndex = 0;
            RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
        }

        private void ActualizarInformacionPaginacion()
        {
            if (gvProveedores.Rows.Count == 0)
            {
                litPaginaActual.Text = "0";
                litPaginaTotal.Text = "0";
                litTotalEmpresas.Text = "0";
                litNumeroPaginas.Text = "";

                btnPrimeraPagina.Enabled = false;
                btnPaginaAnterior.Enabled = false;
                btnPaginaSiguiente.Enabled = false;
                btnUltimaPagina.Enabled = false;
                return;
            }

            int totalEmpresas = ObtenerTotalRegistros();
            int paginaActual = gvProveedores.PageIndex + 1;
            int totalPaginas = gvProveedores.PageCount;
            int pageSize = gvProveedores.PageSize;

            int registroInicio = (gvProveedores.PageIndex * pageSize) + 1;
            int registroFin = Math.Min((gvProveedores.PageIndex + 1) * pageSize, totalEmpresas);

            litPaginaActual.Text = registroInicio.ToString();
            litPaginaTotal.Text = registroFin.ToString();
            litTotalEmpresas.Text = totalEmpresas.ToString();

            GenerarBotonesNumeroPagina(paginaActual, totalPaginas);

            btnPrimeraPagina.Enabled = paginaActual > 1;
            btnPaginaAnterior.Enabled = paginaActual > 1;
            btnPaginaSiguiente.Enabled = paginaActual < totalPaginas;
            btnUltimaPagina.Enabled = paginaActual < totalPaginas;

            System.Diagnostics.Debug.WriteLine($"📄 Paginación - Página {paginaActual}/{totalPaginas}, Mostrando {registroInicio}-{registroFin} de {totalEmpresas}");
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

            if (rangoInicio > 1)
            {
                html.Append($"<button type='button' class='pagination-button' onclick='irAPagina(1)'>1</button>");
                if (rangoInicio > 2)
                {
                    html.Append("<span style='color: var(--muted); padding: 0 8px;'>...</span>");
                }
            }

            for (int i = rangoInicio; i <= rangoFin; i++)
            {
                string activeClass = i == paginaActual ? "active" : "";
                html.Append($"<button type='button' class='pagination-button {activeClass}' onclick='irAPagina({i})'>{i}</button>");
            }

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

        private int ObtenerTotalRegistros()
        {
            if (_datasourceCompleto != null)
            {
                return _datasourceCompleto.Count;
            }

            var dataSource = gvProveedores.DataSource;
            if (dataSource is System.Collections.IEnumerable enumerable)
            {
                return enumerable.Cast<object>().Count();
            }
            return gvProveedores.Rows.Count;
        }

        // ==================== MÉTODOS AUXILIARES ====================

        private void LimpiarFormulario()
        {
            txtSupplierName.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            ddlTipoDocumento.SelectedIndex = 0;
            ddlTipoEmpresa.SelectedIndex = 0;
            ddlActivo.SelectedIndex = 0;

            System.Diagnostics.Debug.WriteLine("✅ Formulario limpiado");
        }

        private void MostrarMensaje(string mensaje, bool esExitoso = false)
        {
            mensaje = mensaje.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"alert('{mensaje}');";

            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarMensaje", script, true);

            System.Diagnostics.Debug.WriteLine($"{(esExitoso ? "✅" : "⚠️")} Mensaje: {mensaje}");
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
    }
}