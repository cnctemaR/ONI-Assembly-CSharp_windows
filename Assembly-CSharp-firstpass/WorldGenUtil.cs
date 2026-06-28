using System;
using System.Collections.Generic;

public static class WorldGenUtil
{
	public static void ShuffleSeeded<T>(this IList<T> list, Random rng)
	{
		int i = list.Count;
		while (i > 1)
		{
			i--;
			int num = rng.Next(i + 1);
			T t = list[num];
			list[num] = list[i];
			list[i] = t;
		}
	}
}
