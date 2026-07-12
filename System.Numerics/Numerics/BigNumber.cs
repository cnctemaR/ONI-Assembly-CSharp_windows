using System;
using System.Buffers;
using System.Globalization;
using System.Text;

namespace System.Numerics
{
	internal static class BigNumber
	{
		internal static bool TryValidateParseStyleInteger(NumberStyles style, out ArgumentException e)
		{
			if ((style & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowHexSpecifier)) != NumberStyles.None)
			{
				e = new ArgumentException(SR.Format("An undefined NumberStyles value is being used.", "style"));
				return false;
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None && (style & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowHexSpecifier)) != NumberStyles.None)
			{
				e = new ArgumentException("With the AllowHexSpecifier bit set in the enum bit field, the only other valid bits that can be combined into the enum value must be a subset of those in HexNumber.");
				return false;
			}
			e = null;
			return true;
		}

		internal static bool TryParseBigInteger(string value, NumberStyles style, NumberFormatInfo info, out BigInteger result)
		{
			if (value == null)
			{
				result = default(BigInteger);
				return false;
			}
			return BigNumber.TryParseBigInteger(value.AsSpan(), style, info, out result);
		}

		internal static bool TryParseBigInteger(ReadOnlySpan<char> value, NumberStyles style, NumberFormatInfo info, out BigInteger result)
		{
			result = BigInteger.Zero;
			ArgumentException ex;
			if (!BigNumber.TryValidateParseStyleInteger(style, out ex))
			{
				throw ex;
			}
			BigNumber.BigNumberBuffer bigNumberBuffer = BigNumber.BigNumberBuffer.Create();
			if (!FormatProvider.TryStringToBigInteger(value, style, info, bigNumberBuffer.digits, out bigNumberBuffer.precision, out bigNumberBuffer.scale, out bigNumberBuffer.sign))
			{
				return false;
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				if (!BigNumber.HexNumberToBigInteger(ref bigNumberBuffer, ref result))
				{
					return false;
				}
			}
			else if (!BigNumber.NumberToBigInteger(ref bigNumberBuffer, ref result))
			{
				return false;
			}
			return true;
		}

		internal static BigInteger ParseBigInteger(string value, NumberStyles style, NumberFormatInfo info)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return BigNumber.ParseBigInteger(value.AsSpan(), style, info);
		}

		internal static BigInteger ParseBigInteger(ReadOnlySpan<char> value, NumberStyles style, NumberFormatInfo info)
		{
			ArgumentException ex;
			if (!BigNumber.TryValidateParseStyleInteger(style, out ex))
			{
				throw ex;
			}
			BigInteger zero = BigInteger.Zero;
			if (!BigNumber.TryParseBigInteger(value, style, info, out zero))
			{
				throw new FormatException("The value could not be parsed.");
			}
			return zero;
		}

		private static bool HexNumberToBigInteger(ref BigNumber.BigNumberBuffer number, ref BigInteger value)
		{
			if (number.digits == null || number.digits.Length == 0)
			{
				return false;
			}
			int num = number.digits.Length - 1;
			byte[] array = new byte[num / 2 + num % 2];
			bool flag = false;
			bool flag2 = false;
			int num2 = 0;
			for (int i = num - 1; i > -1; i--)
			{
				char c = number.digits[i];
				byte b;
				if (c >= '0' && c <= '9')
				{
					b = (byte)(c - '0');
				}
				else if (c >= 'A' && c <= 'F')
				{
					b = (byte)(c - 'A' + '\n');
				}
				else
				{
					b = (byte)(c - 'a' + '\n');
				}
				if (i == 0 && (b & 8) == 8)
				{
					flag2 = true;
				}
				if (flag)
				{
					array[num2] = (byte)((int)array[num2] | ((int)b << 4));
					num2++;
				}
				else
				{
					array[num2] = (flag2 ? (b | 240) : b);
				}
				flag = !flag;
			}
			value = new BigInteger(array);
			return true;
		}

		private static bool NumberToBigInteger(ref BigNumber.BigNumberBuffer number, ref BigInteger value)
		{
			int num = number.scale;
			int num2 = 0;
			BigInteger bigInteger = 10;
			value = 0;
			while (--num >= 0)
			{
				value *= bigInteger;
				if (number.digits[num2] != '\0')
				{
					value += (int)(number.digits[num2++] - '0');
				}
			}
			while (number.digits[num2] != '\0')
			{
				if (number.digits[num2++] != '0')
				{
					return false;
				}
			}
			if (number.sign)
			{
				value = -value;
			}
			return true;
		}

		internal unsafe static char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
		{
			digits = -1;
			if (format.Length == 0)
			{
				return 'R';
			}
			int num = 0;
			char c = (char)(*format[num]);
			if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
			{
				num++;
				int num2 = -1;
				if (num < format.Length && *format[num] >= 48 && *format[num] <= 57)
				{
					num2 = (int)(*format[num++] - 48);
					while (num < format.Length && *format[num] >= 48 && *format[num] <= 57)
					{
						num2 = num2 * 10 + (int)(*format[num++] - 48);
						if (num2 >= 10)
						{
							break;
						}
					}
				}
				if (num >= format.Length || *format[num] == 0)
				{
					digits = num2;
					return c;
				}
			}
			return '\0';
		}

		private unsafe static string FormatBigIntegerToHex(bool targetSpan, BigInteger value, char format, int digits, NumberFormatInfo info, Span<char> destination, out int charsWritten, out bool spanSuccess)
		{
			byte[] array = null;
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)64], 64);
			int num;
			if (!value.TryWriteOrCountBytes(span, out num, false, false))
			{
				span = (array = ArrayPool<byte>.Shared.Rent(num));
				value.TryWriteBytes(span, out num, false, false);
			}
			span = span.Slice(0, num);
			Span<char> span2 = new Span<char>(stackalloc byte[(UIntPtr)256], 128);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(span2);
			int i = span.Length - 1;
			if (i > -1)
			{
				bool flag = false;
				byte b = *span[i];
				if (b > 247)
				{
					b -= 240;
					flag = true;
				}
				if (b < 8 || flag)
				{
					valueStringBuilder.Append((b < 10) ? ((char)(b + 48)) : ((format == 'X') ? ((char)((b & 15) - 10 + 65)) : ((char)((b & 15) - 10 + 97))));
					i--;
				}
			}
			if (i > -1)
			{
				Span<char> span3 = valueStringBuilder.AppendSpan((i + 1) * 2);
				int num2 = 0;
				string text = ((format == 'x') ? "0123456789abcdef" : "0123456789ABCDEF");
				while (i > -1)
				{
					byte b2 = *span[i--];
					*span3[num2++] = text[b2 >> 4];
					*span3[num2++] = text[(int)(b2 & 15)];
				}
			}
			if (digits > valueStringBuilder.Length)
			{
				valueStringBuilder.Insert(0, (value._sign >= 0) ? '0' : ((format == 'x') ? 'f' : 'F'), digits - valueStringBuilder.Length);
			}
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
			if (targetSpan)
			{
				spanSuccess = valueStringBuilder.TryCopyTo(destination, out charsWritten);
				return null;
			}
			charsWritten = 0;
			spanSuccess = false;
			return valueStringBuilder.ToString();
		}

		internal static string FormatBigInteger(BigInteger value, string format, NumberFormatInfo info)
		{
			int num;
			bool flag;
			return BigNumber.FormatBigInteger(false, value, format, format, info, default(Span<char>), out num, out flag);
		}

		internal static bool TryFormatBigInteger(BigInteger value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
		{
			bool flag;
			BigNumber.FormatBigInteger(true, value, null, format, info, destination, out charsWritten, out flag);
			return flag;
		}

		private unsafe static string FormatBigInteger(bool targetSpan, BigInteger value, string formatString, ReadOnlySpan<char> formatSpan, NumberFormatInfo info, Span<char> destination, out int charsWritten, out bool spanSuccess)
		{
			int num = 0;
			char c = BigNumber.ParseFormatSpecifier(formatSpan, out num);
			if (c == 'x' || c == 'X')
			{
				return BigNumber.FormatBigIntegerToHex(targetSpan, value, c, num, info, destination, out charsWritten, out spanSuccess);
			}
			if (value._bits == null)
			{
				if (c == 'g' || c == 'G' || c == 'r' || c == 'R')
				{
					formatSpan = (formatString = ((num > 0) ? string.Format("D{0}", num) : "D"));
				}
				if (targetSpan)
				{
					spanSuccess = value._sign.TryFormat(destination, out charsWritten, formatSpan, info);
					return null;
				}
				charsWritten = 0;
				spanSuccess = false;
				return value._sign.ToString(formatString, info);
			}
			else
			{
				int num2 = value._bits.Length;
				int num3;
				try
				{
					num3 = checked(num2 * 10 / 9 + 2);
				}
				catch (OverflowException ex)
				{
					throw new FormatException("The value is too large to be represented by this format specifier.", ex);
				}
				uint[] array = new uint[num3];
				int num4 = 0;
				int num5 = num2;
				while (--num5 >= 0)
				{
					uint num6 = value._bits[num5];
					for (int i = 0; i < num4; i++)
					{
						ulong num7 = NumericsHelpers.MakeUlong(array[i], num6);
						array[i] = (uint)(num7 % 1000000000UL);
						num6 = (uint)(num7 / 1000000000UL);
					}
					if (num6 != 0U)
					{
						array[num4++] = num6 % 1000000000U;
						num6 /= 1000000000U;
						if (num6 != 0U)
						{
							array[num4++] = num6;
						}
					}
				}
				int num8;
				bool flag;
				char[] array2;
				int num10;
				checked
				{
					try
					{
						num8 = num4 * 9;
					}
					catch (OverflowException ex2)
					{
						throw new FormatException("The value is too large to be represented by this format specifier.", ex2);
					}
					flag = c == 'g' || c == 'G' || c == 'd' || c == 'D' || c == 'r' || c == 'R';
					if (flag)
					{
						if (num > 0 && num > num8)
						{
							num8 = num;
						}
						if (value._sign < 0)
						{
							try
							{
								num8 += info.NegativeSign.Length;
							}
							catch (OverflowException ex3)
							{
								throw new FormatException("The value is too large to be represented by this format specifier.", ex3);
							}
						}
					}
					int num9;
					try
					{
						num9 = num8 + 1;
					}
					catch (OverflowException ex4)
					{
						throw new FormatException("The value is too large to be represented by this format specifier.", ex4);
					}
					array2 = new char[num9];
					num10 = num8;
				}
				for (int j = 0; j < num4 - 1; j++)
				{
					uint num11 = array[j];
					int num12 = 9;
					while (--num12 >= 0)
					{
						array2[--num10] = (char)(48U + num11 % 10U);
						num11 /= 10U;
					}
				}
				for (uint num13 = array[num4 - 1]; num13 != 0U; num13 /= 10U)
				{
					array2[--num10] = (char)(48U + num13 % 10U);
				}
				if (!flag)
				{
					bool flag2 = value._sign < 0;
					int num14 = 29;
					int num15 = num8 - num10;
					Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)256], 128);
					ValueStringBuilder valueStringBuilder = new ValueStringBuilder(span);
					FormatProvider.FormatBigInteger(ref valueStringBuilder, num14, num15, flag2, formatSpan, info, array2, num10);
					if (targetSpan)
					{
						spanSuccess = valueStringBuilder.TryCopyTo(destination, out charsWritten);
						return null;
					}
					charsWritten = 0;
					spanSuccess = false;
					return valueStringBuilder.ToString();
				}
				else
				{
					int num16 = num8 - num10;
					while (num > 0 && num > num16)
					{
						array2[--num10] = '0';
						num--;
					}
					if (value._sign < 0)
					{
						string negativeSign = info.NegativeSign;
						for (int k = info.NegativeSign.Length - 1; k > -1; k--)
						{
							array2[--num10] = info.NegativeSign[k];
						}
					}
					int num17 = num8 - num10;
					if (!targetSpan)
					{
						charsWritten = 0;
						spanSuccess = false;
						return new string(array2, num10, num8 - num10);
					}
					if (new ReadOnlySpan<char>(array2, num10, num8 - num10).TryCopyTo(destination))
					{
						charsWritten = num17;
						spanSuccess = true;
						return null;
					}
					charsWritten = 0;
					spanSuccess = false;
					return null;
				}
			}
		}

		private const NumberStyles InvalidNumberStyles = ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowHexSpecifier);

		private struct BigNumberBuffer
		{
			public static BigNumber.BigNumberBuffer Create()
			{
				return new BigNumber.BigNumberBuffer
				{
					digits = new StringBuilder()
				};
			}

			public StringBuilder digits;

			public int precision;

			public int scale;

			public bool sign;
		}
	}
}
