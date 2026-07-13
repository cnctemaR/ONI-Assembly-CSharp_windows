using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerTypeProxy(typeof(UnsafeMultiHashMapDebuggerTypeProxy<, >))]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct UnsafeMultiHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		public UnsafeMultiHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
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

		public unsafe int Count()
		{
			if (this.m_Buffer->allocatedIndexLength <= 0)
			{
				return 0;
			}
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

		public void Add(TKey key, TValue item)
		{
			UnsafeHashMapBase<TKey, TValue>.TryAdd(this.m_Buffer, key, item, true, this.m_AllocatorLabel);
		}

		public int Remove(TKey key)
		{
			return UnsafeHashMapBase<TKey, TValue>.Remove(this.m_Buffer, key, true);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public void Remove<TValueEQ>(TKey key, TValueEQ value) where TValueEQ : struct, IEquatable<TValueEQ>
		{
			UnsafeHashMapBase<TKey, TValueEQ>.RemoveKeyValue<TValueEQ>(this.m_Buffer, key, value);
		}

		public void Remove(NativeMultiHashMapIterator<TKey> it)
		{
			UnsafeHashMapBase<TKey, TValue>.Remove(this.m_Buffer, it);
		}

		public bool TryGetFirstValue(TKey key, out TValue item, out NativeMultiHashMapIterator<TKey> it)
		{
			return UnsafeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(this.m_Buffer, key, out item, out it);
		}

		public bool TryGetNextValue(out TValue item, ref NativeMultiHashMapIterator<TKey> it)
		{
			return UnsafeHashMapBase<TKey, TValue>.TryGetNextValueAtomic(this.m_Buffer, out item, ref it);
		}

		public bool ContainsKey(TKey key)
		{
			TValue tvalue;
			NativeMultiHashMapIterator<TKey> nativeMultiHashMapIterator;
			return this.TryGetFirstValue(key, out tvalue, out nativeMultiHashMapIterator);
		}

		public int CountValuesForKey(TKey key)
		{
			TValue tvalue;
			NativeMultiHashMapIterator<TKey> nativeMultiHashMapIterator;
			if (!this.TryGetFirstValue(key, out tvalue, out nativeMultiHashMapIterator))
			{
				return 0;
			}
			int num = 1;
			while (this.TryGetNextValue(out tvalue, ref nativeMultiHashMapIterator))
			{
				num++;
			}
			return num;
		}

		public bool SetValue(TValue item, NativeMultiHashMapIterator<TKey> it)
		{
			return UnsafeHashMapBase<TKey, TValue>.SetValue(this.m_Buffer, ref it, ref item);
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
			NativeArray<TKey> nativeArray = CollectionHelper.CreateNativeArray<TKey>(this.Count(), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeHashMapData.GetKeyArray<TKey>(this.m_Buffer, nativeArray);
			return nativeArray;
		}

		public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TValue> nativeArray = CollectionHelper.CreateNativeArray<TValue>(this.Count(), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeHashMapData.GetValueArray<TValue>(this.m_Buffer, nativeArray);
			return nativeArray;
		}

		public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			NativeKeyValueArrays<TKey, TValue> nativeKeyValueArrays = new NativeKeyValueArrays<TKey, TValue>(this.Count(), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeHashMapData.GetKeyValueArrays<TKey, TValue>(this.m_Buffer, nativeKeyValueArrays);
			return nativeKeyValueArrays;
		}

		public UnsafeMultiHashMap<TKey, TValue>.Enumerator GetValuesForKey(TKey key)
		{
			return new UnsafeMultiHashMap<TKey, TValue>.Enumerator
			{
				hashmap = this,
				key = key,
				isFirst = true
			};
		}

		public UnsafeMultiHashMap<TKey, TValue>.ParallelWriter AsParallelWriter()
		{
			UnsafeMultiHashMap<TKey, TValue>.ParallelWriter parallelWriter;
			parallelWriter.m_ThreadIndex = 0;
			parallelWriter.m_Buffer = this.m_Buffer;
			return parallelWriter;
		}

		public UnsafeMultiHashMap<TKey, TValue>.KeyValueEnumerator GetEnumerator()
		{
			return new UnsafeMultiHashMap<TKey, TValue>.KeyValueEnumerator
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

		public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
		{
			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (this.isFirst)
				{
					this.isFirst = false;
					return this.hashmap.TryGetFirstValue(this.key, out this.value, out this.iterator);
				}
				return this.hashmap.TryGetNextValue(out this.value, ref this.iterator);
			}

			public void Reset()
			{
				this.isFirst = true;
			}

			public TValue Current
			{
				get
				{
					return this.value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public UnsafeMultiHashMap<TKey, TValue>.Enumerator GetEnumerator()
			{
				return this;
			}

			internal UnsafeMultiHashMap<TKey, TValue> hashmap;

			internal TKey key;

			internal bool isFirst;

			private TValue value;

			private NativeMultiHashMapIterator<TKey> iterator;
		}

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

			public void Add(TKey key, TValue item)
			{
				UnsafeHashMapBase<TKey, TValue>.AddAtomicMulti(this.m_Buffer, key, item, this.m_ThreadIndex);
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeHashMapData* m_Buffer;

			[NativeSetThreadIndex]
			internal int m_ThreadIndex;
		}

		public struct KeyValueEnumerator : IEnumerator<KeyValue<TKey, TValue>>, IEnumerator, IDisposable
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
