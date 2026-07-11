using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class GroupByIdentityQueryOperatorEnumerator<TSource, TGroupKey, TOrderKey> : GroupByQueryOperatorEnumerator<TSource, TGroupKey, TSource, TOrderKey>
	{
		internal GroupByIdentityQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TSource, TGroupKey>, TOrderKey> source, IEqualityComparer<TGroupKey> keyComparer, CancellationToken cancellationToken)
			: base(source, keyComparer, cancellationToken)
		{
		}

		protected override HashLookup<Wrapper<TGroupKey>, ListChunk<TSource>> BuildHashLookup()
		{
			HashLookup<Wrapper<TGroupKey>, ListChunk<TSource>> hashLookup = new HashLookup<Wrapper<TGroupKey>, ListChunk<TSource>>(new WrapperEqualityComparer<TGroupKey>(this._keyComparer));
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
				ListChunk<TSource> listChunk = null;
				if (!hashLookup.TryGetValue(wrapper, ref listChunk))
				{
					listChunk = new ListChunk<TSource>(2);
					hashLookup.Add(wrapper, listChunk);
				}
				listChunk.Add(pair.First);
			}
			return hashLookup;
		}
	}
}
