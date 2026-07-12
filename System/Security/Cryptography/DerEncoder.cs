using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace System.Security.Cryptography
{
	internal static class DerEncoder
	{
		private static byte[] EncodeLength(int length)
		{
			byte b = (byte)length;
			if (length < 128)
			{
				return new byte[] { b };
			}
			if (length <= 255)
			{
				return new byte[] { 129, b };
			}
			int num = length >> 8;
			byte b2 = (byte)num;
			if (length <= 65535)
			{
				return new byte[] { 130, b2, b };
			}
			num >>= 8;
			byte b3 = (byte)num;
			if (length <= 16777215)
			{
				return new byte[] { 131, b3, b2, b };
			}
			num >>= 8;
			byte b4 = (byte)num;
			return new byte[] { 132, b4, b3, b2, b };
		}

		internal static byte[][] SegmentedEncodeBoolean(bool value)
		{
			byte[] array = new byte[] { value ? byte.MaxValue : 0 };
			return new byte[][]
			{
				new byte[] { 1 },
				new byte[] { 1 },
				array
			};
		}

		internal static byte[][] SegmentedEncodeUnsignedInteger(uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse<byte>(bytes);
			}
			return DerEncoder.SegmentedEncodeUnsignedInteger(bytes);
		}

		internal unsafe static byte[][] SegmentedEncodeUnsignedInteger(ReadOnlySpan<byte> bigEndianBytes)
		{
			int num = 0;
			int num2 = num + bigEndianBytes.Length;
			while (num < num2 && *bigEndianBytes[num] == 0)
			{
				num++;
			}
			if (num == num2)
			{
				num--;
			}
			int num3 = num2 - num;
			int num4 = ((*bigEndianBytes[num] > 127) ? 1 : 0);
			byte[] array = new byte[num3 + num4];
			bigEndianBytes.Slice(num, num3).CopyTo(new Span<byte>(array).Slice(num4));
			return new byte[][]
			{
				new byte[] { 2 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[][] SegmentedEncodeBitString(params byte[][][] childSegments)
		{
			return DerEncoder.SegmentedEncodeBitString(DerEncoder.ConcatenateArrays(childSegments));
		}

		internal static byte[][] SegmentedEncodeBitString(byte[] data)
		{
			return DerEncoder.SegmentedEncodeBitString(0, data);
		}

		internal static byte[][] SegmentedEncodeBitString(int unusedBits, byte[] data)
		{
			byte[] array = new byte[data.Length + 1];
			Buffer.BlockCopy(data, 0, array, 1, data.Length);
			array[0] = (byte)unusedBits;
			byte b = (byte)(-1 << unusedBits);
			byte[] array2 = array;
			int num = data.Length;
			array2[num] &= b;
			return new byte[][]
			{
				new byte[] { 3 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[][] SegmentedEncodeNamedBitList(byte[] bigEndianBytes, int namedBitsCount)
		{
			int num = -1;
			for (int i = Math.Min(bigEndianBytes.Length * 8 - 1, namedBitsCount - 1); i >= 0; i--)
			{
				int num2 = i / 8;
				int num3 = 7 - i % 8;
				int num4 = 1 << num3;
				if (((int)bigEndianBytes[num2] & num4) == num4)
				{
					num = i;
					break;
				}
			}
			byte[] array;
			if (num >= 0)
			{
				int num5 = num + 1;
				int num6 = (7 + num5) / 8;
				int num7 = 7 - num % 8;
				byte b = (byte)(-1 << num7);
				array = new byte[num6 + 1];
				array[0] = (byte)num7;
				Buffer.BlockCopy(bigEndianBytes, 0, array, 1, num6);
				byte[] array2 = array;
				int num8 = num6;
				array2[num8] &= b;
			}
			else
			{
				array = new byte[1];
			}
			return new byte[][]
			{
				new byte[] { 3 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[][] SegmentedEncodeOctetString(byte[] data)
		{
			return new byte[][]
			{
				new byte[] { 4 },
				DerEncoder.EncodeLength(data.Length),
				data
			};
		}

		internal static byte[][] SegmentedEncodeNull()
		{
			return DerEncoder.s_nullTlv;
		}

		internal static byte[] EncodeOid(string oidValue)
		{
			return DerEncoder.ConcatenateArrays(new byte[][][] { DerEncoder.SegmentedEncodeOid(oidValue) });
		}

		internal static byte[][] SegmentedEncodeOid(Oid oid)
		{
			return DerEncoder.SegmentedEncodeOid(oid.Value);
		}

		internal static byte[][] SegmentedEncodeOid(string oidValue)
		{
			if (string.IsNullOrEmpty(oidValue))
			{
				throw new CryptographicException("The OID value was invalid.");
			}
			if (oidValue.Length < 3)
			{
				throw new CryptographicException("The OID value was invalid.");
			}
			if (oidValue[1] != '.')
			{
				throw new CryptographicException("The OID value was invalid.");
			}
			int num;
			switch (oidValue[0])
			{
			case '0':
				num = 0;
				break;
			case '1':
				num = 1;
				break;
			case '2':
				num = 2;
				break;
			default:
				throw new CryptographicException("The OID value was invalid.");
			}
			int i = 2;
			BigInteger bigInteger = DerEncoder.ParseOidRid(oidValue, ref i);
			bigInteger += 40 * num;
			List<byte> list = new List<byte>(oidValue.Length / 2);
			DerEncoder.EncodeRid(list, ref bigInteger);
			while (i < oidValue.Length)
			{
				bigInteger = DerEncoder.ParseOidRid(oidValue, ref i);
				DerEncoder.EncodeRid(list, ref bigInteger);
			}
			return new byte[][]
			{
				new byte[] { 6 },
				DerEncoder.EncodeLength(list.Count),
				list.ToArray()
			};
		}

		internal static byte[][] SegmentedEncodeUtf8String(char[] chars)
		{
			return DerEncoder.SegmentedEncodeUtf8String(chars, 0, chars.Length);
		}

		internal static byte[][] SegmentedEncodeUtf8String(char[] chars, int offset, int count)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(chars, offset, count);
			return new byte[][]
			{
				new byte[] { 12 },
				DerEncoder.EncodeLength(bytes.Length),
				bytes
			};
		}

		internal static byte[][] ConstructSegmentedSequence(params byte[][][] items)
		{
			return DerEncoder.ConstructSegmentedSequence(items);
		}

		internal static byte[][] ConstructSegmentedSequence(IEnumerable<byte[][]> items)
		{
			byte[] array = DerEncoder.ConcatenateArrays(items);
			return new byte[][]
			{
				new byte[] { 48 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[][] ConstructSegmentedContextSpecificValue(int contextId, params byte[][][] items)
		{
			byte[] array = DerEncoder.ConcatenateArrays(items);
			byte b = (byte)(160 | contextId);
			return new byte[][]
			{
				new byte[] { b },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[][] ConstructSegmentedSet(params byte[][][] items)
		{
			byte[][][] array = (byte[][][])items.Clone();
			Array.Sort<byte[][]>(array, DerEncoder.AsnSetValueComparer.Instance);
			byte[] array2 = DerEncoder.ConcatenateArrays(array);
			return new byte[][]
			{
				new byte[] { 49 },
				DerEncoder.EncodeLength(array2.Length),
				array2
			};
		}

		internal static byte[][] ConstructSegmentedPresortedSet(params byte[][][] items)
		{
			byte[] array = DerEncoder.ConcatenateArrays(items);
			return new byte[][]
			{
				new byte[] { 49 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static bool IsValidPrintableString(char[] chars)
		{
			return DerEncoder.IsValidPrintableString(chars, 0, chars.Length);
		}

		internal static bool IsValidPrintableString(char[] chars, int offset, int count)
		{
			int num = count + offset;
			for (int i = offset; i < num; i++)
			{
				if (!DerEncoder.IsPrintableStringCharacter(chars[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal static byte[][] SegmentedEncodePrintableString(char[] chars)
		{
			return DerEncoder.SegmentedEncodePrintableString(chars, 0, chars.Length);
		}

		internal static byte[][] SegmentedEncodePrintableString(char[] chars, int offset, int count)
		{
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = (byte)chars[i + offset];
			}
			return new byte[][]
			{
				new byte[] { 19 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[][] SegmentedEncodeIA5String(char[] chars)
		{
			return DerEncoder.SegmentedEncodeIA5String(chars, 0, chars.Length);
		}

		internal static byte[][] SegmentedEncodeIA5String(char[] chars, int offset, int count)
		{
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				char c = chars[i + offset];
				if (c > '\u007f')
				{
					throw new CryptographicException("The string contains a character not in the 7 bit ASCII character set.");
				}
				array[i] = (byte)c;
			}
			return new byte[][]
			{
				new byte[] { 22 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[][] SegmentedEncodeUtcTime(DateTime utcTime)
		{
			byte[] array = new byte[13];
			int num = utcTime.Year;
			int num2 = utcTime.Month;
			int num3 = utcTime.Day;
			int num4 = utcTime.Hour;
			int num5 = utcTime.Minute;
			int num6 = utcTime.Second;
			array[1] = (byte)(48 + num % 10);
			num /= 10;
			array[0] = (byte)(48 + num % 10);
			array[3] = (byte)(48 + num2 % 10);
			num2 /= 10;
			array[2] = (byte)(48 + num2 % 10);
			array[5] = (byte)(48 + num3 % 10);
			num3 /= 10;
			array[4] = (byte)(48 + num3 % 10);
			array[7] = (byte)(48 + num4 % 10);
			num4 /= 10;
			array[6] = (byte)(48 + num4 % 10);
			array[9] = (byte)(48 + num5 % 10);
			num5 /= 10;
			array[8] = (byte)(48 + num5 % 10);
			array[11] = (byte)(48 + num6 % 10);
			num6 /= 10;
			array[10] = (byte)(48 + num6 % 10);
			array[12] = 90;
			return new byte[][]
			{
				new byte[] { 23 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[][] SegmentedEncodeGeneralizedTime(DateTime utcTime)
		{
			byte[] array = new byte[15];
			int num = utcTime.Year;
			int num2 = utcTime.Month;
			int num3 = utcTime.Day;
			int num4 = utcTime.Hour;
			int num5 = utcTime.Minute;
			int num6 = utcTime.Second;
			array[3] = (byte)(48 + num % 10);
			num /= 10;
			array[2] = (byte)(48 + num % 10);
			num /= 10;
			array[1] = (byte)(48 + num % 10);
			num /= 10;
			array[0] = (byte)(48 + num % 10);
			array[5] = (byte)(48 + num2 % 10);
			num2 /= 10;
			array[4] = (byte)(48 + num2 % 10);
			array[7] = (byte)(48 + num3 % 10);
			num3 /= 10;
			array[6] = (byte)(48 + num3 % 10);
			array[9] = (byte)(48 + num4 % 10);
			num4 /= 10;
			array[8] = (byte)(48 + num4 % 10);
			array[11] = (byte)(48 + num5 % 10);
			num5 /= 10;
			array[10] = (byte)(48 + num5 % 10);
			array[13] = (byte)(48 + num6 % 10);
			num6 /= 10;
			array[12] = (byte)(48 + num6 % 10);
			array[14] = 90;
			return new byte[][]
			{
				new byte[] { 24 },
				DerEncoder.EncodeLength(array.Length),
				array
			};
		}

		internal static byte[] ConstructSequence(params byte[][][] items)
		{
			return DerEncoder.ConstructSequence(items);
		}

		internal static byte[] ConstructSequence(IEnumerable<byte[][]> items)
		{
			int num = 0;
			foreach (byte[][] array in items)
			{
				foreach (byte[] array2 in array)
				{
					num += array2.Length;
				}
			}
			byte[] array3 = DerEncoder.EncodeLength(num);
			byte[] array4 = new byte[1 + array3.Length + num];
			array4[0] = 48;
			int num2 = 1;
			Buffer.BlockCopy(array3, 0, array4, num2, array3.Length);
			num2 += array3.Length;
			foreach (byte[][] array in items)
			{
				foreach (byte[] array5 in array)
				{
					Buffer.BlockCopy(array5, 0, array4, num2, array5.Length);
					num2 += array5.Length;
				}
			}
			return array4;
		}

		private static BigInteger ParseOidRid(string oidValue, ref int startIndex)
		{
			int num = oidValue.IndexOf('.', startIndex);
			if (num == -1)
			{
				num = oidValue.Length;
			}
			BigInteger bigInteger = BigInteger.Zero;
			for (int i = startIndex; i < num; i++)
			{
				bigInteger *= 10;
				bigInteger += DerEncoder.AtoI(oidValue[i]);
			}
			startIndex = num + 1;
			return bigInteger;
		}

		private static int AtoI(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (int)(c - '0');
			}
			throw new CryptographicException("The OID value was invalid.");
		}

		private static void EncodeRid(List<byte> encodedData, ref BigInteger rid)
		{
			BigInteger bigInteger = new BigInteger(128);
			BigInteger bigInteger2 = rid;
			Stack<byte> stack = new Stack<byte>();
			byte b = 0;
			do
			{
				BigInteger bigInteger3;
				bigInteger2 = BigInteger.DivRem(bigInteger2, bigInteger, out bigInteger3);
				byte b2 = (byte)bigInteger3;
				b2 |= b;
				b = 128;
				stack.Push(b2);
			}
			while (bigInteger2 != BigInteger.Zero);
			encodedData.AddRange(stack);
		}

		private static bool IsPrintableStringCharacter(char c)
		{
			if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
			{
				return true;
			}
			if (c <= ':')
			{
				switch (c)
				{
				case ' ':
				case '\'':
				case '(':
				case ')':
				case '+':
				case ',':
				case '-':
				case '.':
				case '/':
					break;
				case '!':
				case '"':
				case '#':
				case '$':
				case '%':
				case '&':
				case '*':
					return false;
				default:
					if (c != ':')
					{
						return false;
					}
					break;
				}
			}
			else if (c != '=' && c != '?')
			{
				return false;
			}
			return true;
		}

		private static byte[] ConcatenateArrays(params byte[][][] segments)
		{
			return DerEncoder.ConcatenateArrays(segments);
		}

		private static byte[] ConcatenateArrays(IEnumerable<byte[][]> segments)
		{
			int num = 0;
			foreach (byte[][] array in segments)
			{
				foreach (byte[] array2 in array)
				{
					num += array2.Length;
				}
			}
			byte[] array3 = new byte[num];
			int num2 = 0;
			foreach (byte[][] array in segments)
			{
				foreach (byte[] array4 in array)
				{
					Buffer.BlockCopy(array4, 0, array3, num2, array4.Length);
					num2 += array4.Length;
				}
			}
			return array3;
		}

		private const byte ConstructedFlag = 32;

		private const byte ConstructedSequenceTag = 48;

		private const byte ConstructedSetTag = 49;

		private static readonly byte[][] s_nullTlv = new byte[][]
		{
			new byte[] { 5 },
			new byte[1],
			Array.Empty<byte>()
		};

		private class AsnSetValueComparer : IComparer<byte[][]>, IComparer
		{
			public static DerEncoder.AsnSetValueComparer Instance { get; } = new DerEncoder.AsnSetValueComparer();

			public int Compare(byte[][] x, byte[][] y)
			{
				int num = (int)(x[0][0] - y[0][0]);
				if (num != 0)
				{
					return num;
				}
				num = x[2].Length - y[2].Length;
				if (num != 0)
				{
					return num;
				}
				for (int i = 0; i < x[2].Length; i++)
				{
					num = (int)(x[2][i] - y[2][i]);
					if (num != 0)
					{
						return num;
					}
				}
				return 0;
			}

			public int Compare(object x, object y)
			{
				return this.Compare(x as byte[][], y as byte[][]);
			}
		}
	}
}
