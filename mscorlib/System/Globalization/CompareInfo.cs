using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
using Mono.Globalization.Unicode;
using Unity;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public class CompareInfo : IDeserializationCallback
	{
		internal CompareInfo(CultureInfo culture)
		{
			this.m_name = culture.m_name;
			this.m_sortName = culture.SortName;
		}

		public static CompareInfo GetCompareInfo(int culture, Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly != typeof(object).Module.Assembly)
			{
				throw new ArgumentException(Environment.GetResourceString("Only mscorlib's assembly is valid."));
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
				throw new ArgumentException(Environment.GetResourceString("Only mscorlib's assembly is valid."));
			}
			return CompareInfo.GetCompareInfo(name);
		}

		public static CompareInfo GetCompareInfo(int culture)
		{
			if (CultureData.IsCustomCultureId(culture))
			{
				throw new ArgumentException(Environment.GetResourceString("Customized cultures cannot be passed by LCID, only by name.", new object[] { "culture" }));
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

		[ComVisible(false)]
		public static bool IsSortable(char ch)
		{
			return CompareInfo.IsSortable(ch.ToString());
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public static bool IsSortable(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			return text.Length != 0 && MSCompatUnicodeTable.IsSortable(text);
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext ctx)
		{
			this.m_name = null;
		}

		private void OnDeserialized()
		{
			CultureInfo cultureInfo;
			if (this.m_name == null)
			{
				cultureInfo = CultureInfo.GetCultureInfo(this.culture);
				this.m_name = cultureInfo.m_name;
			}
			else
			{
				cultureInfo = CultureInfo.GetCultureInfo(this.m_name);
			}
			this.m_sortName = cultureInfo.SortName;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			this.OnDeserialized();
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext ctx)
		{
			this.culture = CultureInfo.GetCultureInfo(this.Name).LCID;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			this.OnDeserialized();
		}

		[ComVisible(false)]
		public virtual string Name
		{
			get
			{
				if (this.m_name == "zh-CHT" || this.m_name == "zh-CHS")
				{
					return this.m_name;
				}
				return this.m_sortName;
			}
		}

		internal static int GetNativeCompareFlags(CompareOptions options)
		{
			int num = 134217728;
			if ((options & CompareOptions.IgnoreCase) != CompareOptions.None)
			{
				num |= 1;
			}
			if ((options & CompareOptions.IgnoreKanaType) != CompareOptions.None)
			{
				num |= 65536;
			}
			if ((options & CompareOptions.IgnoreNonSpace) != CompareOptions.None)
			{
				num |= 2;
			}
			if ((options & CompareOptions.IgnoreSymbols) != CompareOptions.None)
			{
				num |= 4;
			}
			if ((options & CompareOptions.IgnoreWidth) != CompareOptions.None)
			{
				num |= 131072;
			}
			if ((options & CompareOptions.StringSort) != CompareOptions.None)
			{
				num |= 4096;
			}
			if (options == CompareOptions.Ordinal)
			{
				num = 1073741824;
			}
			return num;
		}

		public virtual int Compare(string string1, string string2)
		{
			return this.Compare(string1, string2, CompareOptions.None);
		}

		[SecuritySafeCritical]
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
					throw new ArgumentException(Environment.GetResourceString("CompareOption.Ordinal cannot be used with other options."), "options");
				}
				return string.CompareOrdinal(string1, string2);
			}
			else
			{
				if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort)) != CompareOptions.None)
				{
					throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
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
					return this.internal_compare_switch(string1, 0, string1.Length, string2, 0, string2.Length, options);
				}
			}
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

		[SecuritySafeCritical]
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
					throw new ArgumentOutOfRangeException((length1 < 0) ? "length1" : "length2", Environment.GetResourceString("Positive number required."));
				}
				if (offset1 < 0 || offset2 < 0)
				{
					throw new ArgumentOutOfRangeException((offset1 < 0) ? "offset1" : "offset2", Environment.GetResourceString("Positive number required."));
				}
				if (offset1 > ((string1 == null) ? 0 : string1.Length) - length1)
				{
					throw new ArgumentOutOfRangeException("string1", Environment.GetResourceString("Offset and length must refer to a position in the string."));
				}
				if (offset2 > ((string2 == null) ? 0 : string2.Length) - length2)
				{
					throw new ArgumentOutOfRangeException("string2", Environment.GetResourceString("Offset and length must refer to a position in the string."));
				}
				if ((options & CompareOptions.Ordinal) != CompareOptions.None)
				{
					if (options != CompareOptions.Ordinal)
					{
						throw new ArgumentException(Environment.GetResourceString("CompareOption.Ordinal cannot be used with other options."), "options");
					}
				}
				else if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort)) != CompareOptions.None)
				{
					throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
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
					if (options == CompareOptions.Ordinal)
					{
						return CompareInfo.CompareOrdinal(string1, offset1, length1, string2, offset2, length2);
					}
					return this.internal_compare_switch(string1, offset1, length1, string2, offset2, length2, options);
				}
			}
		}

		[SecurityCritical]
		private static int CompareOrdinal(string string1, int offset1, int length1, string string2, int offset2, int length2)
		{
			int num = string.nativeCompareOrdinalEx(string1, offset1, string2, offset2, (length1 < length2) ? length1 : length2);
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

		[SecuritySafeCritical]
		public virtual bool IsPrefix(string source, string prefix, CompareOptions options)
		{
			if (source == null || prefix == null)
			{
				throw new ArgumentNullException((source == null) ? "source" : "prefix", Environment.GetResourceString("String reference not set to an instance of a String."));
			}
			if (prefix.Length == 0)
			{
				return true;
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
				throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
			}
			if (CompareInfo.UseManagedCollation)
			{
				return this.GetCollator().IsPrefix(source, prefix, options);
			}
			return source.Length >= prefix.Length && this.Compare(source, 0, prefix.Length, prefix, 0, prefix.Length, options) == 0;
		}

		public virtual bool IsPrefix(string source, string prefix)
		{
			return this.IsPrefix(source, prefix, CompareOptions.None);
		}

		[SecuritySafeCritical]
		public virtual bool IsSuffix(string source, string suffix, CompareOptions options)
		{
			if (source == null || suffix == null)
			{
				throw new ArgumentNullException((source == null) ? "source" : "suffix", Environment.GetResourceString("String reference not set to an instance of a String."));
			}
			if (suffix.Length == 0)
			{
				return true;
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
				throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
			}
			if (CompareInfo.UseManagedCollation)
			{
				return this.GetCollator().IsSuffix(source, suffix, options);
			}
			return source.Length >= suffix.Length && this.Compare(source, source.Length - suffix.Length, suffix.Length, suffix, 0, suffix.Length, options) == 0;
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

		[SecuritySafeCritical]
		public virtual int IndexOf(string source, char value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (startIndex < 0 || startIndex > source.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
			}
			if (count < 0 || startIndex > source.Length - count)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Count must be positive and count must refer to a location within the string/array/collection."));
			}
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return source.IndexOf(value.ToString(), startIndex, count, StringComparison.OrdinalIgnoreCase);
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None && options != CompareOptions.Ordinal)
			{
				throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
			}
			return this.internal_index_switch(source, startIndex, count, value, options, true);
		}

		[SecuritySafeCritical]
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
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
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
					throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
				}
				if (count < 0 || startIndex > source.Length - count)
				{
					throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Count must be positive and count must refer to a location within the string/array/collection."));
				}
				if (options == CompareOptions.OrdinalIgnoreCase)
				{
					return source.IndexOf(value, startIndex, count, StringComparison.OrdinalIgnoreCase);
				}
				if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None && options != CompareOptions.Ordinal)
				{
					throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
				}
				return this.internal_index_switch(source, startIndex, count, value, options, true);
			}
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

		[SecuritySafeCritical]
		public virtual int LastIndexOf(string source, char value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None && options != CompareOptions.Ordinal && options != CompareOptions.OrdinalIgnoreCase)
			{
				throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
			}
			if (source.Length == 0 && (startIndex == -1 || startIndex == 0))
			{
				return -1;
			}
			if (startIndex < 0 || startIndex > source.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
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
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Count must be positive and count must refer to a location within the string/array/collection."));
			}
			if (options == CompareOptions.OrdinalIgnoreCase)
			{
				return source.LastIndexOf(value.ToString(), startIndex, count, StringComparison.OrdinalIgnoreCase);
			}
			return this.internal_index_switch(source, startIndex, count, value, options, false);
		}

		[SecuritySafeCritical]
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
				throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
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
					throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
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
					throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Count must be positive and count must refer to a location within the string/array/collection."));
				}
				if (options == CompareOptions.OrdinalIgnoreCase)
				{
					return source.LastIndexOf(value, startIndex, count, StringComparison.OrdinalIgnoreCase);
				}
				return this.internal_index_switch(source, startIndex, count, value, options, false);
			}
		}

		public virtual SortKey GetSortKey(string source, CompareOptions options)
		{
			return this.CreateSortKey(source, options);
		}

		public virtual SortKey GetSortKey(string source)
		{
			return this.CreateSortKey(source, CompareOptions.None);
		}

		[SecuritySafeCritical]
		private SortKey CreateSortKey(string source, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort)) != CompareOptions.None)
			{
				throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
			}
			if (string.IsNullOrEmpty(source))
			{
				source = "\0";
			}
			return this.CreateSortKeyCore(source, options);
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
				return TextInfo.GetHashCodeOrdinalIgnoreCase(source);
			}
			return this.GetHashCodeOfString(source, options, false, 0L);
		}

		internal int GetHashCodeOfString(string source, CompareOptions options)
		{
			return this.GetHashCodeOfString(source, options, false, 0L);
		}

		[SecuritySafeCritical]
		internal int GetHashCodeOfString(string source, CompareOptions options, bool forceRandomizedHashing, long additionalEntropy)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if ((options & ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)) != CompareOptions.None)
			{
				throw new ArgumentException(Environment.GetResourceString("Value of flags is invalid."), "options");
			}
			if (source.Length == 0)
			{
				return 0;
			}
			return this.GetSortKey(source, options).GetHashCode();
		}

		public override string ToString()
		{
			return "CompareInfo - " + this.Name;
		}

		public int LCID
		{
			get
			{
				return CultureInfo.GetCultureInfo(this.Name).LCID;
			}
		}

		internal static bool IsLegacy20SortingBehaviorRequested
		{
			get
			{
				return CompareInfo.InternalSortVersion == 4096U;
			}
		}

		private static uint InternalSortVersion
		{
			[SecuritySafeCritical]
			get
			{
				return 393473U;
			}
		}

		public SortVersion Version
		{
			[SecuritySafeCritical]
			get
			{
				if (this.m_SortVersion == null)
				{
					this.m_SortVersion = new SortVersion(393473, new Guid("00000001-57ee-1e5c-00b4-d0000bb1e11e"));
				}
				return this.m_SortVersion;
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

		private SimpleCollator GetCollator()
		{
			if (this.collator != null)
			{
				return this.collator;
			}
			if (CompareInfo.collators == null)
			{
				Interlocked.CompareExchange<Dictionary<string, SimpleCollator>>(ref CompareInfo.collators, new Dictionary<string, SimpleCollator>(StringComparer.Ordinal), null);
			}
			Dictionary<string, SimpleCollator> dictionary = CompareInfo.collators;
			lock (dictionary)
			{
				if (!CompareInfo.collators.TryGetValue(this.m_sortName, out this.collator))
				{
					this.collator = new SimpleCollator(CultureInfo.GetCultureInfo(this.m_name));
					CompareInfo.collators[this.m_sortName] = this.collator;
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
			SortKey sortKey = new SortKey(this.culture, source, options);
			this.assign_sortkey(sortKey, source, options);
			return sortKey;
		}

		private int internal_index_switch(string s, int sindex, int count, char c, CompareOptions opt, bool first)
		{
			if (opt == CompareOptions.Ordinal && first)
			{
				return s.IndexOfUnchecked(c, sindex, count);
			}
			if (!CompareInfo.UseManagedCollation)
			{
				return this.internal_index(s, sindex, count, c, opt, first);
			}
			return this.internal_index_managed(s, sindex, count, c, opt, first);
		}

		private int internal_index_switch(string s1, int sindex, int count, string s2, CompareOptions opt, bool first)
		{
			if (opt == CompareOptions.Ordinal && first)
			{
				return s1.IndexOfUnchecked(s2, sindex, count);
			}
			if (!CompareInfo.UseManagedCollation)
			{
				return this.internal_index(s1, sindex, count, s2, opt, first);
			}
			return this.internal_index_managed(s1, sindex, count, s2, opt, first);
		}

		private int internal_compare_switch(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options)
		{
			if (!CompareInfo.UseManagedCollation)
			{
				return this.internal_compare(str1, offset1, length1, str2, offset2, length2, options);
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
		private extern void assign_sortkey(object key, string source, CompareOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int internal_compare(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int internal_index(string source, int sindex, int count, char value, CompareOptions options, bool first);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int internal_index(string source, int sindex, int count, string value, CompareOptions options, bool first);

		internal CompareInfo()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private const CompareOptions ValidIndexMaskOffFlags = ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);

		private const CompareOptions ValidCompareMaskOffFlags = ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort);

		private const CompareOptions ValidHashCodeOfStringMaskOffFlags = ~(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);

		[OptionalField(VersionAdded = 2)]
		private string m_name;

		[NonSerialized]
		private string m_sortName;

		[OptionalField(VersionAdded = 1)]
		private int win32LCID;

		private int culture;

		private const int LINGUISTIC_IGNORECASE = 16;

		private const int NORM_IGNORECASE = 1;

		private const int NORM_IGNOREKANATYPE = 65536;

		private const int LINGUISTIC_IGNOREDIACRITIC = 32;

		private const int NORM_IGNORENONSPACE = 2;

		private const int NORM_IGNORESYMBOLS = 4;

		private const int NORM_IGNOREWIDTH = 131072;

		private const int SORT_STRINGSORT = 4096;

		private const int COMPARE_OPTIONS_ORDINAL = 1073741824;

		internal const int NORM_LINGUISTIC_CASING = 134217728;

		private const int RESERVED_FIND_ASCII_STRING = 536870912;

		private const int SORT_VERSION_WHIDBEY = 4096;

		private const int SORT_VERSION_V4 = 393473;

		[OptionalField(VersionAdded = 3)]
		private SortVersion m_SortVersion;

		[NonSerialized]
		private SimpleCollator collator;

		private static Dictionary<string, SimpleCollator> collators;

		private static bool managedCollation;

		private static bool managedCollationChecked;
	}
}
