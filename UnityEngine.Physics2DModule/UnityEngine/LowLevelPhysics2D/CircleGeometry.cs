using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct CircleGeometry
	{
		public CircleGeometry()
		{
			this.m_Center = Vector2.zero;
			this.m_Radius = 0.5f;
		}

		public static CircleGeometry Create(float radius)
		{
			return new CircleGeometry
			{
				center = Vector2.zero,
				radius = radius
			};
		}

		public static CircleGeometry Create(float radius, Vector2 center)
		{
			return new CircleGeometry
			{
				center = center,
				radius = radius
			};
		}

		public readonly bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.CircleGeometry_IsValid(this);
			}
		}

		public Vector2 center
		{
			readonly get
			{
				return this.m_Center;
			}
			set
			{
				this.m_Center = value;
			}
		}

		public float radius
		{
			readonly get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = Mathf.Max(0f, value);
			}
		}

		public readonly PhysicsBody.MassConfiguration CalculateMassConfiguration(float density = 1f)
		{
			return PhysicsLowLevelScripting2D.CircleGeometry_CalculateMassConfiguration(this, density);
		}

		public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
		{
			return PhysicsLowLevelScripting2D.CircleGeometry_CalculateAABB(this, transform);
		}

		public readonly bool OverlapPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.CircleGeometry_OverlapPoint(this, point);
		}

		public readonly Vector2 ClosestPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.CircleGeometry_ClosestPoint(this, point);
		}

		public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput)
		{
			return PhysicsLowLevelScripting2D.CircleGeometry_CastRay(this, castRayInput);
		}

		public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
		{
			return PhysicsLowLevelScripting2D.CircleGeometry_CastShape(this, input);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CircleGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.CircleAndCircle(this, transform, otherGeometry, otherTransform);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CapsuleGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.CapsuleAndCircle(otherGeometry, otherTransform, this, transform);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, PolygonGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.PolygonAndCircle(otherGeometry, otherTransform, this, transform);
		}

		public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, SegmentGeometry otherGeometry, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.SegmentAndCircle(otherGeometry, otherTransform, this, transform);
		}

		public readonly CircleGeometry Transform(PhysicsTransform transform)
		{
			return new CircleGeometry
			{
				center = transform.TransformPoint(this.center),
				radius = this.radius
			};
		}

		public readonly CircleGeometry InverseTransform(PhysicsTransform transform)
		{
			return new CircleGeometry
			{
				center = transform.InverseTransformPoint(this.center),
				radius = this.radius
			};
		}

		public readonly CircleGeometry Transform(Matrix4x4 transform, bool scaleRadius)
		{
			return new CircleGeometry
			{
				center = transform.MultiplyPoint3x4(this.center),
				radius = (scaleRadius ? (PhysicsMath.MaxAbsComponent(transform.lossyScale) * this.radius) : this.radius)
			};
		}

		public readonly CircleGeometry InverseTransform(Matrix4x4 transform, bool scaleRadius)
		{
			transform = transform.inverse;
			return new CircleGeometry
			{
				center = transform.MultiplyPoint3x4(this.center),
				radius = (scaleRadius ? (PhysicsMath.MinAbsComponent(transform.lossyScale) * this.radius) : this.radius)
			};
		}

		public static readonly CircleGeometry defaultGeometry = new CircleGeometry
		{
			center = Vector2.zero,
			radius = 0.5f
		};

		[SerializeField]
		private Vector2 m_Center;

		[Min(0f)]
		[SerializeField]
		private float m_Radius;
	}
}
