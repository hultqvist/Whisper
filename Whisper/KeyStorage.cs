using System;
using System.IO;
using System.Collections.Generic;
using Whisper.Keys;

namespace Whisper
{
	public class KeyStorage
	{
		static KeyStorage _default = null;
		/// <summary>
		/// Default key storage, currently unencrypted on disk
		/// </summary>
		public static KeyStorage Default {
			get {
				if (_default == null)
					_default = new DiskKeyStorage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Whisper"));
				return _default;
			}
		}

		/// <summary>
		/// Empty, in memory key storage
		/// </summary>
		public KeyStorage()
		{
		}

		List<PrivateKey> privateKeys = new List<PrivateKey>();
		List<PublicKey> publicKeys = new List<PublicKey>();

		public virtual void Add(PublicKey key)
		{
			if (key is PrivateKey)
			{
				privateKeys.Add((PrivateKey) key);
				if (DefaultKey == null)
					DefaultKey = (PrivateKey) key;
			}
			publicKeys.Add((PublicKey) key);
		}

		public virtual PrivateKey DefaultKey { get; set; }

		public virtual IEnumerable<PublicKey> PublicKeys {
			get { return publicKeys; }
		}

		public virtual IEnumerable<PrivateKey> PrivateKeys {
			get { return privateKeys; }
		}

		public virtual PublicKey FromName(string name)
		{
			throw new NotImplementedException("Keys in memory have no names");
		}
	}
}

