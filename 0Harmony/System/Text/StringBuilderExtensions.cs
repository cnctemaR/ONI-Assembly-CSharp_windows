using System;
using System.Runtime.CompilerServices;

namespace System.Text
{
	internal static class StringBuilderExtensions
	{
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder Clear(this StringBuilder builder)
		{
			ThrowHelper.ThrowIfArgumentNull(builder, "builder", null);
			return builder.Clear();
		}
	}
}
