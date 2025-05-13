<%@ Page Title="Sohbet" MasterPageFile="~/Site.master" Language="C#" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="Instant_Messaging.Chat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="chat-container">
        <!-- Sol panel: kullanıcılar -->
        <div class="users-panel">
            <h5>Görüşmeler</h5>
            <div class="users-list" id="usersList">
                <asp:Repeater ID="rptUsers" runat="server" OnItemCommand="rptUsers_ItemCommand">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkUser" runat="server" CssClass='user-item <%# Eval("IsSelected").ToString() == "True" ? "active" : "" %>'
                            CommandName="SelectUser" CommandArgument='<%# Eval("UserID") %>'>
                            <div class="d-flex justify-content-between align-items-center">
                                <div class="user-info">
                                    <span class="user-name"><%# Eval("FullName") %></span>
                                    <span class="user-status">
                                        <i class='status-indicator <%# Eval("IsOnline").ToString() == "True" ? "online" : "offline" %>'></i>
                                        <%# Eval("IsOnline").ToString() == "True" ? "Çevrimiçi" : "Çevrimdışı" %>
                                    </span>
                                </div>
                                <div class="unread-badge <%# Convert.ToInt32(Eval("UnreadCount")) > 0 ? "visible" : "hidden" %>">
                                    <%# Eval("UnreadCount") %>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="mt-3">
                <div class="mb-2">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Mesaj ara..." />
                </div>
                <asp:Button ID="btnSearch" runat="server" Text="Ara" CssClass="btn btn-outline-primary btn-sm" OnClick="btnSearch_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Temizle" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnClear_Click" />
            </div>

            <div class="mt-3">
                <h6>Dosya Yükle</h6>
                <asp:FileUpload ID="fuAttachment" runat="server" CssClass="form-control form-control-sm" />
                <asp:Button ID="btnUpload" runat="server" Text="Yükle" CssClass="btn btn-outline-success btn-sm mt-2" OnClick="btnUpload_Click" />
            </div>

            <asp:HiddenField ID="hfSelectedUserId" runat="server" />
            <asp:HiddenField ID="hfMyUserId" runat="server" />

        </div>

        <!-- Sağ panel: sohbet -->
        <div class="chat-panel">
            <div class="chat-header">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <asp:Literal ID="litSelectedUser" runat="server" />
                        <span class="user-role">(<asp:Label ID="lblRole" runat="server" />)</span>
                    </div>
                    <div class="online-status">
                        <span id="selectedUserOnlineStatus" class="user-status"></span>
                    </div>
                </div>
            </div>

            <div class="chat-header">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <asp:Literal ID="Literal1" runat="server" />
                        <span class="user-role">(<asp:Label ID="Label1" runat="server" />)</span>
                    </div>
                    <div class="d-flex align-items-center">
                        <div class="online-status me-3">
                            <span id="selectedUserOnlineStatus" class="user-status"></span>
                        </div>
                        <asp:Button ID="btnClearChat" runat="server" Text="Sohbeti Temizle"
                            CssClass="btn btn-sm btn-outline-danger" OnClick="btnClearChat_Click"
                            OnClientClick="return confirm('Bu sohbeti temizlemek istediğinizden emin misiniz?');" />
                    </div>
                </div>
            </div>

            <!-- Mesajlar -->
            <div class="message-list" id="messageContainer">
                <asp:Repeater ID="rptMessages" runat="server">
                    <ItemTemplate>
                        <div class='message <%# Eval("SenderID").ToString() == Session["UserID"].ToString() ? "sent" : "received" %>'>
                            <%# Eval("MessageText") %>
                            <span class="timestamp">
                                <%# Convert.ToDateTime(Eval("Timestamp")).ToString("HH:mm") %>
                            </span>
                            <%# Eval("SenderID").ToString() == Session["UserID"].ToString() ?
                                "<span class='readStatus'>" + ((bool)Eval("IsRead") ? "✓✓ Okundu" : "✓ Gönderildi") + "</span>" : "" %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <div id="typing-indicator" class="typing-indicator hidden">
                    <div class="typing-dots">
                        <span class="dot"></span>
                        <span class="dot"></span>
                        <span class="dot"></span>
                    </div>
                    <span>yazıyor...</span>
                </div>
            </div>
            <div id="typingIndicator" style="font-style: italic; color: gray;"></div>

            <!-- Mesaj gönder -->
            <div class="chat-footer">
                <asp:TextBox ID="txtMessage" runat="server" placeholder="Mesaj yazın..." CssClass="form-control" />
                <asp:Button ID="btnSend" runat="server" Text="Gönder" CssClass="btn btn-success" OnClick="btnSend_Click" />
            </div>
        </div>
    </div>


    <style>
        body {
            background-color: #ece5dd;
            font-family: 'Segoe UI', sans-serif;
        }

        .chat-container {
            max-width: 900px;
            margin: 20px auto;
            background-color: #fff;
            border-radius: 8px;
            display: flex;
            height: 80vh;
            overflow: hidden;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }

        .users-panel {
            width: 30%;
            background-color: #f8f9fa;
            padding: 15px;
            border-right: 1px solid #ccc;
            overflow-y: auto;
            display: flex;
            flex-direction: column;
        }

        .users-list {
            margin-top: 10px;
            overflow-y: auto;
            flex-grow: 1;
        }

        .user-item {
            display: block;
            padding: 10px;
            border-bottom: 1px solid #e9e9e9;
            text-decoration: none;
            color: #333;
            transition: background-color 0.2s;
        }

            .user-item:hover {
                background-color: #f0f0f0;
                text-decoration: none;
            }

            .user-item.active {
                background-color: #e0f0ff;
                border-left: 3px solid #075E54;
            }

        .user-name {
            display: block;
            font-weight: 500;
        }

        .user-status {
            font-size: 12px;
            color: #666;
            display: flex;
            align-items: center;
        }

        .status-indicator {
            display: inline-block;
            width: 8px;
            height: 8px;
            border-radius: 50%;
            margin-right: 5px;
        }

            .status-indicator.online {
                background-color: #4CAF50;
            }

            .status-indicator.offline {
                background-color: #9e9e9e;
            }

        .unread-badge {
            background-color: #25D366;
            color: white;
            border-radius: 50%;
            width: 20px;
            height: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
        }

            .unread-badge.hidden {
                display: none;
            }

        .chat-panel {
            width: 70%;
            display: flex;
            flex-direction: column;
        }

        .chat-header {
            background-color: #075E54;
            color: white;
            padding: 10px 15px;
            font-weight: bold;
        }

        .user-role {
            font-size: 12px;
            margin-left: 5px;
            font-weight: normal;
        }

        .online-status .user-status {
            color: white;
        }

        .message-list {
            flex: 1;
            padding: 15px;
            overflow-y: auto;
            background-color: #e5ddd5;
        }

        .message {
            padding: 10px;
            border-radius: 7px;
            margin-bottom: 8px;
            max-width: 70%;
            position: relative;
            font-size: 14px;
            word-wrap: break-word;
        }

        .sent {
            background-color: #dcf8c6;
            margin-left: auto;
            text-align: right;
        }

        .received {
            background-color: white;
            margin-right: auto;
        }

        .timestamp {
            font-size: 10px;
            color: #999;
            margin-top: 5px;
            display: block;
        }

        .readStatus {
            font-size: 10px;
            color: #777;
            display: block;
        }

        .chat-footer {
            padding: 10px 15px;
            border-top: 1px solid #ccc;
            display: flex;
            gap: 10px;
            background-color: #f5f5f5;
        }

            .chat-footer input[type="text"] {
                flex: 1;
                border-radius: 20px;
                border: 1px solid #ccc;
                padding: 8px 15px;
            }

            .chat-footer button {
                border-radius: 20px;
            }

        .typing-indicator {
            display: flex;
            align-items: center;
            margin-bottom: 10px;
            background-color: rgba(255, 255, 255, 0.8);
            padding: 5px 10px;
            border-radius: 12px;
            max-width: 100px;
            font-size: 12px;
            color: #666;
        }

            .typing-indicator.hidden {
                display: none;
            }

        .typing-dots {
            display: flex;
            margin-right: 5px;
        }

        .dot {
            width: 6px;
            height: 6px;
            background-color: #888;
            border-radius: 50%;
            margin: 0 2px;
            animation: pulse 1.5s infinite;
        }

            .dot:nth-child(2) {
                animation-delay: 0.2s;
            }

            .dot:nth-child(3) {
                animation-delay: 0.4s;
            }

        .btn-outline-danger {
            color: #dc3545;
            border-color: #dc3545;
        }

            .btn-outline-danger:hover {
                color: #fff;
                background-color: #dc3545;
                border-color: #dc3545;
            }

        .me-3 {
            margin-right: 1rem;
        }

        @keyframes pulse {
            0% {
                transform: scale(1);
                opacity: 1;
            }

            50% {
                transform: scale(1.2);
                opacity: 0.7;
            }

            100% {
                transform: scale(1);
                opacity: 1;
            }
        }
    </style>

    <script src="Scripts/jquery-3.7.1.min.js"></script>
    <script src="Scripts/jquery.signalR-2.4.3.min.js"></script>
    <script src="/signalr/hubs"></script>

    <script>

        $(function () {
            const chat = $.connection.chatHub;
            const userId = '<%= Session["UserID"] %>';
            let selectedUserId = '<%= hfSelectedUserId.Value %>';
            let typingTimeout = null;

            // Sayfa yüklenince
            function initializeChatPage() {
                scrollToBottom();

                // Kullanıcının çevrimiçi olduğunu bildir
                chat.server.updateOnlineStatus(userId, true);

                // Tüm kullanıcıların durumunu sorgula
                refreshAllUserStatuses();



                // Sayfadan ayrılırken çevrimdışı ol
                $(window).on('beforeunload', function () {
                    chat.server.updateOnlineStatus(userId, false);
                });
            }

            // SignalR ile bağlan
            $.connection.hub.qs = { userId: userId }; // Query string ile userId gönder
            $.connection.hub.start().done(function () {
                // Girişte kullanıcıyı çevrimiçi olarak işaretle
                chatHub.server.updateOnlineStatus(userId, true);
            });


            // Mesaj okunduğunda
            chat.client.messageReadBy = function (message) {
                if (message.readerId === selectedUserId) {
                    $(".sent .readStatus").text("✓✓ Okundu");
                }
            };

            // Handle receiving a message
            chatHub.client.receiveMessage = function (message) {
                const isMine = message.senderId === userId;
                const css = isMine ? "sent" : "received";
                const html = `
            <div class="message ${css}">
                ${message.text}
                <span class="timestamp">${message.timestamp}</span>
                ${isMine ? "<span class='readStatus'>✓ Gönderildi</span>" : ""}
            </div>
        `;
                $("#messageContainer").append(html);
                scrollToBottom();



                // Hide typing indicator
                $("#typing-indicator").addClass("hidden");
                $("#typingIndicator").text("");



                // Seçili kullanıcının durumunu güncelle
                //if (userStatusInfo.userId === selectedUserId) {
                //    const statusHtml = `
                //        <i class="status-indicator ${userStatusInfo.isOnline ? 'online' : 'offline'}"></i>
                //        ${userStatusInfo.isOnline ? 'Çevrimiçi' : 'Çevrimdışı'}
                //    `;
                //    $("#selectedUserOnlineStatus").html(statusHtml);
                //}
            };


            // Update when user status changes
            chatHub.client.userStatusChanged = function (data) {
                console.log("Status changed for user:", data.userId, "Online:", data.isOnline);

                // Update in user list
                const userElement = $(`.user-item[data-userid='${data.userId}']`);
                if (userElement.length > 0) {
                    const statusIndicator = userElement.find(".status-indicator");
                    const statusText = userElement.find(".user-status");

                    statusIndicator.removeClass("online offline")
                        .addClass(data.isOnline ? "online" : "offline");

                    statusText.html(`<i class='status-indicator ${data.isOnline ? "online" : "offline"}'></i> 
                ${data.isOnline ? "Çevrimiçi" : "Çevrimdışı"}`);
                }

                // If it's the selected user, update the header status too
                if (data.userId === selectedUserId) {
                    updateSelectedUserStatus(data.isOnline);
                }
            };


            $.connection.hub.qs = { "userId": $("#hfMyUserId").val() }; // Sunucuya userId gönder
            $.connection.hub.start().done(function () {
                console.log("SignalR bağlantısı kuruldu.");
            });

            // Yazıyor göstergesi

            //chat.client.userIsTyping = function (typingInfo) {
            //    if (typingInfo.userId === selectedUserId) {
            //        $("#typing-indicator").removeClass("hidden");
            //        scrollToBottom();
            //    }
            //};

            //chat.client.userStoppedTyping = function (typingInfo) {
            //    if (typingInfo.userId === selectedUserId) {
            //        $("#typing-indicator").addClass("hidden");
            //    }
            //};

            // SignalR bağlantıyı başlat
            //$.connection.hub.qs = { userId: userId };
            //$.connection.hub.start().done(function () {
            //    console.log("SignalR bağlantısı başarılı.");
            initializeChatPage();

            // Server side butonlar için manuel mesaj gönderme işlevi
            $("#<%= btnSend.ClientID %>").click(function () {
                const msg = $("#<%= txtMessage.ClientID %>").val();
                    if (msg.trim() !== "") {
                        selectedUserId = $("#<%= hfSelectedUserId.Value %>");
                        chat.server.send(userId, selectedUserId, msg);

                        // Yazıyor durumunu temizle
                        if (typingTimeout) {
                            clearTimeout(typingTimeout);
                            typingTimeout = null;
                            chat.server.notifyStoppedTyping(userId, selectedUserId);
                        }
                    }
                });
            // Yazıyor bilgisini gönder
            $("#<%= txtMessage.ClientID %>").on("input", function () {
                if (selectedUserId) {
                    chatHub.server.notifyTyping(userId, selectedUserId);

                    // Önceki zamanlayıcıyı temizle ve yenisini ayarla
                    if (typingTimeout) clearTimeout(typingTimeout);

                    typingTimeout = setTimeout(function () {
                        chatHub.server.notifyStoppedTyping(userId, selectedUserId);
                        typingTimeout = null;
                    }, 2000);
                }
            });
            // Enter tuşu kontrolü
            $("#<%= txtMessage.ClientID %>").keypress(function (e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $("#<%= btnSend.ClientID %>").click();
                }
            });

            chatHub.client.userIsTyping = function (data) {
                if (data.userId === selectedUserId) {
                    $("#typing-indicator").removeClass("hidden");
                    $("#typingIndicator").text(data.userId + " yazıyor...");
                    scrollToBottom();
                }
            };

            chatHub.client.userStoppedTyping = function (data) {
                if (data.userId === selectedUserId) {
                    $("#typing-indicator").addClass("hidden");
                    $("#typingIndicator").text("");
                }

            };

            // Yazıyor bilgisini gönder
            $("#<%= txtMessage.ClientID %>").on("input", function () {
                if (selectedUserId) {
                    if (!typingTimeout) {
                        chat.server.notifyTyping(userId, selectedUserId);
                    }

                    // Önceki zamanlayıcıyı temizle ve yenisini ayarla
                    if (typingTimeout) clearTimeout(typingTimeout);

                    typingTimeout = setTimeout(function () {
                        chat.server.notifyStoppedTyping(userId, selectedUserId);
                        typingTimeout = null;
                    }, 2000);
                }
            });
        }).fail(function (error) {
            console.log("SignalR bağlantı hatası: " + error);
        });

        // Enter tuşu kontrolü
        $("#<%= txtMessage.ClientID %>").keypress(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                $("#<%= btnSend.ClientID %>").click();
                }
            });

        // Tüm kullanıcıların durumunu sorgulamak için yeni fonksiyon
        function refreshAllUserStatuses() {
            // Tüm kullanıcı öğelerini kontrol et
            $(".user-item").each(function () {
                const userElement = $(this);
                const checkUserId = userElement.data("userid");

                if (checkUserId) {
                    // Server'dan kullanıcının durumunu sorgula
                    chat.server.checkUserStatus(checkUserId).done(function (isOnline) {
                        const statusIndicator = userElement.find(".status-indicator");
                        const statusText = userElement.find(".user-status span");

                        statusIndicator.removeClass("online offline")
                            .addClass(isOnline ? "online" : "offline");


                        statusText.text(isOnline ? "Çevrimiçi" : "Çevrimdışı");
                    });
                }
            });

            // Seçili kullanıcının durumunu da güncelle
            updateSelectedUserStatus();
        }
        // Tüm kullanıcıların durumunu sorgulamak için yeni fonksiyon
        function refreshAllUserStatuses() {
            // Tüm kullanıcı öğelerini kontrol et
            $(".user-item").each(function () {
                const userElement = $(this);
                const checkUserId = userElement.data("userid");

                if (checkUserId) {
                    // Server'dan kullanıcının durumunu sorgula
                    chat.server.checkUserStatus(checkUserId).done(function (isOnline) {
                        const statusIndicator = userElement.find(".status-indicator");
                        const statusText = userElement.find(".user-status span");

                        statusIndicator.removeClass("online offline")
                            .addClass(isOnline ? "online" : "offline");

                        statusText.text(isOnline ? "Çevrimiçi" : "Çevrimdışı");
                    });
                }
            });

            // Seçili kullanıcının durumunu da güncelle
            updateSelectedUserStatus();
        }

        function scrollToBottom() {
            const container = document.getElementById("messageContainer");
            container.scrollTop = container.scrollHeight;
        }
      
        function updateSelectedUserStatus() {
            selectedUserId = '<%= hfSelectedUserId.Value %>';
            if (selectedUserId) {
                // Server'dan kullanıcının durumunu sorgula
                chat.server.checkUserStatus(selectedUserId).done(function (isOnline) {
                    const statusHtml = `
                <i class="status-indicator ${isOnline ? 'online' : 'offline'}"></i>
                ${isOnline ? 'Çevrimiçi' : 'Çevrimdışı'}
            `;
                    $("#selectedUserOnlineStatus").html(statusHtml);
                });
            }
        }
        // Handle page visibility changes
        document.addEventListener("visibilitychange", function () {
            if (document.visibilityState === "visible") {
                chatHub.server.updateOnlineStatus(userId, true);
            } else {
                chatHub.server.updateOnlineStatus(userId, false);
            }
        });


        // Sayfa arka plana alındığında veya sekme değiştiğinde
        $(window).on("blur", function () {
            chat.server.updateOnlineStatus(userId, false);
        });

        // Sayfa tekrar aktif olduğunda
        $(window).on("focus", function () {
            chat.server.updateOnlineStatus(userId, true);
        });
        });
    </script>
</asp:Content>
