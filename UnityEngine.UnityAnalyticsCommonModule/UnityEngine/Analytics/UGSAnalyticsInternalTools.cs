using System;
using UnityEngine.Internal;

namespace UnityEngine.Analytics
{
	[ExcludeFromDocs]
	public interface UGSAnalyticsInternalTools
	{
		public static void SetPrivacyStatus(bool status)
		{
			AnalyticsCommon.ugsAnalyticsEnabled = status;
		}
	}
}
