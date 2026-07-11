using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class RepeatEnumerable<TResult> : ParallelQuery<TResult>, IParallelPartitionable<TResult>
	{
		internal RepeatEnumerable(TResult element, int count)
			: base(QuerySettings.Empty)
		{
			this._element = element;
			this._count = count;
		}

		public QueryOperatorEnumerator<TResult, int>[] GetPartitions(int partitionCount)
		{
			int num = (this._count + partitionCount - 1) / partitionCount;
			QueryOperatorEnumerator<TResult, int>[] array = new QueryOperatorEnumerator<TResult, int>[partitionCount];
			int i = 0;
			int num2 = 0;
			while (i < partitionCount)
			{
				if (num2 + num > this._count)
				{
					array[i] = new RepeatEnumerable<TResult>.RepeatEnumerator(this._element, (num2 < this._count) ? (this._count - num2) : 0, num2);
				}
				else
				{
					array[i] = new RepeatEnumerable<TResult>.RepeatEnumerator(this._element, num, num2);
				}
				i++;
				num2 += num;
			}
			return array;
		}

		public override IEnumerator<TResult> GetEnumerator()
		{
			return new RepeatEnumerable<TResult>.RepeatEnumerator(this._element, this._count, 0).AsClassicEnumerator();
		}

		private TResult _element;

		private int _count;

		private class RepeatEnumerator : QueryOperatorEnumerator<TResult, int>
		{
			internal RepeatEnumerator(TResult element, int count, int indexOffset)
			{
				this._element = element;
				this._count = count;
				this._indexOffset = indexOffset;
			}

			internal override bool MoveNext(ref TResult currentElement, ref int currentKey)
			{
				if (this._currentIndex == null)
				{
					this._currentIndex = new Shared<int>(-1);
				}
				if (this._currentIndex.Value < this._count - 1)
				{
					this._currentIndex.Value++;
					currentElement = this._element;
					currentKey = this._currentIndex.Value + this._indexOffset;
					return true;
				}
				return false;
			}

			internal override void Reset()
			{
				this._currentIndex = null;
			}

			private readonly TResult _element;

			private readonly int _count;

			private readonly int _indexOffset;

			private Shared<int> _currentIndex;
		}
	}
}
