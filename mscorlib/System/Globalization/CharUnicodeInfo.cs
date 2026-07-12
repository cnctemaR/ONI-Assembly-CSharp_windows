using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Globalization
{
	public static class CharUnicodeInfo
	{
		internal static int InternalConvertToUtf32(string s, int index)
		{
			if (index < s.Length - 1)
			{
				int num = (int)(s[index] - '\ud800');
				if (num <= 1023)
				{
					int num2 = (int)(s[index + 1] - '\udc00');
					if (num2 <= 1023)
					{
						return num * 1024 + num2 + 65536;
					}
				}
			}
			return (int)s[index];
		}

		internal static int InternalConvertToUtf32(StringBuilder s, int index)
		{
			int num = (int)s[index];
			if (index < s.Length - 1)
			{
				int num2 = num - 55296;
				if (num2 <= 1023)
				{
					int num3 = (int)(s[index + 1] - '\udc00');
					if (num3 <= 1023)
					{
						return num2 * 1024 + num3 + 65536;
					}
				}
			}
			return num;
		}

		internal static int InternalConvertToUtf32(string s, int index, out int charLength)
		{
			charLength = 1;
			if (index < s.Length - 1)
			{
				int num = (int)(s[index] - '\ud800');
				if (num <= 1023)
				{
					int num2 = (int)(s[index + 1] - '\udc00');
					if (num2 <= 1023)
					{
						charLength++;
						return num * 1024 + num2 + 65536;
					}
				}
			}
			return (int)s[index];
		}

		internal unsafe static double InternalGetNumericValue(int ch)
		{
			int num = ch >> 8;
			if (num >= CharUnicodeInfo.NumericLevel1Index.Length)
			{
				return -1.0;
			}
			num = (int)(*CharUnicodeInfo.NumericLevel1Index[num]);
			num = (int)(*CharUnicodeInfo.NumericLevel2Index[(num << 4) + ((ch >> 4) & 15)]);
			num = (int)(*CharUnicodeInfo.NumericLevel3Index[(num << 4) + (ch & 15)]);
			ref byte ptr = ref Unsafe.AsRef<byte>(CharUnicodeInfo.NumericValues[num * 8]);
			if (BitConverter.IsLittleEndian)
			{
				return Unsafe.ReadUnaligned<double>(ref ptr);
			}
			return BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<long>(ref ptr)));
		}

		internal unsafe static byte InternalGetDigitValues(int ch, int offset)
		{
			int num = ch >> 8;
			if (num < CharUnicodeInfo.NumericLevel1Index.Length)
			{
				num = (int)(*CharUnicodeInfo.NumericLevel1Index[num]);
				num = (int)(*CharUnicodeInfo.NumericLevel2Index[(num << 4) + ((ch >> 4) & 15)]);
				num = (int)(*CharUnicodeInfo.NumericLevel3Index[(num << 4) + (ch & 15)]);
				return *CharUnicodeInfo.DigitValues[num * 2 + offset];
			}
			return byte.MaxValue;
		}

		public static double GetNumericValue(char ch)
		{
			return CharUnicodeInfo.InternalGetNumericValue((int)ch);
		}

		public static double GetNumericValue(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (index < 0 || index >= s.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			return CharUnicodeInfo.InternalGetNumericValue(CharUnicodeInfo.InternalConvertToUtf32(s, index));
		}

		public static int GetDecimalDigitValue(char ch)
		{
			return (int)((sbyte)CharUnicodeInfo.InternalGetDigitValues((int)ch, 0));
		}

		public static int GetDecimalDigitValue(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (index < 0 || index >= s.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			return (int)((sbyte)CharUnicodeInfo.InternalGetDigitValues(CharUnicodeInfo.InternalConvertToUtf32(s, index), 0));
		}

		public static int GetDigitValue(char ch)
		{
			return (int)((sbyte)CharUnicodeInfo.InternalGetDigitValues((int)ch, 1));
		}

		public static int GetDigitValue(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (index < 0 || index >= s.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			return (int)((sbyte)CharUnicodeInfo.InternalGetDigitValues(CharUnicodeInfo.InternalConvertToUtf32(s, index), 1));
		}

		public static UnicodeCategory GetUnicodeCategory(char ch)
		{
			return CharUnicodeInfo.GetUnicodeCategory((int)ch);
		}

		public static UnicodeCategory GetUnicodeCategory(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (index >= s.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return CharUnicodeInfo.InternalGetUnicodeCategory(s, index);
		}

		public static UnicodeCategory GetUnicodeCategory(int codePoint)
		{
			return (UnicodeCategory)CharUnicodeInfo.InternalGetCategoryValue(codePoint, 0);
		}

		internal unsafe static byte InternalGetCategoryValue(int ch, int offset)
		{
			int num = (int)(*CharUnicodeInfo.CategoryLevel1Index[ch >> 9]);
			num = (int)Unsafe.ReadUnaligned<ushort>(Unsafe.AsRef<byte>(CharUnicodeInfo.CategoryLevel2Index[(num << 6) + ((ch >> 3) & 62)]));
			if (!BitConverter.IsLittleEndian)
			{
				num = (int)BinaryPrimitives.ReverseEndianness((ushort)num);
			}
			num = (int)(*CharUnicodeInfo.CategoryLevel3Index[(num << 4) + (ch & 15)]);
			return *CharUnicodeInfo.CategoriesValue[num * 2 + offset];
		}

		internal static UnicodeCategory InternalGetUnicodeCategory(string value, int index)
		{
			return CharUnicodeInfo.GetUnicodeCategory(CharUnicodeInfo.InternalConvertToUtf32(value, index));
		}

		internal static BidiCategory GetBidiCategory(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (index >= s.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return (BidiCategory)CharUnicodeInfo.InternalGetCategoryValue(CharUnicodeInfo.InternalConvertToUtf32(s, index), 1);
		}

		internal static BidiCategory GetBidiCategory(StringBuilder s, int index)
		{
			return (BidiCategory)CharUnicodeInfo.InternalGetCategoryValue(CharUnicodeInfo.InternalConvertToUtf32(s, index), 1);
		}

		internal static UnicodeCategory InternalGetUnicodeCategory(string str, int index, out int charLength)
		{
			return CharUnicodeInfo.GetUnicodeCategory(CharUnicodeInfo.InternalConvertToUtf32(str, index, out charLength));
		}

		internal static bool IsCombiningCategory(UnicodeCategory uc)
		{
			return uc == UnicodeCategory.NonSpacingMark || uc == UnicodeCategory.SpacingCombiningMark || uc == UnicodeCategory.EnclosingMark;
		}

		internal static bool IsWhiteSpace(string s, int index)
		{
			UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(s, index);
			return unicodeCategory - UnicodeCategory.SpaceSeparator <= 2;
		}

		internal static bool IsWhiteSpace(char c)
		{
			UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			return unicodeCategory - UnicodeCategory.SpaceSeparator <= 2;
		}

		private unsafe static ReadOnlySpan<byte> CategoryLevel1Index
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.B55F94CD2F415D0279D7A1AF2265C4D9A90CE47F8C900D5D09AD088796210838), 2176);
			}
		}

		private unsafe static ReadOnlySpan<byte> CategoryLevel2Index
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.9086502742CE7F0595B57A4E5B32901FF4CF97959B92F7E91A435E4765AC1115), 5952);
			}
		}

		private unsafe static ReadOnlySpan<byte> CategoryLevel3Index
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.56073E3CC3FC817690CC306D0DB7EA63EBCB0801359567CA44CA3D3B9BF63854), 10800);
			}
		}

		private unsafe static ReadOnlySpan<byte> CategoriesValue
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.D6691EE5A533DE7E0859066942261B24D0C836D7EE016D2251377BFEE40FEA15), 172);
			}
		}

		private unsafe static ReadOnlySpan<byte> NumericLevel1Index
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.765BD07ED3CB498A599FFB48B31E077C45B4C2C37CD1547CEA27E60655CF21B6), 761);
			}
		}

		private unsafe static ReadOnlySpan<byte> NumericLevel2Index
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.F7D2AD02ED768134B31339AB059D864789E0A60090CC368B3881EB0631BBAF93), 1024);
			}
		}

		private unsafe static ReadOnlySpan<byte> NumericLevel3Index
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.AB0B9733AAEC4A2806711E41E36D3D0923BAF116156F33445DC2AA58DA5DF877), 1824);
			}
		}

		private unsafe static ReadOnlySpan<byte> NumericValues
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.692DE452EE427272A5F6154F04360D24165B56693B08F60D93127DEDC12D1DDE), 1320);
			}
		}

		private unsafe static ReadOnlySpan<byte> DigitValues
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.1A52279427700E21F7E68A077A8F17857A850718317B7228442260DBA2AF68F0), 330);
			}
		}

		internal const char HIGH_SURROGATE_START = '\ud800';

		internal const char HIGH_SURROGATE_END = '\udbff';

		internal const char LOW_SURROGATE_START = '\udc00';

		internal const char LOW_SURROGATE_END = '\udfff';

		internal const int HIGH_SURROGATE_RANGE = 1023;

		internal const int UNICODE_CATEGORY_OFFSET = 0;

		internal const int BIDI_CATEGORY_OFFSET = 1;

		internal const int UNICODE_PLANE01_START = 65536;
	}
}
