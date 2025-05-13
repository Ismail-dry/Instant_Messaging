<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Instant_Messaging.Login" %>

<!DOCTYPE html>
<html lang="tr">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Giriş / Kayıt</title>
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background: #141e30;  /* fallback */
            background: linear-gradient(to right, #243B55, #141E30);
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            color: white;
            margin: 0;
        }

        .container {
            background-color: #1f1f1f;
            padding: 40px;
            border-radius: 15px;
            width: 380px;
            box-shadow: 0 0 30px rgba(0,0,0,0.5);
        }u

        h2 {
            text-align: center;
            margin-bottom: 25px;
            color: #00d2ff;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: bold;
        }

        .form-group input, .form-group select {
            width: 100%;
            padding: 10px;
            border-radius: 6px;
            border: none;
            background: #333;
            color: white;
        }

        .form-group input:focus {
            outline: 2px solid #00d2ff;
        }

        .form-btn {
            width: 100%;
            background-color: #00d2ff;
            color: black;
            padding: 12px;
            border: none;
            border-radius: 8px;
            font-weight: bold;
            cursor: pointer;
            margin-top: 10px;
        }

        .form-btn:hover {
            background-color: #00aacc;
        }

        .toggle {
            text-align: center;
            margin-top: 20px;
            cursor: pointer;
            color: #aaa;
        }

        .toggle:hover {
            text-decoration: underline;
        }

        .hidden {
            display: none;
        }

        .error, .success {
            margin-top: 10px;
            text-align: center;
        }

        .error {
            color: red;
        }

        .success {
            color: green;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">

            <!-- Login Panel -->
            <div id="loginPanel" runat="server">
                <h2>Giriş Yap</h2>

                <asp:Label ID="lblError" runat="server" CssClass="error" Visible="false" />

                <div class="form-group">
                    <label for="txtUsername">Kullanıcı Adı</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label for="txtPassword">Şifre</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Giriş Yap" CssClass="form-btn" OnClick="btnLogin_Click" />

                <div class="toggle" onclick="togglePanels()">Hesabınız yok mu? Kayıt olun</div>
            </div>

            <!-- Register Panel -->
            <div id="registerPanel" class="hidden" runat="server">
                <h2>Kayıt Ol</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="success" />

            <div class="form-group">
        <label for="txtRegUsername">Kullanıcı Adı</label>
        <asp:TextBox ID="txtRegUsername" runat="server" CssClass="form-control" />
    </div>

    <div class="form-group">
        <label for="txtRegPassword">Şifre</label>
        <asp:TextBox ID="txtRegPassword" runat="server" TextMode="Password" CssClass="form-control" />
    </div>

    <div class="form-group">
        <label for="txtRegFullName">Tam Ad</label>
        <asp:TextBox ID="txtRegFullName" runat="server" CssClass="form-control" />
    </div>

    <div class="form-group">
        <label for="txtRegEmail">E-posta</label>
        <asp:TextBox ID="txtRegEmail" runat="server" TextMode="Email" CssClass="form-control" />
    </div>

    <div class="form-group">
        <label for="ddlRole">Rol Seçimi</label>
        <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
            <asp:ListItem Text="Hasta" Value="Hasta" />
            <asp:ListItem Text="Doktor" Value="Doktor" />
        </asp:DropDownList>
    </div>

                <asp:Button ID="btnRegister" runat="server" Text="Kayıt Ol" CssClass="form-btn" OnClick="btnRegister_Click" />

                <div class="toggle" onclick="togglePanels()">Zaten bir hesabınız var mı? Giriş yapın</div>
            </div>
        </div>
    </form>

    <script>
        function togglePanels() {
            const login = document.getElementById('<%= loginPanel.ClientID %>');
            const register = document.getElementById('<%= registerPanel.ClientID %>');
            login.classList.toggle("hidden");
            register.classList.toggle("hidden");
        }
    </script>
</body>
</html>
