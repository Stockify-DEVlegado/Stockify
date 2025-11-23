<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" CodeBehind="Ordenes.aspx.cs" Inherits="StockifyWeb.Ordenes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_Contenido" runat="server">

    <style>
        :root {
            --bg-global: #0b0c0f;
            --bg-card: #21252d;
            --bg-table: #2a2f39;
            --txt-main: #e6e8eb;
            --txt-muted: #9a9da3;
            --accent-green: #00b46e;
            --accent-blue: #007bff;
            --accent-red: #e74c3c;
            --accent-orange: #ffa500;
            --border-color: #2d313a;
            --border-radius: 6px;
        }

        .ordenes-container {
            background-color: var(--bg-global);
            color: var(--txt-main);
            min-height: 100vh;
            padding: 1rem;
        }

        /* Barra superior de navegación - DISEÑO MINIMALISTA */
        .modes-header {
            padding: 0.5rem 0;
            margin-bottom: 1.5rem;
        }
        
        .modes-buttons {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 0.8rem;
            width: 100%;
        }
        
        .mode-btn {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            background-color: var(--bg-card);
            color: var(--txt-main);
            border: 1px solid var(--border-color);
            padding: 0.5rem 0.3rem;
            border-radius: var(--border-radius);
            font-size: 0.8rem;
            transition: all 0.2s ease;
            cursor: pointer;
            font-weight: 500;
            height: 50px;
            text-align: center;
        }
        
        .mode-btn:hover {
            background-color: var(--bg-table);
            border-color: var(--accent-blue);
        }
        
        .mode-btn.active {
            background-color: var(--accent-blue);
            color: white;
            border-color: var(--accent-blue);
        }

        .mode-btn i {
            font-size: 1rem;
            margin-bottom: 0.2rem;
        }

        .search-section {
            display: flex;
            gap: 1rem;
            margin-bottom: 1rem;
            align-items: center;
        }

        .search-box {
            flex: 1;
            display: flex;
            align-items: center;
            background-color: var(--bg-card);
            border-radius: 6px;
            padding: 0.5rem 1rem;
            border: 1px solid var(--border-color);
        }

        .search-box input {
            background: transparent;
            border: none;
            color: var(--txt-main);
            flex: 1;
            margin-left: 0.5rem;
            font-size: 0.9rem;
        }

        /* CONTENEDORES DE ORDENES */
        .orden-content {
            display: none;
        }

        .orden-content.active {
            display: block;
        }

        .form-card {
            background-color: var(--bg-card);
            border-radius: 8px;
            padding: 1.2rem;
            margin-bottom: 1.2rem;
            border: 1px solid var(--border-color);
        }

        .form-grid-compact {
            display: grid;
            grid-template-columns: 1fr 1fr 1fr;
            gap: 0.8rem;
            margin-bottom: 0.8rem;
        }

        .form-grid-compact-4 {
            display: grid;
            grid-template-columns: 1fr 1fr 1fr auto;
            gap: 0.8rem;
            margin-bottom: 0.8rem;
            align-items: end;
        }

        .form-group {
            display: flex;
            flex-direction: column;
        }

        .form-group label {
            margin-bottom: 0.4rem;
            color: var(--txt-muted);
            font-weight: 500;
            font-size: 0.85rem;
        }

        .form-group input, 
        .form-group select,
        .form-group textarea {
            padding: 0.4rem 0.6rem;
            border-radius: 4px;
            border: 1px solid var(--border-color);
            background-color: var(--bg-table);
            color: var(--txt-main);
            font-size: 0.85rem;
        }

        .file-upload-section {
            display: flex;
            align-items: center;
            gap: 0.8rem;
            margin-top: 0.4rem;
        }

        .date-with-button {
            display: flex;
            gap: 0.5rem;
            align-items: flex-end;
        }

        .date-with-button .form-group {
            flex: 1;
        }

        .date-with-button .btn-agregar {
            height: fit-content;
            margin-bottom: 0.4rem;
            white-space: nowrap;
        }

        .btn-right-align {
            display: flex;
            justify-content: flex-end;
            align-items: flex-end;
            height: 100%;
        }

        .btn-right-align .btn-agregar {
            height: fit-content;
            white-space: nowrap;
        }

        .actions-bar {
            display: flex;
            gap: 0.5rem;
            justify-content: flex-start;
            margin-top: 0.8rem;
        }

        .actions-bar button {
            padding: 0.4rem 0.8rem;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-weight: 500;
            font-size: 0.85rem;
            min-width: 70px;
        }

        .btn-agregar { background-color: var(--accent-green); color: white; }
        .btn-anular { background-color: var(--accent-red); color: white; }
        .btn-view { background-color: var(--accent-orange); color: white; }

        .table-section {
            background-color: var(--bg-card);
            border-radius: 8px;
            padding: 0.8rem;
            margin-bottom: 0.8rem;
            border: 1px solid var(--border-color);
        }

        .table-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 0.6rem;
        }

        .table-header h3 {
            margin: 0;
            color: var(--txt-main);
            font-size: 1rem;
            font-weight: 600;
        }

        .table-responsive {
            overflow-x: auto;
            border: 1px solid var(--border-color);
            border-radius: 4px;
        }

        .data-table {
            width: 100%;
            border-collapse: collapse;
            background-color: var(--bg-table);
            font-size: 0.8rem;
        }

        .data-table th,
        .data-table td {
            padding: 0.5rem;
            text-align: left;
            border: 1px solid var(--border-color);
        }

        .data-table th {
            background-color: var(--bg-card);
            color: var(--txt-main);
            font-weight: 600;
        }

        .data-table tbody tr {
            transition: background-color 0.1s ease;
        }

        .data-table tbody tr:hover {
            background-color: rgba(255, 255, 255, 0.03);
        }

        .data-table tbody tr.selected {
            background-color: var(--accent-blue);
            color: white;
        }

        .checkbox-column {
            width: 25px;
            text-align: center;
        }

        .accion-column {
            width: 80px;
            text-align: center;
        }

        .button-group {
            display: flex;
            gap: 0.3rem;
            justify-content: center;
            align-items: center;
        }

        .accion-column .btn-anular {
            padding: 0.25rem 0.5rem;
            font-size: 0.7rem;
            min-width: 60px;
            white-space: nowrap;
        }

        .badge {
            padding: 0.15rem 0.4rem;
            border-radius: 8px;
            font-size: 0.7rem;
            font-weight: 500;
        }

        .badge-pendiente { background-color: var(--accent-orange); color: black; }
        .badge-procesando { background-color: var(--accent-blue); color: white; }
        .badge-cancelado { background-color: var(--accent-red); color: white; }
        .badge-aceptado { background-color: var(--accent-green); color: white; }

        /* Estilos para modales de importación */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.8);
            backdrop-filter: blur(4px);
            z-index: 1000;
            justify-content: center;
            align-items: center;
            animation: fadeIn 0.3s ease;
        }

        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }

        .modal-content {
            background: var(--bg-card);
            border: 1px solid var(--border-color);
            border-radius: 8px;
            padding: 28px;
            width: 90%;
            max-width: 600px;
            max-height: 90vh;
            overflow-y: auto;
            box-shadow: 0 20px 60px rgba(0,0,0,.6);
            animation: slideUp 0.3s ease;
        }

        @keyframes slideUp {
            from {
                transform: translateY(30px);
                opacity: 0;
            }
            to {
                transform: translateY(0);
                opacity: 1;
            }
        }

        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            border-bottom: 1px solid var(--border-color);
            padding-bottom: 15px;
        }

        .modal-title {
            color: var(--txt-main);
            font-size: 22px;
            font-weight: 700;
            margin: 0;
        }

        .close-modal {
            background: none;
            border: none;
            color: var(--txt-muted);
            font-size: 24px;
            cursor: pointer;
            padding: 0;
            width: 30px;
            height: 30px;
            transition: all 0.2s;
        }

        .close-modal:hover {
            color: var(--txt-main);
            transform: rotate(90deg);
        }

        .upload-area-csv {
            border: 3px dashed var(--accent-green);
            border-radius: 15px;
            padding: 40px;
            text-align: center;
            background: rgba(16, 185, 129, 0.05);
            cursor: pointer;
            transition: all 0.3s ease;
            margin: 20px 0;
        }

        .upload-area-csv:hover {
            background: rgba(16, 185, 129, 0.1);
            border-color: #059669;
        }

        .upload-icon {
            font-size: 48px;
            margin-bottom: 15px;
        }

        .upload-text {
            color: var(--accent-green);
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 8px;
        }

        .upload-subtext {
            color: var(--txt-muted);
            font-size: 14px;
        }

        .file-info {
            background: var(--bg-table);
            border-radius: 10px;
            padding: 15px;
            margin: 15px 0;
            display: none;
        }

        .file-info.visible {
            display: block;
        }

        .file-name {
            color: var(--txt-main);
            font-weight: 600;
            margin-bottom: 5px;
        }

        .file-size {
            color: var(--txt-muted);
            font-size: 14px;
        }

        .modal-actions {
            display: flex;
            justify-content: center;
            gap: 12px;
            margin-top: 24px;
            padding-top: 20px;
        }

        .btn-submit {
            background: var(--accent-green);
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 10px;
            cursor: pointer;
            font-weight: 600;
            font-size: 14px;
            transition: all 0.3s ease;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
        }

        .btn-submit:hover {
            background: #059669;
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(16, 185, 129, 0.4);
        }

        .btn-discard {
            background: var(--bg-table);
            color: var(--txt-main);
            border: 1px solid var(--border-color);
            padding: 12px 24px;
            border-radius: 10px;
            cursor: pointer;
            font-weight: 600;
            font-size: 14px;
            transition: all 0.3s ease;
        }

        .btn-discard:hover {
            background: var(--border-color);
            transform: translateY(-1px);
        }

        /* Responsive */
        @media (max-width: 768px) {
            .modes-buttons {
                grid-template-columns: repeat(2, 1fr);
                gap: 0.6rem;
            }
            
            .mode-btn {
                height: 45px;
                padding: 0.4rem 0.2rem;
                font-size: 0.75rem;
            }
            
            .mode-btn i {
                font-size: 0.9rem;
                margin-bottom: 0.15rem;
            }
            
            .form-grid-compact {
                grid-template-columns: 1fr;
            }

            .form-grid-compact-4 {
                grid-template-columns: 1fr;
            }

            .date-with-button {
                flex-direction: column;
                align-items: stretch;
            }

            .date-with-button .btn-agregar {
                margin-bottom: 0;
                margin-top: 0.5rem;
            }

            .btn-right-align {
                justify-content: flex-start;
                margin-top: 0.5rem;
            }
        }

        @media (max-width: 480px) {
            .modes-buttons {
                grid-template-columns: 1fr;
                gap: 0.5rem;
            }
            
            .mode-btn {
                height: 42px;
            }
        }
    </style>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">

    <div class="ordenes-container">
        <!-- Barra superior de navegación - DISEÑO MINIMALISTA -->
        <div class="modes-header">
            <div class="modes-buttons">
                <asp:Button ID="btnCompra" runat="server" Text="Orden Compra" CssClass="mode-btn active" OnClick="btnCompra_Click" />
                <asp:Button ID="btnVenta" runat="server" Text="Orden Venta" CssClass="mode-btn" OnClick="btnVenta_Click" />
                <asp:Button ID="btnIngreso" runat="server" Text="Ingreso" CssClass="mode-btn" OnClick="btnIngreso_Click" />
                <asp:Button ID="btnSalida" runat="server" Text="Salida" CssClass="mode-btn" OnClick="btnSalida_Click" />
            </div>
        </div>

        <!-- CONTENIDO COMPRA -->
        <div id="compraContent" class="orden-content active" runat="server">
            <div class="form-card">
                <div class="form-grid-compact">
                    <div class="form-group">
                        <label>Nombre Proveedor</label>
                        <asp:DropDownList ID="ddlProveedor" runat="server">
                            <asp:ListItem Text="-- Seleccione un proveedor --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <!-- FECHA CON BOTÓN AGREGAR AL LADO -->
                    <div class="date-with-button">
                        <div class="form-group">
                            <label>Fecha de Orden de Compra</label>
                            <asp:TextBox ID="txtFechaOrdenCompra" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnAgregarCompra" runat="server" Text="Agregar" CssClass="btn-agregar" OnClick="btnAgregarCompra_Click" />
                    </div>
                </div>
            </div>

            <!-- BARRA DE BÚSQUEDA PARA COMPRA -->
            <div class="search-section">
                <div class="search-box">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
                    </svg>
                    <asp:TextBox ID="txtBuscarCompra" runat="server" placeholder="Buscar órdenes de compra..."></asp:TextBox>
                </div>
            </div>

            <!-- Tabla: Ordenes de Compra -->
            <div class="table-section">
                <div class="table-header">
                    <h3>Órdenes de Compra</h3>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvOrdenesCompra" runat="server" CssClass="data-table" AutoGenerateColumns="false"
                        OnRowDataBound="gvOrdenesCompra_RowDataBound" DataKeyNames="IdOrdenCompra">
                        <Columns>
                            <asp:TemplateField HeaderText="" ItemStyle-CssClass="checkbox-column">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSeleccionCompra" runat="server" AutoPostBack="true" OnCheckedChanged="chkSeleccionCompra_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="FechaRegistrada" HeaderText="Fecha Registrada" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre Proveedor" />
                            <asp:BoundField DataField="Total" HeaderText="Total" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoCompra" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado").ToString()) %>' 
                                        Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acción" ItemStyle-CssClass="accion-column">
                                <ItemTemplate>
                                    <div class="button-group">
                                        <asp:Button ID="btnAnularCompraFila" runat="server" Text="Anular" CssClass="btn-anular" CommandArgument='<%# Eval("IdOrdenCompra") %>' OnClick="btnAnularCompraFila_Click" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="7" style="text-align: center; padding: 1.5rem; color: var(--txt-muted);">
                                    No hay datos para mostrar
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <!-- Tabla: Líneas de Orden de Compra -->
            <div class="table-section">
                <div class="table-header">
                    <h3>Líneas de Orden de Compra</h3>
                    <!-- BOTÓN NUEVO PARA IMPORTAR CSV -->
                    <div style="display: flex; gap: 10px; align-items: center;">
                        <asp:Button ID="btnImportarLineasCompra" runat="server" Text="📥 Importar CSV" 
                            CssClass="btn-agregar" OnClientClick="return abrirModalImportarCompra();" 
                            style="background: linear-gradient(135deg, #10b981 0%, #059669 100%); border: none; padding: 8px 16px; font-size: 0.85rem;" />
                    </div>
                </div>
                <!-- SECCIÓN ADJUNTAR DOCUMENTO PARA COMPRA EN LÍNEAS -->
                <div class="form-group" style="margin-bottom: 1rem;">
                    <label>Adjuntar Documento de Compra</label>
                    <div class="file-upload-section">
                        <asp:FileUpload ID="fileDocumentoCompra" runat="server" CssClass="file-upload" />
                    </div>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvDetalleOrdenCompra" runat="server" CssClass="data-table" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="Marca" HeaderText="Marca" />
                            <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unitario" />
                            <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoDetalleCompra" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado").ToString()) %>' 
                                        Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="9" style="text-align: center; padding: 1.5rem; color: var(--txt-muted);">
                                    No hay datos para mostrar
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- CONTENIDO VENTA -->
        <div id="ventaContent" class="orden-content" runat="server">
            <div class="form-card">
                <div class="form-grid-compact">
                    <div class="form-group">
                        <label>Nombre Cliente</label>
                        <asp:DropDownList ID="ddlCliente" runat="server">
                            <asp:ListItem Text="-- Seleccione un cliente --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <!-- FECHA CON BOTÓN AGREGAR AL LADO -->
                    <div class="date-with-button">
                        <div class="form-group">
                            <label>Fecha de Orden Venta</label>
                            <asp:TextBox ID="txtFechaOrdenVenta" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnAgregarVenta" runat="server" Text="Agregar" CssClass="btn-agregar" OnClick="btnAgregarVenta_Click" />
                    </div>
                </div>
            </div>

            <!-- BARRA DE BÚSQUEDA PARA VENTA -->
            <div class="search-section">
                <div class="search-box">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
                    </svg>
                    <asp:TextBox ID="txtBuscarVenta" runat="server" placeholder="Buscar órdenes de venta..."></asp:TextBox>
                </div>
            </div>

            <!-- Tabla: Ordenes de Venta -->
            <div class="table-section">
                <div class="table-header">
                    <h3>Órdenes de Venta</h3>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvOrdenesVenta" runat="server" CssClass="data-table" AutoGenerateColumns="false"
                        OnRowDataBound="gvOrdenesVenta_RowDataBound" DataKeyNames="IdOrdenVenta">
                        <Columns>
                            <asp:TemplateField HeaderText="" ItemStyle-CssClass="checkbox-column">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSeleccionVenta" runat="server" AutoPostBack="true" OnCheckedChanged="chkSeleccionVenta_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="FechaRegistrada" HeaderText="Fecha Registrada" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre Cliente" />
                            <asp:BoundField DataField="Total" HeaderText="Total" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoVenta" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado").ToString()) %>' 
                                        Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acción" ItemStyle-CssClass="accion-column">
                                <ItemTemplate>
                                    <div class="button-group">
                                        <asp:Button ID="btnAnularVentaFila" runat="server" Text="Anular" CssClass="btn-anular" CommandArgument='<%# Eval("IdOrdenVenta") %>' OnClick="btnAnularVentaFila_Click" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="8" style="text-align: center; padding: 1.5rem; color: var(--txt-muted);">
                                    No hay datos para mostrar
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <!-- Tabla: Líneas de Orden de Venta -->
            <div class="table-section">
                <div class="table-header">
                    <h3>Líneas de Orden de Venta</h3>
                    <!-- BOTÓN NUEVO PARA IMPORTAR CSV PARA VENTA -->
                    <div style="display: flex; gap: 10px; align-items: center;">
                        <asp:Button ID="btnImportarLineasVenta" runat="server" Text="📥 Importar CSV" 
                            CssClass="btn-agregar" OnClientClick="return abrirModalImportarVenta();" 
                            style="background: linear-gradient(135deg, #10b981 0%, #059669 100%); border: none; padding: 8px 16px; font-size: 0.85rem;" />
                    </div>
                </div>
                <!-- SECCIÓN ADJUNTAR DOCUMENTO PARA VENTA EN LÍNEAS -->
                <div class="form-group" style="margin-bottom: 1rem;">
                    <label>Adjuntar Documento de Venta</label>
                    <div class="file-upload-section">
                        <asp:FileUpload ID="fileDocumentoVenta" runat="server" CssClass="file-upload" />
                    </div>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvDetalleOrdenVenta" runat="server" CssClass="data-table" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="Marca" HeaderText="Marca" />
                            <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unitario" />
                            <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoDetalleVenta" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado").ToString()) %>' 
                                        Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="9" style="text-align: center; padding: 1.5rem; color: var(--txt-muted);">
                                    No hay datos para mostrar
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- CONTENIDO INGRESO -->
        <div id="ingresoContent" class="orden-content" runat="server">
            <div class="form-card">
                <!-- FORM GRID CON 4 COLUMNAS Y BOTÓN A LA DERECHA -->
                <div class="form-grid-compact-4">
                    <div class="form-group">
                        <label>Orden de Compra Asociada</label>
                        <asp:DropDownList ID="ddlOrdenCompraIngreso" runat="server">
                            <asp:ListItem Text="-- Seleccione una orden de compra --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Fecha de Ingreso</label>
                        <asp:TextBox ID="txtFechaIngreso" runat="server" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Responsable</label>
                        <asp:DropDownList ID="ddlResponsableIngreso" runat="server">
                            <asp:ListItem Text="-- Seleccione un responsable --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <!-- BOTÓN AGREGAR A LA DERECHA -->
                    <div class="btn-right-align">
                        <asp:Button ID="btnAgregarIngreso" runat="server" Text="Agregar" CssClass="btn-agregar" OnClick="btnAgregarIngreso_Click" />
                    </div>
                </div>
            </div>

            <!-- BARRA DE BÚSQUEDA PARA INGRESO -->
            <div class="search-section">
                <div class="search-box">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
                    </svg>
                    <asp:TextBox ID="txtBuscarIngreso" runat="server" placeholder="Buscar registros de ingreso..."></asp:TextBox>
                </div>
            </div>

            <!-- Tabla: Registros de Ingreso -->
            <div class="table-section">
                <div class="table-header">
                    <h3>Registros de Ingreso</h3>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvRegistrosIngreso" runat="server" CssClass="data-table" AutoGenerateColumns="false"
                        OnRowDataBound="gvRegistrosIngreso_RowDataBound" DataKeyNames="IdIngreso">
                        <Columns>
                            <asp:TemplateField HeaderText="" ItemStyle-CssClass="checkbox-column">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSeleccionIngreso" runat="server" AutoPostBack="true" OnCheckedChanged="chkSeleccionIngreso_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="FechaRegistrada" HeaderText="Fecha Registrada" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre Proveedor" />
                            <asp:BoundField DataField="Responsable" HeaderText="Responsable" />
                            <asp:BoundField DataField="Total" HeaderText="Total" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoIngreso" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado").ToString()) %>' 
                                        Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acción" ItemStyle-CssClass="accion-column">
                                <ItemTemplate>
                                    <div class="button-group">
                                        <asp:Button ID="btnAnularIngresoFila" runat="server" Text="Anular" CssClass="btn-anular" CommandArgument='<%# Eval("IdIngreso") %>' OnClick="btnAnularIngresoFila_Click" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="8" style="text-align: center; padding: 1.5rem; color: var(--txt-muted);">
                                    No hay datos para mostrar
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <!-- Tabla: Líneas de Orden de Compra Asociada -->
            <div class="table-section">
                <div class="table-header">
                    <h3>Líneas de Orden de Compra Asociada</h3>
                </div>
                <!-- SECCIÓN ADJUNTAR DOCUMENTO PARA INGRESO EN LÍNEAS -->
                <div class="form-group" style="margin-bottom: 1rem;">
                    <label>Adjuntar Documento de Ingreso</label>
                    <div class="file-upload-section">
                        <asp:FileUpload ID="fileDocumentoIngreso" runat="server" CssClass="file-upload" />
                    </div>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvDetalleOrdenCompraIngreso" runat="server" CssClass="data-table" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="Marca" HeaderText="Marca" />
                            <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unitario" />
                            <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoDetalleCompraIngreso" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado").ToString()) %>' 
                                        Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="9" style="text-align: center; padding: 1.5rem; color: var(--txt-muted);">
                                    Seleccione un ingreso para ver los detalles
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- CONTENIDO SALIDA -->
        <div id="salidaContent" class="orden-content" runat="server">
            <div class="form-card">
                <!-- FORM GRID CON 4 COLUMNAS Y BOTÓN A LA DERECHA -->
                <div class="form-grid-compact-4">
                    <div class="form-group">
                        <label>Orden de Venta Asociada</label>
                        <asp:DropDownList ID="ddlOrdenVentaSalida" runat="server">
                            <asp:ListItem Text="-- Seleccione una orden de venta --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Fecha de Salida</label>
                        <asp:TextBox ID="txtFechaSalida" runat="server" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Responsable</label>
                        <asp:DropDownList ID="ddlResponsableSalida" runat="server">
                            <asp:ListItem Text="-- Seleccione un responsable --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <!-- BOTÓN AGREGAR A LA DERECHA -->
                    <div class="btn-right-align">
                        <asp:Button ID="btnAgregarSalida" runat="server" Text="Agregar" CssClass="btn-agregar" OnClick="btnAgregarSalida_Click" />
                    </div>
                </div>
            </div>

            <!-- BARRA DE BÚSQUEDA PARA SALIDA -->
            <div class="search-section">
                <div class="search-box">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
                    </svg>
                    <asp:TextBox ID="txtBuscarSalida" runat="server" placeholder="Buscar registros de salida..."></asp:TextBox>
                </div>
            </div>

            <!-- Tabla: Registros de Salida -->
            <div class="table-section">
                <div class="table-header">
                    <h3>Registros de Salida</h3>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvRegistrosSalida" runat="server" CssClass="data-table" AutoGenerateColumns="false"
                        OnRowDataBound="gvRegistrosSalida_RowDataBound" DataKeyNames="IdSalida">
                        <Columns>
                            <asp:TemplateField HeaderText="" ItemStyle-CssClass="checkbox-column">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSeleccionSalida" runat="server" AutoPostBack="true" OnCheckedChanged="chkSeleccionSalida_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="FechaRegistrada" HeaderText="Fecha Registrada" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre Cliente" />
                            <asp:BoundField DataField="Responsable" HeaderText="Responsable" />
                            <asp:BoundField DataField="Total" HeaderText="Total" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoSalida" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado").ToString()) %>' 
                                        Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acción" ItemStyle-CssClass="accion-column">
                                <ItemTemplate>
                                    <div class="button-group">
                                        <asp:Button ID="btnAnularSalidaFila" runat="server" Text="Anular" CssClass="btn-anular" CommandArgument='<%# Eval("IdSalida") %>' OnClick="btnAnularSalidaFila_Click" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="7" style="text-align: center; padding: 1.5rem; color: var(--txt-muted);">
                                    No hay datos para mostrar
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <!-- Tabla: Líneas de Orden de Venta Asociada -->
            <div class="table-section">
                <div class="table-header">
                    <h3>Líneas de Orden de Venta Asociada</h3>
                </div>
                <!-- SECCIÓN ADJUNTAR DOCUMENTO PARA SALIDA EN LÍNEAS -->
                <div class="form-group" style="margin-bottom: 1rem;">
                    <label>Adjuntar Documento de Salida</label>
                    <div class="file-upload-section">
                        <asp:FileUpload ID="fileDocumentoSalida" runat="server" CssClass="file-upload" />
                    </div>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvDetalleOrdenVentaSalida" runat="server" CssClass="data-table" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="Marca" HeaderText="Marca" />
                            <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unitario" />
                            <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="SubTotal" HeaderText="SubTotal" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoDetalleVentaSalida" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado").ToString()) %>' 
                                        Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="9" style="text-align: center; padding: 1.5rem; color: var(--txt-muted);">
                                    Seleccione una salida para ver los detalles
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal para Importar Líneas de Compra por CSV -->
    <div class="modal-overlay" id="importModalCompra">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">📥 Importar Líneas de Compra desde CSV</h2>
                <button class="close-modal" type="button" onclick="cerrarModalImportarCompra()">&times;</button>
            </div>
            
            <div class="info-box" style="background: rgba(138, 162, 255, 0.1); border-left: 4px solid var(--accent-blue); padding: 15px; margin-bottom: 20px; border-radius: 5px;">
                <strong>⚠️ Formato del archivo CSV:</strong>
                <ul style="margin-left: 20px; color: var(--txt-muted); font-size: 13px;">
                    <li><strong>Columnas:</strong> idProducto, cantidad, precioUnitario</li>
                    <li><strong>Primera fila:</strong> Debe ser el encabezado</li>
                    <li><strong>Separador:</strong> Coma (,)</li>
                    <li><strong>Codificación:</strong> UTF-8</li>
                    <li><strong>Ejemplo:</strong> 1,10,25.50</li>
                </ul>
            </div>

            <div class="upload-area-csv" id="uploadAreaCSVCompra" 
                 onclick="document.getElementById('<%= fuCSVCompra.ClientID %>').click();">
                <div class="upload-icon">📄</div>
                <div class="upload-text">Haz clic para seleccionar tu archivo CSV</div>
                <div class="upload-subtext">Solo archivos .csv (máx. 10MB)</div>
            </div>

            <asp:FileUpload ID="fuCSVCompra" runat="server" accept=".csv" style="display: none;" 
                onchange="mostrarArchivoSeleccionadoCompra(this);" />

            <div class="file-info" id="fileInfoCSVCompra">
                <div class="file-name" id="fileNameCSVCompra"></div>
                <div class="file-size" id="fileSizeCSVCompra"></div>
            </div>

            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModalImportarCompra()">Cancelar</button>
                <asp:Button ID="btnImportarCSVCompra" runat="server" Text="📤 Importar Líneas"
                    CssClass="btn-submit" OnClick="btnImportarCSVCompra_Click" 
                    style="background: linear-gradient(135deg, #10b981 0%, #059669 100%);" />
            </div>
        </div>
    </div>

    <!-- Modal para Importar Líneas de Venta por CSV -->
    <div class="modal-overlay" id="importModalVenta">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">📥 Importar Líneas de Venta desde CSV</h2>
                <button class="close-modal" type="button" onclick="cerrarModalImportarVenta()">&times;</button>
            </div>
            
            <div class="info-box" style="background: rgba(138, 162, 255, 0.1); border-left: 4px solid var(--accent-blue); padding: 15px; margin-bottom: 20px; border-radius: 5px;">
                <strong>⚠️ Formato del archivo CSV:</strong>
                <ul style="margin-left: 20px; color: var(--txt-muted); font-size: 13px;">
                    <li><strong>Columnas:</strong> idProducto, cantidad, precioUnitario</li>
                    <li><strong>Primera fila:</strong> Debe ser el encabezado</li>
                    <li><strong>Separador:</strong> Coma (,)</li>
                    <li><strong>Codificación:</strong> UTF-8</li>
                    <li><strong>Ejemplo:</strong> 1,10,25.50</li>
                </ul>
            </div>

            <div class="upload-area-csv" id="uploadAreaCSVVenta" 
                 onclick="document.getElementById('<%= fuCSVVenta.ClientID %>').click();">
                <div class="upload-icon">📄</div>
                <div class="upload-text">Haz clic para seleccionar tu archivo CSV</div>
                <div class="upload-subtext">Solo archivos .csv (máx. 10MB)</div>
            </div>

            <asp:FileUpload ID="fuCSVVenta" runat="server" accept=".csv" style="display: none;" 
                onchange="mostrarArchivoSeleccionadoVenta(this);" />

            <div class="file-info" id="fileInfoCSVVenta">
                <div class="file-name" id="fileNameCSVVenta"></div>
                <div class="file-size" id="fileSizeCSVVenta"></div>
            </div>

            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModalImportarVenta()">Cancelar</button>
                <asp:Button ID="btnImportarCSVVenta" runat="server" Text="📤 Importar Líneas"
                    CssClass="btn-submit" OnClick="btnImportarCSVVenta_Click" 
                    style="background: linear-gradient(135deg, #10b981 0%, #059669 100%);" />
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnOrdenCompraSeleccionada" runat="server" Value="0" />
    <asp:HiddenField ID="hdnOrdenVentaSeleccionada" runat="server" Value="0" />

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const btnCompra = document.getElementById('<%= btnCompra.ClientID %>');
            if (btnCompra) {
                btnCompra.innerHTML = '<i class="fas fa-shopping-cart"></i> ' + btnCompra.textContent;
            }

            const btnVenta = document.getElementById('<%= btnVenta.ClientID %>');
            if (btnVenta) {
                btnVenta.innerHTML = '<i class="fas fa-cash-register"></i> ' + btnVenta.textContent;
            }

            const btnIngreso = document.getElementById('<%= btnIngreso.ClientID %>');
            if (btnIngreso) {
                btnIngreso.innerHTML = '<i class="fas fa-sign-in-alt"></i> ' + btnIngreso.textContent;
            }

            const btnSalida = document.getElementById('<%= btnSalida.ClientID %>');
            if (btnSalida) {
                btnSalida.innerHTML = '<i class="fas fa-sign-out-alt"></i> ' + btnSalida.textContent;
            }
        });

        // Modal Importar CSV para Compra
        function abrirModalImportarCompra() {
            // Verificar que hay una orden seleccionada
            var ordenSeleccionada = document.getElementById('<%= hdnOrdenCompraSeleccionada.ClientID %>');
            if (ordenSeleccionada && ordenSeleccionada.value === "0") {
                alert('⚠️ Por favor, selecciona una orden de compra primero.');
                return false;
            }
            
            var modal = document.getElementById('importModalCompra');
            if (modal) {
                modal.style.display = 'flex';
            }
            return false;
        }

        function cerrarModalImportarCompra() {
            var modal = document.getElementById('importModalCompra');
            if (modal) {
                modal.style.display = 'none';
            }
            
            // Limpiar file upload
            var fileUpload = document.getElementById('<%= fuCSVCompra.ClientID %>');
            if (fileUpload) {
                fileUpload.value = '';
            }

            // Ocultar info del archivo
            var fileInfo = document.getElementById('fileInfoCSVCompra');
            if (fileInfo) {
                fileInfo.style.display = 'none';
            }
        }

        function mostrarArchivoSeleccionadoCompra(input) {
            if (input.files && input.files[0]) {
                var file = input.files[0];
                var fileInfo = document.getElementById('fileInfoCSVCompra');

                if (fileInfo) {
                    document.getElementById('fileNameCSVCompra').textContent = '📄 ' + file.name;
                    document.getElementById('fileSizeCSVCompra').textContent = 'Tamaño: ' + formatFileSize(file.size);
                    fileInfo.style.display = 'block';
                }
            }
        }

        // Modal Importar CSV para Venta
        function abrirModalImportarVenta() {
            // Verificar que hay una orden seleccionada
            var ordenSeleccionada = document.getElementById('<%= hdnOrdenVentaSeleccionada.ClientID %>');
            if (ordenSeleccionada && ordenSeleccionada.value === "0") {
                alert('⚠️ Por favor, selecciona una orden de venta primero.');
                return false;
            }
            
            var modal = document.getElementById('importModalVenta');
            if (modal) {
                modal.style.display = 'flex';
            }
            return false;
        }

        function cerrarModalImportarVenta() {
            var modal = document.getElementById('importModalVenta');
            if (modal) {
                modal.style.display = 'none';
            }
            
            // Limpiar file upload
            var fileUpload = document.getElementById('<%= fuCSVVenta.ClientID %>');
            if (fileUpload) {
                fileUpload.value = '';
            }

            // Ocultar info del archivo
            var fileInfo = document.getElementById('fileInfoCSVVenta');
            if (fileInfo) {
                fileInfo.style.display = 'none';
            }
        }

        function mostrarArchivoSeleccionadoVenta(input) {
            if (input.files && input.files[0]) {
                var file = input.files[0];
                var fileInfo = document.getElementById('fileInfoCSVVenta');

                if (fileInfo) {
                    document.getElementById('fileNameCSVVenta').textContent = '📄 ' + file.name;
                    document.getElementById('fileSizeCSVVenta').textContent = 'Tamaño: ' + formatFileSize(file.size);
                    fileInfo.style.display = 'block';
                }
            }
        }

        function formatFileSize(bytes) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        }

        // Cerrar modales al hacer clic fuera
        document.addEventListener('DOMContentLoaded', function () {
            var modalCompra = document.getElementById('importModalCompra');
            if (modalCompra) {
                modalCompra.addEventListener('click', function (e) {
                    if (e.target === this) {
                        cerrarModalImportarCompra();
                    }
                });
            }

            var modalVenta = document.getElementById('importModalVenta');
            if (modalVenta) {
                modalVenta.addEventListener('click', function (e) {
                    if (e.target === this) {
                        cerrarModalImportarVenta();
                    }
                });
            }
        });

        // Cerrar con tecla ESC
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                cerrarModalImportarCompra();
                cerrarModalImportarVenta();
            }
        });
    </script>

</asp:Content>