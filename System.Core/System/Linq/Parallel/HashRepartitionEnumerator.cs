using System;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class HashRepartitionEnumerator<TInputOutput, THashKey, TIgnoreKey> : QueryOperatorEnumerator<Pair<TInputOutput, THashKey>, int>
	{
		internal HashRepartitionEnumerator(QueryOperatorEnumerator<TInputOutput, TIgnoreKey> source, int partitionCount, int partitionIndex, Func<TInputOutput, THashKey> keySelector, HashRepartitionStream<TInputOutput, THashKey, int> repartitionStream, CountdownEvent barrier, ListChunk<Pair<TInputOutput, THashKey>>[][] valueExchangeMatrix, CancellationToken cancellationToken)
		{
			this._source = source;
			this._partitionCount = partitionCount;
			this._partitionIndex = partitionIndex;
			this._keySelector = keySelector;
			this._repartitionStream = repartitionStream;
			this._barrier = barrier;
			this._valueExchangeMatrix = valueExchangeMatrix;
			this._cancellationToken = cancellationToken;
		}

		internal override bool MoveNext(ref Pair<TInputOutput, THashKey> currentElement, ref int currentKey)
		{
			if (this._partitionCount != 1)
			{
				HashRepartitionEnumerator<TInputOutput, THashKey, TIgnoreKey>.Mutables mutables = this._mutables;
				if (mutables == null)
				{
					mutables = (this._mutables = new HashRepartitionEnumerator<TInputOutput, THashKey, TIgnoreKey>.Mutables());
				}
				if (mutables._currentBufferIndex == -1)
				{
					this.EnumerateAndRedistributeElements();
				}
				while (mutables._currentBufferIndex < this._partitionCount)
				{
					if (mutables._currentBuffer != null)
					{
						HashRepartitionEnumerator<TInputOutput, THashKey, TIgnoreKey>.Mutables mutables2 = mutables;
						int num = mutables2._currentIndex + 1;
						mutables2._currentIndex = num;
						if (num < mutables._currentBuffer.Count)
						{
							currentElement = mutables._currentBuffer._chunk[mutables._currentIndex];
							return true;
						}
						mutables._currentIndex = -1;
						mutables._currentBuffer = mutables._currentBuffer.Next;
					}
					else
					{
						if (mutables._currentBufferIndex == this._partitionIndex)
						{
							this._barrier.Wait(this._cancellationToken);
							mutables._currentBufferIndex = -1;
						}
						mutables._currentBufferIndex++;
						mutables._currentIndex = -1;
						if (mutables._currentBufferIndex == this._partitionIndex)
						{
							mutables._currentBufferIndex++;
						}
						if (mutables._currentBufferIndex < this._partitionCount)
						{
							mutables._currentBuffer = this._valueExchangeMatrix[mutables._currentBufferIndex][this._partitionIndex];
						}
					}
				}
				return false;
			}
			TIgnoreKey tignoreKey = default(TIgnoreKey);
			TInputOutput tinputOutput = default(TInputOutput);
			if (this._source.MoveNext(ref tinputOutput, ref tignoreKey))
			{
				currentElement = new Pair<TInputOutput, THashKey>(tinputOutput, (this._keySelector == null) ? default(THashKey) : this._keySelector(tinputOutput));
				return true;
			}
			return false;
		}

		private void EnumerateAndRedistributeElements()
		{
			HashRepartitionEnumerator<TInputOutput, THashKey, TIgnoreKey>.Mutables mutables = this._mutables;
			ListChunk<Pair<TInputOutput, THashKey>>[] array = new ListChunk<Pair<TInputOutput, THashKey>>[this._partitionCount];
			TInputOutput tinputOutput = default(TInputOutput);
			TIgnoreKey tignoreKey = default(TIgnoreKey);
			int num = 0;
			while (this._source.MoveNext(ref tinputOutput, ref tignoreKey))
			{
				if ((num++ & 63) == 0)
				{
					CancellationState.ThrowIfCanceled(this._cancellationToken);
				}
				THashKey thashKey = default(THashKey);
				int num2;
				if (this._keySelector != null)
				{
					thashKey = this._keySelector(tinputOutput);
					num2 = this._repartitionStream.GetHashCode(thashKey) % this._partitionCount;
				}
				else
				{
					num2 = this._repartitionStream.GetHashCode(tinputOutput) % this._partitionCount;
				}
				ListChunk<Pair<TInputOutput, THashKey>> listChunk = array[num2];
				if (listChunk == null)
				{
					listChunk = (array[num2] = new ListChunk<Pair<TInputOutput, THashKey>>(128));
				}
				listChunk.Add(new Pair<TInputOutput, THashKey>(tinputOutput, thashKey));
			}
			for (int i = 0; i < this._partitionCount; i++)
			{
				this._valueExchangeMatrix[this._partitionIndex][i] = array[i];
			}
			this._barrier.Signal();
			mutables._currentBufferIndex = this._partitionIndex;
			mutables._currentBuffer = array[this._partitionIndex];
			mutables._currentIndex = -1;
		}

		protected override void Dispose(bool disposed)
		{
			if (this._barrier != null)
			{
				if (this._mutables == null || this._mutables._currentBufferIndex == -1)
				{
					this._barrier.Signal();
					this._barrier = null;
				}
				this._source.Dispose();
			}
		}

		private const int ENUMERATION_NOT_STARTED = -1;

		private readonly int _partitionCount;

		private readonly int _partitionIndex;

		private readonly Func<TInputOutput, THashKey> _keySelector;

		private readonly HashRepartitionStream<TInputOutput, THashKey, int> _repartitionStream;

		private readonly ListChunk<Pair<TInputOutput, THashKey>>[][] _valueExchangeMatrix;

		private readonly QueryOperatorEnumerator<TInputOutput, TIgnoreKey> _source;

		private CountdownEvent _barrier;

		private readonly CancellationToken _cancellationToken;

		private HashRepartitionEnumerator<TInputOutput, THashKey, TIgnoreKey>.Mutables _mutables;

		private class Mutables
		{
			internal Mutables()
			{
				this._currentBufferIndex = -1;
			}

			internal int _currentBufferIndex;

			internal ListChunk<Pair<TInputOutput, THashKey>> _currentBuffer;

			internal int _currentIndex;
		}
	}
}
