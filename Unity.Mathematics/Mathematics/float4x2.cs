using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float4x2 : IEquatable<float4x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(float4 c0, float4 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(float m00, float m01, float m10, float m11, float m20, float m21, float m30, float m31)
		{
			this.c0 = new float4(m00, m10, m20, m30);
			this.c1 = new float4(m01, m11, m21, m31);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(float v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(bool v)
		{
			this.c0 = math.select(new float4(0f), new float4(1f), v);
			this.c1 = math.select(new float4(0f), new float4(1f), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(bool4x2 v)
		{
			this.c0 = math.select(new float4(0f), new float4(1f), v.c0);
			this.c1 = math.select(new float4(0f), new float4(1f), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(int v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(int4x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(uint v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(uint4x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(double v)
		{
			this.c0 = (float4)v;
			this.c1 = (float4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x2(double4x2 v)
		{
			this.c0 = (float4)v.c0;
			this.c1 = (float4)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x2(float v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x2(bool v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x2(bool4x2 v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x2(int v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x2(int4x2 v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x2(uint v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x2(uint4x2 v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x2(double v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x2(double4x2 v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator *(float4x2 lhs, float4x2 rhs)
		{
			return new float4x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator *(float4x2 lhs, float rhs)
		{
			return new float4x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator *(float lhs, float4x2 rhs)
		{
			return new float4x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator +(float4x2 lhs, float4x2 rhs)
		{
			return new float4x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator +(float4x2 lhs, float rhs)
		{
			return new float4x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator +(float lhs, float4x2 rhs)
		{
			return new float4x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator -(float4x2 lhs, float4x2 rhs)
		{
			return new float4x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator -(float4x2 lhs, float rhs)
		{
			return new float4x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator -(float lhs, float4x2 rhs)
		{
			return new float4x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator /(float4x2 lhs, float4x2 rhs)
		{
			return new float4x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator /(float4x2 lhs, float rhs)
		{
			return new float4x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator /(float lhs, float4x2 rhs)
		{
			return new float4x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator %(float4x2 lhs, float4x2 rhs)
		{
			return new float4x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator %(float4x2 lhs, float rhs)
		{
			return new float4x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator %(float lhs, float4x2 rhs)
		{
			return new float4x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator ++(float4x2 val)
		{
			float4 @float = float4.op_Increment(val.c0);
			val.c0 = @float;
			float4 float2 = @float;
			@float = float4.op_Increment(val.c1);
			val.c1 = @float;
			return new float4x2(float2, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator --(float4x2 val)
		{
			float4 @float = float4.op_Decrement(val.c0);
			val.c0 = @float;
			float4 float2 = @float;
			@float = float4.op_Decrement(val.c1);
			val.c1 = @float;
			return new float4x2(float2, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <(float4x2 lhs, float4x2 rhs)
		{
			return new bool4x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <(float4x2 lhs, float rhs)
		{
			return new bool4x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <(float lhs, float4x2 rhs)
		{
			return new bool4x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <=(float4x2 lhs, float4x2 rhs)
		{
			return new bool4x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <=(float4x2 lhs, float rhs)
		{
			return new bool4x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator <=(float lhs, float4x2 rhs)
		{
			return new bool4x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >(float4x2 lhs, float4x2 rhs)
		{
			return new bool4x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >(float4x2 lhs, float rhs)
		{
			return new bool4x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >(float lhs, float4x2 rhs)
		{
			return new bool4x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >=(float4x2 lhs, float4x2 rhs)
		{
			return new bool4x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >=(float4x2 lhs, float rhs)
		{
			return new bool4x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator >=(float lhs, float4x2 rhs)
		{
			return new bool4x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator -(float4x2 val)
		{
			return new float4x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 operator +(float4x2 val)
		{
			return new float4x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(float4x2 lhs, float4x2 rhs)
		{
			return new bool4x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(float4x2 lhs, float rhs)
		{
			return new bool4x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(float lhs, float4x2 rhs)
		{
			return new bool4x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(float4x2 lhs, float4x2 rhs)
		{
			return new bool4x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(float4x2 lhs, float rhs)
		{
			return new bool4x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(float lhs, float4x2 rhs)
		{
			return new bool4x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		public unsafe ref float4 this[int index]
		{
			get
			{
				fixed (float4x2* ptr = &this)
				{
					return ref *(float4*)(ptr + (IntPtr)index * (IntPtr)sizeof(float4) / (IntPtr)sizeof(float4x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float4x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is float4x2)
			{
				float4x2 float4x = (float4x2)o;
				return this.Equals(float4x);
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
			return string.Format("float4x2({0}f, {1}f,  {2}f, {3}f,  {4}f, {5}f,  {6}f, {7}f)", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y,
				this.c0.z,
				this.c1.z,
				this.c0.w,
				this.c1.w
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("float4x2({0}f, {1}f,  {2}f, {3}f,  {4}f, {5}f,  {6}f, {7}f)", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider),
				this.c0.w.ToString(format, formatProvider),
				this.c1.w.ToString(format, formatProvider)
			});
		}

		public float4 c0;

		public float4 c1;

		public static readonly float4x2 zero;
	}
}
