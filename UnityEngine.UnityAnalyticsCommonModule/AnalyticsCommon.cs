using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Analytics
{
	[NativeHeader("Modules/UnityAnalyticsCommon/Public/UnityAnalyticsCommon.h")]
	[StructLayout(LayoutKind.Sequential)]
	public static class AnalyticsCommon
	{
		[StaticAccessor("GetUnityAnalyticsCommon()", StaticAccessorType.Dot)]
		private static extern bool ugsAnalyticsEnabledInternal
		{
			[NativeMethod("UGSAnalyticsUserOptStatus")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetUGSAnalyticsUserOptStatus")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static bool ugsAnalyticsEnabled
		{
			get
			{
				return AnalyticsCommon.ugsAnalyticsEnabledInternal;
			}
			set
			{
				AnalyticsCommon.ugsAnalyticsEnabledInternal = value;
			}
		}
	}
}
