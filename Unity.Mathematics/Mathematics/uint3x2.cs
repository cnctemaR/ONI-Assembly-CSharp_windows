using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint3x2 : IEquatable<uint3x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(uint3 c0, uint3 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(uint m00, uint m01, uint m10, uint m11, uint m20, uint m21)
		{
			this.c0 = new uint3(m00, m10, m20);
			this.c1 = new uint3(m01, m11, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(uint v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(bool v)
		{
			this.c0 = math.select(new uint3(0U), new uint3(1U), v);
			this.c1 = math.select(new uint3(0U), new uint3(1U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(bool3x2 v)
		{
			this.c0 = math.select(new uint3(0U), new uint3(1U), v.c0);
			this.c1 = math.select(new uint3(0U), new uint3(1U), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(int v)
		{
			this.c0 = (uint3)v;
			this.c1 = (uint3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(int3x2 v)
		{
			this.c0 = (uint3)v.c0;
			this.c1 = (uint3)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(float v)
		{
			this.c0 = (uint3)v;
			this.c1 = (uint3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(float3x2 v)
		{
			this.c0 = (uint3)v.c0;
			this.c1 = (uint3)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(double v)
		{
			this.c0 = (uint3)v;
			this.c1 = (uint3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3x2(double3x2 v)
		{
			this.c0 = (uint3)v.c0;
			this.c1 = (uint3)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint3x2(uint v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3x2(bool v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3x2(bool3x2 v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3x2(int v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3x2(int3x2 v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3x2(float v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3x2(float3x2 v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3x2(double v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3x2(double3x2 v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator *(uint3x2 lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator *(uint3x2 lhs, uint rhs)
		{
			return new uint3x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator *(uint lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator +(uint3x2 lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator +(uint3x2 lhs, uint rhs)
		{
			return new uint3x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator +(uint lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator -(uint3x2 lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator -(uint3x2 lhs, uint rhs)
		{
			return new uint3x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator -(uint lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator /(uint3x2 lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator /(uint3x2 lhs, uint rhs)
		{
			return new uint3x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator /(uint lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator %(uint3x2 lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator %(uint3x2 lhs, uint rhs)
		{
			return new uint3x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator %(uint lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator ++(uint3x2 val)
		{
			uint3 @uint = uint3.op_Increment(val.c0);
			val.c0 = @uint;
			uint3 uint2 = @uint;
			@uint = uint3.op_Increment(val.c1);
			val.c1 = @uint;
			return new uint3x2(uint2, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator --(uint3x2 val)
		{
			uint3 @uint = uint3.op_Decrement(val.c0);
			val.c0 = @uint;
			uint3 uint2 = @uint;
			@uint = uint3.op_Decrement(val.c1);
			val.c1 = @uint;
			return new uint3x2(uint2, @uint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(uint3x2 lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(uint3x2 lhs, uint rhs)
		{
			return new bool3x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(uint lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(uint3x2 lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(uint3x2 lhs, uint rhs)
		{
			return new bool3x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(uint lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(uint3x2 lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(uint3x2 lhs, uint rhs)
		{
			return new bool3x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(uint lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(uint3x2 lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(uint3x2 lhs, uint rhs)
		{
			return new bool3x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(uint lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator -(uint3x2 val)
		{
			return new uint3x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator +(uint3x2 val)
		{
			return new uint3x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator <<(uint3x2 x, int n)
		{
			return new uint3x2(x.c0 << n, x.c1 << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator >>(uint3x2 x, int n)
		{
			return new uint3x2(x.c0 >> n, x.c1 >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(uint3x2 lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(uint3x2 lhs, uint rhs)
		{
			return new bool3x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(uint lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(uint3x2 lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(uint3x2 lhs, uint rhs)
		{
			return new bool3x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(uint lhs, uint3x2 rhs)
		{
			return new bool3x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator ~(uint3x2 val)
		{
			return new uint3x2(~val.c0, ~val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator &(uint3x2 lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator &(uint3x2 lhs, uint rhs)
		{
			return new uint3x2(lhs.c0 & rhs, lhs.c1 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator &(uint lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs & rhs.c0, lhs & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator |(uint3x2 lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator |(uint3x2 lhs, uint rhs)
		{
			return new uint3x2(lhs.c0 | rhs, lhs.c1 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator |(uint lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs | rhs.c0, lhs | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator ^(uint3x2 lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator ^(uint3x2 lhs, uint rhs)
		{
			return new uint3x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 operator ^(uint lhs, uint3x2 rhs)
		{
			return new uint3x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
		}

		public unsafe ref uint3 this[int index]
		{
			get
			{
				fixed (uint3x2* ptr = &this)
				{
					return ref *(uint3*)(ptr + (IntPtr)index * (IntPtr)sizeof(uint3) / (IntPtr)sizeof(uint3x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(uint3x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is uint3x2)
			{
				uint3x2 uint3x = (uint3x2)o;
				return this.Equals(uint3x);
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
			return string.Format("uint3x2({0}, {1},  {2}, {3},  {4}, {5})", new object[]
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
			return string.Format("uint3x2({0}, {1},  {2}, {3},  {4}, {5})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider)
			});
		}

		public uint3 c0;

		public uint3 c1;

		public static readonly uint3x2 zero;
	}
}
