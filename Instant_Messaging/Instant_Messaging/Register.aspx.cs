using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;


namespace Instant_Messaging
{
    public partial class Register : System.Web.UI.Page
    {
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

        }
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\veritabani.accdb";
            string hashedPassword = ComputeSha256Hash(txtPassword.Text);

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // Kullanıcı var mı kontrolü
                string checkUser = "SELECT COUNT(*) FROM USERS WHERE Username = ?";
                using (OleDbCommand cmd = new OleDbCommand(checkUser, conn))
                {
                    cmd.Parameters.AddWithValue("?", txtUsername.Text);
                    int userCount = (int)cmd.ExecuteScalar();

                    if (userCount > 0)
                    {
                        lblMessage.Text = "Bu kullanıcı adı zaten kullanılıyor.";
                        return;
                    }
                }

                // Yeni kullanıcı ekle
                string insertQuery = "INSERT INTO USERS (Username, [Password], FullName, Email, Role) VALUES (?, ?, ?, ?, ?)";
                using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("?", txtUsername.Text);
                    cmd.Parameters.AddWithValue("?", hashedPassword); // Şifreleme istenirse burası değişmeli
                    cmd.Parameters.AddWithValue("?", txtFullName.Text);
                    cmd.Parameters.AddWithValue("?", txtEmail.Text);
                    cmd.Parameters.AddWithValue("?", ddlRole.SelectedValue);

                    cmd.ExecuteNonQuery();
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = "Kayıt başarıyla tamamlandı!";
                }
            }
        }
    }
}
