using System;
using System.IO;
using Whisper.Chunks;

namespace Whisper.Messages
{
	public partial class TreeMessage : Message
	{
		public TreeMessage (TrippleID tree, string name)
		{
			this.TreeChunkID = tree;
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

