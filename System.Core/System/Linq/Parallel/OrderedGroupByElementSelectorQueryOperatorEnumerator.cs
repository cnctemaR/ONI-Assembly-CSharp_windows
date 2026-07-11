using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class OrderedGroupByElementSelectorQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey> : OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>
	{
		internal OrderedGroupByElementSelectorQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TSource, TGroupKey>, TOrderKey> source, Func<TSource, TGroupKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TGroupKey> keyComparer, IComparer<TOrderKey> orderComparer, CancellationToken cancellationToken)
			: base(source, keySelector, keyComparer, orderComparer, cancellationToken)
		{
			this._elementSelector = elementSelector;
		}

		protected override HashLookup<Wrapper<TGroupKey>, OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.GroupKeyData> BuildHashLookup()
		{
			HashLookup<Wrapper<TGroupKey>, OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.GroupKeyData> hashLookup = new HashLookup<Wrapper<TGroupKey>, OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.GroupKeyData>(new WrapperEqualityComparer<TGroupKey>(this._keyComparer));
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
				OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.GroupKeyData groupKeyData = null;
				if (hashLookup.TryGetValue(wrapper, ref groupKeyData))
				{
					if (this._orderComparer.Compare(torderKey, groupKeyData._orderKey) < 0)
					{
						groupKeyData._orderKey = torderKey;
					}
				}
				else
				{
					groupKeyData = new OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TElement, TOrderKey>.GroupKeyData(torderKey, wrapper.Value, this._orderComparer);
					hashLookup.Add(wrapper, groupKeyData);
				}
				groupKeyData._grouping.Add(this._elementSelector(pair.First), torderKey);
			}
			for (int i = 0; i < hashLookup.Count; i++)
			{
				hashLookup[i].Value._grouping.DoneAdding();
			}
			return hashLookup;
		}

		private readonly Func<TSource, TElement> _elementSelector;
	}
}
