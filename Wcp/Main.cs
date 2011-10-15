using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Whisper;
using Whisper.Messages;
using Whisper.Keys;

namespace Wcp
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			try
			{
#if DEBUG_SERVER
				ParseCommand(new string[]{ "tcp", "Storage/" });
#elif DEBUG
				if (args.Length == 0)
				{
					//string storageString = "Storage/";
					//ParseCommand(new string[]{ "test", storageString, "Bob"});

					//ParseCommand(new string[]{ "put", "Source/", "tcp:", "Bob" });
					ParseCommand(new string[]{ "list", "tcp:" });
					ParseCommand(new string[]{ "get", "tcp:", "C6", "Target/"});
				}
#endif
#if !DEBUG
				ParseCommand(args);
#endif
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
	keys
	keys generate <name>
");
			}
		}
		
		public static void ParseCommand(string[] args)
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
			case "pipe":
				Storage ps = Storage.Create(args[1]);
				PipeServer.Run(ps);
				break;
			case "tcp":
				Storage ts = Storage.Create(args[1]);
				TcpServer.Run(ts);
				break;
			case "test":
				Test.Main(args, keyStorage);
				break;
			default:
				throw new HelpException("Unknown command: " + args[0]);
			}
		}
	}
}
