using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafeRingQueueDebugView<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafeRingQueue<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable where T : struct, ValueType
	{
		public bool IsEmpty
		{
			get
			{
				return !this.IsCreated || this.Length == 0;
			}
		}

		public int Length
		{
			get
			{
				return this.Control.Length;
			}
		}

		public int Capacity
		{
			get
			{
				return this.Control.Capacity;
			}
		}

		public unsafe UnsafeRingQueue(T* ptr, int capacity)
		{
			this.Ptr = ptr;
			this.Allocator = AllocatorManager.None;
			this.Control = new RingControl(capacity);
		}

		public unsafe UnsafeRingQueue(int capacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			capacity++;
			this.Allocator = allocator;
			this.Control = new RingControl(capacity);
			int num = capacity * UnsafeUtility.SizeOf<T>();
			this.Ptr = (T*)Memory.Unmanaged.Allocate((long)num, 16, allocator);
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear((void*)this.Ptr, (long)num);
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.Ptr != null;
			}
		}

		public void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				Memory.Unmanaged.Free<T>(this.Ptr, this.Allocator);
				this.Allocator = AllocatorManager.Invalid;
			}
			this.Ptr = null;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				JobHandle jobHandle = new UnsafeDisposeJob
				{
					Ptr = (void*)this.Ptr,
					Allocator = this.Allocator
				}.Schedule(inputDeps);
				this.Ptr = null;
				this.Allocator = AllocatorManager.Invalid;
				return jobHandle;
			}
			this.Ptr = null;
			return inputDeps;
		}

		public unsafe bool TryEnqueue(T value)
		{
			if (1 != this.Control.Reserve(1))
			{
				return false;
			}
			this.Ptr[(IntPtr)this.Control.Current * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
			this.Control.Commit(1);
			return true;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void ThrowQueueFull()
		{
			throw new InvalidOperationException("Trying to enqueue into full queue.");
		}

		public void Enqueue(T value)
		{
			this.TryEnqueue(value);
		}

		public unsafe bool TryDequeue(out T item)
		{
			item = this.Ptr[(IntPtr)this.Control.Read * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			return 1 == this.Control.Consume(1);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void ThrowQueueEmpty()
		{
			throw new InvalidOperationException("Trying to dequeue from an empty queue");
		}

		public T Dequeue()
		{
			T t;
			this.TryDequeue(out t);
			return t;
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe T* Ptr;

		public AllocatorManager.AllocatorHandle Allocator;

		internal RingControl Control;
	}
}
