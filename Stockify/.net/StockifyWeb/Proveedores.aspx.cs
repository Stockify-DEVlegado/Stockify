using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Proveedores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarProveedores();
            }
        }

        protected void btnAddSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener valores del formulario
                string nombre = txtSupplierName.Text.Trim();
                string producto = txtProduct.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                string email = txtEmail.Text.Trim();
                string tipoEmpresa = ddlTipoEmpresa.SelectedValue;
                string activo = ddlActivo.SelectedValue;

                // Validaciones básicas
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(producto) || string.IsNullOrEmpty(telefono))
                {
                    MostrarMensaje("Por favor complete todos los campos obligatorios");
                    return;
                }

                // Validar formato de email
                if (!string.IsNullOrEmpty(email) && !EsEmailValido(email))
                {
                    MostrarMensaje("Por favor ingrese un email válido");
                    return;
                }

                // Guardar proveedor (aquí conectas con tu base de datos)
                GuardarProveedor(nombre, producto, telefono, email, tipoEmpresa, activo);

                // Limpiar formulario
                LimpiarFormulario();

                // Cerrar modal
                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal", "cerrarModal();", true);

                // Recargar proveedores
                CargarProveedores();

                MostrarMensaje("Proveedor agregado correctamente", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al agregar proveedor: " + ex.Message);
            }
        }

        private void GuardarProveedor(string nombre, string producto, string telefono, string email, string tipoEmpresa, string activo)
        {
            // Aquí va tu lógica para guardar en la base de datos
            System.Diagnostics.Debug.WriteLine($"Proveedor agregado:");
            System.Diagnostics.Debug.WriteLine($"Nombre: {nombre}");
            System.Diagnostics.Debug.WriteLine($"Producto: {producto}");
            System.Diagnostics.Debug.WriteLine($"Teléfono: {telefono}");
            System.Diagnostics.Debug.WriteLine($"Email: {email}");
            System.Diagnostics.Debug.WriteLine($"Tipo Empresa: {tipoEmpresa}");
            System.Diagnostics.Debug.WriteLine($"Activo: {activo}");
        }

        private void CargarProveedores()
        {
            // Datos de ejemplo basados en la imagen que proporcionaste
            var proveedores = new List<dynamic>
            {
                new { Nombre = "Changa SAC", Producto = "Monitor", Telefono = "7687784556", Email = "ChangaSAC@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "Knitos", Producto = "Teclados", Telefono = "9867545368", Email = "KnitosCorp@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "Swalone", Producto = "Processador Intel", Telefono = "9867545566", Email = "Sw_prod@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "Marteemo", Producto = "Processador Ryzen", Telefono = "9267545457", Email = "martCorp@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "Papetini", Producto = "Tarjetas de vídeo", Telefono = "9367546531", Email = "papetiniSAC@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "DrackFord", Producto = "Ventiladores", Telefono = "9667545982", Email = "DFDevices@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "ShangChun", Producto = "Monitores", Telefono = "9867545457", Email = "sangChun@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "Razzzer", Producto = "Teclados", Telefono = "9567545769", Email = "razzz@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "Gygaware", Producto = "Motherboard", Telefono = "9667545980", Email = "gygaWare@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "Asus", Producto = "Laptops", Telefono = "9867545457", Email = "asus123@gmail.com", TipoEmpresa = "---", Activo = "Si" },
                new { Nombre = "Impacto", Producto = "Unidad Almacen.", Telefono = "9567545769", Email = "Impactados@gmail.com", TipoEmpresa = "---", Activo = "No" },
                new { Nombre = "CS", Producto = "Apple", Telefono = "9667545980", Email = "cs.c#@gmail.com", TipoEmpresa = "---", Activo = "No" }
            };

            gvProveedores.DataSource = proveedores;
            gvProveedores.DataBind();
        }

        private void LimpiarFormulario()
        {
            txtSupplierName.Text = "";
            txtProduct.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            ddlTipoEmpresa.SelectedIndex = 0;
            ddlActivo.SelectedIndex = 0;
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