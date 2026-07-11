using System;
using System.Collections.Generic;

namespace ProcGen
{
	public static class WeightedRandom
	{
		public static T Choose<T>(List<T> list, SeededRandom rand) where T : IWeighted
		{
			if (list.Count == 0)
			{
				return default(T);
			}
			float num = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				float num2 = num;
				T t = list[i];
				num = num2 + t.weight;
			}
			float num3 = rand.RandomValue() * num;
			float num4 = 0f;
			for (int j = 0; j < list.Count; j++)
			{
				float num5 = num4;
				T t = list[j];
				num4 = num5 + t.weight;
				if (num4 > num3)
				{
					return list[j];
				}
			}
			return list[list.Count - 1];
		}
	}
}
