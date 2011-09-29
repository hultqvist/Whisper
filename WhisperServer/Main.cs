using System;
using System.Runtime.InteropServices;
using Whisper.Storing;
using System.IO;

namespace WhisperServer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.Error.WriteLine("Hello from remote Whisper Server");

			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WhisperStorage");
			DiskStorage storage = new DiskStorage(path);
			Stream stdin = Console.OpenStandardInput();
			Stream stdout = Console.OpenStandardOutput();
			//stdout = new FileStream("ServerOut", FileMode.Create);
			PipeServer s = new PipeServer(stdin, stdout, storage);
			s.Run();

			Console.Error.WriteLine("Bye bye!");
		}
	}
}

