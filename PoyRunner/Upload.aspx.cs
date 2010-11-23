using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Threading;
using System.Web.Services;
using System.Text;

namespace PoyRunner
{
    public partial class Upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Submit_Click(object sender, EventArgs e)
        {

            PoyService.PoyService poy = new PoyService.PoyService();
            int fileId = poy.Init();

          
            poy.AddFile(fileId, PoyHelper.GetBuffer(FastaFile.PostedFile.InputStream), FastaFile.Value);
            poy.AddFile(fileId, PoyHelper.GetBuffer(SpatialFile.PostedFile.InputStream), SpatialFile.Value);
            poy.AddFile(fileId, PoyHelper.getPoyFile(FastaFile.Value, SpatialFile.Value, TextBoxSearchTimeHours.Text, TextBoxSearchTimeHoursMinutes.Text), "run.poy");
            poy.SubmitPoy(fileId, int.Parse(TextBoxNumberOfNodes.Text), int.Parse(TextBoxTotalTimeHours.Text), int.Parse(TextBoxTotalTimeMinutes.Text));


            //while (!poy.isDone(fileId))
            //{
            //    Thread.Sleep(1000);
            //}
            //Response.Redirect("DownLoads.aspx?jobid=" + fileId.ToString());
            

            Response.Redirect("Loading.aspx?jobid="+fileId.ToString());

         
        }

       
    }
}