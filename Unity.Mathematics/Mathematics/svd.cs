using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	public static class svd
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void condSwap(bool c, ref float x, ref float y)
		{
			float num = x;
			x = math.select(x, y, c);
			y = math.select(y, num, c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void condNegSwap(bool c, ref float3 x, ref float3 y)
		{
			float3 @float = -x;
			x = math.select(x, y, c);
			y = math.select(y, @float, c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static quaternion condNegSwapQuat(bool c, quaternion q, float4 mask)
		{
			return math.mul(q, math.select(quaternion.identity.value, mask * 0.70710677f, c));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void sortSingularValues(ref float3x3 b, ref quaternion v)
		{
			float num = math.lengthsq(b.c0);
			float num2 = math.lengthsq(b.c1);
			float num3 = math.lengthsq(b.c2);
			bool flag = num < num2;
			svd.condNegSwap(flag, ref b.c0, ref b.c1);
			v = svd.condNegSwapQuat(flag, v, math.float4(0f, 0f, 1f, 1f));
			svd.condSwap(flag, ref num, ref num2);
			flag = num < num3;
			svd.condNegSwap(flag, ref b.c0, ref b.c2);
			v = svd.condNegSwapQuat(flag, v, math.float4(0f, -1f, 0f, 1f));
			svd.condSwap(flag, ref num, ref num3);
			flag = num2 < num3;
			svd.condNegSwap(flag, ref b.c1, ref b.c2);
			v = svd.condNegSwapQuat(flag, v, math.float4(1f, 0f, 0f, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static quaternion approxGivensQuat(float3 pq, float4 mask)
		{
			float num = 2f * (pq.x - pq.y);
			float z = pq.z;
			return math.normalize(math.select(math.float4(0.38268343f, 0.38268343f, 0.38268343f, 0.9238795f), math.float4(z, z, z, num), 5.8284273f * z * z < num * num) * mask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static quaternion qrGivensQuat(float2 pq, float4 mask)
		{
			float num = math.sqrt(pq.x * pq.x + pq.y * pq.y);
			float num2 = math.select(0f, pq.y, num > 1E-15f);
			float num3 = math.abs(pq.x) + math.max(num, 1E-15f);
			svd.condSwap(pq.x < 0f, ref num2, ref num3);
			return math.normalize(math.float4(num2, num2, num2, num3) * mask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static quaternion givensQRFactorization(float3x3 b, out float3x3 r)
		{
			quaternion quaternion = svd.qrGivensQuat(math.float2(b.c0.x, b.c0.y), math.float4(0f, 0f, 1f, 1f));
			float3x3 float3x = math.float3x3(math.conjugate(quaternion));
			r = math.mul(float3x, b);
			quaternion quaternion2 = svd.qrGivensQuat(math.float2(r.c0.x, r.c0.z), math.float4(0f, -1f, 0f, 1f));
			quaternion quaternion3 = math.mul(quaternion, quaternion2);
			float3x = math.float3x3(math.conjugate(quaternion2));
			r = math.mul(float3x, r);
			quaternion2 = svd.qrGivensQuat(math.float2(r.c1.y, r.c1.z), math.float4(1f, 0f, 0f, 1f));
			quaternion quaternion4 = math.mul(quaternion3, quaternion2);
			float3x = math.float3x3(math.conjugate(quaternion2));
			r = math.mul(float3x, r);
			return quaternion4;
		}

		private static quaternion jacobiIteration(ref float3x3 s, int iterations = 5)
		{
			quaternion quaternion = quaternion.identity;
			for (int i = 0; i < iterations; i++)
			{
				quaternion quaternion2 = svd.approxGivensQuat(math.float3(s.c0.x, s.c1.y, s.c0.y), math.float4(0f, 0f, 1f, 1f));
				quaternion = math.mul(quaternion, quaternion2);
				float3x3 float3x = math.float3x3(quaternion2);
				s = math.mul(math.mul(math.transpose(float3x), s), float3x);
				quaternion2 = svd.approxGivensQuat(math.float3(s.c1.y, s.c2.z, s.c1.z), math.float4(1f, 0f, 0f, 1f));
				quaternion = math.mul(quaternion, quaternion2);
				float3x = math.float3x3(quaternion2);
				s = math.mul(math.mul(math.transpose(float3x), s), float3x);
				quaternion2 = svd.approxGivensQuat(math.float3(s.c2.z, s.c0.x, s.c2.x), math.float4(0f, 1f, 0f, 1f));
				quaternion = math.mul(quaternion, quaternion2);
				float3x = math.float3x3(quaternion2);
				s = math.mul(math.mul(math.transpose(float3x), s), float3x);
			}
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float3 singularValuesDecomposition(float3x3 a, out quaternion u, out quaternion v)
		{
			u = quaternion.identity;
			v = quaternion.identity;
			float3x3 float3x = math.mul(math.transpose(a), a);
			v = svd.jacobiIteration(ref float3x, 5);
			float3x3 float3x2 = math.float3x3(v);
			float3x2 = math.mul(a, float3x2);
			svd.sortSingularValues(ref float3x2, ref v);
			float3x3 float3x3;
			u = svd.givensQRFactorization(float3x2, out float3x3);
			return math.float3(float3x3.c0.x, float3x3.c1.y, float3x3.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float3 rcpsafe(float3 x, float epsilon = 1E-09f)
		{
			return math.select(math.rcp(x), float3.zero, math.abs(x) < epsilon);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 svdInverse(float3x3 a)
		{
			quaternion quaternion;
			quaternion quaternion2;
			float3 @float = svd.singularValuesDecomposition(a, out quaternion, out quaternion2);
			float3x3 float3x = math.float3x3(quaternion);
			return math.mul(math.float3x3(quaternion2), math.scaleMul(svd.rcpsafe(@float, 1E-06f), math.transpose(float3x)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion svdRotation(float3x3 a)
		{
			quaternion quaternion;
			quaternion quaternion2;
			svd.singularValuesDecomposition(a, out quaternion, out quaternion2);
			return math.mul(quaternion, math.conjugate(quaternion2));
		}

		public const float k_EpsilonDeterminant = 1E-06f;

		public const float k_EpsilonRCP = 1E-09f;

		public const float k_EpsilonNormalSqrt = 1E-15f;

		public const float k_EpsilonNormal = 1E-30f;
	}
}
