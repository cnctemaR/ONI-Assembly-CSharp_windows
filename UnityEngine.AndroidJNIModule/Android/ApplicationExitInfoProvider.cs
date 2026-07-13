using System;

namespace UnityEngine.Android
{
	public static class ApplicationExitInfoProvider
	{
		public static IApplicationExitInfo[] GetHistoricalProcessExitInfo(string packageName = null, int pid = 0, int maxNum = 0)
		{
			IApplicationExitInfo[] array = null;
			bool flag = array == null;
			if (flag)
			{
				array = new IApplicationExitInfo[0];
			}
			return array;
		}

		public static void SetProcessStateSummary(sbyte[] buffer)
		{
		}
	}
}
