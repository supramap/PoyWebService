using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PoyService
{
    public partial class PassPhrase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

            //PoyServiceUser u = PoyServiceUser.getUAuthenticatedUser(Session,  new DataAccess());
            //if (u == null)
            //{
            //    Literal1.Text = "log in failure";
            //    return;
            //}
     

            //if(string.IsNullOrEmpty(u.PassPhrase))
            //    Literal1.Text = "Your application to develop for the poy web service has been submitted. You will be notified shortly of your acceptance.";
            //else Literal1.Text = "Your Passphrase user code is: "+u.PassPhrase;

            //if (u.Admin == "1")
            //    Literal1.Text = Literal1.Text + "<br/><br/> Hey Admin do your 'admin'ing <a href='Admin.aspx'>here</a>";
          
        }
    }
}