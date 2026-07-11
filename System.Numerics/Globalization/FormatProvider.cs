using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

namespace System.Globalization
{
	internal class FormatProvider
	{
		[SecurityCritical]
		internal unsafe static string FormatBigInteger(int precision, int scale, bool sign, string format, NumberFormatInfo numberFormatInfo, char[] digits, int startIndex)
		{
			int num;
			char c = FormatProvider.Number.ParseFormatSpecifier(format, out num);
			char* ptr;
			if (digits == null || digits.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &digits[0];
			}
			FormatProvider.Number.NumberBuffer numberBuffer = default(FormatProvider.Number.NumberBuffer);
			numberBuffer.overrideDigits = ptr + startIndex;
			numberBuffer.precision = precision;
			numberBuffer.scale = scale;
			numberBuffer.sign = sign;
			if (c != '\0')
			{
				return FormatProvider.Number.NumberToString(numberBuffer, c, num, numberFormatInfo, false);
			}
			return FormatProvider.Number.NumberToStringFormat(numberBuffer, format, numberFormatInfo);
		}

		[SecurityCritical]
		internal static bool TryStringToBigInteger(ReadOnlySpan<char> s, NumberStyles styles, NumberFormatInfo numberFormatInfo, StringBuilder receiver, out int precision, out int scale, out bool sign)
		{
			FormatProvider.Number.NumberBuffer numberBuffer = default(FormatProvider.Number.NumberBuffer);
			numberBuffer.overrideDigits = 1;
			if (!FormatProvider.Number.TryStringToNumber(s, styles, ref numberBuffer, receiver, numberFormatInfo, false))
			{
				precision = 0;
				scale = 0;
				sign = false;
				return false;
			}
			precision = numberBuffer.precision;
			scale = numberBuffer.scale;
			sign = numberBuffer.sign;
			return true;
		}

		private class Number
		{
			private Number()
			{
			}

			private static bool IsWhite(char ch)
			{
				return ch == ' ' || (ch >= '\t' && ch <= '\r');
			}

			private unsafe static char* MatchChars(char* p, string str)
			{
				char* ptr = str;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				return FormatProvider.Number.MatchChars(p, ptr);
			}

			private unsafe static char* MatchChars(char* p, char* str)
			{
				if (*str == '\0')
				{
					return null;
				}
				while (*p == *str || (*str == '\u00a0' && *p == ' '))
				{
					p++;
					str++;
					if (*str == '\0')
					{
						return p;
					}
				}
				return null;
			}

			private unsafe static bool ParseNumber(ref char* str, NumberStyles options, ref FormatProvider.Number.NumberBuffer number, StringBuilder sb, NumberFormatInfo numfmt, bool parseDecimal)
			{
				number.scale = 0;
				number.sign = false;
				string text = null;
				bool flag = false;
				string text2;
				string text3;
				if ((options & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
				{
					text = numfmt.CurrencySymbol;
					text2 = numfmt.CurrencyDecimalSeparator;
					text3 = numfmt.CurrencyGroupSeparator;
					flag = true;
				}
				else
				{
					text2 = numfmt.NumberDecimalSeparator;
					text3 = numfmt.NumberGroupSeparator;
				}
				int num = 0;
				bool flag2 = sb != null;
				int num2 = (flag2 ? int.MaxValue : 32);
				char* ptr = str;
				char c = *ptr;
				char* digits = number.digits;
				for (;;)
				{
					if (!FormatProvider.Number.IsWhite(c) || (options & NumberStyles.AllowLeadingWhite) == NumberStyles.None || ((num & 1) != 0 && (num & 32) == 0 && numfmt.NumberNegativePattern != 2))
					{
						char* ptr2;
						if ((options & NumberStyles.AllowLeadingSign) != NumberStyles.None && (num & 1) == 0 && ((ptr2 = FormatProvider.Number.MatchChars(ptr, numfmt.PositiveSign)) != null || ((ptr2 = FormatProvider.Number.MatchChars(ptr, numfmt.NegativeSign)) != null && (number.sign = true))))
						{
							num |= 1;
							ptr = ptr2 - 1;
						}
						else if (c == '(' && (options & NumberStyles.AllowParentheses) != NumberStyles.None && (num & 1) == 0)
						{
							num |= 3;
							number.sign = true;
						}
						else
						{
							if (text == null || (ptr2 = FormatProvider.Number.MatchChars(ptr, text)) == null)
							{
								break;
							}
							num |= 32;
							text = null;
							ptr = ptr2 - 1;
						}
					}
					c = *(++ptr);
				}
				int num3 = 0;
				int num4 = 0;
				for (;;)
				{
					char* ptr2;
					if ((c >= '0' && c <= '9') || ((options & NumberStyles.AllowHexSpecifier) != NumberStyles.None && ((c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))))
					{
						num |= 4;
						if (c != '0' || (num & 8) != 0 || (flag2 && (options & NumberStyles.AllowHexSpecifier) != NumberStyles.None))
						{
							if (num3 < num2)
							{
								if (flag2)
								{
									sb.Append(c);
								}
								else
								{
									digits[(IntPtr)(num3++) * 2] = c;
								}
								if (c != '0' || parseDecimal)
								{
									num4 = num3;
								}
							}
							if ((num & 16) == 0)
							{
								number.scale++;
							}
							num |= 8;
						}
						else if ((num & 16) != 0)
						{
							number.scale--;
						}
					}
					else if ((options & NumberStyles.AllowDecimalPoint) != NumberStyles.None && (num & 16) == 0 && ((ptr2 = FormatProvider.Number.MatchChars(ptr, text2)) != null || (flag && (num & 32) == 0 && (ptr2 = FormatProvider.Number.MatchChars(ptr, numfmt.NumberDecimalSeparator)) != null)))
					{
						num |= 16;
						ptr = ptr2 - 1;
					}
					else
					{
						if ((options & NumberStyles.AllowThousands) == NumberStyles.None || (num & 4) == 0 || (num & 16) != 0 || ((ptr2 = FormatProvider.Number.MatchChars(ptr, text3)) == null && (!flag || (num & 32) != 0 || (ptr2 = FormatProvider.Number.MatchChars(ptr, numfmt.NumberGroupSeparator)) == null)))
						{
							break;
						}
						ptr = ptr2 - 1;
					}
					c = *(++ptr);
				}
				bool flag3 = false;
				number.precision = num4;
				if (flag2)
				{
					sb.Append('\0');
				}
				else
				{
					digits[num4] = '\0';
				}
				if ((num & 4) != 0)
				{
					if ((c == 'E' || c == 'e') && (options & NumberStyles.AllowExponent) != NumberStyles.None)
					{
						char* ptr3 = ptr;
						c = *(++ptr);
						char* ptr2;
						if ((ptr2 = FormatProvider.Number.MatchChars(ptr, numfmt.PositiveSign)) != null)
						{
							c = *(ptr = ptr2);
						}
						else if ((ptr2 = FormatProvider.Number.MatchChars(ptr, numfmt.NegativeSign)) != null)
						{
							c = *(ptr = ptr2);
							flag3 = true;
						}
						if (c >= '0' && c <= '9')
						{
							int num5 = 0;
							do
							{
								num5 = num5 * 10 + (int)(c - '0');
								c = *(++ptr);
								if (num5 > 1000)
								{
									num5 = 9999;
									while (c >= '0' && c <= '9')
									{
										c = *(++ptr);
									}
								}
							}
							while (c >= '0' && c <= '9');
							if (flag3)
							{
								num5 = -num5;
							}
							number.scale += num5;
						}
						else
						{
							ptr = ptr3;
							c = *ptr;
						}
					}
					for (;;)
					{
						if (!FormatProvider.Number.IsWhite(c) || (options & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
						{
							char* ptr2;
							if ((options & NumberStyles.AllowTrailingSign) != NumberStyles.None && (num & 1) == 0 && ((ptr2 = FormatProvider.Number.MatchChars(ptr, numfmt.PositiveSign)) != null || ((ptr2 = FormatProvider.Number.MatchChars(ptr, numfmt.NegativeSign)) != null && (number.sign = true))))
							{
								num |= 1;
								ptr = ptr2 - 1;
							}
							else if (c == ')' && (num & 2) != 0)
							{
								num &= -3;
							}
							else
							{
								if (text == null || (ptr2 = FormatProvider.Number.MatchChars(ptr, text)) == null)
								{
									break;
								}
								text = null;
								ptr = ptr2 - 1;
							}
						}
						c = *(++ptr);
					}
					if ((num & 2) == 0)
					{
						if ((num & 8) == 0)
						{
							if (!parseDecimal)
							{
								number.scale = 0;
							}
							if ((num & 16) == 0)
							{
								number.sign = false;
							}
						}
						str = ptr;
						return true;
					}
				}
				str = ptr;
				return false;
			}

			private static bool TrailingZeros(ReadOnlySpan<char> s, int index)
			{
				for (int i = index; i < s.Length; i++)
				{
					if (s[i] != '\0')
					{
						return false;
					}
				}
				return true;
			}

			internal unsafe static bool TryStringToNumber(ReadOnlySpan<char> str, NumberStyles options, ref FormatProvider.Number.NumberBuffer number, StringBuilder sb, NumberFormatInfo numfmt, bool parseDecimal)
			{
				fixed (char* ptr = str.DangerousGetPinnableReference())
				{
					char* ptr2 = ptr;
					char* ptr3 = ptr2;
					if (!FormatProvider.Number.ParseNumber(ref ptr3, options, ref number, sb, numfmt, parseDecimal) || ((long)(ptr3 - ptr2) < (long)str.Length && !FormatProvider.Number.TrailingZeros(str, (int)((long)(ptr3 - ptr2)))))
					{
						return false;
					}
				}
				return true;
			}

			internal unsafe static void Int32ToDecChars(char* buffer, ref int index, uint value, int digits)
			{
				while (--digits >= 0 || value != 0U)
				{
					int num = index - 1;
					index = num;
					buffer[num] = (char)(value % 10U + 48U);
					value /= 10U;
				}
			}

			internal unsafe static char ParseFormatSpecifier(string format, out int digits)
			{
				if (format != null)
				{
					fixed (string text = format)
					{
						char* ptr = text;
						if (ptr != null)
						{
							ptr += RuntimeHelpers.OffsetToStringData / 2;
						}
						int num = 0;
						char c = ptr[num];
						if (c != '\0')
						{
							if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
							{
								num++;
								int num2 = -1;
								if (ptr[num] >= '0' && ptr[num] <= '9')
								{
									num2 = (int)(ptr[(IntPtr)(num++) * 2] - '0');
									while (ptr[num] >= '0' && ptr[num] <= '9')
									{
										num2 = num2 * 10 + (int)ptr[(IntPtr)(num++) * 2] - 48;
										if (num2 >= 10)
										{
											break;
										}
									}
								}
								if (ptr[num] == '\0')
								{
									digits = num2;
									return c;
								}
							}
							digits = -1;
							return '\0';
						}
					}
				}
				digits = -1;
				return 'G';
			}

			internal unsafe static string NumberToString(FormatProvider.Number.NumberBuffer number, char format, int nMaxDigits, NumberFormatInfo info, bool isDecimal)
			{
				StringBuilder stringBuilder = new StringBuilder(105);
				if (format <= 'P')
				{
					switch (format)
					{
					case 'C':
						break;
					case 'D':
						goto IL_0212;
					case 'E':
						goto IL_0128;
					case 'F':
						goto IL_00AB;
					case 'G':
						goto IL_0167;
					default:
						if (format == 'N')
						{
							goto IL_00F8;
						}
						if (format != 'P')
						{
							goto IL_0212;
						}
						goto IL_01D9;
					}
				}
				else
				{
					switch (format)
					{
					case 'c':
						break;
					case 'd':
						goto IL_0212;
					case 'e':
						goto IL_0128;
					case 'f':
						goto IL_00AB;
					case 'g':
						goto IL_0167;
					default:
						if (format == 'n')
						{
							goto IL_00F8;
						}
						if (format != 'p')
						{
							goto IL_0212;
						}
						goto IL_01D9;
					}
				}
				int num = ((nMaxDigits >= 0) ? nMaxDigits : info.CurrencyDecimalDigits);
				if (nMaxDigits < 0)
				{
					nMaxDigits = info.CurrencyDecimalDigits;
				}
				FormatProvider.Number.RoundNumber(ref number, number.scale + nMaxDigits);
				FormatProvider.Number.FormatCurrency(stringBuilder, number, num, nMaxDigits, info);
				goto IL_021D;
				IL_00AB:
				if (nMaxDigits < 0)
				{
					num = (nMaxDigits = info.NumberDecimalDigits);
				}
				else
				{
					num = nMaxDigits;
				}
				FormatProvider.Number.RoundNumber(ref number, number.scale + nMaxDigits);
				if (number.sign)
				{
					stringBuilder.Append(info.NegativeSign);
				}
				FormatProvider.Number.FormatFixed(stringBuilder, number, num, nMaxDigits, info, null, info.NumberDecimalSeparator, null);
				goto IL_021D;
				IL_00F8:
				if (nMaxDigits < 0)
				{
					num = (nMaxDigits = info.NumberDecimalDigits);
				}
				else
				{
					num = nMaxDigits;
				}
				FormatProvider.Number.RoundNumber(ref number, number.scale + nMaxDigits);
				FormatProvider.Number.FormatNumber(stringBuilder, number, num, nMaxDigits, info);
				goto IL_021D;
				IL_0128:
				if (nMaxDigits < 0)
				{
					num = (nMaxDigits = 6);
				}
				else
				{
					num = nMaxDigits;
				}
				nMaxDigits++;
				FormatProvider.Number.RoundNumber(ref number, nMaxDigits);
				if (number.sign)
				{
					stringBuilder.Append(info.NegativeSign);
				}
				FormatProvider.Number.FormatScientific(stringBuilder, number, num, nMaxDigits, info, format);
				goto IL_021D;
				IL_0167:
				bool flag = true;
				if (nMaxDigits < 1)
				{
					if (isDecimal && nMaxDigits == -1)
					{
						num = (nMaxDigits = 29);
						flag = false;
					}
					else
					{
						num = (nMaxDigits = number.precision);
					}
				}
				else
				{
					num = nMaxDigits;
				}
				if (flag)
				{
					FormatProvider.Number.RoundNumber(ref number, nMaxDigits);
				}
				else if (isDecimal && *number.digits == '\0')
				{
					number.sign = false;
				}
				if (number.sign)
				{
					stringBuilder.Append(info.NegativeSign);
				}
				FormatProvider.Number.FormatGeneral(stringBuilder, number, num, nMaxDigits, info, format - '\u0002', !flag);
				goto IL_021D;
				IL_01D9:
				if (nMaxDigits < 0)
				{
					num = (nMaxDigits = info.PercentDecimalDigits);
				}
				else
				{
					num = nMaxDigits;
				}
				number.scale += 2;
				FormatProvider.Number.RoundNumber(ref number, number.scale + nMaxDigits);
				FormatProvider.Number.FormatPercent(stringBuilder, number, num, nMaxDigits, info);
				goto IL_021D;
				IL_0212:
				throw new FormatException("Format specifier was invalid.");
				IL_021D:
				return stringBuilder.ToString();
			}

			private static void FormatCurrency(StringBuilder sb, FormatProvider.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info)
			{
				foreach (char c in number.sign ? FormatProvider.Number.s_negCurrencyFormats[info.CurrencyNegativePattern] : FormatProvider.Number.s_posCurrencyFormats[info.CurrencyPositivePattern])
				{
					if (c != '#')
					{
						if (c != '$')
						{
							if (c != '-')
							{
								sb.Append(c);
							}
							else
							{
								sb.Append(info.NegativeSign);
							}
						}
						else
						{
							sb.Append(info.CurrencySymbol);
						}
					}
					else
					{
						FormatProvider.Number.FormatFixed(sb, number, nMinDigits, nMaxDigits, info, info.CurrencyGroupSizes, info.CurrencyDecimalSeparator, info.CurrencyGroupSeparator);
					}
				}
			}

			private unsafe static int wcslen(char* s)
			{
				int num = 0;
				while (*(s++) != '\0')
				{
					num++;
				}
				return num;
			}

			private unsafe static void FormatFixed(StringBuilder sb, FormatProvider.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info, int[] groupDigits, string sDecimal, string sGroup)
			{
				int i = number.scale;
				char* ptr = number.digits;
				int num = FormatProvider.Number.wcslen(ptr);
				if (i > 0)
				{
					if (groupDigits != null)
					{
						int num2 = 0;
						int num3 = groupDigits[num2];
						int num4 = groupDigits.Length;
						int num5 = i;
						int length = sGroup.Length;
						int num6 = 0;
						if (num4 != 0)
						{
							while (i > num3)
							{
								num6 = groupDigits[num2];
								if (num6 == 0)
								{
									break;
								}
								num5 += length;
								if (num2 < num4 - 1)
								{
									num2++;
								}
								num3 += groupDigits[num2];
								if (num3 < 0 || num5 < 0)
								{
									throw new ArgumentOutOfRangeException();
								}
							}
							if (num3 == 0)
							{
								num6 = 0;
							}
							else
							{
								num6 = groupDigits[0];
							}
						}
						char* ptr2;
						int num7;
						int num8;
						checked
						{
							ptr2 = stackalloc char[unchecked((UIntPtr)num5) * 2];
							num2 = 0;
							num7 = 0;
							num8 = ((i < num) ? i : num);
						}
						char* ptr3 = ptr2 + num5 - 1;
						for (int j = i - 1; j >= 0; j--)
						{
							*(ptr3--) = ((j < num8) ? ptr[j] : '0');
							if (num6 > 0)
							{
								num7++;
								if (num7 == num6 && j != 0)
								{
									for (int k = length - 1; k >= 0; k--)
									{
										*(ptr3--) = sGroup[k];
									}
									if (num2 < num4 - 1)
									{
										num2++;
										num6 = groupDigits[num2];
									}
									num7 = 0;
								}
							}
						}
						sb.Append(ptr2, num5);
						ptr += num8;
					}
					else
					{
						int num9 = Math.Min(num, i);
						sb.Append(ptr, num9);
						ptr += num9;
						if (i > num)
						{
							sb.Append('0', i - num);
						}
					}
				}
				else
				{
					sb.Append('0');
				}
				if (nMaxDigits > 0)
				{
					sb.Append(sDecimal);
					if (i < 0 && nMaxDigits > 0)
					{
						int num10 = Math.Min(-i, nMaxDigits);
						sb.Append('0', num10);
						i += num10;
						nMaxDigits -= num10;
					}
					while (nMaxDigits > 0)
					{
						sb.Append((*ptr != '\0') ? (*(ptr++)) : '0');
						nMaxDigits--;
					}
				}
			}

			private static void FormatNumber(StringBuilder sb, FormatProvider.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info)
			{
				foreach (char c in number.sign ? FormatProvider.Number.s_negNumberFormats[info.NumberNegativePattern] : FormatProvider.Number.s_posNumberFormat)
				{
					if (c != '#')
					{
						if (c != '-')
						{
							sb.Append(c);
						}
						else
						{
							sb.Append(info.NegativeSign);
						}
					}
					else
					{
						FormatProvider.Number.FormatFixed(sb, number, nMinDigits, nMaxDigits, info, info.NumberGroupSizes, info.NumberDecimalSeparator, info.NumberGroupSeparator);
					}
				}
			}

			private unsafe static void FormatScientific(StringBuilder sb, FormatProvider.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info, char expChar)
			{
				char* digits = number.digits;
				sb.Append((*digits != '\0') ? (*(digits++)) : '0');
				if (nMaxDigits != 1)
				{
					sb.Append(info.NumberDecimalSeparator);
				}
				while (--nMaxDigits > 0)
				{
					sb.Append((*digits != '\0') ? (*(digits++)) : '0');
				}
				int num = ((*number.digits == '\0') ? 0 : (number.scale - 1));
				FormatProvider.Number.FormatExponent(sb, info, num, expChar, 3, true);
			}

			private unsafe static void FormatExponent(StringBuilder sb, NumberFormatInfo info, int value, char expChar, int minDigits, bool positiveSign)
			{
				sb.Append(expChar);
				if (value < 0)
				{
					sb.Append(info.NegativeSign);
					value = -value;
				}
				else if (positiveSign)
				{
					sb.Append(info.PositiveSign);
				}
				char* ptr = stackalloc char[(UIntPtr)22];
				int num = 10;
				FormatProvider.Number.Int32ToDecChars(ptr, ref num, (uint)value, minDigits);
				int num2 = 10 - num;
				while (--num2 >= 0)
				{
					sb.Append(ptr[(IntPtr)(num++) * 2]);
				}
			}

			private unsafe static void FormatGeneral(StringBuilder sb, FormatProvider.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info, char expChar, bool bSuppressScientific)
			{
				int i = number.scale;
				bool flag = false;
				if (!bSuppressScientific && (i > nMaxDigits || i < -3))
				{
					i = 1;
					flag = true;
				}
				char* digits = number.digits;
				if (i > 0)
				{
					do
					{
						sb.Append((*digits != '\0') ? (*(digits++)) : '0');
					}
					while (--i > 0);
				}
				else
				{
					sb.Append('0');
				}
				if (*digits != '\0' || i < 0)
				{
					sb.Append(info.NumberDecimalSeparator);
					while (i < 0)
					{
						sb.Append('0');
						i++;
					}
					while (*digits != '\0')
					{
						sb.Append(*(digits++));
					}
				}
				if (flag)
				{
					FormatProvider.Number.FormatExponent(sb, info, number.scale - 1, expChar, 2, true);
				}
			}

			private static void FormatPercent(StringBuilder sb, FormatProvider.Number.NumberBuffer number, int nMinDigits, int nMaxDigits, NumberFormatInfo info)
			{
				foreach (char c in number.sign ? FormatProvider.Number.s_negPercentFormats[info.PercentNegativePattern] : FormatProvider.Number.s_posPercentFormats[info.PercentPositivePattern])
				{
					if (c != '#')
					{
						if (c != '%')
						{
							if (c != '-')
							{
								sb.Append(c);
							}
							else
							{
								sb.Append(info.NegativeSign);
							}
						}
						else
						{
							sb.Append(info.PercentSymbol);
						}
					}
					else
					{
						FormatProvider.Number.FormatFixed(sb, number, nMinDigits, nMaxDigits, info, info.PercentGroupSizes, info.PercentDecimalSeparator, info.PercentGroupSeparator);
					}
				}
			}

			private unsafe static void RoundNumber(ref FormatProvider.Number.NumberBuffer number, int pos)
			{
				char* digits = number.digits;
				int num = 0;
				while (num < pos && digits[num] != '\0')
				{
					num++;
				}
				if (num == pos && digits[num] >= '5')
				{
					while (num > 0 && digits[num - 1] == '9')
					{
						num--;
					}
					if (num > 0)
					{
						char* ptr = digits + (num - 1);
						*ptr += '\u0001';
					}
					else
					{
						number.scale++;
						*digits = '1';
						num = 1;
					}
				}
				else
				{
					while (num > 0 && digits[num - 1] == '0')
					{
						num--;
					}
				}
				if (num == 0)
				{
					number.scale = 0;
					number.sign = false;
				}
				digits[num] = '\0';
			}

			private unsafe static int FindSection(string format, int section)
			{
				if (section == 0)
				{
					return 0;
				}
				char* ptr = format;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				int num = 0;
				for (;;)
				{
					char c2;
					char c = (c2 = ptr[(IntPtr)(num++) * 2]);
					if (c2 <= '"')
					{
						if (c2 == '\0')
						{
							return 0;
						}
						if (c2 != '"')
						{
							continue;
						}
					}
					else if (c2 != '\'')
					{
						if (c2 != ';')
						{
							if (c2 != '\\')
							{
								continue;
							}
							if (ptr[num] != '\0')
							{
								num++;
								continue;
							}
							continue;
						}
						else
						{
							if (--section == 0)
							{
								break;
							}
							continue;
						}
					}
					while (ptr[num] != '\0')
					{
						if (ptr[(IntPtr)(num++) * 2] == c)
						{
							break;
						}
					}
				}
				if (ptr[num] != '\0' && ptr[num] != ';')
				{
					return num;
				}
				return 0;
			}

			internal unsafe static string NumberToStringFormat(FormatProvider.Number.NumberBuffer number, string format, NumberFormatInfo info)
			{
				int num = 0;
				char* digits = number.digits;
				int num2 = FormatProvider.Number.FindSection(format, (*digits == '\0') ? 2 : (number.sign ? 1 : 0));
				int num3;
				int num4;
				int num5;
				int num6;
				bool flag;
				bool flag2;
				int num9;
				for (;;)
				{
					num3 = 0;
					num4 = -1;
					num5 = int.MaxValue;
					num6 = 0;
					flag = false;
					int num7 = -1;
					flag2 = false;
					int num8 = 0;
					num9 = num2;
					fixed (string text = format)
					{
						char* ptr = text;
						if (ptr != null)
						{
							ptr += RuntimeHelpers.OffsetToStringData / 2;
						}
						char c;
						while ((c = ptr[(IntPtr)(num9++) * 2]) != '\0' && c != ';')
						{
							if (c <= 'E')
							{
								switch (c)
								{
								case '"':
								case '\'':
									while (ptr[num9] != '\0')
									{
										if (ptr[(IntPtr)(num9++) * 2] == c)
										{
											break;
										}
									}
									continue;
								case '#':
									num3++;
									continue;
								case '$':
								case '&':
									continue;
								case '%':
									num8 += 2;
									continue;
								default:
									switch (c)
									{
									case ',':
										if (num3 > 0 && num4 < 0)
										{
											if (num7 >= 0)
											{
												if (num7 == num3)
												{
													num++;
													continue;
												}
												flag2 = true;
											}
											num7 = num3;
											num = 1;
											continue;
										}
										continue;
									case '-':
									case '/':
										continue;
									case '.':
										if (num4 < 0)
										{
											num4 = num3;
											continue;
										}
										continue;
									case '0':
										if (num5 == 2147483647)
										{
											num5 = num3;
										}
										num3++;
										num6 = num3;
										continue;
									default:
										if (c != 'E')
										{
											continue;
										}
										break;
									}
									break;
								}
							}
							else if (c != '\\')
							{
								if (c != 'e')
								{
									if (c != '‰')
									{
										continue;
									}
									num8 += 3;
									continue;
								}
							}
							else
							{
								if (ptr[num9] != '\0')
								{
									num9++;
									continue;
								}
								continue;
							}
							if (ptr[num9] == '0' || ((ptr[num9] == '+' || ptr[num9] == '-') && ptr[num9 + 1] == '0'))
							{
								while (ptr[(IntPtr)(++num9) * 2] == '0')
								{
								}
								flag = true;
							}
						}
					}
					if (num4 < 0)
					{
						num4 = num3;
					}
					if (num7 >= 0)
					{
						if (num7 == num4)
						{
							num8 -= num * 3;
						}
						else
						{
							flag2 = true;
						}
					}
					if (*digits == '\0')
					{
						break;
					}
					number.scale += num8;
					int num10 = (flag ? num3 : (number.scale + num3 - num4));
					FormatProvider.Number.RoundNumber(ref number, num10);
					if (*digits != '\0')
					{
						goto IL_025B;
					}
					num9 = FormatProvider.Number.FindSection(format, 2);
					if (num9 == num2)
					{
						goto IL_025B;
					}
					num2 = num9;
				}
				number.sign = false;
				number.scale = 0;
				IL_025B:
				num5 = ((num5 < num4) ? (num4 - num5) : 0);
				num6 = ((num6 > num4) ? (num4 - num6) : 0);
				int num11;
				int i;
				if (flag)
				{
					num11 = num4;
					i = 0;
				}
				else
				{
					num11 = ((number.scale > num4) ? number.scale : num4);
					i = number.scale - num4;
				}
				num9 = num2;
				int[] array = new int[4];
				int num12 = -1;
				if (flag2 && info.NumberGroupSeparator.Length > 0)
				{
					int[] numberGroupSizes = info.NumberGroupSizes;
					int num13 = 0;
					int num14 = 0;
					int num15 = numberGroupSizes.Length;
					if (num15 != 0)
					{
						num14 = numberGroupSizes[num13];
					}
					int num16 = num14;
					int num17 = num11 + ((i < 0) ? i : 0);
					int num18 = ((num5 > num17) ? num5 : num17);
					while (num18 > num14 && num16 != 0)
					{
						num12++;
						if (num12 >= array.Length)
						{
							Array.Resize<int>(ref array, array.Length * 2);
						}
						array[num12] = num14;
						if (num13 < num15 - 1)
						{
							num13++;
							num16 = numberGroupSizes[num13];
						}
						num14 += num16;
					}
				}
				StringBuilder stringBuilder = new StringBuilder(105);
				if (number.sign && num2 == 0)
				{
					stringBuilder.Append(info.NegativeSign);
				}
				bool flag3 = false;
				fixed (string text = format)
				{
					char* ptr2 = text;
					if (ptr2 != null)
					{
						ptr2 += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr3 = digits;
					char c;
					while ((c = ptr2[(IntPtr)(num9++) * 2]) != '\0' && c != ';')
					{
						if (i > 0)
						{
							if (c == '#' || c == '.' || c == '0')
							{
								while (i > 0)
								{
									stringBuilder.Append((*ptr3 != '\0') ? (*(ptr3++)) : '0');
									if (flag2 && num11 > 1 && num12 >= 0 && num11 == array[num12] + 1)
									{
										stringBuilder.Append(info.NumberGroupSeparator);
										num12--;
									}
									num11--;
									i--;
								}
							}
						}
						if (c <= 'E')
						{
							switch (c)
							{
							case '"':
							case '\'':
								while (ptr2[num9] != '\0' && ptr2[num9] != c)
								{
									stringBuilder.Append(ptr2[(IntPtr)(num9++) * 2]);
								}
								if (ptr2[num9] != '\0')
								{
									num9++;
									continue;
								}
								continue;
							case '#':
								break;
							case '$':
							case '&':
								goto IL_06D4;
							case '%':
								stringBuilder.Append(info.PercentSymbol);
								continue;
							default:
								switch (c)
								{
								case ',':
									continue;
								case '-':
								case '/':
									goto IL_06D4;
								case '.':
									if (num11 == 0 && !flag3 && (num6 < 0 || (num4 < num3 && *ptr3 != '\0')))
									{
										stringBuilder.Append(info.NumberDecimalSeparator);
										flag3 = true;
										continue;
									}
									continue;
								case '0':
									break;
								default:
									if (c != 'E')
									{
										goto IL_06D4;
									}
									goto IL_05BC;
								}
								break;
							}
							if (i < 0)
							{
								i++;
								c = ((num11 <= num5) ? '0' : '\0');
							}
							else
							{
								c = ((*ptr3 != '\0') ? (*(ptr3++)) : ((num11 > num6) ? '0' : '\0'));
							}
							if (c != '\0')
							{
								stringBuilder.Append(c);
								if (flag2 && num11 > 1 && num12 >= 0 && num11 == array[num12] + 1)
								{
									stringBuilder.Append(info.NumberGroupSeparator);
									num12--;
								}
							}
							num11--;
							continue;
						}
						if (c != '\\')
						{
							if (c != 'e')
							{
								if (c != '‰')
								{
									goto IL_06D4;
								}
								stringBuilder.Append(info.PerMilleSymbol);
								continue;
							}
						}
						else
						{
							if (ptr2[num9] != '\0')
							{
								stringBuilder.Append(ptr2[(IntPtr)(num9++) * 2]);
								continue;
							}
							continue;
						}
						IL_05BC:
						bool flag4 = false;
						int num19 = 0;
						if (flag)
						{
							if (ptr2[num9] == '0')
							{
								num19++;
							}
							else if (ptr2[num9] == '+' && ptr2[num9 + 1] == '0')
							{
								flag4 = true;
							}
							else if (ptr2[num9] != '-' || ptr2[num9 + 1] != '0')
							{
								stringBuilder.Append(c);
								continue;
							}
							while (ptr2[(IntPtr)(++num9) * 2] == '0')
							{
								num19++;
							}
							if (num19 > 10)
							{
								num19 = 10;
							}
							int num20 = ((*digits == '\0') ? 0 : (number.scale - num4));
							FormatProvider.Number.FormatExponent(stringBuilder, info, num20, c, num19, flag4);
							flag = false;
							continue;
						}
						stringBuilder.Append(c);
						if (ptr2[num9] == '+' || ptr2[num9] == '-')
						{
							stringBuilder.Append(ptr2[(IntPtr)(num9++) * 2]);
						}
						while (ptr2[num9] == '0')
						{
							stringBuilder.Append(ptr2[(IntPtr)(num9++) * 2]);
						}
						continue;
						IL_06D4:
						stringBuilder.Append(c);
					}
				}
				return stringBuilder.ToString();
			}

			private const int NumberMaxDigits = 32;

			internal const int DECIMAL_PRECISION = 29;

			private const int MIN_SB_BUFFER_SIZE = 105;

			private static string[] s_posCurrencyFormats = new string[] { "$#", "#$", "$ #", "# $" };

			private static string[] s_negCurrencyFormats = new string[]
			{
				"($#)", "-$#", "$-#", "$#-", "(#$)", "-#$", "#-$", "#$-", "-# $", "-$ #",
				"# $-", "$ #-", "$ -#", "#- $", "($ #)", "(# $)"
			};

			private static string[] s_posPercentFormats = new string[] { "# %", "#%", "%#", "% #" };

			private static string[] s_negPercentFormats = new string[]
			{
				"-# %", "-#%", "-%#", "%-#", "%#-", "#-%", "#%-", "-% #", "# %-", "% #-",
				"% -#", "#- %"
			};

			private static string[] s_negNumberFormats = new string[] { "(#)", "-#", "- #", "#-", "# -" };

			private static string s_posNumberFormat = "#";

			internal struct NumberBuffer
			{
				public unsafe char* digits
				{
					[SecurityCritical]
					get
					{
						return this.overrideDigits;
					}
				}

				public int precision;

				public int scale;

				public bool sign;

				[SecurityCritical]
				public unsafe char* overrideDigits;
			}
		}
	}
}
