using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	internal struct UnsafeStreamBlockData
	{
		internal unsafe UnsafeStreamBlock* Allocate(UnsafeStreamBlock* oldBlock, int threadIndex)
		{
			UnsafeStreamBlock* ptr = (UnsafeStreamBlock*)Memory.Unmanaged.Allocate(4096L, 16, this.Allocator);
			ptr->Next = null;
			if (oldBlock == null)
			{
				ptr->Next = *(IntPtr*)(this.Blocks + (IntPtr)threadIndex * (IntPtr)sizeof(UnsafeStreamBlock*) / (IntPtr)sizeof(UnsafeStreamBlock*));
				*(IntPtr*)(this.Blocks + (IntPtr)threadIndex * (IntPtr)sizeof(UnsafeStreamBlock*) / (IntPtr)sizeof(UnsafeStreamBlock*)) = ptr;
			}
			else
			{
				oldBlock->Next = ptr;
			}
			return ptr;
		}

		internal const int AllocationSize = 4096;

		internal AllocatorManager.AllocatorHandle Allocator;

		internal unsafe UnsafeStreamBlock** Blocks;

		internal int BlockCount;

		internal unsafe UnsafeStreamBlock* Free;

		internal unsafe UnsafeStreamRange* Ranges;

		internal int RangeCount;
	}
}
