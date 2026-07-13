using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct double3x2 : IEquatable<double3x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(double3 c0, double3 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(double m00, double m01, double m10, double m11, double m20, double m21)
		{
			this.c0 = new double3(m00, m10, m20);
			this.c1 = new double3(m01, m11, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(double v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(bool v)
		{
			this.c0 = math.select(new double3(0.0), new double3(1.0), v);
			this.c1 = math.select(new double3(0.0), new double3(1.0), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(bool3x2 v)
		{
			this.c0 = math.select(new double3(0.0), new double3(1.0), v.c0);
			this.c1 = math.select(new double3(0.0), new double3(1.0), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(int v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(int3x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(uint v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(uint3x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(float v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3x2(float3x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x2(double v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double3x2(bool v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator double3x2(bool3x2 v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x2(int v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x2(int3x2 v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x2(uint v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x2(uint3x2 v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x2(float v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double3x2(float3x2 v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator *(double3x2 lhs, double3x2 rhs)
		{
			return new double3x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator *(double3x2 lhs, double rhs)
		{
			return new double3x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator *(double lhs, double3x2 rhs)
		{
			return new double3x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator +(double3x2 lhs, double3x2 rhs)
		{
			return new double3x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator +(double3x2 lhs, double rhs)
		{
			return new double3x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator +(double lhs, double3x2 rhs)
		{
			return new double3x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator -(double3x2 lhs, double3x2 rhs)
		{
			return new double3x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator -(double3x2 lhs, double rhs)
		{
			return new double3x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator -(double lhs, double3x2 rhs)
		{
			return new double3x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator /(double3x2 lhs, double3x2 rhs)
		{
			return new double3x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator /(double3x2 lhs, double rhs)
		{
			return new double3x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator /(double lhs, double3x2 rhs)
		{
			return new double3x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator %(double3x2 lhs, double3x2 rhs)
		{
			return new double3x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator %(double3x2 lhs, double rhs)
		{
			return new double3x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator %(double lhs, double3x2 rhs)
		{
			return new double3x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator ++(double3x2 val)
		{
			double3 @double = double3.op_Increment(val.c0);
			val.c0 = @double;
			double3 double2 = @double;
			@double = double3.op_Increment(val.c1);
			val.c1 = @double;
			return new double3x2(double2, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator --(double3x2 val)
		{
			double3 @double = double3.op_Decrement(val.c0);
			val.c0 = @double;
			double3 double2 = @double;
			@double = double3.op_Decrement(val.c1);
			val.c1 = @double;
			return new double3x2(double2, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(double3x2 lhs, double3x2 rhs)
		{
			return new bool3x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(double3x2 lhs, double rhs)
		{
			return new bool3x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(double lhs, double3x2 rhs)
		{
			return new bool3x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(double3x2 lhs, double3x2 rhs)
		{
			return new bool3x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(double3x2 lhs, double rhs)
		{
			return new bool3x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(double lhs, double3x2 rhs)
		{
			return new bool3x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(double3x2 lhs, double3x2 rhs)
		{
			return new bool3x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(double3x2 lhs, double rhs)
		{
			return new bool3x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(double lhs, double3x2 rhs)
		{
			return new bool3x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(double3x2 lhs, double3x2 rhs)
		{
			return new bool3x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(double3x2 lhs, double rhs)
		{
			return new bool3x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(double lhs, double3x2 rhs)
		{
			return new bool3x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator -(double3x2 val)
		{
			return new double3x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 operator +(double3x2 val)
		{
			return new double3x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(double3x2 lhs, double3x2 rhs)
		{
			return new bool3x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(double3x2 lhs, double rhs)
		{
			return new bool3x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(double lhs, double3x2 rhs)
		{
			return new bool3x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(double3x2 lhs, double3x2 rhs)
		{
			return new bool3x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(double3x2 lhs, double rhs)
		{
			return new bool3x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(double lhs, double3x2 rhs)
		{
			return new bool3x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		public unsafe ref double3 this[int index]
		{
			get
			{
				fixed (double3x2* ptr = &this)
				{
					return ref *(double3*)(ptr + (IntPtr)index * (IntPtr)sizeof(double3) / (IntPtr)sizeof(double3x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(double3x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is double3x2)
			{
				double3x2 double3x = (double3x2)o;
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
			return string.Format("double3x2({0}, {1},  {2}, {3},  {4}, {5})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y,
				this.c0.z,
				this.c1.z
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("double3x2({0}, {1},  {2}, {3},  {4}, {5})", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider)
			});
		}

		public double3 c0;

		public double3 c1;

		public static readonly double3x2 zero;
	}
}
