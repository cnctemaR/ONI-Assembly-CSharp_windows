using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR
{
	[NativeConditional("ENABLE_VR")]
	public static class XRStats
	{
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetGPUTimeLastFrame(out float gpuTimeLastFrame);

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetDroppedFrameCount(out int droppedFrameCount);

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetFramePresentCount(out int framePresentCount);

		[Obsolete("gpuTimeLastFrame is deprecated. Use XRStats.TryGetGPUTimeLastFrame instead.", false)]
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
