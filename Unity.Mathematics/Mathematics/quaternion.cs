using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct quaternion : IEquatable<quaternion>, IFormattable
	{
		public static implicit operator Quaternion(quaternion q)
		{
			return new Quaternion(q.value.x, q.value.y, q.value.z, q.value.w);
		}

		public static implicit operator quaternion(Quaternion q)
		{
			return new quaternion(q.x, q.y, q.z, q.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public quaternion(float x, float y, float z, float w)
		{
			this.value.x = x;
			this.value.y = y;
			this.value.z = z;
			this.value.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public quaternion(float4 value)
		{
			this.value = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator quaternion(float4 v)
		{
			return new quaternion(v);
		}

		public quaternion(float3x3 m)
		{
			float3 c = m.c0;
			float3 c2 = m.c1;
			float3 c3 = m.c2;
			uint num = math.asuint(c.x) & 2147483648U;
			float num2 = c2.y + math.asfloat(math.asuint(c3.z) ^ num);
			uint4 @uint = math.uint4((int)num >> 31);
			uint4 uint2 = math.uint4(math.asint(num2) >> 31);
			float num3 = 1f + math.abs(c.x);
			uint4 uint3 = math.uint4(0U, 2147483648U, 2147483648U, 2147483648U) ^ (@uint & math.uint4(0U, 2147483648U, 0U, 2147483648U)) ^ (uint2 & math.uint4(2147483648U, 2147483648U, 2147483648U, 0U));
			this.value = math.float4(num3, c.y, c3.x, c2.z) + math.asfloat(math.asuint(math.float4(num2, c2.x, c.z, c3.y)) ^ uint3);
			this.value = math.asfloat((math.asuint(this.value) & ~@uint) | (math.asuint(this.value.zwxy) & @uint));
			this.value = math.asfloat((math.asuint(this.value.wzyx) & ~uint2) | (math.asuint(this.value) & uint2));
			this.value = math.normalize(this.value);
		}

		public quaternion(float4x4 m)
		{
			float4 c = m.c0;
			float4 c2 = m.c1;
			float4 c3 = m.c2;
			uint num = math.asuint(c.x) & 2147483648U;
			float num2 = c2.y + math.asfloat(math.asuint(c3.z) ^ num);
			uint4 @uint = math.uint4((int)num >> 31);
			uint4 uint2 = math.uint4(math.asint(num2) >> 31);
			float num3 = 1f + math.abs(c.x);
			uint4 uint3 = math.uint4(0U, 2147483648U, 2147483648U, 2147483648U) ^ (@uint & math.uint4(0U, 2147483648U, 0U, 2147483648U)) ^ (uint2 & math.uint4(2147483648U, 2147483648U, 2147483648U, 0U));
			this.value = math.float4(num3, c.y, c3.x, c2.z) + math.asfloat(math.asuint(math.float4(num2, c2.x, c.z, c3.y)) ^ uint3);
			this.value = math.asfloat((math.asuint(this.value) & ~@uint) | (math.asuint(this.value.zwxy) & @uint));
			this.value = math.asfloat((math.asuint(this.value.wzyx) & ~uint2) | (math.asuint(this.value) & uint2));
			this.value = math.normalize(this.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion AxisAngle(float3 axis, float angle)
		{
			float num;
			float num2;
			math.sincos(0.5f * angle, out num, out num2);
			return math.quaternion(math.float4(axis * num, num2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerXYZ(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(0.5f * xyz, out @float, out float2);
			return math.quaternion(math.float4(@float.xyz, float2.x) * float2.yxxy * float2.zzyz + @float.yxxy * @float.zzyz * math.float4(float2.xyz, @float.x) * math.float4(-1f, 1f, -1f, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerXZY(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(0.5f * xyz, out @float, out float2);
			return math.quaternion(math.float4(@float.xyz, float2.x) * float2.yxxy * float2.zzyz + @float.yxxy * @float.zzyz * math.float4(float2.xyz, @float.x) * math.float4(1f, 1f, -1f, -1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerYXZ(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(0.5f * xyz, out @float, out float2);
			return math.quaternion(math.float4(@float.xyz, float2.x) * float2.yxxy * float2.zzyz + @float.yxxy * @float.zzyz * math.float4(float2.xyz, @float.x) * math.float4(-1f, 1f, 1f, -1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerYZX(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(0.5f * xyz, out @float, out float2);
			return math.quaternion(math.float4(@float.xyz, float2.x) * float2.yxxy * float2.zzyz + @float.yxxy * @float.zzyz * math.float4(float2.xyz, @float.x) * math.float4(-1f, -1f, 1f, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerZXY(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(0.5f * xyz, out @float, out float2);
			return math.quaternion(math.float4(@float.xyz, float2.x) * float2.yxxy * float2.zzyz + @float.yxxy * @float.zzyz * math.float4(float2.xyz, @float.x) * math.float4(1f, -1f, -1f, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerZYX(float3 xyz)
		{
			float3 @float;
			float3 float2;
			math.sincos(0.5f * xyz, out @float, out float2);
			return math.quaternion(math.float4(@float.xyz, float2.x) * float2.yxxy * float2.zzyz + @float.yxxy * @float.zzyz * math.float4(float2.xyz, @float.x) * math.float4(1f, -1f, 1f, -1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerXYZ(float x, float y, float z)
		{
			return quaternion.EulerXYZ(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerXZY(float x, float y, float z)
		{
			return quaternion.EulerXZY(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerYXZ(float x, float y, float z)
		{
			return quaternion.EulerYXZ(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerYZX(float x, float y, float z)
		{
			return quaternion.EulerYZX(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerZXY(float x, float y, float z)
		{
			return quaternion.EulerZXY(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion EulerZYX(float x, float y, float z)
		{
			return quaternion.EulerZYX(math.float3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion Euler(float3 xyz, math.RotationOrder order = math.RotationOrder.ZXY)
		{
			switch (order)
			{
			case math.RotationOrder.XYZ:
				return quaternion.EulerXYZ(xyz);
			case math.RotationOrder.XZY:
				return quaternion.EulerXZY(xyz);
			case math.RotationOrder.YXZ:
				return quaternion.EulerYXZ(xyz);
			case math.RotationOrder.YZX:
				return quaternion.EulerYZX(xyz);
			case math.RotationOrder.ZXY:
				return quaternion.EulerZXY(xyz);
			case math.RotationOrder.ZYX:
				return quaternion.EulerZYX(xyz);
			default:
				return quaternion.identity;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion Euler(float x, float y, float z, math.RotationOrder order = math.RotationOrder.ZXY)
		{
			return quaternion.Euler(math.float3(x, y, z), order);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion RotateX(float angle)
		{
			float num;
			float num2;
			math.sincos(0.5f * angle, out num, out num2);
			return math.quaternion(num, 0f, 0f, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion RotateY(float angle)
		{
			float num;
			float num2;
			math.sincos(0.5f * angle, out num, out num2);
			return math.quaternion(0f, num, 0f, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion RotateZ(float angle)
		{
			float num;
			float num2;
			math.sincos(0.5f * angle, out num, out num2);
			return math.quaternion(0f, 0f, num, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion LookRotation(float3 forward, float3 up)
		{
			float3 @float = math.normalize(math.cross(up, forward));
			return math.quaternion(math.float3x3(@float, math.cross(forward, @float), forward));
		}

		public static quaternion LookRotationSafe(float3 forward, float3 up)
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
			return math.quaternion(math.select(math.float4(0f, 0f, 0f, 1f), math.quaternion(math.float3x3(@float, math.cross(forward, @float), forward)).value, flag));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(quaternion x)
		{
			return this.value.x == x.value.x && this.value.y == x.value.y && this.value.z == x.value.z && this.value.w == x.value.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object x)
		{
			if (x is quaternion)
			{
				quaternion quaternion = (quaternion)x;
				return this.Equals(quaternion);
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
			return string.Format("quaternion({0}f, {1}f, {2}f, {3}f)", new object[]
			{
				this.value.x,
				this.value.y,
				this.value.z,
				this.value.w
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("quaternion({0}f, {1}f, {2}f, {3}f)", new object[]
			{
				this.value.x.ToString(format, formatProvider),
				this.value.y.ToString(format, formatProvider),
				this.value.z.ToString(format, formatProvider),
				this.value.w.ToString(format, formatProvider)
			});
		}

		public float4 value;

		public static readonly quaternion identity = new quaternion(0f, 0f, 0f, 1f);
	}
}
