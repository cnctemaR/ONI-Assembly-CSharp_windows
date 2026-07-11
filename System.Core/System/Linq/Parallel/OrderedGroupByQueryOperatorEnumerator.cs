using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal abstract class OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey> : QueryOperatorEnumerator<IGrouping<TGroupKey, TElement>, TOrderKey>
	{
		protected OrderedGroupByQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TSource, TGroupKey>, TOrderKey> source, Func<TSource, TGroupKey> keySelector, IEqualityComparer<TGroupKey> keyComparer, IComparer<TOrderKey> orderComparer, CancellationToken cancellationToken)
		{
			this._source = source;
			this._keySelector = keySelector;
			this._keyComparer = keyComparer;
			this._orderComparer = orderComparer;
			this._cancellationToken = cancellationToken;
		}

		internal override bool MoveNext(ref IGrouping<TGroupKey, TElement> currentElement, ref TOrderKey currentKey)
		{
			OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.Mutables mutables = this._mutables;
			if (mutables == null)
			{
				mutables = (this._mutables = new OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.Mutables());
				mutables._hashLookup = this.BuildHashLookup();
				mutables._hashLookupIndex = -1;
			}
			OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.Mutables mutables2 = mutables;
			int num = mutables2._hashLookupIndex + 1;
			mutables2._hashLookupIndex = num;
			if (num < mutables._hashLookup.Count)
			{
				OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.GroupKeyData value = mutables._hashLookup[mutables._hashLookupIndex].Value;
				currentElement = value._grouping;
				currentKey = value._orderKey;
				return true;
			}
			return false;
		}

		protected abstract HashLookup<Wrapper<TGroupKey>, OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.GroupKeyData> BuildHashLookup();

		protected override void Dispose(bool disposing)
		{
			this._source.Dispose();
		}

		protected readonly QueryOperatorEnumerator<Pair<TSource, TGroupKey>, TOrderKey> _source;

		private readonly Func<TSource, TGroupKey> _keySelector;

		protected readonly IEqualityComparer<TGroupKey> _keyComparer;

		protected readonly IComparer<TOrderKey> _orderComparer;

		protected readonly CancellationToken _cancellationToken;

		private OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.Mutables _mutables;

		private class Mutables
		{
			internal HashLookup<Wrapper<TGroupKey>, OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.GroupKeyData> _hashLookup;

			internal int _hashLookupIndex;
		}

		protected class GroupKeyData
		{
			internal GroupKeyData(TOrderKey orderKey, TGroupKey hashKey, IComparer<TOrderKey> orderComparer)
			{
				this._orderKey = orderKey;
				this._grouping = new OrderedGroupByGrouping<TGroupKey, TOrderKey, TElement>(hashKey, orderComparer);
			}

			internal TOrderKey _orderKey;

			internal OrderedGroupByGrouping<TGroupKey, TOrderKey, TElement> _grouping;
		}
	}
}
