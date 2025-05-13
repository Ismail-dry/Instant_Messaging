<%@ Page Title="Profil" MasterPageFile="~/Site.master" Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Instant_Messaging.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="card z-depth-3 dark-mode-card" style="margin-top: 50px; max-width: 600px; margin-left: auto; margin-right: auto;">
            <div class="card-content">
                
                <div class="divider"></div>
                <div class="section">
                    <asp:Label ID="lblInfo" runat="server" CssClass="flow-text grey-text text-darken-2" />
               
            </div>
                <div class="section center-align">

                    <a href="Login.aspx" class="btn waves-effect waves-light red">
                        <i class="material-icons left"</i>Çıkış Yap
                    </a>
                    <a href="EditProfile.aspx" class="btn waves-effect waves-light blue">
                        <i class="material-icons left"</i>Profili Düzenle
                    </a>
                    
                </div>
        </div>
    </div>
</asp:Content>

