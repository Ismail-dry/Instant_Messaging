using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Instant_Messaging
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["FullName"] != null)
                {
                    lblWelcome.Text = "Hoş geldiniz, " + Session["FullName"];
                }
                else
                {
                    lblWelcome.Text = "Hoş geldiniz!";
                }
            }
        }
    }
}