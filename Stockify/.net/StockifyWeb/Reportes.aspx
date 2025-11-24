<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="StockifyWeb.Reportes" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons+Outlined" rel="stylesheet"/>
    <style>
        body {
            background-color: #121212 !important;
            color: #E0E0E0 !important;
            margin: 0;
            padding: 0;
            overflow-x: hidden;
        }
        
        .main-content {
            overflow-x: hidden;
        }
        
        .report-section {
            margin-bottom: 2rem;
            padding: 1.5rem;
            background-color: #1E1E1E;
            border-radius: 12px;
            border: 1px solid #333333;
            max-width: 100%;
            box-sizing: border-box;
        }
        
        .section-title {
            font-size: 1.5rem;
            font-weight: bold;
            color: white;
            margin-bottom: 0.75rem;
        }
        
        .section-description {
            color: #A0A0A0;
            max-width: 42rem;
            margin-bottom: 1.5rem;
            line-height: 1.5;
            font-size: 0.875rem;
        }
        
        .btn-primary-custom {
            background-color: #007AFF;
            color: white;
            font-weight: 600;
            padding: 10px 24px;
            border-radius: 8px;
            border: none;
            transition: background-color 0.3s ease;
            cursor: pointer;
            white-space: nowrap;
            height: 42px;
            display: block;
            width: 100%;
            font-size: 14px;
            -webkit-appearance: none;
            -moz-appearance: none;
            appearance: none;
        }
        
        .btn-primary-custom:hover {
            background-color: #0066CC;
        }
        
        .form-control-custom {
            background-color: #2A2A2A;
            border: 1px solid #333333;
            border-radius: 8px;
            padding: 10px 14px;
            color: #E0E0E0;
            font-size: 14px;
            width: 100%;
            box-sizing: border-box;
            height: 42px;
            min-width: 180px;
        }

        select.form-control-custom {
            appearance: none;
            -webkit-appearance: none;
            -moz-appearance: none;
            background-image: url("data:image/svg+xml;charset=US-ASCII,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'><path fill='%23A0A0A0' d='M2 0L0 2h4zm0 5L0 3h4z'/></svg>");
            background-repeat: no-repeat;
            background-position: right 12px center;
            background-size: 12px;
            padding-right: 35px;
        }

        input[type="date"].form-control-custom {
            appearance: none;
            -webkit-appearance: none;
            -moz-appearance: none;
            position: relative;
            padding-right: 35px;
        }

        input[type="date"].form-control-custom::-webkit-calendar-picker-indicator {
            background: transparent;
            bottom: 0;
            color: transparent;
            cursor: pointer;
            height: auto;
            left: 0;
            position: absolute;
            right: 0;
            top: 0;
            width: auto;
        }

        input[type="date"].form-control-custom::-webkit-inner-spin-button,
        input[type="date"].form-control-custom::-webkit-outer-spin-button {
            -webkit-appearance: none;
            margin: 0;
        }

        input[type="date"].form-control-custom {
            background-image: url("data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' width='20' height='20' viewBox='0 0 24 24' fill='none' stroke='%23A0A0A0' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3crect x='3' y='4' width='18' height='18' rx='2' ry='2'%3e%3c/rect%3e%3cline x1='16' y1='2' x2='16' y2='6'%3e%3c/line%3e%3cline x1='8' y1='2' x2='8' y2='6'%3e%3c/line%3e%3cline x1='3' y1='10' x2='21' y2='10'%3e%3c/line%3e%3c/svg%3e");
            background-repeat: no-repeat;
            background-position: right 12px center;
            background-size: 18px;
        }
        
        .form-control-custom:focus {
            border-color: #007AFF;
            outline: none;
            box-shadow: 0 0 0 2px rgba(0, 122, 255, 0.1);
        }
        
        .input-group {
            display: flex;
            flex-direction: column;
            gap: 0.75rem;
            max-width: 100%;
        }

        .dates-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 0.75rem;
        }

        .material-icons-outlined {
            font-size: inherit;
        }
        
        .space-y-12 > * + * {
            margin-top: 2rem;
        }

        @media (max-width: 768px) {
            .dates-row {
                grid-template-columns: 1fr;
            }
        }

        .form-control-custom option {
            background-color: #2A2A2A;
            color: #E0E0E0;
            padding: 12px 16px;
        }

        .form-control-custom option:checked {
            background-color: #007AFF;
            color: white;
        }

        .status-message {
            padding: 0.875rem 1.25rem;
            border-radius: 8px;
            margin-bottom: 1.5rem;
            font-size: 0.875rem;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 0.75rem;
            animation: slideDown 0.3s ease-out;
        }

        .status-success {
            background-color: rgba(16, 185, 129, 0.1);
            border: 1px solid #10B981;
            color: #10B981;
        }

        .status-error {
            background-color: rgba(239, 68, 68, 0.1);
            border: 1px solid #EF4444;
            color: #EF4444;
        }

        .status-success::before {
            content: "✓";
            font-size: 1.1rem;
            font-weight: bold;
        }

        .status-error::before {
            content: "✗";
            font-size: 1.1rem;
            font-weight: bold;
        }

        @keyframes slideDown {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="cph_Contenido" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    
    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="status-message">
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>

    <div style="max-width: 100%; padding: 0 1rem; box-sizing: border-box;">
        <div class="space-y-12">
            <section class="report-section">
                <h2 class="section-title">Kardex de Inventario</h2>
                <p class="section-description">
                    Genera un kardex detallado de inventario utilizando métodos de valoración PEPS o Promedio Ponderado. Incluye un registro completo de entradas, salidas y saldos de productos.
                </p>
                <div class="input-group">
                    <asp:DropDownList ID="ddlKardexProducto" runat="server" CssClass="form-control-custom">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlMetodoValoracionReporte" runat="server" CssClass="form-control-custom">
                        <asp:ListItem Text="Promedio Ponderado" Value="PP" Selected="True" />
                        <asp:ListItem Text="PEPS (FIFO)" Value="PEPS" />
                    </asp:DropDownList>
                    <div class="dates-row">
                        <asp:TextBox ID="txtFechaDesdeKardex" runat="server" CssClass="form-control-custom" TextMode="Date" />
                        <asp:TextBox ID="txtFechaHastaKardex" runat="server" CssClass="form-control-custom" TextMode="Date" />
                    </div>
                    <asp:Button ID="btnGenerarKardex" runat="server" CssClass="btn-primary-custom" Text="Generar Kardex" OnClick="btnGenerarKardex_Click" />
                </div>
            </section>

            <section class="report-section">
                <h2 class="section-title">Reporte de existencias de productos</h2>
                <p class="section-description">
                    Genera un reporte detallando los niveles actuales de stock de los productos en el inventario. Este reporte incluye nombres de productos, cantidades y ubicaciones de almacenamiento.
                </p>
                <div class="input-group">
                    <asp:Button ID="btnGenerarReporteProductos" runat="server" CssClass="btn-primary-custom" Text="Generar reporte" OnClick="btnGenerarReporteProductos_Click" />
                </div>
            </section>
            
            <section class="report-section">
                <h2 class="section-title">Reporte de proveedores</h2>
                <p class="section-description">
                    Genera un reporte listando todos los proveedores, su información de contacto y los productos que suministran. Este reporte es útil para gestionar las relaciones con los proveedores y las adquisiciones.
                </p>
                <div class="input-group">
                    <asp:DropDownList ID="ddlFiltroProveedor" runat="server" CssClass="form-control-custom">
                    </asp:DropDownList>
                    <asp:Button ID="btnGenerarReporteProveedores" runat="server" CssClass="btn-primary-custom" Text="Generar reporte" OnClick="btnGenerarReporteProveedores_Click" />
                </div>
            </section>
            
            <section class="report-section">
                <h2 class="section-title">Reporte de productos por categoría</h2>
                <p class="section-description">
                    Genera un reporte de productos por categoría. Selecciona una categoría para ver todos los productos dentro de ella, incluyendo sus niveles de stock y demás información.
                </p>
                <div class="input-group">
                    <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-control-custom">
                    </asp:DropDownList>
                    <asp:Button ID="btnGenerarReporteCategorias" runat="server" CssClass="btn-primary-custom" Text="Generar reporte" OnClick="btnGenerarReporteCategorias_Click" />
                </div>
            </section>
        </div>
    </div>
</asp:Content>