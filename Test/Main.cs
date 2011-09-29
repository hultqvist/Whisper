using System;
using System.IO;
using Whisper;
using Whisper.Storing;
using Whisper.Keys;
using System.Threading;

namespace Test
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Thread.CurrentThread.Name = "Main";
			//Test1.Run();
			TestPipe.Run();
		}

	}
}

