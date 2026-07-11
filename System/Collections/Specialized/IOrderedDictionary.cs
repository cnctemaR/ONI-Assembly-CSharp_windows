using System;

namespace System.Collections.Specialized
{
	public interface IOrderedDictionary : IDictionary, ICollection, IEnumerable
	{
		IDictionaryEnumerator GetEnumerator();

		void Insert(int idx, object key, object value);

		void RemoveAt(int idx);

		object this[int idx] { get; set; }
	}
}
