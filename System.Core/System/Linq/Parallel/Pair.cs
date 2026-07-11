using System;

namespace System.Linq.Parallel
{
	internal struct Pair<T, U>
	{
		public Pair(T first, U second)
		{
			this._first = first;
			this._second = second;
		}

		public T First
		{
			get
			{
				return this._first;
			}
			set
			{
				this._first = value;
			}
		}

		public U Second
		{
			get
			{
				return this._second;
			}
			set
			{
				this._second = value;
			}
		}

		internal T _first;

		internal U _second;
	}
}
