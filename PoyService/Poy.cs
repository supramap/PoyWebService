using System;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using System.IO;
using System.Text;

namespace PoyService
{
    /// <summary>
    
    /// </summary>
    public class Poy
    {
        public static readonly string tempDir;
        public SuperComputer superComputer;
		
		public string HostName {get{return superComputer.HostName;}}
		public string UserName {get{return superComputer.UserName;}}
        public string Password {get{return superComputer.Password;}}
		public string PoyPath  {get{return superComputer.PoyPath;}}
		public string DataPath {get{return superComputer.DataPath;}}
		
        static Poy()
        {
            if (System.IO.Path.DirectorySeparatorChar == '\\')
                tempDir = @"C:\temp\";
            else tempDir = @"/var/tmp/";
        }

		public Poy(SuperComputer sc)
        {
			superComputer=sc;
           	
        }
		
        //System.Random randomNumbeGenerator = new System.Random();

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
			sftp.Put(dir,string.Format("{2}{1}/{0}", filename, jobId.ToString(),DataPath));
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
            sftp.Put(dir,string.Format("{2}{1}/{0}", filename, jobId.ToString(),DataPath));
            
            File.Delete(dir);
            return true;
        }
		
		public bool AddZipFile(int jobId, byte[] fileData, string filename)
        {
            if(filename.Contains("/")) return false; //fail for securety
            //if (filename.Contains(' ')) return false; //fail for securety

            string dir = string.Format(@"{0}{1}.zip", tempDir, filename);
            Sftp sftp = new Sftp(HostName, UserName,Password);
            sftp.Connect();

            File.WriteAllBytes(dir, (byte[]) fileData);
            
            sftp.Put(dir,DataPath+'/'+jobId+'/');

            SshStream shell = getShell();
            shell.Write(string.Format("unzip {2}{1}/{0}.zip", filename, jobId.ToString(),DataPath));
			shell.Write(string.Format("rm {2}{1}/{0}.zip", filename, jobId.ToString(),DataPath));
            File.Delete(dir);
            return true;
        }

        public string Submit(int jobId, int numberOfNodes, int wallTimeHours, int wallTimeMinutes, string postBackURL)
        {
            if (wallTimeMinutes >= 60) return "Minutes must be less then 60";
            if (wallTimeHours >= 99) return "Hours must be less then 99";
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
				
				
				if(superComputer.HostName=="glenn.osc.edu")
                	writer.Write(string.Format("#PBS -l nodes={0}:ppn=12 \n",numberOfNodes));
				else if(superComputer.HostName=="superdev.bmi.ohio-state.edu") 
					writer.Write("#PBS -l nodes=5:ppn=8+4:ppn=4  \n" );
				else
					writer.Write(string.Format("#PBS -l nodes={0} \n",numberOfNodes));
				
                writer.Write("#PBS -N poy_" + jobId.ToString() + " \n");
                writer.Write("#PBS -j oe \n");
                //writer.Write("#PBS –o output.txt \n");
                //writer.Write("#PBS -S /bin/ksh \n");
				if(superComputer.HostName!="superdev.bmi.ohio-state.edu") 
                	writer.Write("PATH="+PoyPath+":$PATH \n");
                writer.Write(string.Format("cd {0}{1}/ \n", DataPath,jobId));
                writer.Write("echo the start time is `date`\n"); 
				if(superComputer.HostName=="superdev.bmi.ohio-state.edu") 
					//writer.Write(string.Format("mpirun -np $NUM_PROC -machinefile $PBS_NODEFILE --mca btl tcp,self --mca btl_tcp_if_include eth0 {0}poy -plugin {0}supramap.cmxs *.poy \n",superComputer.PluginPath));
					writer.Write("mpirun -np 56 -machinefile $PBS_NODEFILE --mca btl tcp,self --mca btl_tcp_if_include eth0 poy_mpi -plugin /usr/local/poy-4.1.2/bin/supramap_mpi.cmxs *.poy \n");

				else if (superComputer.HostName == "br0.renci.org")
                    writer.Write(string.Format("mpiexec {0}poy -plugin {1}supramap.cmxs *.poy \n", superComputer.PoyPath, superComputer.PluginPath));
                else
                	writer.Write(string.Format("mpiexec {0}poy -plugin {0}supramap.cmxs *.poy \n",superComputer.PluginPath));
				writer.Write("wget "+postBackURL+ " \n");
                writer.Write(" echo the end time is `date`\n");
            }

            sftp.Put(dir);
            sftp.Close();
			//I could not Ftp to put the files in the right directory so I put them in the base and moved them with a ssh command
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
		
		public bool isDone(int jobId,string command)
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
                commander.RunCommand(string.Format(@"cp {1}{0}/{2}_{0}.* {1}{0}/output.txt", jobId,DataPath,command));
                return true;
            }
            else
                return false;
        }
		
		public string SubmitGenPhen(int jobId,string jobName,string treeName, string postBackURL)
        {

            Sftp sftp = new Sftp(HostName, UserName, Password);
            sftp.Connect();

            string dir = string.Format(@"{0}GenPhenBatch.job", tempDir);

            using (StreamWriter writer = new StreamWriter(dir, false))
            {
                
                writer.Write("#PBS -l walltime=03:00:00 \n#PBS -l nodes=1:ppn=1 \n");
                writer.Write("#PBS -N GenPhen_" + jobId.ToString() + " \n#PBS -j oe \n");
                writer.Write("#PBS -S /bin/ksh \n");
				writer.Write("PATH=$PATH:"+PoyPath+" \n");
                writer.Write(string.Format("cd {0}{1}/ \n", DataPath,jobId));
                writer.Write("echo the start time is `date`\n"); 
				
				writer.Write("mpiexec add_arbitrary_weights.awk "+treeName+" >temp_tree.tre\n");
				writer.Write("cat temp_tree.tre "+jobName+".poy_output | mpiexec add_tree.pl >"+jobName+"_parsed.txt\n");
				writer.Write("mpiexec reweight_tree.awk "+jobName+"_parsed.txt "+jobName+"_parsed.txt > "+jobName+"_rwt.txt \n");
				
				writer.Write("mpiexec divisiderum_postparse_totaldown.pl root  "+jobName+"_rwt.txt > "+jobName+"_down.txt \n");
				writer.Write("mpiexec apomorphy_andtable_test_statistic_cox.pl "+jobName+"_rwt.txt  "+jobName+"_down.txt > "+jobName+"_stat.txt\n");
				
				writer.Write("awk '($1 != $2 && ($3+$4) > 3 && $3 > $5 && ($3-$10)/sqrt($10) >=sqrt(6)){print;}' "+jobName+"_stat.txt > "+jobName+"_stat_p0.05.txt\n");
				writer.Write("awk '($1 != $2 && ($3+$4) > 3 && $3 > $5 && ($3-$10)/sqrt($10) >=sqrt(19)){print;}' "+jobName+"_stat.txt > "+jobName+"_stat_p0.001.txt\n");
				writer.Write("awk '($1 != $2 && ($3+$4) > 3 && $3 > $5 && ($3-$10)/sqrt($10) >=sqrt(200)){print;}' "+jobName+"_stat.txt > "+jobName+"_stat_p0.0001.txt\n");
 				
				writer.Write("wget "+postBackURL+ " \n");
                
                writer.Write(" echo the end time is `date`\n");
            }

            sftp.Put(dir);
            sftp.Close();
			//I could not Ftp to put the files in the right directory so I put them in the base and moved them with a ssh command
            SshStream shell = getShell();
            shell.Write(string.Format("mv {0} {2}{1}/{0} ;", "GenPhenBatch.job", jobId.ToString(),DataPath));
            shell.Write(string.Format("cd {1}{0} ;",  jobId.ToString(),DataPath));
            
			//echo `qstat | awk '/supramap/ {print $1}'`
			
			//shell.Write(string.Format("qsub -W depend=afterok:{0} GenPhenBatch.job",pbs_id));
			
			//please excuese the uglness of haveing code nested in code nested in code.
			//awk is nest in bash that is nested in C#
			shell.Write(string.Format("qsub -W depend=afterany:`qstat | awk '/poy_{0}/ {{split($1,array,\".\");print array[1]}}'` GenPhenBatch.job",jobId.ToString()));
            File.Delete(dir);
            
            //sftp.Get("poy_test.o4281361",@"C:\temp\poy_test.o4281361");
            //FileStream fileStream = File.OpenRead(@"C:\temp\poy_test.o4281361");
            //byte[] filedata = new byte[(int)fileStream.Length];
            //fileStream.Read(filedata,0,(int)fileStream.Length);

            return "Success"; 
        }
		
		public bool Delete(int jobId)
		{
			SshStream shell = getShell();
            shell.Write(string.Format("cd {1}{0} ;",  jobId.ToString(),DataPath));
			shell.Write(string.Format("rm * ;"));
			shell.Write(string.Format("cd \\.. ;",  jobId.ToString(),DataPath));
			shell.Write(string.Format("rmdir {0} ;", jobId.ToString()));
            //shell.Write("qdel batch.job");
			
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
			File.Delete(string.Format(@"{0}{1}", tempDir, fileName));
            return filedata;
        }
		
		public byte[] getCompressedFile(int jobId, string fileName,string compressionType )
        {
            if (fileName.Contains("/")) return null; //fail for securety
            //if (fileName.Contains(' ')) return null; //fail for securety

            Sftp sftp = getSftp();
			SshStream shell = getShell();
			
			switch (compressionType) {
			case "zip" :
				shell.Write(string.Format(@"cd poy_service/{1} ; zip {0}.zip {0}", fileName, jobId ));
				break;
			case "tar.gz" :
				shell.Write(string.Format(@"cd poy_service/{1} ; tar -czvf {0}.temp.tar.gz {0};mv {0}.temp.tar.gz {0}.tar.gz ", fileName, jobId ));
				break;
			default: 
				break;
				//return null ;
			}
			
			
			//Thread.Sleep(5000);
			for(int i = 0 ; i<30 ; i++)
			{
				Thread.Sleep(2000);
				if(sftp.GetFileList(string.Format(@"poy_service/{0}/", jobId)).Contains(fileName+"."+compressionType))  i=30;
				
			}
			
			File.AppendAllText(tempDir+"log.txt",jobId.ToString()+": fileName found\n");
			string vmFileName = string.Format(@"{0}{1}.{2}", tempDir, fileName,compressionType);
			
			if(File.Exists(vmFileName))
		    {
            	File.Delete(vmFileName);
				File.AppendAllText(tempDir+"log.txt",jobId.ToString()+": deleted old version on vm "+vmFileName+"\n");
			}
			
            sftp.Get(
                string.Format(@"poy_service/{0}/{1}.{2}",  jobId,fileName,compressionType),
                vmFileName 
                );
			
			File.AppendAllText(tempDir+"log.txt",jobId.ToString()+": got file\n");
			shell.Write(string.Format(@"rm {0}.{2}", fileName, jobId, compressionType));
			
			File.AppendAllText(tempDir+"log.txt",jobId.ToString()+": deleted old version on glenn\n");
			
            FileStream fileStream = File.OpenRead(vmFileName);
            byte[] filedata = new byte[(int)fileStream.Length];
            fileStream.Read(filedata, 0, (int)fileStream.Length);
            return filedata;
        }
		
		/*public byte[] getZipFile(int jobId, string fileName)
        {
            if (fileName.Contains("/")) return null; //fail for securety
            //if (fileName.Contains(' ')) return null; //fail for securety

            Sftp sftp = getSftp();
			SshStream shell = getShell();
			
			shell.Write(string.Format(@"cd poy_service/{1} ; zip {0}.zip {0}", fileName, jobId ));
			
			for(int i = 0 ; i<30 ; i++)
			{
				Thread.Sleep(2000);
				if(sftp.GetFileList(string.Format(@"poy_service/{0}/", jobId)).Contains(fileName+".zip"))  i=30;
				
			}
			string vmFileName = string.Format(@"{0}{1}.zip", tempDir, fileName);
			
			if(File.Exists(vmFileName))
		    {
            	File.Delete(vmFileName);
			}
			
            sftp.Get(
                string.Format(@"poy_service/{1}/{0}.zip", fileName, jobId),
                vmFileName 
                );
			shell.Write(string.Format(@"rm {0}.zip", fileName, jobId ));
			
			
			
            FileStream fileStream = File.OpenRead(vmFileName);
            byte[] filedata = new byte[(int)fileStream.Length];
            fileStream.Read(filedata, 0, (int)fileStream.Length);
			//File.Delete(string.Format(@"{0}{1}", tempDir, fileName));
            return filedata;
        }*/
		
		public string getTextFile(int jobId, string fileName)
        {
            if (fileName.Contains("/")) return null; //fail for securety
            //if (fileName.Contains(' ')) return null; //fail for securety

            Sftp sftp = new Sftp(HostName, UserName, Password);
            sftp.Connect();

            sftp.Get(
                //string.Format(@"poy_service/{1}/{0}", fileName, jobId, DataPath),
                string.Format(@"{0}{1}/{2}", DataPath, jobId, fileName),
                string.Format(@"{0}{1}",tempDir, fileName)
                );
		   string output = File.ReadAllText(string.Format(@"{0}{1}", tempDir, fileName));
		   File.Delete(string.Format(@"{0}{1}", tempDir, fileName));
           return output;
        }
    }
}