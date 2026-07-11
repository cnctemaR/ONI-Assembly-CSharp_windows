using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Contains all functionality related to a XR device.</para>
	/// </summary>
	[NativeConditional("ENABLE_VR")]
	public static class XRDevice
	{
		/// <summary>
		///   <para>Successfully detected a XR device in working order.</para>
		/// </summary>
		[NativeName("DeviceConnected")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool isPresent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Indicates whether the user is present and interacting with the device.</para>
		/// </summary>
		public static extern UserPresenceState userPresence
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The name of the family of the loaded XR device.</para>
		/// </summary>
		[NativeName("DeviceName")]
		[Obsolete("family is deprecated.  Use XRSettings.loadedDeviceName instead.", false)]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern string family
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Specific model of loaded XR device.</para>
		/// </summary>
		[NativeName("DeviceModel")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern string model
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Refresh rate of the display in Hertz.</para>
		/// </summary>
		[NativeName("DeviceRefreshRate")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern float refreshRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>This method returns an IntPtr representing the native pointer to the XR device if one is available, otherwise the value will be IntPtr.Zero.</para>
		/// </summary>
		/// <returns>
		///   <para>The native pointer to the XR device.</para>
		/// </returns>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetNativePtr();

		/// <summary>
		///   <para>Returns the device's current TrackingSpaceType. This value determines how the camera is positioned relative to its starting position. For more, see the section "Understanding the camera" in.</para>
		/// </summary>
		/// <returns>
		///   <para>The device's current TrackingSpaceType.</para>
		/// </returns>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TrackingSpaceType GetTrackingSpaceType();

		/// <summary>
		///   <para>Sets the device's current TrackingSpaceType. Returns true on success. Returns false if the given TrackingSpaceType is not supported or the device fails to switch.</para>
		/// </summary>
		/// <param name="TrackingSpaceType">The TrackingSpaceType the device should switch to.</param>
		/// <param name="trackingSpaceType"></param>
		/// <returns>
		///   <para>True on success. False if the given TrackingSpaceType is not supported or the device fails to switch.</para>
		/// </returns>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTrackingSpaceType(TrackingSpaceType trackingSpaceType);

		[NativeName("DisableAutoVRCameraTracking")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableAutoXRCameraTracking([NotNull] Camera camera, bool disabled);

		/// <summary>
		///   <para>Zooms the XR projection.</para>
		/// </summary>
		public static extern float fovZoomFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
			[NativeName("SetProjectionZoomFactor")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
