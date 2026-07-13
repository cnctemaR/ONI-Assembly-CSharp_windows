using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(uint4.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint4 : IEquatable<uint4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint x, uint y, uint z, uint w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint x, uint y, uint2 zw)
		{
			this.x = x;
			this.y = y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint x, uint2 yz, uint w)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint x, uint3 yzw)
		{
			this.x = x;
			this.y = yzw.x;
			this.z = yzw.y;
			this.w = yzw.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint2 xy, uint z, uint w)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint2 xy, uint2 zw)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint3 xyz, uint w)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint4 xyzw)
		{
			this.x = xyzw.x;
			this.y = xyzw.y;
			this.z = xyzw.z;
			this.w = xyzw.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(uint v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(bool v)
		{
			this.x = (v ? 1U : 0U);
			this.y = (v ? 1U : 0U);
			this.z = (v ? 1U : 0U);
			this.w = (v ? 1U : 0U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(bool4 v)
		{
			this.x = (v.x ? 1U : 0U);
			this.y = (v.y ? 1U : 0U);
			this.z = (v.z ? 1U : 0U);
			this.w = (v.w ? 1U : 0U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(int v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
			this.z = (uint)v;
			this.w = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(int4 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
			this.z = (uint)v.z;
			this.w = (uint)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(float v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
			this.z = (uint)v;
			this.w = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(float4 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
			this.z = (uint)v.z;
			this.w = (uint)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(double v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
			this.z = (uint)v;
			this.w = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4(double4 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
			this.z = (uint)v.z;
			this.w = (uint)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint4(uint v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4(bool v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4(bool4 v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4(int v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4(int4 v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4(float v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4(float4 v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4(double v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint4(double4 v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator *(uint4 lhs, uint4 rhs)
		{
			return new uint4(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator *(uint4 lhs, uint rhs)
		{
			return new uint4(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator *(uint lhs, uint4 rhs)
		{
			return new uint4(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z, lhs * rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator +(uint4 lhs, uint4 rhs)
		{
			return new uint4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator +(uint4 lhs, uint rhs)
		{
			return new uint4(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator +(uint lhs, uint4 rhs)
		{
			return new uint4(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z, lhs + rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator -(uint4 lhs, uint4 rhs)
		{
			return new uint4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator -(uint4 lhs, uint rhs)
		{
			return new uint4(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs, lhs.w - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator -(uint lhs, uint4 rhs)
		{
			return new uint4(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z, lhs - rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator /(uint4 lhs, uint4 rhs)
		{
			return new uint4(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator /(uint4 lhs, uint rhs)
		{
			return new uint4(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator /(uint lhs, uint4 rhs)
		{
			return new uint4(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z, lhs / rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator %(uint4 lhs, uint4 rhs)
		{
			return new uint4(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z, lhs.w % rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator %(uint4 lhs, uint rhs)
		{
			return new uint4(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs, lhs.w % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator %(uint lhs, uint4 rhs)
		{
			return new uint4(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z, lhs % rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator ++(uint4 val)
		{
			uint num = val.x + 1U;
			val.x = num;
			uint num2 = num;
			num = val.y + 1U;
			val.y = num;
			uint num3 = num;
			num = val.z + 1U;
			val.z = num;
			uint num4 = num;
			num = val.w + 1U;
			val.w = num;
			return new uint4(num2, num3, num4, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator --(uint4 val)
		{
			uint num = val.x - 1U;
			val.x = num;
			uint num2 = num;
			num = val.y - 1U;
			val.y = num;
			uint num3 = num;
			num = val.z - 1U;
			val.z = num;
			uint num4 = num;
			num = val.w - 1U;
			val.w = num;
			return new uint4(num2, num3, num4, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(uint4 lhs, uint4 rhs)
		{
			return new bool4(lhs.x < rhs.x, lhs.y < rhs.y, lhs.z < rhs.z, lhs.w < rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(uint4 lhs, uint rhs)
		{
			return new bool4(lhs.x < rhs, lhs.y < rhs, lhs.z < rhs, lhs.w < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(uint lhs, uint4 rhs)
		{
			return new bool4(lhs < rhs.x, lhs < rhs.y, lhs < rhs.z, lhs < rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(uint4 lhs, uint4 rhs)
		{
			return new bool4(lhs.x <= rhs.x, lhs.y <= rhs.y, lhs.z <= rhs.z, lhs.w <= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(uint4 lhs, uint rhs)
		{
			return new bool4(lhs.x <= rhs, lhs.y <= rhs, lhs.z <= rhs, lhs.w <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(uint lhs, uint4 rhs)
		{
			return new bool4(lhs <= rhs.x, lhs <= rhs.y, lhs <= rhs.z, lhs <= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(uint4 lhs, uint4 rhs)
		{
			return new bool4(lhs.x > rhs.x, lhs.y > rhs.y, lhs.z > rhs.z, lhs.w > rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(uint4 lhs, uint rhs)
		{
			return new bool4(lhs.x > rhs, lhs.y > rhs, lhs.z > rhs, lhs.w > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(uint lhs, uint4 rhs)
		{
			return new bool4(lhs > rhs.x, lhs > rhs.y, lhs > rhs.z, lhs > rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(uint4 lhs, uint4 rhs)
		{
			return new bool4(lhs.x >= rhs.x, lhs.y >= rhs.y, lhs.z >= rhs.z, lhs.w >= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(uint4 lhs, uint rhs)
		{
			return new bool4(lhs.x >= rhs, lhs.y >= rhs, lhs.z >= rhs, lhs.w >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(uint lhs, uint4 rhs)
		{
			return new bool4(lhs >= rhs.x, lhs >= rhs.y, lhs >= rhs.z, lhs >= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator -(uint4 val)
		{
			return new uint4((uint)(-(uint)((ulong)val.x)), (uint)(-(uint)((ulong)val.y)), (uint)(-(uint)((ulong)val.z)), (uint)(-(uint)((ulong)val.w)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator +(uint4 val)
		{
			return new uint4(val.x, val.y, val.z, val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator <<(uint4 x, int n)
		{
			return new uint4(x.x << n, x.y << n, x.z << n, x.w << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator >>(uint4 x, int n)
		{
			return new uint4(x.x >> n, x.y >> n, x.z >> n, x.w >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(uint4 lhs, uint4 rhs)
		{
			return new bool4(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z, lhs.w == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(uint4 lhs, uint rhs)
		{
			return new bool4(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs, lhs.w == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(uint lhs, uint4 rhs)
		{
			return new bool4(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z, lhs == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(uint4 lhs, uint4 rhs)
		{
			return new bool4(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z, lhs.w != rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(uint4 lhs, uint rhs)
		{
			return new bool4(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs, lhs.w != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(uint lhs, uint4 rhs)
		{
			return new bool4(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z, lhs != rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator ~(uint4 val)
		{
			return new uint4(~val.x, ~val.y, ~val.z, ~val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator &(uint4 lhs, uint4 rhs)
		{
			return new uint4(lhs.x & rhs.x, lhs.y & rhs.y, lhs.z & rhs.z, lhs.w & rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator &(uint4 lhs, uint rhs)
		{
			return new uint4(lhs.x & rhs, lhs.y & rhs, lhs.z & rhs, lhs.w & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator &(uint lhs, uint4 rhs)
		{
			return new uint4(lhs & rhs.x, lhs & rhs.y, lhs & rhs.z, lhs & rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator |(uint4 lhs, uint4 rhs)
		{
			return new uint4(lhs.x | rhs.x, lhs.y | rhs.y, lhs.z | rhs.z, lhs.w | rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator |(uint4 lhs, uint rhs)
		{
			return new uint4(lhs.x | rhs, lhs.y | rhs, lhs.z | rhs, lhs.w | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator |(uint lhs, uint4 rhs)
		{
			return new uint4(lhs | rhs.x, lhs | rhs.y, lhs | rhs.z, lhs | rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator ^(uint4 lhs, uint4 rhs)
		{
			return new uint4(lhs.x ^ rhs.x, lhs.y ^ rhs.y, lhs.z ^ rhs.z, lhs.w ^ rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator ^(uint4 lhs, uint rhs)
		{
			return new uint4(lhs.x ^ rhs, lhs.y ^ rhs, lhs.z ^ rhs, lhs.w ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 operator ^(uint lhs, uint4 rhs)
		{
			return new uint4(lhs ^ rhs.x, lhs ^ rhs.y, lhs ^ rhs.z, lhs ^ rhs.w);
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
		public uint4 xxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.x, this.w);
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
		public uint4 xxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.x, this.w, this.w);
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
		public uint4 xyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.x, this.w);
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
		public uint4 xyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.z = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.w = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
				this.y = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
				this.w = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
				this.y = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
				this.z = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 xwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.x, this.w, this.w, this.w);
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
		public uint4 yxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.x, this.w);
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
		public uint4 yxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
				this.z = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
				this.w = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.x, this.w, this.w);
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
		public uint4 yyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.x, this.w);
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
		public uint4 yyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
				this.x = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
				this.w = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 yzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
				this.x = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
				this.z = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 ywww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.y, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
				this.y = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
				this.w = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
				this.x = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
				this.w = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
				this.x = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
				this.y = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 zwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.z, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
				this.y = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
				this.z = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
				this.x = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
				this.z = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
				this.x = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
				this.y = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint4 wwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint4(this.w, this.w, this.w, this.w);
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
		public uint3 xxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.x, this.w);
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
		public uint3 xyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 xww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.x, this.w, this.w);
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
		public uint3 yxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
				this.w = value.z;
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
		public uint3 yyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 ywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 ywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 ywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 yww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 zww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 wwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint3 www
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint3(this.w, this.w, this.w);
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
		public uint2 xz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 xw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 yz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 yw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 zx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 zy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 zz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 zw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 wx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 wy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 wz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public uint2 ww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new uint2(this.w, this.w);
			}
		}

		public unsafe uint this[int index]
		{
			get
			{
				fixed (uint4* ptr = &this)
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
		public bool Equals(uint4 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z && this.w == rhs.w;
		}

		public override bool Equals(object o)
		{
			if (o is uint4)
			{
				uint4 @uint = (uint4)o;
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
			return string.Format("uint4({0}, {1}, {2}, {3})", new object[] { this.x, this.y, this.z, this.w });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("uint4({0}, {1}, {2}, {3})", new object[]
			{
				this.x.ToString(format, formatProvider),
				this.y.ToString(format, formatProvider),
				this.z.ToString(format, formatProvider),
				this.w.ToString(format, formatProvider)
			});
		}

		public uint x;

		public uint y;

		public uint z;

		public uint w;

		public static readonly uint4 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(uint4 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
				this.w = v.w;
			}

			public uint x;

			public uint y;

			public uint z;

			public uint w;
		}
	}
}
