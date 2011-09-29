using System;
using Whisper;
using Whisper.Keys;
using System.IO;

namespace Wcp
{
	public class Keys
	{
		public static void Main(string[] args, KeyStorage keyStorage)
		{
			Console.WriteLine("Keys");

			if (args.Length == 1)
			{
				Console.WriteLine();
				Console.WriteLine("Public keys:");
				foreach (PublicKey pub in keyStorage.PublicKeys)
					Console.WriteLine("	" + pub);
				Console.WriteLine();
				Console.WriteLine("Private keys:");
				foreach (PrivateKey priv in keyStorage.PrivateKeys)
					Console.WriteLine("	" + priv.ToString());
				Console.WriteLine();

				return;
			}

			if (args.Length == 3 && args[1] == "create")
			{
				PrivateKey key = new PrivateKey(true);
				string name = Path.GetFileName(args[2]);
				keyStorage.Add(name, key);

				Console.WriteLine();
				Console.WriteLine("New key: " + name);
				Console.WriteLine(key.ToString());
				Console.WriteLine();

				return;
			}

			throw new HelpException("Missing arguments");
		}
	}
}

