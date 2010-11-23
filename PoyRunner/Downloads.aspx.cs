using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PoyRunner
{
    public partial class Downloads : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string jobId = Request.QueryString["jobid"];

            Literal1.Text = string.Format(@"<a href=downloadfile.ashx?jobId={0}&fileName=output.txt >Program Output</a><br/><br/>
                             <a href=downloadfile.ashx?jobId={0}&fileName=results.kml >Program Kml Results</a><br/><br/>", jobId);
        }
    }
}