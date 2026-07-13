using System;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class LicensingInitAnalytic : AnalyticsEventBase
	{
		public LicensingInitAnalytic()
			: base("license_init", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static LicensingInitAnalytic CreateLicensingInitAnalytic()
		{
			return new LicensingInitAnalytic();
		}

		public string licensingProtocolVersion;

		public string licensingClientVersion;

		public string channelType;

		public double initTime;

		public bool isLegacy;

		public string sessionId;

		public string correlationId;
	}
}
