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
            EmpresaWSClient cliente = null;
            try
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("📥 CARGANDO EMPRESAS DESDE WS");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                gvProveedores.DataSource = null;
                gvProveedores.DataBind();
                System.Diagnostics.Debug.WriteLine("🧹 GridView limpiado");

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
                    gvProveedores.DataSource = new List<object>();
                    gvProveedores.DataBind();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"📊 Total empresas recibidas del WS: {empresas.Length}");

                if (empresas.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("📋 Detalle de empresas recibidas:");

                    var listaEmpresas = new List<object>();

                    for (int i = 0; i < empresas.Length; i++)
                    {
                        var e = empresas[i];

                        System.Diagnostics.Debug.WriteLine($"   [{i + 1}] ID: {e.idEmpresa}, Razón: '{e.razonSocial ?? "NULL"}', TipoDoc: {(e.tipoDocumentoSpecified ? e.tipoDocumento.ToString() : "NULL")}");

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

                    gvProveedores.DataSource = listaEmpresas;
                    gvProveedores.DataBind();

                    System.Diagnostics.Debug.WriteLine($"✅ GridView enlazado correctamente");
                    System.Diagnostics.Debug.WriteLine($"   Filas en GridView después de DataBind: {gvProveedores.Rows.Count}");
                    System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No hay empresas en la base de datos");
                    gvProveedores.DataSource = new List<object>();
                    gvProveedores.DataBind();
                    System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                }
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

                System.Diagnostics.Debug.WriteLine($"📝 Datos ingresados:");
                System.Diagnostics.Debug.WriteLine($"   Razón Social: '{razonSocial}'");
                System.Diagnostics.Debug.WriteLine($"   Teléfono: '{telefono}'");
                System.Diagnostics.Debug.WriteLine($"   Email: '{email}'");
                System.Diagnostics.Debug.WriteLine($"   Tipo Documento: '{tipoDocumentoStr}'");
                System.Diagnostics.Debug.WriteLine($"   Tipo Empresa: '{tipoEmpresaStr}'");
                System.Diagnostics.Debug.WriteLine($"   Activo: {activo}");

                // Validaciones
                if (string.IsNullOrEmpty(razonSocial))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Validación fallida: Razón social vacía");
                    MostrarMensaje("Por favor ingrese la razón social");
                    return;
                }

                if (string.IsNullOrEmpty(tipoDocumentoStr))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Validación fallida: Tipo documento no seleccionado");
                    MostrarMensaje("Por favor seleccione el tipo de documento");
                    return;
                }

                if (string.IsNullOrEmpty(telefono))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Validación fallida: Teléfono vacío");
                    MostrarMensaje("Por favor ingrese el teléfono");
                    return;
                }

                if (string.IsNullOrEmpty(email))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Validación fallida: Email vacío");
                    MostrarMensaje("Por favor ingrese el email");
                    return;
                }

                if (!EsEmailValido(email))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Validación fallida: Email inválido");
                    MostrarMensaje("Por favor ingrese un email válido");
                    return;
                }

                if (string.IsNullOrEmpty(tipoEmpresaStr))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Validación fallida: Tipo empresa no seleccionado");
                    MostrarMensaje("Por favor seleccione el tipo de empresa");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("✅ Validaciones pasadas");

                tipoDocumento tipoDoc;
                if (!Enum.TryParse(tipoDocumentoStr, true, out tipoDoc))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error: No se pudo convertir '{tipoDocumentoStr}' a tipoDocumento");
                    MostrarMensaje("Tipo de documento inválido");
                    return;
                }

                tipoEmpresa tipoEmp;
                if (!Enum.TryParse(tipoEmpresaStr, true, out tipoEmp))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error: No se pudo convertir '{tipoEmpresaStr}' a tipoEmpresa");
                    MostrarMensaje("Tipo de empresa inválido");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("💾 Guardando empresa en el WS...");
                await GuardarEmpresaAsync(razonSocial, telefono, email, tipoDoc, tipoEmp, activo);

                System.Diagnostics.Debug.WriteLine("✅ Empresa guardada exitosamente");
                System.Diagnostics.Debug.WriteLine("🔄 Recargando lista de empresas...");
                await CargarProveedoresAsync();

                System.Diagnostics.Debug.WriteLine($"✅ Lista recargada - Total filas en GridView: {gvProveedores.Rows.Count}");
                System.Diagnostics.Debug.WriteLine("🧹 Limpiando formulario...");
                LimpiarFormulario();

                System.Diagnostics.Debug.WriteLine("🎉 Proceso completado exitosamente");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModalYRecargar",
                    "if(typeof cerrarModal === 'function') { cerrarModal(); }", true);

                MostrarMensaje("Empresa agregada correctamente", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine($"❌ ERROR AL AGREGAR EMPRESA");
                System.Diagnostics.Debug.WriteLine($"   Tipo: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"   Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

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

                System.Diagnostics.Debug.WriteLine($"📝 Datos a actualizar:");
                System.Diagnostics.Debug.WriteLine($"   ID Empresa: {idEmpresa}");
                System.Diagnostics.Debug.WriteLine($"   Razón Social: '{razonSocial}'");
                System.Diagnostics.Debug.WriteLine($"   Teléfono: '{telefono}'");
                System.Diagnostics.Debug.WriteLine($"   Email: '{email}'");
                System.Diagnostics.Debug.WriteLine($"   Tipo Documento: '{tipoDocumentoStr}'");
                System.Diagnostics.Debug.WriteLine($"   Tipo Empresa: '{tipoEmpresaStr}'");
                System.Diagnostics.Debug.WriteLine($"   Activo: {activo}");

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

                System.Diagnostics.Debug.WriteLine("💾 Actualizando empresa en el WS...");

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

                    System.Diagnostics.Debug.WriteLine("📤 Enviando datos actualizados al WS...");
                    await cliente.guardarEmpresaAsync(empresaActualizada, estado.MODIFICADO);

                    System.Diagnostics.Debug.WriteLine($"✅ Empresa actualizada en BD: {razonSocial}");
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

                System.Diagnostics.Debug.WriteLine("🔄 Recargando lista de empresas...");
                await CargarProveedoresAsync();

                LimpiarFormulario();
                hfModoEdicion.Value = "false";
                hfIdEmpresa.Value = "0";

                System.Diagnostics.Debug.WriteLine("🎉 Actualización completada exitosamente");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModalActualizar",
                    "if(typeof cerrarModal === 'function') { cerrarModal(); }", true);

                MostrarMensaje("Empresa actualizada correctamente", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine($"❌ ERROR AL ACTUALIZAR EMPRESA");
                System.Diagnostics.Debug.WriteLine($"   Tipo: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"   Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                MostrarMensaje($"Error al actualizar empresa: {ex.Message}");
            }
        }

        private async Task EliminarEmpresaAsync(int idEmpresa)
        {
            EmpresaWSClient cliente = null;
            try
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("🗑️ ELIMINANDO EMPRESA");
                System.Diagnostics.Debug.WriteLine($"   ID Empresa: {idEmpresa}");

                cliente = new EmpresaWSClient();

                System.Diagnostics.Debug.WriteLine("📤 Llamando al WS para eliminar...");
                await cliente.eliminarEmpresaAsync(idEmpresa);

                System.Diagnostics.Debug.WriteLine("✅ Empresa eliminada del backend");
                System.Diagnostics.Debug.WriteLine("🔄 Recargando lista de empresas...");

                await CargarProveedoresAsync();

                System.Diagnostics.Debug.WriteLine($"✅ Lista recargada - Total filas: {gvProveedores.Rows.Count}");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

                MostrarMensaje("Empresa eliminada correctamente", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine($"❌ ERROR AL ELIMINAR EMPRESA");
                System.Diagnostics.Debug.WriteLine($"   ID: {idEmpresa}");
                System.Diagnostics.Debug.WriteLine($"   Tipo: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"   Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════════");

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
                System.Diagnostics.Debug.WriteLine("📡 Creando cliente WS para guardar...");
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

                System.Diagnostics.Debug.WriteLine("📤 Datos a enviar al WS:");
                System.Diagnostics.Debug.WriteLine($"   Razón Social: {nuevaEmpresa.razonSocial}");
                System.Diagnostics.Debug.WriteLine($"   Teléfono: {nuevaEmpresa.telefono}");
                System.Diagnostics.Debug.WriteLine($"   Email: {nuevaEmpresa.email}");
                System.Diagnostics.Debug.WriteLine($"   Tipo Documento: {nuevaEmpresa.tipoDocumento} (Specified: {nuevaEmpresa.tipoDocumentoSpecified})");
                System.Diagnostics.Debug.WriteLine($"   Tipo Empresa: {nuevaEmpresa.tipoEmpresa} (Specified: {nuevaEmpresa.tipoEmpresaSpecified})");
                System.Diagnostics.Debug.WriteLine($"   Activo: {nuevaEmpresa.activo}");

                await cliente.guardarEmpresaAsync(nuevaEmpresa, estado.NUEVO);

                System.Diagnostics.Debug.WriteLine($"✅ Empresa guardada en BD: {razonSocial}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error en GuardarEmpresaAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
                }
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
                    Nombre = "Proveedor Test OrdenIngreso",
                    Telefono = "987654321",
                    Email = "proveedoringreso@test.com",
                    TipoEmpresa = "PROVEEDOR",
                    TipoDocumento = "RUC",
                    Activo = "Si"
                },
                new {
                    IdEmpresa = 3,
                    Nombre = "Cliente Test OrdenSalida",
                    Telefono = "987654321",
                    Email = "clientesalida@test.com",
                    TipoEmpresa = "CLIENTE",
                    TipoDocumento = "RUC",
                    Activo = "Si"
                },
                new {
                    IdEmpresa = 4,
                    Nombre = "RazonSocialSACTest",
                    Telefono = "999999999",
                    Email = "test@pucp.edu.pe",
                    TipoEmpresa = "CLIENTE",
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

            System.Diagnostics.Debug.WriteLine("✅ Formulario limpiado");
        }

        private void MostrarMensaje(string mensaje, bool esExitoso = false)
        {
            mensaje = mensaje.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
            string script = $"alert('{mensaje}');";

            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarMensaje", script, true);

            System.Diagnostics.Debug.WriteLine($"{(esExitoso ? "✅" : "⚠️")} Mensaje mostrado: {mensaje}");
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