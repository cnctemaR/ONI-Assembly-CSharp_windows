using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double3x4 : IEquatable<double3x4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(double3 c0, double3 c1, double3 c2, double3 c3)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
			this.c3 = c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(double m00, double m01, double m02, double m03, double m10, double m11, double m12, double m13, double m20, double m21, double m22, double m23)
		{
			this.c0 = new double3(m00, m10, m20);
			this.c1 = new double3(m01, m11, m21);
			this.c2 = new double3(m02, m12, m22);
			this.c3 = new double3(m03, m13, m23);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(double v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(bool v)
		{
			this.c0 = math.select(new double3(0.0), new double3(1.0), v);
			this.c1 = math.select(new double3(0.0), new double3(1.0), v);
			this.c2 = math.select(new double3(0.0), new double3(1.0), v);
			this.c3 = math.select(new double3(0.0), new double3(1.0), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(bool3x4 v)
		{
			this.c0 = math.select(new double3(0.0), new double3(1.0), v.c0);
			this.c1 = math.select(new double3(0.0), new double3(1.0), v.c1);
			this.c2 = math.select(new double3(0.0), new double3(1.0), v.c2);
			this.c3 = math.select(new double3(0.0), new double3(1.0), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(int3x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(uint3x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x4(float3x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x4(double v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double3x4(bool v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double3x4(bool3x4 v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x4(int v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x4(int3x4 v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x4(uint v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x4(uint3x4 v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x4(float v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x4(float3x4 v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator *(double3x4 lhs, double3x4 rhs)
		{
			return new double3x4(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2, lhs.c3 * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator *(double3x4 lhs, double rhs)
		{
			return new double3x4(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs, lhs.c3 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator *(double lhs, double3x4 rhs)
		{
			return new double3x4(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2, lhs * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator +(double3x4 lhs, double3x4 rhs)
		{
			return new double3x4(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2, lhs.c3 + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator +(double3x4 lhs, double rhs)
		{
			return new double3x4(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs, lhs.c3 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator +(double lhs, double3x4 rhs)
		{
			return new double3x4(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2, lhs + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator -(double3x4 lhs, double3x4 rhs)
		{
			return new double3x4(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2, lhs.c3 - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator -(double3x4 lhs, double rhs)
		{
			return new double3x4(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs, lhs.c3 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator -(double lhs, double3x4 rhs)
		{
			return new double3x4(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2, lhs - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator /(double3x4 lhs, double3x4 rhs)
		{
			return new double3x4(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2, lhs.c3 / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator /(double3x4 lhs, double rhs)
		{
			return new double3x4(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs, lhs.c3 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator /(double lhs, double3x4 rhs)
		{
			return new double3x4(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2, lhs / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator %(double3x4 lhs, double3x4 rhs)
		{
			return new double3x4(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2, lhs.c3 % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator %(double3x4 lhs, double rhs)
		{
			return new double3x4(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs, lhs.c3 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator %(double lhs, double3x4 rhs)
		{
			return new double3x4(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2, lhs % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator ++(double3x4 val)
		{
			double3 @double = double3.op_Increment(val.c0);
			val.c0 = @double;
			double3 double2 = @double;
			@double = double3.op_Increment(val.c1);
			val.c1 = @double;
			double3 double3 = @double;
			@double = double3.op_Increment(val.c2);
			val.c2 = @double;
			double3 double4 = @double;
			@double = double3.op_Increment(val.c3);
			val.c3 = @double;
			return new double3x4(double2, double3, double4, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator --(double3x4 val)
		{
			double3 @double = double3.op_Decrement(val.c0);
			val.c0 = @double;
			double3 double2 = @double;
			@double = double3.op_Decrement(val.c1);
			val.c1 = @double;
			double3 double3 = @double;
			@double = double3.op_Decrement(val.c2);
			val.c2 = @double;
			double3 double4 = @double;
			@double = double3.op_Decrement(val.c3);
			val.c3 = @double;
			return new double3x4(double2, double3, double4, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <(double3x4 lhs, double3x4 rhs)
		{
			return new bool3x4(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2, lhs.c3 < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <(double3x4 lhs, double rhs)
		{
			return new bool3x4(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs, lhs.c3 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <(double lhs, double3x4 rhs)
		{
			return new bool3x4(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2, lhs < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <=(double3x4 lhs, double3x4 rhs)
		{
			return new bool3x4(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2, lhs.c3 <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <=(double3x4 lhs, double rhs)
		{
			return new bool3x4(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs, lhs.c3 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <=(double lhs, double3x4 rhs)
		{
			return new bool3x4(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2, lhs <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >(double3x4 lhs, double3x4 rhs)
		{
			return new bool3x4(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2, lhs.c3 > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >(double3x4 lhs, double rhs)
		{
			return new bool3x4(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs, lhs.c3 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >(double lhs, double3x4 rhs)
		{
			return new bool3x4(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2, lhs > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >=(double3x4 lhs, double3x4 rhs)
		{
			return new bool3x4(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2, lhs.c3 >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >=(double3x4 lhs, double rhs)
		{
			return new bool3x4(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs, lhs.c3 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >=(double lhs, double3x4 rhs)
		{
			return new bool3x4(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2, lhs >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator -(double3x4 val)
		{
			return new double3x4(-val.c0, -val.c1, -val.c2, -val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 operator +(double3x4 val)
		{
			return new double3x4(+val.c0, +val.c1, +val.c2, +val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(double3x4 lhs, double3x4 rhs)
		{
			return new bool3x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(double3x4 lhs, double rhs)
		{
			return new bool3x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(double lhs, double3x4 rhs)
		{
			return new bool3x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(double3x4 lhs, double3x4 rhs)
		{
			return new bool3x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(double3x4 lhs, double rhs)
		{
			return new bool3x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(double lhs, double3x4 rhs)
		{
			return new bool3x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
		}

		public unsafe ref double3 this[int index]
		{
			get
			{
				fixed (double3x4* ptr = &this)
				{
					return ref *(double3*)(ptr + (IntPtr)index * (IntPtr)sizeof(double3) / (IntPtr)sizeof(double3x4));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(double3x4 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2) && this.c3.Equals(rhs.c3);
		}

		public override bool Equals(object o)
		{
			if (o is double3x4)
			{
				double3x4 double3x = (double3x4)o;
				return this.Equals(double3x);
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
			return string.Format("double3x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7},  {8}, {9}, {10}, {11})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c3.x,
				this.c0.y,
				this.c1.y,
				this.c2.y,
				this.c3.y,
				this.c0.z,
				this.c1.z,
				this.c2.z,
				this.c3.z
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double3x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7},  {8}, {9}, {10}, {11})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c3.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider),
				this.c3.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider),
				this.c2.z.ToString(format, formatProvider),
				this.c3.z.ToString(format, formatProvider)
			});
		}

		public double3 c0;

		public double3 c1;

		public double3 c2;

		public double3 c3;

		public static readonly double3x4 zero;
	}
}
