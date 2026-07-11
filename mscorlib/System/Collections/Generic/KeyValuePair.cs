using System;
using System.Text;

namespace System.Collections.Generic
{
	public static class KeyValuePair
	{
		public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
		{
			return new KeyValuePair<TKey, TValue>(key, value);
		}

		internal static string PairToString(object key, object value)
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			stringBuilder.Append('[');
			if (key != null)
			{
				stringBuilder.Append(key);
			}
			stringBuilder.Append(", ");
			if (value != null)
			{
				stringBuilder.Append(value);
			}
			stringBuilder.Append(']');
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}
	}
}
