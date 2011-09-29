using System;
using System.IO;
using System.Collections.Generic;
using Whisper;
using Whisper.Storing;
using Whisper.Chunks;
using Whisper.Messaging;
using Whisper.Keys;

namespace Test
{
	public class Receiver
	{
		Storage storage;
		string target;
		KeyStorage keyStorage;

		public Receiver(Storage storage, string target, KeyStorage keyStorage)
		{
			this.storage = storage;
			this.target = target;
			this.keyStorage = keyStorage;
		}

		public void Run()
		{
			//Find message
			ICollection<ChunkHash> messages = storage.GetMessageList();
			EncryptedStorage es = new EncryptedStorage(storage, keyStorage);

			foreach (ChunkHash mid in messages)
			{
				Chunk chunk = es.ReadChunk(mid);
				Message message = Message.FromChunk(chunk, keyStorage);
				if (message == null)
				{
					Console.WriteLine("Missing key: " + mid);
					continue;
				}

				TreeMessage tm = message as TreeMessage;
				if (tm != null)
				{
					Console.WriteLine("Found TreeMessage " + tm.Name);
					string targetPath = Path.Combine(target, tm.Name);
					TreeChunk.Extract(es, tm.TreeChunkID, targetPath);
					continue;
				}

				RouteMessage rm = message as RouteMessage;
				if (rm != null)
				{
					Console.WriteLine("Found RouteMessage to " + rm.To);

					//Prepare new storage
					Storage remoteStorage = new DiskStorage(rm.To);

					//Send chunks
					foreach (byte[] chunkHash in rm.Chunks)
					{
						Chunk b = storage.ReadChunk(ChunkHash.FromHashBytes(chunkHash));
						remoteStorage.WriteChunk(b);
					}

					//Send message
					remoteStorage.StoreMessage(ChunkHash.FromHashBytes(rm.MessageChunkHash));
					continue;
				}

				Console.WriteLine("Unhandled message type: " + message.GetType().Name);
			}

		}

	}
}

