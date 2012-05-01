using System;
using System.Collections;
using System.Collections.Generic;
namespace PoyService
{
	public class SPHash
	{
		private static IDictionary<int,Poy> hash = new Dictionary<int, Poy>();
		private static Poy Glenn = new Poy(SuperComputer.GetGlenn());
		private static Poy Superdev = new Poy( SuperComputer.GetDansClustor());
		private static Poy Oakley = new Poy( SuperComputer.GetOakley());
		
		static SPHash()
		{
			
		}
		
		public static Poy getValue(int token)
		{
			if(!hash.ContainsKey( token))
			{
				DataAccess access = new DataAccess();
				
				switch (access.getResourceIdForToken( token).ToLower()) {
					
				case "oakley": 
				{
					hash.Add(token,Glenn);
					break;
				}	
				case "glenn": 
				{
					hash.Add(token,Glenn);
					break;
				}
				case "supradev": 
				{
					hash.Add(token,Superdev);
					break;
				}
				default:
				break;
				}
			

			}
			return hash[token];
		}
	}
}

