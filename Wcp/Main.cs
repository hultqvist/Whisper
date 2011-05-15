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
		public static void Main(string[] args)
		{
			try
			{
				if (args.Length == 0)
					throw new HelpException();
				
				if (args[0] == "list")
					List.Main(args);
				if (args[0] == "put")
					Put.Main(args);
				if (args[0] == "get")
					Get.Main(args);
				
				Console.WriteLine("All done");
			} catch (HelpException he)
			{
				Console.Error.WriteLine("Error: {0}", he.Message);
				Console.WriteLine(@"
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
