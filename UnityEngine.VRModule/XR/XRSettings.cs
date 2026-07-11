using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine.XR
{
	[NativeHeader("Runtime/VR/ScriptBindings/XR.bindings.h")]
	[NativeConditional("ENABLE_VR")]
	[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
	[NativeHeader("Runtime/Interfaces/IVRDevice.h")]
	[NativeHeader("Runtime/VR/VRModule.h")]
	public static class XRSettings
	{
		public static extern bool enabled
		{
			[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("VRModuleBindings::SetDeviceEnabled", true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern GameViewRenderMode gameViewRenderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("Active")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool isDeviceActive
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool showDeviceView
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("renderScale is deprecated, use XRSettings.eyeTextureResolutionScale instead (UnityUpgradable) -> eyeTextureResolutionScale", false)]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern float renderScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("RenderScale")]
		public static extern float eyeTextureResolutionScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern int eyeTextureWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern int eyeTextureHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeConditional("ENABLE_VR", "RenderTextureDesc()")]
		[NativeName("DefaultEyeTextureDesc")]
		public static RenderTextureDescriptor eyeTextureDesc
		{
			get
			{
				RenderTextureDescriptor renderTextureDescriptor;
				XRSettings.get_eyeTextureDesc_Injected(out renderTextureDescriptor);
				return renderTextureDescriptor;
			}
		}

		[NativeName("DeviceEyeTextureDimension")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern TextureDimension deviceEyeTextureDimension
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

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

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		[NativeName("RenderViewportScale")]
		internal static extern float renderViewportScaleInternal
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern float occlusionMaskScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern bool useOcclusionMesh
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("DeviceName")]
		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern string loadedDeviceName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static void LoadDeviceByName(string deviceName)
		{
			XRSettings.LoadDeviceByName(new string[] { deviceName });
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadDeviceByName(string[] prioritizedDeviceNameList);

		public static extern string[] supportedDevices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("GetIVRDevice()", StaticAccessorType.ArrowWithDefaultReturnIfNull)]
		public static extern XRSettings.StereoRenderingMode stereoRenderingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_eyeTextureDesc_Injected(out RenderTextureDescriptor ret);

		public enum StereoRenderingMode
		{
			MultiPass,
			SinglePass,
			SinglePassInstanced,
			SinglePassMultiview
		}
	}
}
