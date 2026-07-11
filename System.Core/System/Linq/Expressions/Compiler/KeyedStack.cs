using System;
using System.Collections.Generic;

namespace System.Linq.Expressions.Compiler
{
	internal sealed class KeyedStack<TKey, TValue> where TValue : class
	{
		internal void Push(TKey key, TValue value)
		{
			Stack<TValue> stack;
			if (!this._data.TryGetValue(key, out stack))
			{
				this._data.Add(key, stack = new Stack<TValue>());
			}
			stack.Push(value);
		}

		internal TValue TryPop(TKey key)
		{
			Stack<TValue> stack;
			TValue tvalue;
			if (!this._data.TryGetValue(key, out stack) || !stack.TryPop(out tvalue))
			{
				return default(TValue);
			}
			return tvalue;
		}

		private readonly Dictionary<TKey, Stack<TValue>> _data = new Dictionary<TKey, Stack<TValue>>();
	}
}
