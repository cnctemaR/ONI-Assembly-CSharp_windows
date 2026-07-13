using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using Mono.Globalization.Unicode;
using Unity;

namespace System.Globalization
{
	[Serializable]
	public class CompareInfo : IDeserializationCallback
	{
		internal unsafe static int InvariantIndexOf(string source, string value, int startIndex, int count, bool ignoreCase)
		{
			char* ptr = source;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = value;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			int num = CompareInfo.InvariantFindString(ptr + startIndex, count, ptr2, value.Length, ignoreCase, true);
			if (num >= 0)
			{
				return num + startIndex;
			}
			return -1;
		}

		internal unsafe static int InvariantIndexOf(ReadOnlySpan<char> source, ReadOnlySpan<char> value, bool ignoreCase)
		{
			fixed (char* reference = MemoryMarshal.GetReference<char>(source))
			{
				char* ptr = reference;
				fixed (char* reference2 = MemoryMarshal.GetReference<char>(value))
				{
					char* ptr2 = reference2;
					return CompareInfo.InvariantFindString(ptr, source.Length, ptr2, value.Length, ignoreCase, true);
				}
			}
		}

		internal unsafe static int InvariantLastIndexOf(string source, string value, int startIndex, int count, bool ignoreCase)
		{
			char* ptr = source;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = value;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			int num = CompareInfo.InvariantFindString(ptr + (startIndex - count + 1), count, ptr2, value.Length, ignoreCase, false);
			if (num >= 0)
			{
				return num + startIndex - count + 1;
			}
			return -1;
		}

		private unsafe static int InvariantFindString(char* source, int sourceCount, char* value, int valueCount, bool ignoreCase, bool start)
		{
			if (valueCount == 0)
			{
				if (!start)
				{
					return sourceCount - 1;
				}
				return 0;
			}
			else
			{
				if (sourceCount < valueCount)
				{
					return -1;
				}
				if (start)
				{
					int num = sourceCount - valueCount;
					if (ignoreCase)
					{
						char c = CompareInfo.InvariantToUpper(*value);
						for (int i = 0; i <= num; i++)
						{
							if (CompareInfo.InvariantToUpper(source[i]) == c)
							{
								int j;
								for (j = 1; j < valueCount; j++)
								{
									char c2 = CompareInfo.InvariantToUpper(source[i + j]);
									char c3 = CompareInfo.InvariantToUpper(value[j]);
									if (c2 != c3)
									{
										break;
									}
								}
								if (j == valueCount)
								{
									return i;
								}
							}
						}
					}
					else
					{
						char c4 = *value;
						for (int i = 0; i <= num; i++)
						{
							if (source[i] == c4)
							{
								int j;
								for (j = 1; j < valueCount; j++)
								{
									char c5 = source[i + j];
									char c3 = value[j];
									if (c5 != c3)
									{
										break;
									}
								}
								if (j == valueCount)
								{
									return i;
								}
							}
						}
					}
				}
				else
				{
					int num = sourceCount - valueCount;
					if (ignoreCase)
					{
						char c6 = CompareInfo.InvariantToUpper(*value);
						for (int i = num; i >= 0; i--)
						{
							if (CompareInfo.InvariantToUpper(source[i]) == c6)
							{
								int j;
								for (j = 1; j < valueCount; j++)
								{
									char c7 = CompareInfo.InvariantToUpper(source[i + j]);
									char c3 = CompareInfo.InvariantToUpper(value[j]);
									if (c7 != c3)
									{
										break;
									}
								}
								if (j == valueCount)
								{
									return i;
								}
							}
						}
					}
					else
					{
						char c8 = *value;
						for (int i = num; i >= 0; i--)
						{
							if (source[i] == c8)
							{
								int j;
								for (j = 1; j < valueCount; j++)
								{
									char c9 = source[i + j];
									char c3 = value[j];
									if (c9 != c3)
									{
										break;
									}
								}
								if (j == valueCount)
								{
									return i;
								}
							}
						}
					}
				}
				return -1;
			}
		}

		private static char InvariantToUpper(char c)
		{
			if (c - 'a' > '\u0019')
			{
				return c;
			}
			return c - ' ';
		}

		private unsafe SortKey InvariantCreateSortKey(string source, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort)) != CompareOptions.None)
			{
				throw new ArgumentException("Value of flags is invalid.", "options");
			}
			byte[] array;
			if (source.Length == 0)
			{
				array = Array.Empty<byte>();
			}
			else
			{
				array = new byte[source.Length * 2];
				fixed (string text = source)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					byte[] array2;
					byte* ptr2;
					if ((array2 = array) == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					if ((options & (CompareOptions.IgnoreCase | CompareOptions.OrdinalIgnoreCase)) != CompareOptions.None)
					{
						short* ptr3 = (short*)ptr2;
						for (int i = 0; i < source.Length; i++)
						{
							ptr3[i] = (short)CompareInfo.InvariantToUpper(source[i]);
						}
					}
					else
					{
						Buffer.MemoryCopy((void*)ptr, (void*)ptr2, (long)array.Length, (long)array.Length);
					}
					array2 = null;
				}
			}
			return new SortKey(this.Name, source, options, array);
		}

		internal CompareInfo(CultureInfo culture)
		{
			this.m_name = culture._name;
			this.InitSort(culture);
		}

		public static CompareInfo GetCompareInfo(int culture, Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly != typeof(object).Module.Assembly)
			{
				throw new ArgumentException("Only mscorlib's assembly is valid.");
			}
			return CompareInfo.GetCompareInfo(culture);
		}

		public static CompareInfo GetCompareInfo(string name, Assembly assembly)
		{
			if (name == null || assembly == null)
			{
				throw new ArgumentNullException((name == null) ? "name" : "assembly");
			}
			if (assembly != typeof(object).Module.Assembly)
			{
				throw new ArgumentException("Only mscorlib's assembly is valid.");
			}
			return CompareInfo.GetCompareInfo(name);
		}

		public static CompareInfo GetCompareInfo(int culture)
		{
			if (CultureData.IsCustomCultureId(culture))
			{
				throw new ArgumentException("Customized cultures cannot be passed by LCID, only by name.", "culture");
			}
			return CultureInfo.GetCultureInfo(culture).CompareInfo;
		}

		public static CompareInfo GetCompareInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return CultureInfo.GetCultureInfo(name).CompareInfo;
		}

		public unsafe static bool IsSortable(char ch)
		{
			return GlobalizationMode.Invariant || CompareInfo.IsSortable(&ch, 1);
		}

		public unsafe static bool IsSortable(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			if (text.Length == 0)
			{
				return false;
			}
			if (GlobalizationMode.Invariant)
			{
				return true;
			}
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return CompareInfo.IsSortable(ptr, text.Length);
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext ctx)
		{
			this.m_name = null;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			this.OnDeserialized();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			this.OnDeserialized();
		}

		private void OnDeserialized()
		{
			if (this.m_name == null)
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(this.culture);
				this.m_name = cultureInfo._name;
				return;
			}
			this.InitSort(CultureInfo.GetCultureInfo(this.m_name));
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext ctx)
		{
			this.culture = CultureInfo.GetCultureInfo(this.Name).LCID;
		}

		public virtual string Name
		{
			get
			{
				if (this.m_name == "zh-CHT" || this.m_name == "zh-CHS")
				{
					return this.m_name;
				}
				return this._sortName;
			}
		}

		public virtual int Compare(string string1, string string2)
		{
			return this.Compare(string1, string2, CompareOptions.None);
		}

		public virtual int Compare(string string1, string string2, CompareOptions options)
		{
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return string.Compare(string1, string2, StringComparison.OrdinalIgnoreCase);
			}
			if ((options & CompareOptions.Ordinal) != CompareOptions.None)
			{
				if (options != CompareOptions.Ordinal)
				{
					throw new ArgumentException("CompareOption.Ordinal cannot be used with other options.", "options");
				}
				return string.CompareOrdinal(string1, string2);
			}
			else
			{
				if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort)) != CompareOptions.None)
				{
					throw new ArgumentException("Value of flags is invalid.", "options");
				}
				if (string1 == null)
				{
					if (string2 == null)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (string2 == null)
					{
						return 1;
					}
					if (!GlobalizationMode.Invariant)
					{
						return this.internal_compare_switch(string1, 0, string1.Length, string2, 0, string2.Length, options);
					}
					if ((options & CompareOptions.IgnoreCase) != CompareOptions.None)
					{
						return CompareInfo.CompareOrdinalIgnoreCase(string1, string2);
					}
					return string.CompareOrdinal(string1, string2);
				}
			}
		}

		internal int Compare(ReadOnlySpan<char> string1, string string2, CompareOptions options)
		{
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return CompareInfo.CompareOrdinalIgnoreCase(string1, string2.AsSpan());
			}
			if ((options & CompareOptions.Ordinal) != CompareOptions.None)
			{
				if (options != CompareOptions.Ordinal)
				{
					throw new ArgumentException("CompareOption.Ordinal cannot be used with other options.", "options");
				}
				return string.CompareOrdinal(string1, string2.AsSpan());
			}
			else
			{
				if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort)) != CompareOptions.None)
				{
					throw new ArgumentException("Value of flags is invalid.", "options");
				}
				if (string2 == null)
				{
					return 1;
				}
				if (!GlobalizationMode.Invariant)
				{
					return this.CompareString(string1, string2, options);
				}
				if ((options & CompareOptions.IgnoreCase) == CompareOptions.None)
				{
					return string.CompareOrdinal(string1, string2.AsSpan());
				}
				return CompareInfo.CompareOrdinalIgnoreCase(string1, string2.AsSpan());
			}
		}

		internal int CompareOptionNone(ReadOnlySpan<char> string1, ReadOnlySpan<char> string2)
		{
			if (string1.Length == 0 || string2.Length == 0)
			{
				return string1.Length - string2.Length;
			}
			if (!GlobalizationMode.Invariant)
			{
				return this.CompareString(string1, string2, CompareOptions.None);
			}
			return string.CompareOrdinal(string1, string2);
		}

		internal int CompareOptionIgnoreCase(ReadOnlySpan<char> string1, ReadOnlySpan<char> string2)
		{
			if (string1.Length == 0 || string2.Length == 0)
			{
				return string1.Length - string2.Length;
			}
			if (!GlobalizationMode.Invariant)
			{
				return this.CompareString(string1, string2, CompareOptions.IgnoreCase);
			}
			return CompareInfo.CompareOrdinalIgnoreCase(string1, string2);
		}

		public virtual int Compare(string string1, int offset1, int length1, string string2, int offset2, int length2)
		{
			return this.Compare(string1, offset1, length1, string2, offset2, length2, CompareOptions.None);
		}

		public virtual int Compare(string string1, int offset1, string string2, int offset2, CompareOptions options)
		{
			return this.Compare(string1, offset1, (string1 == null) ? 0 : (string1.Length - offset1), string2, offset2, (string2 == null) ? 0 : (string2.Length - offset2), options);
		}

		public virtual int Compare(string string1, int offset1, string string2, int offset2)
		{
			return this.Compare(string1, offset1, string2, offset2, CompareOptions.None);
		}

		public virtual int Compare(string string1, int offset1, int length1, string string2, int offset2, int length2, CompareOptions options)
		{
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				int num = string.Compare(string1, offset1, string2, offset2, (length1 < length2) ? length1 : length2, StringComparison.OrdinalIgnoreCase);
				if (length1 == length2 || num != 0)
				{
					return num;
				}
				if (length1 <= length2)
				{
					return -1;
				}
				return 1;
			}
			else
			{
				if (length1 < 0 || length2 < 0)
				{
					throw new ArgumentOutOfRangeException((length1 < 0) ? "length1" : "length2", "Positive number required.");
				}
				if (offset1 < 0 || offset2 < 0)
				{
					throw new ArgumentOutOfRangeException((offset1 < 0) ? "offset1" : "offset2", "Positive number required.");
				}
				if (offset1 > ((string1 == null) ? 0 : string1.Length) - length1)
				{
					throw new ArgumentOutOfRangeException("string1", "Offset and length must refer to a position in the string.");
				}
				if (offset2 > ((string2 == null) ? 0 : string2.Length) - length2)
				{
					throw new ArgumentOutOfRangeException("string2", "Offset and length must refer to a position in the string.");
				}
				if ((options & CompareOptions.Ordinal) != CompareOptions.None)
				{
					if (options != CompareOptions.Ordinal)
					{
						throw new ArgumentException("CompareOption.Ordinal cannot be used with other options.", "options");
					}
				}
				else if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort)) != CompareOptions.None)
				{
					throw new ArgumentException("Value of flags is invalid.", "options");
				}
				if (string1 == null)
				{
					if (string2 == null)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (string2 == null)
					{
						return 1;
					}
					ReadOnlySpan<char> readOnlySpan = string1.AsSpan(offset1, length1);
					ReadOnlySpan<char> readOnlySpan2 = string2.AsSpan(offset2, length2);
					if (options == CompareOptions.Ordinal)
					{
						return string.CompareOrdinal(readOnlySpan, readOnlySpan2);
					}
					if (!GlobalizationMode.Invariant)
					{
						return this.internal_compare_switch(string1, offset1, length1, string2, offset2, length2, options);
					}
					if ((options & CompareOptions.IgnoreCase) != CompareOptions.None)
					{
						return CompareInfo.CompareOrdinalIgnoreCase(readOnlySpan, readOnlySpan2);
					}
					return string.CompareOrdinal(readOnlySpan, readOnlySpan2);
				}
			}
		}

		internal static int CompareOrdinalIgnoreCase(string strA, int indexA, int lengthA, string strB, int indexB, int lengthB)
		{
			return CompareInfo.CompareOrdinalIgnoreCase(strA.AsSpan(indexA, lengthA), strB.AsSpan(indexB, lengthB));
		}

		internal unsafe static int CompareOrdinalIgnoreCase(ReadOnlySpan<char> strA, ReadOnlySpan<char> strB)
		{
			int num = Math.Min(strA.Length, strB.Length);
			int num2 = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(strA))
			{
				char* ptr = reference;
				fixed (char* reference2 = MemoryMarshal.GetReference<char>(strB))
				{
					char* ptr2 = reference2;
					char* ptr3 = ptr;
					char* ptr4 = ptr2;
					char c = (GlobalizationMode.Invariant ? char.MaxValue : '\u007f');
					while (num != 0 && *ptr3 <= c && *ptr4 <= c)
					{
						int num3 = (int)(*ptr3);
						int num4 = (int)(*ptr4);
						if (num3 == num4)
						{
							ptr3++;
							ptr4++;
							num--;
						}
						else
						{
							if (num3 - 97 <= 25)
							{
								num3 -= 32;
							}
							if (num4 - 97 <= 25)
							{
								num4 -= 32;
							}
							if (num3 != num4)
							{
								return num3 - num4;
							}
							ptr3++;
							ptr4++;
							num--;
						}
					}
					if (num == 0)
					{
						return strA.Length - strB.Length;
					}
					num2 -= num;
					return CompareInfo.CompareStringOrdinalIgnoreCase(ptr3, strA.Length - num2, ptr4, strB.Length - num2);
				}
			}
		}

		public virtual bool IsPrefix(string source, string prefix, CompareOptions options)
		{
			if (source == null || prefix == null)
			{
				throw new ArgumentNullException((source == null) ? "source" : "prefix", "String reference not set to an instance of a String.");
			}
			if (prefix.Length == 0)
			{
				return true;
			}
			if (source.Length == 0)
			{
				return false;
			}
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return source.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
			}
			if (options == CompareOptions.Ordinal)
			{
				return source.StartsWith(prefix, StringComparison.Ordinal);
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None)
			{
				throw new ArgumentException("Value of flags is invalid.", "options");
			}
			if (GlobalizationMode.Invariant)
			{
				return source.StartsWith(prefix, ((options & CompareOptions.IgnoreCase) != CompareOptions.None) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			}
			return this.StartsWith(source, prefix, options);
		}

		internal bool IsPrefix(ReadOnlySpan<char> source, ReadOnlySpan<char> prefix, CompareOptions options)
		{
			return this.StartsWith(source, prefix, options);
		}

		public virtual bool IsPrefix(string source, string prefix)
		{
			return this.IsPrefix(source, prefix, CompareOptions.None);
		}

		public virtual bool IsSuffix(string source, string suffix, CompareOptions options)
		{
			if (source == null || suffix == null)
			{
				throw new ArgumentNullException((source == null) ? "source" : "suffix", "String reference not set to an instance of a String.");
			}
			if (suffix.Length == 0)
			{
				return true;
			}
			if (source.Length == 0)
			{
				return false;
			}
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return source.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
			}
			if (options == CompareOptions.Ordinal)
			{
				return source.EndsWith(suffix, StringComparison.Ordinal);
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None)
			{
				throw new ArgumentException("Value of flags is invalid.", "options");
			}
			if (GlobalizationMode.Invariant)
			{
				return source.EndsWith(suffix, ((options & CompareOptions.IgnoreCase) != CompareOptions.None) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			}
			return this.EndsWith(source, suffix, options);
		}

		internal bool IsSuffix(ReadOnlySpan<char> source, ReadOnlySpan<char> suffix, CompareOptions options)
		{
			return this.EndsWith(source, suffix, options);
		}

		public virtual bool IsSuffix(string source, string suffix)
		{
			return this.IsSuffix(source, suffix, CompareOptions.None);
		}

		public virtual int IndexOf(string source, char value)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.IndexOf(source, value, 0, source.Length, CompareOptions.None);
		}

		public virtual int IndexOf(string source, string value)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.IndexOf(source, value, 0, source.Length, CompareOptions.None);
		}

		public virtual int IndexOf(string source, char value, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.IndexOf(source, value, 0, source.Length, options);
		}

		public virtual int IndexOf(string source, string value, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.IndexOf(source, value, 0, source.Length, options);
		}

		public virtual int IndexOf(string source, char value, int startIndex)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.IndexOf(source, value, startIndex, source.Length - startIndex, CompareOptions.None);
		}

		public virtual int IndexOf(string source, string value, int startIndex)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.IndexOf(source, value, startIndex, source.Length - startIndex, CompareOptions.None);
		}

		public virtual int IndexOf(string source, char value, int startIndex, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.IndexOf(source, value, startIndex, source.Length - startIndex, options);
		}

		public virtual int IndexOf(string source, string value, int startIndex, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.IndexOf(source, value, startIndex, source.Length - startIndex, options);
		}

		public virtual int IndexOf(string source, char value, int startIndex, int count)
		{
			return this.IndexOf(source, value, startIndex, count, CompareOptions.None);
		}

		public virtual int IndexOf(string source, string value, int startIndex, int count)
		{
			return this.IndexOf(source, value, startIndex, count, CompareOptions.None);
		}

		public virtual int IndexOf(string source, char value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (startIndex < 0 || startIndex > source.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (count < 0 || startIndex > source.Length - count)
			{
				throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
			}
			if (source.Length == 0)
			{
				return -1;
			}
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return source.IndexOf(value.ToString(), startIndex, count, StringComparison.OrdinalIgnoreCase);
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None && options != CompareOptions.Ordinal)
			{
				throw new ArgumentException("Value of flags is invalid.", "options");
			}
			if (GlobalizationMode.Invariant)
			{
				return this.IndexOfOrdinal(source, new string(value, 1), startIndex, count, (options & (CompareOptions.IgnoreCase | CompareOptions.OrdinalIgnoreCase)) > CompareOptions.None);
			}
			return this.IndexOfCore(source, new string(value, 1), startIndex, count, options, null);
		}

		public virtual int IndexOf(string source, string value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex > source.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (source.Length == 0)
			{
				if (value.Length == 0)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (startIndex < 0)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (count < 0 || startIndex > source.Length - count)
				{
					throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
				}
				if (options == CompareOptions.OrdinalIgnoreCase)
				{
					return this.IndexOfOrdinal(source, value, startIndex, count, true);
				}
				if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None && options != CompareOptions.Ordinal)
				{
					throw new ArgumentException("Value of flags is invalid.", "options");
				}
				if (GlobalizationMode.Invariant)
				{
					return this.IndexOfOrdinal(source, value, startIndex, count, (options & (CompareOptions.IgnoreCase | CompareOptions.OrdinalIgnoreCase)) > CompareOptions.None);
				}
				return this.IndexOfCore(source, value, startIndex, count, options, null);
			}
		}

		internal int IndexOfOrdinal(ReadOnlySpan<char> source, ReadOnlySpan<char> value, bool ignoreCase)
		{
			return this.IndexOfOrdinalCore(source, value, ignoreCase);
		}

		internal int IndexOf(ReadOnlySpan<char> source, ReadOnlySpan<char> value, CompareOptions options)
		{
			return this.IndexOfCore(source, value, options, null);
		}

		internal unsafe int IndexOf(string source, string value, int startIndex, int count, CompareOptions options, int* matchLengthPtr)
		{
			*matchLengthPtr = 0;
			if (source.Length == 0)
			{
				if (value.Length == 0)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (startIndex >= source.Length)
				{
					return -1;
				}
				if (options == CompareOptions.OrdinalIgnoreCase)
				{
					int num = this.IndexOfOrdinal(source, value, startIndex, count, true);
					if (num >= 0)
					{
						*matchLengthPtr = value.Length;
					}
					return num;
				}
				if (GlobalizationMode.Invariant)
				{
					int num2 = this.IndexOfOrdinal(source, value, startIndex, count, (options & (CompareOptions.IgnoreCase | CompareOptions.OrdinalIgnoreCase)) > CompareOptions.None);
					if (num2 >= 0)
					{
						*matchLengthPtr = value.Length;
					}
					return num2;
				}
				return this.IndexOfCore(source, value, startIndex, count, options, matchLengthPtr);
			}
		}

		internal int IndexOfOrdinal(string source, string value, int startIndex, int count, bool ignoreCase)
		{
			if (GlobalizationMode.Invariant)
			{
				return CompareInfo.InvariantIndexOf(source, value, startIndex, count, ignoreCase);
			}
			return CompareInfo.IndexOfOrdinalCore(source, value, startIndex, count, ignoreCase);
		}

		public virtual int LastIndexOf(string source, char value)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.LastIndexOf(source, value, source.Length - 1, source.Length, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, string value)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.LastIndexOf(source, value, source.Length - 1, source.Length, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, char value, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.LastIndexOf(source, value, source.Length - 1, source.Length, options);
		}

		public virtual int LastIndexOf(string source, string value, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return this.LastIndexOf(source, value, source.Length - 1, source.Length, options);
		}

		public virtual int LastIndexOf(string source, char value, int startIndex)
		{
			return this.LastIndexOf(source, value, startIndex, startIndex + 1, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, string value, int startIndex)
		{
			return this.LastIndexOf(source, value, startIndex, startIndex + 1, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, char value, int startIndex, CompareOptions options)
		{
			return this.LastIndexOf(source, value, startIndex, startIndex + 1, options);
		}

		public virtual int LastIndexOf(string source, string value, int startIndex, CompareOptions options)
		{
			return this.LastIndexOf(source, value, startIndex, startIndex + 1, options);
		}

		public virtual int LastIndexOf(string source, char value, int startIndex, int count)
		{
			return this.LastIndexOf(source, value, startIndex, count, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, string value, int startIndex, int count)
		{
			return this.LastIndexOf(source, value, startIndex, count, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, char value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None && options != CompareOptions.Ordinal && options != CompareOptions.OrdinalIgnoreCase)
			{
				throw new ArgumentException("Value of flags is invalid.", "options");
			}
			if (source.Length == 0 && (startIndex == -1 || startIndex == 0))
			{
				return -1;
			}
			if (startIndex < 0 || startIndex > source.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (startIndex == source.Length)
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
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return source.LastIndexOf(value.ToString(), startIndex, count, StringComparison.OrdinalIgnoreCase);
			}
			if (GlobalizationMode.Invariant)
			{
				return CompareInfo.InvariantLastIndexOf(source, new string(value, 1), startIndex, count, (options & (CompareOptions.IgnoreCase | CompareOptions.OrdinalIgnoreCase)) > CompareOptions.None);
			}
			return this.LastIndexOfCore(source, value.ToString(), startIndex, count, options);
		}

		public virtual int LastIndexOf(string source, string value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None && options != CompareOptions.Ordinal && options != CompareOptions.OrdinalIgnoreCase)
			{
				throw new ArgumentException("Value of flags is invalid.", "options");
			}
			if (source.Length == 0 && (startIndex == -1 || startIndex == 0))
			{
				if (value.Length != 0)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (startIndex < 0 || startIndex > source.Length)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (startIndex == source.Length)
				{
					startIndex--;
					if (count > 0)
					{
						count--;
					}
					if (value.Length == 0 && count >= 0 && startIndex - count + 1 >= 0)
					{
						return startIndex;
					}
				}
				if (count < 0 || startIndex - count + 1 < 0)
				{
					throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the string/array/collection.");
				}
				if (options == CompareOptions.OrdinalIgnoreCase)
				{
					return this.LastIndexOfOrdinal(source, value, startIndex, count, true);
				}
				if (GlobalizationMode.Invariant)
				{
					return CompareInfo.InvariantLastIndexOf(source, value, startIndex, count, (options & (CompareOptions.IgnoreCase | CompareOptions.OrdinalIgnoreCase)) > CompareOptions.None);
				}
				return this.LastIndexOfCore(source, value, startIndex, count, options);
			}
		}

		internal int LastIndexOfOrdinal(string source, string value, int startIndex, int count, bool ignoreCase)
		{
			if (GlobalizationMode.Invariant)
			{
				return CompareInfo.InvariantLastIndexOf(source, value, startIndex, count, ignoreCase);
			}
			return CompareInfo.LastIndexOfOrdinalCore(source, value, startIndex, count, ignoreCase);
		}

		public virtual SortKey GetSortKey(string source, CompareOptions options)
		{
			if (GlobalizationMode.Invariant)
			{
				return this.InvariantCreateSortKey(source, options);
			}
			return this.CreateSortKey(source, options);
		}

		public virtual SortKey GetSortKey(string source)
		{
			if (GlobalizationMode.Invariant)
			{
				return this.InvariantCreateSortKey(source, CompareOptions.None);
			}
			return this.CreateSortKey(source, CompareOptions.None);
		}

		public override bool Equals(object value)
		{
			CompareInfo compareInfo = value as CompareInfo;
			return compareInfo != null && this.Name == compareInfo.Name;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		internal unsafe static int GetIgnoreCaseHash(string source)
		{
			if (source.Length == 0)
			{
				return source.GetHashCode();
			}
			char[] array = null;
			Span<char> span;
			if (source.Length <= 255)
			{
				span = new Span<char>(stackalloc byte[(UIntPtr)510], 255);
			}
			else
			{
				span = (array = ArrayPool<char>.Shared.Rent(source.Length));
			}
			Span<char> span2 = span;
			int num = source.AsSpan().ToUpperInvariant(span2);
			int num2 = Marvin.ComputeHash32(MemoryMarshal.AsBytes<char>(span2.Slice(0, num)), Marvin.DefaultSeed);
			if (array != null)
			{
				ArrayPool<char>.Shared.Return(array, false);
			}
			return num2;
		}

		internal int GetHashCodeOfString(string source, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None)
			{
				throw new ArgumentException("Value of flags is invalid.", "options");
			}
			if (!GlobalizationMode.Invariant)
			{
				return this.GetHashCodeOfStringCore(source, options);
			}
			if ((options & CompareOptions.IgnoreCase) == CompareOptions.None)
			{
				return source.GetHashCode();
			}
			return CompareInfo.GetIgnoreCaseHash(source);
		}

		public virtual int GetHashCode(string source, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (options == CompareOptions.Ordinal)
			{
				return source.GetHashCode();
			}
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return CompareInfo.GetIgnoreCaseHash(source);
			}
			return this.GetHashCodeOfString(source, options);
		}

		public override string ToString()
		{
			return "CompareInfo - " + this.Name;
		}

		public SortVersion Version
		{
			get
			{
				if (this.m_SortVersion == null)
				{
					if (GlobalizationMode.Invariant)
					{
						this.m_SortVersion = new SortVersion(0, 127, new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 127));
					}
					else
					{
						this.m_SortVersion = this.GetSortVersion();
					}
				}
				return this.m_SortVersion;
			}
		}

		public int LCID
		{
			get
			{
				return CultureInfo.GetCultureInfo(this.Name).LCID;
			}
		}

		private static bool UseManagedCollation
		{
			get
			{
				if (!CompareInfo.managedCollationChecked)
				{
					CompareInfo.managedCollation = Environment.internalGetEnvironmentVariable("MONO_DISABLE_MANAGED_COLLATION") != "yes" && MSCompatUnicodeTable.IsReady;
					CompareInfo.managedCollationChecked = true;
				}
				return CompareInfo.managedCollation;
			}
		}

		private ISimpleCollator GetCollator()
		{
			if (this.collator != null)
			{
				return this.collator;
			}
			if (CompareInfo.collators == null)
			{
				Interlocked.CompareExchange<Dictionary<string, ISimpleCollator>>(ref CompareInfo.collators, new Dictionary<string, ISimpleCollator>(StringComparer.Ordinal), null);
			}
			Dictionary<string, ISimpleCollator> dictionary = CompareInfo.collators;
			lock (dictionary)
			{
				if (!CompareInfo.collators.TryGetValue(this._sortName, out this.collator))
				{
					this.collator = new SimpleCollator(CultureInfo.GetCultureInfo(this.m_name));
					CompareInfo.collators[this._sortName] = this.collator;
				}
			}
			return this.collator;
		}

		private SortKey CreateSortKeyCore(string source, CompareOptions options)
		{
			if (CompareInfo.UseManagedCollation)
			{
				return this.GetCollator().GetSortKey(source, options);
			}
			return new SortKey(this.culture, source, options);
		}

		private int internal_index_switch(string s1, int sindex, int count, string s2, CompareOptions opt, bool first)
		{
			if (opt == CompareOptions.Ordinal)
			{
				if (!first)
				{
					return s1.LastIndexOfUnchecked(s2, sindex, count);
				}
				return s1.IndexOfUnchecked(s2, sindex, count);
			}
			else
			{
				if (!CompareInfo.UseManagedCollation)
				{
					return CompareInfo.internal_index(s1, sindex, count, s2, first);
				}
				return this.internal_index_managed(s1, sindex, count, s2, opt, first);
			}
		}

		private int internal_compare_switch(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options)
		{
			if (!CompareInfo.UseManagedCollation)
			{
				return CompareInfo.internal_compare(str1, offset1, length1, str2, offset2, length2, options);
			}
			return this.internal_compare_managed(str1, offset1, length1, str2, offset2, length2, options);
		}

		private int internal_compare_managed(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options)
		{
			return this.GetCollator().Compare(str1, offset1, length1, str2, offset2, length2, options);
		}

		private int internal_index_managed(string s, int sindex, int count, char c, CompareOptions opt, bool first)
		{
			if (!first)
			{
				return this.GetCollator().LastIndexOf(s, c, sindex, count, opt);
			}
			return this.GetCollator().IndexOf(s, c, sindex, count, opt);
		}

		private int internal_index_managed(string s1, int sindex, int count, string s2, CompareOptions opt, bool first)
		{
			if (!first)
			{
				return this.GetCollator().LastIndexOf(s1, s2, sindex, count, opt);
			}
			return this.GetCollator().IndexOf(s1, s2, sindex, count, opt);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int internal_compare_icall(char* str1, int length1, char* str2, int length2, CompareOptions options);

		private unsafe static int internal_compare(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options)
		{
			char* ptr = str1;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = str2;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return CompareInfo.internal_compare_icall(ptr + offset1, length1, ptr2 + offset2, length2, options);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int internal_index_icall(char* source, int sindex, int count, char* value, int value_length, bool first);

		private unsafe static int internal_index(string source, int sindex, int count, string value, bool first)
		{
			char* ptr = source;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = value;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return CompareInfo.internal_index_icall(ptr, sindex, count, ptr2, (value != null) ? value.Length : 0, first);
		}

		private void InitSort(CultureInfo culture)
		{
			this._sortName = culture.SortName;
		}

		private unsafe static int CompareStringOrdinalIgnoreCase(char* pString1, int length1, char* pString2, int length2)
		{
			TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
			int num = 0;
			while (num < length1 && num < length2 && textInfo.ToUpper(*pString1) == textInfo.ToUpper(*pString2))
			{
				num++;
				pString1++;
				pString2++;
			}
			if (num >= length1)
			{
				if (num >= length2)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (num >= length2)
				{
					return 1;
				}
				return (int)(textInfo.ToUpper(*pString1) - textInfo.ToUpper(*pString2));
			}
		}

		internal static int IndexOfOrdinalCore(string source, string value, int startIndex, int count, bool ignoreCase)
		{
			if (!ignoreCase)
			{
				return source.IndexOfUnchecked(value, startIndex, count);
			}
			return source.IndexOfUncheckedIgnoreCase(value, startIndex, count);
		}

		internal static int LastIndexOfOrdinalCore(string source, string value, int startIndex, int count, bool ignoreCase)
		{
			if (!ignoreCase)
			{
				return source.LastIndexOfUnchecked(value, startIndex, count);
			}
			return source.LastIndexOfUncheckedIgnoreCase(value, startIndex, count);
		}

		private int LastIndexOfCore(string source, string target, int startIndex, int count, CompareOptions options)
		{
			return this.internal_index_switch(source, startIndex, count, target, options, false);
		}

		private unsafe int IndexOfCore(string source, string target, int startIndex, int count, CompareOptions options, int* matchLengthPtr)
		{
			if (matchLengthPtr != null)
			{
				*matchLengthPtr = target.Length;
			}
			return this.internal_index_switch(source, startIndex, count, target, options, true);
		}

		private unsafe int IndexOfCore(ReadOnlySpan<char> source, ReadOnlySpan<char> target, CompareOptions options, int* matchLengthPtr)
		{
			string text = new string(source);
			string text2 = new string(target);
			return this.IndexOfCore(text, text2, 0, text.Length, options, matchLengthPtr);
		}

		private int IndexOfOrdinalCore(ReadOnlySpan<char> source, ReadOnlySpan<char> value, bool ignoreCase)
		{
			string text = new string(source);
			string text2 = new string(value);
			if (!ignoreCase)
			{
				return text.IndexOfUnchecked(text2, 0, text.Length);
			}
			return text.IndexOfUncheckedIgnoreCase(text2, 0, text.Length);
		}

		private int CompareString(ReadOnlySpan<char> string1, string string2, CompareOptions options)
		{
			string text = new string(string1);
			return this.internal_compare_switch(text, 0, text.Length, string2, 0, string2.Length, options);
		}

		private int CompareString(ReadOnlySpan<char> string1, ReadOnlySpan<char> string2, CompareOptions options)
		{
			string text = new string(string1);
			string text2 = new string(string2);
			return this.internal_compare_switch(text, 0, text.Length, new string(text2), 0, text2.Length, options);
		}

		private unsafe static bool IsSortable(char* text, int length)
		{
			return MSCompatUnicodeTable.IsSortable(new string(text, 0, length));
		}

		private SortKey CreateSortKey(string source, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort)) != CompareOptions.None)
			{
				throw new ArgumentException("Value of flags is invalid.", "options");
			}
			return this.CreateSortKeyCore(source, options);
		}

		private bool StartsWith(string source, string prefix, CompareOptions options)
		{
			if (CompareInfo.UseManagedCollation)
			{
				return this.GetCollator().IsPrefix(source, prefix, options);
			}
			return source.Length >= prefix.Length && this.Compare(source, 0, prefix.Length, prefix, 0, prefix.Length, options) == 0;
		}

		private bool StartsWith(ReadOnlySpan<char> source, ReadOnlySpan<char> prefix, CompareOptions options)
		{
			return this.StartsWith(new string(source), new string(prefix), options);
		}

		private bool EndsWith(string source, string suffix, CompareOptions options)
		{
			if (CompareInfo.UseManagedCollation)
			{
				return this.GetCollator().IsSuffix(source, suffix, options);
			}
			return source.Length >= suffix.Length && this.Compare(source, source.Length - suffix.Length, suffix.Length, suffix, 0, suffix.Length, options) == 0;
		}

		private bool EndsWith(ReadOnlySpan<char> source, ReadOnlySpan<char> suffix, CompareOptions options)
		{
			return this.EndsWith(new string(source), new string(suffix), options);
		}

		internal int GetHashCodeOfStringCore(string source, CompareOptions options)
		{
			return this.GetSortKey(source, options).GetHashCode();
		}

		private SortVersion GetSortVersion()
		{
			throw new NotImplementedException();
		}

		internal CompareInfo()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private const CompareOptions ValidIndexMaskOffFlags = ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);

		private const CompareOptions ValidCompareMaskOffFlags = ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort);

		private const CompareOptions ValidHashCodeOfStringMaskOffFlags = ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);

		private const CompareOptions ValidSortkeyCtorMaskOffFlags = ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort);

		internal static readonly CompareInfo Invariant = CultureInfo.InvariantCulture.CompareInfo;

		[OptionalField(VersionAdded = 2)]
		private string m_name;

		[NonSerialized]
		private string _sortName;

		[OptionalField(VersionAdded = 3)]
		private SortVersion m_SortVersion;

		private int culture;

		[NonSerialized]
		private ISimpleCollator collator;

		private static Dictionary<string, ISimpleCollator> collators;

		private static bool managedCollation;

		private static bool managedCollationChecked;
	}
}
