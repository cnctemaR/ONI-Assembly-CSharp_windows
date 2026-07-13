using System;

namespace UnityEngine.AdaptivePerformance
{
	internal static class APLog
	{
		public static void Debug(string format, params object[] args)
		{
			bool flag = APLog.enabled;
			if (flag)
			{
				UnityEngine.Debug.Log(string.Format("[Adaptive Performance] " + format, args));
			}
		}

		public static bool enabled;
	}
}
