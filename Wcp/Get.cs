using System;
using System.Collections.Generic;
using System.IO;
using Whisper;
using Whisper.Blobing;
using Whisper.Messaging;
using Whisper.Storing;

namespace Wcp
{
	public static class Get
	{
		public static void Main (string[] args)
		{
			if (args.Length != 4)
				throw new HelpException ("Missing arguments");
			
			//Storage
			Storage storage = new Whisper.Storing.DiskStorage (args[1]);
			KeyStorage keyStorage = KeyStorage.Default;
			storage = new EncryptedStorage (storage, keyStorage, new RecipientID (keyStorage.DefaultKey));
			
			//Find message
			Blob blob = null;
			if (args[2].Length == 64)
			{
				BlobHash id = new BlobHash (Hash.FromString (args[2]));
				blob = storage.ReadBlob (id);
			}
			else
			{
				ICollection<BlobHash> messages = storage.GetMessageList ();
				foreach (BlobHash bh in messages) {
					if (bh.ToString ().StartsWith (args[2]))
						blob = storage.ReadBlob (bh);
				}
			}
			
			Message message = Message.FromBlob (blob, keyStorage);
			if (message == null) {
				Console.Error.WriteLine ("Message not found");
				return;
			}
			TreeMessage tm = message as TreeMessage;
			if (tm == null) {
				Console.Error.WriteLine ("Not a TreeMessage: " + message.GetType ().Name);
				return;
			}
			
			Console.WriteLine ("Found TreeMessage " + tm.Name);
			string targetPath = Path.Combine (args[3], tm.Name);
			TreeBlob.Extract (storage, tm.TreeID, targetPath);
		}
		
	}
}

