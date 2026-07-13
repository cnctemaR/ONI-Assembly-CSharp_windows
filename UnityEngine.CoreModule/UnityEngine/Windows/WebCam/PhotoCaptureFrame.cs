using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Windows.WebCam
{
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[MovedFrom("UnityEngine.XR.WSA.WebCam")]
	[NativeHeader("PlatformDependent/Win/Webcam/PhotoCaptureFrame.h")]
	public sealed class PhotoCaptureFrame : IDisposable
	{
		public int dataLength { get; private set; }

		public bool hasLocationData { get; private set; }

		public CapturePixelFormat pixelFormat { get; private set; }

		[ThreadAndSerializationSafe]
		private int GetDataLength()
		{
			IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PhotoCaptureFrame.GetDataLength_Injected(intPtr);
		}

		[ThreadAndSerializationSafe]
		private bool GetHasLocationData()
		{
			IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PhotoCaptureFrame.GetHasLocationData_Injected(intPtr);
		}

		[ThreadAndSerializationSafe]
		private CapturePixelFormat GetCapturePixelFormat()
		{
			IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PhotoCaptureFrame.GetCapturePixelFormat_Injected(intPtr);
		}

		public bool TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix)
		{
			cameraToWorldMatrix = Matrix4x4.identity;
			bool hasLocationData = this.hasLocationData;
			bool flag;
			if (hasLocationData)
			{
				cameraToWorldMatrix = this.GetCameraToWorldMatrix();
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		[NativeConditional("PLATFORM_WIN && !PLATFORM_XBOXONE", "Matrix4x4f()")]
		[ThreadAndSerializationSafe]
		[NativeName("GetCameraToWorld")]
		private Matrix4x4 GetCameraToWorldMatrix()
		{
			IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			PhotoCaptureFrame.GetCameraToWorldMatrix_Injected(intPtr, out matrix4x);
			return matrix4x;
		}

		public bool TryGetProjectionMatrix(out Matrix4x4 projectionMatrix)
		{
			bool hasLocationData = this.hasLocationData;
			bool flag;
			if (hasLocationData)
			{
				projectionMatrix = this.GetProjection();
				flag = true;
			}
			else
			{
				projectionMatrix = Matrix4x4.identity;
				flag = false;
			}
			return flag;
		}

		public bool TryGetProjectionMatrix(float nearClipPlane, float farClipPlane, out Matrix4x4 projectionMatrix)
		{
			bool hasLocationData = this.hasLocationData;
			bool flag3;
			if (hasLocationData)
			{
				float num = 0.01f;
				bool flag = nearClipPlane < num;
				if (flag)
				{
					nearClipPlane = num;
				}
				bool flag2 = farClipPlane < nearClipPlane + num;
				if (flag2)
				{
					farClipPlane = nearClipPlane + num;
				}
				projectionMatrix = this.GetProjection();
				float num2 = 1f / (farClipPlane - nearClipPlane);
				float num3 = -(farClipPlane + nearClipPlane) * num2;
				float num4 = -(2f * farClipPlane * nearClipPlane) * num2;
				projectionMatrix.m22 = num3;
				projectionMatrix.m23 = num4;
				flag3 = true;
			}
			else
			{
				projectionMatrix = Matrix4x4.identity;
				flag3 = false;
			}
			return flag3;
		}

		[NativeConditional("PLATFORM_WIN && !PLATFORM_XBOXONE", "Matrix4x4f()")]
		[ThreadAndSerializationSafe]
		private Matrix4x4 GetProjection()
		{
			IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Matrix4x4 matrix4x;
			PhotoCaptureFrame.GetProjection_Injected(intPtr, out matrix4x);
			return matrix4x;
		}

		public void UploadImageDataToTexture(Texture2D targetTexture)
		{
			bool flag = targetTexture == null;
			if (flag)
			{
				throw new ArgumentNullException("targetTexture");
			}
			bool flag2 = this.pixelFormat > CapturePixelFormat.BGRA32;
			if (flag2)
			{
				throw new ArgumentException("Uploading PhotoCaptureFrame to a texture is only supported with BGRA32 CameraFrameFormat!");
			}
			this.UploadImageDataToTexture_Internal(targetTexture);
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeName("UploadImageDataToTexture")]
		[ThreadAndSerializationSafe]
		private void UploadImageDataToTexture_Internal(Texture2D targetTexture)
		{
			IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PhotoCaptureFrame.UploadImageDataToTexture_Internal_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture2D>(targetTexture));
		}

		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[ThreadAndSerializationSafe]
		public IntPtr GetUnsafePointerToBuffer()
		{
			IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PhotoCaptureFrame.GetUnsafePointerToBuffer_Injected(intPtr);
		}

		public void CopyRawImageDataIntoBuffer(List<byte> byteBuffer)
		{
			bool flag = byteBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("byteBuffer");
			}
			byte[] array = new byte[this.dataLength];
			this.CopyRawImageDataIntoBuffer_Internal(array);
			bool flag2 = byteBuffer.Capacity < array.Length;
			if (flag2)
			{
				byteBuffer.Capacity = array.Length;
			}
			byteBuffer.Clear();
			byteBuffer.AddRange(array);
		}

		[ThreadAndSerializationSafe]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeName("CopyRawImageDataIntoBuffer")]
		internal unsafe void CopyRawImageDataIntoBuffer_Internal([Out] byte[] byteArray)
		{
			try
			{
				IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (byteArray != null)
				{
					fixed (byte[] array = byteArray)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				PhotoCaptureFrame.CopyRawImageDataIntoBuffer_Internal_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				byte[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
			}
		}

		internal PhotoCaptureFrame(IntPtr nativePtr)
		{
			this.m_NativePtr = nativePtr;
			this.dataLength = this.GetDataLength();
			this.hasLocationData = this.GetHasLocationData();
			this.pixelFormat = this.GetCapturePixelFormat();
			GC.AddMemoryPressure((long)this.dataLength);
		}

		private void Cleanup()
		{
			bool flag = this.m_NativePtr != IntPtr.Zero;
			if (flag)
			{
				GC.RemoveMemoryPressure((long)this.dataLength);
				this.Dispose_Internal();
				this.m_NativePtr = IntPtr.Zero;
			}
		}

		[NativeName("Dispose")]
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[ThreadAndSerializationSafe]
		private void Dispose_Internal()
		{
			IntPtr intPtr = PhotoCaptureFrame.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PhotoCaptureFrame.Dispose_Internal_Injected(intPtr);
		}

		public void Dispose()
		{
			this.Cleanup();
			GC.SuppressFinalize(this);
		}

		~PhotoCaptureFrame()
		{
			this.Cleanup();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDataLength_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHasLocationData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CapturePixelFormat GetCapturePixelFormat_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCameraToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetProjection_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UploadImageDataToTexture_Internal_Injected(IntPtr _unity_self, IntPtr targetTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetUnsafePointerToBuffer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyRawImageDataIntoBuffer_Internal_Injected(IntPtr _unity_self, out BlittableArrayWrapper byteArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Dispose_Internal_Injected(IntPtr _unity_self);

		private IntPtr m_NativePtr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(PhotoCaptureFrame photoCaptureFrame)
			{
				return photoCaptureFrame.m_NativePtr;
			}
		}
	}
}
