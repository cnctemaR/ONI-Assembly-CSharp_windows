using System;
using System.Collections.Generic;

namespace ProcGen
{
	public static class WeightedRandom
	{
		public static T Choose<T>(List<T> list, SeededRandom rand) where T : IWeighted
		{
			T t;
			if (list.Count == 0)
			{
				t = default(T);
			}
			else
			{
				float num = 0f;
				for (int i = 0; i < list.Count; i++)
				{
					float num2 = num;
					T t2 = list[i];
					num = num2 + t2.weight;
				}
				float num3 = rand.RandomValue() * num;
				float num4 = 0f;
				for (int j = 0; j < list.Count; j++)
				{
					float num5 = num4;
					T t3 = list[j];
					num4 = num5 + t3.weight;
					if (num4 > num3)
					{
						return list[j];
					}
				}
				t = list[list.Count - 1];
			}
			return t;
		}
	}
}
