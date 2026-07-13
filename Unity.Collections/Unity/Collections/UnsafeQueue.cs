using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafeQueue<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable where T : struct, ValueType
	{
		public UnsafeQueue(AllocatorManager.AllocatorHandle allocator)
		{
			this.m_AllocatorLabel = allocator;
			UnsafeQueueData.AllocateQueue<T>(allocator, out this.m_Buffer);
		}

		internal unsafe static UnsafeQueue<T>* Alloc(AllocatorManager.AllocatorHandle allocator)
		{
			return (UnsafeQueue<T>*)Memory.Unmanaged.Allocate((long)sizeof(UnsafeQueue<T>), UnsafeUtility.AlignOf<UnsafeQueue<T>>(), allocator);
		}

		internal unsafe static void Free(UnsafeQueue<T>* data)
		{
			if (data == null)
			{
				throw new InvalidOperationException("UnsafeQueue has yet to be created or has been destroyed!");
			}
			AllocatorManager.AllocatorHandle allocatorLabel = data->m_AllocatorLabel;
			data->Dispose();
			Memory.Unmanaged.Free<UnsafeQueue<T>>(data, allocatorLabel);
		}

		public unsafe readonly bool IsEmpty()
		{
			if (this.IsCreated)
			{
				int num = 0;
				int currentRead = this.m_Buffer->m_CurrentRead;
				for (UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
				{
					num += ptr->m_NumItems;
					if (num > currentRead)
					{
						return false;
					}
				}
				return num == currentRead;
			}
			return true;
		}

		public unsafe readonly int Count
		{
			get
			{
				int num = 0;
				for (UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
				{
					num += ptr->m_NumItems;
				}
				return num - this.m_Buffer->m_CurrentRead;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe T Peek()
		{
			UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock;
			return UnsafeUtility.ReadArrayElement<T>((void*)(ptr + 1), this.m_Buffer->m_CurrentRead);
		}

		public unsafe void Enqueue(T value)
		{
			UnsafeQueueBlockHeader* ptr = UnsafeQueueData.AllocateWriteBlockMT<T>(this.m_Buffer, this.m_AllocatorLabel, 0);
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
			UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock;
			if (ptr != null)
			{
				UnsafeQueueData* buffer = this.m_Buffer;
				int currentRead = buffer->m_CurrentRead;
				buffer->m_CurrentRead = currentRead + 1;
				int num = currentRead;
				int numItems = ptr->m_NumItems;
				item = UnsafeUtility.ReadArrayElement<T>((void*)(ptr + 1), num);
				if (num + 1 >= numItems)
				{
					this.m_Buffer->m_CurrentRead = 0;
					this.m_Buffer->m_FirstBlock = (IntPtr)((void*)ptr->m_NextBlock);
					if (this.m_Buffer->m_FirstBlock == IntPtr.Zero)
					{
						this.m_Buffer->m_LastBlock = IntPtr.Zero;
					}
					int threadIndexCount = JobsUtility.ThreadIndexCount;
					for (int i = 0; i < threadIndexCount; i++)
					{
						if (this.m_Buffer->GetCurrentWriteBlockTLS(i) == ptr)
						{
							this.m_Buffer->SetCurrentWriteBlockTLS(i, null);
						}
					}
					Memory.Unmanaged.Free<UnsafeQueueBlockHeader>(ptr, this.m_AllocatorLabel);
				}
				return true;
			}
			item = default(T);
			return false;
		}

		public unsafe NativeArray<T> ToArray(AllocatorManager.AllocatorHandle allocator)
		{
			UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock;
			NativeArray<T> nativeArray = CollectionHelper.CreateNativeArray<T>(this.Count, allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeQueueBlockHeader* ptr2 = ptr;
			byte* unsafePtr = (byte*)nativeArray.GetUnsafePtr<T>();
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = 0;
			int num3 = this.m_Buffer->m_CurrentRead * num;
			int num4 = this.m_Buffer->m_CurrentRead;
			while (ptr2 != null)
			{
				int num5 = (ptr2->m_NumItems - num4) * num;
				UnsafeUtility.MemCpy((void*)(unsafePtr + num2), (void*)(ptr2 + 1 + num3 / sizeof(UnsafeQueueBlockHeader)), (long)num5);
				num4 = (num3 = 0);
				num2 += num5;
				ptr2 = ptr2->m_NextBlock;
			}
			return nativeArray;
		}

		public unsafe void Clear()
		{
			UnsafeQueueBlockHeader* nextBlock;
			for (UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = nextBlock)
			{
				nextBlock = ptr->m_NextBlock;
				Memory.Unmanaged.Free<UnsafeQueueBlockHeader>(ptr, this.m_AllocatorLabel);
			}
			this.m_Buffer->m_FirstBlock = IntPtr.Zero;
			this.m_Buffer->m_LastBlock = IntPtr.Zero;
			this.m_Buffer->m_CurrentRead = 0;
			int threadIndexCount = JobsUtility.ThreadIndexCount;
			for (int i = 0; i < threadIndexCount; i++)
			{
				this.m_Buffer->SetCurrentWriteBlockTLS(i, null);
			}
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Buffer != null;
			}
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			UnsafeQueueData.DeallocateQueue(this.m_Buffer, this.m_AllocatorLabel);
			this.m_Buffer = null;
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new UnsafeQueueDisposeJob
			{
				Data = new UnsafeQueueDispose
				{
					m_Buffer = this.m_Buffer,
					m_AllocatorLabel = this.m_AllocatorLabel
				}
			}.Schedule(inputDeps);
			this.m_Buffer = null;
			return jobHandle;
		}

		public UnsafeQueue<T>.ReadOnly AsReadOnly()
		{
			return new UnsafeQueue<T>.ReadOnly(ref this);
		}

		public UnsafeQueue<T>.ParallelWriter AsParallelWriter()
		{
			UnsafeQueue<T>.ParallelWriter parallelWriter;
			parallelWriter.m_Buffer = this.m_Buffer;
			parallelWriter.m_AllocatorLabel = this.m_AllocatorLabel;
			parallelWriter.m_ThreadIndex = 0;
			return parallelWriter;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void CheckNotEmpty()
		{
			this.m_Buffer->m_FirstBlock == (IntPtr)0;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void ThrowEmpty()
		{
			throw new InvalidOperationException("Trying to read from an empty queue.");
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeQueueData* m_Buffer;

		[NativeDisableUnsafePtrRestriction]
		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public void Dispose()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe bool MoveNext()
			{
				this.m_Index++;
				while (this.m_Block != null)
				{
					int numItems = this.m_Block->m_NumItems;
					if (this.m_Index < numItems)
					{
						this.value = UnsafeUtility.ReadArrayElement<T>((void*)(this.m_Block + 1), this.m_Index);
						return true;
					}
					this.m_Index -= numItems;
					this.m_Block = this.m_Block->m_NextBlock;
				}
				this.value = default(T);
				return false;
			}

			public void Reset()
			{
				this.m_Block = this.m_FirstBlock;
				this.m_Index = -1;
			}

			public T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeQueueBlockHeader* m_FirstBlock;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeQueueBlockHeader* m_Block;

			internal int m_Index;

			private T value;
		}

		public struct ReadOnly : IEnumerable<T>, IEnumerable
		{
			internal ReadOnly(ref UnsafeQueue<T> data)
			{
				this.m_Buffer = data.m_Buffer;
			}

			public readonly bool IsCreated
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Buffer != null;
				}
			}

			public unsafe readonly bool IsEmpty()
			{
				int num = 0;
				int currentRead = this.m_Buffer->m_CurrentRead;
				for (UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
				{
					num += ptr->m_NumItems;
					if (num > currentRead)
					{
						return false;
					}
				}
				return num == currentRead;
			}

			public unsafe readonly int Count
			{
				get
				{
					int num = 0;
					for (UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
					{
						num += ptr->m_NumItems;
					}
					return num - this.m_Buffer->m_CurrentRead;
				}
			}

			public readonly T this[int index]
			{
				get
				{
					T t;
					this.TryGetValue(index, out t);
					return t;
				}
			}

			private unsafe readonly bool TryGetValue(int index, out T item)
			{
				if (index >= 0)
				{
					int num = index;
					for (UnsafeQueueBlockHeader* ptr = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
					{
						int numItems = ptr->m_NumItems;
						if (num < numItems)
						{
							item = UnsafeUtility.ReadArrayElement<T>((void*)(ptr + 1), num);
							return true;
						}
						num -= numItems;
					}
				}
				item = default(T);
				return false;
			}

			public unsafe readonly UnsafeQueue<T>.Enumerator GetEnumerator()
			{
				return new UnsafeQueue<T>.Enumerator
				{
					m_FirstBlock = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock,
					m_Block = (UnsafeQueueBlockHeader*)(void*)this.m_Buffer->m_FirstBlock,
					m_Index = -1
				};
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private readonly void ThrowIndexOutOfRangeException(int index)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of bounds [0-{1}].", index, this.Count));
			}

			[NativeDisableUnsafePtrRestriction]
			private unsafe UnsafeQueueData* m_Buffer;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			public unsafe void Enqueue(T value)
			{
				UnsafeQueueBlockHeader* ptr = UnsafeQueueData.AllocateWriteBlockMT<T>(this.m_Buffer, this.m_AllocatorLabel, this.m_ThreadIndex);
				UnsafeUtility.WriteArrayElement<T>((void*)(ptr + 1), ptr->m_NumItems, value);
				ptr->m_NumItems++;
			}

			public unsafe void Enqueue(T value, int threadIndexOverride)
			{
				UnsafeQueueBlockHeader* ptr = UnsafeQueueData.AllocateWriteBlockMT<T>(this.m_Buffer, this.m_AllocatorLabel, threadIndexOverride);
				UnsafeUtility.WriteArrayElement<T>((void*)(ptr + 1), ptr->m_NumItems, value);
				ptr->m_NumItems++;
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeQueueData* m_Buffer;

			internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

			[NativeSetThreadIndex]
			internal int m_ThreadIndex;
		}
	}
}
