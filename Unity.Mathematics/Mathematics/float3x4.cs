using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float3x4 : IEquatable<float3x4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(float3 c0, float3 c1, float3 c2, float3 c3)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
			this.c3 = c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23)
		{
			this.c0 = new float3(m00, m10, m20);
			this.c1 = new float3(m01, m11, m21);
			this.c2 = new float3(m02, m12, m22);
			this.c3 = new float3(m03, m13, m23);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(bool v)
		{
			this.c0 = math.select(new float3(0f), new float3(1f), v);
			this.c1 = math.select(new float3(0f), new float3(1f), v);
			this.c2 = math.select(new float3(0f), new float3(1f), v);
			this.c3 = math.select(new float3(0f), new float3(1f), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(bool3x4 v)
		{
			this.c0 = math.select(new float3(0f), new float3(1f), v.c0);
			this.c1 = math.select(new float3(0f), new float3(1f), v.c1);
			this.c2 = math.select(new float3(0f), new float3(1f), v.c2);
			this.c3 = math.select(new float3(0f), new float3(1f), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(int3x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(uint3x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(double v)
		{
			this.c0 = (float3)v;
			this.c1 = (float3)v;
			this.c2 = (float3)v;
			this.c3 = (float3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x4(double3x4 v)
		{
			this.c0 = (float3)v.c0;
			this.c1 = (float3)v.c1;
			this.c2 = (float3)v.c2;
			this.c3 = (float3)v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x4(float v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x4(bool v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x4(bool3x4 v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x4(int v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x4(int3x4 v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x4(uint v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x4(uint3x4 v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x4(double v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x4(double3x4 v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator *(float3x4 lhs, float3x4 rhs)
		{
			return new float3x4(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2, lhs.c3 * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator *(float3x4 lhs, float rhs)
		{
			return new float3x4(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs, lhs.c3 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator *(float lhs, float3x4 rhs)
		{
			return new float3x4(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2, lhs * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator +(float3x4 lhs, float3x4 rhs)
		{
			return new float3x4(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2, lhs.c3 + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator +(float3x4 lhs, float rhs)
		{
			return new float3x4(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs, lhs.c3 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator +(float lhs, float3x4 rhs)
		{
			return new float3x4(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2, lhs + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator -(float3x4 lhs, float3x4 rhs)
		{
			return new float3x4(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2, lhs.c3 - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator -(float3x4 lhs, float rhs)
		{
			return new float3x4(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs, lhs.c3 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator -(float lhs, float3x4 rhs)
		{
			return new float3x4(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2, lhs - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator /(float3x4 lhs, float3x4 rhs)
		{
			return new float3x4(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2, lhs.c3 / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator /(float3x4 lhs, float rhs)
		{
			return new float3x4(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs, lhs.c3 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator /(float lhs, float3x4 rhs)
		{
			return new float3x4(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2, lhs / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator %(float3x4 lhs, float3x4 rhs)
		{
			return new float3x4(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2, lhs.c3 % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator %(float3x4 lhs, float rhs)
		{
			return new float3x4(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs, lhs.c3 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator %(float lhs, float3x4 rhs)
		{
			return new float3x4(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2, lhs % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator ++(float3x4 val)
		{
			float3 @float = float3.op_Increment(val.c0);
			val.c0 = @float;
			float3 float2 = @float;
			@float = float3.op_Increment(val.c1);
			val.c1 = @float;
			float3 float3 = @float;
			@float = float3.op_Increment(val.c2);
			val.c2 = @float;
			float3 float4 = @float;
			@float = float3.op_Increment(val.c3);
			val.c3 = @float;
			return new float3x4(float2, float3, float4, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator --(float3x4 val)
		{
			float3 @float = float3.op_Decrement(val.c0);
			val.c0 = @float;
			float3 float2 = @float;
			@float = float3.op_Decrement(val.c1);
			val.c1 = @float;
			float3 float3 = @float;
			@float = float3.op_Decrement(val.c2);
			val.c2 = @float;
			float3 float4 = @float;
			@float = float3.op_Decrement(val.c3);
			val.c3 = @float;
			return new float3x4(float2, float3, float4, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <(float3x4 lhs, float3x4 rhs)
		{
			return new bool3x4(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2, lhs.c3 < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <(float3x4 lhs, float rhs)
		{
			return new bool3x4(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs, lhs.c3 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <(float lhs, float3x4 rhs)
		{
			return new bool3x4(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2, lhs < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <=(float3x4 lhs, float3x4 rhs)
		{
			return new bool3x4(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2, lhs.c3 <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <=(float3x4 lhs, float rhs)
		{
			return new bool3x4(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs, lhs.c3 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator <=(float lhs, float3x4 rhs)
		{
			return new bool3x4(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2, lhs <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >(float3x4 lhs, float3x4 rhs)
		{
			return new bool3x4(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2, lhs.c3 > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >(float3x4 lhs, float rhs)
		{
			return new bool3x4(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs, lhs.c3 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >(float lhs, float3x4 rhs)
		{
			return new bool3x4(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2, lhs > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >=(float3x4 lhs, float3x4 rhs)
		{
			return new bool3x4(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2, lhs.c3 >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >=(float3x4 lhs, float rhs)
		{
			return new bool3x4(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs, lhs.c3 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator >=(float lhs, float3x4 rhs)
		{
			return new bool3x4(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2, lhs >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator -(float3x4 val)
		{
			return new float3x4(-val.c0, -val.c1, -val.c2, -val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 operator +(float3x4 val)
		{
			return new float3x4(+val.c0, +val.c1, +val.c2, +val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(float3x4 lhs, float3x4 rhs)
		{
			return new bool3x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(float3x4 lhs, float rhs)
		{
			return new bool3x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(float lhs, float3x4 rhs)
		{
			return new bool3x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(float3x4 lhs, float3x4 rhs)
		{
			return new bool3x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(float3x4 lhs, float rhs)
		{
			return new bool3x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(float lhs, float3x4 rhs)
		{
			return new bool3x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
		}

		public unsafe ref float3 this[int index]
		{
			get
			{
				fixed (float3x4* ptr = &this)
				{
					return ref *(float3*)(ptr + (IntPtr)index * (IntPtr)sizeof(float3) / (IntPtr)sizeof(float3x4));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float3x4 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2) && this.c3.Equals(rhs.c3);
		}

		public override bool Equals(object o)
		{
			if (o is float3x4)
			{
				float3x4 float3x = (float3x4)o;
				return this.Equals(float3x);
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
			return string.Format("float3x4({0}f, {1}f, {2}f, {3}f,  {4}f, {5}f, {6}f, {7}f,  {8}f, {9}f, {10}f, {11}f)", new object[]
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
			return string.Format("float3x4({0}f, {1}f, {2}f, {3}f,  {4}f, {5}f, {6}f, {7}f,  {8}f, {9}f, {10}f, {11}f)", new object[]
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

		public float3 c0;

		public float3 c1;

		public float3 c2;

		public float3 c3;

		public static readonly float3x4 zero;
	}
}
