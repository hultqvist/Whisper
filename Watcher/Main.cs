using System;
using System.IO;
using Whisper.Encryption;
using Whisper.Repos;

namespace Whisper.Watcher
{
	/// <summary>
	/// Watching for incoming messages in repo.
	/// Monitoring folders and send changes to predefined repo.
	/// </summary>
	class MainClass
	{
		public static void Main (string[] args)
		{
#if DEBUG
			args = new string[]{"SafeBox", "Repo", "watchTest", "Bob"};
#else
			if (args.Length != 4) {
				Console.WriteLine ("Usage: Watcher.exe <directory path> <repo> <prefix> <recipient key>");
				return;
			}
#endif

			string path = args [0];
			Repo repo = Repo.Create (args [1]);
			string prefix = args [2];
			string recipientName = args [3];

			Directory.CreateDirectory (path);

			KeyStorage keyStorage = KeyStorage.Default;

			PublicKey recipientKey = null;
			if (recipientName != null) {
				recipientKey = keyStorage.GetPublic (recipientName);
				EncryptedRepo er = new EncryptedRepo (repo, keyStorage);
				er.AddKey (recipientKey);
				repo = er;
			}

			DirectoryWatcher dw = new DirectoryWatcher (path, repo, prefix);
			dw.Run ();
		}
	}
}
