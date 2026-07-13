using System;
using System.Runtime.CompilerServices;

namespace Unity.Mathematics.Geometry
{
	public static class Math
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MinMaxAABB Transform(RigidTransform transform, MinMaxAABB aabb)
		{
			float3 halfExtents = aabb.HalfExtents;
			float3 @float = math.rotate(transform.rot, new float3(halfExtents.x, 0f, 0f));
			float3 float2 = math.rotate(transform.rot, new float3(0f, halfExtents.y, 0f));
			float3 float3 = math.rotate(transform.rot, new float3(0f, 0f, halfExtents.z));
			float3 float4 = math.abs(@float) + math.abs(float2) + math.abs(float3);
			float3 float5 = math.transform(transform, aabb.Center);
			return new MinMaxAABB(float5 - float4, float5 + float4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MinMaxAABB Transform(float4x4 transform, MinMaxAABB aabb)
		{
			MinMaxAABB minMaxAABB = Math.Transform(new float3x3(transform), aabb);
			minMaxAABB.Min += transform.c3.xyz;
			minMaxAABB.Max += transform.c3.xyz;
			return minMaxAABB;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MinMaxAABB Transform(float3x3 transform, MinMaxAABB aabb)
		{
			float3 @float = transform.c0.xyz * aabb.Min.xxx;
			float3 float2 = transform.c0.xyz * aabb.Max.xxx;
			bool3 @bool = @float < float2;
			MinMaxAABB minMaxAABB = new MinMaxAABB(math.select(float2, @float, @bool), math.select(float2, @float, !@bool));
			@float = transform.c1.xyz * aabb.Min.yyy;
			float2 = transform.c1.xyz * aabb.Max.yyy;
			@bool = @float < float2;
			minMaxAABB.Min += math.select(float2, @float, @bool);
			minMaxAABB.Max += math.select(float2, @float, !@bool);
			@float = transform.c2.xyz * aabb.Min.zzz;
			float2 = transform.c2.xyz * aabb.Max.zzz;
			@bool = @float < float2;
			minMaxAABB.Min += math.select(float2, @float, @bool);
			minMaxAABB.Max += math.select(float2, @float, !@bool);
			return minMaxAABB;
		}
	}
}
