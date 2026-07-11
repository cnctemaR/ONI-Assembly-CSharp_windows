using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeType(Header = "Modules/XR/Subsystems/Example/XRExampleSubsystem.h")]
	[UsedByNativeCode]
	public class XRExampleSubsystem : Subsystem<XRExampleSubsystemDescriptor>
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PrintExample();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetBool();
	}
}
