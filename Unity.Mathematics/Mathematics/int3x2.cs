using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct int3x2 : IEquatable<int3x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(int3 c0, int3 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(int m00, int m01, int m10, int m11, int m20, int m21)
		{
			this.c0 = new int3(m00, m10, m20);
			this.c1 = new int3(m01, m11, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(int v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(bool v)
		{
			this.c0 = math.select(new int3(0), new int3(1), v);
			this.c1 = math.select(new int3(0), new int3(1), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(bool3x2 v)
		{
			this.c0 = math.select(new int3(0), new int3(1), v.c0);
			this.c1 = math.select(new int3(0), new int3(1), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(uint v)
		{
			this.c0 = (int3)v;
			this.c1 = (int3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(uint3x2 v)
		{
			this.c0 = (int3)v.c0;
			this.c1 = (int3)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(float v)
		{
			this.c0 = (int3)v;
			this.c1 = (int3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(float3x2 v)
		{
			this.c0 = (int3)v.c0;
			this.c1 = (int3)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(double v)
		{
			this.c0 = (int3)v;
			this.c1 = (int3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x2(double3x2 v)
		{
			this.c0 = (int3)v.c0;
			this.c1 = (int3)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int3x2(int v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x2(bool v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x2(bool3x2 v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x2(uint v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x2(uint3x2 v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x2(float v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x2(float3x2 v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x2(double v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x2(double3x2 v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator *(int3x2 lhs, int3x2 rhs)
		{
			return new int3x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator *(int3x2 lhs, int rhs)
		{
			return new int3x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator *(int lhs, int3x2 rhs)
		{
			return new int3x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator +(int3x2 lhs, int3x2 rhs)
		{
			return new int3x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator +(int3x2 lhs, int rhs)
		{
			return new int3x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator +(int lhs, int3x2 rhs)
		{
			return new int3x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator -(int3x2 lhs, int3x2 rhs)
		{
			return new int3x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator -(int3x2 lhs, int rhs)
		{
			return new int3x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator -(int lhs, int3x2 rhs)
		{
			return new int3x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator /(int3x2 lhs, int3x2 rhs)
		{
			return new int3x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator /(int3x2 lhs, int rhs)
		{
			return new int3x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator /(int lhs, int3x2 rhs)
		{
			return new int3x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator %(int3x2 lhs, int3x2 rhs)
		{
			return new int3x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator %(int3x2 lhs, int rhs)
		{
			return new int3x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator %(int lhs, int3x2 rhs)
		{
			return new int3x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator ++(int3x2 val)
		{
			int3 @int = int3.op_Increment(val.c0);
			val.c0 = @int;
			int3 int2 = @int;
			@int = int3.op_Increment(val.c1);
			val.c1 = @int;
			return new int3x2(int2, @int);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator --(int3x2 val)
		{
			int3 @int = int3.op_Decrement(val.c0);
			val.c0 = @int;
			int3 int2 = @int;
			@int = int3.op_Decrement(val.c1);
			val.c1 = @int;
			return new int3x2(int2, @int);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(int3x2 lhs, int3x2 rhs)
		{
			return new bool3x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(int3x2 lhs, int rhs)
		{
			return new bool3x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(int lhs, int3x2 rhs)
		{
			return new bool3x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(int3x2 lhs, int3x2 rhs)
		{
			return new bool3x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(int3x2 lhs, int rhs)
		{
			return new bool3x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(int lhs, int3x2 rhs)
		{
			return new bool3x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(int3x2 lhs, int3x2 rhs)
		{
			return new bool3x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(int3x2 lhs, int rhs)
		{
			return new bool3x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(int lhs, int3x2 rhs)
		{
			return new bool3x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(int3x2 lhs, int3x2 rhs)
		{
			return new bool3x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(int3x2 lhs, int rhs)
		{
			return new bool3x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(int lhs, int3x2 rhs)
		{
			return new bool3x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator -(int3x2 val)
		{
			return new int3x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator +(int3x2 val)
		{
			return new int3x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator <<(int3x2 x, int n)
		{
			return new int3x2(x.c0 << n, x.c1 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator >>(int3x2 x, int n)
		{
			return new int3x2(x.c0 >> n, x.c1 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(int3x2 lhs, int3x2 rhs)
		{
			return new bool3x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(int3x2 lhs, int rhs)
		{
			return new bool3x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(int lhs, int3x2 rhs)
		{
			return new bool3x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(int3x2 lhs, int3x2 rhs)
		{
			return new bool3x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(int3x2 lhs, int rhs)
		{
			return new bool3x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(int lhs, int3x2 rhs)
		{
			return new bool3x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator ~(int3x2 val)
		{
			return new int3x2(~val.c0, ~val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator &(int3x2 lhs, int3x2 rhs)
		{
			return new int3x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator &(int3x2 lhs, int rhs)
		{
			return new int3x2(lhs.c0 & rhs, lhs.c1 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator &(int lhs, int3x2 rhs)
		{
			return new int3x2(lhs & rhs.c0, lhs & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator |(int3x2 lhs, int3x2 rhs)
		{
			return new int3x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator |(int3x2 lhs, int rhs)
		{
			return new int3x2(lhs.c0 | rhs, lhs.c1 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator |(int lhs, int3x2 rhs)
		{
			return new int3x2(lhs | rhs.c0, lhs | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator ^(int3x2 lhs, int3x2 rhs)
		{
			return new int3x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator ^(int3x2 lhs, int rhs)
		{
			return new int3x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 operator ^(int lhs, int3x2 rhs)
		{
			return new int3x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
		}

		public unsafe ref int3 this[int index]
		{
			get
			{
				fixed (int3x2* ptr = &this)
				{
					return ref *(int3*)(ptr + (IntPtr)index * (IntPtr)sizeof(int3) / (IntPtr)sizeof(int3x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(int3x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is int3x2)
			{
				int3x2 int3x = (int3x2)o;
				return this.Equals(int3x);
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
			return string.Format("int3x2({0}, {1},  {2}, {3},  {4}, {5})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y,
				this.c0.z,
				this.c1.z
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("int3x2({0}, {1},  {2}, {3},  {4}, {5})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider)
			});
		}

		public int3 c0;

		public int3 c1;

		public static readonly int3x2 zero;
	}
}
