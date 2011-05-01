using System;
namespace Whisper.Blobs
{
	/// <summary>
	/// Encrypted key used to decrypt a blob
	/// </summary>
	public class BlobKey
	{
		/// <summary>
		/// Encrypted AES key
		/// </summary>
		public byte[] bytes;
	}
}

