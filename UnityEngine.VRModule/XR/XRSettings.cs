using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Global XR related settings.</para>
	/// </summary>
	[NativeConditional("ENABLE_VR")]
	[NativeHeader("Runtime/VR/VRModule.h")]
	[NativeHeader("Runtime/Interfaces/IVRDevice.h")]
	[NativeHeader("Runtime/VR/ScriptBindings/XR.bindings.h")]
	public static class XRSettings
	{
		/// <summary>
		///   <para>Globally enables or disables XR for the application.</para>
		/// </summary>
		public static extern bool enabled
		{
			[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("VRModuleBindings::SetDeviceEnabled", true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets the render mode for the XR device. The render mode controls how the view of the XR device renders in the Game view and in the main window on a host PC.</para>
		/// </summary>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern GameViewRenderMode gameViewRenderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Read-only value that can be used to determine if the XR device is active.</para>
		/// </summary>
		[NativeName("Active")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool isDeviceActive
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>This property has been deprecated. Use XRSettings.gameViewRenderMode instead.</para>
		/// </summary>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool showDeviceView
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>This field has been deprecated. Use XRSettings.eyeTextureResolutionScale instead.</para>
		/// </summary>
		[Obsolete("renderScale is deprecated, use XRSettings.eyeTextureResolutionScale instead (UnityUpgradable) -> eyeTextureResolutionScale", false)]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern float renderScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls the actual size of eye textures as a multiplier of the device's default resolution.</para>
		/// </summary>
		[NativeName("RenderScale")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern float eyeTextureResolutionScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The current width of an eye texture for the loaded device.</para>
		/// </summary>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern int eyeTextureWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The current height of an eye texture for the loaded device.</para>
		/// </summary>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern int eyeTextureHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Fetch the eye texture RenderTextureDescriptor from the active stereo device.</para>
		/// </summary>
		[NativeName("DefaultEyeTextureDesc")]
		[NativeConditional("ENABLE_VR", "RenderTextureDesc()")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static RenderTextureDescriptor eyeTextureDesc
		{
			get
			{
				RenderTextureDescriptor renderTextureDescriptor;
				XRSettings.get_eyeTextureDesc_Injected(out renderTextureDescriptor);
				return renderTextureDescriptor;
			}
		}

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

		[NativeName("RenderViewportScale")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		internal static extern float renderViewportScaleInternal
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>A scale applied to the standard occulsion mask for each platform.</para>
		/// </summary>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern float occlusionMaskScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specifies whether or not the occlusion mesh should be used when rendering. Enabled by default.</para>
		/// </summary>
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool useOcclusionMesh
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Type of XR device that is currently loaded.</para>
		/// </summary>
		[NativeName("DeviceName")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern string loadedDeviceName
		{
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadDeviceByName(string[] prioritizedDeviceNameList);

		/// <summary>
		///   <para>Returns a list of supported XR devices that were included at build time.</para>
		/// </summary>
		public static extern string[] supportedDevices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_eyeTextureDesc_Injected(out RenderTextureDescriptor ret);
	}
}
