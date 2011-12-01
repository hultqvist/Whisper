using System;
using Whisper;
using Whisper.Keys;
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
			
			string storagePath = args [1];
			
			//Storage
			Repo storage = Repo.Create (storagePath);
			
			//Sender and Recipient keys
			PublicKey recipientKey = null;
			if (recipientName != null) {
				recipientKey = keyStorage.GetPublic (recipientName);
				EncryptedRepo es = new EncryptedRepo (storage, keyStorage);
				es.AddKey (recipientKey);
				storage = es;
			}
			
			Chunk c = new Chunk (buffer);
			
			storage.WriteChunk (c);
			
			Chunk c2 = storage.ReadChunk (c.ChunkHash);
			
			for (int n = 0; n < buffer.Length; n++)
				if (buffer [n] != c2.Data [n])
					throw new InvalidDataException ("Failed at byte " + n);
			
			Console.WriteLine ("Test succeded");
		}
	}
}

