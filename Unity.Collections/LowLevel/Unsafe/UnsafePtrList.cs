using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafePtrListDebugView))]
	[Obsolete("Untyped UnsafePtrList is deprecated, please use UnsafePtrList<T> instead. (RemovedAfter 2021-05-18)", false)]
	public struct UnsafePtrList : INativeDisposable, IDisposable, INativeList<IntPtr>, IIndexable<IntPtr>, IEnumerable<IntPtr>, IEnumerable
	{
		public int Length
		{
			get
			{
				return this.length;
			}
			set
			{
			}
		}

		public int Capacity
		{
			get
			{
				return this.capacity;
			}
			set
			{
			}
		}

		public unsafe IntPtr this[int index]
		{
			get
			{
				return new IntPtr(*(IntPtr*)(this.Ptr + (IntPtr)index * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)));
			}
			set
			{
				*(IntPtr*)(this.Ptr + (IntPtr)index * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)) = (void*)value;
			}
		}

		public unsafe ref IntPtr ElementAt(int index)
		{
			return ref *(IntPtr*)(this.Ptr + (IntPtr)index * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(void*));
		}

		public unsafe UnsafePtrList(void** ptr, int length)
		{
			this = default(UnsafePtrList);
			this.Ptr = ptr;
			this.length = length;
			this.capacity = length;
			this.Allocator = AllocatorManager.None;
		}

		public unsafe UnsafePtrList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafePtrList);
			this.Ptr = null;
			this.length = 0;
			this.capacity = 0;
			this.Allocator = AllocatorManager.None;
			int size = IntPtr.Size;
			*(ref this).ListData() = new UnsafeList(size, size, initialCapacity, allocator, options);
		}

		public unsafe UnsafePtrList(int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafePtrList);
			this.Ptr = null;
			this.length = 0;
			this.capacity = 0;
			this.Allocator = AllocatorManager.None;
			int size = IntPtr.Size;
			*(ref this).ListData() = new UnsafeList(size, size, initialCapacity, allocator, options);
		}

		public unsafe static UnsafePtrList* Create(void** ptr, int length)
		{
			UnsafePtrList* ptr2 = AllocatorManager.Allocate<UnsafePtrList>(AllocatorManager.Persistent, 1);
			*ptr2 = new UnsafePtrList(ptr, length);
			return ptr2;
		}

		public unsafe static UnsafePtrList* Create(int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafePtrList* ptr = AllocatorManager.Allocate<UnsafePtrList>(allocator, 1);
			*ptr = new UnsafePtrList(initialCapacity, allocator, options);
			return ptr;
		}

		public unsafe static void Destroy(UnsafePtrList* listData)
		{
			AllocatorManager.AllocatorHandle allocatorHandle = (((ref *listData).ListData().Allocator.Value == AllocatorManager.Invalid.Value) ? AllocatorManager.Persistent : (ref *listData).ListData().Allocator);
			listData->Dispose();
			AllocatorManager.Free<UnsafePtrList>(allocatorHandle, listData, 1);
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
			(ref this).ListData().Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return (ref this).ListData().Dispose(inputDeps);
		}

		public void Clear()
		{
			(ref this).ListData().Clear();
		}

		public void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			(ref this).ListData().Resize<IntPtr>(length, options);
		}

		public void SetCapacity(int capacity)
		{
			(ref this).ListData().SetCapacity<IntPtr>(capacity);
		}

		public void TrimExcess()
		{
			(ref this).ListData().TrimExcess<IntPtr>();
		}

		public unsafe int IndexOf(void* value)
		{
			for (int i = 0; i < this.Length; i++)
			{
				if (*(IntPtr*)(this.Ptr + (IntPtr)i * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)) == value)
				{
					return i;
				}
			}
			return -1;
		}

		public unsafe bool Contains(void* value)
		{
			return this.IndexOf(value) != -1;
		}

		public unsafe void AddNoResize(void* value)
		{
			(ref this).ListData().AddNoResize<IntPtr>((IntPtr)value);
		}

		public unsafe void AddRangeNoResize(void** ptr, int length)
		{
			(ref this).ListData().AddRangeNoResize<IntPtr>((void*)ptr, length);
		}

		public unsafe void AddRangeNoResize(UnsafePtrList list)
		{
			(ref this).ListData().AddRangeNoResize<IntPtr>((void*)list.Ptr, list.Length);
		}

		public void Add(in IntPtr value)
		{
			(ref this).ListData().Add<IntPtr>(value);
		}

		public unsafe void Add(void* value)
		{
			(ref this).ListData().Add<IntPtr>((IntPtr)value);
		}

		public unsafe void AddRange(void* ptr, int length)
		{
			(ref this).ListData().AddRange<IntPtr>(ptr, length);
		}

		public unsafe void AddRange(UnsafePtrList list)
		{
			(ref this).ListData().AddRange<IntPtr>(*(ref list).ListData());
		}

		public void InsertRangeWithBeginEnd(int begin, int end)
		{
			(ref this).ListData().InsertRangeWithBeginEnd<IntPtr>(begin, end);
		}

		public void RemoveAtSwapBack(int index)
		{
			(ref this).ListData().RemoveAtSwapBack<IntPtr>(index);
		}

		public void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			(ref this).ListData().RemoveRangeSwapBackWithBeginEnd<IntPtr>(begin, end);
		}

		public void RemoveAt(int index)
		{
			(ref this).ListData().RemoveAt<IntPtr>(index);
		}

		public void RemoveRangeWithBeginEnd(int begin, int end)
		{
			(ref this).ListData().RemoveRangeWithBeginEnd<IntPtr>(begin, end);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<IntPtr> IEnumerable<IntPtr>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public UnsafePtrList.ParallelReader AsParallelReader()
		{
			return new UnsafePtrList.ParallelReader(this.Ptr, this.Length);
		}

		public unsafe UnsafePtrList.ParallelWriter AsParallelWriter()
		{
			return new UnsafePtrList.ParallelWriter((void*)this.Ptr, (UnsafeList*)UnsafeUtility.AddressOf<UnsafePtrList>(ref this));
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe readonly void** Ptr;

		public readonly int length;

		public readonly int unused;

		public readonly int capacity;

		public readonly AllocatorManager.AllocatorHandle Allocator;

		public struct ParallelReader
		{
			internal unsafe ParallelReader(void** ptr, int length)
			{
				this.Ptr = ptr;
				this.Length = length;
			}

			public unsafe int IndexOf(void* value)
			{
				for (int i = 0; i < this.Length; i++)
				{
					if (*(IntPtr*)(this.Ptr + (IntPtr)i * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)) == value)
					{
						return i;
					}
				}
				return -1;
			}

			public unsafe bool Contains(void* value)
			{
				return this.IndexOf(value) != -1;
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly void** Ptr;

			public readonly int Length;
		}

		public struct ParallelWriter
		{
			internal unsafe ParallelWriter(void* ptr, UnsafeList* listData)
			{
				this.Ptr = ptr;
				this.ListData = listData;
			}

			public unsafe void AddNoResize(void* value)
			{
				this.ListData->AddNoResize<IntPtr>((IntPtr)value);
			}

			public unsafe void AddRangeNoResize(void** ptr, int length)
			{
				this.ListData->AddRangeNoResize<IntPtr>((void*)ptr, length);
			}

			public unsafe void AddRangeNoResize(UnsafePtrList list)
			{
				this.ListData->AddRangeNoResize<IntPtr>((void*)list.Ptr, list.Length);
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly void* Ptr;

			[NativeDisableUnsafePtrRestriction]
			public unsafe UnsafeList* ListData;
		}
	}
}
