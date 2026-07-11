using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class IndexedSelectQueryOperator<TInput, TOutput> : UnaryQueryOperator<TInput, TOutput>
	{
		internal IndexedSelectQueryOperator(IEnumerable<TInput> child, Func<TInput, int, TOutput> selector)
			: base(child)
		{
			this._selector = selector;
			this._outputOrdered = true;
			this.InitOrdinalIndexState();
		}

		private void InitOrdinalIndexState()
		{
			OrdinalIndexState ordinalIndexState = base.Child.OrdinalIndexState;
			OrdinalIndexState ordinalIndexState2 = ordinalIndexState;
			if (ordinalIndexState.IsWorseThan(OrdinalIndexState.Correct))
			{
				this._prematureMerge = true;
				this._limitsParallelism = ordinalIndexState != OrdinalIndexState.Shuffled;
				ordinalIndexState2 = OrdinalIndexState.Correct;
			}
			base.SetOrdinalIndexState(ordinalIndexState2);
		}

		internal override QueryResults<TOutput> Open(QuerySettings settings, bool preferStriping)
		{
			return IndexedSelectQueryOperator<TInput, TOutput>.IndexedSelectQueryOperatorResults.NewResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TInput, TKey> inputStream, IPartitionedStreamRecipient<TOutput> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<TInput, int> partitionedStream;
			if (this._prematureMerge)
			{
				partitionedStream = QueryOperator<TInput>.ExecuteAndCollectResults<TKey>(inputStream, partitionCount, base.Child.OutputOrdered, preferStriping, settings).GetPartitionedStream();
			}
			else
			{
				partitionedStream = (PartitionedStream<TInput, int>)inputStream;
			}
			PartitionedStream<TOutput, int> partitionedStream2 = new PartitionedStream<TOutput, int>(partitionCount, Util.GetDefaultComparer<int>(), this.OrdinalIndexState);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream2[i] = new IndexedSelectQueryOperator<TInput, TOutput>.IndexedSelectQueryOperatorEnumerator(partitionedStream[i], this._selector);
			}
			recipient.Receive<int>(partitionedStream2);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return this._limitsParallelism;
			}
		}

		internal override IEnumerable<TOutput> AsSequentialQuery(CancellationToken token)
		{
			return base.Child.AsSequentialQuery(token).Select<TInput, TOutput>(this._selector);
		}

		private readonly Func<TInput, int, TOutput> _selector;

		private bool _prematureMerge;

		private bool _limitsParallelism;

		private class IndexedSelectQueryOperatorEnumerator : QueryOperatorEnumerator<TOutput, int>
		{
			internal IndexedSelectQueryOperatorEnumerator(QueryOperatorEnumerator<TInput, int> source, Func<TInput, int, TOutput> selector)
			{
				this._source = source;
				this._selector = selector;
			}

			internal override bool MoveNext(ref TOutput currentElement, ref int currentKey)
			{
				TInput tinput = default(TInput);
				if (this._source.MoveNext(ref tinput, ref currentKey))
				{
					currentElement = this._selector(tinput, currentKey);
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TInput, int> _source;

			private readonly Func<TInput, int, TOutput> _selector;
		}

		private class IndexedSelectQueryOperatorResults : UnaryQueryOperator<TInput, TOutput>.UnaryQueryOperatorResults
		{
			public static QueryResults<TOutput> NewResults(QueryResults<TInput> childQueryResults, IndexedSelectQueryOperator<TInput, TOutput> op, QuerySettings settings, bool preferStriping)
			{
				if (childQueryResults.IsIndexible)
				{
					return new IndexedSelectQueryOperator<TInput, TOutput>.IndexedSelectQueryOperatorResults(childQueryResults, op, settings, preferStriping);
				}
				return new UnaryQueryOperator<TInput, TOutput>.UnaryQueryOperatorResults(childQueryResults, op, settings, preferStriping);
			}

			private IndexedSelectQueryOperatorResults(QueryResults<TInput> childQueryResults, IndexedSelectQueryOperator<TInput, TOutput> op, QuerySettings settings, bool preferStriping)
				: base(childQueryResults, op, settings, preferStriping)
			{
				this._selectOp = op;
				this._childCount = this._childQueryResults.ElementsCount;
			}

			internal override int ElementsCount
			{
				get
				{
					return this._childQueryResults.ElementsCount;
				}
			}

			internal override bool IsIndexible
			{
				get
				{
					return true;
				}
			}

			internal override TOutput GetElement(int index)
			{
				return this._selectOp._selector(this._childQueryResults.GetElement(index), index);
			}

			private IndexedSelectQueryOperator<TInput, TOutput> _selectOp;

			private int _childCount;
		}
	}
}
