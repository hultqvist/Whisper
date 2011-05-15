using System;
using System.IO;
using Whisper.Blobing;
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

		internal override void WriteBlob(BinaryWriter writer)
		{
			TreeID.WriteBlob(writer);
			WriteString(writer, Name);
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			TreeID = TrippleID.FromBlob(reader);
			Name = ReadString(reader);
		}

		#endregion

	}
}

