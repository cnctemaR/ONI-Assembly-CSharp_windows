using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class TakeOrSkipWhileQueryOperator<TResult> : UnaryQueryOperator<TResult, TResult>
	{
		internal TakeOrSkipWhileQueryOperator(IEnumerable<TResult> child, Func<TResult, bool> predicate, Func<TResult, int, bool> indexedPredicate, bool take)
			: base(child)
		{
			this._predicate = predicate;
			this._indexedPredicate = indexedPredicate;
			this._take = take;
			this.InitOrderIndexState();
		}

		private void InitOrderIndexState()
		{
			OrdinalIndexState ordinalIndexState = OrdinalIndexState.Increasing;
			OrdinalIndexState ordinalIndexState2 = base.Child.OrdinalIndexState;
			if (this._indexedPredicate != null)
			{
				ordinalIndexState = OrdinalIndexState.Correct;
				this._limitsParallelism = ordinalIndexState2 == OrdinalIndexState.Increasing;
			}
			OrdinalIndexState ordinalIndexState3 = ordinalIndexState2.Worse(OrdinalIndexState.Correct);
			if (ordinalIndexState3.IsWorseThan(ordinalIndexState))
			{
				this._prematureMerge = true;
			}
			if (!this._take)
			{
				ordinalIndexState3 = ordinalIndexState3.Worse(OrdinalIndexState.Increasing);
			}
			base.SetOrdinalIndexState(ordinalIndexState3);
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
			TakeOrSkipWhileQueryOperator<TResult>.OperatorState<TKey> operatorState = new TakeOrSkipWhileQueryOperator<TResult>.OperatorState<TKey>();
			CountdownEvent countdownEvent = new CountdownEvent(partitionCount);
			Func<TResult, TKey, bool> func = (Func<TResult, TKey, bool>)this._indexedPredicate;
			PartitionedStream<TResult, TKey> partitionedStream = new PartitionedStream<TResult, TKey>(partitionCount, inputStream.KeyComparer, this.OrdinalIndexState);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new TakeOrSkipWhileQueryOperator<TResult>.TakeOrSkipWhileQueryOperatorEnumerator<TKey>(inputStream[i], this._predicate, func, this._take, operatorState, countdownEvent, settings.CancellationState.MergedCancellationToken, inputStream.KeyComparer);
			}
			recipient.Receive<TKey>(partitionedStream);
		}

		internal override QueryResults<TResult> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TResult, TResult>.UnaryQueryOperatorResults(base.Child.Open(settings, true), this, settings, preferStriping);
		}

		internal override IEnumerable<TResult> AsSequentialQuery(CancellationToken token)
		{
			if (this._take)
			{
				if (this._indexedPredicate != null)
				{
					return base.Child.AsSequentialQuery(token).TakeWhile<TResult>(this._indexedPredicate);
				}
				return base.Child.AsSequentialQuery(token).TakeWhile<TResult>(this._predicate);
			}
			else
			{
				if (this._indexedPredicate != null)
				{
					return CancellableEnumerable.Wrap<TResult>(base.Child.AsSequentialQuery(token), token).SkipWhile<TResult>(this._indexedPredicate);
				}
				return CancellableEnumerable.Wrap<TResult>(base.Child.AsSequentialQuery(token), token).SkipWhile<TResult>(this._predicate);
			}
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return this._limitsParallelism;
			}
		}

		private Func<TResult, bool> _predicate;

		private Func<TResult, int, bool> _indexedPredicate;

		private readonly bool _take;

		private bool _prematureMerge;

		private bool _limitsParallelism;

		private class TakeOrSkipWhileQueryOperatorEnumerator<TKey> : QueryOperatorEnumerator<TResult, TKey>
		{
			internal TakeOrSkipWhileQueryOperatorEnumerator(QueryOperatorEnumerator<TResult, TKey> source, Func<TResult, bool> predicate, Func<TResult, TKey, bool> indexedPredicate, bool take, TakeOrSkipWhileQueryOperator<TResult>.OperatorState<TKey> operatorState, CountdownEvent sharedBarrier, CancellationToken cancelToken, IComparer<TKey> keyComparer)
			{
				this._source = source;
				this._predicate = predicate;
				this._indexedPredicate = indexedPredicate;
				this._take = take;
				this._operatorState = operatorState;
				this._sharedBarrier = sharedBarrier;
				this._cancellationToken = cancelToken;
				this._keyComparer = keyComparer;
			}

			internal override bool MoveNext(ref TResult currentElement, ref TKey currentKey)
			{
				if (this._buffer == null)
				{
					List<Pair<TResult, TKey>> list = new List<Pair<TResult, TKey>>();
					try
					{
						TResult tresult = default(TResult);
						TKey tkey = default(TKey);
						int num = 0;
						while (this._source.MoveNext(ref tresult, ref tkey))
						{
							if ((num++ & 63) == 0)
							{
								CancellationState.ThrowIfCanceled(this._cancellationToken);
							}
							list.Add(new Pair<TResult, TKey>(tresult, tkey));
							if (this._updatesSeen != this._operatorState._updatesDone)
							{
								TakeOrSkipWhileQueryOperator<TResult>.OperatorState<TKey> operatorState = this._operatorState;
								lock (operatorState)
								{
									this._currentLowKey = this._operatorState._currentLowKey;
									this._updatesSeen = this._operatorState._updatesDone;
								}
							}
							if (this._updatesSeen > 0 && this._keyComparer.Compare(tkey, this._currentLowKey) > 0)
							{
								break;
							}
							bool flag2;
							if (this._predicate != null)
							{
								flag2 = this._predicate(tresult);
							}
							else
							{
								flag2 = this._indexedPredicate(tresult, tkey);
							}
							if (!flag2)
							{
								TakeOrSkipWhileQueryOperator<TResult>.OperatorState<TKey> operatorState = this._operatorState;
								lock (operatorState)
								{
									if (this._operatorState._updatesDone == 0 || this._keyComparer.Compare(this._operatorState._currentLowKey, tkey) > 0)
									{
										this._currentLowKey = (this._operatorState._currentLowKey = tkey);
										TakeOrSkipWhileQueryOperator<TResult>.OperatorState<TKey> operatorState2 = this._operatorState;
										int num2 = operatorState2._updatesDone + 1;
										operatorState2._updatesDone = num2;
										this._updatesSeen = num2;
									}
									break;
								}
							}
						}
					}
					finally
					{
						this._sharedBarrier.Signal();
					}
					this._sharedBarrier.Wait(this._cancellationToken);
					this._buffer = list;
					this._bufferIndex = new Shared<int>(-1);
				}
				if (this._take)
				{
					if (this._bufferIndex.Value >= this._buffer.Count - 1)
					{
						return false;
					}
					this._bufferIndex.Value++;
					currentElement = this._buffer[this._bufferIndex.Value].First;
					currentKey = this._buffer[this._bufferIndex.Value].Second;
					return this._operatorState._updatesDone == 0 || this._keyComparer.Compare(this._operatorState._currentLowKey, currentKey) > 0;
				}
				else
				{
					if (this._operatorState._updatesDone == 0)
					{
						return false;
					}
					if (this._bufferIndex.Value < this._buffer.Count - 1)
					{
						this._bufferIndex.Value++;
						while (this._bufferIndex.Value < this._buffer.Count)
						{
							if (this._keyComparer.Compare(this._buffer[this._bufferIndex.Value].Second, this._operatorState._currentLowKey) >= 0)
							{
								currentElement = this._buffer[this._bufferIndex.Value].First;
								currentKey = this._buffer[this._bufferIndex.Value].Second;
								return true;
							}
							this._bufferIndex.Value++;
						}
					}
					return this._source.MoveNext(ref currentElement, ref currentKey);
				}
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TResult, TKey> _source;

			private readonly Func<TResult, bool> _predicate;

			private readonly Func<TResult, TKey, bool> _indexedPredicate;

			private readonly bool _take;

			private readonly IComparer<TKey> _keyComparer;

			private readonly TakeOrSkipWhileQueryOperator<TResult>.OperatorState<TKey> _operatorState;

			private readonly CountdownEvent _sharedBarrier;

			private readonly CancellationToken _cancellationToken;

			private List<Pair<TResult, TKey>> _buffer;

			private Shared<int> _bufferIndex;

			private int _updatesSeen;

			private TKey _currentLowKey;
		}

		private class OperatorState<TKey>
		{
			internal volatile int _updatesDone;

			internal TKey _currentLowKey;
		}
	}
}
