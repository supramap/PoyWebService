using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PoyService
{
    public partial class Application : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitApplication_Click(object sender, EventArgs e)
        {
             using (DataAccess dataAccess = new DataAccess())
            {
                dataAccess.AddUser(Name.Text, Email.Text, Organzation.Text, description.Text);
            }
             Email email = new Email();
             email.sendMailToUs(Name.Text + " from " + Organzation.Text + " at " + Email.Text,description.Text);
             Response.Redirect("ApplicationSubmited.htm");
			//Server.Transfer("ApplicationSubmited.htm");
        }
    }
}