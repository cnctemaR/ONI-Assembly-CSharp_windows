using System;

namespace System.Runtime.CompilerServices
{
	public static class RuntimeFeature
	{
		public static bool IsSupported(string feature)
		{
			return feature == "PortablePdb";
		}

		public const string PortablePdb = "PortablePdb";
	}
}
