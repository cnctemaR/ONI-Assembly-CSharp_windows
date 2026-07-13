using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint4x2 : IEquatable<uint4x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(uint4 c0, uint4 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(uint m00, uint m01, uint m10, uint m11, uint m20, uint m21, uint m30, uint m31)
		{
			this.c0 = new uint4(m00, m10, m20, m30);
			this.c1 = new uint4(m01, m11, m21, m31);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(uint v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(bool v)
		{
			this.c0 = math.select(new uint4(0U), new uint4(1U), v);
			this.c1 = math.select(new uint4(0U), new uint4(1U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(bool4x2 v)
		{
			this.c0 = math.select(new uint4(0U), new uint4(1U), v.c0);
			this.c1 = math.select(new uint4(0U), new uint4(1U), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(int v)
		{
			this.c0 = (uint4)v;
			this.c1 = (uint4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(int4x2 v)
		{
			this.c0 = (uint4)v.c0;
			this.c1 = (uint4)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(float v)
		{
			this.c0 = (uint4)v;
			this.c1 = (uint4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(float4x2 v)
		{
			this.c0 = (uint4)v.c0;
			this.c1 = (uint4)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(double v)
		{
			this.c0 = (uint4)v;
			this.c1 = (uint4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4x2(double4x2 v)
		{
			this.c0 = (uint4)v.c0;
			this.c1 = (uint4)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint4x2(uint v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x2(bool v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x2(bool4x2 v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x2(int v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x2(int4x2 v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x2(float v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x2(float4x2 v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x2(double v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4x2(double4x2 v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator *(uint4x2 lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator *(uint4x2 lhs, uint rhs)
		{
			return new uint4x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator *(uint lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator +(uint4x2 lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator +(uint4x2 lhs, uint rhs)
		{
			return new uint4x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator +(uint lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator -(uint4x2 lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator -(uint4x2 lhs, uint rhs)
		{
			return new uint4x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator -(uint lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator /(uint4x2 lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator /(uint4x2 lhs, uint rhs)
		{
			return new uint4x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator /(uint lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator %(uint4x2 lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator %(uint4x2 lhs, uint rhs)
		{
			return new uint4x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator %(uint lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator ++(uint4x2 val)
		{
			uint4 @uint = uint4.op_Increment(val.c0);
			val.c0 = @uint;
			uint4 uint2 = @uint;
			@uint = uint4.op_Increment(val.c1);
			val.c1 = @uint;
			return new uint4x2(uint2, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator --(uint4x2 val)
		{
			uint4 @uint = uint4.op_Decrement(val.c0);
			val.c0 = @uint;
			uint4 uint2 = @uint;
			@uint = uint4.op_Decrement(val.c1);
			val.c1 = @uint;
			return new uint4x2(uint2, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <(uint4x2 lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <(uint4x2 lhs, uint rhs)
		{
			return new bool4x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <(uint lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <=(uint4x2 lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <=(uint4x2 lhs, uint rhs)
		{
			return new bool4x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <=(uint lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >(uint4x2 lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >(uint4x2 lhs, uint rhs)
		{
			return new bool4x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >(uint lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >=(uint4x2 lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >=(uint4x2 lhs, uint rhs)
		{
			return new bool4x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >=(uint lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator -(uint4x2 val)
		{
			return new uint4x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator +(uint4x2 val)
		{
			return new uint4x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator <<(uint4x2 x, int n)
		{
			return new uint4x2(x.c0 << n, x.c1 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator >>(uint4x2 x, int n)
		{
			return new uint4x2(x.c0 >> n, x.c1 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(uint4x2 lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(uint4x2 lhs, uint rhs)
		{
			return new bool4x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(uint lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(uint4x2 lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(uint4x2 lhs, uint rhs)
		{
			return new bool4x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(uint lhs, uint4x2 rhs)
		{
			return new bool4x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator ~(uint4x2 val)
		{
			return new uint4x2(~val.c0, ~val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator &(uint4x2 lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator &(uint4x2 lhs, uint rhs)
		{
			return new uint4x2(lhs.c0 & rhs, lhs.c1 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator &(uint lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs & rhs.c0, lhs & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator |(uint4x2 lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator |(uint4x2 lhs, uint rhs)
		{
			return new uint4x2(lhs.c0 | rhs, lhs.c1 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator |(uint lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs | rhs.c0, lhs | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator ^(uint4x2 lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator ^(uint4x2 lhs, uint rhs)
		{
			return new uint4x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 operator ^(uint lhs, uint4x2 rhs)
		{
			return new uint4x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
		}

		public unsafe ref uint4 this[int index]
		{
			get
			{
				fixed (uint4x2* ptr = &this)
				{
					return ref *(uint4*)(ptr + (IntPtr)index * (IntPtr)sizeof(uint4) / (IntPtr)sizeof(uint4x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(uint4x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is uint4x2)
			{
				uint4x2 uint4x = (uint4x2)o;
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
			return string.Format("uint4x2({0}, {1},  {2}, {3},  {4}, {5},  {6}, {7})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y,
				this.c0.z,
				this.c1.z,
				this.c0.w,
				this.c1.w
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("uint4x2({0}, {1},  {2}, {3},  {4}, {5},  {6}, {7})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider),
				this.c0.w.ToString(format, formatProvider),
				this.c1.w.ToString(format, formatProvider)
			});
		}

		public uint4 c0;

		public uint4 c1;

		public static readonly uint4x2 zero;
	}
}
