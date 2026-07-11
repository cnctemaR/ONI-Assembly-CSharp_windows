using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Class providing information about  XRCameraSubsystem registration.</para>
	/// </summary>
	[NativeType(Header = "Modules/XR/Subsystems/Camera/XRCameraSubsystemDescriptor.h")]
	[UsedByNativeCode]
	[NativeConditional("ENABLE_XR")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	public class XRCameraSubsystemDescriptor : SubsystemDescriptor<XRCameraSubsystem>
	{
		/// <summary>
		///   <para>Specifies if current subsystem is allowed to provide average brightness.</para>
		/// </summary>
		public extern bool ProvidesAverageBrightness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Specifies if current subsystem is allowed to provide average camera temperature.</para>
		/// </summary>
		public extern bool ProvidesAverageColorTemperature
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Specifies if current subsystem is allowed to provide projection matrix.</para>
		/// </summary>
		public extern bool ProvidesProjectionMatrix
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Specifies if current subsystem is allowed to provide display matrix.</para>
		/// </summary>
		public extern bool ProvidesDisplayMatrix
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Specifies if current subsystem is allowed to provide timestamp.</para>
		/// </summary>
		public extern bool ProvidesTimestamp
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
