<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Instant_Messaging.Register" %>

<!DOCTYPE html>

<html>
<head>
    <title>Kayıt Ol</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Kayıt Ol</h2>
        <asp:Label runat="server" Text="Kullanıcı Adı:" />
        <asp:TextBox ID="txtUsername" runat="server" /><br />

        <asp:Label runat="server" Text="Parola:" />
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" /><br />

        <asp:Label runat="server" Text="Tam Ad:" />
        <asp:TextBox ID="txtFullName" runat="server" /><br />

        <asp:Label runat="server" Text="E-posta:" />
        <asp:TextBox ID="txtEmail" runat="server" /><br />

        <asp:Label runat="server" Text="Rol Seçin:" />
        <asp:DropDownList ID="ddlRole" runat="server">
            <asp:ListItem Text="Doktor" Value="Doktor" />
            <asp:ListItem Text="Hasta" Value="Hasta" />
        </asp:DropDownList><br /><br />

        <asp:Button ID="btnRegister" runat="server" Text="Kayıt Ol" OnClick="btnRegister_Click" /><br />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
    </form>
</body>
</html>