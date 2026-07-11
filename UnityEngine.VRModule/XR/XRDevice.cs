using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Contains all functionality related to a XR device.</para>
	/// </summary>
	public static class XRDevice
	{
		/// <summary>
		///   <para>Successfully detected a XR device in working order.</para>
		/// </summary>
		public static extern bool isPresent
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Indicates whether the user is present and interacting with the device.</para>
		/// </summary>
		public static extern UserPresenceState userPresence
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The name of the family of the loaded XR device.</para>
		/// </summary>
		[Obsolete("family is deprecated.  Use XRSettings.loadedDeviceName instead.")]
		public static extern string family
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Specific model of loaded XR device.</para>
		/// </summary>
		public static extern string model
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Refresh rate of the display in Hertz.</para>
		/// </summary>
		public static extern float refreshRate
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the device's current TrackingSpaceType. This value determines how the camera is positioned relative to its starting position. For more, see the section "Understanding the camera" in.</para>
		/// </summary>
		/// <returns>
		///   <para>The device's current TrackingSpaceType.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
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
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTrackingSpaceType(TrackingSpaceType trackingSpaceType);

		/// <summary>
		///   <para>This method returns an IntPtr representing the native pointer to the XR device if one is available, otherwise the value will be IntPtr.Zero.</para>
		/// </summary>
		/// <returns>
		///   <para>The native pointer to the XR device.</para>
		/// </returns>
		public static IntPtr GetNativePtr()
		{
			IntPtr intPtr;
			XRDevice.INTERNAL_CALL_GetNativePtr(out intPtr);
			return intPtr;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativePtr(out IntPtr value);

		public static void DisableAutoXRCameraTracking(Camera camera, bool disabled)
		{
			if (camera == null)
			{
				throw new ArgumentNullException("camera");
			}
			XRDevice.DisableAutoXRCameraTrackingInternal(camera, disabled);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableAutoXRCameraTrackingInternal(Camera camera, bool disabled);

		/// <summary>
		///   <para>Zooms the XR projection.</para>
		/// </summary>
		public static extern float fovZoomFactor
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
