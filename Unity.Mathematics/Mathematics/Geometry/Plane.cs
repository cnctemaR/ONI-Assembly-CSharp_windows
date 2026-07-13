using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics.Geometry
{
	[DebuggerDisplay("{Normal}, {Distance}")]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct Plane
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(float coefficientA, float coefficientB, float coefficientC, float coefficientD)
		{
			this.NormalAndDistance = Plane.Normalize(new float4(coefficientA, coefficientB, coefficientC, coefficientD));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(float3 normal, float distance)
		{
			this.NormalAndDistance = Plane.Normalize(new float4(normal, distance));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(float3 normal, float3 pointInPlane)
		{
			this = new Plane(normal, -math.dot(normal, pointInPlane));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(float3 vector1InPlane, float3 vector2InPlane, float3 pointInPlane)
		{
			this = new Plane(math.cross(vector1InPlane, vector2InPlane), pointInPlane);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Plane CreateFromUnitNormalAndDistance(float3 unitNormal, float distance)
		{
			return new Plane
			{
				NormalAndDistance = new float4(unitNormal, distance)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Plane CreateFromUnitNormalAndPointInPlane(float3 unitNormal, float3 pointInPlane)
		{
			return new Plane
			{
				NormalAndDistance = new float4(unitNormal, -math.dot(unitNormal, pointInPlane))
			};
		}

		public float3 Normal
		{
			get
			{
				return this.NormalAndDistance.xyz;
			}
			set
			{
				this.NormalAndDistance.xyz = value;
			}
		}

		public float Distance
		{
			get
			{
				return this.NormalAndDistance.w;
			}
			set
			{
				this.NormalAndDistance.w = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Plane Normalize(Plane plane)
		{
			return new Plane
			{
				NormalAndDistance = Plane.Normalize(plane.NormalAndDistance)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 Normalize(float4 planeCoefficients)
		{
			float num = math.rsqrt(math.lengthsq(planeCoefficients.xyz));
			return new Plane
			{
				NormalAndDistance = planeCoefficients * num
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float SignedDistanceToPoint(float3 point)
		{
			return math.dot(this.NormalAndDistance, new float4(point, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3 Projection(float3 point)
		{
			return point - this.Normal * this.SignedDistanceToPoint(point);
		}

		public Plane Flipped
		{
			get
			{
				return new Plane
				{
					NormalAndDistance = -this.NormalAndDistance
				};
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4(Plane plane)
		{
			return plane.NormalAndDistance;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckPlaneIsNormalized()
		{
			float num = math.lengthsq(this.Normal.xyz);
			if (num < 0.99800104f || num > 1.002001f)
			{
				throw new ArgumentException("Plane must be normalized. Call Plane.Normalize() to normalize plane.");
			}
		}

		public float4 NormalAndDistance;
	}
}
