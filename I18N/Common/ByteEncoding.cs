using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace I18N.Common
{
	[Serializable]
	public abstract class ByteEncoding : MonoEncoding
	{
		protected ByteEncoding(int codePage, char[] toChars, string encodingName, string bodyName, string headerName, string webName, bool isBrowserDisplay, bool isBrowserSave, bool isMailNewsDisplay, bool isMailNewsSave, int windowsCodePage)
			: base(codePage)
		{
			if (toChars.Length != 256)
			{
				throw new ArgumentException("toChars");
			}
			this.toChars = toChars;
			this.encodingName = encodingName;
			this.bodyName = bodyName;
			this.headerName = headerName;
			this.webName = webName;
			this.isBrowserDisplay = isBrowserDisplay;
			this.isBrowserSave = isBrowserSave;
			this.isMailNewsDisplay = isMailNewsDisplay;
			this.isMailNewsSave = isMailNewsSave;
			this.windowsCodePage = windowsCodePage;
		}

		public override bool IsAlwaysNormalized(NormalizationForm form)
		{
			if (form != 1)
			{
				return false;
			}
			if (ByteEncoding.isNormalized == null)
			{
				ByteEncoding.isNormalized = new byte[8192];
			}
			if (ByteEncoding.isNormalizedComputed == null)
			{
				ByteEncoding.isNormalizedComputed = new byte[8192];
			}
			if (ByteEncoding.normalization_bytes == null)
			{
				ByteEncoding.normalization_bytes = new byte[256];
				byte[] array = ByteEncoding.normalization_bytes;
				lock (array)
				{
					for (int i = 0; i < 256; i++)
					{
						ByteEncoding.normalization_bytes[i] = (byte)i;
					}
				}
			}
			byte b = (byte)(1 << this.CodePage % 8);
			if ((ByteEncoding.isNormalizedComputed[this.CodePage / 8] & b) == 0)
			{
				Encoding encoding = this.Clone() as Encoding;
				encoding.DecoderFallback = new DecoderReplacementFallback(string.Empty);
				string @string = encoding.GetString(ByteEncoding.normalization_bytes);
				if (@string != @string.Normalize(form))
				{
					byte[] array2 = ByteEncoding.isNormalized;
					int num = this.CodePage / 8;
					array2[num] |= b;
				}
				byte[] array3 = ByteEncoding.isNormalizedComputed;
				int num2 = this.CodePage / 8;
				array3[num2] |= b;
			}
			return (ByteEncoding.isNormalized[this.CodePage / 8] & b) == 0;
		}

		public override bool IsSingleByte
		{
			get
			{
				return true;
			}
		}

		public override int GetByteCount(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return s.Length;
		}

		public unsafe override int GetByteCountImpl(char* chars, int count)
		{
			return count;
		}

		protected unsafe abstract void ToBytes(char* chars, int charCount, byte* bytes, int byteCount);

		protected unsafe virtual void ToBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (charCount == 0 || bytes.Length == byteIndex)
			{
				return;
			}
			fixed (char* ptr = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
			{
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					this.ToBytes(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex);
				}
			}
		}

		public unsafe override int GetBytesImpl(char* chars, int charCount, byte* bytes, int byteCount)
		{
			this.ToBytes(chars, charCount, bytes, byteCount);
			return charCount;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", Strings.GetString("ArgRange_Array"));
			}
			if (count < 0 || count > bytes.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", Strings.GetString("ArgRange_Array"));
			}
			return count;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Strings.GetString("ArgRange_Array"));
			}
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteCount", Strings.GetString("ArgRange_Array"));
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Strings.GetString("ArgRange_Array"));
			}
			if (chars.Length - charIndex < byteCount)
			{
				throw new ArgumentException(Strings.GetString("Arg_InsufficientSpace"));
			}
			int num = byteCount;
			char[] array = this.toChars;
			while (num-- > 0)
			{
				chars[charIndex++] = array[(int)bytes[byteIndex++]];
			}
			return byteCount;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Strings.GetString("ArgRange_NonNegative"));
			}
			return charCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", Strings.GetString("ArgRange_NonNegative"));
			}
			return byteCount;
		}

		public unsafe override string GetString(byte[] bytes, int index, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", Strings.GetString("ArgRange_Array"));
			}
			if (count < 0 || count > bytes.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", Strings.GetString("ArgRange_Array"));
			}
			if (count == 0)
			{
				return string.Empty;
			}
			string text = new string('\0', count);
			fixed (byte* ptr = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
			{
				fixed (string text2 = text)
				{
					fixed (char* ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						fixed (char* ptr3 = (ref this.toChars != null && this.toChars.Length != 0 ? ref this.toChars[0] : ref *null))
						{
							byte* ptr4 = ptr + index;
							char* ptr5 = ptr2;
							while (count-- != 0)
							{
								*(ptr5++) = ptr3[(IntPtr)(*(ptr4++)) * 2];
							}
						}
						text2 = null;
						ptr = null;
						return text;
					}
				}
			}
		}

		public override string GetString(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return this.GetString(bytes, 0, bytes.Length);
		}

		public override string BodyName
		{
			get
			{
				return this.bodyName;
			}
		}

		public override string EncodingName
		{
			get
			{
				return this.encodingName;
			}
		}

		public override string HeaderName
		{
			get
			{
				return this.headerName;
			}
		}

		public override bool IsBrowserDisplay
		{
			get
			{
				return this.isBrowserDisplay;
			}
		}

		public override bool IsBrowserSave
		{
			get
			{
				return this.isBrowserSave;
			}
		}

		public override bool IsMailNewsDisplay
		{
			get
			{
				return this.isMailNewsDisplay;
			}
		}

		public override bool IsMailNewsSave
		{
			get
			{
				return this.isMailNewsSave;
			}
		}

		public override string WebName
		{
			get
			{
				return this.webName;
			}
		}

		public override int WindowsCodePage
		{
			get
			{
				return this.windowsCodePage;
			}
		}

		protected char[] toChars;

		protected string encodingName;

		protected string bodyName;

		protected string headerName;

		protected string webName;

		protected bool isBrowserDisplay;

		protected bool isBrowserSave;

		protected bool isMailNewsDisplay;

		protected bool isMailNewsSave;

		protected int windowsCodePage;

		private static byte[] isNormalized;

		private static byte[] isNormalizedComputed;

		private static byte[] normalization_bytes;
	}
}
