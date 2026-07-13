using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	public struct UnsafeStream : INativeDisposable, IDisposable
	{
		public UnsafeStream(int bufferCount, AllocatorManager.AllocatorHandle allocator)
		{
			UnsafeStream.AllocateBlock(out this, allocator);
			this.AllocateForEach(bufferCount);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static JobHandle ScheduleConstruct<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(out UnsafeStream stream, NativeList<T> bufferCount, JobHandle dependency, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			UnsafeStream.AllocateBlock(out stream, allocator);
			return new UnsafeStream.ConstructJobList
			{
				List = (UntypedUnsafeList*)bufferCount.GetUnsafeList(),
				Container = stream
			}.Schedule(dependency);
		}

		public static JobHandle ScheduleConstruct(out UnsafeStream stream, NativeArray<int> bufferCount, JobHandle dependency, AllocatorManager.AllocatorHandle allocator)
		{
			UnsafeStream.AllocateBlock(out stream, allocator);
			return new UnsafeStream.ConstructJob
			{
				Length = bufferCount,
				Container = stream
			}.Schedule(dependency);
		}

		internal unsafe static void AllocateBlock(out UnsafeStream stream, AllocatorManager.AllocatorHandle allocator)
		{
			int threadIndexCount = JobsUtility.ThreadIndexCount;
			int num = sizeof(UnsafeStreamBlockData) + sizeof(UnsafeStreamBlock*) * threadIndexCount;
			AllocatorManager.Block block = (ref allocator).AllocateBlock<AllocatorManager.AllocatorHandle>(num, 16, 1);
			UnsafeUtility.MemClear((void*)block.Range.Pointer, block.AllocatedBytes);
			stream.m_BlockData = block;
			UnsafeStreamBlockData* ptr = (UnsafeStreamBlockData*)(void*)block.Range.Pointer;
			ptr->Allocator = allocator;
			ptr->BlockCount = threadIndexCount;
			ptr->Blocks = (UnsafeStreamBlock**)(void*)(block.Range.Pointer + sizeof(UnsafeStreamBlockData));
			ptr->Ranges = default(AllocatorManager.Block);
			ptr->RangeCount = 0;
		}

		internal unsafe void AllocateForEach(int forEachCount)
		{
			UnsafeStreamBlockData* ptr = (UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer;
			ptr->Ranges = (ref this.m_BlockData.Range.Allocator).AllocateBlock<AllocatorManager.AllocatorHandle>(sizeof(UnsafeStreamRange), 16, forEachCount);
			ptr->RangeCount = forEachCount;
			UnsafeUtility.MemClear((void*)ptr->Ranges.Range.Pointer, ptr->Ranges.AllocatedBytes);
		}

		public unsafe readonly bool IsEmpty()
		{
			if (!this.IsCreated)
			{
				return true;
			}
			UnsafeStreamBlockData* ptr = (UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer;
			UnsafeStreamRange* ptr2 = (UnsafeStreamRange*)(void*)ptr->Ranges.Range.Pointer;
			for (int num = 0; num != ptr->RangeCount; num++)
			{
				if (ptr2[num].ElementCount > 0)
				{
					return false;
				}
			}
			return true;
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_BlockData.Range.Pointer != IntPtr.Zero;
			}
		}

		public unsafe readonly int ForEachCount
		{
			get
			{
				return ((UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer)->RangeCount;
			}
		}

		public UnsafeStream.Reader AsReader()
		{
			return new UnsafeStream.Reader(ref this);
		}

		public UnsafeStream.Writer AsWriter()
		{
			return new UnsafeStream.Writer(ref this);
		}

		public unsafe int Count()
		{
			int num = 0;
			UnsafeStreamBlockData* ptr = (UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer;
			UnsafeStreamRange* ptr2 = (UnsafeStreamRange*)(void*)ptr->Ranges.Range.Pointer;
			for (int num2 = 0; num2 != ptr->RangeCount; num2++)
			{
				num += ptr2[num2].ElementCount;
			}
			return num;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeArray<T> ToNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			NativeArray<T> nativeArray = CollectionHelper.CreateNativeArray<T>(this.Count(), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeStream.Reader reader = this.AsReader();
			int num = 0;
			for (int num2 = 0; num2 != reader.ForEachCount; num2++)
			{
				reader.BeginForEachIndex(num2);
				int remainingItemCount = reader.RemainingItemCount;
				for (int i = 0; i < remainingItemCount; i++)
				{
					nativeArray[num] = *reader.Read<T>();
					num++;
				}
				reader.EndForEachIndex();
			}
			return nativeArray;
		}

		private unsafe void Deallocate()
		{
			if (!this.IsCreated)
			{
				return;
			}
			UnsafeStreamBlockData* ptr = (UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer;
			for (int num = 0; num != ptr->BlockCount; num++)
			{
				UnsafeStreamBlock* next;
				for (UnsafeStreamBlock* ptr2 = *(IntPtr*)(ptr->Blocks + (IntPtr)num * (IntPtr)sizeof(UnsafeStreamBlock*) / (IntPtr)sizeof(UnsafeStreamBlock*)); ptr2 != null; ptr2 = next)
				{
					next = ptr2->Next;
					ptr->Free(ptr2);
				}
			}
			ptr->Ranges.Dispose();
			this.m_BlockData.Dispose();
			this.m_BlockData = default(AllocatorManager.Block);
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			this.Deallocate();
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new UnsafeStream.DisposeJob
			{
				Container = this
			}.Schedule(inputDeps);
			this.m_BlockData = default(AllocatorManager.Block);
			return jobHandle;
		}

		[NativeDisableUnsafePtrRestriction]
		internal AllocatorManager.Block m_BlockData;

		[BurstCompile]
		private struct DisposeJob : IJob
		{
			public void Execute()
			{
				this.Container.Deallocate();
			}

			public UnsafeStream Container;
		}

		[BurstCompile]
		private struct ConstructJobList : IJob
		{
			public unsafe void Execute()
			{
				this.Container.AllocateForEach(this.List->m_length);
			}

			public UnsafeStream Container;

			[ReadOnly]
			[NativeDisableUnsafePtrRestriction]
			public unsafe UntypedUnsafeList* List;
		}

		[BurstCompile]
		private struct ConstructJob : IJob
		{
			public void Execute()
			{
				this.Container.AllocateForEach(this.Length[0]);
			}

			public UnsafeStream Container;

			[ReadOnly]
			public NativeArray<int> Length;
		}

		[GenerateTestsForBurstCompatibility]
		public struct Writer
		{
			internal Writer(ref UnsafeStream stream)
			{
				this.m_BlockData = stream.m_BlockData;
				this.m_ForeachIndex = int.MinValue;
				this.m_ElementCount = -1;
				this.m_CurrentBlock = null;
				this.m_CurrentBlockEnd = null;
				this.m_CurrentPtr = null;
				this.m_FirstBlock = null;
				this.m_NumberOfBlocks = 0;
				this.m_FirstOffset = 0;
				this.m_ThreadIndex = 0;
			}

			public unsafe int ForEachCount
			{
				get
				{
					return ((UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer)->RangeCount;
				}
			}

			public unsafe void BeginForEachIndex(int foreachIndex)
			{
				this.m_ForeachIndex = foreachIndex;
				this.m_ElementCount = 0;
				this.m_NumberOfBlocks = 0;
				this.m_FirstBlock = this.m_CurrentBlock;
				this.m_FirstOffset = (int)((long)((byte*)this.m_CurrentPtr - (byte*)this.m_CurrentBlock));
			}

			public unsafe void EndForEachIndex()
			{
				UnsafeStreamBlockData* ptr = (UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer;
				UnsafeStreamRange* ptr2 = (UnsafeStreamRange*)(void*)ptr->Ranges.Range.Pointer;
				ptr2[this.m_ForeachIndex].ElementCount = this.m_ElementCount;
				ptr2[this.m_ForeachIndex].OffsetInFirstBlock = this.m_FirstOffset;
				ptr2[this.m_ForeachIndex].Block = this.m_FirstBlock;
				ptr2[this.m_ForeachIndex].LastOffset = (int)((long)((byte*)this.m_CurrentPtr - (byte*)this.m_CurrentBlock));
				ptr2[this.m_ForeachIndex].NumberOfBlocks = this.m_NumberOfBlocks;
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void Write<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T value) where T : struct, ValueType
			{
				*this.Allocate<T>() = value;
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Allocate<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
			{
				int num = UnsafeUtility.SizeOf<T>();
				return UnsafeUtility.AsRef<T>((void*)this.Allocate(num));
			}

			public unsafe byte* Allocate(int size)
			{
				byte* ptr = this.m_CurrentPtr;
				this.m_CurrentPtr += size;
				if (this.m_CurrentPtr != this.m_CurrentBlockEnd)
				{
					UnsafeStreamBlock* currentBlock = this.m_CurrentBlock;
					UnsafeStreamBlockData* ptr2 = (UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer;
					this.m_CurrentBlock = ptr2->Allocate(currentBlock, this.m_ThreadIndex);
					this.m_CurrentPtr = &this.m_CurrentBlock->Data.FixedElementField;
					if (this.m_FirstBlock == null)
					{
						this.m_FirstOffset = (int)((long)((byte*)this.m_CurrentPtr - (byte*)this.m_CurrentBlock));
						this.m_FirstBlock = this.m_CurrentBlock;
					}
					else
					{
						this.m_NumberOfBlocks++;
					}
					this.m_CurrentBlockEnd = (byte*)(this.m_CurrentBlock + 4096 / sizeof(UnsafeStreamBlock));
					ptr = this.m_CurrentPtr;
					this.m_CurrentPtr += size;
				}
				this.m_ElementCount++;
				return ptr;
			}

			[NativeDisableUnsafePtrRestriction]
			internal AllocatorManager.Block m_BlockData;

			[NativeDisableUnsafePtrRestriction]
			private unsafe UnsafeStreamBlock* m_CurrentBlock;

			[NativeDisableUnsafePtrRestriction]
			private unsafe byte* m_CurrentPtr;

			[NativeDisableUnsafePtrRestriction]
			private unsafe byte* m_CurrentBlockEnd;

			internal int m_ForeachIndex;

			private int m_ElementCount;

			[NativeDisableUnsafePtrRestriction]
			private unsafe UnsafeStreamBlock* m_FirstBlock;

			private int m_FirstOffset;

			private int m_NumberOfBlocks;

			[NativeSetThreadIndex]
			private int m_ThreadIndex;
		}

		[GenerateTestsForBurstCompatibility]
		public struct Reader
		{
			internal Reader(ref UnsafeStream stream)
			{
				this.m_BlockData = stream.m_BlockData;
				this.m_CurrentBlock = null;
				this.m_CurrentPtr = null;
				this.m_CurrentBlockEnd = null;
				this.m_RemainingItemCount = 0;
				this.m_LastBlockSize = 0;
			}

			public unsafe int BeginForEachIndex(int foreachIndex)
			{
				UnsafeStreamBlockData* ptr = (UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer;
				UnsafeStreamRange* ptr2 = (UnsafeStreamRange*)(void*)ptr->Ranges.Range.Pointer;
				this.m_RemainingItemCount = ptr2[foreachIndex].ElementCount;
				this.m_LastBlockSize = ptr2[foreachIndex].LastOffset;
				this.m_CurrentBlock = ptr2[foreachIndex].Block;
				this.m_CurrentPtr = (byte*)(this.m_CurrentBlock + ptr2[foreachIndex].OffsetInFirstBlock / sizeof(UnsafeStreamBlock));
				this.m_CurrentBlockEnd = (byte*)(this.m_CurrentBlock + 4096 / sizeof(UnsafeStreamBlock));
				return this.m_RemainingItemCount;
			}

			public void EndForEachIndex()
			{
			}

			public unsafe int ForEachCount
			{
				get
				{
					return ((UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer)->RangeCount;
				}
			}

			public int RemainingItemCount
			{
				get
				{
					return this.m_RemainingItemCount;
				}
			}

			public unsafe byte* ReadUnsafePtr(int size)
			{
				this.m_RemainingItemCount--;
				byte* ptr = this.m_CurrentPtr;
				this.m_CurrentPtr += size;
				if (this.m_CurrentPtr != this.m_CurrentBlockEnd)
				{
					this.m_CurrentBlock = this.m_CurrentBlock->Next;
					this.m_CurrentPtr = &this.m_CurrentBlock->Data.FixedElementField;
					this.m_CurrentBlockEnd = (byte*)(this.m_CurrentBlock + 4096 / sizeof(UnsafeStreamBlock));
					ptr = this.m_CurrentPtr;
					this.m_CurrentPtr += size;
				}
				return ptr;
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Read<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
			{
				int num = UnsafeUtility.SizeOf<T>();
				return UnsafeUtility.AsRef<T>((void*)this.ReadUnsafePtr(num));
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Peek<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
			{
				int num = UnsafeUtility.SizeOf<T>();
				byte* ptr = this.m_CurrentPtr;
				if (ptr + num != this.m_CurrentBlockEnd)
				{
					ptr = &this.m_CurrentBlock->Next->Data.FixedElementField;
				}
				return UnsafeUtility.AsRef<T>((void*)ptr);
			}

			public unsafe int Count()
			{
				UnsafeStreamBlockData* ptr = (UnsafeStreamBlockData*)(void*)this.m_BlockData.Range.Pointer;
				UnsafeStreamRange* ptr2 = (UnsafeStreamRange*)(void*)ptr->Ranges.Range.Pointer;
				int num = 0;
				for (int num2 = 0; num2 != ptr->RangeCount; num2++)
				{
					num += ptr2[num2].ElementCount;
				}
				return num;
			}

			[NativeDisableUnsafePtrRestriction]
			internal AllocatorManager.Block m_BlockData;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeStreamBlock* m_CurrentBlock;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe byte* m_CurrentPtr;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe byte* m_CurrentBlockEnd;

			internal int m_RemainingItemCount;

			internal int m_LastBlockSize;
		}
	}
}
