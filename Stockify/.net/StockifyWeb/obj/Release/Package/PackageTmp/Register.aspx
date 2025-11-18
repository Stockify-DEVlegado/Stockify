<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="StockifyWeb.Register" UnobtrusiveValidationMode="None" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Register - Stockify</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
    <style>
        body {
            margin: 0;
            padding: 0;
            background-image: url("https://i.postimg.cc/pVKRj93f/image-2.png");
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 40px 20px;
        }

        .register-wrapper {
            display: flex;
            width: 100%;
            max-width: 1400px;
            gap: 60px;
            align-items: center;
            justify-content: center;
        }

        .register-card {
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

        .register-title {
            font-size: 32px;
            font-weight: 700;
            color: #1a1a1a;
            margin-bottom: 4px;
        }

        .register-subtitle {
            color: #1a1a1a;
            font-size: 18px;
            margin-bottom: 28px;
        }

        .form-group {
            margin-bottom: 18px;
        }

        .form-label {
            display: block;
            color: #1a1a1a;
            font-size: 15px;
            font-weight: 500;
            margin-bottom: 8px;
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

        .btn-register {
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
            margin-top: 24px;
            margin-bottom: 24px;
        }

        .btn-register:hover {
            background: #222;
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
        }

        .btn-register:active {
            transform: translateY(0);
        }

        .login-text {
            text-align: center;
            color: #666;
            font-size: 14px;
        }

        .login-text a {
            color: #000;
            font-weight: 600;
            text-decoration: none;
            margin-left: 4px;
        }

        .login-text a:hover {
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

        .error-message {
            color: red;
            font-size: 13px;
            margin-top: 4px;
        }

        @media (max-width: 992px) {
            .register-wrapper {
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
            body {
                padding: 20px 10px;
            }

            .register-card {
                padding: 32px 24px;
            }

            .register-title {
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
        <div class="register-wrapper">
            <!-- Card de Register -->
            <div class="register-card">
                <div class="welcome-text">Welcome !</div>
                <h1 class="register-title">Sign up to</h1>
                <div class="register-subtitle">Stockify</div>

                <div class="form-group">
                    <label class="form-label">Email</label>
                    <asp:TextBox ID="txtEmail" 
                                 runat="server" 
                                 CssClass="form-input"
                                 TextMode="Email" 
                                 placeholder="example@gmail.com" />
                    <asp:RequiredFieldValidator ID="rfvEmail" 
                                               runat="server" 
                                               ControlToValidate="txtEmail"
                                               ErrorMessage="Email es requerido" 
                                               CssClass="error-message"
                                               Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revEmail" 
                                                   runat="server" 
                                                   ControlToValidate="txtEmail"
                                                   ErrorMessage="Email inválido" 
                                                   CssClass="error-message"
                                                   ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"
                                                   Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label class="form-label">User name</label>
                    <asp:TextBox ID="txtUsername" 
                                 runat="server" 
                                 CssClass="form-input" 
                                 placeholder="username123" />
                    <asp:RequiredFieldValidator ID="rfvUsername" 
                                               runat="server" 
                                               ControlToValidate="txtUsername"
                                               ErrorMessage="Usuario es requerido" 
                                               CssClass="error-message"
                                               Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label class="form-label">Password</label>
                    <div class="password-wrapper">
                        <asp:TextBox ID="txtPassword" 
                                     runat="server" 
                                     TextMode="Password" 
                                     CssClass="form-input" 
                                     placeholder="Enter your password" />
                        <button type="button" class="password-toggle" onclick="togglePassword('txtPassword', 'eyeIcon1')">
                            <i class="fas fa-eye-slash" id="eyeIcon1"></i>
                        </button>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvPassword" 
                                               runat="server" 
                                               ControlToValidate="txtPassword"
                                               ErrorMessage="Contraseña es requerida" 
                                               CssClass="error-message"
                                               Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label class="form-label">Confirm Password</label>
                    <div class="password-wrapper">
                        <asp:TextBox ID="txtConfirmPassword" 
                                     runat="server" 
                                     TextMode="Password" 
                                     CssClass="form-input" 
                                     placeholder="Confirm your password" />
                        <button type="button" class="password-toggle" onclick="togglePassword('txtConfirmPassword', 'eyeIcon2')">
                            <i class="fas fa-eye-slash" id="eyeIcon2"></i>
                        </button>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvConfirmPassword" 
                                               runat="server" 
                                               ControlToValidate="txtConfirmPassword"
                                               ErrorMessage="Confirmar contraseña es requerido" 
                                               CssClass="error-message"
                                               Display="Dynamic" />
                    <asp:CompareValidator ID="cvPassword" 
                                         runat="server" 
                                         ControlToValidate="txtConfirmPassword"
                                         ControlToCompare="txtPassword"
                                         ErrorMessage="Las contraseñas no coinciden" 
                                         CssClass="error-message"
                                         Display="Dynamic" />
                </div>

                <asp:Button ID="btnRegister" 
                           runat="server" 
                           Text="Register" 
                           CssClass="btn-register" 
                           OnClick="btnRegister_Click" />

                <div class="login-text">
                    Already have an Account ? <a href="Login.aspx">Login</a>
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
        function togglePassword(inputId, iconId) {
            // Obtener el input por su ClientID generado por ASP.NET
            const inputs = document.querySelectorAll('input[type="password"], input[type="text"]');
            let passwordInput = null;
            
            for (let input of inputs) {
                if (input.id.includes(inputId)) {
                    passwordInput = input;
                    break;
                }
            }
            
            const eyeIcon = document.getElementById(iconId);
            
            if (passwordInput) {
                if (passwordInput.type === 'password') {
                    passwordInput.type = 'text';
                    eyeIcon.className = 'fas fa-eye';
                } else {
                    passwordInput.type = 'password';
                    eyeIcon.className = 'fas fa-eye-slash';
                }
            }
        }
    </script>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>