using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text
{
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	[Serializable]
	public class UnicodeEncoding : Encoding
	{
		public UnicodeEncoding()
			: this(false, true)
		{
			this.bigEndian = false;
			this.byteOrderMark = true;
		}

		public UnicodeEncoding(bool bigEndian, bool byteOrderMark)
			: this(bigEndian, byteOrderMark, false)
		{
		}

		public UnicodeEncoding(bool bigEndian, bool byteOrderMark, bool throwOnInvalidBytes)
			: base((!bigEndian) ? 1200 : 1201)
		{
			if (throwOnInvalidBytes)
			{
				base.SetFallbackInternal(null, new DecoderExceptionFallback());
			}
			else
			{
				base.SetFallbackInternal(null, new DecoderReplacementFallback("\ufffd"));
			}
			this.bigEndian = bigEndian;
			this.byteOrderMark = byteOrderMark;
			if (bigEndian)
			{
				this.body_name = "unicodeFFFE";
				this.encoding_name = "Unicode (Big-Endian)";
				this.header_name = "unicodeFFFE";
				this.is_browser_save = false;
				this.web_name = "unicodeFFFE";
			}
			else
			{
				this.body_name = "utf-16";
				this.encoding_name = "Unicode";
				this.header_name = "utf-16";
				this.is_browser_save = true;
				this.web_name = "utf-16";
			}
			this.windows_code_page = 1200;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (index < 0 || index > chars.Length)
			{
				throw new ArgumentOutOfRangeException("index", Encoding._("ArgRange_Array"));
			}
			if (count < 0 || count > chars.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", Encoding._("ArgRange_Array"));
			}
			return count * 2;
		}

		public override int GetByteCount(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return s.Length * 2;
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetByteCount(char* chars, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return count * 2;
		}

		public unsafe override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_Array"));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_Array"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			if (charCount == 0)
			{
				return 0;
			}
			int num = bytes.Length - byteIndex;
			if (bytes.Length == 0)
			{
				bytes = new byte[1];
			}
			fixed (char* ptr = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
			{
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					return this.GetBytesInternal(ptr + charIndex, charCount, ptr2 + byteIndex, num);
				}
			}
		}

		public unsafe override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > s.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_StringIndex"));
			}
			if (charCount < 0 || charCount > s.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_StringRange"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			if (charCount == 0)
			{
				return 0;
			}
			int num = bytes.Length - byteIndex;
			if (bytes.Length == 0)
			{
				bytes = new byte[1];
			}
			fixed (char* ptr = s + RuntimeHelpers.OffsetToStringData / 2)
			{
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					return this.GetBytesInternal(ptr + charIndex, charCount, ptr2 + byteIndex, num);
				}
			}
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
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
			return this.GetBytesInternal(chars, charCount, bytes, byteCount);
		}

		private unsafe int GetBytesInternal(char* chars, int charCount, byte* bytes, int byteCount)
		{
			int num = charCount * 2;
			if (byteCount < num)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			UnicodeEncoding.CopyChars((byte*)chars, bytes, num, this.bigEndian);
			return num;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", Encoding._("ArgRange_Array"));
			}
			if (count < 0 || count > bytes.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", Encoding._("ArgRange_Array"));
			}
			return count / 2;
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return count / 2;
		}

		public unsafe override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
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
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_Array"));
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_Array"));
			}
			if (byteCount == 0)
			{
				return 0;
			}
			int num = chars.Length - charIndex;
			if (chars.Length == 0)
			{
				chars = new char[1];
			}
			fixed (byte* ptr = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
			{
				fixed (char* ptr2 = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
				{
					return this.GetCharsInternal(ptr + byteIndex, byteCount, ptr2 + charIndex, num);
				}
			}
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
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
			return this.GetCharsInternal(bytes, byteCount, chars, charCount);
		}

		[ComVisible(false)]
		public unsafe override string GetString(byte[] bytes, int index, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", Encoding._("ArgRange_Array"));
			}
			if (count < 0 || count > bytes.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", Encoding._("ArgRange_Array"));
			}
			if (count == 0)
			{
				return string.Empty;
			}
			int num = count / 2;
			string text = string.InternalAllocateStr(num);
			fixed (byte* ptr = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
			{
				fixed (string text2 = text)
				{
					fixed (char* ptr2 = text2 + RuntimeHelpers.OffsetToStringData / 2)
					{
						this.GetCharsInternal(ptr + index, count, ptr2, num);
						text2 = null;
						ptr = null;
						return text;
					}
				}
			}
		}

		private unsafe int GetCharsInternal(byte* bytes, int byteCount, char* chars, int charCount)
		{
			int num = byteCount / 2;
			if (charCount < num)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			UnicodeEncoding.CopyChars(bytes, (byte*)chars, byteCount, this.bigEndian);
			return num;
		}

		[ComVisible(false)]
		public override Encoder GetEncoder()
		{
			return base.GetEncoder();
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_NonNegative"));
			}
			return charCount * 2;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_NonNegative"));
			}
			return byteCount / 2;
		}

		public override Decoder GetDecoder()
		{
			return new UnicodeEncoding.UnicodeDecoder(this.bigEndian);
		}

		public override byte[] GetPreamble()
		{
			if (this.byteOrderMark)
			{
				byte[] array = new byte[2];
				if (this.bigEndian)
				{
					array[0] = 254;
					array[1] = byte.MaxValue;
				}
				else
				{
					array[0] = byte.MaxValue;
					array[1] = 254;
				}
				return array;
			}
			return new byte[0];
		}

		public override bool Equals(object value)
		{
			UnicodeEncoding unicodeEncoding = value as UnicodeEncoding;
			return unicodeEncoding != null && (this.codePage == unicodeEncoding.codePage && this.bigEndian == unicodeEncoding.bigEndian) && this.byteOrderMark == unicodeEncoding.byteOrderMark;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private unsafe static void CopyChars(byte* src, byte* dest, int count, bool bigEndian)
		{
			if (BitConverter.IsLittleEndian != bigEndian)
			{
				string.memcpy(dest, src, count & -2);
				return;
			}
			switch (count)
			{
			case 0:
				return;
			case 1:
				return;
			case 2:
				goto IL_0220;
			case 3:
				goto IL_0220;
			case 4:
				goto IL_01F1;
			case 5:
				goto IL_01F1;
			case 6:
				goto IL_01F1;
			case 7:
				goto IL_01F1;
			case 8:
				break;
			case 9:
				break;
			case 10:
				break;
			case 11:
				break;
			case 12:
				break;
			case 13:
				break;
			case 14:
				break;
			case 15:
				break;
			default:
				do
				{
					*dest = src[1];
					dest[1] = *src;
					dest[2] = src[3];
					dest[3] = src[2];
					dest[4] = src[5];
					dest[5] = src[4];
					dest[6] = src[7];
					dest[7] = src[6];
					dest[8] = src[9];
					dest[9] = src[8];
					dest[10] = src[11];
					dest[11] = src[10];
					dest[12] = src[13];
					dest[13] = src[12];
					dest[14] = src[15];
					dest[15] = src[14];
					dest += 16;
					src += 16;
					count -= 16;
				}
				while ((count & -16) != 0);
				switch (count)
				{
				case 0:
					return;
				case 1:
					return;
				case 2:
					goto IL_0220;
				case 3:
					goto IL_0220;
				case 4:
					goto IL_01F1;
				case 5:
					goto IL_01F1;
				case 6:
					goto IL_01F1;
				case 7:
					goto IL_01F1;
				}
				break;
			}
			*dest = src[1];
			dest[1] = *src;
			dest[2] = src[3];
			dest[3] = src[2];
			dest[4] = src[5];
			dest[5] = src[4];
			dest[6] = src[7];
			dest[7] = src[6];
			dest += 8;
			src += 8;
			if ((count & 4) == 0)
			{
				goto IL_0217;
			}
			IL_01F1:
			*dest = src[1];
			dest[1] = *src;
			dest[2] = src[3];
			dest[3] = src[2];
			dest += 4;
			src += 4;
			IL_0217:
			if ((count & 2) == 0)
			{
				return;
			}
			IL_0220:
			*dest = src[1];
			dest[1] = *src;
		}

		internal const int UNICODE_CODE_PAGE = 1200;

		internal const int BIG_UNICODE_CODE_PAGE = 1201;

		public const int CharSize = 2;

		private bool bigEndian;

		private bool byteOrderMark;

		private sealed class UnicodeDecoder : Decoder
		{
			public UnicodeDecoder(bool bigEndian)
			{
				this.bigEndian = bigEndian;
				this.leftOverByte = -1;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				if (bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				if (index < 0 || index > bytes.Length)
				{
					throw new ArgumentOutOfRangeException("index", Encoding._("ArgRange_Array"));
				}
				if (count < 0 || count > bytes.Length - index)
				{
					throw new ArgumentOutOfRangeException("count", Encoding._("ArgRange_Array"));
				}
				if (this.leftOverByte != -1)
				{
					return (count + 1) / 2;
				}
				return count / 2;
			}

			public unsafe override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
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
					throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
				}
				if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
				{
					throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_Array"));
				}
				if (charIndex < 0 || charIndex > chars.Length)
				{
					throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_Array"));
				}
				if (byteCount == 0)
				{
					return 0;
				}
				int num = this.leftOverByte;
				int num2;
				if (num != -1)
				{
					num2 = (byteCount + 1) / 2;
				}
				else
				{
					num2 = byteCount / 2;
				}
				if (chars.Length - charIndex < num2)
				{
					throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
				}
				if (num != -1)
				{
					if (this.bigEndian)
					{
						chars[charIndex] = (char)((num << 8) | (int)bytes[byteIndex]);
					}
					else
					{
						chars[charIndex] = (char)(((int)bytes[byteIndex] << 8) | num);
					}
					charIndex++;
					byteIndex++;
					byteCount--;
				}
				if ((byteCount & -2) != 0)
				{
					fixed (byte* ptr = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
					{
						fixed (char* ptr2 = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
						{
							UnicodeEncoding.CopyChars(ptr + byteIndex, (byte*)(ptr2 + charIndex), byteCount, this.bigEndian);
						}
					}
				}
				if ((byteCount & 1) == 0)
				{
					this.leftOverByte = -1;
				}
				else
				{
					this.leftOverByte = (int)bytes[byteCount + byteIndex - 1];
				}
				return num2;
			}

			private bool bigEndian;

			private int leftOverByte;
		}
	}
}
