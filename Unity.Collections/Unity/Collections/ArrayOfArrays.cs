using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	internal struct ArrayOfArrays<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : IDisposable where T : struct, ValueType
	{
		private int BlockSizeInElements
		{
			get
			{
				return 1 << this.m_log2BlockSizeInElements;
			}
		}

		private int BlockSizeInBytes
		{
			get
			{
				return this.BlockSizeInElements * sizeof(T);
			}
		}

		private int BlockMask
		{
			get
			{
				return this.BlockSizeInElements - 1;
			}
		}

		public int Length
		{
			get
			{
				return this.m_lengthInElements;
			}
		}

		public int Capacity
		{
			get
			{
				return this.m_capacityInElements;
			}
		}

		public unsafe ArrayOfArrays(int capacityInElements, AllocatorManager.AllocatorHandle backingAllocatorHandle, int log2BlockSizeInElements = 12)
		{
			this = default(ArrayOfArrays<T>);
			this.m_backingAllocatorHandle = backingAllocatorHandle;
			this.m_lengthInElements = 0;
			this.m_capacityInElements = capacityInElements;
			this.m_log2BlockSizeInElements = log2BlockSizeInElements;
			this.m_blocks = capacityInElements + this.BlockMask >> this.m_log2BlockSizeInElements;
			this.m_block = (IntPtr*)Memory.Unmanaged.Allocate((long)(sizeof(IntPtr) * this.m_blocks), 16, this.m_backingAllocatorHandle);
			UnsafeUtility.MemSet((void*)this.m_block, 0, (long)(sizeof(IntPtr) * this.m_blocks));
		}

		public unsafe void LockfreeAdd(T t)
		{
			int num = Interlocked.Increment(ref this.m_lengthInElements) - 1;
			int num2 = this.BlockIndexOfElement(num);
			if (this.m_block[(IntPtr)num2 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] == IntPtr.Zero)
			{
				void* ptr = Memory.Unmanaged.Allocate((long)this.BlockSizeInBytes, 16, this.m_backingAllocatorHandle);
				int num3 = math.min(this.m_blocks, num2 + 4);
				while (num2 < num3 && !(IntPtr.Zero == Interlocked.CompareExchange(ref this.m_block[(IntPtr)num2 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], (IntPtr)ptr, IntPtr.Zero)))
				{
					num2++;
				}
				if (num2 == num3)
				{
					Memory.Unmanaged.Free(ptr, this.m_backingAllocatorHandle);
				}
			}
			*this[num] = t;
		}

		public unsafe ref T this[int elementIndex]
		{
			get
			{
				int num = this.BlockIndexOfElement(elementIndex);
				IntPtr intPtr = this.m_block[(IntPtr)num * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)];
				int num2 = elementIndex & this.BlockMask;
				T* ptr = (T*)(void*)intPtr;
				return ref ptr[(IntPtr)num2 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			}
		}

		public void Rewind()
		{
			this.m_lengthInElements = 0;
		}

		public unsafe void Clear()
		{
			this.Rewind();
			for (int i = 0; i < this.m_blocks; i++)
			{
				if (this.m_block[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] != IntPtr.Zero)
				{
					Memory.Unmanaged.Free((void*)this.m_block[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], this.m_backingAllocatorHandle);
					this.m_block[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = IntPtr.Zero;
				}
			}
		}

		public void Dispose()
		{
			this.Clear();
			Memory.Unmanaged.Free<IntPtr>(this.m_block, this.m_backingAllocatorHandle);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void CheckElementIndex(int elementIndex)
		{
			if (elementIndex >= this.m_lengthInElements)
			{
				throw new ArgumentException(string.Format("Element index {0} must be less than length in elements {1}.", elementIndex, this.m_lengthInElements));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void CheckBlockIndex(int blockIndex)
		{
			if (blockIndex >= this.m_blocks)
			{
				throw new ArgumentException(string.Format("Block index {0} must be less than number of blocks {1}.", blockIndex, this.m_blocks));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private unsafe void CheckBlockIsNotNull(int blockIndex)
		{
			if (this.m_block[(IntPtr)blockIndex * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] == IntPtr.Zero)
			{
				throw new ArgumentException(string.Format("Block index {0} is a null pointer.", blockIndex));
			}
		}

		public unsafe void RemoveAtSwapBack(int elementIndex)
		{
			*this[elementIndex] = *this[this.Length - 1];
			this.m_lengthInElements--;
		}

		private int BlockIndexOfElement(int elementIndex)
		{
			return elementIndex >> this.m_log2BlockSizeInElements;
		}

		public unsafe void TrimExcess()
		{
			for (int i = this.BlockIndexOfElement(this.m_lengthInElements + this.BlockMask); i < this.m_blocks; i++)
			{
				if (this.m_block[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] != IntPtr.Zero)
				{
					Memory.Unmanaged.Free((void*)this.m_block[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], this.m_backingAllocatorHandle);
					this.m_block[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = IntPtr.Zero;
				}
			}
		}

		private AllocatorManager.AllocatorHandle m_backingAllocatorHandle;

		private int m_lengthInElements;

		private int m_capacityInElements;

		private int m_log2BlockSizeInElements;

		private int m_blocks;

		private unsafe IntPtr* m_block;
	}
}
