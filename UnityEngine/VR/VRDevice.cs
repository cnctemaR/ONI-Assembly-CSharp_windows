using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.VR
{
	public static class VRDevice
	{
		public static extern bool isPresent
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("family is deprecated.  Use VRSettings.loadedDeviceName instead.")]
		public static extern string family
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string model
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float refreshRate
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TrackingSpaceType GetTrackingSpaceType();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTrackingSpaceType(TrackingSpaceType trackingSpaceType);

		public static IntPtr GetNativePtr()
		{
			IntPtr intPtr;
			VRDevice.INTERNAL_CALL_GetNativePtr(out intPtr);
			return intPtr;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativePtr(out IntPtr value);
	}
}
