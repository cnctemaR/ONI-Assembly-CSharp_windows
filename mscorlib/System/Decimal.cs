using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public struct Decimal : IFormattable, IConvertible, IComparable, IComparable<decimal>, IEquatable<decimal>
	{
		public Decimal(int lo, int mid, int hi, bool isNegative, byte scale)
		{
			this.lo = (uint)lo;
			this.mid = (uint)mid;
			this.hi = (uint)hi;
			if (scale > 28)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("scale must be between 0 and 28"));
			}
			this.flags = (uint)scale;
			this.flags <<= 16;
			if (isNegative)
			{
				this.flags |= 2147483648U;
			}
		}

		public Decimal(int value)
		{
			this.hi = (this.mid = 0U);
			if (value < 0)
			{
				this.flags = 2147483648U;
				this.lo = (uint)(~value + 1);
			}
			else
			{
				this.flags = 0U;
				this.lo = (uint)value;
			}
		}

		[CLSCompliant(false)]
		public Decimal(uint value)
		{
			this.lo = value;
			this.flags = (this.hi = (this.mid = 0U));
		}

		public Decimal(long value)
		{
			this.hi = 0U;
			if (value < 0L)
			{
				this.flags = 2147483648U;
				ulong num = (ulong)(~value + 1L);
				this.lo = (uint)num;
				this.mid = (uint)(num >> 32);
			}
			else
			{
				this.flags = 0U;
				this.lo = (uint)value;
				this.mid = (uint)((ulong)value >> 32);
			}
		}

		[CLSCompliant(false)]
		public Decimal(ulong value)
		{
			this.flags = (this.hi = 0U);
			this.lo = (uint)value;
			this.mid = (uint)(value >> 32);
		}

		public Decimal(float value)
		{
			if (value > 7.9228163E+28f || value < -7.9228163E+28f || float.IsNaN(value) || float.IsNegativeInfinity(value) || float.IsPositiveInfinity(value))
			{
				throw new OverflowException(Locale.GetText("Value {0} is greater than Decimal.MaxValue or less than Decimal.MinValue", new object[] { value }));
			}
			decimal num = decimal.Parse(value.ToString(CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
			this.flags = num.flags;
			this.hi = num.hi;
			this.lo = num.lo;
			this.mid = num.mid;
		}

		public Decimal(double value)
		{
			if (value > 7.922816251426434E+28 || value < -7.922816251426434E+28 || double.IsNaN(value) || double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value))
			{
				throw new OverflowException(Locale.GetText("Value {0} is greater than Decimal.MaxValue or less than Decimal.MinValue", new object[] { value }));
			}
			decimal num = decimal.Parse(value.ToString(CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
			this.flags = num.flags;
			this.hi = num.hi;
			this.lo = num.lo;
			this.mid = num.mid;
		}

		public Decimal(int[] bits)
		{
			if (bits == null)
			{
				throw new ArgumentNullException(Locale.GetText("Bits is a null reference"));
			}
			if (bits.GetLength(0) != 4)
			{
				throw new ArgumentException(Locale.GetText("bits does not contain four values"));
			}
			this.lo = (uint)bits[0];
			this.mid = (uint)bits[1];
			this.hi = (uint)bits[2];
			this.flags = (uint)bits[3];
			byte b = (byte)(this.flags >> 16);
			if (b > 28 || (this.flags & 2130771967U) != 0U)
			{
				throw new ArgumentException(Locale.GetText("Invalid bits[3]"));
			}
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			return Convert.ToType(this, targetType, provider, false);
		}

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
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return this;
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

		public static decimal FromOACurrency(long cy)
		{
			return cy / 10000m;
		}

		public static int[] GetBits(decimal d)
		{
			return new int[]
			{
				(int)d.lo,
				(int)d.mid,
				(int)d.hi,
				(int)d.flags
			};
		}

		public static decimal Negate(decimal d)
		{
			d.flags ^= 2147483648U;
			return d;
		}

		public static decimal Add(decimal d1, decimal d2)
		{
			if (decimal.decimalIncr(ref d1, ref d2) == 0)
			{
				return d1;
			}
			throw new OverflowException(Locale.GetText("Overflow on adding decimal number"));
		}

		public static decimal Subtract(decimal d1, decimal d2)
		{
			d2.flags ^= 2147483648U;
			int num = decimal.decimalIncr(ref d1, ref d2);
			if (num == 0)
			{
				return d1;
			}
			throw new OverflowException(Locale.GetText("Overflow on subtracting decimal numbers (" + num + ")"));
		}

		public override int GetHashCode()
		{
			return (int)(this.flags ^ this.hi ^ this.lo ^ this.mid);
		}

		private static ulong u64(decimal value)
		{
			decimal.decimalFloorAndTrunc(ref value, 0);
			ulong num;
			if (decimal.decimal2UInt64(ref value, out num) != 0)
			{
				throw new OverflowException();
			}
			return num;
		}

		private static long s64(decimal value)
		{
			decimal.decimalFloorAndTrunc(ref value, 0);
			long num;
			if (decimal.decimal2Int64(ref value, out num) != 0)
			{
				throw new OverflowException();
			}
			return num;
		}

		public static bool Equals(decimal d1, decimal d2)
		{
			return decimal.Compare(d1, d2) == 0;
		}

		public override bool Equals(object value)
		{
			return value is decimal && decimal.Equals((decimal)value, this);
		}

		private bool IsZero()
		{
			return this.hi == 0U && this.lo == 0U && this.mid == 0U;
		}

		private bool IsNegative()
		{
			return (this.flags & 2147483648U) == 2147483648U;
		}

		public static decimal Floor(decimal d)
		{
			decimal.decimalFloorAndTrunc(ref d, 1);
			return d;
		}

		public static decimal Truncate(decimal d)
		{
			decimal.decimalFloorAndTrunc(ref d, 0);
			return d;
		}

		public static decimal Round(decimal d, int decimals)
		{
			return decimal.Round(d, decimals, MidpointRounding.ToEven);
		}

		public static decimal Round(decimal d, int decimals, MidpointRounding mode)
		{
			if (mode != MidpointRounding.ToEven && mode != MidpointRounding.AwayFromZero)
			{
				throw new ArgumentException("The value '" + mode + "' is not valid for this usage of the type MidpointRounding.", "mode");
			}
			if (decimals < 0 || decimals > 28)
			{
				throw new ArgumentOutOfRangeException("decimals", "[0,28]");
			}
			bool flag = d.IsNegative();
			if (flag)
			{
				d.flags ^= 2147483648U;
			}
			decimal num = (decimal)Math.Pow(10.0, (double)decimals);
			decimal num2 = decimal.Floor(d);
			decimal num3 = d - num2;
			num3 *= 10000000000000000000000000000m;
			num3 = decimal.Floor(num3);
			num3 /= 10000000000000000000000000000m / num;
			num3 = Math.Round(num3, mode);
			num3 /= num;
			decimal num4 = num2 + num3;
			long num5 = (long)decimals - (long)((ulong)((num4.flags & 2147418112U) >> 16));
			if (num5 > 0L)
			{
				while (num5 > 0L)
				{
					if (num4 > decimal.MaxValueDiv10)
					{
						break;
					}
					num4 *= 10m;
					num5 -= 1L;
				}
			}
			else if (num5 < 0L)
			{
				while (num5 < 0L)
				{
					num4 /= 10m;
					num5 += 1L;
				}
			}
			num4.flags = (uint)((uint)((long)decimals - num5) << 16);
			if (flag)
			{
				num4.flags ^= 2147483648U;
			}
			return num4;
		}

		public static decimal Round(decimal d)
		{
			return Math.Round(d);
		}

		public static decimal Round(decimal d, MidpointRounding mode)
		{
			return Math.Round(d, mode);
		}

		public static decimal Multiply(decimal d1, decimal d2)
		{
			if (d1.IsZero() || d2.IsZero())
			{
				return 0m;
			}
			if (decimal.decimalMult(ref d1, ref d2) != 0)
			{
				throw new OverflowException();
			}
			return d1;
		}

		public static decimal Divide(decimal d1, decimal d2)
		{
			if (d2.IsZero())
			{
				throw new DivideByZeroException();
			}
			if (d1.IsZero())
			{
				return 0m;
			}
			d1.flags ^= 2147483648U;
			d1.flags ^= 2147483648U;
			decimal num;
			if (decimal.decimalDiv(out num, ref d1, ref d2) != 0)
			{
				throw new OverflowException();
			}
			return num;
		}

		public static decimal Remainder(decimal d1, decimal d2)
		{
			if (d2.IsZero())
			{
				throw new DivideByZeroException();
			}
			if (d1.IsZero())
			{
				return 0m;
			}
			bool flag = d1.IsNegative();
			if (flag)
			{
				d1.flags ^= 2147483648U;
			}
			if (d2.IsNegative())
			{
				d2.flags ^= 2147483648U;
			}
			if (d1 == d2)
			{
				return 0m;
			}
			decimal num;
			if (d2 > d1)
			{
				num = d1;
			}
			else
			{
				if (decimal.decimalDiv(out num, ref d1, ref d2) != 0)
				{
					throw new OverflowException();
				}
				num = decimal.Truncate(num);
				num = d1 - num * d2;
			}
			if (flag)
			{
				num.flags ^= 2147483648U;
			}
			return num;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static int Compare(decimal d1, decimal d2)
		{
			return decimal.decimalCompare(ref d1, ref d2);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is decimal))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Decimal"));
			}
			return decimal.Compare(this, (decimal)value);
		}

		public int CompareTo(decimal value)
		{
			return decimal.Compare(this, value);
		}

		public bool Equals(decimal value)
		{
			return decimal.Equals(value, this);
		}

		public static decimal Ceiling(decimal d)
		{
			return Math.Ceiling(d);
		}

		public static decimal Parse(string s)
		{
			return decimal.Parse(s, NumberStyles.Number, null);
		}

		public static decimal Parse(string s, NumberStyles style)
		{
			return decimal.Parse(s, style, null);
		}

		public static decimal Parse(string s, IFormatProvider provider)
		{
			return decimal.Parse(s, NumberStyles.Number, provider);
		}

		private static void ThrowAtPos(int pos)
		{
			throw new FormatException(string.Format(Locale.GetText("Invalid character at position {0}"), pos));
		}

		private static void ThrowInvalidExp()
		{
			throw new FormatException(Locale.GetText("Invalid exponent"));
		}

		private static string stripStyles(string s, NumberStyles style, NumberFormatInfo nfi, out int decPos, out bool isNegative, out bool expFlag, out int exp, bool throwex)
		{
			isNegative = false;
			expFlag = false;
			exp = 0;
			decPos = -1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = (style & NumberStyles.AllowLeadingWhite) != NumberStyles.None;
			bool flag5 = (style & NumberStyles.AllowTrailingWhite) != NumberStyles.None;
			bool flag6 = (style & NumberStyles.AllowLeadingSign) != NumberStyles.None;
			bool flag7 = (style & NumberStyles.AllowTrailingSign) != NumberStyles.None;
			bool flag8 = (style & NumberStyles.AllowParentheses) != NumberStyles.None;
			bool flag9 = (style & NumberStyles.AllowThousands) != NumberStyles.None;
			bool flag10 = (style & NumberStyles.AllowDecimalPoint) != NumberStyles.None;
			bool flag11 = (style & NumberStyles.AllowExponent) != NumberStyles.None;
			bool flag12 = false;
			if ((style & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
			{
				int num = s.IndexOf(nfi.CurrencySymbol);
				if (num >= 0)
				{
					s = s.Remove(num, nfi.CurrencySymbol.Length);
					flag12 = true;
				}
			}
			string text = ((!flag12) ? nfi.NumberDecimalSeparator : nfi.CurrencyDecimalSeparator);
			string text2 = ((!flag12) ? nfi.NumberGroupSeparator : nfi.CurrencyGroupSeparator);
			int i = 0;
			int length = s.Length;
			StringBuilder stringBuilder = new StringBuilder(length);
			while (i < length)
			{
				char c = s[i];
				if (char.IsDigit(c))
				{
					break;
				}
				if (flag4 && char.IsWhiteSpace(c))
				{
					i++;
				}
				else if (flag8 && c == '(' && !flag && !flag2)
				{
					flag2 = true;
					flag = true;
					isNegative = true;
					i++;
				}
				else if (flag6 && c == nfi.NegativeSign[0] && !flag)
				{
					int length2 = nfi.NegativeSign.Length;
					if (length2 == 1 || s.IndexOf(nfi.NegativeSign, i, length2) == i)
					{
						flag = true;
						isNegative = true;
						i += length2;
					}
				}
				else if (flag6 && c == nfi.PositiveSign[0] && !flag)
				{
					int length3 = nfi.PositiveSign.Length;
					if (length3 == 1 || s.IndexOf(nfi.PositiveSign, i, length3) == i)
					{
						flag = true;
						i += length3;
					}
				}
				else
				{
					if (flag10 && c == text[0])
					{
						int length4 = text.Length;
						if (length4 != 1 && s.IndexOf(text, i, length4) != i)
						{
							if (!throwex)
							{
								return null;
							}
							decimal.ThrowAtPos(i);
						}
						break;
					}
					if (!throwex)
					{
						return null;
					}
					decimal.ThrowAtPos(i);
				}
			}
			if (i == length)
			{
				if (throwex)
				{
					throw new FormatException(Locale.GetText("No digits found"));
				}
				return null;
			}
			else
			{
				while (i < length)
				{
					char c2 = s[i];
					if (char.IsDigit(c2))
					{
						stringBuilder.Append(c2);
						i++;
					}
					else if (flag9 && c2 == text2[0])
					{
						int length5 = text2.Length;
						if (length5 != 1 && s.IndexOf(text2, i, length5) != i)
						{
							if (!throwex)
							{
								return null;
							}
							decimal.ThrowAtPos(i);
						}
						i += length5;
					}
					else
					{
						if (!flag10 || c2 != text[0] || flag3)
						{
							break;
						}
						int length6 = text.Length;
						if (length6 == 1 || s.IndexOf(text, i, length6) == i)
						{
							decPos = stringBuilder.Length;
							flag3 = true;
							i += length6;
						}
					}
				}
				if (i < length)
				{
					char c3 = s[i];
					if (flag11 && char.ToUpperInvariant(c3) == 'E')
					{
						expFlag = true;
						i++;
						if (i >= length)
						{
							if (!throwex)
							{
								return null;
							}
							decimal.ThrowInvalidExp();
						}
						c3 = s[i];
						bool flag13 = false;
						if (c3 == nfi.PositiveSign[0])
						{
							int length7 = nfi.PositiveSign.Length;
							if (length7 == 1 || s.IndexOf(nfi.PositiveSign, i, length7) == i)
							{
								i += length7;
								if (i >= length)
								{
									if (!throwex)
									{
										return null;
									}
									decimal.ThrowInvalidExp();
								}
							}
						}
						else if (c3 == nfi.NegativeSign[0])
						{
							int length8 = nfi.NegativeSign.Length;
							if (length8 == 1 || s.IndexOf(nfi.NegativeSign, i, length8) == i)
							{
								i += length8;
								if (i >= length)
								{
									if (!throwex)
									{
										return null;
									}
									decimal.ThrowInvalidExp();
								}
								flag13 = true;
							}
						}
						c3 = s[i];
						if (!char.IsDigit(c3))
						{
							if (!throwex)
							{
								return null;
							}
							decimal.ThrowInvalidExp();
						}
						exp = (int)(c3 - '0');
						i++;
						while (i < length && char.IsDigit(s[i]))
						{
							exp *= 10;
							exp += (int)(s[i] - '0');
							i++;
						}
						if (flag13)
						{
							exp *= -1;
						}
					}
				}
				while (i < length)
				{
					char c4 = s[i];
					if (flag5 && char.IsWhiteSpace(c4))
					{
						i++;
					}
					else if (flag8 && c4 == ')' && flag2)
					{
						flag2 = false;
						i++;
					}
					else if (flag7 && c4 == nfi.NegativeSign[0] && !flag)
					{
						int length9 = nfi.NegativeSign.Length;
						if (length9 == 1 || s.IndexOf(nfi.NegativeSign, i, length9) == i)
						{
							flag = true;
							isNegative = true;
							i += length9;
						}
					}
					else if (flag7 && c4 == nfi.PositiveSign[0] && !flag)
					{
						int length10 = nfi.PositiveSign.Length;
						if (length10 == 1 || s.IndexOf(nfi.PositiveSign, i, length10) == i)
						{
							flag = true;
							i += length10;
						}
					}
					else
					{
						if (!throwex)
						{
							return null;
						}
						decimal.ThrowAtPos(i);
					}
				}
				if (!flag2)
				{
					if (!flag3)
					{
						decPos = stringBuilder.Length;
					}
					return stringBuilder.ToString();
				}
				if (throwex)
				{
					throw new FormatException(Locale.GetText("Closing Parentheses not found"));
				}
				return null;
			}
		}

		public static decimal Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				throw new ArgumentException("Decimal.TryParse does not accept AllowHexSpecifier", "style");
			}
			decimal num;
			decimal.PerformParse(s, style, provider, out num, true);
			return num;
		}

		public static bool TryParse(string s, out decimal result)
		{
			if (s == null)
			{
				result = 0m;
				return false;
			}
			return decimal.PerformParse(s, NumberStyles.Number, null, out result, false);
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out decimal result)
		{
			if (s == null || (style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				result = 0m;
				return false;
			}
			return decimal.PerformParse(s, style, provider, out result, false);
		}

		private static bool PerformParse(string s, NumberStyles style, IFormatProvider provider, out decimal res, bool throwex)
		{
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			int num;
			bool flag;
			bool flag2;
			int num2;
			s = decimal.stripStyles(s, style, instance, out num, out flag, out flag2, out num2, throwex);
			if (s == null)
			{
				res = 0m;
				return false;
			}
			if (num < 0)
			{
				if (throwex)
				{
					throw new Exception(Locale.GetText("Error in System.Decimal.Parse"));
				}
				res = 0m;
				return false;
			}
			else
			{
				int num3 = s.Length;
				int num4 = 0;
				while (num4 < num && s[num4] == '0')
				{
					num4++;
				}
				if (num4 > 1 && num3 > 1)
				{
					s = s.Substring(num4, num3 - num4);
					num -= num4;
				}
				int num5 = ((num != 0) ? 28 : 27);
				num3 = s.Length;
				if (num3 >= num5 + 1 && string.Compare(s, 0, "79228162514264337593543950335", 0, num5 + 1, false, CultureInfo.InvariantCulture) <= 0)
				{
					num5++;
				}
				if (num3 > num5 && num < num3)
				{
					int num6 = (int)(s[num5] - '0');
					s = s.Substring(0, num5);
					bool flag3 = false;
					if (num6 > 5)
					{
						flag3 = true;
					}
					else if (num6 == 5)
					{
						if (flag)
						{
							flag3 = true;
						}
						else
						{
							int num7 = (int)(s[num5 - 1] - '0');
							flag3 = (num7 & 1) == 1;
						}
					}
					if (flag3)
					{
						char[] array = s.ToCharArray();
						int i = num5 - 1;
						while (i >= 0)
						{
							int num8 = (int)(array[i] - '0');
							if (array[i] != '9')
							{
								array[i] = (char)(num8 + 49);
								break;
							}
							array[i--] = '0';
						}
						if (i == -1 && array[0] == '0')
						{
							num++;
							s = "1".PadRight(num, '0');
						}
						else
						{
							s = new string(array);
						}
					}
				}
				decimal num9;
				if (decimal.string2decimal(out num9, s, (uint)num, 0) != 0)
				{
					if (throwex)
					{
						throw new OverflowException();
					}
					res = 0m;
					return false;
				}
				else
				{
					if (!flag2 || decimal.decimalSetExponent(ref num9, num2) == 0)
					{
						if (flag)
						{
							num9.flags ^= 2147483648U;
						}
						res = num9;
						return true;
					}
					if (throwex)
					{
						throw new OverflowException();
					}
					res = 0m;
					return false;
				}
			}
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Decimal;
		}

		public static byte ToByte(decimal value)
		{
			if (value > 255m || value < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue or less than Byte.MinValue"));
			}
			return (byte)decimal.Truncate(value);
		}

		public static double ToDouble(decimal d)
		{
			return Convert.ToDouble(d);
		}

		public static short ToInt16(decimal value)
		{
			if (value > 32767m || value < -32768m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue or less than Int16.MinValue"));
			}
			return (short)decimal.Truncate(value);
		}

		public static int ToInt32(decimal d)
		{
			if (d > 2147483647m || d < -2147483648m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int32.MaxValue or less than Int32.MinValue"));
			}
			return (int)decimal.Truncate(d);
		}

		public static long ToInt64(decimal d)
		{
			if (d > 9223372036854775807m || d < -9223372036854775808m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int64.MaxValue or less than Int64.MinValue"));
			}
			return (long)decimal.Truncate(d);
		}

		public static long ToOACurrency(decimal value)
		{
			return (long)(value * 10000m);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(decimal value)
		{
			if (value > 127m || value < -128m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue or less than SByte.MinValue"));
			}
			return (sbyte)decimal.Truncate(value);
		}

		public static float ToSingle(decimal d)
		{
			return Convert.ToSingle(d);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(decimal value)
		{
			if (value > 65535m || value < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue or less than UInt16.MinValue"));
			}
			return (ushort)decimal.Truncate(value);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(decimal d)
		{
			if (d > 4294967295m || d < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt32.MaxValue or less than UInt32.MinValue"));
			}
			return (uint)decimal.Truncate(d);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(decimal d)
		{
			if (d > 18446744073709551615m || d < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt64.MaxValue or less than UInt64.MinValue"));
			}
			return (ulong)decimal.Truncate(d);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(format, this, provider);
		}

		public override string ToString()
		{
			return this.ToString("G", null);
		}

		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		public string ToString(IFormatProvider provider)
		{
			return this.ToString("G", provider);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimal2UInt64(ref decimal val, out ulong result);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimal2Int64(ref decimal val, out long result);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalIncr(ref decimal d1, ref decimal d2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int decimal2string(ref decimal val, int digits, int decimals, char[] bufDigits, int bufSize, out int decPos, out int sign);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int string2decimal(out decimal val, string sDigits, uint decPos, int sign);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int decimalSetExponent(ref decimal val, int exp);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double decimal2double(ref decimal val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void decimalFloorAndTrunc(ref decimal val, int floorFlag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalMult(ref decimal pd1, ref decimal pd2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalDiv(out decimal pc, ref decimal pa, ref decimal pb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalIntDiv(out decimal pc, ref decimal pa, ref decimal pb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalCompare(ref decimal d1, ref decimal d2);

		public static decimal operator +(decimal d1, decimal d2)
		{
			return decimal.Add(d1, d2);
		}

		public static decimal operator --(decimal d)
		{
			return decimal.Add(d, -1m);
		}

		public static decimal operator ++(decimal d)
		{
			return decimal.Add(d, 1m);
		}

		public static decimal operator -(decimal d1, decimal d2)
		{
			return decimal.Subtract(d1, d2);
		}

		public static decimal operator -(decimal d)
		{
			return decimal.Negate(d);
		}

		public static decimal operator +(decimal d)
		{
			return d;
		}

		public static decimal operator *(decimal d1, decimal d2)
		{
			return decimal.Multiply(d1, d2);
		}

		public static decimal operator /(decimal d1, decimal d2)
		{
			return decimal.Divide(d1, d2);
		}

		public static decimal operator %(decimal d1, decimal d2)
		{
			return decimal.Remainder(d1, d2);
		}

		public static explicit operator byte(decimal value)
		{
			ulong num = decimal.u64(value);
			return checked((byte)num);
		}

		[CLSCompliant(false)]
		public static explicit operator sbyte(decimal value)
		{
			long num = decimal.s64(value);
			return checked((sbyte)num);
		}

		public static explicit operator char(decimal value)
		{
			ulong num = decimal.u64(value);
			return checked((char)num);
		}

		public static explicit operator short(decimal value)
		{
			long num = decimal.s64(value);
			return checked((short)num);
		}

		[CLSCompliant(false)]
		public static explicit operator ushort(decimal value)
		{
			ulong num = decimal.u64(value);
			return checked((ushort)num);
		}

		public static explicit operator int(decimal value)
		{
			long num = decimal.s64(value);
			return checked((int)num);
		}

		[CLSCompliant(false)]
		public static explicit operator uint(decimal value)
		{
			ulong num = decimal.u64(value);
			return checked((uint)num);
		}

		public static explicit operator long(decimal value)
		{
			return decimal.s64(value);
		}

		[CLSCompliant(false)]
		public static explicit operator ulong(decimal value)
		{
			return decimal.u64(value);
		}

		public static implicit operator decimal(byte value)
		{
			return new decimal((int)value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(sbyte value)
		{
			return new decimal((int)value);
		}

		public static implicit operator decimal(short value)
		{
			return new decimal((int)value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(ushort value)
		{
			return new decimal((int)value);
		}

		public static implicit operator decimal(char value)
		{
			return new decimal((int)value);
		}

		public static implicit operator decimal(int value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(uint value)
		{
			return new decimal(value);
		}

		public static implicit operator decimal(long value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(ulong value)
		{
			return new decimal(value);
		}

		public static explicit operator decimal(float value)
		{
			return new decimal(value);
		}

		public static explicit operator decimal(double value)
		{
			return new decimal(value);
		}

		public static explicit operator float(decimal value)
		{
			return (float)(double)value;
		}

		public static explicit operator double(decimal value)
		{
			return decimal.decimal2double(ref value);
		}

		public static bool operator !=(decimal d1, decimal d2)
		{
			return !decimal.Equals(d1, d2);
		}

		public static bool operator ==(decimal d1, decimal d2)
		{
			return decimal.Equals(d1, d2);
		}

		public static bool operator >(decimal d1, decimal d2)
		{
			return decimal.Compare(d1, d2) > 0;
		}

		public static bool operator >=(decimal d1, decimal d2)
		{
			return decimal.Compare(d1, d2) >= 0;
		}

		public static bool operator <(decimal d1, decimal d2)
		{
			return decimal.Compare(d1, d2) < 0;
		}

		public static bool operator <=(decimal d1, decimal d2)
		{
			return decimal.Compare(d1, d2) <= 0;
		}

		public const decimal MinValue = -79228162514264337593543950335m;

		public const decimal MaxValue = 79228162514264337593543950335m;

		public const decimal MinusOne = -1m;

		public const decimal One = 1m;

		public const decimal Zero = 0m;

		private const int DECIMAL_DIVIDE_BY_ZERO = 5;

		private const uint MAX_SCALE = 28U;

		private const int iMAX_SCALE = 28;

		private const uint SIGN_FLAG = 2147483648U;

		private const uint SCALE_MASK = 16711680U;

		private const int SCALE_SHIFT = 16;

		private const uint RESERVED_SS32_BITS = 2130771967U;

		private static readonly decimal MaxValueDiv10 = 7922816251426433759354395033.5m;

		private uint flags;

		private uint hi;

		private uint lo;

		private uint mid;
	}
}
