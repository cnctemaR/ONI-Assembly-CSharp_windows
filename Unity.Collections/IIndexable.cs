using System;

namespace Unity.Collections
{
	public interface IIndexable<T> where T : struct
	{
		int Length { get; set; }

		ref T ElementAt(int index);
	}
}
