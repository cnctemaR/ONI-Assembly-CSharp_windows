using System;
using System.Runtime.CompilerServices;

namespace System
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class StringExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Replace(this string self, string oldValue, string newValue, StringComparison comparison)
		{
			ThrowHelper.ThrowIfArgumentNull(self, ExceptionArgument.self);
			ThrowHelper.ThrowIfArgumentNull(oldValue, ExceptionArgument.oldValue);
			ThrowHelper.ThrowIfArgumentNull(newValue, ExceptionArgument.newValue);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(oldValue.Length, 0);
			global::System.ReadOnlySpan<char> readOnlySpan = self.AsSpan();
			global::System.ReadOnlySpan<char> readOnlySpan2 = oldValue.AsSpan();
			for (;;)
			{
				int num = readOnlySpan.IndexOf(readOnlySpan2, comparison);
				if (num < 0)
				{
					break;
				}
				defaultInterpolatedStringHandler.AppendFormatted(readOnlySpan.Slice(0, num));
				defaultInterpolatedStringHandler.AppendLiteral(newValue);
				readOnlySpan = readOnlySpan.Slice(num + readOnlySpan2.Length);
			}
			defaultInterpolatedStringHandler.AppendFormatted(readOnlySpan);
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Contains(this string self, string value, StringComparison comparison)
		{
			ThrowHelper.ThrowIfArgumentNull(self, ExceptionArgument.self);
			ThrowHelper.ThrowIfArgumentNull(value, ExceptionArgument.value);
			return self.IndexOf(value, comparison) >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Contains(this string self, char value, StringComparison comparison)
		{
			ThrowHelper.ThrowIfArgumentNull(self, ExceptionArgument.self);
			return self.IndexOf(value, comparison) >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetHashCode(this string self, StringComparison comparison)
		{
			ThrowHelper.ThrowIfArgumentNull(self, ExceptionArgument.self);
			return StringComparerEx.FromComparison(comparison).GetHashCode(self);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf(this string self, char value, StringComparison comparison)
		{
			ThrowHelper.ThrowIfArgumentNull(self, ExceptionArgument.self);
			return self.IndexOf(new string(value, 1), comparison);
		}
	}
}
