using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Count = {Count()}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafeParallelHashMapDebuggerTypeProxy<, >))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct UnsafeParallelHashMap<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
	{
		public UnsafeParallelHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_AllocatorLabel = allocator;
			UnsafeParallelHashMapData.AllocateHashMap<TKey, TValue>(capacity, capacity * 2, allocator, out this.m_Buffer);
			this.Clear();
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Buffer != null;
			}
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return !this.IsCreated || UnsafeParallelHashMapData.IsEmpty(this.m_Buffer);
			}
		}

		public readonly int Count()
		{
			return UnsafeParallelHashMapData.GetCount(this.m_Buffer);
		}

		public unsafe int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Buffer->keyCapacity;
			}
			set
			{
				UnsafeParallelHashMapData.ReallocateHashMap<TKey, TValue>(this.m_Buffer, value, UnsafeParallelHashMapData.GetBucketSize(value), this.m_AllocatorLabel);
			}
		}

		public void Clear()
		{
			UnsafeParallelHashMapBase<TKey, TValue>.Clear(this.m_Buffer);
		}

		public bool TryAdd(TKey key, TValue item)
		{
			return UnsafeParallelHashMapBase<TKey, TValue>.TryAdd(this.m_Buffer, key, item, false, this.m_AllocatorLabel);
		}

		public void Add(TKey key, TValue item)
		{
			UnsafeParallelHashMapBase<TKey, TValue>.TryAdd(this.m_Buffer, key, item, false, this.m_AllocatorLabel);
		}

		public bool Remove(TKey key)
		{
			return UnsafeParallelHashMapBase<TKey, TValue>.Remove(this.m_Buffer, key, false) != 0;
		}

		public bool TryGetValue(TKey key, out TValue item)
		{
			NativeParallelMultiHashMapIterator<TKey> nativeParallelMultiHashMapIterator;
			return UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(this.m_Buffer, key, out item, out nativeParallelMultiHashMapIterator);
		}

		public bool ContainsKey(TKey key)
		{
			TValue tvalue;
			NativeParallelMultiHashMapIterator<TKey> nativeParallelMultiHashMapIterator;
			return UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(this.m_Buffer, key, out tvalue, out nativeParallelMultiHashMapIterator);
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
				NativeParallelMultiHashMapIterator<TKey> nativeParallelMultiHashMapIterator;
				if (UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(this.m_Buffer, key, out tvalue, out nativeParallelMultiHashMapIterator))
				{
					UnsafeParallelHashMapBase<TKey, TValue>.SetValue(this.m_Buffer, ref nativeParallelMultiHashMapIterator, ref value);
					return;
				}
				UnsafeParallelHashMapBase<TKey, TValue>.TryAdd(this.m_Buffer, key, value, false, this.m_AllocatorLabel);
			}
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			UnsafeParallelHashMapData.DeallocateHashMap(this.m_Buffer, this.m_AllocatorLabel);
			this.m_Buffer = null;
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new UnsafeParallelHashMapDisposeJob
			{
				Data = this.m_Buffer,
				Allocator = this.m_AllocatorLabel
			}.Schedule(inputDeps);
			this.m_Buffer = null;
			return jobHandle;
		}

		public NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TKey> nativeArray = CollectionHelper.CreateNativeArray<TKey>(UnsafeParallelHashMapData.GetCount(this.m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetKeyArray<TKey>(this.m_Buffer, nativeArray);
			return nativeArray;
		}

		public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TValue> nativeArray = CollectionHelper.CreateNativeArray<TValue>(UnsafeParallelHashMapData.GetCount(this.m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetValueArray<TValue>(this.m_Buffer, nativeArray);
			return nativeArray;
		}

		public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			NativeKeyValueArrays<TKey, TValue> nativeKeyValueArrays = new NativeKeyValueArrays<TKey, TValue>(UnsafeParallelHashMapData.GetCount(this.m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetKeyValueArrays<TKey, TValue>(this.m_Buffer, nativeKeyValueArrays);
			return nativeKeyValueArrays;
		}

		public UnsafeParallelHashMap<TKey, TValue>.ParallelWriter AsParallelWriter()
		{
			UnsafeParallelHashMap<TKey, TValue>.ParallelWriter parallelWriter;
			parallelWriter.m_ThreadIndex = 0;
			parallelWriter.m_Buffer = this.m_Buffer;
			return parallelWriter;
		}

		public UnsafeParallelHashMap<TKey, TValue>.ReadOnly AsReadOnly()
		{
			return new UnsafeParallelHashMap<TKey, TValue>.ReadOnly(this);
		}

		public UnsafeParallelHashMap<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new UnsafeParallelHashMap<TKey, TValue>.Enumerator
			{
				m_Enumerator = new UnsafeParallelHashMapDataEnumerator(this.m_Buffer)
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
		internal unsafe UnsafeParallelHashMapData* m_Buffer;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

		[DebuggerDisplay("Count = {m_HashMapData.Count()}, Capacity = {m_HashMapData.Capacity}, IsCreated = {m_HashMapData.IsCreated}, IsEmpty = {IsEmpty}")]
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public struct ReadOnly : IEnumerable<KeyValue<TKey, TValue>>, IEnumerable
		{
			internal ReadOnly(UnsafeParallelHashMap<TKey, TValue> hashMapData)
			{
				this.m_HashMapData = hashMapData;
			}

			public readonly bool IsCreated
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_HashMapData.IsCreated;
				}
			}

			public readonly bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return !this.IsCreated || this.m_HashMapData.IsEmpty;
				}
			}

			public readonly int Count()
			{
				return this.m_HashMapData.Count();
			}

			public readonly int Capacity
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_HashMapData.Capacity;
				}
			}

			public readonly bool TryGetValue(TKey key, out TValue item)
			{
				UnsafeParallelHashMap<TKey, TValue> hashMapData = this.m_HashMapData;
				return hashMapData.TryGetValue(key, out item);
			}

			public readonly bool ContainsKey(TKey key)
			{
				UnsafeParallelHashMap<TKey, TValue> hashMapData = this.m_HashMapData;
				return hashMapData.ContainsKey(key);
			}

			public readonly TValue this[TKey key]
			{
				get
				{
					UnsafeParallelHashMap<TKey, TValue> hashMapData = this.m_HashMapData;
					TValue tvalue;
					if (hashMapData.TryGetValue(key, out tvalue))
					{
						return tvalue;
					}
					return default(TValue);
				}
			}

			public readonly NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
			{
				UnsafeParallelHashMap<TKey, TValue> hashMapData = this.m_HashMapData;
				return hashMapData.GetKeyArray(allocator);
			}

			public readonly NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
			{
				UnsafeParallelHashMap<TKey, TValue> hashMapData = this.m_HashMapData;
				return hashMapData.GetValueArray(allocator);
			}

			public readonly NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
			{
				UnsafeParallelHashMap<TKey, TValue> hashMapData = this.m_HashMapData;
				return hashMapData.GetKeyValueArrays(allocator);
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private readonly void ThrowKeyNotPresent(TKey key)
			{
				throw new ArgumentException(string.Format("Key: {0} is not present in the NativeParallelHashMap.", key));
			}

			public readonly UnsafeParallelHashMap<TKey, TValue>.Enumerator GetEnumerator()
			{
				return new UnsafeParallelHashMap<TKey, TValue>.Enumerator
				{
					m_Enumerator = new UnsafeParallelHashMapDataEnumerator(this.m_HashMapData.m_Buffer)
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

			internal UnsafeParallelHashMap<TKey, TValue> m_HashMapData;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public struct ParallelWriter
		{
			public int ThreadIndex
			{
				get
				{
					return this.m_ThreadIndex;
				}
			}

			public unsafe readonly int Capacity
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Buffer->keyCapacity;
				}
			}

			public bool TryAdd(TKey key, TValue item)
			{
				return UnsafeParallelHashMapBase<TKey, TValue>.TryAddAtomic(this.m_Buffer, key, item, this.m_ThreadIndex);
			}

			public bool TryAdd(TKey key, TValue item, int threadIndexOverride)
			{
				return UnsafeParallelHashMapBase<TKey, TValue>.TryAddAtomic(this.m_Buffer, key, item, threadIndexOverride);
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeParallelHashMapData* m_Buffer;

			[NativeSetThreadIndex]
			internal int m_ThreadIndex;
		}

		public struct Enumerator : IEnumerator<KeyValue<TKey, TValue>>, IEnumerator, IDisposable
		{
			public void Dispose()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

			internal UnsafeParallelHashMapDataEnumerator m_Enumerator;
		}
	}
}
