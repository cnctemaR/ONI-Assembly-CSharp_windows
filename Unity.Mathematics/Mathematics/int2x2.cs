using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct int2x2 : IEquatable<int2x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(int2 c0, int2 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(int m00, int m01, int m10, int m11)
		{
			this.c0 = new int2(m00, m10);
			this.c1 = new int2(m01, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(int v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(bool v)
		{
			this.c0 = math.select(new int2(0), new int2(1), v);
			this.c1 = math.select(new int2(0), new int2(1), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(bool2x2 v)
		{
			this.c0 = math.select(new int2(0), new int2(1), v.c0);
			this.c1 = math.select(new int2(0), new int2(1), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(uint v)
		{
			this.c0 = (int2)v;
			this.c1 = (int2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(uint2x2 v)
		{
			this.c0 = (int2)v.c0;
			this.c1 = (int2)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(float v)
		{
			this.c0 = (int2)v;
			this.c1 = (int2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(float2x2 v)
		{
			this.c0 = (int2)v.c0;
			this.c1 = (int2)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(double v)
		{
			this.c0 = (int2)v;
			this.c1 = (int2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2x2(double2x2 v)
		{
			this.c0 = (int2)v.c0;
			this.c1 = (int2)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int2x2(int v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2x2(bool v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2x2(bool2x2 v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2x2(uint v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2x2(uint2x2 v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2x2(float v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2x2(float2x2 v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2x2(double v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2x2(double2x2 v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator *(int2x2 lhs, int2x2 rhs)
		{
			return new int2x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator *(int2x2 lhs, int rhs)
		{
			return new int2x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator *(int lhs, int2x2 rhs)
		{
			return new int2x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator +(int2x2 lhs, int2x2 rhs)
		{
			return new int2x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator +(int2x2 lhs, int rhs)
		{
			return new int2x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator +(int lhs, int2x2 rhs)
		{
			return new int2x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator -(int2x2 lhs, int2x2 rhs)
		{
			return new int2x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator -(int2x2 lhs, int rhs)
		{
			return new int2x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator -(int lhs, int2x2 rhs)
		{
			return new int2x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator /(int2x2 lhs, int2x2 rhs)
		{
			return new int2x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator /(int2x2 lhs, int rhs)
		{
			return new int2x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator /(int lhs, int2x2 rhs)
		{
			return new int2x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator %(int2x2 lhs, int2x2 rhs)
		{
			return new int2x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator %(int2x2 lhs, int rhs)
		{
			return new int2x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator %(int lhs, int2x2 rhs)
		{
			return new int2x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator ++(int2x2 val)
		{
			int2 @int = int2.op_Increment(val.c0);
			val.c0 = @int;
			int2 int2 = @int;
			@int = int2.op_Increment(val.c1);
			val.c1 = @int;
			return new int2x2(int2, @int);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator --(int2x2 val)
		{
			int2 @int = int2.op_Decrement(val.c0);
			val.c0 = @int;
			int2 int2 = @int;
			@int = int2.op_Decrement(val.c1);
			val.c1 = @int;
			return new int2x2(int2, @int);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(int2x2 lhs, int2x2 rhs)
		{
			return new bool2x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(int2x2 lhs, int rhs)
		{
			return new bool2x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(int lhs, int2x2 rhs)
		{
			return new bool2x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(int2x2 lhs, int2x2 rhs)
		{
			return new bool2x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(int2x2 lhs, int rhs)
		{
			return new bool2x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(int lhs, int2x2 rhs)
		{
			return new bool2x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(int2x2 lhs, int2x2 rhs)
		{
			return new bool2x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(int2x2 lhs, int rhs)
		{
			return new bool2x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(int lhs, int2x2 rhs)
		{
			return new bool2x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(int2x2 lhs, int2x2 rhs)
		{
			return new bool2x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(int2x2 lhs, int rhs)
		{
			return new bool2x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(int lhs, int2x2 rhs)
		{
			return new bool2x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator -(int2x2 val)
		{
			return new int2x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator +(int2x2 val)
		{
			return new int2x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator <<(int2x2 x, int n)
		{
			return new int2x2(x.c0 << n, x.c1 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator >>(int2x2 x, int n)
		{
			return new int2x2(x.c0 >> n, x.c1 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(int2x2 lhs, int2x2 rhs)
		{
			return new bool2x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(int2x2 lhs, int rhs)
		{
			return new bool2x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(int lhs, int2x2 rhs)
		{
			return new bool2x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(int2x2 lhs, int2x2 rhs)
		{
			return new bool2x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(int2x2 lhs, int rhs)
		{
			return new bool2x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(int lhs, int2x2 rhs)
		{
			return new bool2x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator ~(int2x2 val)
		{
			return new int2x2(~val.c0, ~val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator &(int2x2 lhs, int2x2 rhs)
		{
			return new int2x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator &(int2x2 lhs, int rhs)
		{
			return new int2x2(lhs.c0 & rhs, lhs.c1 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator &(int lhs, int2x2 rhs)
		{
			return new int2x2(lhs & rhs.c0, lhs & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator |(int2x2 lhs, int2x2 rhs)
		{
			return new int2x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator |(int2x2 lhs, int rhs)
		{
			return new int2x2(lhs.c0 | rhs, lhs.c1 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator |(int lhs, int2x2 rhs)
		{
			return new int2x2(lhs | rhs.c0, lhs | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator ^(int2x2 lhs, int2x2 rhs)
		{
			return new int2x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator ^(int2x2 lhs, int rhs)
		{
			return new int2x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 operator ^(int lhs, int2x2 rhs)
		{
			return new int2x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
		}

		public unsafe ref int2 this[int index]
		{
			get
			{
				fixed (int2x2* ptr = &this)
				{
					return ref *(int2*)(ptr + (IntPtr)index * (IntPtr)sizeof(int2) / (IntPtr)sizeof(int2x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(int2x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is int2x2)
			{
				int2x2 int2x = (int2x2)o;
				return this.Equals(int2x);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return (int)math.hash(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return string.Format("int2x2({0}, {1},  {2}, {3})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("int2x2({0}, {1},  {2}, {3})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider)
			});
		}

		public int2 c0;

		public int2 c1;

		public static readonly int2x2 identity = new int2x2(1, 0, 0, 1);

		public static readonly int2x2 zero;
	}
}
