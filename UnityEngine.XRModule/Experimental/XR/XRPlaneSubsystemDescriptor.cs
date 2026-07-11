using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Class providing information about XRPlaneSubsystem registration.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeType(Header = "Modules/XR/Subsystems/Planes/XRPlaneSubsystemDescriptor.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	public class XRPlaneSubsystemDescriptor : SubsystemDescriptor<XRPlaneSubsystem>
	{
	}
}
