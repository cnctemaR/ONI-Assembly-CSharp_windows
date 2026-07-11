using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class DistinctQueryOperator<TInputOutput> : UnaryQueryOperator<TInputOutput, TInputOutput>
	{
		internal DistinctQueryOperator(IEnumerable<TInputOutput> source, IEqualityComparer<TInputOutput> comparer)
			: base(source)
		{
			this._comparer = comparer;
			base.SetOrdinalIndexState(OrdinalIndexState.Shuffled);
		}

		internal override QueryResults<TInputOutput> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TInputOutput, TInputOutput>.UnaryQueryOperatorResults(base.Child.Open(settings, false), this, settings, false);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TInputOutput, TKey> inputStream, IPartitionedStreamRecipient<TInputOutput> recipient, bool preferStriping, QuerySettings settings)
		{
			if (base.OutputOrdered)
			{
				this.WrapPartitionedStreamHelper<TKey>(ExchangeUtilities.HashRepartitionOrdered<TInputOutput, NoKeyMemoizationRequired, TKey>(inputStream, null, null, this._comparer, settings.CancellationState.MergedCancellationToken), recipient, settings.CancellationState.MergedCancellationToken);
				return;
			}
			this.WrapPartitionedStreamHelper<int>(ExchangeUtilities.HashRepartition<TInputOutput, NoKeyMemoizationRequired, TKey>(inputStream, null, null, this._comparer, settings.CancellationState.MergedCancellationToken), recipient, settings.CancellationState.MergedCancellationToken);
		}

		private void WrapPartitionedStreamHelper<TKey>(PartitionedStream<Pair<TInputOutput, NoKeyMemoizationRequired>, TKey> hashStream, IPartitionedStreamRecipient<TInputOutput> recipient, CancellationToken cancellationToken)
		{
			int partitionCount = hashStream.PartitionCount;
			PartitionedStream<TInputOutput, TKey> partitionedStream = new PartitionedStream<TInputOutput, TKey>(partitionCount, hashStream.KeyComparer, OrdinalIndexState.Shuffled);
			for (int i = 0; i < partitionCount; i++)
			{
				if (base.OutputOrdered)
				{
					partitionedStream[i] = new DistinctQueryOperator<TInputOutput>.OrderedDistinctQueryOperatorEnumerator<TKey>(hashStream[i], this._comparer, hashStream.KeyComparer, cancellationToken);
				}
				else
				{
					partitionedStream[i] = (QueryOperatorEnumerator<TInputOutput, TKey>)new DistinctQueryOperator<TInputOutput>.DistinctQueryOperatorEnumerator<TKey>(hashStream[i], this._comparer, cancellationToken);
				}
			}
			recipient.Receive<TKey>(partitionedStream);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		internal override IEnumerable<TInputOutput> AsSequentialQuery(CancellationToken token)
		{
			return CancellableEnumerable.Wrap<TInputOutput>(base.Child.AsSequentialQuery(token), token).Distinct<TInputOutput>(this._comparer);
		}

		private readonly IEqualityComparer<TInputOutput> _comparer;

		private class DistinctQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TInputOutput, int>
		{
			internal DistinctQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TKey> source, IEqualityComparer<TInputOutput> comparer, CancellationToken cancellationToken)
			{
				this._source = source;
				this._hashLookup = new Set<TInputOutput>(comparer);
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TInputOutput currentElement, ref int currentKey)
			{
				TKey tkey = default(TKey);
				Pair<TInputOutput, NoKeyMemoizationRequired> pair = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
				if (this._outputLoopCount == null)
				{
					this._outputLoopCount = new Shared<int>(0);
				}
				while (this._source.MoveNext(ref pair, ref tkey))
				{
					Shared<int> outputLoopCount = this._outputLoopCount;
					int value = outputLoopCount.Value;
					outputLoopCount.Value = value + 1;
					if ((value & 63) == 0)
					{
						CancellationState.ThrowIfCanceled(this._cancellationToken);
					}
					if (this._hashLookup.Add(pair.First))
					{
						currentElement = pair.First;
						return true;
					}
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TKey> _source;

			private Set<TInputOutput> _hashLookup;

			private CancellationToken _cancellationToken;

			private Shared<int> _outputLoopCount;
		}

		private class OrderedDistinctQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TInputOutput, TKey>
		{
			internal OrderedDistinctQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TKey> source, IEqualityComparer<TInputOutput> comparer, IComparer<TKey> keyComparer, CancellationToken cancellationToken)
			{
				this._source = source;
				this._keyComparer = keyComparer;
				this._hashLookup = new Dictionary<Wrapper<TInputOutput>, TKey>(new WrapperEqualityComparer<TInputOutput>(comparer));
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TInputOutput currentElement, ref TKey currentKey)
			{
				if (this._hashLookupEnumerator == null)
				{
					Pair<TInputOutput, NoKeyMemoizationRequired> pair = default(Pair<TInputOutput, NoKeyMemoizationRequired>);
					TKey tkey = default(TKey);
					int num = 0;
					while (this._source.MoveNext(ref pair, ref tkey))
					{
						if ((num++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						Wrapper<TInputOutput> wrapper = new Wrapper<TInputOutput>(pair.First);
						TKey tkey2;
						if (!this._hashLookup.TryGetValue(wrapper, out tkey2) || this._keyComparer.Compare(tkey, tkey2) < 0)
						{
							this._hashLookup[wrapper] = tkey;
						}
					}
					this._hashLookupEnumerator = this._hashLookup.GetEnumerator();
				}
				if (this._hashLookupEnumerator.MoveNext())
				{
					KeyValuePair<Wrapper<TInputOutput>, TKey> keyValuePair = this._hashLookupEnumerator.Current;
					currentElement = keyValuePair.Key.Value;
					currentKey = keyValuePair.Value;
					return true;
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
				if (this._hashLookupEnumerator != null)
				{
					this._hashLookupEnumerator.Dispose();
				}
			}

			private QueryOperatorEnumerator<Pair<TInputOutput, NoKeyMemoizationRequired>, TKey> _source;

			private Dictionary<Wrapper<TInputOutput>, TKey> _hashLookup;

			private IComparer<TKey> _keyComparer;

			private IEnumerator<KeyValuePair<Wrapper<TInputOutput>, TKey>> _hashLookupEnumerator;

			private CancellationToken _cancellationToken;
		}
	}
}
