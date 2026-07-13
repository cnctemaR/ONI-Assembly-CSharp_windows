using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.NotBurstCompatible;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}")]
	[DebuggerTypeProxy(typeof(NativeListDebugView<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeList<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable, INativeList<T>, IIndexable<T>, IEnumerable<T>, IEnumerable where T : struct, ValueType
	{
		public NativeList(AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeList<T>(1, allocator, 2);
		}

		public NativeList(int initialCapacity, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeList<T>(initialCapacity, allocator, 2);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal void Initialize<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int initialCapacity, ref U allocator, int disposeSentinelStackDepth) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			this.m_ListData = UnsafeList<T>.Create<U>(initialCapacity, ref allocator, NativeArrayOptions.UninitializedMemory);
			this.m_DeprecatedAllocator = allocator.Handle;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal static NativeList<T> New<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int initialCapacity, ref U allocator, int disposeSentinelStackDepth) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			NativeList<T> nativeList = default(NativeList<T>);
			nativeList.Initialize<U>(initialCapacity, ref allocator, disposeSentinelStackDepth);
			return nativeList;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal static NativeList<T> New<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int initialCapacity, ref U allocator) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			return NativeList<T>.New<U>(initialCapacity, ref allocator, 2);
		}

		private NativeList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, int disposeSentinelStackDepth)
		{
			this = default(NativeList<T>);
			AllocatorManager.AllocatorHandle allocatorHandle = allocator;
			this.Initialize<AllocatorManager.AllocatorHandle>(initialCapacity, ref allocatorHandle, disposeSentinelStackDepth);
		}

		public unsafe T this[int index]
		{
			get
			{
				return (*this.m_ListData)[index];
			}
			set
			{
				(*this.m_ListData)[index] = value;
			}
		}

		public unsafe ref T ElementAt(int index)
		{
			return this.m_ListData->ElementAt(index);
		}

		public unsafe int Length
		{
			get
			{
				return CollectionHelper.AssumePositive(this.m_ListData->Length);
			}
			set
			{
				this.m_ListData->Resize(value, NativeArrayOptions.ClearMemory);
			}
		}

		public unsafe int Capacity
		{
			get
			{
				return this.m_ListData->Capacity;
			}
			set
			{
				this.m_ListData->Capacity = value;
			}
		}

		public unsafe UnsafeList<T>* GetUnsafeList()
		{
			return this.m_ListData;
		}

		public unsafe void AddNoResize(T value)
		{
			this.m_ListData->AddNoResize(value);
		}

		public unsafe void AddRangeNoResize(void* ptr, int count)
		{
			this.m_ListData->AddRangeNoResize(ptr, count);
		}

		public unsafe void AddRangeNoResize(NativeList<T> list)
		{
			this.m_ListData->AddRangeNoResize(*list.m_ListData);
		}

		public unsafe void Add(in T value)
		{
			this.m_ListData->Add(in value);
		}

		public void AddRange(NativeArray<T> array)
		{
			this.AddRange(array.GetUnsafeReadOnlyPtr<T>(), array.Length);
		}

		public unsafe void AddRange(void* ptr, int count)
		{
			this.m_ListData->AddRange(ptr, CollectionHelper.AssumePositive(count));
		}

		public unsafe void InsertRangeWithBeginEnd(int begin, int end)
		{
			this.m_ListData->InsertRangeWithBeginEnd(CollectionHelper.AssumePositive(begin), CollectionHelper.AssumePositive(end));
		}

		public unsafe void RemoveAtSwapBack(int index)
		{
			this.m_ListData->RemoveAtSwapBack(CollectionHelper.AssumePositive(index));
		}

		public unsafe void RemoveRangeSwapBack(int index, int count)
		{
			this.m_ListData->RemoveRangeSwapBack(CollectionHelper.AssumePositive(index), CollectionHelper.AssumePositive(count));
		}

		[Obsolete("RemoveRangeSwapBackWithBeginEnd(begin, end) is deprecated, use RemoveRangeSwapBack(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public unsafe void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			this.m_ListData->RemoveRangeSwapBackWithBeginEnd(CollectionHelper.AssumePositive(begin), CollectionHelper.AssumePositive(end));
		}

		public unsafe void RemoveAt(int index)
		{
			this.m_ListData->RemoveAt(CollectionHelper.AssumePositive(index));
		}

		public unsafe void RemoveRange(int index, int count)
		{
			this.m_ListData->RemoveRange(index, count);
		}

		[Obsolete("RemoveRangeWithBeginEnd(begin, end) is deprecated, use RemoveRange(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public unsafe void RemoveRangeWithBeginEnd(int begin, int end)
		{
			this.m_ListData->RemoveRangeWithBeginEnd(begin, end);
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
				return this.m_ListData != null;
			}
		}

		public void Dispose()
		{
			UnsafeList<T>.Destroy(this.m_ListData);
			this.m_ListData = null;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal void Dispose<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref U allocator) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			UnsafeList<T>.Destroy<U>(this.m_ListData, ref allocator);
			this.m_ListData = null;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			JobHandle jobHandle = new NativeListDisposeJob
			{
				Data = new NativeListDispose
				{
					m_ListData = (UntypedUnsafeList*)this.m_ListData
				}
			}.Schedule(inputDeps);
			this.m_ListData = null;
			return jobHandle;
		}

		public unsafe void Clear()
		{
			this.m_ListData->Clear();
		}

		public static implicit operator NativeArray<T>(NativeList<T> nativeList)
		{
			return nativeList.AsArray();
		}

		public unsafe NativeArray<T> AsArray()
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.m_ListData->Ptr, this.m_ListData->Length, Allocator.None);
		}

		public unsafe NativeArray<T> AsDeferredJobArray()
		{
			byte* ptr = (byte*)this.m_ListData;
			ptr++;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)ptr, 0, Allocator.Invalid);
		}

		[NotBurstCompatible]
		public T[] ToArray()
		{
			return this.ToArrayNBC<T>();
		}

		public NativeArray<T> ToArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<T> nativeArray = CollectionHelper.CreateNativeArray<T>(this.Length, allocator, NativeArrayOptions.UninitializedMemory);
			nativeArray.CopyFrom(this);
			return nativeArray;
		}

		public NativeArray<T>.Enumerator GetEnumerator()
		{
			NativeArray<T> nativeArray = this.AsArray();
			return new NativeArray<T>.Enumerator(ref nativeArray);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		[NotBurstCompatible]
		[Obsolete("Please use `CopyFromNBC` from `Unity.Collections.NotBurstCompatible` namespace instead. (RemovedAfter 2021-06-22)", false)]
		public void CopyFrom(T[] array)
		{
			this.CopyFromNBC<T>(array);
		}

		public void CopyFrom(NativeArray<T> array)
		{
			this.Clear();
			this.Resize(array.Length, NativeArrayOptions.UninitializedMemory);
			this.AsArray().CopyFrom(array);
		}

		public unsafe void Resize(int length, NativeArrayOptions options)
		{
			this.m_ListData->Resize(length, options);
		}

		public void ResizeUninitialized(int length)
		{
			this.Resize(length, NativeArrayOptions.UninitializedMemory);
		}

		public unsafe void SetCapacity(int capacity)
		{
			this.m_ListData->SetCapacity(capacity);
		}

		public unsafe void TrimExcess()
		{
			this.m_ListData->TrimExcess();
		}

		public unsafe NativeArray<T>.ReadOnly AsParallelReader()
		{
			return new NativeArray<T>.ReadOnly((void*)this.m_ListData->Ptr, this.m_ListData->Length);
		}

		public NativeList<T>.ParallelWriter AsParallelWriter()
		{
			return new NativeList<T>.ParallelWriter(this.m_ListData);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckInitialCapacity(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity", "Capacity must be >= 0");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckTotalSize(int initialCapacity, long totalSize)
		{
			if (totalSize > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("initialCapacity", string.Format("Capacity * sizeof(T) cannot exceed {0} bytes", int.MaxValue));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckSufficientCapacity(int capacity, int length)
		{
			if (capacity < length)
			{
				throw new Exception(string.Format("Length {0} exceeds capacity Capacity {1}", length, capacity));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckIndexInRange(int value, int length)
		{
			if (value < 0)
			{
				throw new IndexOutOfRangeException(string.Format("Value {0} must be positive.", value));
			}
			if (value >= length)
			{
				throw new IndexOutOfRangeException(string.Format("Value {0} is out of range in NativeList of '{1}' Length.", value, length));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckArgPositive(int value)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value {0} must be positive.", value));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private unsafe void CheckHandleMatches(AllocatorManager.AllocatorHandle handle)
		{
			if (this.m_ListData == null)
			{
				throw new ArgumentOutOfRangeException(string.Format("Allocator handle {0} can't match because container is not initialized.", handle));
			}
			if (this.m_ListData->Allocator.Index != handle.Index)
			{
				throw new ArgumentOutOfRangeException(string.Format("Allocator handle {0} can't match because container handle index doesn't match.", handle));
			}
			if (this.m_ListData->Allocator.Version != handle.Version)
			{
				throw new ArgumentOutOfRangeException(string.Format("Allocator handle {0} matches container handle index, but has different version.", handle));
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeList<T>* m_ListData;

		internal AllocatorManager.AllocatorHandle m_DeprecatedAllocator;

		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
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

			public unsafe void AddNoResize(T value)
			{
				int num = Interlocked.Increment(ref this.ListData->m_length) - 1;
				UnsafeUtility.WriteArrayElement<T>((void*)this.ListData->Ptr, num, value);
			}

			public unsafe void AddRangeNoResize(void* ptr, int count)
			{
				int num = Interlocked.Add(ref this.ListData->m_length, count) - count;
				int num2 = sizeof(T);
				void* ptr2 = (void*)(this.ListData->Ptr + num * num2 / sizeof(T));
				UnsafeUtility.MemCpy(ptr2, ptr, (long)(count * num2));
			}

			public unsafe void AddRangeNoResize(UnsafeList<T> list)
			{
				this.AddRangeNoResize((void*)list.Ptr, list.Length);
			}

			public unsafe void AddRangeNoResize(NativeList<T> list)
			{
				this.AddRangeNoResize(*list.m_ListData);
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe UnsafeList<T>* ListData;
		}
	}
}
