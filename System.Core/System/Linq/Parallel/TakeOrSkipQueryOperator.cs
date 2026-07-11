using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class TakeOrSkipQueryOperator<TResult> : UnaryQueryOperator<TResult, TResult>
	{
		internal TakeOrSkipQueryOperator(IEnumerable<TResult> child, int count, bool take)
			: base(child)
		{
			this._count = count;
			this._take = take;
			base.SetOrdinalIndexState(this.OutputOrdinalIndexState());
		}

		private OrdinalIndexState OutputOrdinalIndexState()
		{
			OrdinalIndexState ordinalIndexState = base.Child.OrdinalIndexState;
			if (ordinalIndexState == OrdinalIndexState.Indexable)
			{
				return OrdinalIndexState.Indexable;
			}
			if (ordinalIndexState.IsWorseThan(OrdinalIndexState.Increasing))
			{
				this._prematureMerge = true;
				ordinalIndexState = OrdinalIndexState.Correct;
			}
			if (!this._take && ordinalIndexState == OrdinalIndexState.Correct)
			{
				ordinalIndexState = OrdinalIndexState.Increasing;
			}
			return ordinalIndexState;
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TResult, TKey> inputStream, IPartitionedStreamRecipient<TResult> recipient, bool preferStriping, QuerySettings settings)
		{
			if (this._prematureMerge)
			{
				PartitionedStream<TResult, int> partitionedStream = QueryOperator<TResult>.ExecuteAndCollectResults<TKey>(inputStream, inputStream.PartitionCount, base.Child.OutputOrdered, preferStriping, settings).GetPartitionedStream();
				this.WrapHelper<int>(partitionedStream, recipient, settings);
				return;
			}
			this.WrapHelper<TKey>(inputStream, recipient, settings);
		}

		private void WrapHelper<TKey>(PartitionedStream<TResult, TKey> inputStream, IPartitionedStreamRecipient<TResult> recipient, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			FixedMaxHeap<TKey> fixedMaxHeap = new FixedMaxHeap<TKey>(this._count, inputStream.KeyComparer);
			CountdownEvent countdownEvent = new CountdownEvent(partitionCount);
			PartitionedStream<TResult, TKey> partitionedStream = new PartitionedStream<TResult, TKey>(partitionCount, inputStream.KeyComparer, this.OrdinalIndexState);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new TakeOrSkipQueryOperator<TResult>.TakeOrSkipQueryOperatorEnumerator<TKey>(inputStream[i], this._take, fixedMaxHeap, countdownEvent, settings.CancellationState.MergedCancellationToken, inputStream.KeyComparer);
			}
			recipient.Receive<TKey>(partitionedStream);
		}

		internal override QueryResults<TResult> Open(QuerySettings settings, bool preferStriping)
		{
			return TakeOrSkipQueryOperator<TResult>.TakeOrSkipQueryOperatorResults.NewResults(base.Child.Open(settings, true), this, settings, preferStriping);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		internal override IEnumerable<TResult> AsSequentialQuery(CancellationToken token)
		{
			if (this._take)
			{
				return base.Child.AsSequentialQuery(token).Take<TResult>(this._count);
			}
			return CancellableEnumerable.Wrap<TResult>(base.Child.AsSequentialQuery(token), token).Skip<TResult>(this._count);
		}

		private readonly int _count;

		private readonly bool _take;

		private bool _prematureMerge;

		private class TakeOrSkipQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TResult, TKey>
		{
			internal TakeOrSkipQueryOperatorEnumerator(QueryOperatorEnumerator<TResult, TKey> source, bool take, FixedMaxHeap<TKey> sharedIndices, CountdownEvent sharedBarrier, CancellationToken cancellationToken, IComparer<TKey> keyComparer)
			{
				this._source = source;
				this._count = sharedIndices.Size;
				this._take = take;
				this._sharedIndices = sharedIndices;
				this._sharedBarrier = sharedBarrier;
				this._cancellationToken = cancellationToken;
				this._keyComparer = keyComparer;
			}

			internal override bool MoveNext(ref TResult currentElement, ref TKey currentKey)
			{
				if (this._buffer == null && this._count > 0)
				{
					List<Pair<TResult, TKey>> list = new List<Pair<TResult, TKey>>();
					TResult tresult = default(TResult);
					TKey tkey = default(TKey);
					int num = 0;
					while (list.Count < this._count && this._source.MoveNext(ref tresult, ref tkey))
					{
						if ((num++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						list.Add(new Pair<TResult, TKey>(tresult, tkey));
						FixedMaxHeap<TKey> sharedIndices = this._sharedIndices;
						lock (sharedIndices)
						{
							if (!this._sharedIndices.Insert(tkey))
							{
								break;
							}
						}
					}
					this._sharedBarrier.Signal();
					this._sharedBarrier.Wait(this._cancellationToken);
					this._buffer = list;
					this._bufferIndex = new Shared<int>(-1);
				}
				if (!this._take)
				{
					TKey tkey2 = default(TKey);
					if (this._count > 0)
					{
						if (this._sharedIndices.Count < this._count)
						{
							return false;
						}
						tkey2 = this._sharedIndices.MaxValue;
						if (this._bufferIndex.Value < this._buffer.Count - 1)
						{
							this._bufferIndex.Value++;
							while (this._bufferIndex.Value < this._buffer.Count)
							{
								if (this._keyComparer.Compare(this._buffer[this._bufferIndex.Value].Second, tkey2) > 0)
								{
									currentElement = this._buffer[this._bufferIndex.Value].First;
									currentKey = this._buffer[this._bufferIndex.Value].Second;
									return true;
								}
								this._bufferIndex.Value++;
							}
						}
					}
					return this._source.MoveNext(ref currentElement, ref currentKey);
				}
				if (this._count == 0 || this._bufferIndex.Value >= this._buffer.Count - 1)
				{
					return false;
				}
				this._bufferIndex.Value++;
				currentElement = this._buffer[this._bufferIndex.Value].First;
				currentKey = this._buffer[this._bufferIndex.Value].Second;
				return this._sharedIndices.Count == 0 || this._keyComparer.Compare(this._buffer[this._bufferIndex.Value].Second, this._sharedIndices.MaxValue) <= 0;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TResult, TKey> _source;

			private readonly int _count;

			private readonly bool _take;

			private readonly IComparer<TKey> _keyComparer;

			private readonly FixedMaxHeap<TKey> _sharedIndices;

			private readonly CountdownEvent _sharedBarrier;

			private readonly CancellationToken _cancellationToken;

			private List<Pair<TResult, TKey>> _buffer;

			private Shared<int> _bufferIndex;
		}

		private class TakeOrSkipQueryOperatorResults : UnaryQueryOperator<TResult, TResult>.UnaryQueryOperatorResults
		{
			public static QueryResults<TResult> NewResults(QueryResults<TResult> childQueryResults, TakeOrSkipQueryOperator<TResult> op, QuerySettings settings, bool preferStriping)
			{
				if (childQueryResults.IsIndexible)
				{
					return new TakeOrSkipQueryOperator<TResult>.TakeOrSkipQueryOperatorResults(childQueryResults, op, settings, preferStriping);
				}
				return new UnaryQueryOperator<TResult, TResult>.UnaryQueryOperatorResults(childQueryResults, op, settings, preferStriping);
			}

			private TakeOrSkipQueryOperatorResults(QueryResults<TResult> childQueryResults, TakeOrSkipQueryOperator<TResult> takeOrSkipOp, QuerySettings settings, bool preferStriping)
				: base(childQueryResults, takeOrSkipOp, settings, preferStriping)
			{
				this._takeOrSkipOp = takeOrSkipOp;
				this._childCount = this._childQueryResults.ElementsCount;
			}

			internal override bool IsIndexible
			{
				get
				{
					return this._childCount >= 0;
				}
			}

			internal override int ElementsCount
			{
				get
				{
					if (this._takeOrSkipOp._take)
					{
						return Math.Min(this._childCount, this._takeOrSkipOp._count);
					}
					return Math.Max(this._childCount - this._takeOrSkipOp._count, 0);
				}
			}

			internal override TResult GetElement(int index)
			{
				if (this._takeOrSkipOp._take)
				{
					return this._childQueryResults.GetElement(index);
				}
				return this._childQueryResults.GetElement(this._takeOrSkipOp._count + index);
			}

			private TakeOrSkipQueryOperator<TResult> _takeOrSkipOp;

			private int _childCount;
		}
	}
}
