using System;
using UnityEngine.Bindings;

namespace UnityEngine.XR
{
	[StaticAccessor("XRInputTrackingFacade::Get()", StaticAccessorType.Dot)]
	[NativeConditional("ENABLE_VR")]
	[NativeHeader("Modules/XR/Subsystems/Input/Public/XRInputTrackingFacade.h")]
	public class InputDevices
	{
		[NativeConditional("ENABLE_VR", "InputDevice::Identity")]
		public static InputDevice GetDeviceAtXRNode(XRNode node)
		{
			ulong deviceIdAtXRNode = InputTracking.GetDeviceIdAtXRNode(node);
			return new InputDevice(deviceIdAtXRNode);
		}
	}
}
