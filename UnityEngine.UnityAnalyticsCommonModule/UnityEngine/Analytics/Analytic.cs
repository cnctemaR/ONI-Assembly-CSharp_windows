using System;
using UnityEngine.Internal;

namespace UnityEngine.Analytics
{
	[ExcludeFromDocs]
	public class Analytic : AnalyticsEventBase
	{
		public Analytic(IAnalytic instance, AnalyticInfoAttribute info)
			: base(info.eventName, info.version, SendEventOptions.kAppendNone, "")
		{
			this.instance = instance;
			this.info = info;
		}

		public readonly IAnalytic instance;

		public readonly AnalyticInfoAttribute info;
	}
}
