using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Analytics
{
	/// <summary>
	///   <para>Unity Performace provides insight into your game performace.</para>
	/// </summary>
	[StaticAccessor("GetPerformanceReportingManager()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/PerformanceReporting/PerformanceReportingManager.h")]
	public static class PerformanceReporting
	{
		/// <summary>
		///   <para>Controls whether the Performance Reporting service is enabled at runtime.</para>
		/// </summary>
		[ThreadAndSerializationSafe]
		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Time taken to initialize graphics in nanoseconds, measured from application startup.</para>
		/// </summary>
		public static extern long graphicsInitializationFinishTime
		{
			[NativeMethod("GetGfxDoneTime")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
