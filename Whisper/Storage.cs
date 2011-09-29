using System;
using System.Collections.Generic;
using Whisper.Storing;
using Whisper.Chunks;
using Whisper.Messaging;

namespace Whisper
{
	public abstract class Storage
	{
		#region Static Helpers

		public static Storage Create(string name)
		{
			if (name.StartsWith("ssh://"))
				return new PipeStorage("ssh", name.Substring(6) + " mono WhisperServer.exe");

			if (name.StartsWith("pipe:"))
			{
				int space = name.IndexOf(' ');
				if (space < 0)
					return new PipeStorage(name.Substring(5), "");

				return new PipeStorage(name.Substring(5, space - 5), name.Substring(space + 1));
			}
			return new DiskStorage(name);
		}

		#endregion

		#region ChunkHash from CustomID

		/// <summary>
		/// Find out if there already exist a ChunkHash given a CustomID
		/// </summary>
		public abstract ChunkHash GetCustomHash(CustomID customID);

		#endregion

		#region Chunk Data

		public abstract Chunk ReadChunk(ChunkHash chunkHash);

		/// <summary>
		/// Put chunk data in storage.
		/// </summary>
		public abstract void WriteChunk(Chunk chunk);

		#endregion

		#region Messages

		/// <summary>
		/// Get a list of all available messages
		/// </summary>
		public abstract List<ChunkHash> GetMessageList();

		/// <summary>
		/// Put message ChunkHash in special message list
		/// </summary>
		public abstract void StoreMessage(ChunkHash chunkHash);

		#endregion

	}
}

