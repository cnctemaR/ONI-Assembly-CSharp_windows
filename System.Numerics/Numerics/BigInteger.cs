using System;
using System.Diagnostics;
using System.Globalization;

namespace System.Numerics
{
	[Serializable]
	public struct BigInteger : IFormattable, IComparable, IComparable<BigInteger>, IEquatable<BigInteger>
	{
		public BigInteger(int value)
		{
			if (value == -2147483648)
			{
				this = BigInteger.s_bnMinInt;
				return;
			}
			this._sign = value;
			this._bits = null;
		}

		[CLSCompliant(false)]
		public BigInteger(uint value)
		{
			if (value <= 2147483647U)
			{
				this._sign = (int)value;
				this._bits = null;
				return;
			}
			this._sign = 1;
			this._bits = new uint[1];
			this._bits[0] = value;
		}

		public BigInteger(long value)
		{
			if (-2147483648L < value && value <= 2147483647L)
			{
				this._sign = (int)value;
				this._bits = null;
				return;
			}
			if (value == -2147483648L)
			{
				this = BigInteger.s_bnMinInt;
				return;
			}
			ulong num;
			if (value < 0L)
			{
				num = (ulong)(-(ulong)value);
				this._sign = -1;
			}
			else
			{
				num = (ulong)value;
				this._sign = 1;
			}
			if (num <= (ulong)(-1))
			{
				this._bits = new uint[1];
				this._bits[0] = (uint)num;
				return;
			}
			this._bits = new uint[2];
			this._bits[0] = (uint)num;
			this._bits[1] = (uint)(num >> 32);
		}

		[CLSCompliant(false)]
		public BigInteger(ulong value)
		{
			if (value <= 2147483647UL)
			{
				this._sign = (int)value;
				this._bits = null;
				return;
			}
			if (value <= (ulong)(-1))
			{
				this._sign = 1;
				this._bits = new uint[1];
				this._bits[0] = (uint)value;
				return;
			}
			this._sign = 1;
			this._bits = new uint[2];
			this._bits[0] = (uint)value;
			this._bits[1] = (uint)(value >> 32);
		}

		public BigInteger(float value)
		{
			this = new BigInteger((double)value);
		}

		public BigInteger(double value)
		{
			if (!double.IsFinite(value))
			{
				if (double.IsInfinity(value))
				{
					throw new OverflowException("BigInteger cannot represent infinity.");
				}
				throw new OverflowException("The value is not a number.");
			}
			else
			{
				this._sign = 0;
				this._bits = null;
				int num;
				int num2;
				ulong num3;
				bool flag;
				NumericsHelpers.GetDoubleParts(value, out num, out num2, out num3, out flag);
				if (num3 == 0UL)
				{
					this = BigInteger.Zero;
					return;
				}
				if (num2 <= 0)
				{
					if (num2 <= -64)
					{
						this = BigInteger.Zero;
						return;
					}
					this = num3 >> -num2;
					if (num < 0)
					{
						this._sign = -this._sign;
						return;
					}
				}
				else if (num2 <= 11)
				{
					this = num3 << num2;
					if (num < 0)
					{
						this._sign = -this._sign;
						return;
					}
				}
				else
				{
					num3 <<= 11;
					num2 -= 11;
					int num4 = (num2 - 1) / 32 + 1;
					int num5 = num4 * 32 - num2;
					this._bits = new uint[num4 + 2];
					this._bits[num4 + 1] = (uint)(num3 >> num5 + 32);
					this._bits[num4] = (uint)(num3 >> num5);
					if (num5 > 0)
					{
						this._bits[num4 - 1] = (uint)num3 << 32 - num5;
					}
					this._sign = num;
				}
				return;
			}
		}

		public BigInteger(decimal value)
		{
			int[] bits = decimal.GetBits(decimal.Truncate(value));
			int num = 3;
			while (num > 0 && bits[num - 1] == 0)
			{
				num--;
			}
			if (num == 0)
			{
				this = BigInteger.s_bnZeroInt;
				return;
			}
			if (num == 1 && bits[0] > 0)
			{
				this._sign = bits[0];
				this._sign *= (((bits[3] & int.MinValue) != 0) ? (-1) : 1);
				this._bits = null;
				return;
			}
			this._bits = new uint[num];
			this._bits[0] = (uint)bits[0];
			if (num > 1)
			{
				this._bits[1] = (uint)bits[1];
			}
			if (num > 2)
			{
				this._bits[2] = (uint)bits[2];
			}
			this._sign = (((bits[3] & int.MinValue) != 0) ? (-1) : 1);
		}

		[CLSCompliant(false)]
		public BigInteger(byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this = new BigInteger(new ReadOnlySpan<byte>(value));
		}

		public BigInteger(ReadOnlySpan<byte> value)
		{
			int num = value.Length;
			bool flag;
			if (num > 0)
			{
				byte b = value[num - 1];
				flag = (b & 128) > 0;
				if (b == 0)
				{
					num -= 2;
					while (num >= 0 && value[num] == 0)
					{
						num--;
					}
					num++;
				}
			}
			else
			{
				flag = false;
			}
			if (num == 0)
			{
				this._sign = 0;
				this._bits = null;
				return;
			}
			if (num <= 4)
			{
				this._sign = (flag ? (-1) : 0);
				for (int i = num - 1; i >= 0; i--)
				{
					this._sign = (this._sign << 8) | (int)value[i];
				}
				this._bits = null;
				if (this._sign < 0 && !flag)
				{
					this._bits = new uint[] { (uint)this._sign };
					this._sign = 1;
				}
				if (this._sign == -2147483648)
				{
					this = BigInteger.s_bnMinInt;
					return;
				}
			}
			else
			{
				int num2 = num % 4;
				int num3 = num / 4 + ((num2 == 0) ? 0 : 1);
				uint[] array = new uint[num3];
				int j = 3;
				int k;
				for (k = 0; k < num3 - ((num2 == 0) ? 0 : 1); k++)
				{
					for (int l = 0; l < 4; l++)
					{
						byte b2 = value[j];
						array[k] = (array[k] << 8) | (uint)b2;
						j--;
					}
					j += 8;
				}
				if (num2 != 0)
				{
					if (flag)
					{
						array[num3 - 1] = uint.MaxValue;
					}
					for (j = num - 1; j >= num - num2; j--)
					{
						byte b3 = value[j];
						array[k] = (array[k] << 8) | (uint)b3;
					}
				}
				if (flag)
				{
					NumericsHelpers.DangerousMakeTwosComplement(array);
					int num4 = array.Length - 1;
					while (num4 >= 0 && array[num4] == 0U)
					{
						num4--;
					}
					num4++;
					if (num4 == 1)
					{
						uint num5 = array[0];
						if (num5 == 1U)
						{
							this = BigInteger.s_bnMinusOneInt;
							return;
						}
						if (num5 == 2147483648U)
						{
							this = BigInteger.s_bnMinInt;
							return;
						}
						if (array[0] > 0U)
						{
							this._sign = (int)(uint.MaxValue * array[0]);
							this._bits = null;
							return;
						}
					}
					if (num4 != array.Length)
					{
						this._sign = -1;
						this._bits = new uint[num4];
						Array.Copy(array, 0, this._bits, 0, num4);
						return;
					}
					this._sign = -1;
					this._bits = array;
					return;
				}
				else
				{
					this._sign = 1;
					this._bits = array;
				}
			}
		}

		internal BigInteger(int n, uint[] rgu)
		{
			this._sign = n;
			this._bits = rgu;
		}

		internal BigInteger(uint[] value, bool negative)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int num = value.Length;
			while (num > 0 && value[num - 1] == 0U)
			{
				num--;
			}
			if (num == 0)
			{
				this = BigInteger.s_bnZeroInt;
				return;
			}
			if (num == 1 && value[0] < 2147483648U)
			{
				this._sign = (int)(negative ? (-(int)value[0]) : value[0]);
				this._bits = null;
				if (this._sign == -2147483648)
				{
					this = BigInteger.s_bnMinInt;
					return;
				}
			}
			else
			{
				this._sign = (negative ? (-1) : 1);
				this._bits = new uint[num];
				Array.Copy(value, 0, this._bits, 0, num);
			}
		}

		private BigInteger(uint[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int num = value.Length;
			bool flag = num > 0 && (value[num - 1] & 2147483648U) == 2147483648U;
			while (num > 0 && value[num - 1] == 0U)
			{
				num--;
			}
			if (num == 0)
			{
				this = BigInteger.s_bnZeroInt;
				return;
			}
			if (num == 1)
			{
				if (value[0] < 0U && !flag)
				{
					this._bits = new uint[1];
					this._bits[0] = value[0];
					this._sign = 1;
					return;
				}
				if (2147483648U == value[0])
				{
					this = BigInteger.s_bnMinInt;
					return;
				}
				this._sign = (int)value[0];
				this._bits = null;
				return;
			}
			else if (!flag)
			{
				if (num != value.Length)
				{
					this._sign = 1;
					this._bits = new uint[num];
					Array.Copy(value, 0, this._bits, 0, num);
					return;
				}
				this._sign = 1;
				this._bits = value;
				return;
			}
			else
			{
				NumericsHelpers.DangerousMakeTwosComplement(value);
				int num2 = value.Length;
				while (num2 > 0 && value[num2 - 1] == 0U)
				{
					num2--;
				}
				if (num2 == 1 && value[0] > 0U)
				{
					if (value[0] == 1U)
					{
						this = BigInteger.s_bnMinusOneInt;
						return;
					}
					if (value[0] == 2147483648U)
					{
						this = BigInteger.s_bnMinInt;
						return;
					}
					this._sign = (int)(uint.MaxValue * value[0]);
					this._bits = null;
					return;
				}
				else
				{
					if (num2 != value.Length)
					{
						this._sign = -1;
						this._bits = new uint[num2];
						Array.Copy(value, 0, this._bits, 0, num2);
						return;
					}
					this._sign = -1;
					this._bits = value;
					return;
				}
			}
		}

		public static BigInteger Zero
		{
			get
			{
				return BigInteger.s_bnZeroInt;
			}
		}

		public static BigInteger One
		{
			get
			{
				return BigInteger.s_bnOneInt;
			}
		}

		public static BigInteger MinusOne
		{
			get
			{
				return BigInteger.s_bnMinusOneInt;
			}
		}

		public bool IsPowerOfTwo
		{
			get
			{
				if (this._bits == null)
				{
					return (this._sign & (this._sign - 1)) == 0 && this._sign != 0;
				}
				if (this._sign != 1)
				{
					return false;
				}
				int num = this._bits.Length - 1;
				if ((this._bits[num] & (this._bits[num] - 1U)) != 0U)
				{
					return false;
				}
				while (--num >= 0)
				{
					if (this._bits[num] != 0U)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool IsZero
		{
			get
			{
				return this._sign == 0;
			}
		}

		public bool IsOne
		{
			get
			{
				return this._sign == 1 && this._bits == null;
			}
		}

		public bool IsEven
		{
			get
			{
				if (this._bits != null)
				{
					return (this._bits[0] & 1U) == 0U;
				}
				return (this._sign & 1) == 0;
			}
		}

		public int Sign
		{
			get
			{
				return (this._sign >> 31) - (-this._sign >> 31);
			}
		}

		public static BigInteger Parse(string value)
		{
			return BigInteger.Parse(value, NumberStyles.Integer);
		}

		public static BigInteger Parse(string value, NumberStyles style)
		{
			return BigInteger.Parse(value, style, NumberFormatInfo.CurrentInfo);
		}

		public static BigInteger Parse(string value, IFormatProvider provider)
		{
			return BigInteger.Parse(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
		}

		public static BigInteger Parse(string value, NumberStyles style, IFormatProvider provider)
		{
			return BigNumber.ParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider));
		}

		public static bool TryParse(string value, out BigInteger result)
		{
			return BigInteger.TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
		}

		public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out BigInteger result)
		{
			return BigNumber.TryParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		public static BigInteger Parse(ReadOnlySpan<char> value, NumberStyles style = NumberStyles.Integer, IFormatProvider provider = null)
		{
			return BigNumber.ParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider));
		}

		public static bool TryParse(ReadOnlySpan<char> value, out BigInteger result, NumberStyles style = NumberStyles.Integer, IFormatProvider provider = null)
		{
			return BigNumber.TryParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		public static int Compare(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right);
		}

		public static BigInteger Abs(BigInteger value)
		{
			if (!(value >= BigInteger.Zero))
			{
				return -value;
			}
			return value;
		}

		public static BigInteger Add(BigInteger left, BigInteger right)
		{
			return left + right;
		}

		public static BigInteger Subtract(BigInteger left, BigInteger right)
		{
			return left - right;
		}

		public static BigInteger Multiply(BigInteger left, BigInteger right)
		{
			return left * right;
		}

		public static BigInteger Divide(BigInteger dividend, BigInteger divisor)
		{
			return dividend / divisor;
		}

		public static BigInteger Remainder(BigInteger dividend, BigInteger divisor)
		{
			return dividend % divisor;
		}

		public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder)
		{
			bool flag = dividend._bits == null;
			bool flag2 = divisor._bits == null;
			if (flag && flag2)
			{
				remainder = dividend._sign % divisor._sign;
				return dividend._sign / divisor._sign;
			}
			if (flag)
			{
				remainder = dividend;
				return BigInteger.s_bnZeroInt;
			}
			if (flag2)
			{
				uint num;
				uint[] array = BigIntegerCalculator.Divide(dividend._bits, NumericsHelpers.Abs(divisor._sign), out num);
				remainder = (long)((dividend._sign < 0) ? (ulong.MaxValue * (ulong)num) : ((ulong)num));
				return new BigInteger(array, (dividend._sign < 0) ^ (divisor._sign < 0));
			}
			if (dividend._bits.Length < divisor._bits.Length)
			{
				remainder = dividend;
				return BigInteger.s_bnZeroInt;
			}
			uint[] array3;
			uint[] array2 = BigIntegerCalculator.Divide(dividend._bits, divisor._bits, out array3);
			remainder = new BigInteger(array3, dividend._sign < 0);
			return new BigInteger(array2, (dividend._sign < 0) ^ (divisor._sign < 0));
		}

		public static BigInteger Negate(BigInteger value)
		{
			return -value;
		}

		public static double Log(BigInteger value)
		{
			return BigInteger.Log(value, 2.718281828459045);
		}

		public static double Log(BigInteger value, double baseValue)
		{
			if (value._sign < 0 || baseValue == 1.0)
			{
				return double.NaN;
			}
			if (baseValue == double.PositiveInfinity)
			{
				if (!value.IsOne)
				{
					return double.NaN;
				}
				return 0.0;
			}
			else
			{
				if (baseValue == 0.0 && !value.IsOne)
				{
					return double.NaN;
				}
				if (value._bits == null)
				{
					return Math.Log((double)value._sign, baseValue);
				}
				ulong num = (ulong)value._bits[value._bits.Length - 1];
				ulong num2 = (ulong)((value._bits.Length > 1) ? value._bits[value._bits.Length - 2] : 0U);
				ulong num3 = (ulong)((value._bits.Length > 2) ? value._bits[value._bits.Length - 3] : 0U);
				int num4 = NumericsHelpers.CbitHighZero((uint)num);
				long num5 = (long)value._bits.Length * 32L - (long)num4;
				return Math.Log((num << 32 + num4) | (num2 << num4) | (num3 >> 32 - num4), baseValue) + (double)(num5 - 64L) / Math.Log(baseValue, 2.0);
			}
		}

		public static double Log10(BigInteger value)
		{
			return BigInteger.Log(value, 10.0);
		}

		public static BigInteger GreatestCommonDivisor(BigInteger left, BigInteger right)
		{
			bool flag = left._bits == null;
			bool flag2 = right._bits == null;
			if (flag && flag2)
			{
				return BigIntegerCalculator.Gcd(NumericsHelpers.Abs(left._sign), NumericsHelpers.Abs(right._sign));
			}
			if (flag)
			{
				if (left._sign == 0)
				{
					return new BigInteger(right._bits, false);
				}
				return BigIntegerCalculator.Gcd(right._bits, NumericsHelpers.Abs(left._sign));
			}
			else if (flag2)
			{
				if (right._sign == 0)
				{
					return new BigInteger(left._bits, false);
				}
				return BigIntegerCalculator.Gcd(left._bits, NumericsHelpers.Abs(right._sign));
			}
			else
			{
				if (BigIntegerCalculator.Compare(left._bits, right._bits) < 0)
				{
					return BigInteger.GreatestCommonDivisor(right._bits, left._bits);
				}
				return BigInteger.GreatestCommonDivisor(left._bits, right._bits);
			}
		}

		private static BigInteger GreatestCommonDivisor(uint[] leftBits, uint[] rightBits)
		{
			if (rightBits.Length == 1)
			{
				uint num = BigIntegerCalculator.Remainder(leftBits, rightBits[0]);
				return BigIntegerCalculator.Gcd(rightBits[0], num);
			}
			if (rightBits.Length == 2)
			{
				uint[] array = BigIntegerCalculator.Remainder(leftBits, rightBits);
				ulong num2 = ((ulong)rightBits[1] << 32) | (ulong)rightBits[0];
				ulong num3 = ((ulong)array[1] << 32) | (ulong)array[0];
				return BigIntegerCalculator.Gcd(num2, num3);
			}
			return new BigInteger(BigIntegerCalculator.Gcd(leftBits, rightBits), false);
		}

		public static BigInteger Max(BigInteger left, BigInteger right)
		{
			if (left.CompareTo(right) < 0)
			{
				return right;
			}
			return left;
		}

		public static BigInteger Min(BigInteger left, BigInteger right)
		{
			if (left.CompareTo(right) <= 0)
			{
				return left;
			}
			return right;
		}

		public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
		{
			if (exponent.Sign < 0)
			{
				throw new ArgumentOutOfRangeException("exponent", "The number must be greater than or equal to zero.");
			}
			bool flag = value._bits == null;
			bool flag2 = exponent._bits == null;
			if (modulus._bits == null)
			{
				uint num = ((flag && flag2) ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent._sign), NumericsHelpers.Abs(modulus._sign)) : (flag ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), exponent._bits, NumericsHelpers.Abs(modulus._sign)) : (flag2 ? BigIntegerCalculator.Pow(value._bits, NumericsHelpers.Abs(exponent._sign), NumericsHelpers.Abs(modulus._sign)) : BigIntegerCalculator.Pow(value._bits, exponent._bits, NumericsHelpers.Abs(modulus._sign)))));
				return (long)((value._sign < 0 && !exponent.IsEven) ? (ulong.MaxValue * (ulong)num) : ((ulong)num));
			}
			return new BigInteger((flag && flag2) ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent._sign), modulus._bits) : (flag ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), exponent._bits, modulus._bits) : (flag2 ? BigIntegerCalculator.Pow(value._bits, NumericsHelpers.Abs(exponent._sign), modulus._bits) : BigIntegerCalculator.Pow(value._bits, exponent._bits, modulus._bits))), value._sign < 0 && !exponent.IsEven);
		}

		public static BigInteger Pow(BigInteger value, int exponent)
		{
			if (exponent < 0)
			{
				throw new ArgumentOutOfRangeException("exponent", "The number must be greater than or equal to zero.");
			}
			if (exponent == 0)
			{
				return BigInteger.s_bnOneInt;
			}
			if (exponent == 1)
			{
				return value;
			}
			bool flag = value._bits == null;
			if (flag)
			{
				if (value._sign == 1)
				{
					return value;
				}
				if (value._sign == -1)
				{
					if ((exponent & 1) == 0)
					{
						return BigInteger.s_bnOneInt;
					}
					return value;
				}
				else if (value._sign == 0)
				{
					return value;
				}
			}
			return new BigInteger(flag ? BigIntegerCalculator.Pow(NumericsHelpers.Abs(value._sign), NumericsHelpers.Abs(exponent)) : BigIntegerCalculator.Pow(value._bits, NumericsHelpers.Abs(exponent)), value._sign < 0 && (exponent & 1) != 0);
		}

		public override int GetHashCode()
		{
			if (this._bits == null)
			{
				return this._sign;
			}
			int num = this._sign;
			int num2 = this._bits.Length;
			while (--num2 >= 0)
			{
				num = NumericsHelpers.CombineHash(num, (int)this._bits[num2]);
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			return obj is BigInteger && this.Equals((BigInteger)obj);
		}

		public bool Equals(long other)
		{
			if (this._bits == null)
			{
				return (long)this._sign == other;
			}
			int num;
			if (((long)this._sign ^ other) < 0L || (num = this._bits.Length) > 2)
			{
				return false;
			}
			ulong num2 = (ulong)((other < 0L) ? (-(ulong)other) : other);
			if (num == 1)
			{
				return (ulong)this._bits[0] == num2;
			}
			return NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) == num2;
		}

		[CLSCompliant(false)]
		public bool Equals(ulong other)
		{
			if (this._sign < 0)
			{
				return false;
			}
			if (this._bits == null)
			{
				return (long)this._sign == (long)other;
			}
			int num = this._bits.Length;
			if (num > 2)
			{
				return false;
			}
			if (num == 1)
			{
				return (ulong)this._bits[0] == other;
			}
			return NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) == other;
		}

		public bool Equals(BigInteger other)
		{
			if (this._sign != other._sign)
			{
				return false;
			}
			if (this._bits == other._bits)
			{
				return true;
			}
			if (this._bits == null || other._bits == null)
			{
				return false;
			}
			int num = this._bits.Length;
			return num == other._bits.Length && BigInteger.GetDiffLength(this._bits, other._bits, num) == 0;
		}

		public int CompareTo(long other)
		{
			if (this._bits == null)
			{
				return ((long)this._sign).CompareTo(other);
			}
			int num;
			if (((long)this._sign ^ other) < 0L || (num = this._bits.Length) > 2)
			{
				return this._sign;
			}
			ulong num2 = (ulong)((other < 0L) ? (-(ulong)other) : other);
			ulong num3 = ((num == 2) ? NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) : ((ulong)this._bits[0]));
			return this._sign * num3.CompareTo(num2);
		}

		[CLSCompliant(false)]
		public int CompareTo(ulong other)
		{
			if (this._sign < 0)
			{
				return -1;
			}
			if (this._bits == null)
			{
				return ((ulong)((long)this._sign)).CompareTo(other);
			}
			int num = this._bits.Length;
			if (num > 2)
			{
				return 1;
			}
			return ((num == 2) ? NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) : ((ulong)this._bits[0])).CompareTo(other);
		}

		public int CompareTo(BigInteger other)
		{
			if ((this._sign ^ other._sign) < 0)
			{
				if (this._sign >= 0)
				{
					return 1;
				}
				return -1;
			}
			else if (this._bits == null)
			{
				if (other._bits != null)
				{
					return -other._sign;
				}
				if (this._sign < other._sign)
				{
					return -1;
				}
				if (this._sign <= other._sign)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				int num;
				int num2;
				if (other._bits == null || (num = this._bits.Length) > (num2 = other._bits.Length))
				{
					return this._sign;
				}
				if (num < num2)
				{
					return -this._sign;
				}
				int diffLength = BigInteger.GetDiffLength(this._bits, other._bits, num);
				if (diffLength == 0)
				{
					return 0;
				}
				if (this._bits[diffLength - 1] >= other._bits[diffLength - 1])
				{
					return this._sign;
				}
				return -this._sign;
			}
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is BigInteger))
			{
				throw new ArgumentException("The parameter must be a BigInteger.", "obj");
			}
			return this.CompareTo((BigInteger)obj);
		}

		public byte[] ToByteArray()
		{
			int num = 0;
			return this.TryGetBytes(BigInteger.GetBytesMode.AllocateArray, default(Span<byte>), ref num);
		}

		public bool TryWriteBytes(Span<byte> destination, out int bytesWritten)
		{
			bytesWritten = 0;
			return this.TryGetBytes(BigInteger.GetBytesMode.Span, destination, ref bytesWritten) != null;
		}

		public int GetByteCount()
		{
			int num = 0;
			this.TryGetBytes(BigInteger.GetBytesMode.Count, default(Span<byte>), ref num);
			return num;
		}

		private unsafe byte[] TryGetBytes(BigInteger.GetBytesMode mode, Span<byte> destination, ref int bytesWritten)
		{
			int sign = this._sign;
			if (sign != 0)
			{
				int num = 0;
				uint[] bits = this._bits;
				byte b;
				uint num2;
				if (bits == null)
				{
					b = ((sign < 0) ? byte.MaxValue : 0);
					num2 = (uint)sign;
				}
				else if (sign == -1)
				{
					b = byte.MaxValue;
					while (bits[num] == 0U)
					{
						num++;
					}
					num2 = ~bits[bits.Length - 1];
					if (bits.Length - 1 == num)
					{
						num2 += 1U;
					}
				}
				else
				{
					b = 0;
					num2 = bits[bits.Length - 1];
				}
				byte b2;
				int num3;
				if ((b2 = (byte)(num2 >> 24)) != b)
				{
					num3 = 3;
				}
				else if ((b2 = (byte)(num2 >> 16)) != b)
				{
					num3 = 2;
				}
				else if ((b2 = (byte)(num2 >> 8)) != b)
				{
					num3 = 1;
				}
				else
				{
					b2 = (byte)num2;
					num3 = 0;
				}
				bool flag = (b2 & 128) != (b & 128);
				int num4 = num3 + 1 + (flag ? 1 : 0);
				if (bits != null)
				{
					num4 = checked(4 * (bits.Length - 1) + num4);
				}
				byte[] array;
				if (mode != BigInteger.GetBytesMode.AllocateArray)
				{
					if (mode == BigInteger.GetBytesMode.Count)
					{
						bytesWritten = num4;
						return null;
					}
					if (destination.Length < num4)
					{
						return null;
					}
					bytesWritten = num4;
					array = BigInteger.s_success;
				}
				else
				{
					destination = (array = new byte[num4]);
				}
				int num5 = 0;
				if (bits != null)
				{
					for (int i = 0; i < bits.Length - 1; i++)
					{
						uint num6 = bits[i];
						if (sign == -1)
						{
							num6 = ~num6;
							if (i <= num)
							{
								num6 += 1U;
							}
						}
						*destination[num5++] = (byte)num6;
						*destination[num5++] = (byte)(num6 >> 8);
						*destination[num5++] = (byte)(num6 >> 16);
						*destination[num5++] = (byte)(num6 >> 24);
					}
				}
				*destination[num5] = (byte)num2;
				if (num3 != 0)
				{
					*destination[++num5] = (byte)(num2 >> 8);
					if (num3 != 1)
					{
						*destination[++num5] = (byte)(num2 >> 16);
						if (num3 != 2)
						{
							*destination[num5 + 1] = (byte)(num2 >> 24);
						}
					}
				}
				if (flag)
				{
					*destination[num4 - 1] = b;
				}
				return array;
			}
			if (mode == BigInteger.GetBytesMode.AllocateArray)
			{
				return new byte[1];
			}
			if (mode == BigInteger.GetBytesMode.Count)
			{
				bytesWritten = 1;
				return null;
			}
			if (destination.Length != 0)
			{
				*destination[0] = 0;
				bytesWritten = 1;
				return BigInteger.s_success;
			}
			return null;
		}

		private uint[] ToUInt32Array()
		{
			if (this._bits == null && this._sign == 0)
			{
				return new uint[1];
			}
			uint[] array;
			uint num;
			if (this._bits == null)
			{
				array = new uint[] { (uint)this._sign };
				num = ((this._sign < 0) ? uint.MaxValue : 0U);
			}
			else if (this._sign == -1)
			{
				array = (uint[])this._bits.Clone();
				NumericsHelpers.DangerousMakeTwosComplement(array);
				num = uint.MaxValue;
			}
			else
			{
				array = this._bits;
				num = 0U;
			}
			int num2 = array.Length - 1;
			while (num2 > 0 && array[num2] == num)
			{
				num2--;
			}
			bool flag = (array[num2] & 2147483648U) != (num & 2147483648U);
			uint[] array2 = new uint[num2 + 1 + (flag ? 1 : 0)];
			Array.Copy(array, 0, array2, 0, num2 + 1);
			if (flag)
			{
				array2[array2.Length - 1] = num;
			}
			return array2;
		}

		public override string ToString()
		{
			return BigNumber.FormatBigInteger(this, null, NumberFormatInfo.CurrentInfo);
		}

		public string ToString(IFormatProvider provider)
		{
			return BigNumber.FormatBigInteger(this, null, NumberFormatInfo.GetInstance(provider));
		}

		public string ToString(string format)
		{
			return BigNumber.FormatBigInteger(this, format, NumberFormatInfo.CurrentInfo);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return BigNumber.FormatBigInteger(this, format, NumberFormatInfo.GetInstance(provider));
		}

		private static BigInteger Add(uint[] leftBits, int leftSign, uint[] rightBits, int rightSign)
		{
			bool flag = leftBits == null;
			bool flag2 = rightBits == null;
			if (flag && flag2)
			{
				return (long)leftSign + (long)rightSign;
			}
			if (flag)
			{
				return new BigInteger(BigIntegerCalculator.Add(rightBits, NumericsHelpers.Abs(leftSign)), leftSign < 0);
			}
			if (flag2)
			{
				return new BigInteger(BigIntegerCalculator.Add(leftBits, NumericsHelpers.Abs(rightSign)), leftSign < 0);
			}
			if (leftBits.Length < rightBits.Length)
			{
				return new BigInteger(BigIntegerCalculator.Add(rightBits, leftBits), leftSign < 0);
			}
			return new BigInteger(BigIntegerCalculator.Add(leftBits, rightBits), leftSign < 0);
		}

		public static BigInteger operator -(BigInteger left, BigInteger right)
		{
			if (left._sign < 0 != right._sign < 0)
			{
				return BigInteger.Add(left._bits, left._sign, right._bits, -1 * right._sign);
			}
			return BigInteger.Subtract(left._bits, left._sign, right._bits, right._sign);
		}

		private static BigInteger Subtract(uint[] leftBits, int leftSign, uint[] rightBits, int rightSign)
		{
			bool flag = leftBits == null;
			bool flag2 = rightBits == null;
			if (flag && flag2)
			{
				return (long)leftSign - (long)rightSign;
			}
			if (flag)
			{
				return new BigInteger(BigIntegerCalculator.Subtract(rightBits, NumericsHelpers.Abs(leftSign)), leftSign >= 0);
			}
			if (flag2)
			{
				return new BigInteger(BigIntegerCalculator.Subtract(leftBits, NumericsHelpers.Abs(rightSign)), leftSign < 0);
			}
			if (BigIntegerCalculator.Compare(leftBits, rightBits) < 0)
			{
				return new BigInteger(BigIntegerCalculator.Subtract(rightBits, leftBits), leftSign >= 0);
			}
			return new BigInteger(BigIntegerCalculator.Subtract(leftBits, rightBits), leftSign < 0);
		}

		public static implicit operator BigInteger(byte value)
		{
			return new BigInteger((int)value);
		}

		[CLSCompliant(false)]
		public static implicit operator BigInteger(sbyte value)
		{
			return new BigInteger((int)value);
		}

		public static implicit operator BigInteger(short value)
		{
			return new BigInteger((int)value);
		}

		[CLSCompliant(false)]
		public static implicit operator BigInteger(ushort value)
		{
			return new BigInteger((int)value);
		}

		public static implicit operator BigInteger(int value)
		{
			return new BigInteger(value);
		}

		[CLSCompliant(false)]
		public static implicit operator BigInteger(uint value)
		{
			return new BigInteger(value);
		}

		public static implicit operator BigInteger(long value)
		{
			return new BigInteger(value);
		}

		[CLSCompliant(false)]
		public static implicit operator BigInteger(ulong value)
		{
			return new BigInteger(value);
		}

		public static explicit operator BigInteger(float value)
		{
			return new BigInteger(value);
		}

		public static explicit operator BigInteger(double value)
		{
			return new BigInteger(value);
		}

		public static explicit operator BigInteger(decimal value)
		{
			return new BigInteger(value);
		}

		public static explicit operator byte(BigInteger value)
		{
			return checked((byte)(int)value);
		}

		[CLSCompliant(false)]
		public static explicit operator sbyte(BigInteger value)
		{
			return checked((sbyte)(int)value);
		}

		public static explicit operator short(BigInteger value)
		{
			return checked((short)(int)value);
		}

		[CLSCompliant(false)]
		public static explicit operator ushort(BigInteger value)
		{
			return checked((ushort)(int)value);
		}

		public static explicit operator int(BigInteger value)
		{
			if (value._bits == null)
			{
				return value._sign;
			}
			if (value._bits.Length > 1)
			{
				throw new OverflowException("Value was either too large or too small for an Int32.");
			}
			if (value._sign > 0)
			{
				return checked((int)value._bits[0]);
			}
			if (value._bits[0] > 2147483648U)
			{
				throw new OverflowException("Value was either too large or too small for an Int32.");
			}
			return (int)(-(int)value._bits[0]);
		}

		[CLSCompliant(false)]
		public static explicit operator uint(BigInteger value)
		{
			if (value._bits == null)
			{
				return checked((uint)value._sign);
			}
			if (value._bits.Length > 1 || value._sign < 0)
			{
				throw new OverflowException("Value was either too large or too small for a UInt32.");
			}
			return value._bits[0];
		}

		public static explicit operator long(BigInteger value)
		{
			if (value._bits == null)
			{
				return (long)value._sign;
			}
			int num = value._bits.Length;
			if (num > 2)
			{
				throw new OverflowException("Value was either too large or too small for an Int64.");
			}
			ulong num2;
			if (num > 1)
			{
				num2 = NumericsHelpers.MakeUlong(value._bits[1], value._bits[0]);
			}
			else
			{
				num2 = (ulong)value._bits[0];
			}
			long num3 = (long)((value._sign > 0) ? num2 : (-(long)num2));
			if ((num3 > 0L && value._sign > 0) || (num3 < 0L && value._sign < 0))
			{
				return num3;
			}
			throw new OverflowException("Value was either too large or too small for an Int64.");
		}

		[CLSCompliant(false)]
		public static explicit operator ulong(BigInteger value)
		{
			if (value._bits == null)
			{
				return checked((ulong)value._sign);
			}
			int num = value._bits.Length;
			if (num > 2 || value._sign < 0)
			{
				throw new OverflowException("Value was either too large or too small for a UInt64.");
			}
			if (num > 1)
			{
				return NumericsHelpers.MakeUlong(value._bits[1], value._bits[0]);
			}
			return (ulong)value._bits[0];
		}

		public static explicit operator float(BigInteger value)
		{
			return (float)(double)value;
		}

		public static explicit operator double(BigInteger value)
		{
			int sign = value._sign;
			uint[] bits = value._bits;
			if (bits == null)
			{
				return (double)sign;
			}
			int num = bits.Length;
			if (num <= 32)
			{
				ulong num2 = (ulong)bits[num - 1];
				ulong num3 = (ulong)((num > 1) ? bits[num - 2] : 0U);
				ulong num4 = (ulong)((num > 2) ? bits[num - 3] : 0U);
				int num5 = NumericsHelpers.CbitHighZero((uint)num2);
				int num6 = (num - 2) * 32 - num5;
				ulong num7 = (num2 << 32 + num5) | (num3 << num5) | (num4 >> 32 - num5);
				return NumericsHelpers.GetDoubleFromParts(sign, num6, num7);
			}
			if (sign == 1)
			{
				return double.PositiveInfinity;
			}
			return double.NegativeInfinity;
		}

		public static explicit operator decimal(BigInteger value)
		{
			if (value._bits == null)
			{
				return value._sign;
			}
			int num = value._bits.Length;
			if (num > 3)
			{
				throw new OverflowException("Value was either too large or too small for a Decimal.");
			}
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (num > 2)
			{
				num4 = (int)value._bits[2];
			}
			if (num > 1)
			{
				num3 = (int)value._bits[1];
			}
			if (num > 0)
			{
				num2 = (int)value._bits[0];
			}
			return new decimal(num2, num3, num4, value._sign < 0, 0);
		}

		public static BigInteger operator &(BigInteger left, BigInteger right)
		{
			if (left.IsZero || right.IsZero)
			{
				return BigInteger.Zero;
			}
			if (left._bits == null && right._bits == null)
			{
				return left._sign & right._sign;
			}
			uint[] array = left.ToUInt32Array();
			uint[] array2 = right.ToUInt32Array();
			uint[] array3 = new uint[Math.Max(array.Length, array2.Length)];
			uint num = ((left._sign < 0) ? uint.MaxValue : 0U);
			uint num2 = ((right._sign < 0) ? uint.MaxValue : 0U);
			for (int i = 0; i < array3.Length; i++)
			{
				uint num3 = ((i < array.Length) ? array[i] : num);
				uint num4 = ((i < array2.Length) ? array2[i] : num2);
				array3[i] = num3 & num4;
			}
			return new BigInteger(array3);
		}

		public static BigInteger operator |(BigInteger left, BigInteger right)
		{
			if (left.IsZero)
			{
				return right;
			}
			if (right.IsZero)
			{
				return left;
			}
			if (left._bits == null && right._bits == null)
			{
				return left._sign | right._sign;
			}
			uint[] array = left.ToUInt32Array();
			uint[] array2 = right.ToUInt32Array();
			uint[] array3 = new uint[Math.Max(array.Length, array2.Length)];
			uint num = ((left._sign < 0) ? uint.MaxValue : 0U);
			uint num2 = ((right._sign < 0) ? uint.MaxValue : 0U);
			for (int i = 0; i < array3.Length; i++)
			{
				uint num3 = ((i < array.Length) ? array[i] : num);
				uint num4 = ((i < array2.Length) ? array2[i] : num2);
				array3[i] = num3 | num4;
			}
			return new BigInteger(array3);
		}

		public static BigInteger operator ^(BigInteger left, BigInteger right)
		{
			if (left._bits == null && right._bits == null)
			{
				return left._sign ^ right._sign;
			}
			uint[] array = left.ToUInt32Array();
			uint[] array2 = right.ToUInt32Array();
			uint[] array3 = new uint[Math.Max(array.Length, array2.Length)];
			uint num = ((left._sign < 0) ? uint.MaxValue : 0U);
			uint num2 = ((right._sign < 0) ? uint.MaxValue : 0U);
			for (int i = 0; i < array3.Length; i++)
			{
				uint num3 = ((i < array.Length) ? array[i] : num);
				uint num4 = ((i < array2.Length) ? array2[i] : num2);
				array3[i] = num3 ^ num4;
			}
			return new BigInteger(array3);
		}

		public static BigInteger operator <<(BigInteger value, int shift)
		{
			if (shift == 0)
			{
				return value;
			}
			if (shift == -2147483648)
			{
				return value >> int.MaxValue >> 1;
			}
			if (shift < 0)
			{
				return value >> -shift;
			}
			int num = shift / 32;
			int num2 = shift - num * 32;
			uint[] array;
			int num3;
			bool partsForBitManipulation = BigInteger.GetPartsForBitManipulation(ref value, out array, out num3);
			uint[] array2 = new uint[num3 + num + 1];
			if (num2 == 0)
			{
				for (int i = 0; i < num3; i++)
				{
					array2[i + num] = array[i];
				}
			}
			else
			{
				int num4 = 32 - num2;
				uint num5 = 0U;
				int j;
				for (j = 0; j < num3; j++)
				{
					uint num6 = array[j];
					array2[j + num] = (num6 << num2) | num5;
					num5 = num6 >> num4;
				}
				array2[j + num] = num5;
			}
			return new BigInteger(array2, partsForBitManipulation);
		}

		public static BigInteger operator >>(BigInteger value, int shift)
		{
			if (shift == 0)
			{
				return value;
			}
			if (shift == -2147483648)
			{
				return value << int.MaxValue << 1;
			}
			if (shift < 0)
			{
				return value << -shift;
			}
			int num = shift / 32;
			int num2 = shift - num * 32;
			uint[] array;
			int num3;
			bool partsForBitManipulation = BigInteger.GetPartsForBitManipulation(ref value, out array, out num3);
			if (partsForBitManipulation)
			{
				if (shift >= 32 * num3)
				{
					return BigInteger.MinusOne;
				}
				uint[] array2 = new uint[num3];
				Array.Copy(array, 0, array2, 0, num3);
				array = array2;
				NumericsHelpers.DangerousMakeTwosComplement(array);
			}
			int num4 = num3 - num;
			if (num4 < 0)
			{
				num4 = 0;
			}
			uint[] array3 = new uint[num4];
			if (num2 == 0)
			{
				for (int i = num3 - 1; i >= num; i--)
				{
					array3[i - num] = array[i];
				}
			}
			else
			{
				int num5 = 32 - num2;
				uint num6 = 0U;
				for (int j = num3 - 1; j >= num; j--)
				{
					uint num7 = array[j];
					if (partsForBitManipulation && j == num3 - 1)
					{
						array3[j - num] = (num7 >> num2) | (uint.MaxValue << num5);
					}
					else
					{
						array3[j - num] = (num7 >> num2) | num6;
					}
					num6 = num7 << num5;
				}
			}
			if (partsForBitManipulation)
			{
				NumericsHelpers.DangerousMakeTwosComplement(array3);
			}
			return new BigInteger(array3, partsForBitManipulation);
		}

		public static BigInteger operator ~(BigInteger value)
		{
			return -(value + BigInteger.One);
		}

		public static BigInteger operator -(BigInteger value)
		{
			return new BigInteger(-value._sign, value._bits);
		}

		public static BigInteger operator +(BigInteger value)
		{
			return value;
		}

		public static BigInteger operator ++(BigInteger value)
		{
			return value + BigInteger.One;
		}

		public static BigInteger operator --(BigInteger value)
		{
			return value - BigInteger.One;
		}

		public static BigInteger operator +(BigInteger left, BigInteger right)
		{
			if (left._sign < 0 != right._sign < 0)
			{
				return BigInteger.Subtract(left._bits, left._sign, right._bits, -1 * right._sign);
			}
			return BigInteger.Add(left._bits, left._sign, right._bits, right._sign);
		}

		public static BigInteger operator *(BigInteger left, BigInteger right)
		{
			bool flag = left._bits == null;
			bool flag2 = right._bits == null;
			if (flag && flag2)
			{
				return (long)left._sign * (long)right._sign;
			}
			if (flag)
			{
				return new BigInteger(BigIntegerCalculator.Multiply(right._bits, NumericsHelpers.Abs(left._sign)), (left._sign < 0) ^ (right._sign < 0));
			}
			if (flag2)
			{
				return new BigInteger(BigIntegerCalculator.Multiply(left._bits, NumericsHelpers.Abs(right._sign)), (left._sign < 0) ^ (right._sign < 0));
			}
			if (left._bits == right._bits)
			{
				return new BigInteger(BigIntegerCalculator.Square(left._bits), (left._sign < 0) ^ (right._sign < 0));
			}
			if (left._bits.Length < right._bits.Length)
			{
				return new BigInteger(BigIntegerCalculator.Multiply(right._bits, left._bits), (left._sign < 0) ^ (right._sign < 0));
			}
			return new BigInteger(BigIntegerCalculator.Multiply(left._bits, right._bits), (left._sign < 0) ^ (right._sign < 0));
		}

		public static BigInteger operator /(BigInteger dividend, BigInteger divisor)
		{
			bool flag = dividend._bits == null;
			bool flag2 = divisor._bits == null;
			if (flag && flag2)
			{
				return dividend._sign / divisor._sign;
			}
			if (flag)
			{
				return BigInteger.s_bnZeroInt;
			}
			if (flag2)
			{
				return new BigInteger(BigIntegerCalculator.Divide(dividend._bits, NumericsHelpers.Abs(divisor._sign)), (dividend._sign < 0) ^ (divisor._sign < 0));
			}
			if (dividend._bits.Length < divisor._bits.Length)
			{
				return BigInteger.s_bnZeroInt;
			}
			return new BigInteger(BigIntegerCalculator.Divide(dividend._bits, divisor._bits), (dividend._sign < 0) ^ (divisor._sign < 0));
		}

		public static BigInteger operator %(BigInteger dividend, BigInteger divisor)
		{
			bool flag = dividend._bits == null;
			bool flag2 = divisor._bits == null;
			if (flag && flag2)
			{
				return dividend._sign % divisor._sign;
			}
			if (flag)
			{
				return dividend;
			}
			if (flag2)
			{
				uint num = BigIntegerCalculator.Remainder(dividend._bits, NumericsHelpers.Abs(divisor._sign));
				return (long)((dividend._sign < 0) ? (ulong.MaxValue * (ulong)num) : ((ulong)num));
			}
			if (dividend._bits.Length < divisor._bits.Length)
			{
				return dividend;
			}
			return new BigInteger(BigIntegerCalculator.Remainder(dividend._bits, divisor._bits), dividend._sign < 0);
		}

		public static bool operator <(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator <=(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator >=(BigInteger left, BigInteger right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static bool operator ==(BigInteger left, BigInteger right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(BigInteger left, BigInteger right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(BigInteger left, long right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator <=(BigInteger left, long right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >(BigInteger left, long right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator >=(BigInteger left, long right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static bool operator ==(BigInteger left, long right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(BigInteger left, long right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(long left, BigInteger right)
		{
			return right.CompareTo(left) > 0;
		}

		public static bool operator <=(long left, BigInteger right)
		{
			return right.CompareTo(left) >= 0;
		}

		public static bool operator >(long left, BigInteger right)
		{
			return right.CompareTo(left) < 0;
		}

		public static bool operator >=(long left, BigInteger right)
		{
			return right.CompareTo(left) <= 0;
		}

		public static bool operator ==(long left, BigInteger right)
		{
			return right.Equals(left);
		}

		public static bool operator !=(long left, BigInteger right)
		{
			return !right.Equals(left);
		}

		[CLSCompliant(false)]
		public static bool operator <(BigInteger left, ulong right)
		{
			return left.CompareTo(right) < 0;
		}

		[CLSCompliant(false)]
		public static bool operator <=(BigInteger left, ulong right)
		{
			return left.CompareTo(right) <= 0;
		}

		[CLSCompliant(false)]
		public static bool operator >(BigInteger left, ulong right)
		{
			return left.CompareTo(right) > 0;
		}

		[CLSCompliant(false)]
		public static bool operator >=(BigInteger left, ulong right)
		{
			return left.CompareTo(right) >= 0;
		}

		[CLSCompliant(false)]
		public static bool operator ==(BigInteger left, ulong right)
		{
			return left.Equals(right);
		}

		[CLSCompliant(false)]
		public static bool operator !=(BigInteger left, ulong right)
		{
			return !left.Equals(right);
		}

		[CLSCompliant(false)]
		public static bool operator <(ulong left, BigInteger right)
		{
			return right.CompareTo(left) > 0;
		}

		[CLSCompliant(false)]
		public static bool operator <=(ulong left, BigInteger right)
		{
			return right.CompareTo(left) >= 0;
		}

		[CLSCompliant(false)]
		public static bool operator >(ulong left, BigInteger right)
		{
			return right.CompareTo(left) < 0;
		}

		[CLSCompliant(false)]
		public static bool operator >=(ulong left, BigInteger right)
		{
			return right.CompareTo(left) <= 0;
		}

		[CLSCompliant(false)]
		public static bool operator ==(ulong left, BigInteger right)
		{
			return right.Equals(left);
		}

		[CLSCompliant(false)]
		public static bool operator !=(ulong left, BigInteger right)
		{
			return !right.Equals(left);
		}

		private static bool GetPartsForBitManipulation(ref BigInteger x, out uint[] xd, out int xl)
		{
			if (x._bits == null)
			{
				if (x._sign < 0)
				{
					xd = new uint[] { (uint)(-(uint)x._sign) };
				}
				else
				{
					xd = new uint[] { (uint)x._sign };
				}
			}
			else
			{
				xd = x._bits;
			}
			xl = ((x._bits == null) ? 1 : x._bits.Length);
			return x._sign < 0;
		}

		internal static int GetDiffLength(uint[] rgu1, uint[] rgu2, int cu)
		{
			int num = cu;
			while (--num >= 0)
			{
				if (rgu1[num] != rgu2[num])
				{
					return num + 1;
				}
			}
			return 0;
		}

		[Conditional("DEBUG")]
		private void AssertValid()
		{
			uint[] bits = this._bits;
		}

		private const int knMaskHighBit = -2147483648;

		private const uint kuMaskHighBit = 2147483648U;

		private const int kcbitUint = 32;

		private const int kcbitUlong = 64;

		private const int DecimalScaleFactorMask = 16711680;

		private const int DecimalSignMask = -2147483648;

		internal readonly int _sign;

		internal readonly uint[] _bits;

		private static readonly BigInteger s_bnMinInt = new BigInteger(-1, new uint[] { 2147483648U });

		private static readonly BigInteger s_bnOneInt = new BigInteger(1);

		private static readonly BigInteger s_bnZeroInt = new BigInteger(0);

		private static readonly BigInteger s_bnMinusOneInt = new BigInteger(-1);

		private static readonly byte[] s_success = Array.Empty<byte>();

		private enum GetBytesMode
		{
			AllocateArray,
			Count,
			Span
		}
	}
}
