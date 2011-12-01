using System;
using System.IO;
using System.Collections.Generic;
using Whisper;
using Whisper.Repos;
using Whisper.Messages;
using Whisper.Keys;

namespace Wcp
{
	/// <summary>
	/// List all available messages in a storage
	/// </summary>
	public static class List
	{
		public static void Main (string[] args, KeyStorage keyStorage)
		{
			if (args.Length != 2)
				throw new HelpException ("Missing arguments");
			
			//Storage
			Repo storage = Repo.Create (args [1]);
			
			//Find message
			ICollection<ChunkHash > messages = storage.GetMessageList ();
			EncryptedRepo es = new EncryptedRepo (storage, keyStorage);
			
			//Iterate over all messages
			foreach (ChunkHash mid in messages) {
				Console.Write (mid.ToString ().Substring (0, 10) + "... ");
				
				Message message = Message.FromChunk (es.ReadChunk (mid), keyStorage);
				
				//No key found
				if (message == null) {
					Console.WriteLine ("no key");
					continue;
				}
				
				if (message.Signature != null)
					Console.Write ("(signed by " + message.Signature.Name + ") ");
				
				TreeMessage tm = message as TreeMessage;
				if (tm != null) {
					Console.WriteLine ("TreeMessage " + tm.Name);
					continue;
				}
				
				RouteMessage rm = message as RouteMessage;
				if (rm != null) {
					Console.WriteLine ("RouteMessage to " + rm.To);
					continue;
				}
				
				Console.WriteLine ("unknown message type: " + message.GetType ().Name);
			}
		}
	}
}

