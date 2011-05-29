using System;
using System.IO;
using Whisper;

namespace WhisperServer
{
	public class Server
	{
		readonly BinaryReader reader;
		readonly BinaryWriter writer;
		readonly Storage storage;

		public Server(Stream sin, Stream sout, Storage storage)
		{
			this.reader = new BinaryReader(sin);
			this.writer = new BinaryWriter(sout);
			this.storage = storage;
		}

		public void Run()
		{
			try
			{
				while (true)
				{
					throw new System.NotImplementedException();
				}
			} catch (Exception ex)
			{
				Console.Error.WriteLine("========================================");
				Console.Error.WriteLine("Exception Caught: " + ex.GetType().Name);
				Console.Error.WriteLine(ex.Message);
				Console.Error.WriteLine();
				Console.Error.WriteLine(ex.StackTrace);
				Console.Error.WriteLine("========================================");
			}
		}
	}
}

