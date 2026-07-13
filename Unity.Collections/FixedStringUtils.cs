using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[BurstCompatible]
	internal static class FixedStringUtils
	{
		internal static ParseError Base10ToBase2(ref float output, ulong mantissa10, int exponent10)
		{
			if (mantissa10 == 0UL)
			{
				output = 0f;
				return ParseError.None;
			}
			if (exponent10 == 0)
			{
				output = mantissa10;
				return ParseError.None;
			}
			int num = exponent10;
			ulong num2 = mantissa10;
			while (exponent10 > 0)
			{
				while ((num2 & 16140901064495857664UL) != 0UL)
				{
					num2 >>= 1;
					num++;
				}
				num2 *= 5UL;
				exponent10--;
			}
			while (exponent10 < 0)
			{
				while ((num2 & 9223372036854775808UL) == 0UL)
				{
					num2 <<= 1;
					num--;
				}
				num2 /= 5UL;
				exponent10++;
			}
			FixedStringUtils.UintFloatUnion uintFloatUnion = new FixedStringUtils.UintFloatUnion
			{
				floatValue = num2
			};
			int num3 = (int)(((uintFloatUnion.uintValue >> 23) & 255U) - 127U);
			num3 += num;
			if (num3 > 128)
			{
				return ParseError.Overflow;
			}
			if (num3 < -127)
			{
				return ParseError.Underflow;
			}
			uintFloatUnion.uintValue = (uintFloatUnion.uintValue & 2155872255U) | (uint)((uint)(num3 + 127) << 23);
			output = uintFloatUnion.floatValue;
			return ParseError.None;
		}

		internal static void Base2ToBase10(ref ulong mantissa10, ref int exponent10, float input)
		{
			FixedStringUtils.UintFloatUnion uintFloatUnion = new FixedStringUtils.UintFloatUnion
			{
				floatValue = input
			};
			if (uintFloatUnion.uintValue == 0U)
			{
				mantissa10 = 0UL;
				exponent10 = 0;
				return;
			}
			uint num = (uintFloatUnion.uintValue & 8388607U) | 8388608U;
			int i = (int)((uintFloatUnion.uintValue >> 23) - 127U - 23U);
			mantissa10 = (ulong)num;
			exponent10 = i;
			if (i > 0)
			{
				while (i > 0)
				{
					while (mantissa10 <= 1844674407370955161UL)
					{
						mantissa10 *= 10UL;
						exponent10--;
					}
					mantissa10 /= 5UL;
					i--;
				}
			}
			if (i < 0)
			{
				while (i < 0)
				{
					while (mantissa10 > 3689348814741910323UL)
					{
						mantissa10 /= 10UL;
						exponent10++;
					}
					mantissa10 *= 5UL;
					i++;
				}
			}
			while (mantissa10 > 9999999UL || mantissa10 % 10UL == 0UL)
			{
				mantissa10 = (mantissa10 + ((mantissa10 < 100000000UL) ? 5UL : 0UL)) / 10UL;
				exponent10++;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct UintFloatUnion
		{
			[FieldOffset(0)]
			public uint uintValue;

			[FieldOffset(0)]
			public float floatValue;
		}
	}
}
