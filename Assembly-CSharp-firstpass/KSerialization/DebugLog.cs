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
			if (msg_level != DebugLog.Level.Info)
			{
				if (msg_level != DebugLog.Level.Warning)
				{
					if (msg_level == DebugLog.Level.Error)
					{
						global::Debug.LogError(msg);
					}
				}
				else
				{
					global::Debug.LogWarning(msg);
				}
			}
			else
			{
				global::Debug.Log(msg);
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
