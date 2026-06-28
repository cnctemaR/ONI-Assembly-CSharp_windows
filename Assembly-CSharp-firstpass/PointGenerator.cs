using System;
using System.Collections.Generic;
using Delaunay.Geo;
using UnityEngine;

public static class PointGenerator
{
	public static List<Vector2> GetRandomPoints(Polygon boundingArea, float density, float avoidRadius, List<Vector2> avoidPoints, PointGenerator.SampleBehaviour behaviour, bool testInsideBounds, SeededRandom rnd, bool doShuffle = true, bool testAvoidPoints = true)
	{
		float num = boundingArea.bounds.width;
		float num2 = boundingArea.bounds.height;
		float num3 = num / 2f;
		float num4 = num2 / 2f;
		int num5 = (int)Mathf.Floor(num * num2 / density);
		uint num6 = (uint)Mathf.Sqrt((float)num5);
		int num7 = 10;
		uint num8 = (uint)((float)num5 * 0.98f);
		Vector2 min = boundingArea.bounds.min;
		Vector2 max = boundingArea.bounds.max;
		List<Vector2> list = new List<Vector2>();
		switch (behaviour)
		{
		case PointGenerator.SampleBehaviour.UniformSquare:
		{
			for (float num9 = -num4 + density; num9 < num4 - density; num9 += density)
			{
				for (float num10 = -num3 + density; num10 < num3 - density; num10 += density)
				{
					list.Add(boundingArea.Centroid() + new Vector2(num10, num9));
				}
			}
			goto IL_03BF;
		}
		case PointGenerator.SampleBehaviour.UniformHex:
		{
			for (uint num11 = 0U; num11 < num6; num11 += 1U)
			{
				for (uint num12 = 0U; num12 < num6; num12 += 1U)
				{
					list.Add(boundingArea.Centroid() + new Vector2(-num3 + (0.5f + num11) / num6 * num, -num4 + (0.25f + 0.5f * (num11 % 2U) + num12) / num6 * num2));
				}
			}
			goto IL_03BF;
		}
		case PointGenerator.SampleBehaviour.UniformSpiral:
		{
			for (uint num13 = 0U; num13 < num8; num13 += 1U)
			{
				double num14 = num13 / (32.0 * (double)density * 8.0);
				double num15 = Math.Sqrt(num14 * 512.0 * (double)density);
				double num16 = Math.Sqrt(num14);
				double num17 = Math.Sin(num15) * num16;
				double num18 = Math.Cos(num15) * num16;
				list.Add(boundingArea.bounds.center + new Vector2((float)num17 * boundingArea.bounds.width, (float)num18 * boundingArea.bounds.height));
			}
			goto IL_03BF;
		}
		case PointGenerator.SampleBehaviour.UniformCircle:
		{
			float num19 = 6.2831855f * avoidRadius;
			float num20 = num19 / density;
			float num21 = rnd.RandomValue();
			for (uint num22 = 1U; num22 < num20; num22 += 1U)
			{
				float num23 = num21 + num22 / num20 * 3.1415927f * 2f;
				double num24 = Math.Cos((double)num23) * (double)avoidRadius;
				double num25 = Math.Sin((double)num23) * (double)avoidRadius;
				list.Add(boundingArea.bounds.center + new Vector2((float)num24, (float)num25));
			}
			goto IL_03BF;
		}
		case PointGenerator.SampleBehaviour.PoissonDisk:
			list = new UniformPoissonDiskSampler(rnd).SampleRectangle(min, max, density, num7);
			goto IL_03BF;
		}
		for (float num26 = -num4 + avoidRadius * 0.3f + rnd.RandomValue() * 2f; num26 < num4 - (avoidRadius * 0.3f + rnd.RandomValue() * 2f); num26 += density + rnd.RandomValue())
		{
			for (float num27 = -num3 + avoidRadius * 0.3f + rnd.RandomValue() * 2f + rnd.RandomValue() * 2f; num27 < num3 - (avoidRadius * 0.3f + rnd.RandomValue() * 2f); num27 += density + rnd.RandomValue())
			{
				list.Add(boundingArea.Centroid() + new Vector2(num27, num26 + rnd.RandomValue() - 0.5f));
			}
		}
		IL_03BF:
		List<Vector2> list2 = new List<Vector2>();
		for (int i = 0; i < list.Count; i++)
		{
			if (!testInsideBounds || boundingArea.Contains(list[i]))
			{
				bool flag = false;
				if (testAvoidPoints && avoidPoints != null)
				{
					for (int j = 0; j < avoidPoints.Count; j++)
					{
						if (Mathf.Abs((avoidPoints[j] - list[i]).magnitude) < avoidRadius)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					list2.Add(list[i]);
				}
			}
		}
		if (doShuffle)
		{
			list2.ShuffleSeeded<Vector2>(rnd.RandomSource());
		}
		return list2;
	}

	public static List<Vector2> GetArchimedesSpiralPoints(int pointCount, Vector2 startPoint, double tetha, double alpha)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < pointCount; i++)
		{
			double num = tetha / (double)pointCount * (double)i;
			double num2 = alpha / (double)pointCount * (double)i;
			list.Add(new Vector2((float)((double)startPoint.x + num2 * Math.Cos(num)), (float)((double)startPoint.y + num2 * Math.Sin(num))));
		}
		return list;
	}

	public static List<Vector2> GetFilliedRectangle(Rect boundingArea, float density)
	{
		List<Vector2> list = new List<Vector2>();
		for (float num = boundingArea.xMin; num < boundingArea.xMax; num += density)
		{
			for (float num2 = boundingArea.yMin; num2 < boundingArea.yMax; num2 += density)
			{
				list.Add(new Vector2(num, num2));
			}
		}
		return list;
	}

	public static List<Vector2> GetSpaceFillingRandom(Rect boundingArea, float density, SeededRandom rnd)
	{
		List<Vector2> filliedRectangle = PointGenerator.GetFilliedRectangle(boundingArea, density);
		filliedRectangle.ShuffleSeeded<Vector2>(rnd.RandomSource());
		return filliedRectangle;
	}

	private static Vector2I PointOnRightHandSpiralOut(int index)
	{
		int num = (int)Mathf.Ceil(Mathf.Sqrt((float)(4 * index + 1)) * 0.5f - 1f + 0.5f);
		int num2 = (((num & 1) != 0) ? 0 : 1);
		int num3 = num * (num + 1);
		bool flag = num3 - index < num;
		int num4 = 2 * (num2 ^ ((!flag) ? 0 : 1)) - 1;
		Vector2I vector2I = new Vector2I(-num4, 2 * num2 - 1);
		Vector2I vector2I2 = new Vector2I(-((num2 != 0 || !flag) ? 0 : 1), 0) + vector2I * (num / 2);
		Vector2I vector2I3 = new Vector2I((!flag) ? 1 : 0, (!flag) ? 0 : 1) * num4;
		int num5 = index - num3 + 2 * num - ((!flag) ? 0 : 1) * num;
		return vector2I2 + vector2I3 * num5;
	}

	public static List<Vector2> GetSpaceFillingSpiral(Rect boundingArea, float density)
	{
		List<Vector2> list = new List<Vector2>();
		float num = boundingArea.width / density;
		float num2 = boundingArea.height / density;
		int num3 = 0;
		while ((float)num3 < num2 * num)
		{
			Vector2I vector2I = PointGenerator.PointOnRightHandSpiralOut(num3);
			list.Add(new Vector2(boundingArea.center.x + (float)vector2I.x, boundingArea.center.y + (float)vector2I.y - density));
			num3++;
		}
		return list;
	}

	public static List<Vector2> GetSpaceFillingSpiral(Polygon boundingArea, float density)
	{
		List<Vector2> list = new List<Vector2>();
		float num = boundingArea.bounds.width / density;
		float num2 = boundingArea.bounds.height / density;
		int num3 = 0;
		while ((float)num3 < num2 * num)
		{
			Vector2I vector2I = PointGenerator.PointOnRightHandSpiralOut(num3);
			Vector2 vector = new Vector2(boundingArea.bounds.center.x + (float)vector2I.x, boundingArea.bounds.center.y + (float)vector2I.y - density);
			if (boundingArea.Contains(vector))
			{
				list.Add(vector);
			}
			num3++;
		}
		return list;
	}

	[SerializeField]
	public enum SampleBehaviour
	{
		UniformSquare,
		UniformHex,
		UniformScaledHex,
		UniformSpiral,
		UniformCircle,
		PoissonDisk,
		StdRand
	}
}
