using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Windows.WebCam
{
	[MovedFrom("UnityEngine.XR.WSA.WebCam")]
	[StaticAccessor("VideoCaptureBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("PlatformDependent/Win/Webcam/VideoCaptureBindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class VideoCapture : IDisposable
	{
		private static VideoCapture.VideoCaptureResult MakeCaptureResult(VideoCapture.CaptureResultType resultType, long hResult)
		{
			return new VideoCapture.VideoCaptureResult
			{
				resultType = resultType,
				hResult = hResult
			};
		}

		private static VideoCapture.VideoCaptureResult MakeCaptureResult(long hResult)
		{
			VideoCapture.VideoCaptureResult videoCaptureResult = default(VideoCapture.VideoCaptureResult);
			bool flag = hResult == VideoCapture.HR_SUCCESS;
			VideoCapture.CaptureResultType captureResultType;
			if (flag)
			{
				captureResultType = VideoCapture.CaptureResultType.Success;
			}
			else
			{
				captureResultType = VideoCapture.CaptureResultType.UnknownError;
			}
			videoCaptureResult.resultType = captureResultType;
			videoCaptureResult.hResult = hResult;
			return videoCaptureResult;
		}

		public static IEnumerable<Resolution> SupportedResolutions
		{
			get
			{
				bool flag = VideoCapture.s_SupportedResolutions == null;
				if (flag)
				{
					VideoCapture.s_SupportedResolutions = VideoCapture.GetSupportedResolutions_Internal();
				}
				return VideoCapture.s_SupportedResolutions;
			}
		}

		[NativeName("GetSupportedResolutions")]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		private static Resolution[] GetSupportedResolutions_Internal()
		{
			Resolution[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				VideoCapture.GetSupportedResolutions_Internal_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Resolution[] array;
				blittableArrayWrapper.Unmarshal<Resolution>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static IEnumerable<float> GetSupportedFrameRatesForResolution(Resolution resolution)
		{
			return VideoCapture.GetSupportedFrameRatesForResolution_Internal(resolution.width, resolution.height);
		}

		[NativeName("GetSupportedFrameRatesForResolution")]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		private static float[] GetSupportedFrameRatesForResolution_Internal(int resolutionWidth, int resolutionHeight)
		{
			float[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				VideoCapture.GetSupportedFrameRatesForResolution_Internal_Injected(resolutionWidth, resolutionHeight, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				float[] array;
				blittableArrayWrapper.Unmarshal<float>(ref array);
				array2 = array;
			}
			return array2;
		}

		public bool IsRecording
		{
			[NativeMethod("VideoCaptureBindings::IsRecording", HasExplicitThis = true)]
			[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
			get
			{
				IntPtr intPtr = VideoCapture.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoCapture.get_IsRecording_Injected(intPtr);
			}
		}

		public static void CreateAsync(bool showHolograms, VideoCapture.OnVideoCaptureResourceCreatedCallback onCreatedCallback)
		{
			bool flag = onCreatedCallback == null;
			if (flag)
			{
				throw new ArgumentNullException("onCreatedCallback");
			}
			VideoCapture.Instantiate_Internal(showHolograms, onCreatedCallback);
		}

		public static void CreateAsync(VideoCapture.OnVideoCaptureResourceCreatedCallback onCreatedCallback)
		{
			bool flag = onCreatedCallback == null;
			if (flag)
			{
				throw new ArgumentNullException("onCreatedCallback");
			}
			VideoCapture.Instantiate_Internal(false, onCreatedCallback);
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeName("Instantiate")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Instantiate_Internal(bool showHolograms, VideoCapture.OnVideoCaptureResourceCreatedCallback onCreatedCallback);

		[RequiredByNativeCode]
		private static void InvokeOnCreatedVideoCaptureResourceDelegate(VideoCapture.OnVideoCaptureResourceCreatedCallback callback, IntPtr nativePtr)
		{
			bool flag = nativePtr == IntPtr.Zero;
			if (flag)
			{
				callback(null);
			}
			else
			{
				callback(new VideoCapture(nativePtr));
			}
		}

		private VideoCapture(IntPtr nativeCaptureObject)
		{
			this.m_NativePtr = nativeCaptureObject;
		}

		public void StartVideoModeAsync(CameraParameters setupParams, VideoCapture.AudioState audioState, VideoCapture.OnVideoModeStartedCallback onVideoModeStartedCallback)
		{
			bool flag = onVideoModeStartedCallback == null;
			if (flag)
			{
				throw new ArgumentNullException("onVideoModeStartedCallback");
			}
			bool flag2 = setupParams.cameraResolutionWidth == 0 || setupParams.cameraResolutionHeight == 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("setupParams", "The camera resolution must be set to a supported resolution.");
			}
			bool flag3 = setupParams.frameRate == 0f;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("setupParams", "The camera frame rate must be set to a supported recording frame rate.");
			}
			this.StartVideoMode_Internal(setupParams, audioState, onVideoModeStartedCallback);
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeMethod("VideoCaptureBindings::StartVideoMode", HasExplicitThis = true)]
		private void StartVideoMode_Internal(CameraParameters cameraParameters, VideoCapture.AudioState audioState, VideoCapture.OnVideoModeStartedCallback onVideoModeStartedCallback)
		{
			IntPtr intPtr = VideoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoCapture.StartVideoMode_Internal_Injected(intPtr, ref cameraParameters, audioState, onVideoModeStartedCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnVideoModeStartedDelegate(VideoCapture.OnVideoModeStartedCallback callback, long hResult)
		{
			callback(VideoCapture.MakeCaptureResult(hResult));
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeMethod("VideoCaptureBindings::StopVideoMode", HasExplicitThis = true)]
		public void StopVideoModeAsync([NotNull] VideoCapture.OnVideoModeStoppedCallback onVideoModeStoppedCallback)
		{
			if (onVideoModeStoppedCallback == null)
			{
				ThrowHelper.ThrowArgumentNullException(onVideoModeStoppedCallback, "onVideoModeStoppedCallback");
			}
			IntPtr intPtr = VideoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoCapture.StopVideoModeAsync_Injected(intPtr, onVideoModeStoppedCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnVideoModeStoppedDelegate(VideoCapture.OnVideoModeStoppedCallback callback, long hResult)
		{
			callback(VideoCapture.MakeCaptureResult(hResult));
		}

		public void StartRecordingAsync(string filename, VideoCapture.OnStartedRecordingVideoCallback onStartedRecordingVideoCallback)
		{
			bool flag = onStartedRecordingVideoCallback == null;
			if (flag)
			{
				throw new ArgumentNullException("onStartedRecordingVideoCallback");
			}
			bool flag2 = string.IsNullOrEmpty(filename);
			if (flag2)
			{
				throw new ArgumentNullException("filename");
			}
			string directoryName = Path.GetDirectoryName(filename);
			bool flag3 = !string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName);
			if (flag3)
			{
				throw new ArgumentException("The specified directory does not exist.", "filename");
			}
			FileInfo fileInfo = new FileInfo(filename);
			bool flag4 = fileInfo.Exists && fileInfo.IsReadOnly;
			if (flag4)
			{
				throw new ArgumentException("Cannot write to the file because it is read-only.", "filename");
			}
			this.StartRecordingVideoToDisk_Internal(fileInfo.FullName, onStartedRecordingVideoCallback);
		}

		[NativeMethod("VideoCaptureBindings::StartRecordingVideoToDisk", HasExplicitThis = true)]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		private unsafe void StartRecordingVideoToDisk_Internal(string filename, VideoCapture.OnStartedRecordingVideoCallback onStartedRecordingVideoCallback)
		{
			try
			{
				IntPtr intPtr = VideoCapture.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filename.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				VideoCapture.StartRecordingVideoToDisk_Internal_Injected(intPtr, ref managedSpanWrapper, onStartedRecordingVideoCallback);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[RequiredByNativeCode]
		private static void InvokeOnStartedRecordingVideoToDiskDelegate(VideoCapture.OnStartedRecordingVideoCallback callback, long hResult)
		{
			callback(VideoCapture.MakeCaptureResult(hResult));
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeMethod("VideoCaptureBindings::StopRecordingVideoToDisk", HasExplicitThis = true)]
		public void StopRecordingAsync([NotNull] VideoCapture.OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback)
		{
			if (onStoppedRecordingVideoCallback == null)
			{
				ThrowHelper.ThrowArgumentNullException(onStoppedRecordingVideoCallback, "onStoppedRecordingVideoCallback");
			}
			IntPtr intPtr = VideoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoCapture.StopRecordingAsync_Injected(intPtr, onStoppedRecordingVideoCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnStoppedRecordingVideoToDiskDelegate(VideoCapture.OnStoppedRecordingVideoCallback callback, long hResult)
		{
			callback(VideoCapture.MakeCaptureResult(hResult));
		}

		[ThreadAndSerializationSafe]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeMethod("VideoCaptureBindings::GetUnsafePointerToVideoDeviceController", HasExplicitThis = true)]
		public IntPtr GetUnsafePointerToVideoDeviceController()
		{
			IntPtr intPtr = VideoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoCapture.GetUnsafePointerToVideoDeviceController_Injected(intPtr);
		}

		public void Dispose()
		{
			bool flag = this.m_NativePtr != IntPtr.Zero;
			if (flag)
			{
				this.Dispose_Internal();
				this.m_NativePtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		[NativeMethod("VideoCaptureBindings::Dispose", HasExplicitThis = true)]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		private void Dispose_Internal()
		{
			IntPtr intPtr = VideoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoCapture.Dispose_Internal_Injected(intPtr);
		}

		protected override void Finalize()
		{
			try
			{
				bool flag = this.m_NativePtr != IntPtr.Zero;
				if (flag)
				{
					this.DisposeThreaded_Internal();
					this.m_NativePtr = IntPtr.Zero;
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		[NativeMethod("VideoCaptureBindings::DisposeThreaded", HasExplicitThis = true)]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[ThreadAndSerializationSafe]
		private void DisposeThreaded_Internal()
		{
			IntPtr intPtr = VideoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoCapture.DisposeThreaded_Internal_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSupportedResolutions_Internal_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSupportedFrameRatesForResolution_Internal_Injected(int resolutionWidth, int resolutionHeight, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_IsRecording_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartVideoMode_Internal_Injected(IntPtr _unity_self, [In] ref CameraParameters cameraParameters, VideoCapture.AudioState audioState, VideoCapture.OnVideoModeStartedCallback onVideoModeStartedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopVideoModeAsync_Injected(IntPtr _unity_self, VideoCapture.OnVideoModeStoppedCallback onVideoModeStoppedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartRecordingVideoToDisk_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filename, VideoCapture.OnStartedRecordingVideoCallback onStartedRecordingVideoCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopRecordingAsync_Injected(IntPtr _unity_self, VideoCapture.OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetUnsafePointerToVideoDeviceController_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Dispose_Internal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisposeThreaded_Internal_Injected(IntPtr _unity_self);

		internal IntPtr m_NativePtr;

		private static Resolution[] s_SupportedResolutions;

		private static readonly long HR_SUCCESS;

		public enum CaptureResultType
		{
			Success,
			UnknownError
		}

		public enum AudioState
		{
			MicAudio,
			ApplicationAudio,
			ApplicationAndMicAudio,
			None
		}

		public struct VideoCaptureResult
		{
			public bool success
			{
				get
				{
					return this.resultType == VideoCapture.CaptureResultType.Success;
				}
			}

			public VideoCapture.CaptureResultType resultType;

			public long hResult;
		}

		public delegate void OnVideoCaptureResourceCreatedCallback(VideoCapture captureObject);

		public delegate void OnVideoModeStartedCallback(VideoCapture.VideoCaptureResult result);

		public delegate void OnVideoModeStoppedCallback(VideoCapture.VideoCaptureResult result);

		public delegate void OnStartedRecordingVideoCallback(VideoCapture.VideoCaptureResult result);

		public delegate void OnStoppedRecordingVideoCallback(VideoCapture.VideoCaptureResult result);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(VideoCapture videoCapture)
			{
				return videoCapture.m_NativePtr;
			}
		}
	}
}
