using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Struct containing basic FrameTimings and accompanying relevant data.</para>
	/// </summary>
	[NativeHeader("Runtime/GfxDevice/FrameTiming.h")]
	public struct FrameTiming
	{
		/// <summary>
		///   <para>This is the CPU clock time at the point Present was called for the current frame.</para>
		/// </summary>
		[NativeName("m_CPUTimePresentCalled")]
		public ulong cpuTimePresentCalled;

		/// <summary>
		///   <para>The CPU time for a given frame, in ms.</para>
		/// </summary>
		[NativeName("m_CPUFrameTime")]
		public double cpuFrameTime;

		/// <summary>
		///   <para>This is the CPU clock time at the point GPU finished rendering the frame and interrupted the CPU.</para>
		/// </summary>
		[NativeName("m_CPUTimeFrameComplete")]
		public ulong cpuTimeFrameComplete;

		/// <summary>
		///   <para>The GPU time for a given frame, in ms.</para>
		/// </summary>
		[NativeName("m_GPUFrameTime")]
		public double gpuFrameTime;

		/// <summary>
		///   <para>This was the height scale factor of the Dynamic Resolution system(if used) for the given frame and the linked frame timings.</para>
		/// </summary>
		[NativeName("m_HeightScale")]
		public float heightScale;

		/// <summary>
		///   <para>This was the width scale factor of the Dynamic Resolution system(if used) for the given frame and the linked frame timings.</para>
		/// </summary>
		[NativeName("m_WidthScale")]
		public float widthScale;

		/// <summary>
		///   <para>This was the vsync mode for the given frame and the linked frame timings.</para>
		/// </summary>
		[NativeName("m_SyncInterval")]
		public uint syncInterval;
	}
}
