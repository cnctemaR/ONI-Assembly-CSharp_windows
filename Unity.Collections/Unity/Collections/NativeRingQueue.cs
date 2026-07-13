using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(NativeRingQueueDebugView<>))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeRingQueue<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable where T : struct, ValueType
	{
		public unsafe readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_RingQueue != null && this.m_RingQueue->IsCreated;
			}
		}

		public unsafe readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_RingQueue == null || this.m_RingQueue->Length == 0;
			}
		}

		public unsafe readonly int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return CollectionHelper.AssumePositive(this.m_RingQueue->Length);
			}
		}

		public unsafe readonly int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return CollectionHelper.AssumePositive(this.m_RingQueue->Capacity);
			}
		}

		public unsafe NativeRingQueue(int capacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			this.m_RingQueue = UnsafeRingQueue<T>.Alloc(allocator);
			*this.m_RingQueue = new UnsafeRingQueue<T>(capacity, allocator, options);
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			UnsafeRingQueue<T>.Free(this.m_RingQueue);
			this.m_RingQueue = null;
		}

		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new NativeRingQueueDisposeJob
			{
				Data = new NativeRingQueueDispose
				{
					m_QueueData = (UnsafeRingQueue<int>*)this.m_RingQueue
				}
			}.Schedule(inputDeps);
			this.m_RingQueue = null;
			return jobHandle;
		}

		public unsafe bool TryEnqueue(T value)
		{
			return this.m_RingQueue->TryEnqueue(value);
		}

		public unsafe void Enqueue(T value)
		{
			this.m_RingQueue->Enqueue(value);
		}

		public unsafe bool TryDequeue(out T item)
		{
			return this.m_RingQueue->TryDequeue(out item);
		}

		public unsafe T Dequeue()
		{
			return this.m_RingQueue->Dequeue();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly void CheckWrite()
		{
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeRingQueue<T>* m_RingQueue;
	}
}
