using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint2x4 : IEquatable<uint2x4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(uint2 c0, uint2 c1, uint2 c2, uint2 c3)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
			this.c3 = c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(uint m00, uint m01, uint m02, uint m03, uint m10, uint m11, uint m12, uint m13)
		{
			this.c0 = new uint2(m00, m10);
			this.c1 = new uint2(m01, m11);
			this.c2 = new uint2(m02, m12);
			this.c3 = new uint2(m03, m13);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(bool v)
		{
			this.c0 = math.select(new uint2(0U), new uint2(1U), v);
			this.c1 = math.select(new uint2(0U), new uint2(1U), v);
			this.c2 = math.select(new uint2(0U), new uint2(1U), v);
			this.c3 = math.select(new uint2(0U), new uint2(1U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(bool2x4 v)
		{
			this.c0 = math.select(new uint2(0U), new uint2(1U), v.c0);
			this.c1 = math.select(new uint2(0U), new uint2(1U), v.c1);
			this.c2 = math.select(new uint2(0U), new uint2(1U), v.c2);
			this.c3 = math.select(new uint2(0U), new uint2(1U), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(int v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
			this.c2 = (uint2)v;
			this.c3 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(int2x4 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
			this.c2 = (uint2)v.c2;
			this.c3 = (uint2)v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(float v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
			this.c2 = (uint2)v;
			this.c3 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(float2x4 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
			this.c2 = (uint2)v.c2;
			this.c3 = (uint2)v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(double v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
			this.c2 = (uint2)v;
			this.c3 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x4(double2x4 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
			this.c2 = (uint2)v.c2;
			this.c3 = (uint2)v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint2x4(uint v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x4(bool v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x4(bool2x4 v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x4(int v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x4(int2x4 v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x4(float v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x4(float2x4 v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x4(double v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x4(double2x4 v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator *(uint2x4 lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2, lhs.c3 * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator *(uint2x4 lhs, uint rhs)
		{
			return new uint2x4(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs, lhs.c3 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator *(uint lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2, lhs * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator +(uint2x4 lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2, lhs.c3 + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator +(uint2x4 lhs, uint rhs)
		{
			return new uint2x4(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs, lhs.c3 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator +(uint lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2, lhs + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator -(uint2x4 lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2, lhs.c3 - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator -(uint2x4 lhs, uint rhs)
		{
			return new uint2x4(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs, lhs.c3 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator -(uint lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2, lhs - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator /(uint2x4 lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2, lhs.c3 / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator /(uint2x4 lhs, uint rhs)
		{
			return new uint2x4(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs, lhs.c3 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator /(uint lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2, lhs / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator %(uint2x4 lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2, lhs.c3 % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator %(uint2x4 lhs, uint rhs)
		{
			return new uint2x4(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs, lhs.c3 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator %(uint lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2, lhs % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator ++(uint2x4 val)
		{
			uint2 @uint = uint2.op_Increment(val.c0);
			val.c0 = @uint;
			uint2 uint2 = @uint;
			@uint = uint2.op_Increment(val.c1);
			val.c1 = @uint;
			uint2 uint3 = @uint;
			@uint = uint2.op_Increment(val.c2);
			val.c2 = @uint;
			uint2 uint4 = @uint;
			@uint = uint2.op_Increment(val.c3);
			val.c3 = @uint;
			return new uint2x4(uint2, uint3, uint4, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator --(uint2x4 val)
		{
			uint2 @uint = uint2.op_Decrement(val.c0);
			val.c0 = @uint;
			uint2 uint2 = @uint;
			@uint = uint2.op_Decrement(val.c1);
			val.c1 = @uint;
			uint2 uint3 = @uint;
			@uint = uint2.op_Decrement(val.c2);
			val.c2 = @uint;
			uint2 uint4 = @uint;
			@uint = uint2.op_Decrement(val.c3);
			val.c3 = @uint;
			return new uint2x4(uint2, uint3, uint4, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <(uint2x4 lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2, lhs.c3 < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <(uint2x4 lhs, uint rhs)
		{
			return new bool2x4(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs, lhs.c3 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <(uint lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2, lhs < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <=(uint2x4 lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2, lhs.c3 <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <=(uint2x4 lhs, uint rhs)
		{
			return new bool2x4(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs, lhs.c3 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <=(uint lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2, lhs <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >(uint2x4 lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2, lhs.c3 > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >(uint2x4 lhs, uint rhs)
		{
			return new bool2x4(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs, lhs.c3 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >(uint lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2, lhs > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >=(uint2x4 lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2, lhs.c3 >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >=(uint2x4 lhs, uint rhs)
		{
			return new bool2x4(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs, lhs.c3 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >=(uint lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2, lhs >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator -(uint2x4 val)
		{
			return new uint2x4(-val.c0, -val.c1, -val.c2, -val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator +(uint2x4 val)
		{
			return new uint2x4(+val.c0, +val.c1, +val.c2, +val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator <<(uint2x4 x, int n)
		{
			return new uint2x4(x.c0 << n, x.c1 << n, x.c2 << n, x.c3 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator >>(uint2x4 x, int n)
		{
			return new uint2x4(x.c0 >> n, x.c1 >> n, x.c2 >> n, x.c3 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator ==(uint2x4 lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator ==(uint2x4 lhs, uint rhs)
		{
			return new bool2x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator ==(uint lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator !=(uint2x4 lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator !=(uint2x4 lhs, uint rhs)
		{
			return new bool2x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator !=(uint lhs, uint2x4 rhs)
		{
			return new bool2x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator ~(uint2x4 val)
		{
			return new uint2x4(~val.c0, ~val.c1, ~val.c2, ~val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator &(uint2x4 lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1, lhs.c2 & rhs.c2, lhs.c3 & rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator &(uint2x4 lhs, uint rhs)
		{
			return new uint2x4(lhs.c0 & rhs, lhs.c1 & rhs, lhs.c2 & rhs, lhs.c3 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator &(uint lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs & rhs.c0, lhs & rhs.c1, lhs & rhs.c2, lhs & rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator |(uint2x4 lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1, lhs.c2 | rhs.c2, lhs.c3 | rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator |(uint2x4 lhs, uint rhs)
		{
			return new uint2x4(lhs.c0 | rhs, lhs.c1 | rhs, lhs.c2 | rhs, lhs.c3 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator |(uint lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs | rhs.c0, lhs | rhs.c1, lhs | rhs.c2, lhs | rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator ^(uint2x4 lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1, lhs.c2 ^ rhs.c2, lhs.c3 ^ rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator ^(uint2x4 lhs, uint rhs)
		{
			return new uint2x4(lhs.c0 ^ rhs, lhs.c1 ^ rhs, lhs.c2 ^ rhs, lhs.c3 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 operator ^(uint lhs, uint2x4 rhs)
		{
			return new uint2x4(lhs ^ rhs.c0, lhs ^ rhs.c1, lhs ^ rhs.c2, lhs ^ rhs.c3);
		}

		public unsafe ref uint2 this[int index]
		{
			get
			{
				fixed (uint2x4* ptr = &this)
				{
					return ref *(uint2*)(ptr + (IntPtr)index * (IntPtr)sizeof(uint2) / (IntPtr)sizeof(uint2x4));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(uint2x4 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2) && this.c3.Equals(rhs.c3);
		}

		public override bool Equals(object o)
		{
			if (o is uint2x4)
			{
				uint2x4 uint2x = (uint2x4)o;
				return this.Equals(uint2x);
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
			return string.Format("uint2x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c3.x,
				this.c0.y,
				this.c1.y,
				this.c2.y,
				this.c3.y
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("uint2x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c3.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider),
				this.c3.y.ToString(format, formatProvider)
			});
		}

		public uint2 c0;

		public uint2 c1;

		public uint2 c2;

		public uint2 c3;

		public static readonly uint2x4 zero;
	}
}
