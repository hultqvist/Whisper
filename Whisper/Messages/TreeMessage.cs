using System;
using System.IO;
using Whisper.Chunks;

namespace Whisper.Messages
{
	public partial class TreeMessage : Message
	{
		public TreeMessage (ChunkHash tree, string name)
		{
			this.TreeChunkHash = tree.bytes;
			this.Name = name;
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		private TreeMessage ()
		{
		}
	}
}

