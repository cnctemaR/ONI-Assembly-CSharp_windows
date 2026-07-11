using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mono.Unix
{
	[Serializable]
	public class UnixEncoding : Encoding
	{
		private static int InternalGetByteCount(char[] chars, int index, int count, uint leftOver, bool flush)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (index < 0 || index > chars.Length)
			{
				throw new ArgumentOutOfRangeException("index", UnixEncoding._("ArgRange_Array"));
			}
			if (count < 0 || count > chars.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", UnixEncoding._("ArgRange_Array"));
			}
			int num = 0;
			uint num2 = leftOver;
			while (count > 0)
			{
				char c = chars[index];
				if (num2 == 0U)
				{
					if (c == UnixEncoding.EscapeByte && count > 1)
					{
						num++;
						index++;
						count--;
					}
					else if (c < '\u0080')
					{
						num++;
					}
					else if (c < 'ࠀ')
					{
						num += 2;
					}
					else if (c >= '\ud800' && c <= '\udbff')
					{
						num2 = (uint)c;
					}
					else
					{
						num += 3;
					}
				}
				else
				{
					if (c < '\udc00' || c > '\udfff')
					{
						num += 3;
						num2 = 0U;
						continue;
					}
					num += 4;
					num2 = 0U;
				}
				index++;
				count--;
			}
			if (flush && num2 != 0U)
			{
				num += 3;
			}
			return num;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return UnixEncoding.InternalGetByteCount(chars, index, count, 0U, true);
		}

		public override int GetByteCount(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			int num = 0;
			int i = s.Length;
			int num2 = 0;
			while (i > 0)
			{
				char c = s[num++];
				if (c == UnixEncoding.EscapeByte && i > 1)
				{
					num2++;
					num++;
					i--;
				}
				else if (c < '\u0080')
				{
					num2++;
				}
				else if (c < 'ࠀ')
				{
					num2 += 2;
				}
				else if (c >= '\ud800' && c <= '\udbff' && i > 1)
				{
					uint num3 = (uint)s[num];
					if (num3 >= 56320U && num3 <= 57343U)
					{
						num2 += 4;
						num++;
						i--;
					}
					else
					{
						num2 += 3;
					}
				}
				else
				{
					num2 += 3;
				}
				i--;
			}
			return num2;
		}

		private static int InternalGetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, ref uint leftOver, bool flush)
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
				throw new ArgumentOutOfRangeException("charIndex", UnixEncoding._("ArgRange_Array"));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", UnixEncoding._("ArgRange_Array"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", UnixEncoding._("ArgRange_Array"));
			}
			int num = bytes.Length;
			uint num2 = leftOver;
			int num3 = byteIndex;
			while (charCount > 0)
			{
				char c = chars[charIndex++];
				charCount--;
				uint num4;
				if (num2 == 0U)
				{
					if (c >= '\ud800' && c <= '\udbff')
					{
						num2 = (uint)c;
						continue;
					}
					if (c == UnixEncoding.EscapeByte)
					{
						if (num3 >= num)
						{
							throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
						}
						if (--charCount >= 0)
						{
							bytes[num3++] = (byte)chars[charIndex++];
							continue;
						}
						continue;
					}
					else
					{
						num4 = (uint)c;
					}
				}
				else if (c >= '\udc00' && c <= '\udfff')
				{
					num4 = (num2 - 55296U << 10) + (uint)(c - '\udc00') + 65536U;
					num2 = 0U;
				}
				else
				{
					num4 = num2;
					num2 = 0U;
					charIndex--;
					charCount++;
				}
				if (num4 < 128U)
				{
					if (num3 >= num)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num3++] = (byte)num4;
				}
				else if (num4 < 2048U)
				{
					if (num3 + 2 > num)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num3++] = (byte)(192U | (num4 >> 6));
					bytes[num3++] = (byte)(128U | (num4 & 63U));
				}
				else if (num4 < 65536U)
				{
					if (num3 + 3 > num)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num3++] = (byte)(224U | (num4 >> 12));
					bytes[num3++] = (byte)(128U | ((num4 >> 6) & 63U));
					bytes[num3++] = (byte)(128U | (num4 & 63U));
				}
				else
				{
					if (num3 + 4 > num)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num3++] = (byte)(240U | (num4 >> 18));
					bytes[num3++] = (byte)(128U | ((num4 >> 12) & 63U));
					bytes[num3++] = (byte)(128U | ((num4 >> 6) & 63U));
					bytes[num3++] = (byte)(128U | (num4 & 63U));
				}
			}
			if (flush && num2 != 0U)
			{
				if (num3 + 3 > num)
				{
					throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
				}
				bytes[num3++] = (byte)(224U | (num2 >> 12));
				bytes[num3++] = (byte)(128U | ((num2 >> 6) & 63U));
				bytes[num3++] = (byte)(128U | (num2 & 63U));
				num2 = 0U;
			}
			leftOver = num2;
			return num3 - byteIndex;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			uint num = 0U;
			return UnixEncoding.InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, ref num, true);
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
				throw new ArgumentOutOfRangeException("charIndex", UnixEncoding._("ArgRange_StringIndex"));
			}
			if (charCount < 0 || charCount > s.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", UnixEncoding._("ArgRange_StringRange"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", UnixEncoding._("ArgRange_Array"));
			}
			char* ptr = s;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			byte* ptr2;
			if (bytes == null || bytes.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &bytes[0];
			}
			return this.GetBytes(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex);
		}

		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			if (bytes == null || chars == null)
			{
				throw new ArgumentNullException((bytes == null) ? "bytes" : "chars");
			}
			if (charCount < 0 || byteCount < 0)
			{
				throw new ArgumentOutOfRangeException((charCount < 0) ? "charCount" : "byteCount");
			}
			int num = 0;
			int num2 = 0;
			while (charCount > 0)
			{
				char c = chars[num2++];
				uint num3;
				if (c >= '\ud800' && c <= '\udbff' && charCount > 1)
				{
					num3 = (uint)chars[num2];
					if (num3 >= 56320U && num3 <= 57343U)
					{
						num3 = num3 - 56320U + (uint)((uint)(c - '\ud800') << 10) + 65536U;
						num2++;
						charCount--;
					}
					else
					{
						num3 = (uint)c;
					}
				}
				else if (c == UnixEncoding.EscapeByte && charCount > 1)
				{
					if (num >= byteCount)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					charCount -= 2;
					if (charCount >= 0)
					{
						bytes[num++] = (byte)chars[num2++];
						continue;
					}
					continue;
				}
				else
				{
					num3 = (uint)c;
				}
				charCount--;
				if (num3 < 128U)
				{
					if (num >= byteCount)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)num3;
				}
				else if (num3 < 2048U)
				{
					if (num + 2 > byteCount)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)(192U | (num3 >> 6));
					bytes[num++] = (byte)(128U | (num3 & 63U));
				}
				else if (num3 < 65536U)
				{
					if (num + 3 > byteCount)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)(224U | (num3 >> 12));
					bytes[num++] = (byte)(128U | ((num3 >> 6) & 63U));
					bytes[num++] = (byte)(128U | (num3 & 63U));
				}
				else
				{
					if (num + 4 > byteCount)
					{
						throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)(240U | (num3 >> 18));
					bytes[num++] = (byte)(128U | ((num3 >> 12) & 63U));
					bytes[num++] = (byte)(128U | ((num3 >> 6) & 63U));
					bytes[num++] = (byte)(128U | (num3 & 63U));
				}
			}
			return num;
		}

		private static int InternalGetCharCount(byte[] bytes, int index, int count, uint leftOverBits, uint leftOverCount, bool throwOnInvalid, bool flush)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", UnixEncoding._("ArgRange_Array"));
			}
			if (count < 0 || count > bytes.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", UnixEncoding._("ArgRange_Array"));
			}
			int num = 0;
			int num2 = 0;
			uint num3 = leftOverBits;
			uint num4 = leftOverCount & 15U;
			uint num5 = (leftOverCount >> 4) & 15U;
			while (count > 0)
			{
				uint num6 = (uint)bytes[index++];
				num++;
				count--;
				if (num5 == 0U)
				{
					if (num6 < 128U)
					{
						num2++;
						num = 0;
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
						num2 += num * 2;
						num = 0;
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
								num2 += num * 2;
							}
							else
							{
								num2++;
							}
						}
						else if (num3 < 1114112U)
						{
							num2 += 2;
						}
						else if (throwOnInvalid)
						{
							num2 += num * 2;
						}
						num5 = 0U;
						num = 0;
					}
				}
				else
				{
					if (num6 < 128U)
					{
						index--;
						count++;
						num--;
					}
					num2 += num * 2;
					num5 = 0U;
					num = 0;
				}
			}
			if (flush && num5 > 0U && throwOnInvalid)
			{
				num2 += num * 2;
			}
			return num2;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return UnixEncoding.InternalGetCharCount(bytes, index, count, 0U, 0U, true, true);
		}

		private static int InternalGetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, ref uint leftOverBits, ref uint leftOverCount, bool throwOnInvalid, bool flush)
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
				throw new ArgumentOutOfRangeException("byteIndex", UnixEncoding._("ArgRange_Array"));
			}
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteCount", UnixEncoding._("ArgRange_Array"));
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", UnixEncoding._("ArgRange_Array"));
			}
			if (charIndex == chars.Length)
			{
				return 0;
			}
			byte[] array = new byte[6];
			int num = 0;
			int num2 = chars.Length;
			int num3 = charIndex;
			uint num4 = leftOverBits;
			uint num5 = leftOverCount & 15U;
			uint num6 = (leftOverCount >> 4) & 15U;
			while (byteCount > 0)
			{
				uint num7 = (uint)bytes[byteIndex++];
				array[num++] = (byte)num7;
				byteCount--;
				if (num6 == 0U)
				{
					if (num7 < 128U)
					{
						if (num3 >= num2)
						{
							throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "chars");
						}
						num = 0;
						chars[num3++] = (char)num7;
					}
					else if ((num7 & 224U) == 192U)
					{
						num4 = num7 & 31U;
						num5 = 1U;
						num6 = 2U;
					}
					else if ((num7 & 240U) == 224U)
					{
						num4 = num7 & 15U;
						num5 = 1U;
						num6 = 3U;
					}
					else if ((num7 & 248U) == 240U)
					{
						num4 = num7 & 7U;
						num5 = 1U;
						num6 = 4U;
					}
					else if ((num7 & 252U) == 248U)
					{
						num4 = num7 & 3U;
						num5 = 1U;
						num6 = 5U;
					}
					else if ((num7 & 254U) == 252U)
					{
						num4 = num7 & 3U;
						num5 = 1U;
						num6 = 6U;
					}
					else
					{
						num = 0;
						chars[num3++] = UnixEncoding.EscapeByte;
						chars[num3++] = (char)num7;
					}
				}
				else if ((num7 & 192U) == 128U)
				{
					num4 = (num4 << 6) | (num7 & 63U);
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
								UnixEncoding.CopyRaw(array, ref num, chars, ref num3, num2);
							}
							else
							{
								if (num3 >= num2)
								{
									throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "chars");
								}
								chars[num3++] = (char)num4;
							}
						}
						else if (num4 < 1114112U)
						{
							if (num3 + 2 > num2)
							{
								throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "chars");
							}
							num4 -= 65536U;
							chars[num3++] = (char)((num4 >> 10) + 55296U);
							chars[num3++] = (char)((num4 & 1023U) + 56320U);
						}
						else if (throwOnInvalid)
						{
							UnixEncoding.CopyRaw(array, ref num, chars, ref num3, num2);
						}
						num6 = 0U;
						num = 0;
					}
				}
				else
				{
					if (num7 < 128U)
					{
						byteIndex--;
						byteCount++;
						num--;
					}
					UnixEncoding.CopyRaw(array, ref num, chars, ref num3, num2);
					num6 = 0U;
					num = 0;
				}
			}
			if (flush && num6 > 0U && throwOnInvalid)
			{
				UnixEncoding.CopyRaw(array, ref num, chars, ref num3, num2);
			}
			leftOverBits = num4;
			leftOverCount = num5 | (num6 << 4);
			return num3 - charIndex;
		}

		private static void CopyRaw(byte[] raw, ref int next_raw, char[] chars, ref int posn, int length)
		{
			if (posn + next_raw * 2 > length)
			{
				throw new ArgumentException(UnixEncoding._("Arg_InsufficientSpace"), "chars");
			}
			for (int i = 0; i < next_raw; i++)
			{
				int num = posn;
				posn = num + 1;
				chars[num] = UnixEncoding.EscapeByte;
				num = posn;
				posn = num + 1;
				chars[num] = (char)raw[i];
			}
			next_raw = 0;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			uint num = 0U;
			uint num2 = 0U;
			return UnixEncoding.InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref num, ref num2, true, true);
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", UnixEncoding._("ArgRange_NonNegative"));
			}
			return charCount * 4;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", UnixEncoding._("ArgRange_NonNegative"));
			}
			return byteCount;
		}

		public override Decoder GetDecoder()
		{
			return new UnixEncoding.UnixDecoder();
		}

		public override Encoder GetEncoder()
		{
			return new UnixEncoding.UnixEncoder();
		}

		public override byte[] GetPreamble()
		{
			return new byte[0];
		}

		public override bool Equals(object value)
		{
			return value is UnixEncoding;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override byte[] GetBytes(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			byte[] array = new byte[this.GetByteCount(s)];
			this.GetBytes(s, 0, s.Length, array, 0);
			return array;
		}

		private static string _(string arg)
		{
			return arg;
		}

		public static readonly Encoding Instance = new UnixEncoding();

		public static readonly char EscapeByte = '\0';

		[Serializable]
		private class UnixDecoder : Decoder
		{
			public UnixDecoder()
			{
				this.leftOverBits = 0U;
				this.leftOverCount = 0U;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return UnixEncoding.InternalGetCharCount(bytes, index, count, this.leftOverBits, this.leftOverCount, true, false);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				return UnixEncoding.InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref this.leftOverBits, ref this.leftOverCount, true, false);
			}

			private uint leftOverBits;

			private uint leftOverCount;
		}

		[Serializable]
		private class UnixEncoder : Encoder
		{
			public UnixEncoder()
			{
				this.leftOver = 0U;
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return UnixEncoding.InternalGetByteCount(chars, index, count, this.leftOver, flush);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteCount, bool flush)
			{
				return UnixEncoding.InternalGetBytes(chars, charIndex, charCount, bytes, byteCount, ref this.leftOver, flush);
			}

			private uint leftOver;
		}
	}
}
