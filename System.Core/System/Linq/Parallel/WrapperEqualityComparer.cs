using System;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal struct WrapperEqualityComparer<T> : IEqualityComparer<Wrapper<T>>
	{
		internal WrapperEqualityComparer(IEqualityComparer<T> comparer)
		{
			if (comparer == null)
			{
				this._comparer = EqualityComparer<T>.Default;
				return;
			}
			this._comparer = comparer;
		}

		public bool Equals(Wrapper<T> x, Wrapper<T> y)
		{
			return this._comparer.Equals(x.Value, y.Value);
		}

		public int GetHashCode(Wrapper<T> x)
		{
			return this._comparer.GetHashCode(x.Value);
		}

		private IEqualityComparer<T> _comparer;
	}
}
