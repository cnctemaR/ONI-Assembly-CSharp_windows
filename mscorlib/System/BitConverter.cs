using System;
using System.Text;

namespace System
{
	public static class BitConverter
	{
		private static bool AmILittleEndian()
		{
			double num = 1.0;
			return num == (double)0;
		}

		private unsafe static bool DoubleWordsAreSwapped()
		{
			double num = 1.0;
			return *((ref num) + 2) == 240;
		}

		public static long DoubleToInt64Bits(double value)
		{
			return BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
		}

		public static double Int64BitsToDouble(long value)
		{
			return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
		}

		internal static double InternalInt64BitsToDouble(long value)
		{
			return BitConverter.SwappableToDouble(BitConverter.GetBytes(value), 0);
		}

		private unsafe static byte[] GetBytes(byte* ptr, int count)
		{
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = ptr[i];
			}
			return array;
		}

		public unsafe static byte[] GetBytes(bool value)
		{
			return BitConverter.GetBytes((byte*)(&value), 1);
		}

		public unsafe static byte[] GetBytes(char value)
		{
			return BitConverter.GetBytes((byte*)(&value), 2);
		}

		public unsafe static byte[] GetBytes(short value)
		{
			return BitConverter.GetBytes((byte*)(&value), 2);
		}

		public unsafe static byte[] GetBytes(int value)
		{
			return BitConverter.GetBytes((byte*)(&value), 4);
		}

		public unsafe static byte[] GetBytes(long value)
		{
			return BitConverter.GetBytes((byte*)(&value), 8);
		}

		[CLSCompliant(false)]
		public unsafe static byte[] GetBytes(ushort value)
		{
			return BitConverter.GetBytes((byte*)(&value), 2);
		}

		[CLSCompliant(false)]
		public unsafe static byte[] GetBytes(uint value)
		{
			return BitConverter.GetBytes((byte*)(&value), 4);
		}

		[CLSCompliant(false)]
		public unsafe static byte[] GetBytes(ulong value)
		{
			return BitConverter.GetBytes((byte*)(&value), 8);
		}

		public unsafe static byte[] GetBytes(float value)
		{
			return BitConverter.GetBytes((byte*)(&value), 4);
		}

		public unsafe static byte[] GetBytes(double value)
		{
			if (BitConverter.SwappedWordsInDouble)
			{
				return new byte[]
				{
					*((ref value) + 4),
					*((ref value) + 5),
					*((ref value) + 6),
					*((ref value) + 7),
					(byte)value,
					*((ref value) + 1),
					*((ref value) + 2),
					*((ref value) + 3)
				};
			}
			return BitConverter.GetBytes((byte*)(&value), 8);
		}

		private unsafe static void PutBytes(byte* dst, byte[] src, int start_index, int count)
		{
			if (src == null)
			{
				throw new ArgumentNullException("value");
			}
			if (start_index < 0 || start_index > src.Length - 1)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (src.Length - count < start_index)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			for (int i = 0; i < count; i++)
			{
				dst[i] = src[i + start_index];
			}
		}

		public static bool ToBoolean(byte[] value, int startIndex)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > value.Length - 1)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			return value[startIndex] != 0;
		}

		public unsafe static char ToChar(byte[] value, int startIndex)
		{
			char c;
			BitConverter.PutBytes((byte*)(&c), value, startIndex, 2);
			return c;
		}

		public unsafe static short ToInt16(byte[] value, int startIndex)
		{
			short num;
			BitConverter.PutBytes((byte*)(&num), value, startIndex, 2);
			return num;
		}

		public unsafe static int ToInt32(byte[] value, int startIndex)
		{
			int num;
			BitConverter.PutBytes((byte*)(&num), value, startIndex, 4);
			return num;
		}

		public unsafe static long ToInt64(byte[] value, int startIndex)
		{
			long num;
			BitConverter.PutBytes((byte*)(&num), value, startIndex, 8);
			return num;
		}

		[CLSCompliant(false)]
		public unsafe static ushort ToUInt16(byte[] value, int startIndex)
		{
			ushort num;
			BitConverter.PutBytes((byte*)(&num), value, startIndex, 2);
			return num;
		}

		[CLSCompliant(false)]
		public unsafe static uint ToUInt32(byte[] value, int startIndex)
		{
			uint num;
			BitConverter.PutBytes((byte*)(&num), value, startIndex, 4);
			return num;
		}

		[CLSCompliant(false)]
		public unsafe static ulong ToUInt64(byte[] value, int startIndex)
		{
			ulong num;
			BitConverter.PutBytes((byte*)(&num), value, startIndex, 8);
			return num;
		}

		public unsafe static float ToSingle(byte[] value, int startIndex)
		{
			float num;
			BitConverter.PutBytes((byte*)(&num), value, startIndex, 4);
			return num;
		}

		public unsafe static double ToDouble(byte[] value, int startIndex)
		{
			double num;
			if (!BitConverter.SwappedWordsInDouble)
			{
				BitConverter.PutBytes((byte*)(&num), value, startIndex, 8);
				return num;
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > value.Length - 1)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (value.Length - 8 < startIndex)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			num = (double)value[startIndex + 4];
			*((ref num) + 1) = value[startIndex + 5];
			*((ref num) + 2) = value[startIndex + 6];
			*((ref num) + 3) = value[startIndex + 7];
			*((ref num) + 4) = value[startIndex];
			*((ref num) + 5) = value[startIndex + 1];
			*((ref num) + 6) = value[startIndex + 2];
			*((ref num) + 7) = value[startIndex + 3];
			return num;
		}

		internal unsafe static double SwappableToDouble(byte[] value, int startIndex)
		{
			if (BitConverter.SwappedWordsInDouble)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (startIndex < 0 || startIndex > value.Length - 1)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (value.Length - 8 < startIndex)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				double num = (double)value[startIndex + 4];
				*((ref num) + 1) = value[startIndex + 5];
				*((ref num) + 2) = value[startIndex + 6];
				*((ref num) + 3) = value[startIndex + 7];
				*((ref num) + 4) = value[startIndex];
				*((ref num) + 5) = value[startIndex + 1];
				*((ref num) + 6) = value[startIndex + 2];
				*((ref num) + 7) = value[startIndex + 3];
				return num;
			}
			else
			{
				double num;
				if (BitConverter.IsLittleEndian)
				{
					BitConverter.PutBytes((byte*)(&num), value, startIndex, 8);
					return num;
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (startIndex < 0 || startIndex > value.Length - 1)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (value.Length - 8 < startIndex)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				num = (double)value[startIndex + 7];
				*((ref num) + 1) = value[startIndex + 6];
				*((ref num) + 2) = value[startIndex + 5];
				*((ref num) + 3) = value[startIndex + 4];
				*((ref num) + 4) = value[startIndex + 3];
				*((ref num) + 5) = value[startIndex + 2];
				*((ref num) + 6) = value[startIndex + 1];
				*((ref num) + 7) = value[startIndex];
				return num;
			}
		}

		public static string ToString(byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return BitConverter.ToString(value, 0, value.Length);
		}

		public static string ToString(byte[] value, int startIndex)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return BitConverter.ToString(value, startIndex, value.Length - startIndex);
		}

		public static string ToString(byte[] value, int startIndex, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			if (startIndex < 0 || startIndex >= value.Length)
			{
				if (startIndex == 0 && value.Length == 0)
				{
					return string.Empty;
				}
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			else
			{
				if (length < 0)
				{
					throw new ArgumentOutOfRangeException("length", "Value must be positive.");
				}
				if (startIndex > value.Length - length)
				{
					throw new ArgumentException("startIndex + length > value.Length");
				}
				if (length == 0)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder(length * 3 - 1);
				int num = startIndex + length;
				for (int i = startIndex; i < num; i++)
				{
					if (i > startIndex)
					{
						stringBuilder.Append('-');
					}
					char c = (char)((value[i] >> 4) & 15);
					char c2 = (char)(value[i] & 15);
					if (c < '\n')
					{
						c += '0';
					}
					else
					{
						c -= '\n';
						c += 'A';
					}
					if (c2 < '\n')
					{
						c2 += '0';
					}
					else
					{
						c2 -= '\n';
						c2 += 'A';
					}
					stringBuilder.Append(c);
					stringBuilder.Append(c2);
				}
				return stringBuilder.ToString();
			}
		}

		private static readonly bool SwappedWordsInDouble = BitConverter.DoubleWordsAreSwapped();

		public static readonly bool IsLittleEndian = BitConverter.AmILittleEndian();
	}
}
