using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafeRingQueueDebugView<>))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafeRingQueue<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable where T : struct, ValueType
	{
		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Filled == 0;
			}
		}

		public readonly int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Filled;
			}
		}

		public readonly int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Capacity;
			}
		}

		public unsafe UnsafeRingQueue(T* ptr, int capacity)
		{
			this.Ptr = ptr;
			this.Allocator = AllocatorManager.None;
			this.m_Capacity = capacity;
			this.m_Filled = 0;
			this.m_Write = 0;
			this.m_Read = 0;
		}

		public unsafe UnsafeRingQueue(int capacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			this.Allocator = allocator;
			this.m_Capacity = capacity;
			this.m_Filled = 0;
			this.m_Write = 0;
			this.m_Read = 0;
			int num = capacity * UnsafeUtility.SizeOf<T>();
			this.Ptr = (T*)Memory.Unmanaged.Allocate((long)num, 16, allocator);
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear((void*)this.Ptr, (long)num);
			}
		}

		internal unsafe static UnsafeRingQueue<T>* Alloc(AllocatorManager.AllocatorHandle allocator)
		{
			return (UnsafeRingQueue<T>*)Memory.Unmanaged.Allocate((long)sizeof(UnsafeRingQueue<T>), UnsafeUtility.AlignOf<UnsafeRingQueue<T>>(), allocator);
		}

		internal unsafe static void Free(UnsafeRingQueue<T>* data)
		{
			if (data == null)
			{
				throw new InvalidOperationException("UnsafeRingQueue has yet to be created or has been destroyed!");
			}
			AllocatorManager.AllocatorHandle allocator = data->Allocator;
			data->Dispose();
			Memory.Unmanaged.Free<UnsafeRingQueue<T>>(data, allocator);
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Ptr != null;
			}
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				Memory.Unmanaged.Free<T>(this.Ptr, this.Allocator);
				this.Allocator = AllocatorManager.Invalid;
			}
			this.Ptr = null;
		}

		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe bool TryEnqueueInternal(T value)
		{
			if (this.m_Filled == this.m_Capacity)
			{
				return false;
			}
			this.Ptr[(IntPtr)this.m_Write * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
			this.m_Write++;
			if (this.m_Write == this.m_Capacity)
			{
				this.m_Write = 0;
			}
			this.m_Filled++;
			return true;
		}

		public bool TryEnqueue(T value)
		{
			return this.TryEnqueueInternal(value);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void ThrowQueueFull()
		{
			throw new InvalidOperationException("Trying to enqueue into full queue.");
		}

		public void Enqueue(T value)
		{
			this.TryEnqueueInternal(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe bool TryDequeueInternal(out T item)
		{
			item = this.Ptr[(IntPtr)this.m_Read * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			if (this.m_Filled == 0)
			{
				return false;
			}
			this.m_Read++;
			if (this.m_Read == this.m_Capacity)
			{
				this.m_Read = 0;
			}
			this.m_Filled--;
			return true;
		}

		public bool TryDequeue(out T item)
		{
			return this.TryDequeueInternal(out item);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void ThrowQueueEmpty()
		{
			throw new InvalidOperationException("Trying to dequeue from an empty queue");
		}

		public T Dequeue()
		{
			T t;
			this.TryDequeueInternal(out t);
			return t;
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe T* Ptr;

		public AllocatorManager.AllocatorHandle Allocator;

		internal readonly int m_Capacity;

		internal int m_Filled;

		internal int m_Write;

		internal int m_Read;
	}
}
