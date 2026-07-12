using System;
using System.Runtime.CompilerServices;

namespace System.Buffers.Text
{
	internal static class Number
	{
		internal static bool NumberBufferToDouble(ref NumberBuffer number, out double value)
		{
			double num = Number.NumberToDouble(ref number);
			if (!double.IsFinite(num))
			{
				value = 0.0;
				return false;
			}
			if (num == 0.0)
			{
				num = 0.0;
			}
			value = num;
			return true;
		}

		public unsafe static bool NumberBufferToDecimal(ref NumberBuffer number, ref decimal value)
		{
			MutableDecimal mutableDecimal = default(MutableDecimal);
			byte* ptr = number.UnsafeDigits;
			int num = number.Scale;
			if (*ptr == 0)
			{
				if (num > 0)
				{
					num = 0;
				}
			}
			else
			{
				if (num > 29)
				{
					return false;
				}
				while ((num > 0 || (*ptr != 0 && num > -28)) && (mutableDecimal.High < 429496729U || (mutableDecimal.High == 429496729U && (mutableDecimal.Mid < 2576980377U || (mutableDecimal.Mid == 2576980377U && (mutableDecimal.Low < 2576980377U || (mutableDecimal.Low == 2576980377U && *ptr <= 53)))))))
				{
					DecimalDecCalc.DecMul10(ref mutableDecimal);
					if (*ptr != 0)
					{
						DecimalDecCalc.DecAddInt32(ref mutableDecimal, (uint)(*(ptr++) - 48));
					}
					num--;
				}
				if (*(ptr++) >= 53)
				{
					bool flag = true;
					if (*(ptr - 1) == 53 && *(ptr - 2) % 2 == 0)
					{
						int num2 = 20;
						while (*ptr == 48 && num2 != 0)
						{
							ptr++;
							num2--;
						}
						if (*ptr == 0 || num2 == 0)
						{
							flag = false;
						}
					}
					if (flag)
					{
						DecimalDecCalc.DecAddInt32(ref mutableDecimal, 1U);
						if ((mutableDecimal.High | mutableDecimal.Mid | mutableDecimal.Low) == 0U)
						{
							mutableDecimal.High = 429496729U;
							mutableDecimal.Mid = 2576980377U;
							mutableDecimal.Low = 2576980378U;
							num++;
						}
					}
				}
			}
			if (num > 0)
			{
				return false;
			}
			if (num <= -29)
			{
				mutableDecimal.High = 0U;
				mutableDecimal.Low = 0U;
				mutableDecimal.Mid = 0U;
				mutableDecimal.Scale = 28;
			}
			else
			{
				mutableDecimal.Scale = -num;
			}
			mutableDecimal.IsNegative = number.IsNegative;
			value = *Unsafe.As<MutableDecimal, decimal>(ref mutableDecimal);
			return true;
		}

		public unsafe static void DecimalToNumber(decimal value, ref NumberBuffer number)
		{
			ref MutableDecimal ptr = ref Unsafe.As<decimal, MutableDecimal>(ref value);
			Span<byte> digits = number.Digits;
			number.IsNegative = ptr.IsNegative;
			int num = 29;
			while ((ptr.Mid > 0U) | (ptr.High > 0U))
			{
				uint num2 = DecimalDecCalc.DecDivMod1E9(ref ptr);
				for (int i = 0; i < 9; i++)
				{
					*digits[--num] = (byte)(num2 % 10U + 48U);
					num2 /= 10U;
				}
			}
			for (uint num3 = ptr.Low; num3 != 0U; num3 /= 10U)
			{
				*digits[--num] = (byte)(num3 % 10U + 48U);
			}
			int num4 = 29 - num;
			number.Scale = num4 - ptr.Scale;
			Span<byte> digits2 = number.Digits;
			int num5 = 0;
			while (--num4 >= 0)
			{
				*digits2[num5++] = *digits[num++];
			}
			*digits2[num5] = 0;
		}

		private static uint DigitsToInt(ReadOnlySpan<byte> digits, int count)
		{
			uint num;
			int num2;
			Utf8Parser.TryParse(digits.Slice(0, count), out num, out num2, 'D');
			return num;
		}

		private static ulong Mul32x32To64(uint a, uint b)
		{
			return (ulong)a * (ulong)b;
		}

		private static ulong Mul64Lossy(ulong a, ulong b, ref int pexp)
		{
			ulong num = Number.Mul32x32To64((uint)(a >> 32), (uint)(b >> 32)) + (Number.Mul32x32To64((uint)(a >> 32), (uint)b) >> 32) + (Number.Mul32x32To64((uint)a, (uint)(b >> 32)) >> 32);
			if ((num & 9223372036854775808UL) == 0UL)
			{
				num <<= 1;
				pexp--;
			}
			return num;
		}

		private static int abs(int value)
		{
			if (value < 0)
			{
				return -value;
			}
			return value;
		}

		private unsafe static double NumberToDouble(ref NumberBuffer number)
		{
			ReadOnlySpan<byte> readOnlySpan = number.Digits;
			int num = 0;
			int numDigits = number.NumDigits;
			int num2 = numDigits;
			while (*readOnlySpan[num] == 48)
			{
				num2--;
				num++;
			}
			if (num2 == 0)
			{
				return 0.0;
			}
			int num3 = Math.Min(num2, 9);
			num2 -= num3;
			ulong num4 = (ulong)Number.DigitsToInt(readOnlySpan, num3);
			if (num2 > 0)
			{
				num3 = Math.Min(num2, 9);
				num2 -= num3;
				uint num5 = (uint)(Number.s_rgval64Power10[num3 - 1] >> (int)(64 - Number.s_rgexp64Power10[num3 - 1]));
				num4 = Number.Mul32x32To64((uint)num4, num5) + (ulong)Number.DigitsToInt(readOnlySpan.Slice(9), num3);
			}
			int num6 = number.Scale - (numDigits - num2);
			int num7 = Number.abs(num6);
			if (num7 >= 352)
			{
				ulong num8 = ((num6 > 0) ? 9218868437227405312UL : 0UL);
				if (number.IsNegative)
				{
					num8 |= 9223372036854775808UL;
				}
				return *(double*)(&num8);
			}
			int num9 = 64;
			if ((num4 & 18446744069414584320UL) == 0UL)
			{
				num4 <<= 32;
				num9 -= 32;
			}
			if ((num4 & 18446462598732840960UL) == 0UL)
			{
				num4 <<= 16;
				num9 -= 16;
			}
			if ((num4 & 18374686479671623680UL) == 0UL)
			{
				num4 <<= 8;
				num9 -= 8;
			}
			if ((num4 & 17293822569102704640UL) == 0UL)
			{
				num4 <<= 4;
				num9 -= 4;
			}
			if ((num4 & 13835058055282163712UL) == 0UL)
			{
				num4 <<= 2;
				num9 -= 2;
			}
			if ((num4 & 9223372036854775808UL) == 0UL)
			{
				num4 <<= 1;
				num9--;
			}
			int num10 = num7 & 15;
			if (num10 != 0)
			{
				int num11 = (int)Number.s_rgexp64Power10[num10 - 1];
				num9 += ((num6 < 0) ? (-num11 + 1) : num11);
				ulong num12 = Number.s_rgval64Power10[num10 + ((num6 < 0) ? 15 : 0) - 1];
				num4 = Number.Mul64Lossy(num4, num12, ref num9);
			}
			num10 = num7 >> 4;
			if (num10 != 0)
			{
				int num13 = (int)Number.s_rgexp64Power10By16[num10 - 1];
				num9 += ((num6 < 0) ? (-num13 + 1) : num13);
				ulong num14 = Number.s_rgval64Power10By16[num10 + ((num6 < 0) ? 21 : 0) - 1];
				num4 = Number.Mul64Lossy(num4, num14, ref num9);
			}
			if (((int)num4 & 1024) != 0)
			{
				ulong num15 = num4 + 1023UL + (ulong)((long)(((int)num4 >> 11) & 1));
				if (num15 < num4)
				{
					num15 = (num15 >> 1) | 9223372036854775808UL;
					num9++;
				}
				num4 = num15;
			}
			num9 += 1022;
			if (num9 <= 0)
			{
				if (num9 == -52 && num4 >= 9223372036854775896UL)
				{
					num4 = 1UL;
				}
				else if (num9 <= -52)
				{
					num4 = 0UL;
				}
				else
				{
					num4 >>= -num9 + 11 + 1;
				}
			}
			else if (num9 >= 2047)
			{
				num4 = 9218868437227405312UL;
			}
			else
			{
				num4 = (ulong)(((long)num9 << 52) + (long)((num4 >> 11) & 4503599627370495UL));
			}
			if (number.IsNegative)
			{
				num4 |= 9223372036854775808UL;
			}
			return *(double*)(&num4);
		}

		public unsafe static void RoundNumber(ref NumberBuffer number, int pos)
		{
			Span<byte> digits = number.Digits;
			int num = 0;
			while (num < pos && *digits[num] != 0)
			{
				num++;
			}
			if (num == pos && *digits[num] >= 53)
			{
				while (num > 0 && *digits[num - 1] == 57)
				{
					num--;
				}
				if (num > 0)
				{
					ref byte ptr = ref digits[num - 1];
					ptr += 1;
				}
				else
				{
					number.Scale++;
					*digits[0] = 49;
					num = 1;
				}
			}
			else
			{
				while (num > 0 && *digits[num - 1] == 48)
				{
					num--;
				}
			}
			if (num == 0)
			{
				number.Scale = 0;
				number.IsNegative = false;
			}
			*digits[num] = 0;
		}

		private static readonly ulong[] s_rgval64Power10 = new ulong[]
		{
			11529215046068469760UL, 14411518807585587200UL, 18014398509481984000UL, 11258999068426240000UL, 14073748835532800000UL, 17592186044416000000UL, 10995116277760000000UL, 13743895347200000000UL, 17179869184000000000UL, 10737418240000000000UL,
			13421772800000000000UL, 16777216000000000000UL, 10485760000000000000UL, 13107200000000000000UL, 16384000000000000000UL, 14757395258967641293UL, 11805916207174113035UL, 9444732965739290428UL, 15111572745182864686UL, 12089258196146291749UL,
			9671406556917033399UL, 15474250491067253438UL, 12379400392853802751UL, 9903520314283042201UL, 15845632502852867522UL, 12676506002282294018UL, 10141204801825835215UL, 16225927682921336344UL, 12980742146337069075UL, 10384593717069655260UL
		};

		private static readonly sbyte[] s_rgexp64Power10 = new sbyte[]
		{
			4, 7, 10, 14, 17, 20, 24, 27, 30, 34,
			37, 40, 44, 47, 50
		};

		private static readonly ulong[] s_rgval64Power10By16 = new ulong[]
		{
			10240000000000000000UL, 11368683772161602974UL, 12621774483536188886UL, 14012984643248170708UL, 15557538194652854266UL, 17272337110188889248UL, 9588073174409622172UL, 10644899600020376798UL, 11818212630765741798UL, 13120851772591970216UL,
			14567071740625403792UL, 16172698447808779622UL, 17955302187076837696UL, 9967194951097567532UL, 11065809325636130658UL, 12285516299433008778UL, 13639663065038175358UL, 15143067982934716296UL, 16812182738118149112UL, 9332636185032188787UL,
			10361307573072618722UL, 16615349947311448416UL, 14965776766268445891UL, 13479973333575319909UL, 12141680576410806707UL, 10936253623915059637UL, 9850501549098619819UL, 17745086042373215136UL, 15983352577617880260UL, 14396524142538228461UL,
			12967236152753103031UL, 11679847981112819795UL, 10520271803096747049UL, 9475818434452569218UL, 17070116948172427008UL, 15375394465392026135UL, 13848924157002783096UL, 12474001934591998882UL, 11235582092889474480UL, 10120112665365530972UL,
			18230774251475056952UL, 16420821625123739930UL
		};

		private static readonly short[] s_rgexp64Power10By16 = new short[]
		{
			54, 107, 160, 213, 266, 319, 373, 426, 479, 532,
			585, 638, 691, 745, 798, 851, 904, 957, 1010, 1064,
			1117
		};

		internal const int DECIMAL_PRECISION = 29;
	}
}
