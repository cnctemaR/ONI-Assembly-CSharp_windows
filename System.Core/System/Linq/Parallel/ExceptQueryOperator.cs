using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class ExceptQueryOperator<TInputOutput> : BinaryQueryOperator<TInputOutput, TInputOutput, TInputOutput>
	{
		internal ExceptQueryOperator(ParallelQuery<TInputOutput> left, ParallelQuery<TInputOutput> right, IEqualityComparer<TInputOutput> comparer)
			: base(left, right)
		{
			this._comparer = comparer;
			this._outputOrdered = base.LeftChild.OutputOrdered;
			base.SetOrdinalIndex(OrdinalIndexState.Shuffled);
		}

		internal override QueryResults<TInputOutput> Open(QuerySettings settings, bool preferStriping)
		{
			QueryResults<TInputOutput> queryResults = base.LeftChild.Open(settings, false);
			QueryResults<TInputOutput> queryResults2 = base.RightChild.Open(settings, false);
			return new BinaryQueryOperator<TInputOutput, TInputOutput, TInputOutput>.BinaryQueryOperatorResults(queryResults, queryResults2, this, settings, false);
		}

		public override void WrapPartitionedStream<TLeftKey, TRightKey>(PartitionedStream<TInputOutput, TLeftKey> leftStream, PartitionedStream<TInputOutput, TRightKey> rightStream, IPartitionedStreamRecipient<TInputOutput> outputRecipient, bool preferStriping, QuerySettings settings)
		{
			if (base.OutputOrdered)
			{
				this.WrapPartitionedStreamHelper<TLeftKey, TRightKey>(ExchangeUtilities.HashRepartitionOrdered<TInputOutput, NoKeyMemoizationRequired, TLeftKey>(leftStream, null, null, this._comparer, settings.CancellationState.MergedCancellationToken), rightStream, outputRecipient, settings.CancellationState.MergedCancellationToken);
				return;
			}
			this.WrapPartitionedStreamHelper<int, TRightKey>(ExchangeUtilities.HashRepartition<TInputOutput, NoKeyMemoizationRequired, TLeftKey>(leftStream, null, null, this._comparer, settings.CancellationState.MergedCancellationToken), rightStream, outputRecipient, settings.CancellationState.MergedCancellationToken);
		}

		private void WrapPartitionedStreamHelper<TLeftKey, TRightKey>(PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> leftHashStream, PartitionedStream<TInputOutput, TRightKey> rightPartitionedStream, IPartitionedStreamRecipient<TInputOutput> outputRecipient, CancellationToken cancellationToken)
		{
			int partitionCount = leftHashStream.PartitionCount;
			PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, int> partitionedStream = ExchangeUtilities.HashRepartition<TInputOutput, NoKeyMemoizationRequired, TRightKey>(rightPartitionedStream, null, null, this._comparer, cancellationToken);
			PartitionedStream<TInputOutput, TLeftKey> partitionedStream2 = new PartitionedStream<TInputOutput, TLeftKey>(partitionCount, leftHashStream.KeyComparer, OrdinalIndexState.Shuffled);
			for (int i = 0; i < partitionCount; i++)
			{
				if (base.OutputOrdered)
				{
					partitionedStream2[i] = new ExceptQueryOperator<TInputOutput>.OrderedExceptQueryOperatorEnumerator<TLeftKey>(leftHashStream[i], partitionedStream[i], this._comparer, leftHashStream.KeyComparer, cancellationToken);
				}
				else
				{
					partitionedStream2[i] = (QueryOperatorEnumerator<TInputOutput, TLeftKey>)new ExceptQueryOperator<TInputOutput>.ExceptQueryOperatorEnumerator<TLeftKey>(leftHashStream[i], partitionedStream[i], this._comparer, cancellationToken);
				}
			}
			outputRecipient.Receive<TLeftKey>(partitionedStream2);
		}

		internal override IEnumerable<TInputOutput> AsSequentialQuery(CancellationToken token)
		{
			IEnumerable<TInputOutput> enumerable = CancellableEnumerable.Wrap<TInputOutput>(base.LeftChild.AsSequentialQuery(token), token);
			IEnumerable<TInputOutput> enumerable2 = CancellableEnumerable.Wrap<TInputOutput>(base.RightChild.AsSequentialQuery(token), token);
			return enumerable.Except<TInputOutput>(enumerable2, this._comparer);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly IEqualityComparer<TInputOutput> _comparer;

		private class ExceptQueryOperatorEnumerator<TLeftKey> : QueryOperatorEnumerator<TInputOutput, int>
		{
			internal ExceptQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> leftSource, QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, int> rightSource, IEqualityComparer<TInputOutput> comparer, CancellationToken cancellationToken)
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
					this._outputLoopCount = new Shared<int>(0);
					this._hashLookup = new Set<TInputOutput>(this._comparer);
					Pair<TInputOutput, NoKeyMemoizationRequired> pair = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
					int num = 0;
					int num2 = 0;
					while (this._rightSource.MoveNext(ref pair, ref num))
					{
						if ((num2++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						this._hashLookup.Add(pair.First);
					}
				}
				Pair<TInputOutput, NoKeyMemoizationRequired> pair2 = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
				TLeftKey tleftKey = default(TLeftKey);
				while (this._leftSource.MoveNext(ref pair2, ref tleftKey))
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
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._leftSource.Dispose();
				this._rightSource.Dispose();
			}

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> _leftSource;

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, int> _rightSource;

			private IEqualityComparer<TInputOutput> _comparer;

			private Set<TInputOutput> _hashLookup;

			private CancellationToken _cancellationToken;

			private Shared<int> _outputLoopCount;
		}

		private class OrderedExceptQueryOperatorEnumerator<TLeftKey> : QueryOperatorEnumerator<TInputOutput, TLeftKey>
		{
			internal OrderedExceptQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TLeftKey> leftSource, QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, int> rightSource, IEqualityComparer<TInputOutput> comparer, IComparer<TLeftKey> leftKeyComparer, CancellationToken cancellationToken)
			{
				this._leftSource = leftSource;
				this._rightSource = rightSource;
				this._comparer = comparer;
				this._leftKeyComparer = leftKeyComparer;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TInputOutput currentElement, ref TLeftKey currentKey)
			{
				if (this._outputEnumerator == null)
				{
					Set<TInputOutput> set = new Set<TInputOutput>(this._comparer);
					Pair<TInputOutput, NoKeyMemoizationRequired> pair = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
					int num = 0;
					int num2 = 0;
					while (this._rightSource.MoveNext(ref pair, ref num))
					{
						if ((num2++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						set.Add(pair.First);
					}
					Dictionary<Wrapper<TInputOutput>, Pair<TInputOutput, TLeftKey>> dictionary = new Dictionary<Wrapper<TInputOutput>, Pair<TInputOutput, TLeftKey>>(new WrapperEqualityComparer<TInputOutput>(this._comparer));
					Pair<TInputOutput, NoKeyMemoizationRequired> pair2 = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
					TLeftKey tleftKey = default(TLeftKey);
					while (this._leftSource.MoveNext(ref pair2, ref tleftKey))
					{
						if ((num2++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						if (!set.Contains(pair2.First))
						{
							Wrapper<TInputOutput> wrapper = new Wrapper<TInputOutput>(pair2.First);
							Pair<TInputOutput, TLeftKey> pair3;
							if (!dictionary.TryGetValue(wrapper, out pair3) || this._leftKeyComparer.Compare(tleftKey, pair3.Second) < 0)
							{
								dictionary[wrapper] = new Pair<TInputOutput, TLeftKey>(pair2.First, tleftKey);
							}
						}
					}
					this._outputEnumerator = dictionary.GetEnumerator();
				}
				if (this._outputEnumerator.MoveNext())
				{
					KeyValuePair<Wrapper<TInputOutput>, Pair<TInputOutput, TLeftKey>> keyValuePair = this._outputEnumerator.Current;
					Pair<TInputOutput, TLeftKey> value = keyValuePair.Value;
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

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, int> _rightSource;

			private IEqualityComparer<TInputOutput> _comparer;

			private IComparer<TLeftKey> _leftKeyComparer;

			private IEnumerator<KeyValuePair<Wrapper<TInputOutput>, Pair<TInputOutput, TLeftKey>>> _outputEnumerator;

			private CancellationToken _cancellationToken;
		}
	}
}
