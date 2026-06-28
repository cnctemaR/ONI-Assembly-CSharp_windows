using System;

namespace System
{
	public interface IComparable<T>
	{
		int CompareTo(T other);
	}
}
