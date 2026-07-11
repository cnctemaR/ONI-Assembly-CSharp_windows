using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.Tango
{
	[NativeConditional("PLATFORM_ANDROID")]
	[NativeHeader("Runtime/AR/Tango/TangoScriptApi.h")]
	internal static class TangoDevice
	{
		internal static extern CoordinateFrame baseCoordinateFrame
		{
			[NativeConditional(false)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeThrows]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Connect(string[] boolKeys, bool[] boolValues, string[] intKeys, int[] intValues, string[] longKeys, long[] longValues, string[] doubleKeys, double[] doubleValues, string[] stringKeys, string[] stringValues);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Disconnect();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool TryGetHorizontalFov(out float fovOut);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool TryGetVerticalFov(out float fovOut);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetRenderMode(ARRenderMode mode);

		internal static extern uint depthCameraRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool synchronizeFramerateWithColorCamera
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBackgroundMaterial(Material material);

		internal static bool TryGetLatestPointCloud(ref PointCloudData pointCloudData)
		{
			if (pointCloudData.points == null)
			{
				pointCloudData.points = new List<Vector4>();
			}
			pointCloudData.points.Clear();
			return TangoDevice.TryGetLatestPointCloudInternal(pointCloudData.points, out pointCloudData.version, out pointCloudData.timestamp);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetLatestPointCloudInternal(List<Vector4> pointCloudData, out uint version, out double timestamp);

		internal static bool TryGetLatestImageData(ref ImageData image)
		{
			if (image.planeData == null)
			{
				image.planeData = new List<byte>();
			}
			if (image.planeInfos == null)
			{
				image.planeInfos = new List<ImageData.PlaneInfo>();
			}
			image.planeData.Clear();
			return TangoDevice.TryGetLatestImageDataInternal(image.planeData, image.planeInfos, out image.width, out image.height, out image.format, out image.timestampNs, out image.metadata);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetLatestImageDataInternal(List<byte> imageData, List<ImageData.PlaneInfo> planeInfos, out uint width, out uint height, out int format, out long timestampNs, out ImageData.CameraMetadata metadata);

		internal static extern bool isServiceConnected
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool isServiceAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static bool TryAcquireLatestPointCloud(ref NativePointCloud pointCloud)
		{
			return TangoDevice.Internal_TryAcquireLatestPointCloud(out pointCloud.version, out pointCloud.timestamp, out pointCloud.numPoints, out pointCloud.points, out pointCloud.nativePtr);
		}

		internal static void ReleasePointCloud(IntPtr pointCloudNativePtr)
		{
			TangoDevice.Internal_ReleasePointCloud(pointCloudNativePtr);
		}

		internal static bool TryAcquireLatestImageBuffer(ref NativeImage nativeImage)
		{
			if (nativeImage.planeInfos == null)
			{
				nativeImage.planeInfos = new List<ImageData.PlaneInfo>();
			}
			return TangoDevice.Internal_TryAcquireLatestImageBuffer(nativeImage.planeInfos, out nativeImage.width, out nativeImage.height, out nativeImage.format, out nativeImage.timestampNs, out nativeImage.planeData, out nativeImage.nativePtr, out nativeImage.metadata);
		}

		internal static void ReleaseImageBuffer(IntPtr imageBufferNativePtr)
		{
			TangoDevice.Internal_ReleaseImageBuffer(imageBufferNativePtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_TryAcquireLatestImageBuffer(List<ImageData.PlaneInfo> planeInfos, out uint width, out uint height, out int format, out long timestampNs, out IntPtr planeData, out IntPtr nativePtr, out ImageData.CameraMetadata metadata);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_TryAcquireLatestPointCloud(out uint version, out double timestamp, out uint numPoints, out IntPtr points, out IntPtr nativePtr);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ReleasePointCloud(IntPtr pointCloudPtr);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ReleaseImageBuffer(IntPtr imageBufferPtr);

		internal static string areaDescriptionUUID
		{
			get
			{
				return TangoDevice.m_AreaDescriptionUUID;
			}
			set
			{
				TangoDevice.m_AreaDescriptionUUID = value;
			}
		}

		internal static ARBackgroundRenderer backgroundRenderer
		{
			get
			{
				return TangoDevice.m_BackgroundRenderer;
			}
			set
			{
				if (value != null)
				{
					if (TangoDevice.m_BackgroundRenderer != null)
					{
						TangoDevice.m_BackgroundRenderer.backgroundRendererChanged -= TangoDevice.OnBackgroundRendererChanged;
					}
					TangoDevice.m_BackgroundRenderer = value;
					TangoDevice.m_BackgroundRenderer.backgroundRendererChanged += TangoDevice.OnBackgroundRendererChanged;
					TangoDevice.OnBackgroundRendererChanged();
				}
			}
		}

		private static void OnBackgroundRendererChanged()
		{
			TangoDevice.SetBackgroundMaterial(TangoDevice.m_BackgroundRenderer.backgroundMaterial);
			TangoDevice.SetRenderMode(TangoDevice.m_BackgroundRenderer.mode);
		}

		internal static bool Connect(TangoConfig config)
		{
			string[] array;
			bool[] array2;
			TangoDevice.CopyDictionaryToArrays<bool>(config.m_boolParams, out array, out array2);
			string[] array3;
			int[] array4;
			TangoDevice.CopyDictionaryToArrays<int>(config.m_intParams, out array3, out array4);
			string[] array5;
			long[] array6;
			TangoDevice.CopyDictionaryToArrays<long>(config.m_longParams, out array5, out array6);
			string[] array7;
			double[] array8;
			TangoDevice.CopyDictionaryToArrays<double>(config.m_doubleParams, out array7, out array8);
			string[] array9;
			string[] array10;
			TangoDevice.CopyDictionaryToArrays<string>(config.m_stringParams, out array9, out array10);
			return TangoDevice.Connect(array, array2, array3, array4, array5, array6, array7, array8, array9, array10);
		}

		private static void CopyDictionaryToArrays<T>(Dictionary<string, T> dictionary, out string[] keys, out T[] values)
		{
			if (dictionary.Count == 0)
			{
				keys = null;
				values = null;
			}
			else
			{
				keys = new string[dictionary.Count];
				values = new T[dictionary.Count];
				int num = 0;
				foreach (KeyValuePair<string, T> keyValuePair in dictionary)
				{
					keys[num] = keyValuePair.Key;
					values[num++] = keyValuePair.Value;
				}
			}
		}

		private static ARBackgroundRenderer m_BackgroundRenderer = null;

		private static string m_AreaDescriptionUUID = "";
	}
}
