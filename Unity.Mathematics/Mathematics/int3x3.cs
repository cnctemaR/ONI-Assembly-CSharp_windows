using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct int3x3 : IEquatable<int3x3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(int3 c0, int3 c1, int3 c2)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(int m00, int m01, int m02, int m10, int m11, int m12, int m20, int m21, int m22)
		{
			this.c0 = new int3(m00, m10, m20);
			this.c1 = new int3(m01, m11, m21);
			this.c2 = new int3(m02, m12, m22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(bool v)
		{
			this.c0 = math.select(new int3(0), new int3(1), v);
			this.c1 = math.select(new int3(0), new int3(1), v);
			this.c2 = math.select(new int3(0), new int3(1), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(bool3x3 v)
		{
			this.c0 = math.select(new int3(0), new int3(1), v.c0);
			this.c1 = math.select(new int3(0), new int3(1), v.c1);
			this.c2 = math.select(new int3(0), new int3(1), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(uint v)
		{
			this.c0 = (int3)v;
			this.c1 = (int3)v;
			this.c2 = (int3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(uint3x3 v)
		{
			this.c0 = (int3)v.c0;
			this.c1 = (int3)v.c1;
			this.c2 = (int3)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(float v)
		{
			this.c0 = (int3)v;
			this.c1 = (int3)v;
			this.c2 = (int3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(float3x3 v)
		{
			this.c0 = (int3)v.c0;
			this.c1 = (int3)v.c1;
			this.c2 = (int3)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(double v)
		{
			this.c0 = (int3)v;
			this.c1 = (int3)v;
			this.c2 = (int3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3x3(double3x3 v)
		{
			this.c0 = (int3)v.c0;
			this.c1 = (int3)v.c1;
			this.c2 = (int3)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int3x3(int v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x3(bool v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x3(bool3x3 v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x3(uint v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x3(uint3x3 v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x3(float v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x3(float3x3 v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x3(double v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3x3(double3x3 v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator *(int3x3 lhs, int3x3 rhs)
		{
			return new int3x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator *(int3x3 lhs, int rhs)
		{
			return new int3x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator *(int lhs, int3x3 rhs)
		{
			return new int3x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator +(int3x3 lhs, int3x3 rhs)
		{
			return new int3x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator +(int3x3 lhs, int rhs)
		{
			return new int3x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator +(int lhs, int3x3 rhs)
		{
			return new int3x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator -(int3x3 lhs, int3x3 rhs)
		{
			return new int3x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator -(int3x3 lhs, int rhs)
		{
			return new int3x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator -(int lhs, int3x3 rhs)
		{
			return new int3x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator /(int3x3 lhs, int3x3 rhs)
		{
			return new int3x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator /(int3x3 lhs, int rhs)
		{
			return new int3x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator /(int lhs, int3x3 rhs)
		{
			return new int3x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator %(int3x3 lhs, int3x3 rhs)
		{
			return new int3x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator %(int3x3 lhs, int rhs)
		{
			return new int3x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator %(int lhs, int3x3 rhs)
		{
			return new int3x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator ++(int3x3 val)
		{
			int3 @int = int3.op_Increment(val.c0);
			val.c0 = @int;
			int3 int2 = @int;
			@int = int3.op_Increment(val.c1);
			val.c1 = @int;
			int3 int3 = @int;
			@int = int3.op_Increment(val.c2);
			val.c2 = @int;
			return new int3x3(int2, int3, @int);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator --(int3x3 val)
		{
			int3 @int = int3.op_Decrement(val.c0);
			val.c0 = @int;
			int3 int2 = @int;
			@int = int3.op_Decrement(val.c1);
			val.c1 = @int;
			int3 int3 = @int;
			@int = int3.op_Decrement(val.c2);
			val.c2 = @int;
			return new int3x3(int2, int3, @int);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <(int3x3 lhs, int3x3 rhs)
		{
			return new bool3x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <(int3x3 lhs, int rhs)
		{
			return new bool3x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <(int lhs, int3x3 rhs)
		{
			return new bool3x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <=(int3x3 lhs, int3x3 rhs)
		{
			return new bool3x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <=(int3x3 lhs, int rhs)
		{
			return new bool3x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <=(int lhs, int3x3 rhs)
		{
			return new bool3x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >(int3x3 lhs, int3x3 rhs)
		{
			return new bool3x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >(int3x3 lhs, int rhs)
		{
			return new bool3x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >(int lhs, int3x3 rhs)
		{
			return new bool3x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >=(int3x3 lhs, int3x3 rhs)
		{
			return new bool3x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >=(int3x3 lhs, int rhs)
		{
			return new bool3x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >=(int lhs, int3x3 rhs)
		{
			return new bool3x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator -(int3x3 val)
		{
			return new int3x3(-val.c0, -val.c1, -val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator +(int3x3 val)
		{
			return new int3x3(+val.c0, +val.c1, +val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator <<(int3x3 x, int n)
		{
			return new int3x3(x.c0 << n, x.c1 << n, x.c2 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator >>(int3x3 x, int n)
		{
			return new int3x3(x.c0 >> n, x.c1 >> n, x.c2 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator ==(int3x3 lhs, int3x3 rhs)
		{
			return new bool3x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator ==(int3x3 lhs, int rhs)
		{
			return new bool3x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator ==(int lhs, int3x3 rhs)
		{
			return new bool3x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator !=(int3x3 lhs, int3x3 rhs)
		{
			return new bool3x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator !=(int3x3 lhs, int rhs)
		{
			return new bool3x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator !=(int lhs, int3x3 rhs)
		{
			return new bool3x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator ~(int3x3 val)
		{
			return new int3x3(~val.c0, ~val.c1, ~val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator &(int3x3 lhs, int3x3 rhs)
		{
			return new int3x3(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1, lhs.c2 & rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator &(int3x3 lhs, int rhs)
		{
			return new int3x3(lhs.c0 & rhs, lhs.c1 & rhs, lhs.c2 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator &(int lhs, int3x3 rhs)
		{
			return new int3x3(lhs & rhs.c0, lhs & rhs.c1, lhs & rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator |(int3x3 lhs, int3x3 rhs)
		{
			return new int3x3(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1, lhs.c2 | rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator |(int3x3 lhs, int rhs)
		{
			return new int3x3(lhs.c0 | rhs, lhs.c1 | rhs, lhs.c2 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator |(int lhs, int3x3 rhs)
		{
			return new int3x3(lhs | rhs.c0, lhs | rhs.c1, lhs | rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator ^(int3x3 lhs, int3x3 rhs)
		{
			return new int3x3(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1, lhs.c2 ^ rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator ^(int3x3 lhs, int rhs)
		{
			return new int3x3(lhs.c0 ^ rhs, lhs.c1 ^ rhs, lhs.c2 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 operator ^(int lhs, int3x3 rhs)
		{
			return new int3x3(lhs ^ rhs.c0, lhs ^ rhs.c1, lhs ^ rhs.c2);
		}

		public unsafe ref int3 this[int index]
		{
			get
			{
				fixed (int3x3* ptr = &this)
				{
					return ref *(int3*)(ptr + (IntPtr)index * (IntPtr)sizeof(int3) / (IntPtr)sizeof(int3x3));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(int3x3 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2);
		}

		public override bool Equals(object o)
		{
			if (o is int3x3)
			{
				int3x3 int3x = (int3x3)o;
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
			return string.Format("int3x3({0}, {1}, {2},  {3}, {4}, {5},  {6}, {7}, {8})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c0.y,
				this.c1.y,
				this.c2.y,
				this.c0.z,
				this.c1.z,
				this.c2.z
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("int3x3({0}, {1}, {2},  {3}, {4}, {5},  {6}, {7}, {8})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider),
				this.c2.z.ToString(format, formatProvider)
			});
		}

		public int3 c0;

		public int3 c1;

		public int3 c2;

		public static readonly int3x3 identity = new int3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

		public static readonly int3x3 zero;
	}
}
