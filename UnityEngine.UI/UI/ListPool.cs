using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.UI
{
	internal static class ListPool<T>
	{
		private static void Clear(List<T> l)
		{
			l.Clear();
		}

		public static List<T> Get()
		{
			return ListPool<T>.s_ListPool.Get();
		}

		public static void Release(List<T> toRelease)
		{
			ListPool<T>.s_ListPool.Release(toRelease);
		}

		private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, new UnityAction<List<T>>(ListPool<T>.Clear));
	}
}
