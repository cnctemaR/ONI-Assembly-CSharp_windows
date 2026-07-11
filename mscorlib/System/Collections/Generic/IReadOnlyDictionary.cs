using System;

namespace System.Collections.Generic
{
	public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		bool ContainsKey(TKey key);

		bool TryGetValue(TKey key, out TValue value);

		TValue this[TKey key] { get; }

		IEnumerable<TKey> Keys { get; }

		IEnumerable<TValue> Values { get; }
	}
}
