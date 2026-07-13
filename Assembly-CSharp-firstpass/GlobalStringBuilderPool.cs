using System;
using System.Text;
using UnityEngine.Pool;

public static class GlobalStringBuilderPool
{
	public static StringBuilder Alloc()
	{
		return GlobalStringBuilderPool.pool.Get();
	}

	public static void Free(StringBuilder sb)
	{
		if (sb != null)
		{
			sb.Clear();
		}
		GlobalStringBuilderPool.pool.Release(sb);
	}

	public static string ReturnAndFree(StringBuilder sb)
	{
		string text = sb.ToString();
		GlobalStringBuilderPool.Free(sb);
		return text;
	}

	private static ObjectPool<StringBuilder> pool = new ObjectPool<StringBuilder>(() => new StringBuilder(4096), null, null, null, false, 4, 16);
}
