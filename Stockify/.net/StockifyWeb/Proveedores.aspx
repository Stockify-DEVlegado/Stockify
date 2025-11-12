<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" 
CodeBehind="Proveedores.aspx.cs" Inherits="StockifyWeb.Proveedores" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --bg: #0b0c0f;
            --card: #21252d;
            --card2: #2a2f39;
            --stroke: #323844;
            --text: #e7eaf0;
            --muted: #a9b3c7;
            --accent: #8aa2ff;
            --accent2: #f0b75d;
            --radius: 16px;
            --shadow: 0 10px 24px rgba(0,0,0,.35);
        }

        .suppliers-container {
            background: var(--bg);
            padding: 22px 28px;
            border-radius: var(--radius);
            color: var(--text);
        }
        .header-actions {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            flex-wrap: wrap;
            gap: 16px;
        }
        .search-box {
            flex: 1;
            min-width: 300px;
            position: relative;
        }
        .search-box input {
            width: 100%;
            padding: 10px 40px 10px 15px;
            background: #121419;
            border: 1px solid var(--stroke);
            border-radius: 40px;
            font-size: 14px;
            color: var(--text);
        }
        .search-box input::placeholder {
            color: var(--muted);
        }
        .search-box input:focus {
            outline: none;
            border-color: var(--accent);
        }
        .search-box i {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            color: var(--muted);
        }
        .action-buttons {
            display: flex;
            gap: 10px;
        }
        .btn-filter {
            background: var(--card2);
            color: var(--text);
            border: 1px solid var(--stroke);
            padding: 10px 20px;
            border-radius: var(--radius);
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 5px;
            transition: all 0.3s;
        }
        .btn-add {
            background: var(--card);
            color: var(--accent);
            border: 1px solid var(--accent);
            padding: 10px 20px;
            border-radius: var(--radius);
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 5px;
            transition: all 0.3s;
        }
        .btn-filter:hover {
            background: var(--stroke);
            border-color: var(--muted);
        }
        .btn-add:hover {
            background: var(--accent);
            color: var(--bg);
        }
        
        .suppliers-table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            background: var(--card);
            border-radius: var(--radius);
            overflow: hidden;
            box-shadow: var(--shadow);
        }
        .suppliers-table th, .suppliers-table td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid var(--stroke);
            color: var(--text);
        }
        .suppliers-table th {
            background: var(--card2);
            color: var(--text);
            font-weight: 600;
            border-bottom: 2px solid var(--stroke);
        }
        .suppliers-table tbody tr {
            transition: background-color 0.3s;
        }
        .suppliers-table tbody tr:hover {
            background: var(--card2);
        }
        
        .status-active { 
            color: #68d391; 
            font-weight: bold; 
        }
        .status-inactive { 
            color: #fc8181; 
            font-weight: bold; 
        }
        
        .badge-empresa {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }
        
        .badge-proveedor {
            background: rgba(138, 162, 255, 0.2);
            color: #8aa2ff;
            border: 1px solid rgba(138, 162, 255, 0.3);
        }
        
        .badge-cliente {
            background: rgba(240, 183, 93, 0.2);
            color: #f0b75d;
            border: 1px solid rgba(240, 183, 93, 0.3);
        }
        
        .pagination {
            margin-top: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 16px 0;
        }
        .pagination-left {
            flex: 1;
            text-align: left;
        }
        .pagination-center {
            flex: 1;
            text-align: center;
            color: var(--muted);
        }
        .pagination-right {
            flex: 1;
            text-align: right;
        }
        .btn-pagination {
            background: var(--card2);
            color: var(--text);
            border: 1px solid var(--stroke);
            padding: 8px 16px;
            border-radius: var(--radius);
            cursor: pointer;
            transition: all 0.3s;
        }
        .btn-pagination:hover:not(.button-disabled) {
            background: var(--accent);
            color: var(--bg);
            border-color: var(--accent);
        }
        .button-disabled {
            background: var(--bg);
            color: var(--muted);
            cursor: not-allowed;
            border: 1px solid var(--stroke);
            opacity: 0.6;
        }
        
        h1 {
            color: var(--text);
            margin: 0 0 20px 0;
            font-size: 24px;
            font-weight: 600;
        }
        
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.8);
            z-index: 1000;
            justify-content: center;
            align-items: center;
        }
        
        .modal-content {
            background: var(--card);
            border: 1px solid var(--stroke);
            border-radius: var(--radius);
            padding: 24px;
            width: 90%;
            max-width: 500px;
            max-height: 90vh;
            overflow-y: auto;
            box-shadow: var(--shadow);
        }
        
        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            border-bottom: 1px solid var(--stroke);
            padding-bottom: 15px;
        }
        
        .modal-title {
            color: var(--text);
            font-size: 20px;
            font-weight: 600;
            margin: 0;
        }
        
        .close-modal {
            background: none;
            border: none;
            color: var(--muted);
            font-size: 24px;
            cursor: pointer;
            padding: 0;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .close-modal:hover {
            color: var(--text);
        }
        
        .form-group {
            margin-bottom: 15px;
        }
        
        .form-group label {
            display: block;
            color: var(--muted);
            margin-bottom: 5px;
            font-size: 14px;
            font-weight: 500;
        }
        
        .form-control {
            width: 100%;
            padding: 10px 12px;
            background: var(--bg);
            border: 1px solid var(--stroke);
            border-radius: 8px;
            color: var(--text);
            font-size: 14px;
            box-sizing: border-box;
        }
        
        .form-control:focus {
            outline: none;
            border-color: var(--accent);
        }
        
        .modal-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 20px;
            border-top: 1px solid var(--stroke);
            padding-top: 20px;
        }
        
        .btn-discard {
            background: var(--card2);
            color: var(--text);
            border: 1px solid var(--stroke);
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s;
        }
        
        .btn-submit {
            background: var(--accent);
            color: var(--bg);
            border: 1px solid var(--accent);
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 600;
            transition: all 0.3s;
            border: none;
        }
        
        .btn-discard:hover {
            background: var(--stroke);
        }
        
        .btn-submit:hover {
            background: #9ab1ff;
        }
        
        @media (max-width: 768px) {
            .header-actions {
                flex-direction: column;
                align-items: stretch;
            }
            .search-box {
                min-width: 100%;
            }
            .action-buttons {
                justify-content: space-between;
            }
            .pagination {
                flex-direction: column;
                gap: 10px;
            }
            .pagination-left, .pagination-center, .pagination-right {
                text-align: center;
                width: 100%;
            }
            
            .modal-content {
                padding: 16px;
                margin: 20px;
                width: calc(100% - 40px);
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_Contenido" runat="server">
    <div class="suppliers-container">
        <div class="header-actions">
            <div class="search-box">
                <input type="text" placeholder="Buscar empresas..." id="txtBuscar">
                <i class="fas fa-search"></i>
            </div>
            <div class="action-buttons">
                <button class="btn-filter" type="button">
                    <i class="fas fa-filter"></i> Filtros
                </button>
                <asp:Button ID="btnOpenModal" runat="server" Text="Agregar Empresa" 
                    CssClass="btn-add" OnClientClick="abrirModal(); return false;" />
            </div>
        </div>

        <h1>Empresas</h1>
        
        <asp:GridView ID="gvProveedores" runat="server" AutoGenerateColumns="false" CssClass="suppliers-table"
            Width="100%" BorderStyle="None" GridLines="None" ShowHeader="true">
            <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Razón Social" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:TemplateField HeaderText="Tipo Empresa">
                    <ItemTemplate>
                        <span class='<%# "badge-empresa badge-" + Eval("TipoEmpresa").ToString().ToLower() %>'>
                            <%# Eval("TipoEmpresa") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TipoDocumento" HeaderText="Tipo Doc." />
                <asp:TemplateField HeaderText="Activo">
                    <ItemTemplate>
                        <span class='<%# Eval("Activo").ToString() == "Si" ? "status-active" : "status-inactive" %>'>
                            <%# Eval("Activo") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        
        <div class="pagination">
            <div class="pagination-left">
                <button class="btn-pagination button-disabled">Anterior</button>
            </div>
            <div class="pagination-center">
                <span>Página 1 de 1</span>
            </div>
            <div class="pagination-right">
                <button class="btn-pagination button-disabled">Siguiente</button>
            </div>
        </div>
    </div>

    <div class="modal-overlay" id="addSupplierModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">Agregar Empresa</h2>
                <button class="close-modal" type="button" onclick="cerrarModal()">&times;</button>
            </div>
            
            <div class="form-group">
                <label for="<%= txtSupplierName.ClientID %>">Razón Social *</label>
                <asp:TextBox ID="txtSupplierName" runat="server" CssClass="form-control" placeholder="Ingrese razón social"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label for="<%= ddlTipoDocumento.ClientID %>">Tipo de Documento *</label>
                <asp:DropDownList ID="ddlTipoDocumento" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">Seleccione tipo de documento</asp:ListItem>
                    <asp:ListItem Value="DNI">DNI</asp:ListItem>
                    <asp:ListItem Value="RUC">RUC</asp:ListItem>
                    <asp:ListItem Value="CE">Carnet de Extranjería</asp:ListItem>
                    <asp:ListItem Value="PASAPORTE">Pasaporte</asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="form-group">
                <label for="<%= txtTelefono.ClientID %>">Teléfono *</label>
                <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" placeholder="Ingrese número de teléfono"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label for="<%= txtEmail.ClientID %>">Email *</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Ingrese email" TextMode="Email"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label for="<%= ddlTipoEmpresa.ClientID %>">Tipo de Empresa *</label>
                <asp:DropDownList ID="ddlTipoEmpresa" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">Seleccione tipo de empresa</asp:ListItem>
                    <asp:ListItem Value="PROVEEDOR">Proveedor</asp:ListItem>
                    <asp:ListItem Value="CLIENTE">Cliente</asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="form-group">
                <label for="<%= ddlActivo.ClientID %>">Estado</label>
                <asp:DropDownList ID="ddlActivo" runat="server" CssClass="form-control">
                    <asp:ListItem Value="si" Selected="True">Activo</asp:ListItem>
                    <asp:ListItem Value="no">Inactivo</asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModal()">Descartar</button>
                <asp:Button ID="btnAddSupplier" runat="server" Text="Agregar Empresa" 
                    CssClass="btn-submit" OnClick="btnAddSupplier_Click" />
            </div>
        </div>
    </div>

    <script>
        document.getElementById('txtBuscar').addEventListener('keyup', function () {
            var filter = this.value.toLowerCase();
            var rows = document.querySelectorAll('.suppliers-table tbody tr');

            rows.forEach(function (row) {
                var supplierName = row.cells[0].textContent.toLowerCase();
                if (supplierName.indexOf(filter) > -1) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });

        function abrirModal() {
            document.getElementById('addSupplierModal').style.display = 'flex';
        }

        function cerrarModal() {
            document.getElementById('addSupplierModal').style.display = 'none';
        }

        document.getElementById('addSupplierModal').addEventListener('click', function (e) {
            if (e.target === this) {
                cerrarModal();
            }
        });

        document.querySelector('.btn-filter').addEventListener('click', function () {
            alert('Funcionalidad de filtros próximamente...');
        });
    </script>
</asp:Content>