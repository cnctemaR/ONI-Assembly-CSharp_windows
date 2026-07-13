using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsAABB
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PhysicsAABB(Vector2 lowerBound, Vector2 upperBound)
		{
			this.m_LowerBound = lowerBound;
			this.m_UpperBound = upperBound;
		}

		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsAABB_IsValid(this);
			}
		}

		public Vector2 lowerBound
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_LowerBound;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_LowerBound = value;
			}
		}

		public Vector2 upperBound
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_UpperBound;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_UpperBound = value;
			}
		}

		public readonly PhysicsAABB normalized
		{
			get
			{
				return new PhysicsAABB
				{
					lowerBound = Vector2.Min(this.lowerBound, this.upperBound),
					upperBound = Vector2.Max(this.lowerBound, this.upperBound)
				};
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalized()
		{
			this = this.normalized;
		}

		public readonly bool OverlapPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.PhysicsAABB_OverlapPoint(this, point);
		}

		public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput)
		{
			return PhysicsLowLevelScripting2D.PhysicsAABB_CastRay(this, castRayInput);
		}

		public readonly bool Overlap(PhysicsAABB aabb)
		{
			return PhysicsLowLevelScripting2D.PhysicsAABB_Overlap(this, aabb);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlap(Vector2 point)
		{
			return this.OverlapPoint(point);
		}

		public readonly PhysicsAABB Union(PhysicsAABB aabb)
		{
			return PhysicsLowLevelScripting2D.PhysicsAABB_Union(this, aabb);
		}

		public readonly bool Contains(PhysicsAABB aabb)
		{
			return PhysicsLowLevelScripting2D.PhysicsAABB_Contains(this, aabb);
		}

		public readonly Vector2 center
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsAABB_Center(this);
			}
		}

		public readonly Vector2 extents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsAABB_Extents(this);
			}
		}

		public readonly float perimeter
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsAABB_Perimeter(this);
			}
		}

		public override readonly string ToString()
		{
			return string.Format("lowerBound={0}, upperBound={1}, isValid={2}", this.lowerBound, this.upperBound, this.isValid);
		}

		[SerializeField]
		private Vector2 m_LowerBound;

		[SerializeField]
		private Vector2 m_UpperBound;
	}
}
