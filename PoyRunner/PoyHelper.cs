using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace PoyRunner
{
    public class PoyHelper
    {
        public static readonly string tempDir;

         static PoyHelper()
        {
            if (Path.DirectorySeparatorChar== '\\')
                tempDir = @"C:\temp\";
            else tempDir = @"/var/tmp/";
        }
        public static byte[] getPoyFile(string FastaFileName, string SpatialFileName,string hours,string minutes)
        {
            using (StreamWriter writer = new StreamWriter(tempDir + "run.poy", false))
            {
                //writer.Write("read ('{0}') build (1)transform (static_approx)report ('resuls.kml', kml:(supramap, '{1}'))exit () \n\n\n\n", FastaFileName, SpatialFileName);



                //writer.Write("read (\"{0}\") \n", FastaFileName, SpatialFileName);
                //writer.Write("build (100) transform (static_approx) \n");
                //writer.Write("report (\"results.kml\", kml:(supramap, \"{1}\"))  \n", FastaFileName, SpatialFileName);
                //writer.Write("exit () \n");

                writer.Write(@"read (""{0}"")
                search(max_time:0:{2}:{3}, memory:gb:8)
                select(best:1)
                transform (static_approx)
                report (""results.kml"", kml:(supramap, ""{1}""))
                exit ()", FastaFileName, SpatialFileName,hours,minutes);

            }

            return File.ReadAllBytes(tempDir+ "run.poy");

            //StreamWriter output = new StreamWriter(new MemoryStream(), Encoding.UTF8);
            //output.Write("read ('{0}') build (1)transform (static_approx)report ('resuls.kml', kml:(supramap, '{1}'))exit () \n\n\n\n", FastaFileName, SpatialFileName);
            //output.Flush();
            //return output.BaseStream;
        }

        public static byte[] GetBuffer(Stream stream)
        {

            byte[] buffer = new byte[(int)stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}