using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class ZipQueryOperator<TLeftInput, TRightInput, TOutput> : QueryOperator<TOutput>
	{
		internal ZipQueryOperator(ParallelQuery<TLeftInput> leftChildSource, ParallelQuery<TRightInput> rightChildSource, Func<TLeftInput, TRightInput, TOutput> resultSelector)
			: this(QueryOperator<TLeftInput>.AsQueryOperator(leftChildSource), QueryOperator<TRightInput>.AsQueryOperator(rightChildSource), resultSelector)
		{
		}

		private ZipQueryOperator(QueryOperator<TLeftInput> left, QueryOperator<TRightInput> right, Func<TLeftInput, TRightInput, TOutput> resultSelector)
			: base(left.SpecifiedQuerySettings.Merge(right.SpecifiedQuerySettings))
		{
			this._leftChild = left;
			this._rightChild = right;
			this._resultSelector = resultSelector;
			this._outputOrdered = this._leftChild.OutputOrdered || this._rightChild.OutputOrdered;
			OrdinalIndexState ordinalIndexState = this._leftChild.OrdinalIndexState;
			OrdinalIndexState ordinalIndexState2 = this._rightChild.OrdinalIndexState;
			this._prematureMergeLeft = ordinalIndexState > OrdinalIndexState.Indexable;
			this._prematureMergeRight = ordinalIndexState2 > OrdinalIndexState.Indexable;
			this._limitsParallelism = (this._prematureMergeLeft && ordinalIndexState != OrdinalIndexState.Shuffled) || (this._prematureMergeRight && ordinalIndexState2 != OrdinalIndexState.Shuffled);
		}

		internal override QueryResults<TOutput> Open(QuerySettings settings, bool preferStriping)
		{
			QueryResults<TLeftInput> queryResults = this._leftChild.Open(settings, preferStriping);
			QueryResults<TRightInput> queryResults2 = this._rightChild.Open(settings, preferStriping);
			int value = settings.DegreeOfParallelism.Value;
			if (this._prematureMergeLeft)
			{
				PartitionedStreamMerger<TLeftInput> partitionedStreamMerger = new PartitionedStreamMerger<TLeftInput>(false, ParallelMergeOptions.FullyBuffered, settings.TaskScheduler, this._leftChild.OutputOrdered, settings.CancellationState, settings.QueryId);
				queryResults.GivePartitionedStream(partitionedStreamMerger);
				queryResults = new ListQueryResults<TLeftInput>(partitionedStreamMerger.MergeExecutor.GetResultsAsArray(), value, preferStriping);
			}
			if (this._prematureMergeRight)
			{
				PartitionedStreamMerger<TRightInput> partitionedStreamMerger2 = new PartitionedStreamMerger<TRightInput>(false, ParallelMergeOptions.FullyBuffered, settings.TaskScheduler, this._rightChild.OutputOrdered, settings.CancellationState, settings.QueryId);
				queryResults2.GivePartitionedStream(partitionedStreamMerger2);
				queryResults2 = new ListQueryResults<TRightInput>(partitionedStreamMerger2.MergeExecutor.GetResultsAsArray(), value, preferStriping);
			}
			return new ZipQueryOperator<TLeftInput, TRightInput, TOutput>.ZipQueryOperatorResults(queryResults, queryResults2, this._resultSelector, value, preferStriping);
		}

		internal override IEnumerable<TOutput> AsSequentialQuery(CancellationToken token)
		{
			using (IEnumerator<TLeftInput> leftEnumerator = this._leftChild.AsSequentialQuery(token).GetEnumerator())
			{
				using (IEnumerator<TRightInput> rightEnumerator = this._rightChild.AsSequentialQuery(token).GetEnumerator())
				{
					while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
					{
						yield return this._resultSelector(leftEnumerator.Current, rightEnumerator.Current);
					}
				}
				IEnumerator<TRightInput> rightEnumerator = null;
			}
			IEnumerator<TLeftInput> leftEnumerator = null;
			yield break;
			yield break;
		}

		internal override OrdinalIndexState OrdinalIndexState
		{
			get
			{
				return OrdinalIndexState.Indexable;
			}
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return this._limitsParallelism;
			}
		}

		private readonly Func<TLeftInput, TRightInput, TOutput> _resultSelector;

		private readonly QueryOperator<TLeftInput> _leftChild;

		private readonly QueryOperator<TRightInput> _rightChild;

		private readonly bool _prematureMergeLeft;

		private readonly bool _prematureMergeRight;

		private readonly bool _limitsParallelism;

		internal class ZipQueryOperatorResults : QueryResults<TOutput>
		{
			internal ZipQueryOperatorResults(QueryResults<TLeftInput> leftChildResults, QueryResults<TRightInput> rightChildResults, Func<TLeftInput, TRightInput, TOutput> resultSelector, int partitionCount, bool preferStriping)
			{
				this._leftChildResults = leftChildResults;
				this._rightChildResults = rightChildResults;
				this._resultSelector = resultSelector;
				this._partitionCount = partitionCount;
				this._preferStriping = preferStriping;
				this._count = Math.Min(this._leftChildResults.Count, this._rightChildResults.Count);
			}

			internal override int ElementsCount
			{
				get
				{
					return this._count;
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
				return this._resultSelector(this._leftChildResults.GetElement(index), this._rightChildResults.GetElement(index));
			}

			internal override void GivePartitionedStream(IPartitionedStreamRecipient<TOutput> recipient)
			{
				PartitionedStream<TOutput, int> partitionedStream = ExchangeUtilities.PartitionDataSource<TOutput>(this, this._partitionCount, this._preferStriping);
				recipient.Receive<int>(partitionedStream);
			}

			private readonly QueryResults<TLeftInput> _leftChildResults;

			private readonly QueryResults<TRightInput> _rightChildResults;

			private readonly Func<TLeftInput, TRightInput, TOutput> _resultSelector;

			private readonly int _count;

			private readonly int _partitionCount;

			private readonly bool _preferStriping;
		}
	}
}
