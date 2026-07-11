using System;
using System.Collections.Generic;
using System.Linq;

namespace Harmony
{
	public static class CollectionExtensions
	{
		public static void Do<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			bool flag = sequence == null;
			if (!flag)
			{
				foreach (T t in sequence)
				{
					action(t);
				}
			}
		}

		public static void DoIf<T>(this IEnumerable<T> sequence, Func<T, bool> condition, Action<T> action)
		{
			sequence.Where<T>(condition).Do<T>(action);
		}

		public static IEnumerable<T> Add<T>(this IEnumerable<T> sequence, T item)
		{
			return (sequence ?? Enumerable.Empty<T>()).Concat<T>(new T[] { item });
		}

		public static T[] AddRangeToArray<T>(this T[] sequence, T[] items)
		{
			return (sequence ?? Enumerable.Empty<T>()).Concat<T>(items).ToArray<T>();
		}

		public static T[] AddToArray<T>(this T[] sequence, T item)
		{
			return sequence.Add(item).ToArray<T>();
		}
	}
}
