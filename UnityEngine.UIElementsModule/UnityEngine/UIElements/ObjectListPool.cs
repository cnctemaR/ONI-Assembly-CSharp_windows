using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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

		private static ObjectPool<List<T>> pool = new ObjectPool<List<T>>(() => new List<T>(), 20);
	}
}
