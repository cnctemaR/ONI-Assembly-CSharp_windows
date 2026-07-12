using System;
using System.Diagnostics;

namespace System.Threading.Tasks.Sources
{
	internal static class ManualResetValueTaskSourceCoreShared
	{
		[StackTraceHidden]
		internal static void ThrowInvalidOperationException()
		{
			throw new InvalidOperationException();
		}

		private static void CompletionSentinel(object _)
		{
			ManualResetValueTaskSourceCoreShared.ThrowInvalidOperationException();
		}

		internal static readonly Action<object> s_sentinel = new Action<object>(ManualResetValueTaskSourceCoreShared.CompletionSentinel);
	}
}
