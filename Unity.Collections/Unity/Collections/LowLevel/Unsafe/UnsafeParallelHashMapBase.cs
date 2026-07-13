using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	internal struct UnsafeParallelHashMapBase<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue> where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
	{
		internal unsafe static void Clear(UnsafeParallelHashMapData* data)
		{
			UnsafeUtility.MemSet((void*)data->buckets, byte.MaxValue, (long)((data->bucketCapacityMask + 1) * 4));
			UnsafeUtility.MemSet((void*)data->next, byte.MaxValue, (long)(data->keyCapacity * 4));
			int threadIndexCount = JobsUtility.ThreadIndexCount;
			for (int i = 0; i < threadIndexCount; i++)
			{
				data->firstFreeTLS[i * 16] = -1;
			}
			data->allocatedIndexLength = 0;
		}

		internal unsafe static int AllocEntry(UnsafeParallelHashMapData* data, int threadIndex)
		{
			int* next = (int*)data->next;
			int num;
			int num2;
			for (;;)
			{
				num = Volatile.Read(ref data->firstFreeTLS[threadIndex * 16]);
				if (num != -3)
				{
					if (num < 0)
					{
						Interlocked.Exchange(ref data->firstFreeTLS[threadIndex * 16], -2);
						if (data->allocatedIndexLength < data->keyCapacity)
						{
							num = Interlocked.Add(ref data->allocatedIndexLength, 16) - 16;
							if (num < data->keyCapacity - 1)
							{
								break;
							}
							if (num == data->keyCapacity - 1)
							{
								goto Block_6;
							}
						}
						Interlocked.Exchange(ref data->firstFreeTLS[threadIndex * 16], -1);
						int threadIndexCount = JobsUtility.ThreadIndexCount;
						bool flag = true;
						while (flag)
						{
							flag = false;
							for (num2 = (threadIndex + 1) % threadIndexCount; num2 != threadIndex; num2 = (num2 + 1) % threadIndexCount)
							{
								do
								{
									num = Volatile.Read(ref data->firstFreeTLS[num2 * 16]);
								}
								while (num == -3 || (num >= 0 && Interlocked.CompareExchange(ref data->firstFreeTLS[num2 * 16], -3, num) != num));
								if (num == -2)
								{
									flag = true;
								}
								else if (num >= 0)
								{
									goto Block_10;
								}
							}
						}
					}
					if (Interlocked.CompareExchange(ref data->firstFreeTLS[threadIndex * 16], -3, num) == num)
					{
						goto Block_11;
					}
				}
			}
			int num3 = math.min(16, data->keyCapacity - num);
			for (int i = 1; i < num3; i++)
			{
				next[num + i] = num + i + 1;
			}
			next[num + num3 - 1] = -1;
			next[num] = -1;
			Interlocked.Exchange(ref data->firstFreeTLS[threadIndex * 16], num + 1);
			return num;
			Block_6:
			Interlocked.Exchange(ref data->firstFreeTLS[threadIndex * 16], -1);
			return num;
			Block_10:
			Interlocked.Exchange(ref data->firstFreeTLS[num2 * 16], next[num]);
			next[num] = -1;
			return num;
			Block_11:
			Interlocked.Exchange(ref data->firstFreeTLS[threadIndex * 16], next[num]);
			next[num] = -1;
			return num;
		}

		internal unsafe static void FreeEntry(UnsafeParallelHashMapData* data, int idx, int threadIndex)
		{
			int* next = (int*)data->next;
			for (;;)
			{
				int num = Volatile.Read(ref data->firstFreeTLS[threadIndex * 16]);
				if (num != -3)
				{
					next[idx] = num;
					if (Interlocked.CompareExchange(ref data->firstFreeTLS[threadIndex * 16], idx, num) == num)
					{
						break;
					}
				}
			}
		}

		internal unsafe static bool TryAddAtomic(UnsafeParallelHashMapData* data, TKey key, TValue item, int threadIndex)
		{
			TValue tvalue;
			NativeParallelMultiHashMapIterator<TKey> nativeParallelMultiHashMapIterator;
			if (UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(data, key, out tvalue, out nativeParallelMultiHashMapIterator))
			{
				return false;
			}
			int num = UnsafeParallelHashMapBase<TKey, TValue>.AllocEntry(data, threadIndex);
			UnsafeUtility.WriteArrayElement<TKey>((void*)data->keys, num, key);
			UnsafeUtility.WriteArrayElement<TValue>((void*)data->values, num, item);
			int num2 = key.GetHashCode() & data->bucketCapacityMask;
			int* buckets = (int*)data->buckets;
			if (Interlocked.CompareExchange(ref buckets[num2], num, -1) != -1)
			{
				int* next = (int*)data->next;
				for (;;)
				{
					int num3 = buckets[num2];
					next[num] = num3;
					if (UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(data, key, out tvalue, out nativeParallelMultiHashMapIterator))
					{
						break;
					}
					if (Interlocked.CompareExchange(ref buckets[num2], num, num3) == num3)
					{
						return true;
					}
				}
				UnsafeParallelHashMapBase<TKey, TValue>.FreeEntry(data, num, threadIndex);
				return false;
			}
			return true;
		}

		internal unsafe static void AddAtomicMulti(UnsafeParallelHashMapData* data, TKey key, TValue item, int threadIndex)
		{
			int num = UnsafeParallelHashMapBase<TKey, TValue>.AllocEntry(data, threadIndex);
			UnsafeUtility.WriteArrayElement<TKey>((void*)data->keys, num, key);
			UnsafeUtility.WriteArrayElement<TValue>((void*)data->values, num, item);
			int num2 = key.GetHashCode() & data->bucketCapacityMask;
			int* buckets = (int*)data->buckets;
			int* next = (int*)data->next;
			int num3;
			do
			{
				num3 = buckets[num2];
				next[num] = num3;
			}
			while (Interlocked.CompareExchange(ref buckets[num2], num, num3) != num3);
		}

		internal unsafe static bool TryAdd(UnsafeParallelHashMapData* data, TKey key, TValue item, bool isMultiHashMap, AllocatorManager.AllocatorHandle allocation)
		{
			TValue tvalue;
			NativeParallelMultiHashMapIterator<TKey> nativeParallelMultiHashMapIterator;
			if (isMultiHashMap || !UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(data, key, out tvalue, out nativeParallelMultiHashMapIterator))
			{
				int num;
				int* ptr;
				if (data->allocatedIndexLength >= data->keyCapacity && *data->firstFreeTLS < 0)
				{
					int threadIndexCount = JobsUtility.ThreadIndexCount;
					for (int i = 1; i < threadIndexCount; i++)
					{
						if (data->firstFreeTLS[i * 16] >= 0)
						{
							num = data->firstFreeTLS[i * 16];
							ptr = (int*)data->next;
							data->firstFreeTLS[i * 16] = ptr[num];
							ptr[num] = -1;
							*data->firstFreeTLS = num;
							break;
						}
					}
					if (*data->firstFreeTLS < 0)
					{
						int num2 = UnsafeParallelHashMapData.GrowCapacity(data->keyCapacity);
						UnsafeParallelHashMapData.ReallocateHashMap<TKey, TValue>(data, num2, UnsafeParallelHashMapData.GetBucketSize(num2), allocation);
					}
				}
				num = *data->firstFreeTLS;
				if (num >= 0)
				{
					*data->firstFreeTLS = *(int*)(data->next + (IntPtr)num * 4);
				}
				else
				{
					int allocatedIndexLength = data->allocatedIndexLength;
					data->allocatedIndexLength = allocatedIndexLength + 1;
					num = allocatedIndexLength;
				}
				UnsafeUtility.WriteArrayElement<TKey>((void*)data->keys, num, key);
				UnsafeUtility.WriteArrayElement<TValue>((void*)data->values, num, item);
				int num3 = key.GetHashCode() & data->bucketCapacityMask;
				int* buckets = (int*)data->buckets;
				ptr = (int*)data->next;
				ptr[num] = buckets[num3];
				buckets[num3] = num;
				return true;
			}
			return false;
		}

		internal unsafe static int Remove(UnsafeParallelHashMapData* data, TKey key, bool isMultiHashMap)
		{
			if (data->keyCapacity == 0)
			{
				return 0;
			}
			int num = 0;
			int* buckets = (int*)data->buckets;
			int* next = (int*)data->next;
			int num2 = key.GetHashCode() & data->bucketCapacityMask;
			int num3 = -1;
			int num4 = buckets[num2];
			while (num4 >= 0 && num4 < data->keyCapacity)
			{
				TKey tkey = UnsafeUtility.ReadArrayElement<TKey>((void*)data->keys, num4);
				if (tkey.Equals(key))
				{
					num++;
					if (num3 < 0)
					{
						buckets[num2] = next[num4];
					}
					else
					{
						next[num3] = next[num4];
					}
					int num5 = next[num4];
					next[num4] = *data->firstFreeTLS;
					*data->firstFreeTLS = num4;
					num4 = num5;
					if (!isMultiHashMap)
					{
						break;
					}
				}
				else
				{
					num3 = num4;
					num4 = next[num4];
				}
			}
			return num;
		}

		internal unsafe static void Remove(UnsafeParallelHashMapData* data, NativeParallelMultiHashMapIterator<TKey> it)
		{
			int* buckets = (int*)data->buckets;
			int* next = (int*)data->next;
			int num = it.key.GetHashCode() & data->bucketCapacityMask;
			int num2 = buckets[num];
			if (num2 == it.EntryIndex)
			{
				buckets[num] = next[num2];
			}
			else
			{
				while (num2 >= 0 && next[num2] != it.EntryIndex)
				{
					num2 = next[num2];
				}
				next[num2] = next[it.EntryIndex];
			}
			next[it.EntryIndex] = *data->firstFreeTLS;
			*data->firstFreeTLS = it.EntryIndex;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		internal unsafe static void RemoveKeyValue<[global::System.Runtime.CompilerServices.IsUnmanaged] TValueEQ>(UnsafeParallelHashMapData* data, TKey key, TValueEQ value) where TValueEQ : struct, ValueType, IEquatable<TValueEQ>
		{
			if (data->keyCapacity == 0)
			{
				return;
			}
			int* buckets = (int*)data->buckets;
			uint keyCapacity = (uint)data->keyCapacity;
			int* ptr = buckets + (key.GetHashCode() & data->bucketCapacityMask);
			int num = *ptr;
			if (num >= (int)keyCapacity)
			{
				return;
			}
			int* next = (int*)data->next;
			byte* keys = data->keys;
			byte* values = data->values;
			int* firstFreeTLS = data->firstFreeTLS;
			for (;;)
			{
				TKey tkey = UnsafeUtility.ReadArrayElement<TKey>((void*)keys, num);
				if (!tkey.Equals(key))
				{
					goto IL_00AE;
				}
				TValueEQ tvalueEQ = UnsafeUtility.ReadArrayElement<TValueEQ>((void*)values, num);
				if (!tvalueEQ.Equals(value))
				{
					goto IL_00AE;
				}
				int num2 = next[num];
				next[num] = *firstFreeTLS;
				*firstFreeTLS = num;
				num = (*ptr = num2);
				IL_00B9:
				if (num >= (int)keyCapacity)
				{
					break;
				}
				continue;
				IL_00AE:
				ptr = next + num;
				num = *ptr;
				goto IL_00B9;
			}
		}

		internal unsafe static bool TryGetFirstValueAtomic(UnsafeParallelHashMapData* data, TKey key, out TValue item, out NativeParallelMultiHashMapIterator<TKey> it)
		{
			it.key = key;
			if (data->allocatedIndexLength <= 0)
			{
				it.EntryIndex = (it.NextEntryIndex = -1);
				item = default(TValue);
				return false;
			}
			int* buckets = (int*)data->buckets;
			int num = key.GetHashCode() & data->bucketCapacityMask;
			it.EntryIndex = (it.NextEntryIndex = buckets[num]);
			return UnsafeParallelHashMapBase<TKey, TValue>.TryGetNextValueAtomic(data, out item, ref it);
		}

		internal unsafe static bool TryGetNextValueAtomic(UnsafeParallelHashMapData* data, out TValue item, ref NativeParallelMultiHashMapIterator<TKey> it)
		{
			int num = it.NextEntryIndex;
			it.NextEntryIndex = -1;
			it.EntryIndex = -1;
			item = default(TValue);
			if (num < 0 || num >= data->keyCapacity)
			{
				return false;
			}
			int* next = (int*)data->next;
			do
			{
				TKey tkey = UnsafeUtility.ReadArrayElement<TKey>((void*)data->keys, num);
				if (tkey.Equals(it.key))
				{
					goto Block_3;
				}
				num = next[num];
			}
			while (num >= 0 && num < data->keyCapacity);
			return false;
			Block_3:
			it.NextEntryIndex = next[num];
			it.EntryIndex = num;
			item = UnsafeUtility.ReadArrayElement<TValue>((void*)data->values, num);
			return true;
		}

		internal unsafe static bool SetValue(UnsafeParallelHashMapData* data, ref NativeParallelMultiHashMapIterator<TKey> it, ref TValue item)
		{
			int entryIndex = it.EntryIndex;
			if (entryIndex < 0 || entryIndex >= data->keyCapacity)
			{
				return false;
			}
			UnsafeUtility.WriteArrayElement<TValue>((void*)data->values, entryIndex, item);
			return true;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckOutOfCapacity(int idx, int keyCapacity)
		{
			if (idx >= keyCapacity)
			{
				throw new InvalidOperationException(string.Format("nextPtr idx {0} beyond capacity {1}", idx, keyCapacity));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private unsafe static void CheckIndexOutOfBounds(UnsafeParallelHashMapData* data, int idx)
		{
			if (idx < 0 || idx >= data->keyCapacity)
			{
				throw new InvalidOperationException("Internal HashMap error");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void ThrowFull()
		{
			throw new InvalidOperationException("HashMap is full");
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void ThrowInvalidIterator()
		{
			throw new InvalidOperationException("Invalid iterator passed to HashMap remove");
		}

		private const int SentinelRefilling = -2;

		private const int SentinelSwapInProgress = -3;
	}
}
