<%@ Page Title="Gestión de Categorías" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" 
CodeBehind="GestionCategorias.aspx.cs" Inherits="StockifyWeb.GestionCategorias" %>

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
            --danger: #ff5757;
            --success: #68d391;
            --radius: 16px;
            --shadow: 0 10px 24px rgba(0,0,0,.35);
        }

        .categories-container {
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

        .btn-add:hover {
            background: var(--accent);
            color: var(--bg);
        }

        .categories-table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            background: var(--card);
            border-radius: var(--radius);
            overflow: hidden;
            box-shadow: var(--shadow);
        }

        .categories-table th, .categories-table td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid var(--stroke);
            color: var(--text);
        }

        .categories-table th {
            background: var(--card2);
            color: var(--text);
            font-weight: 600;
            border-bottom: 2px solid var(--stroke);
        }

        .categories-table tbody tr {
            transition: background-color 0.3s;
        }

        .categories-table tbody tr:hover {
            background: var(--card2);
        }

        .badge-parent {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
            background: rgba(240, 183, 93, 0.2);
            color: var(--accent2);
            border: 1px solid rgba(240, 183, 93, 0.3);
        }

        .badge-subcategory {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
            background: rgba(138, 162, 255, 0.2);
            color: var(--accent);
            border: 1px solid rgba(138, 162, 255, 0.3);
        }

        .category-hierarchy {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .category-hierarchy i {
            color: var(--muted);
            font-size: 12px;
        }

        .action-buttons-cell {
            display: flex;
            gap: 8px;
            justify-content: center;
        }

        .btn-edit, .btn-delete {
            padding: 6px 12px;
            border-radius: 8px;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-size: 13px;
            transition: all 0.3s;
            border: none;
        }

        .btn-edit {
            background: rgba(138, 162, 255, 0.15);
            color: var(--accent);
            border: 1px solid rgba(138, 162, 255, 0.3);
        }

        .btn-edit:hover {
            background: var(--accent);
            color: var(--bg);
        }

        .btn-delete {
            background: rgba(255, 87, 87, 0.15);
            color: var(--danger);
            border: 1px solid rgba(255, 87, 87, 0.3);
        }

        .btn-delete:hover {
            background: var(--danger);
            color: white;
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

        .form-help {
            display: block;
            color: var(--muted);
            font-size: 12px;
            margin-top: 5px;
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

        .empty-state {
            text-align: center;
            padding: 40px 20px;
            color: var(--muted);
        }

        .empty-state i {
            font-size: 48px;
            margin-bottom: 16px;
            opacity: 0.5;
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
            .modal-content {
                padding: 16px;
                margin: 20px;
                width: calc(100% - 40px);
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_Contenido" runat="server">
    
    <div class="categories-container">
        <div class="header-actions">
            <div class="search-box">
                <asp:TextBox ID="txtBuscar" runat="server" placeholder="Buscar categorías..." CssClass="form-control" />
                <i class="fas fa-search"></i>
            </div>
            <div class="action-buttons">
                <asp:Button ID="btnCrearCategoria" runat="server" Text="Crear Categoría" CssClass="btn-add" 
                    OnClientClick="abrirModalAgregar(); return false;" />
            </div>
        </div>

        <h1>Gestión de Categorías</h1>
        
        <asp:Repeater ID="rptCategorias" runat="server">
            <HeaderTemplate>
                <table class="categories-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nombre</th>
                            <th>Categoría Padre</th>
                            <th>Tipo</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("IdCategoria") %></td>
                    <td>
                        <div class="category-hierarchy">
                            <%# !string.IsNullOrEmpty(Eval("NombrePadre")?.ToString()) ? "<i class='fas fa-level-up-alt fa-rotate-90'></i>" : "" %>
                            <strong><%# Eval("Nombre") %></strong>
                        </div>
                    </td>
                    <td>
                        <%# string.IsNullOrEmpty(Eval("NombrePadre")?.ToString()) ? 
                            "<span style='color: var(--muted);'>-</span>" : 
                            Eval("NombrePadre") %>
                    </td>
                    <td>
                        <span class='<%# string.IsNullOrEmpty(Eval("NombrePadre")?.ToString()) ? "badge-parent" : "badge-subcategory" %>'>
                            <%# string.IsNullOrEmpty(Eval("NombrePadre")?.ToString()) ? "Principal" : "Subcategoría" %>
                        </span>
                    </td>
                    <td>
                        <div class="action-buttons-cell">
                            <button type="button" class="btn-edit" 
                                onclick='editarCategoria(<%# Eval("IdCategoria") %>, "<%# Eval("Nombre") %>", <%# Eval("IdPadre") ?? "null" %>)'>
                                <i class="fas fa-edit"></i> Editar
                            </button>
                            <button type="button" class="btn-delete" 
                                onclick='confirmarEliminacion(<%# Eval("IdCategoria") %>, "<%# Eval("Nombre") %>")'>
                                <i class="fas fa-trash"></i> Eliminar
                            </button>
                        </div>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlEmpty" runat="server" CssClass="empty-state" Visible="false">
            <i class="fas fa-tags"></i>
            <p>No hay categorías registradas</p>
        </asp:Panel>
    </div>

    <!-- Modal Agregar/Editar -->
    <div class="modal-overlay" id="categoryModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title" id="modalTitle">Crear Categoría</h2>
                <button class="close-modal" type="button" onclick="cerrarModal()">&times;</button>
            </div>
            
            <asp:HiddenField ID="hfIdCategoria" runat="server" Value="0" />
            <asp:HiddenField ID="hfModoEdicion" runat="server" Value="false" />
            
            <div class="form-group">
                <label for="<%= txtNombre.ClientID %>">Nombre de la Categoría *</label>
                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" 
                    placeholder="Ej: Electrónica, Ropa, Alimentos..." />
                <small class="form-help">Ingrese un nombre descriptivo para la categoría</small>
            </div>
            
            <div class="form-group">
                <label for="<%= ddlCategoriaPadre.ClientID %>">Categoría Padre (Opcional)</label>
                <asp:DropDownList ID="ddlCategoriaPadre" runat="server" CssClass="form-control">
                    <asp:ListItem Value="" Text="-- Ninguna (Categoría Principal) --" />
                </asp:DropDownList>
                <small class="form-help">Seleccione una categoría padre si desea crear una subcategoría</small>
            </div>
            
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModal()">Cancelar</button>
                <asp:Button ID="btnGuardarCategoria" runat="server" Text="Crear Categoría" 
                    CssClass="btn-submit" OnClick="btnGuardarCategoria_Click" 
                    OnClientClick="return validarFormulario();" />
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfIdEliminar" runat="server" />

    <script>
        // Búsqueda en tiempo real
        var txtBuscar = document.getElementById('<%= txtBuscar.ClientID %>');
        if (txtBuscar) {
            txtBuscar.addEventListener('keyup', function () {
                var filter = this.value.toLowerCase();
                var rows = document.querySelectorAll('.categories-table tbody tr');

                rows.forEach(function (row) {
                    var nombre = row.cells[1].textContent.toLowerCase();
                    var padre = row.cells[2].textContent.toLowerCase();

                    if (nombre.indexOf(filter) > -1 || padre.indexOf(filter) > -1) {
                        row.style.display = '';
                    } else {
                        row.style.display = 'none';
                    }
                });
            });
        }

        function abrirModalAgregar() {
            document.getElementById('modalTitle').innerText = 'Crear Categoría';
            document.getElementById('<%= hfModoEdicion.ClientID %>').value = 'false';
            document.getElementById('<%= hfIdCategoria.ClientID %>').value = '0';
            document.getElementById('<%= btnGuardarCategoria.ClientID %>').value = 'Crear Categoría';
            limpiarFormulario();
            document.getElementById('categoryModal').style.display = 'flex';
        }

        function editarCategoria(id, nombre, idPadre) {
            document.getElementById('modalTitle').innerText = 'Editar Categoría';
            document.getElementById('<%= hfModoEdicion.ClientID %>').value = 'true';
            document.getElementById('<%= hfIdCategoria.ClientID %>').value = id;
            document.getElementById('<%= btnGuardarCategoria.ClientID %>').value = 'Guardar Cambios';

            document.getElementById('<%= txtNombre.ClientID %>').value = nombre;
            
            var ddlPadre = document.getElementById('<%= ddlCategoriaPadre.ClientID %>');
            if (idPadre && idPadre !== 'null') {
                ddlPadre.value = idPadre;
            } else {
                ddlPadre.selectedIndex = 0;
            }

            document.getElementById('categoryModal').style.display = 'flex';
        }

        function cerrarModal() {
            document.getElementById('categoryModal').style.display = 'none';
            limpiarFormulario();
        }

        function limpiarFormulario() {
            document.getElementById('<%= txtNombre.ClientID %>').value = '';
            document.getElementById('<%= ddlCategoriaPadre.ClientID %>').selectedIndex = 0;
        }

        function validarFormulario() {
            var nombre = document.getElementById('<%= txtNombre.ClientID %>').value.trim();

            if (!nombre) {
                alert('Por favor ingrese el nombre de la categoría');
                return false;
            }

            if (nombre.length < 2) {
                alert('El nombre debe tener al menos 2 caracteres');
                return false;
            }

            return true;
        }

        function confirmarEliminacion(id, nombre) {
            if (confirm('¿Está seguro que desea eliminar la categoría "' + nombre + '"?\n\nEsta acción no se puede deshacer.')) {
                document.getElementById('<%= hfIdEliminar.ClientID %>').value = id;
                <%= Page.ClientScript.GetPostBackEventReference(btnEliminar, "") %>;
            }
        }

        document.getElementById('categoryModal').addEventListener('click', function (e) {
            if (e.target === this) {
                cerrarModal();
            }
        });

        <% if (!string.IsNullOrEmpty(Request.QueryString["success"])) { %>
        alert('<%= Request.QueryString["success"] %>');
        window.history.replaceState({}, document.title, window.location.pathname);
        <% } %>

        <% if (!string.IsNullOrEmpty(Request.QueryString["error"])) { %>
        alert('<%= Request.QueryString["error"] %>');
        window.history.replaceState({}, document.title, window.location.pathname);
        <% } %>
    </script>

    <asp:Button ID="btnEliminar" runat="server" OnClick="btnEliminar_Click" 
        style="display:none;" />
</asp:Content>