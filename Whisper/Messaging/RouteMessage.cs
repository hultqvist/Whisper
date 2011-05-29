using System;
using System.Collections.Generic;
using System.IO;
using Whisper.Chunks;

namespace Whisper.Messaging
{
	public class RouteMessage : SignedMessage
	{
		/// <summary>
		/// What to send
		/// </summary>
		public ChunkHash Message { get; set; }

		/// <summary>
		/// Where to send it
		/// </summary>
		public string To { get; set; }

		/// <summary>
		/// All the blobs going with that message
		/// </summary>
		public ICollection<ChunkHash> Chunks { get; set; }

		public RouteMessage()
		{
		}

		public RouteMessage(string to, ChunkHash message, ChunkHash[] chunks)
		{
			this.To = to;
			this.Message = message;
			this.Chunks = chunks;
		}

		#region Blob Reader/Writer

		internal override void WriteChunk(BinaryWriter writer)
		{
			WriteString(writer, To);
			Message.WriteChunk(writer);
			writer.Write((int) Chunks.Count);
			foreach (ChunkHash hash in Chunks)
			{
				hash.WriteChunk(writer);
			}
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			To = ReadString(reader);
			Message = ChunkHash.FromChunk(reader);
			Chunks = new List<ChunkHash>();
			int count = reader.ReadInt32();
			for (int n = 0; n < count; n++)
			{
				Chunks.Add(ChunkHash.FromChunk(reader));
			}
		}
		
		#endregion

	}
}

