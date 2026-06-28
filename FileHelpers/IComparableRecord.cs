using System;

namespace FileHelpers
{
	public interface IComparableRecord<T>
	{
		bool IsEqualRecord(T other);
	}
}
