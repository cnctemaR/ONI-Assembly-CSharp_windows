using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.Xml
{
	public class UniqueId
	{
		public UniqueId()
			: this(Guid.NewGuid())
		{
		}

		public UniqueId(Guid guid)
			: this(guid.ToByteArray())
		{
		}

		public UniqueId(byte[] guid)
			: this(guid, 0)
		{
		}

		[SecuritySafeCritical]
		public unsafe UniqueId(byte[] guid, int offset)
		{
			if (guid == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("guid"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > guid.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { guid.Length })));
			}
			if (16 > guid.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Array too small.  Length of available data must be at least {0}.", new object[] { 16 }), "guid"));
			}
			fixed (byte* ptr = &guid[offset])
			{
				byte* ptr2 = ptr;
				this.idLow = this.UnsafeGetInt64(ptr2);
				this.idHigh = this.UnsafeGetInt64(ptr2 + 8);
			}
		}

		[SecuritySafeCritical]
		public unsafe UniqueId(string value)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			if (value.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("UniqueId cannot be zero length.")));
			}
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				this.UnsafeParse(ptr, value.Length);
			}
			this.s = value;
		}

		[SecuritySafeCritical]
		public unsafe UniqueId(char[] chars, int offset, int count)
		{
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > chars.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - offset })));
			}
			if (count == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("UniqueId cannot be zero length.")));
			}
			fixed (char* ptr = &chars[offset])
			{
				char* ptr2 = ptr;
				this.UnsafeParse(ptr2, count);
			}
			if (!this.IsGuid)
			{
				this.s = new string(chars, offset, count);
			}
		}

		public int CharArrayLength
		{
			[SecuritySafeCritical]
			get
			{
				if (this.s != null)
				{
					return this.s.Length;
				}
				return 45;
			}
		}

		[SecurityCritical]
		private unsafe int UnsafeDecode(short* char2val, char ch1, char ch2)
		{
			if ((ch1 | ch2) >= '\u0080')
			{
				return 256;
			}
			return (int)(char2val[(IntPtr)ch1] | char2val[(IntPtr)('\u0080' + ch2)]);
		}

		[SecurityCritical]
		private unsafe void UnsafeEncode(char* val2char, byte b, char* pch)
		{
			*pch = val2char[b >> 4];
			pch[1] = val2char[b & 15];
		}

		public bool IsGuid
		{
			get
			{
				return (this.idLow | this.idHigh) != 0L;
			}
		}

		[SecurityCritical]
		private unsafe void UnsafeParse(char* chars, int charCount)
		{
			if (charCount != 45 || *chars != 'u' || chars[1] != 'r' || chars[2] != 'n' || chars[3] != ':' || chars[4] != 'u' || chars[5] != 'u' || chars[6] != 'i' || chars[7] != 'd' || chars[8] != ':' || chars[17] != '-' || chars[22] != '-' || chars[27] != '-' || chars[32] != '-')
			{
				return;
			}
			byte* ptr = stackalloc byte[(UIntPtr)16];
			int num = 0;
			short[] array;
			short* ptr2;
			if ((array = UniqueId.char2val) == null || array.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &array[0];
			}
			short* ptr3 = ptr2;
			int num2 = this.UnsafeDecode(ptr3, chars[15], chars[16]);
			*ptr = (byte)num2;
			int num3 = num | num2;
			num2 = this.UnsafeDecode(ptr3, chars[13], chars[14]);
			ptr[1] = (byte)num2;
			int num4 = num3 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[11], chars[12]);
			ptr[2] = (byte)num2;
			int num5 = num4 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[9], chars[10]);
			ptr[3] = (byte)num2;
			int num6 = num5 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[20], chars[21]);
			ptr[4] = (byte)num2;
			int num7 = num6 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[18], chars[19]);
			ptr[5] = (byte)num2;
			int num8 = num7 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[25], chars[26]);
			ptr[6] = (byte)num2;
			int num9 = num8 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[23], chars[24]);
			ptr[7] = (byte)num2;
			int num10 = num9 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[28], chars[29]);
			ptr[8] = (byte)num2;
			int num11 = num10 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[30], chars[31]);
			ptr[9] = (byte)num2;
			int num12 = num11 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[33], chars[34]);
			ptr[10] = (byte)num2;
			int num13 = num12 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[35], chars[36]);
			ptr[11] = (byte)num2;
			int num14 = num13 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[37], chars[38]);
			ptr[12] = (byte)num2;
			int num15 = num14 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[39], chars[40]);
			ptr[13] = (byte)num2;
			int num16 = num15 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[41], chars[42]);
			ptr[14] = (byte)num2;
			int num17 = num16 | num2;
			num2 = this.UnsafeDecode(ptr3, chars[43], chars[44]);
			ptr[15] = (byte)num2;
			if ((num17 | num2) >= 256)
			{
				return;
			}
			this.idLow = this.UnsafeGetInt64(ptr);
			this.idHigh = this.UnsafeGetInt64(ptr + 8);
			array = null;
		}

		[SecuritySafeCritical]
		public unsafe int ToCharArray(char[] chars, int offset)
		{
			int charArrayLength = this.CharArrayLength;
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (charArrayLength > chars.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("chars", global::System.Runtime.Serialization.SR.GetString("Array too small.  Must be able to hold at least {0}.", new object[] { charArrayLength })));
			}
			if (this.s != null)
			{
				this.s.CopyTo(0, chars, offset, charArrayLength);
			}
			else
			{
				byte* ptr = stackalloc byte[(UIntPtr)16];
				this.UnsafeSetInt64(this.idLow, ptr);
				this.UnsafeSetInt64(this.idHigh, ptr + 8);
				fixed (char* ptr2 = &chars[offset])
				{
					char* ptr3 = ptr2;
					*ptr3 = 'u';
					ptr3[1] = 'r';
					ptr3[2] = 'n';
					ptr3[3] = ':';
					ptr3[4] = 'u';
					ptr3[5] = 'u';
					ptr3[6] = 'i';
					ptr3[7] = 'd';
					ptr3[8] = ':';
					ptr3[17] = '-';
					ptr3[22] = '-';
					ptr3[27] = '-';
					ptr3[32] = '-';
					fixed (string text = "0123456789abcdef")
					{
						char* ptr4 = text;
						if (ptr4 != null)
						{
							ptr4 += RuntimeHelpers.OffsetToStringData / 2;
						}
						char* ptr5 = ptr4;
						this.UnsafeEncode(ptr5, *ptr, ptr3 + 15);
						this.UnsafeEncode(ptr5, ptr[1], ptr3 + 13);
						this.UnsafeEncode(ptr5, ptr[2], ptr3 + 11);
						this.UnsafeEncode(ptr5, ptr[3], ptr3 + 9);
						this.UnsafeEncode(ptr5, ptr[4], ptr3 + 20);
						this.UnsafeEncode(ptr5, ptr[5], ptr3 + 18);
						this.UnsafeEncode(ptr5, ptr[6], ptr3 + 25);
						this.UnsafeEncode(ptr5, ptr[7], ptr3 + 23);
						this.UnsafeEncode(ptr5, ptr[8], ptr3 + 28);
						this.UnsafeEncode(ptr5, ptr[9], ptr3 + 30);
						this.UnsafeEncode(ptr5, ptr[10], ptr3 + 33);
						this.UnsafeEncode(ptr5, ptr[11], ptr3 + 35);
						this.UnsafeEncode(ptr5, ptr[12], ptr3 + 37);
						this.UnsafeEncode(ptr5, ptr[13], ptr3 + 39);
						this.UnsafeEncode(ptr5, ptr[14], ptr3 + 41);
						this.UnsafeEncode(ptr5, ptr[15], ptr3 + 43);
					}
				}
			}
			return charArrayLength;
		}

		public bool TryGetGuid(out Guid guid)
		{
			byte[] array = new byte[16];
			if (!this.TryGetGuid(array, 0))
			{
				guid = Guid.Empty;
				return false;
			}
			guid = new Guid(array);
			return true;
		}

		[SecuritySafeCritical]
		public unsafe bool TryGetGuid(byte[] buffer, int offset)
		{
			if (!this.IsGuid)
			{
				return false;
			}
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > buffer.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
			}
			if (16 > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("buffer", global::System.Runtime.Serialization.SR.GetString("Array too small.  Must be able to hold at least {0}.", new object[] { 16 })));
			}
			fixed (byte* ptr = &buffer[offset])
			{
				byte* ptr2 = ptr;
				this.UnsafeSetInt64(this.idLow, ptr2);
				this.UnsafeSetInt64(this.idHigh, ptr2 + 8);
			}
			return true;
		}

		[SecuritySafeCritical]
		public override string ToString()
		{
			if (this.s == null)
			{
				int charArrayLength = this.CharArrayLength;
				char[] array = new char[charArrayLength];
				this.ToCharArray(array, 0);
				this.s = new string(array, 0, charArrayLength);
			}
			return this.s;
		}

		public static bool operator ==(UniqueId id1, UniqueId id2)
		{
			if (id1 == null && id2 == null)
			{
				return true;
			}
			if (id1 == null || id2 == null)
			{
				return false;
			}
			if (id1.IsGuid && id2.IsGuid)
			{
				return id1.idLow == id2.idLow && id1.idHigh == id2.idHigh;
			}
			return id1.ToString() == id2.ToString();
		}

		public static bool operator !=(UniqueId id1, UniqueId id2)
		{
			return !(id1 == id2);
		}

		public override bool Equals(object obj)
		{
			return this == obj as UniqueId;
		}

		public override int GetHashCode()
		{
			if (this.IsGuid)
			{
				long num = this.idLow ^ this.idHigh;
				return (int)(num >> 32) ^ (int)num;
			}
			return this.ToString().GetHashCode();
		}

		[SecurityCritical]
		private unsafe long UnsafeGetInt64(byte* pb)
		{
			int num = this.UnsafeGetInt32(pb);
			return ((long)this.UnsafeGetInt32(pb + 4) << 32) | (long)((ulong)num);
		}

		[SecurityCritical]
		private unsafe int UnsafeGetInt32(byte* pb)
		{
			return ((((((int)pb[3] << 8) | (int)pb[2]) << 8) | (int)pb[1]) << 8) | (int)(*pb);
		}

		[SecurityCritical]
		private unsafe void UnsafeSetInt64(long value, byte* pb)
		{
			this.UnsafeSetInt32((int)value, pb);
			this.UnsafeSetInt32((int)(value >> 32), pb + 4);
		}

		[SecurityCritical]
		private unsafe void UnsafeSetInt32(int value, byte* pb)
		{
			*pb = (byte)value;
			value >>= 8;
			pb[1] = (byte)value;
			value >>= 8;
			pb[2] = (byte)value;
			value >>= 8;
			pb[3] = (byte)value;
		}

		private long idLow;

		private long idHigh;

		[SecurityCritical]
		private string s;

		private const int guidLength = 16;

		private const int uuidLength = 45;

		private static short[] char2val = new short[]
		{
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 0, 16,
			32, 48, 64, 80, 96, 112, 128, 144, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 160, 176, 192,
			208, 224, 240, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 0, 1, 2, 3,
			4, 5, 6, 7, 8, 9, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 10, 11, 12, 13, 14,
			15, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256, 256, 256, 256, 256, 256
		};

		private const string val2char = "0123456789abcdef";
	}
}
