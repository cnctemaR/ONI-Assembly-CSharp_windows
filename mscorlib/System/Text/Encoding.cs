using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Text
{
	[ComVisible(true)]
	[Serializable]
	public abstract class Encoding : ICloneable
	{
		protected Encoding()
		{
		}

		protected Encoding(int codePage)
		{
			this.windows_code_page = codePage;
			this.codePage = codePage;
			if (codePage != 1200 && codePage != 1201 && codePage != 12000 && codePage != 12001 && codePage != 65000 && codePage != 65001)
			{
				if (codePage != 20127 && codePage != 54936)
				{
					this.decoder_fallback = DecoderFallback.ReplacementFallback;
					this.encoder_fallback = EncoderFallback.ReplacementFallback;
				}
				else
				{
					this.decoder_fallback = DecoderFallback.ReplacementFallback;
					this.encoder_fallback = EncoderFallback.ReplacementFallback;
				}
			}
			else
			{
				this.decoder_fallback = DecoderFallback.StandardSafeFallback;
				this.encoder_fallback = EncoderFallback.StandardSafeFallback;
			}
		}

		internal static string _(string arg)
		{
			return arg;
		}

		[ComVisible(false)]
		public bool IsReadOnly
		{
			get
			{
				return this.is_readonly;
			}
		}

		[ComVisible(false)]
		public virtual bool IsSingleByte
		{
			get
			{
				return false;
			}
		}

		[ComVisible(false)]
		public DecoderFallback DecoderFallback
		{
			get
			{
				return this.decoder_fallback;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException("This Encoding is readonly.");
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.decoder_fallback = value;
			}
		}

		[ComVisible(false)]
		public EncoderFallback EncoderFallback
		{
			get
			{
				return this.encoder_fallback;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException("This Encoding is readonly.");
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.encoder_fallback = value;
			}
		}

		internal void SetFallbackInternal(EncoderFallback e, DecoderFallback d)
		{
			if (e != null)
			{
				this.encoder_fallback = e;
			}
			if (d != null)
			{
				this.decoder_fallback = d;
			}
		}

		public static byte[] Convert(Encoding srcEncoding, Encoding dstEncoding, byte[] bytes)
		{
			if (srcEncoding == null)
			{
				throw new ArgumentNullException("srcEncoding");
			}
			if (dstEncoding == null)
			{
				throw new ArgumentNullException("dstEncoding");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return dstEncoding.GetBytes(srcEncoding.GetChars(bytes, 0, bytes.Length));
		}

		public static byte[] Convert(Encoding srcEncoding, Encoding dstEncoding, byte[] bytes, int index, int count)
		{
			if (srcEncoding == null)
			{
				throw new ArgumentNullException("srcEncoding");
			}
			if (dstEncoding == null)
			{
				throw new ArgumentNullException("dstEncoding");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", Encoding._("ArgRange_Array"));
			}
			if (count < 0 || bytes.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count", Encoding._("ArgRange_Array"));
			}
			return dstEncoding.GetBytes(srcEncoding.GetChars(bytes, index, count));
		}

		public override bool Equals(object value)
		{
			Encoding encoding = value as Encoding;
			return encoding != null && (this.codePage == encoding.codePage && this.DecoderFallback.Equals(encoding.DecoderFallback)) && this.EncoderFallback.Equals(encoding.EncoderFallback);
		}

		public abstract int GetByteCount(char[] chars, int index, int count);

		public unsafe virtual int GetByteCount(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return 0;
			}
			fixed (char* ptr = s + RuntimeHelpers.OffsetToStringData / 2)
			{
				return this.GetByteCount(ptr, s.Length);
			}
		}

		public virtual int GetByteCount(char[] chars)
		{
			if (chars != null)
			{
				return this.GetByteCount(chars, 0, chars.Length);
			}
			throw new ArgumentNullException("chars");
		}

		public abstract int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex);

		public unsafe virtual int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (charIndex < 0 || charIndex > s.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_Array"));
			}
			if (charCount < 0 || charIndex > s.Length - charCount)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_Array"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			if (charCount == 0 || bytes.Length == byteIndex)
			{
				return 0;
			}
			fixed (char* ptr = s + RuntimeHelpers.OffsetToStringData / 2)
			{
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					return this.GetBytes(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex);
				}
			}
		}

		public unsafe virtual byte[] GetBytes(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return new byte[0];
			}
			int byteCount = this.GetByteCount(s);
			if (byteCount == 0)
			{
				return new byte[0];
			}
			fixed (char* ptr = s + RuntimeHelpers.OffsetToStringData / 2)
			{
				byte[] array = new byte[byteCount];
				fixed (byte* ptr2 = (ref array != null && array.Length != 0 ? ref array[0] : ref *null))
				{
					this.GetBytes(ptr, s.Length, ptr2, byteCount);
					return array;
				}
			}
		}

		public virtual byte[] GetBytes(char[] chars, int index, int count)
		{
			int byteCount = this.GetByteCount(chars, index, count);
			byte[] array = new byte[byteCount];
			this.GetBytes(chars, index, count, array, 0);
			return array;
		}

		public virtual byte[] GetBytes(char[] chars)
		{
			int byteCount = this.GetByteCount(chars, 0, chars.Length);
			byte[] array = new byte[byteCount];
			this.GetBytes(chars, 0, chars.Length, array, 0);
			return array;
		}

		public abstract int GetCharCount(byte[] bytes, int index, int count);

		public virtual int GetCharCount(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return this.GetCharCount(bytes, 0, bytes.Length);
		}

		public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);

		public virtual char[] GetChars(byte[] bytes, int index, int count)
		{
			int charCount = this.GetCharCount(bytes, index, count);
			char[] array = new char[charCount];
			this.GetChars(bytes, index, count, array, 0);
			return array;
		}

		public virtual char[] GetChars(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			int charCount = this.GetCharCount(bytes, 0, bytes.Length);
			char[] array = new char[charCount];
			this.GetChars(bytes, 0, bytes.Length, array, 0);
			return array;
		}

		public virtual Decoder GetDecoder()
		{
			return new Encoding.ForwardingDecoder(this);
		}

		public virtual Encoder GetEncoder()
		{
			return new Encoding.ForwardingEncoder(this);
		}

		private static object InvokeI18N(string name, params object[] args)
		{
			object obj = Encoding.lockobj;
			object obj2;
			lock (obj)
			{
				if (Encoding.i18nDisabled)
				{
					obj2 = null;
				}
				else
				{
					if (Encoding.i18nAssembly == null)
					{
						try
						{
							try
							{
								Encoding.i18nAssembly = Assembly.Load("I18N, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
							}
							catch (NotImplementedException)
							{
								Encoding.i18nDisabled = true;
								return null;
							}
							if (Encoding.i18nAssembly == null)
							{
								return null;
							}
						}
						catch (SystemException)
						{
							return null;
						}
					}
					Type type;
					try
					{
						type = Encoding.i18nAssembly.GetType("I18N.Common.Manager");
					}
					catch (NotImplementedException)
					{
						Encoding.i18nDisabled = true;
						return null;
					}
					if (type == null)
					{
						obj2 = null;
					}
					else
					{
						object obj3;
						try
						{
							obj3 = type.InvokeMember("PrimaryManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, null, null, null, null, null);
							if (obj3 == null)
							{
								return null;
							}
						}
						catch (MissingMethodException)
						{
							return null;
						}
						catch (SecurityException)
						{
							return null;
						}
						catch (NotImplementedException)
						{
							Encoding.i18nDisabled = true;
							return null;
						}
						try
						{
							obj2 = type.InvokeMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj3, args, null, null, null);
						}
						catch (MissingMethodException)
						{
							obj2 = null;
						}
						catch (SecurityException)
						{
							obj2 = null;
						}
					}
				}
			}
			return obj2;
		}

		public static Encoding GetEncoding(int codepage)
		{
			if (codepage < 0 || codepage > 65535)
			{
				throw new ArgumentOutOfRangeException("codepage", "Valid values are between 0 and 65535, inclusive.");
			}
			int num = codepage;
			if (num == 1200)
			{
				return Encoding.Unicode;
			}
			if (num == 1201)
			{
				return Encoding.BigEndianUnicode;
			}
			if (num == 12000)
			{
				return Encoding.UTF32;
			}
			if (num == 12001)
			{
				return Encoding.BigEndianUTF32;
			}
			if (num == 65000)
			{
				return Encoding.UTF7;
			}
			if (num == 65001)
			{
				return Encoding.UTF8;
			}
			if (num == 0)
			{
				return Encoding.Default;
			}
			if (num == 20127)
			{
				return Encoding.ASCII;
			}
			if (num == 28591)
			{
				return Encoding.ISOLatin1;
			}
			Encoding encoding = (Encoding)Encoding.InvokeI18N("GetEncoding", new object[] { codepage });
			if (encoding != null)
			{
				encoding.is_readonly = true;
				return encoding;
			}
			string text = "System.Text.CP" + codepage.ToString();
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType(text);
			if (type != null)
			{
				encoding = (Encoding)Activator.CreateInstance(type);
				encoding.is_readonly = true;
				return encoding;
			}
			type = Type.GetType(text);
			if (type != null)
			{
				encoding = (Encoding)Activator.CreateInstance(type);
				encoding.is_readonly = true;
				return encoding;
			}
			throw new NotSupportedException(string.Format("CodePage {0} not supported", codepage.ToString()));
		}

		[ComVisible(false)]
		public virtual object Clone()
		{
			Encoding encoding = (Encoding)base.MemberwiseClone();
			encoding.is_readonly = false;
			return encoding;
		}

		public static Encoding GetEncoding(int codepage, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
		{
			if (encoderFallback == null)
			{
				throw new ArgumentNullException("encoderFallback");
			}
			if (decoderFallback == null)
			{
				throw new ArgumentNullException("decoderFallback");
			}
			Encoding encoding = Encoding.GetEncoding(codepage).Clone() as Encoding;
			encoding.is_readonly = false;
			encoding.encoder_fallback = encoderFallback;
			encoding.decoder_fallback = decoderFallback;
			return encoding;
		}

		public static Encoding GetEncoding(string name, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
		{
			if (encoderFallback == null)
			{
				throw new ArgumentNullException("encoderFallback");
			}
			if (decoderFallback == null)
			{
				throw new ArgumentNullException("decoderFallback");
			}
			Encoding encoding = Encoding.GetEncoding(name).Clone() as Encoding;
			encoding.is_readonly = false;
			encoding.encoder_fallback = encoderFallback;
			encoding.decoder_fallback = decoderFallback;
			return encoding;
		}

		public static EncodingInfo[] GetEncodings()
		{
			if (Encoding.encoding_infos == null)
			{
				int[] array = new int[]
				{
					37, 437, 500, 708, 850, 852, 855, 857, 858, 860,
					861, 862, 863, 864, 865, 866, 869, 870, 874, 875,
					932, 936, 949, 950, 1026, 1047, 1140, 1141, 1142, 1143,
					1144, 1145, 1146, 1147, 1148, 1149, 1200, 1201, 1250, 1251,
					1252, 1253, 1254, 1255, 1256, 1257, 1258, 10000, 10079, 12000,
					12001, 20127, 20273, 20277, 20278, 20280, 20284, 20285, 20290, 20297,
					20420, 20424, 20866, 20871, 21025, 21866, 28591, 28592, 28593, 28594,
					28595, 28596, 28597, 28598, 28599, 28605, 38598, 50220, 50221, 50222,
					51932, 51949, 54936, 57002, 57003, 57004, 57005, 57006, 57007, 57008,
					57009, 57010, 57011, 65000, 65001
				};
				Encoding.encoding_infos = new EncodingInfo[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					Encoding.encoding_infos[i] = new EncodingInfo(array[i]);
				}
			}
			return Encoding.encoding_infos;
		}

		[ComVisible(false)]
		public bool IsAlwaysNormalized()
		{
			return this.IsAlwaysNormalized(NormalizationForm.FormC);
		}

		[ComVisible(false)]
		public virtual bool IsAlwaysNormalized(NormalizationForm form)
		{
			return form == NormalizationForm.FormC && this is ASCIIEncoding;
		}

		public static Encoding GetEncoding(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			string text = name.ToLowerInvariant().Replace('-', '_');
			int num = 0;
			for (int i = 0; i < Encoding.encodings.Length; i++)
			{
				object obj = Encoding.encodings[i];
				if (obj is int)
				{
					num = (int)obj;
				}
				else if (text == (string)Encoding.encodings[i])
				{
					return Encoding.GetEncoding(num);
				}
			}
			Encoding encoding = (Encoding)Encoding.InvokeI18N("GetEncoding", new object[] { name });
			if (encoding != null)
			{
				return encoding;
			}
			string text2 = "System.Text.ENC" + text;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType(text2);
			if (type != null)
			{
				return (Encoding)Activator.CreateInstance(type);
			}
			type = Type.GetType(text2);
			if (type != null)
			{
				return (Encoding)Activator.CreateInstance(type);
			}
			throw new ArgumentException(string.Format("Encoding name '{0}' not supported", name), "name");
		}

		public override int GetHashCode()
		{
			return this.DecoderFallback.GetHashCode() << 24 + this.EncoderFallback.GetHashCode() << 16 + this.codePage;
		}

		public abstract int GetMaxByteCount(int charCount);

		public abstract int GetMaxCharCount(int byteCount);

		public virtual byte[] GetPreamble()
		{
			return new byte[0];
		}

		public virtual string GetString(byte[] bytes, int index, int count)
		{
			return new string(this.GetChars(bytes, index, count));
		}

		public virtual string GetString(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return this.GetString(bytes, 0, bytes.Length);
		}

		public virtual string BodyName
		{
			get
			{
				return this.body_name;
			}
		}

		public virtual int CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public virtual string EncodingName
		{
			get
			{
				return this.encoding_name;
			}
		}

		public virtual string HeaderName
		{
			get
			{
				return this.header_name;
			}
		}

		public virtual bool IsBrowserDisplay
		{
			get
			{
				return this.is_browser_display;
			}
		}

		public virtual bool IsBrowserSave
		{
			get
			{
				return this.is_browser_save;
			}
		}

		public virtual bool IsMailNewsDisplay
		{
			get
			{
				return this.is_mail_news_display;
			}
		}

		public virtual bool IsMailNewsSave
		{
			get
			{
				return this.is_mail_news_save;
			}
		}

		public virtual string WebName
		{
			get
			{
				return this.web_name;
			}
		}

		public virtual int WindowsCodePage
		{
			get
			{
				return this.windows_code_page;
			}
		}

		public static Encoding ASCII
		{
			get
			{
				if (Encoding.asciiEncoding == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.asciiEncoding == null)
						{
							Encoding.asciiEncoding = new ASCIIEncoding();
						}
					}
				}
				return Encoding.asciiEncoding;
			}
		}

		public static Encoding BigEndianUnicode
		{
			get
			{
				if (Encoding.bigEndianEncoding == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.bigEndianEncoding == null)
						{
							Encoding.bigEndianEncoding = new UnicodeEncoding(true, true);
						}
					}
				}
				return Encoding.bigEndianEncoding;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalCodePage(ref int code_page);

		public static Encoding Default
		{
			get
			{
				if (Encoding.defaultEncoding == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.defaultEncoding == null)
						{
							int num = 1;
							string text = Encoding.InternalCodePage(ref num);
							try
							{
								if (num == -1)
								{
									Encoding.defaultEncoding = Encoding.GetEncoding(text);
								}
								else
								{
									num &= 268435455;
									switch (num)
									{
									case 1:
										num = 20127;
										break;
									case 2:
										num = 65000;
										break;
									case 3:
										num = 65001;
										break;
									case 4:
										num = 1200;
										break;
									case 5:
										num = 1201;
										break;
									case 6:
										num = 28591;
										break;
									}
									Encoding.defaultEncoding = Encoding.GetEncoding(num);
								}
							}
							catch (NotSupportedException)
							{
								Encoding.defaultEncoding = Encoding.UTF8Unmarked;
							}
							catch (ArgumentException)
							{
								Encoding.defaultEncoding = Encoding.UTF8Unmarked;
							}
							Encoding.defaultEncoding.is_readonly = true;
						}
					}
				}
				return Encoding.defaultEncoding;
			}
		}

		private static Encoding ISOLatin1
		{
			get
			{
				if (Encoding.isoLatin1Encoding == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.isoLatin1Encoding == null)
						{
							Encoding.isoLatin1Encoding = new Latin1Encoding();
						}
					}
				}
				return Encoding.isoLatin1Encoding;
			}
		}

		public static Encoding UTF7
		{
			get
			{
				if (Encoding.utf7Encoding == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.utf7Encoding == null)
						{
							Encoding.utf7Encoding = new UTF7Encoding();
						}
					}
				}
				return Encoding.utf7Encoding;
			}
		}

		public static Encoding UTF8
		{
			get
			{
				if (Encoding.utf8EncodingWithMarkers == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.utf8EncodingWithMarkers == null)
						{
							Encoding.utf8EncodingWithMarkers = new UTF8Encoding(true);
						}
					}
				}
				return Encoding.utf8EncodingWithMarkers;
			}
		}

		internal static Encoding UTF8Unmarked
		{
			get
			{
				if (Encoding.utf8EncodingWithoutMarkers == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.utf8EncodingWithoutMarkers == null)
						{
							Encoding.utf8EncodingWithoutMarkers = new UTF8Encoding(false, false);
						}
					}
				}
				return Encoding.utf8EncodingWithoutMarkers;
			}
		}

		internal static Encoding UTF8UnmarkedUnsafe
		{
			get
			{
				if (Encoding.utf8EncodingUnsafe == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.utf8EncodingUnsafe == null)
						{
							Encoding.utf8EncodingUnsafe = new UTF8Encoding(false, false);
							Encoding.utf8EncodingUnsafe.is_readonly = false;
							Encoding.utf8EncodingUnsafe.DecoderFallback = new DecoderReplacementFallback(string.Empty);
							Encoding.utf8EncodingUnsafe.is_readonly = true;
						}
					}
				}
				return Encoding.utf8EncodingUnsafe;
			}
		}

		public static Encoding Unicode
		{
			get
			{
				if (Encoding.unicodeEncoding == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.unicodeEncoding == null)
						{
							Encoding.unicodeEncoding = new UnicodeEncoding(false, true);
						}
					}
				}
				return Encoding.unicodeEncoding;
			}
		}

		public static Encoding UTF32
		{
			get
			{
				if (Encoding.utf32Encoding == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.utf32Encoding == null)
						{
							Encoding.utf32Encoding = new UTF32Encoding(false, true);
						}
					}
				}
				return Encoding.utf32Encoding;
			}
		}

		internal static Encoding BigEndianUTF32
		{
			get
			{
				if (Encoding.bigEndianUTF32Encoding == null)
				{
					object obj = Encoding.lockobj;
					lock (obj)
					{
						if (Encoding.bigEndianUTF32Encoding == null)
						{
							Encoding.bigEndianUTF32Encoding = new UTF32Encoding(true, true);
						}
					}
				}
				return Encoding.bigEndianUTF32Encoding;
			}
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe virtual int GetByteCount(char* chars, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			char[] array = new char[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = chars[i];
			}
			return this.GetByteCount(array);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe virtual int GetCharCount(byte* bytes, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = bytes[i];
			}
			return this.GetCharCount(array, 0, count);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe virtual int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			byte[] array = new byte[byteCount];
			for (int i = 0; i < byteCount; i++)
			{
				array[i] = bytes[i];
			}
			char[] chars2 = this.GetChars(array, 0, byteCount);
			int num = chars2.Length;
			if (num > charCount)
			{
				throw new ArgumentException("charCount is less than the number of characters produced", "charCount");
			}
			for (int j = 0; j < num; j++)
			{
				chars[j] = chars2[j];
			}
			return num;
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe virtual int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			char[] array = new char[charCount];
			for (int i = 0; i < charCount; i++)
			{
				array[i] = chars[i];
			}
			byte[] bytes2 = this.GetBytes(array, 0, charCount);
			int num = bytes2.Length;
			if (num > byteCount)
			{
				throw new ArgumentException("byteCount is less that the number of bytes produced", "byteCount");
			}
			for (int j = 0; j < num; j++)
			{
				bytes[j] = bytes2[j];
			}
			return bytes2.Length;
		}

		internal int codePage;

		internal int windows_code_page;

		private bool is_readonly = true;

		private DecoderFallback decoder_fallback;

		private EncoderFallback encoder_fallback;

		private static Assembly i18nAssembly;

		private static bool i18nDisabled;

		private static EncodingInfo[] encoding_infos;

		private static readonly object[] encodings = new object[]
		{
			20127, "ascii", "us_ascii", "us", "ansi_x3.4_1968", "ansi_x3.4_1986", "cp367", "csascii", "ibm367", "iso_ir_6",
			"iso646_us", "iso_646.irv:1991", 65000, "utf_7", "csunicode11utf7", "unicode_1_1_utf_7", "unicode_2_0_utf_7", "x_unicode_1_1_utf_7", "x_unicode_2_0_utf_7", 65001,
			"utf_8", "unicode_1_1_utf_8", "unicode_2_0_utf_8", "x_unicode_1_1_utf_8", "x_unicode_2_0_utf_8", 1200, "utf_16", "UTF_16LE", "ucs_2", "unicode",
			"iso_10646_ucs2", 1201, "unicodefffe", "utf_16be", 12000, "utf_32", "UTF_32LE", "ucs_4", 12001, "UTF_32BE",
			28591, "iso_8859_1", "latin1"
		};

		internal string body_name;

		internal string encoding_name;

		internal string header_name;

		internal bool is_mail_news_display;

		internal bool is_mail_news_save;

		internal bool is_browser_save;

		internal bool is_browser_display;

		internal string web_name;

		private static volatile Encoding asciiEncoding;

		private static volatile Encoding bigEndianEncoding;

		private static volatile Encoding defaultEncoding;

		private static volatile Encoding utf7Encoding;

		private static volatile Encoding utf8EncodingWithMarkers;

		private static volatile Encoding utf8EncodingWithoutMarkers;

		private static volatile Encoding unicodeEncoding;

		private static volatile Encoding isoLatin1Encoding;

		private static volatile Encoding utf8EncodingUnsafe;

		private static volatile Encoding utf32Encoding;

		private static volatile Encoding bigEndianUTF32Encoding;

		private static readonly object lockobj = new object();

		private sealed class ForwardingDecoder : Decoder
		{
			public ForwardingDecoder(Encoding enc)
			{
				this.encoding = enc;
				DecoderFallback decoderFallback = this.encoding.DecoderFallback;
				if (decoderFallback != null)
				{
					base.Fallback = decoderFallback;
				}
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return this.encoding.GetCharCount(bytes, index, count);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				return this.encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
			}

			private Encoding encoding;
		}

		private sealed class ForwardingEncoder : Encoder
		{
			public ForwardingEncoder(Encoding enc)
			{
				this.encoding = enc;
				EncoderFallback encoderFallback = this.encoding.EncoderFallback;
				if (encoderFallback != null)
				{
					base.Fallback = encoderFallback;
				}
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return this.encoding.GetByteCount(chars, index, count);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteCount, bool flush)
			{
				return this.encoding.GetBytes(chars, charIndex, charCount, bytes, byteCount);
			}

			private Encoding encoding;
		}
	}
}
