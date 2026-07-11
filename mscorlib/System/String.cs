using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Globalization.Unicode;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class String : IConvertible, IComparable, IEnumerable, ICloneable, IComparable<string>, IEquatable<string>, IEnumerable<char>
	{
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(char* value);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(char* value, int startIndex, int length);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(sbyte* value);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(sbyte* value, int startIndex, int length);

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(sbyte* value, int startIndex, int length, Encoding enc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char[] value, int startIndex, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char c, int count);

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this, provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this, provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this, provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(this, provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this, provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this, provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this, provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this, provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this, provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this, provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this, provider);
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("type");
			}
			return Convert.ToType(this, targetType, provider, false);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this, provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this, provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this, provider);
		}

		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		public unsafe static bool Equals(string a, string b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			int i = a.length;
			if (i != b.length)
			{
				return false;
			}
			char* ptr = &a.start_char;
			char* ptr2 = &b.start_char;
			while (i >= 8)
			{
				if (*(int*)ptr != *(int*)ptr2 || *(int*)(ptr + 2) != *(int*)(ptr2 + 2) || *(int*)(ptr + 4) != *(int*)(ptr2 + 4) || *(int*)(ptr + 6) != *(int*)(ptr2 + 6))
				{
					return false;
				}
				ptr += 8;
				ptr2 += 8;
				i -= 8;
			}
			if (i >= 4)
			{
				if (*(int*)ptr != *(int*)ptr2 || *(int*)(ptr + 2) != *(int*)(ptr2 + 2))
				{
					return false;
				}
				ptr += 4;
				ptr2 += 4;
				i -= 4;
			}
			if (i > 1)
			{
				if (*(int*)ptr != *(int*)ptr2)
				{
					return false;
				}
				ptr += 2;
				ptr2 += 2;
				i -= 2;
			}
			return i == 0 || *ptr == *ptr2;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public override bool Equals(object obj)
		{
			return string.Equals(this, obj as string);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public bool Equals(string value)
		{
			return string.Equals(this, value);
		}

		[IndexerName("Chars")]
		public unsafe char this[int index]
		{
			get
			{
				if (index < 0 || index >= this.length)
				{
					throw new IndexOutOfRangeException();
				}
				return *((ref this.start_char) + index * 2);
			}
		}

		public object Clone()
		{
			return this;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.String;
		}

		public unsafe void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (sourceIndex < 0)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", "Cannot be negative");
			}
			if (destinationIndex < 0)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", "Cannot be negative.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative.");
			}
			if (sourceIndex > this.Length - count)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", "sourceIndex + count > Length");
			}
			if (destinationIndex > destination.Length - count)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", "destinationIndex + count > destination.Length");
			}
			fixed (char* ptr = (ref destination != null && destination.Length != 0 ? ref destination[0] : ref *null))
			{
				fixed (string text = this)
				{
					fixed (char* ptr2 = text + RuntimeHelpers.OffsetToStringData / 2)
					{
						string.CharCopy(ptr + destinationIndex, ptr2 + sourceIndex, count);
						ptr = null;
						text = null;
						return;
					}
				}
			}
		}

		public char[] ToCharArray()
		{
			return this.ToCharArray(0, this.length);
		}

		public unsafe char[] ToCharArray(int startIndex, int length)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "< 0");
			}
			if (startIndex > this.length - length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Must be greater than the length of the string.");
			}
			char[] array = new char[length];
			fixed (char* ptr = (ref array != null && array.Length != 0 ? ref array[0] : ref *null))
			{
				fixed (string text = this)
				{
					fixed (char* ptr2 = text + RuntimeHelpers.OffsetToStringData / 2)
					{
						string.CharCopy(ptr, ptr2 + startIndex, length);
						ptr = null;
						text = null;
						return array;
					}
				}
			}
		}

		public string[] Split(params char[] separator)
		{
			return this.Split(separator, int.MaxValue);
		}

		public string[] Split(char[] separator, int count)
		{
			if (separator == null || separator.Length == 0)
			{
				separator = string.WhiteChars;
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				return new string[0];
			}
			if (count == 1)
			{
				return new string[] { this };
			}
			return this.InternalSplit(separator, count, 0);
		}

		[ComVisible(false)]
		[MonoDocumentationNote("code should be moved to managed")]
		public string[] Split(char[] separator, int count, StringSplitOptions options)
		{
			if (separator == null || separator.Length == 0)
			{
				return this.Split(string.WhiteChars, count, options);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			if (options != StringSplitOptions.None && options != StringSplitOptions.RemoveEmptyEntries)
			{
				throw new ArgumentException("Illegal enum value: " + options + ".");
			}
			if (count == 0)
			{
				return new string[0];
			}
			return this.InternalSplit(separator, count, (int)options);
		}

		[ComVisible(false)]
		public string[] Split(string[] separator, int count, StringSplitOptions options)
		{
			if (separator == null || separator.Length == 0)
			{
				return this.Split(string.WhiteChars, count, options);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			if (options != StringSplitOptions.None && options != StringSplitOptions.RemoveEmptyEntries)
			{
				throw new ArgumentException("Illegal enum value: " + options + ".");
			}
			if (count == 1)
			{
				return new string[] { this };
			}
			bool flag = (options & StringSplitOptions.RemoveEmptyEntries) == StringSplitOptions.RemoveEmptyEntries;
			if (count == 0 || (this == string.Empty && flag))
			{
				return new string[0];
			}
			List<string> list = new List<string>();
			int i = 0;
			int num = 0;
			while (i < this.Length)
			{
				int num2 = -1;
				int num3 = int.MaxValue;
				for (int j = 0; j < separator.Length; j++)
				{
					string text = separator[j];
					if (text != null && !(text == string.Empty))
					{
						int num4 = this.IndexOf(text, i);
						if (num4 > -1 && num4 < num3)
						{
							num2 = j;
							num3 = num4;
						}
					}
				}
				if (num2 == -1)
				{
					break;
				}
				if (num3 != i || !flag)
				{
					if (list.Count == count - 1)
					{
						break;
					}
					list.Add(this.Substring(i, num3 - i));
				}
				i = num3 + separator[num2].Length;
				num++;
			}
			if (num == 0)
			{
				return new string[] { this };
			}
			if (flag && num != 0 && i == this.Length && list.Count == 0)
			{
				return new string[0];
			}
			if (!flag || i != this.Length)
			{
				list.Add(this.Substring(i));
			}
			return list.ToArray();
		}

		[ComVisible(false)]
		public string[] Split(char[] separator, StringSplitOptions options)
		{
			return this.Split(separator, int.MaxValue, options);
		}

		[ComVisible(false)]
		public string[] Split(string[] separator, StringSplitOptions options)
		{
			return this.Split(separator, int.MaxValue, options);
		}

		public string Substring(int startIndex)
		{
			if (startIndex == 0)
			{
				return this;
			}
			if (startIndex < 0 || startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			return this.SubstringUnchecked(startIndex, this.length - startIndex);
		}

		public string Substring(int startIndex, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Cannot be negative.");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative.");
			}
			if (startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot exceed length of string.");
			}
			if (startIndex > this.length - length)
			{
				throw new ArgumentOutOfRangeException("length", "startIndex + length > this.length");
			}
			if (startIndex == 0 && length == this.length)
			{
				return this;
			}
			return this.SubstringUnchecked(startIndex, length);
		}

		internal unsafe string SubstringUnchecked(int startIndex, int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(length);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text3 = this)
					{
						fixed (char* ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr, ptr2 + startIndex, length);
							text2 = null;
							text3 = null;
							return text;
						}
					}
				}
			}
		}

		public string Trim()
		{
			if (this.length == 0)
			{
				return string.Empty;
			}
			int num = this.FindNotWhiteSpace(0, this.length, 1);
			if (num == this.length)
			{
				return string.Empty;
			}
			int num2 = this.FindNotWhiteSpace(this.length - 1, num, -1);
			int num3 = num2 - num + 1;
			if (num3 == this.length)
			{
				return this;
			}
			return this.SubstringUnchecked(num, num3);
		}

		public string Trim(params char[] trimChars)
		{
			if (trimChars == null || trimChars.Length == 0)
			{
				return this.Trim();
			}
			if (this.length == 0)
			{
				return string.Empty;
			}
			int num = this.FindNotInTable(0, this.length, 1, trimChars);
			if (num == this.length)
			{
				return string.Empty;
			}
			int num2 = this.FindNotInTable(this.length - 1, num, -1, trimChars);
			int num3 = num2 - num + 1;
			if (num3 == this.length)
			{
				return this;
			}
			return this.SubstringUnchecked(num, num3);
		}

		public string TrimStart(params char[] trimChars)
		{
			if (this.length == 0)
			{
				return string.Empty;
			}
			int num;
			if (trimChars == null || trimChars.Length == 0)
			{
				num = this.FindNotWhiteSpace(0, this.length, 1);
			}
			else
			{
				num = this.FindNotInTable(0, this.length, 1, trimChars);
			}
			if (num == 0)
			{
				return this;
			}
			return this.SubstringUnchecked(num, this.length - num);
		}

		public string TrimEnd(params char[] trimChars)
		{
			if (this.length == 0)
			{
				return string.Empty;
			}
			int num;
			if (trimChars == null || trimChars.Length == 0)
			{
				num = this.FindNotWhiteSpace(this.length - 1, -1, -1);
			}
			else
			{
				num = this.FindNotInTable(this.length - 1, -1, -1, trimChars);
			}
			num++;
			if (num == this.length)
			{
				return this;
			}
			return this.SubstringUnchecked(0, num);
		}

		private int FindNotWhiteSpace(int pos, int target, int change)
		{
			while (pos != target)
			{
				char c = this[pos];
				if (c < '\u0085')
				{
					if (c != ' ' && (c < '\t' || c > '\r'))
					{
						return pos;
					}
				}
				else if (c != '\u00a0' && c != '\ufeff' && c != '\u3000' && c != '\u0085' && c != '\u1680' && c != '\u2028' && c != '\u2029' && (c < '\u2000' || c > '\u200b'))
				{
					return pos;
				}
				pos += change;
			}
			return pos;
		}

		private unsafe int FindNotInTable(int pos, int target, int change, char[] table)
		{
			fixed (char* ptr = (ref table != null && table.Length != 0 ? ref table[0] : ref *null))
			{
				fixed (string text = this)
				{
					fixed (char* ptr2 = text + RuntimeHelpers.OffsetToStringData / 2)
					{
						while (pos != target)
						{
							char c = ptr2[pos];
							int i;
							for (i = 0; i < table.Length; i++)
							{
								if (c == ptr[i])
								{
									break;
								}
							}
							if (i == table.Length)
							{
								return pos;
							}
							pos += change;
						}
						ptr = null;
						text = null;
						return pos;
					}
				}
			}
		}

		public static int Compare(string strA, string strB)
		{
			return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.None);
		}

		public static int Compare(string strA, string strB, bool ignoreCase)
		{
			return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, (!ignoreCase) ? CompareOptions.None : CompareOptions.IgnoreCase);
		}

		public static int Compare(string strA, string strB, bool ignoreCase, CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return culture.CompareInfo.Compare(strA, strB, (!ignoreCase) ? CompareOptions.None : CompareOptions.IgnoreCase);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length)
		{
			return string.Compare(strA, indexA, strB, indexB, length, false, CultureInfo.CurrentCulture);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase)
		{
			return string.Compare(strA, indexA, strB, indexB, length, ignoreCase, CultureInfo.CurrentCulture);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase, CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (indexA > strA.Length || indexB > strB.Length || indexA < 0 || indexB < 0 || length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (length == 0)
			{
				return 0;
			}
			if (strA == null)
			{
				if (strB == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (strB == null)
				{
					return 1;
				}
				CompareOptions compareOptions;
				if (ignoreCase)
				{
					compareOptions = CompareOptions.IgnoreCase;
				}
				else
				{
					compareOptions = CompareOptions.None;
				}
				int num = length;
				int num2 = length;
				if (length > strA.Length - indexA)
				{
					num = strA.Length - indexA;
				}
				if (length > strB.Length - indexB)
				{
					num2 = strB.Length - indexB;
				}
				return culture.CompareInfo.Compare(strA, indexA, num, strB, indexB, num2, compareOptions);
			}
		}

		public static int Compare(string strA, string strB, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return string.Compare(strA, strB, false, CultureInfo.CurrentCulture);
			case StringComparison.CurrentCultureIgnoreCase:
				return string.Compare(strA, strB, true, CultureInfo.CurrentCulture);
			case StringComparison.InvariantCulture:
				return string.Compare(strA, strB, false, CultureInfo.InvariantCulture);
			case StringComparison.InvariantCultureIgnoreCase:
				return string.Compare(strA, strB, true, CultureInfo.InvariantCulture);
			case StringComparison.Ordinal:
				return string.CompareOrdinalUnchecked(strA, 0, int.MaxValue, strB, 0, int.MaxValue);
			case StringComparison.OrdinalIgnoreCase:
				return string.CompareOrdinalCaseInsensitiveUnchecked(strA, 0, int.MaxValue, strB, 0, int.MaxValue);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", new object[] { comparisonType });
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return string.Compare(strA, indexA, strB, indexB, length, false, CultureInfo.CurrentCulture);
			case StringComparison.CurrentCultureIgnoreCase:
				return string.Compare(strA, indexA, strB, indexB, length, true, CultureInfo.CurrentCulture);
			case StringComparison.InvariantCulture:
				return string.Compare(strA, indexA, strB, indexB, length, false, CultureInfo.InvariantCulture);
			case StringComparison.InvariantCultureIgnoreCase:
				return string.Compare(strA, indexA, strB, indexB, length, true, CultureInfo.InvariantCulture);
			case StringComparison.Ordinal:
				return string.CompareOrdinal(strA, indexA, strB, indexB, length);
			case StringComparison.OrdinalIgnoreCase:
				return string.CompareOrdinalCaseInsensitive(strA, indexA, strB, indexB, length);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", new object[] { comparisonType });
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		public static bool Equals(string a, string b, StringComparison comparisonType)
		{
			return string.Compare(a, b, comparisonType) == 0;
		}

		public bool Equals(string value, StringComparison comparisonType)
		{
			return string.Compare(value, this, comparisonType) == 0;
		}

		public static int Compare(string strA, string strB, CultureInfo culture, CompareOptions options)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return culture.CompareInfo.Compare(strA, strB, options);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, CultureInfo culture, CompareOptions options)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			int num = length;
			int num2 = length;
			if (length > strA.Length - indexA)
			{
				num = strA.Length - indexA;
			}
			if (length > strB.Length - indexB)
			{
				num2 = strB.Length - indexB;
			}
			return culture.CompareInfo.Compare(strA, indexA, num, strB, indexB, num2, options);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is string))
			{
				throw new ArgumentException();
			}
			return string.Compare(this, (string)value);
		}

		public int CompareTo(string strB)
		{
			if (strB == null)
			{
				return 1;
			}
			return string.Compare(this, strB);
		}

		public static int CompareOrdinal(string strA, string strB)
		{
			return string.CompareOrdinalUnchecked(strA, 0, int.MaxValue, strB, 0, int.MaxValue);
		}

		public static int CompareOrdinal(string strA, int indexA, string strB, int indexB, int length)
		{
			if (indexA > strA.Length || indexB > strB.Length || indexA < 0 || indexB < 0 || length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return string.CompareOrdinalUnchecked(strA, indexA, length, strB, indexB, length);
		}

		internal static int CompareOrdinalCaseInsensitive(string strA, int indexA, string strB, int indexB, int length)
		{
			if (indexA > strA.Length || indexB > strB.Length || indexA < 0 || indexB < 0 || length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return string.CompareOrdinalCaseInsensitiveUnchecked(strA, indexA, length, strB, indexB, length);
		}

		internal unsafe static int CompareOrdinalUnchecked(string strA, int indexA, int lenA, string strB, int indexB, int lenB)
		{
			if (strA == null)
			{
				if (strB == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (strB == null)
				{
					return 1;
				}
				int num = Math.Min(lenA, strA.Length - indexA);
				int num2 = Math.Min(lenB, strB.Length - indexB);
				if (num == num2 && object.ReferenceEquals(strA, strB))
				{
					return 0;
				}
				fixed (char* ptr = strA + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (char* ptr2 = strB + RuntimeHelpers.OffsetToStringData / 2)
					{
						char* ptr3 = ptr + indexA;
						char* ptr4 = ptr3 + Math.Min(num, num2);
						char* ptr5 = ptr2 + indexB;
						while (ptr3 < ptr4)
						{
							if (*ptr3 != *ptr5)
							{
								return (int)(*ptr3 - *ptr5);
							}
							ptr3++;
							ptr5++;
						}
						return num - num2;
					}
				}
			}
		}

		internal unsafe static int CompareOrdinalCaseInsensitiveUnchecked(string strA, int indexA, int lenA, string strB, int indexB, int lenB)
		{
			if (strA == null)
			{
				if (strB == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (strB == null)
				{
					return 1;
				}
				int num = Math.Min(lenA, strA.Length - indexA);
				int num2 = Math.Min(lenB, strB.Length - indexB);
				if (num == num2 && object.ReferenceEquals(strA, strB))
				{
					return 0;
				}
				fixed (char* ptr = strA + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (char* ptr2 = strB + RuntimeHelpers.OffsetToStringData / 2)
					{
						char* ptr3 = ptr + indexA;
						char* ptr4 = ptr3 + Math.Min(num, num2);
						char* ptr5 = ptr2 + indexB;
						while (ptr3 < ptr4)
						{
							if (*ptr3 != *ptr5)
							{
								char c = char.ToUpperInvariant(*ptr3);
								char c2 = char.ToUpperInvariant(*ptr5);
								if (c != c2)
								{
									return (int)(c - c2);
								}
							}
							ptr3++;
							ptr5++;
						}
						return num - num2;
					}
				}
			}
		}

		public bool EndsWith(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None);
		}

		public bool EndsWith(string value, bool ignoreCase, CultureInfo culture)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			return culture.CompareInfo.IsSuffix(this, value, (!ignoreCase) ? CompareOptions.None : CompareOptions.IgnoreCase);
		}

		public int IndexOfAny(char[] anyOf)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (this.length == 0)
			{
				return -1;
			}
			return this.IndexOfAnyUnchecked(anyOf, 0, this.length);
		}

		public int IndexOfAny(char[] anyOf, int startIndex)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (startIndex < 0 || startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.IndexOfAnyUnchecked(anyOf, startIndex, this.length - startIndex);
		}

		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (startIndex < 0 || startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (count < 0 || startIndex > this.length - count)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be negative, and startIndex + count must be less than length of the string.");
			}
			return this.IndexOfAnyUnchecked(anyOf, startIndex, count);
		}

		private unsafe int IndexOfAnyUnchecked(char[] anyOf, int startIndex, int count)
		{
			if (anyOf.Length == 0)
			{
				return -1;
			}
			if (anyOf.Length == 1)
			{
				return this.IndexOfUnchecked(anyOf[0], startIndex, count);
			}
			fixed (char* ptr = (ref anyOf != null && anyOf.Length != 0 ? ref anyOf[0] : ref *null))
			{
				int num = (int)(*ptr);
				int num2 = (int)(*ptr);
				char* ptr2 = ptr + anyOf.Length;
				char* ptr3 = ptr;
				while (++ptr3 != ptr2)
				{
					if ((int)(*ptr3) > num)
					{
						num = (int)(*ptr3);
					}
					else if ((int)(*ptr3) < num2)
					{
						num2 = (int)(*ptr3);
					}
				}
				fixed (char* ptr4 = &this.start_char)
				{
					char* ptr5 = ptr4 + startIndex;
					char* ptr6 = ptr5 + count;
					while (ptr5 != ptr6)
					{
						if ((int)(*ptr5) > num || (int)(*ptr5) < num2)
						{
							ptr5++;
						}
						else
						{
							if (*ptr5 == *ptr)
							{
								return (int)((long)(ptr5 - ptr4));
							}
							ptr3 = ptr;
							while (++ptr3 != ptr2)
							{
								if (*ptr5 == *ptr3)
								{
									return (int)((long)(ptr5 - ptr4));
								}
							}
							ptr5++;
						}
					}
				}
			}
			return -1;
		}

		public int IndexOf(string value, StringComparison comparisonType)
		{
			return this.IndexOf(value, 0, this.Length, comparisonType);
		}

		public int IndexOf(string value, int startIndex, StringComparison comparisonType)
		{
			return this.IndexOf(value, startIndex, this.Length - startIndex, comparisonType);
		}

		public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CultureInfo.InvariantCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return this.IndexOfOrdinal(value, startIndex, count, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return this.IndexOfOrdinal(value, startIndex, count, CompareOptions.OrdinalIgnoreCase);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", new object[] { comparisonType });
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		internal int IndexOfOrdinal(string value, int startIndex, int count, CompareOptions options)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || this.length - startIndex < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (options == CompareOptions.Ordinal)
			{
				return this.IndexOfOrdinalUnchecked(value, startIndex, count);
			}
			return this.IndexOfOrdinalIgnoreCaseUnchecked(value, startIndex, count);
		}

		internal unsafe int IndexOfOrdinalUnchecked(string value, int startIndex, int count)
		{
			int num = value.Length;
			if (count < num)
			{
				return -1;
			}
			if (num > 1)
			{
				fixed (string text = this)
				{
					fixed (char* ptr = text + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text2 = value)
						{
							fixed (char* ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
							{
								char* ptr3 = ptr + startIndex;
								char* ptr4 = ptr3 + count - num + 1;
								while (ptr3 != ptr4)
								{
									if (*ptr3 == *ptr2)
									{
										for (int i = 1; i < num; i++)
										{
											if (ptr3[i] != ptr2[i])
											{
												goto IL_00A1;
											}
										}
										return (int)((long)(ptr3 - ptr));
									}
									IL_00A1:
									ptr3++;
								}
								text = null;
								text2 = null;
								return -1;
							}
						}
					}
				}
			}
			if (num == 1)
			{
				return this.IndexOfUnchecked(value[0], startIndex, count);
			}
			return startIndex;
		}

		internal unsafe int IndexOfOrdinalIgnoreCaseUnchecked(string value, int startIndex, int count)
		{
			int num = value.Length;
			if (count < num)
			{
				return -1;
			}
			if (num == 0)
			{
				return startIndex;
			}
			fixed (string text = this)
			{
				fixed (char* ptr = text + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text2 = value)
					{
						fixed (char* ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
						{
							char* ptr3 = ptr + startIndex;
							char* ptr4 = ptr3 + count - num + 1;
							IL_008F:
							while (ptr3 != ptr4)
							{
								for (int i = 0; i < num; i++)
								{
									if (char.ToUpperInvariant(ptr3[i]) != char.ToUpperInvariant(ptr2[i]))
									{
										ptr3++;
										goto IL_008F;
									}
								}
								return (int)((long)(ptr3 - ptr));
							}
							text = null;
							text2 = null;
							return -1;
						}
					}
				}
			}
		}

		public int LastIndexOf(string value, StringComparison comparisonType)
		{
			if (this.Length == 0)
			{
				return (!(value == string.Empty)) ? (-1) : 0;
			}
			return this.LastIndexOf(value, this.Length - 1, this.Length, comparisonType);
		}

		public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
		{
			return this.LastIndexOf(value, startIndex, startIndex + 1, comparisonType);
		}

		public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return this.LastIndexOfOrdinal(value, startIndex, count, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return this.LastIndexOfOrdinal(value, startIndex, count, CompareOptions.OrdinalIgnoreCase);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", new object[] { comparisonType });
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		internal int LastIndexOfOrdinal(string value, int startIndex, int count, CompareOptions options)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || startIndex < count - 1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (options == CompareOptions.Ordinal)
			{
				return this.LastIndexOfOrdinalUnchecked(value, startIndex, count);
			}
			return this.LastIndexOfOrdinalIgnoreCaseUnchecked(value, startIndex, count);
		}

		internal unsafe int LastIndexOfOrdinalUnchecked(string value, int startIndex, int count)
		{
			int num = value.Length;
			if (count < num)
			{
				return -1;
			}
			if (num > 1)
			{
				fixed (string text = this)
				{
					fixed (char* ptr = text + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text2 = value)
						{
							fixed (char* ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
							{
								char* ptr3 = ptr + startIndex - num + 1;
								char* ptr4 = ptr3 - count + num - 1;
								while (ptr3 != ptr4)
								{
									if (*ptr3 == *ptr2)
									{
										for (int i = 1; i < num; i++)
										{
											if (ptr3[i] != ptr2[i])
											{
												goto IL_00A7;
											}
										}
										return (int)((long)(ptr3 - ptr));
									}
									IL_00A7:
									ptr3--;
								}
								text = null;
								text2 = null;
								return -1;
							}
						}
					}
				}
			}
			if (num == 1)
			{
				return this.LastIndexOfUnchecked(value[0], startIndex, count);
			}
			return startIndex;
		}

		internal unsafe int LastIndexOfOrdinalIgnoreCaseUnchecked(string value, int startIndex, int count)
		{
			int num = value.Length;
			if (count < num)
			{
				return -1;
			}
			if (num == 0)
			{
				return startIndex;
			}
			fixed (string text = this)
			{
				fixed (char* ptr = text + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text2 = value)
					{
						fixed (char* ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
						{
							char* ptr3 = ptr + startIndex - num + 1;
							char* ptr4 = ptr3 - count + num - 1;
							IL_0095:
							while (ptr3 != ptr4)
							{
								for (int i = 0; i < num; i++)
								{
									if (char.ToUpperInvariant(ptr3[i]) != char.ToUpperInvariant(ptr2[i]))
									{
										ptr3--;
										goto IL_0095;
									}
								}
								return (int)((long)(ptr3 - ptr));
							}
							text = null;
							text2 = null;
							return -1;
						}
					}
				}
			}
		}

		public int IndexOf(char value)
		{
			if (this.length == 0)
			{
				return -1;
			}
			return this.IndexOfUnchecked(value, 0, this.length);
		}

		public int IndexOf(char value, int startIndex)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0");
			}
			if (startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "startIndex > this.length");
			}
			if ((startIndex == 0 && this.length == 0) || startIndex == this.length)
			{
				return -1;
			}
			return this.IndexOfUnchecked(value, startIndex, this.length - startIndex);
		}

		public int IndexOf(char value, int startIndex, int count)
		{
			if (startIndex < 0 || startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative and must be< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (startIndex > this.length - count)
			{
				throw new ArgumentOutOfRangeException("count", "startIndex + count > this.length");
			}
			if ((startIndex == 0 && this.length == 0) || startIndex == this.length || count == 0)
			{
				return -1;
			}
			return this.IndexOfUnchecked(value, startIndex, count);
		}

		internal unsafe int IndexOfUnchecked(char value, int startIndex, int count)
		{
			char* ptr = (ref this.start_char) + startIndex * 2;
			char* ptr2 = ptr + (count >> 3 << 3);
			while (ptr != ptr2)
			{
				if (*ptr == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2;
				}
				if (ptr[1] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 + 1L / 2L;
				}
				if (ptr[2] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 + 2L / 2L;
				}
				if (ptr[3] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 + 3L / 2L;
				}
				if (ptr[4] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 + 4L / 2L;
				}
				if (ptr[5] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 + 5L / 2L;
				}
				if (ptr[6] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 + 6L / 2L;
				}
				if (ptr[7] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 + 7L / 2L;
				}
				ptr += 8;
			}
			ptr2 += count & 7;
			while (ptr != ptr2)
			{
				if (*ptr == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2;
				}
				ptr++;
			}
			return -1;
		}

		internal unsafe int IndexOfOrdinalIgnoreCase(char value, int startIndex, int count)
		{
			if (this.length == 0)
			{
				return -1;
			}
			int num = startIndex + count;
			char c = char.ToUpperInvariant(value);
			fixed (char* ptr = &this.start_char)
			{
				for (int i = startIndex; i < num; i++)
				{
					if (char.ToUpperInvariant(ptr[i]) == c)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int IndexOf(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.length == 0)
			{
				return 0;
			}
			if (this.length == 0)
			{
				return -1;
			}
			return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, 0, this.length, CompareOptions.Ordinal);
		}

		public int IndexOf(string value, int startIndex)
		{
			return this.IndexOf(value, startIndex, this.length - startIndex);
		}

		public int IndexOf(string value, int startIndex, int count)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative, and should not exceed length of string.");
			}
			if (count < 0 || startIndex > this.length - count)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative, and should point to location in string.");
			}
			if (value.length == 0)
			{
				return startIndex;
			}
			if (startIndex == 0 && this.length == 0)
			{
				return -1;
			}
			if (count == 0)
			{
				return -1;
			}
			return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count);
		}

		public int LastIndexOfAny(char[] anyOf)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			return this.LastIndexOfAnyUnchecked(anyOf, this.length - 1, this.length);
		}

		public int LastIndexOfAny(char[] anyOf, int startIndex)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (startIndex < 0 || startIndex >= this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative, and should be less than length of string.");
			}
			if (this.length == 0)
			{
				return -1;
			}
			return this.LastIndexOfAnyUnchecked(anyOf, startIndex, startIndex + 1);
		}

		public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (startIndex < 0 || startIndex >= this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0 || > this.Length");
			}
			if (count < 0 || count > this.Length)
			{
				throw new ArgumentOutOfRangeException("count", "< 0 || > this.Length");
			}
			if (startIndex - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex - count + 1 < 0");
			}
			if (this.length == 0)
			{
				return -1;
			}
			return this.LastIndexOfAnyUnchecked(anyOf, startIndex, count);
		}

		private unsafe int LastIndexOfAnyUnchecked(char[] anyOf, int startIndex, int count)
		{
			if (anyOf.Length == 1)
			{
				return this.LastIndexOfUnchecked(anyOf[0], startIndex, count);
			}
			fixed (char* ptr = this + RuntimeHelpers.OffsetToStringData / 2)
			{
				fixed (char* ptr2 = (ref anyOf != null && anyOf.Length != 0 ? ref anyOf[0] : ref *null))
				{
					char* ptr3 = ptr + startIndex;
					char* ptr4 = ptr3 - count;
					char* ptr5 = ptr2 + anyOf.Length;
					while (ptr3 != ptr4)
					{
						for (char* ptr6 = ptr2; ptr6 != ptr5; ptr6++)
						{
							if (*ptr6 == *ptr3)
							{
								return (int)((long)(ptr3 - ptr));
							}
						}
						ptr3--;
					}
					return -1;
				}
			}
		}

		public int LastIndexOf(char value)
		{
			if (this.length == 0)
			{
				return -1;
			}
			return this.LastIndexOfUnchecked(value, this.length - 1, this.length);
		}

		public int LastIndexOf(char value, int startIndex)
		{
			return this.LastIndexOf(value, startIndex, startIndex + 1);
		}

		public int LastIndexOf(char value, int startIndex, int count)
		{
			if (startIndex == 0 && this.length == 0)
			{
				return -1;
			}
			if (startIndex < 0 || startIndex >= this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0 || >= this.Length");
			}
			if (count < 0 || count > this.Length)
			{
				throw new ArgumentOutOfRangeException("count", "< 0 || > this.Length");
			}
			if (startIndex - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex - count + 1 < 0");
			}
			return this.LastIndexOfUnchecked(value, startIndex, count);
		}

		internal unsafe int LastIndexOfUnchecked(char value, int startIndex, int count)
		{
			char* ptr = (ref this.start_char) + startIndex * 2;
			char* ptr2 = ptr - (count >> 3 << 3);
			while (ptr != ptr2)
			{
				if (*ptr == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2;
				}
				if (ptr[-1] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 - 1;
				}
				if (ptr[-2] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 - 2;
				}
				if (ptr[-3] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 - 3;
				}
				if (ptr[-4] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 - 4;
				}
				if (ptr[-5] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 - 5;
				}
				if (ptr[-6] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 - 6;
				}
				if (ptr[-7] == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2 - 7;
				}
				ptr -= 8;
			}
			ptr2 -= count & 7;
			while (ptr != ptr2)
			{
				if (*ptr == value)
				{
					return (ptr - (ref this.start_char) / 2) / 2;
				}
				ptr--;
			}
			return -1;
		}

		internal unsafe int LastIndexOfOrdinalIgnoreCase(char value, int startIndex, int count)
		{
			if (this.length == 0)
			{
				return -1;
			}
			int num = startIndex - count;
			char c = char.ToUpperInvariant(value);
			fixed (char* ptr = &this.start_char)
			{
				for (int i = startIndex; i > num; i--)
				{
					if (char.ToUpperInvariant(ptr[i]) == c)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int LastIndexOf(string value)
		{
			if (this.length == 0)
			{
				return this.LastIndexOf(value, 0, 0);
			}
			return this.LastIndexOf(value, this.length - 1, this.length);
		}

		public int LastIndexOf(string value, int startIndex)
		{
			int num = startIndex;
			if (num < this.Length)
			{
				num++;
			}
			return this.LastIndexOf(value, startIndex, num);
		}

		public int LastIndexOf(string value, int startIndex, int count)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < -1 || startIndex > this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0 || > this.Length");
			}
			if (count < 0 || count > this.Length)
			{
				throw new ArgumentOutOfRangeException("count", "< 0 || > this.Length");
			}
			if (startIndex - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex - count + 1 < 0");
			}
			if (value.Length == 0)
			{
				return startIndex;
			}
			if (startIndex == 0 && this.length == 0)
			{
				return -1;
			}
			if (this.length == 0 && value.length > 0)
			{
				return -1;
			}
			if (count == 0)
			{
				return -1;
			}
			if (startIndex == this.Length)
			{
				startIndex--;
			}
			return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count);
		}

		public bool Contains(string value)
		{
			return this.IndexOf(value) != -1;
		}

		public static bool IsNullOrEmpty(string value)
		{
			return value == null || value.Length == 0;
		}

		public string Normalize()
		{
			return Normalization.Normalize(this, 0);
		}

		public string Normalize(NormalizationForm normalizationForm)
		{
			switch (normalizationForm)
			{
			case NormalizationForm.FormD:
				return Normalization.Normalize(this, 1);
			default:
				return Normalization.Normalize(this, 0);
			case NormalizationForm.FormKC:
				return Normalization.Normalize(this, 2);
			case NormalizationForm.FormKD:
				return Normalization.Normalize(this, 3);
			}
		}

		public bool IsNormalized()
		{
			return Normalization.IsNormalized(this, 0);
		}

		public bool IsNormalized(NormalizationForm normalizationForm)
		{
			switch (normalizationForm)
			{
			case NormalizationForm.FormD:
				return Normalization.IsNormalized(this, 1);
			default:
				return Normalization.IsNormalized(this, 0);
			case NormalizationForm.FormKC:
				return Normalization.IsNormalized(this, 2);
			case NormalizationForm.FormKD:
				return Normalization.IsNormalized(this, 3);
			}
		}

		public string Remove(int startIndex)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex can not be less than zero");
			}
			if (startIndex >= this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex must be less than the length of the string");
			}
			return this.Remove(startIndex, this.length - startIndex);
		}

		public string PadLeft(int totalWidth)
		{
			return this.PadLeft(totalWidth, ' ');
		}

		public unsafe string PadLeft(int totalWidth, char paddingChar)
		{
			if (totalWidth < 0)
			{
				throw new ArgumentOutOfRangeException("totalWidth", "< 0");
			}
			if (totalWidth < this.length)
			{
				return this;
			}
			string text = string.InternalAllocateStr(totalWidth);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text3 = this)
					{
						fixed (char* ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							char* ptr3 = ptr;
							char* ptr4 = ptr + (totalWidth - this.length);
							while (ptr3 != ptr4)
							{
								*(ptr3++) = paddingChar;
							}
							string.CharCopy(ptr4, ptr2, this.length);
							text2 = null;
							text3 = null;
							return text;
						}
					}
				}
			}
		}

		public string PadRight(int totalWidth)
		{
			return this.PadRight(totalWidth, ' ');
		}

		public unsafe string PadRight(int totalWidth, char paddingChar)
		{
			if (totalWidth < 0)
			{
				throw new ArgumentOutOfRangeException("totalWidth", "< 0");
			}
			if (totalWidth < this.length)
			{
				return this;
			}
			if (totalWidth == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(totalWidth);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text3 = this)
					{
						fixed (char* ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr, ptr2, this.length);
							char* ptr3 = ptr + this.length;
							char* ptr4 = ptr + totalWidth;
							while (ptr3 != ptr4)
							{
								*(ptr3++) = paddingChar;
							}
							text2 = null;
							text3 = null;
							return text;
						}
					}
				}
			}
		}

		public bool StartsWith(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None);
		}

		[ComVisible(false)]
		public bool StartsWith(string value, StringComparison comparisonType)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CultureInfo.InvariantCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.IsPrefix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.OrdinalIgnoreCase);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", new object[] { comparisonType });
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		[ComVisible(false)]
		public bool EndsWith(string value, StringComparison comparisonType)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CultureInfo.InvariantCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.IsSuffix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.OrdinalIgnoreCase);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", new object[] { comparisonType });
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		public bool StartsWith(string value, bool ignoreCase, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			return culture.CompareInfo.IsPrefix(this, value, (!ignoreCase) ? CompareOptions.None : CompareOptions.IgnoreCase);
		}

		public unsafe string Replace(char oldChar, char newChar)
		{
			if (this.length == 0 || oldChar == newChar)
			{
				return this;
			}
			int num = this.IndexOfUnchecked(oldChar, 0, this.length);
			if (num == -1)
			{
				return this;
			}
			if (num < 4)
			{
				num = 0;
			}
			string text = string.InternalAllocateStr(this.length);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (char* ptr2 = &this.start_char)
					{
						if (num != 0)
						{
							string.CharCopy(ptr, ptr2, num);
						}
						char* ptr3 = ptr + this.length;
						char* ptr4 = ptr + num;
						char* ptr5 = ptr2 + num;
						while (ptr4 != ptr3)
						{
							if (*ptr5 == oldChar)
							{
								*ptr4 = newChar;
							}
							else
							{
								*ptr4 = *ptr5;
							}
							ptr5++;
							ptr4++;
						}
						text2 = null;
					}
					return text;
				}
			}
		}

		public string Replace(string oldValue, string newValue)
		{
			if (oldValue == null)
			{
				throw new ArgumentNullException("oldValue");
			}
			if (oldValue.Length == 0)
			{
				throw new ArgumentException("oldValue is the empty string.");
			}
			if (this.Length == 0)
			{
				return this;
			}
			if (newValue == null)
			{
				newValue = string.Empty;
			}
			return this.ReplaceUnchecked(oldValue, newValue);
		}

		private unsafe string ReplaceUnchecked(string oldValue, string newValue)
		{
			if (oldValue.length > this.length)
			{
				return this;
			}
			if (oldValue.length == 1 && newValue.length == 1)
			{
				return this.Replace(oldValue[0], newValue[0]);
			}
			int* ptr = stackalloc int[checked(200 * 4)];
			fixed (char* ptr2 = this + RuntimeHelpers.OffsetToStringData / 2)
			{
				fixed (char* ptr3 = newValue + RuntimeHelpers.OffsetToStringData / 2)
				{
					int i = 0;
					int num = 0;
					while (i < this.length)
					{
						int num2 = this.IndexOfOrdinalUnchecked(oldValue, i, this.length - i);
						if (num2 < 0)
						{
							break;
						}
						if (num >= 200)
						{
							return this.ReplaceFallback(oldValue, newValue, 200);
						}
						ptr[num++ * 4] = num2;
						i = num2 + oldValue.length;
					}
					if (num == 0)
					{
						return this;
					}
					int num3 = this.length + (newValue.length - oldValue.length) * num;
					string text = string.InternalAllocateStr(num3);
					int num4 = 0;
					int num5 = 0;
					fixed (string text2 = text)
					{
						fixed (char* ptr4 = text2 + RuntimeHelpers.OffsetToStringData / 2)
						{
							for (int j = 0; j < num; j++)
							{
								int num6 = ptr[j] - num5;
								string.CharCopy(ptr4 + num4, ptr2 + num5, num6);
								num4 += num6;
								num5 = ptr[j] + oldValue.length;
								string.CharCopy(ptr4 + num4, ptr3, newValue.length);
								num4 += newValue.length;
							}
							string.CharCopy(ptr4 + num4, ptr2 + num5, this.length - num5);
							text2 = null;
							return text;
						}
					}
				}
			}
		}

		private string ReplaceFallback(string oldValue, string newValue, int testedCount)
		{
			int num = this.length + (newValue.length - oldValue.length) * testedCount;
			StringBuilder stringBuilder = new StringBuilder(num);
			int num2;
			for (int i = 0; i < this.length; i = num2 + oldValue.Length)
			{
				num2 = this.IndexOfOrdinalUnchecked(oldValue, i, this.length - i);
				if (num2 < 0)
				{
					stringBuilder.Append(this.SubstringUnchecked(i, this.length - i));
					break;
				}
				stringBuilder.Append(this.SubstringUnchecked(i, num2 - i));
				stringBuilder.Append(newValue);
			}
			return stringBuilder.ToString();
		}

		public unsafe string Remove(int startIndex, int count)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative.");
			}
			if (startIndex > this.length - count)
			{
				throw new ArgumentOutOfRangeException("count", "startIndex + count > this.length");
			}
			string text = string.InternalAllocateStr(this.length - count);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text3 = this)
					{
						fixed (char* ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							char* ptr3 = ptr;
							string.CharCopy(ptr3, ptr2, startIndex);
							int num = startIndex + count;
							ptr3 += startIndex;
							string.CharCopy(ptr3, ptr2 + num, this.length - num);
							text2 = null;
							text3 = null;
							return text;
						}
					}
				}
			}
		}

		public string ToLower()
		{
			return this.ToLower(CultureInfo.CurrentCulture);
		}

		public string ToLower(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (culture.LCID == 127)
			{
				return this.ToLowerInvariant();
			}
			return culture.TextInfo.ToLower(this);
		}

		public unsafe string ToLowerInvariant()
		{
			if (this.length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(this.length);
			fixed (char* ptr = &this.start_char)
			{
				fixed (string text2 = text)
				{
					fixed (char* ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						char* ptr3 = ptr2;
						char* ptr4 = ptr;
						for (int i = 0; i < this.length; i++)
						{
							*ptr3 = char.ToLowerInvariant(*ptr4);
							ptr4++;
							ptr3++;
						}
						ptr = null;
						text2 = null;
						return text;
					}
				}
			}
		}

		public string ToUpper()
		{
			return this.ToUpper(CultureInfo.CurrentCulture);
		}

		public string ToUpper(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (culture.LCID == 127)
			{
				return this.ToUpperInvariant();
			}
			return culture.TextInfo.ToUpper(this);
		}

		public unsafe string ToUpperInvariant()
		{
			if (this.length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(this.length);
			fixed (char* ptr = &this.start_char)
			{
				fixed (string text2 = text)
				{
					fixed (char* ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						char* ptr3 = ptr2;
						char* ptr4 = ptr;
						for (int i = 0; i < this.length; i++)
						{
							*ptr3 = char.ToUpperInvariant(*ptr4);
							ptr4++;
							ptr3++;
						}
						ptr = null;
						text2 = null;
						return text;
					}
				}
			}
		}

		public override string ToString()
		{
			return this;
		}

		public string ToString(IFormatProvider provider)
		{
			return this;
		}

		public static string Format(string format, object arg0)
		{
			return string.Format(null, format, new object[] { arg0 });
		}

		public static string Format(string format, object arg0, object arg1)
		{
			return string.Format(null, format, new object[] { arg0, arg1 });
		}

		public static string Format(string format, object arg0, object arg1, object arg2)
		{
			return string.Format(null, format, new object[] { arg0, arg1, arg2 });
		}

		public static string Format(string format, params object[] args)
		{
			return string.Format(null, format, args);
		}

		public static string Format(IFormatProvider provider, string format, params object[] args)
		{
			StringBuilder stringBuilder = string.FormatHelper(null, provider, format, args);
			return stringBuilder.ToString();
		}

		internal static StringBuilder FormatHelper(StringBuilder result, IFormatProvider provider, string format, params object[] args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (result == null)
			{
				int num = 0;
				int i;
				for (i = 0; i < args.Length; i++)
				{
					string text = args[i] as string;
					if (text == null)
					{
						break;
					}
					num += text.length;
				}
				if (i == args.Length)
				{
					result = new StringBuilder(num + format.length);
				}
				else
				{
					result = new StringBuilder();
				}
			}
			int j = 0;
			int num2 = j;
			while (j < format.length)
			{
				char c = format[j++];
				if (c == '{')
				{
					result.Append(format, num2, j - num2 - 1);
					if (format[j] == '{')
					{
						num2 = j++;
					}
					else
					{
						int num3;
						int num4;
						bool flag;
						string text2;
						string.ParseFormatSpecifier(format, ref j, out num3, out num4, out flag, out text2);
						if (num3 >= args.Length)
						{
							throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
						}
						object obj = args[num3];
						ICustomFormatter customFormatter = null;
						if (provider != null)
						{
							customFormatter = provider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
						}
						string text3;
						if (obj == null)
						{
							text3 = string.Empty;
						}
						else if (customFormatter != null)
						{
							text3 = customFormatter.Format(text2, obj, provider);
						}
						else if (obj is IFormattable)
						{
							text3 = ((IFormattable)obj).ToString(text2, provider);
						}
						else
						{
							text3 = obj.ToString();
						}
						if (num4 > text3.length)
						{
							int num5 = num4 - text3.length;
							if (flag)
							{
								result.Append(text3);
								result.Append(' ', num5);
							}
							else
							{
								result.Append(' ', num5);
								result.Append(text3);
							}
						}
						else
						{
							result.Append(text3);
						}
						num2 = j;
					}
				}
				else if (c == '}' && j < format.length && format[j] == '}')
				{
					result.Append(format, num2, j - num2 - 1);
					num2 = j++;
				}
				else if (c == '}')
				{
					throw new FormatException("Input string was not in a correct format.");
				}
			}
			if (num2 < format.length)
			{
				result.Append(format, num2, format.Length - num2);
			}
			return result;
		}

		public unsafe static string Copy(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			int num = str.length;
			string text = string.InternalAllocateStr(num);
			if (num != 0)
			{
				fixed (string text2 = text)
				{
					fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text3 = str, ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr, ptr2, num);
							text2 = null;
						}
					}
				}
			}
			return text;
		}

		public static string Concat(object arg0)
		{
			if (arg0 == null)
			{
				return string.Empty;
			}
			return arg0.ToString();
		}

		public static string Concat(object arg0, object arg1)
		{
			return ((arg0 == null) ? null : arg0.ToString()) + ((arg1 == null) ? null : arg1.ToString());
		}

		public static string Concat(object arg0, object arg1, object arg2)
		{
			string text;
			if (arg0 == null)
			{
				text = string.Empty;
			}
			else
			{
				text = arg0.ToString();
			}
			string text2;
			if (arg1 == null)
			{
				text2 = string.Empty;
			}
			else
			{
				text2 = arg1.ToString();
			}
			string text3;
			if (arg2 == null)
			{
				text3 = string.Empty;
			}
			else
			{
				text3 = arg2.ToString();
			}
			return text + text2 + text3;
		}

		[CLSCompliant(false)]
		public static string Concat(object arg0, object arg1, object arg2, object arg3, __arglist)
		{
			string text;
			if (arg0 == null)
			{
				text = string.Empty;
			}
			else
			{
				text = arg0.ToString();
			}
			string text2;
			if (arg1 == null)
			{
				text2 = string.Empty;
			}
			else
			{
				text2 = arg1.ToString();
			}
			string text3;
			if (arg2 == null)
			{
				text3 = string.Empty;
			}
			else
			{
				text3 = arg2.ToString();
			}
			ArgIterator argIterator = new ArgIterator(__arglist);
			int remainingCount = argIterator.GetRemainingCount();
			StringBuilder stringBuilder = new StringBuilder();
			if (arg3 != null)
			{
				stringBuilder.Append(arg3.ToString());
			}
			for (int i = 0; i < remainingCount; i++)
			{
				TypedReference nextArg = argIterator.GetNextArg();
				stringBuilder.Append(TypedReference.ToObject(nextArg));
			}
			string text4 = stringBuilder.ToString();
			return text + text2 + text3 + text4;
		}

		public unsafe static string Concat(string str0, string str1)
		{
			if (str0 == null || str0.Length == 0)
			{
				if (str1 == null || str1.Length == 0)
				{
					return string.Empty;
				}
				return str1;
			}
			else
			{
				if (str1 == null || str1.Length == 0)
				{
					return str0;
				}
				string text = string.InternalAllocateStr(str0.length + str1.length);
				fixed (string text2 = text)
				{
					fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text3 = str0)
						{
							fixed (char* ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
							{
								string.CharCopy(ptr, ptr2, str0.length);
								text2 = null;
								text3 = null;
								fixed (string text4 = text)
								{
									fixed (char* ptr3 = text4 + RuntimeHelpers.OffsetToStringData / 2)
									{
										fixed (string text5 = str1)
										{
											fixed (char* ptr4 = text5 + RuntimeHelpers.OffsetToStringData / 2)
											{
												string.CharCopy(ptr3 + str0.Length, ptr4, str1.length);
												text4 = null;
												text5 = null;
												return text;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public unsafe static string Concat(string str0, string str1, string str2)
		{
			if (str0 == null || str0.Length == 0)
			{
				if (str1 == null || str1.Length == 0)
				{
					if (str2 == null || str2.Length == 0)
					{
						return string.Empty;
					}
					return str2;
				}
				else
				{
					if (str2 == null || str2.Length == 0)
					{
						return str1;
					}
					str0 = string.Empty;
				}
			}
			else if (str1 == null || str1.Length == 0)
			{
				if (str2 == null || str2.Length == 0)
				{
					return str0;
				}
				str1 = string.Empty;
			}
			else if (str2 == null || str2.Length == 0)
			{
				str2 = string.Empty;
			}
			string text = string.InternalAllocateStr(str0.length + str1.length + str2.length);
			if (str0.Length != 0)
			{
				fixed (string text2 = text)
				{
					fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text3 = str0, ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr, ptr2, str0.length);
							text2 = null;
						}
					}
				}
			}
			if (str1.Length != 0)
			{
				fixed (string text4 = text)
				{
					fixed (char* ptr3 = text4 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text5 = str1, ptr4 = text5 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr3 + str0.Length, ptr4, str1.length);
							text4 = null;
						}
					}
				}
			}
			if (str2.Length != 0)
			{
				fixed (string text6 = text)
				{
					fixed (char* ptr5 = text6 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text7 = str2, ptr6 = text7 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr5 + str0.Length + str1.Length, ptr6, str2.length);
							text6 = null;
						}
					}
				}
			}
			return text;
		}

		public unsafe static string Concat(string str0, string str1, string str2, string str3)
		{
			if (str0 == null && str1 == null && str2 == null && str3 == null)
			{
				return string.Empty;
			}
			if (str0 == null)
			{
				str0 = string.Empty;
			}
			if (str1 == null)
			{
				str1 = string.Empty;
			}
			if (str2 == null)
			{
				str2 = string.Empty;
			}
			if (str3 == null)
			{
				str3 = string.Empty;
			}
			string text = string.InternalAllocateStr(str0.length + str1.length + str2.length + str3.length);
			if (str0.Length != 0)
			{
				fixed (string text2 = text)
				{
					fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text3 = str0, ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr, ptr2, str0.length);
							text2 = null;
						}
					}
				}
			}
			if (str1.Length != 0)
			{
				fixed (string text4 = text)
				{
					fixed (char* ptr3 = text4 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text5 = str1, ptr4 = text5 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr3 + str0.Length, ptr4, str1.length);
							text4 = null;
						}
					}
				}
			}
			if (str2.Length != 0)
			{
				fixed (string text6 = text)
				{
					fixed (char* ptr5 = text6 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text7 = str2, ptr6 = text7 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr5 + str0.Length + str1.Length, ptr6, str2.length);
							text6 = null;
						}
					}
				}
			}
			if (str3.Length != 0)
			{
				fixed (string text8 = text)
				{
					fixed (char* ptr7 = text8 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (string text9 = str3, ptr8 = text9 + RuntimeHelpers.OffsetToStringData / 2)
						{
							string.CharCopy(ptr7 + str0.Length + str1.Length + str2.Length, ptr8, str3.length);
							text8 = null;
						}
					}
				}
			}
			return text;
		}

		public static string Concat(params object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			int num = args.Length;
			if (num == 0)
			{
				return string.Empty;
			}
			string[] array = new string[num];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (args[i] != null)
				{
					array[i] = args[i].ToString();
					num2 += array[i].length;
				}
			}
			return string.ConcatInternal(array, num2);
		}

		public static string Concat(params string[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = 0;
			foreach (string text in values)
			{
				if (text != null)
				{
					num += text.length;
				}
			}
			return string.ConcatInternal(values, num);
		}

		private unsafe static string ConcatInternal(string[] values, int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(length);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					int num = 0;
					foreach (string text3 in values)
					{
						if (text3 != null)
						{
							fixed (string text4 = text3)
							{
								fixed (char* ptr2 = text4 + RuntimeHelpers.OffsetToStringData / 2)
								{
									string.CharCopy(ptr + num, ptr2, text3.length);
									text4 = null;
									num += text3.Length;
								}
							}
						}
					}
					text2 = null;
					return text;
				}
			}
		}

		public unsafe string Insert(int startIndex, string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative and must be less than or equal to length of string.");
			}
			if (value.Length == 0)
			{
				return this;
			}
			if (this.Length == 0)
			{
				return value;
			}
			string text = string.InternalAllocateStr(this.length + value.length);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text3 = this)
					{
						fixed (char* ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							fixed (string text4 = value)
							{
								fixed (char* ptr3 = text4 + RuntimeHelpers.OffsetToStringData / 2)
								{
									char* ptr4 = ptr;
									string.CharCopy(ptr4, ptr2, startIndex);
									ptr4 += startIndex;
									string.CharCopy(ptr4, ptr3, value.length);
									ptr4 += value.length;
									string.CharCopy(ptr4, ptr2 + startIndex, this.length - startIndex);
									text2 = null;
									text3 = null;
									text4 = null;
									return text;
								}
							}
						}
					}
				}
			}
		}

		public static string Intern(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return string.InternalIntern(str);
		}

		public static string IsInterned(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return string.InternalIsInterned(str);
		}

		public static string Join(string separator, string[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (separator == null)
			{
				separator = string.Empty;
			}
			return string.JoinUnchecked(separator, value, 0, value.Length);
		}

		public static string Join(string separator, string[] value, int startIndex, int count)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (startIndex > value.Length - count)
			{
				throw new ArgumentOutOfRangeException("startIndex", "startIndex + count > value.length");
			}
			if (startIndex == value.Length)
			{
				return string.Empty;
			}
			if (separator == null)
			{
				separator = string.Empty;
			}
			return string.JoinUnchecked(separator, value, startIndex, count);
		}

		private unsafe static string JoinUnchecked(string separator, string[] value, int startIndex, int count)
		{
			int num = 0;
			int num2 = startIndex + count;
			for (int i = startIndex; i < num2; i++)
			{
				string text = value[i];
				if (text != null)
				{
					num += text.length;
				}
			}
			num += separator.length * (count - 1);
			if (num <= 0)
			{
				return string.Empty;
			}
			string text2 = string.InternalAllocateStr(num);
			num2--;
			fixed (string text3 = text2)
			{
				fixed (char* ptr = text3 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text4 = separator)
					{
						fixed (char* ptr2 = text4 + RuntimeHelpers.OffsetToStringData / 2)
						{
							int num3 = 0;
							for (int j = startIndex; j < num2; j++)
							{
								string text5 = value[j];
								if (text5 != null && text5.Length > 0)
								{
									fixed (string text6 = text5)
									{
										fixed (char* ptr3 = text6 + RuntimeHelpers.OffsetToStringData / 2)
										{
											string.CharCopy(ptr + num3, ptr3, text5.Length);
											text6 = null;
											num3 += text5.Length;
										}
									}
								}
								if (separator.Length > 0)
								{
									string.CharCopy(ptr + num3, ptr2, separator.Length);
									num3 += separator.Length;
								}
							}
							string text7 = value[num2];
							if (text7 != null && text7.Length > 0)
							{
								fixed (string text8 = text7, ptr4 = text8 + RuntimeHelpers.OffsetToStringData / 2)
								{
									string.CharCopy(ptr + num3, ptr4, text7.Length);
								}
							}
							text3 = null;
							text4 = null;
							return text2;
						}
					}
				}
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public CharEnumerator GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		private static void ParseFormatSpecifier(string str, ref int ptr, out int n, out int width, out bool left_align, out string format)
		{
			try
			{
				n = string.ParseDecimal(str, ref ptr);
				if (n < 0)
				{
					throw new FormatException("Input string was not in a correct format.");
				}
				if (str[ptr] == ',')
				{
					ptr++;
					while (char.IsWhiteSpace(str[ptr]))
					{
						ptr++;
					}
					int num = ptr;
					format = str.Substring(num, ptr - num);
					left_align = str[ptr] == '-';
					if (left_align)
					{
						ptr++;
					}
					width = string.ParseDecimal(str, ref ptr);
					if (width < 0)
					{
						throw new FormatException("Input string was not in a correct format.");
					}
				}
				else
				{
					width = 0;
					left_align = false;
					format = string.Empty;
				}
				if (str[ptr] == ':')
				{
					int num2 = ++ptr;
					while (str[ptr] != '}')
					{
						ptr++;
					}
					format += str.Substring(num2, ptr - num2);
				}
				else
				{
					format = null;
				}
				if (str[ptr++] != '}')
				{
					throw new FormatException("Input string was not in a correct format.");
				}
			}
			catch (IndexOutOfRangeException)
			{
				throw new FormatException("Input string was not in a correct format.");
			}
		}

		private static int ParseDecimal(string str, ref int ptr)
		{
			int num = ptr;
			int num2 = 0;
			for (;;)
			{
				char c = str[num];
				if (c < '0' || '9' < c)
				{
					break;
				}
				num2 = num2 * 10 + (int)c - 48;
				num++;
			}
			if (num == ptr)
			{
				return -1;
			}
			ptr = num;
			return num2;
		}

		internal unsafe void InternalSetChar(int idx, char val)
		{
			if (idx >= this.Length)
			{
				throw new ArgumentOutOfRangeException("idx");
			}
			fixed (char* ptr = &this.start_char)
			{
				ptr[idx] = val;
			}
		}

		internal unsafe void InternalSetLength(int newLength)
		{
			if (newLength > this.length)
			{
				throw new ArgumentOutOfRangeException("newLength", "newLength as to be <= length");
			}
			fixed (char* ptr = &this.start_char)
			{
				char* ptr2 = ptr + newLength;
				char* ptr3 = ptr + this.length;
				while (ptr2 < ptr3)
				{
					*ptr2 = '\0';
					ptr2++;
				}
			}
			this.length = newLength;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public unsafe override int GetHashCode()
		{
			fixed (char* ptr = this + RuntimeHelpers.OffsetToStringData / 2)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + this.length - 1;
				int num = 0;
				while (ptr2 < ptr3)
				{
					num = (num << 5) - num + (int)(*ptr2);
					num = (num << 5) - num + (int)ptr2[1];
					ptr2 += 2;
				}
				ptr3++;
				if (ptr2 < ptr3)
				{
					num = (num << 5) - num + (int)(*ptr2);
				}
				return num;
			}
		}

		internal unsafe int GetCaseInsensitiveHashCode()
		{
			fixed (char* ptr = this + RuntimeHelpers.OffsetToStringData / 2)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + this.length - 1;
				int num = 0;
				while (ptr2 < ptr3)
				{
					num = (num << 5) - num + (int)char.ToUpperInvariant(*ptr2);
					num = (num << 5) - num + (int)char.ToUpperInvariant(ptr2[1]);
					ptr2 += 2;
				}
				ptr3++;
				if (ptr2 < ptr3)
				{
					num = (num << 5) - num + (int)char.ToUpperInvariant(*ptr2);
				}
				return num;
			}
		}

		private unsafe string CreateString(sbyte* value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			byte* ptr = (byte*)value;
			int num = 0;
			try
			{
				while (*(ptr++) != 0)
				{
					num++;
				}
			}
			catch (NullReferenceException)
			{
				throw new ArgumentOutOfRangeException("ptr", "Value does not refer to a valid string.");
			}
			catch (AccessViolationException)
			{
				throw new ArgumentOutOfRangeException("ptr", "Value does not refer to a valid string.");
			}
			return this.CreateString(value, 0, num, null);
		}

		private unsafe string CreateString(sbyte* value, int startIndex, int length)
		{
			return this.CreateString(value, startIndex, length, null);
		}

		private unsafe string CreateString(sbyte* value, int startIndex, int length, Encoding enc)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Non-negative number required.");
			}
			if (value + startIndex < value)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Value, startIndex and length do not refer to a valid string.");
			}
			bool flag;
			if (flag = enc == null)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (length == 0)
				{
					return string.Empty;
				}
				enc = Encoding.Default;
			}
			byte[] array = new byte[length];
			if (length != 0)
			{
				fixed (byte* ptr = (ref array != null && array.Length != 0 ? ref array[0] : ref *null))
				{
					try
					{
						string.memcpy(ptr, (byte*)(value + startIndex), length);
					}
					catch (NullReferenceException)
					{
						throw new ArgumentOutOfRangeException("ptr", "Value, startIndex and length do not refer to a valid string.");
					}
					catch (AccessViolationException)
					{
						if (!flag)
						{
							throw;
						}
						throw new ArgumentOutOfRangeException("value", "Value, startIndex and length do not refer to a valid string.");
					}
				}
			}
			return enc.GetString(array);
		}

		private unsafe string CreateString(char* value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			char* ptr = value;
			int num = 0;
			while (*ptr != '\0')
			{
				num++;
				ptr++;
			}
			string text = string.InternalAllocateStr(num);
			if (num != 0)
			{
				fixed (string text2 = text, ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					string.CharCopy(ptr2, value, num);
				}
			}
			return text;
		}

		private unsafe string CreateString(char* value, int startIndex, int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			string text = string.InternalAllocateStr(length);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					string.CharCopy(ptr, value + startIndex, length);
					text2 = null;
					return text;
				}
			}
		}

		private unsafe string CreateString(char[] val, int startIndex, int length)
		{
			if (val == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Cannot be negative.");
			}
			if (startIndex > val.Length - length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative, and should be less than length of string.");
			}
			if (length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(length);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (char* ptr2 = (ref val != null && val.Length != 0 ? ref val[0] : ref *null))
					{
						string.CharCopy(ptr, ptr2 + startIndex, length);
						text2 = null;
					}
					return text;
				}
			}
		}

		private unsafe string CreateString(char[] val)
		{
			if (val == null)
			{
				return string.Empty;
			}
			if (val.Length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(val.Length);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (char* ptr2 = (ref val != null && val.Length != 0 ? ref val[0] : ref *null))
					{
						string.CharCopy(ptr, ptr2, val.Length);
						text2 = null;
					}
					return text;
				}
			}
		}

		private unsafe string CreateString(char c, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(count);
			fixed (string text2 = text)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					char* ptr2 = ptr;
					char* ptr3 = ptr2 + count;
					while (ptr2 < ptr3)
					{
						*ptr2 = c;
						ptr2++;
					}
					text2 = null;
					return text;
				}
			}
		}

		internal unsafe static void memset(byte* dest, int val, int len)
		{
			if (len < 8)
			{
				while (len != 0)
				{
					*dest = (byte)val;
					dest++;
					len--;
				}
				return;
			}
			if (val != 0)
			{
				val |= val << 8;
				val |= val << 16;
			}
			int num = dest & 3;
			if (num != 0)
			{
				num = 4 - num;
				len -= num;
				do
				{
					*dest = (byte)val;
					dest++;
					num--;
				}
				while (num != 0);
			}
			while (len >= 16)
			{
				*(int*)dest = val;
				*(int*)(dest + 4) = val;
				*(int*)(dest + 8) = val;
				*(int*)(dest + 12) = val;
				dest += 16;
				len -= 16;
			}
			while (len >= 4)
			{
				*(int*)dest = val;
				dest += 4;
				len -= 4;
			}
			while (len > 0)
			{
				*dest = (byte)val;
				dest++;
				len--;
			}
		}

		private unsafe static void memcpy4(byte* dest, byte* src, int size)
		{
			while (size >= 16)
			{
				*(int*)dest = *(int*)src;
				*(int*)(dest + 4) = *(int*)(src + 4);
				*(int*)(dest + 8) = *(int*)(src + 8);
				*(int*)(dest + 12) = *(int*)(src + 12);
				dest += 16;
				src += 16;
				size -= 16;
			}
			while (size >= 4)
			{
				*(int*)dest = *(int*)src;
				dest += 4;
				src += 4;
				size -= 4;
			}
			while (size > 0)
			{
				*dest = *src;
				dest++;
				src++;
				size--;
			}
		}

		private unsafe static void memcpy2(byte* dest, byte* src, int size)
		{
			while (size >= 8)
			{
				*(short*)dest = *(short*)src;
				*(short*)(dest + 2) = *(short*)(src + 2);
				*(short*)(dest + 4) = *(short*)(src + 4);
				*(short*)(dest + 6) = *(short*)(src + 6);
				dest += 8;
				src += 8;
				size -= 8;
			}
			while (size >= 2)
			{
				*(short*)dest = *(short*)src;
				dest += 2;
				src += 2;
				size -= 2;
			}
			if (size > 0)
			{
				*dest = *src;
			}
		}

		private unsafe static void memcpy1(byte* dest, byte* src, int size)
		{
			while (size >= 8)
			{
				*dest = *src;
				dest[1] = src[1];
				dest[2] = src[2];
				dest[3] = src[3];
				dest[4] = src[4];
				dest[5] = src[5];
				dest[6] = src[6];
				dest[7] = src[7];
				dest += 8;
				src += 8;
				size -= 8;
			}
			while (size >= 2)
			{
				*dest = *src;
				dest[1] = src[1];
				dest += 2;
				src += 2;
				size -= 2;
			}
			if (size > 0)
			{
				*dest = *src;
			}
		}

		internal unsafe static void memcpy(byte* dest, byte* src, int size)
		{
			if (((dest | src) & 3) != 0)
			{
				if ((dest & 1) != 0 && (src & 1) != 0 && size >= 1)
				{
					*dest = *src;
					dest++;
					src++;
					size--;
				}
				if ((dest & 2) != 0 && (src & 2) != 0 && size >= 2)
				{
					*(short*)dest = *(short*)src;
					dest += 2;
					src += 2;
					size -= 2;
				}
				if (((dest | src) & 1) != 0)
				{
					string.memcpy1(dest, src, size);
					return;
				}
				if (((dest | src) & 2) != 0)
				{
					string.memcpy2(dest, src, size);
					return;
				}
			}
			string.memcpy4(dest, src, size);
		}

		internal unsafe static void CharCopy(char* dest, char* src, int count)
		{
			if (((dest | src) & 3) != 0)
			{
				if ((dest & 2) != 0 && (src & 2) != 0 && count > 0)
				{
					*dest = (char)(*(short*)src);
					dest++;
					src++;
					count--;
				}
				if (((dest | src) & 2) != 0)
				{
					string.memcpy2((byte*)dest, (byte*)src, count * 2);
					return;
				}
			}
			string.memcpy4((byte*)dest, (byte*)src, count * 2);
		}

		internal unsafe static void CharCopyReverse(char* dest, char* src, int count)
		{
			dest += count;
			src += count;
			for (int i = count; i > 0; i--)
			{
				dest--;
				src--;
				*dest = *src;
			}
		}

		internal unsafe static void CharCopy(string target, int targetIndex, string source, int sourceIndex, int count)
		{
			fixed (string text = target)
			{
				fixed (char* ptr = text + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text2 = source, ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						string.CharCopy(ptr + targetIndex, ptr2 + sourceIndex, count);
						text = null;
					}
				}
			}
		}

		internal unsafe static void CharCopy(string target, int targetIndex, char[] source, int sourceIndex, int count)
		{
			fixed (string text = target)
			{
				fixed (char* ptr = text + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (char* ptr2 = (ref source != null && source.Length != 0 ? ref source[0] : ref *null))
					{
						string.CharCopy(ptr + targetIndex, ptr2 + sourceIndex, count);
						text = null;
					}
				}
			}
		}

		internal unsafe static void CharCopyReverse(string target, int targetIndex, string source, int sourceIndex, int count)
		{
			fixed (string text = target)
			{
				fixed (char* ptr = text + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text2 = source, ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						string.CharCopyReverse(ptr + targetIndex, ptr2 + sourceIndex, count);
						text = null;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string[] InternalSplit(char[] separator, int count, int options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalAllocateStr(int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string InternalIntern(string str);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string InternalIsInterned(string str);

		public static bool operator ==(string a, string b)
		{
			return string.Equals(a, b);
		}

		public static bool operator !=(string a, string b)
		{
			return !string.Equals(a, b);
		}

		[NonSerialized]
		private int length;

		[NonSerialized]
		private char start_char;

		public static readonly string Empty = "";

		private static readonly char[] WhiteChars = new char[]
		{
			'\t', '\n', '\v', '\f', '\r', '\u0085', '\u1680', '\u2028', '\u2029', ' ',
			'\u00a0', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008',
			'\u2009', '\u200a', '\u200b', '\u3000', '\ufeff'
		};
	}
}
