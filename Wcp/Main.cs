using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Whisper;
using Whisper.Messaging;
using Whisper.Keys;

namespace Wcp
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			try
			{
				if (args.Length == 0)
					throw new HelpException("Missing command argument");

				KeyStorage keyStorage = KeyStorage.Default;

				switch (args[0].ToLower())
				{
				case "list":
					List.Main(args, keyStorage);
					break;
				case "put":
					Put.Main(args, keyStorage);
					break;
				case "get":
					Get.Main(args, keyStorage);
					break;
				case "keys":
					Keys.Main(args, keyStorage);
					break;
				default:
					throw new HelpException("Unknown command: " + args[0]);
				}
				Console.WriteLine("All done");
			}
			catch (HelpException he)
			{
				Console.Error.WriteLine("Error: {0}", he.Message);
				Console.WriteLine(@"
Usage: wcf.exe <command> [...]
Where command is:
	put <source directory> <storage path> <recipient name>
	list <storage path>
	get <storage path> <message id> <target directory>
	keys list
	keys create <name>
");
			}
		}
		
	}
}
