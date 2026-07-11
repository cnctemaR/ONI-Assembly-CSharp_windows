using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/GraphicsFormatUtility.bindings.h")]
	[NativeHeader("Runtime/Input/GetInput.h")]
	[NativeHeader("Runtime/Camera/RenderLoops/MotionVectorRenderLoop.h")]
	[NativeHeader("Runtime/Shaders/GraphicsCapsScriptBindings.h")]
	[NativeHeader("Runtime/Misc/SystemInfo.h")]
	public sealed class SystemInfo
	{
		[NativeProperty]
		public static float batteryLevel
		{
			get
			{
				return SystemInfo.GetBatteryLevel();
			}
		}

		public static BatteryStatus batteryStatus
		{
			get
			{
				return SystemInfo.GetBatteryStatus();
			}
		}

		public static string operatingSystem
		{
			get
			{
				return SystemInfo.GetOperatingSystem();
			}
		}

		public static OperatingSystemFamily operatingSystemFamily
		{
			get
			{
				return SystemInfo.GetOperatingSystemFamily();
			}
		}

		public static string processorType
		{
			get
			{
				return SystemInfo.GetProcessorType();
			}
		}

		public static int processorFrequency
		{
			get
			{
				return SystemInfo.GetProcessorFrequencyMHz();
			}
		}

		public static int processorCount
		{
			get
			{
				return SystemInfo.GetProcessorCount();
			}
		}

		public static int systemMemorySize
		{
			get
			{
				return SystemInfo.GetPhysicalMemoryMB();
			}
		}

		public static string deviceUniqueIdentifier
		{
			get
			{
				return SystemInfo.GetDeviceUniqueIdentifier();
			}
		}

		public static string deviceName
		{
			get
			{
				return SystemInfo.GetDeviceName();
			}
		}

		public static string deviceModel
		{
			get
			{
				return SystemInfo.GetDeviceModel();
			}
		}

		public static bool supportsAccelerometer
		{
			get
			{
				return SystemInfo.SupportsAccelerometer();
			}
		}

		public static bool supportsGyroscope
		{
			get
			{
				return SystemInfo.IsGyroAvailable();
			}
		}

		public static bool supportsLocationService
		{
			get
			{
				return SystemInfo.SupportsLocationService();
			}
		}

		public static bool supportsVibration
		{
			get
			{
				return SystemInfo.SupportsVibration();
			}
		}

		public static bool supportsAudio
		{
			get
			{
				return SystemInfo.SupportsAudio();
			}
		}

		public static DeviceType deviceType
		{
			get
			{
				return SystemInfo.GetDeviceType();
			}
		}

		public static int graphicsMemorySize
		{
			get
			{
				return SystemInfo.GetGraphicsMemorySize();
			}
		}

		public static string graphicsDeviceName
		{
			get
			{
				return SystemInfo.GetGraphicsDeviceName();
			}
		}

		public static string graphicsDeviceVendor
		{
			get
			{
				return SystemInfo.GetGraphicsDeviceVendor();
			}
		}

		public static int graphicsDeviceID
		{
			get
			{
				return SystemInfo.GetGraphicsDeviceID();
			}
		}

		public static int graphicsDeviceVendorID
		{
			get
			{
				return SystemInfo.GetGraphicsDeviceVendorID();
			}
		}

		public static GraphicsDeviceType graphicsDeviceType
		{
			get
			{
				return SystemInfo.GetGraphicsDeviceType();
			}
		}

		public static bool graphicsUVStartsAtTop
		{
			get
			{
				return SystemInfo.GetGraphicsUVStartsAtTop();
			}
		}

		public static string graphicsDeviceVersion
		{
			get
			{
				return SystemInfo.GetGraphicsDeviceVersion();
			}
		}

		public static int graphicsShaderLevel
		{
			get
			{
				return SystemInfo.GetGraphicsShaderLevel();
			}
		}

		public static bool graphicsMultiThreaded
		{
			get
			{
				return SystemInfo.GetGraphicsMultiThreaded();
			}
		}

		public static bool hasHiddenSurfaceRemovalOnGPU
		{
			get
			{
				return SystemInfo.HasHiddenSurfaceRemovalOnGPU();
			}
		}

		public static bool hasDynamicUniformArrayIndexingInFragmentShaders
		{
			get
			{
				return SystemInfo.HasDynamicUniformArrayIndexingInFragmentShaders();
			}
		}

		public static bool supportsShadows
		{
			get
			{
				return SystemInfo.SupportsShadows();
			}
		}

		public static bool supportsRawShadowDepthSampling
		{
			get
			{
				return SystemInfo.SupportsRawShadowDepthSampling();
			}
		}

		[Obsolete("supportsRenderTextures always returns true, no need to call it")]
		public static bool supportsRenderTextures
		{
			get
			{
				return true;
			}
		}

		public static bool supportsMotionVectors
		{
			get
			{
				return SystemInfo.SupportsMotionVectors();
			}
		}

		public static bool supportsRenderToCubemap
		{
			get
			{
				return SystemInfo.SupportsRenderToCubemap();
			}
		}

		public static bool supportsImageEffects
		{
			get
			{
				return SystemInfo.SupportsImageEffects();
			}
		}

		public static bool supports3DTextures
		{
			get
			{
				return SystemInfo.Supports3DTextures();
			}
		}

		public static bool supports2DArrayTextures
		{
			get
			{
				return SystemInfo.Supports2DArrayTextures();
			}
		}

		public static bool supports3DRenderTextures
		{
			get
			{
				return SystemInfo.Supports3DRenderTextures();
			}
		}

		public static bool supportsCubemapArrayTextures
		{
			get
			{
				return SystemInfo.SupportsCubemapArrayTextures();
			}
		}

		public static CopyTextureSupport copyTextureSupport
		{
			get
			{
				return SystemInfo.GetCopyTextureSupport();
			}
		}

		public static bool supportsComputeShaders
		{
			get
			{
				return SystemInfo.SupportsComputeShaders();
			}
		}

		public static bool supportsInstancing
		{
			get
			{
				return SystemInfo.SupportsInstancing();
			}
		}

		public static bool supportsHardwareQuadTopology
		{
			get
			{
				return SystemInfo.SupportsHardwareQuadTopology();
			}
		}

		public static bool supports32bitsIndexBuffer
		{
			get
			{
				return SystemInfo.Supports32bitsIndexBuffer();
			}
		}

		public static bool supportsSparseTextures
		{
			get
			{
				return SystemInfo.SupportsSparseTextures();
			}
		}

		public static int supportedRenderTargetCount
		{
			get
			{
				return SystemInfo.SupportedRenderTargetCount();
			}
		}

		public static bool supportsSeparatedRenderTargetsBlend
		{
			get
			{
				return SystemInfo.SupportsSeparatedRenderTargetsBlend();
			}
		}

		internal static int supportedRandomWriteTargetCount
		{
			get
			{
				return SystemInfo.SupportedRandomWriteTargetCount();
			}
		}

		public static int supportsMultisampledTextures
		{
			get
			{
				return SystemInfo.SupportsMultisampledTextures();
			}
		}

		public static bool supportsMultisampleAutoResolve
		{
			get
			{
				return SystemInfo.SupportsMultisampleAutoResolve();
			}
		}

		public static int supportsTextureWrapMirrorOnce
		{
			get
			{
				return SystemInfo.SupportsTextureWrapMirrorOnce();
			}
		}

		public static bool usesReversedZBuffer
		{
			get
			{
				return SystemInfo.UsesReversedZBuffer();
			}
		}

		[Obsolete("supportsStencil always returns true, no need to call it")]
		public static int supportsStencil
		{
			get
			{
				return 1;
			}
		}

		private static bool IsValidEnumValue(Enum value)
		{
			return Enum.IsDefined(value.GetType(), value);
		}

		public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
		{
			if (!SystemInfo.IsValidEnumValue(format))
			{
				throw new ArgumentException("Failed SupportsRenderTextureFormat; format is not a valid RenderTextureFormat");
			}
			return SystemInfo.HasRenderTextureNative(format);
		}

		public static bool SupportsBlendingOnRenderTextureFormat(RenderTextureFormat format)
		{
			if (!SystemInfo.IsValidEnumValue(format))
			{
				throw new ArgumentException("Failed SupportsBlendingOnRenderTextureFormat; format is not a valid RenderTextureFormat");
			}
			return SystemInfo.SupportsBlendingOnRenderTextureFormatNative(format);
		}

		public static bool SupportsTextureFormat(TextureFormat format)
		{
			if (!SystemInfo.IsValidEnumValue(format))
			{
				throw new ArgumentException("Failed SupportsTextureFormat; format is not a valid TextureFormat");
			}
			return SystemInfo.SupportsTextureFormatNative(format);
		}

		public static NPOTSupport npotSupport
		{
			get
			{
				return SystemInfo.GetNPOTSupport();
			}
		}

		public static int maxTextureSize
		{
			get
			{
				return SystemInfo.GetMaxTextureSize();
			}
		}

		public static int maxCubemapSize
		{
			get
			{
				return SystemInfo.GetMaxCubemapSize();
			}
		}

		internal static int maxRenderTextureSize
		{
			get
			{
				return SystemInfo.GetMaxRenderTextureSize();
			}
		}

		public static bool supportsAsyncCompute
		{
			get
			{
				return SystemInfo.SupportsAsyncCompute();
			}
		}

		public static bool supportsGPUFence
		{
			get
			{
				return SystemInfo.SupportsGPUFence();
			}
		}

		public static bool supportsAsyncGPUReadback
		{
			get
			{
				return SystemInfo.SupportsAsyncGPUReadback();
			}
		}

		public static bool supportsMipStreaming
		{
			get
			{
				return SystemInfo.SupportsMipStreaming();
			}
		}

		[Obsolete("graphicsPixelFillrate is no longer supported in Unity 5.0+.")]
		public static int graphicsPixelFillrate
		{
			get
			{
				return -1;
			}
		}

		[Obsolete("Vertex program support is required in Unity 5.0+")]
		public static bool supportsVertexPrograms
		{
			get
			{
				return true;
			}
		}

		[FreeFunction("systeminfo::GetBatteryLevel")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetBatteryLevel();

		[FreeFunction("systeminfo::GetBatteryStatus")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern BatteryStatus GetBatteryStatus();

		[FreeFunction("systeminfo::GetOperatingSystem")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetOperatingSystem();

		[FreeFunction("systeminfo::GetOperatingSystemFamily")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern OperatingSystemFamily GetOperatingSystemFamily();

		[FreeFunction("systeminfo::GetProcessorType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetProcessorType();

		[FreeFunction("systeminfo::GetProcessorFrequencyMHz")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetProcessorFrequencyMHz();

		[FreeFunction("systeminfo::GetProcessorCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetProcessorCount();

		[FreeFunction("systeminfo::GetPhysicalMemoryMB")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPhysicalMemoryMB();

		[FreeFunction("systeminfo::GetDeviceUniqueIdentifier")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetDeviceUniqueIdentifier();

		[FreeFunction("systeminfo::GetDeviceName")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetDeviceName();

		[FreeFunction("systeminfo::GetDeviceModel")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetDeviceModel();

		[FreeFunction("systeminfo::SupportsAccelerometer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsAccelerometer();

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsGyroAvailable();

		[FreeFunction("systeminfo::SupportsLocationService")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsLocationService();

		[FreeFunction("systeminfo::SupportsVibration")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsVibration();

		[FreeFunction("systeminfo::SupportsAudio")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsAudio();

		[FreeFunction("systeminfo::GetDeviceType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DeviceType GetDeviceType();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsMemorySize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGraphicsMemorySize();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceName")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetGraphicsDeviceName();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVendor")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetGraphicsDeviceVendor();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceID")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGraphicsDeviceID();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVendorID")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGraphicsDeviceVendorID();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsDeviceType GetGraphicsDeviceType();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsUVStartsAtTop")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetGraphicsUVStartsAtTop();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVersion")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetGraphicsDeviceVersion();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsShaderLevel")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGraphicsShaderLevel();

		[FreeFunction("ScriptingGraphicsCaps::GetGraphicsMultiThreaded")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetGraphicsMultiThreaded();

		[FreeFunction("ScriptingGraphicsCaps::HasHiddenSurfaceRemovalOnGPU")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasHiddenSurfaceRemovalOnGPU();

		[FreeFunction("ScriptingGraphicsCaps::HasDynamicUniformArrayIndexingInFragmentShaders")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasDynamicUniformArrayIndexingInFragmentShaders();

		[FreeFunction("ScriptingGraphicsCaps::SupportsShadows")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsShadows();

		[FreeFunction("ScriptingGraphicsCaps::SupportsRawShadowDepthSampling")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsRawShadowDepthSampling();

		[FreeFunction("SupportsMotionVectors")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsMotionVectors();

		[FreeFunction("ScriptingGraphicsCaps::SupportsRenderToCubemap")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsRenderToCubemap();

		[FreeFunction("ScriptingGraphicsCaps::SupportsImageEffects")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsImageEffects();

		[FreeFunction("ScriptingGraphicsCaps::Supports3DTextures")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Supports3DTextures();

		[FreeFunction("ScriptingGraphicsCaps::Supports2DArrayTextures")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Supports2DArrayTextures();

		[FreeFunction("ScriptingGraphicsCaps::Supports3DRenderTextures")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Supports3DRenderTextures();

		[FreeFunction("ScriptingGraphicsCaps::SupportsCubemapArrayTextures")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsCubemapArrayTextures();

		[FreeFunction("ScriptingGraphicsCaps::GetCopyTextureSupport")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CopyTextureSupport GetCopyTextureSupport();

		[FreeFunction("ScriptingGraphicsCaps::SupportsComputeShaders")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsComputeShaders();

		[FreeFunction("ScriptingGraphicsCaps::SupportsInstancing")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsInstancing();

		[FreeFunction("ScriptingGraphicsCaps::SupportsHardwareQuadTopology")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsHardwareQuadTopology();

		[FreeFunction("ScriptingGraphicsCaps::Supports32bitsIndexBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Supports32bitsIndexBuffer();

		[FreeFunction("ScriptingGraphicsCaps::SupportsSparseTextures")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsSparseTextures();

		[FreeFunction("ScriptingGraphicsCaps::SupportedRenderTargetCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SupportedRenderTargetCount();

		[FreeFunction("ScriptingGraphicsCaps::SupportsSeparatedRenderTargetsBlend")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsSeparatedRenderTargetsBlend();

		[FreeFunction("ScriptingGraphicsCaps::SupportedRandomWriteTargetCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SupportedRandomWriteTargetCount();

		[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampledTextures")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SupportsMultisampledTextures();

		[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampleAutoResolve")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsMultisampleAutoResolve();

		[FreeFunction("ScriptingGraphicsCaps::SupportsTextureWrapMirrorOnce")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SupportsTextureWrapMirrorOnce();

		[FreeFunction("ScriptingGraphicsCaps::UsesReversedZBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UsesReversedZBuffer();

		[FreeFunction("ScriptingGraphicsCaps::HasRenderTexture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasRenderTextureNative(RenderTextureFormat format);

		[FreeFunction("ScriptingGraphicsCaps::SupportsBlendingOnRenderTextureFormat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsBlendingOnRenderTextureFormatNative(RenderTextureFormat format);

		[FreeFunction("ScriptingGraphicsCaps::SupportsTextureFormat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsTextureFormatNative(TextureFormat format);

		[FreeFunction("ScriptingGraphicsCaps::GetNPOTSupport")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NPOTSupport GetNPOTSupport();

		[FreeFunction("ScriptingGraphicsCaps::GetMaxTextureSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaxTextureSize();

		[FreeFunction("ScriptingGraphicsCaps::GetMaxCubemapSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaxCubemapSize();

		[FreeFunction("ScriptingGraphicsCaps::GetMaxRenderTextureSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaxRenderTextureSize();

		[FreeFunction("ScriptingGraphicsCaps::SupportsAsyncCompute")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsAsyncCompute();

		[FreeFunction("ScriptingGraphicsCaps::SupportsGPUFence")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsGPUFence();

		[FreeFunction("ScriptingGraphicsCaps::SupportsAsyncGPUReadback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsAsyncGPUReadback();

		[FreeFunction("ScriptingGraphicsCaps::SupportsMipStreaming")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsMipStreaming();

		[FreeFunction("ScriptingGraphicsCaps::IsFormatSupported")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsFormatSupported(GraphicsFormat format, FormatUsage usage);

		public const string unsupportedIdentifier = "n/a";
	}
}
