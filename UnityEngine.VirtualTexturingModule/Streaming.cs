using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering.VirtualTexturing
{
	[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
	[StaticAccessor("VirtualTexturing::Streaming", StaticAccessorType.DoubleColon)]
	public static class Streaming
	{
		[NativeThrows]
		public static void RequestRegion([NotNull] Material mat, int stackNameId, Rect r, int mipMap, int numMips)
		{
			if (mat == null)
			{
				ThrowHelper.ThrowArgumentNullException(mat, "mat");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(mat);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mat, "mat");
			}
			Streaming.RequestRegion_Injected(intPtr, stackNameId, ref r, mipMap, numMips);
		}

		[NativeThrows]
		public static void GetTextureStackSize([NotNull] Material mat, int stackNameId, out int width, out int height)
		{
			if (mat == null)
			{
				ThrowHelper.ThrowArgumentNullException(mat, "mat");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(mat);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mat, "mat");
			}
			Streaming.GetTextureStackSize_Injected(intPtr, stackNameId, out width, out height);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCPUCacheSize(int sizeInMegabytes);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCPUCacheSize();

		[NativeThrows]
		public unsafe static void SetGPUCacheSettings(GPUCacheSetting[] cacheSettings)
		{
			Span<GPUCacheSetting> span = new Span<GPUCacheSetting>(cacheSettings);
			fixed (GPUCacheSetting* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Streaming.SetGPUCacheSettings_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public static GPUCacheSetting[] GetGPUCacheSettings()
		{
			GPUCacheSetting[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Streaming.GetGPUCacheSettings_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				GPUCacheSetting[] array;
				blittableArrayWrapper.Unmarshal<GPUCacheSetting>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EnableMipPreloading(int texturesPerFrame, int mipCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RequestRegion_Injected(IntPtr mat, int stackNameId, [In] ref Rect r, int mipMap, int numMips);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTextureStackSize_Injected(IntPtr mat, int stackNameId, out int width, out int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGPUCacheSettings_Injected(ref ManagedSpanWrapper cacheSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGPUCacheSettings_Injected(out BlittableArrayWrapper ret);
	}
}
