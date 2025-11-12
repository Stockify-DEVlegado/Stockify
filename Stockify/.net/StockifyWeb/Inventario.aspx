<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" 
    CodeBehind="Inventario.aspx.cs" Inherits="StockifyWeb.Inventario" Async="true" %>

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
            --danger: #ff6b6b;
            --radius: 16px;
            --shadow: 0 10px 24px rgba(0,0,0,.35);
        }

        .products-container {
            background: var(--bg);
            padding: 22px 28px;
            border-radius: var(--radius);
            color: var(--text);
        }

        .products-container h1 {
            color: var(--text);
            margin: 0 0 20px 0;
            font-size: 24px;
            font-weight: 600;
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
            align-items: center;
        }

        .filter-group {
            position: relative;
        }

        .btn-filter-dropdown {
            padding: 10px 40px 10px 15px;
            background: var(--card);
            border: 1px solid var(--stroke);
            border-radius: 12px;
            color: var(--text);
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
            appearance: none;
            -webkit-appearance: none;
            -moz-appearance: none;
            min-width: 220px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
        }

        .btn-filter-dropdown:hover {
            background: var(--card2);
            border-color: var(--accent);
            box-shadow: 0 4px 12px rgba(138, 162, 255, 0.2);
        }

        .btn-filter-dropdown:focus {
            outline: none;
            border-color: var(--accent);
            box-shadow: 0 0 0 3px rgba(138, 162, 255, 0.15);
        }

        .filter-group::after {
            content: '▼';
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            color: var(--accent);
            pointer-events: none;
            font-size: 12px;
        }

        .btn-filter-dropdown option {
            background: var(--card);
            color: var(--text);
            padding: 10px;
            font-size: 14px;
        }

        .btn-filter-dropdown option:hover {
            background: var(--card2);
        }

        .btn-filter-dropdown option:first-child {
            color: var(--accent);
            font-weight: 600;
        }

        .filter-wrapper {
            position: relative;
            display: inline-block;
        }

        .filter-label {
            position: absolute;
            top: -8px;
            left: 12px;
            background: var(--bg);
            color: var(--muted);
            padding: 0 6px;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            z-index: 1;
        }

        .btn-filter, .btn-add {
            padding: 10px 20px;
            border-radius: 12px;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 8px;
            transition: all 0.3s ease;
            border: none;
            font-weight: 500;
            font-size: 14px;
        }

        .btn-filter {
            background: var(--card2);
            color: var(--text);
            border: 1px solid var(--stroke);
        }

        .btn-add {
            background: var(--accent);
            color: var(--bg);
            border: none;
            box-shadow: 0 4px 12px rgba(138, 162, 255, 0.3);
        }

        .btn-filter:hover {
            background: var(--stroke);
            transform: translateY(-1px);
        }

        .btn-add:hover {
            background: #9ab1ff;
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(138, 162, 255, 0.4);
        }

        .products-table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            background: var(--card);
            border-radius: var(--radius);
            overflow: hidden;
            box-shadow: var(--shadow);
        }

        .products-table th,
        .products-table td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid var(--stroke);
            color: var(--text);
        }

        .products-table th {
            background: var(--card2);
            font-weight: 600;
            cursor: pointer;
            user-select: none;
            position: relative;
        }

        .products-table th a {
            color: var(--text);
            text-decoration: none;
            display: block;
            padding: 0;
        }

        .products-table th a:hover {
            color: var(--accent);
        }

        .products-table tbody tr:hover {
            background: var(--card2);
        }

        /* Botones de acción en la tabla */
        .action-buttons-cell {
            display: flex;
            gap: 8px;
            align-items: center;
        }

        .btn-detalle {
            background: var(--accent);
            color: var(--bg);
            border: none;
            padding: 8px 16px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 13px;
            font-weight: 600;
            transition: all 0.3s;
        }

        .btn-detalle:hover {
            background: #9ab1ff;
            transform: translateY(-1px);
        }

        .btn-eliminar {
            background: var(--danger);
            color: #ffffff;
            border: none;
            padding: 8px 16px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 13px;
            font-weight: 600;
            transition: all 0.3s;
        }

        .btn-eliminar:hover {
            background: #ff5252;
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(255, 107, 107, 0.4);
        }

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
            background: var(--card);
            border: 1px solid var(--stroke);
            border-radius: var(--radius);
            padding: 28px;
            width: 90%;
            max-width: 600px;
            max-height: 90vh;
            overflow-y: auto;
            box-shadow: 0 20px 60px rgba(0,0,0,.6);
            animation: slideUp 0.3s ease;
        }

        /* Modal de confirmación de eliminación */
        .modal-content.confirm-delete {
            max-width: 450px;
            text-align: center;
        }

        .modal-content.confirm-delete .modal-icon {
            width: 80px;
            height: 80px;
            margin: 0 auto 20px;
            background: rgba(255, 107, 107, 0.2);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 40px;
            animation: pulse 2s infinite;
        }

        @keyframes pulse {
            0%, 100% { transform: scale(1); }
            50% { transform: scale(1.05); }
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
            border-bottom: 1px solid var(--stroke);
            padding-bottom: 15px;
        }

        .modal-title {
            color: var(--text);
            font-size: 22px;
            font-weight: 700;
            margin: 0;
        }

        .modal-subtitle {
            color: var(--muted);
            font-size: 15px;
            margin: 10px 0 20px;
            line-height: 1.6;
        }

        .product-name-highlight {
            color: var(--danger);
            font-weight: 700;
            font-size: 18px;
            margin: 15px 0;
            padding: 12px;
            background: rgba(255, 107, 107, 0.1);
            border-radius: 8px;
            border-left: 3px solid var(--danger);
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
            transition: all 0.2s;
        }

        .close-modal:hover {
            color: var(--text);
            transform: rotate(90deg);
        }

        .form-group {
            margin-bottom: 18px;
        }

        .form-group label {
            display: block;
            color: var(--muted);
            margin-bottom: 8px;
            font-size: 13px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .form-control {
            width: 100%;
            padding: 12px 14px;
            background: var(--bg);
            border: 1px solid var(--stroke);
            border-radius: 10px;
            color: var(--text);
            font-size: 14px;
            box-sizing: border-box;
            transition: all 0.3s ease;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--accent);
            box-shadow: 0 0 0 3px rgba(138, 162, 255, 0.15);
            background: var(--card);
        }

        .form-control option {
            background: var(--bg);
            color: var(--text);
            padding: 10px;
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
        }

        .modal-actions {
            display: flex;
            justify-content: center;
            gap: 12px;
            margin-top: 24px;
            padding-top: 20px;
        }

        .btn-submit {
            background: var(--accent);
            color: var(--bg);
            border: none;
            padding: 12px 24px;
            border-radius: 10px;
            cursor: pointer;
            font-weight: 600;
            font-size: 14px;
            transition: all 0.3s ease;
            box-shadow: 0 4px 12px rgba(138, 162, 255, 0.3);
        }

        .btn-discard {
            background: var(--card2);
            color: var(--text);
            border: 1px solid var(--stroke);
            padding: 12px 24px;
            border-radius: 10px;
            cursor: pointer;
            font-weight: 600;
            font-size: 14px;
            transition: all 0.3s ease;
        }

        .btn-delete-confirm {
            background: var(--danger);
            color: #ffffff;
            border: none;
            padding: 12px 32px;
            border-radius: 10px;
            cursor: pointer;
            font-weight: 600;
            font-size: 14px;
            transition: all 0.3s ease;
            box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3);
        }

        .btn-submit:hover {
            background: #9ab1ff;
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(138, 162, 255, 0.4);
        }

        .btn-discard:hover {
            background: var(--stroke);
            transform: translateY(-1px);
        }

        .btn-delete-confirm:hover {
            background: #ff5252;
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(255, 107, 107, 0.4);
        }

        @media (max-width: 768px) {
            .header-actions {
                flex-direction: column;
            }

            .btn-filter-dropdown {
                min-width: 100%;
            }

            .action-buttons {
                width: 100%;
                flex-direction: column;
            }

            .btn-add {
                width: 100%;
                justify-content: center;
            }

            .action-buttons-cell {
                flex-direction: column;
            }

            .modal-actions {
                flex-direction: column-reverse;
            }

            .btn-delete-confirm, .btn-discard {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_Contenido" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <div class="products-container">
        <div class="header-actions">
            <div class="search-box">
                <input type="text" placeholder="Buscar productos..." id="txtBuscar" />
                <i class="fas fa-search"></i>
            </div>
            <div class="action-buttons">
                <div class="filter-group">
                    <asp:DropDownList ID="ddlFiltroCategoria" runat="server" CssClass="btn-filter-dropdown" 
                        AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroCategoria_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnOpenModal" runat="server" Text="➕ Agregar Producto" 
                    CssClass="btn-add" OnClick="btnOpenModal_Click" />
            </div>
        </div>

        <h1>📦 Productos</h1>
        
        <asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="false" 
            CssClass="products-table" OnRowCommand="gvProductos_RowCommand" 
            AllowSorting="true" OnSorting="gvProductos_Sorting">
            <Columns>
                <asp:BoundField DataField="Producto" HeaderText="Producto" SortExpression="Producto" />
                <asp:BoundField DataField="Precio" HeaderText="Precio" DataFormatString="S/ {0:N2}" />
                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                <asp:BoundField DataField="Marca" HeaderText="Marca" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <div class="action-buttons-cell">
                            <asp:Button ID="btnVerDetalle" runat="server" Text="👁️ Ver" 
                                CssClass="btn-detalle" CommandName="VerDetalle" 
                                CommandArgument='<%# Eval("IdProducto") %>' />
                            <button type="button" class="btn-eliminar" 
                                onclick='abrirModalEliminar(<%# Eval("IdProducto") %>, "<%# Eval("Producto") %>")'>
                                🗑️ Eliminar
                            </button>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Modal Agregar/Editar Producto -->
    <div class="modal-overlay" id="addProductModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">
                    <asp:Literal ID="litModalTitle" runat="server" Text="✨ Agregar Producto" />
                </h2>
                <button class="close-modal" type="button" onclick="cerrarModal()">&times;</button>
            </div>
            
            <div class="form-group">
                <label>📝 Nombre del producto</label>
                <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control" 
                    placeholder="Ingrese nombre del producto"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>📂 Categoría</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-control" />
            </div>
            
            <div class="form-group">
                <label>💰 Precio unitario (S/)</label>
                <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="form-control" 
                    placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
            </div>
            
            <div class="form-row">
                <div class="form-group">
                    <label>📉 Stock mínimo</label>
                    <asp:TextBox ID="txtStockMinimo" runat="server" CssClass="form-control" 
                        placeholder="0" TextMode="Number" min="0"></asp:TextBox>
                </div>
                
                <div class="form-group">
                    <label>📈 Stock máximo</label>
                    <asp:TextBox ID="txtStockMaximo" runat="server" CssClass="form-control" 
                        placeholder="0" TextMode="Number" min="0"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-group">
                <label>📄 Descripción</label>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" 
                    TextMode="MultiLine" Rows="3" placeholder="Describe el producto..."></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>🏷️ Marca</label>
                <asp:TextBox ID="txtMarca" runat="server" CssClass="form-control" 
                    placeholder="Marca del producto"></asp:TextBox>
            </div>
            
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModal()">Cancelar</button>
                <asp:Button ID="btnSaveProduct" runat="server" Text="💾 Guardar Producto"
                    CssClass="btn-submit" OnClick="btnSaveProduct_Click" />
            </div>
        </div>
    </div>

    <!-- Modal Confirmar Eliminación -->
    <div class="modal-overlay" id="confirmDeleteModal">
        <div class="modal-content confirm-delete">
            <div class="modal-icon">⚠️</div>
            <h2 class="modal-title">¿Eliminar Producto?</h2>
            <p class="modal-subtitle">
                Esta acción es permanente y no se puede deshacer. El producto será eliminado completamente de la base de datos.
            </p>
            <div class="product-name-highlight" id="productNameToDelete"></div>
            
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModalEliminar()">
                    ✖️ Cancelar
                </button>
                <asp:Button ID="btnConfirmDelete" runat="server" Text="🗑️ Sí, Eliminar" 
                    CssClass="btn-delete-confirm" OnClick="btnConfirmDelete_Click" />
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnProductoId" runat="server" Value="0" />
    <asp:HiddenField ID="hdnProductoIdEliminar" runat="server" Value="0" ClientIDMode="Static" />
    
    <script>
        // Función para abrir modal de confirmación de eliminación
        function abrirModalEliminar(productoId, nombreProducto) {
            document.getElementById('hdnProductoIdEliminar').value = productoId;
            document.getElementById('productNameToDelete').textContent = nombreProducto;
            document.getElementById('confirmDeleteModal').style.display = 'flex';
        }

        function cerrarModalEliminar() {
            document.getElementById('confirmDeleteModal').style.display = 'none';
        }

        // Búsqueda en tiempo real
        document.addEventListener('DOMContentLoaded', function () {
            const txtBuscar = document.getElementById('txtBuscar');
            if (txtBuscar) {
                txtBuscar.addEventListener('keyup', function () {
                    var filtro = this.value.toLowerCase();
                    var tabla = document.querySelector('.products-table tbody');
                    if (!tabla) return;

                    var filas = tabla.getElementsByTagName('tr');

                    for (var i = 0; i < filas.length; i++) {
                        var fila = filas[i];
                        if (fila.cells.length > 0) {
                            var texto = '';
                            for (var j = 0; j < fila.cells.length; j++) {
                                texto += fila.cells[j].textContent.toLowerCase() + ' ';
                            }
                            fila.style.display = texto.includes(filtro) ? '' : 'none';
                        }
                    }
                });
            }
        });

        // Modales con animaciones
        function abrirModal() {
            const modal = document.getElementById('addProductModal');
            modal.style.display = 'flex';
            setTimeout(() => modal.classList.add('active'), 10);
        }

        function cerrarModal() {
            const modal = document.getElementById('addProductModal');
            modal.classList.remove('active');
            setTimeout(() => modal.style.display = 'none', 300);
        }

        // Cerrar al hacer clic fuera
        document.getElementById('addProductModal').addEventListener('click', function (e) {
            if (e.target === this) cerrarModal();
        });

        document.getElementById('confirmDeleteModal').addEventListener('click', function (e) {
            if (e.target === this) cerrarModalEliminar();
        });

        // Cerrar con tecla ESC
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                cerrarModal();
                cerrarModalEliminar();
            }
        });
    </script>
    
</asp:Content>