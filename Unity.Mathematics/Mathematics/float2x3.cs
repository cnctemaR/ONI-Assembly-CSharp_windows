using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float2x3 : IEquatable<float2x3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(float2 c0, float2 c1, float2 c2)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(float m00, float m01, float m02, float m10, float m11, float m12)
		{
			this.c0 = new float2(m00, m10);
			this.c1 = new float2(m01, m11);
			this.c2 = new float2(m02, m12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(bool v)
		{
			this.c0 = math.select(new float2(0f), new float2(1f), v);
			this.c1 = math.select(new float2(0f), new float2(1f), v);
			this.c2 = math.select(new float2(0f), new float2(1f), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(bool2x3 v)
		{
			this.c0 = math.select(new float2(0f), new float2(1f), v.c0);
			this.c1 = math.select(new float2(0f), new float2(1f), v.c1);
			this.c2 = math.select(new float2(0f), new float2(1f), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(int2x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(uint2x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(double v)
		{
			this.c0 = (float2)v;
			this.c1 = (float2)v;
			this.c2 = (float2)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2x3(double2x3 v)
		{
			this.c0 = (float2)v.c0;
			this.c1 = (float2)v.c1;
			this.c2 = (float2)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x3(float v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2x3(bool v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2x3(bool2x3 v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x3(int v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x3(int2x3 v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x3(uint v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2x3(uint2x3 v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2x3(double v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2x3(double2x3 v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator *(float2x3 lhs, float2x3 rhs)
		{
			return new float2x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator *(float2x3 lhs, float rhs)
		{
			return new float2x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator *(float lhs, float2x3 rhs)
		{
			return new float2x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator +(float2x3 lhs, float2x3 rhs)
		{
			return new float2x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator +(float2x3 lhs, float rhs)
		{
			return new float2x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator +(float lhs, float2x3 rhs)
		{
			return new float2x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator -(float2x3 lhs, float2x3 rhs)
		{
			return new float2x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator -(float2x3 lhs, float rhs)
		{
			return new float2x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator -(float lhs, float2x3 rhs)
		{
			return new float2x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator /(float2x3 lhs, float2x3 rhs)
		{
			return new float2x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator /(float2x3 lhs, float rhs)
		{
			return new float2x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator /(float lhs, float2x3 rhs)
		{
			return new float2x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator %(float2x3 lhs, float2x3 rhs)
		{
			return new float2x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator %(float2x3 lhs, float rhs)
		{
			return new float2x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator %(float lhs, float2x3 rhs)
		{
			return new float2x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator ++(float2x3 val)
		{
			float2 @float = float2.op_Increment(val.c0);
			val.c0 = @float;
			float2 float2 = @float;
			@float = float2.op_Increment(val.c1);
			val.c1 = @float;
			float2 float3 = @float;
			@float = float2.op_Increment(val.c2);
			val.c2 = @float;
			return new float2x3(float2, float3, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator --(float2x3 val)
		{
			float2 @float = float2.op_Decrement(val.c0);
			val.c0 = @float;
			float2 float2 = @float;
			@float = float2.op_Decrement(val.c1);
			val.c1 = @float;
			float2 float3 = @float;
			@float = float2.op_Decrement(val.c2);
			val.c2 = @float;
			return new float2x3(float2, float3, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(float2x3 lhs, float2x3 rhs)
		{
			return new bool2x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(float2x3 lhs, float rhs)
		{
			return new bool2x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <(float lhs, float2x3 rhs)
		{
			return new bool2x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(float2x3 lhs, float2x3 rhs)
		{
			return new bool2x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(float2x3 lhs, float rhs)
		{
			return new bool2x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator <=(float lhs, float2x3 rhs)
		{
			return new bool2x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(float2x3 lhs, float2x3 rhs)
		{
			return new bool2x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(float2x3 lhs, float rhs)
		{
			return new bool2x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >(float lhs, float2x3 rhs)
		{
			return new bool2x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(float2x3 lhs, float2x3 rhs)
		{
			return new bool2x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(float2x3 lhs, float rhs)
		{
			return new bool2x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator >=(float lhs, float2x3 rhs)
		{
			return new bool2x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator -(float2x3 val)
		{
			return new float2x3(-val.c0, -val.c1, -val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 operator +(float2x3 val)
		{
			return new float2x3(+val.c0, +val.c1, +val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(float2x3 lhs, float2x3 rhs)
		{
			return new bool2x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(float2x3 lhs, float rhs)
		{
			return new bool2x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator ==(float lhs, float2x3 rhs)
		{
			return new bool2x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(float2x3 lhs, float2x3 rhs)
		{
			return new bool2x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(float2x3 lhs, float rhs)
		{
			return new bool2x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 operator !=(float lhs, float2x3 rhs)
		{
			return new bool2x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
		}

		public unsafe ref float2 this[int index]
		{
			get
			{
				fixed (float2x3* ptr = &this)
				{
					return ref *(float2*)(ptr + (IntPtr)index * (IntPtr)sizeof(float2) / (IntPtr)sizeof(float2x3));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float2x3 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2);
		}

		public override bool Equals(object o)
		{
			if (o is float2x3)
			{
				float2x3 float2x = (float2x3)o;
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
			return string.Format("float2x3({0}f, {1}f, {2}f,  {3}f, {4}f, {5}f)", new object[]
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
			return string.Format("float2x3({0}f, {1}f, {2}f,  {3}f, {4}f, {5}f)", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider)
			});
		}

		public float2 c0;

		public float2 c1;

		public float2 c2;

		public static readonly float2x3 zero;
	}
}
