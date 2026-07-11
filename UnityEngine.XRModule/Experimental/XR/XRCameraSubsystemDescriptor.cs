using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeConditional("ENABLE_XR")]
	[UsedByNativeCode]
	[NativeType(Header = "Modules/XR/Subsystems/Camera/XRCameraSubsystemDescriptor.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	public class XRCameraSubsystemDescriptor : IntegratedSubsystemDescriptor<XRCameraSubsystem>
	{
		public extern bool ProvidesAverageBrightness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool ProvidesAverageColorTemperature
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool ProvidesProjectionMatrix
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool ProvidesDisplayMatrix
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool ProvidesTimestamp
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
