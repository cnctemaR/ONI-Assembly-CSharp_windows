using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public struct UnsafeStream : INativeDisposable, IDisposable
	{
		public UnsafeStream(int bufferCount, AllocatorManager.AllocatorHandle allocator)
		{
			UnsafeStream.AllocateBlock(out this, allocator);
			this.AllocateForEach(bufferCount);
		}

		[NotBurstCompatible]
		public unsafe static JobHandle ScheduleConstruct<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(out UnsafeStream stream, NativeList<T> bufferCount, JobHandle dependency, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			UnsafeStream.AllocateBlock(out stream, allocator);
			return new UnsafeStream.ConstructJobList
			{
				List = (UntypedUnsafeList*)bufferCount.GetUnsafeList(),
				Container = stream
			}.Schedule(dependency);
		}

		[NotBurstCompatible]
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
			int num = 128;
			int num2 = sizeof(UnsafeStreamBlockData) + sizeof(UnsafeStreamBlock*) * num;
			byte* ptr = (byte*)Memory.Unmanaged.Allocate((long)num2, 16, allocator);
			UnsafeUtility.MemClear((void*)ptr, (long)num2);
			UnsafeStreamBlockData* ptr2 = (UnsafeStreamBlockData*)ptr;
			stream.m_Block = ptr2;
			stream.m_Allocator = allocator;
			ptr2->Allocator = allocator;
			ptr2->BlockCount = num;
			ptr2->Blocks = (UnsafeStreamBlock**)(ptr + sizeof(UnsafeStreamBlockData));
			ptr2->Ranges = null;
			ptr2->RangeCount = 0;
		}

		internal unsafe void AllocateForEach(int forEachCount)
		{
			long num = (long)(sizeof(UnsafeStreamRange) * forEachCount);
			this.m_Block->Ranges = (UnsafeStreamRange*)Memory.Unmanaged.Allocate(num, 16, this.m_Allocator);
			this.m_Block->RangeCount = forEachCount;
			UnsafeUtility.MemClear((void*)this.m_Block->Ranges, num);
		}

		public unsafe bool IsEmpty()
		{
			if (!this.IsCreated)
			{
				return true;
			}
			for (int num = 0; num != this.m_Block->RangeCount; num++)
			{
				if (this.m_Block->Ranges[num].ElementCount > 0)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Block != null;
			}
		}

		public unsafe int ForEachCount
		{
			get
			{
				return this.m_Block->RangeCount;
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
			for (int num2 = 0; num2 != this.m_Block->RangeCount; num2++)
			{
				num += this.m_Block->Ranges[num2].ElementCount;
			}
			return num;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeArray<T> ToNativeArray<T>(AllocatorManager.AllocatorHandle allocator) where T : struct
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
			if (this.m_Block == null)
			{
				return;
			}
			for (int num = 0; num != this.m_Block->BlockCount; num++)
			{
				UnsafeStreamBlock* next;
				for (UnsafeStreamBlock* ptr = *(IntPtr*)(this.m_Block->Blocks + (IntPtr)num * (IntPtr)sizeof(UnsafeStreamBlock*) / (IntPtr)sizeof(UnsafeStreamBlock*)); ptr != null; ptr = next)
				{
					next = ptr->Next;
					Memory.Unmanaged.Free<UnsafeStreamBlock>(ptr, this.m_Allocator);
				}
			}
			Memory.Unmanaged.Free<UnsafeStreamRange>(this.m_Block->Ranges, this.m_Allocator);
			Memory.Unmanaged.Free<UnsafeStreamBlockData>(this.m_Block, this.m_Allocator);
			this.m_Block = null;
			this.m_Allocator = Allocator.None;
		}

		public void Dispose()
		{
			this.Deallocate();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			JobHandle jobHandle = new UnsafeStream.DisposeJob
			{
				Container = this
			}.Schedule(inputDeps);
			this.m_Block = null;
			return jobHandle;
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeStreamBlockData* m_Block;

		internal AllocatorManager.AllocatorHandle m_Allocator;

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

		[BurstCompatible]
		public struct Writer
		{
			internal Writer(ref UnsafeStream stream)
			{
				this.m_BlockStream = stream.m_Block;
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
					return this.m_BlockStream->RangeCount;
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
				this.m_BlockStream->Ranges[this.m_ForeachIndex].ElementCount = this.m_ElementCount;
				this.m_BlockStream->Ranges[this.m_ForeachIndex].OffsetInFirstBlock = this.m_FirstOffset;
				this.m_BlockStream->Ranges[this.m_ForeachIndex].Block = this.m_FirstBlock;
				this.m_BlockStream->Ranges[this.m_ForeachIndex].LastOffset = (int)((long)((byte*)this.m_CurrentPtr - (byte*)this.m_CurrentBlock));
				this.m_BlockStream->Ranges[this.m_ForeachIndex].NumberOfBlocks = this.m_NumberOfBlocks;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void Write<T>(T value) where T : struct
			{
				*this.Allocate<T>() = value;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Allocate<T>() where T : struct
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
					this.m_CurrentBlock = this.m_BlockStream->Allocate(currentBlock, this.m_ThreadIndex);
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
			internal unsafe UnsafeStreamBlockData* m_BlockStream;

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

		[BurstCompatible]
		public struct Reader
		{
			internal Reader(ref UnsafeStream stream)
			{
				this.m_BlockStream = stream.m_Block;
				this.m_CurrentBlock = null;
				this.m_CurrentPtr = null;
				this.m_CurrentBlockEnd = null;
				this.m_RemainingItemCount = 0;
				this.m_LastBlockSize = 0;
			}

			public unsafe int BeginForEachIndex(int foreachIndex)
			{
				this.m_RemainingItemCount = this.m_BlockStream->Ranges[foreachIndex].ElementCount;
				this.m_LastBlockSize = this.m_BlockStream->Ranges[foreachIndex].LastOffset;
				this.m_CurrentBlock = this.m_BlockStream->Ranges[foreachIndex].Block;
				this.m_CurrentPtr = (byte*)(this.m_CurrentBlock + this.m_BlockStream->Ranges[foreachIndex].OffsetInFirstBlock / sizeof(UnsafeStreamBlock));
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
					return this.m_BlockStream->RangeCount;
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

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Read<T>() where T : struct
			{
				int num = UnsafeUtility.SizeOf<T>();
				return UnsafeUtility.AsRef<T>((void*)this.ReadUnsafePtr(num));
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Peek<T>() where T : struct
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
				int num = 0;
				for (int num2 = 0; num2 != this.m_BlockStream->RangeCount; num2++)
				{
					num += this.m_BlockStream->Ranges[num2].ElementCount;
				}
				return num;
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeStreamBlockData* m_BlockStream;

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
