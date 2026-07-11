using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class LastQueryOperator<TSource> : UnaryQueryOperator<TSource, TSource>
	{
		internal LastQueryOperator(IEnumerable<TSource> child, Func<TSource, bool> predicate)
			: base(child)
		{
			this._predicate = predicate;
			this._prematureMergeNeeded = base.Child.OrdinalIndexState.IsWorseThan(OrdinalIndexState.Increasing);
		}

		internal override QueryResults<TSource> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TSource, TSource>.UnaryQueryOperatorResults(base.Child.Open(settings, false), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TSource, TKey> inputStream, IPartitionedStreamRecipient<TSource> recipient, bool preferStriping, QuerySettings settings)
		{
			if (this._prematureMergeNeeded)
			{
				PartitionedStream<TSource, int> partitionedStream = QueryOperator<TSource>.ExecuteAndCollectResults<TKey>(inputStream, inputStream.PartitionCount, base.Child.OutputOrdered, preferStriping, settings).GetPartitionedStream();
				this.WrapHelper<int>(partitionedStream, recipient, settings);
				return;
			}
			this.WrapHelper<TKey>(inputStream, recipient, settings);
		}

		private void WrapHelper<TKey>(PartitionedStream<TSource, TKey> inputStream, IPartitionedStreamRecipient<TSource> recipient, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			LastQueryOperator<TSource>.LastQueryOperatorState<TKey> lastQueryOperatorState = new LastQueryOperator<TSource>.LastQueryOperatorState<TKey>();
			CountdownEvent countdownEvent = new CountdownEvent(partitionCount);
			PartitionedStream<TSource, int> partitionedStream = new PartitionedStream<TSource, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Shuffled);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new LastQueryOperator<TSource>.LastQueryOperatorEnumerator<TKey>(inputStream[i], this._predicate, lastQueryOperatorState, countdownEvent, settings.CancellationState.MergedCancellationToken, inputStream.KeyComparer, i);
			}
			recipient.Receive<int>(partitionedStream);
		}

		[ExcludeFromCodeCoverage]
		internal override IEnumerable<TSource> AsSequentialQuery(CancellationToken token)
		{
			throw new NotSupportedException();
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly Func<TSource, bool> _predicate;

		private readonly bool _prematureMergeNeeded;

		private class LastQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TSource, int>
		{
			internal LastQueryOperatorEnumerator(QueryOperatorEnumerator<TSource, TKey> source, Func<TSource, bool> predicate, LastQueryOperator<TSource>.LastQueryOperatorState<TKey> operatorState, CountdownEvent sharedBarrier, CancellationToken cancelToken, IComparer<TKey> keyComparer, int partitionId)
			{
				this._source = source;
				this._predicate = predicate;
				this._operatorState = operatorState;
				this._sharedBarrier = sharedBarrier;
				this._cancellationToken = cancelToken;
				this._keyComparer = keyComparer;
				this._partitionId = partitionId;
			}

			internal override bool MoveNext(ref TSource currentElement, ref int currentKey)
			{
				if (this._alreadySearched)
				{
					return false;
				}
				TSource tsource = default(TSource);
				TKey tkey = default(TKey);
				bool flag = false;
				try
				{
					int num = 0;
					TSource tsource2 = default(TSource);
					TKey tkey2 = default(TKey);
					while (this._source.MoveNext(ref tsource2, ref tkey2))
					{
						if ((num & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						if (this._predicate == null || this._predicate(tsource2))
						{
							tsource = tsource2;
							tkey = tkey2;
							flag = true;
						}
						num++;
					}
					if (flag)
					{
						LastQueryOperator<TSource>.LastQueryOperatorState<TKey> operatorState = this._operatorState;
						lock (operatorState)
						{
							if (this._operatorState._partitionId == -1 || this._keyComparer.Compare(tkey, this._operatorState._key) > 0)
							{
								this._operatorState._partitionId = this._partitionId;
								this._operatorState._key = tkey;
							}
						}
					}
				}
				finally
				{
					this._sharedBarrier.Signal();
				}
				this._alreadySearched = true;
				if (this._partitionId == this._operatorState._partitionId)
				{
					this._sharedBarrier.Wait(this._cancellationToken);
					if (this._operatorState._partitionId == this._partitionId)
					{
						currentElement = tsource;
						currentKey = 0;
						return true;
					}
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<TSource, TKey> _source;

			private Func<TSource, bool> _predicate;

			private bool _alreadySearched;

			private int _partitionId;

			private LastQueryOperator<TSource>.LastQueryOperatorState<TKey> _operatorState;

			private CountdownEvent _sharedBarrier;

			private CancellationToken _cancellationToken;

			private IComparer<TKey> _keyComparer;
		}

		private class LastQueryOperatorState<TKey>
		{
			internal TKey _key;

			internal int _partitionId = -1;
		}
	}
}
