using System;
using System.Diagnostics;
using System.Threading;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	internal struct UnsafeHashMapBase<TKey, TValue> where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		internal unsafe static void Clear(UnsafeHashMapData* data)
		{
			UnsafeUtility.MemSet((void*)data->buckets, byte.MaxValue, (long)((data->bucketCapacityMask + 1) * 4));
			UnsafeUtility.MemSet((void*)data->next, byte.MaxValue, (long)(data->keyCapacity * 4));
			for (int i = 0; i < 128; i++)
			{
				*((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(i * 16) * 4) = -1;
			}
			data->allocatedIndexLength = 0;
		}

		internal unsafe static int AllocEntry(UnsafeHashMapData* data, int threadIndex)
		{
			int* next = (int*)data->next;
			int num;
			for (;;)
			{
				num = *((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(threadIndex * 16) * 4);
				if (num < 0)
				{
					Interlocked.Exchange((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(threadIndex * 16) * 4, -2);
					if (data->allocatedIndexLength < data->keyCapacity)
					{
						num = Interlocked.Add(ref data->allocatedIndexLength, 16) - 16;
						if (num < data->keyCapacity - 1)
						{
							break;
						}
						if (num == data->keyCapacity - 1)
						{
							goto Block_5;
						}
					}
					Interlocked.Exchange((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(threadIndex * 16) * 4, -1);
					bool flag = true;
					while (flag)
					{
						flag = false;
						for (int num2 = (threadIndex + 1) % 128; num2 != threadIndex; num2 = (num2 + 1) % 128)
						{
							do
							{
								num = *((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(num2 * 16) * 4);
							}
							while (num >= 0 && Interlocked.CompareExchange((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(num2 * 16) * 4, next[num], num) != num);
							if (num == -2)
							{
								flag = true;
							}
							else if (num >= 0)
							{
								goto Block_8;
							}
						}
					}
				}
				if (Interlocked.CompareExchange((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(threadIndex * 16) * 4, next[num], num) == num)
				{
					goto Block_9;
				}
			}
			int num3 = math.min(16, data->keyCapacity - num);
			for (int i = 1; i < num3; i++)
			{
				next[num + i] = num + i + 1;
			}
			next[num + num3 - 1] = -1;
			next[num] = -1;
			Interlocked.Exchange((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(threadIndex * 16) * 4, num + 1);
			return num;
			Block_5:
			Interlocked.Exchange((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(threadIndex * 16) * 4, -1);
			return num;
			Block_8:
			next[num] = -1;
			return num;
			Block_9:
			next[num] = -1;
			return num;
		}

		internal unsafe static void FreeEntry(UnsafeHashMapData* data, int idx, int threadIndex)
		{
			int* next = (int*)data->next;
			int num;
			do
			{
				num = *((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(threadIndex * 16) * 4);
				next[idx] = num;
			}
			while (Interlocked.CompareExchange((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(threadIndex * 16) * 4, idx, num) != num);
		}

		internal unsafe static bool TryAddAtomic(UnsafeHashMapData* data, TKey key, TValue item, int threadIndex)
		{
			TValue tvalue;
			NativeMultiHashMapIterator<TKey> nativeMultiHashMapIterator;
			if (UnsafeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(data, key, out tvalue, out nativeMultiHashMapIterator))
			{
				return false;
			}
			int num = UnsafeHashMapBase<TKey, TValue>.AllocEntry(data, threadIndex);
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
					if (UnsafeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(data, key, out tvalue, out nativeMultiHashMapIterator))
					{
						break;
					}
					if (Interlocked.CompareExchange(ref buckets[num2], num, num3) == num3)
					{
						return true;
					}
				}
				UnsafeHashMapBase<TKey, TValue>.FreeEntry(data, num, threadIndex);
				return false;
			}
			return true;
		}

		internal unsafe static void AddAtomicMulti(UnsafeHashMapData* data, TKey key, TValue item, int threadIndex)
		{
			int num = UnsafeHashMapBase<TKey, TValue>.AllocEntry(data, threadIndex);
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

		internal unsafe static bool TryAdd(UnsafeHashMapData* data, TKey key, TValue item, bool isMultiHashMap, AllocatorManager.AllocatorHandle allocation)
		{
			TValue tvalue;
			NativeMultiHashMapIterator<TKey> nativeMultiHashMapIterator;
			if (!isMultiHashMap && UnsafeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(data, key, out tvalue, out nativeMultiHashMapIterator))
			{
				return false;
			}
			int num;
			int* ptr;
			if (data->allocatedIndexLength >= data->keyCapacity && data->firstFreeTLS.FixedElementField < 0)
			{
				for (int i = 1; i < 128; i++)
				{
					if (*((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(i * 16) * 4) >= 0)
					{
						num = *((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(i * 16) * 4);
						ptr = (int*)data->next;
						*((ref data->firstFreeTLS.FixedElementField) + (IntPtr)(i * 16) * 4) = ptr[num];
						ptr[num] = -1;
						data->firstFreeTLS.FixedElementField = num;
						break;
					}
				}
				if (data->firstFreeTLS.FixedElementField < 0)
				{
					int num2 = UnsafeHashMapData.GrowCapacity(data->keyCapacity);
					UnsafeHashMapData.ReallocateHashMap<TKey, TValue>(data, num2, UnsafeHashMapData.GetBucketSize(num2), allocation);
				}
			}
			num = data->firstFreeTLS.FixedElementField;
			if (num >= 0)
			{
				data->firstFreeTLS.FixedElementField = *(int*)(data->next + (IntPtr)num * 4);
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

		internal unsafe static int Remove(UnsafeHashMapData* data, TKey key, bool isMultiHashMap)
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
					next[num4] = data->firstFreeTLS.FixedElementField;
					data->firstFreeTLS.FixedElementField = num4;
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

		internal unsafe static void Remove(UnsafeHashMapData* data, NativeMultiHashMapIterator<TKey> it)
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
			next[it.EntryIndex] = data->firstFreeTLS.FixedElementField;
			data->firstFreeTLS.FixedElementField = it.EntryIndex;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		internal unsafe static void RemoveKeyValue<TValueEQ>(UnsafeHashMapData* data, TKey key, TValueEQ value) where TValueEQ : struct, IEquatable<TValueEQ>
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
			int* ptr2 = &data->firstFreeTLS.FixedElementField;
			for (;;)
			{
				TKey tkey = UnsafeUtility.ReadArrayElement<TKey>((void*)keys, num);
				if (!tkey.Equals(key))
				{
					goto IL_00B4;
				}
				TValueEQ tvalueEQ = UnsafeUtility.ReadArrayElement<TValueEQ>((void*)values, num);
				if (!tvalueEQ.Equals(value))
				{
					goto IL_00B4;
				}
				int num2 = next[num];
				next[num] = *ptr2;
				*ptr2 = num;
				num = (*ptr = num2);
				IL_00BF:
				if (num >= (int)keyCapacity)
				{
					break;
				}
				continue;
				IL_00B4:
				ptr = next + num;
				num = *ptr;
				goto IL_00BF;
			}
		}

		internal unsafe static bool TryGetFirstValueAtomic(UnsafeHashMapData* data, TKey key, out TValue item, out NativeMultiHashMapIterator<TKey> it)
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
			return UnsafeHashMapBase<TKey, TValue>.TryGetNextValueAtomic(data, out item, ref it);
		}

		internal unsafe static bool TryGetNextValueAtomic(UnsafeHashMapData* data, out TValue item, ref NativeMultiHashMapIterator<TKey> it)
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

		internal unsafe static bool SetValue(UnsafeHashMapData* data, ref NativeMultiHashMapIterator<TKey> it, ref TValue item)
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
		private static void CheckOutOfCapacity(int idx, int keyCapacity)
		{
			if (idx >= keyCapacity)
			{
				throw new InvalidOperationException(string.Format("nextPtr idx {0} beyond capacity {1}", idx, keyCapacity));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private unsafe static void CheckIndexOutOfBounds(UnsafeHashMapData* data, int idx)
		{
			if (idx < 0 || idx >= data->keyCapacity)
			{
				throw new InvalidOperationException("Internal HashMap error");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void ThrowFull()
		{
			throw new InvalidOperationException("HashMap is full");
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void ThrowInvalidIterator()
		{
			throw new InvalidOperationException("Invalid iterator passed to HashMap remove");
		}
	}
}
