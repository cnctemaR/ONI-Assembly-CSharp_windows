using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	internal sealed class OrderedPartition<TElement> : IPartition<TElement>, IIListProvider<TElement>, IEnumerable<TElement>, IEnumerable
	{
		public OrderedPartition(OrderedEnumerable<TElement> source, int minIdxInclusive, int maxIdxInclusive)
		{
			this._source = source;
			this._minIndexInclusive = minIdxInclusive;
			this._maxIndexInclusive = maxIdxInclusive;
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			return this._source.GetEnumerator(this._minIndexInclusive, this._maxIndexInclusive);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IPartition<TElement> Skip(int count)
		{
			int num = this._minIndexInclusive + count;
			if (num <= this._maxIndexInclusive)
			{
				return new OrderedPartition<TElement>(this._source, num, this._maxIndexInclusive);
			}
			return EmptyPartition<TElement>.Instance;
		}

		public IPartition<TElement> Take(int count)
		{
			int num = this._minIndexInclusive + count - 1;
			if (num >= this._maxIndexInclusive)
			{
				return this;
			}
			return new OrderedPartition<TElement>(this._source, this._minIndexInclusive, num);
		}

		public TElement TryGetElementAt(int index, out bool found)
		{
			if (index <= this._maxIndexInclusive - this._minIndexInclusive)
			{
				return this._source.TryGetElementAt(index + this._minIndexInclusive, out found);
			}
			found = false;
			return default(TElement);
		}

		public TElement TryGetFirst(out bool found)
		{
			return this._source.TryGetElementAt(this._minIndexInclusive, out found);
		}

		public TElement TryGetLast(out bool found)
		{
			return this._source.TryGetLast(this._minIndexInclusive, this._maxIndexInclusive, out found);
		}

		public TElement[] ToArray()
		{
			return this._source.ToArray(this._minIndexInclusive, this._maxIndexInclusive);
		}

		public List<TElement> ToList()
		{
			return this._source.ToList(this._minIndexInclusive, this._maxIndexInclusive);
		}

		public int GetCount(bool onlyIfCheap)
		{
			return this._source.GetCount(this._minIndexInclusive, this._maxIndexInclusive, onlyIfCheap);
		}

		private readonly OrderedEnumerable<TElement> _source;

		private readonly int _minIndexInclusive;

		private readonly int _maxIndexInclusive;
	}
}
