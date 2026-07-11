using System;

namespace System.Linq
{
	internal sealed class SystemCore_EnumerableDebugViewEmptyException : Exception
	{
		public string Empty
		{
			get
			{
				return "Enumeration yielded no results";
			}
		}
	}
}
