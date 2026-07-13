using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class UaaLApplicationLaunchAnalytic : AnalyticsEventBase
	{
		public UaaLApplicationLaunchAnalytic()
			: base("UaaLApplicationLaunch", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		public static UaaLApplicationLaunchAnalytic CreateUaaLApplicationLaunchAnalytic()
		{
			return new UaaLApplicationLaunchAnalytic();
		}

		public int launch_type;

		public int launch_process_type;
	}
}
