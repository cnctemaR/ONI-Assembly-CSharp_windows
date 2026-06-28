using System;

namespace ClipperLib
{
	internal struct Int128
	{
		public Int128(long _lo)
		{
			this.lo = (ulong)_lo;
			if (_lo < 0L)
			{
				this.hi = -1L;
			}
			else
			{
				this.hi = 0L;
			}
		}

		public Int128(long _hi, ulong _lo)
		{
			this.lo = _lo;
			this.hi = _hi;
		}

		public Int128(Int128 val)
		{
			this.hi = val.hi;
			this.lo = val.lo;
		}

		public bool IsNegative()
		{
			return this.hi < 0L;
		}

		public static bool operator ==(Int128 val1, Int128 val2)
		{
			return val1 == val2 || (val1 != null && val2 != null && val1.hi == val2.hi && val1.lo == val2.lo);
		}

		public static bool operator !=(Int128 val1, Int128 val2)
		{
			return !(val1 == val2);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj == null || !(obj is Int128))
			{
				flag = false;
			}
			else
			{
				Int128 @int = (Int128)obj;
				flag = @int.hi == this.hi && @int.lo == this.lo;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.hi.GetHashCode() ^ this.lo.GetHashCode();
		}

		public static bool operator >(Int128 val1, Int128 val2)
		{
			bool flag;
			if (val1.hi != val2.hi)
			{
				flag = val1.hi > val2.hi;
			}
			else
			{
				flag = val1.lo > val2.lo;
			}
			return flag;
		}

		public static bool operator <(Int128 val1, Int128 val2)
		{
			bool flag;
			if (val1.hi != val2.hi)
			{
				flag = val1.hi < val2.hi;
			}
			else
			{
				flag = val1.lo < val2.lo;
			}
			return flag;
		}

		public static Int128 operator +(Int128 lhs, Int128 rhs)
		{
			lhs.hi += rhs.hi;
			lhs.lo += rhs.lo;
			if (lhs.lo < rhs.lo)
			{
				lhs.hi += 1L;
			}
			return lhs;
		}

		public static Int128 operator -(Int128 lhs, Int128 rhs)
		{
			return lhs + -rhs;
		}

		public static Int128 operator -(Int128 val)
		{
			Int128 @int;
			if (val.lo == 0UL)
			{
				@int = new Int128(-val.hi, 0UL);
			}
			else
			{
				@int = new Int128(~val.hi, ~val.lo + 1UL);
			}
			return @int;
		}

		public static explicit operator double(Int128 val)
		{
			double num;
			if (val.hi < 0L)
			{
				if (val.lo == 0UL)
				{
					num = (double)val.hi * 1.8446744073709552E+19;
				}
				else
				{
					num = -(~val.lo + (double)(~(double)val.hi) * 1.8446744073709552E+19);
				}
			}
			else
			{
				num = val.lo + (double)val.hi * 1.8446744073709552E+19;
			}
			return num;
		}

		public static Int128 Int128Mul(long lhs, long rhs)
		{
			bool flag = lhs < 0L != rhs < 0L;
			if (lhs < 0L)
			{
				lhs = -lhs;
			}
			if (rhs < 0L)
			{
				rhs = -rhs;
			}
			ulong num = (ulong)lhs >> 32;
			ulong num2 = (ulong)(lhs & (long)((ulong)(-1)));
			ulong num3 = (ulong)rhs >> 32;
			ulong num4 = (ulong)(rhs & (long)((ulong)(-1)));
			ulong num5 = num * num3;
			ulong num6 = num2 * num4;
			ulong num7 = num * num4 + num2 * num3;
			long num8 = (long)(num5 + (num7 >> 32));
			ulong num9 = (num7 << 32) + num6;
			if (num9 < num6)
			{
				num8 += 1L;
			}
			Int128 @int = new Int128(num8, num9);
			return (!flag) ? @int : (-@int);
		}

		private long hi;

		private ulong lo;
	}
}
