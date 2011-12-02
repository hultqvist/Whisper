using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Whisper;
using Whisper.Repos;
using Whisper.Repos.Pipe;
using Whisper.Chunks;
using ProtocolBuffers;

namespace Wcp
{
	public class PipeServer
	{
		readonly Stream input;
		readonly Stream output;
		readonly Repo localRepo;

		public PipeServer (Stream sin, Stream sout, Repo localRepo)
		{
			this.input = sin;
			this.output = sout;
			this.localRepo = localRepo;
		}

#if DEBUG_SERVER_PROFILING
		TimeSpan waiting = new TimeSpan();
		TimeSpan processing = new TimeSpan();
		DateTime nextReport = DateTime.Now;
#endif

		public static void Run (Repo localRepo)
		{
			Stream stdin = Console.OpenStandardInput ();
			Stream stdout = Console.OpenStandardOutput ();
			PipeServer s = new PipeServer (stdin, stdout, localRepo);
			s.Run ();
		}

		public void Run ()
		{
			try {
				while (true) {
					ReceiveMessage ();
				}
			} catch (EndOfStreamException) {
				Console.WriteLine ("End of stream");
			}
		}

		public void ReceiveMessage ()
		{
#if DEBUG_SERVER_PROFILING
			if(nextReport < DateTime.Now)
			{
				Console.WriteLine("Waiting: {0}", waiting.TotalSeconds / processing.TotalSeconds);
				nextReport = DateTime.Now.AddSeconds(1);
			}

			DateTime start = DateTime.Now;
#endif
			PipeHeader head;
			try {
				head = PipeHeader.Deserialize (ProtocolParser.ReadBytes (input));
			} catch (IOException ioe) {
				throw new EndOfStreamException ("At next header", ioe);
			}
#if DEBUG_SERVER_PROFILING
			if(waiting.Ticks == 0)
				waiting = new TimeSpan(1);
			else
				waiting = waiting + (DateTime.Now - start);
			start = DateTime.Now;
#endif
			if (head == null) {
				Console.Error.WriteLine ("Null header");
				return;
			}

#if DEBUG
			Console.WriteLine("Header(" + head.TypeID + ", " + head.DebugNumber + ")");
#endif

			switch (head.TypeID) {
			case 1:
				ProcessGetCustomHash ();
				break;
			case 2:
				ProcessReadChunk ();
				break;
			case 3:
				ProcessWriteChunk ();
				break;
			case 4:
				ProcessMessageList ();
				break;
			case 5:
				ProcessStoreMessage ();
				break;
			default:
				throw new InvalidDataException ("Unknown message type");
			}

			output.Flush ();
#if DEBUG_SERVER_PROFILING
			processing = processing + (DateTime.Now - start);
#endif
		}

		void ProcessGetCustomHash ()
		{
			RequestCustomHash request = RequestCustomHash.Deserialize (ProtocolParser.ReadBytes (input));

			ReplyCustomHash reply = new ReplyCustomHash ();
			ChunkHash ch = localRepo.GetCustomHash (CustomID.FromBytes (request.CustomID));
			if (ch == null)
				reply.ChunkHash = null;
			else
				reply.ChunkHash = ch.bytes;
			//Console.Error.WriteLine("PipeServer: Sending: " + reply);
			byte[] rbytes = ReplyCustomHash.SerializeToBytes (reply);
			//Console.Error.WriteLine("PipeServer: Sending: " + rbytes.Length + ", " + BitConverter.ToString(rbytes));
			ProtocolParser.WriteBytes (output, rbytes);
		}

		void ProcessReadChunk ()
		{
			RequestReadChunk request = RequestReadChunk.Deserialize (ProtocolParser.ReadBytes (input));
			ReplyReadChunk reply = new ReplyReadChunk ();
			Chunk c = localRepo.ReadChunk (ChunkHash.FromHashBytes (request.ChunkHash));
			reply.ChunkData = c.Data;

			ProtocolParser.WriteBytes (output, ReplyReadChunk.SerializeToBytes (reply));
		}

		void ProcessWriteChunk ()
		{
			RequestWriteChunk request = RequestWriteChunk.Deserialize (ProtocolParser.ReadBytes (input));
			ReplyWriteChunk reply = new ReplyWriteChunk ();
			Chunk chunk = new Chunk (request.ChunkData);
			localRepo.WriteChunk (chunk);

			ProtocolParser.WriteBytes (output, ReplyWriteChunk.SerializeToBytes (reply));
		}

		void ProcessMessageList ()
		{
			RequestMessageList.Deserialize (ProtocolParser.ReadBytes (input));
			ReplyMessageList reply = new ReplyMessageList ();
			List<ChunkHash > list = localRepo.GetMessageList ();
			foreach (ChunkHash ch in list)
				reply.ChunkHash.Add (ch.bytes);
			ProtocolParser.WriteBytes (output, ReplyMessageList.SerializeToBytes (reply));
		}

		void ProcessStoreMessage ()
		{
			RequestStoreMessage request = RequestStoreMessage.Deserialize (ProtocolParser.ReadBytes (input));
			ReplyStoreMessage reply = new ReplyStoreMessage ();
			localRepo.StoreMessage (ChunkHash.FromHashBytes (request.ChunkHash));
			ProtocolParser.WriteBytes (output, ReplyStoreMessage.SerializeToBytes (reply));
		}
	}
}

