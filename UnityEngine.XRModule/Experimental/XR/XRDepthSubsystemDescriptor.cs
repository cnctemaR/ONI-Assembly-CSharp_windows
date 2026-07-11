using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Class providing information about XRDepthSubsystem registration.</para>
	/// </summary>
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/Depth/XRDepthSubsystemDescriptor.h")]
	[UsedByNativeCode]
	[NativeConditional("ENABLE_XR")]
	public class XRDepthSubsystemDescriptor : SubsystemDescriptor<XRDepthSubsystem>
	{
		/// <summary>
		///   <para>When true, XRDepthSubsystem will provide list of feature points detected so far.</para>
		/// </summary>
		public extern bool SupportsFeaturePoints
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
