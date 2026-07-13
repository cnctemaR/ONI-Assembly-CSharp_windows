using System;
using System.Runtime.CompilerServices;
using MonoMod.Logs;

namespace MonoMod
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class MMDbgLog
	{
		public static bool IsWritingLog
		{
			get
			{
				return DebugLog.IsWritingLog;
			}
		}

		[ModuleInitializer]
		internal static void LogVersion()
		{
			MMDbgLog.Info("Version 25.0.11");
		}

		public static void Log(LogLevel level, string message)
		{
			DebugLog.Log("MonoMod.Utils", level, message);
		}

		public static void Log(LogLevel level, [InterpolatedStringHandlerArgument("level")] ref DebugLogInterpolatedStringHandler message)
		{
			DebugLog.Log("MonoMod.Utils", level, ref message);
		}

		public static void Spam(string message)
		{
			MMDbgLog.Log(LogLevel.Spam, message);
		}

		public static void Spam(ref MMDbgLog.DebugLogSpamStringHandler message)
		{
			MMDbgLog.Log(LogLevel.Spam, ref message.handler);
		}

		public static void Trace(string message)
		{
			MMDbgLog.Log(LogLevel.Trace, message);
		}

		public static void Trace(ref MMDbgLog.DebugLogTraceStringHandler message)
		{
			MMDbgLog.Log(LogLevel.Trace, ref message.handler);
		}

		public static void Info(string message)
		{
			MMDbgLog.Log(LogLevel.Info, message);
		}

		public static void Info(ref MMDbgLog.DebugLogInfoStringHandler message)
		{
			MMDbgLog.Log(LogLevel.Info, ref message.handler);
		}

		public static void Warning(string message)
		{
			MMDbgLog.Log(LogLevel.Warning, message);
		}

		public static void Warning(ref MMDbgLog.DebugLogWarningStringHandler message)
		{
			MMDbgLog.Log(LogLevel.Warning, ref message.handler);
		}

		public static void Error(string message)
		{
			MMDbgLog.Log(LogLevel.Error, message);
		}

		public static void Error(ref MMDbgLog.DebugLogErrorStringHandler message)
		{
			MMDbgLog.Log(LogLevel.Error, ref message.handler);
		}

		[Nullable(0)]
		[InterpolatedStringHandler]
		internal ref struct DebugLogSpamStringHandler
		{
			public DebugLogSpamStringHandler(int literalLen, int formattedCount, out bool isEnabled)
			{
				this.handler = new DebugLogInterpolatedStringHandler(literalLen, formattedCount, LogLevel.Spam, out isEnabled);
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

			internal DebugLogInterpolatedStringHandler handler;
		}

		[Nullable(0)]
		[InterpolatedStringHandler]
		internal ref struct DebugLogTraceStringHandler
		{
			public DebugLogTraceStringHandler(int literalLen, int formattedCount, out bool isEnabled)
			{
				this.handler = new DebugLogInterpolatedStringHandler(literalLen, formattedCount, LogLevel.Trace, out isEnabled);
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

			internal DebugLogInterpolatedStringHandler handler;
		}

		[Nullable(0)]
		[InterpolatedStringHandler]
		internal ref struct DebugLogInfoStringHandler
		{
			public DebugLogInfoStringHandler(int literalLen, int formattedCount, out bool isEnabled)
			{
				this.handler = new DebugLogInterpolatedStringHandler(literalLen, formattedCount, LogLevel.Info, out isEnabled);
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

			internal DebugLogInterpolatedStringHandler handler;
		}

		[Nullable(0)]
		[InterpolatedStringHandler]
		internal ref struct DebugLogWarningStringHandler
		{
			public DebugLogWarningStringHandler(int literalLen, int formattedCount, out bool isEnabled)
			{
				this.handler = new DebugLogInterpolatedStringHandler(literalLen, formattedCount, LogLevel.Warning, out isEnabled);
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

			internal DebugLogInterpolatedStringHandler handler;
		}

		[Nullable(0)]
		[InterpolatedStringHandler]
		internal ref struct DebugLogErrorStringHandler
		{
			public DebugLogErrorStringHandler(int literalLen, int formattedCount, out bool isEnabled)
			{
				this.handler = new DebugLogInterpolatedStringHandler(literalLen, formattedCount, LogLevel.Error, out isEnabled);
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

			internal DebugLogInterpolatedStringHandler handler;
		}
	}
}
