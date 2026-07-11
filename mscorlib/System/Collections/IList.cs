using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface IList : ICollection, IEnumerable
	{
		object this[int index] { get; set; }

		int Add(object value);

		bool Contains(object value);

		void Clear();

		bool IsReadOnly { get; }

		bool IsFixedSize { get; }

		int IndexOf(object value);

		void Insert(int index, object value);

		void Remove(object value);

		void RemoveAt(int index);
	}
}
