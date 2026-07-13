using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(double2.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double2 : IEquatable<double2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(double2 xy)
		{
			this.x = xy.x;
			this.y = xy.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(double v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(bool v)
		{
			this.x = (v ? 1.0 : 0.0);
			this.y = (v ? 1.0 : 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(bool2 v)
		{
			this.x = (v.x ? 1.0 : 0.0);
			this.y = (v.y ? 1.0 : 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(int v)
		{
			this.x = (double)v;
			this.y = (double)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(int2 v)
		{
			this.x = (double)v.x;
			this.y = (double)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(uint v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(uint2 v)
		{
			this.x = v.x;
			this.y = v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(half v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(half2 v)
		{
			this.x = v.x;
			this.y = v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(float v)
		{
			this.x = (double)v;
			this.y = (double)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2(float2 v)
		{
			this.x = (double)v.x;
			this.y = (double)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(double v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double2(bool v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double2(bool2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(int v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(int2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(uint v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(uint2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(half v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(half2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(float v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2(float2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator *(double2 lhs, double2 rhs)
		{
			return new double2(lhs.x * rhs.x, lhs.y * rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator *(double2 lhs, double rhs)
		{
			return new double2(lhs.x * rhs, lhs.y * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator *(double lhs, double2 rhs)
		{
			return new double2(lhs * rhs.x, lhs * rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator +(double2 lhs, double2 rhs)
		{
			return new double2(lhs.x + rhs.x, lhs.y + rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator +(double2 lhs, double rhs)
		{
			return new double2(lhs.x + rhs, lhs.y + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator +(double lhs, double2 rhs)
		{
			return new double2(lhs + rhs.x, lhs + rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator -(double2 lhs, double2 rhs)
		{
			return new double2(lhs.x - rhs.x, lhs.y - rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator -(double2 lhs, double rhs)
		{
			return new double2(lhs.x - rhs, lhs.y - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator -(double lhs, double2 rhs)
		{
			return new double2(lhs - rhs.x, lhs - rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator /(double2 lhs, double2 rhs)
		{
			return new double2(lhs.x / rhs.x, lhs.y / rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator /(double2 lhs, double rhs)
		{
			return new double2(lhs.x / rhs, lhs.y / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator /(double lhs, double2 rhs)
		{
			return new double2(lhs / rhs.x, lhs / rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator %(double2 lhs, double2 rhs)
		{
			return new double2(lhs.x % rhs.x, lhs.y % rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator %(double2 lhs, double rhs)
		{
			return new double2(lhs.x % rhs, lhs.y % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator %(double lhs, double2 rhs)
		{
			return new double2(lhs % rhs.x, lhs % rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator ++(double2 val)
		{
			double num = val.x + 1.0;
			val.x = num;
			double num2 = num;
			num = val.y + 1.0;
			val.y = num;
			return new double2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator --(double2 val)
		{
			double num = val.x - 1.0;
			val.x = num;
			double num2 = num;
			num = val.y - 1.0;
			val.y = num;
			return new double2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(double2 lhs, double2 rhs)
		{
			return new bool2(lhs.x < rhs.x, lhs.y < rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(double2 lhs, double rhs)
		{
			return new bool2(lhs.x < rhs, lhs.y < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(double lhs, double2 rhs)
		{
			return new bool2(lhs < rhs.x, lhs < rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(double2 lhs, double2 rhs)
		{
			return new bool2(lhs.x <= rhs.x, lhs.y <= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(double2 lhs, double rhs)
		{
			return new bool2(lhs.x <= rhs, lhs.y <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(double lhs, double2 rhs)
		{
			return new bool2(lhs <= rhs.x, lhs <= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(double2 lhs, double2 rhs)
		{
			return new bool2(lhs.x > rhs.x, lhs.y > rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(double2 lhs, double rhs)
		{
			return new bool2(lhs.x > rhs, lhs.y > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(double lhs, double2 rhs)
		{
			return new bool2(lhs > rhs.x, lhs > rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(double2 lhs, double2 rhs)
		{
			return new bool2(lhs.x >= rhs.x, lhs.y >= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(double2 lhs, double rhs)
		{
			return new bool2(lhs.x >= rhs, lhs.y >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(double lhs, double2 rhs)
		{
			return new bool2(lhs >= rhs.x, lhs >= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator -(double2 val)
		{
			return new double2(-val.x, -val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 operator +(double2 val)
		{
			return new double2(val.x, val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(double2 lhs, double2 rhs)
		{
			return new bool2(lhs.x == rhs.x, lhs.y == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(double2 lhs, double rhs)
		{
			return new bool2(lhs.x == rhs, lhs.y == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(double lhs, double2 rhs)
		{
			return new bool2(lhs == rhs.x, lhs == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(double2 lhs, double2 rhs)
		{
			return new bool2(lhs.x != rhs.x, lhs.y != rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(double2 lhs, double rhs)
		{
			return new bool2(lhs.x != rhs, lhs.y != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(double lhs, double2 rhs)
		{
			return new bool2(lhs != rhs.x, lhs != rhs.y);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 xxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 xxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 xyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 xyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 yxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 yxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 yyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 yyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 xx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 xy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 yx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 yy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.y, this.y);
			}
		}

		public unsafe double this[int index]
		{
			get
			{
				fixed (double2* ptr = &this)
				{
					return ((double*)ptr)[index];
				}
			}
			set
			{
				fixed (double* ptr = &this.x)
				{
					ptr[index] = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(double2 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y;
		}

		public override bool Equals(object o)
		{
			if (o is double2)
			{
				double2 @double = (double2)o;
				return this.Equals(@double);
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
			return string.Format("double2({0}, {1})", this.x, this.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double2({0}, {1})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider));
		}

		public double x;

		public double y;

		public static readonly double2 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(double2 v)
			{
				this.x = v.x;
				this.y = v.y;
			}

			public double x;

			public double y;
		}
	}
}
