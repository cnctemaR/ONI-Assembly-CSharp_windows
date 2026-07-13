using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeQueue<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable where T : struct, ValueType
	{
		public unsafe NativeQueue(AllocatorManager.AllocatorHandle allocator)
		{
			this.m_Queue = UnsafeQueue<T>.Alloc(allocator);
			*this.m_Queue = new UnsafeQueue<T>(allocator);
		}

		public unsafe readonly bool IsEmpty()
		{
			return !this.IsCreated || this.m_Queue->IsEmpty();
		}

		public unsafe readonly int Count
		{
			get
			{
				return this.m_Queue->Count;
			}
		}

		public unsafe T Peek()
		{
			return this.m_Queue->Peek();
		}

		public unsafe void Enqueue(T value)
		{
			this.m_Queue->Enqueue(value);
		}

		public unsafe T Dequeue()
		{
			return this.m_Queue->Dequeue();
		}

		public unsafe bool TryDequeue(out T item)
		{
			return this.m_Queue->TryDequeue(out item);
		}

		public unsafe NativeArray<T> ToArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Queue->ToArray(allocator);
		}

		public unsafe void Clear()
		{
			this.m_Queue->Clear();
		}

		public unsafe readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Queue != null && this.m_Queue->IsCreated;
			}
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			UnsafeQueue<T>.Free(this.m_Queue);
			this.m_Queue = null;
		}

		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new NativeQueueDisposeJob
			{
				Data = new NativeQueueDispose
				{
					m_QueueData = (UnsafeQueue<int>*)this.m_Queue
				}
			}.Schedule(inputDeps);
			this.m_Queue = null;
			return jobHandle;
		}

		public NativeQueue<T>.ReadOnly AsReadOnly()
		{
			return new NativeQueue<T>.ReadOnly(ref this);
		}

		public unsafe NativeQueue<T>.ParallelWriter AsParallelWriter()
		{
			NativeQueue<T>.ParallelWriter parallelWriter;
			parallelWriter.unsafeWriter = this.m_Queue->AsParallelWriter();
			return parallelWriter;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckWrite()
		{
		}

		[NativeDisableUnsafePtrRestriction]
		private unsafe UnsafeQueue<T>* m_Queue;

		[NativeContainer]
		[NativeContainerIsReadOnly]
		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public void Dispose()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_Enumerator.Reset();
			}

			public T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal UnsafeQueue<T>.Enumerator m_Enumerator;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
		public struct ReadOnly : IEnumerable<T>, IEnumerable
		{
			internal unsafe ReadOnly(ref NativeQueue<T> data)
			{
				this.m_ReadOnly = new UnsafeQueue<T>.ReadOnly(ref *data.m_Queue);
			}

			public readonly bool IsCreated
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_ReadOnly.IsCreated;
				}
			}

			public readonly bool IsEmpty()
			{
				return this.m_ReadOnly.IsEmpty();
			}

			public readonly int Count
			{
				get
				{
					return this.m_ReadOnly.Count;
				}
			}

			public readonly T this[int index]
			{
				get
				{
					return this.m_ReadOnly[index];
				}
			}

			public readonly NativeQueue<T>.Enumerator GetEnumerator()
			{
				return new NativeQueue<T>.Enumerator
				{
					m_Enumerator = this.m_ReadOnly.GetEnumerator()
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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private readonly void CheckRead()
			{
			}

			private UnsafeQueue<T>.ReadOnly m_ReadOnly;
		}

		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			public void Enqueue(T value)
			{
				this.unsafeWriter.Enqueue(value);
			}

			public void Enqueue(T value, int threadIndexOverride)
			{
				this.unsafeWriter.Enqueue(value, threadIndexOverride);
			}

			internal UnsafeQueue<T>.ParallelWriter unsafeWriter;
		}
	}
}
