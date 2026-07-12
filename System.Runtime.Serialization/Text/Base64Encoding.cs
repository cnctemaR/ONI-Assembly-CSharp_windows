using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.Text
{
	internal class Base64Encoding : Encoding
	{
		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charCount", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (charCount % 4 != 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Base64 sequence length ({0}) not valid. Must be a multiple of 4.", new object[] { charCount.ToString(NumberFormatInfo.CurrentInfo) })));
			}
			return charCount / 4 * 3;
		}

		private bool IsValidLeadBytes(int v1, int v2, int v3, int v4)
		{
			return (v1 | v2) < 64 && (v3 | v4) != 255;
		}

		private bool IsValidTailBytes(int v3, int v4)
		{
			return v3 != 64 || v4 == 64;
		}

		[SecuritySafeCritical]
		public unsafe override int GetByteCount(char[] chars, int index, int count)
		{
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (index < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (index > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > chars.Length - index)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - index })));
			}
			if (count == 0)
			{
				return 0;
			}
			if (count % 4 != 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Base64 sequence length ({0}) not valid. Must be a multiple of 4.", new object[] { count.ToString(NumberFormatInfo.CurrentInfo) })));
			}
			byte[] array;
			byte* ptr;
			if ((array = Base64Encoding.char2val) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			fixed (char* ptr2 = &chars[index])
			{
				char* ptr3 = ptr2;
				int num = 0;
				char* ptr4 = ptr3;
				char* ptr5 = ptr3 + count;
				while (ptr4 < ptr5)
				{
					char c = *ptr4;
					char c2 = ptr4[1];
					char c3 = ptr4[2];
					char c4 = ptr4[3];
					if ((c | c2 | c3 | c4) >= '\u0080')
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("The characters '{0}' at offset {1} are not a valid Base64 sequence.", new object[]
						{
							new string(ptr4, 0, 4),
							index + (int)((long)(ptr4 - ptr3))
						})));
					}
					int num2 = (int)ptr[c];
					int num3 = (int)ptr[c2];
					int num4 = (int)ptr[c3];
					int num5 = (int)ptr[c4];
					if (!this.IsValidLeadBytes(num2, num3, num4, num5) || !this.IsValidTailBytes(num4, num5))
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("The characters '{0}' at offset {1} are not a valid Base64 sequence.", new object[]
						{
							new string(ptr4, 0, 4),
							index + (int)((long)(ptr4 - ptr3))
						})));
					}
					int num6 = ((num5 != 64) ? 3 : ((num4 != 64) ? 2 : 1));
					num += num6;
					ptr4 += 4;
				}
				return num;
			}
		}

		[SecuritySafeCritical]
		public unsafe override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (charIndex < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charIndex", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (charIndex > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charIndex", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (charCount < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charCount", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (charCount > chars.Length - charIndex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charCount", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - charIndex })));
			}
			if (bytes == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("bytes"));
			}
			if (byteIndex < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteIndex", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (byteIndex > bytes.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteIndex", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { bytes.Length })));
			}
			if (charCount == 0)
			{
				return 0;
			}
			if (charCount % 4 != 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Base64 sequence length ({0}) not valid. Must be a multiple of 4.", new object[] { charCount.ToString(NumberFormatInfo.CurrentInfo) })));
			}
			byte[] array;
			byte* ptr;
			if ((array = Base64Encoding.char2val) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			fixed (char* ptr2 = &chars[charIndex])
			{
				char* ptr3 = ptr2;
				fixed (byte* ptr4 = &bytes[byteIndex])
				{
					byte* ptr5 = ptr4;
					char* ptr6 = ptr3;
					char* ptr7 = ptr3 + charCount;
					byte* ptr8 = ptr5;
					byte* ptr9 = ptr5 + bytes.Length - byteIndex;
					while (ptr6 < ptr7)
					{
						char c = *ptr6;
						char c2 = ptr6[1];
						char c3 = ptr6[2];
						char c4 = ptr6[3];
						if ((c | c2 | c3 | c4) >= '\u0080')
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("The characters '{0}' at offset {1} are not a valid Base64 sequence.", new object[]
							{
								new string(ptr6, 0, 4),
								charIndex + (int)((long)(ptr6 - ptr3))
							})));
						}
						int num = (int)ptr[c];
						int num2 = (int)ptr[c2];
						int num3 = (int)ptr[c3];
						int num4 = (int)ptr[c4];
						if (!this.IsValidLeadBytes(num, num2, num3, num4) || !this.IsValidTailBytes(num3, num4))
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("The characters '{0}' at offset {1} are not a valid Base64 sequence.", new object[]
							{
								new string(ptr6, 0, 4),
								charIndex + (int)((long)(ptr6 - ptr3))
							})));
						}
						int num5 = ((num4 != 64) ? 3 : ((num3 != 64) ? 2 : 1));
						if (ptr8 + num5 != ptr9)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Array too small."), "bytes"));
						}
						*ptr8 = (byte)((num << 2) | ((num2 >> 4) & 3));
						if (num5 > 1)
						{
							ptr8[1] = (byte)((num2 << 4) | ((num3 >> 2) & 15));
							if (num5 > 2)
							{
								ptr8[2] = (byte)((num3 << 6) | (num4 & 63));
							}
						}
						ptr8 += num5;
						ptr6 += 4;
					}
					return (int)((long)(ptr8 - ptr5));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe virtual int GetBytes(byte[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (charIndex < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charIndex", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (charIndex > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charIndex", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (charCount < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charCount", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (charCount > chars.Length - charIndex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charCount", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - charIndex })));
			}
			if (bytes == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("bytes"));
			}
			if (byteIndex < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteIndex", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (byteIndex > bytes.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteIndex", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { bytes.Length })));
			}
			if (charCount == 0)
			{
				return 0;
			}
			if (charCount % 4 != 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Base64 sequence length ({0}) not valid. Must be a multiple of 4.", new object[] { charCount.ToString(NumberFormatInfo.CurrentInfo) })));
			}
			byte[] array;
			byte* ptr;
			if ((array = Base64Encoding.char2val) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			fixed (byte* ptr2 = &chars[charIndex])
			{
				byte* ptr3 = ptr2;
				fixed (byte* ptr4 = &bytes[byteIndex])
				{
					byte* ptr5 = ptr4;
					byte* ptr6 = ptr3;
					byte* ptr7 = ptr3 + charCount;
					byte* ptr8 = ptr5;
					byte* ptr9 = ptr5 + bytes.Length - byteIndex;
					while (ptr6 < ptr7)
					{
						byte b = *ptr6;
						byte b2 = ptr6[1];
						byte b3 = ptr6[2];
						byte b4 = ptr6[3];
						if ((b | b2 | b3 | b4) >= 128)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("The characters '{0}' at offset {1} are not a valid Base64 sequence.", new object[]
							{
								new string((sbyte*)ptr6, 0, 4),
								charIndex + (int)((long)(ptr6 - ptr3))
							})));
						}
						int num = (int)ptr[b];
						int num2 = (int)ptr[b2];
						int num3 = (int)ptr[b3];
						int num4 = (int)ptr[b4];
						if (!this.IsValidLeadBytes(num, num2, num3, num4) || !this.IsValidTailBytes(num3, num4))
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("The characters '{0}' at offset {1} are not a valid Base64 sequence.", new object[]
							{
								new string((sbyte*)ptr6, 0, 4),
								charIndex + (int)((long)(ptr6 - ptr3))
							})));
						}
						int num5 = ((num4 != 64) ? 3 : ((num3 != 64) ? 2 : 1));
						if (ptr8 + num5 != ptr9)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Array too small."), "bytes"));
						}
						*ptr8 = (byte)((num << 2) | ((num2 >> 4) & 3));
						if (num5 > 1)
						{
							ptr8[1] = (byte)((num2 << 4) | ((num3 >> 2) & 15));
							if (num5 > 2)
							{
								ptr8[2] = (byte)((num3 << 6) | (num4 & 63));
							}
						}
						ptr8 += num5;
						ptr6 += 4;
					}
					return (int)((long)(ptr8 - ptr5));
				}
			}
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0 || byteCount > 1610612731)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteCount", global::System.Runtime.Serialization.SR.GetString("The value of this argument must fall within the range {0} to {1}.", new object[] { 0, 1610612731 })));
			}
			return (byteCount + 2) / 3 * 4;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return this.GetMaxCharCount(count);
		}

		[SecuritySafeCritical]
		public unsafe override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			if (bytes == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("bytes"));
			}
			if (byteIndex < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteIndex", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (byteIndex > bytes.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteIndex", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { bytes.Length })));
			}
			if (byteCount < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteCount", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (byteCount > bytes.Length - byteIndex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteCount", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { bytes.Length - byteIndex })));
			}
			int charCount = this.GetCharCount(bytes, byteIndex, byteCount);
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (charIndex < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charIndex", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (charIndex > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charIndex", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Array too small."), "chars"));
			}
			if (byteCount > 0)
			{
				fixed (string text = Base64Encoding.val2char)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					fixed (byte* ptr2 = &bytes[byteIndex])
					{
						byte* ptr3 = ptr2;
						fixed (char* ptr4 = &chars[charIndex])
						{
							char* ptr5 = ptr4;
							byte* ptr6 = ptr3;
							byte* ptr7 = ptr6 + byteCount - 3;
							char* ptr8 = ptr5;
							while (ptr6 == ptr7)
							{
								*ptr8 = ptr[*ptr6 >> 2];
								ptr8[1] = ptr[((int)(*ptr6 & 3) << 4) | (ptr6[1] >> 4)];
								ptr8[2] = ptr[((int)(ptr6[1] & 15) << 2) | (ptr6[2] >> 6)];
								ptr8[3] = ptr[ptr6[2] & 63];
								ptr6 += 3;
								ptr8 += 4;
							}
							if ((long)(ptr6 - ptr7) == 2L)
							{
								*ptr8 = ptr[*ptr6 >> 2];
								ptr8[1] = ptr[(*ptr6 & 3) << 4];
								ptr8[2] = '=';
								ptr8[3] = '=';
							}
							else if ((long)(ptr6 - ptr7) == 1L)
							{
								*ptr8 = ptr[*ptr6 >> 2];
								ptr8[1] = ptr[((int)(*ptr6 & 3) << 4) | (ptr6[1] >> 4)];
								ptr8[2] = ptr[(ptr6[1] & 15) << 2];
								ptr8[3] = '=';
							}
						}
					}
				}
			}
			return charCount;
		}

		[SecuritySafeCritical]
		public unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount, byte[] chars, int charIndex)
		{
			if (bytes == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("bytes"));
			}
			if (byteIndex < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteIndex", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (byteIndex > bytes.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteIndex", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { bytes.Length })));
			}
			if (byteCount < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteCount", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (byteCount > bytes.Length - byteIndex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteCount", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { bytes.Length - byteIndex })));
			}
			int charCount = this.GetCharCount(bytes, byteIndex, byteCount);
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (charIndex < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charIndex", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (charIndex > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charIndex", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Array too small."), "chars"));
			}
			if (byteCount > 0)
			{
				byte[] array;
				byte* ptr;
				if ((array = Base64Encoding.val2byte) == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte* ptr2 = &bytes[byteIndex])
				{
					byte* ptr3 = ptr2;
					fixed (byte* ptr4 = &chars[charIndex])
					{
						byte* ptr5 = ptr4;
						byte* ptr6 = ptr3;
						byte* ptr7 = ptr6 + byteCount - 3;
						byte* ptr8 = ptr5;
						while (ptr6 == ptr7)
						{
							*ptr8 = ptr[*ptr6 >> 2];
							ptr8[1] = ptr[((int)(*ptr6 & 3) << 4) | (ptr6[1] >> 4)];
							ptr8[2] = ptr[((int)(ptr6[1] & 15) << 2) | (ptr6[2] >> 6)];
							ptr8[3] = ptr[ptr6[2] & 63];
							ptr6 += 3;
							ptr8 += 4;
						}
						if ((long)(ptr6 - ptr7) == 2L)
						{
							*ptr8 = ptr[*ptr6 >> 2];
							ptr8[1] = ptr[(*ptr6 & 3) << 4];
							ptr8[2] = 61;
							ptr8[3] = 61;
						}
						else if ((long)(ptr6 - ptr7) == 1L)
						{
							*ptr8 = ptr[*ptr6 >> 2];
							ptr8[1] = ptr[((int)(*ptr6 & 3) << 4) | (ptr6[1] >> 4)];
							ptr8[2] = ptr[(ptr6[1] & 15) << 2];
							ptr8[3] = 61;
						}
					}
				}
				array = null;
			}
			return charCount;
		}

		private static byte[] char2val = new byte[]
		{
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, 62, byte.MaxValue, byte.MaxValue, byte.MaxValue, 63, 52, 53,
			54, 55, 56, 57, 58, 59, 60, 61, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, 64, byte.MaxValue, byte.MaxValue, byte.MaxValue, 0, 1, 2, 3, 4,
			5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
			25, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 26, 27, 28,
			29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
			39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
			49, 50, 51, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
		};

		private static string val2char = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

		private static byte[] val2byte = new byte[]
		{
			65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
			75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
			85, 86, 87, 88, 89, 90, 97, 98, 99, 100,
			101, 102, 103, 104, 105, 106, 107, 108, 109, 110,
			111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
			121, 122, 48, 49, 50, 51, 52, 53, 54, 55,
			56, 57, 43, 47
		};
	}
}
