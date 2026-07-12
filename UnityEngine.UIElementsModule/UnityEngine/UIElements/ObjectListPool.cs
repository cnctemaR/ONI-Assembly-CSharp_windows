using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class ObjectListPool<T>
	{
		public static List<T> Get()
		{
			return ObjectListPool<T>.pool.Get();
		}

		public static void Release(List<T> elements)
		{
			elements.Clear();
			ObjectListPool<T>.pool.Release(elements);
		}

		private static ObjectPool<List<T>> pool = new ObjectPool<List<T>>(20);
	}
}
