using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[UsedByNativeCode]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/Planes/XRPlaneSubsystemDescriptor.h")]
	public class XRPlaneSubsystemDescriptor : IntegratedSubsystemDescriptor<XRPlaneSubsystem>
	{
	}
}
