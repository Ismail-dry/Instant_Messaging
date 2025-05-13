<%@ Page Title="Dashboard" MasterPageFile="~/Site.master" Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Instant_Messaging.Dashboard" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .dashboard-container {
            padding: 20px;
        }
        .dashboard-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
        }
        .dashboard-panel {
            background-color: #f9f9f9;
            border-radius: 5px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .user-welcome {
            font-size: 24px;
            font-weight: bold;
        }
        .stats-container {
            display: flex;
            flex-wrap: wrap;
            margin: 0 -10px;
        }
        .stat-box {
            flex: 1;
            min-width: 200px
        flex: 1;
            min-width: 200px;
            background-color: white;
            border-radius: 5px;
            padding: 15px;
            margin: 10px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
            text-align: center;
        }
        .stat-number {
            font-size: 32px;
            font-weight: bold;
            color: #007bff;
            margin: 10px 0;
        }
        .stat-label {
            color: #666;
            font-size: 14px;
        }
        .recent-messages {
            margin-top: 20px;
        }
        .message-item {
            display: flex;
            justify-content: space-between;
            padding: 10px;
            border-bottom: 1px solid #eee;
        }
        .message-item:last-child {
            border-bottom: none;
        }
        .message-from {
            font-weight: bold;
        }
        .message-preview {
            color: #666;
            text-overflow: ellipsis;
            overflow: hidden;
            white-space: nowrap;
            max-width: 300px;
        }
        .message-time {
            color: #888;
            font-size: 12px;
        }
        .unread {
            background-color: #f0f7ff;
        }
        .action-buttons {
            margin-top: 20px;
        }
        .action-button {
            display: inline-block;
            padding: 10px 15px;
            background-color: #007bff;
            color: white;
            border-radius: 4px;
            text-decoration: none;
            margin-right: 10px;
        }
        .action-button:hover {
            background-color: #0069d9;
            text-decoration: none;
            color: white;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dashboard-container">
        <div class="dashboard-header">
            <div class="user-welcome">
                Merhaba, <asp:Literal ID="litUserName" runat="server"></asp:Literal>
                <div style="font-size: 16px; font-weight: normal; margin-top: 5px;">
                    <asp:Literal ID="litUserRole" runat="server"></asp:Literal>
                </div>
            </div>
            <div class="profile-actions">
                <asp:LinkButton ID="lnkEditProfile" runat="server" CssClass="btn btn-outline-primary" OnClick="lnkEditProfile_Click">
                    <i class="fa fa-user"></i> Profil Düzenle
                </asp:LinkButton>
            </div>
        </div>
        
        <div class="dashboard-panel">
            <h3>Genel Bakış</h3>
            <div class="stats-container">
                <div class="stat-box">
                    <div class="stat-number">
                        <asp:Literal ID="litTotalContacts" runat="server"></asp:Literal>
                    </div>
                    <div class="stat-label">Toplam Kişi</div>
                </div>
                <div class="stat-box">
                    <div class="stat-number">
                        <asp:Literal ID="litTotalMessages" runat="server"></asp:Literal>
                    </div>
                    <div class="stat-label">Toplam Mesaj</div>
                </div>
                <div class="stat-box">
                    <div class="stat-number">
                        <asp:Literal ID="litUnreadMessages" runat="server"></asp:Literal>
                    </div>
                    <div class="stat-label">Okunmamış Mesaj</div>
                </div>
                <div class="stat-box">
                    <div class="stat-number">
                        <asp:Literal ID="litLastActive" runat="server"></asp:Literal>
                    </div>
                    <div class="stat-label">Son Aktivite</div>
                </div>
            </div>
        </div>
        
        <div class="dashboard-panel">
            <h3>Son Mesajlaşmalar</h3>
            <div class="recent-messages">
                <asp:Repeater ID="rptRecentMessages" runat="server">
                    <ItemTemplate>
                        <div class="message-item <%# ((bool)Eval("IsUnread")) ? "unread" : "" %>">
                            <div>
                                <div class="message-from"><%# Eval("UserName") %></div>
                                <div class="message-preview"><%# Eval("MessagePreview") %></div>
                            </div>
                            <div class="message-time"><%# Eval("Timestamp") %></div
                        </div>
                    </ItemTemplate>
   
                </asp:Repeater>
                                 <asp:Panel ID="pnlNoMessages" runat="server" Visible="false">
    <div style="text-align: center; padding: 20px; color: #666;">
        Henüz mesajlaşma bulunmamaktadır.
    </div>
</asp:Panel>
            </div>
        </div>
        
        <div class="action-buttons">
            <a href="Chat.aspx" class="action-button">Mesajlaşmaya Git</a>
            
            <!-- Eğer kullanıcı doktor ise randevu yönetimi göster -->
            <asp:PlaceHolder ID="phDoctorActions" runat="server" Visible="false">
                <a href="Appointments.aspx" class="action-button">Randevu Yönetimi</a>
            </asp:PlaceHolder>
            
            <!-- Eğer kullanıcı hasta ise randevu oluştur göster -->
            <asp:PlaceHolder ID="phPatientActions" runat="server" Visible="false">
                <a href="CreateAppointment.aspx" class="action-button">Randevu Oluştur</a>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>