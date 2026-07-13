using System;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[ExcludeFromDocs]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class LicensingErrorAnalytic : AnalyticsEventBase
	{
		public LicensingErrorAnalytic()
			: base("license_error", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static LicensingErrorAnalytic CreateLicensingErrorAnalytic()
		{
			return new LicensingErrorAnalytic();
		}

		public string licensingErrorType;

		public string additionalData;

		public string errorMessage;

		public string correlationId;

		public string sessionId;
	}
}
