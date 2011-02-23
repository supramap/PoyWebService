using System;
using PoyService;
namespace UnitTests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try{
				Console.WriteLine ("Hello World!");
				Poy poy = new Poy();
				poy.getTextFile(1222951586,"sam_copy.poy_output");
				Console.WriteLine ("Success");
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.ToString());
			}
		
			Console.ReadLine();
			
		}
	}
}

