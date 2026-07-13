using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(int2.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct int2 : IEquatable<int2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(int2 xy)
		{
			this.x = xy.x;
			this.y = xy.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(int v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(bool v)
		{
			this.x = (v ? 1 : 0);
			this.y = (v ? 1 : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(bool2 v)
		{
			this.x = (v.x ? 1 : 0);
			this.y = (v.y ? 1 : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(uint v)
		{
			this.x = (int)v;
			this.y = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(uint2 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(float v)
		{
			this.x = (int)v;
			this.y = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(float2 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(double v)
		{
			this.x = (int)v;
			this.y = (int)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2(double2 v)
		{
			this.x = (int)v.x;
			this.y = (int)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int2(int v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2(bool v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2(bool2 v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2(uint v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2(uint2 v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2(float v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2(float2 v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2(double v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int2(double2 v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator *(int2 lhs, int2 rhs)
		{
			return new int2(lhs.x * rhs.x, lhs.y * rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator *(int2 lhs, int rhs)
		{
			return new int2(lhs.x * rhs, lhs.y * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator *(int lhs, int2 rhs)
		{
			return new int2(lhs * rhs.x, lhs * rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator +(int2 lhs, int2 rhs)
		{
			return new int2(lhs.x + rhs.x, lhs.y + rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator +(int2 lhs, int rhs)
		{
			return new int2(lhs.x + rhs, lhs.y + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator +(int lhs, int2 rhs)
		{
			return new int2(lhs + rhs.x, lhs + rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator -(int2 lhs, int2 rhs)
		{
			return new int2(lhs.x - rhs.x, lhs.y - rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator -(int2 lhs, int rhs)
		{
			return new int2(lhs.x - rhs, lhs.y - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator -(int lhs, int2 rhs)
		{
			return new int2(lhs - rhs.x, lhs - rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator /(int2 lhs, int2 rhs)
		{
			return new int2(lhs.x / rhs.x, lhs.y / rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator /(int2 lhs, int rhs)
		{
			return new int2(lhs.x / rhs, lhs.y / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator /(int lhs, int2 rhs)
		{
			return new int2(lhs / rhs.x, lhs / rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator %(int2 lhs, int2 rhs)
		{
			return new int2(lhs.x % rhs.x, lhs.y % rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator %(int2 lhs, int rhs)
		{
			return new int2(lhs.x % rhs, lhs.y % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator %(int lhs, int2 rhs)
		{
			return new int2(lhs % rhs.x, lhs % rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator ++(int2 val)
		{
			int num = val.x + 1;
			val.x = num;
			int num2 = num;
			num = val.y + 1;
			val.y = num;
			return new int2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator --(int2 val)
		{
			int num = val.x - 1;
			val.x = num;
			int num2 = num;
			num = val.y - 1;
			val.y = num;
			return new int2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(int2 lhs, int2 rhs)
		{
			return new bool2(lhs.x < rhs.x, lhs.y < rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(int2 lhs, int rhs)
		{
			return new bool2(lhs.x < rhs, lhs.y < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(int lhs, int2 rhs)
		{
			return new bool2(lhs < rhs.x, lhs < rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(int2 lhs, int2 rhs)
		{
			return new bool2(lhs.x <= rhs.x, lhs.y <= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(int2 lhs, int rhs)
		{
			return new bool2(lhs.x <= rhs, lhs.y <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(int lhs, int2 rhs)
		{
			return new bool2(lhs <= rhs.x, lhs <= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(int2 lhs, int2 rhs)
		{
			return new bool2(lhs.x > rhs.x, lhs.y > rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(int2 lhs, int rhs)
		{
			return new bool2(lhs.x > rhs, lhs.y > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(int lhs, int2 rhs)
		{
			return new bool2(lhs > rhs.x, lhs > rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(int2 lhs, int2 rhs)
		{
			return new bool2(lhs.x >= rhs.x, lhs.y >= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(int2 lhs, int rhs)
		{
			return new bool2(lhs.x >= rhs, lhs.y >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(int lhs, int2 rhs)
		{
			return new bool2(lhs >= rhs.x, lhs >= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator -(int2 val)
		{
			return new int2(-val.x, -val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator +(int2 val)
		{
			return new int2(val.x, val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator <<(int2 x, int n)
		{
			return new int2(x.x << n, x.y << n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator >>(int2 x, int n)
		{
			return new int2(x.x >> n, x.y >> n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(int2 lhs, int2 rhs)
		{
			return new bool2(lhs.x == rhs.x, lhs.y == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(int2 lhs, int rhs)
		{
			return new bool2(lhs.x == rhs, lhs.y == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(int lhs, int2 rhs)
		{
			return new bool2(lhs == rhs.x, lhs == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(int2 lhs, int2 rhs)
		{
			return new bool2(lhs.x != rhs.x, lhs.y != rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(int2 lhs, int rhs)
		{
			return new bool2(lhs.x != rhs, lhs.y != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(int lhs, int2 rhs)
		{
			return new bool2(lhs != rhs.x, lhs != rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator ~(int2 val)
		{
			return new int2(~val.x, ~val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator &(int2 lhs, int2 rhs)
		{
			return new int2(lhs.x & rhs.x, lhs.y & rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator &(int2 lhs, int rhs)
		{
			return new int2(lhs.x & rhs, lhs.y & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator &(int lhs, int2 rhs)
		{
			return new int2(lhs & rhs.x, lhs & rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator |(int2 lhs, int2 rhs)
		{
			return new int2(lhs.x | rhs.x, lhs.y | rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator |(int2 lhs, int rhs)
		{
			return new int2(lhs.x | rhs, lhs.y | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator |(int lhs, int2 rhs)
		{
			return new int2(lhs | rhs.x, lhs | rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator ^(int2 lhs, int2 rhs)
		{
			return new int2(lhs.x ^ rhs.x, lhs.y ^ rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator ^(int2 lhs, int rhs)
		{
			return new int2(lhs.x ^ rhs, lhs.y ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 operator ^(int lhs, int2 rhs)
		{
			return new int2(lhs ^ rhs.x, lhs ^ rhs.y);
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

		public unsafe int this[int index]
		{
			get
			{
				fixed (int2* ptr = &this)
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
		public bool Equals(int2 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y;
		}

		public override bool Equals(object o)
		{
			if (o is int2)
			{
				int2 @int = (int2)o;
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
			return string.Format("int2({0}, {1})", this.x, this.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("int2({0}, {1})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider));
		}

		public int x;

		public int y;

		public static readonly int2 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(int2 v)
			{
				this.x = v.x;
				this.y = v.y;
			}

			public int x;

			public int y;
		}
	}
}
