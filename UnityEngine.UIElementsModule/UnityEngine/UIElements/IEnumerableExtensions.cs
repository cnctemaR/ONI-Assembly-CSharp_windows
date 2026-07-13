using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class IEnumerableExtensions
	{
		internal static bool HasValues(this IEnumerable<string> collection)
		{
			bool flag = collection == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				using (IEnumerator<string> enumerator = collection.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						return true;
					}
				}
				flag2 = false;
			}
			return flag2;
		}

		internal static bool NoElementOfTypeMatchesPredicate<T>(this IEnumerable collection, Func<T, bool> predicate)
		{
			foreach (object obj in collection)
			{
				bool flag;
				if (obj is T)
				{
					T t = (T)((object)obj);
					flag = predicate(t);
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static int GetCount(this IEnumerable collection)
		{
			int num = 0;
			foreach (object obj in collection)
			{
				num++;
			}
			return num;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static bool Any<T>(this List<T> source, Func<T, bool> predicate)
		{
			foreach (T t in source)
			{
				bool flag = predicate(t);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static HashSet<TResult> UniqueSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			HashSet<TResult> hashSet = new HashSet<TResult>();
			foreach (TSource tsource in source)
			{
				TResult tresult = selector(tsource);
				bool flag = tresult != null;
				if (flag)
				{
					hashSet.Add(selector(tsource));
				}
			}
			return hashSet;
		}
	}
}
