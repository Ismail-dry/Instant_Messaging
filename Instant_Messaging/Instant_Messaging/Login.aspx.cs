using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;

namespace Instant_Messaging
{
    public partial class Login : System.Web.UI.Page
    {
        // Basit örnek kullanıcı "veritabanı"
        // Gerçek kullanımda bir veritabanı (SQL, Entity Framework vs.) gerekir.
        private static Dictionary<string, (string Email, string Password)> users = new Dictionary<string, (string, string)>();

        private string HashPassword(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // Sayfa yüklendiğinde yapılacak özel bir şey yok.
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string hashedPassword = HashPassword(password);

            string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                string query = "SELECT * FROM USERS WHERE Username=? AND Password=?";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", username);
                cmd.Parameters.AddWithValue("?", hashedPassword);

                OleDbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string role = reader["Role"].ToString();
                    Session["Username"] = username;
                    Session["UserID"] = reader["UserID"].ToString(); // EKLENDİ

                    Session["Role"] = role;
                    Session["FullName"] = reader["FullName"].ToString();

                    if (role == "Doktor")
                        Response.Redirect("DoctorDashboard.aspx");
                    else if (role == "Hasta")
                        Response.Redirect("PatientDashboard.aspx");
                }
                else
                {
                    ShowError("Hatalı kullanıcı adı veya parola!");
                }
            }
        }
        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

            // Make sure password field isn't empty
            if (string.IsNullOrEmpty(txtRegPassword.Text))
            {
                lblMessage.Text = "Şifre boş olamaz.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string hashedPassword = ComputeSha256Hash(txtRegPassword.Text);

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Kullanıcı var mı kontrolü
                    string checkUser = "SELECT COUNT(*) FROM USERS WHERE Username = ?";
                    using (OleDbCommand cmd = new OleDbCommand(checkUser, conn))
                    {
                        cmd.Parameters.AddWithValue("?", txtRegUsername.Text);
                        int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (userCount > 0)
                        {
                            lblMessage.Text = "Bu kullanıcı adı zaten kullanılıyor.";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            return;
                        }
                    }

                    // Yeni kullanıcı ekle
                    string insertQuery = "INSERT INTO USERS (Username, [Password], FullName, Email, Role) VALUES (?, ?, ?, ?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@p1", txtRegUsername.Text);
                        cmd.Parameters.AddWithValue("@p2", hashedPassword);
                        cmd.Parameters.AddWithValue("@p3", txtRegFullName.Text);
                        cmd.Parameters.AddWithValue("@p4", txtRegEmail.Text);
                        cmd.Parameters.AddWithValue("@p5", ddlRole.SelectedValue);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblMessage.ForeColor = System.Drawing.Color.Green;
                            lblMessage.Text = "Kayıt başarıyla tamamlandı!";

                            // Form alanlarını temizle
                            txtRegUsername.Text = "";
                            txtRegPassword.Text = "";
                            txtRegFullName.Text = "";
                            txtRegEmail.Text = "";
                        }
                        else
                        {
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            lblMessage.Text = "Kayıt sırasında bir hata oluştu.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Hata: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }

   }