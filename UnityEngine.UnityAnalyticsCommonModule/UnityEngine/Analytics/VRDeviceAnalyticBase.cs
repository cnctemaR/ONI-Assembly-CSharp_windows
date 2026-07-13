using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class VRDeviceAnalyticBase : AnalyticsEventBase
	{
		public VRDeviceAnalyticBase()
			: base("deviceStatus", 1, SendEventOptions.kAppendNone, "")
		{
		}
	}
}
