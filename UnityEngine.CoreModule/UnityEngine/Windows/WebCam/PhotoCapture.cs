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
	[NativeHeader("PlatformDependent/Win/Webcam/PhotoCapture.h")]
	[StaticAccessor("PhotoCapture", StaticAccessorType.DoubleColon)]
	[StructLayout(LayoutKind.Sequential)]
	public class PhotoCapture : IDisposable
	{
		private static PhotoCapture.PhotoCaptureResult MakeCaptureResult(PhotoCapture.CaptureResultType resultType, long hResult)
		{
			return new PhotoCapture.PhotoCaptureResult
			{
				resultType = resultType,
				hResult = hResult
			};
		}

		private static PhotoCapture.PhotoCaptureResult MakeCaptureResult(long hResult)
		{
			PhotoCapture.PhotoCaptureResult photoCaptureResult = default(PhotoCapture.PhotoCaptureResult);
			bool flag = hResult == PhotoCapture.HR_SUCCESS;
			PhotoCapture.CaptureResultType captureResultType;
			if (flag)
			{
				captureResultType = PhotoCapture.CaptureResultType.Success;
			}
			else
			{
				captureResultType = PhotoCapture.CaptureResultType.UnknownError;
			}
			photoCaptureResult.resultType = captureResultType;
			photoCaptureResult.hResult = hResult;
			return photoCaptureResult;
		}

		public static IEnumerable<Resolution> SupportedResolutions
		{
			get
			{
				bool flag = PhotoCapture.s_SupportedResolutions == null;
				if (flag)
				{
					PhotoCapture.s_SupportedResolutions = PhotoCapture.GetSupportedResolutions_Internal();
				}
				return PhotoCapture.s_SupportedResolutions;
			}
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeName("GetSupportedResolutions")]
		private static Resolution[] GetSupportedResolutions_Internal()
		{
			Resolution[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PhotoCapture.GetSupportedResolutions_Internal_Injected(out blittableArrayWrapper);
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

		public static void CreateAsync(bool showHolograms, PhotoCapture.OnCaptureResourceCreatedCallback onCreatedCallback)
		{
			bool flag = onCreatedCallback == null;
			if (flag)
			{
				throw new ArgumentNullException("onCreatedCallback");
			}
			PhotoCapture.Instantiate_Internal(showHolograms, onCreatedCallback);
		}

		public static void CreateAsync(PhotoCapture.OnCaptureResourceCreatedCallback onCreatedCallback)
		{
			bool flag = onCreatedCallback == null;
			if (flag)
			{
				throw new ArgumentNullException("onCreatedCallback");
			}
			PhotoCapture.Instantiate_Internal(false, onCreatedCallback);
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeName("Instantiate")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Instantiate_Internal(bool showHolograms, PhotoCapture.OnCaptureResourceCreatedCallback onCreatedCallback);

		[RequiredByNativeCode]
		private static void InvokeOnCreatedResourceDelegate(PhotoCapture.OnCaptureResourceCreatedCallback callback, IntPtr nativePtr)
		{
			bool flag = nativePtr == IntPtr.Zero;
			if (flag)
			{
				callback(null);
			}
			else
			{
				callback(new PhotoCapture(nativePtr));
			}
		}

		private PhotoCapture(IntPtr nativeCaptureObject)
		{
			this.m_NativePtr = nativeCaptureObject;
		}

		public void StartPhotoModeAsync(CameraParameters setupParams, PhotoCapture.OnPhotoModeStartedCallback onPhotoModeStartedCallback)
		{
			bool flag = onPhotoModeStartedCallback == null;
			if (flag)
			{
				throw new ArgumentException("onPhotoModeStartedCallback");
			}
			bool flag2 = setupParams.cameraResolutionWidth == 0 || setupParams.cameraResolutionHeight == 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("setupParams", "The camera resolution must be set to a supported resolution.");
			}
			this.StartPhotoMode_Internal(setupParams, onPhotoModeStartedCallback);
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeName("StartPhotoMode")]
		private void StartPhotoMode_Internal(CameraParameters setupParams, PhotoCapture.OnPhotoModeStartedCallback onPhotoModeStartedCallback)
		{
			IntPtr intPtr = PhotoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PhotoCapture.StartPhotoMode_Internal_Injected(intPtr, ref setupParams, onPhotoModeStartedCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnPhotoModeStartedDelegate(PhotoCapture.OnPhotoModeStartedCallback callback, long hResult)
		{
			callback(PhotoCapture.MakeCaptureResult(hResult));
		}

		[NativeName("StopPhotoMode")]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		public void StopPhotoModeAsync(PhotoCapture.OnPhotoModeStoppedCallback onPhotoModeStoppedCallback)
		{
			IntPtr intPtr = PhotoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PhotoCapture.StopPhotoModeAsync_Injected(intPtr, onPhotoModeStoppedCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnPhotoModeStoppedDelegate(PhotoCapture.OnPhotoModeStoppedCallback callback, long hResult)
		{
			callback(PhotoCapture.MakeCaptureResult(hResult));
		}

		public void TakePhotoAsync(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, PhotoCapture.OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
		{
			bool flag = onCapturedPhotoToDiskCallback == null;
			if (flag)
			{
				throw new ArgumentNullException("onCapturedPhotoToDiskCallback");
			}
			bool flag2 = string.IsNullOrEmpty(filename);
			if (flag2)
			{
				throw new ArgumentNullException("filename");
			}
			filename = filename.Replace("/", "\\");
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
			this.CapturePhotoToDisk_Internal(filename, fileOutputFormat, onCapturedPhotoToDiskCallback);
		}

		[NativeName("CapturePhotoToDisk")]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		private unsafe void CapturePhotoToDisk_Internal(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, PhotoCapture.OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
		{
			try
			{
				IntPtr intPtr = PhotoCapture.BindingsMarshaller.ConvertToNative(this);
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
				PhotoCapture.CapturePhotoToDisk_Internal_Injected(intPtr, ref managedSpanWrapper, fileOutputFormat, onCapturedPhotoToDiskCallback);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[RequiredByNativeCode]
		private static void InvokeOnCapturedPhotoToDiskDelegate(PhotoCapture.OnCapturedToDiskCallback callback, long hResult)
		{
			callback(PhotoCapture.MakeCaptureResult(hResult));
		}

		public void TakePhotoAsync(PhotoCapture.OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
		{
			bool flag = onCapturedPhotoToMemoryCallback == null;
			if (flag)
			{
				throw new ArgumentNullException("onCapturedPhotoToMemoryCallback");
			}
			this.CapturePhotoToMemory_Internal(onCapturedPhotoToMemoryCallback);
		}

		[NativeName("CapturePhotoToMemory")]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		private void CapturePhotoToMemory_Internal(PhotoCapture.OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
		{
			IntPtr intPtr = PhotoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PhotoCapture.CapturePhotoToMemory_Internal_Injected(intPtr, onCapturedPhotoToMemoryCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnCapturedPhotoToMemoryDelegate(PhotoCapture.OnCapturedToMemoryCallback callback, long hResult, IntPtr photoCaptureFramePtr)
		{
			PhotoCaptureFrame photoCaptureFrame = null;
			bool flag = photoCaptureFramePtr != IntPtr.Zero;
			if (flag)
			{
				photoCaptureFrame = new PhotoCaptureFrame(photoCaptureFramePtr);
			}
			callback(PhotoCapture.MakeCaptureResult(hResult), photoCaptureFrame);
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[ThreadAndSerializationSafe]
		[NativeName("GetUnsafePointerToVideoDeviceController")]
		public IntPtr GetUnsafePointerToVideoDeviceController()
		{
			IntPtr intPtr = PhotoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PhotoCapture.GetUnsafePointerToVideoDeviceController_Injected(intPtr);
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

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeName("Dispose")]
		private void Dispose_Internal()
		{
			IntPtr intPtr = PhotoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PhotoCapture.Dispose_Internal_Injected(intPtr);
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

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeName("DisposeThreaded")]
		[ThreadAndSerializationSafe]
		private void DisposeThreaded_Internal()
		{
			IntPtr intPtr = PhotoCapture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PhotoCapture.DisposeThreaded_Internal_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSupportedResolutions_Internal_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartPhotoMode_Internal_Injected(IntPtr _unity_self, [In] ref CameraParameters setupParams, PhotoCapture.OnPhotoModeStartedCallback onPhotoModeStartedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopPhotoModeAsync_Injected(IntPtr _unity_self, PhotoCapture.OnPhotoModeStoppedCallback onPhotoModeStoppedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CapturePhotoToDisk_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filename, PhotoCaptureFileOutputFormat fileOutputFormat, PhotoCapture.OnCapturedToDiskCallback onCapturedPhotoToDiskCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CapturePhotoToMemory_Internal_Injected(IntPtr _unity_self, PhotoCapture.OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback);

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

		public struct PhotoCaptureResult
		{
			public bool success
			{
				get
				{
					return this.resultType == PhotoCapture.CaptureResultType.Success;
				}
			}

			public PhotoCapture.CaptureResultType resultType;

			public long hResult;
		}

		public delegate void OnCaptureResourceCreatedCallback(PhotoCapture captureObject);

		public delegate void OnPhotoModeStartedCallback(PhotoCapture.PhotoCaptureResult result);

		public delegate void OnPhotoModeStoppedCallback(PhotoCapture.PhotoCaptureResult result);

		public delegate void OnCapturedToDiskCallback(PhotoCapture.PhotoCaptureResult result);

		public delegate void OnCapturedToMemoryCallback(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(PhotoCapture photoCapture)
			{
				return photoCapture.m_NativePtr;
			}
		}
	}
}
