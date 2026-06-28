using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.VR
{
	public static class VRStats
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

		[Obsolete("gpuTimeLastFrame is deprecated. Use VRStats.TryGetGPUTimeLastFrame instead.")]
		public static float gpuTimeLastFrame
		{
			get
			{
				float num;
				float num2;
				if (VRStats.TryGetGPUTimeLastFrame(out num))
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
