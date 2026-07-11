using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class DefaultIfEmptyQueryOperator<TSource> : UnaryQueryOperator<TSource, TSource>
	{
		internal DefaultIfEmptyQueryOperator(IEnumerable<TSource> child, TSource defaultValue)
			: base(child)
		{
			this._defaultValue = defaultValue;
			base.SetOrdinalIndexState(base.Child.OrdinalIndexState.Worse(OrdinalIndexState.Correct));
		}

		internal override QueryResults<TSource> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TSource, TSource>.UnaryQueryOperatorResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TSource, TKey> inputStream, IPartitionedStreamRecipient<TSource> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			Shared<int> shared = new Shared<int>(0);
			CountdownEvent countdownEvent = new CountdownEvent(partitionCount - 1);
			PartitionedStream<TSource, TKey> partitionedStream = new PartitionedStream<TSource, TKey>(partitionCount, inputStream.KeyComparer, this.OrdinalIndexState);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new DefaultIfEmptyQueryOperator<TSource>.DefaultIfEmptyQueryOperatorEnumerator<TKey>(inputStream[i], this._defaultValue, i, partitionCount, shared, countdownEvent, settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<TKey>(partitionedStream);
		}

		internal override IEnumerable<TSource> AsSequentialQuery(CancellationToken token)
		{
			return base.Child.AsSequentialQuery(token).DefaultIfEmpty(this._defaultValue);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly TSource _defaultValue;

		private class DefaultIfEmptyQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TSource, TKey>
		{
			internal DefaultIfEmptyQueryOperatorEnumerator(QueryOperatorEnumerator<TSource, TKey> source, TSource defaultValue, int partitionIndex, int partitionCount, Shared<int> sharedEmptyCount, CountdownEvent sharedLatch, CancellationToken cancelToken)
			{
				this._source = source;
				this._defaultValue = defaultValue;
				this._partitionIndex = partitionIndex;
				this._partitionCount = partitionCount;
				this._sharedEmptyCount = sharedEmptyCount;
				this._sharedLatch = sharedLatch;
				this._cancelToken = cancelToken;
			}

			internal override bool MoveNext(ref TSource currentElement, ref TKey currentKey)
			{
				bool flag = this._source.MoveNext(ref currentElement, ref currentKey);
				if (!this._lookedForEmpty)
				{
					this._lookedForEmpty = true;
					if (!flag)
					{
						if (this._partitionIndex == 0)
						{
							this._sharedLatch.Wait(this._cancelToken);
							this._sharedLatch.Dispose();
							if (this._sharedEmptyCount.Value == this._partitionCount - 1)
							{
								currentElement = this._defaultValue;
								currentKey = default(TKey);
								return true;
							}
							return false;
						}
						else
						{
							Interlocked.Increment(ref this._sharedEmptyCount.Value);
						}
					}
					if (this._partitionIndex != 0)
					{
						this._sharedLatch.Signal();
					}
				}
				return flag;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private QueryOperatorEnumerator<TSource, TKey> _source;

			private bool _lookedForEmpty;

			private int _partitionIndex;

			private int _partitionCount;

			private TSource _defaultValue;

			private Shared<int> _sharedEmptyCount;

			private CountdownEvent _sharedLatch;

			private CancellationToken _cancelToken;
		}
	}
}
