using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, TRightInput, THashKey, TOutput> : QueryOperatorEnumerator<TOutput, TLeftKey>
	{
		internal HashJoinQueryOperatorEnumerator(QueryOperatorEnumerator<Pair<TLeftInput, THashKey>, TLeftKey> leftSource, QueryOperatorEnumerator<Pair<TRightInput, THashKey>, int> rightSource, Func<TLeftInput, TRightInput, TOutput> singleResultSelector, Func<TLeftInput, IEnumerable<TRightInput>, TOutput> groupResultSelector, IEqualityComparer<THashKey> keyComparer, CancellationToken cancellationToken)
		{
			this._leftSource = leftSource;
			this._rightSource = rightSource;
			this._singleResultSelector = singleResultSelector;
			this._groupResultSelector = groupResultSelector;
			this._keyComparer = keyComparer;
			this._cancellationToken = cancellationToken;
		}

		internal override bool MoveNext(ref TOutput currentElement, ref TLeftKey currentKey)
		{
			HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, TRightInput, THashKey, TOutput>.Mutables mutables = this._mutables;
			if (mutables == null)
			{
				mutables = (this._mutables = new HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, TRightInput, THashKey, TOutput>.Mutables());
				mutables._rightHashLookup = new HashLookup<THashKey, Pair<TRightInput, ListChunk<TRightInput>>>(this._keyComparer);
				Pair<TRightInput, THashKey> pair = default(Pair<TRightInput, THashKey>);
				int num = 0;
				int num2 = 0;
				while (this._rightSource.MoveNext(ref pair, ref num))
				{
					if ((num2++ & 63) == 0)
					{
						CancellationState.ThrowIfCanceled(this._cancellationToken);
					}
					TRightInput first = pair.First;
					THashKey second = pair.Second;
					if (second != null)
					{
						Pair<TRightInput, ListChunk<TRightInput>> pair2 = default(Pair<TRightInput, ListChunk<TRightInput>>);
						if (!mutables._rightHashLookup.TryGetValue(second, ref pair2))
						{
							pair2 = new Pair<TRightInput, ListChunk<TRightInput>>(first, null);
							if (this._groupResultSelector != null)
							{
								pair2.Second = new ListChunk<TRightInput>(2);
								pair2.Second.Add(first);
							}
							mutables._rightHashLookup.Add(second, pair2);
						}
						else
						{
							if (pair2.Second == null)
							{
								pair2.Second = new ListChunk<TRightInput>(2);
								mutables._rightHashLookup[second] = pair2;
							}
							pair2.Second.Add(first);
						}
					}
				}
			}
			ListChunk<TRightInput> currentRightMatches = mutables._currentRightMatches;
			if (currentRightMatches != null && mutables._currentRightMatchesIndex == currentRightMatches.Count)
			{
				ListChunk<TRightInput> listChunk = (mutables._currentRightMatches = currentRightMatches.Next);
				mutables._currentRightMatchesIndex = 0;
			}
			if (mutables._currentRightMatches == null)
			{
				Pair<TLeftInput, THashKey> pair3 = default(Pair<TLeftInput, THashKey>);
				TLeftKey tleftKey = default(TLeftKey);
				while (this._leftSource.MoveNext(ref pair3, ref tleftKey))
				{
					HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, TRightInput, THashKey, TOutput>.Mutables mutables2 = mutables;
					int outputLoopCount = mutables2._outputLoopCount;
					mutables2._outputLoopCount = outputLoopCount + 1;
					if ((outputLoopCount & 63) == 0)
					{
						CancellationState.ThrowIfCanceled(this._cancellationToken);
					}
					Pair<TRightInput, ListChunk<TRightInput>> pair4 = default(Pair<TRightInput, ListChunk<TRightInput>>);
					TLeftInput first2 = pair3.First;
					THashKey second2 = pair3.Second;
					if (second2 != null && mutables._rightHashLookup.TryGetValue(second2, ref pair4) && this._singleResultSelector != null)
					{
						mutables._currentRightMatches = pair4.Second;
						mutables._currentRightMatchesIndex = 0;
						currentElement = this._singleResultSelector(first2, pair4.First);
						currentKey = tleftKey;
						if (pair4.Second != null)
						{
							mutables._currentLeft = first2;
							mutables._currentLeftKey = tleftKey;
						}
						return true;
					}
					if (this._groupResultSelector != null)
					{
						IEnumerable<TRightInput> enumerable = pair4.Second;
						if (enumerable == null)
						{
							enumerable = ParallelEnumerable.Empty<TRightInput>();
						}
						currentElement = this._groupResultSelector(first2, enumerable);
						currentKey = tleftKey;
						return true;
					}
				}
				return false;
			}
			currentElement = this._singleResultSelector(mutables._currentLeft, mutables._currentRightMatches._chunk[mutables._currentRightMatchesIndex]);
			currentKey = mutables._currentLeftKey;
			mutables._currentRightMatchesIndex++;
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			this._leftSource.Dispose();
			this._rightSource.Dispose();
		}

		private readonly QueryOperatorEnumerator<Pair<TLeftInput, THashKey>, TLeftKey> _leftSource;

		private readonly QueryOperatorEnumerator<Pair<TRightInput, THashKey>, int> _rightSource;

		private readonly Func<TLeftInput, TRightInput, TOutput> _singleResultSelector;

		private readonly Func<TLeftInput, IEnumerable<TRightInput>, TOutput> _groupResultSelector;

		private readonly IEqualityComparer<THashKey> _keyComparer;

		private readonly CancellationToken _cancellationToken;

		private HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, TRightInput, THashKey, TOutput>.Mutables _mutables;

		private class Mutables
		{
			internal TLeftInput _currentLeft;

			internal TLeftKey _currentLeftKey;

			internal HashLookup<THashKey, Pair<TRightInput, ListChunk<TRightInput>>> _rightHashLookup;

			internal ListChunk<TRightInput> _currentRightMatches;

			internal int _currentRightMatchesIndex;

			internal int _outputLoopCount;
		}
	}
}
