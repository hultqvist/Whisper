using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Whisper.Encryption
{
	public abstract class KeyStorage
	{
		static KeyStorage _default = null;
		/// <summary>
		/// Default key storage, currently unencrypted on disk
		/// </summary>
		public static KeyStorage Default {
			get {
				if (_default == null)
					_default = new DiskKeyStorage();
				return _default;
			}
		}

		public abstract void Add(string name, IKey key);

		public virtual PrivateKey DefaultKey { get; set; }

		public abstract IEnumerable<PublicKey> PublicKeys  { get; }

		public abstract IEnumerable<PrivateKey> PrivateKeys { get; }

		public abstract PublicKey GetPublic(string name);
		public abstract PrivateKey GetPrivate(string name);
	}
}

