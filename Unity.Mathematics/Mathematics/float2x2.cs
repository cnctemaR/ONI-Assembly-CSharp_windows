using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float2x2 : IEquatable<float2x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(float2 c0, float2 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(float m00, float m01, float m10, float m11)
		{
			this.c0 = new float2(m00, m10);
			this.c1 = new float2(m01, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(float v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(bool v)
		{
			this.c0 = math.select(new float2(0f), new float2(1f), v);
			this.c1 = math.select(new float2(0f), new float2(1f), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(bool2x2 v)
		{
			this.c0 = math.select(new float2(0f), new float2(1f), v.c0);
			this.c1 = math.select(new float2(0f), new float2(1f), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(int v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(int2x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(uint v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(uint2x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(double v)
		{
			this.c0 = (float2)v;
			this.c1 = (float2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x2(double2x2 v)
		{
			this.c0 = (float2)v.c0;
			this.c1 = (float2)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x2(float v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2x2(bool v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2x2(bool2x2 v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x2(int v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x2(int2x2 v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x2(uint v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x2(uint2x2 v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2x2(double v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2x2(double2x2 v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator *(float2x2 lhs, float2x2 rhs)
		{
			return new float2x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator *(float2x2 lhs, float rhs)
		{
			return new float2x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator *(float lhs, float2x2 rhs)
		{
			return new float2x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator +(float2x2 lhs, float2x2 rhs)
		{
			return new float2x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator +(float2x2 lhs, float rhs)
		{
			return new float2x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator +(float lhs, float2x2 rhs)
		{
			return new float2x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator -(float2x2 lhs, float2x2 rhs)
		{
			return new float2x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator -(float2x2 lhs, float rhs)
		{
			return new float2x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator -(float lhs, float2x2 rhs)
		{
			return new float2x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator /(float2x2 lhs, float2x2 rhs)
		{
			return new float2x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator /(float2x2 lhs, float rhs)
		{
			return new float2x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator /(float lhs, float2x2 rhs)
		{
			return new float2x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator %(float2x2 lhs, float2x2 rhs)
		{
			return new float2x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator %(float2x2 lhs, float rhs)
		{
			return new float2x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator %(float lhs, float2x2 rhs)
		{
			return new float2x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator ++(float2x2 val)
		{
			float2 @float = float2.op_Increment(val.c0);
			val.c0 = @float;
			float2 float2 = @float;
			@float = float2.op_Increment(val.c1);
			val.c1 = @float;
			return new float2x2(float2, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator --(float2x2 val)
		{
			float2 @float = float2.op_Decrement(val.c0);
			val.c0 = @float;
			float2 float2 = @float;
			@float = float2.op_Decrement(val.c1);
			val.c1 = @float;
			return new float2x2(float2, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(float2x2 lhs, float2x2 rhs)
		{
			return new bool2x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(float2x2 lhs, float rhs)
		{
			return new bool2x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <(float lhs, float2x2 rhs)
		{
			return new bool2x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(float2x2 lhs, float2x2 rhs)
		{
			return new bool2x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(float2x2 lhs, float rhs)
		{
			return new bool2x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator <=(float lhs, float2x2 rhs)
		{
			return new bool2x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(float2x2 lhs, float2x2 rhs)
		{
			return new bool2x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(float2x2 lhs, float rhs)
		{
			return new bool2x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >(float lhs, float2x2 rhs)
		{
			return new bool2x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(float2x2 lhs, float2x2 rhs)
		{
			return new bool2x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(float2x2 lhs, float rhs)
		{
			return new bool2x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator >=(float lhs, float2x2 rhs)
		{
			return new bool2x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator -(float2x2 val)
		{
			return new float2x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 operator +(float2x2 val)
		{
			return new float2x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(float2x2 lhs, float2x2 rhs)
		{
			return new bool2x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(float2x2 lhs, float rhs)
		{
			return new bool2x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(float lhs, float2x2 rhs)
		{
			return new bool2x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(float2x2 lhs, float2x2 rhs)
		{
			return new bool2x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(float2x2 lhs, float rhs)
		{
			return new bool2x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(float lhs, float2x2 rhs)
		{
			return new bool2x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		public unsafe ref float2 this[int index]
		{
			get
			{
				fixed (float2x2* ptr = &this)
				{
					return ref *(float2*)(ptr + (IntPtr)index * (IntPtr)sizeof(float2) / (IntPtr)sizeof(float2x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float2x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is float2x2)
			{
				float2x2 float2x = (float2x2)o;
				return this.Equals(float2x);
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
			return string.Format("float2x2({0}f, {1}f,  {2}f, {3}f)", new object[]
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
			return string.Format("float2x2({0}f, {1}f,  {2}f, {3}f)", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider)
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 Rotate(float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			return math.float2x2(num2, -num, num, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 Scale(float s)
		{
			return math.float2x2(s, 0f, 0f, s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 Scale(float x, float y)
		{
			return math.float2x2(x, 0f, 0f, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 Scale(float2 v)
		{
			return float2x2.Scale(v.x, v.y);
		}

		public float2 c0;

		public float2 c1;

		public static readonly float2x2 identity = new float2x2(1f, 0f, 0f, 1f);

		public static readonly float2x2 zero;
	}
}
