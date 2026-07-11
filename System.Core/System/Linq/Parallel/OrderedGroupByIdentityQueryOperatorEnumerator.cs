using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class OrderedGroupByIdentityQueryOperatorEnumerator<TSource, TGroupKey, TOrderKey> : OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TSource, TOrderKey>
	{
		internal OrderedGroupByIdentityQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TSource, TGroupKey>, TOrderKey> source, Func<TSource, TGroupKey> keySelector, IEqualityComparer<TGroupKey> keyComparer, IComparer<TOrderKey> orderComparer, CancellationToken cancellationToken)
			: base(source, keySelector, keyComparer, orderComparer, cancellationToken)
		{
		}

		protected override HashLookup<Wrapper<TGroupKey>, OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TSource, TOrderKey>.GroupKeyData> BuildHashLookup()
		{
			HashLookup<Wrapper<TGroupKey>, OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TSource, TOrderKey>.GroupKeyData> hashLookup = new HashLookup<Wrapper<TGroupKey>, OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TSource, TOrderKey>.GroupKeyData>(new WrapperEqualityComparer<TGroupKey>(this._keyComparer));
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
				OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TSource, TOrderKey>.GroupKeyData groupKeyData = null;
				if (hashLookup.TryGetValue(wrapper, ref groupKeyData))
				{
					if (this._orderComparer.Compare(torderKey, groupKeyData._orderKey) < 0)
					{
						groupKeyData._orderKey = torderKey;
					}
				}
				else
				{
					groupKeyData = new OrderedGroupByQueryOperatorEnumerator<TSource, TGroupKey, TSource, TOrderKey>.GroupKeyData(torderKey, wrapper.Value, this._orderComparer);
					hashLookup.Add(wrapper, groupKeyData);
				}
				groupKeyData._grouping.Add(pair.First, torderKey);
			}
			for (int i = 0; i < hashLookup.Count; i++)
			{
				hashLookup[i].Value._grouping.DoneAdding();
			}
			return hashLookup;
		}
	}
}
