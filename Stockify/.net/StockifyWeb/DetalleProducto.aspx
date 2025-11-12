<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" 
    CodeBehind="DetalleProducto.aspx.cs" Inherits="StockifyWeb.DetalleProducto" Async="true" %>

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
            --radius: 16px;
            --shadow: 0 10px 24px rgba(0,0,0,.35);
        }

        .detalle-container {
            background: #000000;
            padding: 32px;
            border-radius: var(--radius);
            color: var(--text);
            min-height: calc(100vh - 100px);
        }

        .detalle-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
            padding-bottom: 20px;
            border-bottom: 2px solid var(--stroke);
        }

        .detalle-title {
            color: #ffffff;
            font-size: 28px;
            font-weight: 700;
            margin: 0;
        }

        .detalle-actions {
            display: flex;
            gap: 12px;
            align-items: center;
        }

        .btn-back {
            background: var(--card2);
            color: var(--text);
            border: 1px solid var(--stroke);
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 8px;
            font-weight: 600;
            transition: all 0.3s;
            text-decoration: none;
        }

        .btn-edit {
            background: var(--accent);
            color: #000000;
            border: none;
            padding: 10px 24px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            transition: all 0.3s;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .btn-edit:disabled {
            background: var(--card2);
            color: var(--muted);
            cursor: not-allowed;
            opacity: 0.5;
        }

        .btn-back:hover {
            background: var(--stroke);
            transform: translateX(-2px);
        }

        .btn-edit:hover:not(:disabled) {
            background: #9ab1ff;
            transform: translateY(-1px);
        }

        .section-title {
            color: #ffffff;
            font-size: 18px;
            font-weight: 700;
            margin-bottom: 20px;
            margin-top: 30px;
            padding-bottom: 12px;
            border-bottom: 2px solid #323844;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .detalle-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 20px;
            margin-bottom: 30px;
        }

        .detalle-field {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .detalle-field.full-width {
            grid-column: 1 / -1;
        }

        .detalle-label {
            color: #8a95aa;
            font-size: 13px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .detalle-value {
            color: #ffffff;
            font-size: 16px;
            padding: 14px 16px;
            background: #0a0a0a;
            border-radius: 8px;
            border: 1px solid #2a2f39;
            min-height: 24px;
        }

        .stock-section {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
            margin-top: 20px;
        }

        .stock-card {
            background: #0a0a0a;
            padding: 24px 16px;
            border-radius: 12px;
            text-align: center;
            border: 1px solid #2a2f39;
            transition: all 0.3s;
        }

        .stock-card:hover {
            border-color: var(--accent);
            transform: translateY(-2px);
        }

        .stock-label {
            color: #8a95aa;
            font-size: 12px;
            margin-bottom: 12px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .stock-value {
            color: #ffffff;
            font-size: 32px;
            font-weight: 700;
        }

        .info-card {
            background: #0a0a0a;
            padding: 20px;
            border-radius: 12px;
            border: 1px solid #2a2f39;
            margin-top: 20px;
        }
        
        /* Estilos para el modal */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.85);
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

        /* Campos en línea para Stock */
        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
        }
        
        .modal-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 24px;
            border-top: 1px solid var(--stroke);
            padding-top: 20px;
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
        
        .btn-discard:hover {
            background: var(--stroke);
            transform: translateY(-1px);
        }
        
        .btn-submit:hover {
            background: #9ab1ff;
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(138, 162, 255, 0.4);
        }

        @media (max-width: 768px) {
            .detalle-container {
                padding: 20px;
            }

            .detalle-header {
                flex-direction: column;
                gap: 16px;
                align-items: flex-start;
            }

            .detalle-grid, .stock-section {
                grid-template-columns: 1fr;
            }

            .detalle-title {
                font-size: 22px;
            }

            .form-row {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_Contenido" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <div class="detalle-container">
        <div class="detalle-header">
            <h1 class="detalle-title">
                <asp:Literal ID="litNombreProducto" runat="server" Text="Detalle del Producto" />
            </h1>
            <div class="detalle-actions">
                <a href="Inventario.aspx" class="btn-back">
                    <i class="fas fa-arrow-left"></i> Volver
                </a>
                <button type="button" class="btn-edit" id="btnEditar" onclick="abrirModalEditar()" disabled>
                    <i class="fas fa-edit"></i> Editar
                </button>
            </div>
        </div>

        <h3 class="section-title">📋 Información del Producto</h3>
        
        <div class="detalle-grid">
            <div class="detalle-field">
                <span class="detalle-label">ID Producto</span>
                <div class="detalle-value">
                    <asp:Literal ID="litIdProducto" runat="server" Text="---" />
                </div>
            </div>
            
            <div class="detalle-field">
                <span class="detalle-label">Nombre del Producto</span>
                <div class="detalle-value">
                    <asp:Literal ID="litNombre" runat="server" Text="---" />
                </div>
            </div>
            
            <div class="detalle-field">
                <span class="detalle-label">Categoría</span>
                <div class="detalle-value">
                    <asp:Literal ID="litCategoria" runat="server" Text="---" />
                </div>
            </div>
            
            <div class="detalle-field">
                <span class="detalle-label">Marca</span>
                <div class="detalle-value">
                    <asp:Literal ID="litMarca" runat="server" Text="---" />
                </div>
            </div>
            
            <div class="detalle-field">
                <span class="detalle-label">Precio Unitario</span>
                <div class="detalle-value">
                    <asp:Literal ID="litPrecio" runat="server" Text="---" />
                </div>
            </div>
            
            <div class="detalle-field full-width">
                <span class="detalle-label">Descripción</span>
                <div class="detalle-value">
                    <asp:Literal ID="litDescripcion" runat="server" Text="---" />
                </div>
            </div>
        </div>
        
        <h3 class="section-title">📦 Información de Stock</h3>
        
        <div class="stock-section">
            <div class="stock-card">
                <div class="stock-label">Stock Máximo</div>
                <div class="stock-value">
                    <asp:Literal ID="litStockMax" runat="server" Text="0" />
                </div>
            </div>
            <div class="stock-card">
                <div class="stock-label">Stock Actual</div>
                <div class="stock-value">
                    <asp:Literal ID="litStockActual" runat="server" Text="0" />
                </div>
            </div>
            <div class="stock-card">
                <div class="stock-label">Stock Mínimo</div>
                <div class="stock-value">
                    <asp:Literal ID="litStockMin" runat="server" Text="0" />
                </div>
            </div>
        </div>
    </div>
    
    <!-- Modal Editar Producto -->
    <div class="modal-overlay" id="editarProductoModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">✏️ Editar Producto</h2>
                <button class="close-modal" type="button" onclick="cerrarModalEditar()">&times;</button>
            </div>
        
            <div class="form-group">
                <label for="txtNombreEditar">📝 Nombre del Producto</label>
                <asp:TextBox ID="txtNombreEditar" runat="server" CssClass="form-control" 
                    ClientIDMode="Static" placeholder="Ingrese el nombre"></asp:TextBox>
            </div>
        
            <div class="form-group">
                <label for="ddlCategoriaEditar">📂 Categoría</label>
                <asp:DropDownList ID="ddlCategoriaEditar" runat="server" CssClass="form-control" 
                    DataTextField="nombre" DataValueField="idCategoria" ClientIDMode="Static">
                </asp:DropDownList>
            </div>
            
            <div class="form-group">
                <label for="txtMarcaEditar">🏷️ Marca</label>
                <asp:TextBox ID="txtMarcaEditar" runat="server" CssClass="form-control" 
                    ClientIDMode="Static" placeholder="Marca del producto"></asp:TextBox>
            </div>

            <div class="form-group">
                <label for="txtPrecioEditar">💰 Precio Unitario (S/)</label>
                <asp:TextBox ID="txtPrecioEditar" runat="server" CssClass="form-control" 
                    TextMode="Number" step="0.01" ClientIDMode="Static" placeholder="0.00"></asp:TextBox>
            </div>
        
            <div class="form-row">
                <div class="form-group">
                    <label for="txtStockMinEditar">📉 Stock Mínimo</label>
                    <asp:TextBox ID="txtStockMinEditar" runat="server" CssClass="form-control" 
                        TextMode="Number" ClientIDMode="Static" placeholder="0" min="0"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label for="txtStockMaxEditar">📈 Stock Máximo</label>
                    <asp:TextBox ID="txtStockMaxEditar" runat="server" CssClass="form-control" 
                        TextMode="Number" ClientIDMode="Static" placeholder="0" min="0"></asp:TextBox>
                </div>
            </div>

            <div class="form-group">
                <label for="txtDescripcionEditar">📄 Descripción</label>
                <asp:TextBox ID="txtDescripcionEditar" runat="server" CssClass="form-control" 
                    TextMode="MultiLine" Rows="3" ClientIDMode="Static" 
                    placeholder="Descripción del producto..."></asp:TextBox>
            </div>
        
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModalEditar()">
                    ✖️ Cancelar
                </button>
                <asp:Button ID="btnGuardarCambios" runat="server" Text="✅ Guardar Cambios" 
                    CssClass="btn-submit" OnClick="BtnGuardarCambios_Click" />
            </div>
        </div>
    </div>
    
    <!-- HiddenFields para almacenar datos del producto -->
    <asp:HiddenField ID="hdnNombre" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnCategoria" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnCategoriaId" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnMarca" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnPrecio" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnDescripcion" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnStockMin" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnStockMax" runat="server" ClientIDMode="Static" />
    
    <script>
        function habilitarBotonEditar() {
            var btn = document.getElementById('btnEditar');
            if (btn) {
                btn.disabled = false;
            }
        }

        function abrirModalEditar() {
            try {
                // Obtener valores de los HiddenFields
                var nombre = document.getElementById('hdnNombre').value;
                var categoriaId = document.getElementById('hdnCategoriaId').value;
                var marca = document.getElementById('hdnMarca').value;
                var precio = document.getElementById('hdnPrecio').value;
                var descripcion = document.getElementById('hdnDescripcion').value;
                var stockMin = document.getElementById('hdnStockMin').value;
                var stockMax = document.getElementById('hdnStockMax').value;

                // Validación
                if (!nombre || nombre === '') {
                    alert("Los datos del producto no están listos. Por favor, espere un momento.");
                    return;
                }

                // Asignar valores a los campos del formulario
                document.getElementById('txtNombreEditar').value = nombre;
                document.getElementById('txtMarcaEditar').value = marca;
                document.getElementById('txtPrecioEditar').value = precio;
                document.getElementById('txtDescripcionEditar').value = descripcion;
                document.getElementById('txtStockMaxEditar').value = stockMax;
                document.getElementById('txtStockMinEditar').value = stockMin;

                // Seleccionar categoría en el DropDownList
                var ddlCategoria = document.getElementById('ddlCategoriaEditar');
                if (ddlCategoria && categoriaId) {
                    ddlCategoria.value = categoriaId;
                }

                // Mostrar modal
                document.getElementById('editarProductoModal').style.display = 'flex';
            }
            catch (e) {
                console.error("Error al abrir el modal:", e);
                alert("Error al intentar abrir el formulario de edición.");
            }
        }

        function cerrarModalEditar() {
            document.getElementById('editarProductoModal').style.display = 'none';
        }

        // Cerrar modal al hacer clic fuera
        document.getElementById('editarProductoModal').addEventListener('click', function (e) {
            if (e.target === this) {
                cerrarModalEditar();
            }
        });

        // Cerrar modal con tecla ESC
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                cerrarModalEditar();
            }
        });
    </script>
    
</asp:Content>