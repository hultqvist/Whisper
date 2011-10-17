using System;

namespace Whisper.Watcher.Config
{
	public class FolderConfig
	{
		public string StoragePath { get; set; }
		/// <summary>
		/// If null don't send any changes
		/// </summary>
		public string Recipipent { get; set; }
		/// <summary>
		/// If null don't accept any incoming
		/// </summary>
		public string AcceptFrom { get; set; }
	}
}

