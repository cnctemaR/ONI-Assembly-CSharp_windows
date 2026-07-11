using System;

namespace System.Xml
{
	internal static class Ref
	{
		public static bool Equal(string strA, string strB)
		{
			return strA == strB;
		}

		public new static void Equals(object objA, object objB)
		{
		}
	}
}
