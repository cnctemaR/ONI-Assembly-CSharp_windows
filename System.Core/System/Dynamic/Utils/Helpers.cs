using System;
using System.Collections.Generic;

namespace System.Dynamic.Utils
{
	internal static class Helpers
	{
		internal static T CommonNode<T>(T first, T second, Func<T, T> parent) where T : class
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			if (@default.Equals(first, second))
			{
				return first;
			}
			HashSet<T> hashSet = new HashSet<T>(@default);
			for (T t = first; t != null; t = parent(t))
			{
				hashSet.Add(t);
			}
			for (T t2 = second; t2 != null; t2 = parent(t2))
			{
				if (hashSet.Contains(t2))
				{
					return t2;
				}
			}
			return default(T);
		}

		internal static void IncrementCount<T>(T key, Dictionary<T, int> dict)
		{
			int num;
			dict.TryGetValue(key, out num);
			dict[key] = num + 1;
		}
	}
}
