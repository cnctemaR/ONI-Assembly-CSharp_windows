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
				global::Debug.LogError(msg, null);
				break;
			case DebugLog.Level.Warning:
				global::Debug.LogWarning(msg, null);
				break;
			case DebugLog.Level.Info:
				global::Debug.Log(msg, null);
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
