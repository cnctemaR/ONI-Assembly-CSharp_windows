using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal abstract class GroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey> : QueryOperatorEnumerator<IGrouping<TGroupKey, TElement>, TOrderKey>
	{
		protected GroupByQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TSource, TGroupKey>, TOrderKey> source, IEqualityComparer<TGroupKey> keyComparer, CancellationToken cancellationToken)
		{
			this._source = source;
			this._keyComparer = keyComparer;
			this._cancellationToken = cancellationToken;
		}

		internal override bool MoveNext(ref IGrouping<TGroupKey, TElement> currentElement, ref TOrderKey currentKey)
		{
			GroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.Mutables mutables = this._mutables;
			if (mutables == null)
			{
				mutables = (this._mutables = new GroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.Mutables());
				mutables._hashLookup = this.BuildHashLookup();
				mutables._hashLookupIndex = -1;
			}
			GroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.Mutables mutables2 = mutables;
			int num = mutables2._hashLookupIndex + 1;
			mutables2._hashLookupIndex = num;
			if (num < mutables._hashLookup.Count)
			{
				currentElement = new GroupByGrouping<TGroupKey, TElement>(mutables._hashLookup[mutables._hashLookupIndex]);
				return true;
			}
			return false;
		}

		protected abstract HashLookup<Wrapper<TGroupKey>, ListChunk<TElement>> BuildHashLookup();

		protected override void Dispose(bool disposing)
		{
			this._source.Dispose();
		}

		protected readonly QueryOperatorEnumerator<Pair<TSource, TGroupKey>, TOrderKey> _source;

		protected readonly IEqualityComparer<TGroupKey> _keyComparer;

		protected readonly CancellationToken _cancellationToken;

		private GroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.Mutables _mutables;

		private class Mutables
		{
			internal HashLookup<Wrapper<TGroupKey>, ListChunk<TElement>> _hashLookup;

			internal int _hashLookupIndex;
		}
	}
}
