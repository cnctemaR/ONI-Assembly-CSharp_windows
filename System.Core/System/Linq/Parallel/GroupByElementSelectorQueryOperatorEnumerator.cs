using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class GroupByElementSelectorQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey> : GroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>
	{
		internal GroupByElementSelectorQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TSource, TGroupKey>, TOrderKey> source, IEqualityComparer<TGroupKey> keyComparer, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken)
			: base(source, keyComparer, cancellationToken)
		{
			this._elementSelector = elementSelector;
		}

		protected override HashLookup<Wrapper<TGroupKey>, ListChunk<TElement>> BuildHashLookup()
		{
			HashLookup<Wrapper<TGroupKey>, ListChunk<TElement>> hashLookup = new HashLookup<Wrapper<TGroupKey>, ListChunk<TElement>>(new WrapperEqualityComparer<TGroupKey>(this._keyComparer));
			Pair<TSource, TGroupKey> pair = default(Pair<TSource, TGroupKey>);
			TOrderKey torderKey = default(TOrderKey);
			int num = 0;
			while (this._source.MoveNext(ref pair, ref torderKey))
			{
				if ((num++ & 63) == 0)
				{
					CancellationState.ThrowIfCanceled(this._cancellationToken);
				}
				Wrapper<TGroupKey> wrapper = new Wrapper<TGroupKey>(pair.Second);
				ListChunk<TElement> listChunk = null;
				if (!hashLookup.TryGetValue(wrapper, ref listChunk))
				{
					listChunk = new ListChunk<TElement>(2);
					hashLookup.Add(wrapper, listChunk);
				}
				listChunk.Add(this._elementSelector(pair.First));
			}
			return hashLookup;
		}

		private readonly Func<TSource, TElement> _elementSelector;
	}
}
