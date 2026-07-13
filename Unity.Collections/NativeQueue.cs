using System;
using System.Diagnostics;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeQueue<T> : INativeDisposable, IDisposable where T : struct
	{
		public NativeQueue(AllocatorManager.AllocatorHandle allocator)
		{
			this.m_QueuePool = NativeQueueBlockPool.GetQueueBlockPool();
			this.m_AllocatorLabel = allocator;
			NativeQueueData.AllocateQueue<T>(allocator, out this.m_Buffer);
		}

		public unsafe bool IsEmpty()
		{
			if (!this.IsCreated)
			{
				return true;
			}
			int num = 0;
			int currentRead = this.m_Buffer->m_CurrentRead;
			for (NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
			{
				num += ptr->m_NumItems;
				if (num > currentRead)
				{
					return false;
				}
			}
			return num == currentRead;
		}

		public unsafe int Count
		{
			get
			{
				int num = 0;
				for (NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
				{
					num += ptr->m_NumItems;
				}
				return num - this.m_Buffer->m_CurrentRead;
			}
		}

		internal unsafe static int PersistentMemoryBlockCount
		{
			get
			{
				return NativeQueueBlockPool.GetQueueBlockPool()->m_MaxBlocks;
			}
			set
			{
				Interlocked.Exchange(ref NativeQueueBlockPool.GetQueueBlockPool()->m_MaxBlocks, value);
			}
		}

		internal static int MemoryBlockSize
		{
			get
			{
				return 16384;
			}
		}

		public unsafe T Peek()
		{
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock;
			return UnsafeUtility.ReadArrayElement<T>((void*)(ptr + 1), this.m_Buffer->m_CurrentRead);
		}

		public unsafe void Enqueue(T value)
		{
			NativeQueueBlockHeader* ptr = NativeQueueData.AllocateWriteBlockMT<T>(this.m_Buffer, this.m_QueuePool, 0);
			UnsafeUtility.WriteArrayElement<T>((void*)(ptr + 1), ptr->m_NumItems, value);
			ptr->m_NumItems++;
		}

		public T Dequeue()
		{
			T t;
			this.TryDequeue(out t);
			return t;
		}

		public unsafe bool TryDequeue(out T item)
		{
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock;
			if (ptr == null)
			{
				item = default(T);
				return false;
			}
			NativeQueueData* buffer = this.m_Buffer;
			int currentRead = buffer->m_CurrentRead;
			buffer->m_CurrentRead = currentRead + 1;
			int num = currentRead;
			item = UnsafeUtility.ReadArrayElement<T>((void*)(ptr + 1), num);
			if (this.m_Buffer->m_CurrentRead >= ptr->m_NumItems)
			{
				this.m_Buffer->m_CurrentRead = 0;
				this.m_Buffer->m_FirstBlock = (IntPtr)((void*)ptr->m_NextBlock);
				if (this.m_Buffer->m_FirstBlock == IntPtr.Zero)
				{
					this.m_Buffer->m_LastBlock = IntPtr.Zero;
				}
				for (int i = 0; i < 128; i++)
				{
					if (this.m_Buffer->GetCurrentWriteBlockTLS(i) == ptr)
					{
						this.m_Buffer->SetCurrentWriteBlockTLS(i, null);
					}
				}
				this.m_QueuePool->FreeBlock(ptr);
			}
			return true;
		}

		public unsafe NativeArray<T> ToArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock;
			NativeArray<T> nativeArray = CollectionHelper.CreateNativeArray<T>(this.Count, allocator, NativeArrayOptions.ClearMemory);
			NativeQueueBlockHeader* ptr2 = ptr;
			byte* unsafePtr = (byte*)nativeArray.GetUnsafePtr<T>();
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = 0;
			int num3 = this.m_Buffer->m_CurrentRead * num;
			int num4 = this.m_Buffer->m_CurrentRead;
			while (ptr2 != null)
			{
				int num5 = (ptr2->m_NumItems - num4) * num;
				UnsafeUtility.MemCpy((void*)(unsafePtr + num2), (void*)(ptr2 + 1 + num3 / sizeof(NativeQueueBlockHeader)), (long)num5);
				num4 = (num3 = 0);
				num2 += num5;
				ptr2 = ptr2->m_NextBlock;
			}
			return nativeArray;
		}

		public unsafe void Clear()
		{
			NativeQueueBlockHeader* nextBlock;
			for (NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = nextBlock)
			{
				nextBlock = ptr->m_NextBlock;
				this.m_QueuePool->FreeBlock(ptr);
			}
			this.m_Buffer->m_FirstBlock = IntPtr.Zero;
			this.m_Buffer->m_LastBlock = IntPtr.Zero;
			this.m_Buffer->m_CurrentRead = 0;
			for (int i = 0; i < 128; i++)
			{
				this.m_Buffer->SetCurrentWriteBlockTLS(i, null);
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Buffer != null;
			}
		}

		public void Dispose()
		{
			NativeQueueData.DeallocateQueue(this.m_Buffer, this.m_QueuePool, this.m_AllocatorLabel);
			this.m_Buffer = null;
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			JobHandle jobHandle = new NativeQueueDisposeJob
			{
				Data = new NativeQueueDispose
				{
					m_Buffer = this.m_Buffer,
					m_QueuePool = this.m_QueuePool,
					m_AllocatorLabel = this.m_AllocatorLabel
				}
			}.Schedule(inputDeps);
			this.m_Buffer = null;
			return jobHandle;
		}

		public NativeQueue<T>.ParallelWriter AsParallelWriter()
		{
			NativeQueue<T>.ParallelWriter parallelWriter;
			parallelWriter.m_Buffer = this.m_Buffer;
			parallelWriter.m_QueuePool = this.m_QueuePool;
			parallelWriter.m_ThreadIndex = 0;
			return parallelWriter;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private unsafe void CheckReadNotEmpty()
		{
			this.m_Buffer->m_FirstBlock == (IntPtr)0;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void ThrowEmpty()
		{
			throw new InvalidOperationException("Trying to read from an empty queue.");
		}

		[NativeDisableUnsafePtrRestriction]
		private unsafe NativeQueueData* m_Buffer;

		[NativeDisableUnsafePtrRestriction]
		private unsafe NativeQueueBlockPoolData* m_QueuePool;

		private AllocatorManager.AllocatorHandle m_AllocatorLabel;

		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			public unsafe void Enqueue(T value)
			{
				NativeQueueBlockHeader* ptr = NativeQueueData.AllocateWriteBlockMT<T>(this.m_Buffer, this.m_QueuePool, this.m_ThreadIndex);
				UnsafeUtility.WriteArrayElement<T>((void*)(ptr + 1), ptr->m_NumItems, value);
				ptr->m_NumItems++;
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe NativeQueueData* m_Buffer;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe NativeQueueBlockPoolData* m_QueuePool;

			[NativeSetThreadIndex]
			internal int m_ThreadIndex;
		}
	}
}
