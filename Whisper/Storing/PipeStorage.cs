using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Whisper.Chunks;
using Whisper.Storing.Pipe;
using ProtocolBuffers;

namespace Whisper.Storing
{
	/// <summary>
	/// Store via a pipe to another program.
	/// This can be used for direct tcp communication or via ssh
	/// </summary>
	public class PipeStorage : Storage, IDisposable
	{
		Stream input;
		Stream output;
		Process p;

		public PipeStorage(string command, string arguments)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = command;
			psi.Arguments = arguments;
			psi.UseShellExecute = false;
			psi.RedirectStandardInput = true;
			psi.RedirectStandardOutput = true;
			p = Process.Start(psi);

			input = p.StandardOutput.BaseStream;
			output = p.StandardInput.BaseStream;
		}

		public PipeStorage(Stream input, Stream output)
		{
			this.input = input;
			this.output = output;
		}

		public override string ToString()
		{
			if (p == null)
				return "Pipe";
			return p.ProcessName;
		}

		public void Dispose()
		{
			input.Dispose();
			output.Dispose();
			if (p != null)
				p.Dispose();
		}


		/// <summary>
		/// Helper for sending messages
		/// </summary>
		private void SendMessage(IPipeMessage message)
		{
			PipeHeader h = PipeHeader.Next();
			h.TypeID = message.TypeID;
			//Send Header
#if DEBUG
			//Console.WriteLine("PipeStorage: Header(" + h.TypeID + ", " + h.DebugNumber + ")");
#endif
			ProtocolParser.WriteBytes(output, PipeHeader.SerializeToBytes(h));

			//Send Message
#if DEBUG
			//Console.WriteLine("PipeStorage: Message(" + message + ")");
#endif
			byte[] messageBytes = GetMessageBytes(message);
			ProtocolParser.WriteBytes(output, messageBytes);
		}

		static byte[] GetMessageBytes(IPipeMessage message)
		{
			if (message is RequestCustomHash)
				return RequestCustomHash.SerializeToBytes((RequestCustomHash) message);
			if (message is RequestMessageList)
				return RequestMessageList.SerializeToBytes((RequestMessageList) message);
			if (message is RequestReadChunk)
				return RequestReadChunk.SerializeToBytes((RequestReadChunk) message);
			if (message is RequestStoreMessage)
				return RequestStoreMessage.SerializeToBytes((RequestStoreMessage) message);
			if (message is RequestWriteChunk)
				return RequestWriteChunk.SerializeToBytes((RequestWriteChunk) message);
			throw new NotImplementedException();
		}

		public override ChunkHash GetCustomHash(CustomID customID)
		{
			//Slightly optimized :)
			return null;

			RequestCustomHash msg = new RequestCustomHash();
			msg.CustomID = customID.bytes;
			SendMessage(msg);

			byte[] rbytes = ProtocolParser.ReadBytes(input);

			//Console.WriteLine("Got reply " + rbytes.Length);

			ReplyCustomHash reply = ReplyCustomHash.Deserialize(rbytes);

			//Console.WriteLine("Got reply " + reply);

			return ChunkHash.FromHashBytes(reply.ChunkHash);
		}

		public override Chunk ReadChunk(ChunkHash chunkHash)
		{
			RequestReadChunk msg = new RequestReadChunk();
			msg.ChunkHash = chunkHash.bytes;
			SendMessage(msg);

			ReplyReadChunk reply = ReplyReadChunk.Deserialize(ProtocolParser.ReadBytes(input));
			Chunk c = new Chunk(reply.ChunkData);
			c.DataHash = ChunkHash.FromHashBytes(Hash.ComputeHash(c.Data).bytes);
			//Verify Hash
			if (c.DataHash.Equals(chunkHash) == false)
				throw new InvalidDataException("Hash mismatch: " + chunkHash);

			return c;
		}

		public override void WriteChunk(Chunk chunk)
		{
			RequestWriteChunk msg = new RequestWriteChunk();
			msg.ChunkData = chunk.Data;
			SendMessage(msg);

			ReplyWriteChunk.Deserialize(ProtocolParser.ReadBytes(input));
		}

		public override List<ChunkHash> GetMessageList()
		{
			RequestMessageList msg = new RequestMessageList();
			SendMessage(msg);

			ReplyMessageList reply = ReplyMessageList.Deserialize(ProtocolParser.ReadBytes(input));
			List<ChunkHash > list = new List<ChunkHash>();
			foreach (byte[] hash in reply.ChunkHash)
				list.Add(ChunkHash.FromHashBytes(hash));

			return list;
		}

		public override void StoreMessage(ChunkHash chunkHash)
		{
			RequestStoreMessage msg = new RequestStoreMessage();
			msg.ChunkHash = chunkHash.bytes;
			SendMessage(msg);

			ReplyStoreMessage.Deserialize(ProtocolParser.ReadBytes(input));
		}
		
	}
}

