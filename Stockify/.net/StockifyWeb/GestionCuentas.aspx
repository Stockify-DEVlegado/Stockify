<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" 
CodeBehind="GestionCuentas.aspx.cs" Inherits="StockifyWeb.GestionCuentas" Async="true" %>

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
        
        .badge-operario {
            background: rgba(138, 162, 255, 0.2);
            color: #8aa2ff;
            border: 1px solid rgba(138, 162, 255, 0.3);
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
    
    <div class="accounts-container">
        <div class="header-actions">
            <div class="search-box">
                <asp:TextBox ID="txtBuscar" runat="server" placeholder="Buscar cuentas..." CssClass="form-control" />
                <i class="fas fa-search"></i>
            </div>
            <div class="action-buttons">
                <asp:Button ID="btnCrearCuenta" runat="server" Text="Crear Cuenta" CssClass="btn-add" 
                    OnClientClick="abrirModalAgregar(); return false;" />
            </div>
        </div>

        <h1>Gestión de Cuentas</h1>
        
        <asp:Repeater ID="rptCuentas" runat="server">
            <HeaderTemplate>
                <table class="accounts-table">
                    <thead>
                        <tr>
                            <th>Usuario</th>
                            <th>Nombre Completo</th>
                            <th>Email</th>
                            <th>Rol</th>
                            <th>Teléfono</th>
                            <th>Activo</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Username") %></td>
                    <td><%# Eval("NombreCompleto") %></td>
                    <td><%# Eval("Email") %></td>
                    <td>
                        <span class='badge-rol <%# Eval("TipoUsuario").ToString() == "ADMINISTRADOR" ? "badge-admin" : "badge-operario" %>'>
                            <%# Eval("TipoUsuario") %>
                        </span>
                    </td>
                    <td><%# string.IsNullOrEmpty(Eval("Telefono")?.ToString()) ? "-" : Eval("Telefono") %></td>
                    <td>
                        <span class='<%# (bool)Eval("Activo") ? "status-active" : "status-inactive" %>'>
                            <%# (bool)Eval("Activo") ? "Sí" : "No" %>
                        </span>
                    </td>
                    <td>
                        <div class="action-buttons-cell">
                            <button type="button" class="btn-edit" 
                                onclick='editarCuenta(<%# Eval("IdUsuario") %>, "<%# Eval("Username") %>", "<%# Eval("Nombres") %>", "<%# Eval("Apellidos") %>", "<%# Eval("Email") %>", "<%# Eval("Telefono") %>", "<%# Eval("TipoUsuario") %>", <%# Eval("Activo").ToString().ToLower() %>)'>
                                Editar
                            </button>
                            <button type="button" class="btn-delete" 
                                onclick='confirmarEliminacion(<%# Eval("IdUsuario") %>, "<%# Eval("Username") %>")'>
                                Eliminar
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
            <i class="fas fa-users-slash"></i>
            <p>No hay cuentas registradas</p>
        </asp:Panel>
    </div>

    <!-- Modal Agregar/Editar -->
    <div class="modal-overlay" id="accountModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title" id="modalTitle">Crear Cuenta</h2>
                <button class="close-modal" type="button" onclick="cerrarModal()">&times;</button>
            </div>
            
            <asp:HiddenField ID="hfIdUsuario" runat="server" Value="0" />
            <asp:HiddenField ID="hfModoEdicion" runat="server" Value="false" />
            
            <div class="form-group">
                <label for="<%= txtUsername.ClientID %>">Nombre de Usuario *</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" 
                    placeholder="Ingrese nombre de usuario" />
            </div>
            
            <div class="form-group">
                <label for="<%= txtNombres.ClientID %>">Nombres *</label>
                <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" 
                    placeholder="Ingrese nombres" />
            </div>

            <div class="form-group">
                <label for="<%= txtApellidos.ClientID %>">Apellidos *</label>
                <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control" 
                    placeholder="Ingrese apellidos" />
            </div>
            
            <div class="form-group">
                <label for="<%= txtEmail.ClientID %>">Email *</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                    TextMode="Email" placeholder="Ingrese email" />
            </div>

            <div class="form-group">
                <label for="<%= txtTelefono.ClientID %>">Teléfono</label>
                <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" 
                    placeholder="Ingrese teléfono (opcional)" />
            </div>
            
            <div class="form-group" id="passwordGroup">
                <label for="<%= txtPassword.ClientID %>">Contraseña *</label>
                <div class="password-field">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" 
                        CssClass="form-control" placeholder="Mínimo 8 caracteres" />
                    <button type="button" class="toggle-password" onclick="togglePassword()">
                        <i class="fas fa-eye" id="eyeIcon"></i>
                    </button>
                </div>
                <small style="color: var(--muted); font-size: 12px;">
                    Mínimo 8 caracteres, debe incluir mayúsculas, minúsculas y números
                </small>
            </div>
            
            <div class="form-group">
                <label for="<%= ddlTipoUsuario.ClientID %>">Rol *</label>
                <asp:DropDownList ID="ddlTipoUsuario" runat="server" CssClass="form-control">
                    <asp:ListItem Value="" Text="Seleccione un rol" />
                    <asp:ListItem Value="ADMINISTRADOR" Text="Administrador" />
                    <asp:ListItem Value="OPERARIO" Text="Operario" />
                </asp:DropDownList>
            </div>
            
            <div class="form-group">
                <label for="<%= ddlActivo.ClientID %>">Estado</label>
                <asp:DropDownList ID="ddlActivo" runat="server" CssClass="form-control">
                    <asp:ListItem Value="true" Text="Activo" Selected="True" />
                    <asp:ListItem Value="false" Text="Inactivo" />
                </asp:DropDownList>
            </div>
            
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModal()">Descartar</button>
                <asp:Button ID="btnGuardarCuenta" runat="server" Text="Crear Cuenta" 
                    CssClass="btn-submit" OnClick="btnGuardarCuenta_Click" 
                    OnClientClick="return validarFormulario();" />
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfAccion" runat="server" />
    <asp:HiddenField ID="hfIdEliminar" runat="server" />

    <script>
        // Búsqueda en tiempo real
        var txtBuscar = document.getElementById('<%= txtBuscar.ClientID %>');
        if (txtBuscar) {
            txtBuscar.addEventListener('keyup', function () {
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
        }

        function abrirModalAgregar() {
            document.getElementById('modalTitle').innerText = 'Crear Cuenta';
            document.getElementById('<%= hfModoEdicion.ClientID %>').value = 'false';
            document.getElementById('<%= hfIdUsuario.ClientID %>').value = '0';
            document.getElementById('<%= btnGuardarCuenta.ClientID %>').value = 'Crear Cuenta';
            document.getElementById('passwordGroup').style.display = 'block';
            limpiarFormulario();
            document.getElementById('accountModal').style.display = 'flex';
        }

        function editarCuenta(id, usuario, nombres, apellidos, email, telefono, rol, activo) {
            document.getElementById('modalTitle').innerText = 'Editar Cuenta';
            document.getElementById('<%= hfModoEdicion.ClientID %>').value = 'true';
            document.getElementById('<%= hfIdUsuario.ClientID %>').value = id;
            document.getElementById('<%= btnGuardarCuenta.ClientID %>').value = 'Guardar Cambios';
            document.getElementById('passwordGroup').style.display = 'none';

            document.getElementById('<%= txtUsername.ClientID %>').value = usuario;
            document.getElementById('<%= txtUsername.ClientID %>').disabled = true;
            document.getElementById('<%= txtNombres.ClientID %>').value = nombres;
            document.getElementById('<%= txtApellidos.ClientID %>').value = apellidos;
            document.getElementById('<%= txtEmail.ClientID %>').value = email;
            document.getElementById('<%= txtTelefono.ClientID %>').value = telefono || '';
            document.getElementById('<%= ddlTipoUsuario.ClientID %>').value = rol;
            document.getElementById('<%= ddlActivo.ClientID %>').value = activo.toString();

            document.getElementById('accountModal').style.display = 'flex';
        }

        function cerrarModal() {
            document.getElementById('accountModal').style.display = 'none';
            limpiarFormulario();
            document.getElementById('<%= txtUsername.ClientID %>').disabled = false;
        }

        function limpiarFormulario() {
            document.getElementById('<%= txtUsername.ClientID %>').value = '';
            document.getElementById('<%= txtNombres.ClientID %>').value = '';
            document.getElementById('<%= txtApellidos.ClientID %>').value = '';
            document.getElementById('<%= txtEmail.ClientID %>').value = '';
            document.getElementById('<%= txtTelefono.ClientID %>').value = '';
            document.getElementById('<%= txtPassword.ClientID %>').value = '';
            document.getElementById('<%= ddlTipoUsuario.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= ddlActivo.ClientID %>').selectedIndex = 0;

            var eyeIcon = document.getElementById('eyeIcon');
            eyeIcon.classList.remove('fa-eye-slash');
            eyeIcon.classList.add('fa-eye');
            document.getElementById('<%= txtPassword.ClientID %>').type = 'password';
        }

        function validarFormulario() {
            var modoEdicion = document.getElementById('<%= hfModoEdicion.ClientID %>').value === 'true';
            var usuario = document.getElementById('<%= txtUsername.ClientID %>').value.trim();
            var nombres = document.getElementById('<%= txtNombres.ClientID %>').value.trim();
            var apellidos = document.getElementById('<%= txtApellidos.ClientID %>').value.trim();
            var email = document.getElementById('<%= txtEmail.ClientID %>').value.trim();
            var password = document.getElementById('<%= txtPassword.ClientID %>').value;
            var rol = document.getElementById('<%= ddlTipoUsuario.ClientID %>').value;

            if (!usuario || !nombres || !apellidos || !email || !rol) {
                alert('Por favor complete todos los campos obligatorios (*)');
                return false;
            }

            if (!modoEdicion && password.length < 8) {
                alert('La contraseña debe tener al menos 8 caracteres');
                return false;
            }

            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                alert('Por favor ingrese un email válido');
                return false;
            }

            return true;
        }

        function confirmarEliminacion(id, nombre) {
            if (confirm('¿Está seguro que desea eliminar la cuenta "' + nombre + '"?\n\nEsta acción eliminará el usuario tanto de la base de datos como de AWS Cognito.')) {
                document.getElementById('<%= hfIdEliminar.ClientID %>').value = id;
                <%= Page.ClientScript.GetPostBackEventReference(btnEliminar, "") %>;
            }
        }

        function togglePassword() {
            var passwordField = document.getElementById('<%= txtPassword.ClientID %>');
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

        document.getElementById('accountModal').addEventListener('click', function (e) {
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