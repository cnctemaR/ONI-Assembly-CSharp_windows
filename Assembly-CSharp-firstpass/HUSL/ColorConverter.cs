using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace HUSL
{
	public class ColorConverter
	{
		protected static IList<double[]> GetBounds(double L)
		{
			List<double[]> list = new List<double[]>();
			double num = Math.Pow(L + 16.0, 3.0) / 1560896.0;
			double num2 = ((num <= ColorConverter.Epsilon) ? (L / ColorConverter.Kappa) : num);
			for (int i = 0; i < 3; i++)
			{
				double num3 = ColorConverter.M[i][0];
				double num4 = ColorConverter.M[i][1];
				double num5 = ColorConverter.M[i][2];
				for (int j = 0; j < 2; j++)
				{
					double num6 = (284517.0 * num3 - 94839.0 * num5) * num2;
					double num7 = (838422.0 * num5 + 769860.0 * num4 + 731718.0 * num3) * L * num2 - (double)(769860 * j) * L;
					double num8 = (632260.0 * num5 - 126452.0 * num4) * num2 + (double)(126452 * j);
					list.Add(new double[]
					{
						num6 / num8,
						num7 / num8
					});
				}
			}
			return list;
		}

		protected static double IntersectLineLine(IList<double> lineA, IList<double> lineB)
		{
			return (lineA[1] - lineB[1]) / (lineB[0] - lineA[0]);
		}

		protected static double DistanceFromPole(IList<double> point)
		{
			return Math.Sqrt(Math.Pow(point[0], 2.0) + Math.Pow(point[1], 2.0));
		}

		protected static bool LengthOfRayUntilIntersect(double theta, IList<double> line, out double length)
		{
			length = line[1] / (Math.Sin(theta) - line[0] * Math.Cos(theta));
			return length >= 0.0;
		}

		protected static double MaxSafeChromaForL(double L)
		{
			IList<double[]> bounds = ColorConverter.GetBounds(L);
			double num = double.MaxValue;
			for (int i = 0; i < 2; i++)
			{
				double num2 = bounds[i][0];
				double num3 = bounds[i][1];
				double[] array = new double[] { num2, num3 };
				IList<double> list = array;
				double[] array2 = new double[2];
				array2[0] = -1.0 / num2;
				double num4 = ColorConverter.IntersectLineLine(list, array2);
				double num5 = ColorConverter.DistanceFromPole(new double[]
				{
					num4,
					num3 + num4 * num2
				});
				num = Math.Min(num, num5);
			}
			return num;
		}

		protected static double MaxChromaForLH(double L, double H)
		{
			double num = H / 360.0 * 3.141592653589793 * 2.0;
			IList<double[]> bounds = ColorConverter.GetBounds(L);
			double num2 = double.MaxValue;
			foreach (double[] array in bounds)
			{
				double num3;
				if (ColorConverter.LengthOfRayUntilIntersect(num, array, out num3))
				{
					num2 = Math.Min(num2, num3);
				}
			}
			return num2;
		}

		protected static double DotProduct(IList<double> a, IList<double> b)
		{
			double num = 0.0;
			for (int i = 0; i < a.Count; i++)
			{
				num += a[i] * b[i];
			}
			return num;
		}

		protected static double Round(double value, int places)
		{
			double num = Math.Pow(10.0, (double)places);
			return Math.Round(value * num) / num;
		}

		protected static double FromLinear(double c)
		{
			double num;
			if (c <= 0.0031308)
			{
				num = 12.92 * c;
			}
			else
			{
				num = 1.055 * Math.Pow(c, 0.4166666666666667) - 0.055;
			}
			return num;
		}

		protected static double ToLinear(double c)
		{
			double num;
			if (c > 0.04045)
			{
				num = Math.Pow((c + 0.055) / 1.055, 2.4);
			}
			else
			{
				num = c / 12.92;
			}
			return num;
		}

		protected static IList<int> RGBPrepare(IList<double> tuple)
		{
			for (int i = 0; i < tuple.Count; i++)
			{
				tuple[i] = ColorConverter.Round(tuple[i], 3);
			}
			for (int j = 0; j < tuple.Count; j++)
			{
				double num = tuple[j];
				if (num < -0.0001 || num > 1.0001)
				{
					throw new Exception("Illegal rgb value: " + num);
				}
			}
			int[] array = new int[tuple.Count];
			for (int k = 0; k < tuple.Count; k++)
			{
				array[k] = (int)Math.Round(tuple[k] * 255.0);
			}
			return array;
		}

		protected static double YToL(double Y)
		{
			double num;
			if (Y <= ColorConverter.Epsilon)
			{
				num = Y / ColorConverter.RefY * ColorConverter.Kappa;
			}
			else
			{
				num = 116.0 * Math.Pow(Y / ColorConverter.RefY, 0.3333333333333333) - 16.0;
			}
			return num;
		}

		protected static double LToY(double L)
		{
			double num;
			if (L <= 8.0)
			{
				num = ColorConverter.RefY * L / ColorConverter.Kappa;
			}
			else
			{
				num = ColorConverter.RefY * Math.Pow((L + 16.0) / 116.0, 3.0);
			}
			return num;
		}

		public static IList<double> XYZToRGB(IList<double> tuple)
		{
			return new double[]
			{
				ColorConverter.FromLinear(ColorConverter.DotProduct(ColorConverter.M[0], tuple)),
				ColorConverter.FromLinear(ColorConverter.DotProduct(ColorConverter.M[1], tuple)),
				ColorConverter.FromLinear(ColorConverter.DotProduct(ColorConverter.M[2], tuple))
			};
		}

		public static IList<double> RGBToXYZ(IList<double> tuple)
		{
			double[] array = new double[]
			{
				ColorConverter.ToLinear(tuple[0]),
				ColorConverter.ToLinear(tuple[1]),
				ColorConverter.ToLinear(tuple[2])
			};
			return new double[]
			{
				ColorConverter.DotProduct(ColorConverter.MInv[0], array),
				ColorConverter.DotProduct(ColorConverter.MInv[1], array),
				ColorConverter.DotProduct(ColorConverter.MInv[2], array)
			};
		}

		public static IList<double> XYZToLUV(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			double num4 = 4.0 * num / (num + 15.0 * num2 + 3.0 * num3);
			double num5 = 9.0 * num2 / (num + 15.0 * num2 + 3.0 * num3);
			double num6 = ColorConverter.YToL(num2);
			IList<double> list;
			if (num6 == 0.0)
			{
				list = new double[3];
			}
			else
			{
				double num7 = 13.0 * num6 * (num4 - ColorConverter.RefU);
				double num8 = 13.0 * num6 * (num5 - ColorConverter.RefV);
				list = new double[] { num6, num7, num8 };
			}
			return list;
		}

		public static IList<double> LUVToXYZ(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			IList<double> list;
			if (num == 0.0)
			{
				list = new double[3];
			}
			else
			{
				double num4 = num2 / (13.0 * num) + ColorConverter.RefU;
				double num5 = num3 / (13.0 * num) + ColorConverter.RefV;
				double num6 = ColorConverter.LToY(num);
				double num7 = 0.0 - 9.0 * num6 * num4 / ((num4 - 4.0) * num5 - num4 * num5);
				double num8 = (9.0 * num6 - 15.0 * num5 * num6 - num5 * num7) / (3.0 * num5);
				list = new double[] { num7, num6, num8 };
			}
			return list;
		}

		public static IList<double> LUVToLCH(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			double num4 = Math.Pow(Math.Pow(num2, 2.0) + Math.Pow(num3, 2.0), 0.5);
			double num5 = Math.Atan2(num3, num2);
			double num6 = num5 * 180.0 / 3.141592653589793;
			if (num6 < 0.0)
			{
				num6 = 360.0 + num6;
			}
			return new double[] { num, num4, num6 };
		}

		public static IList<double> LCHToLUV(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			double num4 = num3 / 360.0 * 2.0 * 3.141592653589793;
			double num5 = Math.Cos(num4) * num2;
			double num6 = Math.Sin(num4) * num2;
			return new double[] { num, num5, num6 };
		}

		public static IList<double> HUSLToLCH(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			IList<double> list;
			if (num3 > 99.9999999)
			{
				list = new double[] { 100.0, 0.0, num };
			}
			else if (num3 < 1E-08)
			{
				list = new double[] { 0.0, 0.0, num };
			}
			else
			{
				double num4 = ColorConverter.MaxChromaForLH(num3, num);
				double num5 = num4 / 100.0 * num2;
				list = new double[] { num3, num5, num };
			}
			return list;
		}

		public static IList<double> LCHToHUSL(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			IList<double> list;
			if (num > 99.9999999)
			{
				list = new double[] { num3, 0.0, 100.0 };
			}
			else if (num < 1E-08)
			{
				double[] array = new double[3];
				array[0] = num3;
				list = array;
			}
			else
			{
				double num4 = ColorConverter.MaxChromaForLH(num, num3);
				double num5 = num2 / num4 * 100.0;
				list = new double[] { num3, num5, num };
			}
			return list;
		}

		public static IList<double> HUSLPToLCH(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			IList<double> list;
			if (num3 > 99.9999999)
			{
				list = new double[] { 100.0, 0.0, num };
			}
			else if (num3 < 1E-08)
			{
				list = new double[] { 0.0, 0.0, num };
			}
			else
			{
				double num4 = ColorConverter.MaxSafeChromaForL(num3);
				double num5 = num4 / 100.0 * num2;
				list = new double[] { num3, num5, num };
			}
			return list;
		}

		public static IList<double> LCHToHUSLP(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			IList<double> list;
			if (num > 99.9999999)
			{
				list = new double[] { num3, 0.0, 100.0 };
			}
			else if (num < 1E-08)
			{
				double[] array = new double[3];
				array[0] = num3;
				list = array;
			}
			else
			{
				double num4 = ColorConverter.MaxSafeChromaForL(num);
				double num5 = num2 / num4 * 100.0;
				list = new double[] { num3, num5, num };
			}
			return list;
		}

		public static string RGBToHex(IList<double> tuple)
		{
			IList<int> list = ColorConverter.RGBPrepare(tuple);
			return string.Format("#{0}{1}{2}", list[0].ToString("x2"), list[1].ToString("x2"), list[2].ToString("x2"));
		}

		public static IList<double> HexToRGB(string hex)
		{
			return new double[]
			{
				(double)int.Parse(hex.Substring(1, 2), NumberStyles.HexNumber) / 255.0,
				(double)int.Parse(hex.Substring(3, 2), NumberStyles.HexNumber) / 255.0,
				(double)int.Parse(hex.Substring(5, 2), NumberStyles.HexNumber) / 255.0
			};
		}

		public static IList<double> LCHToRGB(IList<double> tuple)
		{
			return ColorConverter.XYZToRGB(ColorConverter.LUVToXYZ(ColorConverter.LCHToLUV(tuple)));
		}

		public static IList<double> RGBToLCH(IList<double> tuple)
		{
			return ColorConverter.LUVToLCH(ColorConverter.XYZToLUV(ColorConverter.RGBToXYZ(tuple)));
		}

		public static IList<double> HUSLToRGB(IList<double> tuple)
		{
			return ColorConverter.LCHToRGB(ColorConverter.HUSLToLCH(tuple));
		}

		public static IList<double> RGBToHUSL(IList<double> tuple)
		{
			return ColorConverter.LCHToHUSL(ColorConverter.RGBToLCH(tuple));
		}

		public static IList<double> HUSLPToRGB(IList<double> tuple)
		{
			return ColorConverter.LCHToRGB(ColorConverter.HUSLPToLCH(tuple));
		}

		public static IList<double> RGBToHUSLP(IList<double> tuple)
		{
			return ColorConverter.LCHToHUSLP(ColorConverter.RGBToLCH(tuple));
		}

		public static string HUSLToHex(IList<double> tuple)
		{
			return ColorConverter.RGBToHex(ColorConverter.HUSLToRGB(tuple));
		}

		public static string HUSLPToHex(IList<double> tuple)
		{
			return ColorConverter.RGBToHex(ColorConverter.HUSLPToRGB(tuple));
		}

		public static IList<double> HexToHUSL(string s)
		{
			return ColorConverter.RGBToHUSL(ColorConverter.HexToRGB(s));
		}

		public static IList<double> HexToHUSLP(string s)
		{
			return ColorConverter.RGBToHUSLP(ColorConverter.HexToRGB(s));
		}

		public static Color HUSLToColor(float h, float s, float l)
		{
			double[] array = new double[]
			{
				(double)h,
				(double)s,
				(double)l
			};
			IList<double> list = ColorConverter.HUSLToRGB(new List<double>(array));
			return new Color((float)list[0], (float)list[1], (float)list[2]);
		}

		public static Color HUSLPToColor(float h, float s, float l)
		{
			double[] array = new double[]
			{
				(double)h,
				(double)s,
				(double)l
			};
			IList<double> list = ColorConverter.HUSLPToRGB(new List<double>(array));
			return new Color((float)list[0], (float)list[1], (float)list[2]);
		}

		protected static double[][] M = new double[][]
		{
			new double[] { 3.240969941904521, -1.537383177570093, -0.498610760293 },
			new double[] { -0.96924363628087, 1.87596750150772, 0.041555057407175 },
			new double[] { 0.055630079696993, -0.20397695888897, 1.056971514242878 }
		};

		protected static double[][] MInv = new double[][]
		{
			new double[] { 0.41239079926595, 0.35758433938387, 0.18048078840183 },
			new double[] { 0.21263900587151, 0.71516867876775, 0.072192315360733 },
			new double[] { 0.019330818715591, 0.11919477979462, 0.95053215224966 }
		};

		protected static double RefX = 0.95045592705167;

		protected static double RefY = 1.0;

		protected static double RefZ = 1.089057750759878;

		protected static double RefU = 0.19783000664283;

		protected static double RefV = 0.46831999493879;

		protected static double Kappa = 903.2962962;

		protected static double Epsilon = 0.0088564516;
	}
}
