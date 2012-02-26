using System;
using System.Collections.Generic;
using Whisper.Repos;
using Whisper.Chunks;
using Whisper.Messages;
using System.Net.Sockets;
using System.Net;

namespace Whisper
{
	/// <summary>
	/// Base class for chunk repositories
	/// </summary>
	public abstract class Repo
	{
		#region Static Helpers

		/// <summary>
		/// Create repo from name/address
		/// </summary>
		public static Repo Create (string name)
		{
			if (name.StartsWith ("ssh://")) {
				int pathsep = name.IndexOf ("/", 6);
				if (pathsep < 0)
					throw new ArgumentException ("Missing target path");
				string host = name.Substring (6, pathsep - 6);
				string path = name.Substring (pathsep + 1);
				return new PipeRepo ("ssh", host + " wcp.exe pipe " + path);
			}

			if (name == "tcp:") {
				TcpClient tcp = new TcpClient ();
				tcp.Connect (IPAddress.Loopback, PipeRepo.DefaultTcpPort);
				NetworkStream s = tcp.GetStream ();
				return new PipeRepo (s, s);
			}

			if (name.StartsWith ("pipe:")) {
				int space = name.IndexOf (' ');
				if (space < 0)
					return new PipeRepo (name.Substring (5), "");

				return new PipeRepo (name.Substring (5, space - 5), name.Substring (space + 1));
			}
			return new DiskRepo (name);
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
		/// Put chunk data in the repo.
		/// Return chunk hash which can have changed if the chunk got encrypted before storing.
		/// </summary>
		public abstract ChunkHash WriteChunk (Chunk chunk);

		#endregion

		#region Messages

		/// <summary>
		/// Get a list of all available messages
		/// </summary>
		public abstract List<ChunkHash> GetMessageList (string prefix);

		/// <summary>
		/// Put message ChunkHash in special message list
		/// </summary>
		/// <param name='prefix'>
		/// A custom namespace for messages, can be used to separate different sources or protocols.
		/// </param>
		/// <param name='chunkHash'>
		/// Hash of the chunk containing the message
		/// </param>
		public abstract void StoreMessage (string prefix, ChunkHash chunkHash);

		#endregion

	}
}

