using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float3x3 : IEquatable<float3x3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(float3 c0, float3 c1, float3 c2)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
		{
			this.c0 = new float3(m00, m10, m20);
			this.c1 = new float3(m01, m11, m21);
			this.c2 = new float3(m02, m12, m22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(float v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(bool v)
		{
			this.c0 = math.select(new float3(0f), new float3(1f), v);
			this.c1 = math.select(new float3(0f), new float3(1f), v);
			this.c2 = math.select(new float3(0f), new float3(1f), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(bool3x3 v)
		{
			this.c0 = math.select(new float3(0f), new float3(1f), v.c0);
			this.c1 = math.select(new float3(0f), new float3(1f), v.c1);
			this.c2 = math.select(new float3(0f), new float3(1f), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(int v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(int3x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(uint v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(uint3x3 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
			this.c2 = v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(double v)
		{
			this.c0 = (float3)v;
			this.c1 = (float3)v;
			this.c2 = (float3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x3(double3x3 v)
		{
			this.c0 = (float3)v.c0;
			this.c1 = (float3)v.c1;
			this.c2 = (float3)v.c2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x3(float v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x3(bool v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x3(bool3x3 v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x3(int v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x3(int3x3 v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x3(uint v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x3(uint3x3 v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x3(double v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x3(double3x3 v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator *(float3x3 lhs, float3x3 rhs)
		{
			return new float3x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator *(float3x3 lhs, float rhs)
		{
			return new float3x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator *(float lhs, float3x3 rhs)
		{
			return new float3x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator +(float3x3 lhs, float3x3 rhs)
		{
			return new float3x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator +(float3x3 lhs, float rhs)
		{
			return new float3x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator +(float lhs, float3x3 rhs)
		{
			return new float3x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator -(float3x3 lhs, float3x3 rhs)
		{
			return new float3x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator -(float3x3 lhs, float rhs)
		{
			return new float3x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator -(float lhs, float3x3 rhs)
		{
			return new float3x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator /(float3x3 lhs, float3x3 rhs)
		{
			return new float3x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator /(float3x3 lhs, float rhs)
		{
			return new float3x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator /(float lhs, float3x3 rhs)
		{
			return new float3x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator %(float3x3 lhs, float3x3 rhs)
		{
			return new float3x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator %(float3x3 lhs, float rhs)
		{
			return new float3x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator %(float lhs, float3x3 rhs)
		{
			return new float3x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator ++(float3x3 val)
		{
			float3 @float = float3.op_Increment(val.c0);
			val.c0 = @float;
			float3 float2 = @float;
			@float = float3.op_Increment(val.c1);
			val.c1 = @float;
			float3 float3 = @float;
			@float = float3.op_Increment(val.c2);
			val.c2 = @float;
			return new float3x3(float2, float3, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator --(float3x3 val)
		{
			float3 @float = float3.op_Decrement(val.c0);
			val.c0 = @float;
			float3 float2 = @float;
			@float = float3.op_Decrement(val.c1);
			val.c1 = @float;
			float3 float3 = @float;
			@float = float3.op_Decrement(val.c2);
			val.c2 = @float;
			return new float3x3(float2, float3, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <(float3x3 lhs, float3x3 rhs)
		{
			return new bool3x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <(float3x3 lhs, float rhs)
		{
			return new bool3x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <(float lhs, float3x3 rhs)
		{
			return new bool3x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <=(float3x3 lhs, float3x3 rhs)
		{
			return new bool3x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <=(float3x3 lhs, float rhs)
		{
			return new bool3x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator <=(float lhs, float3x3 rhs)
		{
			return new bool3x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >(float3x3 lhs, float3x3 rhs)
		{
			return new bool3x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >(float3x3 lhs, float rhs)
		{
			return new bool3x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >(float lhs, float3x3 rhs)
		{
			return new bool3x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >=(float3x3 lhs, float3x3 rhs)
		{
			return new bool3x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >=(float3x3 lhs, float rhs)
		{
			return new bool3x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator >=(float lhs, float3x3 rhs)
		{
			return new bool3x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator -(float3x3 val)
		{
			return new float3x3(-val.c0, -val.c1, -val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 operator +(float3x3 val)
		{
			return new float3x3(+val.c0, +val.c1, +val.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator ==(float3x3 lhs, float3x3 rhs)
		{
			return new bool3x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator ==(float3x3 lhs, float rhs)
		{
			return new bool3x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator ==(float lhs, float3x3 rhs)
		{
			return new bool3x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator !=(float3x3 lhs, float3x3 rhs)
		{
			return new bool3x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator !=(float3x3 lhs, float rhs)
		{
			return new bool3x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 operator !=(float lhs, float3x3 rhs)
		{
			return new bool3x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
		}

		public unsafe ref float3 this[int index]
		{
			get
			{
				fixed (float3x3* ptr = &this)
				{
					return ref *(float3*)(ptr + (IntPtr)index * (IntPtr)sizeof(float3) / (IntPtr)sizeof(float3x3));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float3x3 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2);
		}

		public override bool Equals(object o)
		{
			if (o is float3x3)
			{
				float3x3 float3x = (float3x3)o;
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
			return string.Format("float3x3({0}f, {1}f, {2}f,  {3}f, {4}f, {5}f,  {6}f, {7}f, {8}f)", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c0.y,
				this.c1.y,
				this.c2.y,
				this.c0.z,
				this.c1.z,
				this.c2.z
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("float3x3({0}f, {1}f, {2}f,  {3}f, {4}f, {5}f,  {6}f, {7}f, {8}f)", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c2.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c2.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider),
				this.c2.z.ToString(format, formatProvider)
			});
		}

		public float3x3(float4x4 f4x4)
		{
			this.c0 = f4x4.c0.xyz;
			this.c1 = f4x4.c1.xyz;
			this.c2 = f4x4.c2.xyz;
		}

		public float3x3(quaternion q)
		{
			float4 value = q.value;
			float4 @float = value + value;
			uint3 @uint = math.uint3(2147483648U, 0U, 2147483648U);
			uint3 uint2 = math.uint3(2147483648U, 2147483648U, 0U);
			uint3 uint3 = math.uint3(0U, 2147483648U, 2147483648U);
			this.c0 = @float.y * math.asfloat(math.asuint(value.yxw) ^ @uint) - @float.z * math.asfloat(math.asuint(value.zwx) ^ uint3) + math.float3(1f, 0f, 0f);
			this.c1 = @float.z * math.asfloat(math.asuint(value.wzy) ^ uint2) - @float.x * math.asfloat(math.asuint(value.yxw) ^ @uint) + math.float3(0f, 1f, 0f);
			this.c2 = @float.x * math.asfloat(math.asuint(value.zwx) ^ uint3) - @float.y * math.asfloat(math.asuint(value.wzy) ^ uint2) + math.float3(0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 AxisAngle(float3 axis, float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			float3 @float = axis;
			float3 yzx = @float.yzx;
			float3 zxy = @float.zxy;
			float3 float2 = @float - @float * num2;
			float4 float3 = math.float4(@float * num, num2);
			uint3 @uint = math.uint3(0U, 0U, 2147483648U);
			uint3 uint2 = math.uint3(2147483648U, 0U, 0U);
			uint3 uint3 = math.uint3(0U, 2147483648U, 0U);
			return math.float3x3(@float.x * float2 + math.asfloat(math.asuint(float3.wzy) ^ @uint), @float.y * float2 + math.asfloat(math.asuint(float3.zwx) ^ uint2), @float.z * float2 + math.asfloat(math.asuint(float3.yxw) ^ uint3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerXYZ(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float3x3(float2.y * float2.z, float2.z * @float.x * @float.y - float2.x * @float.z, float2.x * float2.z * @float.y + @float.x * @float.z, float2.y * @float.z, float2.x * float2.z + @float.x * @float.y * @float.z, float2.x * @float.y * @float.z - float2.z * @float.x, -@float.y, float2.y * @float.x, float2.x * float2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerXZY(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float3x3(float2.y * float2.z, @float.x * @float.y - float2.x * float2.y * @float.z, float2.x * @float.y + float2.y * @float.x * @float.z, @float.z, float2.x * float2.z, -float2.z * @float.x, -float2.z * @float.y, float2.y * @float.x + float2.x * @float.y * @float.z, float2.x * float2.y - @float.x * @float.y * @float.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerYXZ(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float3x3(float2.y * float2.z - @float.x * @float.y * @float.z, -float2.x * @float.z, float2.z * @float.y + float2.y * @float.x * @float.z, float2.z * @float.x * @float.y + float2.y * @float.z, float2.x * float2.z, @float.y * @float.z - float2.y * float2.z * @float.x, -float2.x * @float.y, @float.x, float2.x * float2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerYZX(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float3x3(float2.y * float2.z, -@float.z, float2.z * @float.y, @float.x * @float.y + float2.x * float2.y * @float.z, float2.x * float2.z, float2.x * @float.y * @float.z - float2.y * @float.x, float2.y * @float.x * @float.z - float2.x * @float.y, float2.z * @float.x, float2.x * float2.y + @float.x * @float.y * @float.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerZXY(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float3x3(float2.y * float2.z + @float.x * @float.y * @float.z, float2.z * @float.x * @float.y - float2.y * @float.z, float2.x * @float.y, float2.x * @float.z, float2.x * float2.z, -@float.x, float2.y * @float.x * @float.z - float2.z * @float.y, float2.y * float2.z * @float.x + @float.y * @float.z, float2.x * float2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerZYX(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(xyz, out @float, out float2);
			return math.float3x3(float2.y * float2.z, -float2.y * @float.z, @float.y, float2.z * @float.x * @float.y + float2.x * @float.z, float2.x * float2.z - @float.x * @float.y * @float.z, -float2.y * @float.x, @float.x * @float.z - float2.x * float2.z * @float.y, float2.z * @float.x + float2.x * @float.y * @float.z, float2.x * float2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerXYZ(float x, float y, float z)
		{
			return float3x3.EulerXYZ(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerXZY(float x, float y, float z)
		{
			return float3x3.EulerXZY(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerYXZ(float x, float y, float z)
		{
			return float3x3.EulerYXZ(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerYZX(float x, float y, float z)
		{
			return float3x3.EulerYZX(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerZXY(float x, float y, float z)
		{
			return float3x3.EulerZXY(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 EulerZYX(float x, float y, float z)
		{
			return float3x3.EulerZYX(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 Euler(float3 xyz, math.RotationOrder order = math.RotationOrder.ZXY)
		{
			switch (order)
			{
			case math.RotationOrder.XYZ:
				return float3x3.EulerXYZ(xyz);
			case math.RotationOrder.XZY:
				return float3x3.EulerXZY(xyz);
			case math.RotationOrder.YXZ:
				return float3x3.EulerYXZ(xyz);
			case math.RotationOrder.YZX:
				return float3x3.EulerYZX(xyz);
			case math.RotationOrder.ZXY:
				return float3x3.EulerZXY(xyz);
			case math.RotationOrder.ZYX:
				return float3x3.EulerZYX(xyz);
			default:
				return float3x3.identity;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 Euler(float x, float y, float z, math.RotationOrder order = math.RotationOrder.ZXY)
		{
			return float3x3.Euler(math.float3(x, y, z), order);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 RotateX(float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			return math.float3x3(1f, 0f, 0f, 0f, num2, -num, 0f, num, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 RotateY(float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			return math.float3x3(num2, 0f, num, 0f, 1f, 0f, -num, 0f, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 RotateZ(float angle)
		{
			float num;
			float num2;
			math.sincos(angle, out num, out num2);
			return math.float3x3(num2, -num, 0f, num, num2, 0f, 0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 Scale(float s)
		{
			return math.float3x3(s, 0f, 0f, 0f, s, 0f, 0f, 0f, s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 Scale(float x, float y, float z)
		{
			return math.float3x3(x, 0f, 0f, 0f, y, 0f, 0f, 0f, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 Scale(float3 v)
		{
			return float3x3.Scale(v.x, v.y, v.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 LookRotation(float3 forward, float3 up)
		{
			float3 @float = math.normalize(math.cross(up, forward));
			return math.float3x3(@float, math.cross(forward, @float), forward);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 LookRotationSafe(float3 forward, float3 up)
		{
			float num = math.dot(forward, forward);
			float num2 = math.dot(up, up);
			forward *= math.rsqrt(num);
			up *= math.rsqrt(num2);
			float3 @float = math.cross(up, forward);
			float num3 = math.dot(@float, @float);
			@float *= math.rsqrt(num3);
			float num4 = math.min(math.min(num, num2), num3);
			float num5 = math.max(math.max(num, num2), num3);
			bool flag = num4 > 1E-35f && num5 < 1E+35f && math.isfinite(num) && math.isfinite(num2) && math.isfinite(num3);
			return math.float3x3(math.select(math.float3(1f, 0f, 0f), @float, flag), math.select(math.float3(0f, 1f, 0f), math.cross(forward, @float), flag), math.select(math.float3(0f, 0f, 1f), forward, flag));
		}

		public static explicit operator float3x3(float4x4 f4x4)
		{
			return new float3x3(f4x4);
		}

		public float3 c0;

		public float3 c1;

		public float3 c2;

		public static readonly float3x3 identity = new float3x3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		public static readonly float3x3 zero;
	}
}
