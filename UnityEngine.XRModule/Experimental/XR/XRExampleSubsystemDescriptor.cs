using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeType(Header = "Modules/XR/Subsystems/Example/XRExampleSubsystemDescriptor.h")]
	[UsedByNativeCode]
	public class XRExampleSubsystemDescriptor : IntegratedSubsystemDescriptor<XRExampleSubsystem>
	{
		public extern bool supportsEditorMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool disableBackbufferMSAA
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool stereoscopicBackbuffer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool usePBufferEGL
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
