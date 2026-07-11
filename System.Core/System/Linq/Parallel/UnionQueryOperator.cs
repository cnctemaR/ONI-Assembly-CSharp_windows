using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class UnionQueryOperator<TInputOutput> : BinaryQueryOperator<TInputOutput, TInputOutput, TInputOutput>
	{
		internal UnionQueryOperator(ParallelQuery<TInputOutput> left, ParallelQuery<TInputOutput> right, IEqualityComparer<TInputOutput> comparer)
			: base(left, right)
		{
			this._comparer = comparer;
			this._outputOrdered = base.LeftChild.OutputOrdered || base.RightChild.OutputOrdered;
		}

		internal override QueryResults<TInputOutput> Open(QuerySettings settings, bool preferStriping)
		{
			QueryResults<TInputOutput> queryResults = base.LeftChild.Open(settings, false);
			QueryResults<TInputOutput> queryResults2 = base.RightChild.Open(settings, false);
			return new BinaryQueryOperator<TInputOutput, TInputOutput, TInputOutput>.BinaryQueryOperatorResults(queryResults, queryResults2, this, settings, false);
		}

		public override void WrapPartitionedStream<TLeftKey, TRightKey>(PartitionedStream<TInputOutput, TLeftKey> leftStream, PartitionedStream<TInputOutput, TRightKey> rightStream, IPartitionedStreamRecipient<TInputOutput> outputRecipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = leftStream.PartitionCount;
			if (base.LeftChild.OutputOrdered)
			{
				PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> partitionedStream = ExchangeUtilities.HashRepartitionOrdered<TInputOutput, NoKeyMemoizationRequired, TLeftKey>(leftStream, null, null, this._comparer, settings.CancellationState.MergedCancellationToken);
				this.WrapPartitionedStreamFixedLeftType<TLeftKey, TRightKey>(partitionedStream, rightStream, outputRecipient, partitionCount, settings.CancellationState.MergedCancellationToken);
				return;
			}
			PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, int> partitionedStream2 = ExchangeUtilities.HashRepartition<TInputOutput, NoKeyMemoizationRequired, TLeftKey>(leftStream, null, null, this._comparer, settings.CancellationState.MergedCancellationToken);
			this.WrapPartitionedStreamFixedLeftType<int, TRightKey>(partitionedStream2, rightStream, outputRecipient, partitionCount, settings.CancellationState.MergedCancellationToken);
		}

		private void WrapPartitionedStreamFixedLeftType<TLeftKey, TRightKey>(PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> leftHashStream, PartitionedStream<TInputOutput, TRightKey> rightStream, IPartitionedStreamRecipient<TInputOutput> outputRecipient, int partitionCount, CancellationToken cancellationToken)
		{
			if (base.RightChild.OutputOrdered)
			{
				PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, TRightKey> partitionedStream = ExchangeUtilities.HashRepartitionOrdered<TInputOutput, NoKeyMemoizationRequired, TRightKey>(rightStream, null, null, this._comparer, cancellationToken);
				this.WrapPartitionedStreamFixedBothTypes<TLeftKey, TRightKey>(leftHashStream, partitionedStream, outputRecipient, partitionCount, cancellationToken);
				return;
			}
			PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, int> partitionedStream2 = ExchangeUtilities.HashRepartition<TInputOutput, NoKeyMemoizationRequired, TRightKey>(rightStream, null, null, this._comparer, cancellationToken);
			this.WrapPartitionedStreamFixedBothTypes<TLeftKey, int>(leftHashStream, partitionedStream2, outputRecipient, partitionCount, cancellationToken);
		}

		private void WrapPartitionedStreamFixedBothTypes<TLeftKey, TRightKey>(PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> leftHashStream, PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, TRightKey> rightHashStream, IPartitionedStreamRecipient<TInputOutput> outputRecipient, int partitionCount, CancellationToken cancellationToken)
		{
			if (base.LeftChild.OutputOrdered || base.RightChild.OutputOrdered)
			{
				IComparer<ConcatKey<TLeftKey, TRightKey>> comparer = ConcatKey<TLeftKey, TRightKey>.MakeComparer(leftHashStream.KeyComparer, rightHashStream.KeyComparer);
				PartitionedStream<TInputOutput, ConcatKey<TLeftKey, TRightKey>> partitionedStream = new PartitionedStream<TInputOutput, ConcatKey<TLeftKey, TRightKey>>(partitionCount, comparer, OrdinalIndexState.Shuffled);
				for (int i = 0; i < partitionCount; i++)
				{
					partitionedStream[i] = new UnionQueryOperator<TInputOutput>.OrderedUnionQueryOperatorEnumerator<TLeftKey, TRightKey>(leftHashStream[i], rightHashStream[i], base.LeftChild.OutputOrdered, base.RightChild.OutputOrdered, this._comparer, comparer, cancellationToken);
				}
				outputRecipient.Receive<ConcatKey<TLeftKey, TRightKey>>(partitionedStream);
				return;
			}
			PartitionedStream<TInputOutput, int> partitionedStream2 = new PartitionedStream<TInputOutput, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Shuffled);
			for (int j = 0; j < partitionCount; j++)
			{
				partitionedStream2[j] = new UnionQueryOperator<TInputOutput>.UnionQueryOperatorEnumerator<TLeftKey, TRightKey>(leftHashStream[j], rightHashStream[j], this._comparer, cancellationToken);
			}
			outputRecipient.Receive<int>(partitionedStream2);
		}

		internal override IEnumerable<TInputOutput> AsSequentialQuery(CancellationToken token)
		{
			IEnumerable<TInputOutput> enumerable = CancellableEnumerable.Wrap<TInputOutput>(base.LeftChild.AsSequentialQuery(token), token);
			IEnumerable<TInputOutput> enumerable2 = CancellableEnumerable.Wrap<TInputOutput>(base.RightChild.AsSequentialQuery(token), token);
			return enumerable.Union<TInputOutput>(enumerable2, this._comparer);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly IEqualityComparer<TInputOutput> _comparer;

		private class UnionQueryOperatorEnumerator<TLeftKey, TRightKey> : QueryOperatorEnumerator<TInputOutput, int>
		{
			internal UnionQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> leftSource, QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TRightKey> rightSource, IEqualityComparer<TInputOutput> comparer, CancellationToken cancellationToken)
			{
				this._leftSource = leftSource;
				this._rightSource = rightSource;
				this._comparer = comparer;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TInputOutput currentElement, ref int currentKey)
			{
				if (this._hashLookup == null)
				{
					this._hashLookup = new Set<TInputOutput>(this._comparer);
					this._outputLoopCount = new Shared<int>(0);
				}
				if (this._leftSource != null)
				{
					TLeftKey tleftKey = default(TLeftKey);
					Pair<TInputOutput, NoKeyMemoizationRequired> pair = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
					int num = 0;
					while (this._leftSource.MoveNext(ref pair, ref tleftKey))
					{
						if ((num++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						if (this._hashLookup.Add(pair.First))
						{
							currentElement = pair.First;
							return true;
						}
					}
					this._leftSource.Dispose();
					this._leftSource = null;
				}
				if (this._rightSource != null)
				{
					TRightKey trightKey = default(TRightKey);
					Pair<TInputOutput, NoKeyMemoizationRequired> pair2 = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
					while (this._rightSource.MoveNext(ref pair2, ref trightKey))
					{
						Shared<int> outputLoopCount = this._outputLoopCount;
						int value = outputLoopCount.Value;
						outputLoopCount.Value = value + 1;
						if ((value & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						if (this._hashLookup.Add(pair2.First))
						{
							currentElement = pair2.First;
							return true;
						}
					}
					this._rightSource.Dispose();
					this._rightSource = null;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				if (this._leftSource != null)
				{
					this._leftSource.Dispose();
				}
				if (this._rightSource != null)
				{
					this._rightSource.Dispose();
				}
			}

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> _leftSource;

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TRightKey> _rightSource;

			private Set<TInputOutput> _hashLookup;

			private CancellationToken _cancellationToken;

			private Shared<int> _outputLoopCount;

			private readonly IEqualityComparer<TInputOutput> _comparer;
		}

		private class OrderedUnionQueryOperatorEnumerator<TLeftKey, TRightKey> : QueryOperatorEnumerator<TInputOutput, ConcatKey<TLeftKey, TRightKey>>
		{
			internal OrderedUnionQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> leftSource, QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TRightKey> rightSource, bool leftOrdered, bool rightOrdered, IEqualityComparer<TInputOutput> comparer, IComparer<ConcatKey<TLeftKey, TRightKey>> keyComparer, CancellationToken cancellationToken)
			{
				this._leftSource = leftSource;
				this._rightSource = rightSource;
				this._keyComparer = keyComparer;
				this._leftOrdered = leftOrdered;
				this._rightOrdered = rightOrdered;
				this._comparer = comparer;
				if (this._comparer == null)
				{
					this._comparer = EqualityComparer<TInputOutput>.Default;
				}
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TInputOutput currentElement, ref ConcatKey<TLeftKey, TRightKey> currentKey)
			{
				if (this._outputEnumerator == null)
				{
					Dictionary<Wrapper<TInputOutput>, Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>>> dictionary = new Dictionary<Wrapper<TInputOutput>, Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>>>(new WrapperEqualityComparer<TInputOutput>(this._comparer));
					Pair<TInputOutput, NoKeyMemoizationRequired> pair = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
					TLeftKey tleftKey = default(TLeftKey);
					int num = 0;
					while (this._leftSource.MoveNext(ref pair, ref tleftKey))
					{
						if ((num++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						ConcatKey<TLeftKey, TRightKey> concatKey = ConcatKey<TLeftKey, TRightKey>.MakeLeft(this._leftOrdered ? tleftKey : default(TLeftKey));
						Wrapper<TInputOutput> wrapper = new Wrapper<TInputOutput>(pair.First);
						Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>> pair2;
						if (!dictionary.TryGetValue(wrapper, out pair2) || this._keyComparer.Compare(concatKey, pair2.Second) < 0)
						{
							dictionary[wrapper] = new Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>>(pair.First, concatKey);
						}
					}
					TRightKey trightKey = default(TRightKey);
					while (this._rightSource.MoveNext(ref pair, ref trightKey))
					{
						if ((num++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						ConcatKey<TLeftKey, TRightKey> concatKey2 = ConcatKey<TLeftKey, TRightKey>.MakeRight(this._rightOrdered ? trightKey : default(TRightKey));
						Wrapper<TInputOutput> wrapper2 = new Wrapper<TInputOutput>(pair.First);
						Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>> pair3;
						if (!dictionary.TryGetValue(wrapper2, out pair3) || this._keyComparer.Compare(concatKey2, pair3.Second) < 0)
						{
							dictionary[wrapper2] = new Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>>(pair.First, concatKey2);
						}
					}
					this._outputEnumerator = dictionary.GetEnumerator();
				}
				if (this._outputEnumerator.MoveNext())
				{
					KeyValuePair<Wrapper<TInputOutput>, Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>>> keyValuePair = this._outputEnumerator.Current;
					Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>> value = keyValuePair.Value;
					currentElement = value.First;
					currentKey = value.Second;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._leftSource.Dispose();
				this._rightSource.Dispose();
			}

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> _leftSource;

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TRightKey> _rightSource;

			private IComparer<ConcatKey<TLeftKey, TRightKey>> _keyComparer;

			private IEnumerator<KeyValuePair<Wrapper<TInputOutput>, Pair<TInputOutput, ConcatKey<TLeftKey, TRightKey>>>> _outputEnumerator;

			private bool _leftOrdered;

			private bool _rightOrdered;

			private IEqualityComparer<TInputOutput> _comparer;

			private CancellationToken _cancellationToken;
		}
	}
}
