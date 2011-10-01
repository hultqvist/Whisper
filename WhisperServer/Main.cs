using System;
using System.Runtime.InteropServices;
using Whisper.Storages;
using System.IO;
using Whisper;

namespace WhisperServer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length == 0 || args.Length > 2) {
				PrintHelp ();
				return;
			}
			
			string path = Path.GetFullPath (args [0]);
			DiskStorage storage = new DiskStorage (path);
			
			bool tcp = false;
			if (args.Length == 2) {
				if (args [1] == "--tcp") {
					tcp = true;
				} else {
					PrintHelp ();
					return;
				}
			}
			
			if (tcp)
				TcpServer.Run (storage);
			else
				RunPipeServer (storage);
		}
		
		private static void RunPipeServer (Storage storage)
		{
			Stream stdin = Console.OpenStandardInput ();
			Stream stdout = Console.OpenStandardOutput ();
			PipeServer s = new PipeServer (stdin, stdout, storage);
			s.Run ();
		}
		
		private static void PrintHelp ()
		{
			Console.Error.WriteLine ("Usage: WhisperServer <storage path> [--tcp]");
			Console.Error.WriteLine ("	--tcp	Listen to localhost:" + PipeStorage.DefaultTcpPort);
			Console.Error.WriteLine ("	By default the server listens to stdin.");
		}
	}
}

