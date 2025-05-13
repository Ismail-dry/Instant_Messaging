using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Instant_Messaging
{
    public partial class Chat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["UserID"] == null || Session["Role"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            hfMyUserId.Value = Session["UserID"].ToString(); // Kullanıcı ID'yi hidden field'a ata

            if (!IsPostBack)
            {
                LoadOnlineUsers();

                // hfSelectedUserId değeri varsa veya rptUsers içinde en az bir öğe varsa
                if (!string.IsNullOrEmpty(hfSelectedUserId.Value) || (rptUsers.Items.Count > 0 && rptUsers.Items[0].FindControl("lnkUser") != null))
                {
                    if (string.IsNullOrEmpty(hfSelectedUserId.Value) && rptUsers.Items.Count > 0)
                    {
                        // İlk kullanıcıyı seç
                        LinkButton firstUser = rptUsers.Items[0].FindControl("lnkUser") as LinkButton;
                        if (firstUser != null)
                        {
                            hfSelectedUserId.Value = firstUser.CommandArgument;
                        }
                    }

                    LoadMessages();
                    UpdateSelectedUserInfo();
                }
            }
        }

        private void UpdateSelectedUserInfo()
        {
            if (!string.IsNullOrEmpty(hfSelectedUserId.Value))
            {
                string selectedUserId = hfSelectedUserId.Value;

                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("SELECT FullName FROM USERS WHERE UserID = ?", conn);
                    cmd.Parameters.AddWithValue("?", selectedUserId);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string selectedUserName = result.ToString();
                        litSelectedUser.Text = selectedUserName;

                        string myRole = Session["Role"].ToString();
                        string oppositeRole = (myRole == "Doktor") ? "Hasta" : "Doktor";
                        lblRole.Text = oppositeRole;
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSelectedUserId.Value))
                return;

            string myId = Session["UserID"].ToString();
            string otherId = hfSelectedUserId.Value;
            string keyword = txtSearch.Text;

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(
                    "SELECT MessageID, SenderID, ReceiverID, MessageText, [Timestamp], IsRead FROM CHAT WHERE ((SenderID=? AND ReceiverID=?) OR (SenderID=? AND ReceiverID=?)) AND MessageText LIKE ? ORDER BY [Timestamp]", conn);

                cmd.Parameters.AddWithValue("?", myId);
                cmd.Parameters.AddWithValue("?", otherId);
                cmd.Parameters.AddWithValue("?", otherId);
                cmd.Parameters.AddWithValue("?", myId);
                cmd.Parameters.AddWithValue("?", "%" + keyword + "%");

                OleDbDataReader dr = cmd.ExecuteReader();
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Load(dr);
                rptMessages.DataSource = dt;
                rptMessages.DataBind();
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSelectedUserId.Value))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen önce bir kullanıcı seçin.');", true);
                return;
            }

            if (fuAttachment.HasFile)
            {
                string fileName = Path.GetFileName(fuAttachment.FileName);
                string extension = Path.GetExtension(fileName).ToLower();

                List<string> allowedExtensions = new List<string> { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".txt" };

                if (!allowedExtensions.Contains(extension))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Geçersiz dosya türü.');", true);
                    return;
                }

                if (fuAttachment.PostedFile.ContentLength > 5 * 1024 * 1024)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Dosya 5MB'dan büyük olamaz.');", true);
                    return;
                }

                try
                {
                    string uploadFolder = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + extension;
                    string savePath = Path.Combine(uploadFolder, uniqueFileName);
                    fuAttachment.SaveAs(savePath);

                    string senderId = Session["UserID"].ToString();
                    string receiverId = hfSelectedUserId.Value;
                    string fileLink = $"<a href='Uploads/{uniqueFileName}' target='_blank'>Dosya: {fileName}</a>";

                    SaveMessage(senderId, receiverId, fileLink);
                    Response.Redirect(Request.RawUrl);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Yükleme hatası: {ex.Message}');", true);
                }
                // Dosyayı yükledikten ve mesajı kaydettikten sonra:
                Response.Redirect(Request.RawUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private void LoadMessages()
        {
            if (string.IsNullOrEmpty(hfSelectedUserId.Value))
                return;

            string myId = Session["UserID"].ToString();
            string otherId = hfSelectedUserId.Value;

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(
                    "SELECT MessageID, SenderID, ReceiverID, MessageText, [Timestamp], IsRead FROM CHAT WHERE (SenderID=? AND ReceiverID=?) OR (SenderID=? AND ReceiverID=?) ORDER BY [Timestamp]", conn);

                cmd.Parameters.AddWithValue("?", myId);
                cmd.Parameters.AddWithValue("?", otherId);
                cmd.Parameters.AddWithValue("?", otherId);
                cmd.Parameters.AddWithValue("?", myId);

                OleDbDataReader dr = cmd.ExecuteReader();
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Load(dr);

                rptMessages.DataSource = dt;
                rptMessages.DataBind();
            }
        }

        private void MarkMessagesAsRead()
        {
            if (string.IsNullOrEmpty(hfSelectedUserId.Value))
                return;

            string myId = Session["UserID"].ToString();
            string otherId = hfSelectedUserId.Value;

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("UPDATE CHAT SET IsRead = True WHERE SenderID = ? AND ReceiverID = ? AND IsRead = False", conn);

                cmd.Parameters.AddWithValue("?", otherId);
                cmd.Parameters.AddWithValue("?", myId);
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveMessage(string senderId, string receiverId, string messageText)
        {
            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO CHAT (SenderID, ReceiverID, MessageText, [Timestamp], IsRead) VALUES (?, ?, ?, ?, ?)";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", senderId);
                    cmd.Parameters.AddWithValue("?", receiverId);
                    cmd.Parameters.AddWithValue("?", messageText);
                    cmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;

                    cmd.Parameters.Add("?", OleDbType.Boolean).Value = false; 
                    cmd.ExecuteNonQuery();
                }
            }
        }


        // ASPX dosyasındaki rptUsers kontrol olayı
        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectUser")
            {
                string selectedUserId = e.CommandArgument.ToString();

                // Seçilen kullanıcıyı hidden field'a kaydet
                hfSelectedUserId.Value = selectedUserId;

                // Kullanıcı listesini seçili kullanıcı ile yeniden yükle
                LoadOnlineUsers();

                // Seçili kullanıcı için mesajları yükle
                LoadMessages();

                // Seçili kullanıcı bilgilerini güncelle
                UpdateSelectedUserInfo();

                // Mesajları okundu olarak işaretle
                MarkMessagesAsRead();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSelectedUserId.Value))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen önce bir kullanıcı seçin.');", true);
                return;
            }

            if (string.IsNullOrEmpty(txtMessage.Text.Trim()))
                return;

            string senderId = Session["UserID"].ToString();
            string receiverId = hfSelectedUserId.Value;
            string messageText = txtMessage.Text.Trim();

            // Mesajı veritabanına kaydet
            SaveMessage(senderId, receiverId, messageText);

            // Metin kutusunu temizle
            txtMessage.Text = string.Empty;

            // PRG desenini uygula: Sayfayı yeniden yükle ve form gönderimini engelle
            Response.Redirect(Request.RawUrl, false);
            Context.ApplicationInstance.CompleteRequest();

            // Mesajları yeniden yükle
            LoadMessages();

            // SignalR üzerinden istemciye bildirim gönderme JavaScript ile halledilir
        }
        protected void btnClearChat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSelectedUserId.Value))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen önce bir kullanıcı seçin.');", true);
                return;
            }

            string myId = Session["UserID"].ToString();
            string otherId = hfSelectedUserId.Value;

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(
                    "DELETE FROM CHAT WHERE (SenderID = ? AND ReceiverID = ?) OR (SenderID = ? AND ReceiverID = ?)", conn);

                cmd.Parameters.AddWithValue("?", myId);
                cmd.Parameters.AddWithValue("?", otherId);
                cmd.Parameters.AddWithValue("?", otherId);
                cmd.Parameters.AddWithValue("?", myId);

                int affectedRows = cmd.ExecuteNonQuery();

                // Mesajları temizledikten sonra sayfayı yenileyelim
                LoadMessages();

                // Kullanıcıya kaç mesajın silindiğini bildirelim
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('{affectedRows} mesaj başarıyla silindi.');", true);
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            // Arama metnini temizle
            txtSearch.Text = string.Empty;

            // Mesajları yeniden yükle
            LoadMessages();
        }

        // Çevrimiçi kullanıcıları yükleme (rptUsers için gerekli)
        private void LoadOnlineUsers()
        {
            string myRole = Session["Role"].ToString();
            string oppositeRole = (myRole == "Doktor") ? "Hasta" : "Doktor";
            string selectedUserId = hfSelectedUserId.Value;

            DataTable dt = new DataTable();
            dt.Columns.Add("UserID");
            dt.Columns.Add("FullName");
            dt.Columns.Add("IsSelected", typeof(bool));
            dt.Columns.Add("IsOnline", typeof(bool)); // Varsayılan false olacak çünkü veritabanında yok
            dt.Columns.Add("UnreadCount", typeof(int));

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                conn.Open();

                string query = @"
            SELECT USERS.USERID, USERS.Fullname, 
                (SELECT COUNT(*) FROM CHAT 
                 WHERE CHAT.SenderID = USERS.USERID AND CHAT.ReceiverID = ? AND CHAT.IsRead = False) AS UnreadCount 
            FROM USERS 
            WHERE USERS.Role = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("?", oppositeRole);

                    using (OleDbDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DataRow row = dt.NewRow();
                            row["UserID"] = dr["USERID"].ToString();
                            row["FullName"] = dr["Fullname"].ToString();
                            row["IsSelected"] = (dr["USERID"].ToString() == selectedUserId);
                            row["IsOnline"] = false; // Veritabanında olmadığı için sabit false atanıyor
                            row["UnreadCount"] = Convert.ToInt32(dr["UnreadCount"]);
                            dt.Rows.Add(row);
                        }
                    }
                }
            }

            rptUsers.DataSource = dt;
            rptUsers.DataBind();

            // JavaScript ile çevrimiçi durumlarını güncelle
            ScriptManager.RegisterStartupScript(this, GetType(), "updateUserStatuses",
                "$(function() { if(typeof updateSelectedUserStatus === 'function') { updateSelectedUserStatus(); } });", true);
        }
    }
    }