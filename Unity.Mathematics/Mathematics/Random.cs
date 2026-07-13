using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct Random
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Random(uint seed)
		{
			this.state = seed;
			this.NextState();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Random CreateFromIndex(uint index)
		{
			return new Random(Random.WangHash(index + 62U));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint WangHash(uint n)
		{
			n = n ^ 61U ^ (n >> 16);
			n *= 9U;
			n ^= n >> 4;
			n *= 668265261U;
			n ^= n >> 15;
			return n;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InitState(uint seed = 1851936439U)
		{
			this.state = seed;
			this.NextState();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool NextBool()
		{
			return (this.NextState() & 1U) == 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool2 NextBool2()
		{
			return (math.uint2(this.NextState()) & math.uint2(1U, 2U)) == 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool3 NextBool3()
		{
			return (math.uint3(this.NextState()) & math.uint3(1U, 2U, 4U)) == 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4 NextBool4()
		{
			return (math.uint4(this.NextState()) & math.uint4(1U, 2U, 4U, 8U)) == 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int NextInt()
		{
			return (int)(this.NextState() ^ 2147483648U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2 NextInt2()
		{
			return math.int2((int)this.NextState(), (int)this.NextState()) ^ int.MinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 NextInt3()
		{
			return math.int3((int)this.NextState(), (int)this.NextState(), (int)this.NextState()) ^ int.MinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4 NextInt4()
		{
			return math.int4((int)this.NextState(), (int)this.NextState(), (int)this.NextState(), (int)this.NextState()) ^ int.MinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int NextInt(int max)
		{
			return (int)((ulong)this.NextState() * (ulong)((long)max) >> 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2 NextInt2(int2 max)
		{
			return math.int2((int)((ulong)this.NextState() * (ulong)((long)max.x) >> 32), (int)((ulong)this.NextState() * (ulong)((long)max.y) >> 32));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 NextInt3(int3 max)
		{
			return math.int3((int)((ulong)this.NextState() * (ulong)((long)max.x) >> 32), (int)((ulong)this.NextState() * (ulong)((long)max.y) >> 32), (int)((ulong)this.NextState() * (ulong)((long)max.z) >> 32));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4 NextInt4(int4 max)
		{
			return math.int4((int)((ulong)this.NextState() * (ulong)((long)max.x) >> 32), (int)((ulong)this.NextState() * (ulong)((long)max.y) >> 32), (int)((ulong)this.NextState() * (ulong)((long)max.z) >> 32), (int)((ulong)this.NextState() * (ulong)((long)max.w) >> 32));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int NextInt(int min, int max)
		{
			uint num = (uint)(max - min);
			return (int)((ulong)this.NextState() * (ulong)num >> 32) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int2 NextInt2(int2 min, int2 max)
		{
			uint2 @uint = (uint2)(max - min);
			return math.int2((int)((ulong)this.NextState() * (ulong)@uint.x >> 32), (int)((ulong)this.NextState() * (ulong)@uint.y >> 32)) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 NextInt3(int3 min, int3 max)
		{
			uint3 @uint = (uint3)(max - min);
			return math.int3((int)((ulong)this.NextState() * (ulong)@uint.x >> 32), (int)((ulong)this.NextState() * (ulong)@uint.y >> 32), (int)((ulong)this.NextState() * (ulong)@uint.z >> 32)) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int4 NextInt4(int4 min, int4 max)
		{
			uint4 @uint = (uint4)(max - min);
			return math.int4((int)((ulong)this.NextState() * (ulong)@uint.x >> 32), (int)((ulong)this.NextState() * (ulong)@uint.y >> 32), (int)((ulong)this.NextState() * (ulong)@uint.z >> 32), (int)((ulong)this.NextState() * (ulong)@uint.w >> 32)) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint NextUInt()
		{
			return this.NextState() - 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2 NextUInt2()
		{
			return math.uint2(this.NextState(), this.NextState()) - 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3 NextUInt3()
		{
			return math.uint3(this.NextState(), this.NextState(), this.NextState()) - 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4 NextUInt4()
		{
			return math.uint4(this.NextState(), this.NextState(), this.NextState(), this.NextState()) - 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint NextUInt(uint max)
		{
			return (uint)((ulong)this.NextState() * (ulong)max >> 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2 NextUInt2(uint2 max)
		{
			return math.uint2((uint)((ulong)this.NextState() * (ulong)max.x >> 32), (uint)((ulong)this.NextState() * (ulong)max.y >> 32));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3 NextUInt3(uint3 max)
		{
			return math.uint3((uint)((ulong)this.NextState() * (ulong)max.x >> 32), (uint)((ulong)this.NextState() * (ulong)max.y >> 32), (uint)((ulong)this.NextState() * (ulong)max.z >> 32));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4 NextUInt4(uint4 max)
		{
			return math.uint4((uint)((ulong)this.NextState() * (ulong)max.x >> 32), (uint)((ulong)this.NextState() * (ulong)max.y >> 32), (uint)((ulong)this.NextState() * (ulong)max.z >> 32), (uint)((ulong)this.NextState() * (ulong)max.w >> 32));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint NextUInt(uint min, uint max)
		{
			uint num = max - min;
			return (uint)((ulong)this.NextState() * (ulong)num >> 32) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint2 NextUInt2(uint2 min, uint2 max)
		{
			uint2 @uint = max - min;
			return math.uint2((uint)((ulong)this.NextState() * (ulong)@uint.x >> 32), (uint)((ulong)this.NextState() * (ulong)@uint.y >> 32)) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint3 NextUInt3(uint3 min, uint3 max)
		{
			uint3 @uint = max - min;
			return math.uint3((uint)((ulong)this.NextState() * (ulong)@uint.x >> 32), (uint)((ulong)this.NextState() * (ulong)@uint.y >> 32), (uint)((ulong)this.NextState() * (ulong)@uint.z >> 32)) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint4 NextUInt4(uint4 min, uint4 max)
		{
			uint4 @uint = max - min;
			return math.uint4((uint)((ulong)this.NextState() * (ulong)@uint.x >> 32), (uint)((ulong)this.NextState() * (ulong)@uint.y >> 32), (uint)((ulong)this.NextState() * (ulong)@uint.z >> 32), (uint)((ulong)this.NextState() * (ulong)@uint.w >> 32)) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float NextFloat()
		{
			return math.asfloat(1065353216U | (this.NextState() >> 9)) - 1f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2 NextFloat2()
		{
			return math.asfloat(1065353216U | (math.uint2(this.NextState(), this.NextState()) >> 9)) - 1f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3 NextFloat3()
		{
			return math.asfloat(1065353216U | (math.uint3(this.NextState(), this.NextState(), this.NextState()) >> 9)) - 1f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4 NextFloat4()
		{
			return math.asfloat(1065353216U | (math.uint4(this.NextState(), this.NextState(), this.NextState(), this.NextState()) >> 9)) - 1f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float NextFloat(float max)
		{
			return this.NextFloat() * max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2 NextFloat2(float2 max)
		{
			return this.NextFloat2() * max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3 NextFloat3(float3 max)
		{
			return this.NextFloat3() * max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4 NextFloat4(float4 max)
		{
			return this.NextFloat4() * max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float NextFloat(float min, float max)
		{
			return this.NextFloat() * (max - min) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2 NextFloat2(float2 min, float2 max)
		{
			return this.NextFloat2() * (max - min) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3 NextFloat3(float3 min, float3 max)
		{
			return this.NextFloat3() * (max - min) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4 NextFloat4(float4 min, float4 max)
		{
			return this.NextFloat4() * (max - min) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double NextDouble()
		{
			ulong num = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			return math.asdouble(4607182418800017408UL | num) - 1.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2 NextDouble2()
		{
			ulong num = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			ulong num2 = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			return math.double2(math.asdouble(4607182418800017408UL | num), math.asdouble(4607182418800017408UL | num2)) - 1.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3 NextDouble3()
		{
			ulong num = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			ulong num2 = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			ulong num3 = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			return math.double3(math.asdouble(4607182418800017408UL | num), math.asdouble(4607182418800017408UL | num2), math.asdouble(4607182418800017408UL | num3)) - 1.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4 NextDouble4()
		{
			ulong num = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			ulong num2 = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			ulong num3 = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			ulong num4 = ((ulong)this.NextState() << 20) ^ (ulong)this.NextState();
			return math.double4(math.asdouble(4607182418800017408UL | num), math.asdouble(4607182418800017408UL | num2), math.asdouble(4607182418800017408UL | num3), math.asdouble(4607182418800017408UL | num4)) - 1.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double NextDouble(double max)
		{
			return this.NextDouble() * max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2 NextDouble2(double2 max)
		{
			return this.NextDouble2() * max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3 NextDouble3(double3 max)
		{
			return this.NextDouble3() * max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4 NextDouble4(double4 max)
		{
			return this.NextDouble4() * max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double NextDouble(double min, double max)
		{
			return this.NextDouble() * (max - min) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2 NextDouble2(double2 min, double2 max)
		{
			return this.NextDouble2() * (max - min) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3 NextDouble3(double3 min, double3 max)
		{
			return this.NextDouble3() * (max - min) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double4 NextDouble4(double4 min, double4 max)
		{
			return this.NextDouble4() * (max - min) + min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2 NextFloat2Direction()
		{
			float num;
			float num2;
			math.sincos(this.NextFloat() * 3.1415927f * 2f, out num, out num2);
			return math.float2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double2 NextDouble2Direction()
		{
			double num;
			double num2;
			math.sincos(this.NextDouble() * 3.141592653589793 * 2.0, out num, out num2);
			return math.double2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3 NextFloat3Direction()
		{
			float2 @float = this.NextFloat2();
			float num = @float.x * 2f - 1f;
			float num2 = math.sqrt(math.max(1f - num * num, 0f));
			float num3;
			float num4;
			math.sincos(@float.y * 3.1415927f * 2f, out num3, out num4);
			return math.float3(num4 * num2, num3 * num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double3 NextDouble3Direction()
		{
			double2 @double = this.NextDouble2();
			double num = @double.x * 2.0 - 1.0;
			double num2 = math.sqrt(math.max(1.0 - num * num, 0.0));
			double num3;
			double num4;
			math.sincos(@double.y * 3.141592653589793 * 2.0, out num3, out num4);
			return math.double3(num4 * num2, num3 * num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public quaternion NextQuaternionRotation()
		{
			float3 @float = this.NextFloat3(math.float3(6.2831855f, 6.2831855f, 1f));
			float z = @float.z;
			float2 xy = @float.xy;
			float num = math.sqrt(1f - z);
			float num2 = math.sqrt(z);
			float2 float2;
			float2 float3;
			math.sincos(xy, out float2, out float3);
			quaternion quaternion = math.quaternion(num * float2.x, num * float3.x, num2 * float2.y, num2 * float3.y);
			return math.quaternion(math.select(quaternion.value, -quaternion.value, quaternion.value.w < 0f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private uint NextState()
		{
			uint num = this.state;
			this.state ^= this.state << 13;
			this.state ^= this.state >> 17;
			this.state ^= this.state << 5;
			return num;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckInitState()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckIndexForHash(uint index)
		{
			if (index == 4294967295U)
			{
				throw new ArgumentException("Index must not be uint.MaxValue");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckState()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckNextIntMax(int max)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckNextIntMinMax(int min, int max)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckNextUIntMinMax(uint min, uint max)
		{
		}

		public uint state;
	}
}
