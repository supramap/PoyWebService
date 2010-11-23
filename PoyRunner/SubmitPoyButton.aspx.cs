using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;

namespace PoyRunner
{
    public partial class SubmitPoyButton : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string queryId = Request.QueryString["query"];
  

            PoyService.PoyService poy = new PoyService.PoyService();
            int fileId = poy.Init();

            string fatetaFileName = "dna.fasta";
            string spatialFileName = "spatial.csv";
            Stream FastaFile = ((HttpWebRequest)WebRequest.Create("www.whatecer.com")).GetResponse().GetResponseStream();
            Stream SpatialFile = ((HttpWebRequest)WebRequest.Create("www.whatecer.com")).GetResponse().GetResponseStream();
            poy.AddFile(fileId, PoyHelper.GetBuffer(FastaFile), fatetaFileName);
            poy.AddFile(fileId, PoyHelper.GetBuffer(SpatialFile), spatialFileName);
            poy.AddFile(fileId, PoyHelper.getPoyFile(fatetaFileName, spatialFileName,"0","45"), "run.poy");
            poy.SubmitPoy(fileId, 50, 2, 0);
            
            //uri
            //HttpRequest request = new HttpRequest( HttpPostedFile
            //poy.AddFile(
        }
    }
}