using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Collections.Concurrent
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ConcurrentExtensions
	{
		public static void Clear<[Nullable(2)] T>(this ConcurrentBag<T> bag)
		{
			ThrowHelper.ThrowIfArgumentNull(bag, "bag", null);
			T t;
			while (bag.TryTake(out t))
			{
			}
		}

		public static void Clear<[Nullable(2)] T>(this ConcurrentQueue<T> queue)
		{
			ThrowHelper.ThrowIfArgumentNull(queue, "queue", null);
			T t;
			while (queue.TryDequeue(out t))
			{
			}
		}

		public static TValue AddOrUpdate<TKey, [Nullable(2)] TValue, [Nullable(2)] TArg>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, Func<TKey, TArg, TValue> addValueFactory, Func<TKey, TValue, TArg, TValue> updateValueFactory, TArg factoryArgument)
		{
			ThrowHelper.ThrowIfArgumentNull(dict, "dict", null);
			return dict.AddOrUpdate(key, ([Nullable(1)] TKey k) => addValueFactory(k, factoryArgument), ([Nullable(1)] TKey k, TValue v) => updateValueFactory(k, v, factoryArgument));
		}

		public static TValue GetOrAdd<TKey, [Nullable(2)] TValue, [Nullable(2)] TArg>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument)
		{
			ThrowHelper.ThrowIfArgumentNull(dict, "dict", null);
			return dict.GetOrAdd(key, ([Nullable(1)] TKey k) => valueFactory(k, factoryArgument));
		}

		public static bool TryRemove<TKey, [Nullable(2)] TValue>(this ConcurrentDictionary<TKey, TValue> dict, [Nullable(new byte[] { 0, 1, 1 })] KeyValuePair<TKey, TValue> item)
		{
			ThrowHelper.ThrowIfArgumentNull(dict, "dict", null);
			TValue value;
			if (!dict.TryRemove(item.Key, out value))
			{
				return false;
			}
			if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
			{
				return true;
			}
			dict.AddOrUpdate(item.Key, ([Nullable(1)] TKey _) => value, ([Nullable(1)] TKey _, TValue _) => value);
			return false;
		}
	}
}
