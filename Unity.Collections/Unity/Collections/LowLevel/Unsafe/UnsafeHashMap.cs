using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerTypeProxy(typeof(UnsafeHashMapDebuggerTypeProxy<, >))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct UnsafeHashMap<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue> : INativeDisposable, IDisposable, IEnumerable<KVPair<TKey, TValue>>, IEnumerable where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
	{
		public UnsafeHashMap(int initialCapacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_Data = default(HashMapHelper<TKey>);
			this.m_Data.Init(initialCapacity, sizeof(TValue), 256, allocator);
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			this.m_Data.Dispose();
		}

		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new UnsafeDisposeJob
			{
				Ptr = (void*)this.m_Data.Ptr,
				Allocator = this.m_Data.Allocator
			}.Schedule(inputDeps);
			this.m_Data = default(HashMapHelper<TKey>);
			return jobHandle;
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data.IsCreated;
			}
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data.IsEmpty;
			}
		}

		public readonly int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data.Count;
			}
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Data.Capacity;
			}
			set
			{
				this.m_Data.Resize(value);
			}
		}

		public void Clear()
		{
			this.m_Data.Clear();
		}

		public unsafe bool TryAdd(TKey key, TValue item)
		{
			int num = this.m_Data.TryAdd(in key);
			if (-1 != num)
			{
				UnsafeUtility.WriteArrayElement<TValue>((void*)this.m_Data.Ptr, num, item);
				return true;
			}
			return false;
		}

		public void Add(TKey key, TValue item)
		{
			this.TryAdd(key, item);
		}

		public bool Remove(TKey key)
		{
			return -1 != this.m_Data.TryRemove(key);
		}

		public bool TryGetValue(TKey key, out TValue item)
		{
			return this.m_Data.TryGetValue<TValue>(key, out item);
		}

		public bool ContainsKey(TKey key)
		{
			return -1 != this.m_Data.Find(key);
		}

		public void TrimExcess()
		{
			this.m_Data.TrimExcess();
		}

		public unsafe TValue this[TKey key]
		{
			get
			{
				TValue tvalue;
				this.m_Data.TryGetValue<TValue>(key, out tvalue);
				return tvalue;
			}
			set
			{
				int num = this.m_Data.Find(key);
				if (-1 != num)
				{
					UnsafeUtility.WriteArrayElement<TValue>((void*)this.m_Data.Ptr, num, value);
					return;
				}
				this.TryAdd(key, value);
			}
		}

		public NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Data.GetKeyArray(allocator);
		}

		public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Data.GetValueArray<TValue>(allocator);
		}

		public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Data.GetKeyValueArrays<TValue>(allocator);
		}

		public unsafe UnsafeHashMap<TKey, TValue>.Enumerator GetEnumerator()
		{
			fixed (HashMapHelper<TKey>* ptr = &this.m_Data)
			{
				HashMapHelper<TKey>* ptr2 = ptr;
				return new UnsafeHashMap<TKey, TValue>.Enumerator
				{
					m_Enumerator = new HashMapHelper<TKey>.Enumerator(ptr2)
				};
			}
		}

		IEnumerator<KVPair<TKey, TValue>> IEnumerable<KVPair<TKey, TValue>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public UnsafeHashMap<TKey, TValue>.ReadOnly AsReadOnly()
		{
			return new UnsafeHashMap<TKey, TValue>.ReadOnly(ref this.m_Data);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void ThrowKeyNotPresent(TKey key)
		{
			throw new ArgumentException(string.Format("Key: {0} is not present.", key));
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void ThrowKeyAlreadyAdded(TKey key)
		{
			throw new ArgumentException(string.Format("An item with the same key has already been added: {0}", key));
		}

		[NativeDisableUnsafePtrRestriction]
		internal HashMapHelper<TKey> m_Data;

		public struct Enumerator : IEnumerator<KVPair<TKey, TValue>>, IEnumerator, IDisposable
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

			public KVPair<TKey, TValue> Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Enumerator.GetCurrent<TValue>();
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal HashMapHelper<TKey>.Enumerator m_Enumerator;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public struct ReadOnly : IEnumerable<KVPair<TKey, TValue>>, IEnumerable
		{
			internal ReadOnly(ref HashMapHelper<TKey> data)
			{
				this.m_Data = data;
			}

			public readonly bool IsCreated
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.IsCreated;
				}
			}

			public readonly bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.IsEmpty;
				}
			}

			public readonly int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.Count;
				}
			}

			public readonly int Capacity
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.Capacity;
				}
			}

			public readonly bool TryGetValue(TKey key, out TValue item)
			{
				HashMapHelper<TKey> data = this.m_Data;
				return data.TryGetValue<TValue>(key, out item);
			}

			public readonly bool ContainsKey(TKey key)
			{
				int num = -1;
				HashMapHelper<TKey> data = this.m_Data;
				return num != data.Find(key);
			}

			public readonly TValue this[TKey key]
			{
				get
				{
					HashMapHelper<TKey> data = this.m_Data;
					TValue tvalue;
					data.TryGetValue<TValue>(key, out tvalue);
					return tvalue;
				}
			}

			public readonly NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
			{
				HashMapHelper<TKey> data = this.m_Data;
				return data.GetKeyArray(allocator);
			}

			public readonly NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
			{
				HashMapHelper<TKey> data = this.m_Data;
				return data.GetValueArray<TValue>(allocator);
			}

			public readonly NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
			{
				HashMapHelper<TKey> data = this.m_Data;
				return data.GetKeyValueArrays<TValue>(allocator);
			}

			public unsafe readonly UnsafeHashMap<TKey, TValue>.Enumerator GetEnumerator()
			{
				fixed (HashMapHelper<TKey>* ptr = &this.m_Data)
				{
					HashMapHelper<TKey>* ptr2 = ptr;
					return new UnsafeHashMap<TKey, TValue>.Enumerator
					{
						m_Enumerator = new HashMapHelper<TKey>.Enumerator(ptr2)
					};
				}
			}

			IEnumerator<KVPair<TKey, TValue>> IEnumerable<KVPair<TKey, TValue>>.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			[NativeDisableUnsafePtrRestriction]
			internal HashMapHelper<TKey> m_Data;
		}
	}
}
