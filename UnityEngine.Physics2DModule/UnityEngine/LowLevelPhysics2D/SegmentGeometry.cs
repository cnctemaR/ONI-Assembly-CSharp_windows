using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct SegmentGeometry
	{
		public SegmentGeometry()
		{
			this.m_Point1 = Vector2.right * 0.5f;
			this.m_Point2 = Vector2.left * 0.5f;
		}

		public static SegmentGeometry Create(Vector2 point1, Vector2 point2)
		{
			return new SegmentGeometry
			{
				point1 = point1,
				point2 = point2
			};
		}

		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.SegmentGeometry_IsValid(this);
			}
		}

		public Vector2 point1
		{
			readonly get
			{
				return this.m_Point1;
			}
			set
			{
				this.m_Point1 = value;
			}
		}

		public Vector2 point2
		{
			readonly get
			{
				return this.m_Point2;
			}
			set
			{
				this.m_Point2 = value;
			}
		}

		public readonly Vector2 midPoint
		{
			get
			{
				return (this.point1 + this.point2) * 0.5f;
			}
		}

		public readonly Vector2 forward
		{
			get
			{
				return this.point2 - this.point1;
			}
		}

		public readonly Vector2 backward
		{
			get
			{
				return this.point1 - this.point2;
			}
		}

		public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.SegmentGeometry_CalculateAABB(this, transform);
		}

		public readonly Vector2 ClosestPoint(PhysicsTransform transform, Vector2 point)
		{
			return PhysicsLowLevelScripting2D.SegmentGeometry_ClosestPoint(this, transform.TransformPoint(point));
		}

		public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput, bool oneSided = false)
		{
			return PhysicsLowLevelScripting2D.SegmentGeometry_CastRay(this, castRayInput, oneSided);
		}

		public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
		{
			return PhysicsLowLevelScripting2D.SegmentGeometry_CastShape(this, input);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CircleGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.SegmentAndCircle(this, transform, otherGeometry, otherTransform);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CapsuleGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.SegmentAndCapsule(this, transform, otherGeometry, otherTransform);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, PolygonGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.SegmentAndPolygon(this, transform, otherGeometry, otherTransform);
		}

		public readonly SegmentGeometry Transform(PhysicsTransform transform)
		{
			return new SegmentGeometry
			{
				point1 = transform.TransformPoint(this.point1),
				point2 = transform.TransformPoint(this.point2)
			};
		}

		public readonly SegmentGeometry InverseTransform(PhysicsTransform transform)
		{
			return new SegmentGeometry
			{
				point1 = transform.InverseTransformPoint(this.point1),
				point2 = transform.InverseTransformPoint(this.point2)
			};
		}

		public readonly SegmentGeometry Transform(Matrix4x4 transform)
		{
			return new SegmentGeometry
			{
				point1 = transform.MultiplyPoint3x4(this.point1),
				point2 = transform.MultiplyPoint3x4(this.point2)
			};
		}

		public readonly SegmentGeometry InverseTransform(Matrix4x4 transform)
		{
			transform = transform.inverse;
			return new SegmentGeometry
			{
				point1 = transform.MultiplyPoint3x4(this.point1),
				point2 = transform.MultiplyPoint3x4(this.point2)
			};
		}

		public readonly SegmentGeometry Scale(float scale)
		{
			Vector2 midPoint = this.midPoint;
			Vector2 vector = this.forward * 0.5f * scale;
			return new SegmentGeometry
			{
				point1 = this.midPoint - vector,
				point2 = this.midPoint + vector
			};
		}

		public static readonly SegmentGeometry defaultGeometry = new SegmentGeometry
		{
			point1 = Vector2.right * 0.5f,
			point2 = Vector2.left * 0.5f
		};

		[SerializeField]
		private Vector2 m_Point1;

		[SerializeField]
		private Vector2 m_Point2;
	}
}
