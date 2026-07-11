using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeConditional("ENABLE_VR")]
	public static class XRDevice
	{
		[NativeName("DeviceConnected")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool isPresent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("This is obsolete, and should no longer be used.  Please use CommonUsages.userPresence.")]
		public static extern UserPresenceState userPresence
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeName("DeviceName")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[Obsolete("family is deprecated.  Use XRSettings.loadedDeviceName instead.", false)]
		public static extern string family
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeName("DeviceModel")]
		[Obsolete("This is obsolete, and should no longer be used.  Please use InputDevice.name.")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern string model
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeName("DeviceRefreshRate")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern float refreshRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetNativePtr();

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[Obsolete("This is obsolete, and should no longer be used.  Please use XRInputSubsystem.GetTrackingOriginMode.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TrackingSpaceType GetTrackingSpaceType();

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[Obsolete("This is obsolete, and should no longer be used.  Please use XRInputSubsystem.TrySetTrackingOriginMode.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTrackingSpaceType(TrackingSpaceType trackingSpaceType);

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("DisableAutoVRCameraTracking")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableAutoXRCameraTracking([NotNull] Camera camera, bool disabled);

		[NativeName("UpdateEyeTextureMSAASetting")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateEyeTextureMSAASetting();

		public static extern float fovZoomFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetProjectionZoomFactor")]
			[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("This is obsolete, and should no longer be used.  Please use XRInputSubsystem.GetTrackingOriginMode.")]
		public static extern TrackingOriginMode trackingOriginMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<string> deviceLoaded;

		[RequiredByNativeCode]
		private static void InvokeDeviceLoaded(string loadedDeviceName)
		{
			bool flag = XRDevice.deviceLoaded != null;
			if (flag)
			{
				XRDevice.deviceLoaded(loadedDeviceName);
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static XRDevice()
		{
			XRDevice.deviceLoaded = null;
		}
	}
}
