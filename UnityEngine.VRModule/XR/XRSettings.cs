using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Global XR related settings.</para>
	/// </summary>
	[NativeHeader("Runtime/VR/VRDevice.h")]
	[NativeHeader("Runtime/VR/PluginInterface/Headers/IUnityVR.h")]
	public static class XRSettings
	{
		/// <summary>
		///   <para>Globally enables or disables XR for the application.</para>
		/// </summary>
		public static extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Read-only value that can be used to determine if the XR device is active.</para>
		/// </summary>
		public static extern bool isDeviceActive
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>This property has been deprecated. Use XRSettings.gameViewRenderMode instead.</para>
		/// </summary>
		public static extern bool showDeviceView
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>This field has been deprecated. Use XRSettings.eyeTextureResolutionScale instead.</para>
		/// </summary>
		[Obsolete("renderScale is deprecated, use XRSettings.eyeTextureResolutionScale instead (UnityUpgradable) -> eyeTextureResolutionScale")]
		public static extern float renderScale
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls the actual size of eye textures as a multiplier of the device's default resolution.</para>
		/// </summary>
		public static extern float eyeTextureResolutionScale
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The current width of an eye texture for the loaded device.</para>
		/// </summary>
		public static extern int eyeTextureWidth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The current height of an eye texture for the loaded device.</para>
		/// </summary>
		public static extern int eyeTextureHeight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern float renderViewportScaleInternal
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Fetch the eye texture RenderTextureDescriptor from the active stereo device.</para>
		/// </summary>
		public static RenderTextureDescriptor eyeTextureDesc
		{
			get
			{
				RenderTextureDescriptor renderTextureDescriptor;
				XRSettings.INTERNAL_get_eyeTextureDesc(out renderTextureDescriptor);
				return renderTextureDescriptor;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_eyeTextureDesc(out RenderTextureDescriptor value);

		/// <summary>
		///   <para>Controls how much of the allocated eye texture should be used for rendering.</para>
		/// </summary>
		public static float renderViewportScale
		{
			get
			{
				return XRSettings.renderViewportScaleInternal;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("value", "Render viewport scale should be between 0 and 1.");
				}
				XRSettings.renderViewportScaleInternal = value;
			}
		}

		/// <summary>
		///   <para>A scale applied to the standard occulsion mask for each platform.</para>
		/// </summary>
		public static extern float occlusionMaskScale
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specifies whether or not the occlusion mesh should be used when rendering. Enabled by default.</para>
		/// </summary>
		public static extern bool useOcclusionMesh
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Type of XR device that is currently loaded.</para>
		/// </summary>
		public static extern string loadedDeviceName
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Loads the requested device at the beginning of the next frame.</para>
		/// </summary>
		/// <param name="deviceName">Name of the device from XRSettings.supportedDevices.</param>
		/// <param name="prioritizedDeviceNameList">Prioritized list of device names from XRSettings.supportedDevices.</param>
		public static void LoadDeviceByName(string deviceName)
		{
			XRSettings.LoadDeviceByName(new string[] { deviceName });
		}

		/// <summary>
		///   <para>Loads the requested device at the beginning of the next frame.</para>
		/// </summary>
		/// <param name="deviceName">Name of the device from XRSettings.supportedDevices.</param>
		/// <param name="prioritizedDeviceNameList">Prioritized list of device names from XRSettings.supportedDevices.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadDeviceByName(string[] prioritizedDeviceNameList);

		/// <summary>
		///   <para>Returns a list of supported XR devices that were included at build time.</para>
		/// </summary>
		public static extern string[] supportedDevices
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Sets the render mode for the XR device. The render mode controls how the view of the XR device renders in the Game view and in the main window on a host PC.</para>
		/// </summary>
		public static extern GameViewRenderMode gameViewRenderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
