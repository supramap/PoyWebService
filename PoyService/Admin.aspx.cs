using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Web.UI.HtmlControls;

namespace PoyService
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["password"]==null || 
                Session["password"].ToString() != "dan$123" || 
                Session["userName"]==null ||
                Session["userName"].ToString() != "dan")
                Server.Transfer("LogIn.aspx");
           
           
            foreach (DataRow row in DataAccess.getuserData().Rows)
            {
                
                HtmlTableRow tableRow = new HtmlTableRow();
                tableRow.Cells.Add(new HtmlTableCell{InnerText= row["Name"].ToString()} );
                tableRow.Cells.Add(new HtmlTableCell{InnerText= row["Email"].ToString()} );
                tableRow.Cells.Add(new HtmlTableCell { InnerText = row["Organization"].ToString() });
                tableRow.Cells.Add(new HtmlTableCell { InnerText = row["NumberOfJobs"].ToString() });
              	tableRow.Cells.Add(new HtmlTableCell { InnerText = row["TotalNodeMinutes"].ToString() });
                if (row["PassPhrase"] == null || string.IsNullOrEmpty(row["PassPhrase"].ToString()))
                {
                    HtmlTableCell cell = new HtmlTableCell();
                    var b = new Button();
                    b.Text="Grant Access";
                    b.ID = row["UserId"].ToString();
                    b.CommandArgument = row["UserId"].ToString();
                    b.Click += new EventHandler(b_Click);
                    cell.Controls.Add(b);
                    //cell.Controls.Add(new Button { Text = "Grant Access" });
                    tableRow.Cells.Add(cell);
                }
                else
                    tableRow.Cells.Add(new HtmlTableCell { InnerText = "Active" });

                Maintable.Rows.Add(tableRow);

            }
           
            //UserLiteral.Text=html.ToString();
        }

        void b_Click(object sender, EventArgs e)
        {
            string userId = ((Button)sender).ID;
            PoyServiceUser user;
            using (DataAccess access = new DataAccess())
            {
                string passphrase = access.makePassPrase(userId);
                user = new PoyServiceUser(userId,access);
            }

            Email email = new Email();
            email.sendMailToUser(user);
            Server.Transfer("Admin.aspx");
          
        }
    }
}