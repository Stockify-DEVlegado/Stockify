using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StockifyWeb
{
    public partial class Ordenes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MostrarCompra();
                CargarDatosIniciales();
            }
        }

        private void CargarDatosIniciales()
        {
            CargarOrdenesCompra();
            CargarOrdenesVenta();
            CargarRegistrosIngreso();
            CargarRegistrosSalida();
            CargarDetalleOrdenVacia();

            // Establecer fechas por defecto
            txtFechaOrdenCompra.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            txtFechaOrdenVenta.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            // Establecer fechas por defecto para ingreso y salida
            txtFechaIngreso.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaSalida.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void CargarOrdenesCompra()
        {
            var ordenes = new List<object>
            {
                new {
                    Codigo = "PO-2025-001",
                    FechaRegistrada = "2025-01-01",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Diego Alvarez Castillo",
                    Total = "7500 $",
                    Estado = "Procesando"
                },
                new {
                    Codigo = "PO-2025-002",
                    FechaRegistrada = "2025-01-02",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Patrick Cruz Apolaya",
                    Total = "4500 $",
                    Estado = "Cancelado"
                },
                new {
                    Codigo = "PO-2025-003",
                    FechaRegistrada = "2025-01-03",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Alonso Chipana Cuellar",
                    Total = "10000 $",
                    Estado = "Aceptado"
                }
            };

            gvOrdenesCompra.DataSource = ordenes;
            gvOrdenesCompra.DataBind();
        }

        private void CargarOrdenesVenta()
        {
            var ordenes = new List<object>
            {
                new {
                    Codigo = "SO-2025-001",
                    FechaRegistrada = "2025-01-01",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Diego Alvarez Castillo",
                    Total = "7500 $",
                    Estado = "Procesando"
                },
                new {
                    Codigo = "SO-2025-002",
                    FechaRegistrada = "2025-01-02",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Patrick Cruz Apolaya",
                    Total = "4500 $",
                    Estado = "Cancelado"
                },
                new {
                    Codigo = "SO-2025-003",
                    FechaRegistrada = "2025-01-03",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Alonso Chipana Cuellar",
                    Total = "10000 $",
                    Estado = "Aceptado"
                }
            };

            gvOrdenesVenta.DataSource = ordenes;
            gvOrdenesVenta.DataBind();
        }

        private void CargarRegistrosIngreso()
        {
            var registros = new List<object>
            {
                new {
                    Codigo = "ING-2025-001",
                    FechaRegistrada = "2025-01-01",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Diego Alvarez Castillo",
                    Total = "7500 $",
                    Estado = "Procesando"
                },
                new {
                    Codigo = "ING-2025-002",
                    FechaRegistrada = "2025-01-02",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Patrick Cruz Apolaya",
                    Total = "4500 $",
                    Estado = "Cancelado"
                }
            };

            gvRegistrosIngreso.DataSource = registros;
            gvRegistrosIngreso.DataBind();
        }

        private void CargarRegistrosSalida()
        {
            var registros = new List<object>
            {
                new {
                    Codigo = "SAL-2025-001",
                    FechaRegistrada = "2025-01-03",
                    Nombre = "BHAVANI SALES CORPORATION",
                    Responsable = "Alonso Chipana Cuellar",
                    Total = "10000 $",
                    Estado = "Aceptado"
                }
            };

            gvRegistrosSalida.DataSource = registros;
            gvRegistrosSalida.DataBind();
        }

        private void CargarDetalleOrdenVacia()
        {
            gvDetalleOrdenCompra.DataSource = new List<object>();
            gvDetalleOrdenCompra.DataBind();
            gvDetalleOrdenVenta.DataSource = new List<object>();
            gvDetalleOrdenVenta.DataBind();
            gvDetalleOrdenCompraIngreso.DataSource = new List<object>();
            gvDetalleOrdenCompraIngreso.DataBind();
            gvDetalleOrdenVentaSalida.DataSource = new List<object>();
            gvDetalleOrdenVentaSalida.DataBind();
        }

        private void CargarDetalleOrdenCompra(string codigoOrden)
        {
            var detalle = ObtenerDetalleOrdenCompra(codigoOrden);
            gvDetalleOrdenCompra.DataSource = detalle;
            gvDetalleOrdenCompra.DataBind();
        }

        private void CargarDetalleOrdenVenta(string codigoOrden)
        {
            var detalle = ObtenerDetalleOrdenVenta(codigoOrden);
            gvDetalleOrdenVenta.DataSource = detalle;
            gvDetalleOrdenVenta.DataBind();
        }

        private void CargarDetalleOrdenCompraIngreso(string codigoOrden)
        {
            var detalle = ObtenerDetalleOrdenCompra(codigoOrden);
            gvDetalleOrdenCompraIngreso.DataSource = detalle;
            gvDetalleOrdenCompraIngreso.DataBind();
        }

        private void CargarDetalleOrdenVentaSalida(string codigoOrden)
        {
            var detalle = ObtenerDetalleOrdenVenta(codigoOrden);
            gvDetalleOrdenVentaSalida.DataSource = detalle;
            gvDetalleOrdenVentaSalida.DataBind();
        }

        // MÉTODO GETBADGECLASS - REQUERIDO PARA LOS BADGES
        public string GetBadgeClass(string estado)
        {
            if (string.IsNullOrEmpty(estado))
                return "badge";

            switch (estado.ToLower())
            {
                case "procesando":
                    return "badge badge-procesando";
                case "cancelado":
                    return "badge badge-cancelado";
                case "aceptado":
                    return "badge badge-aceptado";
                case "disponible":
                    return "badge badge-aceptado";
                case "entregado":
                    return "badge badge-aceptado";
                case "en camino":
                    return "badge badge-procesando";
                default:
                    return "badge";
            }
        }

        // NUEVOS MÉTODOS PARA MANEJAR LOS DROPDOWN DE INGRESO/SALIDA
        protected void ddlOrdenCompraIngreso_SelectedIndexChanged(object sender, EventArgs e)
        {
            string codigoOrden = ddlOrdenCompraIngreso.SelectedValue;
            if (!string.IsNullOrEmpty(codigoOrden))
            {
                CargarDetalleOrdenCompraIngreso(codigoOrden);
            }
            else
            {
                gvDetalleOrdenCompraIngreso.DataSource = new List<object>();
                gvDetalleOrdenCompraIngreso.DataBind();
            }
        }

        protected void ddlOrdenVentaSalida_SelectedIndexChanged(object sender, EventArgs e)
        {
            string codigoOrden = ddlOrdenVentaSalida.SelectedValue;
            if (!string.IsNullOrEmpty(codigoOrden))
            {
                CargarDetalleOrdenVentaSalida(codigoOrden);
            }
            else
            {
                gvDetalleOrdenVentaSalida.DataSource = new List<object>();
                gvDetalleOrdenVentaSalida.DataBind();
            }
        }

        // MÉTODOS AUXILIARES PARA OBTENER DETALLES
        private List<object> ObtenerDetalleOrdenCompra(string codigoOrden)
        {
            var detalle = new List<object>();

            switch (codigoOrden)
            {
                case "PO-2025-001":
                    detalle.Add(new
                    {
                        Codigo = "PROD-001",
                        Nombre = "Laptop HP EliteBook",
                        Descripcion = "Laptop empresarial i7, 16GB RAM, 512GB SSD",
                        Marca = "HP",
                        PrecioUnitario = "1500 $",
                        Categoria = "Tecnología",
                        Cantidad = "5",
                        SubTotal = "7500 $",
                        Estado = "Disponible"
                    });
                    break;
                case "PO-2025-002":
                    detalle.Add(new
                    {
                        Codigo = "PROD-002",
                        Nombre = "Monitor Dell 24\"",
                        Descripcion = "Monitor Full HD 24 pulgadas",
                        Marca = "Dell",
                        PrecioUnitario = "300 $",
                        Categoria = "Tecnología",
                        Cantidad = "15",
                        SubTotal = "4500 $",
                        Estado = "Cancelado"
                    });
                    break;
                case "PO-2025-003":
                    detalle.Add(new
                    {
                        Codigo = "PROD-003",
                        Nombre = "Silla Ergonómica",
                        Descripcion = "Silla de oficina ergonómica ejecutiva",
                        Marca = "ErgoPlus",
                        PrecioUnitario = "250 $",
                        Categoria = "Mobiliario",
                        Cantidad = "40",
                        SubTotal = "10000 $",
                        Estado = "Aceptado"
                    });
                    break;
                default:
                    detalle.Add(new
                    {
                        Codigo = "PROD-000",
                        Nombre = "Producto Genérico",
                        Descripcion = "Descripción del producto",
                        Marca = "Marca",
                        PrecioUnitario = "100 $",
                        Categoria = "Categoría",
                        Cantidad = "1",
                        SubTotal = "100 $",
                        Estado = "Disponible"
                    });
                    break;
            }

            return detalle;
        }

        private List<object> ObtenerDetalleOrdenVenta(string codigoOrden)
        {
            var detalle = new List<object>();

            switch (codigoOrden)
            {
                case "SO-2025-001":
                    detalle.Add(new
                    {
                        Codigo = "PROD-001",
                        Nombre = "Laptop HP EliteBook",
                        Descripcion = "Laptop empresarial i7, 16GB RAM, 512GB SSD",
                        Marca = "HP",
                        PrecioUnitario = "1500 $",
                        Categoria = "Tecnología",
                        Cantidad = "5",
                        SubTotal = "7500 $",
                        Estado = "Disponible"
                    });
                    break;
                case "SO-2025-002":
                    detalle.Add(new
                    {
                        Codigo = "PROD-002",
                        Nombre = "Monitor Dell 24\"",
                        Descripcion = "Monitor Full HD 24 pulgadas",
                        Marca = "Dell",
                        PrecioUnitario = "300 $",
                        Categoria = "Tecnología",
                        Cantidad = "15",
                        SubTotal = "4500 $",
                        Estado = "Cancelado"
                    });
                    break;
                case "SO-2025-003":
                    detalle.Add(new
                    {
                        Codigo = "PROD-003",
                        Nombre = "Silla Ergonómica",
                        Descripcion = "Silla de oficina ergonómica ejecutiva",
                        Marca = "ErgoPlus",
                        PrecioUnitario = "250 $",
                        Categoria = "Mobiliario",
                        Cantidad = "40",
                        SubTotal = "10000 $",
                        Estado = "Aceptado"
                    });
                    break;
                default:
                    detalle.Add(new
                    {
                        Codigo = "PROD-000",
                        Nombre = "Producto Genérico",
                        Descripcion = "Descripción del producto",
                        Marca = "Marca",
                        PrecioUnitario = "100 $",
                        Categoria = "Categoría",
                        Cantidad = "1",
                        SubTotal = "100 $",
                        Estado = "Disponible"
                    });
                    break;
            }

            return detalle;
        }

        // EVENTOS DE BOTONES PRINCIPALES
        protected void btnCompra_Click(object sender, EventArgs e)
        {
            MostrarCompra();
        }

        protected void btnVenta_Click(object sender, EventArgs e)
        {
            MostrarVenta();
        }

        protected void btnIngreso_Click(object sender, EventArgs e)
        {
            MostrarIngreso();
        }

        protected void btnSalida_Click(object sender, EventArgs e)
        {
            MostrarSalida();
        }

        private void MostrarCompra()
        {
            compraContent.Style["display"] = "block";
            ventaContent.Style["display"] = "none";
            ingresoContent.Style["display"] = "none";
            salidaContent.Style["display"] = "none";
            btnCompra.CssClass = "mode-btn active";
            btnVenta.CssClass = "mode-btn";
            btnIngreso.CssClass = "mode-btn";
            btnSalida.CssClass = "mode-btn";
        }

        private void MostrarVenta()
        {
            compraContent.Style["display"] = "none";
            ventaContent.Style["display"] = "block";
            ingresoContent.Style["display"] = "none";
            salidaContent.Style["display"] = "none";
            btnVenta.CssClass = "mode-btn active";
            btnCompra.CssClass = "mode-btn";
            btnIngreso.CssClass = "mode-btn";
            btnSalida.CssClass = "mode-btn";
        }

        private void MostrarIngreso()
        {
            compraContent.Style["display"] = "none";
            ventaContent.Style["display"] = "none";
            ingresoContent.Style["display"] = "block";
            salidaContent.Style["display"] = "none";
            btnIngreso.CssClass = "mode-btn active";
            btnCompra.CssClass = "mode-btn";
            btnVenta.CssClass = "mode-btn";
            btnSalida.CssClass = "mode-btn";
        }

        private void MostrarSalida()
        {
            compraContent.Style["display"] = "none";
            ventaContent.Style["display"] = "none";
            ingresoContent.Style["display"] = "none";
            salidaContent.Style["display"] = "block";
            btnSalida.CssClass = "mode-btn active";
            btnCompra.CssClass = "mode-btn";
            btnVenta.CssClass = "mode-btn";
            btnIngreso.CssClass = "mode-btn";
        }

        // EVENTOS DE CHECKBOX INDIVIDUAL - COMPRA
        protected void chkSeleccionCompra_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSeleccion = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkSeleccion.NamingContainer;

            if (chkSeleccion.Checked)
            {
                string codigoSeleccionado = gvOrdenesCompra.DataKeys[row.RowIndex].Value.ToString();
                CargarDetalleOrdenCompra(codigoSeleccionado);

                // Desmarcar otros checkboxes
                foreach (GridViewRow otherRow in gvOrdenesCompra.Rows)
                {
                    if (otherRow.RowIndex != row.RowIndex)
                    {
                        CheckBox otherChk = (CheckBox)otherRow.FindControl("chkSeleccionCompra");
                        if (otherChk != null)
                        {
                            otherChk.Checked = false;
                        }
                    }
                }
            }
            else
            {
                CargarDetalleOrdenVacia();
            }
        }

        // EVENTOS DE CHECKBOX INDIVIDUAL - VENTA
        protected void chkSeleccionVenta_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSeleccion = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkSeleccion.NamingContainer;

            if (chkSeleccion.Checked)
            {
                string codigoSeleccionado = gvOrdenesVenta.DataKeys[row.RowIndex].Value.ToString();
                CargarDetalleOrdenVenta(codigoSeleccionado);

                // Desmarcar otros checkboxes
                foreach (GridViewRow otherRow in gvOrdenesVenta.Rows)
                {
                    if (otherRow.RowIndex != row.RowIndex)
                    {
                        CheckBox otherChk = (CheckBox)otherRow.FindControl("chkSeleccionVenta");
                        if (otherChk != null)
                        {
                            otherChk.Checked = false;
                        }
                    }
                }
            }
            else
            {
                CargarDetalleOrdenVacia();
            }
        }

        // NUEVOS EVENTOS DE CHECKBOX PARA INGRESO Y SALIDA
        protected void chkSeleccionIngreso_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSeleccion = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkSeleccion.NamingContainer;

            if (chkSeleccion.Checked)
            {
                string codigoSeleccionado = gvRegistrosIngreso.DataKeys[row.RowIndex].Value.ToString();
                // Aquí puedes cargar detalles específicos de ingreso si es necesario

                // Desmarcar otros checkboxes
                foreach (GridViewRow otherRow in gvRegistrosIngreso.Rows)
                {
                    if (otherRow.RowIndex != row.RowIndex)
                    {
                        CheckBox otherChk = (CheckBox)otherRow.FindControl("chkSeleccionIngreso");
                        if (otherChk != null)
                        {
                            otherChk.Checked = false;
                        }
                    }
                }
            }
        }

        protected void chkSeleccionSalida_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSeleccion = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkSeleccion.NamingContainer;

            if (chkSeleccion.Checked)
            {
                string codigoSeleccionado = gvRegistrosSalida.DataKeys[row.RowIndex].Value.ToString();
                // Aquí puedes cargar detalles específicos de salida si es necesario

                // Desmarcar otros checkboxes
                foreach (GridViewRow otherRow in gvRegistrosSalida.Rows)
                {
                    if (otherRow.RowIndex != row.RowIndex)
                    {
                        CheckBox otherChk = (CheckBox)otherRow.FindControl("chkSeleccionSalida");
                        if (otherChk != null)
                        {
                            otherChk.Checked = false;
                        }
                    }
                }
            }
        }

        // NUEVOS EVENTOS PARA BOTONES EDITAR EN CADA FILA
        protected void btnEditarCompraFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string codigoOrden = btn.CommandArgument;

            // Seleccionar automáticamente el checkbox correspondiente
            foreach (GridViewRow row in gvOrdenesCompra.Rows)
            {
                CheckBox chkSeleccion = (CheckBox)row.FindControl("chkSeleccionCompra");
                string codigoFila = gvOrdenesCompra.DataKeys[row.RowIndex].Value.ToString();

                if (codigoFila == codigoOrden)
                {
                    chkSeleccion.Checked = true;
                    CargarDetalleOrdenCompra(codigoOrden);
                }
                else
                {
                    chkSeleccion.Checked = false;
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "editarCompraFila",
                $"alert('Editando orden de compra: {codigoOrden}');", true);
        }

        protected void btnEditarVentaFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string codigoOrden = btn.CommandArgument;

            // Seleccionar automáticamente el checkbox correspondiente
            foreach (GridViewRow row in gvOrdenesVenta.Rows)
            {
                CheckBox chkSeleccion = (CheckBox)row.FindControl("chkSeleccionVenta");
                string codigoFila = gvOrdenesVenta.DataKeys[row.RowIndex].Value.ToString();

                if (codigoFila == codigoOrden)
                {
                    chkSeleccion.Checked = true;
                    CargarDetalleOrdenVenta(codigoOrden);
                }
                else
                {
                    chkSeleccion.Checked = false;
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "editarVentaFila",
                $"alert('Editando orden de venta: {codigoOrden}');", true);
        }

        protected void btnEditarIngresoFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string codigoIngreso = btn.CommandArgument;

            // Seleccionar automáticamente el checkbox correspondiente
            foreach (GridViewRow row in gvRegistrosIngreso.Rows)
            {
                CheckBox chkSeleccion = (CheckBox)row.FindControl("chkSeleccionIngreso");
                string codigoFila = gvRegistrosIngreso.DataKeys[row.RowIndex].Value.ToString();

                if (codigoFila == codigoIngreso)
                {
                    chkSeleccion.Checked = true;
                }
                else
                {
                    chkSeleccion.Checked = false;
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "editarIngresoFila",
                $"alert('Editando registro de ingreso: {codigoIngreso}');", true);
        }

        protected void btnEditarSalidaFila_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string codigoSalida = btn.CommandArgument;

            // Seleccionar automáticamente el checkbox correspondiente
            foreach (GridViewRow row in gvRegistrosSalida.Rows)
            {
                CheckBox chkSeleccion = (CheckBox)row.FindControl("chkSeleccionSalida");
                string codigoFila = gvRegistrosSalida.DataKeys[row.RowIndex].Value.ToString();

                if (codigoFila == codigoSalida)
                {
                    chkSeleccion.Checked = true;
                }
                else
                {
                    chkSeleccion.Checked = false;
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "editarSalidaFila",
                $"alert('Editando registro de salida: {codigoSalida}');", true);
        }

        // EVENTOS DE GRILLAS
        protected void gvOrdenesCompra_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Lógica específica para la grilla de compras si es necesario
        }

        protected void gvOrdenesVenta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Lógica específica para la grilla de ventas si es necesario
        }

        protected void gvRegistrosIngreso_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Lógica específica para la grilla de ingresos si es necesario
        }

        protected void gvRegistrosSalida_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Lógica específica para la grilla de salidas si es necesario
        }

        // EVENTOS DE BOTONES - COMPRA
        protected void btnAgregarCompra_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "agregarCompra",
                "alert('Función Agregar Orden Compra - Por implementar');", true);
        }

        protected void btnAnularCompra_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "anularCompra",
                "alert('Función Anular Orden Compra - Por implementar');", true);
        }

        // EVENTOS DE BOTONES - VENTA
        protected void btnAgregarVenta_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "agregarVenta",
                "alert('Función Agregar Orden Venta - Por implementar');", true);
        }

        protected void btnAnularVenta_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "anularVenta",
                "alert('Función Anular Orden Venta - Por implementar');", true);
        }

        // EVENTOS DE BOTONES - INGRESO
        protected void btnAgregarIngreso_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "agregarIngreso",
                "alert('Función Agregar Ingreso - Por implementar');", true);
        }

        protected void btnAnularIngreso_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "anularIngreso",
                "alert('Función Anular Ingreso - Por implementar');", true);
        }

        // EVENTOS DE BOTONES - SALIDA
        protected void btnAgregarSalida_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "agregarSalida",
                "alert('Función Agregar Salida - Por implementar');", true);
        }

        protected void btnAnularSalida_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "anularSalida",
                "alert('Función Anular Salida - Por implementar');", true);
        }

        // EVENTOS PARA VIEW DOCUMENTOS
        protected void btnViewCompra_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "viewCompra",
                "alert('Función View Documento Compra - Por implementar');", true);
        }

        protected void btnViewVenta_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "viewVenta",
                "alert('Función View Documento Venta - Por implementar');", true);
        }
    }
}