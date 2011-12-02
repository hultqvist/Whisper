using System;
using Whisper;
using Whisper.Encryption;
using Whisper.Chunks;
using Whisper.Repos;
using System.IO;
using Whisper.Messages;

namespace Wcp
{

	public class Test
	{
		/// <summary>
		/// Test reading and writing to repo
		/// </summary>
		public static void Main (string[] args, KeyStorage keyStorage)
		{
			//Test buffer to transmit
			byte[] buffer = new byte[4096];
			Random r = new Random ();
			r.NextBytes (buffer);
			
			string recipientName = null;
			
			if (args.Length == 3)
				recipientName = args [2];
			if (args.Length > 3 || args.Length < 2)
				throw new HelpException ("Missing arguments");
			
			string repoPath = args [1];
			
			//Repository
			Repo repo = Repo.Create (repoPath);
			
			//Sender and Recipient keys
			PublicKey recipientKey = null;
			if (recipientName != null) {
				recipientKey = keyStorage.GetPublic (recipientName);
				EncryptedRepo es = new EncryptedRepo (repo, keyStorage);
				es.AddKey (recipientKey);
				repo = es;
			}
			
			Chunk c = new Chunk (buffer);
			
			repo.WriteChunk (c);
			
			Chunk c2 = repo.ReadChunk (c.ChunkHash);
			
			for (int n = 0; n < buffer.Length; n++)
				if (buffer [n] != c2.Data [n])
					throw new InvalidDataException ("Failed at byte " + n);
			
			Console.WriteLine ("Test succeded");
		}
	}
}

