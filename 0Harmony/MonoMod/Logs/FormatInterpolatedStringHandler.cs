using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Logs
{
	[NullableContext(1)]
	[Nullable(0)]
	[InterpolatedStringHandler]
	internal ref struct FormatInterpolatedStringHandler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FormatInterpolatedStringHandler(int literalLen, int formattedCount)
		{
			bool flag;
			this.handler = new DebugLogInterpolatedStringHandler(literalLen, formattedCount, true, false, out flag);
		}

		public override string ToString()
		{
			return this.handler.ToString();
		}

		public string ToStringAndClear()
		{
			return this.handler.ToStringAndClear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLiteral(string s)
		{
			this.handler.AppendLiteral(s);
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(string s)
		{
			this.handler.AppendFormatted(s);
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(string s, int alignment = 0, string format = null)
		{
			this.handler.AppendFormatted(s, alignment, format);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(global::System.ReadOnlySpan<char> s)
		{
			this.handler.AppendFormatted(s);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(global::System.ReadOnlySpan<char> s, int alignment = 0, [Nullable(2)] string format = null)
		{
			this.handler.AppendFormatted(s, alignment, format);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<[Nullable(2)] T>(T value)
		{
			this.handler.AppendFormatted<T>(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<[Nullable(2)] T>(T value, int alignment)
		{
			this.handler.AppendFormatted<T>(value, alignment);
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<T>([Nullable(1)] T value, string format)
		{
			this.handler.AppendFormatted<T>(value, format);
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
		{
			this.handler.AppendFormatted<T>(value, alignment, format);
		}

		private DebugLogInterpolatedStringHandler handler;
	}
}
