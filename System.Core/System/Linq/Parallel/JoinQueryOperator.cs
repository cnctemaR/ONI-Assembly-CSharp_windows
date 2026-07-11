using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class JoinQueryOperator<TLeftInput, TRightInput, TKey, TOutput> : BinaryQueryOperator<TLeftInput, TRightInput, TOutput>
	{
		internal JoinQueryOperator(ParallelQuery<TLeftInput> left, ParallelQuery<TRightInput> right, Func<TLeftInput, TKey> leftKeySelector, Func<TRightInput, TKey> rightKeySelector, Func<TLeftInput, TRightInput, TOutput> resultSelector, IEqualityComparer<TKey> keyComparer)
			: base(left, right)
		{
			this._leftKeySelector = leftKeySelector;
			this._rightKeySelector = rightKeySelector;
			this._resultSelector = resultSelector;
			this._keyComparer = keyComparer;
			this._outputOrdered = base.LeftChild.OutputOrdered;
			base.SetOrdinalIndex(OrdinalIndexState.Shuffled);
		}

		public override void WrapPartitionedStream<TLeftKey, TRightKey>(PartitionedStream<TLeftInput, TLeftKey> leftStream, PartitionedStream<TRightInput, TRightKey> rightStream, IPartitionedStreamRecipient<TOutput> outputRecipient, bool preferStriping, QuerySettings settings)
		{
			if (base.LeftChild.OutputOrdered)
			{
				this.WrapPartitionedStreamHelper<TLeftKey, TRightKey>(ExchangeUtilities.HashRepartitionOrdered<TLeftInput, TKey, TLeftKey>(leftStream, this._leftKeySelector, this._keyComparer, null, settings.CancellationState.MergedCancellationToken), rightStream, outputRecipient, settings.CancellationState.MergedCancellationToken);
				return;
			}
			this.WrapPartitionedStreamHelper<int, TRightKey>(ExchangeUtilities.HashRepartition<TLeftInput, TKey, TLeftKey>(leftStream, this._leftKeySelector, this._keyComparer, null, settings.CancellationState.MergedCancellationToken), rightStream, outputRecipient, settings.CancellationState.MergedCancellationToken);
		}

		private void WrapPartitionedStreamHelper<TLeftKey, TRightKey>(PartitionedStream<Pair<TLeftInput, TKey>, TLeftKey> leftHashStream, PartitionedStream<TRightInput, TRightKey> rightPartitionedStream, IPartitionedStreamRecipient<TOutput> outputRecipient, CancellationToken cancellationToken)
		{
			int partitionCount = leftHashStream.PartitionCount;
			PartitionedStream<Pair<TRightInput, TKey>, int> partitionedStream = ExchangeUtilities.HashRepartition<TRightInput, TKey, TRightKey>(rightPartitionedStream, this._rightKeySelector, this._keyComparer, null, cancellationToken);
			PartitionedStream<TOutput, TLeftKey> partitionedStream2 = new PartitionedStream<TOutput, TLeftKey>(partitionCount, leftHashStream.KeyComparer, this.OrdinalIndexState);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream2[i] = new HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, TRightInput, TKey, TOutput>(leftHashStream[i], partitionedStream[i], this._resultSelector, null, this._keyComparer, cancellationToken);
			}
			outputRecipient.Receive<TLeftKey>(partitionedStream2);
		}

		internal override QueryResults<TOutput> Open(QuerySettings settings, bool preferStriping)
		{
			QueryResults<TLeftInput> queryResults = base.LeftChild.Open(settings, false);
			QueryResults<TRightInput> queryResults2 = base.RightChild.Open(settings, false);
			return new BinaryQueryOperator<TLeftInput, TRightInput, TOutput>.BinaryQueryOperatorResults(queryResults, queryResults2, this, settings, false);
		}

		internal override IEnumerable<TOutput> AsSequentialQuery(CancellationToken token)
		{
			IEnumerable<TLeftInput> enumerable = CancellableEnumerable.Wrap<TLeftInput>(base.LeftChild.AsSequentialQuery(token), token);
			IEnumerable<TRightInput> enumerable2 = CancellableEnumerable.Wrap<TRightInput>(base.RightChild.AsSequentialQuery(token), token);
			return enumerable.Join<TLeftInput, TRightInput, TKey, TOutput>(enumerable2, this._leftKeySelector, this._rightKeySelector, this._resultSelector, this._keyComparer);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly Func<TLeftInput, TKey> _leftKeySelector;

		private readonly Func<TRightInput, TKey> _rightKeySelector;

		private readonly Func<TLeftInput, TRightInput, TOutput> _resultSelector;

		private readonly IEqualityComparer<TKey> _keyComparer;
	}
}
