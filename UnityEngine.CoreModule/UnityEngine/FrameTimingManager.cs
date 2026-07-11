using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The FrameTimingManager allows the user to capture and access FrameTiming data for multple frames.</para>
	/// </summary>
	[StaticAccessor("GetFrameTimingManager()", StaticAccessorType.Dot)]
	public static class FrameTimingManager
	{
		/// <summary>
		///   <para>This function triggers the FrameTimingManager to capture a snapshot of FrameTiming's data, that can then be accessed by the user.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CaptureFrameTimings();

		/// <summary>
		///   <para>Allows the user to access the currently captured FrameTimings.</para>
		/// </summary>
		/// <param name="numFrames">User supplies a desired number of frames they would like FrameTimings for. This should be equal to or less than the maximum FrameTimings the platform can capture.</param>
		/// <param name="timings">An array of FrameTiming structs that is passed in by the user and will be filled with data as requested. It is the users job to make sure the array that is passed is large enough to hold the requested number of FrameTimings.</param>
		/// <returns>
		///   <para>Returns the number of FrameTimings it actually was able to get. This will always be equal to or less than the requested numFrames depending on availability of captured FrameTimings.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetLatestTimings(uint numFrames, FrameTiming[] timings);

		/// <summary>
		///   <para>This returns the number of vsyncs per second on the current platform, used to interpret timing results. If the platform does not support returning this value it will return 0.</para>
		/// </summary>
		/// <returns>
		///   <para>Number of vsyncs per second of the current platform.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetVSyncsPerSecond();

		/// <summary>
		///   <para>This returns the frequency of GPU timer on the current platform, used to interpret timing results. If the platform does not support returning this value it will return 0.</para>
		/// </summary>
		/// <returns>
		///   <para>GPU timer frequency for current platform.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong GetGpuTimerFrequency();

		/// <summary>
		///   <para>This returns the frequency of CPU timer on the current platform, used to interpret timing results. If the platform does not support returning this value it will return 0.</para>
		/// </summary>
		/// <returns>
		///   <para>CPU timer frequency for current platform.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong GetCpuTimerFrequency();
	}
}
