using System;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay.Geo
{
	public class LineSegment
	{
		public LineSegment(Vector2? p0, Vector2? p1)
		{
			this.p0 = p0;
			this.p1 = p1;
		}

		public static int CompareLengths_MAX(LineSegment segment0, LineSegment segment1)
		{
			Vector2? vector = segment0.p0;
			Vector2 value = vector.Value;
			Vector2? vector2 = segment0.p1;
			float num = Vector2.Distance(value, vector2.Value);
			Vector2? vector3 = segment1.p0;
			Vector2 value2 = vector3.Value;
			Vector2? vector4 = segment1.p1;
			float num2 = Vector2.Distance(value2, vector4.Value);
			int num3;
			if (num < num2)
			{
				num3 = 1;
			}
			else if (num > num2)
			{
				num3 = -1;
			}
			else
			{
				num3 = 0;
			}
			return num3;
		}

		public static int CompareLengths(LineSegment edge0, LineSegment edge1)
		{
			return -LineSegment.CompareLengths_MAX(edge0, edge1);
		}

		public Vector2? Center()
		{
			Vector2? vector = this.p0;
			Vector2? vector2;
			if (vector == null)
			{
				vector2 = this.p1;
			}
			else
			{
				Vector2? vector3 = this.p1;
				if (vector3 == null)
				{
					vector2 = this.p0;
				}
				else
				{
					vector2 = new Vector2?(this.p0.Value + 0.5f * this.Direction());
				}
			}
			return vector2;
		}

		public Vector2 Direction()
		{
			Vector2? vector = this.p0;
			if (vector != null)
			{
				Vector2? vector2 = this.p1;
				if (vector2 != null)
				{
					return this.p1.Value - this.p0.Value;
				}
			}
			return Vector2.zero;
		}

		private static float[] OverlapIntervals(float ub1, float ub2)
		{
			float num = Math.Min(ub1, ub2);
			float num2 = Math.Max(ub1, ub2);
			float num3 = Math.Max(0f, num);
			float num4 = Math.Min(1f, num2);
			float[] array;
			if (num3 > num4)
			{
				array = new float[0];
			}
			else if (num3 == num4)
			{
				array = new float[] { num3 };
			}
			else
			{
				array = new float[] { num3, num4 };
			}
			return array;
		}

		private static Vector2[] OneD_Intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			float num = a2.x - a1.x;
			float num2 = a2.y - a1.y;
			float num3;
			float num4;
			if (Math.Abs(num) > Math.Abs(num2))
			{
				num3 = (b1.x - a1.x) / num;
				num4 = (b2.x - a1.x) / num;
			}
			else
			{
				num3 = (b1.y - a1.y) / num2;
				num4 = (b2.y - a1.y) / num2;
			}
			List<Vector2> list = new List<Vector2>();
			float[] array = LineSegment.OverlapIntervals(num3, num4);
			foreach (float num5 in array)
			{
				float num6 = a2.x * num5 + a1.x * (1f - num5);
				float num7 = a2.y * num5 + a1.y * (1f - num5);
				Vector2 vector = new Vector2(num6, num7);
				list.Add(vector);
			}
			return list.ToArray();
		}

		private static bool PointOnLine(Vector2 p, Vector2 a1, Vector2 a2)
		{
			float num = 0f;
			double num2 = LineSegment.DistFromSeg(p, a1, a2, (double)Mathf.Epsilon, ref num);
			return num2 < (double)Mathf.Epsilon;
		}

		private static double DistFromSeg(Vector2 p, Vector2 q0, Vector2 q1, double radius, ref float u)
		{
			double num = (double)(q1.x - q0.x);
			double num2 = (double)(q1.y - q0.y);
			double num3 = (double)(q0.x - p.x);
			double num4 = (double)(q0.y - p.y);
			double num5 = Math.Sqrt(num * num + num2 * num2);
			if (num5 < (double)Mathf.Epsilon)
			{
				throw new Exception("Expected line segment, not point.");
			}
			double num6 = Math.Abs(num * num4 - num3 * num2);
			return num6 / num5;
		}

		public bool DoesIntersect(LineSegment other)
		{
			return LineSegment.DoesIntersect(this, other);
		}

		public static bool DoesIntersect(LineSegment a, LineSegment b)
		{
			Vector2[] array = LineSegment.Intersection(a.p0.Value, a.p1.Value, b.p0.Value, b.p1.Value);
			return array.Length > 0;
		}

		public static LineSegment Intersection(LineSegment a, LineSegment b)
		{
			Vector2[] array = LineSegment.Intersection(a.p0.Value, a.p1.Value, b.p0.Value, b.p1.Value);
			LineSegment lineSegment;
			if (array.Length == 1)
			{
				lineSegment = new LineSegment(new Vector2?(array[0]), null);
			}
			else if (array.Length == 2)
			{
				lineSegment = new LineSegment(new Vector2?(array[0]), new Vector2?(array[1]));
			}
			else
			{
				lineSegment = new LineSegment(null, null);
			}
			return lineSegment;
		}

		public static Vector2[] Intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			Vector2[] array;
			if (a1.Equals(a2) && b1.Equals(b2))
			{
				if (a1.Equals(b1))
				{
					array = new Vector2[] { a1 };
				}
				else
				{
					array = new Vector2[0];
				}
			}
			else if (b1.Equals(b2))
			{
				if (LineSegment.PointOnLine(b1, a1, a2))
				{
					array = new Vector2[] { b1 };
				}
				else
				{
					array = new Vector2[0];
				}
			}
			else if (a1.Equals(a2))
			{
				if (LineSegment.PointOnLine(a1, b1, b2))
				{
					array = new Vector2[] { a1 };
				}
				else
				{
					array = new Vector2[0];
				}
			}
			else
			{
				float num = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
				float num2 = (a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x);
				float num3 = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);
				if (-Mathf.Epsilon >= num3 || num3 >= Mathf.Epsilon)
				{
					float num4 = num / num3;
					float num5 = num2 / num3;
					if (0f <= num4 && num4 <= 1f && 0f <= num5 && num5 <= 1f)
					{
						array = new Vector2[]
						{
							new Vector2(a1.x + num4 * (a2.x - a1.x), a1.y + num4 * (a2.y - a1.y))
						};
					}
					else
					{
						array = new Vector2[0];
					}
				}
				else if ((-Mathf.Epsilon < num && num < Mathf.Epsilon) || (-Mathf.Epsilon < num2 && num2 < Mathf.Epsilon))
				{
					if (a1.Equals(a2))
					{
						array = LineSegment.OneD_Intersection(b1, b2, a1, a2);
					}
					else
					{
						array = LineSegment.OneD_Intersection(a1, a2, b1, b2);
					}
				}
				else
				{
					array = new Vector2[0];
				}
			}
			return array;
		}

		public Vector2? p0;

		public Vector2? p1;
	}
}
