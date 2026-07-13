using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double2x2 : IEquatable<double2x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(double2 c0, double2 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(double m00, double m01, double m10, double m11)
		{
			this.c0 = new double2(m00, m10);
			this.c1 = new double2(m01, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(double v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(bool v)
		{
			this.c0 = math.select(new double2(0.0), new double2(1.0), v);
			this.c1 = math.select(new double2(0.0), new double2(1.0), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(bool2x2 v)
		{
			this.c0 = math.select(new double2(0.0), new double2(1.0), v.c0);
			this.c1 = math.select(new double2(0.0), new double2(1.0), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(int v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(int2x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(uint v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(uint2x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(float v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2x2(float2x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x2(double v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double2x2(bool v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double2x2(bool2x2 v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x2(int v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x2(int2x2 v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x2(uint v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x2(uint2x2 v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x2(float v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double2x2(float2x2 v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator *(double2x2 lhs, double2x2 rhs)
		{
			return new double2x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator *(double2x2 lhs, double rhs)
		{
			return new double2x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator *(double lhs, double2x2 rhs)
		{
			return new double2x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator +(double2x2 lhs, double2x2 rhs)
		{
			return new double2x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator +(double2x2 lhs, double rhs)
		{
			return new double2x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator +(double lhs, double2x2 rhs)
		{
			return new double2x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator -(double2x2 lhs, double2x2 rhs)
		{
			return new double2x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator -(double2x2 lhs, double rhs)
		{
			return new double2x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator -(double lhs, double2x2 rhs)
		{
			return new double2x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator /(double2x2 lhs, double2x2 rhs)
		{
			return new double2x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator /(double2x2 lhs, double rhs)
		{
			return new double2x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator /(double lhs, double2x2 rhs)
		{
			return new double2x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator %(double2x2 lhs, double2x2 rhs)
		{
			return new double2x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator %(double2x2 lhs, double rhs)
		{
			return new double2x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator %(double lhs, double2x2 rhs)
		{
			return new double2x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator ++(double2x2 val)
		{
			double2 @double = double2.op_Increment(val.c0);
			val.c0 = @double;
			double2 double2 = @double;
			@double = double2.op_Increment(val.c1);
			val.c1 = @double;
			return new double2x2(double2, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator --(double2x2 val)
		{
			double2 @double = double2.op_Decrement(val.c0);
			val.c0 = @double;
			double2 double2 = @double;
			@double = double2.op_Decrement(val.c1);
			val.c1 = @double;
			return new double2x2(double2, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(double2x2 lhs, double2x2 rhs)
		{
			return new bool2x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(double2x2 lhs, double rhs)
		{
			return new bool2x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(double lhs, double2x2 rhs)
		{
			return new bool2x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(double2x2 lhs, double2x2 rhs)
		{
			return new bool2x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(double2x2 lhs, double rhs)
		{
			return new bool2x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(double lhs, double2x2 rhs)
		{
			return new bool2x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(double2x2 lhs, double2x2 rhs)
		{
			return new bool2x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(double2x2 lhs, double rhs)
		{
			return new bool2x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(double lhs, double2x2 rhs)
		{
			return new bool2x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(double2x2 lhs, double2x2 rhs)
		{
			return new bool2x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(double2x2 lhs, double rhs)
		{
			return new bool2x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(double lhs, double2x2 rhs)
		{
			return new bool2x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator -(double2x2 val)
		{
			return new double2x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 operator +(double2x2 val)
		{
			return new double2x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(double2x2 lhs, double2x2 rhs)
		{
			return new bool2x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(double2x2 lhs, double rhs)
		{
			return new bool2x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(double lhs, double2x2 rhs)
		{
			return new bool2x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(double2x2 lhs, double2x2 rhs)
		{
			return new bool2x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(double2x2 lhs, double rhs)
		{
			return new bool2x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(double lhs, double2x2 rhs)
		{
			return new bool2x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		public unsafe ref double2 this[int index]
		{
			get
			{
				fixed (double2x2* ptr = &this)
				{
					return ref *(double2*)(ptr + (IntPtr)index * (IntPtr)sizeof(double2) / (IntPtr)sizeof(double2x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(double2x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is double2x2)
			{
				double2x2 double2x = (double2x2)o;
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
			return string.Format("double2x2({0}, {1},  {2}, {3})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double2x2({0}, {1},  {2}, {3})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider)
			});
		}

		public double2 c0;

		public double2 c1;

		public static readonly double2x2 identity = new double2x2(1.0, 0.0, 0.0, 1.0);

		public static readonly double2x2 zero;
	}
}
