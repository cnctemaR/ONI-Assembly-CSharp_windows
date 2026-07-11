using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class SelectManyQueryOperator<TLeftInput, TRightInput, TOutput> : UnaryQueryOperator<TLeftInput, TOutput>
	{
		internal SelectManyQueryOperator(IEnumerable<TLeftInput> leftChild, Func<TLeftInput, IEnumerable<TRightInput>> rightChildSelector, Func<TLeftInput, int, IEnumerable<TRightInput>> indexedRightChildSelector, Func<TLeftInput, TRightInput, TOutput> resultSelector)
			: base(leftChild)
		{
			this._rightChildSelector = rightChildSelector;
			this._indexedRightChildSelector = indexedRightChildSelector;
			this._resultSelector = resultSelector;
			this._outputOrdered = base.Child.OutputOrdered || indexedRightChildSelector != null;
			this.InitOrderIndex();
		}

		private void InitOrderIndex()
		{
			OrdinalIndexState ordinalIndexState = base.Child.OrdinalIndexState;
			if (this._indexedRightChildSelector != null)
			{
				this._prematureMerge = ordinalIndexState.IsWorseThan(OrdinalIndexState.Correct);
				this._limitsParallelism = this._prematureMerge && ordinalIndexState != OrdinalIndexState.Shuffled;
			}
			else if (base.OutputOrdered)
			{
				this._prematureMerge = ordinalIndexState.IsWorseThan(OrdinalIndexState.Increasing);
			}
			base.SetOrdinalIndexState(OrdinalIndexState.Increasing);
		}

		internal override void WrapPartitionedStream<TLeftKey>(PartitionedStream<TLeftInput, TLeftKey> inputStream, IPartitionedStreamRecipient<TOutput> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			if (this._indexedRightChildSelector != null)
			{
				PartitionedStream<TLeftInput, int> partitionedStream;
				if (this._prematureMerge)
				{
					partitionedStream = QueryOperator<TLeftInput>.ExecuteAndCollectResults<TLeftKey>(inputStream, partitionCount, base.OutputOrdered, preferStriping, settings).GetPartitionedStream();
				}
				else
				{
					partitionedStream = (PartitionedStream<TLeftInput, int>)inputStream;
				}
				this.WrapPartitionedStreamIndexed(partitionedStream, recipient, settings);
				return;
			}
			if (this._prematureMerge)
			{
				PartitionedStream<TLeftInput, int> partitionedStream2 = QueryOperator<TLeftInput>.ExecuteAndCollectResults<TLeftKey>(inputStream, partitionCount, base.OutputOrdered, preferStriping, settings).GetPartitionedStream();
				this.WrapPartitionedStreamNotIndexed<int>(partitionedStream2, recipient, settings);
				return;
			}
			this.WrapPartitionedStreamNotIndexed<TLeftKey>(inputStream, recipient, settings);
		}

		private void WrapPartitionedStreamNotIndexed<TLeftKey>(PartitionedStream<TLeftInput, TLeftKey> inputStream, IPartitionedStreamRecipient<TOutput> recipient, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PairComparer<TLeftKey, int> pairComparer = new PairComparer<TLeftKey, int>(inputStream.KeyComparer, Util.GetDefaultComparer<int>());
			PartitionedStream<TOutput, Pair<TLeftKey, int>> partitionedStream = new PartitionedStream<TOutput, Pair<TLeftKey, int>>(partitionCount, pairComparer, this.OrdinalIndexState);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new SelectManyQueryOperator<TLeftInput, TRightInput, TOutput>.SelectManyQueryOperatorEnumerator<TLeftKey>(inputStream[i], this, settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<Pair<TLeftKey, int>>(partitionedStream);
		}

		private void WrapPartitionedStreamIndexed(PartitionedStream<TLeftInput, int> inputStream, IPartitionedStreamRecipient<TOutput> recipient, QuerySettings settings)
		{
			PairComparer<int, int> pairComparer = new PairComparer<int, int>(inputStream.KeyComparer, Util.GetDefaultComparer<int>());
			PartitionedStream<TOutput, Pair<int, int>> partitionedStream = new PartitionedStream<TOutput, Pair<int, int>>(inputStream.PartitionCount, pairComparer, this.OrdinalIndexState);
			for (int i = 0; i < inputStream.PartitionCount; i++)
			{
				partitionedStream[i] = new SelectManyQueryOperator<TLeftInput, TRightInput, TOutput>.IndexedSelectManyQueryOperatorEnumerator(inputStream[i], this, settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<Pair<int, int>>(partitionedStream);
		}

		internal override QueryResults<TOutput> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TLeftInput, TOutput>.UnaryQueryOperatorResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override IEnumerable<TOutput> AsSequentialQuery(CancellationToken token)
		{
			if (this._rightChildSelector != null)
			{
				if (this._resultSelector != null)
				{
					return CancellableEnumerable.Wrap<TLeftInput>(base.Child.AsSequentialQuery(token), token).SelectMany<TLeftInput, TRightInput, TOutput>(this._rightChildSelector, this._resultSelector);
				}
				return (IEnumerable<TOutput>)CancellableEnumerable.Wrap<TLeftInput>(base.Child.AsSequentialQuery(token), token).SelectMany<TLeftInput, TRightInput>(this._rightChildSelector);
			}
			else
			{
				if (this._resultSelector != null)
				{
					return CancellableEnumerable.Wrap<TLeftInput>(base.Child.AsSequentialQuery(token), token).SelectMany<TLeftInput, TRightInput, TOutput>(this._indexedRightChildSelector, this._resultSelector);
				}
				return (IEnumerable<TOutput>)CancellableEnumerable.Wrap<TLeftInput>(base.Child.AsSequentialQuery(token), token).SelectMany<TLeftInput, TRightInput>(this._indexedRightChildSelector);
			}
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return this._limitsParallelism;
			}
		}

		private readonly Func<TLeftInput, IEnumerable<TRightInput>> _rightChildSelector;

		private readonly Func<TLeftInput, int, IEnumerable<TRightInput>> _indexedRightChildSelector;

		private readonly Func<TLeftInput, TRightInput, TOutput> _resultSelector;

		private bool _prematureMerge;

		private bool _limitsParallelism;

		private class IndexedSelectManyQueryOperatorEnumerator : QueryOperatorEnumerator<TOutput, Pair<int, int>>
		{
			internal IndexedSelectManyQueryOperatorEnumerator(QueryOperatorEnumerator<TLeftInput, int> leftSource, SelectManyQueryOperator<TLeftInput, TRightInput, TOutput> selectManyOperator, CancellationToken cancellationToken)
			{
				this._leftSource = leftSource;
				this._selectManyOperator = selectManyOperator;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TOutput currentElement, ref Pair<int, int> currentKey)
			{
				for (;;)
				{
					if (this._currentRightSource == null)
					{
						this._mutables = new SelectManyQueryOperator<TLeftInput, TRightInput, TOutput>.IndexedSelectManyQueryOperatorEnumerator.Mutables();
						SelectManyQueryOperator<TLeftInput, TRightInput, TOutput>.IndexedSelectManyQueryOperatorEnumerator.Mutables mutables = this._mutables;
						int lhsCount = mutables._lhsCount;
						mutables._lhsCount = lhsCount + 1;
						if ((lhsCount & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						if (!this._leftSource.MoveNext(ref this._mutables._currentLeftElement, ref this._mutables._currentLeftSourceIndex))
						{
							break;
						}
						IEnumerable<TRightInput> enumerable = this._selectManyOperator._indexedRightChildSelector(this._mutables._currentLeftElement, this._mutables._currentLeftSourceIndex);
						this._currentRightSource = enumerable.GetEnumerator();
						if (this._selectManyOperator._resultSelector == null)
						{
							this._currentRightSourceAsOutput = (IEnumerator<TOutput>)this._currentRightSource;
						}
					}
					if (this._currentRightSource.MoveNext())
					{
						goto Block_4;
					}
					this._currentRightSource.Dispose();
					this._currentRightSource = null;
					this._currentRightSourceAsOutput = null;
				}
				return false;
				Block_4:
				this._mutables._currentRightSourceIndex++;
				if (this._selectManyOperator._resultSelector != null)
				{
					currentElement = this._selectManyOperator._resultSelector(this._mutables._currentLeftElement, this._currentRightSource.Current);
				}
				else
				{
					currentElement = this._currentRightSourceAsOutput.Current;
				}
				currentKey = new Pair<int, int>(this._mutables._currentLeftSourceIndex, this._mutables._currentRightSourceIndex);
				return true;
			}

			protected override void Dispose(bool disposing)
			{
				this._leftSource.Dispose();
				if (this._currentRightSource != null)
				{
					this._currentRightSource.Dispose();
				}
			}

			private readonly QueryOperatorEnumerator<TLeftInput, int> _leftSource;

			private readonly SelectManyQueryOperator<TLeftInput, TRightInput, TOutput> _selectManyOperator;

			private IEnumerator<TRightInput> _currentRightSource;

			private IEnumerator<TOutput> _currentRightSourceAsOutput;

			private SelectManyQueryOperator<TLeftInput, TRightInput, TOutput>.IndexedSelectManyQueryOperatorEnumerator.Mutables _mutables;

			private readonly CancellationToken _cancellationToken;

			private class Mutables
			{
				internal int _currentRightSourceIndex = -1;

				internal TLeftInput _currentLeftElement;

				internal int _currentLeftSourceIndex;

				internal int _lhsCount;
			}
		}

		private class SelectManyQueryOperatorEnumerator<TLeftKey> : QueryOperatorEnumerator<TOutput, Pair<TLeftKey, int>>
		{
			internal SelectManyQueryOperatorEnumerator(QueryOperatorEnumerator<TLeftInput, TLeftKey> leftSource, SelectManyQueryOperator<TLeftInput, TRightInput, TOutput> selectManyOperator, CancellationToken cancellationToken)
			{
				this._leftSource = leftSource;
				this._selectManyOperator = selectManyOperator;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TOutput currentElement, ref Pair<TLeftKey, int> currentKey)
			{
				for (;;)
				{
					if (this._currentRightSource == null)
					{
						this._mutables = new SelectManyQueryOperator<TLeftInput, TRightInput, TOutput>.SelectManyQueryOperatorEnumerator<TLeftKey>.Mutables();
						SelectManyQueryOperator<TLeftInput, TRightInput, TOutput>.SelectManyQueryOperatorEnumerator<TLeftKey>.Mutables mutables = this._mutables;
						int lhsCount = mutables._lhsCount;
						mutables._lhsCount = lhsCount + 1;
						if ((lhsCount & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(this._cancellationToken);
						}
						if (!this._leftSource.MoveNext(ref this._mutables._currentLeftElement, ref this._mutables._currentLeftKey))
						{
							break;
						}
						IEnumerable<TRightInput> enumerable = this._selectManyOperator._rightChildSelector(this._mutables._currentLeftElement);
						this._currentRightSource = enumerable.GetEnumerator();
						if (this._selectManyOperator._resultSelector == null)
						{
							this._currentRightSourceAsOutput = (IEnumerator<TOutput>)this._currentRightSource;
						}
					}
					if (this._currentRightSource.MoveNext())
					{
						goto Block_4;
					}
					this._currentRightSource.Dispose();
					this._currentRightSource = null;
					this._currentRightSourceAsOutput = null;
				}
				return false;
				Block_4:
				this._mutables._currentRightSourceIndex++;
				if (this._selectManyOperator._resultSelector != null)
				{
					currentElement = this._selectManyOperator._resultSelector(this._mutables._currentLeftElement, this._currentRightSource.Current);
				}
				else
				{
					currentElement = this._currentRightSourceAsOutput.Current;
				}
				currentKey = new Pair<TLeftKey, int>(this._mutables._currentLeftKey, this._mutables._currentRightSourceIndex);
				return true;
			}

			protected override void Dispose(bool disposing)
			{
				this._leftSource.Dispose();
				if (this._currentRightSource != null)
				{
					this._currentRightSource.Dispose();
				}
			}

			private readonly QueryOperatorEnumerator<TLeftInput, TLeftKey> _leftSource;

			private readonly SelectManyQueryOperator<TLeftInput, TRightInput, TOutput> _selectManyOperator;

			private IEnumerator<TRightInput> _currentRightSource;

			private IEnumerator<TOutput> _currentRightSourceAsOutput;

			private SelectManyQueryOperator<TLeftInput, TRightInput, TOutput>.SelectManyQueryOperatorEnumerator<TLeftKey>.Mutables _mutables;

			private readonly CancellationToken _cancellationToken;

			private class Mutables
			{
				internal int _currentRightSourceIndex = -1;

				internal TLeftInput _currentLeftElement;

				internal TLeftKey _currentLeftKey;

				internal int _lhsCount;
			}
		}
	}
}
