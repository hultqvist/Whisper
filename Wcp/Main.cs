using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Whisper;
using Whisper.Messaging;

namespace Wcp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try {
				if (args.Length == 0)
					throw new HelpException ("Missing command argument");
				
				switch (args[0].ToLower()) {
				case "list":
					List.Main (args);
					break;
				case "put":
					Put.Main (args);
					break;
				case "get":
					Get.Main (args);
					break;
				default:
					throw new HelpException ("Unknown command: " + args[0]);
				}
				Console.WriteLine ("All done");
			} catch (HelpException he) {
				Console.Error.WriteLine ("Error: {0}", he.Message);
				Console.WriteLine (@"
Usage: wcf.exe <command> [...]
Where command is:
	put <source directory> <storage path> <recipient name>
	list <storage path>
	get <storage path> <message id> <target directory>
");
			}
		}
		
	}
}
