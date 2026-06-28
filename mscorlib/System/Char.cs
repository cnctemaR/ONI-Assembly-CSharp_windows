using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public struct Char : IConvertible, IComparable, IComparable<char>, IEquatable<char>
	{
		static Char()
		{
			char.GetDataTablePointers(out char.category_data, out char.numeric_data, out char.numeric_data_values, out char.to_lower_data_low, out char.to_lower_data_high, out char.to_upper_data_low, out char.to_upper_data_high);
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
			throw new InvalidCastException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return this;
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException();
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
			throw new InvalidCastException();
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetDataTablePointers(out byte* category_data, out byte* numeric_data, out double* numeric_data_values, out ushort* to_lower_data_low, out ushort* to_lower_data_high, out ushort* to_upper_data_low, out ushort* to_upper_data_high);

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is char))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Char"));
			}
			char c = (char)value;
			if (this == c)
			{
				return 0;
			}
			if (this > c)
			{
				return 1;
			}
			return -1;
		}

		public override bool Equals(object obj)
		{
			return obj is char && (char)obj == this;
		}

		public int CompareTo(char value)
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

		public static string ConvertFromUtf32(int utf32)
		{
			if (utf32 < 0 || utf32 > 1114111)
			{
				throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
			}
			if (55296 <= utf32 && utf32 <= 57343)
			{
				throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
			}
			if (utf32 < 65536)
			{
				return new string((char)utf32, 1);
			}
			utf32 -= 65536;
			return new string(new char[]
			{
				(char)((utf32 >> 10) + 55296),
				(char)(utf32 % 1024 + 56320)
			});
		}

		public static int ConvertToUtf32(char highSurrogate, char lowSurrogate)
		{
			if (highSurrogate < '\ud800' || '\udbff' < highSurrogate)
			{
				throw new ArgumentOutOfRangeException("highSurrogate");
			}
			if (lowSurrogate < '\udc00' || '\udfff' < lowSurrogate)
			{
				throw new ArgumentOutOfRangeException("lowSurrogate");
			}
			return 65536 + (int)((int)(highSurrogate - '\ud800') << 10) + (int)(lowSurrogate - '\udc00');
		}

		public static int ConvertToUtf32(string s, int index)
		{
			char.CheckParameter(s, index);
			if (!char.IsSurrogate(s[index]))
			{
				return (int)s[index];
			}
			if (!char.IsHighSurrogate(s[index]) || index == s.Length - 1 || !char.IsLowSurrogate(s[index + 1]))
			{
				throw new ArgumentException(string.Format("The string contains invalid surrogate pair character at {0}", index));
			}
			return char.ConvertToUtf32(s[index], s[index + 1]);
		}

		public bool Equals(char obj)
		{
			return this == obj;
		}

		public static bool IsSurrogatePair(char highSurrogate, char lowSurrogate)
		{
			return '\ud800' <= highSurrogate && highSurrogate <= '\udbff' && '\udc00' <= lowSurrogate && lowSurrogate <= '\udfff';
		}

		public static bool IsSurrogatePair(string s, int index)
		{
			char.CheckParameter(s, index);
			return index + 1 < s.Length && char.IsSurrogatePair(s[index], s[index + 1]);
		}

		public override int GetHashCode()
		{
			return (int)this;
		}

		public unsafe static double GetNumericValue(char c)
		{
			if (c <= '㊉')
			{
				return char.numeric_data_values[char.numeric_data[c]];
			}
			if (c >= '０' && c <= '９')
			{
				return (double)(c - '０');
			}
			return -1.0;
		}

		public static double GetNumericValue(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.GetNumericValue(s[index]);
		}

		public unsafe static UnicodeCategory GetUnicodeCategory(char c)
		{
			return (UnicodeCategory)char.category_data[c];
		}

		public static UnicodeCategory GetUnicodeCategory(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.GetUnicodeCategory(s[index]);
		}

		public unsafe static bool IsControl(char c)
		{
			return char.category_data[c] == 14;
		}

		public static bool IsControl(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsControl(s[index]);
		}

		public unsafe static bool IsDigit(char c)
		{
			return char.category_data[c] == 8;
		}

		public static bool IsDigit(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsDigit(s[index]);
		}

		public static bool IsHighSurrogate(char c)
		{
			return c >= '\ud800' && c <= '\udbff';
		}

		public static bool IsHighSurrogate(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsHighSurrogate(s[index]);
		}

		public unsafe static bool IsLetter(char c)
		{
			return char.category_data[c] <= 4;
		}

		public static bool IsLetter(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsLetter(s[index]);
		}

		public unsafe static bool IsLetterOrDigit(char c)
		{
			int num = (int)char.category_data[c];
			return num <= 4 || num == 8;
		}

		public static bool IsLetterOrDigit(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsLetterOrDigit(s[index]);
		}

		public unsafe static bool IsLower(char c)
		{
			return char.category_data[c] == 1;
		}

		public static bool IsLower(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsLower(s[index]);
		}

		public static bool IsLowSurrogate(char c)
		{
			return c >= '\udc00' && c <= '\udfff';
		}

		public static bool IsLowSurrogate(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsLowSurrogate(s[index]);
		}

		public unsafe static bool IsNumber(char c)
		{
			int num = (int)char.category_data[c];
			return num >= 8 && num <= 10;
		}

		public static bool IsNumber(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsNumber(s[index]);
		}

		public unsafe static bool IsPunctuation(char c)
		{
			int num = (int)char.category_data[c];
			return num >= 18 && num <= 24;
		}

		public static bool IsPunctuation(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsPunctuation(s[index]);
		}

		public unsafe static bool IsSeparator(char c)
		{
			int num = (int)char.category_data[c];
			return num >= 11 && num <= 13;
		}

		public static bool IsSeparator(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsSeparator(s[index]);
		}

		public unsafe static bool IsSurrogate(char c)
		{
			return char.category_data[c] == 16;
		}

		public static bool IsSurrogate(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsSurrogate(s[index]);
		}

		public unsafe static bool IsSymbol(char c)
		{
			int num = (int)char.category_data[c];
			return num >= 25 && num <= 28;
		}

		public static bool IsSymbol(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsSymbol(s[index]);
		}

		public unsafe static bool IsUpper(char c)
		{
			return char.category_data[c] == 0;
		}

		public static bool IsUpper(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsUpper(s[index]);
		}

		public unsafe static bool IsWhiteSpace(char c)
		{
			int num = (int)char.category_data[c];
			return num > 10 && (num <= 13 || (c >= '\t' && c <= '\r') || c == '\u0085' || c == '\u205f');
		}

		public static bool IsWhiteSpace(string s, int index)
		{
			char.CheckParameter(s, index);
			return char.IsWhiteSpace(s[index]);
		}

		private static void CheckParameter(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (index < 0 || index >= s.Length)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("The value of index is less than zero, or greater than or equal to the length of s."));
			}
		}

		public static bool TryParse(string s, out char result)
		{
			if (s == null || s.Length != 1)
			{
				result = '\0';
				return false;
			}
			result = s[0];
			return true;
		}

		public static char Parse(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length != 1)
			{
				throw new FormatException(Locale.GetText("s contains more than one character."));
			}
			return s[0];
		}

		public static char ToLower(char c)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToLower(c);
		}

		public unsafe static char ToLowerInvariant(char c)
		{
			if (c <= 'Ⓩ')
			{
				return (char)char.to_lower_data_low[c];
			}
			if (c >= 'Ａ')
			{
				return (char)char.to_lower_data_high[c - 'Ａ'];
			}
			return c;
		}

		public static char ToLower(char c, CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (culture.LCID == 127)
			{
				return char.ToLowerInvariant(c);
			}
			return culture.TextInfo.ToLower(c);
		}

		public static char ToUpper(char c)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToUpper(c);
		}

		public unsafe static char ToUpperInvariant(char c)
		{
			if (c <= 'ⓩ')
			{
				return (char)char.to_upper_data_low[c];
			}
			if (c >= 'Ａ')
			{
				return (char)char.to_upper_data_high[c - 'Ａ'];
			}
			return c;
		}

		public static char ToUpper(char c, CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (culture.LCID == 127)
			{
				return char.ToUpperInvariant(c);
			}
			return culture.TextInfo.ToUpper(c);
		}

		public override string ToString()
		{
			return new string(this, 1);
		}

		public static string ToString(char c)
		{
			return new string(c, 1);
		}

		public string ToString(IFormatProvider provider)
		{
			return new string(this, 1);
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Char;
		}

		public const char MaxValue = '\uffff';

		public const char MinValue = '\0';

		internal char m_value;

		private unsafe static readonly byte* category_data;

		private unsafe static readonly byte* numeric_data;

		private unsafe static readonly double* numeric_data_values;

		private unsafe static readonly ushort* to_lower_data_low;

		private unsafe static readonly ushort* to_lower_data_high;

		private unsafe static readonly ushort* to_upper_data_low;

		private unsafe static readonly ushort* to_upper_data_high;
	}
}
