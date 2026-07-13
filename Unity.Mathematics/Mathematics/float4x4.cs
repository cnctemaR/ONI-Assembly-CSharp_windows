using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float4x4 : IEquatable<float4x4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(float4 c0, float4 c1, float4 c2, float4 c3)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
			this.c3 = c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
		{
			this.c0 = new float4(m00, m10, m20, m30);
			this.c1 = new float4(m01, m11, m21, m31);
			this.c2 = new float4(m02, m12, m22, m32);
			this.c3 = new float4(m03, m13, m23, m33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(bool v)
		{
			this.c0 = math.select(new float4(0f), new float4(1f), v);
			this.c1 = math.select(new float4(0f), new float4(1f), v);
			this.c2 = math.select(new float4(0f), new float4(1f), v);
			this.c3 = math.select(new float4(0f), new float4(1f), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(bool4x4 v)
		{
			this.c0 = math.select(new float4(0f), new float4(1f), v.c0);
			this.c1 = math.select(new float4(0f), new float4(1f), v.c1);
			this.c2 = math.select(new float4(0f), new float4(1f), v.c2);
			this.c3 = math.select(new float4(0f), new float4(1f), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(int4x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(uint4x4 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
			this.c3 = v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(double v)
		{
			this.c0 = (float4)v;
			this.c1 = (float4)v;
			this.c2 = (float4)v;
			this.c3 = (float4)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4x4(double4x4 v)
		{
			this.c0 = (float4)v.c0;
			this.c1 = (float4)v.c1;
			this.c2 = (float4)v.c2;
			this.c3 = (float4)v.c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x4(float v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x4(bool v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x4(bool4x4 v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x4(int v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x4(int4x4 v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x4(uint v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4x4(uint4x4 v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x4(double v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4x4(double4x4 v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator *(float4x4 lhs, float4x4 rhs)
		{
			return new float4x4(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2, lhs.c3 * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator *(float4x4 lhs, float rhs)
		{
			return new float4x4(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs, lhs.c3 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator *(float lhs, float4x4 rhs)
		{
			return new float4x4(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2, lhs * rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator +(float4x4 lhs, float4x4 rhs)
		{
			return new float4x4(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2, lhs.c3 + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator +(float4x4 lhs, float rhs)
		{
			return new float4x4(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs, lhs.c3 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator +(float lhs, float4x4 rhs)
		{
			return new float4x4(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2, lhs + rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator -(float4x4 lhs, float4x4 rhs)
		{
			return new float4x4(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2, lhs.c3 - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator -(float4x4 lhs, float rhs)
		{
			return new float4x4(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs, lhs.c3 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator -(float lhs, float4x4 rhs)
		{
			return new float4x4(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2, lhs - rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator /(float4x4 lhs, float4x4 rhs)
		{
			return new float4x4(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2, lhs.c3 / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator /(float4x4 lhs, float rhs)
		{
			return new float4x4(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs, lhs.c3 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator /(float lhs, float4x4 rhs)
		{
			return new float4x4(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2, lhs / rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator %(float4x4 lhs, float4x4 rhs)
		{
			return new float4x4(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2, lhs.c3 % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator %(float4x4 lhs, float rhs)
		{
			return new float4x4(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs, lhs.c3 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator %(float lhs, float4x4 rhs)
		{
			return new float4x4(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2, lhs % rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator ++(float4x4 val)
		{
			float4 @float = float4.op_Increment(val.c0);
			val.c0 = @float;
			float4 float2 = @float;
			@float = float4.op_Increment(val.c1);
			val.c1 = @float;
			float4 float3 = @float;
			@float = float4.op_Increment(val.c2);
			val.c2 = @float;
			float4 float4 = @float;
			@float = float4.op_Increment(val.c3);
			val.c3 = @float;
			return new float4x4(float2, float3, float4, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator --(float4x4 val)
		{
			float4 @float = float4.op_Decrement(val.c0);
			val.c0 = @float;
			float4 float2 = @float;
			@float = float4.op_Decrement(val.c1);
			val.c1 = @float;
			float4 float3 = @float;
			@float = float4.op_Decrement(val.c2);
			val.c2 = @float;
			float4 float4 = @float;
			@float = float4.op_Decrement(val.c3);
			val.c3 = @float;
			return new float4x4(float2, float3, float4, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <(float4x4 lhs, float4x4 rhs)
		{
			return new bool4x4(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2, lhs.c3 < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <(float4x4 lhs, float rhs)
		{
			return new bool4x4(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs, lhs.c3 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <(float lhs, float4x4 rhs)
		{
			return new bool4x4(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2, lhs < rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <=(float4x4 lhs, float4x4 rhs)
		{
			return new bool4x4(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2, lhs.c3 <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <=(float4x4 lhs, float rhs)
		{
			return new bool4x4(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs, lhs.c3 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator <=(float lhs, float4x4 rhs)
		{
			return new bool4x4(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2, lhs <= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >(float4x4 lhs, float4x4 rhs)
		{
			return new bool4x4(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2, lhs.c3 > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >(float4x4 lhs, float rhs)
		{
			return new bool4x4(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs, lhs.c3 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >(float lhs, float4x4 rhs)
		{
			return new bool4x4(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2, lhs > rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >=(float4x4 lhs, float4x4 rhs)
		{
			return new bool4x4(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2, lhs.c3 >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >=(float4x4 lhs, float rhs)
		{
			return new bool4x4(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs, lhs.c3 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator >=(float lhs, float4x4 rhs)
		{
			return new bool4x4(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2, lhs >= rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator -(float4x4 val)
		{
			return new float4x4(-val.c0, -val.c1, -val.c2, -val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 operator +(float4x4 val)
		{
			return new float4x4(+val.c0, +val.c1, +val.c2, +val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator ==(float4x4 lhs, float4x4 rhs)
		{
			return new bool4x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator ==(float4x4 lhs, float rhs)
		{
			return new bool4x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator ==(float lhs, float4x4 rhs)
		{
			return new bool4x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator !=(float4x4 lhs, float4x4 rhs)
		{
			return new bool4x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator !=(float4x4 lhs, float rhs)
		{
			return new bool4x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 operator !=(float lhs, float4x4 rhs)
		{
			return new bool4x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
		}

		public unsafe ref float4 this[int index]
		{
			get
			{
				fixed (float4x4* ptr = &this)
				{
					return ref *(float4*)(ptr + (IntPtr)index * (IntPtr)sizeof(float4) / (IntPtr)sizeof(float4x4));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float4x4 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2) && this.c3.Equals(rhs.c3);
		}

		public override bool Equals(object o)
		{
			if (o is float4x4)
			{
				float4x4 float4x = (float4x4)o;
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
			return string.Format("float4x4({0}f, {1}f, {2}f, {3}f,  {4}f, {5}f, {6}f, {7}f,  {8}f, {9}f, {10}f, {11}f,  {12}f, {13}f, {14}f, {15}f)", new object[]
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
				this.c3.z,
				this.c0.w,
				this.c1.w,
				this.c2.w,
				this.c3.w
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("float4x4({0}f, {1}f, {2}f, {3}f,  {4}f, {5}f, {6}f, {7}f,  {8}f, {9}f, {10}f, {11}f,  {12}f, {13}f, {14}f, {15}f)", new object[]
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
				this.c3.z.ToString(format, formatProvider),
				this.c0.w.ToString(format, formatProvider),
				this.c1.w.ToString(format, formatProvider),
				this.c2.w.ToString(format, formatProvider),
				this.c3.w.ToString(format, formatProvider)
			});
		}

		public static implicit operator float4x4(Matrix4x4 m)
		{
			return new float4x4(m.GetColumn(0), m.GetColumn(1), m.GetColumn(2), m.GetColumn(3));
		}

		public static implicit operator Matrix4x4(float4x4 m)
		{
			return new Matrix4x4(m.c0, m.c1, m.c2, m.c3);
		}

		public float4x4(float3x3 rotation, float3 translation)
		{
			this.c0 = math.float4(rotation.c0, 0f);
			this.c1 = math.float4(rotation.c1, 0f);
			this.c2 = math.float4(rotation.c2, 0f);
			this.c3 = math.float4(translation, 1f);
		}

		public float4x4(quaternion rotation, float3 translation)
		{
			float3x3 float3x = math.float3x3(rotation);
			this.c0 = math.float4(float3x.c0, 0f);
			this.c1 = math.float4(float3x.c1, 0f);
			this.c2 = math.float4(float3x.c2, 0f);
			this.c3 = math.float4(translation, 1f);
		}

		public float4x4(RigidTransform transform)
		{
			float3x3 float3x = math.float3x3(transform.rot);
			this.c0 = math.float4(float3x.c0, 0f);
			this.c1 = math.float4(float3x.c1, 0f);
			this.c2 = math.float4(float3x.c2, 0f);
			this.c3 = math.float4(transform.pos, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 AxisAngle(float3 axis, float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			float4 @float = math.float4(axis, 0f);
			float4 yzxx = @float.yzxx;
			float4 zxyx = @float.zxyx;
			float4 float2 = @float - @float * num2;
			float4 float3 = math.float4(@float.xyz * num, num2);
			uint4 @uint = math.uint4(0U, 0U, 2147483648U, 0U);
			uint4 uint2 = math.uint4(2147483648U, 0U, 0U, 0U);
			uint4 uint3 = math.uint4(0U, 2147483648U, 0U, 0U);
			uint4 uint4 = math.uint4(uint.MaxValue, uint.MaxValue, uint.MaxValue, 0U);
			return math.float4x4(@float.x * float2 + math.asfloat((math.asuint(float3.wzyx) ^ @uint) & uint4), @float.y * float2 + math.asfloat((math.asuint(float3.zwxx) ^ uint2) & uint4), @float.z * float2 + math.asfloat((math.asuint(float3.yxwx) ^ uint3) & uint4), math.float4(0f, 0f, 0f, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerXYZ(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float4x4(float2.y * float2.z, float2.z * @float.x * @float.y - float2.x * @float.z, float2.x * float2.z * @float.y + @float.x * @float.z, 0f, float2.y * @float.z, float2.x * float2.z + @float.x * @float.y * @float.z, float2.x * @float.y * @float.z - float2.z * @float.x, 0f, -@float.y, float2.y * @float.x, float2.x * float2.y, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerXZY(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float4x4(float2.y * float2.z, @float.x * @float.y - float2.x * float2.y * @float.z, float2.x * @float.y + float2.y * @float.x * @float.z, 0f, @float.z, float2.x * float2.z, -float2.z * @float.x, 0f, -float2.z * @float.y, float2.y * @float.x + float2.x * @float.y * @float.z, float2.x * float2.y - @float.x * @float.y * @float.z, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerYXZ(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float4x4(float2.y * float2.z - @float.x * @float.y * @float.z, -float2.x * @float.z, float2.z * @float.y + float2.y * @float.x * @float.z, 0f, float2.z * @float.x * @float.y + float2.y * @float.z, float2.x * float2.z, @float.y * @float.z - float2.y * float2.z * @float.x, 0f, -float2.x * @float.y, @float.x, float2.x * float2.y, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerYZX(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float4x4(float2.y * float2.z, -@float.z, float2.z * @float.y, 0f, @float.x * @float.y + float2.x * float2.y * @float.z, float2.x * float2.z, float2.x * @float.y * @float.z - float2.y * @float.x, 0f, float2.y * @float.x * @float.z - float2.x * @float.y, float2.z * @float.x, float2.x * float2.y + @float.x * @float.y * @float.z, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerZXY(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float4x4(float2.y * float2.z + @float.x * @float.y * @float.z, float2.z * @float.x * @float.y - float2.y * @float.z, float2.x * @float.y, 0f, float2.x * @float.z, float2.x * float2.z, -@float.x, 0f, float2.y * @float.x * @float.z - float2.z * @float.y, float2.y * float2.z * @float.x + @float.y * @float.z, float2.x * float2.y, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerZYX(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float4x4(float2.y * float2.z, -float2.y * @float.z, @float.y, 0f, float2.z * @float.x * @float.y + float2.x * @float.z, float2.x * float2.z - @float.x * @float.y * @float.z, -float2.y * @float.x, 0f, @float.x * @float.z - float2.x * float2.z * @float.y, float2.z * @float.x + float2.x * @float.y * @float.z, float2.x * float2.y, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerXYZ(float x, float y, float z)
		{
			return float4x4.EulerXYZ(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerXZY(float x, float y, float z)
		{
			return float4x4.EulerXZY(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerYXZ(float x, float y, float z)
		{
			return float4x4.EulerYXZ(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerYZX(float x, float y, float z)
		{
			return float4x4.EulerYZX(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerZXY(float x, float y, float z)
		{
			return float4x4.EulerZXY(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 EulerZYX(float x, float y, float z)
		{
			return float4x4.EulerZYX(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 Euler(float3 xyz, math.RotationOrder order = math.RotationOrder.ZXY)
		{
			switch (order)
			{
			case math.RotationOrder.XYZ:
				return float4x4.EulerXYZ(xyz);
			case math.RotationOrder.XZY:
				return float4x4.EulerXZY(xyz);
			case math.RotationOrder.YXZ:
				return float4x4.EulerYXZ(xyz);
			case math.RotationOrder.YZX:
				return float4x4.EulerYZX(xyz);
			case math.RotationOrder.ZXY:
				return float4x4.EulerZXY(xyz);
			case math.RotationOrder.ZYX:
				return float4x4.EulerZYX(xyz);
			default:
				return float4x4.identity;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 Euler(float x, float y, float z, math.RotationOrder order = math.RotationOrder.ZXY)
		{
			return float4x4.Euler(math.float3(x, y, z), order);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 RotateX(float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			return math.float4x4(1f, 0f, 0f, 0f, 0f, num2, -num, 0f, 0f, num, num2, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 RotateY(float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			return math.float4x4(num2, 0f, num, 0f, 0f, 1f, 0f, 0f, -num, 0f, num2, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 RotateZ(float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			return math.float4x4(num2, -num, 0f, 0f, num, num2, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 Scale(float s)
		{
			return math.float4x4(s, 0f, 0f, 0f, 0f, s, 0f, 0f, 0f, 0f, s, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 Scale(float x, float y, float z)
		{
			return math.float4x4(x, 0f, 0f, 0f, 0f, y, 0f, 0f, 0f, 0f, z, 0f, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 Scale(float3 scales)
		{
			return float4x4.Scale(scales.x, scales.y, scales.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 Translate(float3 vector)
		{
			return math.float4x4(math.float4(1f, 0f, 0f, 0f), math.float4(0f, 1f, 0f, 0f), math.float4(0f, 0f, 1f, 0f), math.float4(vector.x, vector.y, vector.z, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 LookAt(float3 eye, float3 target, float3 up)
		{
			float3x3 float3x = float3x3.LookRotation(math.normalize(target - eye), up);
			float4x4 float4x;
			float4x.c0 = math.float4(float3x.c0, 0f);
			float4x.c1 = math.float4(float3x.c1, 0f);
			float4x.c2 = math.float4(float3x.c2, 0f);
			float4x.c3 = math.float4(eye, 1f);
			return float4x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 Ortho(float width, float height, float near, float far)
		{
			float num = 1f / width;
			float num2 = 1f / height;
			float num3 = 1f / (far - near);
			return math.float4x4(2f * num, 0f, 0f, 0f, 0f, 2f * num2, 0f, 0f, 0f, 0f, -2f * num3, -(far + near) * num3, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 OrthoOffCenter(float left, float right, float bottom, float top, float near, float far)
		{
			float num = 1f / (right - left);
			float num2 = 1f / (top - bottom);
			float num3 = 1f / (far - near);
			return math.float4x4(2f * num, 0f, 0f, -(right + left) * num, 0f, 2f * num2, 0f, -(top + bottom) * num2, 0f, 0f, -2f * num3, -(far + near) * num3, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 PerspectiveFov(float verticalFov, float aspect, float near, float far)
		{
			float num = 1f / math.tan(verticalFov * 0.5f);
			float num2 = 1f / (near - far);
			return math.float4x4(num / aspect, 0f, 0f, 0f, 0f, num, 0f, 0f, 0f, 0f, (far + near) * num2, 2f * near * far * num2, 0f, 0f, -1f, 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
		{
			float num = 1f / (near - far);
			float num2 = 1f / (right - left);
			float num3 = 1f / (top - bottom);
			return math.float4x4(2f * near * num2, 0f, (left + right) * num2, 0f, 0f, 2f * near * num3, (bottom + top) * num3, 0f, 0f, 0f, (far + near) * num, 2f * near * far * num, 0f, 0f, -1f, 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 TRS(float3 translation, quaternion rotation, float3 scale)
		{
			float3x3 float3x = math.float3x3(rotation);
			return math.float4x4(math.float4(float3x.c0 * scale.x, 0f), math.float4(float3x.c1 * scale.y, 0f), math.float4(float3x.c2 * scale.z, 0f), math.float4(translation, 1f));
		}

		public float4 c0;

		public float4 c1;

		public float4 c2;

		public float4 c3;

		public static readonly float4x4 identity = new float4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

		public static readonly float4x4 zero;
	}
}
