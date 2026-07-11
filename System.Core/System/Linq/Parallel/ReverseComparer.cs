using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class ReverseComparer<T> : IComparer<T>
	{
		internal ReverseComparer(IComparer<T> comparer)
		{
			this._comparer = comparer;
		}

		public int Compare(T x, T y)
		{
			return this._comparer.Compare(y, x);
		}

		private IComparer<T> _comparer;
	}
}
