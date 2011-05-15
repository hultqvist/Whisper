using System;
using System.IO;
namespace Whisper
{
	public class KeyStorage
	{
		readonly string path;

		public KeyStorage()
		{
			path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Whisper");
			
		}

		public PrivateKey DefaultKey()
		{
			return GetPrivate("Default");
		}

		public PublicKey GetPublic(string name)
		{
			PublicKey pk = new PublicKey();
			ReadKey(pk, name);
			return pk;
		}

		public PrivateKey GetPrivate(string name)
		{
			PrivateKey pk = new PrivateKey();
			ReadKey(pk, name);
			return pk;
		}

		private void ReadKey(Key key, string name)
		{
			string keyPath = Path.Combine(this.path, name + ".key");
			using (BinaryReader br = new BinaryReader(new FileStream(keyPath, FileMode.Open)))
			{
				key.ReadBlob(br);
			}
		}
	}
}

