using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using PoyRunner.PoyService;
namespace PoyRunner
{
    public partial class Loading : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string jobId = Request.QueryString["jobid"];

            PoyService.PoyService p = new PoyService.PoyService();
            if (p.IsDoneYet(int.Parse(jobId)))
                Server.Transfer("Downloads.aspx?jobid=" + jobId);

            
            ScriptCode.Text = @"<script src='http://code.jquery.com/jquery-latest.js'></script>
                        <script>
                       
                        function wait(id) {
                        
                    if (PageMethods.update(id) ) {
                        window.location('DownLoads.aspx?jobid='+id);
                    } else {
                        setTimeout(wait(id), 500);
                    }
                }
                $(document).ready( wait(" + jobId + "));</script>";

        }

        [WebMethod]
        public static bool update(int id)
        {
            //return true;

            PoyService.PoyService p = new PoyService.PoyService();
            return p.IsDoneYet(id);
        }
    }
}