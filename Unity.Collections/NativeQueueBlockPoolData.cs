using System;
using System.Threading;

namespace Unity.Collections
{
	[BurstCompatible]
	internal struct NativeQueueBlockPoolData
	{
		public unsafe NativeQueueBlockHeader* AllocateBlock()
		{
			while (Interlocked.CompareExchange(ref this.m_AllocLock, 1, 0) != 0)
			{
			}
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)this.m_FirstBlock;
			NativeQueueBlockHeader* ptr2;
			for (;;)
			{
				ptr2 = ptr;
				if (ptr2 == null)
				{
					break;
				}
				ptr = (NativeQueueBlockHeader*)(void*)Interlocked.CompareExchange(ref this.m_FirstBlock, (IntPtr)((void*)ptr2->m_NextBlock), (IntPtr)((void*)ptr2));
				if (ptr == ptr2)
				{
					goto Block_2;
				}
			}
			Interlocked.Exchange(ref this.m_AllocLock, 0);
			Interlocked.Increment(ref this.m_NumBlocks);
			return (NativeQueueBlockHeader*)Memory.Unmanaged.Allocate(16384L, 16, Allocator.Persistent);
			Block_2:
			Interlocked.Exchange(ref this.m_AllocLock, 0);
			return ptr2;
		}

		public unsafe void FreeBlock(NativeQueueBlockHeader* block)
		{
			if (this.m_NumBlocks > this.m_MaxBlocks)
			{
				if (Interlocked.Decrement(ref this.m_NumBlocks) + 1 > this.m_MaxBlocks)
				{
					Memory.Unmanaged.Free<NativeQueueBlockHeader>(block, Allocator.Persistent);
					return;
				}
				Interlocked.Increment(ref this.m_NumBlocks);
			}
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)this.m_FirstBlock;
			NativeQueueBlockHeader* ptr2;
			do
			{
				ptr2 = ptr;
				block->m_NextBlock = ptr;
				ptr = (NativeQueueBlockHeader*)(void*)Interlocked.CompareExchange(ref this.m_FirstBlock, (IntPtr)((void*)block), (IntPtr)((void*)ptr));
			}
			while (ptr != ptr2);
		}

		internal IntPtr m_FirstBlock;

		internal int m_NumBlocks;

		internal int m_MaxBlocks;

		internal const int m_BlockSize = 16384;

		internal int m_AllocLock;
	}
}
