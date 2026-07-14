using System;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	internal static class SpanExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(1)]
		public static string ToBase64String(this ReadOnlySpan<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
		{
			return Convert.ToBase64String(bytes, options);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(1)]
		public static string ToBase64String(this Span<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
		{
			return bytes.ToBase64String(options);
		}
	}
}
