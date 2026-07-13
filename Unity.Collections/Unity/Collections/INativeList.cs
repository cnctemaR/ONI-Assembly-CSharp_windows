using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	public interface INativeList<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : IIndexable<T> where T : struct, ValueType
	{
		int Capacity { get; set; }

		bool IsEmpty { get; }

		T this[int index] { get; set; }

		void Clear();
	}
}
