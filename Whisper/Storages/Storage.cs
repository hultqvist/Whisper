using System;
using Whisper.ID;
using Whisper.Blobs;
using System.Collections.Generic;
using Whisper.Messages;

namespace Whisper.Storages
{
	public abstract class Storage
	{
		#region BlobHash from CustomID

		/// <summary>
		/// Find out if there already exist a BlobHash given a CustomID
		/// </summary>
		public abstract BlobHash GetBlobHash(CustomID customID);

		#endregion

		#region Blob Data

		public abstract Blob ReadBlob(BlobHash blobHash);

		/// <summary>
		/// Put blob data in storage.
		/// </summary>
		public abstract void WriteBlob(Blob blob);

		#endregion

		#region Messages

		/// <summary>
		/// Get a list of all available messages
		/// </summary>
		public abstract List<BlobHash> GetMessageList();

		/// <summary>
		/// Put message BlobHash in special message list
		/// </summary>
		public abstract void StoreMessage(BlobHash blobHash);

		#endregion

	}
}

