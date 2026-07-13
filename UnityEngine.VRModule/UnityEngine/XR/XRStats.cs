using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR
{
	[Obsolete("UnityEngine.VRModule is deprecated and will be removed in a future version. Please use the APIs in the UnityEngine.XRModule instead")]
	[NativeConditional("ENABLE_VR")]
	public static class XRStats
	{
		[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetGPUTimeLastFrame(out float gpuTimeLastFrame);

		[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetDroppedFrameCount(out int droppedFrameCount);

		[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool TryGetFramePresentCount(out int framePresentCount);
	}
}
