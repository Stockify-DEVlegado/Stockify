<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" CodeBehind="Inventario.aspx.cs" Inherits="StockifyWeb.Inventario" %>

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

        .products-container {
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
        
        /* ESTILOS ORIGINALES DE LA TABLA */
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            background: var(--card);
            border-radius: var(--radius);
            overflow: hidden;
            box-shadow: var(--shadow);
        }
        th, td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid var(--stroke);
            color: var(--text);
        }
        th {
            background: var(--card2);
            color: var(--text);
            font-weight: 600;
            border-bottom: 2px solid var(--stroke);
        }
        tbody tr {
            transition: background-color 0.3s;
        }
        tbody tr:hover {
            background: var(--card2);
        }
        
        /* COLORES ORIGINALES DE DISPONIBILIDAD */
        .in-stock { color: #68d391; font-weight: bold; }
        .out-of-stock { color: #fc8181; font-weight: bold; }
        .low-stock { color: #faf089; font-weight: bold; }
        .availability { color: #76e4f7; font-weight: bold; }
        
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
        
        /* Estilos para el modal */
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
        
        .image-upload {
            border: 2px dashed var(--stroke);
            border-radius: 8px;
            padding: 30px;
            text-align: center;
            cursor: pointer;
            transition: border-color 0.3s;
            margin-bottom: 15px;
        }
        
        .image-upload:hover {
            border-color: var(--accent);
        }
        
        .upload-icon {
            font-size: 40px;
            color: var(--muted);
            margin-bottom: 10px;
        }
        
        .upload-text {
            color: var(--muted);
            font-size: 14px;
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

        /* Estilos para la GridView */
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
            color: var(--text);
            font-weight: 600;
            border-bottom: 2px solid var(--stroke);
        }
        
        .products-table tr:hover {
            background: var(--card2);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_Contenido" runat="server">
    <div class="products-container">
        <!-- Header con buscador y botones -->
        <div class="header-actions">
            <div class="search-box">
                <input type="text" placeholder="Buscar productos..." id="txtBuscar">
                <i class="fas fa-search"></i>
            </div>
            <div class="action-buttons">
                <button class="btn-filter" type="button">
                    <i class="fas fa-filter"></i> Filters
                </button>
                <asp:Button ID="btnOpenModal" runat="server" Text="Add Product" 
                    CssClass="btn-add" OnClientClick="abrirModal(); return false;" />
            </div>
        </div>

        <h1>Productos</h1>
        
        <asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="false" CssClass="products-table"
            Width="100%" BorderStyle="None" GridLines="None" ShowHeader="true">
            <Columns>
                <asp:BoundField DataField="Producto" HeaderText="Producto" />
                <asp:BoundField DataField="Precio" HeaderText="Precio Unitario" DataFormatString="₹{0}" />
                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                <asp:BoundField DataField="Marca" HeaderText="Marca" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:TemplateField HeaderText="Disponibilidad">
                    <ItemTemplate>
                        <span class='<%# GetClaseDisponibilidad(Eval("Disponibilidad").ToString()) %>'>
                            <%# Eval("Disponibilidad") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        
        <!-- Paginación reorganizada -->
        <div class="pagination">
            <div class="pagination-left">
                <button class="btn-pagination button-disabled">Anterior</button>
            </div>
            <div class="pagination-center">
                <span>Página 1 de 10</span>
            </div>
            <div class="pagination-right">
                <button class="btn-pagination">Siguiente</button>
            </div>
        </div>
    </div>

    <!-- Modal para Agregar Producto -->
    <div class="modal-overlay" id="addProductModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">Agregar Producto</h2>
                <button class="close-modal" type="button" onclick="cerrarModal()">&times;</button>
            </div>
            
            <div class="form-group">
                <label for="<%= fuProductImage.ClientID %>">Imagen del Producto</label>
                <div class="image-upload" onclick="document.getElementById('<%= fuProductImage.ClientID %>').click()">
                    <div class="upload-icon">
                        <i class="fas fa-cloud-upload-alt"></i>
                    </div>
                    <div class="upload-text">
                        Drag image here or<br>
                        <span style="color: var(--accent);">Browse image</span>
                    </div>
                </div>
                <asp:FileUpload ID="fuProductImage" runat="server" Style="display: none;" accept="image/*" />
            </div>
            
            <div class="form-group">
                <label for="<%= txtProductName.ClientID %>">Nombre del producto</label>
                <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control" placeholder="Ingrese nombre del producto"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label for="<%= txtProductID.ClientID %>">ID producto</label>
                <asp:TextBox ID="txtProductID" runat="server" CssClass="form-control" placeholder="Ingrese ID del producto"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label for="<%= ddlCategoria.ClientID %>">Categoría</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">Seleccione categoría</asp:ListItem>
                    <asp:ListItem Value="bebidas">Bebidas</asp:ListItem>
                    <asp:ListItem Value="alimentos">Alimentos</asp:ListItem>
                    <asp:ListItem Value="limpieza">Limpieza</asp:ListItem>
                    <asp:ListItem Value="electronicos">Electrónicos</asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="form-group">
                <label for="<%= txtPrecioUnitario.ClientID %>">Precio unitario</label>
                <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="form-control" 
                    placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label for="<%= txtDescripcion.ClientID %>">Descripción</label>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" 
                    TextMode="MultiLine" Rows="3" placeholder="Ingrese descripción del producto"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label for="<%= txtMarca.ClientID %>">Marca</label>
                <asp:TextBox ID="txtMarca" runat="server" CssClass="form-control" placeholder="Ingrese marca del producto"></asp:TextBox>
            </div>
            
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModal()">Discard</button>
                <asp:Button ID="btnAddProduct" runat="server" Text="Add Product" 
                    CssClass="btn-submit" OnClick="btnAddProduct_Click" />
            </div>
        </div>
    </div>

    <script>
        // Funcionalidad básica de búsqueda
        document.getElementById('txtBuscar').addEventListener('keyup', function () {
            var filter = this.value.toLowerCase();
            var rows = document.querySelectorAll('.products-table tbody tr');

            rows.forEach(function (row) {
                var productName = row.cells[0].textContent.toLowerCase();
                if (productName.indexOf(filter) > -1) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });

        // Funcionalidad del modal
        function abrirModal() {
            document.getElementById('addProductModal').style.display = 'flex';
        }

        function cerrarModal() {
            document.getElementById('addProductModal').style.display = 'none';
        }

        // Cerrar modal al hacer click fuera
        document.getElementById('addProductModal').addEventListener('click', function (e) {
            if (e.target === this) {
                cerrarModal();
            }
        });

        // Funcionalidad del botón siguiente
        document.querySelector('.pagination-right .btn-pagination').addEventListener('click', function () {
            alert('Navegando a la siguiente página...');
        });

        // Funcionalidad de filtros
        document.querySelector('.btn-filter').addEventListener('click', function () {
            alert('Funcionalidad de filtros próximamente...');
        });

        // Actualizar imagen seleccionada
        document.getElementById('<%= fuProductImage.ClientID %>').addEventListener('change', function (e) {
            if (e.target.files.length > 0) {
                const fileName = e.target.files[0].name;
                const imageUpload = document.querySelector('.image-upload');
                imageUpload.innerHTML = `
                    <div class="upload-icon">
                        <i class="fas fa-check" style="color: #68d391;"></i>
                    </div>
                    <div class="upload-text">
                        ${fileName}<br>
                        <span style="color: var(--accent);">Click para cambiar</span>
                    </div>
                `;
            }
        });
    </script>
</asp:Content>