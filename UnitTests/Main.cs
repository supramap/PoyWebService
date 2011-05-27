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
				Poy poy = new Poy(SuperComputer.GetGlenn());
				//poy.getTextFile(1222951586,"sam_copy.poy_output");
				poy.getCompressedFile(1173033225,"igor_3.poy_output","tar.gz");
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