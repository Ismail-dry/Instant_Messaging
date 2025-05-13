<%@ Page Title="Hasta Paneli" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PatientDashboard.aspx.cs" Inherits="Instant_Messaging.PatientDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .dashboard-box {
            padding: 20px;
            background-color: #f5f5f5;
            border-radius: 10px;
            font-family: Arial;
        }
        .dashboard-box ul {
            list-style: none;
            padding-left: 0;
        }
        .dashboard-box ul li {
            margin: 10px 0;
        }
        .dashboard-box ul li a {
            text-decoration: none;
            color: #007bff;
        }
    </style>

    <div class="dashboard-box">
        <h2>Hasta Paneline Hoş Geldiniz</h2>
        <asp:Label ID="lblPatientName" runat="server" Font-Bold="true" Font-Size="Large" /><br /><br />
        <ul>
            <li><a href="Chat.aspx">Doktorla Mesajlaş</a></li>
            <li><a href="Appointments.aspx">Randevularımı Gör</a></li>
        </ul>
    </div>
</asp:Content>