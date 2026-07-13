using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Count = {m_HashMapData.Count()}, Capacity = {m_HashMapData.Capacity}, IsCreated = {m_HashMapData.IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(NativeHashMapDebuggerTypeProxy<, >))]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct NativeHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		public NativeHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeHashMap<TKey, TValue>(capacity, allocator, 2);
		}

		private NativeHashMap(int capacity, AllocatorManager.AllocatorHandle allocator, int disposeSentinelStackDepth)
		{
			this.m_HashMapData = new UnsafeHashMap<TKey, TValue>(capacity, allocator);
		}

		public bool IsEmpty
		{
			get
			{
				return !this.IsCreated || this.m_HashMapData.IsEmpty;
			}
		}

		public int Count()
		{
			return this.m_HashMapData.Count();
		}

		public int Capacity
		{
			get
			{
				return this.m_HashMapData.Capacity;
			}
			set
			{
				this.m_HashMapData.Capacity = value;
			}
		}

		public void Clear()
		{
			this.m_HashMapData.Clear();
		}

		public bool TryAdd(TKey key, TValue item)
		{
			return this.m_HashMapData.TryAdd(key, item);
		}

		public void Add(TKey key, TValue item)
		{
			this.TryAdd(key, item);
		}

		public bool Remove(TKey key)
		{
			return this.m_HashMapData.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue item)
		{
			return this.m_HashMapData.TryGetValue(key, out item);
		}

		public bool ContainsKey(TKey key)
		{
			return this.m_HashMapData.ContainsKey(key);
		}

		public TValue this[TKey key]
		{
			get
			{
				TValue tvalue;
				if (this.m_HashMapData.TryGetValue(key, out tvalue))
				{
					return tvalue;
				}
				return default(TValue);
			}
			set
			{
				this.m_HashMapData[key] = value;
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_HashMapData.IsCreated;
			}
		}

		public void Dispose()
		{
			this.m_HashMapData.Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			JobHandle jobHandle = new UnsafeHashMapDataDisposeJob
			{
				Data = new UnsafeHashMapDataDispose
				{
					m_Buffer = this.m_HashMapData.m_Buffer,
					m_AllocatorLabel = this.m_HashMapData.m_AllocatorLabel
				}
			}.Schedule(inputDeps);
			this.m_HashMapData.m_Buffer = null;
			return jobHandle;
		}

		public NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_HashMapData.GetKeyArray(allocator);
		}

		public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_HashMapData.GetValueArray(allocator);
		}

		public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_HashMapData.GetKeyValueArrays(allocator);
		}

		public NativeHashMap<TKey, TValue>.ParallelWriter AsParallelWriter()
		{
			NativeHashMap<TKey, TValue>.ParallelWriter parallelWriter;
			parallelWriter.m_Writer = this.m_HashMapData.AsParallelWriter();
			return parallelWriter;
		}

		public NativeHashMap<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new NativeHashMap<TKey, TValue>.Enumerator
			{
				m_Enumerator = new UnsafeHashMapDataEnumerator(this.m_HashMapData.m_Buffer)
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

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowKeyNotPresent(TKey key)
		{
			throw new ArgumentException(string.Format("Key: {0} is not present in the NativeHashMap.", key));
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowKeyAlreadyAdded(TKey key)
		{
			throw new ArgumentException("An item with the same key has already been added", "key");
		}

		internal UnsafeHashMap<TKey, TValue> m_HashMapData;

		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
		[DebuggerDisplay("Capacity = {m_Writer.Capacity}")]
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public struct ParallelWriter
		{
			public int m_ThreadIndex
			{
				get
				{
					return this.m_Writer.m_ThreadIndex;
				}
			}

			public int Capacity
			{
				get
				{
					return this.m_Writer.Capacity;
				}
			}

			public bool TryAdd(TKey key, TValue item)
			{
				return this.m_Writer.TryAdd(key, item);
			}

			internal UnsafeHashMap<TKey, TValue>.ParallelWriter m_Writer;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
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
