using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	public struct Unicode
	{
		public static bool IsValidCodePoint(int codepoint)
		{
			return codepoint <= 1114111 && codepoint >= 0;
		}

		public static bool NotTrailer(byte b)
		{
			return (b & 192) != 128;
		}

		public static Unicode.Rune ReplacementCharacter
		{
			get
			{
				return new Unicode.Rune
				{
					value = 65533
				};
			}
		}

		public static Unicode.Rune BadRune
		{
			get
			{
				return new Unicode.Rune
				{
					value = 0
				};
			}
		}

		public unsafe static ConversionError Utf8ToUcs(out Unicode.Rune rune, byte* buffer, ref int index, int capacity)
		{
			rune = Unicode.ReplacementCharacter;
			if (index + 1 > capacity)
			{
				return ConversionError.Overflow;
			}
			if ((buffer[index] & 128) == 0)
			{
				rune.value = (int)buffer[index];
				index++;
				return ConversionError.None;
			}
			if ((buffer[index] & 224) == 192)
			{
				if (index + 2 > capacity)
				{
					index++;
					return ConversionError.Overflow;
				}
				int num = (int)(buffer[index] & 31);
				num = (num << 6) | (int)(buffer[index + 1] & 63);
				if (num < 128 || Unicode.NotTrailer(buffer[index + 1]))
				{
					index++;
					return ConversionError.Encoding;
				}
				rune.value = num;
				index += 2;
				return ConversionError.None;
			}
			else if ((buffer[index] & 240) == 224)
			{
				if (index + 3 > capacity)
				{
					index++;
					return ConversionError.Overflow;
				}
				int num = (int)(buffer[index] & 15);
				num = (num << 6) | (int)(buffer[index + 1] & 63);
				num = (num << 6) | (int)(buffer[index + 2] & 63);
				if (num < 2048 || !Unicode.IsValidCodePoint(num) || Unicode.NotTrailer(buffer[index + 1]) || Unicode.NotTrailer(buffer[index + 2]))
				{
					index++;
					return ConversionError.Encoding;
				}
				rune.value = num;
				index += 3;
				return ConversionError.None;
			}
			else
			{
				if ((buffer[index] & 248) != 240)
				{
					index++;
					return ConversionError.Encoding;
				}
				if (index + 4 > capacity)
				{
					index++;
					return ConversionError.Overflow;
				}
				int num = (int)(buffer[index] & 7);
				num = (num << 6) | (int)(buffer[index + 1] & 63);
				num = (num << 6) | (int)(buffer[index + 2] & 63);
				num = (num << 6) | (int)(buffer[index + 3] & 63);
				if (num < 65536 || !Unicode.IsValidCodePoint(num) || Unicode.NotTrailer(buffer[index + 1]) || Unicode.NotTrailer(buffer[index + 2]) || Unicode.NotTrailer(buffer[index + 3]))
				{
					index++;
					return ConversionError.Encoding;
				}
				rune.value = num;
				index += 4;
				return ConversionError.None;
			}
		}

		private static bool IsLeadingSurrogate(char c)
		{
			return c >= '\ud800' && c <= '\udbff';
		}

		private static bool IsTrailingSurrogate(char c)
		{
			return c >= '\udc00' && c <= '\udfff';
		}

		public unsafe static ConversionError Utf16ToUcs(out Unicode.Rune rune, char* buffer, ref int index, int capacity)
		{
			rune = Unicode.ReplacementCharacter;
			if (index + 1 > capacity)
			{
				return ConversionError.Overflow;
			}
			if (!Unicode.IsLeadingSurrogate(buffer[index]) || index + 2 > capacity)
			{
				rune.value = (int)buffer[index];
				index++;
				return ConversionError.None;
			}
			int num = (int)(buffer[index] & 'Ͽ');
			if (!Unicode.IsTrailingSurrogate(buffer[index + 1]))
			{
				rune.value = (int)buffer[index];
				index++;
				return ConversionError.None;
			}
			num = (num << 10) | (int)(buffer[index + 1] & 'Ͽ');
			num += 65536;
			rune.value = num;
			index += 2;
			return ConversionError.None;
		}

		public unsafe static ConversionError UcsToUtf8(byte* buffer, ref int index, int capacity, Unicode.Rune rune)
		{
			if (!Unicode.IsValidCodePoint(rune.value))
			{
				return ConversionError.CodePoint;
			}
			if (index + 1 > capacity)
			{
				return ConversionError.Overflow;
			}
			if (rune.value <= 127)
			{
				int num = index;
				index = num + 1;
				buffer[num] = (byte)rune.value;
				return ConversionError.None;
			}
			if (rune.value <= 2047)
			{
				if (index + 2 > capacity)
				{
					return ConversionError.Overflow;
				}
				int num = index;
				index = num + 1;
				buffer[num] = (byte)(192 | (rune.value >> 6));
				num = index;
				index = num + 1;
				buffer[num] = (byte)(128 | (rune.value & 63));
				return ConversionError.None;
			}
			else if (rune.value <= 65535)
			{
				if (index + 3 > capacity)
				{
					return ConversionError.Overflow;
				}
				int num = index;
				index = num + 1;
				buffer[num] = (byte)(224 | (rune.value >> 12));
				num = index;
				index = num + 1;
				buffer[num] = (byte)(128 | ((rune.value >> 6) & 63));
				num = index;
				index = num + 1;
				buffer[num] = (byte)(128 | (rune.value & 63));
				return ConversionError.None;
			}
			else
			{
				if (rune.value > 2097151)
				{
					return ConversionError.Encoding;
				}
				if (index + 4 > capacity)
				{
					return ConversionError.Overflow;
				}
				int num = index;
				index = num + 1;
				buffer[num] = (byte)(240 | (rune.value >> 18));
				num = index;
				index = num + 1;
				buffer[num] = (byte)(128 | ((rune.value >> 12) & 63));
				num = index;
				index = num + 1;
				buffer[num] = (byte)(128 | ((rune.value >> 6) & 63));
				num = index;
				index = num + 1;
				buffer[num] = (byte)(128 | (rune.value & 63));
				return ConversionError.None;
			}
		}

		public unsafe static ConversionError UcsToUtf16(char* buffer, ref int index, int capacity, Unicode.Rune rune)
		{
			if (!Unicode.IsValidCodePoint(rune.value))
			{
				return ConversionError.CodePoint;
			}
			if (index + 1 > capacity)
			{
				return ConversionError.Overflow;
			}
			int num;
			if (rune.value < 65536)
			{
				num = index;
				index = num + 1;
				buffer[num] = (char)rune.value;
				return ConversionError.None;
			}
			if (index + 2 > capacity)
			{
				return ConversionError.Overflow;
			}
			int num2 = rune.value - 65536;
			if (num2 >= 1048576)
			{
				return ConversionError.Encoding;
			}
			num = index;
			index = num + 1;
			buffer[num] = (char)(55296 | (num2 >> 10));
			num = index;
			index = num + 1;
			buffer[num] = (char)(56320 | (num2 & 1023));
			return ConversionError.None;
		}

		public unsafe static ConversionError Utf16ToUtf8(char* utf16Buffer, int utf16Length, byte* utf8Buffer, out int utf8Length, int utf8Capacity)
		{
			utf8Length = 0;
			int i = 0;
			while (i < utf16Length)
			{
				Unicode.Rune rune;
				Unicode.Utf16ToUcs(out rune, utf16Buffer, ref i, utf16Length);
				if (Unicode.UcsToUtf8(utf8Buffer, ref utf8Length, utf8Capacity, rune) == ConversionError.Overflow)
				{
					return ConversionError.Overflow;
				}
			}
			return ConversionError.None;
		}

		public unsafe static ConversionError Utf8ToUtf8(byte* srcBuffer, int srcLength, byte* destBuffer, out int destLength, int destCapacity)
		{
			if (destCapacity >= srcLength)
			{
				UnsafeUtility.MemCpy((void*)destBuffer, (void*)srcBuffer, (long)srcLength);
				destLength = srcLength;
				return ConversionError.None;
			}
			destLength = 0;
			int i = 0;
			while (i < srcLength)
			{
				Unicode.Rune rune;
				Unicode.Utf8ToUcs(out rune, srcBuffer, ref i, srcLength);
				if (Unicode.UcsToUtf8(destBuffer, ref destLength, destCapacity, rune) == ConversionError.Overflow)
				{
					return ConversionError.Overflow;
				}
			}
			return ConversionError.None;
		}

		public unsafe static ConversionError Utf8ToUtf16(byte* utf8Buffer, int utf8Length, char* utf16Buffer, out int utf16Length, int utf16Capacity)
		{
			utf16Length = 0;
			int i = 0;
			while (i < utf8Length)
			{
				Unicode.Rune rune;
				Unicode.Utf8ToUcs(out rune, utf8Buffer, ref i, utf8Length);
				if (Unicode.UcsToUtf16(utf16Buffer, ref utf16Length, utf16Capacity, rune) == ConversionError.Overflow)
				{
					return ConversionError.Overflow;
				}
			}
			return ConversionError.None;
		}

		public const int kMaximumValidCodePoint = 1114111;

		[BurstCompatible]
		public struct Rune
		{
			public Rune(int codepoint)
			{
				this.value = codepoint;
			}

			public static explicit operator Unicode.Rune(char codepoint)
			{
				return new Unicode.Rune
				{
					value = (int)codepoint
				};
			}

			public static bool IsDigit(Unicode.Rune r)
			{
				return r.value >= 48 && r.value <= 57;
			}

			public int LengthInUtf8Bytes()
			{
				if (this.value < 0)
				{
					return 4;
				}
				if (this.value <= 127)
				{
					return 1;
				}
				if (this.value <= 2047)
				{
					return 2;
				}
				if (this.value <= 65535)
				{
					return 3;
				}
				int num = this.value;
				return 4;
			}

			public int value;
		}
	}
}
