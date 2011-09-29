using System;

namespace Whisper.Keys
{
	public class NamedPrivateKey : PrivateKey
	{
		public string Name { get; set; }

		public override string ToString()
		{
			return "(private) " + Name + " " + base.ToString();
		}
	}
}

