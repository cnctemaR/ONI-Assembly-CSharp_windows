using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeType(Header = "Modules/XR/Subsystems/Session/XRSessionSubsystemDescriptor.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[UsedByNativeCode]
	public class XRSessionSubsystemDescriptor : IntegratedSubsystemDescriptor<XRSessionSubsystem>
	{
	}
}
