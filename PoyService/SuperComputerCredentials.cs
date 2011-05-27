using System;
namespace PoyService
{
	public class SuperComputer
	{
		public string HostName;
		public string UserName;
		public string Password;
		public string PoyPath;
		public string DataPath;
		public string PluginPath;

		public static SuperComputer GetGlenn ()
		{
			return new SuperComputer 
			{ 
				HostName = "glenn.osc.edu", 
				UserName = "supramap", 
				Password = "B633077S", 
				PoyPath = "/nfs/03/supramap/bin:/nfs/03/supramap/lib:/nfs/03/supramap/phengen", 
				DataPath = "/nfs/03/supramap/poy_service/" ,
				PluginPath ="/nfs/03/supramap/bin/"
			};
		}
		
		public static SuperComputer GetDansClustor ()
		{
			return new SuperComputer 
			{ 
				HostName = "superdev.bmi.ohio-state.edu", 
				UserName = "supramap", 
				Password = "B633077S", 
				PoyPath = "/home/supramap/bin:/home/supramap/lib:/home/supramap/phengen", 
				DataPath = "/home/supramap/poy_service/",
				PluginPath = "/home/supramap/bin/"
			};
		}

		public SuperComputer (){}
		
	}
}

