using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.LowLevelPhysics2D
{
	public struct ChainGeometry
	{
		public ChainGeometry(NativeArray<Vector2> vertices)
		{
			bool flag = vertices.Length < 4;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("vertices", "Chain Geometry must contain a minimum of 4 vertices.");
			}
			this.m_Points = new IntPtr(vertices.GetUnsafeReadOnlyPtr<Vector2>());
			this.m_Count = vertices.Length;
		}

		public unsafe ChainGeometry(ReadOnlySpan<Vector2> vertices)
		{
			bool flag = vertices.Length < 4;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("vertices", "Chain Geometry must contain a minimum of 4 vertices.");
			}
			fixed (Vector2* pinnableReference = vertices.GetPinnableReference())
			{
				Vector2* ptr = pinnableReference;
				this.m_Points = new IntPtr((void*)ptr);
				this.m_Count = vertices.Length;
			}
		}

		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.ChainGeometry_IsValid(this);
			}
		}

		public readonly ReadOnlySpan<Vector2> vertices
		{
			get
			{
				return new ReadOnlySpan<Vector2>(this.m_Points.ToPointer(), this.m_Count);
			}
		}

		public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.ChainGeometry_CalculateAABB(this, transform);
		}

		public readonly Vector2 ClosestPoint(PhysicsTransform transform, Vector2 point)
		{
			return PhysicsLowLevelScripting2D.ChainGeometry_ClosestPoint(this, transform.TransformPoint(point));
		}

		public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput, bool oneSided = true)
		{
			return PhysicsLowLevelScripting2D.ChainGeometry_CastRay(this, castRayInput, oneSided);
		}

		public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
		{
			return PhysicsLowLevelScripting2D.ChainGeometry_CastShape(this, input);
		}

		private IntPtr m_Points;

		private int m_Count;
	}
}
