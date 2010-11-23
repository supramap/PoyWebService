using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PoyService
{
    public partial class Welcome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            Session["password"] = Login1.Password;
            Session["userName"] = Login1.UserName;

            Server.Transfer("PassPhrase.aspx");
        }

        protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
        {
           // using(DataAccess dataAccess = new DataAccess())
           // {
           //     dataAccess.AddUser(CreateUserWizard1.UserName, CreateUserWizard1.Password,CreateUserWizard1.Email);
           // }

           // Session["password"] = CreateUserWizard1.Password;
           // Session["userName"] = CreateUserWizard1.UserName;

           // Email email = new Email();
           //email.sendMailToUs(CreateUserWizard1.UserName+" from "+CreateUserWizard1.Email);

         

            Server.Transfer("PassPhrase.aspx");
        }
    }
}