using System;

namespace UnityEngine.UnityConsent
{
	public struct ConsentState
	{
		public ConsentState()
		{
			this.AdsIntent = ConsentStatus.Unspecified;
			this.AnalyticsIntent = ConsentStatus.Unspecified;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}: {3}", new object[] { "AdsIntent", this.AdsIntent, "AnalyticsIntent", this.AnalyticsIntent });
		}

		public ConsentStatus AdsIntent;

		public ConsentStatus AnalyticsIntent;
	}
}
