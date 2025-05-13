using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Instant_Messaging
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblInfo.Text = $"<b>Ad:</b> {Session["FullName"]}<br />" +
                               $"<b>Kullanıcı:</b> {Session["Username"]}<br />" +
                               $"<b>Rol:</b> {Session["Role"]}";
            }
        }
    }
}