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
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafeList<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable, INativeList<T>, IIndexable<T>, IEnumerable<T>, IEnumerable where T : struct, ValueType
	{
		public int Length
		{
			get
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
			get
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
			get
			{
				return this.Ptr[(IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			}
			set
			{
				this.Ptr[(IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
			}
		}

		public unsafe ref T ElementAt(int index)
		{
			return ref this.Ptr[(IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}

		public unsafe UnsafeList(T* ptr, int length)
		{
			this = default(UnsafeList<T>);
			this.Ptr = ptr;
			this.m_length = length;
			this.m_capacity = 0;
			this.Allocator = AllocatorManager.None;
		}

		public unsafe UnsafeList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafeList<T>);
			this.Ptr = null;
			this.m_length = 0;
			this.m_capacity = 0;
			this.Allocator = allocator;
			if (initialCapacity != 0)
			{
				this.SetCapacity(initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && this.Ptr != null)
			{
				int num = sizeof(T);
				UnsafeUtility.MemClear((void*)this.Ptr, (long)(this.Capacity * num));
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal void Initialize<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			this.Ptr = null;
			this.m_length = 0;
			this.m_capacity = 0;
			this.Allocator = AllocatorManager.None;
			this.Initialize<U>(initialCapacity, ref allocator, options);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal static UnsafeList<T> New<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			UnsafeList<T> unsafeList = default(UnsafeList<T>);
			unsafeList.Initialize<U>(initialCapacity, ref allocator, options);
			return unsafeList;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe static UnsafeList<T>* Create<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			UnsafeList<T>* ptr = (ref allocator).Allocate(default(UnsafeList<T>), 1);
			UnsafeUtility.MemClear((void*)ptr, (long)sizeof(UnsafeList<T>));
			ptr->Allocator = allocator.Handle;
			if (initialCapacity != 0)
			{
				ptr->SetCapacity<U>(ref allocator, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && ptr->Ptr != null)
			{
				int num = sizeof(T);
				UnsafeUtility.MemClear((void*)ptr->Ptr, (long)(ptr->Capacity * num));
			}
			return ptr;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
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

		public bool IsEmpty
		{
			get
			{
				return !this.IsCreated || this.m_length == 0;
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.Ptr != null;
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal void Dispose<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			(ref allocator).Free<U, T>(this.Ptr, this.m_length);
			this.Ptr = null;
			this.m_length = 0;
			this.m_capacity = 0;
		}

		public void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				AllocatorManager.Free<T>(this.Allocator, this.Ptr, 1);
				this.Allocator = AllocatorManager.Invalid;
			}
			this.Ptr = null;
			this.m_length = 0;
			this.m_capacity = 0;
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

		public void Clear()
		{
			this.m_length = 0;
		}

		public unsafe void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			int num = this.m_length;
			if (length > this.Capacity)
			{
				this.SetCapacity(length);
			}
			this.m_length = length;
			if (options == NativeArrayOptions.ClearMemory && num < length)
			{
				int num2 = length - num;
				byte* ptr = (byte*)this.Ptr;
				int num3 = sizeof(T);
				UnsafeUtility.MemClear((void*)(ptr + num * num3), (long)(num2 * num3));
			}
		}

		private unsafe void Realloc<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator, int newCapacity) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			T* ptr = null;
			int num = UnsafeUtility.AlignOf<T>();
			int num2 = sizeof(T);
			if (newCapacity > 0)
			{
				ptr = (T*)(ref allocator).Allocate<U>(num2, num, newCapacity);
				if (this.m_capacity > 0)
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

		private void Realloc(int capacity)
		{
			this.Realloc<AllocatorManager.AllocatorHandle>(ref this.Allocator, capacity);
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
			this.Realloc<U>(ref allocator, num2);
		}

		public void SetCapacity(int capacity)
		{
			this.SetCapacity<AllocatorManager.AllocatorHandle>(ref this.Allocator, capacity);
		}

		public void TrimExcess()
		{
			if (this.Capacity != this.m_length)
			{
				this.Realloc(this.m_length);
			}
		}

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

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddRangeNoResize(UnsafeList<T> list)
		{
			this.AddRangeNoResize((void*)list.Ptr, CollectionHelper.AssumePositive(list.m_length));
		}

		public unsafe void Add(in T value)
		{
			int num = this.m_length;
			if (this.m_length + 1 > this.Capacity)
			{
				this.Resize(num + 1, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.m_length++;
			}
			UnsafeUtility.WriteArrayElement<T>((void*)this.Ptr, num, value);
		}

		public unsafe void AddRange(void* ptr, int count)
		{
			int num = this.m_length;
			if (this.m_length + count > this.Capacity)
			{
				this.Resize(this.m_length + count, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.m_length += count;
			}
			int num2 = sizeof(T);
			void* ptr2 = (void*)(this.Ptr + num * num2 / sizeof(T));
			UnsafeUtility.MemCpy(ptr2, ptr, (long)(count * num2));
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddRange(UnsafeList<T> list)
		{
			this.AddRange((void*)list.Ptr, list.Length);
		}

		public unsafe void InsertRangeWithBeginEnd(int begin, int end)
		{
			int num = end - begin;
			if (num < 1)
			{
				return;
			}
			int num2 = this.m_length;
			if (this.m_length + num > this.Capacity)
			{
				this.Resize(this.m_length + num, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.m_length += num;
			}
			int num3 = num2 - begin;
			if (num3 < 1)
			{
				return;
			}
			int num4 = sizeof(T);
			int num5 = num3 * num4;
			byte* ptr = (byte*)this.Ptr;
			void* ptr2 = (void*)(ptr + end * num4);
			byte* ptr3 = ptr + begin * num4;
			UnsafeUtility.MemMove(ptr2, (void*)ptr3, (long)num5);
		}

		public void RemoveAtSwapBack(int index)
		{
			this.RemoveRangeSwapBack(index, 1);
		}

		public unsafe void RemoveRangeSwapBack(int index, int count)
		{
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

		[Obsolete("RemoveRangeSwapBackWithBeginEnd(begin, end) is deprecated, use RemoveRangeSwapBack(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public unsafe void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				int num2 = math.max(this.m_length - num, end);
				int num3 = sizeof(T);
				void* ptr = (void*)(this.Ptr + begin * num3 / sizeof(T));
				void* ptr2 = (void*)(this.Ptr + num2 * num3 / sizeof(T));
				UnsafeUtility.MemCpy(ptr, ptr2, (long)((this.m_length - num2) * num3));
				this.m_length -= num;
			}
		}

		public void RemoveAt(int index)
		{
			this.RemoveRange(index, 1);
		}

		public unsafe void RemoveRange(int index, int count)
		{
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

		[Obsolete("RemoveRangeWithBeginEnd(begin, end) is deprecated, use RemoveRange(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public unsafe void RemoveRangeWithBeginEnd(int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				int num2 = math.min(begin + num, this.m_length);
				int num3 = sizeof(T);
				void* ptr = (void*)(this.Ptr + begin * num3 / sizeof(T));
				void* ptr2 = (void*)(this.Ptr + num2 * num3 / sizeof(T));
				UnsafeUtility.MemCpy(ptr, ptr2, (long)((this.m_length - num2) * num3));
				this.m_length -= num;
			}
		}

		public UnsafeList<T>.ParallelReader AsParallelReader()
		{
			return new UnsafeList<T>.ParallelReader(this.Ptr, this.Length);
		}

		public unsafe UnsafeList<T>.ParallelWriter AsParallelWriter()
		{
			return new UnsafeList<T>.ParallelWriter((UnsafeList<T>*)UnsafeUtility.AddressOf<UnsafeList<T>>(ref this));
		}

		public unsafe void CopyFrom(UnsafeList<T> array)
		{
			this.Resize(array.Length, NativeArrayOptions.UninitializedMemory);
			UnsafeUtility.MemCpy((void*)this.Ptr, (void*)array.Ptr, (long)(UnsafeUtility.SizeOf<T>() * this.Length));
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
		internal unsafe static void CheckNull(void* listData)
		{
			if (listData == null)
			{
				throw new Exception("UnsafeList has yet to be created or has been destroyed!");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckIndexCount(int index, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for cound {0} must be positive.", count));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for index {0} must be positive.", index));
			}
			if (index > this.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for index {0} is out of bounds.", index));
			}
			if (index + count > this.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for count {0} is out of bounds.", count));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckBeginEnd(int begin, int end)
		{
			if (begin > end)
			{
				throw new ArgumentException(string.Format("Value for begin {0} index must less or equal to end {1}.", begin, end));
			}
			if (begin < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value for begin {0} must be positive.", begin));
			}
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
		private void CheckNoResizeHasEnoughCapacity(int length)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckNoResizeHasEnoughCapacity(int length, int index)
		{
			if (this.Capacity < index + length)
			{
				throw new Exception(string.Format("AddNoResize assumes that list capacity is sufficient (Capacity {0}, Length {1}), requested length {2}!", this.Capacity, this.Length, length));
			}
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe T* Ptr;

		public int m_length;

		public int m_capacity;

		public AllocatorManager.AllocatorHandle Allocator;

		[Obsolete("Use Length property (UnityUpgradable) -> Length", true)]
		public int length;

		[Obsolete("Use Capacity property (UnityUpgradable) -> Capacity", true)]
		public int capacity;

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
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

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			public unsafe readonly void* Ptr
			{
				get
				{
					return (void*)this.ListData->Ptr;
				}
			}

			internal unsafe ParallelWriter(UnsafeList<T>* listData)
			{
				this.ListData = listData;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void AddNoResize(T value)
			{
				int num = Interlocked.Increment(ref this.ListData->m_length) - 1;
				UnsafeUtility.WriteArrayElement<T>((void*)this.ListData->Ptr, num, value);
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void AddRangeNoResize(void* ptr, int count)
			{
				int num = Interlocked.Add(ref this.ListData->m_length, count) - count;
				void* ptr2 = (void*)(this.ListData->Ptr + num * sizeof(T) / sizeof(T));
				UnsafeUtility.MemCpy(ptr2, ptr, (long)(count * sizeof(T)));
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
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
