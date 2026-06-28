using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public struct Double : IFormattable, IConvertible, IComparable, IComparable<double>, IEquatable<double>
	{
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
			if (!(value is double))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Double"));
			}
			double num = (double)value;
			if (double.IsPositiveInfinity(this) && double.IsPositiveInfinity(num))
			{
				return 0;
			}
			if (double.IsNegativeInfinity(this) && double.IsNegativeInfinity(num))
			{
				return 0;
			}
			if (double.IsNaN(num))
			{
				if (double.IsNaN(this))
				{
					return 0;
				}
				return 1;
			}
			else if (double.IsNaN(this))
			{
				if (double.IsNaN(num))
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (this > num)
				{
					return 1;
				}
				if (this < num)
				{
					return -1;
				}
				return 0;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is double))
			{
				return false;
			}
			double num = (double)obj;
			if (double.IsNaN(num))
			{
				return double.IsNaN(this);
			}
			return num == this;
		}

		public int CompareTo(double value)
		{
			if (double.IsPositiveInfinity(this) && double.IsPositiveInfinity(value))
			{
				return 0;
			}
			if (double.IsNegativeInfinity(this) && double.IsNegativeInfinity(value))
			{
				return 0;
			}
			if (double.IsNaN(value))
			{
				if (double.IsNaN(this))
				{
					return 0;
				}
				return 1;
			}
			else if (double.IsNaN(this))
			{
				if (double.IsNaN(value))
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (this > value)
				{
					return 1;
				}
				if (this < value)
				{
					return -1;
				}
				return 0;
			}
		}

		public bool Equals(double obj)
		{
			if (double.IsNaN(obj))
			{
				return double.IsNaN(this);
			}
			return obj == this;
		}

		public override int GetHashCode()
		{
			double num = this;
			return num.GetHashCode();
		}

		public static bool IsInfinity(double d)
		{
			return d == double.PositiveInfinity || d == double.NegativeInfinity;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static bool IsNaN(double d)
		{
			return d != d;
		}

		public static bool IsNegativeInfinity(double d)
		{
			return d < 0.0 && (d == double.NegativeInfinity || d == double.PositiveInfinity);
		}

		public static bool IsPositiveInfinity(double d)
		{
			return d > 0.0 && (d == double.NegativeInfinity || d == double.PositiveInfinity);
		}

		public static double Parse(string s)
		{
			return double.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, null);
		}

		public static double Parse(string s, IFormatProvider provider)
		{
			return double.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, provider);
		}

		public static double Parse(string s, NumberStyles style)
		{
			return double.Parse(s, style, null);
		}

		public static double Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			double num;
			Exception ex;
			if (!double.Parse(s, style, provider, false, out num, out ex))
			{
				throw ex;
			}
			return num;
		}

		internal unsafe static bool Parse(string s, NumberStyles style, IFormatProvider provider, bool tryParse, out double result, out Exception exc)
		{
			result = 0.0;
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
					exc = new FormatException();
				}
				return false;
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				string text = Locale.GetText("Double doesn't support parsing with '{0}'.", new object[] { "AllowHexSpecifier" });
				throw new ArgumentException(text);
			}
			if (style > NumberStyles.Any)
			{
				if (!tryParse)
				{
					exc = new ArgumentException();
				}
				return false;
			}
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			if (instance == null)
			{
				throw new Exception("How did this happen?");
			}
			int length = s.Length;
			int num = 0;
			int i = 0;
			bool flag = (style & NumberStyles.AllowLeadingWhite) != NumberStyles.None;
			bool flag2 = (style & NumberStyles.AllowTrailingWhite) != NumberStyles.None;
			if (flag)
			{
				while (i < length && char.IsWhiteSpace(s[i]))
				{
					i++;
				}
				if (i == length)
				{
					if (!tryParse)
					{
						exc = int.GetFormatException();
					}
					return false;
				}
			}
			int num2 = s.Length - 1;
			if (flag2)
			{
				while (char.IsWhiteSpace(s[num2]))
				{
					num2--;
				}
			}
			if (double.TryParseStringConstant(instance.NaNSymbol, s, i, num2))
			{
				result = double.NaN;
				return true;
			}
			if (double.TryParseStringConstant(instance.PositiveInfinitySymbol, s, i, num2))
			{
				result = double.PositiveInfinity;
				return true;
			}
			if (double.TryParseStringConstant(instance.NegativeInfinitySymbol, s, i, num2))
			{
				result = double.NegativeInfinity;
				return true;
			}
			byte[] array = new byte[length + 1];
			int num3 = 1;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			if ((style & NumberStyles.AllowDecimalPoint) != NumberStyles.None)
			{
				text2 = instance.NumberDecimalSeparator;
				num4 = text2.Length;
			}
			if ((style & NumberStyles.AllowThousands) != NumberStyles.None)
			{
				text3 = instance.NumberGroupSeparator;
				num5 = text3.Length;
			}
			if ((style & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
			{
				text4 = instance.CurrencySymbol;
				num6 = text4.Length;
			}
			string positiveSign = instance.PositiveSign;
			string negativeSign = instance.NegativeSign;
			while (i < length)
			{
				char c = s[i];
				if (c == '\0')
				{
					i = length;
				}
				else
				{
					switch (num3)
					{
					case 1:
						if ((style & NumberStyles.AllowLeadingSign) != NumberStyles.None)
						{
							if (c == positiveSign[0] && s.Substring(i, positiveSign.Length) == positiveSign)
							{
								num3 = 2;
								i += positiveSign.Length - 1;
								goto IL_0624;
							}
							if (c == negativeSign[0] && s.Substring(i, negativeSign.Length) == negativeSign)
							{
								num3 = 2;
								array[num++] = 45;
								i += negativeSign.Length - 1;
								goto IL_0624;
							}
						}
						num3 = 2;
						goto IL_0304;
					case 2:
						goto IL_0304;
					case 3:
						goto IL_0429;
					case 4:
						if (char.IsDigit(c))
						{
							num3 = 5;
							goto IL_0599;
						}
						if (c == positiveSign[0] && s.Substring(i, positiveSign.Length) == positiveSign)
						{
							num3 = 2;
							i += positiveSign.Length - 1;
							goto IL_0624;
						}
						if (c == negativeSign[0] && s.Substring(i, negativeSign.Length) == negativeSign)
						{
							num3 = 2;
							array[num++] = 45;
							i += negativeSign.Length - 1;
							goto IL_0624;
						}
						if (char.IsWhiteSpace(c))
						{
							goto IL_05E7;
						}
						if (!tryParse)
						{
							exc = new FormatException("Unknown char: " + c);
						}
						return false;
					case 5:
						goto IL_0599;
					case 6:
						goto IL_05E7;
					}
					IL_0617:
					if (num3 == 7)
					{
						break;
					}
					goto IL_0624;
					IL_0429:
					if (char.IsDigit(c))
					{
						array[num++] = (byte)c;
						goto IL_0617;
					}
					if (c == 'e' || c == 'E')
					{
						if ((style & NumberStyles.AllowExponent) == NumberStyles.None)
						{
							if (!tryParse)
							{
								exc = new FormatException("Unknown char: " + c);
							}
							return false;
						}
						array[num++] = (byte)c;
						num3 = 4;
						goto IL_0617;
					}
					else
					{
						if (char.IsWhiteSpace(c))
						{
							goto IL_05E7;
						}
						if (!tryParse)
						{
							exc = new FormatException("Unknown char: " + c);
						}
						return false;
					}
					IL_0304:
					if (char.IsDigit(c))
					{
						array[num++] = (byte)c;
						goto IL_0617;
					}
					if (c == 'e' || c == 'E')
					{
						goto IL_0429;
					}
					if (num4 > 0 && text2[0] == c && string.CompareOrdinal(s, i, text2, 0, num4) == 0)
					{
						array[num++] = 46;
						i += num4 - 1;
						num3 = 3;
						goto IL_0617;
					}
					if (num5 > 0 && text3[0] == c && s.Substring(i, num5) == text3)
					{
						i += num5 - 1;
						num3 = 2;
						goto IL_0617;
					}
					if (num6 > 0 && text4[0] == c && s.Substring(i, num6) == text4)
					{
						i += num6 - 1;
						num3 = 2;
						goto IL_0617;
					}
					if (char.IsWhiteSpace(c))
					{
						goto IL_05E7;
					}
					if (!tryParse)
					{
						exc = new FormatException("Unknown char: " + c);
					}
					return false;
					IL_0599:
					if (char.IsDigit(c))
					{
						array[num++] = (byte)c;
						goto IL_0617;
					}
					if (!char.IsWhiteSpace(c))
					{
						if (!tryParse)
						{
							exc = new FormatException("Unknown char: " + c);
						}
						return false;
					}
					IL_05E7:
					if (flag2 && char.IsWhiteSpace(c))
					{
						num3 = 6;
						goto IL_0617;
					}
					if (!tryParse)
					{
						exc = new FormatException("Unknown char");
					}
					return false;
				}
				IL_0624:
				i++;
			}
			array[num] = 0;
			double num7;
			if (!double.ParseImpl(&array[0], out num7))
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			if (double.IsPositiveInfinity(num7) || double.IsNegativeInfinity(num7))
			{
				if (!tryParse)
				{
					exc = new OverflowException();
				}
				return false;
			}
			result = num7;
			return true;
		}

		private static bool TryParseStringConstant(string format, string s, int start, int end)
		{
			return end - start + 1 == format.Length && string.CompareOrdinal(format, 0, s, start, format.Length) == 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool ParseImpl(byte* byte_ptr, out double value);

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out double result)
		{
			Exception ex;
			if (!double.Parse(s, style, provider, true, out result, out ex))
			{
				result = 0.0;
				return false;
			}
			return true;
		}

		public static bool TryParse(string s, out double result)
		{
			return double.TryParse(s, NumberStyles.Any, null, out result);
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
			return TypeCode.Double;
		}

		public const double Epsilon = 5E-324;

		public const double MaxValue = 1.7976931348623157E+308;

		public const double MinValue = -1.7976931348623157E+308;

		public const double NaN = double.NaN;

		public const double NegativeInfinity = double.NegativeInfinity;

		public const double PositiveInfinity = double.PositiveInfinity;

		private const int State_AllowSign = 1;

		private const int State_Digits = 2;

		private const int State_Decimal = 3;

		private const int State_ExponentSign = 4;

		private const int State_Exponent = 5;

		private const int State_ConsumeWhiteSpace = 6;

		private const int State_Exit = 7;

		internal double m_value;
	}
}
