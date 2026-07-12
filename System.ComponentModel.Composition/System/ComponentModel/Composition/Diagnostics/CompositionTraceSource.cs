using System;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Diagnostics
{
	internal static class CompositionTraceSource
	{
		public static bool CanWriteInformation
		{
			get
			{
				return CompositionTraceSource.Source.CanWriteInformation;
			}
		}

		public static bool CanWriteWarning
		{
			get
			{
				return CompositionTraceSource.Source.CanWriteWarning;
			}
		}

		public static bool CanWriteError
		{
			get
			{
				return CompositionTraceSource.Source.CanWriteError;
			}
		}

		public static void WriteInformation(CompositionTraceId traceId, string format, params object[] arguments)
		{
			CompositionTraceSource.EnsureEnabled(CompositionTraceSource.CanWriteInformation);
			CompositionTraceSource.Source.WriteInformation(traceId, format, arguments);
		}

		public static void WriteWarning(CompositionTraceId traceId, string format, params object[] arguments)
		{
			CompositionTraceSource.EnsureEnabled(CompositionTraceSource.CanWriteWarning);
			CompositionTraceSource.Source.WriteWarning(traceId, format, arguments);
		}

		public static void WriteError(CompositionTraceId traceId, string format, params object[] arguments)
		{
			CompositionTraceSource.EnsureEnabled(CompositionTraceSource.CanWriteError);
			CompositionTraceSource.Source.WriteError(traceId, format, arguments);
		}

		private static void EnsureEnabled(bool condition)
		{
			Assumes.IsTrue(condition, "To avoid unnecessary work when a trace level has not been enabled, check CanWriteXXX before calling this method.");
		}

		private static readonly DebuggerTraceWriter Source = new DebuggerTraceWriter();
	}
}
