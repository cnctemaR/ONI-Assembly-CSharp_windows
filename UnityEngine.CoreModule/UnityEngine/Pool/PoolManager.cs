using System;
using System.Collections.Generic;

namespace UnityEngine.Pool
{
	internal static class PoolManager
	{
		public static void Reset()
		{
			for (int i = PoolManager.s_WeakPoolReferences.Count - 1; i >= 0; i--)
			{
				IPool pool;
				bool flag = PoolManager.s_WeakPoolReferences[i].TryGetTarget(out pool);
				if (flag)
				{
					pool.Clear();
				}
				else
				{
					PoolManager.s_WeakPoolReferences.RemoveAt(i);
				}
			}
		}

		public static void Register(IPool pool)
		{
			PoolManager.s_WeakPoolReferences.Add(new WeakReference<IPool>(pool));
		}

		private static readonly List<WeakReference<IPool>> s_WeakPoolReferences = new List<WeakReference<IPool>>();
	}
}
