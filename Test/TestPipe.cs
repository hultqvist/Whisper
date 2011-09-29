using System;
using Whisper;
using Whisper.Storing;
using System.IO;
using Whisper.Keys;
using WhisperServer;
using System.Threading;
using Whisper.Messaging;
using System.Net.Sockets;
using System.Net;

namespace Test
{
	/// <summary>
	/// Test using the pipe interface
	/// </summary>
	public class TestPipe
	{
		public static void Run()
		{
			string sourcePath = "Source/";
			string receipientName = "Bob";
			
			//Source
			if (Directory.Exists(sourcePath) == false)
				throw new FileNotFoundException("Source directory not found", sourcePath);
			
			//Storage
			PipeStorage storage = PrepareStorage();
			
			//Sender and Recipient keys
			PrivateKey senderKey = KeyStorage.Default.DefaultKey;
			PublicKey recipientKey = KeyStorage.Default.GetPublic(receipientName);
			
			//Send Tree
			Console.Write("Generating Tree...");
			Tree tree = new Tree();
			tree.SourcePath = sourcePath;
			tree.TargetName = Path.GetDirectoryName(sourcePath);
			tree.Storage.Add(storage);
			//tree.EncryptKeys.Add(recipientKey);
			//tree.SigningKey = senderKey;
			ChunkHash treeMessage = tree.Generate();
			Console.WriteLine("done");
			
			//Store Message ID
			storage.StoreMessage(treeMessage);
			
			//RouteMessage
			RouteMessage rm = new RouteMessage();
			rm.MessageChunkHash = treeMessage.bytes;
			foreach (ChunkHash ch in tree.ChunkList)
				rm.Chunks.Add(ch.bytes);
			rm.To = receipientName;
			//Store unencrypted RouteMessage
			Whisper.Chunks.Chunk rmChunk = Message.ToChunk(rm); //, senderKey
			storage.WriteChunk(rmChunk);
			storage.StoreMessage(rmChunk.DataHash);
			Console.WriteLine("RouteMessage Stored");
			storage.Dispose();
			Console.WriteLine("Storing Test Complete");
		}

		private static PipeStorage PrepareStorage()
		{
			TcpClient server;
			TcpListener listener = new TcpListener(IPAddress.Loopback, 12345);
			TcpClient client = new TcpClient();
			listener.Start();

			//"Remote" Server
			DiskStorage remoteStorage = new DiskStorage("Remote/");
			Thread t = new Thread(() => {
				server = listener.AcceptTcpClient();
				Stream s = server.GetStream();
				PipeServer ps = new PipeServer(s, s, remoteStorage);
				Console.WriteLine("Got incoming connection.");
				try
				{
					ps.Run();
				}
				catch (IOException ioe)
				{
					Console.WriteLine(ioe.Message);
				}
			});
			t.Name = "ServerSide";
			t.Start();

			Thread.Sleep(500);
			client.Connect(IPAddress.Loopback, 12345);
			Stream c = client.GetStream();
			return new PipeStorage(c, c);
		}
	}
}

