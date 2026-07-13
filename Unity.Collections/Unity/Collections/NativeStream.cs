using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility]
	public struct NativeStream : INativeDisposable, IDisposable
	{
		public NativeStream(int bufferCount, AllocatorManager.AllocatorHandle allocator)
		{
			NativeStream.AllocateBlock(out this, allocator);
			this.m_Stream.AllocateForEach(bufferCount);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static JobHandle ScheduleConstruct<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(out NativeStream stream, NativeList<T> bufferCount, JobHandle dependency, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			NativeStream.AllocateBlock(out stream, allocator);
			return new NativeStream.ConstructJobList
			{
				List = (UntypedUnsafeList*)bufferCount.GetUnsafeList(),
				Container = stream
			}.Schedule(dependency);
		}

		public static JobHandle ScheduleConstruct(out NativeStream stream, NativeArray<int> bufferCount, JobHandle dependency, AllocatorManager.AllocatorHandle allocator)
		{
			NativeStream.AllocateBlock(out stream, allocator);
			return new NativeStream.ConstructJob
			{
				Length = bufferCount,
				Container = stream
			}.Schedule(dependency);
		}

		public readonly bool IsEmpty()
		{
			return this.m_Stream.IsEmpty();
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Stream.IsCreated;
			}
		}

		public readonly int ForEachCount
		{
			get
			{
				return this.m_Stream.ForEachCount;
			}
		}

		public NativeStream.Reader AsReader()
		{
			return new NativeStream.Reader(ref this);
		}

		public NativeStream.Writer AsWriter()
		{
			return new NativeStream.Writer(ref this);
		}

		public int Count()
		{
			return this.m_Stream.Count();
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public NativeArray<T> ToNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			return this.m_Stream.ToNativeArray<T>(allocator);
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			this.m_Stream.Dispose();
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new NativeStreamDisposeJob
			{
				Data = new NativeStreamDispose
				{
					m_StreamData = this.m_Stream
				}
			}.Schedule(inputDeps);
			this.m_Stream = default(UnsafeStream);
			return jobHandle;
		}

		private static void AllocateBlock(out NativeStream stream, AllocatorManager.AllocatorHandle allocator)
		{
			UnsafeStream.AllocateBlock(out stream.m_Stream, allocator);
		}

		private void AllocateForEach(int forEachCount)
		{
			this.m_Stream.AllocateForEach(forEachCount);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckForEachCountGreaterThanZero(int forEachCount)
		{
			if (forEachCount <= 0)
			{
				throw new ArgumentException("foreachCount must be > 0", "foreachCount");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private readonly void CheckRead()
		{
		}

		private UnsafeStream m_Stream;

		[BurstCompile]
		private struct ConstructJobList : IJob
		{
			public unsafe void Execute()
			{
				this.Container.AllocateForEach(this.List->m_length);
			}

			public NativeStream Container;

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

			public NativeStream Container;

			[ReadOnly]
			public NativeArray<int> Length;
		}

		[NativeContainer]
		[NativeContainerSupportsMinMaxWriteRestriction]
		[GenerateTestsForBurstCompatibility]
		public struct Writer
		{
			internal Writer(ref NativeStream stream)
			{
				this.m_Writer = stream.m_Stream.AsWriter();
			}

			public int ForEachCount
			{
				get
				{
					return this.m_Writer.ForEachCount;
				}
			}

			public void PatchMinMaxRange(int foreEachIndex)
			{
			}

			public void BeginForEachIndex(int foreachIndex)
			{
				this.m_Writer.BeginForEachIndex(foreachIndex);
			}

			public void EndForEachIndex()
			{
				this.m_Writer.EndForEachIndex();
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
				return this.m_Writer.Allocate(size);
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckBeginForEachIndex(int foreachIndex)
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckEndForEachIndex()
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckAllocateSize(int size)
			{
			}

			private UnsafeStream.Writer m_Writer;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
		[GenerateTestsForBurstCompatibility]
		public struct Reader
		{
			internal Reader(ref NativeStream stream)
			{
				this.m_Reader = stream.m_Stream.AsReader();
			}

			public int BeginForEachIndex(int foreachIndex)
			{
				return this.m_Reader.BeginForEachIndex(foreachIndex);
			}

			public void EndForEachIndex()
			{
				this.m_Reader.EndForEachIndex();
			}

			public int ForEachCount
			{
				get
				{
					return this.m_Reader.ForEachCount;
				}
			}

			public int RemainingItemCount
			{
				get
				{
					return this.m_Reader.RemainingItemCount;
				}
			}

			public unsafe byte* ReadUnsafePtr(int size)
			{
				this.m_Reader.m_RemainingItemCount = this.m_Reader.m_RemainingItemCount - 1;
				byte* ptr = this.m_Reader.m_CurrentPtr;
				this.m_Reader.m_CurrentPtr = this.m_Reader.m_CurrentPtr + size;
				if (this.m_Reader.m_CurrentPtr != this.m_Reader.m_CurrentBlockEnd)
				{
					this.m_Reader.m_CurrentBlock = this.m_Reader.m_CurrentBlock->Next;
					this.m_Reader.m_CurrentPtr = &this.m_Reader.m_CurrentBlock->Data.FixedElementField;
					this.m_Reader.m_CurrentBlockEnd = (byte*)(this.m_Reader.m_CurrentBlock + 4096 / sizeof(UnsafeStreamBlock));
					ptr = this.m_Reader.m_CurrentPtr;
					this.m_Reader.m_CurrentPtr = this.m_Reader.m_CurrentPtr + size;
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
			public ref T Peek<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
			{
				UnsafeUtility.SizeOf<T>();
				return this.m_Reader.Peek<T>();
			}

			public int Count()
			{
				return this.m_Reader.Count();
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckNotReadingOutOfBounds(int size)
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private void CheckRead()
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckReadSize(int size)
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckBeginForEachIndex(int forEachIndex)
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private void CheckEndForEachIndex()
			{
				if (this.m_Reader.m_RemainingItemCount != 0)
				{
					throw new ArgumentException("Not all elements (Count) have been read. If this is intentional, simply skip calling EndForEachIndex();");
				}
				if (this.m_Reader.m_CurrentBlockEnd != this.m_Reader.m_CurrentPtr)
				{
					throw new ArgumentException("Not all data (Data Size) has been read. If this is intentional, simply skip calling EndForEachIndex();");
				}
			}

			private UnsafeStream.Reader m_Reader;
		}
	}
}
