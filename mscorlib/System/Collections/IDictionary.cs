using System;

namespace System.Collections
{
	public interface IDictionary : ICollection, IEnumerable
	{
		object this[object key] { get; set; }

		ICollection Keys { get; }

		ICollection Values { get; }

		bool Contains(object key);

		void Add(object key, object value);

		void Clear();

		bool IsReadOnly { get; }

		bool IsFixedSize { get; }

		IDictionaryEnumerator GetEnumerator();

		void Remove(object key);
	}
}
