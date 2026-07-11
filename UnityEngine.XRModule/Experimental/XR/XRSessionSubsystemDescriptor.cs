using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Class providing information about XRSessionSubsystem registration.</para>
	/// </summary>
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/Session/XRSessionSubsystemDescriptor.h")]
	[UsedByNativeCode]
	public class XRSessionSubsystemDescriptor : SubsystemDescriptor<XRSessionSubsystem>
	{
	}
}
