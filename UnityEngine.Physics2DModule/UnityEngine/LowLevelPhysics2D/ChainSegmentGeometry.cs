using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct ChainSegmentGeometry
	{
		public ChainSegmentGeometry()
		{
			this.m_Segment = new SegmentGeometry();
			this.m_Ghost1 = this.m_Segment.point1 * 2f;
			this.m_Ghost2 = this.m_Segment.point2 * 2f;
			this.m_ChainId = 0;
		}

		public ChainSegmentGeometry(SegmentGeometry segmentGeometry, Vector2 ghost1, Vector2 ghost2)
		{
			this.m_Segment = segmentGeometry;
			this.m_Ghost1 = ghost1;
			this.m_Ghost2 = ghost2;
			this.m_ChainId = 0;
		}

		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.ChainSegmentGeometry_IsValid(this);
			}
		}

		public Vector2 ghost1
		{
			readonly get
			{
				return this.m_Ghost1;
			}
			set
			{
				this.m_Ghost1 = value;
			}
		}

		public SegmentGeometry segment
		{
			readonly get
			{
				return this.m_Segment;
			}
			set
			{
				this.m_Segment = value;
			}
		}

		public Vector2 ghost2
		{
			readonly get
			{
				return this.m_Ghost2;
			}
			set
			{
				this.m_Ghost2 = value;
			}
		}

		public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.ChainSegmentGeometry_CalculateAABB(this, transform);
		}

		public readonly Vector2 ClosestPoint(PhysicsTransform transform, Vector2 point)
		{
			return PhysicsLowLevelScripting2D.ChainSegmentGeometry_ClosestPoint(this, transform.TransformPoint(point));
		}

		public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput, bool oneSided)
		{
			return PhysicsLowLevelScripting2D.ChainSegmentGeometry_CastRay(this, castRayInput, oneSided);
		}

		public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
		{
			return PhysicsLowLevelScripting2D.ChainSegmentGeometry_CastShape(this, input);
		}

		public readonly ChainSegmentGeometry Transform(PhysicsTransform transform)
		{
			return new ChainSegmentGeometry
			{
				ghost1 = transform.TransformPoint(this.ghost1),
				segment = this.segment.Transform(transform),
				ghost2 = transform.TransformPoint(this.ghost2)
			};
		}

		public readonly ChainSegmentGeometry InverseTransform(PhysicsTransform transform)
		{
			return new ChainSegmentGeometry
			{
				ghost1 = transform.InverseTransformPoint(this.ghost1),
				segment = this.segment.InverseTransform(transform),
				ghost2 = transform.TransformPoint(this.ghost2)
			};
		}

		public readonly ChainSegmentGeometry Transform(Matrix4x4 transform)
		{
			return new ChainSegmentGeometry
			{
				ghost1 = transform.MultiplyPoint3x4(this.ghost1),
				segment = this.segment.Transform(transform),
				ghost2 = transform.MultiplyPoint3x4(this.ghost2)
			};
		}

		public readonly ChainSegmentGeometry InverseTransform(Matrix4x4 transform)
		{
			transform = transform.inverse;
			return new ChainSegmentGeometry
			{
				ghost1 = transform.MultiplyPoint3x4(this.ghost1),
				segment = this.segment.Transform(transform),
				ghost2 = transform.MultiplyPoint3x4(this.ghost2)
			};
		}

		public static readonly ChainSegmentGeometry defaultGeometry = new ChainSegmentGeometry
		{
			segment = SegmentGeometry.defaultGeometry,
			ghost1 = SegmentGeometry.defaultGeometry.point1 * 2f,
			ghost2 = SegmentGeometry.defaultGeometry.point2 * 2f
		};

		[SerializeField]
		private Vector2 m_Ghost1;

		[SerializeField]
		private SegmentGeometry m_Segment;

		[SerializeField]
		private Vector2 m_Ghost2;

		private readonly int m_ChainId;
	}
}
