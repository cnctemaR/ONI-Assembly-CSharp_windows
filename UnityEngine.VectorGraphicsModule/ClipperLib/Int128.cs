using System;

namespace ClipperLib
{
	internal struct Int128
	{
		public Int128(long _lo)
		{
			this.lo = (ulong)_lo;
			bool flag = _lo < 0L;
			if (flag)
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
			bool flag = val1 == val2;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = val1 == null || val2 == null;
				flag2 = !flag3 && val1.hi == val2.hi && val1.lo == val2.lo;
			}
			return flag2;
		}

		public static bool operator !=(Int128 val1, Int128 val2)
		{
			return !(val1 == val2);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null || !(obj is Int128);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				Int128 @int = (Int128)obj;
				flag2 = @int.hi == this.hi && @int.lo == this.lo;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			return this.hi.GetHashCode() ^ this.lo.GetHashCode();
		}

		public static bool operator >(Int128 val1, Int128 val2)
		{
			bool flag = val1.hi != val2.hi;
			bool flag2;
			if (flag)
			{
				flag2 = val1.hi > val2.hi;
			}
			else
			{
				flag2 = val1.lo > val2.lo;
			}
			return flag2;
		}

		public static bool operator <(Int128 val1, Int128 val2)
		{
			bool flag = val1.hi != val2.hi;
			bool flag2;
			if (flag)
			{
				flag2 = val1.hi < val2.hi;
			}
			else
			{
				flag2 = val1.lo < val2.lo;
			}
			return flag2;
		}

		public static Int128 operator +(Int128 lhs, Int128 rhs)
		{
			lhs.hi += rhs.hi;
			lhs.lo += rhs.lo;
			bool flag = lhs.lo < rhs.lo;
			if (flag)
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
			bool flag = val.lo == 0UL;
			Int128 @int;
			if (flag)
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
			bool flag = val.hi < 0L;
			double num;
			if (flag)
			{
				bool flag2 = val.lo == 0UL;
				if (flag2)
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
			bool flag2 = lhs < 0L;
			if (flag2)
			{
				lhs = -lhs;
			}
			bool flag3 = rhs < 0L;
			if (flag3)
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
			bool flag4 = num9 < num6;
			if (flag4)
			{
				num8 += 1L;
			}
			Int128 @int = new Int128(num8, num9);
			return flag ? (-@int) : @int;
		}

		private long hi;

		private ulong lo;
	}
}
