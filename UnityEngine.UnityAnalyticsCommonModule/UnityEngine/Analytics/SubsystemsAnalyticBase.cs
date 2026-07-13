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
	public class SubsystemsAnalyticBase : AnalyticsEventBase
	{
		public SubsystemsAnalyticBase(string eventName)
			: base(eventName, 1, SendEventOptions.kAppendNone, "")
		{
		}

		public string subsystem;
	}
}
