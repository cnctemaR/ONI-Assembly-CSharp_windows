using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[Obsolete("Untyped UnsafeList is deprecated, please use UnsafeList<T> instead. (RemovedAfter 2021-05-18)", false)]
	public struct UnsafeList : INativeDisposable, IDisposable
	{
		public UnsafeList(Allocator allocator)
		{
			this = default(UnsafeList);
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
			this.Allocator = allocator;
		}

		public unsafe UnsafeList(void* ptr, int length)
		{
			this = default(UnsafeList);
			this.Ptr = ptr;
			this.Length = length;
			this.Capacity = length;
			this.Allocator = Unity.Collections.Allocator.None;
		}

		internal void Initialize<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int sizeOf, int alignOf, int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			this.Allocator = allocator.Handle;
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
			if (initialCapacity != 0)
			{
				this.SetCapacity<U>(ref allocator, sizeOf, alignOf, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && this.Ptr != null)
			{
				UnsafeUtility.MemClear(this.Ptr, (long)(this.Capacity * sizeOf));
			}
		}

		internal static UnsafeList New<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int sizeOf, int alignOf, int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			UnsafeList unsafeList = default(UnsafeList);
			unsafeList.Initialize<U>(sizeOf, alignOf, initialCapacity, ref allocator, options);
			return unsafeList;
		}

		public UnsafeList(int sizeOf, int alignOf, int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafeList);
			this = default(UnsafeList);
			this.Initialize<AllocatorManager.AllocatorHandle>(sizeOf, alignOf, initialCapacity, ref allocator, options);
		}

		public UnsafeList(int sizeOf, int alignOf, int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafeList);
			this.Allocator = allocator;
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
			if (initialCapacity != 0)
			{
				this.SetCapacity(sizeOf, alignOf, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && this.Ptr != null)
			{
				UnsafeUtility.MemClear(this.Ptr, (long)(this.Capacity * sizeOf));
			}
		}

		public unsafe static UnsafeList* Create(int sizeOf, int alignOf, int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafeList* ptr = AllocatorManager.Allocate<UnsafeList>(allocator, 1);
			UnsafeUtility.MemClear((void*)ptr, (long)UnsafeUtility.SizeOf<UnsafeList>());
			ptr->Allocator = allocator;
			if (initialCapacity != 0)
			{
				ptr->SetCapacity(sizeOf, alignOf, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && ptr->Ptr != null)
			{
				UnsafeUtility.MemClear(ptr->Ptr, (long)(ptr->Capacity * sizeOf));
			}
			return ptr;
		}

		internal unsafe static UnsafeList* Create<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int sizeOf, int alignOf, int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			UnsafeList* ptr = (ref allocator).Allocate(default(UnsafeList), 1);
			UnsafeUtility.MemClear((void*)ptr, (long)UnsafeUtility.SizeOf<UnsafeList>());
			ptr->Allocator = allocator.Handle;
			if (initialCapacity != 0)
			{
				ptr->SetCapacity<U>(ref allocator, sizeOf, alignOf, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && ptr->Ptr != null)
			{
				UnsafeUtility.MemClear(ptr->Ptr, (long)(ptr->Capacity * sizeOf));
			}
			return ptr;
		}

		internal unsafe static void Destroy<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(UnsafeList* listData, ref U allocator, int sizeOf, int alignOf) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			listData->Dispose<U>(ref allocator, sizeOf, alignOf);
			(ref allocator).Free<U>((void*)listData, UnsafeUtility.SizeOf<UnsafeList>(), UnsafeUtility.AlignOf<UnsafeList>(), 1);
		}

		public unsafe static void Destroy(UnsafeList* listData)
		{
			AllocatorManager.AllocatorHandle allocator = listData->Allocator;
			listData->Dispose();
			AllocatorManager.Free<UnsafeList>(allocator, listData, 1);
		}

		public bool IsEmpty
		{
			get
			{
				return !this.IsCreated || this.Length == 0;
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
				AllocatorManager.Free(this.Allocator, this.Ptr);
				this.Allocator = AllocatorManager.Invalid;
			}
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
		}

		internal void Dispose<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator, int sizeOf, int alignOf) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			(ref allocator).Free<U>(this.Ptr, sizeOf, alignOf, this.Length);
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				JobHandle jobHandle = new UnsafeDisposeJob
				{
					Ptr = this.Ptr,
					Allocator = (Allocator)this.Allocator.Value
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
			this.Length = 0;
		}

		public unsafe void Resize(int sizeOf, int alignOf, int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			int length2 = this.Length;
			if (length > this.Capacity)
			{
				this.SetCapacity(sizeOf, alignOf, length);
			}
			this.Length = length;
			if (options == NativeArrayOptions.ClearMemory && length2 < length)
			{
				int num = length - length2;
				byte* ptr = (byte*)this.Ptr;
				UnsafeUtility.MemClear((void*)(ptr + length2 * sizeOf), (long)(num * sizeOf));
			}
		}

		public void Resize<T>(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where T : struct
		{
			this.Resize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), length, options);
		}

		private unsafe void Realloc<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator, int sizeOf, int alignOf, int capacity) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			void* ptr = null;
			if (capacity > 0)
			{
				ptr = (ref allocator).Allocate<U>(sizeOf, alignOf, capacity);
				if (this.Capacity > 0)
				{
					int num = math.min(capacity, this.Capacity) * sizeOf;
					UnsafeUtility.MemCpy(ptr, this.Ptr, (long)num);
				}
			}
			(ref allocator).Free<U>(this.Ptr, sizeOf, alignOf, this.Capacity);
			this.Ptr = ptr;
			this.Capacity = capacity;
			this.Length = math.min(this.Length, capacity);
		}

		private void Realloc(int sizeOf, int alignOf, int capacity)
		{
			this.Realloc<AllocatorManager.AllocatorHandle>(ref this.Allocator, sizeOf, alignOf, capacity);
		}

		private void SetCapacity<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator, int sizeOf, int alignOf, int capacity) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			int num = math.max(capacity, 64 / sizeOf);
			num = math.ceilpow2(num);
			if (num == this.Capacity)
			{
				return;
			}
			this.Realloc<U>(ref allocator, sizeOf, alignOf, num);
		}

		private void SetCapacity(int sizeOf, int alignOf, int capacity)
		{
			this.SetCapacity<AllocatorManager.AllocatorHandle>(ref this.Allocator, sizeOf, alignOf, capacity);
		}

		public void SetCapacity<T>(int capacity) where T : struct
		{
			this.SetCapacity(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), capacity);
		}

		public void TrimExcess<T>() where T : struct
		{
			if (this.Capacity != this.Length)
			{
				this.Realloc(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), this.Length);
			}
		}

		public int IndexOf<T>(T value) where T : struct, IEquatable<T>
		{
			return NativeArrayExtensions.IndexOf<T, T>(this.Ptr, this.Length, value);
		}

		public bool Contains<T>(T value) where T : struct, IEquatable<T>
		{
			return this.IndexOf<T>(value) != -1;
		}

		public void AddNoResize<T>(T value) where T : struct
		{
			UnsafeUtility.WriteArrayElement<T>(this.Ptr, this.Length, value);
			this.Length++;
		}

		private unsafe void AddRangeNoResize(int sizeOf, void* ptr, int length)
		{
			void* ptr2 = (void*)((byte*)this.Ptr + this.Length * sizeOf);
			UnsafeUtility.MemCpy(ptr2, ptr, (long)(length * sizeOf));
			this.Length += length;
		}

		public unsafe void AddRangeNoResize<T>(void* ptr, int length) where T : struct
		{
			this.AddRangeNoResize(UnsafeUtility.SizeOf<T>(), ptr, length);
		}

		public void AddRangeNoResize<T>(UnsafeList list) where T : struct
		{
			this.AddRangeNoResize(UnsafeUtility.SizeOf<T>(), list.Ptr, CollectionHelper.AssumePositive(list.Length));
		}

		public void Add<T>(T value) where T : struct
		{
			int length = this.Length;
			if (this.Length + 1 > this.Capacity)
			{
				this.Resize<T>(length + 1, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.Length++;
			}
			UnsafeUtility.WriteArrayElement<T>(this.Ptr, length, value);
		}

		private unsafe void AddRange(int sizeOf, int alignOf, void* ptr, int length)
		{
			int length2 = this.Length;
			if (this.Length + length > this.Capacity)
			{
				this.Resize(sizeOf, alignOf, this.Length + length, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.Length += length;
			}
			void* ptr2 = (void*)((byte*)this.Ptr + length2 * sizeOf);
			UnsafeUtility.MemCpy(ptr2, ptr, (long)(length * sizeOf));
		}

		public unsafe void AddRange<T>(void* ptr, int length) where T : struct
		{
			this.AddRange(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), ptr, length);
		}

		public void AddRange<T>(UnsafeList list) where T : struct
		{
			this.AddRange(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), list.Ptr, list.Length);
		}

		private unsafe void InsertRangeWithBeginEnd(int sizeOf, int alignOf, int begin, int end)
		{
			int num = end - begin;
			if (num < 1)
			{
				return;
			}
			int length = this.Length;
			if (this.Length + num > this.Capacity)
			{
				this.Resize(sizeOf, alignOf, this.Length + num, NativeArrayOptions.UninitializedMemory);
			}
			else
			{
				this.Length += num;
			}
			int num2 = length - begin;
			if (num2 < 1)
			{
				return;
			}
			int num3 = num2 * sizeOf;
			byte* ptr = (byte*)this.Ptr;
			void* ptr2 = (void*)(ptr + end * sizeOf);
			byte* ptr3 = ptr + begin * sizeOf;
			UnsafeUtility.MemMove(ptr2, (void*)ptr3, (long)num3);
		}

		public void InsertRangeWithBeginEnd<T>(int begin, int end) where T : struct
		{
			this.InsertRangeWithBeginEnd(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), begin, end);
		}

		private unsafe void RemoveRangeSwapBackWithBeginEnd(int sizeOf, int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				int num2 = math.max(this.Length - num, end);
				void* ptr = (void*)((byte*)this.Ptr + begin * sizeOf);
				void* ptr2 = (void*)((byte*)this.Ptr + num2 * sizeOf);
				UnsafeUtility.MemCpy(ptr, ptr2, (long)((this.Length - num2) * sizeOf));
				this.Length -= num;
			}
		}

		public void RemoveAtSwapBack<T>(int index) where T : struct
		{
			this.RemoveRangeSwapBackWithBeginEnd<T>(index, index + 1);
		}

		public void RemoveRangeSwapBackWithBeginEnd<T>(int begin, int end) where T : struct
		{
			this.RemoveRangeSwapBackWithBeginEnd(UnsafeUtility.SizeOf<T>(), begin, end);
		}

		private unsafe void RemoveRangeWithBeginEnd(int sizeOf, int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				int num2 = math.min(begin + num, this.Length);
				void* ptr = (void*)((byte*)this.Ptr + begin * sizeOf);
				void* ptr2 = (void*)((byte*)this.Ptr + num2 * sizeOf);
				UnsafeUtility.MemCpy(ptr, ptr2, (long)((this.Length - num2) * sizeOf));
				this.Length -= num;
			}
		}

		public void RemoveAt<T>(int index) where T : struct
		{
			this.RemoveRangeWithBeginEnd<T>(index, index + 1);
		}

		public void RemoveRangeWithBeginEnd<T>(int begin, int end) where T : struct
		{
			this.RemoveRangeWithBeginEnd(UnsafeUtility.SizeOf<T>(), begin, end);
		}

		public UnsafeList.ParallelReader AsParallelReader()
		{
			return new UnsafeList.ParallelReader(this.Ptr, this.Length);
		}

		public unsafe UnsafeList.ParallelWriter AsParallelWriter()
		{
			return new UnsafeList.ParallelWriter(this.Ptr, (UnsafeList*)UnsafeUtility.AddressOf<UnsafeList>(ref this));
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
		private static void CheckAllocator(Allocator a)
		{
			if (!CollectionHelper.ShouldDeallocate(a))
			{
				throw new Exception("UnsafeList is not initialized, it must be initialized with allocator before use.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckAllocator(AllocatorManager.AllocatorHandle a)
		{
			if (!CollectionHelper.ShouldDeallocate(a))
			{
				throw new Exception("UnsafeList is not initialized, it must be initialized with allocator before use.");
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
		public unsafe void* Ptr;

		public int Length;

		public readonly int unused;

		public int Capacity;

		public AllocatorManager.AllocatorHandle Allocator;

		public struct ParallelReader
		{
			internal unsafe ParallelReader(void* ptr, int length)
			{
				this.Ptr = ptr;
				this.Length = length;
			}

			public int IndexOf<T>(T value) where T : struct, IEquatable<T>
			{
				return NativeArrayExtensions.IndexOf<T, T>(this.Ptr, this.Length, value);
			}

			public bool Contains<T>(T value) where T : struct, IEquatable<T>
			{
				return this.IndexOf<T>(value) != -1;
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly void* Ptr;

			public readonly int Length;
		}

		public struct ParallelWriter
		{
			internal unsafe ParallelWriter(void* ptr, UnsafeList* listData)
			{
				this.Ptr = ptr;
				this.ListData = listData;
			}

			public unsafe void AddNoResize<T>(T value) where T : struct
			{
				int num = Interlocked.Increment(ref this.ListData->Length) - 1;
				UnsafeUtility.WriteArrayElement<T>(this.Ptr, num, value);
			}

			private unsafe void AddRangeNoResize(int sizeOf, int alignOf, void* ptr, int length)
			{
				int num = Interlocked.Add(ref this.ListData->Length, length) - length;
				void* ptr2 = (void*)((byte*)this.Ptr + num * sizeOf);
				UnsafeUtility.MemCpy(ptr2, ptr, (long)(length * sizeOf));
			}

			public unsafe void AddRangeNoResize<T>(void* ptr, int length) where T : struct
			{
				this.AddRangeNoResize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), ptr, length);
			}

			public void AddRangeNoResize<T>(UnsafeList list) where T : struct
			{
				this.AddRangeNoResize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), list.Ptr, list.Length);
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly void* Ptr;

			[NativeDisableUnsafePtrRestriction]
			public unsafe UnsafeList* ListData;
		}
	}
}
