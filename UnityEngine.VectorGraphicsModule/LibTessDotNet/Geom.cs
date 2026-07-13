using System;

namespace LibTessDotNet
{
	internal static class Geom
	{
		public static bool IsWindingInside(WindingRule rule, int n)
		{
			bool flag;
			switch (rule)
			{
			case WindingRule.EvenOdd:
				flag = (n & 1) == 1;
				break;
			case WindingRule.NonZero:
				flag = n != 0;
				break;
			case WindingRule.Positive:
				flag = n > 0;
				break;
			case WindingRule.Negative:
				flag = n < 0;
				break;
			case WindingRule.AbsGeqTwo:
				flag = n >= 2 || n <= -2;
				break;
			default:
				throw new Exception("Wrong winding rule");
			}
			return flag;
		}

		public static bool VertCCW(MeshUtils.Vertex u, MeshUtils.Vertex v, MeshUtils.Vertex w)
		{
			return u._s * (v._t - w._t) + v._s * (w._t - u._t) + w._s * (u._t - v._t) >= 0f;
		}

		public static bool VertEq(MeshUtils.Vertex lhs, MeshUtils.Vertex rhs)
		{
			return lhs._s == rhs._s && lhs._t == rhs._t;
		}

		public static bool VertLeq(MeshUtils.Vertex lhs, MeshUtils.Vertex rhs)
		{
			return lhs._s < rhs._s || (lhs._s == rhs._s && lhs._t <= rhs._t);
		}

		public static float EdgeEval(MeshUtils.Vertex u, MeshUtils.Vertex v, MeshUtils.Vertex w)
		{
			float num = v._s - u._s;
			float num2 = w._s - v._s;
			bool flag = num + num2 > 0f;
			float num3;
			if (flag)
			{
				bool flag2 = num < num2;
				if (flag2)
				{
					num3 = v._t - u._t + (u._t - w._t) * (num / (num + num2));
				}
				else
				{
					num3 = v._t - w._t + (w._t - u._t) * (num2 / (num + num2));
				}
			}
			else
			{
				num3 = 0f;
			}
			return num3;
		}

		public static float EdgeSign(MeshUtils.Vertex u, MeshUtils.Vertex v, MeshUtils.Vertex w)
		{
			float num = v._s - u._s;
			float num2 = w._s - v._s;
			bool flag = num + num2 > 0f;
			float num3;
			if (flag)
			{
				num3 = (v._t - w._t) * num + (v._t - u._t) * num2;
			}
			else
			{
				num3 = 0f;
			}
			return num3;
		}

		public static bool TransLeq(MeshUtils.Vertex lhs, MeshUtils.Vertex rhs)
		{
			return lhs._t < rhs._t || (lhs._t == rhs._t && lhs._s <= rhs._s);
		}

		public static float TransEval(MeshUtils.Vertex u, MeshUtils.Vertex v, MeshUtils.Vertex w)
		{
			float num = v._t - u._t;
			float num2 = w._t - v._t;
			bool flag = num + num2 > 0f;
			float num3;
			if (flag)
			{
				bool flag2 = num < num2;
				if (flag2)
				{
					num3 = v._s - u._s + (u._s - w._s) * (num / (num + num2));
				}
				else
				{
					num3 = v._s - w._s + (w._s - u._s) * (num2 / (num + num2));
				}
			}
			else
			{
				num3 = 0f;
			}
			return num3;
		}

		public static float TransSign(MeshUtils.Vertex u, MeshUtils.Vertex v, MeshUtils.Vertex w)
		{
			float num = v._t - u._t;
			float num2 = w._t - v._t;
			bool flag = num + num2 > 0f;
			float num3;
			if (flag)
			{
				num3 = (v._s - w._s) * num + (v._s - u._s) * num2;
			}
			else
			{
				num3 = 0f;
			}
			return num3;
		}

		public static bool EdgeGoesLeft(MeshUtils.Edge e)
		{
			return Geom.VertLeq(e._Dst, e._Org);
		}

		public static bool EdgeGoesRight(MeshUtils.Edge e)
		{
			return Geom.VertLeq(e._Org, e._Dst);
		}

		public static float VertL1dist(MeshUtils.Vertex u, MeshUtils.Vertex v)
		{
			return Math.Abs(u._s - v._s) + Math.Abs(u._t - v._t);
		}

		public static void AddWinding(MeshUtils.Edge eDst, MeshUtils.Edge eSrc)
		{
			eDst._winding += eSrc._winding;
			eDst._Sym._winding += eSrc._Sym._winding;
		}

		public static float Interpolate(float a, float x, float b, float y)
		{
			bool flag = a < 0f;
			if (flag)
			{
				a = 0f;
			}
			bool flag2 = b < 0f;
			if (flag2)
			{
				b = 0f;
			}
			return (a <= b) ? ((b == 0f) ? ((x + y) / 2f) : (x + (y - x) * (a / (a + b)))) : (y + (x - y) * (b / (a + b)));
		}

		private static void Swap(ref MeshUtils.Vertex a, ref MeshUtils.Vertex b)
		{
			MeshUtils.Vertex vertex = a;
			a = b;
			b = vertex;
		}

		public static void EdgeIntersect(MeshUtils.Vertex o1, MeshUtils.Vertex d1, MeshUtils.Vertex o2, MeshUtils.Vertex d2, MeshUtils.Vertex v)
		{
			bool flag = !Geom.VertLeq(o1, d1);
			if (flag)
			{
				Geom.Swap(ref o1, ref d1);
			}
			bool flag2 = !Geom.VertLeq(o2, d2);
			if (flag2)
			{
				Geom.Swap(ref o2, ref d2);
			}
			bool flag3 = !Geom.VertLeq(o1, o2);
			if (flag3)
			{
				Geom.Swap(ref o1, ref o2);
				Geom.Swap(ref d1, ref d2);
			}
			bool flag4 = !Geom.VertLeq(o2, d1);
			if (flag4)
			{
				v._s = (o2._s + d1._s) / 2f;
			}
			else
			{
				bool flag5 = Geom.VertLeq(d1, d2);
				if (flag5)
				{
					float num = Geom.EdgeEval(o1, o2, d1);
					float num2 = Geom.EdgeEval(o2, d1, d2);
					bool flag6 = num + num2 < 0f;
					if (flag6)
					{
						num = -num;
						num2 = -num2;
					}
					v._s = Geom.Interpolate(num, o2._s, num2, d1._s);
				}
				else
				{
					float num3 = Geom.EdgeSign(o1, o2, d1);
					float num4 = -Geom.EdgeSign(o1, d2, d1);
					bool flag7 = num3 + num4 < 0f;
					if (flag7)
					{
						num3 = -num3;
						num4 = -num4;
					}
					v._s = Geom.Interpolate(num3, o2._s, num4, d2._s);
				}
			}
			bool flag8 = !Geom.TransLeq(o1, d1);
			if (flag8)
			{
				Geom.Swap(ref o1, ref d1);
			}
			bool flag9 = !Geom.TransLeq(o2, d2);
			if (flag9)
			{
				Geom.Swap(ref o2, ref d2);
			}
			bool flag10 = !Geom.TransLeq(o1, o2);
			if (flag10)
			{
				Geom.Swap(ref o1, ref o2);
				Geom.Swap(ref d1, ref d2);
			}
			bool flag11 = !Geom.TransLeq(o2, d1);
			if (flag11)
			{
				v._t = (o2._t + d1._t) / 2f;
			}
			else
			{
				bool flag12 = Geom.TransLeq(d1, d2);
				if (flag12)
				{
					float num5 = Geom.TransEval(o1, o2, d1);
					float num6 = Geom.TransEval(o2, d1, d2);
					bool flag13 = num5 + num6 < 0f;
					if (flag13)
					{
						num5 = -num5;
						num6 = -num6;
					}
					v._t = Geom.Interpolate(num5, o2._t, num6, d1._t);
				}
				else
				{
					float num7 = Geom.TransSign(o1, o2, d1);
					float num8 = -Geom.TransSign(o1, d2, d1);
					bool flag14 = num7 + num8 < 0f;
					if (flag14)
					{
						num7 = -num7;
						num8 = -num8;
					}
					v._t = Geom.Interpolate(num7, o2._t, num8, d2._t);
				}
			}
		}
	}
}
