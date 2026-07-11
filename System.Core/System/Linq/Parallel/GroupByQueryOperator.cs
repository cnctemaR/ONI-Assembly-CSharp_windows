using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class GroupByQueryOperator<TSource, TGroupKey, TElement> : UnaryQueryOperator<TSource, IGrouping<TGroupKey, TElement>>
	{
		internal GroupByQueryOperator(IEnumerable<TSource> child, Func<TSource, TGroupKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TGroupKey> keyComparer)
			: base(child)
		{
			this._keySelector = keySelector;
			this._elementSelector = elementSelector;
			this._keyComparer = keyComparer;
			base.SetOrdinalIndexState(OrdinalIndexState.Shuffled);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TSource, TKey> inputStream, IPartitionedStreamRecipient<IGrouping<TGroupKey, TElement>> recipient, bool preferStriping, QuerySettings settings)
		{
			if (base.Child.OutputOrdered)
			{
				this.WrapPartitionedStreamHelperOrdered<TKey>(ExchangeUtilities.HashRepartitionOrdered<TSource, TGroupKey, TKey>(inputStream, this._keySelector, this._keyComparer, null, settings.CancellationState.MergedCancellationToken), recipient, settings.CancellationState.MergedCancellationToken);
				return;
			}
			this.WrapPartitionedStreamHelper<TKey, int>(ExchangeUtilities.HashRepartition<TSource, TGroupKey, TKey>(inputStream, this._keySelector, this._keyComparer, null, settings.CancellationState.MergedCancellationToken), recipient, settings.CancellationState.MergedCancellationToken);
		}

		private void WrapPartitionedStreamHelper<TIgnoreKey, TKey>(PartitionedStream<Pair<TSource, TGroupKey>, TKey> hashStream, IPartitionedStreamRecipient<IGrouping<TGroupKey, TElement>> recipient, CancellationToken cancellationToken)
		{
			int partitionCount = hashStream.PartitionCount;
			PartitionedStream<IGrouping<TGroupKey, TElement>, TKey> partitionedStream = new PartitionedStream<IGrouping<TGroupKey, TElement>, TKey>(partitionCount, hashStream.KeyComparer, OrdinalIndexState.Shuffled);
			for (int i = 0; i < partitionCount; i++)
			{
				if (this._elementSelector == null)
				{
					GroupByIdentityQueryOperatorEnumerator<TSource, TGroupKey, TKey> groupByIdentityQueryOperatorEnumerator = new GroupByIdentityQueryOperatorEnumerator<TSource, TGroupKey, TKey>(hashStream[i], this._keyComparer, cancellationToken);
					partitionedStream[i] = (QueryOperatorEnumerator<IGrouping<TGroupKey, TElement>, TKey>)groupByIdentityQueryOperatorEnumerator;
				}
				else
				{
					partitionedStream[i] = new GroupByElementSelectorQueryOperatorEnumerator<TSource, TGroupKey, TElement, TKey>(hashStream[i], this._keyComparer, this._elementSelector, cancellationToken);
				}
			}
			recipient.Receive<TKey>(partitionedStream);
		}

		private void WrapPartitionedStreamHelperOrdered<TKey>(PartitionedStream<Pair<TSource, TGroupKey>, TKey> hashStream, IPartitionedStreamRecipient<IGrouping<TGroupKey, TElement>> recipient, CancellationToken cancellationToken)
		{
			int partitionCount = hashStream.PartitionCount;
			PartitionedStream<IGrouping<TGroupKey, TElement>, TKey> partitionedStream = new PartitionedStream<IGrouping<TGroupKey, TElement>, TKey>(partitionCount, hashStream.KeyComparer, OrdinalIndexState.Shuffled);
			IComparer<TKey> keyComparer = hashStream.KeyComparer;
			for (int i = 0; i < partitionCount; i++)
			{
				if (this._elementSelector == null)
				{
					OrderedGroupByIdentityQueryOperatorEnumerator<TSource, TGroupKey, TKey> orderedGroupByIdentityQueryOperatorEnumerator = new OrderedGroupByIdentityQueryOperatorEnumerator<TSource, TGroupKey, TKey>(hashStream[i], this._keySelector, this._keyComparer, keyComparer, cancellationToken);
					partitionedStream[i] = (QueryOperatorEnumerator<IGrouping<TGroupKey, TElement>, TKey>)orderedGroupByIdentityQueryOperatorEnumerator;
				}
				else
				{
					partitionedStream[i] = new OrderedGroupByElementSelectorQueryOperatorEnumerator<TSource, TGroupKey, TElement, TKey>(hashStream[i], this._keySelector, this._elementSelector, this._keyComparer, keyComparer, cancellationToken);
				}
			}
			recipient.Receive<TKey>(partitionedStream);
		}

		internal override QueryResults<IGrouping<TGroupKey, TElement>> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TSource, IGrouping<TGroupKey, TElement>>.UnaryQueryOperatorResults(base.Child.Open(settings, false), this, settings, false);
		}

		internal override IEnumerable<IGrouping<TGroupKey, TElement>> AsSequentialQuery(CancellationToken token)
		{
			IEnumerable<TSource> enumerable = CancellableEnumerable.Wrap<TSource>(base.Child.AsSequentialQuery(token), token);
			if (this._elementSelector == null)
			{
				return (IEnumerable<IGrouping<TGroupKey, TElement>>)enumerable.GroupBy<TSource, TGroupKey>(this._keySelector, this._keyComparer);
			}
			return enumerable.GroupBy<TSource, TGroupKey, TElement>(this._keySelector, this._elementSelector, this._keyComparer);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly Func<TSource, TGroupKey> _keySelector;

		private readonly Func<TSource, TElement> _elementSelector;

		private readonly IEqualityComparer<TGroupKey> _keyComparer;
	}
}
