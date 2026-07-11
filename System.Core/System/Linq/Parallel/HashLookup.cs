using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class HashLookup<TKey, TValue>
	{
		internal HashLookup()
			: this(null)
		{
		}

		internal HashLookup(IEqualityComparer<TKey> comparer)
		{
			this.comparer = comparer;
			this.buckets = new int[7];
			this.slots = new HashLookup<TKey, TValue>.Slot[7];
			this.freeList = -1;
		}

		internal bool Add(TKey key, TValue value)
		{
			return !this.Find(key, true, false, ref value);
		}

		internal bool TryGetValue(TKey key, ref TValue value)
		{
			return this.Find(key, false, false, ref value);
		}

		internal TValue this[TKey key]
		{
			set
			{
				TValue tvalue = value;
				this.Find(key, false, true, ref tvalue);
			}
		}

		private int GetKeyHashCode(TKey key)
		{
			return int.MaxValue & ((this.comparer == null) ? ((key == null) ? 0 : key.GetHashCode()) : this.comparer.GetHashCode(key));
		}

		private bool AreKeysEqual(TKey key1, TKey key2)
		{
			if (this.comparer != null)
			{
				return this.comparer.Equals(key1, key2);
			}
			return (key1 == null && key2 == null) || (key1 != null && key1.Equals(key2));
		}

		private bool Find(TKey key, bool add, bool set, ref TValue value)
		{
			int keyHashCode = this.GetKeyHashCode(key);
			int i = this.buckets[keyHashCode % this.buckets.Length] - 1;
			while (i >= 0)
			{
				if (this.slots[i].hashCode == keyHashCode && this.AreKeysEqual(this.slots[i].key, key))
				{
					if (set)
					{
						this.slots[i].value = value;
						return true;
					}
					value = this.slots[i].value;
					return true;
				}
				else
				{
					i = this.slots[i].next;
				}
			}
			if (add)
			{
				int num;
				if (this.freeList >= 0)
				{
					num = this.freeList;
					this.freeList = this.slots[num].next;
				}
				else
				{
					if (this.count == this.slots.Length)
					{
						this.Resize();
					}
					num = this.count;
					this.count++;
				}
				int num2 = keyHashCode % this.buckets.Length;
				this.slots[num].hashCode = keyHashCode;
				this.slots[num].key = key;
				this.slots[num].value = value;
				this.slots[num].next = this.buckets[num2] - 1;
				this.buckets[num2] = num + 1;
			}
			return false;
		}

		private void Resize()
		{
			int num = checked(this.count * 2 + 1);
			int[] array = new int[num];
			HashLookup<TKey, TValue>.Slot[] array2 = new HashLookup<TKey, TValue>.Slot[num];
			Array.Copy(this.slots, 0, array2, 0, this.count);
			for (int i = 0; i < this.count; i++)
			{
				int num2 = array2[i].hashCode % num;
				array2[i].next = array[num2] - 1;
				array[num2] = i + 1;
			}
			this.buckets = array;
			this.slots = array2;
		}

		internal int Count
		{
			get
			{
				return this.count;
			}
		}

		internal KeyValuePair<TKey, TValue> this[int index]
		{
			get
			{
				return new KeyValuePair<TKey, TValue>(this.slots[index].key, this.slots[index].value);
			}
		}

		private int[] buckets;

		private HashLookup<TKey, TValue>.Slot[] slots;

		private int count;

		private int freeList;

		private IEqualityComparer<TKey> comparer;

		private const int HashCodeMask = 2147483647;

		internal struct Slot
		{
			internal int hashCode;

			internal int next;

			internal TKey key;

			internal TValue value;
		}
	}
}
