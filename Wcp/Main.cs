using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Whisper;
using Whisper.Messages;
using Whisper.Encryption;

namespace Wcp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try {
#if DEBUG_SERVER
				ParseCommand(new string[]{ "tcp", "Repo/" });
#elif DEBUG
				if (args.Length == 0)
				{
					//string repoString = "Repo/";
					//ParseCommand(new string[]{ "test", repoString, "Bob"});

					//ParseCommand(new string[]{ "put", "Source/", "tcp:", "Bob" });
					ParseCommand(new string[]{ "list", "tcp:" });
					ParseCommand(new string[]{ "get", "tcp:", "C6", "Target/"});
				}
#endif
#if !DEBUG
				ParseCommand (args);
#endif
			} catch (HelpException he) {
				Console.Error.WriteLine ("Error: {0}", he.Message);
				Console.WriteLine (@"
Usage: wcf.exe <command> [...]
Where command is:
	put <source directory> <repo path> <recipient name>
	list <repo path>
	get <repo path> <message id> <target directory>
	keys
	keys generate <name>
");
			}
		}
		
		public static void ParseCommand (string[] args)
		{
			if (args.Length == 0)
				throw new HelpException ("Missing command argument");

			KeyStorage keyStorage = KeyStorage.Default;

			switch (args [0].ToLower ()) {
			case "list":
				List.Main (args, keyStorage);
				break;
			case "put":
				Put.Main (args, keyStorage);
				break;
			case "get":
				Get.Main (args, keyStorage);
				break;
			case "keys":
				Keys.Main (args, keyStorage);
				break;
			//Serve a single repo via the pipe
			//Usually via a remote ssh connection
			case "pipe":
				Repo pr = Repo.Create (args [1]);
				PipeServer.Run (pr);
				break;
			//Listen to localhost and serve a single repo
			//Used for debugging
			case "tcp":
				Repo tr = Repo.Create (args [1]);
				TcpServer.Run (tr);
				break;
			case "test":
				Test.Main (args, keyStorage);
				break;
			default:
				throw new HelpException ("Unknown command: " + args [0]);
			}
		}
	}
}
