using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PoyService
{
    public partial class LogIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonLogOn_Click(object sender, EventArgs e)
        {
            Session["password"] = TextBoxPassword.Text;
            Session["userName"] = TextBoxUserName.Text;


            if (Session["password"] == null ||
                Session["password"].ToString() != "dan$123" ||
                Session["userName"] == null ||
                Session["userName"].ToString() != "dan")
                Label1.Text = "Bad User Name Or Password";
            else
                Server.Transfer("Admin.aspx");
        }

    }
}