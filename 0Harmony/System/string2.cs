using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class string2
	{
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrEmpty([<b37590d4-39fb-478a-88de-d293f3364852>NotNullWhen(false)] string value)
		{
			return string.IsNullOrEmpty(value);
		}
	}
}
