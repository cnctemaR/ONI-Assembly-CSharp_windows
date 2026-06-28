using System;

namespace System.Text
{
	[Serializable]
	public sealed class UTF32Encoding : Encoding
	{
		public UTF32Encoding()
			: this(false, true, false)
		{
		}

		public UTF32Encoding(bool bigEndian, bool byteOrderMark)
			: this(bigEndian, byteOrderMark, false)
		{
		}

		public UTF32Encoding(bool bigEndian, bool byteOrderMark, bool throwOnInvalidCharacters)
			: base((!bigEndian) ? 12000 : 12001)
		{
			this.bigEndian = bigEndian;
			this.byteOrderMark = byteOrderMark;
			if (throwOnInvalidCharacters)
			{
				base.SetFallbackInternal(EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
			}
			else
			{
				base.SetFallbackInternal(new EncoderReplacementFallback("\ufffd"), new DecoderReplacementFallback("\ufffd"));
			}
			if (bigEndian)
			{
				this.body_name = "utf-32BE";
				this.encoding_name = "UTF-32 (Big-Endian)";
				this.header_name = "utf-32BE";
				this.web_name = "utf-32BE";
			}
			else
			{
				this.body_name = "utf-32";
				this.encoding_name = "UTF-32";
				this.header_name = "utf-32";
				this.web_name = "utf-32";
			}
			this.windows_code_page = 12000;
		}

		[MonoTODO("handle fallback")]
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
			int num = 0;
			for (int i = index; i < index + count; i++)
			{
				if (char.IsSurrogate(chars[i]))
				{
					if (i + 1 < chars.Length && char.IsSurrogate(chars[i + 1]))
					{
						num += 4;
					}
					else
					{
						num += 4;
					}
				}
				else
				{
					num += 4;
				}
			}
			return num;
		}

		[MonoTODO("handle fallback")]
		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
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
			if (bytes.Length - byteIndex < charCount * 4)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			int num = byteIndex;
			while (charCount-- > 0)
			{
				char c = chars[charIndex++];
				if (char.IsSurrogate(c))
				{
					if (charCount-- > 0)
					{
						int num2 = (int)('Ѐ' * (c - '\ud800')) + 65536 + (int)chars[charIndex++] - 56320;
						if (this.bigEndian)
						{
							for (int i = 0; i < 4; i++)
							{
								bytes[num + 3 - i] = (byte)(num2 % 256);
								num2 >>= 8;
							}
							num += 4;
						}
						else
						{
							for (int j = 0; j < 4; j++)
							{
								bytes[num++] = (byte)(num2 % 256);
								num2 >>= 8;
							}
						}
					}
					else if (this.bigEndian)
					{
						bytes[num++] = 0;
						bytes[num++] = 0;
						bytes[num++] = 0;
						bytes[num++] = 63;
					}
					else
					{
						bytes[num++] = 63;
						bytes[num++] = 0;
						bytes[num++] = 0;
						bytes[num++] = 0;
					}
				}
				else if (this.bigEndian)
				{
					bytes[num++] = 0;
					bytes[num++] = 0;
					bytes[num++] = (byte)(c >> 8);
					bytes[num++] = (byte)c;
				}
				else
				{
					bytes[num++] = (byte)c;
					bytes[num++] = (byte)(c >> 8);
					bytes[num++] = 0;
					bytes[num++] = 0;
				}
			}
			return num - byteIndex;
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
			return count / 4;
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
			if (chars.Length - charIndex < byteCount / 4)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			int num = charIndex;
			if (this.bigEndian)
			{
				while (byteCount >= 4)
				{
					chars[num++] = (char)(((int)bytes[byteIndex] << 24) | ((int)bytes[byteIndex + 1] << 16) | ((int)bytes[byteIndex + 2] << 8) | (int)bytes[byteIndex + 3]);
					byteIndex += 4;
					byteCount -= 4;
				}
			}
			else
			{
				while (byteCount >= 4)
				{
					chars[num++] = (char)((int)bytes[byteIndex] | ((int)bytes[byteIndex + 1] << 8) | ((int)bytes[byteIndex + 2] << 16) | ((int)bytes[byteIndex + 3] << 24));
					byteIndex += 4;
					byteCount -= 4;
				}
			}
			return num - charIndex;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_NonNegative"));
			}
			return charCount * 4;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_NonNegative"));
			}
			return byteCount / 4;
		}

		public override Decoder GetDecoder()
		{
			return new UTF32Encoding.UTF32Decoder(this.bigEndian);
		}

		public override byte[] GetPreamble()
		{
			if (this.byteOrderMark)
			{
				byte[] array = new byte[4];
				if (this.bigEndian)
				{
					array[2] = 254;
					array[3] = byte.MaxValue;
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
			UTF32Encoding utf32Encoding = value as UTF32Encoding;
			return utf32Encoding != null && (this.codePage == utf32Encoding.codePage && this.bigEndian == utf32Encoding.bigEndian && this.byteOrderMark == utf32Encoding.byteOrderMark) && base.Equals(value);
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			if (this.bigEndian)
			{
				num ^= 31;
			}
			if (this.byteOrderMark)
			{
				num ^= 63;
			}
			return num;
		}

		[CLSCompliant(false)]
		public unsafe override int GetByteCount(char* chars, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			return count * 4;
		}

		public override int GetByteCount(string s)
		{
			return base.GetByteCount(s);
		}

		[CLSCompliant(false)]
		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			return base.GetBytes(chars, charCount, bytes, byteCount);
		}

		public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return base.GetBytes(s, charIndex, charCount, bytes, byteIndex);
		}

		[CLSCompliant(false)]
		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			return base.GetCharCount(bytes, count);
		}

		[CLSCompliant(false)]
		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			return base.GetChars(bytes, byteCount, chars, charCount);
		}

		public override string GetString(byte[] bytes, int index, int count)
		{
			return base.GetString(bytes, index, count);
		}

		public override Encoder GetEncoder()
		{
			return base.GetEncoder();
		}

		internal const int UTF32_CODE_PAGE = 12000;

		internal const int BIG_UTF32_CODE_PAGE = 12001;

		private bool bigEndian;

		private bool byteOrderMark;

		private sealed class UTF32Decoder : Decoder
		{
			public UTF32Decoder(bool bigEndian)
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
					return (count + 1) / 4;
				}
				return count / 4;
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
				int num = charIndex;
				int num2 = this.leftOverByte;
				int num3 = chars.Length;
				int num4 = 4 - this.leftOverLength;
				if (this.leftOverLength > 0 && byteCount > num4)
				{
					if (this.bigEndian)
					{
						for (int i = 0; i < num4; i++)
						{
							num2 += (int)bytes[byteIndex++] << 4 - byteCount--;
						}
					}
					else
					{
						for (int j = 0; j < num4; j++)
						{
							num2 += (int)bytes[byteIndex++] << byteCount--;
						}
					}
					if ((num2 > 65535 && num + 1 < num3) || num < num3)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
					}
					if (num2 > 65535)
					{
						chars[num++] = (char)((num2 - 10000) / 1024 + 55296);
						chars[num++] = (char)((num2 - 10000) % 1024 + 56320);
					}
					else
					{
						chars[num++] = (char)num2;
					}
					this.leftOverLength = 0;
				}
				while (byteCount > 3)
				{
					char c;
					if (this.bigEndian)
					{
						c = (char)(((int)bytes[byteIndex++] << 24) | ((int)bytes[byteIndex++] << 16) | ((int)bytes[byteIndex++] << 8) | (int)bytes[byteIndex++]);
					}
					else
					{
						c = (char)((int)bytes[byteIndex++] | ((int)bytes[byteIndex++] << 8) | ((int)bytes[byteIndex++] << 16) | ((int)bytes[byteIndex++] << 24));
					}
					byteCount -= 4;
					if (num >= num3)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
					}
					chars[num++] = c;
				}
				if (byteCount > 0)
				{
					this.leftOverLength = byteCount;
					num2 = 0;
					if (this.bigEndian)
					{
						for (int k = 0; k < byteCount; k++)
						{
							num2 += (int)bytes[byteIndex++] << 4 - byteCount--;
						}
					}
					else
					{
						for (int l = 0; l < byteCount; l++)
						{
							num2 += (int)bytes[byteIndex++] << byteCount--;
						}
					}
					this.leftOverByte = num2;
				}
				return num - charIndex;
			}

			private bool bigEndian;

			private int leftOverByte;

			private int leftOverLength;
		}
	}
}
