using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.Text
{
	internal class BinHexEncoding : Encoding
	{
		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("charCount", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (charCount % 2 != 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("BinHex sequence length ({0}) not valid. Must be a multiple of 2.", new object[] { charCount.ToString(NumberFormatInfo.CurrentInfo) })));
			}
			return charCount / 2;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return this.GetMaxByteCount(count);
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
			int byteCount = this.GetByteCount(chars, charIndex, charCount);
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Array too small."), "bytes"));
			}
			if (charCount > 0)
			{
				byte[] array;
				byte* ptr;
				if ((array = BinHexEncoding.char2val) == null || array.Length == 0)
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
					fixed (char* ptr4 = &chars[charIndex])
					{
						char* ptr5 = ptr4;
						char* ptr6 = ptr5;
						char* ptr7 = ptr5 + charCount;
						byte* ptr8 = ptr3;
						while (ptr6 < ptr7)
						{
							char c = *ptr6;
							char c2 = ptr6[1];
							if ((c | c2) >= '\u0080')
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("The characters '{0}' at offset {1} are not a valid BinHex sequence.", new object[]
								{
									new string(ptr6, 0, 2),
									charIndex + (int)((long)(ptr6 - ptr5))
								})));
							}
							byte b = ptr[c];
							byte b2 = ptr[c2];
							if ((b | b2) == 255)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("The characters '{0}' at offset {1} are not a valid BinHex sequence.", new object[]
								{
									new string(ptr6, 0, 2),
									charIndex + (int)((long)(ptr6 - ptr5))
								})));
							}
							*ptr8 = (byte)(((int)b << 4) + (int)b2);
							ptr6 += 2;
							ptr8++;
						}
					}
				}
				array = null;
			}
			return byteCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0 || byteCount > 1073741823)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("byteCount", global::System.Runtime.Serialization.SR.GetString("The value of this argument must fall within the range {0} to {1}.", new object[] { 0, 1073741823 })));
			}
			return byteCount * 2;
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
				fixed (string text = BinHexEncoding.val2char)
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
							byte* ptr7 = ptr3 + byteCount;
							while (ptr6 < ptr7)
							{
								*ptr5 = ptr[*ptr6 >> 4];
								ptr5[1] = ptr[*ptr6 & 15];
								ptr6++;
								ptr5 += 2;
							}
						}
					}
				}
			}
			return charCount;
		}

		private static byte[] char2val = new byte[]
		{
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 0, 1,
			2, 3, 4, 5, 6, 7, 8, 9, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 10, 11, 12, 13, 14,
			15, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 10, 11, 12,
			13, 14, 15, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
			byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue
		};

		private static string val2char = "0123456789ABCDEF";
	}
}
