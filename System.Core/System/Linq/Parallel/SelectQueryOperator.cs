using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class SelectQueryOperator<TInput, TOutput> : UnaryQueryOperator<TInput, TOutput>
	{
		internal SelectQueryOperator(IEnumerable<TInput> child, Func<TInput, TOutput> selector)
			: base(child)
		{
			this._selector = selector;
			base.SetOrdinalIndexState(base.Child.OrdinalIndexState);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TInput, TKey> inputStream, IPartitionedStreamRecipient<TOutput> recipient, bool preferStriping, QuerySettings settings)
		{
			PartitionedStream<TOutput, TKey> partitionedStream = new PartitionedStream<TOutput, TKey>(inputStream.PartitionCount, inputStream.KeyComparer, this.OrdinalIndexState);
			for (int i = 0; i < inputStream.PartitionCount; i++)
			{
				partitionedStream[i] = new SelectQueryOperator<TInput, TOutput>.SelectQueryOperatorEnumerator<TKey>(inputStream[i], this._selector);
			}
			recipient.Receive<TKey>(partitionedStream);
		}

		internal override QueryResults<TOutput> Open(QuerySettings settings, bool preferStriping)
		{
			return SelectQueryOperator<TInput, TOutput>.SelectQueryOperatorResults.NewResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override IEnumerable<TOutput> AsSequentialQuery(CancellationToken token)
		{
			return base.Child.AsSequentialQuery(token).Select<TInput, TOutput>(this._selector);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private Func<TInput, TOutput> _selector;

		private class SelectQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TOutput, TKey>
		{
			internal SelectQueryOperatorEnumerator(QueryOperatorEnumerator<TInput, TKey> source, Func<TInput, TOutput> selector)
			{
				this._source = source;
				this._selector = selector;
			}

			internal override bool MoveNext(ref TOutput currentElement, ref TKey currentKey)
			{
				TInput tinput = default(TInput);
				if (this._source.MoveNext(ref tinput, ref currentKey))
				{
					currentElement = this._selector(tinput);
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TInput, TKey> _source;

			private readonly Func<TInput, TOutput> _selector;
		}

		private class SelectQueryOperatorResults : UnaryQueryOperator<TInput, TOutput>.UnaryQueryOperatorResults
		{
			public static QueryResults<TOutput> NewResults(QueryResults<TInput> childQueryResults, SelectQueryOperator<TInput, TOutput> op, QuerySettings settings, bool preferStriping)
			{
				if (childQueryResults.IsIndexible)
				{
					return new SelectQueryOperator<TInput, TOutput>.SelectQueryOperatorResults(childQueryResults, op, settings, preferStriping);
				}
				return new UnaryQueryOperator<TInput, TOutput>.UnaryQueryOperatorResults(childQueryResults, op, settings, preferStriping);
			}

			private SelectQueryOperatorResults(QueryResults<TInput> childQueryResults, SelectQueryOperator<TInput, TOutput> op, QuerySettings settings, bool preferStriping)
				: base(childQueryResults, op, settings, preferStriping)
			{
				this._selector = op._selector;
				this._childCount = this._childQueryResults.ElementsCount;
			}

			internal override bool IsIndexible
			{
				get
				{
					return true;
				}
			}

			internal override int ElementsCount
			{
				get
				{
					return this._childCount;
				}
			}

			internal override TOutput GetElement(int index)
			{
				return this._selector(this._childQueryResults.GetElement(index));
			}

			private Func<TInput, TOutput> _selector;

			private int _childCount;
		}
	}
}
