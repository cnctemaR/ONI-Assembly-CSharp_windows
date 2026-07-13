using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint4x4 : IEquatable<uint4x4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(uint4 c0, uint4 c1, uint4 c2, uint4 c3)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
			this.c3 = c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(uint m00, uint m01, uint m02, uint m03, uint m10, uint m11, uint m12, uint m13, uint m20, uint m21, uint m22, uint m23, uint m30, uint m31, uint m32, uint m33)
		{
			this.c0 = new uint4(m00, m10, m20, m30);
			this.c1 = new uint4(m01, m11, m21, m31);
			this.c2 = new uint4(m02, m12, m22, m32);
			this.c3 = new uint4(m03, m13, m23, m33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(bool v)
		{
			this.c0 = math.select(new uint4(0U), new uint4(1U), v);
			this.c1 = math.select(new uint4(0U), new uint4(1U), v);
			this.c2 = math.select(new uint4(0U), new uint4(1U), v);
			this.c3 = math.select(new uint4(0U), new uint4(1U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(bool4x4 v)
		{
			this.c0 = math.select(new uint4(0U), new uint4(1U), v.c0);
			this.c1 = math.select(new uint4(0U), new uint4(1U), v.c1);
			this.c2 = math.select(new uint4(0U), new uint4(1U), v.c2);
			this.c3 = math.select(new uint4(0U), new uint4(1U), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(int v)
		{
			this.c0 = (uint4)v;
			this.c1 = (uint4)v;
			this.c2 = (uint4)v;
			this.c3 = (uint4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(int4x4 v)
		{
			this.c0 = (uint4)v.c0;
			this.c1 = (uint4)v.c1;
			this.c2 = (uint4)v.c2;
			this.c3 = (uint4)v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(float v)
		{
			this.c0 = (uint4)v;
			this.c1 = (uint4)v;
			this.c2 = (uint4)v;
			this.c3 = (uint4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(float4x4 v)
		{
			this.c0 = (uint4)v.c0;
			this.c1 = (uint4)v.c1;
			this.c2 = (uint4)v.c2;
			this.c3 = (uint4)v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(double v)
		{
			this.c0 = (uint4)v;
			this.c1 = (uint4)v;
			this.c2 = (uint4)v;
			this.c3 = (uint4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x4(double4x4 v)
		{
			this.c0 = (uint4)v.c0;
			this.c1 = (uint4)v.c1;
			this.c2 = (uint4)v.c2;
			this.c3 = (uint4)v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint4x4(uint v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x4(bool v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x4(bool4x4 v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x4(int v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x4(int4x4 v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x4(float v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x4(float4x4 v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x4(double v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x4(double4x4 v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator *(uint4x4 lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2, lhs.c3 * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator *(uint4x4 lhs, uint rhs)
		{
			return new uint4x4(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs, lhs.c3 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator *(uint lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2, lhs * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator +(uint4x4 lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2, lhs.c3 + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator +(uint4x4 lhs, uint rhs)
		{
			return new uint4x4(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs, lhs.c3 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator +(uint lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2, lhs + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator -(uint4x4 lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2, lhs.c3 - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator -(uint4x4 lhs, uint rhs)
		{
			return new uint4x4(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs, lhs.c3 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator -(uint lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2, lhs - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator /(uint4x4 lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2, lhs.c3 / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator /(uint4x4 lhs, uint rhs)
		{
			return new uint4x4(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs, lhs.c3 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator /(uint lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2, lhs / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator %(uint4x4 lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2, lhs.c3 % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator %(uint4x4 lhs, uint rhs)
		{
			return new uint4x4(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs, lhs.c3 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator %(uint lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2, lhs % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator ++(uint4x4 val)
		{
			uint4 @uint = uint4.op_Increment(val.c0);
			val.c0 = @uint;
			uint4 uint2 = @uint;
			@uint = uint4.op_Increment(val.c1);
			val.c1 = @uint;
			uint4 uint3 = @uint;
			@uint = uint4.op_Increment(val.c2);
			val.c2 = @uint;
			uint4 uint4 = @uint;
			@uint = uint4.op_Increment(val.c3);
			val.c3 = @uint;
			return new uint4x4(uint2, uint3, uint4, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator --(uint4x4 val)
		{
			uint4 @uint = uint4.op_Decrement(val.c0);
			val.c0 = @uint;
			uint4 uint2 = @uint;
			@uint = uint4.op_Decrement(val.c1);
			val.c1 = @uint;
			uint4 uint3 = @uint;
			@uint = uint4.op_Decrement(val.c2);
			val.c2 = @uint;
			uint4 uint4 = @uint;
			@uint = uint4.op_Decrement(val.c3);
			val.c3 = @uint;
			return new uint4x4(uint2, uint3, uint4, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <(uint4x4 lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2, lhs.c3 < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <(uint4x4 lhs, uint rhs)
		{
			return new bool4x4(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs, lhs.c3 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <(uint lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2, lhs < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <=(uint4x4 lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2, lhs.c3 <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <=(uint4x4 lhs, uint rhs)
		{
			return new bool4x4(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs, lhs.c3 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <=(uint lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2, lhs <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >(uint4x4 lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2, lhs.c3 > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >(uint4x4 lhs, uint rhs)
		{
			return new bool4x4(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs, lhs.c3 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >(uint lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2, lhs > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >=(uint4x4 lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2, lhs.c3 >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >=(uint4x4 lhs, uint rhs)
		{
			return new bool4x4(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs, lhs.c3 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >=(uint lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2, lhs >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator -(uint4x4 val)
		{
			return new uint4x4(-val.c0, -val.c1, -val.c2, -val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator +(uint4x4 val)
		{
			return new uint4x4(+val.c0, +val.c1, +val.c2, +val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator <<(uint4x4 x, int n)
		{
			return new uint4x4(x.c0 << n, x.c1 << n, x.c2 << n, x.c3 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator >>(uint4x4 x, int n)
		{
			return new uint4x4(x.c0 >> n, x.c1 >> n, x.c2 >> n, x.c3 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator ==(uint4x4 lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator ==(uint4x4 lhs, uint rhs)
		{
			return new bool4x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator ==(uint lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator !=(uint4x4 lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator !=(uint4x4 lhs, uint rhs)
		{
			return new bool4x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator !=(uint lhs, uint4x4 rhs)
		{
			return new bool4x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator ~(uint4x4 val)
		{
			return new uint4x4(~val.c0, ~val.c1, ~val.c2, ~val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator &(uint4x4 lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1, lhs.c2 & rhs.c2, lhs.c3 & rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator &(uint4x4 lhs, uint rhs)
		{
			return new uint4x4(lhs.c0 & rhs, lhs.c1 & rhs, lhs.c2 & rhs, lhs.c3 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator &(uint lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs & rhs.c0, lhs & rhs.c1, lhs & rhs.c2, lhs & rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator |(uint4x4 lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1, lhs.c2 | rhs.c2, lhs.c3 | rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator |(uint4x4 lhs, uint rhs)
		{
			return new uint4x4(lhs.c0 | rhs, lhs.c1 | rhs, lhs.c2 | rhs, lhs.c3 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator |(uint lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs | rhs.c0, lhs | rhs.c1, lhs | rhs.c2, lhs | rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator ^(uint4x4 lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1, lhs.c2 ^ rhs.c2, lhs.c3 ^ rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator ^(uint4x4 lhs, uint rhs)
		{
			return new uint4x4(lhs.c0 ^ rhs, lhs.c1 ^ rhs, lhs.c2 ^ rhs, lhs.c3 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 operator ^(uint lhs, uint4x4 rhs)
		{
			return new uint4x4(lhs ^ rhs.c0, lhs ^ rhs.c1, lhs ^ rhs.c2, lhs ^ rhs.c3);
		}

		public unsafe ref uint4 this[int index]
		{
			get
			{
				fixed (uint4x4* ptr = &this)
				{
					return ref *(uint4*)(ptr + (IntPtr)index * (IntPtr)sizeof(uint4) / (IntPtr)sizeof(uint4x4));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(uint4x4 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2) && this.c3.Equals(rhs.c3);
		}

		public override bool Equals(object o)
		{
			if (o is uint4x4)
			{
				uint4x4 uint4x = (uint4x4)o;
				return this.Equals(uint4x);
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
			return string.Format("uint4x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7},  {8}, {9}, {10}, {11},  {12}, {13}, {14}, {15})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c3.x,
				this.c0.y,
				this.c1.y,
				this.c2.y,
				this.c3.y,
				this.c0.z,
				this.c1.z,
				this.c2.z,
				this.c3.z,
				this.c0.w,
				this.c1.w,
				this.c2.w,
				this.c3.w
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("uint4x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7},  {8}, {9}, {10}, {11},  {12}, {13}, {14}, {15})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c3.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider),
				this.c3.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider),
				this.c2.z.ToString(format, formatProvider),
				this.c3.z.ToString(format, formatProvider),
				this.c0.w.ToString(format, formatProvider),
				this.c1.w.ToString(format, formatProvider),
				this.c2.w.ToString(format, formatProvider),
				this.c3.w.ToString(format, formatProvider)
			});
		}

		public uint4 c0;

		public uint4 c1;

		public uint4 c2;

		public uint4 c3;

		public static readonly uint4x4 identity = new uint4x4(1U, 0U, 0U, 0U, 0U, 1U, 0U, 0U, 0U, 0U, 1U, 0U, 0U, 0U, 0U, 1U);

		public static readonly uint4x4 zero;
	}
}
