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
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("DeviceConnected")]
		public static extern bool isPresent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern UserPresenceState userPresence
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[Obsolete("family is deprecated.  Use XRSettings.loadedDeviceName instead.", false)]
		[NativeName("DeviceName")]
		public static extern string family
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("DeviceModel")]
		public static extern string model
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("DeviceRefreshRate")]
		public static extern float refreshRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetNativePtr();

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TrackingSpaceType GetTrackingSpaceType();

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTrackingSpaceType(TrackingSpaceType trackingSpaceType);

		[NativeName("DisableAutoVRCameraTracking")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableAutoXRCameraTracking([NotNull] Camera camera, bool disabled);

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("UpdateEyeTextureMSAASetting")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateEyeTextureMSAASetting();

		public static extern float fovZoomFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
			[NativeName("SetProjectionZoomFactor")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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
			if (XRDevice.deviceLoaded != null)
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
