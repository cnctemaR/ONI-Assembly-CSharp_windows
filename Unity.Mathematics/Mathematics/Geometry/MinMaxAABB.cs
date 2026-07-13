using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics.Geometry
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	internal struct MinMaxAABB : IEquatable<MinMaxAABB>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public MinMaxAABB(float3 min, float3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MinMaxAABB CreateFromCenterAndExtents(float3 center, float3 extents)
		{
			return MinMaxAABB.CreateFromCenterAndHalfExtents(center, extents * 0.5f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MinMaxAABB CreateFromCenterAndHalfExtents(float3 center, float3 halfExtents)
		{
			return new MinMaxAABB(center - halfExtents, center + halfExtents);
		}

		public float3 Extents
		{
			get
			{
				return this.Max - this.Min;
			}
		}

		public float3 HalfExtents
		{
			get
			{
				return (this.Max - this.Min) * 0.5f;
			}
		}

		public float3 Center
		{
			get
			{
				return (this.Max + this.Min) * 0.5f;
			}
		}

		public bool IsValid
		{
			get
			{
				return math.all(this.Min <= this.Max);
			}
		}

		public float SurfaceArea
		{
			get
			{
				float3 @float = this.Max - this.Min;
				return 2f * math.dot(@float, @float.yzx);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(float3 point)
		{
			return math.all((point >= this.Min) & (point <= this.Max));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(MinMaxAABB aabb)
		{
			return math.all((this.Min <= aabb.Min) & (this.Max >= aabb.Max));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Overlaps(MinMaxAABB aabb)
		{
			return math.all((this.Max >= aabb.Min) & (this.Min <= aabb.Max));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Expand(float signedDistance)
		{
			this.Min -= signedDistance;
			this.Max += signedDistance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Encapsulate(MinMaxAABB aabb)
		{
			this.Min = math.min(this.Min, aabb.Min);
			this.Max = math.max(this.Max, aabb.Max);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Encapsulate(float3 point)
		{
			this.Min = math.min(this.Min, point);
			this.Max = math.max(this.Max, point);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(MinMaxAABB other)
		{
			return this.Min.Equals(other.Min) && this.Max.Equals(other.Max);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return string.Format("MinMaxAABB({0}, {1})", this.Min, this.Max);
		}

		public float3 Min;

		public float3 Max;
	}
}
