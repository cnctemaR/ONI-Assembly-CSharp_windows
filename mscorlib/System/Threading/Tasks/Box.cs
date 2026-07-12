using System;

namespace System.Threading.Tasks
{
	internal class Box<T>
	{
		internal Box(T value)
		{
			this.Value = value;
		}

		internal T Value;
	}
}
