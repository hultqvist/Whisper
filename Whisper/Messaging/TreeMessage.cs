using System;
using System.IO;
using Whisper.Chunks;
namespace Whisper.Messaging
{
	public class TreeMessage : SignedMessage
	{
		/// <summary>
		/// Suggested directory name
		/// </summary>
		public string Name { get; set; }
		public TrippleID TreeID { get; set; }

		public TreeMessage()
		{
		}

		public TreeMessage(TrippleID tree, string name)
		{
			this.TreeID = tree;
			this.Name = name;
		}

		#region Blob Reader/Writer

		internal override void WriteChunk(BinaryWriter writer)
		{
			TreeID.WriteChunk(writer);
			WriteString(writer, Name);
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			TreeID = TrippleID.FromBlob(reader);
			Name = ReadString(reader);
		}

		#endregion

	}
}

