using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(int3.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct int3 : IEquatable<int3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(int x, int2 yz)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(int2 xy, int z)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(int3 xyz)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(int v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(bool v)
		{
			this.x = (v ? 1 : 0);
			this.y = (v ? 1 : 0);
			this.z = (v ? 1 : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(bool3 v)
		{
			this.x = (v.x ? 1 : 0);
			this.y = (v.y ? 1 : 0);
			this.z = (v.z ? 1 : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(uint v)
		{
			this.x = (int)v;
			this.y = (int)v;
			this.z = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(uint3 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
			this.z = (int)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(float v)
		{
			this.x = (int)v;
			this.y = (int)v;
			this.z = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(float3 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
			this.z = (int)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(double v)
		{
			this.x = (int)v;
			this.y = (int)v;
			this.z = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3(double3 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
			this.z = (int)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int3(int v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3(bool v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3(bool3 v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3(uint v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3(uint3 v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3(float v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3(float3 v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3(double v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int3(double3 v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator *(int3 lhs, int3 rhs)
		{
			return new int3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator *(int3 lhs, int rhs)
		{
			return new int3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator *(int lhs, int3 rhs)
		{
			return new int3(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator +(int3 lhs, int3 rhs)
		{
			return new int3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator +(int3 lhs, int rhs)
		{
			return new int3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator +(int lhs, int3 rhs)
		{
			return new int3(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator -(int3 lhs, int3 rhs)
		{
			return new int3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator -(int3 lhs, int rhs)
		{
			return new int3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator -(int lhs, int3 rhs)
		{
			return new int3(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator /(int3 lhs, int3 rhs)
		{
			return new int3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator /(int3 lhs, int rhs)
		{
			return new int3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator /(int lhs, int3 rhs)
		{
			return new int3(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator %(int3 lhs, int3 rhs)
		{
			return new int3(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator %(int3 lhs, int rhs)
		{
			return new int3(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator %(int lhs, int3 rhs)
		{
			return new int3(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator ++(int3 val)
		{
			int num = val.x + 1;
			val.x = num;
			int num2 = num;
			num = val.y + 1;
			val.y = num;
			int num3 = num;
			num = val.z + 1;
			val.z = num;
			return new int3(num2, num3, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator --(int3 val)
		{
			int num = val.x - 1;
			val.x = num;
			int num2 = num;
			num = val.y - 1;
			val.y = num;
			int num3 = num;
			num = val.z - 1;
			val.z = num;
			return new int3(num2, num3, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(int3 lhs, int3 rhs)
		{
			return new bool3(lhs.x < rhs.x, lhs.y < rhs.y, lhs.z < rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(int3 lhs, int rhs)
		{
			return new bool3(lhs.x < rhs, lhs.y < rhs, lhs.z < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(int lhs, int3 rhs)
		{
			return new bool3(lhs < rhs.x, lhs < rhs.y, lhs < rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(int3 lhs, int3 rhs)
		{
			return new bool3(lhs.x <= rhs.x, lhs.y <= rhs.y, lhs.z <= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(int3 lhs, int rhs)
		{
			return new bool3(lhs.x <= rhs, lhs.y <= rhs, lhs.z <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(int lhs, int3 rhs)
		{
			return new bool3(lhs <= rhs.x, lhs <= rhs.y, lhs <= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(int3 lhs, int3 rhs)
		{
			return new bool3(lhs.x > rhs.x, lhs.y > rhs.y, lhs.z > rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(int3 lhs, int rhs)
		{
			return new bool3(lhs.x > rhs, lhs.y > rhs, lhs.z > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(int lhs, int3 rhs)
		{
			return new bool3(lhs > rhs.x, lhs > rhs.y, lhs > rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(int3 lhs, int3 rhs)
		{
			return new bool3(lhs.x >= rhs.x, lhs.y >= rhs.y, lhs.z >= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(int3 lhs, int rhs)
		{
			return new bool3(lhs.x >= rhs, lhs.y >= rhs, lhs.z >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(int lhs, int3 rhs)
		{
			return new bool3(lhs >= rhs.x, lhs >= rhs.y, lhs >= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator -(int3 val)
		{
			return new int3(-val.x, -val.y, -val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator +(int3 val)
		{
			return new int3(val.x, val.y, val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator <<(int3 x, int n)
		{
			return new int3(x.x << n, x.y << n, x.z << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator >>(int3 x, int n)
		{
			return new int3(x.x >> n, x.y >> n, x.z >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(int3 lhs, int3 rhs)
		{
			return new bool3(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(int3 lhs, int rhs)
		{
			return new bool3(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(int lhs, int3 rhs)
		{
			return new bool3(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(int3 lhs, int3 rhs)
		{
			return new bool3(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(int3 lhs, int rhs)
		{
			return new bool3(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(int lhs, int3 rhs)
		{
			return new bool3(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator ~(int3 val)
		{
			return new int3(~val.x, ~val.y, ~val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator &(int3 lhs, int3 rhs)
		{
			return new int3(lhs.x & rhs.x, lhs.y & rhs.y, lhs.z & rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator &(int3 lhs, int rhs)
		{
			return new int3(lhs.x & rhs, lhs.y & rhs, lhs.z & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator &(int lhs, int3 rhs)
		{
			return new int3(lhs & rhs.x, lhs & rhs.y, lhs & rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator |(int3 lhs, int3 rhs)
		{
			return new int3(lhs.x | rhs.x, lhs.y | rhs.y, lhs.z | rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator |(int3 lhs, int rhs)
		{
			return new int3(lhs.x | rhs, lhs.y | rhs, lhs.z | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator |(int lhs, int3 rhs)
		{
			return new int3(lhs | rhs.x, lhs | rhs.y, lhs | rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator ^(int3 lhs, int3 rhs)
		{
			return new int3(lhs.x ^ rhs.x, lhs.y ^ rhs.y, lhs.z ^ rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator ^(int3 lhs, int rhs)
		{
			return new int3(lhs.x ^ rhs, lhs.y ^ rhs, lhs.z ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 operator ^(int lhs, int3 rhs)
		{
			return new int3(lhs ^ rhs.x, lhs ^ rhs.y, lhs ^ rhs.z);
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

		public unsafe int this[int index]
		{
			get
			{
				fixed (int3* ptr = &this)
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
		public bool Equals(int3 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z;
		}

		public override bool Equals(object o)
		{
			if (o is int3)
			{
				int3 @int = (int3)o;
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
			return string.Format("int3({0}, {1}, {2})", this.x, this.y, this.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("int3({0}, {1}, {2})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider), this.z.ToString(format, formatProvider));
		}

		public int x;

		public int y;

		public int z;

		public static readonly int3 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(int3 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
			}

			public int x;

			public int y;

			public int z;
		}
	}
}
