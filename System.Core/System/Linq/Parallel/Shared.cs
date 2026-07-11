using System;

namespace System.Linq.Parallel
{
	internal class Shared<T>
	{
		internal Shared(T value)
		{
			this.Value = value;
		}

		internal T Value;
	}
}
