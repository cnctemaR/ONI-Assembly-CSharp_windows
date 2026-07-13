using System;
using System.Collections.Generic;
using FuzzySharp.Utils;

namespace FuzzySharp.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> MaxN<T>(this IEnumerable<T> source, int n) where T : IComparable<T>
		{
			MinHeap<T> queue = new MinHeap<T>(Comparer<T>.Create((T x, T y) => x.CompareTo(y)));
			foreach (T t in source)
			{
				if (queue.Count < n)
				{
					queue.Add(t);
				}
				else if (t.CompareTo(queue.GetMin()) > 0)
				{
					queue.ExtractDominating();
					queue.Add(t);
				}
			}
			int i = 0;
			while (i < n && queue.Count > 0)
			{
				yield return queue.ExtractDominating();
				int num = i;
				i = num + 1;
			}
			yield break;
		}

		public static IEnumerable<T> MaxNBy<T, TVal>(this IEnumerable<T> source, int n, Func<T, TVal> selector) where TVal : IComparable<TVal>
		{
			MinHeap<T> queue = new MinHeap<T>(Comparer<T>.Create(delegate(T x, T y)
			{
				TVal tval2 = selector(x);
				return tval2.CompareTo(selector(y));
			}));
			foreach (T t in source)
			{
				if (queue.Count < n)
				{
					queue.Add(t);
				}
				else
				{
					TVal tval = selector(t);
					if (tval.CompareTo(selector(queue.GetMin())) > 0)
					{
						queue.ExtractDominating();
						queue.Add(t);
					}
				}
			}
			int i = 0;
			while (i < n && queue.Count > 0)
			{
				yield return queue.ExtractDominating();
				int num = i;
				i = num + 1;
			}
			yield break;
		}
	}
}
