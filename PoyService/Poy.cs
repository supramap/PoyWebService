using System;
using System.Collections.Generic;
using System.Web;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using System.IO;
using System.Text;


namespace PoyService
{
    public class Poy
    {
        public static readonly string tempDir;
        
		//public const string HostName = "glenn.osc.edu";
        //public const string UserName = "osu6013";
        //public const string Password = "C114154N"; 
		//public const string PoyPath = "/nfs/17/osu6013/bin:/nfs/17/osu6013/lib";
		//public const string DataPath= "/nfs/17/osu6013/poy_service/";
		
		public const string HostName = "glenn.osc.edu";
		public const string UserName = "supramap";
        public const string Password = "B633077S"; 
		public const string PoyPath = "/nfs/03/supramap/bin:/nfs/03/supramap/lib";
		public const string DataPath= "/nfs/03/supramap/poy_service/";
		
        static Poy()
        {
            if (System.IO.Path.DirectorySeparatorChar == '\\')
                tempDir = @"C:\temp\";
            else tempDir = @"/var/tmp/";
        }

        System.Random randomNumbeGenerator = new System.Random();
        //Glenn glenn = new Glenn();

        public int Init(int dir)
        {
            SshStream ssh = getShell();
            ssh.Write("mkdir "+DataPath+dir.ToString());
            return dir;
        }

		public bool AddTextFile(int jobId, string fileData, string filename)
        {
            if(filename.Contains("/")) return false; //fail for securety
            //if (filename.Contains(' ')) return false; //fail for securety

            string dir = string.Format(@"{0}{1}", tempDir, filename);
            Sftp sftp = new Sftp(HostName, UserName,Password);
            sftp.Connect();
            File.WriteAllText(dir,fileData);
            sftp.Put(dir);
            SshStream shell = getShell();
            shell.Write(string.Format("mv {0} {2}{1}/{0}", filename, jobId.ToString(),DataPath));
           
            
            File.Delete(dir);
            return true;
        }
		
        public bool AddFile(int jobId, byte[] fileData, string filename)
        {
            if(filename.Contains("/")) return false; //fail for securety
            //if (filename.Contains(' ')) return false; //fail for securety

            string dir = string.Format(@"{0}{1}", tempDir, filename);
            Sftp sftp = new Sftp(HostName, UserName,Password);
            sftp.Connect();

            File.WriteAllBytes(dir, (byte[]) fileData);
            
            sftp.Put(dir);
            SshStream shell = getShell();
            shell.Write(string.Format("mv {0} {2}{1}/{0}", filename, jobId.ToString(),DataPath));
           
            
            File.Delete(dir);
            return true;
        }

        public string Submit(int jobId, int numberOfNodes, int wallTimeHours, int wallTimeMinutes)
        {
            if (wallTimeMinutes >= 60) return "Minutes must be less then 60";
            if (wallTimeHours >= 3) return "Hours must be less then 10";
            if (numberOfNodes >= 101) return "Number of Nodes must be less then 101";
            if (numberOfNodes < 1) return "you need at least 1 node";
            if (wallTimeHours < 0 || wallTimeMinutes < 0) return "We currently don't support time travel at this time";

            Sftp sftp = new Sftp(HostName, UserName, Password);
            sftp.Connect();

            string dir = string.Format(@"{0}batch.job", tempDir);

            using (StreamWriter writer = new StreamWriter(dir, false))
            {
                writer.Write(string.Format("#PBS -l walltime={0}:{1}:00 \n",
                    wallTimeHours.ToString().PadLeft(2,'0'),
                    wallTimeMinutes.ToString().PadLeft(2,'0')
                     
                    ));
                writer.Write(string.Format("#PBS -l nodes={0}:ppn=4:olddual \n",numberOfNodes));
                writer.Write("#PBS -N poy_" + jobId.ToString() + " \n");
                writer.Write("#PBS -j oe \n");
                //writer.Write("#PBS –o output.txt \n");
                writer.Write("#PBS -S /bin/ksh \n");
                writer.Write("PATH=$PATH:"+PoyPath+" \n");
                writer.Write(string.Format("cd {0}{1}/ \n", DataPath,jobId));
                writer.Write("echo the start time is `date`\n"); 
                writer.Write("mpiexec poy -plugin ~/bin/supramap.cmxs *.poy \n");
                writer.Write(" echo the end time is `date`\n");
            }

            sftp.Put(dir);
            sftp.Close();
            SshStream shell = getShell();
            shell.Write(string.Format("mv {0} {2}{1}/{0} ;", "batch.job", jobId.ToString(),DataPath));
            shell.Write(string.Format("cd {1}{0} ;",  jobId.ToString(),DataPath));
            shell.Write("qsub batch.job");

            File.Delete(dir);
            
            //sftp.Get("poy_test.o4281361",@"C:\temp\poy_test.o4281361");
            //FileStream fileStream = File.OpenRead(@"C:\temp\poy_test.o4281361");
            //byte[] filedata = new byte[(int)fileStream.Length];
            //fileStream.Read(filedata,0,(int)fileStream.Length);

            return "Success"; 
        }

        public bool isDone(int jobId)
        {
             //SshStream ssh = getShell();
             //ssh.Write(string.Format(@"[ -f ~/poy_service/{0}/poy_{0}.* ] && echo 'yes' || echo 'no'", jobId));
             //string responce = ssh.ReadResponse();
             //return responce == "yes";

            SshExec commander = getCommander();
            string output="";
            string error="";
            commander.RunCommand(
                string.Format(@"[ -f {1}{0}/poy_{0}.* ] && echo 'yes' || echo 'no'", jobId,DataPath),
                ref output,
                ref error);

            if (output == "yes\n")
            {
                commander.RunCommand(string.Format(@"cp {1}{0}/poy_{0}.* {1}{0}/output.txt", jobId,DataPath));
                return true;
            }
            else
                return false;
        }
		
		public bool Delete(int jobId)
		{
			SshStream shell = getShell();
            shell.Write(string.Format("cd {1}{0} ;",  jobId.ToString(),DataPath));
			shell.Write(string.Format("rm * ;"));
			shell.Write(string.Format("cd \\.. ;",  jobId.ToString(),DataPath));
			shell.Write(string.Format("rmdir {0} ;", jobId.ToString()));
            shell.Write("qsub batch.job");
			
			return true;
		}

        public byte[] getFile(int jobId, string fileName)
        {
            if (fileName.Contains("/")) return null; //fail for securety
            //if (fileName.Contains(' ')) return null; //fail for securety

            Sftp sftp = new Sftp(HostName, UserName, Password);
            sftp.Connect();

            sftp.Get(
                string.Format(@"poy_service/{1}/{0}", fileName, jobId,DataPath),
                string.Format(@"{0}{1}",tempDir, fileName)
                );

            FileStream fileStream = File.OpenRead(string.Format(@"{0}{1}", tempDir, fileName));
            byte[] filedata = new byte[(int)fileStream.Length];
            fileStream.Read(filedata, 0, (int)fileStream.Length);

            return filedata;
        }
		
		public string getTextFile(int jobId, string fileName)
        {
            if (fileName.Contains("/")) return null; //fail for securety
            //if (fileName.Contains(' ')) return null; //fail for securety

            Sftp sftp = new Sftp(HostName, UserName, Password);
            sftp.Connect();

            sftp.Get(
                string.Format(@"poy_service/{1}/{0}", fileName, jobId,DataPath),
                string.Format(@"{0}{1}",tempDir, fileName)
                );
			
           return File.ReadAllText(string.Format(@"{0}{1}", tempDir, fileName));
        }

        private SshStream getShell()
        {
            SshStream ssh = new SshStream(HostName, UserName, Password);
            ssh.Prompt = "#";
            ssh.RemoveTerminalEmulationCharacters = true;
            return ssh;
        }

        private SshExec getCommander()
        {
            //using Tamir.SharpSsh;
            Tamir.SharpSsh.SshExec e = new SshExec(HostName, UserName, Password);
            e.Connect();
            return e;
        }

        private Sftp getSftp()
        {
            Sftp sftp = new Sftp(HostName, UserName, Password);
            sftp.Connect();
            return sftp;
        }
    }
}