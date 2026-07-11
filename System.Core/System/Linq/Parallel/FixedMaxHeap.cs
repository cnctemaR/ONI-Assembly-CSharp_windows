using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class FixedMaxHeap<TElement>
	{
		internal FixedMaxHeap(int maximumSize)
			: this(maximumSize, Util.GetDefaultComparer<TElement>())
		{
		}

		internal FixedMaxHeap(int maximumSize, IComparer<TElement> comparer)
		{
			this._elements = new TElement[maximumSize];
			this._comparer = comparer;
		}

		internal int Count
		{
			get
			{
				return this._count;
			}
		}

		internal int Size
		{
			get
			{
				return this._elements.Length;
			}
		}

		internal TElement MaxValue
		{
			get
			{
				if (this._count == 0)
				{
					throw new InvalidOperationException("Sequence contains no elements");
				}
				return this._elements[0];
			}
		}

		internal void Clear()
		{
			this._count = 0;
		}

		internal bool Insert(TElement e)
		{
			if (this._count < this._elements.Length)
			{
				this._elements[this._count] = e;
				this._count++;
				this.HeapifyLastLeaf();
				return true;
			}
			if (this._comparer.Compare(e, this._elements[0]) < 0)
			{
				this._elements[0] = e;
				this.HeapifyRoot();
				return true;
			}
			return false;
		}

		internal void ReplaceMax(TElement newValue)
		{
			this._elements[0] = newValue;
			this.HeapifyRoot();
		}

		internal void RemoveMax()
		{
			this._count--;
			if (this._count > 0)
			{
				this._elements[0] = this._elements[this._count];
				this.HeapifyRoot();
			}
		}

		private void Swap(int i, int j)
		{
			TElement telement = this._elements[i];
			this._elements[i] = this._elements[j];
			this._elements[j] = telement;
		}

		private void HeapifyRoot()
		{
			int i = 0;
			int count = this._count;
			while (i < count)
			{
				int num = (i + 1) * 2 - 1;
				int num2 = num + 1;
				if (num < count && this._comparer.Compare(this._elements[i], this._elements[num]) < 0)
				{
					if (num2 < count && this._comparer.Compare(this._elements[num], this._elements[num2]) < 0)
					{
						this.Swap(i, num2);
						i = num2;
					}
					else
					{
						this.Swap(i, num);
						i = num;
					}
				}
				else
				{
					if (num2 >= count || this._comparer.Compare(this._elements[i], this._elements[num2]) >= 0)
					{
						break;
					}
					this.Swap(i, num2);
					i = num2;
				}
			}
		}

		private void HeapifyLastLeaf()
		{
			int num;
			for (int i = this._count - 1; i > 0; i = num)
			{
				num = (i + 1) / 2 - 1;
				if (this._comparer.Compare(this._elements[i], this._elements[num]) <= 0)
				{
					break;
				}
				this.Swap(i, num);
			}
		}

		private TElement[] _elements;

		private int _count;

		private IComparer<TElement> _comparer;
	}
}
