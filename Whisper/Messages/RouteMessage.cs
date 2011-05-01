using System;
using System.Collections.Generic;
using System.IO;
using Whisper.Blobs;

namespace Whisper.Messages
{
	public class RouteMessage : SignedMessage
	{
		/// <summary>
		/// What to send
		/// </summary>
		public BlobHash Message { get; set; }

		/// <summary>
		/// Where to send it
		/// </summary>
		public string To { get; set; }

		/// <summary>
		/// All the blobs going with that message
		/// </summary>
		public BlobHash[] Blobs { get; set; }

		public RouteMessage()
		{
		}

		public RouteMessage(string to, BlobHash message, BlobHash[] blobs)
		{
			this.To = to;
			this.Message = message;
			this.Blobs = blobs;
		}

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			WriteString(writer, To);
			Message.WriteBlob(writer);
			writer.Write((int) Blobs.Length);
			foreach (BlobHash hash in Blobs)
			{
				hash.WriteBlob(writer);
			}
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			To = ReadString(reader);
			Message = BlobHash.FromBlob(reader);
			Blobs = new BlobHash[reader.ReadInt32()];
			for (int n = 0; n < Blobs.Length; n++)
			{
				Blobs[n] = BlobHash.FromBlob(reader);
			}
		}
		
		#endregion

	}
}

