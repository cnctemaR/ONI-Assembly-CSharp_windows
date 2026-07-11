using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Analytics
{
	[StaticAccessor("GetPerformanceReportingManager()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/PerformanceReporting/PerformanceReportingManager.h")]
	public static class PerformanceReporting
	{
		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern long graphicsInitializationFinishTime
		{
			[NativeMethod("GetGfxDoneTime")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
