using System;

namespace System.Numerics
{
	internal static class BigIntegerCalculator
	{
		public static uint[] Add(uint[] left, uint right)
		{
			uint[] array = new uint[left.Length + 1];
			long num = (long)((ulong)left[0] + (ulong)right);
			array[0] = (uint)num;
			long num2 = num >> 32;
			for (int i = 1; i < left.Length; i++)
			{
				num = (long)((ulong)left[i] + (ulong)num2);
				array[i] = (uint)num;
				num2 = num >> 32;
			}
			array[left.Length] = (uint)num2;
			return array;
		}

		public unsafe static uint[] Add(uint[] left, uint[] right)
		{
			uint[] array = new uint[left.Length + 1];
			fixed (uint[] array2 = left)
			{
				uint* ptr;
				if (left == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				fixed (uint[] array3 = right)
				{
					uint* ptr2;
					if (right == null || array3.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array3[0];
					}
					fixed (uint* ptr3 = &array[0])
					{
						uint* ptr4 = ptr3;
						BigIntegerCalculator.Add(ptr, left.Length, ptr2, right.Length, ptr4, array.Length);
						array2 = null;
						array3 = null;
					}
					return array;
				}
			}
		}

		private unsafe static void Add(uint* left, int leftLength, uint* right, int rightLength, uint* bits, int bitsLength)
		{
			int i = 0;
			long num = 0L;
			while (i < rightLength)
			{
				long num2 = (long)((ulong)left[i] + (ulong)num + (ulong)right[i]);
				bits[i] = (uint)num2;
				num = num2 >> 32;
				i++;
			}
			while (i < leftLength)
			{
				long num3 = (long)((ulong)left[i] + (ulong)num);
				bits[i] = (uint)num3;
				num = num3 >> 32;
				i++;
			}
			bits[i] = (uint)num;
		}

		private unsafe static void AddSelf(uint* left, int leftLength, uint* right, int rightLength)
		{
			int i = 0;
			long num = 0L;
			while (i < rightLength)
			{
				long num2 = (long)((ulong)left[i] + (ulong)num + (ulong)right[i]);
				left[i] = (uint)num2;
				num = num2 >> 32;
				i++;
			}
			while (num != 0L && i < leftLength)
			{
				long num3 = (long)((ulong)left[i] + (ulong)num);
				left[i] = (uint)num3;
				num = num3 >> 32;
				i++;
			}
		}

		public static uint[] Subtract(uint[] left, uint right)
		{
			uint[] array = new uint[left.Length];
			long num = (long)((ulong)left[0] - (ulong)right);
			array[0] = (uint)num;
			long num2 = num >> 32;
			for (int i = 1; i < left.Length; i++)
			{
				num = (long)((ulong)left[i] + (ulong)num2);
				array[i] = (uint)num;
				num2 = num >> 32;
			}
			return array;
		}

		public unsafe static uint[] Subtract(uint[] left, uint[] right)
		{
			uint[] array = new uint[left.Length];
			fixed (uint[] array2 = left)
			{
				uint* ptr;
				if (left == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				uint[] array4;
				fixed (uint[] array3 = right)
				{
					uint* ptr2;
					if (right == null || array3.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array3[0];
					}
					uint* ptr3;
					if ((array4 = array) == null || array4.Length == 0)
					{
						ptr3 = null;
					}
					else
					{
						ptr3 = &array4[0];
					}
					BigIntegerCalculator.Subtract(ptr, left.Length, ptr2, right.Length, ptr3, array.Length);
					array2 = null;
				}
				array4 = null;
				return array;
			}
		}

		private unsafe static void Subtract(uint* left, int leftLength, uint* right, int rightLength, uint* bits, int bitsLength)
		{
			int i = 0;
			long num = 0L;
			while (i < rightLength)
			{
				long num2 = (long)((ulong)left[i] + (ulong)num - (ulong)right[i]);
				bits[i] = (uint)num2;
				num = num2 >> 32;
				i++;
			}
			while (i < leftLength)
			{
				long num3 = (long)((ulong)left[i] + (ulong)num);
				bits[i] = (uint)num3;
				num = num3 >> 32;
				i++;
			}
		}

		private unsafe static void SubtractSelf(uint* left, int leftLength, uint* right, int rightLength)
		{
			int i = 0;
			long num = 0L;
			while (i < rightLength)
			{
				long num2 = (long)((ulong)left[i] + (ulong)num - (ulong)right[i]);
				left[i] = (uint)num2;
				num = num2 >> 32;
				i++;
			}
			while (num != 0L && i < leftLength)
			{
				long num3 = (long)((ulong)left[i] + (ulong)num);
				left[i] = (uint)num3;
				num = num3 >> 32;
				i++;
			}
		}

		public static int Compare(uint[] left, uint[] right)
		{
			if (left.Length < right.Length)
			{
				return -1;
			}
			if (left.Length > right.Length)
			{
				return 1;
			}
			for (int i = left.Length - 1; i >= 0; i--)
			{
				if (left[i] < right[i])
				{
					return -1;
				}
				if (left[i] > right[i])
				{
					return 1;
				}
			}
			return 0;
		}

		private unsafe static int Compare(uint* left, int leftLength, uint* right, int rightLength)
		{
			if (leftLength < rightLength)
			{
				return -1;
			}
			if (leftLength > rightLength)
			{
				return 1;
			}
			for (int i = leftLength - 1; i >= 0; i--)
			{
				if (left[i] < right[i])
				{
					return -1;
				}
				if (left[i] > right[i])
				{
					return 1;
				}
			}
			return 0;
		}

		public static uint[] Divide(uint[] left, uint right, out uint remainder)
		{
			uint[] array = new uint[left.Length];
			ulong num = 0UL;
			for (int i = left.Length - 1; i >= 0; i--)
			{
				ulong num2 = (num << 32) | (ulong)left[i];
				ulong num3 = num2 / (ulong)right;
				array[i] = (uint)num3;
				num = num2 - num3 * (ulong)right;
			}
			remainder = (uint)num;
			return array;
		}

		public static uint[] Divide(uint[] left, uint right)
		{
			uint[] array = new uint[left.Length];
			ulong num = 0UL;
			for (int i = left.Length - 1; i >= 0; i--)
			{
				ulong num2 = (num << 32) | (ulong)left[i];
				ulong num3 = num2 / (ulong)right;
				array[i] = (uint)num3;
				num = num2 - num3 * (ulong)right;
			}
			return array;
		}

		public static uint Remainder(uint[] left, uint right)
		{
			ulong num = 0UL;
			for (int i = left.Length - 1; i >= 0; i--)
			{
				num = ((num << 32) | (ulong)left[i]) % (ulong)right;
			}
			return (uint)num;
		}

		public unsafe static uint[] Divide(uint[] left, uint[] right, out uint[] remainder)
		{
			uint[] array = BigIntegerCalculator.CreateCopy(left);
			uint[] array2 = new uint[left.Length - right.Length + 1];
			fixed (uint* ptr = &array[0])
			{
				uint* ptr2 = ptr;
				fixed (uint* ptr3 = &right[0])
				{
					uint* ptr4 = ptr3;
					fixed (uint* ptr5 = &array2[0])
					{
						uint* ptr6 = ptr5;
						BigIntegerCalculator.Divide(ptr2, array.Length, ptr4, right.Length, ptr6, array2.Length);
						ptr = null;
						ptr3 = null;
					}
					remainder = array;
					return array2;
				}
			}
		}

		public unsafe static uint[] Divide(uint[] left, uint[] right)
		{
			uint[] array = BigIntegerCalculator.CreateCopy(left);
			uint[] array2 = new uint[left.Length - right.Length + 1];
			fixed (uint* ptr = &array[0])
			{
				uint* ptr2 = ptr;
				fixed (uint* ptr3 = &right[0])
				{
					uint* ptr4 = ptr3;
					fixed (uint* ptr5 = &array2[0])
					{
						uint* ptr6 = ptr5;
						BigIntegerCalculator.Divide(ptr2, array.Length, ptr4, right.Length, ptr6, array2.Length);
						ptr = null;
						ptr3 = null;
					}
					return array2;
				}
			}
		}

		public unsafe static uint[] Remainder(uint[] left, uint[] right)
		{
			uint[] array = BigIntegerCalculator.CreateCopy(left);
			fixed (uint* ptr = &array[0])
			{
				uint* ptr2 = ptr;
				fixed (uint* ptr3 = &right[0])
				{
					uint* ptr4 = ptr3;
					BigIntegerCalculator.Divide(ptr2, array.Length, ptr4, right.Length, null, 0);
					ptr = null;
				}
				return array;
			}
		}

		private unsafe static void Divide(uint* left, int leftLength, uint* right, int rightLength, uint* bits, int bitsLength)
		{
			uint num = right[rightLength - 1];
			uint num2 = ((rightLength > 1) ? right[rightLength - 2] : 0U);
			int num3 = BigIntegerCalculator.LeadingZeros(num);
			int num4 = 32 - num3;
			if (num3 > 0)
			{
				uint num5 = ((rightLength > 2) ? right[rightLength - 3] : 0U);
				num = (num << num3) | (num2 >> num4);
				num2 = (num2 << num3) | (num5 >> num4);
			}
			for (int i = leftLength; i >= rightLength; i--)
			{
				int num6 = i - rightLength;
				uint num7 = ((i < leftLength) ? left[i] : 0U);
				ulong num8 = ((ulong)num7 << 32) | (ulong)left[i - 1];
				uint num9 = ((i > 1) ? left[i - 2] : 0U);
				if (num3 > 0)
				{
					uint num10 = ((i > 2) ? left[i - 3] : 0U);
					num8 = (num8 << num3) | (ulong)(num9 >> num4);
					num9 = (num9 << num3) | (num10 >> num4);
				}
				ulong num11 = num8 / (ulong)num;
				if (num11 > (ulong)(-1))
				{
					num11 = (ulong)(-1);
				}
				while (BigIntegerCalculator.DivideGuessTooBig(num11, num8, num9, num, num2))
				{
					num11 -= 1UL;
				}
				if (num11 > 0UL && BigIntegerCalculator.SubtractDivisor(left + num6, leftLength - num6, right, rightLength, num11) != num7)
				{
					BigIntegerCalculator.AddDivisor(left + num6, leftLength - num6, right, rightLength);
					num11 -= 1UL;
				}
				if (bitsLength != 0)
				{
					bits[num6] = (uint)num11;
				}
				if (i < leftLength)
				{
					left[i] = 0U;
				}
			}
		}

		private unsafe static uint AddDivisor(uint* left, int leftLength, uint* right, int rightLength)
		{
			ulong num = 0UL;
			for (int i = 0; i < rightLength; i++)
			{
				ulong num2 = (ulong)left[i] + num + (ulong)right[i];
				left[i] = (uint)num2;
				num = num2 >> 32;
			}
			return (uint)num;
		}

		private unsafe static uint SubtractDivisor(uint* left, int leftLength, uint* right, int rightLength, ulong q)
		{
			ulong num = 0UL;
			for (int i = 0; i < rightLength; i++)
			{
				num += (ulong)right[i] * q;
				uint num2 = (uint)num;
				num >>= 32;
				if (left[i] < num2)
				{
					num += 1UL;
				}
				left[i] = left[i] - num2;
			}
			return (uint)num;
		}

		private static bool DivideGuessTooBig(ulong q, ulong valHi, uint valLo, uint divHi, uint divLo)
		{
			ulong num = (ulong)divHi * q;
			ulong num2 = (ulong)divLo * q;
			num += num2 >> 32;
			num2 &= (ulong)(-1);
			return num >= valHi && (num > valHi || (num2 >= (ulong)valLo && num2 > (ulong)valLo));
		}

		private static uint[] CreateCopy(uint[] value)
		{
			uint[] array = new uint[value.Length];
			Array.Copy(value, 0, array, 0, array.Length);
			return array;
		}

		private static int LeadingZeros(uint value)
		{
			if (value == 0U)
			{
				return 32;
			}
			int num = 0;
			if ((value & 4294901760U) == 0U)
			{
				num += 16;
				value <<= 16;
			}
			if ((value & 4278190080U) == 0U)
			{
				num += 8;
				value <<= 8;
			}
			if ((value & 4026531840U) == 0U)
			{
				num += 4;
				value <<= 4;
			}
			if ((value & 3221225472U) == 0U)
			{
				num += 2;
				value <<= 2;
			}
			if ((value & 2147483648U) == 0U)
			{
				num++;
			}
			return num;
		}

		public static uint Gcd(uint left, uint right)
		{
			while (right != 0U)
			{
				uint num = left % right;
				left = right;
				right = num;
			}
			return left;
		}

		public static ulong Gcd(ulong left, ulong right)
		{
			while (right > (ulong)(-1))
			{
				ulong num = left % right;
				left = right;
				right = num;
			}
			if (right != 0UL)
			{
				return (ulong)BigIntegerCalculator.Gcd((uint)right, (uint)(left % right));
			}
			return left;
		}

		public static uint Gcd(uint[] left, uint right)
		{
			uint num = BigIntegerCalculator.Remainder(left, right);
			return BigIntegerCalculator.Gcd(right, num);
		}

		public static uint[] Gcd(uint[] left, uint[] right)
		{
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(left.Length, left);
			BigIntegerCalculator.BitsBuffer bitsBuffer2 = new BigIntegerCalculator.BitsBuffer(right.Length, right);
			BigIntegerCalculator.Gcd(ref bitsBuffer, ref bitsBuffer2);
			return bitsBuffer.GetBits();
		}

		private static void Gcd(ref BigIntegerCalculator.BitsBuffer left, ref BigIntegerCalculator.BitsBuffer right)
		{
			while (right.GetLength() > 2)
			{
				ulong num;
				ulong num2;
				BigIntegerCalculator.ExtractDigits(ref left, ref right, out num, out num2);
				uint num3 = 1U;
				uint num4 = 0U;
				uint num5 = 0U;
				uint num6 = 1U;
				int num7 = 0;
				while (num2 != 0UL)
				{
					ulong num8 = num / num2;
					if (num8 > (ulong)(-1))
					{
						break;
					}
					ulong num9 = (ulong)num3 + num8 * (ulong)num5;
					ulong num10 = (ulong)num4 + num8 * (ulong)num6;
					ulong num11 = num - num8 * num2;
					if (num9 > 2147483647UL || num10 > 2147483647UL || num11 < num10 || num11 + num9 > num2 - (ulong)num5)
					{
						break;
					}
					num3 = (uint)num9;
					num4 = (uint)num10;
					num = num11;
					num7++;
					if (num == (ulong)num4)
					{
						break;
					}
					num8 = num2 / num;
					if (num8 > (ulong)(-1))
					{
						break;
					}
					num9 = (ulong)num6 + num8 * (ulong)num4;
					num10 = (ulong)num5 + num8 * (ulong)num3;
					num11 = num2 - num8 * num;
					if (num9 > 2147483647UL || num10 > 2147483647UL || num11 < num10 || num11 + num9 > num - (ulong)num4)
					{
						break;
					}
					num6 = (uint)num9;
					num5 = (uint)num10;
					num2 = num11;
					num7++;
					if (num2 == (ulong)num5)
					{
						break;
					}
				}
				if (num4 == 0U)
				{
					left.Reduce(ref right);
					BigIntegerCalculator.BitsBuffer bitsBuffer = left;
					left = right;
					right = bitsBuffer;
				}
				else
				{
					BigIntegerCalculator.LehmerCore(ref left, ref right, (long)((ulong)num3), (long)((ulong)num4), (long)((ulong)num5), (long)((ulong)num6));
					if (num7 % 2 == 1)
					{
						BigIntegerCalculator.BitsBuffer bitsBuffer2 = left;
						left = right;
						right = bitsBuffer2;
					}
				}
			}
			if (right.GetLength() > 0)
			{
				left.Reduce(ref right);
				uint[] bits = right.GetBits();
				uint[] bits2 = left.GetBits();
				ulong num12 = ((ulong)bits[1] << 32) | (ulong)bits[0];
				ulong num13 = ((ulong)bits2[1] << 32) | (ulong)bits2[0];
				left.Overwrite(BigIntegerCalculator.Gcd(num12, num13));
				right.Overwrite(0U);
			}
		}

		private static void ExtractDigits(ref BigIntegerCalculator.BitsBuffer xBuffer, ref BigIntegerCalculator.BitsBuffer yBuffer, out ulong x, out ulong y)
		{
			uint[] bits = xBuffer.GetBits();
			int length = xBuffer.GetLength();
			uint[] bits2 = yBuffer.GetBits();
			int length2 = yBuffer.GetLength();
			ulong num = (ulong)bits[length - 1];
			ulong num2 = (ulong)bits[length - 2];
			ulong num3 = (ulong)bits[length - 3];
			ulong num4;
			ulong num5;
			ulong num6;
			switch (length - length2)
			{
			case 0:
				num4 = (ulong)bits2[length2 - 1];
				num5 = (ulong)bits2[length2 - 2];
				num6 = (ulong)bits2[length2 - 3];
				break;
			case 1:
				num4 = 0UL;
				num5 = (ulong)bits2[length2 - 1];
				num6 = (ulong)bits2[length2 - 2];
				break;
			case 2:
				num4 = 0UL;
				num5 = 0UL;
				num6 = (ulong)bits2[length2 - 1];
				break;
			default:
				num4 = 0UL;
				num5 = 0UL;
				num6 = 0UL;
				break;
			}
			int num7 = BigIntegerCalculator.LeadingZeros((uint)num);
			x = ((num << 32 + num7) | (num2 << num7) | (num3 >> 32 - num7)) >> 1;
			y = ((num4 << 32 + num7) | (num5 << num7) | (num6 >> 32 - num7)) >> 1;
		}

		private static void LehmerCore(ref BigIntegerCalculator.BitsBuffer xBuffer, ref BigIntegerCalculator.BitsBuffer yBuffer, long a, long b, long c, long d)
		{
			uint[] bits = xBuffer.GetBits();
			uint[] bits2 = yBuffer.GetBits();
			int length = yBuffer.GetLength();
			long num = 0L;
			long num2 = 0L;
			for (int i = 0; i < length; i++)
			{
				long num3 = a * (long)((ulong)bits[i]) - b * (long)((ulong)bits2[i]) + num;
				long num4 = d * (long)((ulong)bits2[i]) - c * (long)((ulong)bits[i]) + num2;
				num = num3 >> 32;
				num2 = num4 >> 32;
				bits[i] = (uint)num3;
				bits2[i] = (uint)num4;
			}
			xBuffer.Refresh(length);
			yBuffer.Refresh(length);
		}

		public static uint[] Pow(uint value, uint power)
		{
			int num = BigIntegerCalculator.PowBound(power, 1, 1);
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(num, value);
			return BigIntegerCalculator.PowCore(power, ref bitsBuffer);
		}

		public static uint[] Pow(uint[] value, uint power)
		{
			int num = BigIntegerCalculator.PowBound(power, value.Length, 1);
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(num, value);
			return BigIntegerCalculator.PowCore(power, ref bitsBuffer);
		}

		private static uint[] PowCore(uint power, ref BigIntegerCalculator.BitsBuffer value)
		{
			int size = value.GetSize();
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(size, 0U);
			BigIntegerCalculator.BitsBuffer bitsBuffer2 = new BigIntegerCalculator.BitsBuffer(size, 1U);
			BigIntegerCalculator.PowCore(power, ref value, ref bitsBuffer2, ref bitsBuffer);
			return bitsBuffer2.GetBits();
		}

		private static int PowBound(uint power, int valueLength, int resultLength)
		{
			checked
			{
				while (power != 0U)
				{
					if ((power & 1U) == 1U)
					{
						resultLength += valueLength;
					}
					if (power != 1U)
					{
						valueLength += valueLength;
					}
					power >>= 1;
				}
				return resultLength;
			}
		}

		private static void PowCore(uint power, ref BigIntegerCalculator.BitsBuffer value, ref BigIntegerCalculator.BitsBuffer result, ref BigIntegerCalculator.BitsBuffer temp)
		{
			while (power != 0U)
			{
				if ((power & 1U) == 1U)
				{
					result.MultiplySelf(ref value, ref temp);
				}
				if (power != 1U)
				{
					value.SquareSelf(ref temp);
				}
				power >>= 1;
			}
		}

		public static uint Pow(uint value, uint power, uint modulus)
		{
			return BigIntegerCalculator.PowCore(power, modulus, (ulong)value, 1UL);
		}

		public static uint Pow(uint[] value, uint power, uint modulus)
		{
			uint num = BigIntegerCalculator.Remainder(value, modulus);
			return BigIntegerCalculator.PowCore(power, modulus, (ulong)num, 1UL);
		}

		public static uint Pow(uint value, uint[] power, uint modulus)
		{
			return BigIntegerCalculator.PowCore(power, modulus, (ulong)value, 1UL);
		}

		public static uint Pow(uint[] value, uint[] power, uint modulus)
		{
			uint num = BigIntegerCalculator.Remainder(value, modulus);
			return BigIntegerCalculator.PowCore(power, modulus, (ulong)num, 1UL);
		}

		private static uint PowCore(uint[] power, uint modulus, ulong value, ulong result)
		{
			for (int i = 0; i < power.Length - 1; i++)
			{
				uint num = power[i];
				for (int j = 0; j < 32; j++)
				{
					if ((num & 1U) == 1U)
					{
						result = result * value % (ulong)modulus;
					}
					value = value * value % (ulong)modulus;
					num >>= 1;
				}
			}
			return BigIntegerCalculator.PowCore(power[power.Length - 1], modulus, value, result);
		}

		private static uint PowCore(uint power, uint modulus, ulong value, ulong result)
		{
			while (power != 0U)
			{
				if ((power & 1U) == 1U)
				{
					result = result * value % (ulong)modulus;
				}
				if (power != 1U)
				{
					value = value * value % (ulong)modulus;
				}
				power >>= 1;
			}
			return (uint)(result % (ulong)modulus);
		}

		public static uint[] Pow(uint value, uint power, uint[] modulus)
		{
			int num = modulus.Length + modulus.Length;
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(num, value);
			return BigIntegerCalculator.PowCore(power, modulus, ref bitsBuffer);
		}

		public static uint[] Pow(uint[] value, uint power, uint[] modulus)
		{
			if (value.Length > modulus.Length)
			{
				value = BigIntegerCalculator.Remainder(value, modulus);
			}
			int num = modulus.Length + modulus.Length;
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(num, value);
			return BigIntegerCalculator.PowCore(power, modulus, ref bitsBuffer);
		}

		public static uint[] Pow(uint value, uint[] power, uint[] modulus)
		{
			int num = modulus.Length + modulus.Length;
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(num, value);
			return BigIntegerCalculator.PowCore(power, modulus, ref bitsBuffer);
		}

		public static uint[] Pow(uint[] value, uint[] power, uint[] modulus)
		{
			if (value.Length > modulus.Length)
			{
				value = BigIntegerCalculator.Remainder(value, modulus);
			}
			int num = modulus.Length + modulus.Length;
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(num, value);
			return BigIntegerCalculator.PowCore(power, modulus, ref bitsBuffer);
		}

		private static uint[] PowCore(uint[] power, uint[] modulus, ref BigIntegerCalculator.BitsBuffer value)
		{
			int size = value.GetSize();
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(size, 0U);
			BigIntegerCalculator.BitsBuffer bitsBuffer2 = new BigIntegerCalculator.BitsBuffer(size, 1U);
			if (modulus.Length < BigIntegerCalculator.ReducerThreshold)
			{
				BigIntegerCalculator.PowCore(power, modulus, ref value, ref bitsBuffer2, ref bitsBuffer);
			}
			else
			{
				BigIntegerCalculator.FastReducer fastReducer = new BigIntegerCalculator.FastReducer(modulus);
				BigIntegerCalculator.PowCore(power, ref fastReducer, ref value, ref bitsBuffer2, ref bitsBuffer);
			}
			return bitsBuffer2.GetBits();
		}

		private static uint[] PowCore(uint power, uint[] modulus, ref BigIntegerCalculator.BitsBuffer value)
		{
			int size = value.GetSize();
			BigIntegerCalculator.BitsBuffer bitsBuffer = new BigIntegerCalculator.BitsBuffer(size, 0U);
			BigIntegerCalculator.BitsBuffer bitsBuffer2 = new BigIntegerCalculator.BitsBuffer(size, 1U);
			if (modulus.Length < BigIntegerCalculator.ReducerThreshold)
			{
				BigIntegerCalculator.PowCore(power, modulus, ref value, ref bitsBuffer2, ref bitsBuffer);
			}
			else
			{
				BigIntegerCalculator.FastReducer fastReducer = new BigIntegerCalculator.FastReducer(modulus);
				BigIntegerCalculator.PowCore(power, ref fastReducer, ref value, ref bitsBuffer2, ref bitsBuffer);
			}
			return bitsBuffer2.GetBits();
		}

		private static void PowCore(uint[] power, uint[] modulus, ref BigIntegerCalculator.BitsBuffer value, ref BigIntegerCalculator.BitsBuffer result, ref BigIntegerCalculator.BitsBuffer temp)
		{
			for (int i = 0; i < power.Length - 1; i++)
			{
				uint num = power[i];
				for (int j = 0; j < 32; j++)
				{
					if ((num & 1U) == 1U)
					{
						result.MultiplySelf(ref value, ref temp);
						result.Reduce(modulus);
					}
					value.SquareSelf(ref temp);
					value.Reduce(modulus);
					num >>= 1;
				}
			}
			BigIntegerCalculator.PowCore(power[power.Length - 1], modulus, ref value, ref result, ref temp);
		}

		private static void PowCore(uint power, uint[] modulus, ref BigIntegerCalculator.BitsBuffer value, ref BigIntegerCalculator.BitsBuffer result, ref BigIntegerCalculator.BitsBuffer temp)
		{
			while (power != 0U)
			{
				if ((power & 1U) == 1U)
				{
					result.MultiplySelf(ref value, ref temp);
					result.Reduce(modulus);
				}
				if (power != 1U)
				{
					value.SquareSelf(ref temp);
					value.Reduce(modulus);
				}
				power >>= 1;
			}
		}

		private static void PowCore(uint[] power, ref BigIntegerCalculator.FastReducer reducer, ref BigIntegerCalculator.BitsBuffer value, ref BigIntegerCalculator.BitsBuffer result, ref BigIntegerCalculator.BitsBuffer temp)
		{
			for (int i = 0; i < power.Length - 1; i++)
			{
				uint num = power[i];
				for (int j = 0; j < 32; j++)
				{
					if ((num & 1U) == 1U)
					{
						result.MultiplySelf(ref value, ref temp);
						result.Reduce(ref reducer);
					}
					value.SquareSelf(ref temp);
					value.Reduce(ref reducer);
					num >>= 1;
				}
			}
			BigIntegerCalculator.PowCore(power[power.Length - 1], ref reducer, ref value, ref result, ref temp);
		}

		private static void PowCore(uint power, ref BigIntegerCalculator.FastReducer reducer, ref BigIntegerCalculator.BitsBuffer value, ref BigIntegerCalculator.BitsBuffer result, ref BigIntegerCalculator.BitsBuffer temp)
		{
			while (power != 0U)
			{
				if ((power & 1U) == 1U)
				{
					result.MultiplySelf(ref value, ref temp);
					result.Reduce(ref reducer);
				}
				if (power != 1U)
				{
					value.SquareSelf(ref temp);
					value.Reduce(ref reducer);
				}
				power >>= 1;
			}
		}

		private static int ActualLength(uint[] value)
		{
			return BigIntegerCalculator.ActualLength(value, value.Length);
		}

		private static int ActualLength(uint[] value, int length)
		{
			while (length > 0 && value[length - 1] == 0U)
			{
				length--;
			}
			return length;
		}

		public unsafe static uint[] Square(uint[] value)
		{
			uint[] array = new uint[value.Length + value.Length];
			uint[] array3;
			fixed (uint[] array2 = value)
			{
				uint* ptr;
				if (value == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				uint* ptr2;
				if ((array3 = array) == null || array3.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array3[0];
				}
				BigIntegerCalculator.Square(ptr, value.Length, ptr2, array.Length);
			}
			array3 = null;
			return array;
		}

		private unsafe static void Square(uint* value, int valueLength, uint* bits, int bitsLength)
		{
			if (valueLength < BigIntegerCalculator.SquareThreshold)
			{
				for (int i = 0; i < valueLength; i++)
				{
					ulong num = 0UL;
					for (int j = 0; j < i; j++)
					{
						ulong num2 = (ulong)bits[i + j] + num;
						ulong num3 = (ulong)value[j] * (ulong)value[i];
						bits[i + j] = (uint)(num2 + (num3 << 1));
						num = num3 + (num2 >> 1) >> 31;
					}
					ulong num4 = (ulong)value[i] * (ulong)value[i] + num;
					bits[i + i] = (uint)num4;
					bits[i + i + 1] = (uint)(num4 >> 32);
				}
				return;
			}
			int num5 = valueLength >> 1;
			int num6 = num5 << 1;
			int num7 = num5;
			uint* ptr = value + num5;
			int num8 = valueLength - num5;
			int num9 = num6;
			uint* ptr2 = bits + num6;
			int num10 = bitsLength - num6;
			BigIntegerCalculator.Square(value, num7, bits, num9);
			BigIntegerCalculator.Square(ptr, num8, ptr2, num10);
			int num11 = num8 + 1;
			int num12 = num11 + num11;
			if (num12 < BigIntegerCalculator.AllocationThreshold)
			{
				uint* ptr4;
				checked
				{
					uint* ptr3 = stackalloc uint[unchecked((UIntPtr)num11) * 4];
					ptr4 = stackalloc uint[unchecked((UIntPtr)num12) * 4];
					BigIntegerCalculator.Add(ptr, num8, value, num7, ptr3, num11);
					BigIntegerCalculator.Square(ptr3, num11, ptr4, num12);
					BigIntegerCalculator.SubtractCore(ptr2, num10, bits, num9, ptr4, num12);
				}
				BigIntegerCalculator.AddSelf(bits + num5, bitsLength - num5, ptr4, num12);
				return;
			}
			uint[] array;
			uint* ptr5;
			if ((array = new uint[num11]) == null || array.Length == 0)
			{
				ptr5 = null;
			}
			else
			{
				ptr5 = &array[0];
			}
			uint[] array2;
			uint* ptr6;
			if ((array2 = new uint[num12]) == null || array2.Length == 0)
			{
				ptr6 = null;
			}
			else
			{
				ptr6 = &array2[0];
			}
			BigIntegerCalculator.Add(ptr, num8, value, num7, ptr5, num11);
			BigIntegerCalculator.Square(ptr5, num11, ptr6, num12);
			BigIntegerCalculator.SubtractCore(ptr2, num10, bits, num9, ptr6, num12);
			BigIntegerCalculator.AddSelf(bits + num5, bitsLength - num5, ptr6, num12);
			array = null;
			array2 = null;
		}

		public static uint[] Multiply(uint[] left, uint right)
		{
			int i = 0;
			ulong num = 0UL;
			uint[] array = new uint[left.Length + 1];
			while (i < left.Length)
			{
				ulong num2 = (ulong)left[i] * (ulong)right + num;
				array[i] = (uint)num2;
				num = num2 >> 32;
				i++;
			}
			array[i] = (uint)num;
			return array;
		}

		public unsafe static uint[] Multiply(uint[] left, uint[] right)
		{
			uint[] array = new uint[left.Length + right.Length];
			fixed (uint[] array2 = left)
			{
				uint* ptr;
				if (left == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				uint[] array4;
				fixed (uint[] array3 = right)
				{
					uint* ptr2;
					if (right == null || array3.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array3[0];
					}
					uint* ptr3;
					if ((array4 = array) == null || array4.Length == 0)
					{
						ptr3 = null;
					}
					else
					{
						ptr3 = &array4[0];
					}
					BigIntegerCalculator.Multiply(ptr, left.Length, ptr2, right.Length, ptr3, array.Length);
					array2 = null;
				}
				array4 = null;
				return array;
			}
		}

		private unsafe static void Multiply(uint* left, int leftLength, uint* right, int rightLength, uint* bits, int bitsLength)
		{
			if (rightLength < BigIntegerCalculator.MultiplyThreshold)
			{
				for (int i = 0; i < rightLength; i++)
				{
					ulong num = 0UL;
					for (int j = 0; j < leftLength; j++)
					{
						ulong num2 = (ulong)bits[i + j] + num + (ulong)left[j] * (ulong)right[i];
						bits[i + j] = (uint)num2;
						num = num2 >> 32;
					}
					bits[i + leftLength] = (uint)num;
				}
				return;
			}
			int num3 = rightLength >> 1;
			int num4 = num3 << 1;
			int num5 = num3;
			uint* ptr = left + num3;
			int num6 = leftLength - num3;
			int num7 = num3;
			uint* ptr2 = right + num3;
			int num8 = rightLength - num3;
			int num9 = num4;
			uint* ptr3 = bits + num4;
			int num10 = bitsLength - num4;
			BigIntegerCalculator.Multiply(left, num5, right, num7, bits, num9);
			BigIntegerCalculator.Multiply(ptr, num6, ptr2, num8, ptr3, num10);
			int num11 = num6 + 1;
			int num12 = num8 + 1;
			int num13 = num11 + num12;
			if (num13 < BigIntegerCalculator.AllocationThreshold)
			{
				uint* ptr6;
				checked
				{
					uint* ptr4 = stackalloc uint[unchecked((UIntPtr)num11) * 4];
					uint* ptr5 = stackalloc uint[unchecked((UIntPtr)num12) * 4];
					ptr6 = stackalloc uint[unchecked((UIntPtr)num13) * 4];
					BigIntegerCalculator.Add(ptr, num6, left, num5, ptr4, num11);
					BigIntegerCalculator.Add(ptr2, num8, right, num7, ptr5, num12);
					BigIntegerCalculator.Multiply(ptr4, num11, ptr5, num12, ptr6, num13);
					BigIntegerCalculator.SubtractCore(ptr3, num10, bits, num9, ptr6, num13);
				}
				BigIntegerCalculator.AddSelf(bits + num3, bitsLength - num3, ptr6, num13);
				return;
			}
			uint[] array;
			uint* ptr7;
			if ((array = new uint[num11]) == null || array.Length == 0)
			{
				ptr7 = null;
			}
			else
			{
				ptr7 = &array[0];
			}
			uint[] array2;
			uint* ptr8;
			if ((array2 = new uint[num12]) == null || array2.Length == 0)
			{
				ptr8 = null;
			}
			else
			{
				ptr8 = &array2[0];
			}
			uint[] array3;
			uint* ptr9;
			if ((array3 = new uint[num13]) == null || array3.Length == 0)
			{
				ptr9 = null;
			}
			else
			{
				ptr9 = &array3[0];
			}
			BigIntegerCalculator.Add(ptr, num6, left, num5, ptr7, num11);
			BigIntegerCalculator.Add(ptr2, num8, right, num7, ptr8, num12);
			BigIntegerCalculator.Multiply(ptr7, num11, ptr8, num12, ptr9, num13);
			BigIntegerCalculator.SubtractCore(ptr3, num10, bits, num9, ptr9, num13);
			BigIntegerCalculator.AddSelf(bits + num3, bitsLength - num3, ptr9, num13);
			array = null;
			array2 = null;
			array3 = null;
		}

		private unsafe static void SubtractCore(uint* left, int leftLength, uint* right, int rightLength, uint* core, int coreLength)
		{
			int i = 0;
			long num = 0L;
			while (i < rightLength)
			{
				long num2 = (long)((ulong)core[i] + (ulong)num - (ulong)left[i] - (ulong)right[i]);
				core[i] = (uint)num2;
				num = num2 >> 32;
				i++;
			}
			while (i < leftLength)
			{
				long num3 = (long)((ulong)core[i] + (ulong)num - (ulong)left[i]);
				core[i] = (uint)num3;
				num = num3 >> 32;
				i++;
			}
			while (num != 0L && i < coreLength)
			{
				long num4 = (long)((ulong)core[i] + (ulong)num);
				core[i] = (uint)num4;
				num = num4 >> 32;
				i++;
			}
		}

		private static int ReducerThreshold = 32;

		private static int SquareThreshold = 32;

		private static int AllocationThreshold = 256;

		private static int MultiplyThreshold = 32;

		internal struct BitsBuffer
		{
			public BitsBuffer(int size, uint value)
			{
				this._bits = new uint[size];
				this._length = ((value != 0U) ? 1 : 0);
				this._bits[0] = value;
			}

			public BitsBuffer(int size, uint[] value)
			{
				this._bits = new uint[size];
				this._length = BigIntegerCalculator.ActualLength(value);
				Array.Copy(value, 0, this._bits, 0, this._length);
			}

			public unsafe void MultiplySelf(ref BigIntegerCalculator.BitsBuffer value, ref BigIntegerCalculator.BitsBuffer temp)
			{
				uint[] array;
				uint* ptr;
				if ((array = this._bits) == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				uint[] array2;
				uint* ptr2;
				if ((array2 = value._bits) == null || array2.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array2[0];
				}
				uint[] array3;
				uint* ptr3;
				if ((array3 = temp._bits) == null || array3.Length == 0)
				{
					ptr3 = null;
				}
				else
				{
					ptr3 = &array3[0];
				}
				if (this._length < value._length)
				{
					BigIntegerCalculator.Multiply(ptr2, value._length, ptr, this._length, ptr3, this._length + value._length);
				}
				else
				{
					BigIntegerCalculator.Multiply(ptr, this._length, ptr2, value._length, ptr3, this._length + value._length);
				}
				array = null;
				array2 = null;
				array3 = null;
				this.Apply(ref temp, this._length + value._length);
			}

			public unsafe void SquareSelf(ref BigIntegerCalculator.BitsBuffer temp)
			{
				uint[] array;
				uint* ptr;
				if ((array = this._bits) == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				uint[] array2;
				uint* ptr2;
				if ((array2 = temp._bits) == null || array2.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array2[0];
				}
				BigIntegerCalculator.Square(ptr, this._length, ptr2, this._length + this._length);
				array = null;
				array2 = null;
				this.Apply(ref temp, this._length + this._length);
			}

			public void Reduce(ref BigIntegerCalculator.FastReducer reducer)
			{
				this._length = reducer.Reduce(this._bits, this._length);
			}

			public unsafe void Reduce(uint[] modulus)
			{
				if (this._length >= modulus.Length)
				{
					uint[] array;
					uint* ptr;
					if ((array = this._bits) == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					fixed (uint[] array2 = modulus)
					{
						uint* ptr2;
						if (modulus == null || array2.Length == 0)
						{
							ptr2 = null;
						}
						else
						{
							ptr2 = &array2[0];
						}
						BigIntegerCalculator.Divide(ptr, this._length, ptr2, modulus.Length, null, 0);
						array = null;
					}
					this._length = BigIntegerCalculator.ActualLength(this._bits, modulus.Length);
				}
			}

			public unsafe void Reduce(ref BigIntegerCalculator.BitsBuffer modulus)
			{
				if (this._length >= modulus._length)
				{
					uint[] array;
					uint* ptr;
					if ((array = this._bits) == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					uint[] array2;
					uint* ptr2;
					if ((array2 = modulus._bits) == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					BigIntegerCalculator.Divide(ptr, this._length, ptr2, modulus._length, null, 0);
					array = null;
					array2 = null;
					this._length = BigIntegerCalculator.ActualLength(this._bits, modulus._length);
				}
			}

			public void Overwrite(ulong value)
			{
				if (this._length > 2)
				{
					Array.Clear(this._bits, 2, this._length - 2);
				}
				uint num = (uint)value;
				uint num2 = (uint)(value >> 32);
				this._bits[0] = num;
				this._bits[1] = num2;
				this._length = ((num2 != 0U) ? 2 : ((num != 0U) ? 1 : 0));
			}

			public void Overwrite(uint value)
			{
				if (this._length > 1)
				{
					Array.Clear(this._bits, 1, this._length - 1);
				}
				this._bits[0] = value;
				this._length = ((value != 0U) ? 1 : 0);
			}

			public uint[] GetBits()
			{
				return this._bits;
			}

			public int GetSize()
			{
				return this._bits.Length;
			}

			public int GetLength()
			{
				return this._length;
			}

			public void Refresh(int maxLength)
			{
				if (this._length > maxLength)
				{
					Array.Clear(this._bits, maxLength, this._length - maxLength);
				}
				this._length = BigIntegerCalculator.ActualLength(this._bits, maxLength);
			}

			private void Apply(ref BigIntegerCalculator.BitsBuffer temp, int maxLength)
			{
				Array.Clear(this._bits, 0, this._length);
				uint[] bits = temp._bits;
				temp._bits = this._bits;
				this._bits = bits;
				this._length = BigIntegerCalculator.ActualLength(this._bits, maxLength);
			}

			private uint[] _bits;

			private int _length;
		}

		internal readonly struct FastReducer
		{
			public FastReducer(uint[] modulus)
			{
				uint[] array = new uint[modulus.Length * 2 + 1];
				array[array.Length - 1] = 1U;
				this._mu = BigIntegerCalculator.Divide(array, modulus);
				this._modulus = modulus;
				this._q1 = new uint[modulus.Length * 2 + 2];
				this._q2 = new uint[modulus.Length * 2 + 1];
				this._muLength = BigIntegerCalculator.ActualLength(this._mu);
			}

			public int Reduce(uint[] value, int length)
			{
				if (length < this._modulus.Length)
				{
					return length;
				}
				int num = BigIntegerCalculator.FastReducer.DivMul(value, length, this._mu, this._muLength, this._q1, this._modulus.Length - 1);
				int num2 = BigIntegerCalculator.FastReducer.DivMul(this._q1, num, this._modulus, this._modulus.Length, this._q2, this._modulus.Length + 1);
				return BigIntegerCalculator.FastReducer.SubMod(value, length, this._q2, num2, this._modulus, this._modulus.Length + 1);
			}

			private unsafe static int DivMul(uint[] left, int leftLength, uint[] right, int rightLength, uint[] bits, int k)
			{
				Array.Clear(bits, 0, bits.Length);
				if (leftLength > k)
				{
					leftLength -= k;
					fixed (uint[] array = left)
					{
						uint* ptr;
						if (left == null || array.Length == 0)
						{
							ptr = null;
						}
						else
						{
							ptr = &array[0];
						}
						fixed (uint[] array2 = right)
						{
							uint* ptr2;
							if (right == null || array2.Length == 0)
							{
								ptr2 = null;
							}
							else
							{
								ptr2 = &array2[0];
							}
							fixed (uint[] array3 = bits)
							{
								uint* ptr3;
								if (bits == null || array3.Length == 0)
								{
									ptr3 = null;
								}
								else
								{
									ptr3 = &array3[0];
								}
								if (leftLength < rightLength)
								{
									BigIntegerCalculator.Multiply(ptr2, rightLength, ptr + k, leftLength, ptr3, leftLength + rightLength);
								}
								else
								{
									BigIntegerCalculator.Multiply(ptr + k, leftLength, ptr2, rightLength, ptr3, leftLength + rightLength);
								}
								array = null;
								array2 = null;
							}
							return BigIntegerCalculator.ActualLength(bits, leftLength + rightLength);
						}
					}
				}
				return 0;
			}

			private unsafe static int SubMod(uint[] left, int leftLength, uint[] right, int rightLength, uint[] modulus, int k)
			{
				if (leftLength > k)
				{
					leftLength = k;
				}
				if (rightLength > k)
				{
					rightLength = k;
				}
				fixed (uint[] array = left)
				{
					uint* ptr;
					if (left == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					fixed (uint[] array2 = right)
					{
						uint* ptr2;
						if (right == null || array2.Length == 0)
						{
							ptr2 = null;
						}
						else
						{
							ptr2 = &array2[0];
						}
						fixed (uint[] array3 = modulus)
						{
							uint* ptr3;
							if (modulus == null || array3.Length == 0)
							{
								ptr3 = null;
							}
							else
							{
								ptr3 = &array3[0];
							}
							BigIntegerCalculator.SubtractSelf(ptr, leftLength, ptr2, rightLength);
							leftLength = BigIntegerCalculator.ActualLength(left, leftLength);
							while (BigIntegerCalculator.Compare(ptr, leftLength, ptr3, modulus.Length) >= 0)
							{
								BigIntegerCalculator.SubtractSelf(ptr, leftLength, ptr3, modulus.Length);
								leftLength = BigIntegerCalculator.ActualLength(left, leftLength);
							}
							array = null;
							array2 = null;
						}
						Array.Clear(left, leftLength, left.Length - leftLength);
						return leftLength;
					}
				}
			}

			private readonly uint[] _modulus;

			private readonly uint[] _mu;

			private readonly uint[] _q1;

			private readonly uint[] _q2;

			private readonly int _muLength;
		}
	}
}
