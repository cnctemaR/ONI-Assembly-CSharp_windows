using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	internal struct UnsafeQueueData
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe UnsafeQueueBlockHeader* GetCurrentWriteBlockTLS(int threadIndex)
		{
			UnsafeQueueBlockHeader** ptr = (UnsafeQueueBlockHeader**)(this.m_CurrentWriteBlockTLS + threadIndex * 64);
			return *(IntPtr*)ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void SetCurrentWriteBlockTLS(int threadIndex, UnsafeQueueBlockHeader* currentWriteBlock)
		{
			UnsafeQueueBlockHeader** ptr = (UnsafeQueueBlockHeader**)(this.m_CurrentWriteBlockTLS + threadIndex * 64);
			*(IntPtr*)ptr = currentWriteBlock;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static UnsafeQueueBlockHeader* AllocateWriteBlockMT<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(UnsafeQueueData* data, AllocatorManager.AllocatorHandle allocator, int threadIndex) where T : struct, ValueType
		{
			UnsafeQueueBlockHeader* ptr = data->GetCurrentWriteBlockTLS(threadIndex);
			if (ptr != null)
			{
				if (ptr->m_NumItems != data->m_MaxItems)
				{
					return ptr;
				}
				ptr = null;
			}
			ptr = (UnsafeQueueBlockHeader*)Memory.Unmanaged.Allocate(16384L, 16, allocator);
			ptr->m_NextBlock = null;
			ptr->m_NumItems = 0;
			UnsafeQueueBlockHeader* ptr2 = (UnsafeQueueBlockHeader*)(void*)Interlocked.Exchange(ref data->m_LastBlock, (IntPtr)((void*)ptr));
			if (ptr2 == null)
			{
				data->m_FirstBlock = (IntPtr)((void*)ptr);
			}
			else
			{
				ptr2->m_NextBlock = ptr;
			}
			data->SetCurrentWriteBlockTLS(threadIndex, ptr);
			return ptr;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void AllocateQueue<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(AllocatorManager.AllocatorHandle allocator, out UnsafeQueueData* outBuf) where T : struct, ValueType
		{
			int threadIndexCount = JobsUtility.ThreadIndexCount;
			int num = CollectionHelper.Align(UnsafeUtility.SizeOf<UnsafeQueueData>(), 64);
			UnsafeQueueData* ptr = (UnsafeQueueData*)Memory.Unmanaged.Allocate((long)(num + 64 * threadIndexCount), 64, allocator);
			ptr->m_CurrentWriteBlockTLS = (byte*)(ptr + num / sizeof(UnsafeQueueData));
			ptr->m_FirstBlock = IntPtr.Zero;
			ptr->m_LastBlock = IntPtr.Zero;
			ptr->m_MaxItems = (16384 - UnsafeUtility.SizeOf<UnsafeQueueBlockHeader>()) / UnsafeUtility.SizeOf<T>();
			ptr->m_CurrentRead = 0;
			for (int i = 0; i < threadIndexCount; i++)
			{
				ptr->SetCurrentWriteBlockTLS(i, null);
			}
			outBuf = ptr;
		}

		public unsafe static void DeallocateQueue(UnsafeQueueData* data, AllocatorManager.AllocatorHandle allocator)
		{
			UnsafeQueueBlockHeader* nextBlock;
			for (UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)data->m_FirstBlock; ptr != null; ptr = nextBlock)
			{
				nextBlock = ptr->m_NextBlock;
				Memory.Unmanaged.Free<UnsafeQueueBlockHeader>(ptr, allocator);
			}
			Memory.Unmanaged.Free<UnsafeQueueData>(data, allocator);
		}

		internal const int m_BlockSize = 16384;

		public IntPtr m_FirstBlock;

		public IntPtr m_LastBlock;

		public int m_MaxItems;

		public int m_CurrentRead;

		public unsafe byte* m_CurrentWriteBlockTLS;
	}
}
