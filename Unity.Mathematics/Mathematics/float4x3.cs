using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float4x3 : IEquatable<float4x3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(float4 c0, float4 c1, float4 c2)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22, float m30, float m31, float m32)
		{
			this.c0 = new float4(m00, m10, m20, m30);
			this.c1 = new float4(m01, m11, m21, m31);
			this.c2 = new float4(m02, m12, m22, m32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(bool v)
		{
			this.c0 = math.select(new float4(0f), new float4(1f), v);
			this.c1 = math.select(new float4(0f), new float4(1f), v);
			this.c2 = math.select(new float4(0f), new float4(1f), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(bool4x3 v)
		{
			this.c0 = math.select(new float4(0f), new float4(1f), v.c0);
			this.c1 = math.select(new float4(0f), new float4(1f), v.c1);
			this.c2 = math.select(new float4(0f), new float4(1f), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(int4x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(uint4x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(double v)
		{
			this.c0 = (float4)v;
			this.c1 = (float4)v;
			this.c2 = (float4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x3(double4x3 v)
		{
			this.c0 = (float4)v.c0;
			this.c1 = (float4)v.c1;
			this.c2 = (float4)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x3(float v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x3(bool v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x3(bool4x3 v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x3(int v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x3(int4x3 v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x3(uint v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x3(uint4x3 v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x3(double v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x3(double4x3 v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator *(float4x3 lhs, float4x3 rhs)
		{
			return new float4x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator *(float4x3 lhs, float rhs)
		{
			return new float4x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator *(float lhs, float4x3 rhs)
		{
			return new float4x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator +(float4x3 lhs, float4x3 rhs)
		{
			return new float4x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator +(float4x3 lhs, float rhs)
		{
			return new float4x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator +(float lhs, float4x3 rhs)
		{
			return new float4x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator -(float4x3 lhs, float4x3 rhs)
		{
			return new float4x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator -(float4x3 lhs, float rhs)
		{
			return new float4x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator -(float lhs, float4x3 rhs)
		{
			return new float4x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator /(float4x3 lhs, float4x3 rhs)
		{
			return new float4x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator /(float4x3 lhs, float rhs)
		{
			return new float4x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator /(float lhs, float4x3 rhs)
		{
			return new float4x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator %(float4x3 lhs, float4x3 rhs)
		{
			return new float4x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator %(float4x3 lhs, float rhs)
		{
			return new float4x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator %(float lhs, float4x3 rhs)
		{
			return new float4x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator ++(float4x3 val)
		{
			float4 @float = float4.op_Increment(val.c0);
			val.c0 = @float;
			float4 float2 = @float;
			@float = float4.op_Increment(val.c1);
			val.c1 = @float;
			float4 float3 = @float;
			@float = float4.op_Increment(val.c2);
			val.c2 = @float;
			return new float4x3(float2, float3, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator --(float4x3 val)
		{
			float4 @float = float4.op_Decrement(val.c0);
			val.c0 = @float;
			float4 float2 = @float;
			@float = float4.op_Decrement(val.c1);
			val.c1 = @float;
			float4 float3 = @float;
			@float = float4.op_Decrement(val.c2);
			val.c2 = @float;
			return new float4x3(float2, float3, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <(float4x3 lhs, float4x3 rhs)
		{
			return new bool4x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <(float4x3 lhs, float rhs)
		{
			return new bool4x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <(float lhs, float4x3 rhs)
		{
			return new bool4x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <=(float4x3 lhs, float4x3 rhs)
		{
			return new bool4x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <=(float4x3 lhs, float rhs)
		{
			return new bool4x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator <=(float lhs, float4x3 rhs)
		{
			return new bool4x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >(float4x3 lhs, float4x3 rhs)
		{
			return new bool4x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >(float4x3 lhs, float rhs)
		{
			return new bool4x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >(float lhs, float4x3 rhs)
		{
			return new bool4x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >=(float4x3 lhs, float4x3 rhs)
		{
			return new bool4x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >=(float4x3 lhs, float rhs)
		{
			return new bool4x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator >=(float lhs, float4x3 rhs)
		{
			return new bool4x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator -(float4x3 val)
		{
			return new float4x3(-val.c0, -val.c1, -val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 operator +(float4x3 val)
		{
			return new float4x3(+val.c0, +val.c1, +val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator ==(float4x3 lhs, float4x3 rhs)
		{
			return new bool4x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator ==(float4x3 lhs, float rhs)
		{
			return new bool4x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator ==(float lhs, float4x3 rhs)
		{
			return new bool4x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator !=(float4x3 lhs, float4x3 rhs)
		{
			return new bool4x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator !=(float4x3 lhs, float rhs)
		{
			return new bool4x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 operator !=(float lhs, float4x3 rhs)
		{
			return new bool4x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
		}

		public unsafe ref float4 this[int index]
		{
			get
			{
				fixed (float4x3* ptr = &this)
				{
					return ref *(float4*)(ptr + (IntPtr)index * (IntPtr)sizeof(float4) / (IntPtr)sizeof(float4x3));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float4x3 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2);
		}

		public override bool Equals(object o)
		{
			if (o is float4x3)
			{
				float4x3 float4x = (float4x3)o;
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
			return string.Format("float4x3({0}f, {1}f, {2}f,  {3}f, {4}f, {5}f,  {6}f, {7}f, {8}f,  {9}f, {10}f, {11}f)", new object[]
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
			return string.Format("float4x3({0}f, {1}f, {2}f,  {3}f, {4}f, {5}f,  {6}f, {7}f, {8}f,  {9}f, {10}f, {11}f)", new object[]
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

		public float4 c0;

		public float4 c1;

		public float4 c2;

		public static readonly float4x3 zero;
	}
}
