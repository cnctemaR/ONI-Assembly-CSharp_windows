using System;
using System.Diagnostics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public struct UnsafeScratchAllocator
	{
		public unsafe UnsafeScratchAllocator(void* ptr, int capacityInBytes)
		{
			this.m_Pointer = ptr;
			this.m_LengthInBytes = 0;
			this.m_CapacityInBytes = capacityInBytes;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckAllocationDoesNotExceedCapacity(ulong requestedSize)
		{
			if (requestedSize > (ulong)((long)this.m_CapacityInBytes))
			{
				throw new ArgumentException(string.Format("Cannot allocate more than provided size in UnsafeScratchAllocator. Requested: {0} Size: {1} Capacity: {2}", requestedSize, this.m_LengthInBytes, this.m_CapacityInBytes));
			}
		}

		public unsafe void* Allocate(int sizeInBytes, int alignmentInBytes)
		{
			if (sizeInBytes == 0)
			{
				return null;
			}
			ulong num = (ulong)((long)(alignmentInBytes - 1));
			long num2 = ((long)((IntPtr)this.m_Pointer) + (long)this.m_LengthInBytes + (long)num) & (long)(~(long)num);
			long num3 = (long)((byte*)(void*)((IntPtr)num2) - (byte*)this.m_Pointer);
			num3 += (long)sizeInBytes;
			this.m_LengthInBytes = (int)num3;
			return (void*)((IntPtr)num2);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void* Allocate<T>(int count = 1) where T : struct
		{
			return this.Allocate(UnsafeUtility.SizeOf<T>() * count, UnsafeUtility.AlignOf<T>());
		}

		private unsafe void* m_Pointer;

		private int m_LengthInBytes;

		private readonly int m_CapacityInBytes;
	}
}
