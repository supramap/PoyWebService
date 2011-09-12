using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.IO;

namespace PoyService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "supramap", Description = "(Note: do not use space in any of the file names)")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class PoyService : System.Web.Services.WebService
    {
        //public Poy poy = new Poy();
		 public Poy Glenn = new Poy(SuperComputer.GetGlenn());
		 public Poy Superdev = new Poy( SuperComputer.GetDansClustor());

        [WebMethod(Description = @"The 'Init' method basically creates a directory on the super computer it returns a token that must be 
          used on all subsequent method calls as the jobId parameter.Note this token will only work with the ipaddress that was used to 
          call this method. Note it is not enforced but it highly encouraged to use this method over https.
          to get a pass prase to use this web service please vist http://glenn-webservice.bmi.ohio-state.edu/Application.aspx
          The current to option for resource are Glenn and supradev")]
        
        public int Init(string passPhase,string resource)
        {
            int Token;
            using (DataAccess dataAccess = new DataAccess())
            {
                Token = dataAccess.getToken(passPhase, HttpContext.Current.Request.UserHostAddress,resource);
            }
            if (Token == -1) return -1;
            
            SPHash.getValue(Token).Init(Token);
            return Token;
        }

		[WebMethod(Description = @"This method removes all files on the super computer for this job")]
		public bool DeleteJob(int jobId)
		{
			using (DataAccess dataAccess = new DataAccess())
            {
                if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                    return true;
				
				dataAccess.Inactivate(jobId); 
				
            }

            return SPHash.getValue(jobId).Delete(jobId); 
		}
		
        [WebMethod(Description = @"The 'AddFile' method is used to basically put a file on the super 
         computer. It should be called once to a load a .poy script. Plus 
         it should be called for each file that the POY script references. 
         It is the responsibility of the downstream client developer to 
         generate the POY scripts this places more work in there hands but 
         also give them great flexibility to design web apps that can 
         use POY for anything.
		Note: there is a bug and the method won't work if there is a space in any of the file names
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

                SPHash.getValue(jobId).AddFile(jobId, fileData, fileName);
                return "Success";
              
            }
            catch(Exception ex)
            {
                return "Error: "+ex.Message+ex.StackTrace ;

            }
        }

        [WebMethod(Description = @"The AddTextFile method is an alternative to the Add File command. This function allows the permissioned user to add a file as a simple string instead of using binary data in the fileData field.")]
        public string AddTextFile(int jobId, string fileData,string fileName)
        {
            try
            {
                using (DataAccess dataAccess = new DataAccess())
                {
                    if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                        return "You have an invalid JobId or you are calling it from an invalid IP address";
                }

                SPHash.getValue(jobId).AddTextFile(jobId, fileData, fileName);
                return "Success";
                
            }
            catch(Exception ex)
            {
                return "Error: " + ex.Message + ex.StackTrace;
            }
        }

        [WebMethod(Description = @"The GetFile method is utilized to retrieve the output files specified in the Poy script. This function is called once per file ")]
        public byte[] GetFile(int jobId, string fileName)
        {
            using (DataAccess dataAccess = new DataAccess())
            {
                if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                    return null;	
            }
            return SPHash.getValue(jobId).getFile(jobId,fileName);
        }
		
		[WebMethod(Description = @"The GetZipedFile is a second method that can be used to retrieve an output file. This command allows the user to compress the file prior to retrieving it. ")]
        public byte[] GetZipedFile(int jobId, string fileName,string compressionType )
        {
            using (DataAccess dataAccess = new DataAccess())
            {
                if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                    return null;
				
				
            }

            return SPHash.getValue(jobId).getCompressedFile(jobId,fileName,compressionType);
        }
		
		  [WebMethod(Description = @"The basically 'GetFile' but returns a string instead of binary data should only be used on text files")]
        public string GetTextFile(int jobId, string fileName)
        {
            using (DataAccess dataAccess = new DataAccess())
            {
                if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                    return null;
            }

            return SPHash.getValue(jobId).getTextFile(jobId,fileName);
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
        public string SubmitPoy(int jobId, int numberOfNodes,int wallTimeHours, int wallTimeMinutes, string postBackURL)
        {
            try
            {
				string output;
                using (DataAccess dataAccess = new DataAccess())
                {
                    if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                        return "You have an invalid JobId or you are calling it from an invalid IP address";

					output = SPHash.getValue(jobId).Submit(jobId, numberOfNodes, wallTimeHours, wallTimeMinutes,postBackURL);
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

      
		
		[WebMethod]
		public string SubmitGenPhen(int jobId, string jobName,string treeName,string postBackURL)
        {
            try
            {
				string output;
                using (DataAccess dataAccess = new DataAccess())
                {
                    if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                        return "You have an invalid JobId or you are calling it from an invalid IP address";

					output = SPHash.getValue(jobId).SubmitGenPhen(jobId, jobName,treeName,postBackURL);
					//if(output=="Success")
                    	//dataAccess.updateNodeMinutes(jobId, numberOfNodes * (wallTimeMinutes + (wallTimeHours * 60)));
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
        done yet. The command value can ether be 'poy' or 'GenPhen' ")]
        public bool IsDoneYet(int jobId, string command)
        {
            using (DataAccess dataAccess = new DataAccess())
            {
                if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
                    return true;
            }

            return SPHash.getValue(jobId).isDone(jobId,command); 
        }
		
//		   [WebMethod]
//        public bool IsGenPhenDoneYet(int jobId)
//        {
//            using (DataAccess dataAccess = new DataAccess())
//            {
//                if (!dataAccess.ValidateToken(jobId, HttpContext.Current.Request.UserHostAddress))
//                    return true;
//            }
//
//            return SPHash.getValue(jobId).isGenPhenDone(jobId); 
//        }
		
    }
}