using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[StaticAccessor("GetUncheckedRealGfxDevice().GetFrameTimingManager()", StaticAccessorType.Dot)]
	public static class FrameTimingManager
	{
		[StaticAccessor("FrameTimingManager", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsFeatureEnabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CaptureFrameTimings();

		public unsafe static uint GetLatestTimings(uint numFrames, FrameTiming[] timings)
		{
			Span<FrameTiming> span = new Span<FrameTiming>(timings);
			uint latestTimings_Injected;
			fixed (FrameTiming* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				latestTimings_Injected = FrameTimingManager.GetLatestTimings_Injected(numFrames, ref managedSpanWrapper);
			}
			return latestTimings_Injected;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetVSyncsPerSecond();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong GetGpuTimerFrequency();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong GetCpuTimerFrequency();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetLatestTimings_Injected(uint numFrames, ref ManagedSpanWrapper timings);
	}
}
