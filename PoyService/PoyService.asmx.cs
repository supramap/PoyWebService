using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace PoyService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "supramap", Description = "Service for running poy on glenn osc super computer")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class PoyService : System.Web.Services.WebService
    {
        public Poy poy= new Poy();

        [WebMethod(Description = @"The 'Init' method basically creates a directory on the super computer it returns a token that must be 
          used on all subsequent method calls as the jobId parameter.Note this token will only work with the ipaddress that was used to 
          call this method. Note it is not enforced but it highly encouraged to use this method over https.
          a pass prase to use this web service please vist http://glenn-service.bmi.ohio-state.edu/Welcome.aspx")]
        
        public int Init(string passPhase)
        {
            int Token;
            using (DataAccess dataAccess = new DataAccess())
            {
                Token = dataAccess.getToken(passPhase, HttpContext.Current.Request.UserHostAddress);
            }
            if (Token == -1) return -1;
            
            poy.Init(Token);
            return Token;
        }

        [WebMethod(Description = @"The 'AddFile' method is used to basically put a file on the super 
         computer. It should be called once to a load a .poy script. Plus 
         it should be called for each file that the POY script references. 
         It is the responsibility of the downstream client developer to 
         generate the POY scripts this places more work in there hands but 
         also give them great flexibility to do design web apps that can 
         use POY for anything.
		")]
        public string AddFile(int jobId, byte[] fileData,string fileName)
        {
            try
            {
                using (DataAccess dataAccess = new DataAccess())
                {
                    if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                        return "You have an invalid JobId or you are calling it from an invalid IP address";
                }

                poy.AddFile(jobId, fileData, fileName);
                return "Success";
              
            }
            catch(Exception ex)
            {
                return "Error: "+ex.Message+ex.StackTrace ;

            }
        }

        [WebMethod(Description = @"An alternative to the Add File method that uses a simple string as parameter  instead of binary data.")]
        public string AddTextFile(int jobId, string fileData,string fileName)
        {
            try
            {
                using (DataAccess dataAccess = new DataAccess())
                {
                    if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                        return "You have an invalid JobId or you are calling it from an invalid IP address";
                }

                poy.AddTextFile(jobId, fileData, fileName);
                return "Success";
                
            }
            catch(Exception ex)
            {
                return "Error: " + ex.Message + ex.StackTrace;
            }
        }

        /// <summary>
        /// The 'SubmitSmallPoy' method start a job on the super computer 
        /// that start the POY job. When running a job on a super computer 
        /// you have to request a what resource you need like how many 
        /// processors, how many nodes, how much memory and how long it 
        /// will take. If you ask for a lot of resources your job could 
        /// get scheduled for a future date instead of running immediately. 
        /// So it would not be suitable for  realtime-ish apps. On the other 
        /// hand if you request to little resources your job might not finish. 
        /// Also we could extend the web service by adding new method that 
        /// are similar to this one but only use other back-end Linux tools. 
        /// These methods could take advantage of all of the existing infrastructure. 
        /// </summary>
        /// <param name="jobId">The results of the init method</param>
        /// <param name="numberOfNodes">The mumber of nodes the super computer will use</param>
        /// <param name="wallTime">the total amount of time the job is allowed to run</param>
        /// <returns>Whether the method succeeded or failed.</returns>
        [WebMethod(Description = @"   The 'SubmitPoy' method start a job on the super computer 
         that start the POY job. When running a job on a super computer 
         you have to request a what resource you need like how many 
         processors, how many nodes, how much memory and how long it 
         will take. If you ask for a lot of resources your job could 
         get scheduled for a future date instead of running immediately. 
         So it would not be suitable for  realtime-ish apps. On the other 
         hand if you request to little resources your job might not finish. 
         Also we could extend the web service by adding new method that 
         are similar to this one but only use other back-end Linux tools. 
         These methods could take advantage of all of the existing infrastructure. " )]
        public string SubmitPoy(int jobId, int numberOfNodes,int wallTimeHours, int wallTimeMinutes)
        {
            try
            {
				string output;
                using (DataAccess dataAccess = new DataAccess())
                {
                    if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                        return "You have an invalid JobId or you are calling it from an invalid IP address";

					output = poy.Submit(jobId, numberOfNodes, wallTimeHours, wallTimeMinutes);
					if(output=="Success")
                    	dataAccess.updateNodeMinutes(jobId, numberOfNodes * (wallTimeMinutes + (wallTimeHours * 60)));
                }

                return output;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message + ex.StackTrace;
            }
        }

        [WebMethod(Description = @"  The client must periodically poll the service after it submits its job so it knows when its job is done. 
        The 'IsDoneYet' method does just that and returns true if the job is done and false if the job is not 
        done yet. ")]
        public bool IsDoneYet(int jobId)
        {

            using (DataAccess dataAccess = new DataAccess())
            {
                if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                    return true;
            }

            return poy.isDone(jobId); 
        }

        [WebMethod(Description = @"The last method 'GetFile' basically is used to retrieve the output files specified in the POY script. It is called once per file. ")]
        public byte[] GetFile(int jobId, string fileName)
        {
            using (DataAccess dataAccess = new DataAccess())
            {
                if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                    return null;
            }

            return poy.getFile(jobId,fileName);
        }
    }
}