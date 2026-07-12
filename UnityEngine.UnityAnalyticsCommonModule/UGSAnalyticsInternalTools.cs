using System;

namespace UnityEngine.Analytics
{
	public interface UGSAnalyticsInternalTools
	{
		public static void SetPrivacyStatus(bool status)
		{
			AnalyticsCommon.ugsAnalyticsEnabled = status;
		}
	}
}
