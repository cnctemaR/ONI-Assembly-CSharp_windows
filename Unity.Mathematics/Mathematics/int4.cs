using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(int4.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct int4 : IEquatable<int4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int x, int y, int z, int w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int x, int y, int2 zw)
		{
			this.x = x;
			this.y = y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int x, int2 yz, int w)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int x, int3 yzw)
		{
			this.x = x;
			this.y = yzw.x;
			this.z = yzw.y;
			this.w = yzw.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int2 xy, int z, int w)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int2 xy, int2 zw)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int3 xyz, int w)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int4 xyzw)
		{
			this.x = xyzw.x;
			this.y = xyzw.y;
			this.z = xyzw.z;
			this.w = xyzw.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(int v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(bool v)
		{
			this.x = (v ? 1 : 0);
			this.y = (v ? 1 : 0);
			this.z = (v ? 1 : 0);
			this.w = (v ? 1 : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(bool4 v)
		{
			this.x = (v.x ? 1 : 0);
			this.y = (v.y ? 1 : 0);
			this.z = (v.z ? 1 : 0);
			this.w = (v.w ? 1 : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(uint v)
		{
			this.x = (int)v;
			this.y = (int)v;
			this.z = (int)v;
			this.w = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(uint4 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
			this.z = (int)v.z;
			this.w = (int)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(float v)
		{
			this.x = (int)v;
			this.y = (int)v;
			this.z = (int)v;
			this.w = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(float4 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
			this.z = (int)v.z;
			this.w = (int)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(double v)
		{
			this.x = (int)v;
			this.y = (int)v;
			this.z = (int)v;
			this.w = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4(double4 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
			this.z = (int)v.z;
			this.w = (int)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int4(int v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int4(bool v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int4(bool4 v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int4(uint v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int4(uint4 v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int4(float v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int4(float4 v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int4(double v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int4(double4 v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator *(int4 lhs, int4 rhs)
		{
			return new int4(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator *(int4 lhs, int rhs)
		{
			return new int4(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator *(int lhs, int4 rhs)
		{
			return new int4(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z, lhs * rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator +(int4 lhs, int4 rhs)
		{
			return new int4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator +(int4 lhs, int rhs)
		{
			return new int4(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator +(int lhs, int4 rhs)
		{
			return new int4(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z, lhs + rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator -(int4 lhs, int4 rhs)
		{
			return new int4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator -(int4 lhs, int rhs)
		{
			return new int4(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs, lhs.w - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator -(int lhs, int4 rhs)
		{
			return new int4(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z, lhs - rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator /(int4 lhs, int4 rhs)
		{
			return new int4(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator /(int4 lhs, int rhs)
		{
			return new int4(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator /(int lhs, int4 rhs)
		{
			return new int4(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z, lhs / rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator %(int4 lhs, int4 rhs)
		{
			return new int4(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z, lhs.w % rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator %(int4 lhs, int rhs)
		{
			return new int4(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs, lhs.w % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator %(int lhs, int4 rhs)
		{
			return new int4(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z, lhs % rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator ++(int4 val)
		{
			int num = val.x + 1;
			val.x = num;
			int num2 = num;
			num = val.y + 1;
			val.y = num;
			int num3 = num;
			num = val.z + 1;
			val.z = num;
			int num4 = num;
			num = val.w + 1;
			val.w = num;
			return new int4(num2, num3, num4, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator --(int4 val)
		{
			int num = val.x - 1;
			val.x = num;
			int num2 = num;
			num = val.y - 1;
			val.y = num;
			int num3 = num;
			num = val.z - 1;
			val.z = num;
			int num4 = num;
			num = val.w - 1;
			val.w = num;
			return new int4(num2, num3, num4, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(int4 lhs, int4 rhs)
		{
			return new bool4(lhs.x < rhs.x, lhs.y < rhs.y, lhs.z < rhs.z, lhs.w < rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(int4 lhs, int rhs)
		{
			return new bool4(lhs.x < rhs, lhs.y < rhs, lhs.z < rhs, lhs.w < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(int lhs, int4 rhs)
		{
			return new bool4(lhs < rhs.x, lhs < rhs.y, lhs < rhs.z, lhs < rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(int4 lhs, int4 rhs)
		{
			return new bool4(lhs.x <= rhs.x, lhs.y <= rhs.y, lhs.z <= rhs.z, lhs.w <= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(int4 lhs, int rhs)
		{
			return new bool4(lhs.x <= rhs, lhs.y <= rhs, lhs.z <= rhs, lhs.w <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(int lhs, int4 rhs)
		{
			return new bool4(lhs <= rhs.x, lhs <= rhs.y, lhs <= rhs.z, lhs <= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(int4 lhs, int4 rhs)
		{
			return new bool4(lhs.x > rhs.x, lhs.y > rhs.y, lhs.z > rhs.z, lhs.w > rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(int4 lhs, int rhs)
		{
			return new bool4(lhs.x > rhs, lhs.y > rhs, lhs.z > rhs, lhs.w > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(int lhs, int4 rhs)
		{
			return new bool4(lhs > rhs.x, lhs > rhs.y, lhs > rhs.z, lhs > rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(int4 lhs, int4 rhs)
		{
			return new bool4(lhs.x >= rhs.x, lhs.y >= rhs.y, lhs.z >= rhs.z, lhs.w >= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(int4 lhs, int rhs)
		{
			return new bool4(lhs.x >= rhs, lhs.y >= rhs, lhs.z >= rhs, lhs.w >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(int lhs, int4 rhs)
		{
			return new bool4(lhs >= rhs.x, lhs >= rhs.y, lhs >= rhs.z, lhs >= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator -(int4 val)
		{
			return new int4(-val.x, -val.y, -val.z, -val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator +(int4 val)
		{
			return new int4(val.x, val.y, val.z, val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator <<(int4 x, int n)
		{
			return new int4(x.x << n, x.y << n, x.z << n, x.w << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator >>(int4 x, int n)
		{
			return new int4(x.x >> n, x.y >> n, x.z >> n, x.w >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(int4 lhs, int4 rhs)
		{
			return new bool4(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z, lhs.w == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(int4 lhs, int rhs)
		{
			return new bool4(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs, lhs.w == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(int lhs, int4 rhs)
		{
			return new bool4(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z, lhs == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(int4 lhs, int4 rhs)
		{
			return new bool4(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z, lhs.w != rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(int4 lhs, int rhs)
		{
			return new bool4(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs, lhs.w != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(int lhs, int4 rhs)
		{
			return new bool4(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z, lhs != rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator ~(int4 val)
		{
			return new int4(~val.x, ~val.y, ~val.z, ~val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator &(int4 lhs, int4 rhs)
		{
			return new int4(lhs.x & rhs.x, lhs.y & rhs.y, lhs.z & rhs.z, lhs.w & rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator &(int4 lhs, int rhs)
		{
			return new int4(lhs.x & rhs, lhs.y & rhs, lhs.z & rhs, lhs.w & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator &(int lhs, int4 rhs)
		{
			return new int4(lhs & rhs.x, lhs & rhs.y, lhs & rhs.z, lhs & rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator |(int4 lhs, int4 rhs)
		{
			return new int4(lhs.x | rhs.x, lhs.y | rhs.y, lhs.z | rhs.z, lhs.w | rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator |(int4 lhs, int rhs)
		{
			return new int4(lhs.x | rhs, lhs.y | rhs, lhs.z | rhs, lhs.w | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator |(int lhs, int4 rhs)
		{
			return new int4(lhs | rhs.x, lhs | rhs.y, lhs | rhs.z, lhs | rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator ^(int4 lhs, int4 rhs)
		{
			return new int4(lhs.x ^ rhs.x, lhs.y ^ rhs.y, lhs.z ^ rhs.z, lhs.w ^ rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator ^(int4 lhs, int rhs)
		{
			return new int4(lhs.x ^ rhs, lhs.y ^ rhs, lhs.z ^ rhs, lhs.w ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 operator ^(int lhs, int4 rhs)
		{
			return new int4(lhs ^ rhs.x, lhs ^ rhs.y, lhs ^ rhs.z, lhs ^ rhs.w);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.z, this.w);
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
		public int4 xywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.w, this.z);
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
		public int4 xyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.y, this.w);
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
		public int4 xzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.w, this.y);
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
		public int4 xzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.y, this.z);
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
		public int4 xwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.z, this.y);
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
		public int4 xwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 xwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.x, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.z, this.w);
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
		public int4 yxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.w, this.z);
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
		public int4 yxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.x, this.w);
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
		public int4 yzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.w, this.x);
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
		public int4 yzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 yzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.x, this.z);
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
		public int4 ywxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.z, this.x);
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
		public int4 ywzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 ywww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.y, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.y, this.w);
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
		public int4 zxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.w, this.y);
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
		public int4 zxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.x, this.w);
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
		public int4 zyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.w, this.x);
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
		public int4 zywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.x, this.y);
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
		public int4 zwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.y, this.x);
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
		public int4 zwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 zwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.z, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.y, this.z);
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
		public int4 wxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.z, this.y);
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
		public int4 wxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.x, this.z);
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
		public int4 wyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.z, this.x);
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
		public int4 wyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.x, this.y);
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
		public int4 wzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.y, this.x);
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
		public int4 wzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int4 wwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int4(this.w, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.y, this.z);
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
		public int3 xyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.y, this.w);
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
		public int3 xzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.z, this.y);
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
		public int3 xzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.z, this.w);
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
		public int3 xwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 xwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.w, this.y);
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
		public int3 xwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.w, this.z);
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
		public int3 xww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.x, this.z);
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
		public int3 yxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.x, this.w);
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
		public int3 yyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.z, this.x);
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
		public int3 yzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 yzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.z, this.w);
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
		public int3 ywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.w, this.x);
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
		public int3 ywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 ywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.w, this.z);
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
		public int3 yww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.x, this.y);
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
		public int3 zxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.x, this.w);
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
		public int3 zyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.y, this.x);
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
		public int3 zyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.y, this.w);
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
		public int3 zzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.w, this.x);
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
		public int3 zwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.w, this.y);
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
		public int3 zwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 zww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.x, this.y);
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
		public int3 wxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.x, this.z);
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
		public int3 wxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.y, this.x);
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
		public int3 wyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.y, this.z);
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
		public int3 wyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.z, this.x);
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
		public int3 wzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.z, this.y);
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
		public int3 wzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 wwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int3 www
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int3(this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 xx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 xy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 xz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 xw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 yx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 yy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 yz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 yw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 zx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 zy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 zz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 zw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 wx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 wy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 wz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int2 ww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new int2(this.w, this.w);
			}
		}

		public unsafe int this[int index]
		{
			get
			{
				fixed (int4* ptr = &this)
				{
					return ((int*)ptr)[index];
				}
			}
			set
			{
				fixed (int* ptr = &this.x)
				{
					ptr[index] = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(int4 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z && this.w == rhs.w;
		}

		public override bool Equals(object o)
		{
			if (o is int4)
			{
				int4 @int = (int4)o;
				return this.Equals(@int);
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
			return string.Format("int4({0}, {1}, {2}, {3})", new object[] { this.x, this.y, this.z, this.w });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("int4({0}, {1}, {2}, {3})", new object[]
			{
				this.x.ToString(format, formatProvider),
				this.y.ToString(format, formatProvider),
				this.z.ToString(format, formatProvider),
				this.w.ToString(format, formatProvider)
			});
		}

		public int x;

		public int y;

		public int z;

		public int w;

		public static readonly int4 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(int4 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
				this.w = v.w;
			}

			public int x;

			public int y;

			public int z;

			public int w;
		}
	}
}
