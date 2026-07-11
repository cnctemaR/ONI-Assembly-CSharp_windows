using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal struct ConcatKey<TLeftKey, TRightKey>
	{
		private ConcatKey(TLeftKey leftKey, TRightKey rightKey, bool isLeft)
		{
			this._leftKey = leftKey;
			this._rightKey = rightKey;
			this._isLeft = isLeft;
		}

		internal static ConcatKey<TLeftKey, TRightKey> MakeLeft(TLeftKey leftKey)
		{
			return new ConcatKey<TLeftKey, TRightKey>(leftKey, default(TRightKey), true);
		}

		internal static ConcatKey<TLeftKey, TRightKey> MakeRight(TRightKey rightKey)
		{
			return new ConcatKey<TLeftKey, TRightKey>(default(TLeftKey), rightKey, false);
		}

		internal static IComparer<ConcatKey<TLeftKey, TRightKey>> MakeComparer(IComparer<TLeftKey> leftComparer, IComparer<TRightKey> rightComparer)
		{
			return new ConcatKey<TLeftKey, TRightKey>.ConcatKeyComparer(leftComparer, rightComparer);
		}

		private readonly TLeftKey _leftKey;

		private readonly TRightKey _rightKey;

		private readonly bool _isLeft;

		private class ConcatKeyComparer : IComparer<ConcatKey<TLeftKey, TRightKey>>
		{
			internal ConcatKeyComparer(IComparer<TLeftKey> leftComparer, IComparer<TRightKey> rightComparer)
			{
				this._leftComparer = leftComparer;
				this._rightComparer = rightComparer;
			}

			public int Compare(ConcatKey<TLeftKey, TRightKey> x, ConcatKey<TLeftKey, TRightKey> y)
			{
				if (x._isLeft != y._isLeft)
				{
					if (!x._isLeft)
					{
						return 1;
					}
					return -1;
				}
				else
				{
					if (x._isLeft)
					{
						return this._leftComparer.Compare(x._leftKey, y._leftKey);
					}
					return this._rightComparer.Compare(x._rightKey, y._rightKey);
				}
			}

			private IComparer<TLeftKey> _leftComparer;

			private IComparer<TRightKey> _rightComparer;
		}
	}
}
