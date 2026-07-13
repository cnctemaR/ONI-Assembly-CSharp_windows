using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[StructLayout(LayoutKind.Sequential)]
	public class AnalyticsEventBase
	{
		public string EventName()
		{
			return this.eventName;
		}

		public int EventVersion()
		{
			return this.eventVersion;
		}

		public string EventPrefix()
		{
			return this.eventPrefix;
		}

		public AnalyticsEventBase(string eventName, int eventVersion, SendEventOptions sendEventOptions = SendEventOptions.kAppendNone, string eventPrefix = "")
		{
			this.eventName = eventName;
			this.eventVersion = eventVersion;
			this.sendEventOptions = sendEventOptions;
			this.eventPrefix = eventPrefix;
		}

		public AnalyticsEventBase(AnalyticsEventBase e)
			: this(e.eventName, e.eventVersion, SendEventOptions.kAppendNone, "")
		{
		}

		public AnalyticsEventBase()
		{
		}

		private string eventName;

		private int eventVersion;

		private string eventPrefix;

		private SendEventOptions sendEventOptions;
	}
}
