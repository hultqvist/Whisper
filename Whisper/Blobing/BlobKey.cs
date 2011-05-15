using System;
namespace Whisper.Blobing
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

