using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Instant_Messaging
    {
        public partial class Dashboard : System.Web.UI.Page
        {
            protected void Page_Load(object sender, EventArgs e)
            {
            if (Session["Role"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
                {
                // Kullanıcı bilgilerini yükle
                LoadUserInfo();

                // İstatistikleri yükle
                LoadStatistics();

                // Son mesajlaşmaları yükle
                LoadRecentMessages();

                // Rol bazlı görünüm ayarlamaları
                ConfigureRoleBasedView();
            }
            }
        private void LoadUserInfo()
        {
            string userId = Session["UserID"].ToString();

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT FullName, Role FROM USERS WHERE UserID = ?", conn);
                cmd.Parameters.AddWithValue("?", userId);

                using (OleDbDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        litUserName.Text = dr["FullName"].ToString();
                        string role = dr["Role"].ToString();
                        litUserRole.Text = role == "Doktor" ? "Doktor Paneli" : "Hasta Paneli";
                    }
                }
            }
        }
        private void LoadStatistics()
        {
            string userId = Session["UserID"].ToString();
            string userRole = Session["Role"].ToString();
            string oppositeRole = userRole == "Doktor" ? "Hasta" : "Doktor";

            // Toplam kişi sayısı
            int totalContacts = 0;
            // Toplam mesaj sayısı
            int totalMessages = 0;
            // Okunmamış mesaj sayısı
            int unreadMessages = 0;
            // Son aktivite zamanı
            DateTime? lastActivity = null;

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();

                // Toplam kişi sayısı
                OleDbCommand cmdContacts = new OleDbCommand("SELECT COUNT(*) FROM USERS WHERE Role = ?", conn);
                cmdContacts.Parameters.AddWithValue("?", oppositeRole);
                totalContacts = Convert.ToInt32(cmdContacts.ExecuteScalar());

                // Toplam mesaj sayısı
                OleDbCommand cmdMessages = new OleDbCommand(
                    "SELECT COUNT(*) FROM CHAT WHERE SenderID = ? OR ReceiverID = ?", conn);
                cmdMessages.Parameters.AddWithValue("?", userId);
                cmdMessages.Parameters.AddWithValue("?", userId);
                totalMessages = Convert.ToInt32(cmdMessages.ExecuteScalar());

                // Okunmamış mesaj sayısı
                OleDbCommand cmdUnread = new OleDbCommand(
                    "SELECT COUNT(*) FROM CHAT WHERE ReceiverID = ? AND IsRead = False", conn);
                cmdUnread.Parameters.AddWithValue("?", userId);
                unreadMessages = Convert.ToInt32(cmdUnread.ExecuteScalar());

                // Son aktivite
                OleDbCommand cmdLastActivity = new OleDbCommand(
                    "SELECT MAX(Timestamp) FROM CHAT WHERE SenderID = ? OR ReceiverID = ?", conn);
                cmdLastActivity.Parameters.AddWithValue("?", userId);
                cmdLastActivity.Parameters.AddWithValue("?", userId);

                object lastActivityObj = cmdLastActivity.ExecuteScalar();
                if (lastActivityObj != null && lastActivityObj != DBNull.Value)
                {
                    lastActivity = Convert.ToDateTime(lastActivityObj);
                }
            }

            // İstatistikleri göster
            litTotalContacts.Text = totalContacts.ToString();
            litTotalMessages.Text = totalMessages.ToString();
            litUnreadMessages.Text = unreadMessages.ToString();

            if (lastActivity.HasValue)
            {
                TimeSpan timeDiff = DateTime.Now - lastActivity.Value;

                if (timeDiff.TotalDays < 1)
                {
                    if (timeDiff.TotalHours < 1)
                    {
                        litLastActive.Text = $"{(int)timeDiff.TotalMinutes} dk önce";
                    }
                    else
                    {
                        litLastActive.Text = $"{(int)timeDiff.TotalHours} saat önce";
                    }
                }
                else if (timeDiff.TotalDays < 30)
                {
                    litLastActive.Text = $"{(int)timeDiff.TotalDays} gün önce";
                }
                else
                {
                    litLastActive.Text = lastActivity.Value.ToString("dd.MM.yyyy");
                }
            }
            else
            {
                litLastActive.Text = "Hiç";
            }
        }
        private void LoadRecentMessages()
        {
            string userId = Session["UserID"].ToString();

            DataTable dt = new DataTable();
            dt.Columns.Add("UserName");
            dt.Columns.Add("MessagePreview");
            dt.Columns.Add("Timestamp");
            dt.Columns.Add("IsUnread", typeof(bool));

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();

                // Son mesajlaşmaları al (en son mesajlaşılan kişileri bul)
                string query = @"
                          SELECT 
                         ContactID,
                         MAX(Timestamp) AS LastMessageTime
                    FROM (
                            SELECT 
                            IIF(SenderID = ?, ReceiverID, SenderID) AS ContactID,
                            Timestamp
                     FROM CHAT
        WHERE SenderID = ? OR ReceiverID = ?
    ) AS SubQuery
    GROUP BY ContactID
    ORDER BY MAX(Timestamp) DESC";

                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", userId);
                cmd.Parameters.AddWithValue("?", userId);
                cmd.Parameters.AddWithValue("?", userId);
                cmd.Parameters.AddWithValue("?", userId);

                using (OleDbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string contactId = dr["ContactID"].ToString();
                        DateTime lastMessageTime = Convert.ToDateTime(dr["LastMessageTime"]);

                        // Kişi bilgilerini al
                        string contactName = GetUserName(conn, contactId);

                        // Son mesajı al
                        string messagePreview = "";
                        bool isUnread = false;
                        GetLastMessageDetails(conn, userId, contactId, out messagePreview, out isUnread);

                        DataRow row = dt.NewRow();
                        row["UserName"] = contactName;
                        row["MessagePreview"] = messagePreview;

                        // Zaman gösterimi
                        TimeSpan timeDiff = DateTime.Now - lastMessageTime;
                        string timestamp;

                        if (timeDiff.TotalDays < 1)
                        {
                            timestamp = lastMessageTime.ToString("HH:mm");
                        }
                        else if (timeDiff.TotalDays < 7)
                        {
                            timestamp = lastMessageTime.ToString("ddd");
                        }
                        else
                        {
                            timestamp = lastMessageTime.ToString("dd.MM.yyyy");
                        }

                        row["Timestamp"] = timestamp;
                        row["IsUnread"] = isUnread;

                        dt.Rows.Add(row);
                    }
                }
            }

            // En fazla 5 son mesajlaşma göster
            rptRecentMessages.DataSource = dt;
            rptRecentMessages.DataBind();
            pnlNoMessages.Visible = dt.Rows.Count == 0;

        }
        private string GetUserName(OleDbConnection conn, string userId)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT FullName FROM USERS WHERE UserID = ?", conn);
            cmd.Parameters.AddWithValue("?", userId);
            return cmd.ExecuteScalar()?.ToString() ?? "Bilinmeyen Kullanıcı";
        }
        private void GetLastMessageDetails(OleDbConnection conn, string userId, string contactId, out string messagePreview, out bool isUnread)
        {
            messagePreview = "";
            isUnread = false;

            string query = @"
                SELECT TOP 1 MessageText, IsRead, SenderID
                FROM CHAT 
                WHERE (SenderID = ? AND ReceiverID = ?) OR (SenderID = ? AND ReceiverID = ?)
                ORDER BY Timestamp DESC";

            OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("?", userId);
            cmd.Parameters.AddWithValue("?", contactId);
            cmd.Parameters.AddWithValue("?", contactId);
            cmd.Parameters.AddWithValue("?", userId);

            using (OleDbDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    string messageText = dr["MessageText"].ToString();
                    bool isRead = Convert.ToBoolean(dr["IsRead"]);
                    string senderId = dr["SenderID"].ToString();

                    // Mesaj önizlemesi (HTML etiketlerini temizle)
                    messagePreview = StripHtml(messageText);
                    if (messagePreview.Length > 50)
                    {
                        messagePreview = messagePreview.Substring(0, 47) + "...";
                    }

                    // Eğer karşıdan gelen ve okunmadıysa
                    isUnread = (senderId == contactId && !isRead);
                }
            }
        }
        private string StripHtml(string html)
        {
            // Basit bir HTML temizleme
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        }

        protected void lnkEditProfile_Click(object sender, EventArgs e)
        {
            Response.Redirect("Profile.aspx");
        }

        private void ConfigureRoleBasedView()
        {
            string role = Session["Role"].ToString();

            if (role == "Doktor")
            {
                phDoctorActions.Visible = true;
                phPatientActions.Visible = false;
            }
            else // Hasta
            {
                phDoctorActions.Visible = false;
                phPatientActions.Visible = true;
            }
        }
    }
    }