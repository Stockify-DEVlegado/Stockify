<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" 
CodeBehind="GestionCuentas.aspx.cs" Inherits="StockifyWeb.GestionCuentas" %>

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

        .accounts-container {
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
        
        .accounts-table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            background: var(--card);
            border-radius: var(--radius);
            overflow: hidden;
            box-shadow: var(--shadow);
        }
        .accounts-table th, .accounts-table td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid var(--stroke);
            color: var(--text);
        }
        .accounts-table th {
            background: var(--card2);
            color: var(--text);
            font-weight: 600;
            border-bottom: 2px solid var(--stroke);
        }
        .accounts-table tbody tr {
            transition: background-color 0.3s;
        }
        .accounts-table tbody tr:hover {
            background: var(--card2);
        }
        
        .status-active { 
            color: var(--success); 
            font-weight: bold; 
        }
        .status-inactive { 
            color: var(--danger); 
            font-weight: bold; 
        }
        
        .badge-rol {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }
        
        .badge-admin {
            background: rgba(255, 87, 87, 0.2);
            color: #ff5757;
            border: 1px solid rgba(255, 87, 87, 0.3);
        }
        
        .badge-usuario {
            background: rgba(138, 162, 255, 0.2);
            color: #8aa2ff;
            border: 1px solid rgba(138, 162, 255, 0.3);
        }

        .badge-supervisor {
            background: rgba(240, 183, 93, 0.2);
            color: #f0b75d;
            border: 1px solid rgba(240, 183, 93, 0.3);
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

        .password-field {
            position: relative;
        }

        .toggle-password {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
            background: none;
            border: none;
            color: var(--muted);
            cursor: pointer;
            padding: 0;
            font-size: 16px;
        }

        .toggle-password:hover {
            color: var(--text);
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
    
    <div class="accounts-container">
        <div class="header-actions">
            <div class="search-box">
                <input type="text" placeholder="Buscar cuentas..." id="txtBuscar">
                <i class="fas fa-search"></i>
            </div>
            <div class="action-buttons">
                <button class="btn-filter" type="button">
                    <i class="fas fa-filter"></i> Filtros
                </button>
                <button class="btn-add" type="button" onclick="abrirModalAgregar()">
                    <i class="fas fa-plus"></i> Crear Cuenta
                </button>
            </div>
        </div>

        <h1>Gestión de Cuentas</h1>
        
        <!-- Aquí va tu GridView con los datos de la base de datos -->
        <table class="accounts-table">
            <thead>
                <tr>
                    <th>Usuario</th>
                    <th>Nombre Completo</th>
                    <th>Email</th>
                    <th>Rol</th>
                    <th>Fecha Creación</th>
                    <th>Activo</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <!-- Ejemplo de fila - Reemplaza con tu GridView -->
                <tr>
                    <td>admin</td>
                    <td>Administrador Sistema</td>
                    <td>admin@stockify.com</td>
                    <td><span class="badge-rol badge-admin">ADMIN</span></td>
                    <td>15/01/2025</td>
                    <td><span class="status-active">Si</span></td>
                    <td>
                        <div class="action-buttons-cell">
                            <button type="button" class="btn-edit" onclick="editarCuenta(1, 'admin', 'Administrador Sistema', 'admin@stockify.com', 'ADMIN', 'si')">
                                Editar
                            </button>
                            <button type="button" class="btn-delete" onclick="confirmarEliminacion('admin')">
                                Eliminar
                            </button>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>usuario1</td>
                    <td>Juan Pérez</td>
                    <td>juan.perez@stockify.com</td>
                    <td><span class="badge-rol badge-usuario">USUARIO</span></td>
                    <td>20/01/2025</td>
                    <td><span class="status-active">Si</span></td>
                    <td>
                        <div class="action-buttons-cell">
                            <button type="button" class="btn-edit" onclick="editarCuenta(2, 'usuario1', 'Juan Pérez', 'juan.perez@stockify.com', 'USUARIO', 'si')">
                                Editar
                            </button>
                            <button type="button" class="btn-delete" onclick="confirmarEliminacion('usuario1')">
                                Eliminar
                            </button>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>supervisor1</td>
                    <td>María García</td>
                    <td>maria.garcia@stockify.com</td>
                    <td><span class="badge-rol badge-supervisor">SUPERVISOR</span></td>
                    <td>22/01/2025</td>
                    <td><span class="status-inactive">No</span></td>
                    <td>
                        <div class="action-buttons-cell">
                            <button type="button" class="btn-edit" onclick="editarCuenta(3, 'supervisor1', 'María García', 'maria.garcia@stockify.com', 'SUPERVISOR', 'no')">
                                Editar
                            </button>
                            <button type="button" class="btn-delete" onclick="confirmarEliminacion('supervisor1')">
                                Eliminar
                            </button>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        
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

    <!-- Modal Agregar/Editar -->
    <div class="modal-overlay" id="accountModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title" id="modalTitle">Crear Cuenta</h2>
                <button class="close-modal" type="button" onclick="cerrarModal()">&times;</button>
            </div>
            
            <input type="hidden" id="hfIdCuenta" value="0" />
            <input type="hidden" id="hfModoEdicion" value="false" />
            
            <div class="form-group">
                <label for="txtNombreUsuario">Nombre de Usuario *</label>
                <input type="text" id="txtNombreUsuario" class="form-control" placeholder="Ingrese nombre de usuario" />
            </div>
            
            <div class="form-group">
                <label for="txtNombreCompleto">Nombre Completo *</label>
                <input type="text" id="txtNombreCompleto" class="form-control" placeholder="Ingrese nombre completo" />
            </div>
            
            <div class="form-group">
                <label for="txtEmail">Email *</label>
                <input type="email" id="txtEmail" class="form-control" placeholder="Ingrese email" />
            </div>
            
            <div class="form-group" id="passwordGroup">
                <label for="txtPassword">Contraseña *</label>
                <div class="password-field">
                    <input type="password" id="txtPassword" class="form-control" placeholder="Ingrese contraseña" />
                    <button type="button" class="toggle-password" onclick="togglePassword()">
                        <i class="fas fa-eye" id="eyeIcon"></i>
                    </button>
                </div>
            </div>
            
            <div class="form-group">
                <label for="ddlRol">Rol *</label>
                <select id="ddlRol" class="form-control">
                    <option value="">Seleccione un rol</option>
                    <option value="ADMIN">Administrador</option>
                    <option value="SUPERVISOR">Supervisor</option>
                    <option value="USUARIO">Usuario</option>
                </select>
            </div>
            
            <div class="form-group">
                <label for="ddlActivo">Estado</label>
                <select id="ddlActivo" class="form-control">
                    <option value="si" selected>Activo</option>
                    <option value="no">Inactivo</option>
                </select>
            </div>
            
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModal()">Descartar</button>
                <button type="button" id="btnSubmit" class="btn-submit" onclick="guardarCuenta()">Crear Cuenta</button>
            </div>
        </div>
    </div>

    <script>
        // Búsqueda en tiempo real
        document.getElementById('txtBuscar').addEventListener('keyup', function () {
            var filter = this.value.toLowerCase();
            var rows = document.querySelectorAll('.accounts-table tbody tr');

            rows.forEach(function (row) {
                var userName = row.cells[0].textContent.toLowerCase();
                var fullName = row.cells[1].textContent.toLowerCase();
                var email = row.cells[2].textContent.toLowerCase();
                
                if (userName.indexOf(filter) > -1 || fullName.indexOf(filter) > -1 || email.indexOf(filter) > -1) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });

        // Abrir modal para agregar
        function abrirModalAgregar() {
            document.getElementById('modalTitle').innerText = 'Crear Cuenta';
            document.getElementById('hfModoEdicion').value = 'false';
            document.getElementById('hfIdCuenta').value = '0';
            document.getElementById('btnSubmit').innerText = 'Crear Cuenta';
            document.getElementById('passwordGroup').style.display = 'block';
            limpiarFormulario();
            document.getElementById('accountModal').style.display = 'flex';
        }

        // Abrir modal para editar
        function editarCuenta(id, usuario, nombreCompleto, email, rol, activo) {
            document.getElementById('modalTitle').innerText = 'Editar Cuenta';
            document.getElementById('hfModoEdicion').value = 'true';
            document.getElementById('hfIdCuenta').value = id;
            document.getElementById('btnSubmit').innerText = 'Guardar Cambios';
            document.getElementById('passwordGroup').style.display = 'none';
            
            document.getElementById('txtNombreUsuario').value = usuario;
            document.getElementById('txtNombreCompleto').value = nombreCompleto;
            document.getElementById('txtEmail').value = email;
            document.getElementById('ddlRol').value = rol;
            document.getElementById('ddlActivo').value = activo.toLowerCase();
            
            document.getElementById('accountModal').style.display = 'flex';
        }

        // Cerrar modal
        function cerrarModal() {
            document.getElementById('accountModal').style.display = 'none';
            limpiarFormulario();
        }

        // Limpiar formulario
        function limpiarFormulario() {
            document.getElementById('txtNombreUsuario').value = '';
            document.getElementById('txtNombreCompleto').value = '';
            document.getElementById('txtEmail').value = '';
            document.getElementById('txtPassword').value = '';
            document.getElementById('ddlRol').selectedIndex = 0;
            document.getElementById('ddlActivo').selectedIndex = 0;
            
            var eyeIcon = document.getElementById('eyeIcon');
            eyeIcon.classList.remove('fa-eye-slash');
            eyeIcon.classList.add('fa-eye');
            document.getElementById('txtPassword').type = 'password';
        }

        // Confirmar eliminación
        function confirmarEliminacion(nombre) {
            if (confirm('¿Está seguro que desea eliminar la cuenta "' + nombre + '"?\n\nEsta acción no se puede deshacer.')) {
                // Aquí llamarías a tu función del code-behind para eliminar
                console.log('Eliminar cuenta: ' + nombre);
            }
        }

        // Toggle mostrar/ocultar contraseña
        function togglePassword() {
            var passwordField = document.getElementById('txtPassword');
            var eyeIcon = document.getElementById('eyeIcon');
            
            if (passwordField.type === 'password') {
                passwordField.type = 'text';
                eyeIcon.classList.remove('fa-eye');
                eyeIcon.classList.add('fa-eye-slash');
            } else {
                passwordField.type = 'password';
                eyeIcon.classList.remove('fa-eye-slash');
                eyeIcon.classList.add('fa-eye');
            }
        }

        // Guardar cuenta (conecta con tu code-behind)
        function guardarCuenta() {
            var modoEdicion = document.getElementById('hfModoEdicion').value === 'true';
            var id = document.getElementById('hfIdCuenta').value;
            var usuario = document.getElementById('txtNombreUsuario').value;
            var nombreCompleto = document.getElementById('txtNombreCompleto').value;
            var email = document.getElementById('txtEmail').value;
            var password = document.getElementById('txtPassword').value;
            var rol = document.getElementById('ddlRol').value;
            var activo = document.getElementById('ddlActivo').value;

            // Validaciones básicas
            if (!usuario || !nombreCompleto || !email || !rol) {
                alert('Por favor complete todos los campos obligatorios (*)');
                return;
            }

            if (!modoEdicion && !password) {
                alert('La contraseña es obligatoria para crear una cuenta');
                return;
            }

            // Aquí llamarías a tu función del code-behind
            console.log('Guardar cuenta:', { id, usuario, nombreCompleto, email, rol, activo, modoEdicion });
            
            // Después de guardar exitosamente:
            // cerrarModal();
            // Recargar el GridView o la tabla
        }

        // Cerrar modal al hacer clic fuera
        document.getElementById('accountModal').addEventListener('click', function (e) {
            if (e.target === this) {
                cerrarModal();
            }
        });

        // Funcionalidad de filtros (placeholder)
        document.querySelector('.btn-filter').addEventListener('click', function () {
            alert('Funcionalidad de filtros próximamente...');
        });
    </script>
</asp:Content>