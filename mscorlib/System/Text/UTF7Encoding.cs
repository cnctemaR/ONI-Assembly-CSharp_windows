using System;
using System.Runtime.InteropServices;

namespace System.Text
{
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with .NET")]
	[Serializable]
	public class UTF7Encoding : Encoding
	{
		public UTF7Encoding()
			: this(false)
		{
		}

		public UTF7Encoding(bool allowOptionals)
			: base(65000)
		{
			this.allowOptionals = allowOptionals;
			this.body_name = "utf-7";
			this.encoding_name = "Unicode (UTF-7)";
			this.header_name = "utf-7";
			this.is_mail_news_display = true;
			this.is_mail_news_save = true;
			this.web_name = "utf-7";
			this.windows_code_page = 1200;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			return (!this.allowOptionals) ? hashCode : (-hashCode);
		}

		[ComVisible(false)]
		public override bool Equals(object value)
		{
			UTF7Encoding utf7Encoding = value as UTF7Encoding;
			return utf7Encoding != null && (this.allowOptionals == utf7Encoding.allowOptionals && base.EncoderFallback.Equals(utf7Encoding.EncoderFallback)) && base.DecoderFallback.Equals(utf7Encoding.DecoderFallback);
		}

		private static int InternalGetByteCount(char[] chars, int index, int count, bool flush, int leftOver, bool isInShifted, bool allowOptionals)
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
			int i = leftOver >> 8;
			byte[] array = UTF7Encoding.encodingRules;
			while (count > 0)
			{
				int num2 = (int)chars[index++];
				count--;
				int num3;
				if (num2 < 128)
				{
					num3 = (int)array[num2];
				}
				else
				{
					num3 = 0;
				}
				switch (num3)
				{
				case 0:
					break;
				case 1:
					goto IL_00E3;
				case 2:
					if (allowOptionals)
					{
						goto IL_00E3;
					}
					break;
				case 3:
					if (isInShifted)
					{
						if (i != 0)
						{
							num++;
							i = 0;
						}
						num++;
						isInShifted = false;
					}
					num += 2;
					continue;
				default:
					continue;
				}
				if (!isInShifted)
				{
					num++;
					i = 0;
					isInShifted = true;
				}
				for (i += 16; i >= 6; i -= 6)
				{
					num++;
				}
				continue;
				IL_00E3:
				if (isInShifted)
				{
					if (i != 0)
					{
						num++;
						i = 0;
					}
					num++;
					isInShifted = false;
				}
				num++;
			}
			if (isInShifted && flush)
			{
				if (i != 0)
				{
					num++;
				}
				num++;
			}
			return num;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return UTF7Encoding.InternalGetByteCount(chars, index, count, true, 0, false, this.allowOptionals);
		}

		private static int InternalGetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush, ref int leftOver, ref bool isInShifted, bool allowOptionals)
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
			int num = byteIndex;
			int num2 = bytes.Length;
			int i = leftOver >> 8;
			int num3 = leftOver & 255;
			byte[] array = UTF7Encoding.encodingRules;
			string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
			while (charCount > 0)
			{
				int num4 = (int)chars[charIndex++];
				charCount--;
				int num5;
				if (num4 < 128)
				{
					num5 = (int)array[num4];
				}
				else
				{
					num5 = 0;
				}
				switch (num5)
				{
				case 0:
					break;
				case 1:
					goto IL_019D;
				case 2:
					if (allowOptionals)
					{
						goto IL_019D;
					}
					break;
				case 3:
					if (isInShifted)
					{
						if (i != 0)
						{
							if (num + 1 > num2)
							{
								throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
							}
							bytes[num++] = (byte)text[num3 << 6 - i];
						}
						if (num + 1 > num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
						}
						bytes[num++] = 45;
						isInShifted = false;
						i = 0;
						num3 = 0;
					}
					if (num + 2 > num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = 43;
					bytes[num++] = 45;
					continue;
				default:
					continue;
				}
				if (!isInShifted)
				{
					if (num >= num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = 43;
					isInShifted = true;
					i = 0;
				}
				num3 = (num3 << 16) | num4;
				i += 16;
				while (i >= 6)
				{
					if (num >= num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
					}
					i -= 6;
					bytes[num++] = (byte)text[num3 >> i];
					num3 &= (1 << i) - 1;
				}
				continue;
				IL_019D:
				if (isInShifted)
				{
					if (i != 0)
					{
						if (num + 1 > num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
						}
						bytes[num++] = (byte)text[num3 << 6 - i];
					}
					if (num + 1 > num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = 45;
					isInShifted = false;
					i = 0;
					num3 = 0;
				}
				if (num >= num2)
				{
					throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
				}
				bytes[num++] = (byte)num4;
			}
			if (isInShifted && flush)
			{
				if (i != 0)
				{
					if (num + 1 > num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)text[num3 << 6 - i];
				}
				bytes[num++] = 45;
				i = 0;
				num3 = 0;
				isInShifted = false;
			}
			leftOver = (i << 8) | num3;
			return num - byteIndex;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			int num = 0;
			bool flag = false;
			return UTF7Encoding.InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, true, ref num, ref flag, this.allowOptionals);
		}

		private static int InternalGetCharCount(byte[] bytes, int index, int count, int leftOver)
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
			int num = 0;
			bool flag = (leftOver & 16777216) == 0;
			bool flag2 = (leftOver & 33554432) != 0;
			int num2 = (leftOver >> 16) & 255;
			sbyte[] array = UTF7Encoding.base64Values;
			while (count > 0)
			{
				int num3 = (int)bytes[index++];
				count--;
				if (flag)
				{
					if (num3 != 43)
					{
						num++;
					}
					else
					{
						flag = false;
						flag2 = true;
					}
				}
				else
				{
					if (num3 == 45)
					{
						if (flag2)
						{
							num++;
						}
						num2 = 0;
						flag = true;
					}
					else if ((int)array[num3] != -1)
					{
						num2 += 6;
						if (num2 >= 16)
						{
							num++;
							num2 -= 16;
						}
					}
					else
					{
						num++;
						flag = true;
						num2 = 0;
					}
					flag2 = false;
				}
			}
			return num;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return UTF7Encoding.InternalGetCharCount(bytes, index, count, 0);
		}

		private static int InternalGetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, ref int leftOver)
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
			int num2 = chars.Length;
			bool flag = (leftOver & 16777216) == 0;
			bool flag2 = (leftOver & 33554432) != 0;
			bool flag3 = (leftOver & 67108864) != 0;
			int num3 = (leftOver >> 16) & 255;
			int num4 = leftOver & 65535;
			sbyte[] array = UTF7Encoding.base64Values;
			while (byteCount > 0)
			{
				int num5 = (int)bytes[byteIndex++];
				byteCount--;
				if (flag)
				{
					if (num5 != 43)
					{
						if (num >= num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
						}
						if (flag3)
						{
							throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
						}
						chars[num++] = (char)num5;
					}
					else
					{
						flag = false;
						flag2 = true;
					}
				}
				else
				{
					int num6;
					if (num5 == 45)
					{
						if (flag2)
						{
							if (num >= num2)
							{
								throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
							}
							if (flag3)
							{
								throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
							}
							chars[num++] = '+';
						}
						flag = true;
						num3 = 0;
						num4 = 0;
					}
					else if ((num6 = (int)array[num5]) != -1)
					{
						num4 = (num4 << 6) | num6;
						num3 += 6;
						if (num3 >= 16)
						{
							if (num >= num2)
							{
								throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
							}
							num3 -= 16;
							char c = (char)(num4 >> num3);
							if ((c & 'ﰀ') == '\ud800')
							{
								flag3 = true;
							}
							else if ((c & 'ﰀ') == '\udc00')
							{
								if (!flag3)
								{
									throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
								}
								flag3 = false;
							}
							chars[num++] = c;
							num4 &= (1 << num3) - 1;
						}
					}
					else
					{
						if (num >= num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
						}
						if (flag3)
						{
							throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
						}
						chars[num++] = (char)num5;
						flag = true;
						num3 = 0;
						num4 = 0;
					}
					flag2 = false;
				}
			}
			leftOver = num4 | (num3 << 16) | ((!flag) ? 16777216 : 0) | ((!flag2) ? 0 : 33554432) | ((!flag3) ? 0 : 67108864);
			return num - charIndex;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			int num = 0;
			int num2 = UTF7Encoding.InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref num);
			if ((num & 67108864) != 0)
			{
				throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
			}
			return num2;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_NonNegative"));
			}
			if (charCount == 0)
			{
				return 0;
			}
			return 8 * (charCount / 3) + charCount % 3 * 3 + 2;
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
			return new UTF7Encoding.UTF7Decoder();
		}

		public override Encoder GetEncoder()
		{
			return new UTF7Encoding.UTF7Encoder(this.allowOptionals);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetByteCount(char* chars, int count)
		{
			return base.GetByteCount(chars, count);
		}

		[ComVisible(false)]
		public override int GetByteCount(string s)
		{
			return base.GetByteCount(s);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			return base.GetBytes(chars, charCount, bytes, byteCount);
		}

		[ComVisible(false)]
		public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return base.GetBytes(s, charIndex, charCount, bytes, byteIndex);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			return base.GetCharCount(bytes, count);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			return base.GetChars(bytes, byteCount, chars, charCount);
		}

		[ComVisible(false)]
		public override string GetString(byte[] bytes, int index, int count)
		{
			return base.GetString(bytes, index, count);
		}

		internal const int UTF7_CODE_PAGE = 65000;

		private const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

		private bool allowOptionals;

		private static readonly byte[] encodingRules = new byte[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
			1, 0, 0, 1, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 1, 2, 2, 2, 2, 2, 2, 1,
			1, 1, 2, 3, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 2,
			2, 2, 2, 1, 2, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 2, 0, 2, 2, 2, 2, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 2, 2, 2, 0, 0
		};

		private static readonly sbyte[] base64Values = new sbyte[]
		{
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, 62, -1, -1, -1, 63, 52, 53,
			54, 55, 56, 57, 58, 59, 60, 61, -1, -1,
			-1, -1, -1, -1, -1, 0, 1, 2, 3, 4,
			5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
			25, -1, -1, -1, -1, -1, -1, 26, 27, 28,
			29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
			39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
			49, 50, 51, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1
		};

		private sealed class UTF7Decoder : Decoder
		{
			public UTF7Decoder()
			{
				this.leftOver = 0;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return UTF7Encoding.InternalGetCharCount(bytes, index, count, this.leftOver);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				return UTF7Encoding.InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref this.leftOver);
			}

			private int leftOver;
		}

		private sealed class UTF7Encoder : Encoder
		{
			public UTF7Encoder(bool allowOptionals)
			{
				this.allowOptionals = allowOptionals;
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return UTF7Encoding.InternalGetByteCount(chars, index, count, flush, this.leftOver, this.isInShifted, this.allowOptionals);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
			{
				return UTF7Encoding.InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, flush, ref this.leftOver, ref this.isInShifted, this.allowOptionals);
			}

			private bool allowOptionals;

			private int leftOver;

			private bool isInShifted;
		}
	}
}
