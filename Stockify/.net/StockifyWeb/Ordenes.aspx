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
        }

        .ordenes-container {
            background-color: var(--bg-global);
            color: var(--txt-main);
            min-height: 100vh;
            padding: 1rem;
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
            border-radius: 8px;
            padding: 0.5rem 1rem;
        }

        .search-box input {
            background: transparent;
            border: none;
            color: var(--txt-main);
            flex: 1;
            margin-left: 0.5rem;
        }

        .toggle-tabs {
            display: flex;
            background-color: var(--bg-card);
            border-radius: 8px;
            padding: 0.25rem;
        }

        .toggle-tabs button {
            padding: 0.5rem 1.5rem;
            border: none;
            background: transparent;
            color: var(--txt-muted);
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .toggle-tabs button.active {
            background-color: var(--accent-blue);
            color: white;
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
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 1.5rem;
        }

        .form-grid-compact {
            display: grid;
            grid-template-columns: 1fr 1fr 1fr;
            gap: 1rem;
            margin-bottom: 1rem;
        }

        .form-group {
            display: flex;
            flex-direction: column;
        }

        .form-group label {
            margin-bottom: 0.5rem;
            color: var(--txt-muted);
            font-weight: bold;
            font-size: 0.9rem;
        }

        .form-group input, 
        .form-group select,
        .form-group textarea {
            padding: 0.5rem;
            border-radius: 6px;
            border: 1px solid var(--border-color);
            background-color: var(--bg-table);
            color: var(--txt-main);
            font-size: 0.9rem;
        }

        .file-actions {
            display: flex;
            gap: 0.5rem;
            align-items: center;
            margin: 1rem 0;
        }

        .file-actions button {
            background-color: var(--accent-blue);
            color: white;
            border: none;
            padding: 0.4rem 0.8rem;
            border-radius: 4px;
            cursor: pointer;
            font-size: 0.9rem;
        }

        .file-actions input {
            background-color: var(--bg-table);
            border: 1px solid var(--border-color);
            border-radius: 4px;
            padding: 0.4rem;
            color: var(--txt-main);
            flex: 1;
        }

        .actions-bar {
            display: flex;
            gap: 0.5rem;
            justify-content: flex-start;
            margin-top: 1rem;
        }

        .actions-bar button {
            padding: 0.5rem 1rem;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-weight: bold;
            font-size: 0.9rem;
            min-width: 80px;
        }

        .btn-agregar { background-color: var(--accent-green); color: white; }
        .btn-anular { background-color: var(--accent-red); color: white; }
        .btn-eliminar { background-color: var(--accent-red); color: white; }
        .btn-editar { background-color: var(--accent-orange); color: white; }
        .btn-guardar { background-color: var(--accent-blue); color: white; }
        .btn-view { background-color: var(--accent-blue); color: white; }

        .table-section {
            background-color: var(--bg-card);
            border-radius: 12px;
            padding: 1rem;
            margin-bottom: 1rem;
        }

        .table-section h3 {
            margin-bottom: 0.8rem;
            color: var(--txt-main);
            border-bottom: 2px solid var(--accent-blue);
            padding-bottom: 0.3rem;
            font-size: 1.1rem;
        }

        .table-responsive {
            overflow-x: auto;
            border: 1px solid var(--border-color);
            border-radius: 6px;
        }

        .data-table {
            width: 100%;
            border-collapse: collapse;
            background-color: var(--bg-table);
            font-size: 0.85rem;
        }

        .data-table th,
        .data-table td {
            padding: 0.6rem;
            text-align: left;
            border: 1px solid var(--border-color);
        }

        .data-table th {
            background-color: var(--bg-card);
            color: var(--txt-main);
            font-weight: bold;
            border-bottom: 2px solid var(--accent-blue);
        }

        .data-table tbody tr:hover {
            background-color: rgba(255, 255, 255, 0.05);
        }

        .checkbox-column {
            width: 30px;
            text-align: center;
        }

        .badge {
            padding: 0.2rem 0.5rem;
            border-radius: 10px;
            font-size: 0.75rem;
            font-weight: bold;
        }

        .badge-procesado { background-color: var(--accent-orange); color: black; }
        .badge-cancelado { background-color: var(--accent-red); color: white; }
        .badge-aceptado { background-color: var(--accent-green); color: white; }
        .badge-disponible { background-color: var(--accent-green); color: white; }
    </style>

    <div class="ordenes-container">
        <!-- Barra de búsqueda y tabs -->
        <div class="search-section">
            <div class="search-box">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
                </svg>
                <asp:TextBox ID="txtBuscar" runat="server" placeholder="Search product, supplier, order"></asp:TextBox>
            </div>
            <div class="toggle-tabs">
                <asp:Button ID="btnOrdenCompra" runat="server" Text="Orden Compra" CssClass="active" OnClick="btnOrdenCompra_Click" />
                <asp:Button ID="btnOrdenVenta" runat="server" Text="Orden Venta" OnClick="btnOrdenVenta_Click" />
            </div>
        </div>

        <!-- ORDEN COMPRA -->
        <div id="ordenCompraContent" class="orden-content active" runat="server">
            <div class="form-card">
                <div class="form-grid-compact">
                    <div class="form-group">
                        <label>Fecha de Entrada</label>
                        <asp:TextBox ID="txtFechaIngreso" runat="server" Text="09-10-2025"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Código de Entrada</label>
                        <asp:TextBox ID="txtCodigoIngreso" runat="server" ReadOnly="true" Text="INW-001"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Descripción</label>
                        <asp:TextBox ID="txtDescripcionCompra" runat="server" TextMode="MultiLine" Rows="1"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Proveedor</label>
                        <asp:DropDownList ID="ddlProveedor" runat="server">
                            <asp:ListItem Text="194 - BHAVANI SALES CORPORATION" Value="194"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Número de Orden de Compra</label>
                        <asp:TextBox ID="txtNumeroOrdenCompra" runat="server" placeholder="Número de orden"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Fecha de Orden de Compra</label>
                        <asp:TextBox ID="txtFechaOrdenCompra" runat="server" Text="08-10-2025"></asp:TextBox>
                    </div>
                </div>

                <div class="file-actions">
                    <asp:Button ID="btnViewCompra" runat="server" Text="View" CssClass="btn-view" />
                    <asp:Button ID="btnAdjuntarOrdenCompra" runat="server" Text="Adjuntar Orden de Ingreso" CssClass="btn-guardar" />
                    <asp:TextBox ID="txtArchivoCompra" runat="server" placeholder="Archivo.pdf" ReadOnly="true" Text="Archivo .pdf"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Responsable</label>
                    <asp:TextBox ID="txtResponsableCompra" runat="server" ReadOnly="true" Text="Carlos Chipana Cruz"></asp:TextBox>
                </div>

                <div class="actions-bar">
                    <asp:Button ID="btnAgregarCompra" runat="server" Text="Agregar" CssClass="btn-agregar" OnClick="btnAgregarCompra_Click" />
                    <asp:Button ID="btnAnularCompra" runat="server" Text="Anular" CssClass="btn-anular" OnClick="btnAnularCompra_Click" />
                    <asp:Button ID="btnEditarCompra" runat="server" Text="Editar" CssClass="btn-editar" OnClick="btnEditarCompra_Click" />
                    <asp:Button ID="btnGuardarCompra" runat="server" Text="Guardar" CssClass="btn-guardar" OnClick="btnGuardarCompra_Click" />
                </div>
            </div>
        </div>

        <!-- ORDEN VENTA -->
        <div id="ordenVentaContent" class="orden-content" runat="server">
            <div class="form-card">
                <div class="form-grid-compact">
                    <div class="form-group">
                        <label>Fecha de Salida</label>
                        <asp:TextBox ID="txtFechaSalida" runat="server" Text="08-10-2025"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Código de Salida</label>
                        <asp:TextBox ID="txtCodigoSalida" runat="server" ReadOnly="true" Text="INW-001"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Descripción</label>
                        <asp:TextBox ID="txtDescripcionVenta" runat="server" TextMode="MultiLine" Rows="1"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Cliente</label>
                        <asp:DropDownList ID="ddlCliente" runat="server">
                            <asp:ListItem Text="194 - BHAVANI SALES CORPORATION" Value="194"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Número de Orden de Venta</label>
                        <asp:TextBox ID="txtNumeroOrdenVenta" runat="server" placeholder="Número de orden de venta"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Fecha de Orden Venta</label>
                        <asp:TextBox ID="txtFechaOrdenVenta" runat="server" Text="08-10-2025"></asp:TextBox>
                    </div>
                </div>

                <div class="file-actions">
                    <asp:Button ID="btnViewVenta" runat="server" Text="View" CssClass="btn-view" />
                    <asp:Button ID="btnAdjuntarOrdenVenta" runat="server" Text="Adjuntar Orden de Salida" CssClass="btn-guardar" />
                    <asp:TextBox ID="txtArchivoVenta" runat="server" placeholder="Archivo.pdf" ReadOnly="true" Text="Archivo .pdf"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Responsable</label>
                    <asp:TextBox ID="txtResponsableVenta" runat="server" ReadOnly="true" Text="Carlos Chipana Cruz"></asp:TextBox>
                </div>

                <div class="actions-bar">
                    <asp:Button ID="btnAgregarVenta" runat="server" Text="Agregar" CssClass="btn-agregar" OnClick="btnAgregarVenta_Click" />
                    <asp:Button ID="btnEliminarVenta" runat="server" Text="Eliminar" CssClass="btn-eliminar" OnClick="btnEliminarVenta_Click" />
                    <asp:Button ID="btnEditarVenta" runat="server" Text="Editar" CssClass="btn-editar" OnClick="btnEditarVenta_Click" />
                    <asp:Button ID="btnGuardarVenta" runat="server" Text="Guardar" CssClass="btn-guardar" OnClick="btnGuardarVenta_Click" />
                </div>
            </div>
        </div>

        <!-- Tabla: Ordenes de Ingreso/Salida -->
        <div class="table-section">
            <h3>
                <asp:Label ID="lblTituloTabla" runat="server" Text="Ordenes de Ingreso"></asp:Label>
            </h3>
            <div class="table-responsive">
                <asp:GridView ID="gvProductos" runat="server" CssClass="data-table" AutoGenerateColumns="false"
                    OnRowDataBound="gvProductos_RowDataBound" OnSelectedIndexChanged="gvProductos_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderStyle-CssClass="checkbox-column" ItemStyle-CssClass="checkbox-column">
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelectAll" runat="server" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSeleccion" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Código" />
                        <asp:BoundField DataField="FechaRegistrada" HeaderText="Fecha Registrada" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre Proveedor/Cliente" />
                        <asp:BoundField DataField="Responsable" HeaderText="Responsable" />
                        <asp:BoundField DataField="Total" HeaderText="Total" />
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <asp:Label ID="lblEstado" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado") != null ? Eval("Estado").ToString() : "") %>' 
                                    Text='<%# Eval("Estado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- Tabla: Líneas de Orden de Ingreso/Salida -->
        <div class="table-section">
            <h3>
                <asp:Label ID="lblTituloDetalle" runat="server" Text="Líneas de Orden de Ingreso"></asp:Label>
            </h3>
            <div class="table-responsive">
                <asp:GridView ID="gvDetalleProducto" runat="server" CssClass="data-table" AutoGenerateColumns="false"
                    OnRowDataBound="gvDetalleProducto_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderStyle-CssClass="checkbox-column" ItemStyle-CssClass="checkbox-column">
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelectAllDetalle" runat="server" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkEliminar" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Código" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                        <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                        <asp:BoundField DataField="Marca" HeaderText="Marca" />
                        <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unitario" />
                        <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                        <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                        <asp:BoundField DataField="SubTotal" HeaderText="subTotal" />
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <asp:Label ID="lblEstadoDetalle" runat="server" CssClass='<%# GetBadgeClass(Eval("Estado") != null ? Eval("Estado").ToString() : "") %>' 
                                    Text='<%# Eval("Estado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <tr>
                            <td colspan="10" style="text-align: center; padding: 2rem; color: var(--txt-muted);">
                                No hay datos para mostrar
                            </td>
                        </tr>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

</asp:Content>