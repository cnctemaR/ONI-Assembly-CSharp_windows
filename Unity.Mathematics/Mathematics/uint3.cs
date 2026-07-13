using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(uint3.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct uint3 : IEquatable<uint3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(uint x, uint y, uint z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(uint x, uint2 yz)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(uint2 xy, uint z)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(uint3 xyz)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(uint v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(bool v)
		{
			this.x = (v ? 1U : 0U);
			this.y = (v ? 1U : 0U);
			this.z = (v ? 1U : 0U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(bool3 v)
		{
			this.x = (v.x ? 1U : 0U);
			this.y = (v.y ? 1U : 0U);
			this.z = (v.z ? 1U : 0U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(int v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
			this.z = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(int3 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
			this.z = (uint)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(float v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
			this.z = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(float3 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
			this.z = (uint)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(double v)
		{
			this.x = (uint)v;
			this.y = (uint)v;
			this.z = (uint)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3(double3 v)
		{
			this.x = (uint)v.x;
			this.y = (uint)v.y;
			this.z = (uint)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator uint3(uint v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3(bool v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3(bool3 v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3(int v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3(int3 v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3(float v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3(float3 v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3(double v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator uint3(double3 v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator *(uint3 lhs, uint3 rhs)
		{
			return new uint3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator *(uint3 lhs, uint rhs)
		{
			return new uint3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator *(uint lhs, uint3 rhs)
		{
			return new uint3(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator +(uint3 lhs, uint3 rhs)
		{
			return new uint3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator +(uint3 lhs, uint rhs)
		{
			return new uint3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator +(uint lhs, uint3 rhs)
		{
			return new uint3(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator -(uint3 lhs, uint3 rhs)
		{
			return new uint3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator -(uint3 lhs, uint rhs)
		{
			return new uint3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator -(uint lhs, uint3 rhs)
		{
			return new uint3(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator /(uint3 lhs, uint3 rhs)
		{
			return new uint3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator /(uint3 lhs, uint rhs)
		{
			return new uint3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator /(uint lhs, uint3 rhs)
		{
			return new uint3(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator %(uint3 lhs, uint3 rhs)
		{
			return new uint3(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator %(uint3 lhs, uint rhs)
		{
			return new uint3(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator %(uint lhs, uint3 rhs)
		{
			return new uint3(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator ++(uint3 val)
		{
			uint num = val.x + 1U;
			val.x = num;
			uint num2 = num;
			num = val.y + 1U;
			val.y = num;
			uint num3 = num;
			num = val.z + 1U;
			val.z = num;
			return new uint3(num2, num3, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator --(uint3 val)
		{
			uint num = val.x - 1U;
			val.x = num;
			uint num2 = num;
			num = val.y - 1U;
			val.y = num;
			uint num3 = num;
			num = val.z - 1U;
			val.z = num;
			return new uint3(num2, num3, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(uint3 lhs, uint3 rhs)
		{
			return new bool3(lhs.x < rhs.x, lhs.y < rhs.y, lhs.z < rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(uint3 lhs, uint rhs)
		{
			return new bool3(lhs.x < rhs, lhs.y < rhs, lhs.z < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(uint lhs, uint3 rhs)
		{
			return new bool3(lhs < rhs.x, lhs < rhs.y, lhs < rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(uint3 lhs, uint3 rhs)
		{
			return new bool3(lhs.x <= rhs.x, lhs.y <= rhs.y, lhs.z <= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(uint3 lhs, uint rhs)
		{
			return new bool3(lhs.x <= rhs, lhs.y <= rhs, lhs.z <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(uint lhs, uint3 rhs)
		{
			return new bool3(lhs <= rhs.x, lhs <= rhs.y, lhs <= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(uint3 lhs, uint3 rhs)
		{
			return new bool3(lhs.x > rhs.x, lhs.y > rhs.y, lhs.z > rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(uint3 lhs, uint rhs)
		{
			return new bool3(lhs.x > rhs, lhs.y > rhs, lhs.z > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(uint lhs, uint3 rhs)
		{
			return new bool3(lhs > rhs.x, lhs > rhs.y, lhs > rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(uint3 lhs, uint3 rhs)
		{
			return new bool3(lhs.x >= rhs.x, lhs.y >= rhs.y, lhs.z >= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(uint3 lhs, uint rhs)
		{
			return new bool3(lhs.x >= rhs, lhs.y >= rhs, lhs.z >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(uint lhs, uint3 rhs)
		{
			return new bool3(lhs >= rhs.x, lhs >= rhs.y, lhs >= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator -(uint3 val)
		{
			return new uint3((uint)(-(uint)((ulong)val.x)), (uint)(-(uint)((ulong)val.y)), (uint)(-(uint)((ulong)val.z)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator +(uint3 val)
		{
			return new uint3(val.x, val.y, val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator <<(uint3 x, int n)
		{
			return new uint3(x.x << n, x.y << n, x.z << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator >>(uint3 x, int n)
		{
			return new uint3(x.x >> n, x.y >> n, x.z >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(uint3 lhs, uint3 rhs)
		{
			return new bool3(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(uint3 lhs, uint rhs)
		{
			return new bool3(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(uint lhs, uint3 rhs)
		{
			return new bool3(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(uint3 lhs, uint3 rhs)
		{
			return new bool3(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(uint3 lhs, uint rhs)
		{
			return new bool3(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(uint lhs, uint3 rhs)
		{
			return new bool3(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator ~(uint3 val)
		{
			return new uint3(~val.x, ~val.y, ~val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator &(uint3 lhs, uint3 rhs)
		{
			return new uint3(lhs.x & rhs.x, lhs.y & rhs.y, lhs.z & rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator &(uint3 lhs, uint rhs)
		{
			return new uint3(lhs.x & rhs, lhs.y & rhs, lhs.z & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator &(uint lhs, uint3 rhs)
		{
			return new uint3(lhs & rhs.x, lhs & rhs.y, lhs & rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator |(uint3 lhs, uint3 rhs)
		{
			return new uint3(lhs.x | rhs.x, lhs.y | rhs.y, lhs.z | rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator |(uint3 lhs, uint rhs)
		{
			return new uint3(lhs.x | rhs, lhs.y | rhs, lhs.z | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator |(uint lhs, uint3 rhs)
		{
			return new uint3(lhs | rhs.x, lhs | rhs.y, lhs | rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator ^(uint3 lhs, uint3 rhs)
		{
			return new uint3(lhs.x ^ rhs.x, lhs.y ^ rhs.y, lhs.z ^ rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator ^(uint3 lhs, uint rhs)
		{
			return new uint3(lhs.x ^ rhs, lhs.y ^ rhs, lhs.z ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 operator ^(uint lhs, uint3 rhs)
		{
			return new uint3(lhs ^ rhs.x, lhs ^ rhs.y, lhs ^ rhs.z);
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

		public unsafe uint this[int index]
		{
			get
			{
				fixed (uint3* ptr = &this)
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
		public bool Equals(uint3 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z;
		}

		public override bool Equals(object o)
		{
			if (o is uint3)
			{
				uint3 @uint = (uint3)o;
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
			return string.Format("uint3({0}, {1}, {2})", this.x, this.y, this.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("uint3({0}, {1}, {2})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider), this.z.ToString(format, formatProvider));
		}

		public uint x;

		public uint y;

		public uint z;

		public static readonly uint3 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(uint3 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
			}

			public uint x;

			public uint y;

			public uint z;
		}
	}
}
