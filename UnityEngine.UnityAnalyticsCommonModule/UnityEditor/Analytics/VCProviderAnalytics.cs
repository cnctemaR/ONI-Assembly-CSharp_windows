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
	public class VCProviderAnalytics : AnalyticsEventBase
	{
		public VCProviderAnalytics()
			: base("versioncontrol_ProviderSettings_OnUpdate", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static VCProviderAnalytics CreateVCProviderAnalytics()
		{
			return new VCProviderAnalytics();
		}

		public string Mode;
	}
}
