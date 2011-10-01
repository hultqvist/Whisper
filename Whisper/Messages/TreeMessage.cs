using System;
using System.IO;
using Whisper.Chunks;
namespace Whisper.Messages
{
	public partial class TreeMessage : Message
	{
		public TreeMessage(Chunk tree, string name)
		{
			this.TreeChunkID = new TrippleID(tree);
			this.Name = name;
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		private TreeMessage()
		{}
	}
}

