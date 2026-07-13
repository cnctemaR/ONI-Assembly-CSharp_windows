using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	public static class NativeArrayUnsafeUtility
	{
		public unsafe static NativeArray<T> ConvertExistingDataToNativeArray<T>(void* dataPointer, int length, Allocator allocator) where T : struct
		{
			return new NativeArray<T>
			{
				m_Buffer = dataPointer,
				m_Length = length,
				m_AllocatorLabel = allocator
			};
		}

		public unsafe static NativeArray<T> ConvertExistingDataToNativeArray<[IsUnmanaged] T>(Span<T> data, Allocator allocator) where T : struct, ValueType
		{
			fixed (T* pinnableReference = data.GetPinnableReference())
			{
				T* ptr = pinnableReference;
				return new NativeArray<T>
				{
					m_Buffer = (void*)ptr,
					m_Length = data.Length,
					m_AllocatorLabel = allocator
				};
			}
		}

		public unsafe static void* GetUnsafePtr<T>(this NativeArray<T> nativeArray) where T : struct
		{
			return nativeArray.m_Buffer;
		}

		public unsafe static void* GetUnsafeReadOnlyPtr<T>(this NativeArray<T> nativeArray) where T : struct
		{
			return nativeArray.m_Buffer;
		}

		public unsafe static void* GetUnsafeReadOnlyPtr<T>(this NativeArray<T>.ReadOnly nativeArray) where T : struct
		{
			return nativeArray.m_Buffer;
		}

		public unsafe static void* GetUnsafeBufferPointerWithoutChecks<T>(NativeArray<T> nativeArray) where T : struct
		{
			return nativeArray.m_Buffer;
		}
	}
}
