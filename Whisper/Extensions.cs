using System;
using System.Collections.Generic;

namespace Whisper
{
	public static class Extensions
	{
		/// <summary>
		/// Return the first item in the enumerable;
		/// </summary>
		public static T First<T> (this IEnumerable<T> e)
		{
			foreach (T i in e)
				return i;
			return default(T);
		}
	}
}

