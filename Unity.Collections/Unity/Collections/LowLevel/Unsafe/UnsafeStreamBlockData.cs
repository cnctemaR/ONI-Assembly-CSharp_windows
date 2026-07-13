using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	internal struct UnsafeStreamBlockData
	{
		internal unsafe UnsafeStreamBlock* Allocate(UnsafeStreamBlock* oldBlock, int threadIndex)
		{
			UnsafeStreamBlock* ptr = (UnsafeStreamBlock*)Memory.Unmanaged.Array.Resize(null, 0L, 4096L, this.Allocator, 1L, 16);
			ptr->Next = null;
			if (oldBlock == null)
			{
				ptr->Next = *(IntPtr*)(this.Blocks + (IntPtr)threadIndex * (IntPtr)sizeof(UnsafeStreamBlock*) / (IntPtr)sizeof(UnsafeStreamBlock*));
				*(IntPtr*)(this.Blocks + (IntPtr)threadIndex * (IntPtr)sizeof(UnsafeStreamBlock*) / (IntPtr)sizeof(UnsafeStreamBlock*)) = ptr;
			}
			else
			{
				ptr->Next = oldBlock->Next;
				oldBlock->Next = ptr;
			}
			return ptr;
		}

		internal unsafe void Free(UnsafeStreamBlock* oldBlock)
		{
			Memory.Unmanaged.Array.Resize((void*)oldBlock, 4096L, 0L, this.Allocator, 1L, 16);
		}

		internal const int AllocationSize = 4096;

		internal AllocatorManager.AllocatorHandle Allocator;

		internal unsafe UnsafeStreamBlock** Blocks;

		internal int BlockCount;

		internal AllocatorManager.Block Ranges;

		internal int RangeCount;
	}
}
