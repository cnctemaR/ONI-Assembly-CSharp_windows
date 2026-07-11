using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[UsedByNativeCode]
	[NativeType(Header = "Modules/XR/Subsystems/ReferencePoints/XRReferencePointSubsystemDescriptor.h")]
	public class XRReferencePointSubsystemDescriptor : IntegratedSubsystemDescriptor<XRReferencePointSubsystem>
	{
	}
}
