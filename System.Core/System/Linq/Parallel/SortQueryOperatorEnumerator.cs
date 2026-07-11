using System;

namespace System.Linq.Parallel
{
	internal class SortQueryOperatorEnumerator<TInputOutput, TKey, TSortKey> : QueryOperatorEnumerator<TInputOutput, TSortKey>
	{
		internal SortQueryOperatorEnumerator(QueryOperatorEnumerator<TInputOutput, TKey> source, Func<TInputOutput, TSortKey> keySelector)
		{
			this._source = source;
			this._keySelector = keySelector;
		}

		internal override bool MoveNext(ref TInputOutput currentElement, ref TSortKey currentKey)
		{
			TKey tkey = default(TKey);
			if (!this._source.MoveNext(ref currentElement, ref tkey))
			{
				return false;
			}
			currentKey = this._keySelector(currentElement);
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			this._source.Dispose();
		}

		private readonly QueryOperatorEnumerator<TInputOutput, TKey> _source;

		private readonly Func<TInputOutput, TSortKey> _keySelector;
	}
}
