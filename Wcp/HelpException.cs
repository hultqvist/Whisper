using System;

namespace Wcp
{
	/// <summary>
	/// Trigger to show help message
	/// </summary>
	public class HelpException : Exception
	{
		public HelpException (string message) : base(message)
		{
		}
	}
}

