using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Class providing information about XRReferencePointSubsystem registration.</para>
	/// </summary>
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/ReferencePoints/XRReferencePointSubsystemDescriptor.h")]
	[UsedByNativeCode]
	public class XRReferencePointSubsystemDescriptor : SubsystemDescriptor<XRReferencePointSubsystem>
	{
	}
}
