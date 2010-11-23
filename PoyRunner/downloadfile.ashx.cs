using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PoyRunner
{
    /// <summary>
    /// Summary description for downloadfile
    /// </summary>
    public class downloadfile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {

            //jobId=409802994&fileName='
            int jobId = int.Parse( context.Request.QueryString["jobid"]);
            string fileName = context.Request.QueryString["fileName"];

            PoyService.PoyService poy = new PoyService.PoyService();
            byte[] buffer = poy.GetFile(jobId, fileName);

       
            var r = context.Response;
            r.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            r.ContentType = "text/plain";
            r.OutputStream.Write(buffer,0,buffer.Length);
        }
        public bool IsReusable { get { return false; } }


    }
}