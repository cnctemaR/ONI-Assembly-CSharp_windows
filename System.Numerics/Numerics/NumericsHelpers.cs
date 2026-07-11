using System;

namespace System.Numerics
{
	internal static class NumericsHelpers
	{
		public static void GetDoubleParts(double dbl, out int sign, out int exp, out ulong man, out bool fFinite)
		{
			DoubleUlong doubleUlong;
			doubleUlong.uu = 0UL;
			doubleUlong.dbl = dbl;
			sign = 1 - ((int)(doubleUlong.uu >> 62) & 2);
			man = doubleUlong.uu & 4503599627370495UL;
			exp = (int)(doubleUlong.uu >> 52) & 2047;
			if (exp == 0)
			{
				fFinite = true;
				if (man != 0UL)
				{
					exp = -1074;
					return;
				}
			}
			else
			{
				if (exp == 2047)
				{
					fFinite = false;
					exp = int.MaxValue;
					return;
				}
				fFinite = true;
				man |= 4503599627370496UL;
				exp -= 1075;
			}
		}

		public static double GetDoubleFromParts(int sign, int exp, ulong man)
		{
			DoubleUlong doubleUlong;
			doubleUlong.dbl = 0.0;
			if (man == 0UL)
			{
				doubleUlong.uu = 0UL;
			}
			else
			{
				int num = NumericsHelpers.CbitHighZero(man) - 11;
				if (num < 0)
				{
					man >>= -num;
				}
				else
				{
					man <<= num;
				}
				exp -= num;
				exp += 1075;
				if (exp >= 2047)
				{
					doubleUlong.uu = 9218868437227405312UL;
				}
				else if (exp <= 0)
				{
					exp--;
					if (exp < -52)
					{
						doubleUlong.uu = 0UL;
					}
					else
					{
						doubleUlong.uu = man >> -exp;
					}
				}
				else
				{
					doubleUlong.uu = (man & 4503599627370495UL) | (ulong)((ulong)((long)exp) << 52);
				}
			}
			if (sign < 0)
			{
				doubleUlong.uu |= 9223372036854775808UL;
			}
			return doubleUlong.dbl;
		}

		public static void DangerousMakeTwosComplement(uint[] d)
		{
			if (d != null && d.Length != 0)
			{
				d[0] = ~d[0] + 1U;
				int i = 1;
				while (d[i - 1] == 0U)
				{
					if (i >= d.Length)
					{
						break;
					}
					d[i] = ~d[i] + 1U;
					i++;
				}
				while (i < d.Length)
				{
					d[i] = ~d[i];
					i++;
				}
			}
		}

		public static ulong MakeUlong(uint uHi, uint uLo)
		{
			return ((ulong)uHi << 32) | (ulong)uLo;
		}

		public static uint Abs(int a)
		{
			uint num = (uint)(a >> 31);
			return (uint)((a ^ (int)num) - (int)num);
		}

		public static uint CombineHash(uint u1, uint u2)
		{
			return ((u1 << 7) | (u1 >> 25)) ^ u2;
		}

		public static int CombineHash(int n1, int n2)
		{
			return (int)NumericsHelpers.CombineHash((uint)n1, (uint)n2);
		}

		public static int CbitHighZero(uint u)
		{
			if (u == 0U)
			{
				return 32;
			}
			int num = 0;
			if ((u & 4294901760U) == 0U)
			{
				num += 16;
				u <<= 16;
			}
			if ((u & 4278190080U) == 0U)
			{
				num += 8;
				u <<= 8;
			}
			if ((u & 4026531840U) == 0U)
			{
				num += 4;
				u <<= 4;
			}
			if ((u & 3221225472U) == 0U)
			{
				num += 2;
				u <<= 2;
			}
			if ((u & 2147483648U) == 0U)
			{
				num++;
			}
			return num;
		}

		public static int CbitHighZero(ulong uu)
		{
			if ((uu & 18446744069414584320UL) == 0UL)
			{
				return 32 + NumericsHelpers.CbitHighZero((uint)uu);
			}
			return NumericsHelpers.CbitHighZero((uint)(uu >> 32));
		}

		private const int kcbitUint = 32;
	}
}
