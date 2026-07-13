using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(double4.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double4 : IEquatable<double4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double x, double y, double2 zw)
		{
			this.x = x;
			this.y = y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double x, double2 yz, double w)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double x, double3 yzw)
		{
			this.x = x;
			this.y = yzw.x;
			this.z = yzw.y;
			this.w = yzw.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double2 xy, double z, double w)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double2 xy, double2 zw)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double3 xyz, double w)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double4 xyzw)
		{
			this.x = xyzw.x;
			this.y = xyzw.y;
			this.z = xyzw.z;
			this.w = xyzw.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(double v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(bool v)
		{
			this.x = (v ? 1.0 : 0.0);
			this.y = (v ? 1.0 : 0.0);
			this.z = (v ? 1.0 : 0.0);
			this.w = (v ? 1.0 : 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(bool4 v)
		{
			this.x = (v.x ? 1.0 : 0.0);
			this.y = (v.y ? 1.0 : 0.0);
			this.z = (v.z ? 1.0 : 0.0);
			this.w = (v.w ? 1.0 : 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(int v)
		{
			this.x = (double)v;
			this.y = (double)v;
			this.z = (double)v;
			this.w = (double)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(int4 v)
		{
			this.x = (double)v.x;
			this.y = (double)v.y;
			this.z = (double)v.z;
			this.w = (double)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(uint v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(uint4 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(half v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(half4 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(float v)
		{
			this.x = (double)v;
			this.y = (double)v;
			this.z = (double)v;
			this.w = (double)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4(float4 v)
		{
			this.x = (double)v.x;
			this.y = (double)v.y;
			this.z = (double)v.z;
			this.w = (double)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(double v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double4(bool v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double4(bool4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(int v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(int4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(uint v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(uint4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(half v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(half4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(float v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4(float4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator *(double4 lhs, double4 rhs)
		{
			return new double4(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator *(double4 lhs, double rhs)
		{
			return new double4(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator *(double lhs, double4 rhs)
		{
			return new double4(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z, lhs * rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator +(double4 lhs, double4 rhs)
		{
			return new double4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator +(double4 lhs, double rhs)
		{
			return new double4(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator +(double lhs, double4 rhs)
		{
			return new double4(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z, lhs + rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator -(double4 lhs, double4 rhs)
		{
			return new double4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator -(double4 lhs, double rhs)
		{
			return new double4(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs, lhs.w - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator -(double lhs, double4 rhs)
		{
			return new double4(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z, lhs - rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator /(double4 lhs, double4 rhs)
		{
			return new double4(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator /(double4 lhs, double rhs)
		{
			return new double4(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator /(double lhs, double4 rhs)
		{
			return new double4(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z, lhs / rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator %(double4 lhs, double4 rhs)
		{
			return new double4(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z, lhs.w % rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator %(double4 lhs, double rhs)
		{
			return new double4(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs, lhs.w % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator %(double lhs, double4 rhs)
		{
			return new double4(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z, lhs % rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator ++(double4 val)
		{
			double num = val.x + 1.0;
			val.x = num;
			double num2 = num;
			num = val.y + 1.0;
			val.y = num;
			double num3 = num;
			num = val.z + 1.0;
			val.z = num;
			double num4 = num;
			num = val.w + 1.0;
			val.w = num;
			return new double4(num2, num3, num4, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator --(double4 val)
		{
			double num = val.x - 1.0;
			val.x = num;
			double num2 = num;
			num = val.y - 1.0;
			val.y = num;
			double num3 = num;
			num = val.z - 1.0;
			val.z = num;
			double num4 = num;
			num = val.w - 1.0;
			val.w = num;
			return new double4(num2, num3, num4, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(double4 lhs, double4 rhs)
		{
			return new bool4(lhs.x < rhs.x, lhs.y < rhs.y, lhs.z < rhs.z, lhs.w < rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(double4 lhs, double rhs)
		{
			return new bool4(lhs.x < rhs, lhs.y < rhs, lhs.z < rhs, lhs.w < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(double lhs, double4 rhs)
		{
			return new bool4(lhs < rhs.x, lhs < rhs.y, lhs < rhs.z, lhs < rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(double4 lhs, double4 rhs)
		{
			return new bool4(lhs.x <= rhs.x, lhs.y <= rhs.y, lhs.z <= rhs.z, lhs.w <= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(double4 lhs, double rhs)
		{
			return new bool4(lhs.x <= rhs, lhs.y <= rhs, lhs.z <= rhs, lhs.w <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(double lhs, double4 rhs)
		{
			return new bool4(lhs <= rhs.x, lhs <= rhs.y, lhs <= rhs.z, lhs <= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(double4 lhs, double4 rhs)
		{
			return new bool4(lhs.x > rhs.x, lhs.y > rhs.y, lhs.z > rhs.z, lhs.w > rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(double4 lhs, double rhs)
		{
			return new bool4(lhs.x > rhs, lhs.y > rhs, lhs.z > rhs, lhs.w > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(double lhs, double4 rhs)
		{
			return new bool4(lhs > rhs.x, lhs > rhs.y, lhs > rhs.z, lhs > rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(double4 lhs, double4 rhs)
		{
			return new bool4(lhs.x >= rhs.x, lhs.y >= rhs.y, lhs.z >= rhs.z, lhs.w >= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(double4 lhs, double rhs)
		{
			return new bool4(lhs.x >= rhs, lhs.y >= rhs, lhs.z >= rhs, lhs.w >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(double lhs, double4 rhs)
		{
			return new bool4(lhs >= rhs.x, lhs >= rhs.y, lhs >= rhs.z, lhs >= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator -(double4 val)
		{
			return new double4(-val.x, -val.y, -val.z, -val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 operator +(double4 val)
		{
			return new double4(val.x, val.y, val.z, val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(double4 lhs, double4 rhs)
		{
			return new bool4(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z, lhs.w == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(double4 lhs, double rhs)
		{
			return new bool4(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs, lhs.w == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(double lhs, double4 rhs)
		{
			return new bool4(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z, lhs == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(double4 lhs, double4 rhs)
		{
			return new bool4(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z, lhs.w != rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(double4 lhs, double rhs)
		{
			return new bool4(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs, lhs.w != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(double lhs, double4 rhs)
		{
			return new bool4(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z, lhs != rhs.w);
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
		public double4 xxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.x, this.w);
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
		public double4 xxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.y, this.w);
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
		public double4 xxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.x, this.w, this.w);
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
		public double4 xyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.x, this.w);
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
		public double4 xyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.y, this.w);
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
		public double4 xyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.z, this.w);
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
		public double4 xywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.w, this.z);
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
		public double4 xyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.y, this.w, this.w);
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
		public double4 xzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.x, this.w);
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
		public double4 xzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.y, this.w);
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
		public double4 xzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.w, this.y);
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
		public double4 xzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.y, this.z);
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
		public double4 xwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.z, this.y);
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
		public double4 xwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 xwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.x, this.w, this.w, this.w);
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
		public double4 yxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.x, this.w);
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
		public double4 yxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.y, this.w);
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
		public double4 yxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.z, this.w);
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
		public double4 yxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.w, this.z);
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
		public double4 yxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.x, this.w, this.w);
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
		public double4 yyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.x, this.w);
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
		public double4 yyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.y, this.w);
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
		public double4 yyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.y, this.w, this.w);
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
		public double4 yzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.x, this.w);
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
		public double4 yzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.y, this.w);
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
		public double4 yzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.w, this.x);
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
		public double4 yzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 yzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.x, this.z);
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
		public double4 ywxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.z, this.x);
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
		public double4 ywzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 ywww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.y, this.w, this.w, this.w);
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
		public double4 zxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.x, this.w);
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
		public double4 zxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.y, this.w);
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
		public double4 zxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.w, this.y);
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
		public double4 zxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.x, this.w, this.w);
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
		public double4 zyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.x, this.w);
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
		public double4 zyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.y, this.w);
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
		public double4 zyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.w, this.x);
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
		public double4 zywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.y, this.w, this.w);
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
		public double4 zzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.x, this.w);
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
		public double4 zzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.y, this.w);
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
		public double4 zzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.x, this.y);
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
		public double4 zwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.y, this.x);
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
		public double4 zwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 zwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.z, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.y, this.z);
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
		public double4 wxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.z, this.y);
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
		public double4 wxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.x, this.z);
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
		public double4 wyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.z, this.x);
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
		public double4 wyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.x, this.y);
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
		public double4 wzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.y, this.x);
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
		public double4 wzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double4 wwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double4(this.w, this.w, this.w, this.w);
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
		public double3 xxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.x, this.w);
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
		public double3 xyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.y, this.w);
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
		public double3 xzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.z, this.w);
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
		public double3 xwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 xwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.w, this.y);
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
		public double3 xwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.w, this.z);
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
		public double3 xww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.x, this.w, this.w);
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
		public double3 yxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.x, this.w);
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
		public double3 yyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.y, this.w);
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
		public double3 yzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.z, this.w);
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
		public double3 ywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.w, this.x);
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
		public double3 ywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 ywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.w, this.z);
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
		public double3 yww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.y, this.w, this.w);
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
		public double3 zxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.x, this.w);
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
		public double3 zyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.y, this.w);
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
		public double3 zzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.w, this.x);
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
		public double3 zwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.w, this.y);
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
		public double3 zwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 zww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.x, this.y);
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
		public double3 wxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.x, this.z);
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
		public double3 wxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.y, this.x);
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
		public double3 wyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.y, this.z);
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
		public double3 wyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.z, this.x);
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
		public double3 wzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.z, this.y);
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
		public double3 wzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 wwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double3 www
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double3(this.w, this.w, this.w);
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
		public double2 xw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
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
		public double2 yw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 zw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 wx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 wy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 wz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public double2 ww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new double2(this.w, this.w);
			}
		}

		public unsafe double this[int index]
		{
			get
			{
				fixed (double4* ptr = &this)
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
		public bool Equals(double4 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z && this.w == rhs.w;
		}

		public override bool Equals(object o)
		{
			if (o is double4)
			{
				double4 @double = (double4)o;
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
			return string.Format("double4({0}, {1}, {2}, {3})", new object[] { this.x, this.y, this.z, this.w });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double4({0}, {1}, {2}, {3})", new object[]
			{
				this.x.ToString(format, formatProvider),
				this.y.ToString(format, formatProvider),
				this.z.ToString(format, formatProvider),
				this.w.ToString(format, formatProvider)
			});
		}

		public double x;

		public double y;

		public double z;

		public double w;

		public static readonly double4 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(double4 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
				this.w = v.w;
			}

			public double x;

			public double y;

			public double z;

			public double w;
		}
	}
}
