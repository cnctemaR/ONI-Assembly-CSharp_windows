using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeConditional("ENABLE_XR")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/Depth/XRDepthSubsystemDescriptor.h")]
	[UsedByNativeCode]
	public class XRDepthSubsystemDescriptor : IntegratedSubsystemDescriptor<XRDepthSubsystem>
	{
		public extern bool SupportsFeaturePoints
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
