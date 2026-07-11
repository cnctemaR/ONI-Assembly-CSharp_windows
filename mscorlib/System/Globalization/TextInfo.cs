using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Globalization
{
	[ComVisible(true)]
	[MonoTODO("IDeserializationCallback isn't implemented.")]
	[Serializable]
	public class TextInfo : ICloneable, IDeserializationCallback
	{
		internal unsafe TextInfo(CultureInfo ci, int lcid, void* data, bool read_only)
		{
			this.m_isReadOnly = read_only;
			this.m_win32LangID = lcid;
			this.ci = ci;
			if (data != null)
			{
				this.data = *(TextInfo.Data*)data;
			}
			else
			{
				this.data = default(TextInfo.Data);
				this.data.list_sep = 44;
			}
			CultureInfo cultureInfo = ci;
			while (cultureInfo.Parent != null && cultureInfo.Parent.LCID != 127 && cultureInfo.Parent != cultureInfo)
			{
				cultureInfo = cultureInfo.Parent;
			}
			if (cultureInfo != null)
			{
				int lcid2 = cultureInfo.LCID;
				if (lcid2 == 31 || lcid2 == 44)
				{
					this.handleDotI = true;
				}
			}
		}

		private TextInfo(TextInfo textInfo)
		{
			this.m_win32LangID = textInfo.m_win32LangID;
			this.m_nDataItem = textInfo.m_nDataItem;
			this.m_useUserOverride = textInfo.m_useUserOverride;
			this.m_listSeparator = textInfo.ListSeparator;
			this.customCultureName = textInfo.CultureName;
			this.ci = textInfo.ci;
			this.handleDotI = textInfo.handleDotI;
			this.data = textInfo.data;
		}

		[MonoTODO]
		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		public virtual int ANSICodePage
		{
			get
			{
				return this.data.ansi;
			}
		}

		public virtual int EBCDICCodePage
		{
			get
			{
				return this.data.ebcdic;
			}
		}

		[ComVisible(false)]
		public int LCID
		{
			get
			{
				return this.m_win32LangID;
			}
		}

		public virtual string ListSeparator
		{
			get
			{
				if (this.m_listSeparator == null)
				{
					this.m_listSeparator = ((char)this.data.list_sep).ToString();
				}
				return this.m_listSeparator;
			}
			[ComVisible(false)]
			set
			{
				this.m_listSeparator = value;
			}
		}

		public virtual int MacCodePage
		{
			get
			{
				return this.data.mac;
			}
		}

		public virtual int OEMCodePage
		{
			get
			{
				return this.data.oem;
			}
		}

		[ComVisible(false)]
		public string CultureName
		{
			get
			{
				if (this.customCultureName == null)
				{
					this.customCultureName = this.ci.Name;
				}
				return this.customCultureName;
			}
		}

		[ComVisible(false)]
		public bool IsReadOnly
		{
			get
			{
				return this.m_isReadOnly;
			}
		}

		[ComVisible(false)]
		public bool IsRightToLeft
		{
			get
			{
				int win32LangID = this.m_win32LangID;
				return win32LangID == 1 || win32LangID == 13 || win32LangID == 32 || win32LangID == 41 || win32LangID == 90 || win32LangID == 101 || win32LangID == 1025 || win32LangID == 1037 || win32LangID == 1056 || win32LangID == 1065 || win32LangID == 1114 || win32LangID == 1125 || win32LangID == 2049 || win32LangID == 3073 || win32LangID == 4097 || win32LangID == 5121 || win32LangID == 6145 || win32LangID == 7169 || win32LangID == 8193 || win32LangID == 9217 || win32LangID == 10241 || win32LangID == 11265 || win32LangID == 12289 || win32LangID == 13313 || win32LangID == 14337 || win32LangID == 15361 || win32LangID == 16385;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			TextInfo textInfo = obj as TextInfo;
			return textInfo != null && textInfo.m_win32LangID == this.m_win32LangID && textInfo.ci == this.ci;
		}

		public override int GetHashCode()
		{
			return this.m_win32LangID;
		}

		public override string ToString()
		{
			return "TextInfo - " + this.m_win32LangID;
		}

		public string ToTitleCase(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			StringBuilder stringBuilder = null;
			int i = 0;
			int num = 0;
			while (i < str.Length)
			{
				if (char.IsLetter(str[i++]))
				{
					i--;
					char c = this.ToTitleCase(str[i]);
					bool flag = true;
					if (c == str[i])
					{
						flag = false;
						bool flag2 = true;
						int num2 = i;
						while (++i < str.Length)
						{
							if (char.IsWhiteSpace(str[i]))
							{
								break;
							}
							c = this.ToTitleCase(str[i]);
							if (c != str[i])
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							continue;
						}
						i = num2;
						while (++i < str.Length)
						{
							if (char.IsWhiteSpace(str[i]))
							{
								break;
							}
							if (this.ToLower(str[i]) != str[i])
							{
								flag = true;
								i = num2;
								break;
							}
						}
					}
					if (flag)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(str.Length);
						}
						stringBuilder.Append(str, num, i - num);
						stringBuilder.Append(this.ToTitleCase(str[i]));
						num = i + 1;
						while (++i < str.Length)
						{
							if (char.IsWhiteSpace(str[i]))
							{
								break;
							}
							stringBuilder.Append(this.ToLower(str[i]));
						}
						num = i;
					}
				}
			}
			if (stringBuilder != null)
			{
				stringBuilder.Append(str, num, str.Length - num);
			}
			return (stringBuilder == null) ? str : stringBuilder.ToString();
		}

		public virtual char ToLower(char c)
		{
			if (c < '@' || ('`' < c && c < '\u0080'))
			{
				return c;
			}
			if ('A' <= c && c <= 'Z' && (!this.handleDotI || c != 'I'))
			{
				return c + ' ';
			}
			if (this.ci == null || this.ci.LCID == 127)
			{
				return char.ToLowerInvariant(c);
			}
			switch (c)
			{
			case 'ǅ':
				return 'ǆ';
			default:
				switch (c)
				{
				case 'ϒ':
					return 'υ';
				case 'ϓ':
					return 'ύ';
				case 'ϔ':
					return 'ϋ';
				default:
					if (c != 'I')
					{
						if (c == 'İ')
						{
							return 'i';
						}
						if (c == 'ǋ')
						{
							return 'ǌ';
						}
						if (c == 'ǲ')
						{
							return 'ǳ';
						}
					}
					else if (this.handleDotI)
					{
						return 'ı';
					}
					return char.ToLowerInvariant(c);
				}
				break;
			case 'ǈ':
				return 'ǉ';
			}
		}

		public virtual char ToUpper(char c)
		{
			if (c < '`')
			{
				return c;
			}
			if ('a' <= c && c <= 'z' && (!this.handleDotI || c != 'i'))
			{
				return c - ' ';
			}
			if (this.ci == null || this.ci.LCID == 127)
			{
				return char.ToUpperInvariant(c);
			}
			switch (c)
			{
			case 'ϐ':
				return 'Β';
			case 'ϑ':
				return 'Θ';
			default:
				switch (c)
				{
				case 'ǅ':
					return 'Ǆ';
				default:
					if (c == 'ϰ')
					{
						return 'Κ';
					}
					if (c != 'ϱ')
					{
						if (c != 'i')
						{
							if (c == 'ı')
							{
								return 'I';
							}
							if (c == 'ǋ')
							{
								return 'Ǌ';
							}
							if (c == 'ǲ')
							{
								return 'Ǳ';
							}
							if (c == 'ΐ')
							{
								return 'Ϊ';
							}
							if (c == 'ΰ')
							{
								return 'Ϋ';
							}
						}
						else if (this.handleDotI)
						{
							return 'İ';
						}
						return char.ToUpperInvariant(c);
					}
					return 'Ρ';
				case 'ǈ':
					return 'Ǉ';
				}
				break;
			case 'ϕ':
				return 'Φ';
			case 'ϖ':
				return 'Π';
			}
		}

		private char ToTitleCase(char c)
		{
			switch (c)
			{
			case 'Ǆ':
			case 'ǅ':
			case 'ǆ':
				return 'ǅ';
			case 'Ǉ':
			case 'ǈ':
			case 'ǉ':
				return 'ǈ';
			case 'Ǌ':
			case 'ǋ':
			case 'ǌ':
				return 'ǋ';
			default:
				switch (c)
				{
				case 'Ǳ':
				case 'ǲ':
				case 'ǳ':
					return 'ǲ';
				default:
					if (('ⅰ' <= c && c <= 'ⅿ') || ('ⓐ' <= c && c <= 'ⓩ'))
					{
						return c;
					}
					return this.ToUpper(c);
				}
				break;
			}
		}

		public unsafe virtual string ToLower(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(str.Length);
			fixed (string text2 = str)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text3 = text)
					{
						fixed (char* ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							char* ptr3 = ptr2;
							char* ptr4 = ptr;
							for (int i = 0; i < str.Length; i++)
							{
								*ptr3 = this.ToLower(*ptr4);
								ptr4++;
								ptr3++;
							}
							text2 = null;
							text3 = null;
							return text;
						}
					}
				}
			}
		}

		public unsafe virtual string ToUpper(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(str.Length);
			fixed (string text2 = str)
			{
				fixed (char* ptr = text2 + RuntimeHelpers.OffsetToStringData / 2)
				{
					fixed (string text3 = text)
					{
						fixed (char* ptr2 = text3 + RuntimeHelpers.OffsetToStringData / 2)
						{
							char* ptr3 = ptr2;
							char* ptr4 = ptr;
							for (int i = 0; i < str.Length; i++)
							{
								*ptr3 = this.ToUpper(*ptr4);
								ptr4++;
								ptr3++;
							}
							text2 = null;
							text3 = null;
							return text;
						}
					}
				}
			}
		}

		[ComVisible(false)]
		public static TextInfo ReadOnly(TextInfo textInfo)
		{
			if (textInfo == null)
			{
				throw new ArgumentNullException("textInfo");
			}
			return new TextInfo(textInfo)
			{
				m_isReadOnly = true
			};
		}

		[ComVisible(false)]
		public virtual object Clone()
		{
			return new TextInfo(this);
		}

		private string m_listSeparator;

		private bool m_isReadOnly;

		private string customCultureName;

		[NonSerialized]
		private int m_nDataItem;

		private bool m_useUserOverride;

		private int m_win32LangID;

		[NonSerialized]
		private readonly CultureInfo ci;

		[NonSerialized]
		private readonly bool handleDotI;

		[NonSerialized]
		private readonly TextInfo.Data data;

		private struct Data
		{
			public int ansi;

			public int ebcdic;

			public int mac;

			public int oem;

			public byte list_sep;
		}
	}
}
