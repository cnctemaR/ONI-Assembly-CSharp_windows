using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Information about an Input subsystem.</para>
	/// </summary>
	[NativeConditional("ENABLE_XR")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/Input/XRInputSubsystemDescriptor.h")]
	[UsedByNativeCode]
	public class XRInputSubsystemDescriptor : SubsystemDescriptor<XRInputSubsystem>
	{
		/// <summary>
		///   <para>When true, will suppress legacy support for Daydream, Oculus, OpenVR, and Windows MR built directly into the Unity runtime from generating input. This is useful when adding an XRInputSubsystem that supports these devices.</para>
		/// </summary>
		[NativeConditional("ENABLE_XR")]
		public extern bool disablesLegacyInput
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
