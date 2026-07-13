using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double4x3 : IEquatable<double4x3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(double4 c0, double4 c1, double4 c2)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(double m00, double m01, double m02, double m10, double m11, double m12, double m20, double m21, double m22, double m30, double m31, double m32)
		{
			this.c0 = new double4(m00, m10, m20, m30);
			this.c1 = new double4(m01, m11, m21, m31);
			this.c2 = new double4(m02, m12, m22, m32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(double v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(bool v)
		{
			this.c0 = math.select(new double4(0.0), new double4(1.0), v);
			this.c1 = math.select(new double4(0.0), new double4(1.0), v);
			this.c2 = math.select(new double4(0.0), new double4(1.0), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(bool4x3 v)
		{
			this.c0 = math.select(new double4(0.0), new double4(1.0), v.c0);
			this.c1 = math.select(new double4(0.0), new double4(1.0), v.c1);
			this.c2 = math.select(new double4(0.0), new double4(1.0), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(int4x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(uint4x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4x3(float4x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4x3(double v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double4x3(bool v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double4x3(bool4x3 v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4x3(int v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4x3(int4x3 v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4x3(uint v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4x3(uint4x3 v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4x3(float v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double4x3(float4x3 v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator *(double4x3 lhs, double4x3 rhs)
		{
			return new double4x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator *(double4x3 lhs, double rhs)
		{
			return new double4x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator *(double lhs, double4x3 rhs)
		{
			return new double4x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator +(double4x3 lhs, double4x3 rhs)
		{
			return new double4x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator +(double4x3 lhs, double rhs)
		{
			return new double4x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator +(double lhs, double4x3 rhs)
		{
			return new double4x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator -(double4x3 lhs, double4x3 rhs)
		{
			return new double4x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator -(double4x3 lhs, double rhs)
		{
			return new double4x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator -(double lhs, double4x3 rhs)
		{
			return new double4x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator /(double4x3 lhs, double4x3 rhs)
		{
			return new double4x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator /(double4x3 lhs, double rhs)
		{
			return new double4x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator /(double lhs, double4x3 rhs)
		{
			return new double4x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator %(double4x3 lhs, double4x3 rhs)
		{
			return new double4x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator %(double4x3 lhs, double rhs)
		{
			return new double4x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator %(double lhs, double4x3 rhs)
		{
			return new double4x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator ++(double4x3 val)
		{
			double4 @double = double4.op_Increment(val.c0);
			val.c0 = @double;
			double4 double2 = @double;
			@double = double4.op_Increment(val.c1);
			val.c1 = @double;
			double4 double3 = @double;
			@double = double4.op_Increment(val.c2);
			val.c2 = @double;
			return new double4x3(double2, double3, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator --(double4x3 val)
		{
			double4 @double = double4.op_Decrement(val.c0);
			val.c0 = @double;
			double4 double2 = @double;
			@double = double4.op_Decrement(val.c1);
			val.c1 = @double;
			double4 double3 = @double;
			@double = double4.op_Decrement(val.c2);
			val.c2 = @double;
			return new double4x3(double2, double3, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <(double4x3 lhs, double4x3 rhs)
		{
			return new bool4x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <(double4x3 lhs, double rhs)
		{
			return new bool4x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <(double lhs, double4x3 rhs)
		{
			return new bool4x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <=(double4x3 lhs, double4x3 rhs)
		{
			return new bool4x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <=(double4x3 lhs, double rhs)
		{
			return new bool4x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <=(double lhs, double4x3 rhs)
		{
			return new bool4x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >(double4x3 lhs, double4x3 rhs)
		{
			return new bool4x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >(double4x3 lhs, double rhs)
		{
			return new bool4x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >(double lhs, double4x3 rhs)
		{
			return new bool4x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >=(double4x3 lhs, double4x3 rhs)
		{
			return new bool4x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >=(double4x3 lhs, double rhs)
		{
			return new bool4x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >=(double lhs, double4x3 rhs)
		{
			return new bool4x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator -(double4x3 val)
		{
			return new double4x3(-val.c0, -val.c1, -val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 operator +(double4x3 val)
		{
			return new double4x3(+val.c0, +val.c1, +val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator ==(double4x3 lhs, double4x3 rhs)
		{
			return new bool4x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator ==(double4x3 lhs, double rhs)
		{
			return new bool4x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator ==(double lhs, double4x3 rhs)
		{
			return new bool4x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator !=(double4x3 lhs, double4x3 rhs)
		{
			return new bool4x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator !=(double4x3 lhs, double rhs)
		{
			return new bool4x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator !=(double lhs, double4x3 rhs)
		{
			return new bool4x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
		}

		public unsafe ref double4 this[int index]
		{
			get
			{
				fixed (double4x3* ptr = &this)
				{
					return ref *(double4*)(ptr + (IntPtr)index * (IntPtr)sizeof(double4) / (IntPtr)sizeof(double4x3));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(double4x3 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2);
		}

		public override bool Equals(object o)
		{
			if (o is double4x3)
			{
				double4x3 double4x = (double4x3)o;
				return this.Equals(double4x);
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
			return string.Format("double4x3({0}, {1}, {2},  {3}, {4}, {5},  {6}, {7}, {8},  {9}, {10}, {11})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c0.y,
				this.c1.y,
				this.c2.y,
				this.c0.z,
				this.c1.z,
				this.c2.z,
				this.c0.w,
				this.c1.w,
				this.c2.w
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double4x3({0}, {1}, {2},  {3}, {4}, {5},  {6}, {7}, {8},  {9}, {10}, {11})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider),
				this.c2.z.ToString(format, formatProvider),
				this.c0.w.ToString(format, formatProvider),
				this.c1.w.ToString(format, formatProvider),
				this.c2.w.ToString(format, formatProvider)
			});
		}

		public double4 c0;

		public double4 c1;

		public double4 c2;

		public static readonly double4x3 zero;
	}
}
