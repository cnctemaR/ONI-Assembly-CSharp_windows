using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double2x3 : IEquatable<double2x3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(double2 c0, double2 c1, double2 c2)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(double m00, double m01, double m02, double m10, double m11, double m12)
		{
			this.c0 = new double2(m00, m10);
			this.c1 = new double2(m01, m11);
			this.c2 = new double2(m02, m12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(double v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(bool v)
		{
			this.c0 = math.select(new double2(0.0), new double2(1.0), v);
			this.c1 = math.select(new double2(0.0), new double2(1.0), v);
			this.c2 = math.select(new double2(0.0), new double2(1.0), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(bool2x3 v)
		{
			this.c0 = math.select(new double2(0.0), new double2(1.0), v.c0);
			this.c1 = math.select(new double2(0.0), new double2(1.0), v.c1);
			this.c2 = math.select(new double2(0.0), new double2(1.0), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(int2x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(uint2x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x3(float2x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x3(double v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double2x3(bool v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double2x3(bool2x3 v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x3(int v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x3(int2x3 v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x3(uint v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x3(uint2x3 v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x3(float v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x3(float2x3 v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator *(double2x3 lhs, double2x3 rhs)
		{
			return new double2x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator *(double2x3 lhs, double rhs)
		{
			return new double2x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator *(double lhs, double2x3 rhs)
		{
			return new double2x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator +(double2x3 lhs, double2x3 rhs)
		{
			return new double2x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator +(double2x3 lhs, double rhs)
		{
			return new double2x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator +(double lhs, double2x3 rhs)
		{
			return new double2x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator -(double2x3 lhs, double2x3 rhs)
		{
			return new double2x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator -(double2x3 lhs, double rhs)
		{
			return new double2x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator -(double lhs, double2x3 rhs)
		{
			return new double2x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator /(double2x3 lhs, double2x3 rhs)
		{
			return new double2x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator /(double2x3 lhs, double rhs)
		{
			return new double2x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator /(double lhs, double2x3 rhs)
		{
			return new double2x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator %(double2x3 lhs, double2x3 rhs)
		{
			return new double2x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator %(double2x3 lhs, double rhs)
		{
			return new double2x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator %(double lhs, double2x3 rhs)
		{
			return new double2x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator ++(double2x3 val)
		{
			double2 @double = double2.op_Increment(val.c0);
			val.c0 = @double;
			double2 double2 = @double;
			@double = double2.op_Increment(val.c1);
			val.c1 = @double;
			double2 double3 = @double;
			@double = double2.op_Increment(val.c2);
			val.c2 = @double;
			return new double2x3(double2, double3, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator --(double2x3 val)
		{
			double2 @double = double2.op_Decrement(val.c0);
			val.c0 = @double;
			double2 double2 = @double;
			@double = double2.op_Decrement(val.c1);
			val.c1 = @double;
			double2 double3 = @double;
			@double = double2.op_Decrement(val.c2);
			val.c2 = @double;
			return new double2x3(double2, double3, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(double2x3 lhs, double2x3 rhs)
		{
			return new bool2x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(double2x3 lhs, double rhs)
		{
			return new bool2x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(double lhs, double2x3 rhs)
		{
			return new bool2x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(double2x3 lhs, double2x3 rhs)
		{
			return new bool2x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(double2x3 lhs, double rhs)
		{
			return new bool2x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(double lhs, double2x3 rhs)
		{
			return new bool2x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(double2x3 lhs, double2x3 rhs)
		{
			return new bool2x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(double2x3 lhs, double rhs)
		{
			return new bool2x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(double lhs, double2x3 rhs)
		{
			return new bool2x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(double2x3 lhs, double2x3 rhs)
		{
			return new bool2x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(double2x3 lhs, double rhs)
		{
			return new bool2x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(double lhs, double2x3 rhs)
		{
			return new bool2x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator -(double2x3 val)
		{
			return new double2x3(-val.c0, -val.c1, -val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 operator +(double2x3 val)
		{
			return new double2x3(+val.c0, +val.c1, +val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(double2x3 lhs, double2x3 rhs)
		{
			return new bool2x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(double2x3 lhs, double rhs)
		{
			return new bool2x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(double lhs, double2x3 rhs)
		{
			return new bool2x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(double2x3 lhs, double2x3 rhs)
		{
			return new bool2x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(double2x3 lhs, double rhs)
		{
			return new bool2x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(double lhs, double2x3 rhs)
		{
			return new bool2x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
		}

		public unsafe ref double2 this[int index]
		{
			get
			{
				fixed (double2x3* ptr = &this)
				{
					return ref *(double2*)(ptr + (IntPtr)index * (IntPtr)sizeof(double2) / (IntPtr)sizeof(double2x3));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(double2x3 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2);
		}

		public override bool Equals(object o)
		{
			if (o is double2x3)
			{
				double2x3 double2x = (double2x3)o;
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
			return string.Format("double2x3({0}, {1}, {2},  {3}, {4}, {5})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c0.y,
				this.c1.y,
				this.c2.y
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double2x3({0}, {1}, {2},  {3}, {4}, {5})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider)
			});
		}

		public double2 c0;

		public double2 c1;

		public double2 c2;

		public static readonly double2x3 zero;
	}
}
