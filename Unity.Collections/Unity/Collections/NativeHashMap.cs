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
	[DebuggerTypeProxy(typeof(NativeHashMapDebuggerTypeProxy<, >))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct NativeHashMap<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue> : INativeDisposable, IDisposable, IEnumerable<KVPair<TKey, TValue>>, IEnumerable where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
	{
		public NativeHashMap(int initialCapacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_Data = HashMapHelper<TKey>.Alloc(initialCapacity, sizeof(TValue), 256, allocator);
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			HashMapHelper<TKey>.Free(this.m_Data);
			this.m_Data = null;
		}

		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new NativeHashMapDisposeJob
			{
				Data = new NativeHashMapDispose
				{
					m_HashMapData = (UnsafeHashMap<int, int>*)this.m_Data
				}
			}.Schedule(inputDeps);
			this.m_Data = null;
			return jobHandle;
		}

		public unsafe readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data != null && this.m_Data->IsCreated;
			}
		}

		public unsafe readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return !this.IsCreated || this.m_Data->IsEmpty;
			}
		}

		public unsafe readonly int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data->Count;
			}
		}

		public unsafe int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Data->Capacity;
			}
			set
			{
				this.m_Data->Resize(value);
			}
		}

		public unsafe void Clear()
		{
			this.m_Data->Clear();
		}

		public unsafe bool TryAdd(TKey key, TValue item)
		{
			int num = this.m_Data->TryAdd(in key);
			if (-1 != num)
			{
				UnsafeUtility.WriteArrayElement<TValue>((void*)this.m_Data->Ptr, num, item);
				return true;
			}
			return false;
		}

		public void Add(TKey key, TValue item)
		{
			this.TryAdd(key, item);
		}

		public unsafe bool Remove(TKey key)
		{
			return -1 != this.m_Data->TryRemove(key);
		}

		public unsafe bool TryGetValue(TKey key, out TValue item)
		{
			return this.m_Data->TryGetValue<TValue>(key, out item);
		}

		public unsafe bool ContainsKey(TKey key)
		{
			return -1 != this.m_Data->Find(key);
		}

		public unsafe void TrimExcess()
		{
			this.m_Data->TrimExcess();
		}

		public unsafe TValue this[TKey key]
		{
			get
			{
				TValue tvalue;
				this.m_Data->TryGetValue<TValue>(key, out tvalue);
				return tvalue;
			}
			set
			{
				int num = this.m_Data->Find(key);
				if (-1 == num)
				{
					this.TryAdd(key, value);
					return;
				}
				UnsafeUtility.WriteArrayElement<TValue>((void*)this.m_Data->Ptr, num, value);
			}
		}

		public unsafe NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Data->GetKeyArray(allocator);
		}

		public unsafe NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Data->GetValueArray<TValue>(allocator);
		}

		public unsafe NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Data->GetKeyValueArrays<TValue>(allocator);
		}

		public NativeHashMap<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new NativeHashMap<TKey, TValue>.Enumerator
			{
				m_Enumerator = new HashMapHelper<TKey>.Enumerator(this.m_Data)
			};
		}

		IEnumerator<KVPair<TKey, TValue>> IEnumerable<KVPair<TKey, TValue>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public NativeHashMap<TKey, TValue>.ReadOnly AsReadOnly()
		{
			return new NativeHashMap<TKey, TValue>.ReadOnly(ref this);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckWrite()
		{
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
		internal unsafe HashMapHelper<TKey>* m_Data;

		[NativeContainer]
		[NativeContainerIsReadOnly]
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

			[NativeDisableUnsafePtrRestriction]
			internal HashMapHelper<TKey>.Enumerator m_Enumerator;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public struct ReadOnly : IEnumerable<KVPair<TKey, TValue>>, IEnumerable
		{
			internal ReadOnly(ref NativeHashMap<TKey, TValue> data)
			{
				this.m_Data = data.m_Data;
			}

			public unsafe readonly bool IsCreated
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data != null && this.m_Data->IsCreated;
				}
			}

			public unsafe readonly bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return !this.IsCreated || this.m_Data->IsEmpty;
				}
			}

			public unsafe readonly int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data->Count;
				}
			}

			public unsafe readonly int Capacity
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data->Capacity;
				}
			}

			public unsafe readonly bool TryGetValue(TKey key, out TValue item)
			{
				return this.m_Data->TryGetValue<TValue>(key, out item);
			}

			public unsafe readonly bool ContainsKey(TKey key)
			{
				return -1 != this.m_Data->Find(key);
			}

			public unsafe readonly TValue this[TKey key]
			{
				get
				{
					TValue tvalue;
					this.m_Data->TryGetValue<TValue>(key, out tvalue);
					return tvalue;
				}
			}

			public unsafe readonly NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
			{
				return this.m_Data->GetKeyArray(allocator);
			}

			public unsafe readonly NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
			{
				return this.m_Data->GetValueArray<TValue>(allocator);
			}

			public unsafe readonly NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
			{
				return this.m_Data->GetKeyValueArrays<TValue>(allocator);
			}

			public readonly NativeHashMap<TKey, TValue>.Enumerator GetEnumerator()
			{
				return new NativeHashMap<TKey, TValue>.Enumerator
				{
					m_Enumerator = new HashMapHelper<TKey>.Enumerator(this.m_Data)
				};
			}

			IEnumerator<KVPair<TKey, TValue>> IEnumerable<KVPair<TKey, TValue>>.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private readonly void CheckRead()
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
			private readonly void ThrowKeyNotPresent(TKey key)
			{
				throw new ArgumentException(string.Format("Key: {0} is not present.", key));
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe HashMapHelper<TKey>* m_Data;
		}
	}
}
