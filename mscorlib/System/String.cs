using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace System
{
	[Serializable]
	public sealed class String : IComparable, IEnumerable, IEnumerable<char>, IComparable<string>, IEquatable<string>, IConvertible, ICloneable
	{
		private unsafe static int CompareOrdinalIgnoreCaseHelper(string strA, string strB)
		{
			int num = Math.Min(strA.Length, strB.Length);
			fixed (char* ptr = &strA._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &strB._firstChar)
				{
					char* ptr4 = ptr3;
					char* ptr5 = ptr2;
					char* ptr6 = ptr4;
					while (num != 0)
					{
						int num2 = (int)(*ptr5);
						int num3 = (int)(*ptr6);
						if (num2 - 97 <= 25)
						{
							num2 -= 32;
						}
						if (num3 - 97 <= 25)
						{
							num3 -= 32;
						}
						if (num2 != num3)
						{
							return num2 - num3;
						}
						ptr5++;
						ptr6++;
						num--;
					}
					return strA.Length - strB.Length;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool EqualsHelper(string strA, string strB)
		{
			return SpanHelpers.SequenceEqual(Unsafe.As<char, byte>(strA.GetRawStringData()), Unsafe.As<char, byte>(strB.GetRawStringData()), (ulong)((long)strA.Length * 2L));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CompareOrdinalHelper(string strA, int indexA, int countA, string strB, int indexB, int countB)
		{
			return SpanHelpers.SequenceCompareTo(Unsafe.Add<char>(strA.GetRawStringData(), indexA), countA, Unsafe.Add<char>(strB.GetRawStringData(), indexB), countB);
		}

		private unsafe static bool EqualsIgnoreCaseAsciiHelper(string strA, string strB)
		{
			int num = strA.Length;
			fixed (char* ptr = &strA._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &strB._firstChar)
				{
					char* ptr4 = ptr3;
					char* ptr5 = ptr2;
					char* ptr6 = ptr4;
					while (num != 0)
					{
						int num2 = (int)(*ptr5);
						int num3 = (int)(*ptr6);
						if (num2 != num3 && ((num2 | 32) != (num3 | 32) || (num2 | 32) - 97 > 25))
						{
							return false;
						}
						ptr5++;
						ptr6++;
						num--;
					}
					return true;
				}
			}
		}

		private unsafe static int CompareOrdinalHelper(string strA, string strB)
		{
			int i = Math.Min(strA.Length, strB.Length);
			fixed (char* ptr = &strA._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &strB._firstChar)
				{
					char* ptr4 = ptr3;
					char* ptr5 = ptr2;
					char* ptr6 = ptr4;
					if (ptr5[1] == ptr6[1])
					{
						i -= 2;
						ptr5 += 2;
						ptr6 += 2;
						while (i >= 12)
						{
							if (*(long*)ptr5 == *(long*)ptr6)
							{
								if (*(long*)(ptr5 + 4) == *(long*)(ptr6 + 4))
								{
									if (*(long*)(ptr5 + 8) == *(long*)(ptr6 + 8))
									{
										i -= 12;
										ptr5 += 12;
										ptr6 += 12;
										continue;
									}
									ptr5 += 4;
									ptr6 += 4;
								}
								ptr5 += 4;
								ptr6 += 4;
							}
							if (*(int*)ptr5 == *(int*)ptr6)
							{
								ptr5 += 2;
								ptr6 += 2;
							}
							IL_010E:
							if (*ptr5 != *ptr6)
							{
								return (int)(*ptr5 - *ptr6);
							}
							goto IL_011E;
						}
						while (i > 0)
						{
							if (*(int*)ptr5 != *(int*)ptr6)
							{
								goto IL_010E;
							}
							i -= 2;
							ptr5 += 2;
							ptr6 += 2;
						}
						return strA.Length - strB.Length;
					}
					IL_011E:
					return (int)(ptr5[1] - ptr6[1]);
				}
			}
		}

		public static int Compare(string strA, string strB)
		{
			return string.Compare(strA, strB, StringComparison.CurrentCulture);
		}

		public static int Compare(string strA, string strB, bool ignoreCase)
		{
			StringComparison stringComparison = (ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
			return string.Compare(strA, strB, stringComparison);
		}

		public static int Compare(string strA, string strB, StringComparison comparisonType)
		{
			if (strA == strB)
			{
				string.CheckStringComparison(comparisonType);
				return 0;
			}
			if (strA == null)
			{
				string.CheckStringComparison(comparisonType);
				return -1;
			}
			if (strB == null)
			{
				string.CheckStringComparison(comparisonType);
				return 1;
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CompareInfo.Invariant.Compare(strA, strB, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CompareInfo.Invariant.Compare(strA, strB, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				if (strA._firstChar != strB._firstChar)
				{
					return (int)(strA._firstChar - strB._firstChar);
				}
				return string.CompareOrdinalHelper(strA, strB);
			case StringComparison.OrdinalIgnoreCase:
				return CompareInfo.CompareOrdinalIgnoreCase(strA, 0, strA.Length, strB, 0, strB.Length);
			default:
				throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
			}
		}

		public static int Compare(string strA, string strB, CultureInfo culture, CompareOptions options)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return culture.CompareInfo.Compare(strA, strB, options);
		}

		public static int Compare(string strA, string strB, bool ignoreCase, CultureInfo culture)
		{
			CompareOptions compareOptions = (ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
			return string.Compare(strA, strB, culture, compareOptions);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length)
		{
			return string.Compare(strA, indexA, strB, indexB, length, false);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase)
		{
			int num = length;
			int num2 = length;
			if (strA != null)
			{
				num = Math.Min(num, strA.Length - indexA);
			}
			if (strB != null)
			{
				num2 = Math.Min(num2, strB.Length - indexB);
			}
			CompareOptions compareOptions = (ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
			return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, num, strB, indexB, num2, compareOptions);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase, CultureInfo culture)
		{
			CompareOptions compareOptions = (ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
			return string.Compare(strA, indexA, strB, indexB, length, culture, compareOptions);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, CultureInfo culture, CompareOptions options)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			int num = length;
			int num2 = length;
			if (strA != null)
			{
				num = Math.Min(num, strA.Length - indexA);
			}
			if (strB != null)
			{
				num2 = Math.Min(num2, strB.Length - indexB);
			}
			return culture.CompareInfo.Compare(strA, indexA, num, strB, indexB, num2, options);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, StringComparison comparisonType)
		{
			string.CheckStringComparison(comparisonType);
			if (strA == null || strB == null)
			{
				if (strA == strB)
				{
					return 0;
				}
				if (strA != null)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (length < 0)
				{
					throw new ArgumentOutOfRangeException("length", "Length cannot be less than zero.");
				}
				if (indexA < 0 || indexB < 0)
				{
					throw new ArgumentOutOfRangeException((indexA < 0) ? "indexA" : "indexB", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (strA.Length - indexA < 0 || strB.Length - indexB < 0)
				{
					throw new ArgumentOutOfRangeException((strA.Length - indexA < 0) ? "indexA" : "indexB", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (length == 0 || (strA == strB && indexA == indexB))
				{
					return 0;
				}
				int num = Math.Min(length, strA.Length - indexA);
				int num2 = Math.Min(length, strB.Length - indexB);
				switch (comparisonType)
				{
				case StringComparison.CurrentCulture:
					return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, num, strB, indexB, num2, CompareOptions.None);
				case StringComparison.CurrentCultureIgnoreCase:
					return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, num, strB, indexB, num2, CompareOptions.IgnoreCase);
				case StringComparison.InvariantCulture:
					return CompareInfo.Invariant.Compare(strA, indexA, num, strB, indexB, num2, CompareOptions.None);
				case StringComparison.InvariantCultureIgnoreCase:
					return CompareInfo.Invariant.Compare(strA, indexA, num, strB, indexB, num2, CompareOptions.IgnoreCase);
				case StringComparison.Ordinal:
					return string.CompareOrdinalHelper(strA, indexA, num, strB, indexB, num2);
				case StringComparison.OrdinalIgnoreCase:
					return CompareInfo.CompareOrdinalIgnoreCase(strA, indexA, num, strB, indexB, num2);
				default:
					throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
				}
			}
		}

		public static int CompareOrdinal(string strA, string strB)
		{
			if (strA == strB)
			{
				return 0;
			}
			if (strA == null)
			{
				return -1;
			}
			if (strB == null)
			{
				return 1;
			}
			if (strA._firstChar != strB._firstChar)
			{
				return (int)(strA._firstChar - strB._firstChar);
			}
			return string.CompareOrdinalHelper(strA, strB);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int CompareOrdinal(ReadOnlySpan<char> strA, ReadOnlySpan<char> strB)
		{
			return SpanHelpers.SequenceCompareTo(MemoryMarshal.GetReference<char>(strA), strA.Length, MemoryMarshal.GetReference<char>(strB), strB.Length);
		}

		public static int CompareOrdinal(string strA, int indexA, string strB, int indexB, int length)
		{
			if (strA == null || strB == null)
			{
				if (strA == strB)
				{
					return 0;
				}
				if (strA != null)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (length < 0)
				{
					throw new ArgumentOutOfRangeException("length", "Count cannot be less than zero.");
				}
				if (indexA < 0 || indexB < 0)
				{
					throw new ArgumentOutOfRangeException((indexA < 0) ? "indexA" : "indexB", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				int num = Math.Min(length, strA.Length - indexA);
				int num2 = Math.Min(length, strB.Length - indexB);
				if (num < 0 || num2 < 0)
				{
					throw new ArgumentOutOfRangeException((num < 0) ? "indexA" : "indexB", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (length == 0 || (strA == strB && indexA == indexB))
				{
					return 0;
				}
				return string.CompareOrdinalHelper(strA, indexA, num, strB, indexB, num2);
			}
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			string text = value as string;
			if (text == null)
			{
				throw new ArgumentException("Object must be of type String.");
			}
			return this.CompareTo(text);
		}

		public int CompareTo(string strB)
		{
			return string.Compare(this, strB, StringComparison.CurrentCulture);
		}

		public bool EndsWith(string value)
		{
			return this.EndsWith(value, StringComparison.CurrentCulture);
		}

		public bool EndsWith(string value, StringComparison comparisonType)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this == value)
			{
				string.CheckStringComparison(comparisonType);
				return true;
			}
			if (value.Length == 0)
			{
				string.CheckStringComparison(comparisonType);
				return true;
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CompareInfo.Invariant.IsSuffix(this, value, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CompareInfo.Invariant.IsSuffix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return this.Length >= value.Length && string.CompareOrdinalHelper(this, this.Length - value.Length, value.Length, value, 0, value.Length) == 0;
			case StringComparison.OrdinalIgnoreCase:
				return this.Length >= value.Length && CompareInfo.CompareOrdinalIgnoreCase(this, this.Length - value.Length, value.Length, value, 0, value.Length) == 0;
			default:
				throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
			}
		}

		public bool EndsWith(string value, bool ignoreCase, CultureInfo culture)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return this == value || (culture ?? CultureInfo.CurrentCulture).CompareInfo.IsSuffix(this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		public bool EndsWith(char value)
		{
			int length = this.Length;
			return length != 0 && this[length - 1] == value;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			string text = obj as string;
			return text != null && this.Length == text.Length && string.EqualsHelper(this, text);
		}

		public bool Equals(string value)
		{
			return this == value || (value != null && this.Length == value.Length && string.EqualsHelper(this, value));
		}

		public bool Equals(string value, StringComparison comparisonType)
		{
			if (this == value)
			{
				string.CheckStringComparison(comparisonType);
				return true;
			}
			if (value == null)
			{
				string.CheckStringComparison(comparisonType);
				return false;
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.Compare(this, value, CompareOptions.None) == 0;
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.Compare(this, value, CompareOptions.IgnoreCase) == 0;
			case StringComparison.InvariantCulture:
				return CompareInfo.Invariant.Compare(this, value, CompareOptions.None) == 0;
			case StringComparison.InvariantCultureIgnoreCase:
				return CompareInfo.Invariant.Compare(this, value, CompareOptions.IgnoreCase) == 0;
			case StringComparison.Ordinal:
				return this.Length == value.Length && string.EqualsHelper(this, value);
			case StringComparison.OrdinalIgnoreCase:
				return this.Length == value.Length && CompareInfo.CompareOrdinalIgnoreCase(this, 0, this.Length, value, 0, value.Length) == 0;
			default:
				throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
			}
		}

		public static bool Equals(string a, string b)
		{
			return a == b || (a != null && b != null && a.Length == b.Length && string.EqualsHelper(a, b));
		}

		public static bool Equals(string a, string b, StringComparison comparisonType)
		{
			if (a == b)
			{
				string.CheckStringComparison(comparisonType);
				return true;
			}
			if (a == null || b == null)
			{
				string.CheckStringComparison(comparisonType);
				return false;
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.Compare(a, b, CompareOptions.None) == 0;
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.Compare(a, b, CompareOptions.IgnoreCase) == 0;
			case StringComparison.InvariantCulture:
				return CompareInfo.Invariant.Compare(a, b, CompareOptions.None) == 0;
			case StringComparison.InvariantCultureIgnoreCase:
				return CompareInfo.Invariant.Compare(a, b, CompareOptions.IgnoreCase) == 0;
			case StringComparison.Ordinal:
				return a.Length == b.Length && string.EqualsHelper(a, b);
			case StringComparison.OrdinalIgnoreCase:
				return a.Length == b.Length && CompareInfo.CompareOrdinalIgnoreCase(a, 0, a.Length, b, 0, b.Length) == 0;
			default:
				throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
			}
		}

		public static bool operator ==(string a, string b)
		{
			return string.Equals(a, b);
		}

		public static bool operator !=(string a, string b)
		{
			return !string.Equals(a, b);
		}

		public override int GetHashCode()
		{
			return this.GetLegacyNonRandomizedHashCode();
		}

		public int GetHashCode(StringComparison comparisonType)
		{
			return StringComparer.FromComparison(comparisonType).GetHashCode(this);
		}

		internal unsafe int GetLegacyNonRandomizedHashCode()
		{
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				int num = 5381;
				int num2 = num;
				char* ptr3 = ptr2;
				int num3;
				while ((num3 = (int)(*ptr3)) != 0)
				{
					num = ((num << 5) + num) ^ num3;
					num3 = (int)ptr3[1];
					if (num3 == 0)
					{
						break;
					}
					num2 = ((num2 << 5) + num2) ^ num3;
					ptr3 += 2;
				}
				return num + num2 * 1566083941;
			}
		}

		public bool StartsWith(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return this.StartsWith(value, StringComparison.CurrentCulture);
		}

		public bool StartsWith(string value, StringComparison comparisonType)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this == value)
			{
				string.CheckStringComparison(comparisonType);
				return true;
			}
			if (value.Length == 0)
			{
				string.CheckStringComparison(comparisonType);
				return true;
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CompareInfo.Invariant.IsPrefix(this, value, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CompareInfo.Invariant.IsPrefix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return this.Length >= value.Length && this._firstChar == value._firstChar && (value.Length == 1 || SpanHelpers.SequenceEqual(Unsafe.As<char, byte>(this.GetRawStringData()), Unsafe.As<char, byte>(value.GetRawStringData()), (ulong)((long)value.Length * 2L)));
			case StringComparison.OrdinalIgnoreCase:
				return this.Length >= value.Length && CompareInfo.CompareOrdinalIgnoreCase(this, 0, value.Length, value, 0, value.Length) == 0;
			default:
				throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
			}
		}

		public bool StartsWith(string value, bool ignoreCase, CultureInfo culture)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return this == value || (culture ?? CultureInfo.CurrentCulture).CompareInfo.IsPrefix(this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		public bool StartsWith(char value)
		{
			return this.Length != 0 && this._firstChar == value;
		}

		internal static void CheckStringComparison(StringComparison comparisonType)
		{
			if (comparisonType - StringComparison.CurrentCulture > 5)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison, ExceptionArgument.comparisonType);
			}
		}

		[PreserveDependency("CreateString(System.Char[])", "System.String")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char[] value);

		private unsafe static string Ctor(char[] value)
		{
			if (value == null || value.Length == 0)
			{
				return string.Empty;
			}
			string text = string.FastAllocateString(value.Length);
			fixed (char* ptr = &text._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char[] array = value)
				{
					char* ptr3;
					if (value == null || array.Length == 0)
					{
						ptr3 = null;
					}
					else
					{
						ptr3 = &array[0];
					}
					string.wstrcpy(ptr2, ptr3, value.Length);
					ptr = null;
				}
				return text;
			}
		}

		[PreserveDependency("CreateString(System.Char[], System.Int32, System.Int32)", "System.String")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char[] value, int startIndex, int length);

		private unsafe static string Ctor(char[] value, int startIndex, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length cannot be less than zero.");
			}
			if (startIndex > value.Length - length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (length == 0)
			{
				return string.Empty;
			}
			string text = string.FastAllocateString(length);
			fixed (char* ptr = &text._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char[] array = value)
				{
					char* ptr3;
					if (value == null || array.Length == 0)
					{
						ptr3 = null;
					}
					else
					{
						ptr3 = &array[0];
					}
					string.wstrcpy(ptr2, ptr3 + startIndex, length);
					ptr = null;
				}
				return text;
			}
		}

		[CLSCompliant(false)]
		[PreserveDependency("CreateString(System.Char*)", "System.String")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(char* value);

		private unsafe static string Ctor(char* ptr)
		{
			if (ptr == null)
			{
				return string.Empty;
			}
			int num = string.wcslen(ptr);
			if (num == 0)
			{
				return string.Empty;
			}
			string text = string.FastAllocateString(num);
			fixed (char* ptr2 = &text._firstChar)
			{
				string.wstrcpy(ptr2, ptr, num);
			}
			return text;
		}

		[PreserveDependency("CreateString(System.Char*, System.Int32, System.Int32)", "System.String")]
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(char* value, int startIndex, int length);

		private unsafe static string Ctor(char* ptr, int startIndex, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length cannot be less than zero.");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
			}
			char* ptr2 = ptr + startIndex;
			if (ptr2 < ptr)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Pointer startIndex and length do not refer to a valid string.");
			}
			if (length == 0)
			{
				return string.Empty;
			}
			if (ptr == null)
			{
				throw new ArgumentOutOfRangeException("ptr", "Pointer startIndex and length do not refer to a valid string.");
			}
			string text = string.FastAllocateString(length);
			fixed (char* ptr3 = &text._firstChar)
			{
				string.wstrcpy(ptr3, ptr2, length);
			}
			return text;
		}

		[CLSCompliant(false)]
		[PreserveDependency("CreateString(System.SByte*)", "System.String")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(sbyte* value);

		private unsafe static string Ctor(sbyte* value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			int num = new ReadOnlySpan<byte>((void*)value, int.MaxValue).IndexOf(0);
			if (num < 0)
			{
				throw new ArgumentException("The string must be null-terminated.");
			}
			return string.CreateStringForSByteConstructor((byte*)value, num);
		}

		[CLSCompliant(false)]
		[PreserveDependency("CreateString(System.SByte*, System.Int32, System.Int32)", "System.String")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(sbyte* value, int startIndex, int length);

		private unsafe static string Ctor(sbyte* value, int startIndex, int length)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length cannot be less than zero.");
			}
			if (value == null)
			{
				if (length == 0)
				{
					return string.Empty;
				}
				throw new ArgumentNullException("value");
			}
			else
			{
				byte* ptr = (byte*)(value + startIndex);
				if (ptr < (byte*)value)
				{
					throw new ArgumentOutOfRangeException("value", "Pointer startIndex and length do not refer to a valid string.");
				}
				return string.CreateStringForSByteConstructor(ptr, length);
			}
		}

		private unsafe static string CreateStringForSByteConstructor(byte* pb, int numBytes)
		{
			if (numBytes == 0)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(pb, numBytes);
		}

		[PreserveDependency("CreateString(System.SByte*, System.Int32, System.Int32, System.Text.Encoding)", "System.String")]
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe extern String(sbyte* value, int startIndex, int length, Encoding enc);

		private unsafe static string Ctor(sbyte* value, int startIndex, int length, Encoding enc)
		{
			if (enc == null)
			{
				return new string(value, startIndex, length);
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
			}
			if (value == null)
			{
				if (length == 0)
				{
					return string.Empty;
				}
				throw new ArgumentNullException("value");
			}
			else
			{
				byte* ptr = (byte*)(value + startIndex);
				if (ptr < (byte*)value)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Pointer startIndex and length do not refer to a valid string.");
				}
				return enc.GetString(new ReadOnlySpan<byte>((void*)ptr, length));
			}
		}

		[PreserveDependency("CreateString(System.Char, System.Int32)", "System.String")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char c, int count);

		private unsafe static string Ctor(char c, int count)
		{
			if (count > 0)
			{
				string text = string.FastAllocateString(count);
				fixed (char* ptr = &text._firstChar)
				{
					uint* ptr2 = (uint*)ptr;
					uint num = (uint)(((uint)c << 16) | c);
					uint* ptr3 = ptr2;
					if (count >= 4)
					{
						count -= 4;
						do
						{
							*ptr3 = num;
							ptr3[1] = num;
							ptr3 += 2;
							count -= 4;
						}
						while (count >= 0);
					}
					if ((count & 2) != 0)
					{
						*ptr3 = num;
						ptr3++;
					}
					if ((count & 1) != 0)
					{
						*(short*)ptr3 = (short)c;
					}
				}
				return text;
			}
			if (count == 0)
			{
				return string.Empty;
			}
			throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
		}

		[PreserveDependency("CreateString(System.ReadOnlySpan`1<System.Char>)", "System.String")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(ReadOnlySpan<char> value);

		private unsafe static string Ctor(ReadOnlySpan<char> value)
		{
			if (value.Length == 0)
			{
				return string.Empty;
			}
			string text = string.FastAllocateString(value.Length);
			fixed (char* ptr = &text._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* reference = MemoryMarshal.GetReference<char>(value))
				{
					char* ptr3 = reference;
					string.wstrcpy(ptr2, ptr3, value.Length);
					ptr = null;
				}
				return text;
			}
		}

		public static string Create<TState>(int length, TState state, SpanAction<char, TState> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (length > 0)
			{
				string text = string.FastAllocateString(length);
				action(new Span<char>(text.GetRawStringData(), length), state);
				return text;
			}
			if (length == 0)
			{
				return string.Empty;
			}
			throw new ArgumentOutOfRangeException("length");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlySpan<char>(string value)
		{
			if (value == null)
			{
				return default(ReadOnlySpan<char>);
			}
			return new ReadOnlySpan<char>(value.GetRawStringData(), value.Length);
		}

		public object Clone()
		{
			return this;
		}

		public unsafe static string Copy(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			string text = string.FastAllocateString(str.Length);
			fixed (char* ptr = &text._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &str._firstChar)
				{
					char* ptr4 = ptr3;
					string.wstrcpy(ptr2, ptr4, str.Length);
					ptr = null;
				}
				return text;
			}
		}

		public unsafe void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			if (sourceIndex < 0)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (count > this.Length - sourceIndex)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", "Index and count must refer to a location within the string.");
			}
			if (destinationIndex > destination.Length - count || destinationIndex < 0)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", "Index and count must refer to a location within the string.");
			}
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char[] array = destination)
				{
					char* ptr3;
					if (destination == null || array.Length == 0)
					{
						ptr3 = null;
					}
					else
					{
						ptr3 = &array[0];
					}
					string.wstrcpy(ptr3 + destinationIndex, ptr2 + sourceIndex, count);
					ptr = null;
				}
				return;
			}
		}

		public unsafe char[] ToCharArray()
		{
			if (this.Length == 0)
			{
				return Array.Empty<char>();
			}
			char[] array = new char[this.Length];
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &array[0])
				{
					string.wstrcpy(ptr3, ptr2, this.Length);
					ptr = null;
				}
				return array;
			}
		}

		public unsafe char[] ToCharArray(int startIndex, int length)
		{
			if (startIndex < 0 || startIndex > this.Length || startIndex > this.Length - length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (length > 0)
			{
				char[] array = new char[length];
				fixed (char* ptr = &this._firstChar)
				{
					char* ptr2 = ptr;
					fixed (char* ptr3 = &array[0])
					{
						string.wstrcpy(ptr3, ptr2 + startIndex, length);
						ptr = null;
					}
					return array;
				}
			}
			if (length == 0)
			{
				return Array.Empty<char>();
			}
			throw new ArgumentOutOfRangeException("length", "Index was out of range. Must be non-negative and less than the size of the collection.");
		}

		[NonVersionable]
		public static bool IsNullOrEmpty(string value)
		{
			return value == null || 0 >= value.Length;
		}

		public static bool IsNullOrWhiteSpace(string value)
		{
			if (value == null)
			{
				return true;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal ref char GetRawStringData()
		{
			return ref this._firstChar;
		}

		internal unsafe static string CreateStringFromEncoding(byte* bytes, int byteLength, Encoding encoding)
		{
			int charCount = encoding.GetCharCount(bytes, byteLength, null);
			if (charCount == 0)
			{
				return string.Empty;
			}
			string text = string.FastAllocateString(charCount);
			fixed (char* ptr = &text._firstChar)
			{
				char* ptr2 = ptr;
				encoding.GetChars(bytes, byteLength, ptr2, charCount, null);
			}
			return text;
		}

		internal static string CreateFromChar(char c)
		{
			string text = string.FastAllocateString(1);
			text._firstChar = c;
			return text;
		}

		internal unsafe static void wstrcpy(char* dmem, char* smem, int charCount)
		{
			Buffer.Memmove((byte*)dmem, (byte*)smem, (uint)(charCount * 2));
		}

		public override string ToString()
		{
			return this;
		}

		public string ToString(IFormatProvider provider)
		{
			return this;
		}

		public CharEnumerator GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		internal unsafe static int wcslen(char* ptr)
		{
			char* ptr2 = ptr;
			int num = IntPtr.Size - 1;
			while ((ptr2 & (uint)num) != 0U)
			{
				if (*ptr2 != '\0')
				{
					ptr2++;
				}
				else
				{
					IL_006E:
					int num2 = (int)((long)(ptr2 - ptr));
					if (ptr + num2 != ptr2)
					{
						throw new ArgumentException("The string must be null-terminated.");
					}
					return num2;
				}
			}
			for (;;)
			{
				if (((*(long*)ptr2 + 9223231297218904063L) | 9223231297218904063L) == -1L)
				{
					ptr2 += 4;
				}
				else
				{
					if (*ptr2 == '\0')
					{
						goto IL_006E;
					}
					if (ptr2[1] == '\0')
					{
						goto IL_006A;
					}
					if (ptr2[2] == '\0')
					{
						goto IL_0066;
					}
					if (ptr2[3] == '\0')
					{
						break;
					}
					ptr2 += 4;
				}
			}
			ptr2++;
			IL_0066:
			ptr2++;
			IL_006A:
			ptr2++;
			goto IL_006E;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.String;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this, provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this, provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this, provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this, provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this, provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this, provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this, provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this, provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this, provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this, provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this, provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this, provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this, provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(this, provider);
		}

		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return Convert.DefaultToType(this, type, provider);
		}

		public bool IsNormalized()
		{
			return this.IsNormalized(NormalizationForm.FormC);
		}

		public bool IsNormalized(NormalizationForm normalizationForm)
		{
			return Normalization.IsNormalized(this, normalizationForm);
		}

		public string Normalize()
		{
			return this.Normalize(NormalizationForm.FormC);
		}

		public string Normalize(NormalizationForm normalizationForm)
		{
			return Normalization.Normalize(this, normalizationForm);
		}

		private unsafe static void FillStringChecked(string dest, int destPos, string src)
		{
			if (src.Length > dest.Length - destPos)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (char* ptr = &dest._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &src._firstChar)
				{
					char* ptr4 = ptr3;
					string.wstrcpy(ptr2 + destPos, ptr4, src.Length);
				}
			}
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
			if (arg0 == null)
			{
				arg0 = string.Empty;
			}
			if (arg1 == null)
			{
				arg1 = string.Empty;
			}
			return arg0.ToString() + arg1.ToString();
		}

		public static string Concat(object arg0, object arg1, object arg2)
		{
			if (arg0 == null)
			{
				arg0 = string.Empty;
			}
			if (arg1 == null)
			{
				arg1 = string.Empty;
			}
			if (arg2 == null)
			{
				arg2 = string.Empty;
			}
			return arg0.ToString() + arg1.ToString() + arg2.ToString();
		}

		public static string Concat(params object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (args.Length <= 1)
			{
				string text;
				if (args.Length != 0)
				{
					object obj = args[0];
					if ((text = ((obj != null) ? obj.ToString() : null)) == null)
					{
						return string.Empty;
					}
				}
				else
				{
					text = string.Empty;
				}
				return text;
			}
			string[] array = new string[args.Length];
			int num = 0;
			for (int i = 0; i < args.Length; i++)
			{
				object obj2 = args[i];
				string text2 = ((obj2 != null) ? obj2.ToString() : null) ?? string.Empty;
				array[i] = text2;
				num += text2.Length;
				if (num < 0)
				{
					throw new OutOfMemoryException();
				}
			}
			if (num == 0)
			{
				return string.Empty;
			}
			string text3 = string.FastAllocateString(num);
			int num2 = 0;
			foreach (string text4 in array)
			{
				string.FillStringChecked(text3, num2, text4);
				num2 += text4.Length;
			}
			return text3;
		}

		public static string Concat<T>(IEnumerable<T> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (typeof(T) == typeof(char))
			{
				using (IEnumerator<char> enumerator = Unsafe.As<IEnumerable<char>>(values).GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return string.Empty;
					}
					char c = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						return string.CreateFromChar(c);
					}
					StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
					stringBuilder.Append(c);
					do
					{
						c = enumerator.Current;
						stringBuilder.Append(c);
					}
					while (enumerator.MoveNext());
					return StringBuilderCache.GetStringAndRelease(stringBuilder);
				}
			}
			string text;
			using (IEnumerator<T> enumerator2 = values.GetEnumerator())
			{
				if (!enumerator2.MoveNext())
				{
					text = string.Empty;
				}
				else
				{
					T t = enumerator2.Current;
					string text2 = ((t != null) ? t.ToString() : null);
					if (!enumerator2.MoveNext())
					{
						text = text2 ?? string.Empty;
					}
					else
					{
						StringBuilder stringBuilder2 = StringBuilderCache.Acquire(16);
						stringBuilder2.Append(text2);
						do
						{
							t = enumerator2.Current;
							if (t != null)
							{
								stringBuilder2.Append(t.ToString());
							}
						}
						while (enumerator2.MoveNext());
						text = StringBuilderCache.GetStringAndRelease(stringBuilder2);
					}
				}
			}
			return text;
		}

		public static string Concat(IEnumerable<string> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			string text;
			using (IEnumerator<string> enumerator = values.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					text = string.Empty;
				}
				else
				{
					string text2 = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						text = text2 ?? string.Empty;
					}
					else
					{
						StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
						stringBuilder.Append(text2);
						do
						{
							stringBuilder.Append(enumerator.Current);
						}
						while (enumerator.MoveNext());
						text = StringBuilderCache.GetStringAndRelease(stringBuilder);
					}
				}
			}
			return text;
		}

		public static string Concat(string str0, string str1)
		{
			if (string.IsNullOrEmpty(str0))
			{
				if (string.IsNullOrEmpty(str1))
				{
					return string.Empty;
				}
				return str1;
			}
			else
			{
				if (string.IsNullOrEmpty(str1))
				{
					return str0;
				}
				int length = str0.Length;
				string text = string.FastAllocateString(length + str1.Length);
				string.FillStringChecked(text, 0, str0);
				string.FillStringChecked(text, length, str1);
				return text;
			}
		}

		public static string Concat(string str0, string str1, string str2)
		{
			if (string.IsNullOrEmpty(str0))
			{
				return str1 + str2;
			}
			if (string.IsNullOrEmpty(str1))
			{
				return str0 + str2;
			}
			if (string.IsNullOrEmpty(str2))
			{
				return str0 + str1;
			}
			string text = string.FastAllocateString(str0.Length + str1.Length + str2.Length);
			string.FillStringChecked(text, 0, str0);
			string.FillStringChecked(text, str0.Length, str1);
			string.FillStringChecked(text, str0.Length + str1.Length, str2);
			return text;
		}

		public static string Concat(string str0, string str1, string str2, string str3)
		{
			if (string.IsNullOrEmpty(str0))
			{
				return str1 + str2 + str3;
			}
			if (string.IsNullOrEmpty(str1))
			{
				return str0 + str2 + str3;
			}
			if (string.IsNullOrEmpty(str2))
			{
				return str0 + str1 + str3;
			}
			if (string.IsNullOrEmpty(str3))
			{
				return str0 + str1 + str2;
			}
			string text = string.FastAllocateString(str0.Length + str1.Length + str2.Length + str3.Length);
			string.FillStringChecked(text, 0, str0);
			string.FillStringChecked(text, str0.Length, str1);
			string.FillStringChecked(text, str0.Length + str1.Length, str2);
			string.FillStringChecked(text, str0.Length + str1.Length + str2.Length, str3);
			return text;
		}

		public static string Concat(params string[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length <= 1)
			{
				string text;
				if (values.Length != 0)
				{
					if ((text = values[0]) == null)
					{
						return string.Empty;
					}
				}
				else
				{
					text = string.Empty;
				}
				return text;
			}
			long num = 0L;
			foreach (string text2 in values)
			{
				if (text2 != null)
				{
					num += (long)text2.Length;
				}
			}
			if (num > 2147483647L)
			{
				throw new OutOfMemoryException();
			}
			int num2 = (int)num;
			if (num2 == 0)
			{
				return string.Empty;
			}
			string text3 = string.FastAllocateString(num2);
			int num3 = 0;
			foreach (string text4 in values)
			{
				if (!string.IsNullOrEmpty(text4))
				{
					int length = text4.Length;
					if (length > num2 - num3)
					{
						num3 = -1;
						break;
					}
					string.FillStringChecked(text3, num3, text4);
					num3 += length;
				}
			}
			if (num3 != num2)
			{
				return string.Concat((string[])values.Clone());
			}
			return text3;
		}

		public static string Format(string format, object arg0)
		{
			return string.FormatHelper(null, format, new ParamsArray(arg0));
		}

		public static string Format(string format, object arg0, object arg1)
		{
			return string.FormatHelper(null, format, new ParamsArray(arg0, arg1));
		}

		public static string Format(string format, object arg0, object arg1, object arg2)
		{
			return string.FormatHelper(null, format, new ParamsArray(arg0, arg1, arg2));
		}

		public static string Format(string format, params object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException((format == null) ? "format" : "args");
			}
			return string.FormatHelper(null, format, new ParamsArray(args));
		}

		public static string Format(IFormatProvider provider, string format, object arg0)
		{
			return string.FormatHelper(provider, format, new ParamsArray(arg0));
		}

		public static string Format(IFormatProvider provider, string format, object arg0, object arg1)
		{
			return string.FormatHelper(provider, format, new ParamsArray(arg0, arg1));
		}

		public static string Format(IFormatProvider provider, string format, object arg0, object arg1, object arg2)
		{
			return string.FormatHelper(provider, format, new ParamsArray(arg0, arg1, arg2));
		}

		public static string Format(IFormatProvider provider, string format, params object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException((format == null) ? "format" : "args");
			}
			return string.FormatHelper(provider, format, new ParamsArray(args));
		}

		private static string FormatHelper(IFormatProvider provider, string format, ParamsArray args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			return StringBuilderCache.GetStringAndRelease(StringBuilderCache.Acquire(format.Length + args.Length * 8).AppendFormatHelper(provider, format, args));
		}

		public unsafe string Insert(int startIndex, string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			int length = this.Length;
			int length2 = value.Length;
			if (length == 0)
			{
				return value;
			}
			if (length2 == 0)
			{
				return this;
			}
			string text = string.FastAllocateString(length + length2);
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &value._firstChar)
				{
					char* ptr4 = ptr3;
					fixed (char* ptr5 = &text._firstChar)
					{
						char* ptr6 = ptr5;
						string.wstrcpy(ptr6, ptr2, startIndex);
						string.wstrcpy(ptr6 + startIndex, ptr4, length2);
						string.wstrcpy(ptr6 + startIndex + length2, ptr2 + startIndex, length - startIndex);
					}
				}
			}
			return text;
		}

		public static string Join(char separator, params string[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return string.Join(separator, value, 0, value.Length);
		}

		public unsafe static string Join(char separator, params object[] values)
		{
			return string.JoinCore(&separator, 1, values);
		}

		public unsafe static string Join<T>(char separator, IEnumerable<T> values)
		{
			return string.JoinCore<T>(&separator, 1, values);
		}

		public unsafe static string Join(char separator, string[] value, int startIndex, int count)
		{
			return string.JoinCore(&separator, 1, value, startIndex, count);
		}

		public static string Join(string separator, params string[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return string.Join(separator, value, 0, value.Length);
		}

		public unsafe static string Join(string separator, params object[] values)
		{
			separator = separator ?? string.Empty;
			fixed (char* ptr = &separator._firstChar)
			{
				return string.JoinCore(ptr, separator.Length, values);
			}
		}

		public unsafe static string Join<T>(string separator, IEnumerable<T> values)
		{
			separator = separator ?? string.Empty;
			fixed (char* ptr = &separator._firstChar)
			{
				return string.JoinCore<T>(ptr, separator.Length, values);
			}
		}

		public static string Join(string separator, IEnumerable<string> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			string text;
			using (IEnumerator<string> enumerator = values.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					text = string.Empty;
				}
				else
				{
					string text2 = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						text = text2 ?? string.Empty;
					}
					else
					{
						StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
						stringBuilder.Append(text2);
						do
						{
							stringBuilder.Append(separator);
							stringBuilder.Append(enumerator.Current);
						}
						while (enumerator.MoveNext());
						text = StringBuilderCache.GetStringAndRelease(stringBuilder);
					}
				}
			}
			return text;
		}

		public unsafe static string Join(string separator, string[] value, int startIndex, int count)
		{
			separator = separator ?? string.Empty;
			fixed (char* ptr = &separator._firstChar)
			{
				return string.JoinCore(ptr, separator.Length, value, startIndex, count);
			}
		}

		private unsafe static string JoinCore(char* separator, int separatorLength, object[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				return string.Empty;
			}
			object obj = values[0];
			string text = ((obj != null) ? obj.ToString() : null);
			if (values.Length == 1)
			{
				return text ?? string.Empty;
			}
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			stringBuilder.Append(text);
			for (int i = 1; i < values.Length; i++)
			{
				stringBuilder.Append(separator, separatorLength);
				object obj2 = values[i];
				if (obj2 != null)
				{
					stringBuilder.Append(obj2.ToString());
				}
			}
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}

		private unsafe static string JoinCore<T>(char* separator, int separatorLength, IEnumerable<T> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			string text;
			using (IEnumerator<T> enumerator = values.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					text = string.Empty;
				}
				else
				{
					T t = enumerator.Current;
					string text2 = ((t != null) ? t.ToString() : null);
					if (!enumerator.MoveNext())
					{
						text = text2 ?? string.Empty;
					}
					else
					{
						StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
						stringBuilder.Append(text2);
						do
						{
							t = enumerator.Current;
							stringBuilder.Append(separator, separatorLength);
							if (t != null)
							{
								stringBuilder.Append(t.ToString());
							}
						}
						while (enumerator.MoveNext());
						text = StringBuilderCache.GetStringAndRelease(stringBuilder);
					}
				}
			}
			return text;
		}

		private unsafe static string JoinCore(char* separator, int separatorLength, string[] value, int startIndex, int count)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			if (startIndex > value.Length - count)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index and count must refer to a location within the buffer.");
			}
			if (count <= 1)
			{
				string text;
				if (count != 0)
				{
					if ((text = value[startIndex]) == null)
					{
						return string.Empty;
					}
				}
				else
				{
					text = string.Empty;
				}
				return text;
			}
			long num = (long)(count - 1) * (long)separatorLength;
			if (num > 2147483647L)
			{
				throw new OutOfMemoryException();
			}
			int num2 = (int)num;
			int i = startIndex;
			int num3 = startIndex + count;
			while (i < num3)
			{
				string text2 = value[i];
				if (text2 != null)
				{
					num2 += text2.Length;
					if (num2 < 0)
					{
						throw new OutOfMemoryException();
					}
				}
				i++;
			}
			string text3 = string.FastAllocateString(num2);
			int num4 = 0;
			int j = startIndex;
			int num5 = startIndex + count;
			while (j < num5)
			{
				string text4 = value[j];
				if (text4 != null)
				{
					int length = text4.Length;
					if (length > num2 - num4)
					{
						num4 = -1;
						break;
					}
					string.FillStringChecked(text3, num4, text4);
					num4 += length;
				}
				if (j < num5 - 1)
				{
					fixed (char* ptr = &text3._firstChar)
					{
						char* ptr2 = ptr;
						if (separatorLength == 1)
						{
							ptr2[num4] = *separator;
						}
						else
						{
							string.wstrcpy(ptr2 + num4, separator, separatorLength);
						}
					}
					num4 += separatorLength;
				}
				j++;
			}
			if (num4 != num2)
			{
				return string.JoinCore(separator, separatorLength, (string[])value.Clone(), startIndex, count);
			}
			return text3;
		}

		public string PadLeft(int totalWidth)
		{
			return this.PadLeft(totalWidth, ' ');
		}

		public unsafe string PadLeft(int totalWidth, char paddingChar)
		{
			if (totalWidth < 0)
			{
				throw new ArgumentOutOfRangeException("totalWidth", "Non-negative number required.");
			}
			int length = this.Length;
			int num = totalWidth - length;
			if (num <= 0)
			{
				return this;
			}
			string text = string.FastAllocateString(totalWidth);
			fixed (char* ptr = &text._firstChar)
			{
				char* ptr2 = ptr;
				for (int i = 0; i < num; i++)
				{
					ptr2[i] = paddingChar;
				}
				fixed (char* ptr3 = &this._firstChar)
				{
					char* ptr4 = ptr3;
					string.wstrcpy(ptr2 + num, ptr4, length);
				}
			}
			return text;
		}

		public string PadRight(int totalWidth)
		{
			return this.PadRight(totalWidth, ' ');
		}

		public unsafe string PadRight(int totalWidth, char paddingChar)
		{
			if (totalWidth < 0)
			{
				throw new ArgumentOutOfRangeException("totalWidth", "Non-negative number required.");
			}
			int length = this.Length;
			int num = totalWidth - length;
			if (num <= 0)
			{
				return this;
			}
			string text = string.FastAllocateString(totalWidth);
			fixed (char* ptr = &text._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &this._firstChar)
				{
					char* ptr4 = ptr3;
					string.wstrcpy(ptr2, ptr4, length);
				}
				for (int i = 0; i < num; i++)
				{
					ptr2[length + i] = paddingChar;
				}
			}
			return text;
		}

		public unsafe string Remove(int startIndex, int count)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			int length = this.Length;
			if (count > length - startIndex)
			{
				throw new ArgumentOutOfRangeException("count", "Index and count must refer to a location within the string.");
			}
			if (count == 0)
			{
				return this;
			}
			int num = length - count;
			if (num == 0)
			{
				return string.Empty;
			}
			string text = string.FastAllocateString(num);
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &text._firstChar)
				{
					char* ptr4 = ptr3;
					string.wstrcpy(ptr4, ptr2, startIndex);
					string.wstrcpy(ptr4 + startIndex, ptr2 + startIndex + count, num - startIndex);
				}
			}
			return text;
		}

		public string Remove(int startIndex)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
			}
			if (startIndex >= this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "startIndex must be less than length of string.");
			}
			return this.Substring(0, startIndex);
		}

		public string Replace(string oldValue, string newValue, bool ignoreCase, CultureInfo culture)
		{
			return this.ReplaceCore(oldValue, newValue, culture, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		public string Replace(string oldValue, string newValue, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return this.ReplaceCore(oldValue, newValue, CultureInfo.CurrentCulture, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return this.ReplaceCore(oldValue, newValue, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return this.ReplaceCore(oldValue, newValue, CultureInfo.InvariantCulture, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return this.ReplaceCore(oldValue, newValue, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return this.Replace(oldValue, newValue);
			case StringComparison.OrdinalIgnoreCase:
				return this.ReplaceCore(oldValue, newValue, CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase);
			default:
				throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
			}
		}

		private unsafe string ReplaceCore(string oldValue, string newValue, CultureInfo culture, CompareOptions options)
		{
			if (oldValue == null)
			{
				throw new ArgumentNullException("oldValue");
			}
			if (oldValue.Length == 0)
			{
				throw new ArgumentException("String cannot be of zero length.", "oldValue");
			}
			if (newValue == null)
			{
				newValue = string.Empty;
			}
			CultureInfo cultureInfo = culture ?? CultureInfo.CurrentCulture;
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			int num = 0;
			int num2 = 0;
			bool flag = false;
			CompareInfo compareInfo = cultureInfo.CompareInfo;
			for (;;)
			{
				int num3 = compareInfo.IndexOf(this, oldValue, num, this.Length - num, options, &num2);
				if (num3 >= 0)
				{
					stringBuilder.Append(this, num, num3 - num);
					stringBuilder.Append(newValue);
					num = num3 + num2;
					flag = true;
				}
				else
				{
					if (!flag)
					{
						break;
					}
					stringBuilder.Append(this, num, this.Length - num);
				}
				if (num3 < 0)
				{
					goto Block_7;
				}
			}
			StringBuilderCache.Release(stringBuilder);
			return this;
			Block_7:
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}

		public unsafe string Replace(char oldChar, char newChar)
		{
			if (oldChar == newChar)
			{
				return this;
			}
			int num = this.Length;
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				while (num > 0 && *ptr2 != oldChar)
				{
					num--;
					ptr2++;
				}
			}
			if (num == 0)
			{
				return this;
			}
			string text = string.FastAllocateString(this.Length);
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr3 = ptr;
				fixed (char* ptr4 = &text._firstChar)
				{
					char* ptr5 = ptr4;
					int num2 = this.Length - num;
					if (num2 > 0)
					{
						string.wstrcpy(ptr5, ptr3, num2);
					}
					char* ptr6 = ptr3 + num2;
					char* ptr7 = ptr5 + num2;
					do
					{
						char c = *ptr6;
						if (c == oldChar)
						{
							c = newChar;
						}
						*ptr7 = c;
						num--;
						ptr6++;
						ptr7++;
					}
					while (num > 0);
				}
			}
			return text;
		}

		public unsafe string Replace(string oldValue, string newValue)
		{
			if (oldValue == null)
			{
				throw new ArgumentNullException("oldValue");
			}
			if (oldValue.Length == 0)
			{
				throw new ArgumentException("String cannot be of zero length.", "oldValue");
			}
			if (newValue == null)
			{
				newValue = string.Empty;
			}
			Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)512], 128);
			ValueListBuilder<int> valueListBuilder = new ValueListBuilder<int>(span);
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				int i = 0;
				int num = this.Length - oldValue.Length;
				IL_00B6:
				while (i <= num)
				{
					char* ptr3 = ptr2 + i;
					for (int j = 0; j < oldValue.Length; j++)
					{
						if (ptr3[j] != oldValue[j])
						{
							i++;
							goto IL_00B6;
						}
					}
					valueListBuilder.Append(i);
					i += oldValue.Length;
				}
			}
			if (valueListBuilder.Length == 0)
			{
				return this;
			}
			string text = this.ReplaceHelper(oldValue.Length, newValue, valueListBuilder.AsSpan());
			valueListBuilder.Dispose();
			return text;
		}

		private unsafe string ReplaceHelper(int oldValueLength, string newValue, ReadOnlySpan<int> indices)
		{
			long num = (long)this.Length + (long)(newValue.Length - oldValueLength) * (long)indices.Length;
			if (num > 2147483647L)
			{
				throw new OutOfMemoryException();
			}
			string text = string.FastAllocateString((int)num);
			Span<char> span = new Span<char>(text.GetRawStringData(), text.Length);
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < indices.Length; i++)
			{
				int num4 = *indices[i];
				int num5 = num4 - num2;
				if (num5 != 0)
				{
					this.AsSpan(num2, num5).CopyTo(span.Slice(num3));
					num3 += num5;
				}
				num2 = num4 + oldValueLength;
				newValue.AsSpan().CopyTo(span.Slice(num3));
				num3 += newValue.Length;
			}
			this.AsSpan(num2).CopyTo(span.Slice(num3));
			return text;
		}

		public string[] Split(char separator, StringSplitOptions options = StringSplitOptions.None)
		{
			return this.SplitInternal(new ReadOnlySpan<char>(ref separator, 1), int.MaxValue, options);
		}

		public string[] Split(char separator, int count, StringSplitOptions options = StringSplitOptions.None)
		{
			return this.SplitInternal(new ReadOnlySpan<char>(ref separator, 1), count, options);
		}

		public string[] Split(params char[] separator)
		{
			return this.SplitInternal(separator, int.MaxValue, StringSplitOptions.None);
		}

		public string[] Split(char[] separator, int count)
		{
			return this.SplitInternal(separator, count, StringSplitOptions.None);
		}

		public string[] Split(char[] separator, StringSplitOptions options)
		{
			return this.SplitInternal(separator, int.MaxValue, options);
		}

		public string[] Split(char[] separator, int count, StringSplitOptions options)
		{
			return this.SplitInternal(separator, count, options);
		}

		private unsafe string[] SplitInternal(ReadOnlySpan<char> separators, int count, StringSplitOptions options)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			if (options < StringSplitOptions.None || options > StringSplitOptions.RemoveEmptyEntries)
			{
				throw new ArgumentException(SR.Format("Illegal enum value: {0}.", options));
			}
			bool flag = options == StringSplitOptions.RemoveEmptyEntries;
			if (count == 0 || (flag && this.Length == 0))
			{
				return Array.Empty<string>();
			}
			if (count == 1)
			{
				return new string[] { this };
			}
			Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)512], 128);
			ValueListBuilder<int> valueListBuilder = new ValueListBuilder<int>(span);
			this.MakeSeparatorList(separators, ref valueListBuilder);
			ReadOnlySpan<int> readOnlySpan = valueListBuilder.AsSpan();
			if (readOnlySpan.Length == 0)
			{
				return new string[] { this };
			}
			string[] array = (flag ? this.SplitOmitEmptyEntries(readOnlySpan, default(ReadOnlySpan<int>), 1, count) : this.SplitKeepEmptyEntries(readOnlySpan, default(ReadOnlySpan<int>), 1, count));
			valueListBuilder.Dispose();
			return array;
		}

		public string[] Split(string separator, StringSplitOptions options = StringSplitOptions.None)
		{
			return this.SplitInternal(separator ?? string.Empty, null, int.MaxValue, options);
		}

		public string[] Split(string separator, int count, StringSplitOptions options = StringSplitOptions.None)
		{
			return this.SplitInternal(separator ?? string.Empty, null, count, options);
		}

		public string[] Split(string[] separator, StringSplitOptions options)
		{
			return this.SplitInternal(null, separator, int.MaxValue, options);
		}

		public string[] Split(string[] separator, int count, StringSplitOptions options)
		{
			return this.SplitInternal(null, separator, count, options);
		}

		private unsafe string[] SplitInternal(string separator, string[] separators, int count, StringSplitOptions options)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			if (options < StringSplitOptions.None || options > StringSplitOptions.RemoveEmptyEntries)
			{
				throw new ArgumentException(SR.Format("Illegal enum value: {0}.", (int)options));
			}
			bool flag = options == StringSplitOptions.RemoveEmptyEntries;
			bool flag2 = separator != null;
			if (!flag2 && (separators == null || separators.Length == 0))
			{
				return this.SplitInternal(null, count, options);
			}
			if (count == 0 || (flag && this.Length == 0))
			{
				return Array.Empty<string>();
			}
			if (count == 1 || (flag2 && separator.Length == 0))
			{
				return new string[] { this };
			}
			if (flag2)
			{
				return this.SplitInternal(separator, count, options);
			}
			Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)512], 128);
			ValueListBuilder<int> valueListBuilder = new ValueListBuilder<int>(span);
			Span<int> span2 = new Span<int>(stackalloc byte[(UIntPtr)512], 128);
			ValueListBuilder<int> valueListBuilder2 = new ValueListBuilder<int>(span2);
			this.MakeSeparatorList(separators, ref valueListBuilder, ref valueListBuilder2);
			ReadOnlySpan<int> readOnlySpan = valueListBuilder.AsSpan();
			ReadOnlySpan<int> readOnlySpan2 = valueListBuilder2.AsSpan();
			if (readOnlySpan.Length == 0)
			{
				return new string[] { this };
			}
			string[] array = (flag ? this.SplitOmitEmptyEntries(readOnlySpan, readOnlySpan2, 0, count) : this.SplitKeepEmptyEntries(readOnlySpan, readOnlySpan2, 0, count));
			valueListBuilder.Dispose();
			valueListBuilder2.Dispose();
			return array;
		}

		private unsafe string[] SplitInternal(string separator, int count, StringSplitOptions options)
		{
			Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)512], 128);
			ValueListBuilder<int> valueListBuilder = new ValueListBuilder<int>(span);
			this.MakeSeparatorList(separator, ref valueListBuilder);
			ReadOnlySpan<int> readOnlySpan = valueListBuilder.AsSpan();
			if (readOnlySpan.Length == 0)
			{
				return new string[] { this };
			}
			string[] array = ((options == StringSplitOptions.RemoveEmptyEntries) ? this.SplitOmitEmptyEntries(readOnlySpan, default(ReadOnlySpan<int>), separator.Length, count) : this.SplitKeepEmptyEntries(readOnlySpan, default(ReadOnlySpan<int>), separator.Length, count));
			valueListBuilder.Dispose();
			return array;
		}

		private unsafe string[] SplitKeepEmptyEntries(ReadOnlySpan<int> sepList, ReadOnlySpan<int> lengthList, int defaultLength, int count)
		{
			int num = 0;
			int num2 = 0;
			count--;
			int num3 = ((sepList.Length < count) ? sepList.Length : count);
			string[] array = new string[num3 + 1];
			int num4 = 0;
			while (num4 < num3 && num < this.Length)
			{
				array[num2++] = this.Substring(num, *sepList[num4] - num);
				num = *sepList[num4] + (lengthList.IsEmpty ? defaultLength : (*lengthList[num4]));
				num4++;
			}
			if (num < this.Length && num3 >= 0)
			{
				array[num2] = this.Substring(num);
			}
			else if (num2 == num3)
			{
				array[num2] = string.Empty;
			}
			return array;
		}

		private unsafe string[] SplitOmitEmptyEntries(ReadOnlySpan<int> sepList, ReadOnlySpan<int> lengthList, int defaultLength, int count)
		{
			int length = sepList.Length;
			int num = ((length < count) ? (length + 1) : count);
			string[] array = new string[num];
			int num2 = 0;
			int num3 = 0;
			int i = 0;
			while (i < length && num2 < this.Length)
			{
				if (*sepList[i] - num2 > 0)
				{
					array[num3++] = this.Substring(num2, *sepList[i] - num2);
				}
				num2 = *sepList[i] + (lengthList.IsEmpty ? defaultLength : (*lengthList[i]));
				if (num3 == count - 1)
				{
					while (i < length - 1)
					{
						if (num2 != *sepList[++i])
						{
							break;
						}
						num2 += (lengthList.IsEmpty ? defaultLength : (*lengthList[i]));
					}
					break;
				}
				i++;
			}
			if (num2 < this.Length)
			{
				array[num3++] = this.Substring(num2);
			}
			string[] array2 = array;
			if (num3 != num)
			{
				array2 = new string[num3];
				for (int j = 0; j < num3; j++)
				{
					array2[j] = array[j];
				}
			}
			return array2;
		}

		private unsafe void MakeSeparatorList(ReadOnlySpan<char> separators, ref ValueListBuilder<int> sepListBuilder)
		{
			switch (separators.Length)
			{
			case 0:
			{
				for (int i = 0; i < this.Length; i++)
				{
					if (char.IsWhiteSpace(this[i]))
					{
						sepListBuilder.Append(i);
					}
				}
				return;
			}
			case 1:
			{
				char c = (char)(*separators[0]);
				for (int j = 0; j < this.Length; j++)
				{
					if (this[j] == c)
					{
						sepListBuilder.Append(j);
					}
				}
				return;
			}
			case 2:
			{
				char c = (char)(*separators[0]);
				char c2 = (char)(*separators[1]);
				for (int k = 0; k < this.Length; k++)
				{
					char c3 = this[k];
					if (c3 == c || c3 == c2)
					{
						sepListBuilder.Append(k);
					}
				}
				return;
			}
			case 3:
			{
				char c = (char)(*separators[0]);
				char c2 = (char)(*separators[1]);
				char c4 = (char)(*separators[2]);
				for (int l = 0; l < this.Length; l++)
				{
					char c5 = this[l];
					if (c5 == c || c5 == c2 || c5 == c4)
					{
						sepListBuilder.Append(l);
					}
				}
				return;
			}
			default:
			{
				string.ProbabilisticMap probabilisticMap = default(string.ProbabilisticMap);
				uint* ptr = (uint*)(&probabilisticMap);
				string.InitializeProbabilisticMap(ptr, separators);
				for (int m = 0; m < this.Length; m++)
				{
					char c6 = this[m];
					if (string.IsCharBitSet(ptr, (byte)c6) && string.IsCharBitSet(ptr, (byte)(c6 >> 8)) && separators.Contains(c6))
					{
						sepListBuilder.Append(m);
					}
				}
				return;
			}
			}
		}

		private void MakeSeparatorList(string separator, ref ValueListBuilder<int> sepListBuilder)
		{
			int length = separator.Length;
			for (int i = 0; i < this.Length; i++)
			{
				if (this[i] == separator[0] && length <= this.Length - i && (length == 1 || this.AsSpan(i, length).SequenceEqual<char>(separator)))
				{
					sepListBuilder.Append(i);
					i += length - 1;
				}
			}
		}

		private void MakeSeparatorList(string[] separators, ref ValueListBuilder<int> sepListBuilder, ref ValueListBuilder<int> lengthListBuilder)
		{
			int num = separators.Length;
			for (int i = 0; i < this.Length; i++)
			{
				foreach (string text in separators)
				{
					if (!string.IsNullOrEmpty(text))
					{
						int length = text.Length;
						if (this[i] == text[0] && length <= this.Length - i && (length == 1 || this.AsSpan(i, length).SequenceEqual<char>(text)))
						{
							sepListBuilder.Append(i);
							lengthListBuilder.Append(length);
							i += length - 1;
							break;
						}
					}
				}
			}
		}

		public string Substring(int startIndex)
		{
			return this.Substring(startIndex, this.Length - startIndex);
		}

		public string Substring(int startIndex, int length)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
			}
			if (startIndex > this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "startIndex cannot be larger than length of string.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length cannot be less than zero.");
			}
			if (startIndex > this.Length - length)
			{
				throw new ArgumentOutOfRangeException("length", "Index and length must refer to a location within the string.");
			}
			if (length == 0)
			{
				return string.Empty;
			}
			if (startIndex == 0 && length == this.Length)
			{
				return this;
			}
			return this.InternalSubString(startIndex, length);
		}

		private unsafe string InternalSubString(int startIndex, int length)
		{
			string text = string.FastAllocateString(length);
			fixed (char* ptr = &text._firstChar)
			{
				char* ptr2 = ptr;
				fixed (char* ptr3 = &this._firstChar)
				{
					char* ptr4 = ptr3;
					string.wstrcpy(ptr2, ptr4 + startIndex, length);
				}
			}
			return text;
		}

		public string ToLower()
		{
			return CultureInfo.CurrentCulture.TextInfo.ToLower(this);
		}

		public string ToLower(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return culture.TextInfo.ToLower(this);
		}

		public string ToLowerInvariant()
		{
			return CultureInfo.InvariantCulture.TextInfo.ToLower(this);
		}

		public string ToUpper()
		{
			return CultureInfo.CurrentCulture.TextInfo.ToUpper(this);
		}

		public string ToUpper(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return culture.TextInfo.ToUpper(this);
		}

		public string ToUpperInvariant()
		{
			return CultureInfo.InvariantCulture.TextInfo.ToUpper(this);
		}

		public string Trim()
		{
			return this.TrimWhiteSpaceHelper(string.TrimType.Both);
		}

		public unsafe string Trim(char trimChar)
		{
			return this.TrimHelper(&trimChar, 1, string.TrimType.Both);
		}

		public unsafe string Trim(params char[] trimChars)
		{
			if (trimChars == null || trimChars.Length == 0)
			{
				return this.TrimWhiteSpaceHelper(string.TrimType.Both);
			}
			fixed (char* ptr = &trimChars[0])
			{
				char* ptr2 = ptr;
				return this.TrimHelper(ptr2, trimChars.Length, string.TrimType.Both);
			}
		}

		public string TrimStart()
		{
			return this.TrimWhiteSpaceHelper(string.TrimType.Head);
		}

		public unsafe string TrimStart(char trimChar)
		{
			return this.TrimHelper(&trimChar, 1, string.TrimType.Head);
		}

		public unsafe string TrimStart(params char[] trimChars)
		{
			if (trimChars == null || trimChars.Length == 0)
			{
				return this.TrimWhiteSpaceHelper(string.TrimType.Head);
			}
			fixed (char* ptr = &trimChars[0])
			{
				char* ptr2 = ptr;
				return this.TrimHelper(ptr2, trimChars.Length, string.TrimType.Head);
			}
		}

		public string TrimEnd()
		{
			return this.TrimWhiteSpaceHelper(string.TrimType.Tail);
		}

		public unsafe string TrimEnd(char trimChar)
		{
			return this.TrimHelper(&trimChar, 1, string.TrimType.Tail);
		}

		public unsafe string TrimEnd(params char[] trimChars)
		{
			if (trimChars == null || trimChars.Length == 0)
			{
				return this.TrimWhiteSpaceHelper(string.TrimType.Tail);
			}
			fixed (char* ptr = &trimChars[0])
			{
				char* ptr2 = ptr;
				return this.TrimHelper(ptr2, trimChars.Length, string.TrimType.Tail);
			}
		}

		private string TrimWhiteSpaceHelper(string.TrimType trimType)
		{
			int num = this.Length - 1;
			int num2 = 0;
			if (trimType != string.TrimType.Tail)
			{
				num2 = 0;
				while (num2 < this.Length && char.IsWhiteSpace(this[num2]))
				{
					num2++;
				}
			}
			if (trimType != string.TrimType.Head)
			{
				num = this.Length - 1;
				while (num >= num2 && char.IsWhiteSpace(this[num]))
				{
					num--;
				}
			}
			return this.CreateTrimmedString(num2, num);
		}

		private unsafe string TrimHelper(char* trimChars, int trimCharsLength, string.TrimType trimType)
		{
			int i = this.Length - 1;
			int j = 0;
			if (trimType != string.TrimType.Tail)
			{
				for (j = 0; j < this.Length; j++)
				{
					char c = this[j];
					int num = 0;
					while (num < trimCharsLength && trimChars[num] != c)
					{
						num++;
					}
					if (num == trimCharsLength)
					{
						break;
					}
				}
			}
			if (trimType != string.TrimType.Head)
			{
				for (i = this.Length - 1; i >= j; i--)
				{
					char c2 = this[i];
					int num2 = 0;
					while (num2 < trimCharsLength && trimChars[num2] != c2)
					{
						num2++;
					}
					if (num2 == trimCharsLength)
					{
						break;
					}
				}
			}
			return this.CreateTrimmedString(j, i);
		}

		private string CreateTrimmedString(int start, int end)
		{
			int num = end - start + 1;
			if (num == this.Length)
			{
				return this;
			}
			if (num != 0)
			{
				return this.InternalSubString(start, num);
			}
			return string.Empty;
		}

		public bool Contains(string value)
		{
			return this.IndexOf(value, StringComparison.Ordinal) >= 0;
		}

		public bool Contains(string value, StringComparison comparisonType)
		{
			return this.IndexOf(value, comparisonType) >= 0;
		}

		public bool Contains(char value)
		{
			return this.IndexOf(value) != -1;
		}

		public bool Contains(char value, StringComparison comparisonType)
		{
			return this.IndexOf(value, comparisonType) != -1;
		}

		public int IndexOf(char value)
		{
			return SpanHelpers.IndexOf(ref this._firstChar, value, this.Length);
		}

		public int IndexOf(char value, int startIndex)
		{
			return this.IndexOf(value, startIndex, this.Length - startIndex);
		}

		public int IndexOf(char value, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CompareInfo.Invariant.IndexOf(this, value, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CompareInfo.Invariant.IndexOf(this, value, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return CompareInfo.Invariant.IndexOf(this, value, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return CompareInfo.Invariant.IndexOf(this, value, CompareOptions.OrdinalIgnoreCase);
			default:
				throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
			}
		}

		public int IndexOf(char value, int startIndex, int count)
		{
			if (startIndex > this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (count > this.Length - startIndex)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
			}
			int num = SpanHelpers.IndexOf(Unsafe.Add<char>(ref this._firstChar, startIndex), value, count);
			if (num != -1)
			{
				return num + startIndex;
			}
			return num;
		}

		public int IndexOfAny(char[] anyOf)
		{
			return this.IndexOfAny(anyOf, 0, this.Length);
		}

		public int IndexOfAny(char[] anyOf, int startIndex)
		{
			return this.IndexOfAny(anyOf, startIndex, this.Length - startIndex);
		}

		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException("anyOf");
			}
			if (startIndex > this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (count > this.Length - startIndex)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
			}
			if (anyOf.Length == 2)
			{
				return this.IndexOfAny(anyOf[0], anyOf[1], startIndex, count);
			}
			if (anyOf.Length == 3)
			{
				return this.IndexOfAny(anyOf[0], anyOf[1], anyOf[2], startIndex, count);
			}
			if (anyOf.Length > 3)
			{
				return this.IndexOfCharArray(anyOf, startIndex, count);
			}
			if (anyOf.Length == 1)
			{
				return this.IndexOf(anyOf[0], startIndex, count);
			}
			return -1;
		}

		private unsafe int IndexOfAny(char value1, char value2, int startIndex, int count)
		{
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + startIndex;
				while (count > 0)
				{
					char c = *ptr3;
					if (c == value1 || c == value2)
					{
						return (int)((long)(ptr3 - ptr2));
					}
					c = ptr3[1];
					if (c == value1 || c == value2)
					{
						if (count != 1)
						{
							return (int)((long)(ptr3 - ptr2)) + 1;
						}
						return -1;
					}
					else
					{
						ptr3 += 2;
						count -= 2;
					}
				}
				return -1;
			}
		}

		private unsafe int IndexOfAny(char value1, char value2, char value3, int startIndex, int count)
		{
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + startIndex;
				while (count > 0)
				{
					char c = *ptr3;
					if (c == value1 || c == value2 || c == value3)
					{
						return (int)((long)(ptr3 - ptr2));
					}
					ptr3++;
					count--;
				}
				return -1;
			}
		}

		private unsafe int IndexOfCharArray(char[] anyOf, int startIndex, int count)
		{
			string.ProbabilisticMap probabilisticMap = default(string.ProbabilisticMap);
			uint* ptr = (uint*)(&probabilisticMap);
			string.InitializeProbabilisticMap(ptr, anyOf);
			fixed (char* ptr2 = &this._firstChar)
			{
				char* ptr3 = ptr2;
				char* ptr4 = ptr3 + startIndex;
				while (count > 0)
				{
					int num = (int)(*ptr4);
					if (string.IsCharBitSet(ptr, (byte)num) && string.IsCharBitSet(ptr, (byte)(num >> 8)) && string.ArrayContains((char)num, anyOf))
					{
						return (int)((long)(ptr4 - ptr3));
					}
					count--;
					ptr4++;
				}
				return -1;
			}
		}

		private unsafe static void InitializeProbabilisticMap(uint* charMap, ReadOnlySpan<char> anyOf)
		{
			bool flag = false;
			for (int i = 0; i < anyOf.Length; i++)
			{
				int num = (int)(*anyOf[i]);
				string.SetCharBit(charMap, (byte)num);
				num >>= 8;
				if (num == 0)
				{
					flag = true;
				}
				else
				{
					string.SetCharBit(charMap, (byte)num);
				}
			}
			if (flag)
			{
				*charMap |= 1U;
			}
		}

		private static bool ArrayContains(char searchChar, char[] anyOf)
		{
			for (int i = 0; i < anyOf.Length; i++)
			{
				if (anyOf[i] == searchChar)
				{
					return true;
				}
			}
			return false;
		}

		private unsafe static bool IsCharBitSet(uint* charMap, byte value)
		{
			return (charMap[value & 7] & (1U << (value >> 3))) > 0U;
		}

		private unsafe static void SetCharBit(uint* charMap, byte value)
		{
			charMap[value & 7] |= 1U << (value >> 3);
		}

		public int IndexOf(string value)
		{
			return this.IndexOf(value, StringComparison.CurrentCulture);
		}

		public int IndexOf(string value, int startIndex)
		{
			return this.IndexOf(value, startIndex, StringComparison.CurrentCulture);
		}

		public int IndexOf(string value, int startIndex, int count)
		{
			if (startIndex < 0 || startIndex > this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (count < 0 || count > this.Length - startIndex)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
			}
			return this.IndexOf(value, startIndex, count, StringComparison.CurrentCulture);
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
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (count < 0 || startIndex > this.Length - count)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CompareInfo.Invariant.IndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CompareInfo.Invariant.IndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return CompareInfo.Invariant.IndexOfOrdinal(this, value, startIndex, count, false);
			case StringComparison.OrdinalIgnoreCase:
				return CompareInfo.Invariant.IndexOfOrdinal(this, value, startIndex, count, true);
			default:
				throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
			}
		}

		public int LastIndexOf(char value)
		{
			return SpanHelpers.LastIndexOf(ref this._firstChar, value, this.Length);
		}

		public int LastIndexOf(char value, int startIndex)
		{
			return this.LastIndexOf(value, startIndex, startIndex + 1);
		}

		public int LastIndexOf(char value, int startIndex, int count)
		{
			if (this.Length == 0)
			{
				return -1;
			}
			if (startIndex >= this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (count > startIndex + 1)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
			}
			int num = startIndex + 1 - count;
			int num2 = SpanHelpers.LastIndexOf(Unsafe.Add<char>(ref this._firstChar, num), value, count);
			if (num2 != -1)
			{
				return num2 + num;
			}
			return num2;
		}

		public int LastIndexOfAny(char[] anyOf)
		{
			return this.LastIndexOfAny(anyOf, this.Length - 1, this.Length);
		}

		public int LastIndexOfAny(char[] anyOf, int startIndex)
		{
			return this.LastIndexOfAny(anyOf, startIndex, startIndex + 1);
		}

		public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException("anyOf");
			}
			if (this.Length == 0)
			{
				return -1;
			}
			if (startIndex >= this.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (count < 0 || count - 1 > startIndex)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
			}
			if (anyOf.Length > 1)
			{
				return this.LastIndexOfCharArray(anyOf, startIndex, count);
			}
			if (anyOf.Length == 1)
			{
				return this.LastIndexOf(anyOf[0], startIndex, count);
			}
			return -1;
		}

		private unsafe int LastIndexOfCharArray(char[] anyOf, int startIndex, int count)
		{
			string.ProbabilisticMap probabilisticMap = default(string.ProbabilisticMap);
			uint* ptr = (uint*)(&probabilisticMap);
			string.InitializeProbabilisticMap(ptr, anyOf);
			fixed (char* ptr2 = &this._firstChar)
			{
				char* ptr3 = ptr2;
				char* ptr4 = ptr3 + startIndex;
				while (count > 0)
				{
					int num = (int)(*ptr4);
					if (string.IsCharBitSet(ptr, (byte)num) && string.IsCharBitSet(ptr, (byte)(num >> 8)) && string.ArrayContains((char)num, anyOf))
					{
						return (int)((long)(ptr4 - ptr3));
					}
					count--;
					ptr4--;
				}
				return -1;
			}
		}

		public int LastIndexOf(string value)
		{
			return this.LastIndexOf(value, this.Length - 1, this.Length, StringComparison.CurrentCulture);
		}

		public int LastIndexOf(string value, int startIndex)
		{
			return this.LastIndexOf(value, startIndex, startIndex + 1, StringComparison.CurrentCulture);
		}

		public int LastIndexOf(string value, int startIndex, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
			}
			return this.LastIndexOf(value, startIndex, count, StringComparison.CurrentCulture);
		}

		public int LastIndexOf(string value, StringComparison comparisonType)
		{
			return this.LastIndexOf(value, this.Length - 1, this.Length, comparisonType);
		}

		public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
		{
			return this.LastIndexOf(value, startIndex, startIndex + 1, comparisonType);
		}

		public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.Length == 0 && (startIndex == -1 || startIndex == 0))
			{
				if (value.Length != 0)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (startIndex < 0 || startIndex > this.Length)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (startIndex == this.Length)
				{
					startIndex--;
					if (count > 0)
					{
						count--;
					}
				}
				if (count < 0 || startIndex - count + 1 < 0)
				{
					throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
				}
				if (value.Length == 0)
				{
					return startIndex;
				}
				switch (comparisonType)
				{
				case StringComparison.CurrentCulture:
					return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.None);
				case StringComparison.CurrentCultureIgnoreCase:
					return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
				case StringComparison.InvariantCulture:
					return CompareInfo.Invariant.LastIndexOf(this, value, startIndex, count, CompareOptions.None);
				case StringComparison.InvariantCultureIgnoreCase:
					return CompareInfo.Invariant.LastIndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
				case StringComparison.Ordinal:
					return CompareInfo.Invariant.LastIndexOfOrdinal(this, value, startIndex, count, false);
				case StringComparison.OrdinalIgnoreCase:
					return CompareInfo.Invariant.LastIndexOfOrdinal(this, value, startIndex, count, true);
				default:
					throw new ArgumentException("The string comparison type passed in is currently not supported.", "comparisonType");
				}
			}
		}

		public int Length
		{
			get
			{
				return this._stringLength;
			}
		}

		internal unsafe int IndexOfUnchecked(string value, int startIndex, int count)
		{
			int length = value.Length;
			if (count < length)
			{
				return -1;
			}
			if (length == 0)
			{
				return startIndex;
			}
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				fixed (string text = value)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr4 = ptr2 + startIndex;
					char* ptr5 = ptr4 + count - length + 1;
					while (ptr4 != ptr5)
					{
						if (*ptr4 == *ptr3)
						{
							for (int i = 1; i < length; i++)
							{
								if (ptr4[i] != ptr3[i])
								{
									goto IL_007B;
								}
							}
							return (int)((long)(ptr4 - ptr2));
						}
						IL_007B:
						ptr4++;
					}
					ptr = null;
				}
				return -1;
			}
		}

		[CLSCompliant(false)]
		public static string Concat(object arg0, object arg1, object arg2, object arg3, __arglist)
		{
			throw new PlatformNotSupportedException();
		}

		internal unsafe int IndexOfUncheckedIgnoreCase(string value, int startIndex, int count)
		{
			int length = value.Length;
			if (count < length)
			{
				return -1;
			}
			if (length == 0)
			{
				return startIndex;
			}
			TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				fixed (string text = value)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr4 = ptr2 + startIndex;
					char* ptr5 = ptr4 + count - length + 1;
					char c = textInfo.ToUpper(*ptr3);
					while (ptr4 != ptr5)
					{
						if (textInfo.ToUpper(*ptr4) == c)
						{
							for (int i = 1; i < length; i++)
							{
								if (textInfo.ToUpper(ptr4[i]) != textInfo.ToUpper(ptr3[i]))
								{
									goto IL_00A4;
								}
							}
							return (int)((long)(ptr4 - ptr2));
						}
						IL_00A4:
						ptr4++;
					}
					ptr = null;
				}
				return -1;
			}
		}

		internal unsafe int LastIndexOfUnchecked(string value, int startIndex, int count)
		{
			int length = value.Length;
			if (count < length)
			{
				return -1;
			}
			if (length == 0)
			{
				return startIndex;
			}
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				fixed (string text = value)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr4 = ptr2 + startIndex;
					char* ptr5 = ptr4 - count + length - 1;
					char* ptr6 = ptr3 + length - 1;
					while (ptr4 != ptr5)
					{
						if (*ptr4 == *ptr6)
						{
							char* ptr7 = ptr4;
							while (ptr3 != ptr6)
							{
								ptr6--;
								ptr4--;
								if (*ptr4 != *ptr6)
								{
									ptr6 = ptr3 + length - 1;
									ptr4 = ptr7;
									goto IL_0092;
								}
							}
							return (int)((long)(ptr4 - ptr2));
						}
						IL_0092:
						ptr4--;
					}
					ptr = null;
				}
				return -1;
			}
		}

		internal unsafe int LastIndexOfUncheckedIgnoreCase(string value, int startIndex, int count)
		{
			int length = value.Length;
			if (count < length)
			{
				return -1;
			}
			if (length == 0)
			{
				return startIndex;
			}
			TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
			fixed (char* ptr = &this._firstChar)
			{
				char* ptr2 = ptr;
				fixed (string text = value)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr4 = ptr2 + startIndex;
					char* ptr5 = ptr4 - count + length - 1;
					char* ptr6 = ptr3 + length - 1;
					char c = textInfo.ToUpper(*ptr6);
					while (ptr4 != ptr5)
					{
						if (textInfo.ToUpper(*ptr4) == c)
						{
							char* ptr7 = ptr4;
							while (ptr3 != ptr6)
							{
								ptr6--;
								ptr4--;
								if (textInfo.ToUpper(*ptr4) != textInfo.ToUpper(*ptr6))
								{
									ptr6 = ptr3 + length - 1;
									ptr4 = ptr7;
									goto IL_00BB;
								}
							}
							return (int)((long)(ptr4 - ptr2));
						}
						IL_00BB:
						ptr4--;
					}
					ptr = null;
				}
				return -1;
			}
		}

		internal bool StartsWithOrdinalUnchecked(string value)
		{
			return this.Length >= value.Length && this._firstChar == value._firstChar && (value.Length == 1 || SpanHelpers.SequenceEqual(Unsafe.As<char, byte>(this.GetRawStringData()), Unsafe.As<char, byte>(value.GetRawStringData()), (ulong)((long)value.Length * 2L)));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string FastAllocateString(int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string InternalIsInterned(string str);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string InternalIntern(string str);

		private unsafe static int FastCompareStringHelper(uint* strAChars, int countA, uint* strBChars, int countB)
		{
			char* ptr = (char*)strAChars;
			char* ptr2 = (char*)strBChars;
			char* ptr3 = ptr + Math.Min(countA, countB);
			while (ptr < ptr3)
			{
				if (*ptr != *ptr2)
				{
					return (int)(*ptr - *ptr2);
				}
				ptr++;
				ptr2++;
			}
			return countA - countB;
		}

		private unsafe static void memset(byte* dest, int val, int len)
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
				*(int*)(dest + (IntPtr)2 * 4) = val;
				*(int*)(dest + (IntPtr)3 * 4) = val;
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

		private unsafe static void memcpy(byte* dest, byte* src, int size)
		{
			Buffer.Memcpy(dest, src, size);
		}

		internal unsafe static void bzero(byte* dest, int len)
		{
			string.memset(dest, 0, len);
		}

		internal unsafe static void bzero_aligned_1(byte* dest, int len)
		{
			*dest = 0;
		}

		internal unsafe static void bzero_aligned_2(byte* dest, int len)
		{
			*(short*)dest = 0;
		}

		internal unsafe static void bzero_aligned_4(byte* dest, int len)
		{
			*(int*)dest = 0;
		}

		internal unsafe static void bzero_aligned_8(byte* dest, int len)
		{
			*(long*)dest = 0L;
		}

		internal unsafe static void memcpy_aligned_1(byte* dest, byte* src, int size)
		{
			*dest = *src;
		}

		internal unsafe static void memcpy_aligned_2(byte* dest, byte* src, int size)
		{
			*(short*)dest = *(short*)src;
		}

		internal unsafe static void memcpy_aligned_4(byte* dest, byte* src, int size)
		{
			*(int*)dest = *(int*)src;
		}

		internal unsafe static void memcpy_aligned_8(byte* dest, byte* src, int size)
		{
			*(long*)dest = *(long*)src;
		}

		private unsafe string CreateString(sbyte* value)
		{
			return string.Ctor(value);
		}

		private unsafe string CreateString(sbyte* value, int startIndex, int length)
		{
			return string.Ctor(value, startIndex, length);
		}

		private unsafe string CreateString(char* value)
		{
			return string.Ctor(value);
		}

		private unsafe string CreateString(char* value, int startIndex, int length)
		{
			return string.Ctor(value, startIndex, length);
		}

		private string CreateString(char[] val, int startIndex, int length)
		{
			return string.Ctor(val, startIndex, length);
		}

		private string CreateString(char[] val)
		{
			return string.Ctor(val);
		}

		private string CreateString(char c, int count)
		{
			return string.Ctor(c, count);
		}

		private unsafe string CreateString(sbyte* value, int startIndex, int length, Encoding enc)
		{
			return string.Ctor(value, startIndex, length, enc);
		}

		private string CreateString(ReadOnlySpan<char> value)
		{
			return string.Ctor(value);
		}

		[IndexerName("Chars")]
		public unsafe char this[int index]
		{
			[Intrinsic]
			get
			{
				if ((ulong)index >= (ulong)((long)this._stringLength))
				{
					ThrowHelper.ThrowIndexOutOfRangeException();
				}
				return *Unsafe.Add<char>(ref this._firstChar, index);
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

		private unsafe int LegacyStringGetHashCode()
		{
			int num = 5381;
			int num2 = num;
			fixed (string text = this)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr;
				int num3;
				while ((num3 = (int)(*ptr2)) != 0)
				{
					num = ((num << 5) + num) ^ num3;
					num3 = (int)ptr2[1];
					if (num3 == 0)
					{
						break;
					}
					num2 = ((num2 << 5) + num2) ^ num3;
					ptr2 += 2;
				}
			}
			return num + num2 * 1566083941;
		}

		private const int StackallocIntBufferSizeLimit = 128;

		private const int PROBABILISTICMAP_BLOCK_INDEX_MASK = 7;

		private const int PROBABILISTICMAP_BLOCK_INDEX_SHIFT = 3;

		private const int PROBABILISTICMAP_SIZE = 8;

		[NonSerialized]
		private int _stringLength;

		[NonSerialized]
		private char _firstChar;

		public static readonly string Empty;

		private enum TrimType
		{
			Head,
			Tail,
			Both
		}

		[StructLayout(LayoutKind.Explicit, Size = 32)]
		private struct ProbabilisticMap
		{
		}
	}
}
