using System;
using System.Diagnostics;

namespace KSerialization
{
	internal static class DebugLog
	{
		[Conditional("DEBUG_LOG")]
		public static void Output(DebugLog.Level msg_level, string msg)
		{
			if (msg_level > DebugLog.Level.Error)
			{
				return;
			}
			switch (msg_level)
			{
			case DebugLog.Level.Error:
				global::Output.LogError(new object[] { msg });
				break;
			case DebugLog.Level.Warning:
				global::Output.LogWarning(new object[] { msg });
				break;
			case DebugLog.Level.Info:
				global::Output.Log(new object[] { msg });
				break;
			}
		}

		private const DebugLog.Level OutputLevel = DebugLog.Level.Error;

		public enum Level
		{
			Error,
			Warning,
			Info
		}
	}
}
