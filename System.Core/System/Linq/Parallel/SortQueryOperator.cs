using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class SortQueryOperator<TInputOutput, TSortKey> : UnaryQueryOperator<TInputOutput, TInputOutput>, IOrderedEnumerable<TInputOutput>, IEnumerable<TInputOutput>, IEnumerable
	{
		internal SortQueryOperator(IEnumerable<TInputOutput> source, Func<TInputOutput, TSortKey> keySelector, IComparer<TSortKey> comparer, bool descending)
			: base(source, true)
		{
			this._keySelector = keySelector;
			if (comparer == null)
			{
				this._comparer = Util.GetDefaultComparer<TSortKey>();
			}
			else
			{
				this._comparer = comparer;
			}
			if (descending)
			{
				this._comparer = new ReverseComparer<TSortKey>(this._comparer);
			}
			base.SetOrdinalIndexState(OrdinalIndexState.Shuffled);
		}

		IOrderedEnumerable<TInputOutput> IOrderedEnumerable<TInputOutput>.CreateOrderedEnumerable<TKey2>(Func<TInputOutput, TKey2> key2Selector, IComparer<TKey2> key2Comparer, bool descending)
		{
			key2Comparer = key2Comparer ?? Util.GetDefaultComparer<TKey2>();
			if (descending)
			{
				key2Comparer = new ReverseComparer<TKey2>(key2Comparer);
			}
			IComparer<Pair<TSortKey, TKey2>> comparer = new PairComparer<TSortKey, TKey2>(this._comparer, key2Comparer);
			Func<TInputOutput, Pair<TSortKey, TKey2>> func = (TInputOutput elem) => new Pair<TSortKey, TKey2>(this._keySelector(elem), key2Selector(elem));
			return new SortQueryOperator<TInputOutput, Pair<TSortKey, TKey2>>(base.Child, func, comparer, false);
		}

		internal override QueryResults<TInputOutput> Open(QuerySettings settings, bool preferStriping)
		{
			return new SortQueryOperatorResults<TInputOutput, TSortKey>(base.Child.Open(settings, false), this, settings);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TInputOutput, TKey> inputStream, IPartitionedStreamRecipient<TInputOutput> recipient, bool preferStriping, QuerySettings settings)
		{
			PartitionedStream<TInputOutput, TSortKey> partitionedStream = new PartitionedStream<TInputOutput, TSortKey>(inputStream.PartitionCount, this._comparer, this.OrdinalIndexState);
			for (int i = 0; i < partitionedStream.PartitionCount; i++)
			{
				partitionedStream[i] = new SortQueryOperatorEnumerator<TInputOutput, TKey, TSortKey>(inputStream[i], this._keySelector);
			}
			recipient.Receive<TSortKey>(partitionedStream);
		}

		internal override IEnumerable<TInputOutput> AsSequentialQuery(CancellationToken token)
		{
			return CancellableEnumerable.Wrap<TInputOutput>(base.Child.AsSequentialQuery(token), token).OrderBy<TInputOutput, TSortKey>(this._keySelector, this._comparer);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly Func<TInputOutput, TSortKey> _keySelector;

		private readonly IComparer<TSortKey> _comparer;
	}
}
