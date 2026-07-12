using System;
using System.Collections.Generic;

namespace System.Data
{
	internal class SortExpressionBuilder<T> : IComparer<List<object>>
	{
		internal void Add(Func<T, object> keySelector, Comparison<object> compare, bool isOrderBy)
		{
			if (isOrderBy)
			{
				this._currentSelector = this._selectors.AddFirst(keySelector);
				this._currentComparer = this._comparers.AddFirst(compare);
				return;
			}
			this._currentSelector = this._selectors.AddAfter(this._currentSelector, keySelector);
			this._currentComparer = this._comparers.AddAfter(this._currentComparer, compare);
		}

		public List<object> Select(T row)
		{
			List<object> list = new List<object>();
			foreach (Func<T, object> func in this._selectors)
			{
				list.Add(func(row));
			}
			return list;
		}

		public int Compare(List<object> a, List<object> b)
		{
			int num = 0;
			foreach (Comparison<object> comparison in this._comparers)
			{
				int num2 = comparison(a[num], b[num]);
				if (num2 != 0)
				{
					return num2;
				}
				num++;
			}
			return 0;
		}

		internal int Count
		{
			get
			{
				return this._selectors.Count;
			}
		}

		internal SortExpressionBuilder<T> Clone()
		{
			SortExpressionBuilder<T> sortExpressionBuilder = new SortExpressionBuilder<T>();
			foreach (Func<T, object> func in this._selectors)
			{
				if (func == this._currentSelector.Value)
				{
					sortExpressionBuilder._currentSelector = sortExpressionBuilder._selectors.AddLast(func);
				}
				else
				{
					sortExpressionBuilder._selectors.AddLast(func);
				}
			}
			foreach (Comparison<object> comparison in this._comparers)
			{
				if (comparison == this._currentComparer.Value)
				{
					sortExpressionBuilder._currentComparer = sortExpressionBuilder._comparers.AddLast(comparison);
				}
				else
				{
					sortExpressionBuilder._comparers.AddLast(comparison);
				}
			}
			return sortExpressionBuilder;
		}

		internal SortExpressionBuilder<TResult> CloneCast<TResult>()
		{
			SortExpressionBuilder<TResult> sortExpressionBuilder = new SortExpressionBuilder<TResult>();
			using (LinkedList<Func<T, object>>.Enumerator enumerator = this._selectors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Func<T, object> selector = enumerator.Current;
					if (selector == this._currentSelector.Value)
					{
						sortExpressionBuilder._currentSelector = sortExpressionBuilder._selectors.AddLast((TResult r) => selector((T)((object)r)));
					}
					else
					{
						sortExpressionBuilder._selectors.AddLast((TResult r) => selector((T)((object)r)));
					}
				}
			}
			foreach (Comparison<object> comparison in this._comparers)
			{
				if (comparison == this._currentComparer.Value)
				{
					sortExpressionBuilder._currentComparer = sortExpressionBuilder._comparers.AddLast(comparison);
				}
				else
				{
					sortExpressionBuilder._comparers.AddLast(comparison);
				}
			}
			return sortExpressionBuilder;
		}

		private LinkedList<Func<T, object>> _selectors = new LinkedList<Func<T, object>>();

		private LinkedList<Comparison<object>> _comparers = new LinkedList<Comparison<object>>();

		private LinkedListNode<Func<T, object>> _currentSelector;

		private LinkedListNode<Comparison<object>> _currentComparer;
	}
}
