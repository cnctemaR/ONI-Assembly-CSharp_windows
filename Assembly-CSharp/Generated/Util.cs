using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;

namespace Generated
{
	public static class Util
	{
		public static HashSet<Vector2> GetPointsOnHermiteCurve(Vector2 p0, Vector2 p1, Vector2 t0, Vector2 t1, int numberOfPoints)
		{
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			Vector2 vector = t0 - p0;
			Vector2 vector2 = t1 - p1;
			float num = 1f / (float)numberOfPoints;
			for (int i = 0; i < numberOfPoints; i++)
			{
				float num2 = (float)i * num;
				Vector2 vector3 = (2f * num2 * num2 * num2 - 3f * num2 * num2 + 1f) * p0 + (num2 * num2 * num2 - 2f * num2 * num2 + num2) * vector + (-2f * num2 * num2 * num2 + 3f * num2 * num2) * p1 + (num2 * num2 * num2 - num2 * num2) * vector2;
				hashSet.Add(vector3);
			}
			return hashSet;
		}

		public static HashSet<Vector2> GetPointsOnCatmullRomSpline(List<Vector2> controlPoints, int numberOfPoints)
		{
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			float num = 1f / (float)numberOfPoints;
			for (int i = 0; i < controlPoints.Count - 1; i++)
			{
				Vector2 vector = controlPoints[i];
				Vector2 vector2 = controlPoints[i + 1];
				Vector2 vector3;
				if (i > 0)
				{
					vector3 = 0.5f * (controlPoints[i + 1] - controlPoints[i - 1]);
				}
				else
				{
					vector3 = controlPoints[i + 1] - controlPoints[i];
				}
				Vector2 vector4;
				if (i < controlPoints.Count - 2)
				{
					vector4 = 0.5f * (controlPoints[i + 2] - controlPoints[i]);
				}
				else
				{
					vector4 = controlPoints[i + 1] - controlPoints[i];
				}
				if (i == controlPoints.Count - 2)
				{
					num = 1f / ((float)numberOfPoints - 1f);
				}
				for (int j = 0; j < numberOfPoints; j++)
				{
					float num2 = (float)j * num;
					Vector2 vector5 = (2f * num2 * num2 * num2 - 3f * num2 * num2 + 1f) * vector + (num2 * num2 * num2 - 2f * num2 * num2 + num2) * vector3 + (-2f * num2 * num2 * num2 + 3f * num2 * num2) * vector2 + (num2 * num2 * num2 - num2 * num2) * vector4;
					hashSet.Add(vector5);
				}
			}
			return hashSet;
		}

		public static List<Vector2I> StaggerLine(Vector2 p0, Vector2 p1, int numberOfBreaks, float staggerRange = 3f)
		{
			List<Vector2I> list = new List<Vector2I>();
			if (numberOfBreaks == 0)
			{
				return Util.GetLine(p0, p1);
			}
			Vector2 vector = p1 - p0;
			Vector2 vector2 = p0;
			Vector2 vector3 = p1;
			for (int i = 0; i < numberOfBreaks; i++)
			{
				vector3 = p0 + vector * (1f / (float)numberOfBreaks) * (float)i + Vector2.one * WorldGen.RandomRange(-staggerRange, staggerRange);
				list.AddRange(Util.GetLine(vector2, vector3));
				vector2 = vector3;
			}
			list.AddRange(Util.GetLine(vector3, p1));
			return list;
		}

		public static List<Vector2I> GetLine(Vector2 p0, Vector2 p1)
		{
			List<Vector2I> list = new List<Vector2I>();
			Vector2 vector = p1 - p0;
			float num = Mathf.Abs(vector.x);
			float num2 = Mathf.Abs(vector.y);
			int num3 = -1;
			if (p0.x < p1.x)
			{
				num3 = 1;
			}
			int num4 = -1;
			if (p0.y < p1.y)
			{
				num4 = 1;
			}
			float num5 = 0f;
			int num6 = 0;
			while ((float)num6 < num + num2)
			{
				list.Add(new Vector2I(Mathf.FloorToInt(p0.x), Mathf.FloorToInt(p0.y)));
				float num7 = num5 + num2;
				float num8 = num5 - num;
				if (Mathf.Abs(num7) < Mathf.Abs(num8))
				{
					p0.x += (float)num3;
					num5 = num7;
				}
				else
				{
					p0.y += (float)num4;
					num5 = num8;
				}
				num6++;
			}
			return list;
		}

		public static List<Vector2> GetCircle(Vector2 center, int radius)
		{
			int i = radius;
			int num = 0;
			int num2 = 1 - i;
			List<Vector2> list = new List<Vector2>();
			while (i >= num)
			{
				list.Add(new Vector2((float)i + center.x, (float)num + center.y));
				list.Add(new Vector2((float)num + center.x, (float)i + center.y));
				list.Add(new Vector2((float)(-(float)i) + center.x, (float)num + center.y));
				list.Add(new Vector2((float)(-(float)num) + center.x, (float)i + center.y));
				list.Add(new Vector2((float)(-(float)i) + center.x, (float)(-(float)num) + center.y));
				list.Add(new Vector2((float)(-(float)num) + center.x, (float)(-(float)i) + center.y));
				list.Add(new Vector2((float)i + center.x, (float)(-(float)num) + center.y));
				list.Add(new Vector2((float)num + center.x, (float)(-(float)i) + center.y));
				num++;
				if (num2 < 0)
				{
					num2 += 2 * num + 1;
				}
				else
				{
					i--;
					num2 += 2 * (num - i) + 1;
				}
			}
			return list;
		}

		private static void get8points(Vector2 c, float x, float y, List<Vector2I> points)
		{
			Vector2 vector = new Vector2(c.x - x, c.y + y);
			Vector2 vector2 = new Vector2(c.x + x, c.y + y);
			List<Vector2I> list = Util.GetLine(vector, vector2);
			points.AddRange(list);
			Vector2 vector3 = new Vector2(c.x - x, c.y - y);
			Vector2 vector4 = new Vector2(c.x + x, c.y - y);
			list = Util.GetLine(vector3, vector4);
			points.AddRange(list);
			if (x != y)
			{
				vector = new Vector2(c.x - y, c.y + x);
				vector2 = new Vector2(c.x + y, c.y + x);
				list = Util.GetLine(vector, vector2);
				points.AddRange(list);
				vector3 = new Vector2(c.x - y, c.y - x);
				vector4 = new Vector2(c.x + y, c.y - x);
				list = Util.GetLine(vector3, vector4);
				points.AddRange(list);
			}
		}

		public static List<Vector2I> GetFilledCircle(Vector2 center, float radius)
		{
			radius = Mathf.Floor(radius);
			List<Vector2I> list = new List<Vector2I>();
			float num = -radius;
			float num2 = radius;
			float num3 = 0f;
			while (num2 >= num3)
			{
				Util.get8points(center, num2, num3, list);
				num += num3;
				num3 += 1f;
				num += num3;
				if (num >= 0f)
				{
					num -= num2;
					num2 -= 1f;
					num -= num2;
				}
			}
			return list;
		}

		public static Vector2 RandomInUnitCircle(global::System.Random rng = null)
		{
			if (rng == null)
			{
				return global::UnityEngine.Random.insideUnitCircle;
			}
			double num = rng.NextDouble();
			double num2 = rng.NextDouble();
			double num3 = Math.Sqrt(num);
			return new Vector2((float)(num3 * Math.Cos(num2)), (float)(num3 * Math.Sin(num2)));
		}

		public static List<Vector2I> GetBlob(Vector2 center, float radius, global::System.Random rng = null)
		{
			List<Vector2> circle = Util.GetCircle(center, (int)Mathf.Ceil(radius + 0.5f));
			if (rng != null)
			{
				circle.ShuffleSeeded<Vector2>(rng);
			}
			else
			{
				Debug.LogWarning("GetBlob(): Calling unseeded random");
				circle.Shuffle<Vector2>();
			}
			for (int i = 0; i < circle.Count; i++)
			{
				List<Vector2> list2;
				List<Vector2> list = (list2 = circle);
				int num2;
				int num = (num2 = i);
				Vector2 vector = list2[num2];
				list[num] = vector + Util.RandomInUnitCircle(rng) * radius;
			}
			HashSet<Vector2> pointsOnCatmullRomSpline = Util.GetPointsOnCatmullRomSpline(circle, (int)(2f * radius * radius));
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			foreach (Vector2 vector2 in pointsOnCatmullRomSpline)
			{
				hashSet.Add(new Vector2I((int)vector2.x, (int)vector2.y));
			}
			return new List<Vector2I>(hashSet);
		}

		public static List<Vector2I> GetBorder(HashSet<Vector2I> sourcePoints, int radius)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			IEnumerator<Vector2I> enumerator = sourcePoints.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				Vector2I vector2I = enumerator.Current;
				int x = vector2I.x;
				Vector2I vector2I2 = enumerator.Current;
				int y = vector2I2.y;
				for (int i = x - radius; i <= x + radius; i++)
				{
					for (int j = y - radius; j <= y + radius; j++)
					{
						if (i != x || j != y)
						{
							Vector2I vector2I3 = new Vector2I(i, j);
							if (!sourcePoints.Contains(vector2I3))
							{
								hashSet.Add(vector2I3);
							}
						}
					}
				}
				num++;
			}
			return new List<Vector2I>(hashSet);
		}

		public static List<Vector2I> GetFilledRectangle(Vector2 center, float width, float height, float jitterMaxStep = 2f, float jitterRange = 2f)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			if (width < 1f)
			{
				width = 1f;
			}
			if (height < 1f)
			{
				height = 1f;
			}
			float num = 0f;
			float num2 = 0f;
			int num3 = (int)(center.x - width / 2f);
			int num4 = (int)(center.x + width / 2f);
			int num5 = (int)(center.y - height / 2f);
			int num6 = (int)(center.y + height / 2f);
			for (int i = num5; i < num6; i++)
			{
				num = Mathf.Max(-jitterRange, Mathf.Min(num + WorldGen.RandomRange(-jitterMaxStep, jitterMaxStep), jitterRange));
				num2 = Mathf.Max(-jitterRange, Mathf.Min(num2 + WorldGen.RandomRange(-jitterMaxStep, jitterMaxStep), jitterRange));
				int num7 = (int)((float)num3 - num);
				while ((float)num7 < (float)num4 + num2)
				{
					hashSet.Add(new Vector2I(num7, i));
					num7++;
				}
			}
			float num8 = 0f;
			float num9 = 0f;
			for (int j = num3; j < num4; j++)
			{
				num8 = Mathf.Max(-jitterRange, Mathf.Min(num8 + WorldGen.RandomRange(-jitterMaxStep, jitterMaxStep), jitterRange));
				num9 = Mathf.Max(-jitterRange, Mathf.Min(num9 + WorldGen.RandomRange(-jitterMaxStep, jitterMaxStep), jitterRange));
				for (int k = (int)((float)num5 - num8); k < num5; k++)
				{
					hashSet.Add(new Vector2I(j, k));
				}
				int num10 = num6;
				while ((float)num10 < (float)num6 + num9)
				{
					hashSet.Add(new Vector2I(j, num10));
					num10++;
				}
			}
			return new List<Vector2I>(hashSet);
		}

		public interface IWeighted
		{
			float weight { get; set; }
		}

		public static class WeightedRandom
		{
			public static T Choose<T>(List<T> list) where T : Util.IWeighted
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
				float num3 = WorldGen.RandomValue() * num;
				float num4 = 0f;
				for (int j = 0; j < list.Count; j++)
				{
					float num5 = num4;
					T t2 = list[j];
					num4 = num5 + t2.weight;
					if (num4 > num3)
					{
						return list[j];
					}
				}
				return list[list.Count - 1];
			}
		}
	}
}
