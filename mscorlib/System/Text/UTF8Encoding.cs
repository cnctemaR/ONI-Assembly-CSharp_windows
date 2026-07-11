using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text
{
	[MonoTODO("EncoderFallback is not handled")]
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with .NET")]
	[Serializable]
	public class UTF8Encoding : Encoding
	{
		public UTF8Encoding()
			: this(false, false)
		{
		}

		public UTF8Encoding(bool encoderShouldEmitUTF8Identifier)
			: this(encoderShouldEmitUTF8Identifier, false)
		{
		}

		public UTF8Encoding(bool encoderShouldEmitUTF8Identifier, bool throwOnInvalidBytes)
			: base(65001)
		{
			this.emitIdentifier = encoderShouldEmitUTF8Identifier;
			if (throwOnInvalidBytes)
			{
				base.SetFallbackInternal(null, DecoderFallback.ExceptionFallback);
			}
			else
			{
				base.SetFallbackInternal(null, DecoderFallback.StandardSafeFallback);
			}
			this.web_name = (this.body_name = (this.header_name = "utf-8"));
			this.encoding_name = "Unicode (UTF-8)";
			this.is_browser_save = true;
			this.is_browser_display = true;
			this.is_mail_news_display = true;
			this.is_mail_news_save = true;
			this.windows_code_page = 1200;
		}

		private unsafe static int InternalGetByteCount(char[] chars, int index, int count, ref char leftOver, bool flush)
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
			if (index != chars.Length)
			{
				fixed (char* ptr = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
				{
					return UTF8Encoding.InternalGetByteCount(ptr + index, count, ref leftOver, flush);
				}
			}
			if (flush && leftOver != '\0')
			{
				leftOver = '\0';
				return 3;
			}
			return 0;
		}

		private unsafe static int InternalGetByteCount(char* chars, int count, ref char leftOver, bool flush)
		{
			int num = 0;
			char* ptr = chars + count;
			while (chars < ptr)
			{
				if (leftOver == '\0')
				{
					while (chars < ptr)
					{
						if (*chars < '\u0080')
						{
							num++;
						}
						else if (*chars < 'ࠀ')
						{
							num += 2;
						}
						else if (*chars < '\ud800' || *chars > '\udfff')
						{
							num += 3;
						}
						else if (*chars <= '\udbff')
						{
							if (chars + 1 >= ptr || chars[1] < '\udc00' || chars[1] > '\udfff')
							{
								leftOver = *chars;
								chars++;
								break;
							}
							num += 4;
							chars++;
						}
						else
						{
							num += 3;
							leftOver = '\0';
						}
						chars++;
					}
				}
				else
				{
					if (*chars >= '\udc00' && *chars <= '\udfff')
					{
						num += 4;
						chars++;
					}
					else
					{
						num += 3;
					}
					leftOver = '\0';
				}
			}
			if (flush && leftOver != '\0')
			{
				num += 3;
				leftOver = '\0';
			}
			return num;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			char c = '\0';
			return UTF8Encoding.InternalGetByteCount(chars, index, count, ref c, true);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetByteCount(char* chars, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (count == 0)
			{
				return 0;
			}
			char c = '\0';
			return UTF8Encoding.InternalGetByteCount(chars, count, ref c, true);
		}

		private unsafe static int InternalGetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, ref char leftOver, bool flush)
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
			if (charIndex == chars.Length)
			{
				if (flush && leftOver != '\0')
				{
					leftOver = '\0';
				}
				return 0;
			}
			fixed (char* ptr = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
			{
				if (bytes.Length == byteIndex)
				{
					return UTF8Encoding.InternalGetBytes(ptr + charIndex, charCount, null, 0, ref leftOver, flush);
				}
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					return UTF8Encoding.InternalGetBytes(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex, ref leftOver, flush);
				}
			}
		}

		private unsafe static int InternalGetBytes(char* chars, int count, byte* bytes, int bcount, ref char leftOver, bool flush)
		{
			char* ptr = chars + count;
			byte* ptr2 = bytes + bcount;
			while (chars < ptr)
			{
				if (leftOver == '\0')
				{
					while (chars < ptr)
					{
						int num = (int)(*chars);
						if (num < 128)
						{
							if (bytes >= ptr2)
							{
								goto IL_029B;
							}
							*(bytes++) = (byte)num;
						}
						else if (num < 2048)
						{
							if (bytes + 1 >= ptr2)
							{
								goto IL_029B;
							}
							*bytes = (byte)(192 | (num >> 6));
							bytes[1] = (byte)(128 | (num & 63));
							bytes += 2;
						}
						else if (num < 55296 || num > 57343)
						{
							if (bytes + 2 >= ptr2)
							{
								goto IL_029B;
							}
							*bytes = (byte)(224 | (num >> 12));
							bytes[1] = (byte)(128 | ((num >> 6) & 63));
							bytes[2] = (byte)(128 | (num & 63));
							bytes += 3;
						}
						else
						{
							if (num <= 56319)
							{
								leftOver = *chars;
								chars++;
								break;
							}
							if (bytes + 2 >= ptr2)
							{
								goto IL_029B;
							}
							*bytes = (byte)(224 | (num >> 12));
							bytes[1] = (byte)(128 | ((num >> 6) & 63));
							bytes[2] = (byte)(128 | (num & 63));
							bytes += 3;
							leftOver = '\0';
						}
						chars++;
					}
					continue;
				}
				if (*chars >= '\udc00' && *chars <= '\udfff')
				{
					int num2 = 65536 + (int)(*chars) - 56320 + (int)((int)(leftOver - '\ud800') << 10);
					if (bytes + 3 >= ptr2)
					{
						goto IL_029B;
					}
					*bytes = (byte)(240 | (num2 >> 18));
					bytes[1] = (byte)(128 | ((num2 >> 12) & 63));
					bytes[2] = (byte)(128 | ((num2 >> 6) & 63));
					bytes[3] = (byte)(128 | (num2 & 63));
					bytes += 4;
					chars++;
				}
				else
				{
					int num3 = (int)leftOver;
					if (bytes + 2 >= ptr2)
					{
						goto IL_029B;
					}
					*bytes = (byte)(224 | (num3 >> 12));
					bytes[1] = (byte)(128 | ((num3 >> 6) & 63));
					bytes[2] = (byte)(128 | (num3 & 63));
					bytes += 3;
				}
				leftOver = '\0';
				continue;
				IL_029B:
				throw new ArgumentException("Insufficient Space", "bytes");
			}
			if (flush && leftOver != '\0')
			{
				int num4 = (int)leftOver;
				if (bytes + 2 >= ptr2)
				{
					goto IL_029B;
				}
				*bytes = (byte)(224 | (num4 >> 12));
				bytes[1] = (byte)(128 | ((num4 >> 6) & 63));
				bytes[2] = (byte)(128 | (num4 & 63));
				bytes += 3;
				leftOver = '\0';
			}
			return (int)((long)(bytes - (ptr2 - bcount)));
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			char c = '\0';
			return UTF8Encoding.InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, ref c, true);
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
			if (charIndex == s.Length)
			{
				return 0;
			}
			fixed (char* ptr = s + RuntimeHelpers.OffsetToStringData / 2)
			{
				char c = '\0';
				if (bytes.Length == byteIndex)
				{
					return UTF8Encoding.InternalGetBytes(ptr + charIndex, charCount, null, 0, ref c, true);
				}
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					return UTF8Encoding.InternalGetBytes(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex, ref c, true);
				}
			}
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charCount < 0)
			{
				throw new IndexOutOfRangeException("charCount");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (byteCount < 0)
			{
				throw new IndexOutOfRangeException("charCount");
			}
			if (charCount == 0)
			{
				return 0;
			}
			char c = '\0';
			if (byteCount == 0)
			{
				return UTF8Encoding.InternalGetBytes(chars, charCount, null, 0, ref c, true);
			}
			return UTF8Encoding.InternalGetBytes(chars, charCount, bytes, byteCount, ref c, true);
		}

		private unsafe static int InternalGetCharCount(byte[] bytes, int index, int count, uint leftOverBits, uint leftOverCount, object provider, ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
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
				return 0;
			}
			fixed (byte* ptr = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
			{
				return UTF8Encoding.InternalGetCharCount(ptr + index, count, leftOverBits, leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
			}
		}

		private unsafe static int InternalGetCharCount(byte* bytes, int count, uint leftOverBits, uint leftOverCount, object provider, ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
		{
			int i = 0;
			int num = 0;
			if (leftOverCount == 0U)
			{
				int num2 = i + count;
				while (i < num2)
				{
					if (bytes[i] >= 128)
					{
						break;
					}
					num++;
					i++;
					count--;
				}
			}
			uint num3 = leftOverBits;
			uint num4 = leftOverCount & 15U;
			uint num5 = (leftOverCount >> 4) & 15U;
			while (count > 0)
			{
				uint num6 = (uint)bytes[i++];
				count--;
				if (num5 == 0U)
				{
					if (num6 < 128U)
					{
						num++;
					}
					else if ((num6 & 224U) == 192U)
					{
						num3 = num6 & 31U;
						num4 = 1U;
						num5 = 2U;
					}
					else if ((num6 & 240U) == 224U)
					{
						num3 = num6 & 15U;
						num4 = 1U;
						num5 = 3U;
					}
					else if ((num6 & 248U) == 240U)
					{
						num3 = num6 & 7U;
						num4 = 1U;
						num5 = 4U;
					}
					else if ((num6 & 252U) == 248U)
					{
						num3 = num6 & 3U;
						num4 = 1U;
						num5 = 5U;
					}
					else if ((num6 & 254U) == 252U)
					{
						num3 = num6 & 3U;
						num4 = 1U;
						num5 = 6U;
					}
					else
					{
						num += UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)(i - 1), 1U);
					}
				}
				else if ((num6 & 192U) == 128U)
				{
					num3 = (num3 << 6) | (num6 & 63U);
					if ((num4 += 1U) >= num5)
					{
						if (num3 < 65536U)
						{
							bool flag = false;
							switch (num5)
							{
							case 2U:
								flag = num3 <= 127U;
								break;
							case 3U:
								flag = num3 <= 2047U;
								break;
							case 4U:
								flag = num3 <= 65535U;
								break;
							case 5U:
								flag = num3 <= 2097151U;
								break;
							case 6U:
								flag = num3 <= 67108863U;
								break;
							}
							if (flag)
							{
								num += UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num4), num4);
							}
							else if ((num3 & 63488U) == 55296U)
							{
								num += UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num4), num4);
							}
							else
							{
								num++;
							}
						}
						else if (num3 < 1114112U)
						{
							num += 2;
						}
						else
						{
							num += UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num4), num4);
						}
						num5 = 0U;
					}
				}
				else
				{
					num += UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num4), num4);
					num5 = 0U;
					i--;
					count++;
				}
			}
			if (flush && num5 != 0U)
			{
				num += UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num4), num4);
			}
			return num;
		}

		private unsafe static int Fallback(object provider, ref DecoderFallbackBuffer buffer, ref byte[] bufferArg, byte* bytes, long index, uint size)
		{
			if (buffer == null)
			{
				DecoderFallback decoderFallback = provider as DecoderFallback;
				if (decoderFallback != null)
				{
					buffer = decoderFallback.CreateFallbackBuffer();
				}
				else
				{
					buffer = ((Decoder)provider).FallbackBuffer;
				}
			}
			if (bufferArg == null)
			{
				bufferArg = new byte[1];
			}
			int num = 0;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)size))
			{
				bufferArg[0] = bytes[(int)index + num2];
				buffer.Fallback(bufferArg, 0);
				num += buffer.Remaining;
				buffer.Reset();
				num2++;
			}
			return num;
		}

		private unsafe static void Fallback(object provider, ref DecoderFallbackBuffer buffer, ref byte[] bufferArg, byte* bytes, long byteIndex, uint size, char* chars, ref int charIndex)
		{
			if (buffer == null)
			{
				DecoderFallback decoderFallback = provider as DecoderFallback;
				if (decoderFallback != null)
				{
					buffer = decoderFallback.CreateFallbackBuffer();
				}
				else
				{
					buffer = ((Decoder)provider).FallbackBuffer;
				}
			}
			if (bufferArg == null)
			{
				bufferArg = new byte[1];
			}
			int num = 0;
			while ((long)num < (long)((ulong)size))
			{
				bufferArg[0] = bytes[byteIndex + (long)num];
				buffer.Fallback(bufferArg, 0);
				while (buffer.Remaining > 0)
				{
					chars[charIndex++] = buffer.GetNextChar();
				}
				buffer.Reset();
				num++;
			}
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			DecoderFallbackBuffer decoderFallbackBuffer = null;
			byte[] array = null;
			return UTF8Encoding.InternalGetCharCount(bytes, index, count, 0U, 0U, base.DecoderFallback, ref decoderFallbackBuffer, ref array, true);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			DecoderFallbackBuffer decoderFallbackBuffer = null;
			byte[] array = null;
			return UTF8Encoding.InternalGetCharCount(bytes, count, 0U, 0U, base.DecoderFallback, ref decoderFallbackBuffer, ref array, true);
		}

		private unsafe static int InternalGetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, ref uint leftOverBits, ref uint leftOverCount, object provider, ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
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
			if (charIndex == chars.Length)
			{
				return 0;
			}
			fixed (char* ptr = (ref chars != null && chars.Length != 0 ? ref chars[0] : ref *null))
			{
				if (byteCount == 0 || byteIndex == bytes.Length)
				{
					return UTF8Encoding.InternalGetChars(null, 0, ptr + charIndex, chars.Length - charIndex, ref leftOverBits, ref leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
				}
				fixed (byte* ptr2 = (ref bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *null))
				{
					return UTF8Encoding.InternalGetChars(ptr2 + byteIndex, byteCount, ptr + charIndex, chars.Length - charIndex, ref leftOverBits, ref leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
				}
			}
		}

		private unsafe static int InternalGetChars(byte* bytes, int byteCount, char* chars, int charCount, ref uint leftOverBits, ref uint leftOverCount, object provider, ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
		{
			int num = 0;
			int i = 0;
			int num2 = num;
			if (leftOverCount == 0U)
			{
				int num3 = i + byteCount;
				while (i < num3)
				{
					if (bytes[i] >= 128)
					{
						break;
					}
					chars[num2] = (char)bytes[i];
					num2++;
					i++;
					byteCount--;
				}
			}
			uint num4 = leftOverBits;
			uint num5 = leftOverCount & 15U;
			uint num6 = (leftOverCount >> 4) & 15U;
			int num7 = i + byteCount;
			while (i < num7)
			{
				uint num8 = (uint)bytes[i];
				if (num6 == 0U)
				{
					if (num8 < 128U)
					{
						if (num2 >= charCount)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
						}
						chars[num2++] = (char)num8;
					}
					else if ((num8 & 224U) == 192U)
					{
						num4 = num8 & 31U;
						num5 = 1U;
						num6 = 2U;
					}
					else if ((num8 & 240U) == 224U)
					{
						num4 = num8 & 15U;
						num5 = 1U;
						num6 = 3U;
					}
					else if ((num8 & 248U) == 240U)
					{
						num4 = num8 & 7U;
						num5 = 1U;
						num6 = 4U;
					}
					else if ((num8 & 252U) == 248U)
					{
						num4 = num8 & 3U;
						num5 = 1U;
						num6 = 5U;
					}
					else if ((num8 & 254U) == 252U)
					{
						num4 = num8 & 3U;
						num5 = 1U;
						num6 = 6U;
					}
					else
					{
						UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i, 1U, chars, ref num2);
					}
				}
				else if ((num8 & 192U) == 128U)
				{
					num4 = (num4 << 6) | (num8 & 63U);
					if ((num5 += 1U) >= num6)
					{
						if (num4 < 65536U)
						{
							bool flag = false;
							switch (num6)
							{
							case 2U:
								flag = num4 <= 127U;
								break;
							case 3U:
								flag = num4 <= 2047U;
								break;
							case 4U:
								flag = num4 <= 65535U;
								break;
							case 5U:
								flag = num4 <= 2097151U;
								break;
							case 6U:
								flag = num4 <= 67108863U;
								break;
							}
							if (flag)
							{
								UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num5), num5, chars, ref num2);
							}
							else if ((num4 & 63488U) == 55296U)
							{
								UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num5), num5, chars, ref num2);
							}
							else
							{
								if (num2 >= charCount)
								{
									throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
								}
								chars[num2++] = (char)num4;
							}
						}
						else if (num4 < 1114112U)
						{
							if (num2 + 2 > charCount)
							{
								throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
							}
							num4 -= 65536U;
							chars[num2++] = (char)((num4 >> 10) + 55296U);
							chars[num2++] = (char)((num4 & 1023U) + 56320U);
						}
						else
						{
							UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num5), num5, chars, ref num2);
						}
						num6 = 0U;
					}
				}
				else
				{
					UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num5), num5, chars, ref num2);
					num6 = 0U;
					i--;
				}
				i++;
			}
			if (flush && num6 != 0U)
			{
				UTF8Encoding.Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, (long)i - (long)((ulong)num5), num5, chars, ref num2);
			}
			leftOverBits = num4;
			leftOverCount = num5 | (num6 << 4);
			return num2 - num;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			uint num = 0U;
			uint num2 = 0U;
			DecoderFallbackBuffer decoderFallbackBuffer = null;
			byte[] array = null;
			return UTF8Encoding.InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref num, ref num2, base.DecoderFallback, ref decoderFallbackBuffer, ref array, true);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			DecoderFallbackBuffer decoderFallbackBuffer = null;
			byte[] array = null;
			uint num = 0U;
			uint num2 = 0U;
			return UTF8Encoding.InternalGetChars(bytes, byteCount, chars, charCount, ref num, ref num2, base.DecoderFallback, ref decoderFallbackBuffer, ref array, true);
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
			return byteCount;
		}

		public override Decoder GetDecoder()
		{
			return new UTF8Encoding.UTF8Decoder(base.DecoderFallback);
		}

		public override Encoder GetEncoder()
		{
			return new UTF8Encoding.UTF8Encoder(this.emitIdentifier);
		}

		public override byte[] GetPreamble()
		{
			if (this.emitIdentifier)
			{
				return new byte[] { 239, 187, 191 };
			}
			return new byte[0];
		}

		public override bool Equals(object value)
		{
			UTF8Encoding utf8Encoding = value as UTF8Encoding;
			return utf8Encoding != null && (this.codePage == utf8Encoding.codePage && this.emitIdentifier == utf8Encoding.emitIdentifier && base.DecoderFallback.Equals(utf8Encoding.DecoderFallback)) && base.EncoderFallback.Equals(utf8Encoding.EncoderFallback);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override int GetByteCount(string chars)
		{
			return base.GetByteCount(chars);
		}

		[ComVisible(false)]
		public override string GetString(byte[] bytes, int index, int count)
		{
			return base.GetString(bytes, index, count);
		}

		internal const int UTF8_CODE_PAGE = 65001;

		private bool emitIdentifier;

		[Serializable]
		private class UTF8Decoder : Decoder
		{
			public UTF8Decoder(DecoderFallback fallback)
			{
				base.Fallback = fallback;
				this.leftOverBits = 0U;
				this.leftOverCount = 0U;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				DecoderFallbackBuffer decoderFallbackBuffer = null;
				byte[] array = null;
				return UTF8Encoding.InternalGetCharCount(bytes, index, count, this.leftOverBits, this.leftOverCount, this, ref decoderFallbackBuffer, ref array, false);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				DecoderFallbackBuffer decoderFallbackBuffer = null;
				byte[] array = null;
				return UTF8Encoding.InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref this.leftOverBits, ref this.leftOverCount, this, ref decoderFallbackBuffer, ref array, false);
			}

			private uint leftOverBits;

			private uint leftOverCount;
		}

		[Serializable]
		private class UTF8Encoder : Encoder
		{
			public UTF8Encoder(bool emitIdentifier)
			{
				this.leftOverForCount = '\0';
				this.leftOverForConv = '\0';
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return UTF8Encoding.InternalGetByteCount(chars, index, count, ref this.leftOverForCount, flush);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
			{
				return UTF8Encoding.InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, ref this.leftOverForConv, flush);
			}

			public unsafe override int GetByteCount(char* chars, int count, bool flush)
			{
				return UTF8Encoding.InternalGetByteCount(chars, count, ref this.leftOverForCount, flush);
			}

			public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, bool flush)
			{
				return UTF8Encoding.InternalGetBytes(chars, charCount, bytes, byteCount, ref this.leftOverForConv, flush);
			}

			private char leftOverForCount;

			private char leftOverForConv;
		}
	}
}
