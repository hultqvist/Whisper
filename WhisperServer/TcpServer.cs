using System;
using System.Net.Sockets;
using System.Net;
using Whisper.Storages;
using Whisper;
using System.Threading;

namespace WhisperServer
{
	public static class TcpServer
	{
		public static void Run (Storage storage)
		{
			TcpListener listener = new TcpListener (IPAddress.Loopback, PipeStorage.DefaultTcpPort);
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

