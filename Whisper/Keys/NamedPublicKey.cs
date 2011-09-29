using System;

namespace Whisper.Keys
{
	public class NamedPublicKey : PublicKey
	{
		public string Name {
			get;
			set;
		}

		public override string ToString()
		{
			return "(public) " + Name + " " + base.ToString();
		}
	}
}

