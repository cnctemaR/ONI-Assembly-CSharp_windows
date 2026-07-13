using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace UnityEngine.Device
{
	public static class SystemInfo
	{
		public static float batteryLevel
		{
			get
			{
				return SystemInfo.batteryLevel;
			}
		}

		public static BatteryStatus batteryStatus
		{
			get
			{
				return SystemInfo.batteryStatus;
			}
		}

		public static string operatingSystem
		{
			get
			{
				return SystemInfo.operatingSystem;
			}
		}

		public static OperatingSystemFamily operatingSystemFamily
		{
			get
			{
				return SystemInfo.operatingSystemFamily;
			}
		}

		public static string processorType
		{
			get
			{
				return SystemInfo.processorType;
			}
		}

		public static string ProcessorModel
		{
			get
			{
				return SystemInfo.processorModel;
			}
		}

		public static string ProcessorManufacturer
		{
			get
			{
				return SystemInfo.processorManufacturer;
			}
		}

		public static int processorFrequency
		{
			get
			{
				return SystemInfo.processorFrequency;
			}
		}

		public static int processorCount
		{
			get
			{
				return SystemInfo.processorCount;
			}
		}

		public static int systemMemorySize
		{
			get
			{
				return SystemInfo.systemMemorySize;
			}
		}

		public static string deviceUniqueIdentifier
		{
			get
			{
				return SystemInfo.deviceUniqueIdentifier;
			}
		}

		public static string deviceName
		{
			get
			{
				return SystemInfo.deviceName;
			}
		}

		public static string deviceModel
		{
			get
			{
				return SystemInfo.deviceModel;
			}
		}

		public static bool supportsAccelerometer
		{
			get
			{
				return SystemInfo.supportsAccelerometer;
			}
		}

		public static bool supportsGyroscope
		{
			get
			{
				return SystemInfo.supportsGyroscope;
			}
		}

		public static bool supportsLocationService
		{
			get
			{
				return SystemInfo.supportsLocationService;
			}
		}

		public static bool supportsVibration
		{
			get
			{
				return SystemInfo.supportsVibration;
			}
		}

		public static bool supportsAudio
		{
			get
			{
				return SystemInfo.supportsAudio;
			}
		}

		public static bool supportsRendering
		{
			get
			{
				return SystemInfo.supportsRendering;
			}
		}

		public static DeviceType deviceType
		{
			get
			{
				return SystemInfo.deviceType;
			}
		}

		public static int graphicsMemorySize
		{
			get
			{
				return SystemInfo.graphicsMemorySize;
			}
		}

		public static string graphicsDeviceName
		{
			get
			{
				return SystemInfo.graphicsDeviceName;
			}
		}

		public static string graphicsDeviceVendor
		{
			get
			{
				return SystemInfo.graphicsDeviceVendor;
			}
		}

		public static int graphicsDeviceID
		{
			get
			{
				return SystemInfo.graphicsDeviceID;
			}
		}

		public static int graphicsDeviceVendorID
		{
			get
			{
				return SystemInfo.graphicsDeviceVendorID;
			}
		}

		public static GraphicsDeviceType graphicsDeviceType
		{
			get
			{
				return SystemInfo.graphicsDeviceType;
			}
		}

		public static bool graphicsUVStartsAtTop
		{
			get
			{
				return SystemInfo.graphicsUVStartsAtTop;
			}
		}

		public static string graphicsDeviceVersion
		{
			get
			{
				return SystemInfo.graphicsDeviceVersion;
			}
		}

		public static int graphicsShaderLevel
		{
			get
			{
				return SystemInfo.graphicsShaderLevel;
			}
		}

		public static bool graphicsMultiThreaded
		{
			get
			{
				return SystemInfo.graphicsMultiThreaded;
			}
		}

		public static RenderingThreadingMode renderingThreadingMode
		{
			get
			{
				return SystemInfo.renderingThreadingMode;
			}
		}

		public static FoveatedRenderingCaps foveatedRenderingCaps
		{
			get
			{
				return SystemInfo.foveatedRenderingCaps;
			}
		}

		public static bool supportsVariableRateShading
		{
			get
			{
				return SystemInfo.supportsVariableRateShading;
			}
		}

		public static int maxTiledPixelStorageSize
		{
			get
			{
				return SystemInfo.maxTiledPixelStorageSize;
			}
		}

		public static bool hasTiledGPU
		{
			get
			{
				return SystemInfo.hasTiledGPU;
			}
		}

		public static bool hasHiddenSurfaceRemovalOnGPU
		{
			get
			{
				return SystemInfo.hasHiddenSurfaceRemovalOnGPU;
			}
		}

		public static bool hasDynamicUniformArrayIndexingInFragmentShaders
		{
			get
			{
				return SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders;
			}
		}

		public static bool supportsShadows
		{
			get
			{
				return SystemInfo.supportsShadows;
			}
		}

		public static bool supportsRawShadowDepthSampling
		{
			get
			{
				return SystemInfo.supportsRawShadowDepthSampling;
			}
		}

		public static bool supportsMotionVectors
		{
			get
			{
				return SystemInfo.supportsMotionVectors;
			}
		}

		public static bool supports3DTextures
		{
			get
			{
				return SystemInfo.supports3DTextures;
			}
		}

		public static bool supportsCompressed3DTextures
		{
			get
			{
				return SystemInfo.supportsCompressed3DTextures;
			}
		}

		public static bool supports2DArrayTextures
		{
			get
			{
				return SystemInfo.supports2DArrayTextures;
			}
		}

		public static bool supports3DRenderTextures
		{
			get
			{
				return SystemInfo.supports3DRenderTextures;
			}
		}

		public static bool supportsCubemapArrayTextures
		{
			get
			{
				return SystemInfo.supportsCubemapArrayTextures;
			}
		}

		public static bool supportsAnisotropicFilter
		{
			get
			{
				return SystemInfo.supportsAnisotropicFilter;
			}
		}

		public static CopyTextureSupport copyTextureSupport
		{
			get
			{
				return SystemInfo.copyTextureSupport;
			}
		}

		public static bool supportsComputeShaders
		{
			get
			{
				return SystemInfo.supportsComputeShaders;
			}
		}

		public static bool supportsGeometryShaders
		{
			get
			{
				return SystemInfo.supportsGeometryShaders;
			}
		}

		public static bool supportsTessellationShaders
		{
			get
			{
				return SystemInfo.supportsTessellationShaders;
			}
		}

		public static bool supportsRenderTargetArrayIndexFromVertexShader
		{
			get
			{
				return SystemInfo.supportsRenderTargetArrayIndexFromVertexShader;
			}
		}

		public static bool supportsInstancing
		{
			get
			{
				return SystemInfo.supportsInstancing;
			}
		}

		public static bool supportsHardwareQuadTopology
		{
			get
			{
				return SystemInfo.supportsHardwareQuadTopology;
			}
		}

		public static bool supports32bitsIndexBuffer
		{
			get
			{
				return SystemInfo.supports32bitsIndexBuffer;
			}
		}

		public static bool supportsSparseTextures
		{
			get
			{
				return SystemInfo.supportsSparseTextures;
			}
		}

		public static int supportedRenderTargetCount
		{
			get
			{
				return SystemInfo.supportedRenderTargetCount;
			}
		}

		public static bool supportsSeparatedRenderTargetsBlend
		{
			get
			{
				return SystemInfo.supportsSeparatedRenderTargetsBlend;
			}
		}

		public static int supportedRandomWriteTargetCount
		{
			get
			{
				return SystemInfo.supportedRandomWriteTargetCount;
			}
		}

		public static bool supportsMemorylessTextures
		{
			get
			{
				return SystemInfo.supportsMemorylessTextures;
			}
		}

		public static int supportsMultisampledTextures
		{
			get
			{
				return SystemInfo.supportsMultisampledTextures;
			}
		}

		public static bool supportsMultisampled2DArrayTextures
		{
			get
			{
				return SystemInfo.supportsMultisampled2DArrayTextures;
			}
		}

		public static bool supportsMultisampledBackBuffer
		{
			get
			{
				return SystemInfo.supportsMultisampledBackBuffer;
			}
		}

		public static bool supportsMultisampleAutoResolve
		{
			get
			{
				return SystemInfo.supportsMultisampleAutoResolve;
			}
		}

		public static bool supportsMultisampledShaderResolve
		{
			get
			{
				return SystemInfo.supportsMultisampledShaderResolve;
			}
		}

		public static int supportsTextureWrapMirrorOnce
		{
			get
			{
				return SystemInfo.supportsTextureWrapMirrorOnce;
			}
		}

		public static bool usesReversedZBuffer
		{
			get
			{
				return SystemInfo.usesReversedZBuffer;
			}
		}

		public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
		{
			return SystemInfo.SupportsRenderTextureFormat(format);
		}

		public static bool SupportsBlendingOnRenderTextureFormat(RenderTextureFormat format)
		{
			return SystemInfo.SupportsBlendingOnRenderTextureFormat(format);
		}

		public static bool SupportsTextureFormat(TextureFormat format)
		{
			return SystemInfo.SupportsTextureFormat(format);
		}

		public static bool SupportsVertexAttributeFormat(VertexAttributeFormat format, int dimension)
		{
			return SystemInfo.SupportsVertexAttributeFormat(format, dimension);
		}

		public static NPOTSupport npotSupport
		{
			get
			{
				return SystemInfo.npotSupport;
			}
		}

		public static int maxTextureSize
		{
			get
			{
				return SystemInfo.maxTextureSize;
			}
		}

		public static int maxTexture3DSize
		{
			get
			{
				return SystemInfo.maxTexture3DSize;
			}
		}

		public static int maxTextureArraySlices
		{
			get
			{
				return SystemInfo.maxTextureArraySlices;
			}
		}

		public static int maxCubemapSize
		{
			get
			{
				return SystemInfo.maxCubemapSize;
			}
		}

		public static int maxAnisotropyLevel
		{
			get
			{
				return SystemInfo.maxAnisotropyLevel;
			}
		}

		public static int maxComputeBufferInputsVertex
		{
			get
			{
				return SystemInfo.maxComputeBufferInputsVertex;
			}
		}

		public static int maxComputeBufferInputsFragment
		{
			get
			{
				return SystemInfo.maxComputeBufferInputsFragment;
			}
		}

		public static int maxComputeBufferInputsGeometry
		{
			get
			{
				return SystemInfo.maxComputeBufferInputsGeometry;
			}
		}

		public static int maxComputeBufferInputsDomain
		{
			get
			{
				return SystemInfo.maxComputeBufferInputsDomain;
			}
		}

		public static int maxComputeBufferInputsHull
		{
			get
			{
				return SystemInfo.maxComputeBufferInputsHull;
			}
		}

		public static int maxComputeBufferInputsCompute
		{
			get
			{
				return SystemInfo.maxComputeBufferInputsCompute;
			}
		}

		public static int maxComputeWorkGroupSize
		{
			get
			{
				return SystemInfo.maxComputeWorkGroupSize;
			}
		}

		public static int maxComputeWorkGroupSizeX
		{
			get
			{
				return SystemInfo.maxComputeWorkGroupSizeX;
			}
		}

		public static int maxComputeWorkGroupSizeY
		{
			get
			{
				return SystemInfo.maxComputeWorkGroupSizeY;
			}
		}

		public static int maxComputeWorkGroupSizeZ
		{
			get
			{
				return SystemInfo.maxComputeWorkGroupSizeZ;
			}
		}

		public static int computeSubGroupSize
		{
			get
			{
				return SystemInfo.computeSubGroupSize;
			}
		}

		public static bool supportsAsyncCompute
		{
			get
			{
				return SystemInfo.supportsAsyncCompute;
			}
		}

		public static bool supportsGpuRecorder
		{
			get
			{
				return SystemInfo.supportsGpuRecorder;
			}
		}

		public static bool supportsGraphicsFence
		{
			get
			{
				return SystemInfo.supportsGraphicsFence;
			}
		}

		public static bool supportsAsyncGPUReadback
		{
			get
			{
				return SystemInfo.supportsAsyncGPUReadback;
			}
		}

		public static bool supportsParallelPSOCreation
		{
			get
			{
				return SystemInfo.supportsParallelPSOCreation;
			}
		}

		public static bool supportsRayTracing
		{
			get
			{
				return SystemInfo.supportsRayTracing;
			}
		}

		public static bool supportsRayTracingShaders
		{
			get
			{
				return SystemInfo.supportsRayTracingShaders;
			}
		}

		public static bool supportsInlineRayTracing
		{
			get
			{
				return SystemInfo.supportsInlineRayTracing;
			}
		}

		public static bool supportsIndirectDispatchRays
		{
			get
			{
				return SystemInfo.supportsIndirectDispatchRays;
			}
		}

		public static bool supportsSetConstantBuffer
		{
			get
			{
				return SystemInfo.supportsSetConstantBuffer;
			}
		}

		public static int constantBufferOffsetAlignment
		{
			get
			{
				return SystemInfo.constantBufferOffsetAlignment;
			}
		}

		public static int maxConstantBufferSize
		{
			get
			{
				return SystemInfo.maxConstantBufferSize;
			}
		}

		public static long maxGraphicsBufferSize
		{
			get
			{
				return SystemInfo.maxGraphicsBufferSize;
			}
		}

		public static bool hasMipMaxLevel
		{
			get
			{
				return SystemInfo.hasMipMaxLevel;
			}
		}

		public static bool supportsMipStreaming
		{
			get
			{
				return SystemInfo.supportsMipStreaming;
			}
		}

		public static bool usesLoadStoreActions
		{
			get
			{
				return SystemInfo.usesLoadStoreActions;
			}
		}

		public static HDRDisplaySupportFlags hdrDisplaySupportFlags
		{
			get
			{
				return SystemInfo.hdrDisplaySupportFlags;
			}
		}

		public static bool supportsConservativeRaster
		{
			get
			{
				return SystemInfo.supportsConservativeRaster;
			}
		}

		public static bool supportsMultiview
		{
			get
			{
				return SystemInfo.supportsMultiview;
			}
		}

		public static bool supportsStoreAndResolveAction
		{
			get
			{
				return SystemInfo.supportsStoreAndResolveAction;
			}
		}

		public static bool supportsMultisampleResolveDepth
		{
			get
			{
				return SystemInfo.supportsMultisampleResolveDepth;
			}
		}

		public static bool supportsMultisampleResolveStencil
		{
			get
			{
				return SystemInfo.supportsMultisampleResolveStencil;
			}
		}

		public static bool supportsIndirectArgumentsBuffer
		{
			get
			{
				return SystemInfo.supportsIndirectArgumentsBuffer;
			}
		}

		public static bool supportsDepthFetchInRenderPass
		{
			get
			{
				return SystemInfo.supportsDepthFetchInRenderPass;
			}
		}

		public static bool supportsDynamicResolution
		{
			get
			{
				return SystemInfo.supportsDynamicResolution;
			}
		}

		[Obsolete("Use overload with a GraphicsFormatUsage parameter instead", false)]
		public static bool IsFormatSupported(GraphicsFormat format, FormatUsage usage)
		{
			GraphicsFormatUsage graphicsFormatUsage = (GraphicsFormatUsage)(1 << (int)usage);
			return SystemInfo.IsFormatSupported(format, graphicsFormatUsage);
		}

		public static bool IsFormatSupported(GraphicsFormat format, GraphicsFormatUsage usage)
		{
			return SystemInfo.IsFormatSupported(format, usage);
		}

		[Obsolete("Use overload with a GraphicsFormatUsage parameter instead", false)]
		public static GraphicsFormat GetCompatibleFormat(GraphicsFormat format, FormatUsage usage)
		{
			GraphicsFormatUsage graphicsFormatUsage = (GraphicsFormatUsage)(1 << (int)usage);
			return SystemInfo.GetCompatibleFormat(format, graphicsFormatUsage);
		}

		public static GraphicsFormat GetCompatibleFormat(GraphicsFormat format, GraphicsFormatUsage usage)
		{
			return SystemInfo.GetCompatibleFormat(format, usage);
		}

		public static GraphicsFormat GetGraphicsFormat(DefaultFormat format)
		{
			return SystemInfo.GetGraphicsFormat(format);
		}

		public static int GetRenderTextureSupportedMSAASampleCount(RenderTextureDescriptor desc)
		{
			return SystemInfo.GetRenderTextureSupportedMSAASampleCount(desc);
		}

		public static bool SupportsRandomWriteOnRenderTextureFormat(RenderTextureFormat format)
		{
			return SystemInfo.SupportsRandomWriteOnRenderTextureFormat(format);
		}

		public static int GetTiledRenderTargetStorageSize(GraphicsFormat format, int sampleCount)
		{
			return SystemInfo.GetTiledRenderTargetStorageSize(format, sampleCount);
		}

		public const string unsupportedIdentifier = "n/a";
	}
}
