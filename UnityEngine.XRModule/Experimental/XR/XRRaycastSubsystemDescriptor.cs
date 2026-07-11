using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeType(Header = "Modules/XR/Subsystems/Raycast/XRRaycastSubsystemDescriptor.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[UsedByNativeCode]
	public class XRRaycastSubsystemDescriptor : IntegratedSubsystemDescriptor<XRRaycastSubsystem>
	{
	}
}
