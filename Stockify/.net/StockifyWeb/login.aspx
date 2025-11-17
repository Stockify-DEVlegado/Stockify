<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="StockifyWeb.Login" Async="true" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Login - Stockify</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <style>
        body {
            margin: 0;
            padding: 0;
            background: #2c2f33;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .login-wrapper {
            display: flex;
            width: 100%;
            max-width: 1400px;
            padding: 20px;
            gap: 60px;
            align-items: center;
            justify-content: center;
        }

        .login-card {
            background: white;
            border-radius: 24px;
            padding: 48px 44px;
            width: 100%;
            max-width: 480px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
        }

        .welcome-text {
            color: #666;
            font-size: 16px;
            margin-bottom: 8px;
        }

        .login-title {
            font-size: 32px;
            font-weight: 700;
            color: #1a1a1a;
            margin-bottom: 4px;
        }

        .login-subtitle {
            color: #1a1a1a;
            font-size: 18px;
            margin-bottom: 36px;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            color: #1a1a1a;
            font-size: 15px;
            font-weight: 500;
            margin-bottom: 10px;
        }

        .form-input {
            width: 100%;
            padding: 14px 16px;
            border: 1.5px solid #d4d4d4;
            border-radius: 8px;
            font-size: 15px;
            transition: all 0.2s;
            box-sizing: border-box;
        }

        .form-input:focus {
            outline: none;
            border-color: #2196F3;
            box-shadow: 0 0 0 3px rgba(33, 150, 243, 0.1);
        }

        .password-wrapper {
            position: relative;
        }

        .password-toggle {
            position: absolute;
            right: 14px;
            top: 14px;
            background: none;
            border: none;
            cursor: pointer;
            color: #666;
            padding: 4px;
            font-size: 16px;
        }

        .password-toggle:hover {
            color: #333;
        }

        .form-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 28px;
        }

        .remember-wrapper {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .remember-wrapper input[type="checkbox"] {
            width: 18px;
            height: 18px;
            cursor: pointer;
        }

        .remember-wrapper label {
            color: #333;
            font-size: 14px;
            cursor: pointer;
            margin: 0;
        }

        .forgot-link {
            color: #888;
            font-size: 14px;
            text-decoration: none;
            transition: color 0.2s;
        }

        .forgot-link:hover {
            color: #2196F3;
        }

        .btn-login {
            width: 100%;
            padding: 15px;
            background: #000;
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s;
            margin-bottom: 28px;
        }

        .btn-login:hover {
            background: #222;
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
        }

        .btn-login:active {
            transform: translateY(0);
        }

        .register-text {
            text-align: center;
            color: #666;
            font-size: 14px;
        }

        .register-text a {
            color: #000;
            font-weight: 600;
            text-decoration: none;
            margin-left: 4px;
        }

        .register-text a:hover {
            text-decoration: underline;
        }

        .logo-section {
            flex: 1;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 40px;
        }

        .logo-section img {
            max-width: 100%;
            height: auto;
            filter: drop-shadow(0 10px 30px rgba(0, 0, 0, 0.3));
        }

        @media (max-width: 992px) {
            .login-wrapper {
                flex-direction: column;
                gap: 40px;
            }

            .logo-section {
                order: -1;
            }

            .logo-section img {
                max-width: 400px;
            }
        }

        @media (max-width: 576px) {
            .login-card {
                padding: 32px 24px;
            }

            .login-title {
                font-size: 28px;
            }

            .logo-section img {
                max-width: 300px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Hidden fields para comunicación con el servidor -->
        <asp:HiddenField ID="hfLoginExitoso" runat="server" />
        <asp:HiddenField ID="hfUsername" runat="server" />
        <asp:HiddenField ID="hfEmail" runat="server" />
        
        <div class="login-wrapper">
            <!-- Card de Login -->
            <div class="login-card">
                <div class="welcome-text">Welcome !</div>
                <h1 class="login-title">Sign in to</h1>
                <div class="login-subtitle">Stockify</div>

                <div class="form-group">
                    <label class="form-label">User name</label>
                    <asp:TextBox ID="txtUsername" 
                                 runat="server" 
                                 CssClass="form-input" 
                                 placeholder="Enter your username" />
                </div>

                <div class="form-group">
                    <label class="form-label">Password</label>
                    <div class="password-wrapper">
                        <asp:TextBox ID="txtPassword" 
                                     runat="server" 
                                     TextMode="Password" 
                                     CssClass="form-input" 
                                     placeholder="Enter your Password" />
                        <button type="button" class="password-toggle" onclick="togglePassword()">
                            <i class="fas fa-eye-slash" id="eyeIcon"></i>
                        </button>
                    </div>
                </div>

                <div class="form-footer">
                    <div class="remember-wrapper">
                        <asp:CheckBox ID="chkRemember" runat="server" />
                        <label for="<%= chkRemember.ClientID %>">Remember me</label>
                    </div>
                    <a href="#" class="forgot-link">Forgot Password ?</a>
                </div>

                <asp:Button ID="btnLogin" 
                           runat="server" 
                           Text="Login" 
                           CssClass="btn-login" 
                           OnClick="btnLogin_Click" />

                <div class="register-text">
                    Don't have an Account ? <a href="Register.aspx">Register</a>
                </div>
            </div>

            <!-- Sección del Logo -->
            <div class="logo-section">
                <img src="https://i.postimg.cc/wjpVBrLj/Group-48095761.png" 
                     alt="Stockify Logo" />
            </div>
        </div>
    </form>

    <script>
        function togglePassword() {
            const passwordInput = document.getElementById('<%= txtPassword.ClientID %>');
            const eyeIcon = document.getElementById('eyeIcon');

            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                eyeIcon.className = 'fas fa-eye';
            } else {
                passwordInput.type = 'password';
                eyeIcon.className = 'fas fa-eye-slash';
            }
        }
    </script>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>