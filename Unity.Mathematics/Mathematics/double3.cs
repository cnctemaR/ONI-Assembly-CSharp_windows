using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(double3.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double3 : IEquatable<double3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(double x, double2 yz)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(double2 xy, double z)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(double3 xyz)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(double v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(bool v)
		{
			this.x = (v ? 1.0 : 0.0);
			this.y = (v ? 1.0 : 0.0);
			this.z = (v ? 1.0 : 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(bool3 v)
		{
			this.x = (v.x ? 1.0 : 0.0);
			this.y = (v.y ? 1.0 : 0.0);
			this.z = (v.z ? 1.0 : 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(int v)
		{
			this.x = (double)v;
			this.y = (double)v;
			this.z = (double)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(int3 v)
		{
			this.x = (double)v.x;
			this.y = (double)v.y;
			this.z = (double)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(uint v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(uint3 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(half v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(half3 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(float v)
		{
			this.x = (double)v;
			this.y = (double)v;
			this.z = (double)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3(float3 v)
		{
			this.x = (double)v.x;
			this.y = (double)v.y;
			this.z = (double)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(double v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double3(bool v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double3(bool3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(int v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(int3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(uint v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(uint3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(half v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(half3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(float v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3(float3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator *(double3 lhs, double3 rhs)
		{
			return new double3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator *(double3 lhs, double rhs)
		{
			return new double3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator *(double lhs, double3 rhs)
		{
			return new double3(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator +(double3 lhs, double3 rhs)
		{
			return new double3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator +(double3 lhs, double rhs)
		{
			return new double3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator +(double lhs, double3 rhs)
		{
			return new double3(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator -(double3 lhs, double3 rhs)
		{
			return new double3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator -(double3 lhs, double rhs)
		{
			return new double3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator -(double lhs, double3 rhs)
		{
			return new double3(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator /(double3 lhs, double3 rhs)
		{
			return new double3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator /(double3 lhs, double rhs)
		{
			return new double3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator /(double lhs, double3 rhs)
		{
			return new double3(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator %(double3 lhs, double3 rhs)
		{
			return new double3(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator %(double3 lhs, double rhs)
		{
			return new double3(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator %(double lhs, double3 rhs)
		{
			return new double3(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator ++(double3 val)
		{
			double num = val.x + 1.0;
			val.x = num;
			double num2 = num;
			num = val.y + 1.0;
			val.y = num;
			double num3 = num;
			num = val.z + 1.0;
			val.z = num;
			return new double3(num2, num3, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator --(double3 val)
		{
			double num = val.x - 1.0;
			val.x = num;
			double num2 = num;
			num = val.y - 1.0;
			val.y = num;
			double num3 = num;
			num = val.z - 1.0;
			val.z = num;
			return new double3(num2, num3, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(double3 lhs, double3 rhs)
		{
			return new bool3(lhs.x < rhs.x, lhs.y < rhs.y, lhs.z < rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(double3 lhs, double rhs)
		{
			return new bool3(lhs.x < rhs, lhs.y < rhs, lhs.z < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(double lhs, double3 rhs)
		{
			return new bool3(lhs < rhs.x, lhs < rhs.y, lhs < rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(double3 lhs, double3 rhs)
		{
			return new bool3(lhs.x <= rhs.x, lhs.y <= rhs.y, lhs.z <= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(double3 lhs, double rhs)
		{
			return new bool3(lhs.x <= rhs, lhs.y <= rhs, lhs.z <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(double lhs, double3 rhs)
		{
			return new bool3(lhs <= rhs.x, lhs <= rhs.y, lhs <= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(double3 lhs, double3 rhs)
		{
			return new bool3(lhs.x > rhs.x, lhs.y > rhs.y, lhs.z > rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(double3 lhs, double rhs)
		{
			return new bool3(lhs.x > rhs, lhs.y > rhs, lhs.z > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(double lhs, double3 rhs)
		{
			return new bool3(lhs > rhs.x, lhs > rhs.y, lhs > rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(double3 lhs, double3 rhs)
		{
			return new bool3(lhs.x >= rhs.x, lhs.y >= rhs.y, lhs.z >= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(double3 lhs, double rhs)
		{
			return new bool3(lhs.x >= rhs, lhs.y >= rhs, lhs.z >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(double lhs, double3 rhs)
		{
			return new bool3(lhs >= rhs.x, lhs >= rhs.y, lhs >= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator -(double3 val)
		{
			return new double3(-val.x, -val.y, -val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 operator +(double3 val)
		{
			return new double3(val.x, val.y, val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(double3 lhs, double3 rhs)
		{
			return new bool3(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(double3 lhs, double rhs)
		{
			return new bool3(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(double lhs, double3 rhs)
		{
			return new bool3(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(double3 lhs, double3 rhs)
		{
			return new bool3(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(double3 lhs, double rhs)
		{
			return new bool3(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(double lhs, double3 rhs)
		{
			return new bool3(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z);
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
		public double4 xxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.x, this.z);
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
		public double4 xxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.z, this.z);
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
		public double4 xyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.x, this.z);
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
		public double4 xyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.z, this.z);
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
		public double4 yxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.x, this.z);
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
		public double4 yxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.z, this.z);
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
		public double4 yyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.x, this.z);
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
		public double4 yyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.z, this.z);
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
		public double3 xxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.x, this.z);
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
		public double3 xyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.y, this.z);
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
		public double3 xzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 xzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.z, this.y);
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
		public double3 xzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.z, this.z);
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
		public double3 yxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.x, this.z);
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
		public double3 yyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 yzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.z, this.x);
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
		public double3 yzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 yzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.x, this.y);
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
		public double3 zxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.y, this.x);
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
		public double3 zyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.z, this.z);
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
		public double2 xz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 yz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 zx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 zy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 zz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.z, this.z);
			}
		}

		public unsafe double this[int index]
		{
			get
			{
				fixed (double3* ptr = &this)
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
		public bool Equals(double3 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z;
		}

		public override bool Equals(object o)
		{
			if (o is double3)
			{
				double3 @double = (double3)o;
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
			return string.Format("double3({0}, {1}, {2})", this.x, this.y, this.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double3({0}, {1}, {2})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider), this.z.ToString(format, formatProvider));
		}

		public double x;

		public double y;

		public double z;

		public static readonly double3 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(double3 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
			}

			public double x;

			public double y;

			public double z;
		}
	}
}
