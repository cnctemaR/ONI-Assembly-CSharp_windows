using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Class providing information about XRRaycastSubsystem registration.</para>
	/// </summary>
	[NativeType(Header = "Modules/XR/Subsystems/Raycast/XRRaycastSubsystemDescriptor.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[UsedByNativeCode]
	public class XRRaycastSubsystemDescriptor : SubsystemDescriptor<XRRaycastSubsystem>
	{
	}
}
