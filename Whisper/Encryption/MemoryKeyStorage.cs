using System;
using System.Collections.Generic;

namespace Whisper.Encryption
{
	/// <summary>
	/// Empty, in memory key storage
	/// </summary>
	public class MemoryKeyStorage : KeyStorage
	{
		public MemoryKeyStorage()
		{
		}

		Dictionary<string, PrivateKey> privateKeys = new Dictionary<string, PrivateKey>();
		Dictionary<string, PublicKey> publicKeys = new Dictionary<string, PublicKey>();

		public override void Add(string name, IKey key)
		{
			if (key is PrivateKey)
			{
				privateKeys.Add(name, (PrivateKey) key);
				if (DefaultKey == null)
					DefaultKey = (PrivateKey) key;
				publicKeys.Add(name, ((PrivateKey) key).PublicKey);
			}
			else
			{
				publicKeys.Add(name, (PublicKey) key);
			}
		}

		public override IEnumerable<PublicKey> PublicKeys {
			get {
				return publicKeys.Values;
			}
		}

		public override IEnumerable<PrivateKey> PrivateKeys {
			get {
				return privateKeys.Values;
			}
		}

		public override PublicKey GetPublic(string name)
		{
			throw new NotImplementedException();
		}

		public override PrivateKey GetPrivate(string name)
		{
			throw new NotImplementedException();
		}
	}
}

