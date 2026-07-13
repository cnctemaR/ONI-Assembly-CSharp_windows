using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(uint2.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint2 : IEquatable<uint2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(uint x, uint y)
		{
			this.x = x;
			this.y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(uint2 xy)
		{
			this.x = xy.x;
			this.y = xy.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(uint v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(bool v)
		{
			this.x = (v ? 1U : 0U);
			this.y = (v ? 1U : 0U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(bool2 v)
		{
			this.x = (v.x ? 1U : 0U);
			this.y = (v.y ? 1U : 0U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(int v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(int2 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(float v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(float2 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(double v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2(double2 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint2(uint v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2(bool v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2(bool2 v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2(int v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2(int2 v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2(float v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2(float2 v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2(double v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint2(double2 v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator *(uint2 lhs, uint2 rhs)
		{
			return new uint2(lhs.x * rhs.x, lhs.y * rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator *(uint2 lhs, uint rhs)
		{
			return new uint2(lhs.x * rhs, lhs.y * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator *(uint lhs, uint2 rhs)
		{
			return new uint2(lhs * rhs.x, lhs * rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator +(uint2 lhs, uint2 rhs)
		{
			return new uint2(lhs.x + rhs.x, lhs.y + rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator +(uint2 lhs, uint rhs)
		{
			return new uint2(lhs.x + rhs, lhs.y + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator +(uint lhs, uint2 rhs)
		{
			return new uint2(lhs + rhs.x, lhs + rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator -(uint2 lhs, uint2 rhs)
		{
			return new uint2(lhs.x - rhs.x, lhs.y - rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator -(uint2 lhs, uint rhs)
		{
			return new uint2(lhs.x - rhs, lhs.y - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator -(uint lhs, uint2 rhs)
		{
			return new uint2(lhs - rhs.x, lhs - rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator /(uint2 lhs, uint2 rhs)
		{
			return new uint2(lhs.x / rhs.x, lhs.y / rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator /(uint2 lhs, uint rhs)
		{
			return new uint2(lhs.x / rhs, lhs.y / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator /(uint lhs, uint2 rhs)
		{
			return new uint2(lhs / rhs.x, lhs / rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator %(uint2 lhs, uint2 rhs)
		{
			return new uint2(lhs.x % rhs.x, lhs.y % rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator %(uint2 lhs, uint rhs)
		{
			return new uint2(lhs.x % rhs, lhs.y % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator %(uint lhs, uint2 rhs)
		{
			return new uint2(lhs % rhs.x, lhs % rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator ++(uint2 val)
		{
			uint num = val.x + 1U;
			val.x = num;
			uint num2 = num;
			num = val.y + 1U;
			val.y = num;
			return new uint2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator --(uint2 val)
		{
			uint num = val.x - 1U;
			val.x = num;
			uint num2 = num;
			num = val.y - 1U;
			val.y = num;
			return new uint2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(uint2 lhs, uint2 rhs)
		{
			return new bool2(lhs.x < rhs.x, lhs.y < rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(uint2 lhs, uint rhs)
		{
			return new bool2(lhs.x < rhs, lhs.y < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(uint lhs, uint2 rhs)
		{
			return new bool2(lhs < rhs.x, lhs < rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(uint2 lhs, uint2 rhs)
		{
			return new bool2(lhs.x <= rhs.x, lhs.y <= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(uint2 lhs, uint rhs)
		{
			return new bool2(lhs.x <= rhs, lhs.y <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(uint lhs, uint2 rhs)
		{
			return new bool2(lhs <= rhs.x, lhs <= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(uint2 lhs, uint2 rhs)
		{
			return new bool2(lhs.x > rhs.x, lhs.y > rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(uint2 lhs, uint rhs)
		{
			return new bool2(lhs.x > rhs, lhs.y > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(uint lhs, uint2 rhs)
		{
			return new bool2(lhs > rhs.x, lhs > rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(uint2 lhs, uint2 rhs)
		{
			return new bool2(lhs.x >= rhs.x, lhs.y >= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(uint2 lhs, uint rhs)
		{
			return new bool2(lhs.x >= rhs, lhs.y >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(uint lhs, uint2 rhs)
		{
			return new bool2(lhs >= rhs.x, lhs >= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator -(uint2 val)
		{
			return new uint2((uint)(-(uint)((ulong)val.x)), (uint)(-(uint)((ulong)val.y)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator +(uint2 val)
		{
			return new uint2(val.x, val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator <<(uint2 x, int n)
		{
			return new uint2(x.x << n, x.y << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator >>(uint2 x, int n)
		{
			return new uint2(x.x >> n, x.y >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(uint2 lhs, uint2 rhs)
		{
			return new bool2(lhs.x == rhs.x, lhs.y == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(uint2 lhs, uint rhs)
		{
			return new bool2(lhs.x == rhs, lhs.y == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(uint lhs, uint2 rhs)
		{
			return new bool2(lhs == rhs.x, lhs == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(uint2 lhs, uint2 rhs)
		{
			return new bool2(lhs.x != rhs.x, lhs.y != rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(uint2 lhs, uint rhs)
		{
			return new bool2(lhs.x != rhs, lhs.y != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(uint lhs, uint2 rhs)
		{
			return new bool2(lhs != rhs.x, lhs != rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator ~(uint2 val)
		{
			return new uint2(~val.x, ~val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator &(uint2 lhs, uint2 rhs)
		{
			return new uint2(lhs.x & rhs.x, lhs.y & rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator &(uint2 lhs, uint rhs)
		{
			return new uint2(lhs.x & rhs, lhs.y & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator &(uint lhs, uint2 rhs)
		{
			return new uint2(lhs & rhs.x, lhs & rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator |(uint2 lhs, uint2 rhs)
		{
			return new uint2(lhs.x | rhs.x, lhs.y | rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator |(uint2 lhs, uint rhs)
		{
			return new uint2(lhs.x | rhs, lhs.y | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator |(uint lhs, uint2 rhs)
		{
			return new uint2(lhs | rhs.x, lhs | rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator ^(uint2 lhs, uint2 rhs)
		{
			return new uint2(lhs.x ^ rhs.x, lhs.y ^ rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator ^(uint2 lhs, uint rhs)
		{
			return new uint2(lhs.x ^ rhs, lhs.y ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 operator ^(uint lhs, uint2 rhs)
		{
			return new uint2(lhs ^ rhs.x, lhs ^ rhs.y);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 xx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 xy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 yx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 yy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.y, this.y);
			}
		}

		public unsafe uint this[int index]
		{
			get
			{
				fixed (uint2* ptr = &this)
				{
					return ((uint*)ptr)[index];
				}
			}
			set
			{
				fixed (uint* ptr = &this.x)
				{
					ptr[index] = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(uint2 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y;
		}

		public override bool Equals(object o)
		{
			if (o is uint2)
			{
				uint2 @uint = (uint2)o;
				return this.Equals(@uint);
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
			return string.Format("uint2({0}, {1})", this.x, this.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("uint2({0}, {1})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider));
		}

		public uint x;

		public uint y;

		public static readonly uint2 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(uint2 v)
			{
				this.x = v.x;
				this.y = v.y;
			}

			public uint x;

			public uint y;
		}
	}
}
