using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Instant_Messaging
{
    public partial class PatientDashboard : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"]?.ToString() != "Hasta")
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadPatientInfo();
            }
        }

        private void LoadPatientInfo()
        {
            string userId = Session["UserID"].ToString();

            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                string query = "SELECT FullName, Email FROM Users WHERE UserID = @UserID AND Role = 'Hasta'";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblPatientName.Text = reader["FullName"].ToString() + " (" + reader["Email"].ToString() + ")";
                    }
                }
            }
        }
    }
}