using System;
using System.Collections.Generic;

public class Biome : List<ElementGradient>
{
	public Biome()
	{
	}

	public Biome(int size)
		: base(size)
	{
	}

	public Biome(IEnumerable<ElementGradient> collection)
		: base(collection)
	{
	}

	public List<float> ConvertBandSizeToMaxSize()
	{
		List<float> list = new List<float>();
		float num = 0f;
		for (int i = 0; i < this.Count; i++)
		{
			ElementGradient elementGradient = base[i];
			num += elementGradient.bandSize;
		}
		float num2 = 0f;
		for (int j = 0; j < this.Count; j++)
		{
			ElementGradient elementGradient2 = base[j];
			elementGradient2.maxValue = num2 + elementGradient2.bandSize / num;
			num2 = elementGradient2.maxValue;
			list.Add(elementGradient2.maxValue);
		}
		return list;
	}
}
