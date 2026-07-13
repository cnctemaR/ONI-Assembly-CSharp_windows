using System;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsQuery
	{
		public static PhysicsShape.ContactManifold ShapeAndShape(PhysicsShape shapeA, PhysicsTransform transformA, PhysicsShape shapeB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_ShapeAndShape(shapeA, transformA, shapeB, transformB);
		}

		public static PhysicsShape.ContactManifold CircleAndCircle(CircleGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_CircleAndCircle(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold CapsuleAndCircle(CapsuleGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_CapsuleAndCircle(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold SegmentAndCircle(SegmentGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_SegmentAndCircle(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold PolygonAndCircle(PolygonGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_PolygonAndCircle(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold CapsuleAndCapsule(CapsuleGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_CapsuleAndCapsule(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold SegmentAndCapsule(SegmentGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_SegmentAndCapsule(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold PolygonAndCapsule(PolygonGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_PolygonAndCapsule(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold PolygonAndPolygon(PolygonGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_PolygonAndPolygon(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold SegmentAndPolygon(SegmentGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_SegmentAndPolygon(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold ChainSegmentAndCircle(ChainSegmentGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_ChainSegmentAndCircle(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold ChainSegmentAndCapsule(ChainSegmentGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_ChainSegmentAndCapsule(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsShape.ContactManifold ChainSegmentAndPolygon(ChainSegmentGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_ChainSegmentAndPolygon(geometryA, transformA, geometryB, transformB);
		}

		public static PhysicsQuery.SegmentDistanceResult SegmentDistance(SegmentGeometry geometryA, PhysicsTransform transformA, SegmentGeometry geometryB, PhysicsTransform transformB)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_SegmentDistance(geometryA, transformA, geometryB, transformB);
		}

		public static void CastShapes(PhysicsQuery.CastShapePairInput castShapePairInput)
		{
			PhysicsLowLevelScripting2D.PhysicsQuery_CastShapes(castShapePairInput);
		}

		public static PhysicsQuery.DistanceResult ShapeDistance(PhysicsQuery.DistanceInput distanceInput)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_ShapeDistance(distanceInput);
		}

		public static PhysicsQuery.TimeOfImpactResult ShapeTimeOfImpact(PhysicsQuery.TimeOfImpactInput toiInput)
		{
			return PhysicsLowLevelScripting2D.PhysicsQuery_ShapeTimeOfImpact(toiInput);
		}

		[Serializable]
		public struct QueryFilter
		{
			public QueryFilter()
			{
				this = PhysicsQuery.QueryFilter.defaultFilter;
			}

			public QueryFilter(PhysicsMask categories, PhysicsMask hitCategories)
			{
				this.m_Categories = categories;
				this.m_HitCategories = hitCategories;
			}

			public PhysicsMask categories
			{
				readonly get
				{
					return this.m_Categories;
				}
				set
				{
					this.m_Categories = value;
				}
			}

			public PhysicsMask hitCategories
			{
				readonly get
				{
					return this.m_HitCategories;
				}
				set
				{
					this.m_HitCategories = value;
				}
			}

			public static readonly PhysicsMask DefaultCategories = PhysicsMask.One;

			public static readonly PhysicsMask DefaultHitCategories = PhysicsMask.All;

			public static readonly PhysicsQuery.QueryFilter Everything = new PhysicsQuery.QueryFilter(PhysicsMask.All, PhysicsMask.All);

			public static readonly PhysicsQuery.QueryFilter defaultFilter = new PhysicsQuery.QueryFilter(PhysicsQuery.QueryFilter.DefaultCategories, PhysicsQuery.QueryFilter.DefaultHitCategories);

			[SerializeField]
			internal PhysicsMask m_Categories;

			[SerializeField]
			internal PhysicsMask m_HitCategories;
		}

		public readonly struct WorldOverlapResult
		{
			public PhysicsShape shape
			{
				get
				{
					return this.m_Shape;
				}
			}

			public bool isValid
			{
				get
				{
					return this.m_Shape.isValid;
				}
			}

			private readonly PhysicsShape m_Shape;
		}

		public readonly struct WorldCastResult
		{
			public PhysicsShape shape
			{
				get
				{
					return this.m_Shape;
				}
			}

			public Vector2 point
			{
				get
				{
					return this.m_Point;
				}
			}

			public Vector2 normal
			{
				get
				{
					return this.m_Normal;
				}
			}

			public float fraction
			{
				get
				{
					return this.m_Fraction;
				}
			}

			public bool isValid
			{
				get
				{
					return this.m_Shape.isValid;
				}
			}

			private readonly PhysicsShape m_Shape;

			private readonly Vector2 m_Point;

			private readonly Vector2 m_Normal;

			private readonly float m_Fraction;
		}

		[Serializable]
		public struct WorldMoverInput
		{
			public WorldMoverInput()
			{
				this = PhysicsQuery.WorldMoverInput.s_WorldMoverInput;
			}

			public CapsuleGeometry geometry
			{
				readonly get
				{
					return this.m_Geometry;
				}
				set
				{
					this.m_Geometry = value;
				}
			}

			public PhysicsTransform transform
			{
				readonly get
				{
					return this.m_Transform;
				}
				set
				{
					this.m_Transform = value;
				}
			}

			public Vector2 targetPosition
			{
				readonly get
				{
					return this.m_TargetPosition;
				}
				set
				{
					this.m_TargetPosition = value;
				}
			}

			public Vector2 velocity
			{
				readonly get
				{
					return this.m_Velocity;
				}
				set
				{
					this.m_Velocity = value;
				}
			}

			public PhysicsQuery.QueryFilter overlapFilter
			{
				readonly get
				{
					return this.m_OverlapFilter;
				}
				set
				{
					this.m_OverlapFilter = value;
				}
			}

			public PhysicsQuery.QueryFilter castFilter
			{
				readonly get
				{
					return this.m_CastFilter;
				}
				set
				{
					this.m_CastFilter = value;
				}
			}

			public int maxIterations
			{
				readonly get
				{
					return this.m_MaxIterations;
				}
				set
				{
					this.m_MaxIterations = Mathf.Max(0, value);
				}
			}

			public float moveTolerance
			{
				readonly get
				{
					return this.m_MoveTolerance;
				}
				set
				{
					this.m_MoveTolerance = Mathf.Max(0.01f, value);
				}
			}

			public static PhysicsQuery.WorldMoverInput defaultInput
			{
				get
				{
					return PhysicsQuery.WorldMoverInput.s_WorldMoverInput;
				}
			}

			private static PhysicsQuery.WorldMoverInput s_WorldMoverInput = new PhysicsQuery.WorldMoverInput
			{
				geometry = CapsuleGeometry.defaultGeometry,
				overlapFilter = PhysicsQuery.QueryFilter.defaultFilter,
				castFilter = PhysicsQuery.QueryFilter.defaultFilter,
				transform = PhysicsTransform.identity,
				velocity = Vector2.zero,
				targetPosition = Vector2.zero,
				maxIterations = 5,
				moveTolerance = 0.1f
			};

			[SerializeField]
			private CapsuleGeometry m_Geometry;

			[SerializeField]
			private PhysicsTransform m_Transform;

			[SerializeField]
			private Vector2 m_TargetPosition;

			[SerializeField]
			private Vector2 m_Velocity;

			[SerializeField]
			private PhysicsQuery.QueryFilter m_OverlapFilter;

			[SerializeField]
			private PhysicsQuery.QueryFilter m_CastFilter;

			[SerializeField]
			private int m_MaxIterations;

			[SerializeField]
			private float m_MoveTolerance;
		}

		public readonly struct WorldMoverResult
		{
			public PhysicsTransform transform
			{
				get
				{
					return this.m_Transform;
				}
			}

			public Vector2 velocity
			{
				get
				{
					return this.m_Velocity;
				}
			}

			private readonly PhysicsTransform m_Transform;

			private readonly Vector2 m_Velocity;
		}

		public enum WorldCastMode
		{
			Closest,
			All,
			AllSorted
		}

		[Serializable]
		public struct CastRayInput
		{
			public CastRayInput()
			{
				this.m_Origin = default(Vector2);
				this.m_Translation = default(Vector2);
				this.m_MaxFraction = 1f;
			}

			public CastRayInput(Vector2 origin, Vector2 translation)
			{
				this.m_Origin = origin;
				this.m_Translation = translation;
				this.m_MaxFraction = 1f;
			}

			public static PhysicsQuery.CastRayInput FromTo(Vector2 from, Vector2 to)
			{
				return new PhysicsQuery.CastRayInput(from, to - from);
			}

			public Vector2 origin
			{
				readonly get
				{
					return this.m_Origin;
				}
				set
				{
					this.m_Origin = value;
				}
			}

			public Vector2 translation
			{
				readonly get
				{
					return this.m_Translation;
				}
				set
				{
					this.m_Translation = value;
				}
			}

			public float maxFraction
			{
				readonly get
				{
					return this.m_MaxFraction;
				}
				set
				{
					this.m_MaxFraction = Mathf.Clamp01(value);
				}
			}

			[SerializeField]
			private Vector2 m_Origin;

			[SerializeField]
			private Vector2 m_Translation;

			[Range(0f, 1f)]
			[SerializeField]
			private float m_MaxFraction;
		}

		[Serializable]
		public struct CastShapePairInput
		{
			public PhysicsShape.ShapeProxy shapeProxyA
			{
				readonly get
				{
					return this.m_ShapeProxyA;
				}
				set
				{
					this.m_ShapeProxyA = value;
				}
			}

			public PhysicsShape.ShapeProxy shapeProxyB
			{
				readonly get
				{
					return this.m_ShapeProxyB;
				}
				set
				{
					this.m_ShapeProxyB = value;
				}
			}

			public PhysicsTransform transformA
			{
				readonly get
				{
					return this.m_TransformA;
				}
				set
				{
					this.m_TransformA = value;
				}
			}

			public PhysicsTransform transformB
			{
				readonly get
				{
					return this.m_TransformB;
				}
				set
				{
					this.m_TransformB = value;
				}
			}

			public Vector2 translationB
			{
				readonly get
				{
					return this.m_TranslationB;
				}
				set
				{
					this.m_TranslationB = value;
				}
			}

			public float maxFraction
			{
				readonly get
				{
					return this.m_MaxFraction;
				}
				set
				{
					this.m_MaxFraction = Mathf.Clamp01(value);
				}
			}

			public bool canEncroach
			{
				readonly get
				{
					return this.m_CanEncroach;
				}
				set
				{
					this.m_CanEncroach = value;
				}
			}

			[SerializeField]
			private PhysicsShape.ShapeProxy m_ShapeProxyA;

			[SerializeField]
			private PhysicsShape.ShapeProxy m_ShapeProxyB;

			[SerializeField]
			private PhysicsTransform m_TransformA;

			[SerializeField]
			private PhysicsTransform m_TransformB;

			[SerializeField]
			private Vector2 m_TranslationB;

			[SerializeField]
			[Range(0f, 1f)]
			private float m_MaxFraction;

			[SerializeField]
			private bool m_CanEncroach;
		}

		[Serializable]
		public struct CastShapeInput
		{
			public CastShapeInput()
			{
				this.m_ShapeProxy = default(PhysicsShape.ShapeProxy);
				this.m_Translation = default(Vector2);
				this.m_MaxFraction = 1f;
				this.m_CanEncroach = false;
			}

			public CastShapeInput(CircleGeometry circleGeometry, Vector2 translation)
			{
				this.m_ShapeProxy = new PhysicsShape.ShapeProxy(circleGeometry);
				this.m_Translation = translation;
				this.m_MaxFraction = 1f;
				this.m_CanEncroach = false;
			}

			public CastShapeInput(CapsuleGeometry capsuleGeometry, Vector2 translation)
			{
				this.m_ShapeProxy = new PhysicsShape.ShapeProxy(capsuleGeometry);
				this.m_Translation = translation;
				this.m_MaxFraction = 1f;
				this.m_CanEncroach = false;
			}

			public CastShapeInput(SegmentGeometry segmentGeometry, Vector2 translation)
			{
				this.m_ShapeProxy = new PhysicsShape.ShapeProxy(segmentGeometry);
				this.m_Translation = translation;
				this.m_MaxFraction = 1f;
				this.m_CanEncroach = false;
			}

			public CastShapeInput(PolygonGeometry polygonGeometry, Vector2 translation)
			{
				this.m_ShapeProxy = new PhysicsShape.ShapeProxy(polygonGeometry);
				this.m_Translation = translation;
				this.m_MaxFraction = 1f;
				this.m_CanEncroach = false;
			}

			public CastShapeInput(ChainSegmentGeometry chainSegmentGeometry, Vector2 translation)
			{
				this.m_ShapeProxy = new PhysicsShape.ShapeProxy(chainSegmentGeometry);
				this.m_Translation = translation;
				this.m_MaxFraction = 1f;
				this.m_CanEncroach = false;
			}

			public static PhysicsQuery.CastShapeInput FromShape(PhysicsShape shape, Vector2 translation)
			{
				bool flag = !shape.isValid;
				if (flag)
				{
					throw new ArgumentException("PhysicsShape is not valid.");
				}
				PhysicsTransform transform = shape.body.transform;
				PhysicsShape.ShapeType shapeType = shape.shapeType;
				if (!true)
				{
				}
				PhysicsQuery.CastShapeInput castShapeInput;
				switch (shapeType)
				{
				case PhysicsShape.ShapeType.Circle:
					castShapeInput = new PhysicsQuery.CastShapeInput(shape.circleGeometry.Transform(transform), translation);
					break;
				case PhysicsShape.ShapeType.Capsule:
					castShapeInput = new PhysicsQuery.CastShapeInput(shape.capsuleGeometry.Transform(transform), translation);
					break;
				case PhysicsShape.ShapeType.Segment:
					castShapeInput = new PhysicsQuery.CastShapeInput(shape.segmentGeometry.Transform(transform), translation);
					break;
				case PhysicsShape.ShapeType.Polygon:
					castShapeInput = new PhysicsQuery.CastShapeInput(shape.polygonGeometry.Transform(transform), translation);
					break;
				case PhysicsShape.ShapeType.ChainSegment:
					castShapeInput = new PhysicsQuery.CastShapeInput(shape.chainSegmentGeometry.Transform(transform), translation);
					break;
				default:
					throw new NotImplementedException();
				}
				if (!true)
				{
				}
				return castShapeInput;
			}

			public PhysicsShape.ShapeProxy shapeProxy
			{
				readonly get
				{
					return this.m_ShapeProxy;
				}
				set
				{
					this.m_ShapeProxy = value;
				}
			}

			public Vector2 translation
			{
				readonly get
				{
					return this.m_Translation;
				}
				set
				{
					this.m_Translation = value;
				}
			}

			public float maxFraction
			{
				readonly get
				{
					return this.m_MaxFraction;
				}
				set
				{
					this.m_MaxFraction = Mathf.Clamp01(value);
				}
			}

			public bool canEncroach
			{
				readonly get
				{
					return this.m_CanEncroach;
				}
				set
				{
					this.m_CanEncroach = value;
				}
			}

			[SerializeField]
			private PhysicsShape.ShapeProxy m_ShapeProxy;

			[SerializeField]
			private Vector2 m_Translation;

			[Range(0f, 1f)]
			[SerializeField]
			private float m_MaxFraction;

			[SerializeField]
			private bool m_CanEncroach;
		}

		public readonly struct CastResult
		{
			public Vector2 normal
			{
				get
				{
					return this.m_Normal;
				}
			}

			public Vector2 point
			{
				get
				{
					return this.m_Point;
				}
			}

			public float fraction
			{
				get
				{
					return this.m_Fraction;
				}
			}

			public int iterations
			{
				get
				{
					return this.m_Iterations;
				}
			}

			public bool hit
			{
				get
				{
					return this.m_Hit;
				}
			}

			public static implicit operator bool(PhysicsQuery.CastResult output)
			{
				return output.hit;
			}

			private readonly Vector2 m_Normal;

			private readonly Vector2 m_Point;

			private readonly float m_Fraction;

			private readonly int m_Iterations;

			private readonly bool m_Hit;
		}

		[Serializable]
		public struct DistanceInput
		{
			public PhysicsShape.ShapeProxy shapeProxyA
			{
				readonly get
				{
					return this.m_ShapeProxyA;
				}
				set
				{
					this.m_ShapeProxyA = value;
				}
			}

			public PhysicsShape.ShapeProxy shapeProxyB
			{
				readonly get
				{
					return this.m_ShapeProxyB;
				}
				set
				{
					this.m_ShapeProxyB = value;
				}
			}

			public PhysicsTransform transformA
			{
				readonly get
				{
					return this.m_TransformA;
				}
				set
				{
					this.m_TransformA = value;
				}
			}

			public PhysicsTransform transformB
			{
				readonly get
				{
					return this.m_TransformB;
				}
				set
				{
					this.m_TransformB = value;
				}
			}

			public bool useRadii
			{
				readonly get
				{
					return this.m_UseRadii;
				}
				set
				{
					this.m_UseRadii = value;
				}
			}

			[SerializeField]
			private PhysicsShape.ShapeProxy m_ShapeProxyA;

			[SerializeField]
			private PhysicsShape.ShapeProxy m_ShapeProxyB;

			[SerializeField]
			private PhysicsTransform m_TransformA;

			[SerializeField]
			private PhysicsTransform m_TransformB;

			[SerializeField]
			private bool m_UseRadii;
		}

		public readonly struct DistanceResult
		{
			public Vector2 pointA
			{
				get
				{
					return this.m_PointA;
				}
			}

			public Vector2 pointB
			{
				get
				{
					return this.m_PointB;
				}
			}

			public Vector2 normal
			{
				get
				{
					return this.m_Normal;
				}
			}

			public float distance
			{
				get
				{
					return this.m_Distance;
				}
			}

			public int iterations
			{
				get
				{
					return this.m_Iterations;
				}
			}

			private readonly Vector2 m_PointA;

			private readonly Vector2 m_PointB;

			private readonly Vector2 m_Normal;

			private readonly float m_Distance;

			private readonly int m_Iterations;

			private readonly int m_SimplexCount;
		}

		public readonly struct SegmentDistanceResult
		{
			public Vector2 closest1
			{
				get
				{
					return this.m_Closest1;
				}
			}

			public Vector2 closest2
			{
				get
				{
					return this.m_Closest2;
				}
			}

			public float fraction1
			{
				get
				{
					return this.m_Fraction1;
				}
			}

			public float fraction2
			{
				get
				{
					return this.m_Fraction2;
				}
			}

			public float distance
			{
				get
				{
					return this.m_Distance;
				}
			}

			private readonly Vector2 m_Closest1;

			private readonly Vector2 m_Closest2;

			private readonly float m_Fraction1;

			private readonly float m_Fraction2;

			private readonly float m_Distance;
		}

		[Serializable]
		public struct ShapeSweep
		{
			public Vector2 localCOM
			{
				readonly get
				{
					return this.m_LocalCOM;
				}
				set
				{
					this.m_LocalCOM = value;
				}
			}

			public Vector2 positionStart
			{
				readonly get
				{
					return this.m_PositionStart;
				}
				set
				{
					this.m_PositionStart = value;
				}
			}

			public Vector2 positionEnd
			{
				readonly get
				{
					return this.m_PositionEnd;
				}
				set
				{
					this.m_PositionEnd = value;
				}
			}

			public PhysicsRotate rotationStart
			{
				readonly get
				{
					return this.m_RotationStart;
				}
				set
				{
					this.m_RotationStart = value;
				}
			}

			public PhysicsRotate rotationEnd
			{
				readonly get
				{
					return this.m_RotationEnd;
				}
				set
				{
					this.m_RotationEnd = value;
				}
			}

			[SerializeField]
			private Vector2 m_LocalCOM;

			[SerializeField]
			private Vector2 m_PositionStart;

			[SerializeField]
			private Vector2 m_PositionEnd;

			[SerializeField]
			private PhysicsRotate m_RotationStart;

			[SerializeField]
			private PhysicsRotate m_RotationEnd;
		}

		[Serializable]
		public struct TimeOfImpactInput
		{
			public PhysicsShape.ShapeProxy shapeProxyA
			{
				readonly get
				{
					return this.m_ShapeProxyA;
				}
				set
				{
					this.m_ShapeProxyA = value;
				}
			}

			public PhysicsShape.ShapeProxy shapeProxyB
			{
				readonly get
				{
					return this.m_ShapeProxyB;
				}
				set
				{
					this.m_ShapeProxyB = value;
				}
			}

			public PhysicsQuery.ShapeSweep shapeSweepA
			{
				readonly get
				{
					return this.m_ShapeSweepA;
				}
				set
				{
					this.m_ShapeSweepA = value;
				}
			}

			public PhysicsQuery.ShapeSweep shapeSweepB
			{
				readonly get
				{
					return this.m_ShapeSweepB;
				}
				set
				{
					this.m_ShapeSweepB = value;
				}
			}

			public float maxFraction
			{
				readonly get
				{
					return this.m_MaxFraction;
				}
				set
				{
					this.m_MaxFraction = value;
				}
			}

			[SerializeField]
			private PhysicsShape.ShapeProxy m_ShapeProxyA;

			[SerializeField]
			private PhysicsShape.ShapeProxy m_ShapeProxyB;

			[SerializeField]
			private PhysicsQuery.ShapeSweep m_ShapeSweepA;

			[SerializeField]
			private PhysicsQuery.ShapeSweep m_ShapeSweepB;

			[SerializeField]
			private float m_MaxFraction;
		}

		public readonly struct TimeOfImpactResult
		{
			public Vector2 point
			{
				get
				{
					return this.m_Point;
				}
			}

			public Vector2 normal
			{
				get
				{
					return this.m_Normal;
				}
			}

			public PhysicsQuery.TimeOfImpactResult.State impactState
			{
				get
				{
					return this.m_ImpactState;
				}
			}

			public float fraction
			{
				get
				{
					return this.m_Fraction;
				}
			}

			private readonly Vector2 m_Point;

			private readonly Vector2 m_Normal;

			private readonly PhysicsQuery.TimeOfImpactResult.State m_ImpactState;

			private readonly float m_Fraction;

			public enum State
			{
				Unknown,
				Failed,
				Overlapped,
				Hit,
				Separated
			}
		}
	}
}
