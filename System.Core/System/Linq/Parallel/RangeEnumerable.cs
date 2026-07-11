using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class RangeEnumerable : ParallelQuery<int>, IParallelPartitionable<int>
	{
		internal RangeEnumerable(int from, int count)
			: base(QuerySettings.Empty)
		{
			this._from = from;
			this._count = count;
		}

		public QueryOperatorEnumerator<int, int>[] GetPartitions(int partitionCount)
		{
			int num = this._count / partitionCount;
			int num2 = this._count % partitionCount;
			int num3 = 0;
			QueryOperatorEnumerator<int, int>[] array = new QueryOperatorEnumerator<int, int>[partitionCount];
			for (int i = 0; i < partitionCount; i++)
			{
				int num4 = ((i < num2) ? (num + 1) : num);
				array[i] = new RangeEnumerable.RangeEnumerator(this._from + num3, num4, num3);
				num3 += num4;
			}
			return array;
		}

		public override IEnumerator<int> GetEnumerator()
		{
			return new RangeEnumerable.RangeEnumerator(this._from, this._count, 0).AsClassicEnumerator();
		}

		private int _from;

		private int _count;

		private class RangeEnumerator : QueryOperatorEnumerator<int, int>
		{
			internal RangeEnumerator(int from, int count, int initialIndex)
			{
				this._from = from;
				this._count = count;
				this._initialIndex = initialIndex;
			}

			internal override bool MoveNext(ref int currentElement, ref int currentKey)
			{
				if (this._currentCount == null)
				{
					this._currentCount = new Shared<int>(-1);
				}
				int num = this._currentCount.Value + 1;
				if (num < this._count)
				{
					this._currentCount.Value = num;
					currentElement = num + this._from;
					currentKey = num + this._initialIndex;
					return true;
				}
				return false;
			}

			internal override void Reset()
			{
				this._currentCount = null;
			}

			private readonly int _from;

			private readonly int _count;

			private readonly int _initialIndex;

			private Shared<int> _currentCount;
		}
	}
}
