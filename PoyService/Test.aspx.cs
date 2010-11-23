using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PoyService
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        
                GridView1.DataSource = DataAccess.getuserData();
                GridView1.DataBind();
                //GridView1. .Add(ButtonFieldBase ());
        }
    }
}