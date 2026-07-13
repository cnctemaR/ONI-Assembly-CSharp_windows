using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafePtrListDebugView<>))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafePtrList<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable, IEnumerable<IntPtr>, IEnumerable where T : struct, ValueType
	{
		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.ListDataRO<T>().Length;
			}
			set
			{
				(ref this).ListData<T>().Length = value;
			}
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.ListDataRO<T>().Capacity;
			}
			set
			{
				(ref this).ListData<T>().Capacity = value;
			}
		}

		public unsafe T* this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return *(IntPtr*)(this.Ptr + (IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T*) / (IntPtr)sizeof(T*));
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				*(IntPtr*)(this.Ptr + (IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T*) / (IntPtr)sizeof(T*)) = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref T* ElementAt(int index)
		{
			return ref this.Ptr[(IntPtr)CollectionHelper.AssumePositive(index) * (IntPtr)sizeof(T*) / (IntPtr)sizeof(T*)];
		}

		public unsafe UnsafePtrList(T** ptr, int length)
		{
			this = default(UnsafePtrList<T>);
			this.Ptr = ptr;
			this.m_length = length;
			this.m_capacity = length;
			this.Allocator = AllocatorManager.None;
		}

		public unsafe UnsafePtrList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this.Ptr = null;
			this.m_length = 0;
			this.m_capacity = 0;
			this.padding = 0;
			this.Allocator = AllocatorManager.None;
			*(ref this).ListData<T>() = new UnsafeList<IntPtr>(initialCapacity, allocator, options);
		}

		public unsafe static UnsafePtrList<T>* Create(T** ptr, int length)
		{
			UnsafePtrList<T>* ptr2 = AllocatorManager.Allocate<UnsafePtrList<T>>(AllocatorManager.Persistent, 1);
			*ptr2 = new UnsafePtrList<T>(ptr, length);
			return ptr2;
		}

		public unsafe static UnsafePtrList<T>* Create(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafePtrList<T>* ptr = AllocatorManager.Allocate<UnsafePtrList<T>>(allocator, 1);
			*ptr = new UnsafePtrList<T>(initialCapacity, allocator, options);
			return ptr;
		}

		public unsafe static void Destroy(UnsafePtrList<T>* listData)
		{
			AllocatorManager.AllocatorHandle allocatorHandle = (((ref *listData).ListData<T>().Allocator.Value == AllocatorManager.Invalid.Value) ? AllocatorManager.Persistent : (ref *listData).ListData<T>().Allocator);
			listData->Dispose();
			AllocatorManager.Free<UnsafePtrList<T>>(allocatorHandle, listData, 1);
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return !this.IsCreated || this.Length == 0;
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

		public void Dispose()
		{
			(ref this).ListData<T>().Dispose();
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			return (ref this).ListData<T>().Dispose(inputDeps);
		}

		public void Clear()
		{
			(ref this).ListData<T>().Clear();
		}

		public void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			(ref this).ListData<T>().Resize(length, options);
		}

		public void SetCapacity(int capacity)
		{
			(ref this).ListData<T>().SetCapacity(capacity);
		}

		public void TrimExcess()
		{
			(ref this).ListData<T>().TrimExcess();
		}

		public unsafe int IndexOf(void* ptr)
		{
			for (int i = 0; i < this.Length; i++)
			{
				if (*(IntPtr*)(this.Ptr + (IntPtr)i * (IntPtr)sizeof(T*) / (IntPtr)sizeof(T*)) == ptr)
				{
					return i;
				}
			}
			return -1;
		}

		public unsafe bool Contains(void* ptr)
		{
			return this.IndexOf(ptr) != -1;
		}

		public unsafe void AddNoResize(void* value)
		{
			(ref this).ListData<T>().AddNoResize((IntPtr)value);
		}

		public unsafe void AddRangeNoResize(void** ptr, int count)
		{
			(ref this).ListData<T>().AddRangeNoResize((void*)ptr, count);
		}

		public unsafe void AddRangeNoResize(UnsafePtrList<T> list)
		{
			(ref this).ListData<T>().AddRangeNoResize((void*)list.Ptr, list.Length);
		}

		public void Add(in IntPtr value)
		{
			(ref this).ListData<T>().Add(in value);
		}

		public unsafe void Add(void* value)
		{
			ref UnsafeList<IntPtr> ptr = ref (ref this).ListData<T>();
			IntPtr intPtr = (IntPtr)value;
			ptr.Add(in intPtr);
		}

		public unsafe void AddRange(void* ptr, int length)
		{
			(ref this).ListData<T>().AddRange(ptr, length);
		}

		public unsafe void AddRange(UnsafePtrList<T> list)
		{
			(ref this).ListData<T>().AddRange(*(ref list).ListData<T>());
		}

		public void InsertRangeWithBeginEnd(int begin, int end)
		{
			(ref this).ListData<T>().InsertRangeWithBeginEnd(begin, end);
		}

		public void RemoveAtSwapBack(int index)
		{
			(ref this).ListData<T>().RemoveAtSwapBack(index);
		}

		public void RemoveRangeSwapBack(int index, int count)
		{
			(ref this).ListData<T>().RemoveRangeSwapBack(index, count);
		}

		public void RemoveAt(int index)
		{
			(ref this).ListData<T>().RemoveAt(index);
		}

		public void RemoveRange(int index, int count)
		{
			(ref this).ListData<T>().RemoveRange(index, count);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<IntPtr> IEnumerable<IntPtr>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public UnsafePtrList<T>.ReadOnly AsReadOnly()
		{
			return new UnsafePtrList<T>.ReadOnly(this.Ptr, this.Length);
		}

		public UnsafePtrList<T>.ParallelReader AsParallelReader()
		{
			return new UnsafePtrList<T>.ParallelReader(this.Ptr, this.Length);
		}

		public unsafe UnsafePtrList<T>.ParallelWriter AsParallelWriter()
		{
			return new UnsafePtrList<T>.ParallelWriter(this.Ptr, (UnsafeList<IntPtr>*)UnsafeUtility.AddressOf<UnsafePtrList<T>>(ref this));
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe readonly T** Ptr;

		public readonly int m_length;

		public readonly int m_capacity;

		public readonly AllocatorManager.AllocatorHandle Allocator;

		private readonly int padding;

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ReadOnly
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

			internal unsafe ReadOnly(T** ptr, int length)
			{
				this.Ptr = ptr;
				this.Length = length;
			}

			public unsafe int IndexOf(void* ptr)
			{
				for (int i = 0; i < this.Length; i++)
				{
					if (*(IntPtr*)(this.Ptr + (IntPtr)i * (IntPtr)sizeof(T*) / (IntPtr)sizeof(T*)) == ptr)
					{
						return i;
					}
				}
				return -1;
			}

			public unsafe bool Contains(void* ptr)
			{
				return this.IndexOf(ptr) != -1;
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly T** Ptr;

			public readonly int Length;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelReader
		{
			internal unsafe ParallelReader(T** ptr, int length)
			{
				this.Ptr = ptr;
				this.Length = length;
			}

			public unsafe int IndexOf(void* ptr)
			{
				for (int i = 0; i < this.Length; i++)
				{
					if (*(IntPtr*)(this.Ptr + (IntPtr)i * (IntPtr)sizeof(T*) / (IntPtr)sizeof(T*)) == ptr)
					{
						return i;
					}
				}
				return -1;
			}

			public unsafe bool Contains(void* ptr)
			{
				return this.IndexOf(ptr) != -1;
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly T** Ptr;

			public readonly int Length;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			internal unsafe ParallelWriter(T** ptr, UnsafeList<IntPtr>* listData)
			{
				this.Ptr = ptr;
				this.ListData = listData;
			}

			public unsafe void AddNoResize(T* value)
			{
				this.ListData->AddNoResize((IntPtr)((void*)value));
			}

			public unsafe void AddRangeNoResize(T** ptr, int count)
			{
				this.ListData->AddRangeNoResize((void*)ptr, count);
			}

			public unsafe void AddRangeNoResize(UnsafePtrList<T> list)
			{
				this.ListData->AddRangeNoResize((void*)list.Ptr, list.Length);
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly T** Ptr;

			[NativeDisableUnsafePtrRestriction]
			public unsafe UnsafeList<IntPtr>* ListData;
		}
	}
}
