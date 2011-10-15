using System;
using System.Collections.Generic;
using Whisper.Storages;
using Whisper.Chunks;
using Whisper.Messages;
using System.Net.Sockets;
using System.Net;

namespace Whisper
{
	public abstract class Storage
	{
		#region Static Helpers

		public static Storage Create (string name)
		{
			if (name.StartsWith ("ssh://")) {
				int pathsep = name.IndexOf ("/", 6);
				if (pathsep < 0)
					throw new ArgumentException ("Missing target path");
				string host = name.Substring (6, pathsep - 6);
				string path = name.Substring (pathsep + 1);
				return new PipeStorage ("ssh", host + " wcp.exe " + path);
			}

			if (name == "tcp:") {
				TcpClient tcp = new TcpClient();
				tcp.Connect(IPAddress.Loopback, PipeStorage.DefaultTcpPort);
				NetworkStream s = tcp.GetStream();
				return new PipeStorage (s, s);
			}

			if (name.StartsWith ("pipe:")) {
				int space = name.IndexOf (' ');
				if (space < 0)
					return new PipeStorage (name.Substring (5), "");

				return new PipeStorage (name.Substring (5, space - 5), name.Substring (space + 1));
			}
			return new DiskStorage (name);
		}

		#endregion

		#region ChunkHash from CustomID

		/// <summary>
		/// Find out if there already exist a ChunkHash given a CustomID
		/// </summary>
		public abstract ChunkHash GetCustomHash (CustomID customID);

		#endregion

		#region Chunk Data

		public abstract Chunk ReadChunk (ChunkHash chunkHash);

		/// <summary>
		/// Put chunk data in storage.
		/// </summary>
		public abstract void WriteChunk (Chunk chunk);

		#endregion

		#region Messages

		/// <summary>
		/// Get a list of all available messages
		/// </summary>
		public abstract List<ChunkHash> GetMessageList ();

		/// <summary>
		/// Put message ChunkHash in special message list
		/// </summary>
		public abstract void StoreMessage (ChunkHash chunkHash);

		#endregion

	}
}

