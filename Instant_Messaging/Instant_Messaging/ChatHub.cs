using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    // Kullanıcılar ve bağlantı ID'leri
    private static Dictionary<string, string> userConnections = new Dictionary<string, string>();

    private static Dictionary<string, List<string>> _userConnections = new Dictionary<string, List<string>>();


    // Kullanıcı online durumları
    private static Dictionary<string, bool> userStatus = new Dictionary<string, bool>();

    public void Send(string senderId, string receiverId, string message)
    {
        try
        {
            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO CHAT (SenderID, ReceiverID, MessageText, [Timestamp], IsRead) VALUES (?, ?, ?, ?, ?)";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", senderId);
                    cmd.Parameters.AddWithValue("?", receiverId);
                    cmd.Parameters.AddWithValue("?", message);
                    cmd.Parameters.AddWithValue("?", DateTime.Now);
                    cmd.Parameters.AddWithValue("?", false);
                    cmd.ExecuteNonQuery();
                }
            }

            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            Clients.Caller.receiveMessage(new
            {
                senderId = senderId,
                text = message,
                timestamp = timestamp,
                isRead = false
            });

            foreach (var connectionId in _userConnections[receiverId])
            {
                Clients.Client(connectionId).receiveMessage(new
                {
                    senderId = senderId,
                    text = message,
                    timestamp = timestamp,
                    isRead = false
                });
            }
        }
        catch (Exception ex)
        {
            Clients.Caller.receiveMessage(new
            {
                senderId = senderId,
                text = "Hata: " + ex.Message,
                timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                isRead = false
            });
        }
    }

    public void UpdateOnlineStatus(string userId, bool isOnline)
    {
        if (isOnline)
        {
            // Kullanıcı bağlantısını ekle
            if (!_userConnections.ContainsKey(userId))
                _userConnections[userId] = new List<string>();

            string connectionId = Context.ConnectionId;
            if (!_userConnections[userId].Contains(connectionId))
                _userConnections[userId].Add(connectionId);

            // userStatus sözlüğünü de güncelle
            userStatus[userId] = true;
        }
        else
        {
            // Kullanıcı bağlantısını kaldır
            string connectionId = Context.ConnectionId;
            if (_userConnections.ContainsKey(userId))
            {
                _userConnections[userId].Remove(connectionId);
                if (_userConnections[userId].Count == 0)
                    _userConnections.Remove(userId);

                // userStatus sözlüğünü de güncelle
                userStatus[userId] = false;
            }
        }

        // Tüm kullanıcılara bildirim gönder
        Clients.All.userStatusChanged(new { userId = userId, isOnline = isOnline });
    }
    // IsUserOnline metodunu tek bir yerde tanımla (dosyanın sonundakini silin)
    public bool CheckUserStatus(string userId)
    {
        return _userConnections.ContainsKey(userId) && _userConnections[userId].Count > 0;
    }
    // Typing göstergesi için yeni metodlar ekle
    public void NotifyTyping(string senderId, string receiverId)
    {
        if (_userConnections.ContainsKey(receiverId))
        {
            foreach (var connectionId in _userConnections[receiverId])
            {
                Clients.Client(connectionId).userIsTyping(new { userId = senderId });
            }
        }
    }
    public void NotifyStoppedTyping(string senderId, string receiverId)
    {
        if (_userConnections.ContainsKey(receiverId))
        {
            foreach (var connectionId in _userConnections[receiverId])
            {
                Clients.Client(connectionId).userStoppedTyping(new { userId = senderId });
            }
        }
    }
    public bool IsUserOnline(string userId)
    {
        return _userConnections.ContainsKey(userId) && _userConnections[userId].Count > 0;
    }
    public List<object> GetChatHistory(string senderId, string receiverId)
    {
        List<object> messages = new List<object>();

        using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
        {
            conn.Open();
            OleDbCommand cmd = new OleDbCommand(
                "SELECT SenderID, ReceiverID, MessageText, Timestamp, IsRead FROM CHAT " +
                "WHERE (SenderID = ? AND ReceiverID = ?) OR (SenderID = ? AND ReceiverID = ?) ORDER BY Timestamp", conn);

            cmd.Parameters.AddWithValue("?", senderId);
            cmd.Parameters.AddWithValue("?", receiverId);
            cmd.Parameters.AddWithValue("?", receiverId); // tersini de al
            cmd.Parameters.AddWithValue("?", senderId);

            using (OleDbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    messages.Add(new
                    {
                        senderId = reader["SenderID"].ToString(),
                        receiverId = reader["ReceiverID"].ToString(),
                        text = reader["MessageText"].ToString(),
                        timestamp = Convert.ToDateTime(reader["Timestamp"]).ToString("dd.MM.yyyy HH:mm"),
                        isRead = Convert.ToBoolean(reader["IsRead"])
                    });
                }
            }
        }

        return messages;
    }

    public void MarkAsRead(string senderId, string receiverId)
    {
        // Veritabanında okundu olarak işaretle
        using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
        {
            conn.Open();
            OleDbCommand cmd = new OleDbCommand(
                "UPDATE CHAT SET IsRead = True WHERE SenderID = ? AND ReceiverID = ? AND IsRead = False", conn);

            cmd.Parameters.AddWithValue("?", senderId);
            cmd.Parameters.AddWithValue("?", receiverId);

            cmd.ExecuteNonQuery();
        }

        // Gönderici online ise okundu bilgisini gönder
        if (userConnections.ContainsKey(senderId))
        {
            Clients.Client(userConnections[senderId]).messageReadBy(receiverId);
        }
    }

    public void Typing(string senderId, string receiverId)
    {
        // Alıcı online ise yazıyor bilgisini gönder
        if (userConnections.ContainsKey(receiverId))
        {
            Clients.Client(userConnections[receiverId]).showTyping(senderId);
        }
    }

    public override Task OnConnected()
    {
        string userId = Context.QueryString["userId"];

        if (!string.IsNullOrEmpty(userId))
        {
            

            userConnections[userId] = Context.ConnectionId;
            userStatus[userId] = true;

            // _userConnections sözlüğünü güncelle (eğer kullanıyorsanız)
            if (!_userConnections.ContainsKey(userId))
                _userConnections[userId] = new List<string>();

            if (!_userConnections[userId].Contains(Context.ConnectionId))
                _userConnections[userId].Add(Context.ConnectionId);

            // Tüm kullanıcılara bu kullanıcının online olduğunu bildir
            Clients.All.userStatusChanged(userId, true);
        }

        return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
        string userId = Context.QueryString["userId"];

        if (!string.IsNullOrEmpty(userId) && _userConnections.ContainsKey(userId))
        {
            _userConnections[userId].Remove(Context.ConnectionId);

            // If this was the user's last connection, remove the user and notify others
            if (_userConnections[userId].Count == 0)
            {
                _userConnections.Remove(userId);
                Clients.All.userStatusChanged(new { userId = userId, isOnline = false });
            }
        }

        return base.OnDisconnected(stopCalled);
    }

    public override Task OnReconnected()
    {
        string userId = Context.QueryString["userId"];

        if (!string.IsNullOrEmpty(userId))
        {
            userConnections[userId] = Context.ConnectionId;
            userStatus[userId] = true;

            // Tüm kullanıcılara bu kullanıcının online olduğunu bildir
            Clients.All.userStatusChanged(new { userId = userId, isOnline = true });
        }

        return base.OnReconnected();
    }

    // Okunmamış mesaj sayısını almak için
    public int GetUnreadMessageCount(string senderId, string receiverId)
    {
        int count = 0;

        using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
        {
            conn.Open();
            OleDbCommand cmd = new OleDbCommand(
                "SELECT COUNT(*) FROM CHAT WHERE SenderID = ? AND ReceiverID = ? AND IsRead = False", conn);

            cmd.Parameters.AddWithValue("?", senderId);
            cmd.Parameters.AddWithValue("?", receiverId);

            count = (int)cmd.ExecuteScalar();
        }

        return count;
    }

   
}