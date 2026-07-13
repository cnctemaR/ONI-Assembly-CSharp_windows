using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double2x4 : IEquatable<double2x4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(double2 c0, double2 c1, double2 c2, double2 c3)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
			this.c3 = c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(double m00, double m01, double m02, double m03, double m10, double m11, double m12, double m13)
		{
			this.c0 = new double2(m00, m10);
			this.c1 = new double2(m01, m11);
			this.c2 = new double2(m02, m12);
			this.c3 = new double2(m03, m13);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(double v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(bool v)
		{
			this.c0 = math.select(new double2(0.0), new double2(1.0), v);
			this.c1 = math.select(new double2(0.0), new double2(1.0), v);
			this.c2 = math.select(new double2(0.0), new double2(1.0), v);
			this.c3 = math.select(new double2(0.0), new double2(1.0), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(bool2x4 v)
		{
			this.c0 = math.select(new double2(0.0), new double2(1.0), v.c0);
			this.c1 = math.select(new double2(0.0), new double2(1.0), v.c1);
			this.c2 = math.select(new double2(0.0), new double2(1.0), v.c2);
			this.c3 = math.select(new double2(0.0), new double2(1.0), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(int2x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(uint2x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x4(float2x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x4(double v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double2x4(bool v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double2x4(bool2x4 v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x4(int v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x4(int2x4 v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x4(uint v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x4(uint2x4 v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x4(float v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x4(float2x4 v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator *(double2x4 lhs, double2x4 rhs)
		{
			return new double2x4(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2, lhs.c3 * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator *(double2x4 lhs, double rhs)
		{
			return new double2x4(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs, lhs.c3 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator *(double lhs, double2x4 rhs)
		{
			return new double2x4(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2, lhs * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator +(double2x4 lhs, double2x4 rhs)
		{
			return new double2x4(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2, lhs.c3 + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator +(double2x4 lhs, double rhs)
		{
			return new double2x4(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs, lhs.c3 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator +(double lhs, double2x4 rhs)
		{
			return new double2x4(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2, lhs + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator -(double2x4 lhs, double2x4 rhs)
		{
			return new double2x4(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2, lhs.c3 - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator -(double2x4 lhs, double rhs)
		{
			return new double2x4(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs, lhs.c3 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator -(double lhs, double2x4 rhs)
		{
			return new double2x4(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2, lhs - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator /(double2x4 lhs, double2x4 rhs)
		{
			return new double2x4(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2, lhs.c3 / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator /(double2x4 lhs, double rhs)
		{
			return new double2x4(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs, lhs.c3 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator /(double lhs, double2x4 rhs)
		{
			return new double2x4(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2, lhs / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator %(double2x4 lhs, double2x4 rhs)
		{
			return new double2x4(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2, lhs.c3 % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator %(double2x4 lhs, double rhs)
		{
			return new double2x4(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs, lhs.c3 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator %(double lhs, double2x4 rhs)
		{
			return new double2x4(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2, lhs % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator ++(double2x4 val)
		{
			double2 @double = double2.op_Increment(val.c0);
			val.c0 = @double;
			double2 double2 = @double;
			@double = double2.op_Increment(val.c1);
			val.c1 = @double;
			double2 double3 = @double;
			@double = double2.op_Increment(val.c2);
			val.c2 = @double;
			double2 double4 = @double;
			@double = double2.op_Increment(val.c3);
			val.c3 = @double;
			return new double2x4(double2, double3, double4, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator --(double2x4 val)
		{
			double2 @double = double2.op_Decrement(val.c0);
			val.c0 = @double;
			double2 double2 = @double;
			@double = double2.op_Decrement(val.c1);
			val.c1 = @double;
			double2 double3 = @double;
			@double = double2.op_Decrement(val.c2);
			val.c2 = @double;
			double2 double4 = @double;
			@double = double2.op_Decrement(val.c3);
			val.c3 = @double;
			return new double2x4(double2, double3, double4, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <(double2x4 lhs, double2x4 rhs)
		{
			return new bool2x4(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2, lhs.c3 < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <(double2x4 lhs, double rhs)
		{
			return new bool2x4(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs, lhs.c3 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <(double lhs, double2x4 rhs)
		{
			return new bool2x4(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2, lhs < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <=(double2x4 lhs, double2x4 rhs)
		{
			return new bool2x4(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2, lhs.c3 <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <=(double2x4 lhs, double rhs)
		{
			return new bool2x4(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs, lhs.c3 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator <=(double lhs, double2x4 rhs)
		{
			return new bool2x4(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2, lhs <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >(double2x4 lhs, double2x4 rhs)
		{
			return new bool2x4(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2, lhs.c3 > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >(double2x4 lhs, double rhs)
		{
			return new bool2x4(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs, lhs.c3 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >(double lhs, double2x4 rhs)
		{
			return new bool2x4(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2, lhs > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >=(double2x4 lhs, double2x4 rhs)
		{
			return new bool2x4(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2, lhs.c3 >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >=(double2x4 lhs, double rhs)
		{
			return new bool2x4(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs, lhs.c3 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator >=(double lhs, double2x4 rhs)
		{
			return new bool2x4(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2, lhs >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator -(double2x4 val)
		{
			return new double2x4(-val.c0, -val.c1, -val.c2, -val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 operator +(double2x4 val)
		{
			return new double2x4(+val.c0, +val.c1, +val.c2, +val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator ==(double2x4 lhs, double2x4 rhs)
		{
			return new bool2x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator ==(double2x4 lhs, double rhs)
		{
			return new bool2x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator ==(double lhs, double2x4 rhs)
		{
			return new bool2x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator !=(double2x4 lhs, double2x4 rhs)
		{
			return new bool2x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator !=(double2x4 lhs, double rhs)
		{
			return new bool2x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 operator !=(double lhs, double2x4 rhs)
		{
			return new bool2x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
		}

		public unsafe ref double2 this[int index]
		{
			get
			{
				fixed (double2x4* ptr = &this)
				{
					return ref *(double2*)(ptr + (IntPtr)index * (IntPtr)sizeof(double2) / (IntPtr)sizeof(double2x4));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(double2x4 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2) && this.c3.Equals(rhs.c3);
		}

		public override bool Equals(object o)
		{
			if (o is double2x4)
			{
				double2x4 double2x = (double2x4)o;
				return this.Equals(double2x);
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
			return string.Format("double2x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c3.x,
				this.c0.y,
				this.c1.y,
				this.c2.y,
				this.c3.y
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double2x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c3.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider),
				this.c3.y.ToString(format, formatProvider)
			});
		}

		public double2 c0;

		public double2 c1;

		public double2 c2;

		public double2 c3;

		public static readonly double2x4 zero;
	}
}
