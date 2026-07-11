using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Runtime.CompilerServices
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerStepThrough]
	public class RuleCache<T> where T : class
	{
		internal RuleCache()
		{
		}

		internal T[] GetRules()
		{
			return this._rules;
		}

		internal void MoveRule(T rule, int i)
		{
			object cacheLock = this._cacheLock;
			lock (cacheLock)
			{
				int num = this._rules.Length - i;
				if (num > 8)
				{
					num = 8;
				}
				int num2 = -1;
				int num3 = Math.Min(this._rules.Length, i + num);
				for (int j = i; j < num3; j++)
				{
					if (this._rules[j] == rule)
					{
						num2 = j;
						break;
					}
				}
				if (num2 >= 2)
				{
					T t = this._rules[num2];
					this._rules[num2] = this._rules[num2 - 1];
					this._rules[num2 - 1] = this._rules[num2 - 2];
					this._rules[num2 - 2] = t;
				}
			}
		}

		internal void AddRule(T newRule)
		{
			object cacheLock = this._cacheLock;
			lock (cacheLock)
			{
				this._rules = RuleCache<T>.AddOrInsert(this._rules, newRule);
			}
		}

		internal void ReplaceRule(T oldRule, T newRule)
		{
			object cacheLock = this._cacheLock;
			lock (cacheLock)
			{
				int num = Array.IndexOf<T>(this._rules, oldRule);
				if (num >= 0)
				{
					this._rules[num] = newRule;
				}
				else
				{
					this._rules = RuleCache<T>.AddOrInsert(this._rules, newRule);
				}
			}
		}

		private static T[] AddOrInsert(T[] rules, T item)
		{
			if (rules.Length < 64)
			{
				return rules.AddLast(item);
			}
			int num = rules.Length + 1;
			T[] array;
			if (num > 128)
			{
				num = 128;
				array = rules;
			}
			else
			{
				array = new T[num];
			}
			Array.Copy(rules, 0, array, 0, 64);
			array[64] = item;
			Array.Copy(rules, 64, array, 65, num - 64 - 1);
			return array;
		}

		private T[] _rules = Array.Empty<T>();

		private readonly object _cacheLock = new object();

		private const int MaxRules = 128;

		private const int InsertPosition = 64;
	}
}
