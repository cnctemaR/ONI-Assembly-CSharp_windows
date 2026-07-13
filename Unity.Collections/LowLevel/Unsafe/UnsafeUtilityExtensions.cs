using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public static class UnsafeUtilityExtensions
	{
		internal unsafe static void MemSwap(void* ptr, void* otherPtr, long size)
		{
			byte* ptr2 = (byte*)ptr;
			byte* ptr3 = (byte*)otherPtr;
			byte* ptr4 = stackalloc byte[(UIntPtr)1024];
			while (size > 0L)
			{
				long num = math.min(size, 1024L);
				UnsafeUtility.MemCpy((void*)ptr4, (void*)ptr2, num);
				UnsafeUtility.MemCpy((void*)ptr2, (void*)ptr3, num);
				UnsafeUtility.MemCpy((void*)ptr3, (void*)ptr4, num);
				size -= num;
				ptr3 += num;
				ptr2 += num;
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static T ReadArrayElementBoundsChecked<T>(void* source, int index, int capacity)
		{
			return UnsafeUtility.ReadArrayElement<T>(source, index);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void WriteArrayElementBoundsChecked<T>(void* destination, int index, T value, int capacity)
		{
			UnsafeUtility.WriteArrayElement<T>(destination, index, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* AddressOf<T>(in T value) where T : struct
		{
			return ILSupport.AddressOf<T>(in value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<T>(in T value) where T : struct
		{
			return ILSupport.AsRef<T>(in value);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private unsafe static void CheckMemSwapOverlap(byte* dst, byte* src, long size)
		{
			if (dst + size != src && src + size != dst)
			{
				throw new InvalidOperationException("MemSwap memory blocks are overlapped.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckIndexRange(int index, int capacity)
		{
			if (index > capacity - 1 || index < 0)
			{
				throw new IndexOutOfRangeException(string.Format("Attempt to read or write from array index {0}, which is out of bounds. Array capacity is {1}. ", index, capacity) + "This may lead to a crash, data corruption, or reading invalid data.");
			}
		}
	}
}
