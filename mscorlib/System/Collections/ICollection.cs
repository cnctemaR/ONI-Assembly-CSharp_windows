using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface ICollection : IEnumerable
	{
		void CopyTo(Array array, int index);

		int Count { get; }

		object SyncRoot { get; }

		bool IsSynchronized { get; }
	}
}
