using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafeListTDebugView<>))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafeList<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable, INativeList<T>, IIndexable<T>, IEnumerable<T>, IEnumerable where T : struct, ValueType
	{
		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return CollectionHelper.AssumePositive(this.m_length);
			}
			set
			{
				if (value > this.Capacity)
				{
					this.Resize(value, NativeArrayOptions.UninitializedMemory);
					return;
				}
				this.m_length = value;
			}
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return CollectionHelper.AssumePositive(this.m_capacity);
			}
			set
			{
				this.SetCapacity(value);
			}
		}

		public unsafe T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Ptr[(IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.Ptr[(IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref T ElementAt(int index)
		{
			return ref this.Ptr[(IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}

		public unsafe UnsafeList(T* ptr, int length)
		{
			this = default(UnsafeList<T>);
			this.Ptr = ptr;
			this.m_length = length;
			this.m_capacity = length;
			this.Allocator = AllocatorManager.None;
		}

		public unsafe UnsafeList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this.Ptr = null;
			this.m_length = 0;
			this.m_capacity = 0;
			this.Allocator = allocator;
			this.padding = 0;
			this.SetCapacity(math.max(initialCapacity, 1));
			if (options == NativeArrayOptions.ClearMemory && this.Ptr != null)
			{
				int num = sizeof(T);
				UnsafeUtility.MemClear((void*)this.Ptr, (long)(this.Capacity * num));
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe static UnsafeList<T>* Create<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int initialCapacity, ref U allocator, NativeArrayOptions options) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			UnsafeList<T>* ptr = (ref allocator).Allocate(default(UnsafeList<T>), 1);
			*ptr = new UnsafeList<T>(initialCapacity, allocator.Handle, options);
			return ptr;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe static void Destroy<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(UnsafeList<T>* listData, ref U allocator) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			listData->Dispose<U>(ref allocator);
			(ref allocator).Free<U>((void*)listData, sizeof(UnsafeList<T>), UnsafeUtility.AlignOf<UnsafeList<T>>(), 1);
		}

		public unsafe static UnsafeList<T>* Create(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafeList<T>* ptr = AllocatorManager.Allocate<UnsafeList<T>>(allocator, 1);
			*ptr = new UnsafeList<T>(initialCapacity, allocator, options);
			return ptr;
		}

		public unsafe static void Destroy(UnsafeList<T>* listData)
		{
			AllocatorManager.AllocatorHandle allocator = listData->Allocator;
			listData->Dispose();
			AllocatorManager.Free<UnsafeList<T>>(allocator, listData, 1);
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return !this.IsCreated || this.m_length == 0;
			}
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Ptr != null;
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal void Dispose<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			(ref allocator).Free<U, T>(this.Ptr, this.m_capacity);
			this.Ptr = null;
			this.m_length = 0;
			this.m_capacity = 0;
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				AllocatorManager.Free<T>(this.Allocator, this.Ptr, this.m_capacity);
				this.Allocator = AllocatorManager.Invalid;
			}
			this.Ptr = null;
			this.m_length = 0;
			this.m_capacity = 0;
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

		public void Clear()
		{
			this.m_length = 0;
		}

		public unsafe void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			int length2 = this.m_length;
			if (length > this.Capacity)
			{
				this.SetCapacity(length);
			}
			this.m_length = length;
			if (options == NativeArrayOptions.ClearMemory && length2 < length)
			{
				int num = length - length2;
				byte* ptr = (byte*)this.Ptr;
				int num2 = sizeof(T);
				UnsafeUtility.MemClear((void*)(ptr + length2 * num2), (long)(num * num2));
			}
		}

		private unsafe void ResizeExact<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator, int newCapacity) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			newCapacity = math.max(0, newCapacity);
			T* ptr = null;
			int num = UnsafeUtility.AlignOf<T>();
			int num2 = sizeof(T);
			if (newCapacity > 0)
			{
				ptr = (T*)(ref allocator).Allocate<U>(num2, num, newCapacity);
				if (this.Ptr != null && this.m_capacity > 0)
				{
					int num3 = math.min(newCapacity, this.Capacity) * num2;
					UnsafeUtility.MemCpy((void*)ptr, (void*)this.Ptr, (long)num3);
				}
			}
			(ref allocator).Free<U, T>(this.Ptr, this.Capacity);
			this.Ptr = ptr;
			this.m_capacity = newCapacity;
			this.m_length = math.min(this.m_length, newCapacity);
		}

		private void ResizeExact(int capacity)
		{
			this.ResizeExact<AllocatorManager.AllocatorHandle>(ref this.Allocator, capacity);
		}

		private void SetCapacity<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator, int capacity) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			int num = sizeof(T);
			int num2 = math.max(capacity, 64 / num);
			num2 = math.ceilpow2(num2);
			if (num2 == this.Capacity)
			{
				return;
			}
			this.ResizeExact<U>(ref allocator, num2);
		}

		public void SetCapacity(int capacity)
		{
			this.SetCapacity<AllocatorManager.AllocatorHandle>(ref this.Allocator, capacity);
		}

		public void TrimExcess()
		{
			if (this.Capacity != this.m_length)
			{
				this.ResizeExact(this.m_length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void AddNoResize(T value)
		{
			UnsafeUtility.WriteArrayElement<T>((void*)this.Ptr, this.m_length, value);
			this.m_length++;
		}

		public unsafe void AddRangeNoResize(void* ptr, int count)
		{
			int num = sizeof(T);
			void* ptr2 = (void*)(this.Ptr + this.m_length * num / sizeof(T));
			UnsafeUtility.MemCpy(ptr2, ptr, (long)(count * num));
			this.m_length += count;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddRangeNoResize(UnsafeList<T> list)
		{
			this.AddRangeNoResize((void*)list.Ptr, CollectionHelper.AssumePositive(list.Length));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Add(in T value)
		{
			int length = this.m_length;
			if (this.m_length < this.m_capacity)
			{
				this.Ptr[(IntPtr)length * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
				this.m_length++;
				return;
			}
			this.Resize(length + 1, NativeArrayOptions.UninitializedMemory);
			this.Ptr[(IntPtr)length * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
		}

		public unsafe void AddRange(void* ptr, int count)
		{
			int length = this.m_length;
			if (this.m_length + count > this.Capacity)
			{
				this.Resize(this.m_length + count, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.m_length += count;
			}
			int num = sizeof(T);
			void* ptr2 = (void*)(this.Ptr + length * num / sizeof(T));
			UnsafeUtility.MemCpy(ptr2, ptr, (long)(count * num));
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddRange(UnsafeList<T> list)
		{
			this.AddRange((void*)list.Ptr, list.Length);
		}

		public unsafe void AddReplicate(in T value, int count)
		{
			int length = this.m_length;
			if (this.m_length + count > this.Capacity)
			{
				this.Resize(this.m_length + count, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.m_length += count;
			}
			fixed (T* ptr = &value)
			{
				void* ptr2 = (void*)ptr;
				UnsafeUtility.MemCpyReplicate((void*)(this.Ptr + (IntPtr)length * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), ptr2, UnsafeUtility.SizeOf<T>(), count);
			}
		}

		public unsafe void InsertRangeWithBeginEnd(int begin, int end)
		{
			begin = CollectionHelper.AssumePositive(begin);
			end = CollectionHelper.AssumePositive(end);
			int num = end - begin;
			if (num < 1)
			{
				return;
			}
			int length = this.m_length;
			if (this.m_length + num > this.Capacity)
			{
				this.Resize(this.m_length + num, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.m_length += num;
			}
			int num2 = length - begin;
			if (num2 < 1)
			{
				return;
			}
			int num3 = sizeof(T);
			int num4 = num2 * num3;
			byte* ptr = (byte*)this.Ptr;
			void* ptr2 = (void*)(ptr + end * num3);
			byte* ptr3 = ptr + begin * num3;
			UnsafeUtility.MemMove(ptr2, (void*)ptr3, (long)num4);
		}

		public void InsertRange(int index, int count)
		{
			this.InsertRangeWithBeginEnd(index, index + count);
		}

		public unsafe void RemoveAtSwapBack(int index)
		{
			index = CollectionHelper.AssumePositive(index);
			int num = this.m_length - 1;
			ref T ptr = ref this.Ptr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			T* ptr2 = this.Ptr + (IntPtr)num * (IntPtr)sizeof(T) / (IntPtr)sizeof(T);
			ptr = *ptr2;
			this.m_length--;
		}

		public unsafe void RemoveRangeSwapBack(int index, int count)
		{
			index = CollectionHelper.AssumePositive(index);
			count = CollectionHelper.AssumePositive(count);
			if (count > 0)
			{
				int num = math.max(this.m_length - count, index + count);
				int num2 = sizeof(T);
				void* ptr = (void*)(this.Ptr + index * num2 / sizeof(T));
				void* ptr2 = (void*)(this.Ptr + num * num2 / sizeof(T));
				UnsafeUtility.MemCpy(ptr, ptr2, (long)((this.m_length - num) * num2));
				this.m_length -= count;
			}
		}

		public unsafe void RemoveAt(int index)
		{
			index = CollectionHelper.AssumePositive(index);
			T* ptr = this.Ptr + (IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T);
			T* ptr2 = ptr + sizeof(T) / sizeof(T);
			this.m_length--;
			for (int i = index; i < this.m_length; i++)
			{
				T* ptr3 = ptr;
				ptr = ptr3 + sizeof(T) / sizeof(T);
				T* ptr4 = ptr2;
				ptr2 = ptr4 + sizeof(T) / sizeof(T);
				*ptr3 = *ptr4;
			}
		}

		public unsafe void RemoveRange(int index, int count)
		{
			index = CollectionHelper.AssumePositive(index);
			count = CollectionHelper.AssumePositive(count);
			if (count > 0)
			{
				int num = math.min(index + count, this.m_length);
				int num2 = sizeof(T);
				void* ptr = (void*)(this.Ptr + index * num2 / sizeof(T));
				void* ptr2 = (void*)(this.Ptr + num * num2 / sizeof(T));
				UnsafeUtility.MemCpy(ptr, ptr2, (long)((this.m_length - num) * num2));
				this.m_length -= count;
			}
		}

		public UnsafeList<T>.ReadOnly AsReadOnly()
		{
			return new UnsafeList<T>.ReadOnly(this.Ptr, this.Length);
		}

		public UnsafeList<T>.ParallelReader AsParallelReader()
		{
			return new UnsafeList<T>.ParallelReader(this.Ptr, this.Length);
		}

		public unsafe UnsafeList<T>.ParallelWriter AsParallelWriter()
		{
			return new UnsafeList<T>.ParallelWriter((UnsafeList<T>*)UnsafeUtility.AddressOf<UnsafeList<T>>(ref this));
		}

		public unsafe void CopyFrom(in NativeArray<T> other)
		{
			NativeArray<T> nativeArray = other;
			this.Resize(nativeArray.Length, NativeArrayOptions.UninitializedMemory);
			void* ptr = (void*)this.Ptr;
			void* unsafeReadOnlyPtr = other.GetUnsafeReadOnlyPtr<T>();
			int num = UnsafeUtility.SizeOf<T>();
			nativeArray = other;
			UnsafeUtility.MemCpy(ptr, unsafeReadOnlyPtr, (long)(num * nativeArray.Length));
		}

		public unsafe void CopyFrom(in UnsafeList<T> other)
		{
			this.Resize(other.Length, NativeArrayOptions.UninitializedMemory);
			UnsafeUtility.MemCpy((void*)this.Ptr, (void*)other.Ptr, (long)(UnsafeUtility.SizeOf<T>() * other.Length));
		}

		public UnsafeList<T>.Enumerator GetEnumerator()
		{
			return new UnsafeList<T>.Enumerator
			{
				m_Ptr = this.Ptr,
				m_Length = this.Length,
				m_Index = -1
			};
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal unsafe static void CheckNull(void* listData)
		{
			if (listData == null)
			{
				throw new InvalidOperationException("UnsafeList has yet to be created or has been destroyed!");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void CheckIndexCount(int index, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for count {0} must be positive.", count));
			}
			if (index < 0)
			{
				throw new IndexOutOfRangeException(string.Format("Value for index {0} must be positive.", index));
			}
			if (index > this.Length)
			{
				throw new IndexOutOfRangeException(string.Format("Value for index {0} is out of bounds.", index));
			}
			if (index + count > this.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for count {0} is out of bounds.", count));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void CheckBeginEndNoLength(int begin, int end)
		{
			if (begin > end)
			{
				throw new ArgumentException(string.Format("Value for begin {0} index must less or equal to end {1}.", begin, end));
			}
			if (begin < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for begin {0} must be positive.", begin));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void CheckBeginEnd(int begin, int end)
		{
			if (begin > this.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for begin {0} is out of bounds.", begin));
			}
			if (end > this.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for end {0} is out of bounds.", end));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckNoResizeHasEnoughCapacity(int length)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckNoResizeHasEnoughCapacity(int length, int index)
		{
			if (this.Capacity < index + length)
			{
				throw new InvalidOperationException(string.Format("AddNoResize assumes that list capacity is sufficient (Capacity {0}, Length {1}), requested length {2}!", this.Capacity, this.Length, length));
			}
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe T* Ptr;

		public int m_length;

		public int m_capacity;

		public AllocatorManager.AllocatorHandle Allocator;

		private readonly int padding;

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ReadOnly : IEnumerable<T>, IEnumerable
		{
			public readonly bool IsCreated
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.Ptr != null;
				}
			}

			public readonly bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return !this.IsCreated || this.Length == 0;
				}
			}

			internal unsafe ReadOnly(T* ptr, int length)
			{
				this.Ptr = ptr;
				this.Length = length;
			}

			public UnsafeList<T>.Enumerator GetEnumerator()
			{
				return new UnsafeList<T>.Enumerator
				{
					m_Ptr = this.Ptr,
					m_Length = this.Length,
					m_Index = -1
				};
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly T* Ptr;

			public readonly int Length;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelReader
		{
			internal unsafe ParallelReader(T* ptr, int length)
			{
				this.Ptr = ptr;
				this.Length = length;
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly T* Ptr;

			public readonly int Length;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			public unsafe readonly void* Ptr
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return (void*)this.ListData->Ptr;
				}
			}

			internal unsafe ParallelWriter(UnsafeList<T>* listData)
			{
				this.ListData = listData;
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void AddNoResize(T value)
			{
				int num = Interlocked.Increment(ref this.ListData->m_length) - 1;
				UnsafeUtility.WriteArrayElement<T>((void*)this.ListData->Ptr, num, value);
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void AddRangeNoResize(void* ptr, int count)
			{
				int num = Interlocked.Add(ref this.ListData->m_length, count) - count;
				void* ptr2 = (void*)(this.ListData->Ptr + num * sizeof(T) / sizeof(T));
				UnsafeUtility.MemCpy(ptr2, ptr, (long)(count * sizeof(T)));
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void AddRangeNoResize(UnsafeList<T> list)
			{
				this.AddRangeNoResize((void*)list.Ptr, list.Length);
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe UnsafeList<T>* ListData;
		}

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public void Dispose()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_Length;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public unsafe T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Ptr[(IntPtr)this.m_Index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal unsafe T* m_Ptr;

			internal int m_Length;

			internal int m_Index;
		}
	}
}
