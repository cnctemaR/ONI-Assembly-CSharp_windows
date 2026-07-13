using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerTypeProxy(typeof(NativeMultiHashMapDebuggerTypeProxy<, >))]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct NativeMultiHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		public NativeMultiHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeMultiHashMap<TKey, TValue>(capacity, allocator, 2);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal void Initialize<[global::System.Runtime.CompilerServices.IsUnmanaged] U>(int capacity, ref U allocator, int disposeSentinelStackDepth) where U : struct, ValueType, AllocatorManager.IAllocator
		{
			this.m_MultiHashMapData = new UnsafeMultiHashMap<TKey, TValue>(capacity, allocator.Handle);
		}

		private NativeMultiHashMap(int capacity, AllocatorManager.AllocatorHandle allocator, int disposeSentinelStackDepth)
		{
			this = default(NativeMultiHashMap<TKey, TValue>);
			this.Initialize<AllocatorManager.AllocatorHandle>(capacity, ref allocator, disposeSentinelStackDepth);
		}

		public bool IsEmpty
		{
			get
			{
				return this.m_MultiHashMapData.IsEmpty;
			}
		}

		public int Count()
		{
			return this.m_MultiHashMapData.Count();
		}

		public int Capacity
		{
			get
			{
				return this.m_MultiHashMapData.Capacity;
			}
			set
			{
				this.m_MultiHashMapData.Capacity = value;
			}
		}

		public void Clear()
		{
			this.m_MultiHashMapData.Clear();
		}

		public void Add(TKey key, TValue item)
		{
			this.m_MultiHashMapData.Add(key, item);
		}

		public int Remove(TKey key)
		{
			return this.m_MultiHashMapData.Remove(key);
		}

		public void Remove(NativeMultiHashMapIterator<TKey> it)
		{
			this.m_MultiHashMapData.Remove(it);
		}

		public bool TryGetFirstValue(TKey key, out TValue item, out NativeMultiHashMapIterator<TKey> it)
		{
			return this.m_MultiHashMapData.TryGetFirstValue(key, out item, out it);
		}

		public bool TryGetNextValue(out TValue item, ref NativeMultiHashMapIterator<TKey> it)
		{
			return this.m_MultiHashMapData.TryGetNextValue(out item, ref it);
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
			return this.m_MultiHashMapData.SetValue(item, it);
		}

		public bool IsCreated
		{
			get
			{
				return this.m_MultiHashMapData.IsCreated;
			}
		}

		public void Dispose()
		{
			this.m_MultiHashMapData.Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			JobHandle jobHandle = new UnsafeHashMapDataDisposeJob
			{
				Data = new UnsafeHashMapDataDispose
				{
					m_Buffer = this.m_MultiHashMapData.m_Buffer,
					m_AllocatorLabel = this.m_MultiHashMapData.m_AllocatorLabel
				}
			}.Schedule(inputDeps);
			this.m_MultiHashMapData.m_Buffer = null;
			return jobHandle;
		}

		public NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_MultiHashMapData.GetKeyArray(allocator);
		}

		public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_MultiHashMapData.GetValueArray(allocator);
		}

		public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_MultiHashMapData.GetKeyValueArrays(allocator);
		}

		public NativeMultiHashMap<TKey, TValue>.ParallelWriter AsParallelWriter()
		{
			NativeMultiHashMap<TKey, TValue>.ParallelWriter parallelWriter;
			parallelWriter.m_Writer = this.m_MultiHashMapData.AsParallelWriter();
			return parallelWriter;
		}

		public NativeMultiHashMap<TKey, TValue>.Enumerator GetValuesForKey(TKey key)
		{
			return new NativeMultiHashMap<TKey, TValue>.Enumerator
			{
				hashmap = this,
				key = key,
				isFirst = true
			};
		}

		public NativeMultiHashMap<TKey, TValue>.KeyValueEnumerator GetEnumerator()
		{
			return new NativeMultiHashMap<TKey, TValue>.KeyValueEnumerator
			{
				m_Enumerator = new UnsafeHashMapDataEnumerator(this.m_MultiHashMapData.m_Buffer)
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

		internal UnsafeMultiHashMap<TKey, TValue> m_MultiHashMapData;

		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
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

			public void Add(TKey key, TValue item)
			{
				this.m_Writer.Add(key, item);
			}

			internal UnsafeMultiHashMap<TKey, TValue>.ParallelWriter m_Writer;
		}

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

			public NativeMultiHashMap<TKey, TValue>.Enumerator GetEnumerator()
			{
				return this;
			}

			internal NativeMultiHashMap<TKey, TValue> hashmap;

			internal TKey key;

			internal bool isFirst;

			private TValue value;

			private NativeMultiHashMapIterator<TKey> iterator;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
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
