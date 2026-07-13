using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Count = {Count()}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafeHashMapDebuggerTypeProxy<, >))]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct UnsafeHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		public UnsafeHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_AllocatorLabel = allocator;
			UnsafeHashMapData.AllocateHashMap<TKey, TValue>(capacity, capacity * 2, allocator, out this.m_Buffer);
			this.Clear();
		}

		public bool IsEmpty
		{
			get
			{
				return !this.IsCreated || UnsafeHashMapData.IsEmpty(this.m_Buffer);
			}
		}

		public int Count()
		{
			return UnsafeHashMapData.GetCount(this.m_Buffer);
		}

		public unsafe int Capacity
		{
			get
			{
				return this.m_Buffer->keyCapacity;
			}
			set
			{
				UnsafeHashMapData.ReallocateHashMap<TKey, TValue>(this.m_Buffer, value, UnsafeHashMapData.GetBucketSize(value), this.m_AllocatorLabel);
			}
		}

		public void Clear()
		{
			UnsafeHashMapBase<TKey, TValue>.Clear(this.m_Buffer);
		}

		public bool TryAdd(TKey key, TValue item)
		{
			return UnsafeHashMapBase<TKey, TValue>.TryAdd(this.m_Buffer, key, item, false, this.m_AllocatorLabel);
		}

		public void Add(TKey key, TValue item)
		{
			this.TryAdd(key, item);
		}

		public bool Remove(TKey key)
		{
			return UnsafeHashMapBase<TKey, TValue>.Remove(this.m_Buffer, key, false) != 0;
		}

		public bool TryGetValue(TKey key, out TValue item)
		{
			NativeMultiHashMapIterator<TKey> nativeMultiHashMapIterator;
			return UnsafeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(this.m_Buffer, key, out item, out nativeMultiHashMapIterator);
		}

		public bool ContainsKey(TKey key)
		{
			TValue tvalue;
			NativeMultiHashMapIterator<TKey> nativeMultiHashMapIterator;
			return UnsafeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(this.m_Buffer, key, out tvalue, out nativeMultiHashMapIterator);
		}

		public TValue this[TKey key]
		{
			get
			{
				TValue tvalue;
				this.TryGetValue(key, out tvalue);
				return tvalue;
			}
			set
			{
				TValue tvalue;
				NativeMultiHashMapIterator<TKey> nativeMultiHashMapIterator;
				if (UnsafeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(this.m_Buffer, key, out tvalue, out nativeMultiHashMapIterator))
				{
					UnsafeHashMapBase<TKey, TValue>.SetValue(this.m_Buffer, ref nativeMultiHashMapIterator, ref value);
					return;
				}
				UnsafeHashMapBase<TKey, TValue>.TryAdd(this.m_Buffer, key, value, false, this.m_AllocatorLabel);
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Buffer != null;
			}
		}

		public void Dispose()
		{
			UnsafeHashMapData.DeallocateHashMap(this.m_Buffer, this.m_AllocatorLabel);
			this.m_Buffer = null;
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			JobHandle jobHandle = new UnsafeHashMapDisposeJob
			{
				Data = this.m_Buffer,
				Allocator = this.m_AllocatorLabel
			}.Schedule(inputDeps);
			this.m_Buffer = null;
			return jobHandle;
		}

		public NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TKey> nativeArray = CollectionHelper.CreateNativeArray<TKey>(UnsafeHashMapData.GetCount(this.m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeHashMapData.GetKeyArray<TKey>(this.m_Buffer, nativeArray);
			return nativeArray;
		}

		public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TValue> nativeArray = CollectionHelper.CreateNativeArray<TValue>(UnsafeHashMapData.GetCount(this.m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeHashMapData.GetValueArray<TValue>(this.m_Buffer, nativeArray);
			return nativeArray;
		}

		public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			NativeKeyValueArrays<TKey, TValue> nativeKeyValueArrays = new NativeKeyValueArrays<TKey, TValue>(UnsafeHashMapData.GetCount(this.m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeHashMapData.GetKeyValueArrays<TKey, TValue>(this.m_Buffer, nativeKeyValueArrays);
			return nativeKeyValueArrays;
		}

		public UnsafeHashMap<TKey, TValue>.ParallelWriter AsParallelWriter()
		{
			UnsafeHashMap<TKey, TValue>.ParallelWriter parallelWriter;
			parallelWriter.m_ThreadIndex = 0;
			parallelWriter.m_Buffer = this.m_Buffer;
			return parallelWriter;
		}

		public UnsafeHashMap<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new UnsafeHashMap<TKey, TValue>.Enumerator
			{
				m_Enumerator = new UnsafeHashMapDataEnumerator(this.m_Buffer)
			};
		}

		IEnumerator<KeyValue<TKey, TValue>> IEnumerable<KeyValue<TKey, TValue>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeHashMapData* m_Buffer;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

		[NativeContainerIsAtomicWriteOnly]
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public struct ParallelWriter
		{
			public unsafe int Capacity
			{
				get
				{
					return this.m_Buffer->keyCapacity;
				}
			}

			public bool TryAdd(TKey key, TValue item)
			{
				return UnsafeHashMapBase<TKey, TValue>.TryAddAtomic(this.m_Buffer, key, item, this.m_ThreadIndex);
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeHashMapData* m_Buffer;

			[NativeSetThreadIndex]
			internal int m_ThreadIndex;
		}

		public struct Enumerator : IEnumerator<KeyValue<TKey, TValue>>, IEnumerator, IDisposable
		{
			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_Enumerator.Reset();
			}

			public KeyValue<TKey, TValue> Current
			{
				get
				{
					return this.m_Enumerator.GetCurrent<TKey, TValue>();
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal UnsafeHashMapDataEnumerator m_Enumerator;
		}
	}
}
