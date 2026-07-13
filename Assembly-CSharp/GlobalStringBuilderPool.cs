using System;
using System.Text;

public static class GlobalStringBuilderPool
{
	public static StringBuilder Alloc()
	{
		return GlobalStringBuilderPool.pool.GetInstance();
	}

	public static void Free(StringBuilder sb)
	{
		if (sb != null)
		{
			sb.Clear();
		}
		GlobalStringBuilderPool.pool.ReleaseInstance(sb);
	}

	public static string ReturnAndFree(StringBuilder sb)
	{
		string text = sb.ToString();
		GlobalStringBuilderPool.Free(sb);
		return text;
	}

	private static ObjectPool<StringBuilder> pool = new ObjectPool<StringBuilder>(() => new StringBuilder(4096), 4);
}
