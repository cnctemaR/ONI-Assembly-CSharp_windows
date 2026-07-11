using System;
using System.Collections.Generic;

namespace System.Linq
{
	internal sealed class Set<TElement>
	{
		public Set(IEqualityComparer<TElement> comparer)
		{
			this._comparer = comparer ?? EqualityComparer<TElement>.Default;
			this._buckets = new int[7];
			this._slots = new Set<TElement>.Slot[7];
		}

		public bool Add(TElement value)
		{
			int num = this.InternalGetHashCode(value);
			for (int i = this._buckets[num % this._buckets.Length] - 1; i >= 0; i = this._slots[i]._next)
			{
				if (this._slots[i]._hashCode == num && this._comparer.Equals(this._slots[i]._value, value))
				{
					return false;
				}
			}
			if (this._count == this._slots.Length)
			{
				this.Resize();
			}
			int count = this._count;
			this._count++;
			int num2 = num % this._buckets.Length;
			this._slots[count]._hashCode = num;
			this._slots[count]._value = value;
			this._slots[count]._next = this._buckets[num2] - 1;
			this._buckets[num2] = count + 1;
			return true;
		}

		public bool Remove(TElement value)
		{
			int num = this.InternalGetHashCode(value);
			int num2 = num % this._buckets.Length;
			int num3 = -1;
			for (int i = this._buckets[num2] - 1; i >= 0; i = this._slots[i]._next)
			{
				if (this._slots[i]._hashCode == num && this._comparer.Equals(this._slots[i]._value, value))
				{
					if (num3 < 0)
					{
						this._buckets[num2] = this._slots[i]._next + 1;
					}
					else
					{
						this._slots[num3]._next = this._slots[i]._next;
					}
					this._slots[i]._hashCode = -1;
					this._slots[i]._value = default(TElement);
					this._slots[i]._next = -1;
					return true;
				}
				num3 = i;
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
				int num2 = array2[i]._hashCode % num;
				array2[i]._next = array[num2] - 1;
				array[num2] = i + 1;
			}
			this._buckets = array;
			this._slots = array2;
		}

		public TElement[] ToArray()
		{
			TElement[] array = new TElement[this._count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = this._slots[num]._value;
			}
			return array;
		}

		public List<TElement> ToList()
		{
			int count = this._count;
			List<TElement> list = new List<TElement>(count);
			for (int num = 0; num != count; num++)
			{
				list.Add(this._slots[num]._value);
			}
			return list;
		}

		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public void UnionWith(IEnumerable<TElement> other)
		{
			foreach (TElement telement in other)
			{
				this.Add(telement);
			}
		}

		private int InternalGetHashCode(TElement value)
		{
			if (value != null)
			{
				return this._comparer.GetHashCode(value) & int.MaxValue;
			}
			return 0;
		}

		private readonly IEqualityComparer<TElement> _comparer;

		private int[] _buckets;

		private Set<TElement>.Slot[] _slots;

		private int _count;

		private struct Slot
		{
			internal int _hashCode;

			internal int _next;

			internal TElement _value;
		}
	}
}
