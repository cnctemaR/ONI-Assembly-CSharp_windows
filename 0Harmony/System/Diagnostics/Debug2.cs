using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics
{
	internal static class Debug2
	{
		[Conditional("DEBUG")]
		public static void Assert([<b37590d4-39fb-478a-88de-d293f3364852>DoesNotReturnIf(false)] bool condition)
		{
		}
	}
}
