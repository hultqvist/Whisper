using System;
using System.IO;
using Whisper.Blobs;
namespace Whisper.Messages
{
	public class TreeMessage : SignedMessage
	{
		/// <summary>
		/// Suggested directory name
		/// </summary>
		public string Name { get; set; }
		public ClearID TreeID { get; set; }

		public TreeMessage()
		{
		}

		public TreeMessage(ClearID tree, string name)
		{
			this.TreeID = tree;
			this.Name = name;
		}

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			TreeID.WriteBlob(writer);
			WriteString(writer, Name);
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			TreeID = ClearID.FromBlob(reader);
			Name = ReadString(reader);
		}

		#endregion

	}
}

