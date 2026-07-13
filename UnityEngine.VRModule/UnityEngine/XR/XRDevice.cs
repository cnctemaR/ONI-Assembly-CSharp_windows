using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[Obsolete("UnityEngine.VRModule is deprecated and will be removed in a future version. Please use the APIs in the UnityEngine.XRModule instead")]
	[NativeConditional("ENABLE_VR")]
	public static class XRDevice
	{
		[Obsolete("This is obsolete, and should no longer be used. Instead, find the active XRDisplaySubsystem and check that the running property is true (for details, see XRDevice.isPresent documentation).", true)]
		public static bool isPresent
		{
			get
			{
				throw new NotSupportedException("XRDevice is Obsolete. Instead, find the active XRDisplaySubsystem and check to see if it is running.");
			}
		}

		[NativeName("DeviceRefreshRate")]
		[StaticAccessor("GetIVRDeviceSwapChain()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern float refreshRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetNativePtr();

		[Obsolete("This is obsolete, and should no longer be used.  Please use XRInputSubsystem.GetTrackingOriginMode.")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TrackingSpaceType GetTrackingSpaceType();

		[Obsolete("This is obsolete, and should no longer be used.  Please use XRInputSubsystem.TrySetTrackingOriginMode.")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTrackingSpaceType(TrackingSpaceType trackingSpaceType);

		[NativeName("DisableAutoVRCameraTracking")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static void DisableAutoXRCameraTracking([NotNull] Camera camera, bool disabled)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			XRDevice.DisableAutoXRCameraTracking_Injected(intPtr, disabled);
		}

		[NativeName("UpdateEyeTextureMSAASetting")]
		[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateEyeTextureMSAASetting();

		public static extern float fovZoomFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetProjectionZoomFactor")]
			[StaticAccessor("GetIVRDeviceScripting()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableAutoXRCameraTracking_Injected(IntPtr camera, bool disabled);
	}
}
