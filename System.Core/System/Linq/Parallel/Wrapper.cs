using System;

namespace System.Linq.Parallel
{
	internal struct Wrapper<T>
	{
		internal Wrapper(T value)
		{
			this.Value = value;
		}

		internal T Value;
	}
}
