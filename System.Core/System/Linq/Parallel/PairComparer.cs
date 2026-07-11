using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal sealed class PairComparer<T, U> : IComparer<Pair<T, U>>
	{
		public PairComparer(IComparer<T> comparer1, IComparer<U> comparer2)
		{
			this._comparer1 = comparer1;
			this._comparer2 = comparer2;
		}

		public int Compare(Pair<T, U> x, Pair<T, U> y)
		{
			int num = this._comparer1.Compare(x.First, y.First);
			if (num != 0)
			{
				return num;
			}
			return this._comparer2.Compare(x.Second, y.Second);
		}

		private readonly IComparer<T> _comparer1;

		private readonly IComparer<U> _comparer2;
	}
}
