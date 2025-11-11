<%@ Page Title="Reportes" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="StockifyWeb.Reportes" %>

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
        
        .reports-header {
            border-bottom: 1px solid #333333;
            padding: 1rem 0;
            margin-bottom: 2rem;
        }
        
        .header-buttons {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 0.5rem;
            width: 100%;
        }
        
        .header-btn {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
            background-color: transparent;
            color: #E0E0E0;
            border: 1px solid #333333;
            padding: 0.5rem 0;
            border-radius: 8px;
            font-size: 0.875rem;
            transition: all 0.3s ease;
            cursor: pointer;
            font-weight: 500;
            width: 48%;
            height: 40px;
        }
        
        .header-btn:hover {
            background-color: #2A2A2A;
        }
        
        .header-btn.active {
            background-color: white;
            color: #121212;
            border-color: white;
        }

        .content-section {
            display: none;
        }

        .content-section.active {
            display: block;
        }

        .report-section {
            margin-bottom: 3rem;
            padding: 2rem;
            background-color: #1E1E1E;
            border-radius: 12px;
            border: 1px solid #333333;
            max-width: 100%;
            box-sizing: border-box;
        }
        
        .section-title {
            font-size: 2.25rem;
            font-weight: bold;
            color: white;
            margin-bottom: 1rem;
        }
        
        .section-description {
            color: #A0A0A0;
            max-width: 42rem;
            margin-bottom: 2rem;
            line-height: 1.6;
        }
        
        .btn-primary-custom {
            background-color: #007AFF;
            color: white;
            font-weight: 600;
            padding: 12px 32px;
            border-radius: 8px;
            border: none;
            transition: background-color 0.3s ease;
            cursor: pointer;
            white-space: nowrap;
            height: 48px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .btn-primary-custom:hover {
            background-color: #0066CC;
        }
        
        .form-control-custom {
            background-color: #2A2A2A;
            border: 1px solid #333333;
            border-radius: 8px;
            padding: 12px 16px;
            color: #E0E0E0;
            font-size: 14px;
            width: 100%;
            box-sizing: border-box;
            height: 48px;
            min-width: 300px;
            appearance: none;
            -webkit-appearance: none;
            -moz-appearance: none;
            background-image: url("data:image/svg+xml;charset=US-ASCII,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'><path fill='%23A0A0A0' d='M2 0L0 2h4zm0 5L0 3h4z'/></svg>");
            background-repeat: no-repeat;
            background-position: right 12px center;
            background-size: 12px;
        }
        
        .form-control-custom:focus {
            border-color: #007AFF;
            outline: none;
            box-shadow: 0 0 0 2px rgba(0, 122, 255, 0.1);
        }
        
        .input-group {
            display: flex;
            align-items: center;
            gap: 1rem;
            max-width: 100%;
            flex-wrap: nowrap;
        }

        .kardex-container {
            max-width: 100%;
            margin: 0 auto;
            padding: 0 1rem;
            box-sizing: border-box;
            overflow-x: hidden;
        }
        
        .kardex-header {
            margin-bottom: 1.5rem;
        }
        
        .kardex-title {
            font-size: 1.75rem;
            font-weight: bold;
            color: white;
            margin-bottom: 0.5rem;
        }
        
        .kardex-subtitle {
            color: #A0A0A0;
            font-size: 0.9rem;
        }
        
        .kardex-layout {
            display: grid;
            grid-template-columns: 1fr;
            gap: 1.5rem;
            max-width: 100%;
        }
        
        @media (min-width: 1024px) {
            .kardex-layout {
                grid-template-columns: 280px 1fr;
            }
        }
        
        .articles-panel {
            background-color: #1E1E1E;
            border-radius: 8px;
            border: 1px solid #333333;
            padding: 1.25rem;
            max-width: 100%;
            box-sizing: border-box;
        }
        
        .articles-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: white;
            margin-bottom: 1rem;
        }
        
        .search-box {
            position: relative;
            margin-bottom: 1rem;
        }
        
        .search-icon {
            position: absolute;
            left: 0.75rem;
            top: 50%;
            transform: translateY(-50%);
            color: #A0A0A0;
            font-size: 1rem;
        }
        
        .search-input {
            width: 100%;
            padding: 0.5rem 0.75rem 0.5rem 2.5rem;
            background-color: #2A2A2A;
            border: 1px solid #333333;
            border-radius: 6px;
            color: #E0E0E0;
            font-size: 0.875rem;
            box-sizing: border-box;
        }
        
        .product-list {
            list-style: none;
            padding: 0;
            margin: 0;
        }
        
        .product-item {
            padding: 0.75rem;
            border-radius: 6px;
            margin-bottom: 0.5rem;
            cursor: pointer;
            border: 1px solid transparent;
            transition: all 0.2s ease;
        }
        
        .product-item a {
            text-decoration: none;
            color: inherit;
            display: block;
        }
        
        .product-item.active {
            background-color: rgba(59, 130, 246, 0.1);
            border-color: #3b82f6;
        }
        
        .product-item:hover:not(.active) {
            background-color: #2A2A2A;
        }
        
        .product-name {
            font-weight: 500;
            color: #E0E0E0;
            font-size: 0.9rem;
            margin-bottom: 0.25rem;
        }
        
        .product-id {
            font-size: 0.75rem;
            color: #A0A0A0;
        }
        
        .kardex-content {
            display: flex;
            flex-direction: column;
            gap: 1.5rem;
            max-width: 100%;
            box-sizing: border-box;
        }
        
        .product-header {
            display: flex;
            flex-direction: column;
            gap: 1rem;
            background-color: #1E1E1E;
            border-radius: 8px;
            border: 1px solid #333333;
            padding: 1.25rem;
        }
        
        @media (min-width: 768px) {
            .product-header {
                flex-direction: row;
                justify-content: space-between;
                align-items: center;
            }
        }
        
        .product-info h2 {
            font-size: 1.3rem;
            font-weight: bold;
            color: white;
            margin-bottom: 0.25rem;
        }
        
        .product-info p {
            color: #A0A0A0;
            font-size: 0.875rem;
        }
        
        .valuation-selector {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            flex-wrap: wrap;
        }
        
        .valuation-selector label {
            color: #A0A0A0;
            font-size: 0.875rem;
        }
        
        .select-field {
            background-color: #2A2A2A;
            border: 1px solid #333333;
            border-radius: 6px;
            padding: 0.5rem;
            color: #E0E0E0;
            font-size: 0.875rem;
            min-width: 180px;
        }
        
        .metrics-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 1rem;
        }
        
        @media (min-width: 640px) {
            .metrics-grid {
                grid-template-columns: repeat(4, 1fr);
            }
        }
        
        .metric-card {
            background-color: #1E1E1E;
            border-radius: 8px;
            border: 1px solid #333333;
            padding: 1rem;
            text-align: center;
        }
        
        .metric-value {
            font-size: 1.5rem;
            font-weight: bold;
            color: white;
            margin-bottom: 0.25rem;
        }
        
        .metric-label {
            font-size: 0.75rem;
            color: #A0A0A0;
            margin-bottom: 0.5rem;
        }
        
        .metric-unit {
            font-size: 0.75rem;
            color: #A0A0A0;
        }
        
        .movement-details {
            display: flex;
            flex-direction: column;
            gap: 0.25rem;
            margin-top: 0.5rem;
        }
        
        .movement-item {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-size: 0.75rem;
        }
        
        .movement-in {
            color: #10B981;
        }
        
        .movement-out {
            color: #EF4444;
        }
        
        .filters-section {
            background-color: #1E1E1E;
            border-radius: 8px;
            border: 1px solid #333333;
            padding: 1.25rem;
        }
        
        .filters-header {
            font-size: 1rem;
            font-weight: 600;
            color: white;
            margin-bottom: 1rem;
        }
        
        .filters-content {
            display: flex;
            flex-direction: column;
            gap: 1rem;
        }
        
        @media (min-width: 768px) {
            .filters-content {
                flex-direction: row;
                justify-content: space-between;
                align-items: flex-end;
            }
        }
        
        .date-filters {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
        }
        
        .date-filter {
            display: flex;
            flex-direction: column;
            min-width: 140px;
        }
        
        .date-filter label {
            font-size: 0.75rem;
            color: #A0A0A0;
            margin-bottom: 0.25rem;
        }
        
        .input-field {
            background-color: #2A2A2A;
            border: 1px solid #333333;
            border-radius: 6px;
            padding: 0.5rem;
            color: #E0E0E0;
            font-size: 0.875rem;
            width: 100%;
            box-sizing: border-box;
        }
        
        .action-buttons {
            display: flex;
            gap: 0.5rem;
            flex-wrap: wrap;
        }
        
        .btn {
            padding: 0.5rem 1rem;
            border-radius: 6px;
            font-size: 0.875rem;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
            white-space: nowrap;
        }
        
        .btn-outline {
            background-color: transparent;
            border: 1px solid #333333;
            color: #E0E0E0;
        }
        
        .btn-outline:hover {
            background-color: #2A2A2A;
        }
        
        .btn-primary {
            background-color: #3b82f6;
            color: white;
            border: none;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-primary:hover {
            background-color: #2563eb;
        }
        
        .table-section {
            background-color: #1E1E1E;
            border-radius: 8px;
            border: 1px solid #333333;
            padding: 0;
            overflow: hidden;
        }
        
        .table-container {
            overflow-x: auto;
            max-width: 100%;
        }
        
        table {
            width: 100%;
            border-collapse: collapse;
            background-color: #1E1E1E;
            min-width: 800px;
        }
        
        th, td {
            padding: 0.75rem;
            text-align: left;
            border-bottom: 1px solid #333333;
            white-space: nowrap;
        }
        
        thead {
            background-color: #2A2A2A;
        }
        
        .text-right {
            text-align: right;
        }
        
        .text-center {
            text-align: center;
        }
        
        .table-heading {
            font-size: 0.7rem;
            text-transform: uppercase;
            color: #A0A0A0;
            font-weight: 500;
        }
        
        .table-cell {
            padding: 0.6rem;
            border-bottom: 1px solid #333333;
            font-size: 0.8rem;
        }
        
        .border-r {
            border-right: 1px solid #333333;
        }
        
        .movement-count {
            text-align: right;
            font-size: 0.75rem;
            color: #A0A0A0;
            padding: 0.75rem;
            border-top: 1px solid #333333;
        }
        
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 1rem;
            margin-top: 1.5rem;
        }
        
        .stat-card {
            background-color: #1E1E1E;
            border-radius: 8px;
            padding: 1.25rem;
            text-align: center;
            border: 1px solid #333333;
        }
        
        .stat-value {
            font-size: 1.75rem;
            font-weight: bold;
            color: white;
            margin-bottom: 0.5rem;
        }
        
        .stat-label {
            font-size: 0.75rem;
            color: #A0A0A0;
        }
        
        .material-icons-outlined {
            font-size: inherit;
        }
        
        .flex {
            display: flex;
        }
        
        .items-center {
            align-items: center;
        }
        
        .justify-between {
            justify-content: space-between;
        }
        
        .mb-4 {
            margin-bottom: 1rem;
        }
        
        .space-y-4 > * + * {
            margin-top: 1rem;
        }

        .space-y-12 > * + * {
            margin-top: 3rem;
        }

        @media (max-width: 768px) {
            .input-group {
                flex-direction: column;
                align-items: stretch;
            }
            
            .btn-primary-custom {
                width: 100%;
            }
        }

        /* Estilos para el dropdown nativo - SIMPLE Y OSCURO */
        .form-control-custom option {
            background-color: #2A2A2A;
            color: #E0E0E0;
            padding: 12px 16px;
            border-bottom: 1px solid #333333;
        }

        .form-control-custom option:checked {
            background-color: #007AFF;
            color: white;
        }

        .form-control-custom option:hover {
            background-color: #007AFF;
            color: white;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="cph_Contenido" runat="server">
    <div class="reports-header">
        <div class="header-buttons">
            <button id="btnGestionInventario" class="header-btn active" type="button">
                <i class="fas fa-boxes"></i>
                Gestión de Inventario
            </button>
            <button id="btnKardex" class="header-btn" type="button">
                <i class="fas fa-file-alt"></i>
                Kardex (PEPS)
            </button>
        </div>
    </div>

    <div id="gestion-inventario" class="content-section active">
        <div style="max-width: 100%; padding: 0 1rem; box-sizing: border-box;">
            <div class="space-y-12">
                <section class="report-section">
                    <h2 class="section-title">Reporte de existencias de productos</h2>
                    <p class="section-description">
                        Genera un reporte detallando los niveles actuales de stock de los productos en el inventario. Este reporte incluye nombres de productos, cantidades y ubicaciones de almacenamiento.
                    </p>
                    <div class="input-group">
                        <asp:DropDownList ID="ddlFiltroProducto" runat="server" CssClass="form-control-custom">
                            <asp:ListItem Text="Selecciona un producto" Value=""></asp:ListItem>
                            <asp:ListItem Text="Producto A" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Producto B" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Producto C" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Producto D" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Producto E" Value="5"></asp:ListItem>
                            <asp:ListItem Text="Producto F" Value="6"></asp:ListItem>
                            <asp:ListItem Text="Producto G" Value="7"></asp:ListItem>
                        </asp:DropDownList>
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
                            <asp:ListItem Text="Selecciona un proveedor" Value=""></asp:ListItem>
                            <asp:ListItem Text="Proveedor X" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Proveedor Y" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Proveedor Z" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Proveedor W" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Proveedor V" Value="5"></asp:ListItem>
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
    </div>

    <div id="kardex" class="content-section">
        <div class="kardex-container">
            <div class="kardex-header">
                <h1 class="kardex-title">Kardex de Inventario</h1>
                <p class="kardex-subtitle">Control de inventario con métodos PEPS y Promedio Ponderado</p>
            </div>

            <div class="kardex-layout">
                <div class="articles-panel">
                    <h3 class="articles-title">Artículos</h3>
                    <div class="search-box">
                        <span class="material-icons-outlined search-icon">search</span>
                        <asp:TextBox ID="txtBuscarArticulo" runat="server" CssClass="search-input" placeholder="Buscar..." AutoPostBack="true" OnTextChanged="txtBuscarArticulo_TextChanged" />
                    </div>
                    <ul class="product-list">
                        <asp:Repeater ID="rptArticulos" runat="server" OnItemCommand="rptArticulos_ItemCommand">
                            <ItemTemplate>
                                <li class="product-item">
                                    <asp:LinkButton ID="lnkArticulo" runat="server" CommandName="SeleccionarArticulo" CommandArgument='<%# Eval("ProductoId") %>'>
                                        <div>
                                            <p class="product-name"><%# Eval("Nombre") %></p>
                                            <p class="product-id"><%# Eval("Codigo") %></p>
                                        </div>
                                    </asp:LinkButton>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <div class="kardex-content">
                    <div class="product-header">
                        <div class="product-info">
                            <h2><asp:Literal ID="litNombreProducto" runat="server" Text="Desktop Computer" /></h2>
                            <p>ID: <asp:Literal ID="litCodigoProducto" runat="server" Text="PROD-001" /></p>
                        </div>
                        <div class="valuation-selector">
                            <label for="<%= ddlMetodoValoracion.ClientID %>">Método de Valoración:</label>
                            <asp:DropDownList ID="ddlMetodoValoracion" runat="server" CssClass="select-field" AutoPostBack="true" OnSelectedIndexChanged="ddlMetodoValoracion_SelectedIndexChanged">
                                <asp:ListItem Text="Promedio Ponderado" Value="PP" Selected="True" />
                                <asp:ListItem Text="PEPS" Value="PEPS" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="metrics-grid">
                        <div class="metric-card">
                            <p class="metric-label">Saldo Actual</p>
                            <div class="metric-value"><asp:Literal ID="litSaldoActual" runat="server" Text="8" /></div>
                            <span class="metric-unit">unidades</span>
                        </div>
                        <div class="metric-card">
                            <p class="metric-label">Costo Unitario</p>
                            <div class="metric-value">$<asp:Literal ID="litCostoUnitario" runat="server" Text="800.00" /></div>
                        </div>
                        <div class="metric-card">
                            <p class="metric-label">Valor Total</p>
                            <div class="metric-value">$<asp:Literal ID="litValorTotal" runat="server" Text="6400.00" /></div>
                        </div>
                        <div class="metric-card">
                            <p class="metric-label">Movimientos</p>
                            <div class="movement-details">
                                <div class="movement-item movement-in">
                                    <span class="material-icons-outlined" style="font-size: 14px;">trending_up</span>
                                    Entradas: <asp:Literal ID="litEntradas" runat="server" Text="10" />
                                </div>
                                <div class="movement-item movement-out">
                                    <span class="material-icons-outlined" style="font-size: 14px;">trending_down</span>
                                    Salidas: <asp:Literal ID="litSalidas" runat="server" Text="12" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="filters-section">
                        <h4 class="filters-header">Filtrar por Rango de Fechas</h4>
                        <div class="filters-content">
                            <div class="date-filters">
                                <div class="date-filter">
                                    <label for="<%= txtFechaDesde.ClientID %>">Desde</label>
                                    <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="input-field" TextMode="Date" />
                                </div>
                                <div class="date-filter">
                                    <label for="<%= txtFechaHasta.ClientID %>">Hasta</label>
                                    <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="input-field" TextMode="Date" />
                                </div>
                            </div>
                            <div class="action-buttons">
                                <asp:Button ID="btnLimpiarFiltro" runat="server" CssClass="btn btn-outline" Text="Limpiar Filtro" OnClick="btnLimpiarFiltro_Click" />
                                <asp:Button ID="btnExportarCSV" runat="server" CssClass="btn btn-primary" Text="Exportar CSV" OnClick="btnExportarCSV_Click" />
                            </div>
                        </div>
                    </div>

                    <div class="table-section">
                        <div class="table-container">
                            <table>
                                <thead>
                                    <tr>
                                        <th class="table-heading border-r" rowspan="2">Fecha</th>
                                        <th class="table-heading border-r" rowspan="2">Detalle</th>
                                        <th class="table-heading border-r" rowspan="2">Documento</th>
                                        <th class="table-heading text-center border-r" colspan="3">Entradas</th>
                                        <th class="table-heading text-center border-r" colspan="3">Salidas</th>
                                        <th class="table-heading text-center" colspan="3">Saldo</th>
                                    </tr>
                                    <tr>
                                        <th class="table-heading text-right">Cant.</th>
                                        <th class="table-heading text-right">P.U.</th>
                                        <th class="table-heading text-right border-r">Total</th>
                                        <th class="table-heading text-right">Cant.</th>
                                        <th class="table-heading text-right">P.U.</th>
                                        <th class="table-heading text-right border-r">Total</th>
                                        <th class="table-heading text-right">Cant.</th>
                                        <th class="table-heading text-right">P.U.</th>
                                        <th class="table-heading text-right">Total</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptMovimientos" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="table-cell"><%# Eval("Fecha", "{0:yyyy-MM-dd}") %></td>
                                                <td class="table-cell"><%# Eval("Detalle") %></td>
                                                <td class="table-cell"><%# Eval("Documento") %></td>
                                                <td class="table-cell text-right" style='<%# Convert.ToInt32(Eval("EntradaCantidad")) > 0 ? "color: #10B981;" : "" %>'>
                                                    <%# Convert.ToInt32(Eval("EntradaCantidad")) > 0 ? Eval("EntradaCantidad") : "-" %>
                                                </td>
                                                <td class="table-cell text-right" style='<%# Convert.ToDecimal(Eval("EntradaPrecio")) > 0 ? "color: #10B981;" : "" %>'>
                                                    <%# Convert.ToDecimal(Eval("EntradaPrecio")) > 0 ? "$" + Eval("EntradaPrecio", "{0:N2}") : "-" %>
                                                </td>
                                                <td class="table-cell text-right border-r" style='<%# Convert.ToDecimal(Eval("EntradaTotal")) > 0 ? "color: #10B981;" : "" %>'>
                                                    <%# Convert.ToDecimal(Eval("EntradaTotal")) > 0 ? "$" + Eval("EntradaTotal", "{0:N2}") : "-" %>
                                                </td>
                                                <td class="table-cell text-right" style='<%# Convert.ToInt32(Eval("SalidaCantidad")) > 0 ? "color: #EF4444;" : "" %>'>
                                                    <%# Convert.ToInt32(Eval("SalidaCantidad")) > 0 ? Eval("SalidaCantidad") : "-" %>
                                                </td>
                                                <td class="table-cell text-right" style='<%# Convert.ToDecimal(Eval("SalidaPrecio")) > 0 ? "color: #EF4444;" : "" %>'>
                                                    <%# Convert.ToDecimal(Eval("SalidaPrecio")) > 0 ? "$" + Eval("SalidaPrecio", "{0:N2}") : "-" %>
                                                </td>
                                                <td class="table-cell text-right border-r" style='<%# Convert.ToDecimal(Eval("SalidaTotal")) > 0 ? "color: #EF4444;" : "" %>'>
                                                    <%# Convert.ToDecimal(Eval("SalidaTotal")) > 0 ? "$" + Eval("SalidaTotal", "{0:N2}") : "-" %>
                                                </td>
                                                <td class="table-cell text-right" style="font-weight: 600;"><%# Eval("SaldoCantidad") %></td>
                                                <td class="table-cell text-right" style="font-weight: 600;">$<%# Eval("SaldoPrecio", "{0:N2}") %></td>
                                                <td class="table-cell text-right" style="font-weight: 600;">$<%# Eval("SaldoTotal", "{0:N2}") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                        <div class="movement-count">Mostrando <asp:Literal ID="litCantidadMovimientos" runat="server" Text="0" /> movimientos</div>
                    </div>
                </div>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <p class="stat-label">Total Artículos</p>
                    <p class="stat-value"><asp:Literal ID="litTotalArticulos" runat="server" Text="3" /></p>
                </div>
                <div class="stat-card">
                    <p class="stat-label">Valor Total de Inventario</p>
                    <p class="stat-value">$<asp:Literal ID="litValorTotalInventario" runat="server" Text="8900.00" /></p>
                </div>
                <div class="stat-card">
                    <p class="stat-label">Unidades Totales</p>
                    <p class="stat-value"><asp:Literal ID="litUnidadesTotales" runat="server" Text="33" /></p>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showSection(sectionId) {
            var sections = document.getElementsByClassName('content-section');
            for (var i = 0; i < sections.length; i++) {
                sections[i].classList.remove('active');
            }

            var activeSection = document.getElementById(sectionId);
            if (activeSection) {
                activeSection.classList.add('active');
            }

            var buttons = document.getElementsByClassName('header-btn');
            for (var i = 0; i < buttons.length; i++) {
                buttons[i].classList.remove('active');
            }

            if (sectionId === 'gestion-inventario') {
                document.getElementById('btnGestionInventario').classList.add('active');
            } else if (sectionId === 'kardex') {
                document.getElementById('btnKardex').classList.add('active');
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('btnGestionInventario').addEventListener('click', function () {
                showSection('gestion-inventario');
            });

            document.getElementById('btnKardex').addEventListener('click', function () {
                showSection('kardex');
            });

            showSection('gestion-inventario');
        });
    </script>
</asp:Content>