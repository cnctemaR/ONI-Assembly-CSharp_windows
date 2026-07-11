using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class ConcatQueryOperator<TSource> : BinaryQueryOperator<TSource, TSource, TSource>
	{
		internal ConcatQueryOperator(ParallelQuery<TSource> firstChild, ParallelQuery<TSource> secondChild)
			: base(firstChild, secondChild)
		{
			this._outputOrdered = base.LeftChild.OutputOrdered || base.RightChild.OutputOrdered;
			this._prematureMergeLeft = base.LeftChild.OrdinalIndexState.IsWorseThan(OrdinalIndexState.Increasing);
			this._prematureMergeRight = base.RightChild.OrdinalIndexState.IsWorseThan(OrdinalIndexState.Increasing);
			if (base.LeftChild.OrdinalIndexState == OrdinalIndexState.Indexable && base.RightChild.OrdinalIndexState == OrdinalIndexState.Indexable)
			{
				base.SetOrdinalIndex(OrdinalIndexState.Indexable);
				return;
			}
			base.SetOrdinalIndex(OrdinalIndexState.Increasing.Worse(base.LeftChild.OrdinalIndexState.Worse(base.RightChild.OrdinalIndexState)));
		}

		internal override QueryResults<TSource> Open(QuerySettings settings, bool preferStriping)
		{
			QueryResults<TSource> queryResults = base.LeftChild.Open(settings, preferStriping);
			QueryResults<TSource> queryResults2 = base.RightChild.Open(settings, preferStriping);
			return ConcatQueryOperator<TSource>.ConcatQueryOperatorResults.NewResults(queryResults, queryResults2, this, settings, preferStriping);
		}

		public override void WrapPartitionedStream<TLeftKey, TRightKey>(PartitionedStream<TSource, TLeftKey> leftStream, PartitionedStream<TSource, TRightKey> rightStream, IPartitionedStreamRecipient<TSource> outputRecipient, bool preferStriping, QuerySettings settings)
		{
			if (this._prematureMergeLeft)
			{
				PartitionedStream<TSource, int> partitionedStream = QueryOperator<TSource>.ExecuteAndCollectResults<TLeftKey>(leftStream, leftStream.PartitionCount, base.LeftChild.OutputOrdered, preferStriping, settings).GetPartitionedStream();
				this.WrapHelper<int, TRightKey>(partitionedStream, rightStream, outputRecipient, settings, preferStriping);
				return;
			}
			this.WrapHelper<TLeftKey, TRightKey>(leftStream, rightStream, outputRecipient, settings, preferStriping);
		}

		private void WrapHelper<TLeftKey, TRightKey>(PartitionedStream<TSource, TLeftKey> leftStreamInc, PartitionedStream<TSource, TRightKey> rightStream, IPartitionedStreamRecipient<TSource> outputRecipient, QuerySettings settings, bool preferStriping)
		{
			if (this._prematureMergeRight)
			{
				PartitionedStream<TSource, int> partitionedStream = QueryOperator<TSource>.ExecuteAndCollectResults<TRightKey>(rightStream, leftStreamInc.PartitionCount, base.LeftChild.OutputOrdered, preferStriping, settings).GetPartitionedStream();
				this.WrapHelper2<TLeftKey, int>(leftStreamInc, partitionedStream, outputRecipient);
				return;
			}
			this.WrapHelper2<TLeftKey, TRightKey>(leftStreamInc, rightStream, outputRecipient);
		}

		private void WrapHelper2<TLeftKey, TRightKey>(PartitionedStream<TSource, TLeftKey> leftStreamInc, PartitionedStream<TSource, TRightKey> rightStreamInc, IPartitionedStreamRecipient<TSource> outputRecipient)
		{
			int partitionCount = leftStreamInc.PartitionCount;
			IComparer<ConcatKey<TLeftKey, TRightKey>> comparer = ConcatKey<TLeftKey, TRightKey>.MakeComparer(leftStreamInc.KeyComparer, rightStreamInc.KeyComparer);
			PartitionedStream<TSource, ConcatKey<TLeftKey, TRightKey>> partitionedStream = new PartitionedStream<TSource, ConcatKey<TLeftKey, TRightKey>>(partitionCount, comparer, this.OrdinalIndexState);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new ConcatQueryOperator<TSource>.ConcatQueryOperatorEnumerator<TLeftKey, TRightKey>(leftStreamInc[i], rightStreamInc[i]);
			}
			outputRecipient.Receive<ConcatKey<TLeftKey, TRightKey>>(partitionedStream);
		}

		internal override IEnumerable<TSource> AsSequentialQuery(CancellationToken token)
		{
			return base.LeftChild.AsSequentialQuery(token).Concat<TSource>(base.RightChild.AsSequentialQuery(token));
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly bool _prematureMergeLeft;

		private readonly bool _prematureMergeRight;

		private sealed class ConcatQueryOperatorEnumerator<TLeftKey, TRightKey> : QueryOperatorEnumerator<TSource, ConcatKey<TLeftKey, TRightKey>>
		{
			internal ConcatQueryOperatorEnumerator(QueryOperatorEnumerator<TSource, TLeftKey> firstSource, QueryOperatorEnumerator<TSource, TRightKey> secondSource)
			{
				this._firstSource = firstSource;
				this._secondSource = secondSource;
			}

			internal override bool MoveNext(ref TSource currentElement, ref ConcatKey<TLeftKey, TRightKey> currentKey)
			{
				if (!this._begunSecond)
				{
					TLeftKey tleftKey = default(TLeftKey);
					if (this._firstSource.MoveNext(ref currentElement, ref tleftKey))
					{
						currentKey = ConcatKey<TLeftKey, TRightKey>.MakeLeft(tleftKey);
						return true;
					}
					this._begunSecond = true;
				}
				TRightKey trightKey = default(TRightKey);
				if (this._secondSource.MoveNext(ref currentElement, ref trightKey))
				{
					currentKey = ConcatKey<TLeftKey, TRightKey>.MakeRight(trightKey);
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._firstSource.Dispose();
				this._secondSource.Dispose();
			}

			private QueryOperatorEnumerator<TSource, TLeftKey> _firstSource;

			private QueryOperatorEnumerator<TSource, TRightKey> _secondSource;

			private bool _begunSecond;
		}

		private class ConcatQueryOperatorResults : BinaryQueryOperator<TSource, TSource, TSource>.BinaryQueryOperatorResults
		{
			public static QueryResults<TSource> NewResults(QueryResults<TSource> leftChildQueryResults, QueryResults<TSource> rightChildQueryResults, ConcatQueryOperator<TSource> op, QuerySettings settings, bool preferStriping)
			{
				if (leftChildQueryResults.IsIndexible && rightChildQueryResults.IsIndexible)
				{
					return new ConcatQueryOperator<TSource>.ConcatQueryOperatorResults(leftChildQueryResults, rightChildQueryResults, op, settings, preferStriping);
				}
				return new BinaryQueryOperator<TSource, TSource, TSource>.BinaryQueryOperatorResults(leftChildQueryResults, rightChildQueryResults, op, settings, preferStriping);
			}

			private ConcatQueryOperatorResults(QueryResults<TSource> leftChildQueryResults, QueryResults<TSource> rightChildQueryResults, ConcatQueryOperator<TSource> concatOp, QuerySettings settings, bool preferStriping)
				: base(leftChildQueryResults, rightChildQueryResults, concatOp, settings, preferStriping)
			{
				this._leftChildCount = leftChildQueryResults.ElementsCount;
				this._rightChildCount = rightChildQueryResults.ElementsCount;
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
					return this._leftChildCount + this._rightChildCount;
				}
			}

			internal override TSource GetElement(int index)
			{
				if (index < this._leftChildCount)
				{
					return this._leftChildQueryResults.GetElement(index);
				}
				return this._rightChildQueryResults.GetElement(index - this._leftChildCount);
			}

			private int _leftChildCount;

			private int _rightChildCount;
		}
	}
}
