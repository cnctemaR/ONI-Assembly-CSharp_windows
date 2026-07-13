using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine
{
	[NativeHeader("Modules/ImageConversion/ScriptBindings/ImageConversion.bindings.h")]
	public static class ImageConversion
	{
		public static bool EnableLegacyPngGammaRuntimeLoadBehavior
		{
			get
			{
				return ImageConversion.GetEnableLegacyPngGammaRuntimeLoadBehavior();
			}
			set
			{
				ImageConversion.SetEnableLegacyPngGammaRuntimeLoadBehavior(value);
			}
		}

		[NativeMethod(Name = "ImageConversionBindings::GetEnableLegacyPngGammaRuntimeLoadBehavior", IsFreeFunction = true, ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetEnableLegacyPngGammaRuntimeLoadBehavior();

		[NativeMethod(Name = "ImageConversionBindings::SetEnableLegacyPngGammaRuntimeLoadBehavior", IsFreeFunction = true, ThrowsException = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEnableLegacyPngGammaRuntimeLoadBehavior(bool enable);

		[NativeMethod(Name = "ImageConversionBindings::EncodeToTGA", IsFreeFunction = true, ThrowsException = true)]
		public static byte[] EncodeToTGA(this Texture2D tex)
		{
			byte[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeToTGA_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(tex), out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "ImageConversionBindings::EncodeToPNG", IsFreeFunction = true, ThrowsException = true)]
		public static byte[] EncodeToPNG(this Texture2D tex)
		{
			byte[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeToPNG_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(tex), out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "ImageConversionBindings::EncodeToJPG", IsFreeFunction = true, ThrowsException = true)]
		public static byte[] EncodeToJPG(this Texture2D tex, int quality)
		{
			byte[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeToJPG_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(tex), quality, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static byte[] EncodeToJPG(this Texture2D tex)
		{
			return tex.EncodeToJPG(75);
		}

		[NativeMethod(Name = "ImageConversionBindings::EncodeToEXR", IsFreeFunction = true, ThrowsException = true)]
		public static byte[] EncodeToEXR(this Texture2D tex, Texture2D.EXRFlags flags)
		{
			byte[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeToEXR_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(tex), flags, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static byte[] EncodeToEXR(this Texture2D tex)
		{
			return tex.EncodeToEXR(Texture2D.EXRFlags.None);
		}

		[NativeMethod(Name = "ImageConversionBindings::EncodeToR2D", IsFreeFunction = true, ThrowsException = true)]
		internal static byte[] EncodeToR2DInternal(this Texture2D tex)
		{
			byte[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeToR2DInternal_Injected(Object.MarshalledUnityObject.Marshal<Texture2D>(tex), out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeMethod(Name = "ImageConversionBindings::LoadImage", IsFreeFunction = true)]
		public unsafe static bool LoadImage([NotNull] this Texture2D tex, ReadOnlySpan<byte> data, bool markNonReadable)
		{
			if (tex == null)
			{
				ThrowHelper.ThrowArgumentNullException(tex, "tex");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(tex);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(tex, "tex");
			}
			ReadOnlySpan<byte> readOnlySpan = data;
			bool flag;
			fixed (byte* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				flag = ImageConversion.LoadImage_Injected(intPtr, ref managedSpanWrapper, markNonReadable);
			}
			return flag;
		}

		public static bool LoadImage(this Texture2D tex, ReadOnlySpan<byte> data)
		{
			return tex.LoadImage(data, false);
		}

		public static bool LoadImage(this Texture2D tex, byte[] data, bool markNonReadable)
		{
			return tex.LoadImage(new ReadOnlySpan<byte>(data), markNonReadable);
		}

		public static bool LoadImage(this Texture2D tex, byte[] data)
		{
			return tex.LoadImage(new ReadOnlySpan<byte>(data), false);
		}

		[FreeFunction("ImageConversionBindings::EncodeArrayToTGA", true)]
		public static byte[] EncodeArrayToTGA(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U)
		{
			byte[] array3;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeArrayToTGA_Injected(array, format, width, height, rowBytes, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array2;
				blittableArrayWrapper.Unmarshal<byte>(ref array2);
				array3 = array2;
			}
			return array3;
		}

		[FreeFunction("ImageConversionBindings::EncodeArrayToPNG", true)]
		public static byte[] EncodeArrayToPNG(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U)
		{
			byte[] array3;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeArrayToPNG_Injected(array, format, width, height, rowBytes, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array2;
				blittableArrayWrapper.Unmarshal<byte>(ref array2);
				array3 = array2;
			}
			return array3;
		}

		[FreeFunction("ImageConversionBindings::EncodeArrayToJPG", true)]
		public static byte[] EncodeArrayToJPG(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U, int quality = 75)
		{
			byte[] array3;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeArrayToJPG_Injected(array, format, width, height, rowBytes, quality, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array2;
				blittableArrayWrapper.Unmarshal<byte>(ref array2);
				array3 = array2;
			}
			return array3;
		}

		[FreeFunction("ImageConversionBindings::EncodeArrayToEXR", true)]
		public static byte[] EncodeArrayToEXR(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U, Texture2D.EXRFlags flags = Texture2D.EXRFlags.None)
		{
			byte[] array3;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeArrayToEXR_Injected(array, format, width, height, rowBytes, flags, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array2;
				blittableArrayWrapper.Unmarshal<byte>(ref array2);
				array3 = array2;
			}
			return array3;
		}

		[FreeFunction("ImageConversionBindings::EncodeArrayToR2D", true)]
		internal static byte[] EncodeArrayToR2DInternal(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U)
		{
			byte[] array3;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ImageConversion.EncodeArrayToR2DInternal_Injected(array, format, width, height, rowBytes, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array2;
				blittableArrayWrapper.Unmarshal<byte>(ref array2);
				array3 = array2;
			}
			return array3;
		}

		public unsafe static NativeArray<byte> EncodeNativeArrayToTGA<T>(NativeArray<T> input, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U) where T : struct
		{
			int num = input.Length * UnsafeUtility.SizeOf<T>();
			void* ptr = ImageConversion.UnsafeEncodeNativeArrayToTGA(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(input), ref num, format, width, height, rowBytes);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(ptr, num, Allocator.Persistent);
		}

		public unsafe static NativeArray<byte> EncodeNativeArrayToPNG<T>(NativeArray<T> input, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U) where T : struct
		{
			int num = input.Length * UnsafeUtility.SizeOf<T>();
			void* ptr = ImageConversion.UnsafeEncodeNativeArrayToPNG(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(input), ref num, format, width, height, rowBytes);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(ptr, num, Allocator.Persistent);
		}

		public unsafe static NativeArray<byte> EncodeNativeArrayToJPG<T>(NativeArray<T> input, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U, int quality = 75) where T : struct
		{
			int num = input.Length * UnsafeUtility.SizeOf<T>();
			void* ptr = ImageConversion.UnsafeEncodeNativeArrayToJPG(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(input), ref num, format, width, height, rowBytes, quality);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(ptr, num, Allocator.Persistent);
		}

		public unsafe static NativeArray<byte> EncodeNativeArrayToEXR<T>(NativeArray<T> input, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U, Texture2D.EXRFlags flags = Texture2D.EXRFlags.None) where T : struct
		{
			int num = input.Length * UnsafeUtility.SizeOf<T>();
			void* ptr = ImageConversion.UnsafeEncodeNativeArrayToEXR(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(input), ref num, format, width, height, rowBytes, flags);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(ptr, num, Allocator.Persistent);
		}

		internal unsafe static NativeArray<byte> EncodeNativeArrayToR2DInternal<T>(NativeArray<T> input, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U) where T : struct
		{
			int num = input.Length * UnsafeUtility.SizeOf<T>();
			void* ptr = ImageConversion.UnsafeEncodeNativeArrayToR2D(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(input), ref num, format, width, height, rowBytes);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(ptr, num, Allocator.Persistent);
		}

		[FreeFunction("ImageConversionBindings::UnsafeEncodeNativeArrayToTGA", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* UnsafeEncodeNativeArrayToTGA(void* array, ref int sizeInBytes, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U);

		[FreeFunction("ImageConversionBindings::UnsafeEncodeNativeArrayToPNG", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* UnsafeEncodeNativeArrayToPNG(void* array, ref int sizeInBytes, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U);

		[FreeFunction("ImageConversionBindings::UnsafeEncodeNativeArrayToJPG", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* UnsafeEncodeNativeArrayToJPG(void* array, ref int sizeInBytes, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U, int quality = 75);

		[FreeFunction("ImageConversionBindings::UnsafeEncodeNativeArrayToEXR", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* UnsafeEncodeNativeArrayToEXR(void* array, ref int sizeInBytes, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U, Texture2D.EXRFlags flags = Texture2D.EXRFlags.None);

		[FreeFunction("ImageConversionBindings::UnsafeEncodeNativeArrayToR2D", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* UnsafeEncodeNativeArrayToR2D(void* array, ref int sizeInBytes, GraphicsFormat format, uint width, uint height, uint rowBytes = 0U);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeToTGA_Injected(IntPtr tex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeToPNG_Injected(IntPtr tex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeToJPG_Injected(IntPtr tex, int quality, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeToEXR_Injected(IntPtr tex, Texture2D.EXRFlags flags, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeToR2DInternal_Injected(IntPtr tex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool LoadImage_Injected(IntPtr tex, ref ManagedSpanWrapper data, bool markNonReadable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeArrayToTGA_Injected(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeArrayToPNG_Injected(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeArrayToJPG_Injected(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes, int quality, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeArrayToEXR_Injected(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes, Texture2D.EXRFlags flags, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EncodeArrayToR2DInternal_Injected(Array array, GraphicsFormat format, uint width, uint height, uint rowBytes, out BlittableArrayWrapper ret);
	}
}
