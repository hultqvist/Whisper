using System;
using System.Collections.Generic;
using System.IO;
using Whisper;
using Whisper.Chunks;
using Whisper.Messages;
using Whisper.Storages;
using Whisper.Keys;

namespace Wcp
{
	public static class Get
	{
		public static void Main(string[] args, KeyStorage keyStorage)
		{
			if (args.Length != 4)
				throw new HelpException("Missing arguments");
			
			//Storage
			Storage storage = Storage.Create(args[1]);
			storage = new EncryptedStorage(storage, keyStorage, new RecipientID(keyStorage.DefaultKey.PublicKey));
			
			//Find message
			Chunk chunk = null;
			if (args[2].Length == 64)
			{
				ChunkHash id = ChunkHash.FromHashBytes(Hash.FromString(args[2]).bytes);
				chunk = storage.ReadChunk(id);
			} else
			{
				ICollection<ChunkHash> messages = storage.GetMessageList();
				foreach (ChunkHash bh in messages)
				{
					if (bh.ToString().StartsWith(args[2]))
						chunk = storage.ReadChunk(bh);
				}
			}
			
			Message message = Message.FromChunk(chunk, keyStorage);
			if (message == null)
			{
				Console.Error.WriteLine("Message not found");
				return;
			}
			TreeMessage tm = message as TreeMessage;
			if (tm == null)
			{
				Console.Error.WriteLine("Not a TreeMessage: " + message.GetType().Name);
				return;
			}
			
			Console.WriteLine("Found TreeMessage " + tm.Name);
			string targetPath = Path.Combine(args[3], tm.Name);
			TreeChunk.Extract(storage, tm.TreeChunkID, targetPath);
		}
		
	}
}

