using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeConditional("ENABLE_XR")]
	[UsedByNativeCode]
	[NativeType(Header = "Modules/XR/Subsystems/Input/XRInputSubsystemDescriptor.h")]
	public class XRInputSubsystemDescriptor : IntegratedSubsystemDescriptor<XRInputSubsystem>
	{
		[NativeConditional("ENABLE_XR")]
		public extern bool disablesLegacyInput
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
