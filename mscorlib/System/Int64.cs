using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public struct Int64 : IFormattable, IConvertible, IComparable, IComparable<long>, IEquatable<long>
	{
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(this);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this);
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			return Convert.ToType(this, targetType, provider, false);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is long))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Int64"));
			}
			long num = (long)value;
			if (this == num)
			{
				return 0;
			}
			return (this >= num) ? 1 : (-1);
		}

		public override bool Equals(object obj)
		{
			return obj is long && (long)obj == this;
		}

		public override int GetHashCode()
		{
			return (int)(this & (long)((ulong)(-1))) ^ (int)(this >> 32);
		}

		public int CompareTo(long value)
		{
			if (this == value)
			{
				return 0;
			}
			if (this > value)
			{
				return 1;
			}
			return -1;
		}

		public bool Equals(long obj)
		{
			return obj == this;
		}

		internal static bool Parse(string s, bool tryParse, out long result, out Exception exc)
		{
			long num = 0L;
			int num2 = 1;
			bool flag = false;
			result = 0L;
			exc = null;
			if (s == null)
			{
				if (!tryParse)
				{
					exc = new ArgumentNullException("s");
				}
				return false;
			}
			int length = s.Length;
			int i;
			char c;
			for (i = 0; i < length; i++)
			{
				c = s[i];
				if (!char.IsWhiteSpace(c))
				{
					break;
				}
			}
			if (i == length)
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			c = s[i];
			if (c == '+')
			{
				i++;
			}
			else if (c == '-')
			{
				num2 = -1;
				i++;
			}
			while (i < length)
			{
				c = s[i];
				if (c >= '0' && c <= '9')
				{
					byte b = (byte)(c - '0');
					if (num <= 922337203685477580L)
					{
						if (num != 922337203685477580L)
						{
							num = num * 10L + (long)b;
							flag = true;
							goto IL_0166;
						}
						if ((long)b <= 7L || (num2 != 1 && (long)b <= 8L))
						{
							if (num2 == -1)
							{
								num = num * (long)num2 * 10L - (long)b;
							}
							else
							{
								num = num * 10L + (long)b;
							}
							if (int.ProcessTrailingWhitespace(tryParse, s, i + 1, ref exc))
							{
								result = num;
								return true;
							}
						}
					}
					if (!tryParse)
					{
						exc = new OverflowException("Value is too large");
					}
					return false;
				}
				if (!int.ProcessTrailingWhitespace(tryParse, s, i, ref exc))
				{
					return false;
				}
				IL_0166:
				i++;
			}
			if (!flag)
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			if (num2 == -1)
			{
				result = num * (long)num2;
			}
			else
			{
				result = num;
			}
			return true;
		}

		public static long Parse(string s, IFormatProvider provider)
		{
			return long.Parse(s, NumberStyles.Integer, provider);
		}

		public static long Parse(string s, NumberStyles style)
		{
			return long.Parse(s, style, null);
		}

		internal static bool Parse(string s, NumberStyles style, IFormatProvider fp, bool tryParse, out long result, out Exception exc)
		{
			result = 0L;
			exc = null;
			if (s == null)
			{
				if (!tryParse)
				{
					exc = new ArgumentNullException("s");
				}
				return false;
			}
			if (s.Length == 0)
			{
				if (!tryParse)
				{
					exc = new FormatException("Input string was not in the correct format: s.Length==0.");
				}
				return false;
			}
			NumberFormatInfo numberFormatInfo = null;
			if (fp != null)
			{
				Type typeFromHandle = typeof(NumberFormatInfo);
				numberFormatInfo = (NumberFormatInfo)fp.GetFormat(typeFromHandle);
			}
			if (numberFormatInfo == null)
			{
				numberFormatInfo = Thread.CurrentThread.CurrentCulture.NumberFormat;
			}
			if (!int.CheckStyle(style, tryParse, ref exc))
			{
				return false;
			}
			bool flag = (style & NumberStyles.AllowCurrencySymbol) != NumberStyles.None;
			bool flag2 = (style & NumberStyles.AllowHexSpecifier) != NumberStyles.None;
			bool flag3 = (style & NumberStyles.AllowThousands) != NumberStyles.None;
			bool flag4 = (style & NumberStyles.AllowDecimalPoint) != NumberStyles.None;
			bool flag5 = (style & NumberStyles.AllowParentheses) != NumberStyles.None;
			bool flag6 = (style & NumberStyles.AllowTrailingSign) != NumberStyles.None;
			bool flag7 = (style & NumberStyles.AllowLeadingSign) != NumberStyles.None;
			bool flag8 = (style & NumberStyles.AllowTrailingWhite) != NumberStyles.None;
			bool flag9 = (style & NumberStyles.AllowLeadingWhite) != NumberStyles.None;
			int num = 0;
			if (flag9 && !int.JumpOverWhite(ref num, s, true, tryParse, ref exc))
			{
				return false;
			}
			bool flag10 = false;
			bool flag11 = false;
			bool flag12 = false;
			bool flag13 = false;
			if (flag5 && s[num] == '(')
			{
				flag10 = true;
				flag12 = true;
				flag11 = true;
				num++;
				if (flag9 && !int.JumpOverWhite(ref num, s, true, tryParse, ref exc))
				{
					return false;
				}
				if (s.Substring(num, numberFormatInfo.NegativeSign.Length) == numberFormatInfo.NegativeSign)
				{
					if (!tryParse)
					{
						exc = new FormatException("Input string was not in the correct format: Has Negative Sign.");
					}
					return false;
				}
				if (s.Substring(num, numberFormatInfo.PositiveSign.Length) == numberFormatInfo.PositiveSign)
				{
					if (!tryParse)
					{
						exc = new FormatException("Input string was not in the correct format: Has Positive Sign.");
					}
					return false;
				}
			}
			if (flag7 && !flag12)
			{
				int.FindSign(ref num, s, numberFormatInfo, ref flag12, ref flag11);
				if (flag12)
				{
					if (flag9 && !int.JumpOverWhite(ref num, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (flag)
					{
						int.FindCurrency(ref num, s, numberFormatInfo, ref flag13);
						if (flag13 && flag9 && !int.JumpOverWhite(ref num, s, true, tryParse, ref exc))
						{
							return false;
						}
					}
				}
			}
			if (flag && !flag13)
			{
				int.FindCurrency(ref num, s, numberFormatInfo, ref flag13);
				if (flag13)
				{
					if (flag9 && !int.JumpOverWhite(ref num, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (flag13 && !flag12 && flag7)
					{
						int.FindSign(ref num, s, numberFormatInfo, ref flag12, ref flag11);
						if (flag12 && flag9 && !int.JumpOverWhite(ref num, s, true, tryParse, ref exc))
						{
							return false;
						}
					}
				}
			}
			long num2 = 0L;
			int num3 = 0;
			bool flag14 = false;
			do
			{
				if (!int.ValidDigit(s[num], flag2))
				{
					if (!flag3 || (!int.FindOther(ref num, s, numberFormatInfo.NumberGroupSeparator) && !int.FindOther(ref num, s, numberFormatInfo.CurrencyGroupSeparator)))
					{
						if (flag14 || !flag4 || (!int.FindOther(ref num, s, numberFormatInfo.NumberDecimalSeparator) && !int.FindOther(ref num, s, numberFormatInfo.CurrencyDecimalSeparator)))
						{
							break;
						}
						flag14 = true;
					}
				}
				else if (flag2)
				{
					num3++;
					char c = s[num++];
					int num4;
					if (char.IsDigit(c))
					{
						num4 = (int)(c - '0');
					}
					else if (char.IsLower(c))
					{
						num4 = (int)(c - 'a' + '\n');
					}
					else
					{
						num4 = (int)(c - 'A' + '\n');
					}
					ulong num5 = (ulong)num2;
					try
					{
						num2 = (long)(checked(num5 * 16UL + (ulong)num4));
					}
					catch (OverflowException ex)
					{
						if (!tryParse)
						{
							exc = ex;
						}
						return false;
					}
				}
				else if (flag14)
				{
					num3++;
					if (s[num++] != '0')
					{
						goto Block_49;
					}
				}
				else
				{
					num3++;
					checked
					{
						try
						{
							num2 = num2 * 10L - unchecked((long)(checked(s[num++] - '0')));
						}
						catch (OverflowException)
						{
							if (!tryParse)
							{
								exc = new OverflowException("Value too large or too small.");
							}
							return false;
						}
					}
				}
			}
			while (num < s.Length);
			goto IL_0462;
			Block_49:
			if (!tryParse)
			{
				exc = new OverflowException("Value too large or too small.");
			}
			return false;
			IL_0462:
			if (num3 == 0)
			{
				if (!tryParse)
				{
					exc = new FormatException("Input string was not in the correct format: nDigits == 0.");
				}
				return false;
			}
			if (flag6 && !flag12)
			{
				int.FindSign(ref num, s, numberFormatInfo, ref flag12, ref flag11);
				if (flag12)
				{
					if (flag8 && !int.JumpOverWhite(ref num, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (flag)
					{
						int.FindCurrency(ref num, s, numberFormatInfo, ref flag13);
					}
				}
			}
			if (flag && !flag13)
			{
				if (numberFormatInfo.CurrencyPositivePattern == 3 && s[num++] != ' ')
				{
					if (tryParse)
					{
						return false;
					}
					throw new FormatException("Input string was not in the correct format: no space between number and currency symbol.");
				}
				else
				{
					int.FindCurrency(ref num, s, numberFormatInfo, ref flag13);
					if (flag13 && num < s.Length)
					{
						if (flag8 && !int.JumpOverWhite(ref num, s, true, tryParse, ref exc))
						{
							return false;
						}
						if (!flag12 && flag6)
						{
							int.FindSign(ref num, s, numberFormatInfo, ref flag12, ref flag11);
						}
					}
				}
			}
			if (flag8 && num < s.Length && !int.JumpOverWhite(ref num, s, false, tryParse, ref exc))
			{
				return false;
			}
			if (flag10)
			{
				if (num >= s.Length || s[num++] != ')')
				{
					if (!tryParse)
					{
						exc = new FormatException("Input string was not in the correct format: No room for close parens.");
					}
					return false;
				}
				if (flag8 && num < s.Length && !int.JumpOverWhite(ref num, s, false, tryParse, ref exc))
				{
					return false;
				}
			}
			if (num < s.Length && s[num] != '\0')
			{
				if (!tryParse)
				{
					exc = new FormatException(string.Concat(new object[] { "Input string was not in the correct format: Did not parse entire string. pos = ", num, " s.Length = ", s.Length }));
				}
				return false;
			}
			checked
			{
				if (!flag11 && !flag2)
				{
					try
					{
						num2 = (long)(unchecked((ulong)0) - (ulong)num2);
					}
					catch (OverflowException ex2)
					{
						if (!tryParse)
						{
							exc = ex2;
						}
						return false;
					}
				}
				result = num2;
				return true;
			}
		}

		public static long Parse(string s)
		{
			long num;
			Exception ex;
			if (!long.Parse(s, false, out num, out ex))
			{
				throw ex;
			}
			return num;
		}

		public static long Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			long num;
			Exception ex;
			if (!long.Parse(s, style, provider, false, out num, out ex))
			{
				throw ex;
			}
			return num;
		}

		public static bool TryParse(string s, out long result)
		{
			Exception ex;
			if (!long.Parse(s, true, out result, out ex))
			{
				result = 0L;
				return false;
			}
			return true;
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out long result)
		{
			Exception ex;
			if (!long.Parse(s, style, provider, true, out result, out ex))
			{
				result = 0L;
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return NumberFormatter.NumberToString(this, null);
		}

		public string ToString(IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(this, provider);
		}

		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(format, this, provider);
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Int64;
		}

		public const long MaxValue = 9223372036854775807L;

		public const long MinValue = -9223372036854775808L;

		internal long m_value;
	}
}
