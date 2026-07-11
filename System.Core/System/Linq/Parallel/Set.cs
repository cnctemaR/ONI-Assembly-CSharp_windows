using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class Set<TElement>
	{
		public Set(IEqualityComparer<TElement> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TElement>.Default;
			}
			this._comparer = comparer;
			this._buckets = new int[7];
			this._slots = new Set<TElement>.Slot[7];
		}

		public bool Add(TElement value)
		{
			return !this.Find(value, true);
		}

		public bool Contains(TElement value)
		{
			return this.Find(value, false);
		}

		public bool Remove(TElement value)
		{
			int num = this.InternalGetHashCode(value);
			int num2 = num % this._buckets.Length;
			int num3 = -1;
			for (int i = this._buckets[num2] - 1; i >= 0; i = this._slots[i].next)
			{
				if (this._slots[i].hashCode == num && this._comparer.Equals(this._slots[i].value, value))
				{
					if (num3 < 0)
					{
						this._buckets[num2] = this._slots[i].next + 1;
					}
					else
					{
						this._slots[num3].next = this._slots[i].next;
					}
					this._slots[i].hashCode = -1;
					this._slots[i].value = default(TElement);
					this._slots[i].next = -1;
					return true;
				}
				num3 = i;
			}
			return false;
		}

		private bool Find(TElement value, bool add)
		{
			int num = this.InternalGetHashCode(value);
			for (int i = this._buckets[num % this._buckets.Length] - 1; i >= 0; i = this._slots[i].next)
			{
				if (this._slots[i].hashCode == num && this._comparer.Equals(this._slots[i].value, value))
				{
					return true;
				}
			}
			if (add)
			{
				if (this._count == this._slots.Length)
				{
					this.Resize();
				}
				int count = this._count;
				this._count++;
				int num2 = num % this._buckets.Length;
				this._slots[count].hashCode = num;
				this._slots[count].value = value;
				this._slots[count].next = this._buckets[num2] - 1;
				this._buckets[num2] = count + 1;
			}
			return false;
		}

		private void Resize()
		{
			int num = checked(this._count * 2 + 1);
			int[] array = new int[num];
			Set<TElement>.Slot[] array2 = new Set<TElement>.Slot[num];
			Array.Copy(this._slots, 0, array2, 0, this._count);
			for (int i = 0; i < this._count; i++)
			{
				int num2 = array2[i].hashCode % num;
				array2[i].next = array[num2] - 1;
				array[num2] = i + 1;
			}
			this._buckets = array;
			this._slots = array2;
		}

		internal int InternalGetHashCode(TElement value)
		{
			if (value != null)
			{
				return this._comparer.GetHashCode(value) & int.MaxValue;
			}
			return 0;
		}

		private int[] _buckets;

		private Set<TElement>.Slot[] _slots;

		private int _count;

		private readonly IEqualityComparer<TElement> _comparer;

		private const int InitialSize = 7;

		private const int HashCodeMask = 2147483647;

		internal struct Slot
		{
			internal int hashCode;

			internal int next;

			internal TElement value;
		}
	}
}
