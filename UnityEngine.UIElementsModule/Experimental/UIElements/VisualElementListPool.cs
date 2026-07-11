using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualElementListPool
	{
		public static List<VisualElement> Copy(List<VisualElement> elements)
		{
			List<VisualElement> list = VisualElementListPool.pool.Get();
			list.AddRange(elements);
			return list;
		}

		public static List<VisualElement> Get(int initialCapacity = 0)
		{
			List<VisualElement> list = VisualElementListPool.pool.Get();
			if (initialCapacity > 0 && list.Capacity < initialCapacity)
			{
				list.Capacity = initialCapacity;
			}
			return list;
		}

		public static void Release(List<VisualElement> elements)
		{
			elements.Clear();
			VisualElementListPool.pool.Release(elements);
		}

		private static ObjectPool<List<VisualElement>> pool = new ObjectPool<List<VisualElement>>(20);
	}
}
