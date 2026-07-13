using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	internal struct HashMapHelper<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey> where TKey : struct, ValueType, IEquatable<TKey>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int CalcCapacityCeilPow2(int capacity)
		{
			capacity = math.max(math.max(1, this.Count), capacity);
			return math.ceilpow2(math.max(capacity, 1 << this.Log2MinGrowth));
		}

		internal static int GetBucketSize(int capacity)
		{
			return capacity * 2;
		}

		internal readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Ptr != null;
			}
		}

		internal readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return !this.IsCreated || this.Count == 0;
			}
		}

		internal unsafe void Clear()
		{
			UnsafeUtility.MemSet((void*)this.Buckets, byte.MaxValue, (long)(this.BucketCapacity * 4));
			UnsafeUtility.MemSet((void*)this.Next, byte.MaxValue, (long)(this.Capacity * 4));
			this.Count = 0;
			this.FirstFreeIdx = -1;
			this.AllocatedIndex = 0;
		}

		internal unsafe void Init(int capacity, int sizeOfValueT, int minGrowth, AllocatorManager.AllocatorHandle allocator)
		{
			this.Count = 0;
			this.Log2MinGrowth = (int)((byte)(32 - math.lzcnt(math.max(1, minGrowth) - 1)));
			capacity = this.CalcCapacityCeilPow2(capacity);
			this.Capacity = capacity;
			this.BucketCapacity = HashMapHelper<TKey>.GetBucketSize(capacity);
			this.Allocator = allocator;
			this.SizeOfTValue = sizeOfValueT;
			int num2;
			int num3;
			int num4;
			int num = HashMapHelper<TKey>.CalculateDataSize(capacity, this.BucketCapacity, sizeOfValueT, out num2, out num3, out num4);
			this.Ptr = (byte*)Memory.Unmanaged.Allocate((long)num, 64, allocator);
			this.Keys = (TKey*)(this.Ptr + num2);
			this.Next = (int*)(this.Ptr + num3);
			this.Buckets = (int*)(this.Ptr + num4);
			this.Clear();
		}

		internal void Dispose()
		{
			Memory.Unmanaged.Free<byte>(this.Ptr, this.Allocator);
			this.Ptr = null;
			this.Keys = null;
			this.Next = null;
			this.Buckets = null;
			this.Count = 0;
			this.BucketCapacity = 0;
		}

		internal unsafe static HashMapHelper<TKey>* Alloc(int capacity, int sizeOfValueT, int minGrowth, AllocatorManager.AllocatorHandle allocator)
		{
			HashMapHelper<TKey>* ptr = (HashMapHelper<TKey>*)Memory.Unmanaged.Allocate((long)sizeof(HashMapHelper<TKey>), UnsafeUtility.AlignOf<HashMapHelper<TKey>>(), allocator);
			ptr->Init(capacity, sizeOfValueT, minGrowth, allocator);
			return ptr;
		}

		internal unsafe static void Free(HashMapHelper<TKey>* data)
		{
			if (data == null)
			{
				throw new InvalidOperationException("Hash based container has yet to be created or has been destroyed!");
			}
			data->Dispose();
			Memory.Unmanaged.Free<HashMapHelper<TKey>>(data, data->Allocator);
		}

		internal void Resize(int newCapacity)
		{
			newCapacity = math.max(newCapacity, this.Count);
			int num = math.ceilpow2(HashMapHelper<TKey>.GetBucketSize(newCapacity));
			if (this.Capacity == newCapacity && this.BucketCapacity == num)
			{
				return;
			}
			this.ResizeExact(newCapacity, num);
		}

		internal unsafe void ResizeExact(int newCapacity, int newBucketCapacity)
		{
			int num2;
			int num3;
			int num4;
			int num = HashMapHelper<TKey>.CalculateDataSize(newCapacity, newBucketCapacity, this.SizeOfTValue, out num2, out num3, out num4);
			byte* ptr = this.Ptr;
			TKey* keys = this.Keys;
			int* next = this.Next;
			int* buckets = this.Buckets;
			int bucketCapacity = this.BucketCapacity;
			this.Ptr = (byte*)Memory.Unmanaged.Allocate((long)num, 64, this.Allocator);
			this.Keys = (TKey*)(this.Ptr + num2);
			this.Next = (int*)(this.Ptr + num3);
			this.Buckets = (int*)(this.Ptr + num4);
			this.Capacity = newCapacity;
			this.BucketCapacity = newBucketCapacity;
			this.Clear();
			int i = 0;
			int num5 = bucketCapacity;
			while (i < num5)
			{
				for (int num6 = buckets[i]; num6 != -1; num6 = next[num6])
				{
					int num7 = this.TryAdd(in keys[(IntPtr)num6 * (IntPtr)sizeof(TKey) / (IntPtr)sizeof(TKey)]);
					UnsafeUtility.MemCpy((void*)(this.Ptr + this.SizeOfTValue * num7), (void*)(ptr + this.SizeOfTValue * num6), (long)this.SizeOfTValue);
				}
				i++;
			}
			Memory.Unmanaged.Free<byte>(ptr, this.Allocator);
		}

		internal void TrimExcess()
		{
			int num = this.CalcCapacityCeilPow2(this.Count);
			this.ResizeExact(num, HashMapHelper<TKey>.GetBucketSize(num));
		}

		internal static int CalculateDataSize(int capacity, int bucketCapacity, int sizeOfTValue, out int outKeyOffset, out int outNextOffset, out int outBucketOffset)
		{
			int num = sizeof(TKey);
			int num2 = 4;
			int num3 = sizeOfTValue * capacity;
			int num4 = num * capacity;
			int num5 = num2 * capacity;
			int num6 = num2 * bucketCapacity;
			int num7 = num3 + num4 + num5 + num6;
			outKeyOffset = num3;
			outNextOffset = outKeyOffset + num4;
			outBucketOffset = outNextOffset + num5;
			return num7;
		}

		internal unsafe readonly int GetCount()
		{
			if (this.AllocatedIndex <= 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = this.FirstFreeIdx; i >= 0; i = this.Next[i])
			{
				num++;
			}
			return math.min(this.Capacity, this.AllocatedIndex) - num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetBucket(in TKey key)
		{
			TKey tkey = key;
			return (int)((ulong)tkey.GetHashCode() & (ulong)((long)(this.BucketCapacity - 1)));
		}

		internal unsafe int TryAdd(in TKey key)
		{
			if (-1 == this.Find(key))
			{
				if (this.AllocatedIndex >= this.Capacity && this.FirstFreeIdx < 0)
				{
					int num = this.CalcCapacityCeilPow2(this.Capacity + (1 << this.Log2MinGrowth));
					this.Resize(num);
				}
				int num2 = this.FirstFreeIdx;
				if (num2 >= 0)
				{
					this.FirstFreeIdx = this.Next[num2];
				}
				else
				{
					int allocatedIndex = this.AllocatedIndex;
					this.AllocatedIndex = allocatedIndex + 1;
					num2 = allocatedIndex;
				}
				UnsafeUtility.WriteArrayElement<TKey>((void*)this.Keys, num2, key);
				int bucket = this.GetBucket(in key);
				this.Next[num2] = this.Buckets[bucket];
				this.Buckets[bucket] = num2;
				this.Count++;
				return num2;
			}
			return -1;
		}

		internal unsafe int Find(TKey key)
		{
			if (this.AllocatedIndex > 0)
			{
				int bucket = this.GetBucket(in key);
				int num = this.Buckets[bucket];
				if (num < this.Capacity)
				{
					int* next = this.Next;
					do
					{
						TKey tkey = UnsafeUtility.ReadArrayElement<TKey>((void*)this.Keys, num);
						if (tkey.Equals(key))
						{
							return num;
						}
						num = next[num];
					}
					while (num < this.Capacity);
					return -1;
				}
			}
			return -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		internal unsafe bool TryGetValue<[global::System.Runtime.CompilerServices.IsUnmanaged] TValue>(TKey key, out TValue item) where TValue : struct, ValueType
		{
			int num = this.Find(key);
			if (-1 != num)
			{
				item = UnsafeUtility.ReadArrayElement<TValue>((void*)this.Ptr, num);
				return true;
			}
			item = default(TValue);
			return false;
		}

		internal unsafe int TryRemove(TKey key)
		{
			if (this.Capacity == 0)
			{
				return -1;
			}
			int num = 0;
			int bucket = this.GetBucket(in key);
			int num2 = -1;
			int num3 = this.Buckets[bucket];
			while (num3 >= 0 && num3 < this.Capacity)
			{
				TKey tkey = UnsafeUtility.ReadArrayElement<TKey>((void*)this.Keys, num3);
				if (tkey.Equals(key))
				{
					num++;
					if (num2 < 0)
					{
						this.Buckets[bucket] = this.Next[num3];
					}
					else
					{
						this.Next[num2] = this.Next[num3];
					}
					int num4 = this.Next[num3];
					this.Next[num3] = this.FirstFreeIdx;
					this.FirstFreeIdx = num3;
					break;
				}
				num2 = num3;
				num3 = this.Next[num3];
			}
			this.Count -= num;
			if (num == 0)
			{
				return -1;
			}
			return num;
		}

		internal unsafe bool MoveNextSearch(ref int bucketIndex, ref int nextIndex, out int index)
		{
			int i = bucketIndex;
			int bucketCapacity = this.BucketCapacity;
			while (i < bucketCapacity)
			{
				int num = this.Buckets[i];
				if (num != -1)
				{
					index = num;
					bucketIndex = i + 1;
					nextIndex = this.Next[num];
					return true;
				}
				i++;
			}
			index = -1;
			bucketIndex = this.BucketCapacity;
			nextIndex = -1;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe bool MoveNext(ref int bucketIndex, ref int nextIndex, out int index)
		{
			if (nextIndex != -1)
			{
				index = nextIndex;
				nextIndex = this.Next[nextIndex];
				return true;
			}
			return this.MoveNextSearch(ref bucketIndex, ref nextIndex, out index);
		}

		internal unsafe NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TKey> nativeArray = CollectionHelper.CreateNativeArray<TKey>(this.Count, allocator, NativeArrayOptions.UninitializedMemory);
			int num = 0;
			int num2 = 0;
			int length = nativeArray.Length;
			int bucketCapacity = this.BucketCapacity;
			while (num < bucketCapacity && num2 < length)
			{
				for (int num3 = this.Buckets[num]; num3 != -1; num3 = this.Next[num3])
				{
					nativeArray[num2++] = UnsafeUtility.ReadArrayElement<TKey>((void*)this.Keys, num3);
				}
				num++;
			}
			return nativeArray;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		internal unsafe NativeArray<TValue> GetValueArray<[global::System.Runtime.CompilerServices.IsUnmanaged] TValue>(AllocatorManager.AllocatorHandle allocator) where TValue : struct, ValueType
		{
			NativeArray<TValue> nativeArray = CollectionHelper.CreateNativeArray<TValue>(this.Count, allocator, NativeArrayOptions.UninitializedMemory);
			int num = 0;
			int num2 = 0;
			int length = nativeArray.Length;
			int bucketCapacity = this.BucketCapacity;
			while (num < bucketCapacity && num2 < length)
			{
				for (int num3 = this.Buckets[num]; num3 != -1; num3 = this.Next[num3])
				{
					nativeArray[num2++] = UnsafeUtility.ReadArrayElement<TValue>((void*)this.Ptr, num3);
				}
				num++;
			}
			return nativeArray;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		internal unsafe NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays<[global::System.Runtime.CompilerServices.IsUnmanaged] TValue>(AllocatorManager.AllocatorHandle allocator) where TValue : struct, ValueType
		{
			NativeKeyValueArrays<TKey, TValue> nativeKeyValueArrays = new NativeKeyValueArrays<TKey, TValue>(this.Count, allocator, NativeArrayOptions.UninitializedMemory);
			int num = 0;
			int num2 = 0;
			int length = nativeKeyValueArrays.Length;
			int bucketCapacity = this.BucketCapacity;
			while (num < bucketCapacity && num2 < length)
			{
				for (int num3 = this.Buckets[num]; num3 != -1; num3 = this.Next[num3])
				{
					nativeKeyValueArrays.Keys[num2] = UnsafeUtility.ReadArrayElement<TKey>((void*)this.Keys, num3);
					nativeKeyValueArrays.Values[num2] = UnsafeUtility.ReadArrayElement<TValue>((void*)this.Ptr, num3);
					num2++;
				}
				num++;
			}
			return nativeKeyValueArrays;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckIndexOutOfBounds(int idx)
		{
			if (idx >= this.Capacity)
			{
				throw new InvalidOperationException(string.Format("Internal HashMap error. idx {0}", idx));
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe byte* Ptr;

		[NativeDisableUnsafePtrRestriction]
		internal unsafe TKey* Keys;

		[NativeDisableUnsafePtrRestriction]
		internal unsafe int* Next;

		[NativeDisableUnsafePtrRestriction]
		internal unsafe int* Buckets;

		internal int Count;

		internal int Capacity;

		internal int Log2MinGrowth;

		internal int BucketCapacity;

		internal int AllocatedIndex;

		internal int FirstFreeIdx;

		internal int SizeOfTValue;

		internal AllocatorManager.AllocatorHandle Allocator;

		internal const int kMinimumCapacity = 256;

		internal struct Enumerator
		{
			internal unsafe Enumerator(HashMapHelper<TKey>* data)
			{
				this.m_Data = data;
				this.m_Index = -1;
				this.m_BucketIndex = 0;
				this.m_NextIndex = -1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe bool MoveNext()
			{
				return this.m_Data->MoveNext(ref this.m_BucketIndex, ref this.m_NextIndex, out this.m_Index);
			}

			internal void Reset()
			{
				this.m_Index = -1;
				this.m_BucketIndex = 0;
				this.m_NextIndex = -1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal KVPair<TKey, TValue> GetCurrent<[global::System.Runtime.CompilerServices.IsUnmanaged] TValue>() where TValue : struct, ValueType
			{
				return new KVPair<TKey, TValue>
				{
					m_Data = this.m_Data,
					m_Index = this.m_Index
				};
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal unsafe TKey GetCurrentKey()
			{
				if (this.m_Index != -1)
				{
					return this.m_Data->Keys[(IntPtr)this.m_Index * (IntPtr)sizeof(TKey) / (IntPtr)sizeof(TKey)];
				}
				return default(TKey);
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe HashMapHelper<TKey>* m_Data;

			internal int m_Index;

			internal int m_BucketIndex;

			internal int m_NextIndex;
		}
	}
}
