using System;
using System.Threading;

namespace System.Linq.Parallel
{
	internal abstract class InlinedAggregationOperatorEnumerator<TIntermediate> : QueryOperatorEnumerator<TIntermediate, int>
	{
		internal InlinedAggregationOperatorEnumerator(int partitionIndex, CancellationToken cancellationToken)
		{
			this._partitionIndex = partitionIndex;
			this._cancellationToken = cancellationToken;
		}

		internal sealed override bool MoveNext(ref TIntermediate currentElement, ref int currentKey)
		{
			if (!this._done && this.MoveNextCore(ref currentElement))
			{
				currentKey = this._partitionIndex;
				this._done = true;
				return true;
			}
			return false;
		}

		protected abstract bool MoveNextCore(ref TIntermediate currentElement);

		private int _partitionIndex;

		private bool _done;

		protected CancellationToken _cancellationToken;
	}
}
