using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint2x2 : IEquatable<uint2x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(uint2 c0, uint2 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(uint m00, uint m01, uint m10, uint m11)
		{
			this.c0 = new uint2(m00, m10);
			this.c1 = new uint2(m01, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(uint v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(bool v)
		{
			this.c0 = math.select(new uint2(0U), new uint2(1U), v);
			this.c1 = math.select(new uint2(0U), new uint2(1U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(bool2x2 v)
		{
			this.c0 = math.select(new uint2(0U), new uint2(1U), v.c0);
			this.c1 = math.select(new uint2(0U), new uint2(1U), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(int v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(int2x2 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(float v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(float2x2 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(double v)
		{
			this.c0 = (uint2)v;
			this.c1 = (uint2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2x2(double2x2 v)
		{
			this.c0 = (uint2)v.c0;
			this.c1 = (uint2)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint2x2(uint v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x2(bool v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x2(bool2x2 v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x2(int v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x2(int2x2 v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x2(float v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x2(float2x2 v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x2(double v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2x2(double2x2 v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator *(uint2x2 lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator *(uint2x2 lhs, uint rhs)
		{
			return new uint2x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator *(uint lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator +(uint2x2 lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator +(uint2x2 lhs, uint rhs)
		{
			return new uint2x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator +(uint lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator -(uint2x2 lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator -(uint2x2 lhs, uint rhs)
		{
			return new uint2x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator -(uint lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator /(uint2x2 lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator /(uint2x2 lhs, uint rhs)
		{
			return new uint2x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator /(uint lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator %(uint2x2 lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator %(uint2x2 lhs, uint rhs)
		{
			return new uint2x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator %(uint lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator ++(uint2x2 val)
		{
			uint2 @uint = uint2.op_Increment(val.c0);
			val.c0 = @uint;
			uint2 uint2 = @uint;
			@uint = uint2.op_Increment(val.c1);
			val.c1 = @uint;
			return new uint2x2(uint2, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator --(uint2x2 val)
		{
			uint2 @uint = uint2.op_Decrement(val.c0);
			val.c0 = @uint;
			uint2 uint2 = @uint;
			@uint = uint2.op_Decrement(val.c1);
			val.c1 = @uint;
			return new uint2x2(uint2, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(uint2x2 lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(uint2x2 lhs, uint rhs)
		{
			return new bool2x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(uint lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(uint2x2 lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(uint2x2 lhs, uint rhs)
		{
			return new bool2x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(uint lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(uint2x2 lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(uint2x2 lhs, uint rhs)
		{
			return new bool2x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(uint lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(uint2x2 lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(uint2x2 lhs, uint rhs)
		{
			return new bool2x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(uint lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator -(uint2x2 val)
		{
			return new uint2x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator +(uint2x2 val)
		{
			return new uint2x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator <<(uint2x2 x, int n)
		{
			return new uint2x2(x.c0 << n, x.c1 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator >>(uint2x2 x, int n)
		{
			return new uint2x2(x.c0 >> n, x.c1 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(uint2x2 lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(uint2x2 lhs, uint rhs)
		{
			return new bool2x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(uint lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(uint2x2 lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(uint2x2 lhs, uint rhs)
		{
			return new bool2x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(uint lhs, uint2x2 rhs)
		{
			return new bool2x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator ~(uint2x2 val)
		{
			return new uint2x2(~val.c0, ~val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator &(uint2x2 lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator &(uint2x2 lhs, uint rhs)
		{
			return new uint2x2(lhs.c0 & rhs, lhs.c1 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator &(uint lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs & rhs.c0, lhs & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator |(uint2x2 lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator |(uint2x2 lhs, uint rhs)
		{
			return new uint2x2(lhs.c0 | rhs, lhs.c1 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator |(uint lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs | rhs.c0, lhs | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator ^(uint2x2 lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator ^(uint2x2 lhs, uint rhs)
		{
			return new uint2x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 operator ^(uint lhs, uint2x2 rhs)
		{
			return new uint2x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
		}

		public unsafe ref uint2 this[int index]
		{
			get
			{
				fixed (uint2x2* ptr = &this)
				{
					return ref *(uint2*)(ptr + (IntPtr)index * (IntPtr)sizeof(uint2) / (IntPtr)sizeof(uint2x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(uint2x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is uint2x2)
			{
				uint2x2 uint2x = (uint2x2)o;
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
			return string.Format("uint2x2({0}, {1},  {2}, {3})", new object[]
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
			return string.Format("uint2x2({0}, {1},  {2}, {3})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider)
			});
		}

		public uint2 c0;

		public uint2 c1;

		public static readonly uint2x2 identity = new uint2x2(1U, 0U, 0U, 1U);

		public static readonly uint2x2 zero;
	}
}
