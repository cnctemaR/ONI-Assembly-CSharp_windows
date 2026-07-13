using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint2x3 : IEquatable<uint2x3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(uint2 c0, uint2 c1, uint2 c2)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(uint m00, uint m01, uint m02, uint m10, uint m11, uint m12)
		{
			this.c0 = new uint2(m00, m10);
			this.c1 = new uint2(m01, m11);
			this.c2 = new uint2(m02, m12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(bool v)
		{
			this.c0 = math.select(new uint2(0U), new uint2(1U), v);
			this.c1 = math.select(new uint2(0U), new uint2(1U), v);
			this.c2 = math.select(new uint2(0U), new uint2(1U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(bool2x3 v)
		{
			this.c0 = math.select(new uint2(0U), new uint2(1U), v.c0);
			this.c1 = math.select(new uint2(0U), new uint2(1U), v.c1);
			this.c2 = math.select(new uint2(0U), new uint2(1U), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(int v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
			this.c2 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(int2x3 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
			this.c2 = (uint2)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(float v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
			this.c2 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(float2x3 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
			this.c2 = (uint2)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(double v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
			this.c2 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x3(double2x3 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
			this.c2 = (uint2)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint2x3(uint v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x3(bool v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x3(bool2x3 v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x3(int v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x3(int2x3 v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x3(float v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x3(float2x3 v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x3(double v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x3(double2x3 v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator *(uint2x3 lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator *(uint2x3 lhs, uint rhs)
		{
			return new uint2x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator *(uint lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator +(uint2x3 lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator +(uint2x3 lhs, uint rhs)
		{
			return new uint2x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator +(uint lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator -(uint2x3 lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator -(uint2x3 lhs, uint rhs)
		{
			return new uint2x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator -(uint lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator /(uint2x3 lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator /(uint2x3 lhs, uint rhs)
		{
			return new uint2x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator /(uint lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator %(uint2x3 lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator %(uint2x3 lhs, uint rhs)
		{
			return new uint2x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator %(uint lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator ++(uint2x3 val)
		{
			uint2 @uint = uint2.op_Increment(val.c0);
			val.c0 = @uint;
			uint2 uint2 = @uint;
			@uint = uint2.op_Increment(val.c1);
			val.c1 = @uint;
			uint2 uint3 = @uint;
			@uint = uint2.op_Increment(val.c2);
			val.c2 = @uint;
			return new uint2x3(uint2, uint3, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator --(uint2x3 val)
		{
			uint2 @uint = uint2.op_Decrement(val.c0);
			val.c0 = @uint;
			uint2 uint2 = @uint;
			@uint = uint2.op_Decrement(val.c1);
			val.c1 = @uint;
			uint2 uint3 = @uint;
			@uint = uint2.op_Decrement(val.c2);
			val.c2 = @uint;
			return new uint2x3(uint2, uint3, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(uint2x3 lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(uint2x3 lhs, uint rhs)
		{
			return new bool2x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(uint lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(uint2x3 lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(uint2x3 lhs, uint rhs)
		{
			return new bool2x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(uint lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(uint2x3 lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(uint2x3 lhs, uint rhs)
		{
			return new bool2x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(uint lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(uint2x3 lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(uint2x3 lhs, uint rhs)
		{
			return new bool2x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(uint lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator -(uint2x3 val)
		{
			return new uint2x3(-val.c0, -val.c1, -val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator +(uint2x3 val)
		{
			return new uint2x3(+val.c0, +val.c1, +val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator <<(uint2x3 x, int n)
		{
			return new uint2x3(x.c0 << n, x.c1 << n, x.c2 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator >>(uint2x3 x, int n)
		{
			return new uint2x3(x.c0 >> n, x.c1 >> n, x.c2 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(uint2x3 lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(uint2x3 lhs, uint rhs)
		{
			return new bool2x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(uint lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(uint2x3 lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(uint2x3 lhs, uint rhs)
		{
			return new bool2x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(uint lhs, uint2x3 rhs)
		{
			return new bool2x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator ~(uint2x3 val)
		{
			return new uint2x3(~val.c0, ~val.c1, ~val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator &(uint2x3 lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1, lhs.c2 & rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator &(uint2x3 lhs, uint rhs)
		{
			return new uint2x3(lhs.c0 & rhs, lhs.c1 & rhs, lhs.c2 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator &(uint lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs & rhs.c0, lhs & rhs.c1, lhs & rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator |(uint2x3 lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1, lhs.c2 | rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator |(uint2x3 lhs, uint rhs)
		{
			return new uint2x3(lhs.c0 | rhs, lhs.c1 | rhs, lhs.c2 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator |(uint lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs | rhs.c0, lhs | rhs.c1, lhs | rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator ^(uint2x3 lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1, lhs.c2 ^ rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator ^(uint2x3 lhs, uint rhs)
		{
			return new uint2x3(lhs.c0 ^ rhs, lhs.c1 ^ rhs, lhs.c2 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 operator ^(uint lhs, uint2x3 rhs)
		{
			return new uint2x3(lhs ^ rhs.c0, lhs ^ rhs.c1, lhs ^ rhs.c2);
		}

		public unsafe ref uint2 this[int index]
		{
			get
			{
				fixed (uint2x3* ptr = &this)
				{
					return ref *(uint2*)(ptr + (IntPtr)index * (IntPtr)sizeof(uint2) / (IntPtr)sizeof(uint2x3));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(uint2x3 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2);
		}

		public override bool Equals(object o)
		{
			if (o is uint2x3)
			{
				uint2x3 uint2x = (uint2x3)o;
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
			return string.Format("uint2x3({0}, {1}, {2},  {3}, {4}, {5})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c0.y,
				this.c1.y,
				this.c2.y
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("uint2x3({0}, {1}, {2},  {3}, {4}, {5})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider)
			});
		}

		public uint2 c0;

		public uint2 c1;

		public uint2 c2;

		public static readonly uint2x3 zero;
	}
}
