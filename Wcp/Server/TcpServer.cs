using System;
using System.Net.Sockets;
using System.Net;
using Whisper.Repos;
using Whisper;
using System.Threading;

namespace Wcp
{
	public static class TcpServer
	{
		public static void Run (Repo storage)
		{
			TcpListener listener = new TcpListener (IPAddress.Loopback, PipeRepo.DefaultTcpPort);
			listener.Start ();
			Console.WriteLine ("Listening on " + listener.LocalEndpoint);
			
			while (true) {
				TcpClient c = listener.AcceptTcpClient ();
				Console.WriteLine ("Incoming from " + c.Client.RemoteEndPoint);

				NetworkStream s = c.GetStream ();
				PipeServer p = new PipeServer (s, s, storage);
				Thread t = new Thread (p.Run);
				t.Start ();
			}
		}
	}
}

