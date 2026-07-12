using System;
using System.Collections;

namespace System.Data
{
	public abstract class EnumerableRowCollection : IEnumerable
	{
		internal abstract Type ElementType { get; }

		internal abstract DataTable Table { get; }

		internal EnumerableRowCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return null;
		}
	}
}
