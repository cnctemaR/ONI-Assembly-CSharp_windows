using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Timing and other statistics from the XR subsystem.</para>
	/// </summary>
	public static class XRStats
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetGPUTimeLastFrame(out float gpuTimeLastFrame);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetDroppedFrameCount(out int droppedFrameCount);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetFramePresentCount(out int framePresentCount);

		/// <summary>
		///   <para>Total GPU time utilized last frame as measured by the XR subsystem.</para>
		/// </summary>
		[Obsolete("gpuTimeLastFrame is deprecated. Use XRStats.TryGetGPUTimeLastFrame instead.")]
		public static float gpuTimeLastFrame
		{
			get
			{
				float num;
				float num2;
				if (XRStats.TryGetGPUTimeLastFrame(out num))
				{
					num2 = num;
				}
				else
				{
					num2 = 0f;
				}
				return num2;
			}
		}
	}
}
