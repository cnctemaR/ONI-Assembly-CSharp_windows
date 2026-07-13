using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Serialization;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsShape : IEquatable<PhysicsShape>
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("PhysicsShape.frictionCombine has been deprecated. Please use PhysicsShape.frictionMixing instead.", false)]
		public PhysicsMaterialCombine2D frictionCombine
		{
			get
			{
				return (PhysicsMaterialCombine2D)this.frictionMixing;
			}
			set
			{
				this.frictionMixing = (PhysicsShape.SurfaceMaterial.MixingMode)value;
			}
		}

		[Obsolete("PhysicsShape.bouncinessCombine has been deprecated. Please use PhysicsShape.bouncinessMixing instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PhysicsMaterialCombine2D bouncinessCombine
		{
			get
			{
				return (PhysicsMaterialCombine2D)this.bouncinessMixing;
			}
			set
			{
				this.bouncinessMixing = (PhysicsShape.SurfaceMaterial.MixingMode)value;
			}
		}

		public override string ToString()
		{
			return this.isValid ? string.Format("type={0}, index={1}, world={2}, generation={3}", new object[] { this.shapeType, this.m_Index1, this.m_World0, this.m_Generation }) : "<INVALID>";
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(PhysicsShape other)
		{
			return this.m_Index1 == other.m_Index1 && this.m_World0 == other.m_World0 && this.m_Generation == other.m_Generation;
		}

		public static bool operator ==(PhysicsShape lhs, PhysicsShape rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsShape lhs, PhysicsShape rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, ushort, ushort>(this.m_Index1, this.m_World0, this.m_Generation);
		}

		public static PhysicsShape CreateShape(PhysicsBody body, CircleGeometry geometry)
		{
			return PhysicsShape.CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
		}

		public static PhysicsShape CreateShape(PhysicsBody body, CircleGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateCircleShape(body, geometry, definition);
		}

		public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<CircleGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<CircleGeometry>(geometry), PhysicsShape.ShapeType.Circle, definition, allocator).ToNativeArray<PhysicsShape>();
		}

		public static PhysicsShape CreateShape(PhysicsBody body, PolygonGeometry geometry)
		{
			return PhysicsShape.CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
		}

		public static PhysicsShape CreateShape(PhysicsBody body, PolygonGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreatePolygonShape(body, geometry, definition);
		}

		public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<PolygonGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<PolygonGeometry>(geometry), PhysicsShape.ShapeType.Polygon, definition, allocator).ToNativeArray<PhysicsShape>();
		}

		public static PhysicsShape CreateShape(PhysicsBody body, CapsuleGeometry geometry)
		{
			return PhysicsShape.CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
		}

		public static PhysicsShape CreateShape(PhysicsBody body, CapsuleGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateCapsuleShape(body, geometry, definition);
		}

		public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<CapsuleGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<CapsuleGeometry>(geometry), PhysicsShape.ShapeType.Capsule, definition, allocator).ToNativeArray<PhysicsShape>();
		}

		public static PhysicsShape CreateShape(PhysicsBody body, SegmentGeometry geometry)
		{
			return PhysicsShape.CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
		}

		public static PhysicsShape CreateShape(PhysicsBody body, SegmentGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateSegmentShape(body, geometry, definition);
		}

		public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<SegmentGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<SegmentGeometry>(geometry), PhysicsShape.ShapeType.Segment, definition, allocator).ToNativeArray<PhysicsShape>();
		}

		public static PhysicsShape CreateShape(PhysicsBody body, ChainSegmentGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateChainSegmenShapet(body, geometry, definition);
		}

		public static PhysicsShape CreateShape(PhysicsBody body, ChainSegmentGeometry geometry)
		{
			return PhysicsShape.CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
		}

		public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<ChainSegmentGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan<ChainSegmentGeometry>(geometry), PhysicsShape.ShapeType.ChainSegment, definition, allocator).ToNativeArray<PhysicsShape>();
		}

		public bool Destroy(bool updateBodyMass = true, int ownerKey = 0)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_Destroy(this, updateBodyMass, ownerKey);
		}

		public static void DestroyBatch(ReadOnlySpan<PhysicsShape> shapes, bool updateBodyMass)
		{
			PhysicsLowLevelScripting2D.PhysicsShape_DestroyBatch(shapes, updateBodyMass);
		}

		public PhysicsShapeDefinition definition
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_ReadDefinition(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_WriteDefinition(this, value, false);
			}
		}

		public bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_IsValid(this);
			}
		}

		public PhysicsWorld world
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetWorld(this);
			}
		}

		public PhysicsBody body
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetBody(this);
			}
		}

		public bool isTrigger
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetIsTrigger(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetIsTrigger(this, value);
			}
		}

		public PhysicsShape.ShapeType shapeType
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetShapeType(this);
			}
		}

		public PhysicsTransform transform
		{
			get
			{
				return this.body.transform;
			}
		}

		public void SetDensity(float density, bool updateBodyMass)
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetDensity(this, density, updateBodyMass);
		}

		public float GetDensity()
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetDensity(this);
		}

		public PhysicsBody.MassConfiguration massConfiguration
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetMassConfiguration(this);
			}
		}

		public float friction
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetFriction(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetFriction(this, value);
			}
		}

		public float bounciness
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetBounciness(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetBounciness(this, value);
			}
		}

		public PhysicsShape.SurfaceMaterial.MixingMode frictionMixing
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetFrictionMixing(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetFrictionMixing(this, value);
			}
		}

		public PhysicsShape.SurfaceMaterial.MixingMode bouncinessMixing
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetBouncinessMixing(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetBouncinessMixing(this, value);
			}
		}

		public ushort frictionPriority
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetFrictionPriority(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetFrictionPriority(this, value);
			}
		}

		public ushort bouncinessPriority
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetBouncinessPriority(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetBouncinessPriority(this, value);
			}
		}

		public float rollingResistance
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetRollingResistance(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetRollingResistance(this, value);
			}
		}

		public float tangentSpeed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetTangentSpeed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetTangentSpeed(this, value);
			}
		}

		public Color32 customColor
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetCustomColor(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetCustomColor(this, value);
			}
		}

		public PhysicsShape.SurfaceMaterial surfaceMaterial
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetSurfaceMaterial(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetSurfaceMaterial(this, value);
			}
		}

		public PhysicsShape.ContactFilter contactFilter
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetContactFilter(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetContactFilter(this, value);
			}
		}

		public PhysicsShape.MoverData moverData
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetMoverData(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetMoverData(this, value);
			}
		}

		public void ApplyWind(Vector2 force, float drag, float lift, bool wake = true)
		{
			PhysicsLowLevelScripting2D.PhysicsShape_ApplyWind(this, force, drag, lift, wake);
		}

		public bool triggerEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetTriggerEvents(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetTriggerEvents(this, value);
			}
		}

		public bool contactEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetContactEvents(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetContactEvents(this, value);
			}
		}

		public bool hitEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetHitEvents(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetHitEvents(this, value);
			}
		}

		public bool contactFilterCallbacks
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetContactFilterCallbacks(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetContacFiltertCallbacks(this, value);
			}
		}

		public bool preSolveCallbacks
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetPreSolveCallbacks(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetPreSolveCallbacks(this, value);
			}
		}

		public bool OverlapPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_OverlapPoint(this, point);
		}

		public Vector2 ClosestPoint(Vector2 point)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_ClosestPoint(this, point);
		}

		public PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CastRay(this, castRayInput);
		}

		public PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_CastShape(this, input);
		}

		public PhysicsShape.ContactManifold Intersect(PhysicsShape otherShape)
		{
			return PhysicsQuery.ShapeAndShape(this, this.body.transform, otherShape, otherShape.body.transform);
		}

		public PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, PhysicsShape otherShape, PhysicsTransform otherTransform)
		{
			return PhysicsQuery.ShapeAndShape(this, transform, otherShape, otherTransform);
		}

		public CircleGeometry circleGeometry
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetCircleGeometry(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetCircleGeometry(this, value);
			}
		}

		public CapsuleGeometry capsuleGeometry
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetCapsuleGeometry(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetCapsuleGeometry(this, value);
			}
		}

		public PolygonGeometry polygonGeometry
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetPolygonGeometry(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetPolygonGeometry(this, value);
			}
		}

		public SegmentGeometry segmentGeometry
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetSegmentGeometry(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetSegmentGeometry(this, value);
			}
		}

		public ChainSegmentGeometry chainSegmentGeometry
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetChainSegmentGeometry(this);
			}
		}

		public bool isChainSegment
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_IsChainSegmentShape(this);
			}
		}

		public PhysicsChain chain
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetChain(this);
			}
		}

		public NativeArray<PhysicsShape.Contact> GetContacts(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetContacts(this, allocator).ToNativeArray<PhysicsShape.Contact>();
		}

		public NativeArray<PhysicsShape> GetTriggerVisitors(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetTriggerVisitors(this, allocator).ToNativeArray<PhysicsShape>();
		}

		public PhysicsAABB aabb
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_CalculateAABB(this);
			}
		}

		public Vector2 localCenter
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetLocalCenter(this);
			}
		}

		public float GetPerimeter()
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetPerimeter(this);
		}

		public float GetPerimeterProjected(Vector2 axis)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetPerimeterProjected(this, axis);
		}

		public int SetOwner(Object owner)
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_SetOwner(this, owner);
		}

		public Object GetOwner()
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetOwner(this);
		}

		public bool isOwned
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_IsOwned(this);
			}
		}

		public object callbackTarget
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetCallbackTarget(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetCallbackTarget(this, value);
			}
		}

		public PhysicsUserData userData
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetUserData(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsShape_SetUserData(this, value);
			}
		}

		public PhysicsShape.ShapeProxy CreateShapeProxy()
		{
			bool flag = !this.isValid;
			if (flag)
			{
				throw new ArgumentException("PhysicsShape is not valid.");
			}
			PhysicsShape.ShapeType shapeType = this.shapeType;
			if (!true)
			{
			}
			PhysicsShape.ShapeProxy shapeProxy;
			switch (shapeType)
			{
			case PhysicsShape.ShapeType.Circle:
				shapeProxy = new PhysicsShape.ShapeProxy(this.circleGeometry);
				break;
			case PhysicsShape.ShapeType.Capsule:
				shapeProxy = new PhysicsShape.ShapeProxy(this.capsuleGeometry);
				break;
			case PhysicsShape.ShapeType.Segment:
				shapeProxy = new PhysicsShape.ShapeProxy(this.segmentGeometry);
				break;
			case PhysicsShape.ShapeType.Polygon:
				shapeProxy = new PhysicsShape.ShapeProxy(this.polygonGeometry);
				break;
			default:
				throw new ArgumentException("PhysicsShape cannot be a Chain.");
			}
			if (!true)
			{
			}
			return shapeProxy;
		}

		public void Draw()
		{
			PhysicsLowLevelScripting2D.PhysicsShape_Draw(this);
		}

		private readonly int m_Index1;

		private readonly ushort m_World0;

		private readonly ushort m_Generation;

		[Serializable]
		public struct SurfaceMaterial
		{
			[Obsolete("PhysicsShape.SurfaceMaterial.frictionCombine has been deprecated. Please use PhysicsShape.SurfaceMaterial.frictionMixing instead.", false)]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public PhysicsMaterialCombine2D frictionCombine
			{
				readonly get
				{
					return (PhysicsMaterialCombine2D)this.frictionMixing;
				}
				set
				{
					this.frictionMixing = (PhysicsShape.SurfaceMaterial.MixingMode)value;
				}
			}

			[Obsolete("PhysicsShape.SurfaceMaterial.bouncinessCombine has been deprecated. Please use PhysicsShape.SurfaceMaterial.bouncinessMixing instead.", false)]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public PhysicsMaterialCombine2D bouncinessCombine
			{
				readonly get
				{
					return (PhysicsMaterialCombine2D)this.bouncinessMixing;
				}
				set
				{
					this.bouncinessMixing = (PhysicsShape.SurfaceMaterial.MixingMode)value;
				}
			}

			public SurfaceMaterial()
			{
				this = PhysicsShape.SurfaceMaterial.Default;
			}

			public static PhysicsShape.SurfaceMaterial Default
			{
				get
				{
					return PhysicsLowLevelScripting2D.PhysicsShape_GetDefaultSurfaceMaterial();
				}
			}

			public float friction
			{
				readonly get
				{
					return this.m_Friction;
				}
				set
				{
					this.m_Friction = Mathf.Max(0f, value);
				}
			}

			public float bounciness
			{
				readonly get
				{
					return this.m_Bounciness;
				}
				set
				{
					this.m_Bounciness = Mathf.Max(0f, value);
				}
			}

			public PhysicsShape.SurfaceMaterial.MixingMode frictionMixing
			{
				readonly get
				{
					return this.m_FrictionMixing;
				}
				set
				{
					this.m_FrictionMixing = value;
				}
			}

			public PhysicsShape.SurfaceMaterial.MixingMode bouncinessMixing
			{
				readonly get
				{
					return this.m_BouncinessMixing;
				}
				set
				{
					this.m_BouncinessMixing = value;
				}
			}

			public ushort frictionPriority
			{
				readonly get
				{
					return this.m_FrictionPriority;
				}
				set
				{
					this.m_FrictionPriority = value;
				}
			}

			public ushort bouncinessPriority
			{
				readonly get
				{
					return this.m_BouncinessPriority;
				}
				set
				{
					this.m_BouncinessPriority = value;
				}
			}

			public float rollingResistance
			{
				readonly get
				{
					return this.m_RollingResistance;
				}
				set
				{
					this.m_RollingResistance = Mathf.Max(0f, value);
				}
			}

			public float tangentSpeed
			{
				readonly get
				{
					return this.m_TangentSpeed;
				}
				set
				{
					this.m_TangentSpeed = value;
				}
			}

			public Color32 customColor
			{
				readonly get
				{
					return this.m_CustomColor;
				}
				set
				{
					this.m_CustomColor = value;
				}
			}

			[Min(0f)]
			[SerializeField]
			private float m_Friction;

			[SerializeField]
			[Min(0f)]
			private float m_Bounciness;

			[FormerlySerializedAs("m_FrictionCombine")]
			[SerializeField]
			private PhysicsShape.SurfaceMaterial.MixingMode m_FrictionMixing;

			[FormerlySerializedAs("m_BouncinessCombine")]
			[SerializeField]
			private PhysicsShape.SurfaceMaterial.MixingMode m_BouncinessMixing;

			[Range(0f, 65535f)]
			[SerializeField]
			private ushort m_FrictionPriority;

			[SerializeField]
			[Range(0f, 65535f)]
			private ushort m_BouncinessPriority;

			[SerializeField]
			[Min(0f)]
			private float m_RollingResistance;

			[SerializeField]
			private float m_TangentSpeed;

			[SerializeField]
			private Color32 m_CustomColor;

			public enum MixingMode
			{
				Average,
				Mean,
				Multiply,
				Minimum,
				Maximum
			}
		}

		public enum ShapeType
		{
			Circle,
			Capsule,
			Segment,
			Polygon,
			ChainSegment
		}

		public readonly struct ContactManifold : IEnumerable<PhysicsShape.ContactManifold.ManifoldPoint>, IEnumerable
		{
			public Vector2 normal
			{
				get
				{
					return this.m_Normal;
				}
			}

			public float rollingImpulse
			{
				get
				{
					return this.m_RollingImpulse;
				}
			}

			public PhysicsShape.ContactManifold.ManifoldPointArray points
			{
				get
				{
					return this.m_Points;
				}
			}

			public int pointCount
			{
				get
				{
					return this.m_PointCount;
				}
			}

			public int speculativePointCount
			{
				get
				{
					return this.m_Points.speculativePointCount;
				}
			}

			public PhysicsShape.ContactManifold.ManifoldPoint this[int index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					bool flag = index >= 0 && index < this.pointCount;
					if (flag)
					{
						return this.points[index];
					}
					throw new IndexOutOfRangeException(string.Format("{0} is not valid. The current number of valid points is {1}", index, this.pointCount));
				}
			}

			public IEnumerator<PhysicsShape.ContactManifold.ManifoldPoint> GetEnumerator()
			{
				return new PhysicsShape.ContactManifold.ManifoldPointIterator(this);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new PhysicsShape.ContactManifold.ManifoldPointIterator(this);
			}

			private readonly Vector2 m_Normal;

			private readonly float m_RollingImpulse;

			private readonly PhysicsShape.ContactManifold.ManifoldPointArray m_Points;

			private readonly int m_PointCount;

			public readonly struct ManifoldPoint
			{
				public Vector2 point
				{
					get
					{
						return this.m_Point;
					}
				}

				public Vector2 anchorA
				{
					get
					{
						return this.m_AnchorA;
					}
				}

				public Vector2 anchorB
				{
					get
					{
						return this.m_AnchorB;
					}
				}

				public float separation
				{
					get
					{
						return this.m_Separation;
					}
				}

				public float normalImpulse
				{
					get
					{
						return this.m_NormalImpulse;
					}
				}

				public float tangentImpulse
				{
					get
					{
						return this.m_TangentImpulse;
					}
				}

				public float totalNormalImpulse
				{
					get
					{
						return this.m_TotalNormalImpulse;
					}
				}

				public float normalVelocity
				{
					get
					{
						return this.m_NormalVelocity;
					}
				}

				public ushort id
				{
					get
					{
						return this.m_Id;
					}
				}

				public bool persisted
				{
					get
					{
						return this.m_Persisted;
					}
				}

				public bool speculative
				{
					get
					{
						return this.totalNormalImpulse > 0f;
					}
				}

				private readonly Vector2 m_Point;

				private readonly Vector2 m_AnchorA;

				private readonly Vector2 m_AnchorB;

				private readonly float m_Separation;

				private readonly float m_NormalImpulse;

				private readonly float m_TangentImpulse;

				private readonly float m_TotalNormalImpulse;

				private readonly float m_NormalVelocity;

				private readonly ushort m_Id;

				private readonly bool m_Persisted;
			}

			public readonly struct ManifoldPointArray
			{
				public PhysicsShape.ContactManifold.ManifoldPoint contactInfo0
				{
					get
					{
						return this.m_ContactInfo0;
					}
				}

				public PhysicsShape.ContactManifold.ManifoldPoint contactInfo1
				{
					get
					{
						return this.m_ContactInfo1;
					}
				}

				public unsafe PhysicsShape.ContactManifold.ManifoldPoint this[int index]
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get
					{
						bool flag = index >= 0 && index < 2;
						if (flag)
						{
							fixed (PhysicsShape.ContactManifold.ManifoldPoint* ptr = &this.m_ContactInfo0)
							{
								PhysicsShape.ContactManifold.ManifoldPoint* ptr2 = ptr;
								return ptr2[index];
							}
						}
						throw new IndexOutOfRangeException(string.Format("{0} must be in the range [0, 1]", index));
					}
				}

				public int speculativePointCount
				{
					get
					{
						return (this.m_ContactInfo0.speculative ? 1 : 0) + (this.m_ContactInfo1.speculative ? 1 : 0);
					}
				}

				private readonly PhysicsShape.ContactManifold.ManifoldPoint m_ContactInfo0;

				private readonly PhysicsShape.ContactManifold.ManifoldPoint m_ContactInfo1;
			}

			public struct ManifoldPointIterator : IEnumerator<PhysicsShape.ContactManifold.ManifoldPoint>, IEnumerator, IDisposable
			{
				public ManifoldPointIterator(PhysicsShape.ContactManifold contactManifold)
				{
					this.m_ContactManifold = contactManifold;
					this.m_PointIndex = -1;
				}

				PhysicsShape.ContactManifold.ManifoldPoint IEnumerator<PhysicsShape.ContactManifold.ManifoldPoint>.Current
				{
					get
					{
						return this.m_ContactManifold[this.m_PointIndex];
					}
				}

				private readonly object Current
				{
					get
					{
						return this.m_ContactManifold[this.m_PointIndex];
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				bool IEnumerator.MoveNext()
				{
					int num = this.m_PointIndex + 1;
					this.m_PointIndex = num;
					return num < this.m_ContactManifold.pointCount;
				}

				void IEnumerator.Reset()
				{
					this.m_PointIndex = -1;
				}

				readonly void IDisposable.Dispose()
				{
				}

				private PhysicsShape.ContactManifold m_ContactManifold;

				private int m_PointIndex;
			}
		}

		public readonly struct Contact
		{
			public PhysicsShape.ContactId contactId
			{
				get
				{
					return this.m_ContactId;
				}
			}

			public PhysicsShape shapeA
			{
				get
				{
					return this.m_ShapeA;
				}
			}

			public PhysicsShape shapeB
			{
				get
				{
					return this.m_ShapeB;
				}
			}

			public PhysicsShape.ContactManifold manifold
			{
				get
				{
					return this.m_Manifold;
				}
			}

			private readonly PhysicsShape.ContactId m_ContactId;

			private readonly PhysicsShape m_ShapeA;

			private readonly PhysicsShape m_ShapeB;

			private readonly PhysicsShape.ContactManifold m_Manifold;
		}

		public readonly struct ContactId
		{
			public override string ToString()
			{
				return this.isValid ? string.Format("index={0}, world={1}, generation={2}", this.m_IndexId, this.m_WorldId, this.m_GenerationId) : "<INVALID>";
			}

			public bool isValid
			{
				get
				{
					return PhysicsLowLevelScripting2D.PhysicsContactId_IsValid(this);
				}
			}

			public PhysicsShape.Contact contact
			{
				get
				{
					return PhysicsLowLevelScripting2D.PhysicsContactId_GetContact(this);
				}
			}

			private readonly int m_IndexId;

			private readonly ushort m_WorldId;

			private readonly ushort m_Padding;

			private readonly int m_GenerationId;
		}

		[Serializable]
		public struct ContactFilter
		{
			public ContactFilter(PhysicsMask categories, PhysicsMask contacts, int groupIndex = 0)
			{
				this.m_Categories = categories;
				this.m_Contacts = contacts;
				this.m_GroupIndex = groupIndex;
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

			public PhysicsMask contacts
			{
				readonly get
				{
					return this.m_Contacts;
				}
				set
				{
					this.m_Contacts = value;
				}
			}

			public int groupIndex
			{
				readonly get
				{
					return this.m_GroupIndex;
				}
				set
				{
					this.m_GroupIndex = value;
				}
			}

			public static PhysicsMask DefaultCategories = PhysicsMask.One;

			public static PhysicsMask DefaultContacts = PhysicsMask.All;

			public static PhysicsShape.ContactFilter Everything = new PhysicsShape.ContactFilter(PhysicsMask.All, PhysicsMask.All, 0);

			public static PhysicsShape.ContactFilter defaultFilter = new PhysicsShape.ContactFilter(PhysicsShape.ContactFilter.DefaultCategories, PhysicsShape.ContactFilter.DefaultContacts, 0);

			[SerializeField]
			internal PhysicsMask m_Categories;

			[SerializeField]
			internal PhysicsMask m_Contacts;

			[SerializeField]
			internal int m_GroupIndex;
		}

		[Serializable]
		public struct ShapeArray
		{
			public Vector2 vertex0
			{
				readonly get
				{
					return this.m_Vertex0;
				}
				set
				{
					this.m_Vertex0 = value;
				}
			}

			public Vector2 vertex1
			{
				readonly get
				{
					return this.m_Vertex1;
				}
				set
				{
					this.m_Vertex1 = value;
				}
			}

			public Vector2 vertex2
			{
				readonly get
				{
					return this.m_Vertex2;
				}
				set
				{
					this.m_Vertex2 = value;
				}
			}

			public Vector2 vertex3
			{
				readonly get
				{
					return this.m_Vertex3;
				}
				set
				{
					this.m_Vertex3 = value;
				}
			}

			public Vector2 vertex4
			{
				readonly get
				{
					return this.m_Vertex4;
				}
				set
				{
					this.m_Vertex4 = value;
				}
			}

			public Vector2 vertex5
			{
				readonly get
				{
					return this.m_Vertex5;
				}
				set
				{
					this.m_Vertex5 = value;
				}
			}

			public Vector2 vertex6
			{
				readonly get
				{
					return this.m_Vertex6;
				}
				set
				{
					this.m_Vertex6 = value;
				}
			}

			public Vector2 vertex7
			{
				readonly get
				{
					return this.m_Vertex7;
				}
				set
				{
					this.m_Vertex7 = value;
				}
			}

			public unsafe ref Vector2 this[int index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					bool flag = index >= 0 && index < 8;
					if (flag)
					{
						fixed (Vector2* ptr = &this.m_Vertex0)
						{
							Vector2* ptr2 = ptr;
							return ref ptr2[index];
						}
					}
					throw new IndexOutOfRangeException(string.Format("{0} must be in the range [0, {1}]", index, 7));
				}
			}

			[SerializeField]
			internal Vector2 m_Vertex0;

			[SerializeField]
			private Vector2 m_Vertex1;

			[SerializeField]
			private Vector2 m_Vertex2;

			[SerializeField]
			private Vector2 m_Vertex3;

			[SerializeField]
			private Vector2 m_Vertex4;

			[SerializeField]
			private Vector2 m_Vertex5;

			[SerializeField]
			private Vector2 m_Vertex6;

			[SerializeField]
			private Vector2 m_Vertex7;
		}

		[Serializable]
		public struct MoverData
		{
			public MoverData()
			{
				this.m_PushLimit = float.MaxValue;
				this.m_ClipVelocity = true;
			}

			public float pushLimit
			{
				readonly get
				{
					return this.m_PushLimit;
				}
				set
				{
					this.m_PushLimit = value;
				}
			}

			public bool clipVelocity
			{
				readonly get
				{
					return this.m_ClipVelocity;
				}
				set
				{
					this.m_ClipVelocity = value;
				}
			}

			private float m_PushLimit;

			private bool m_ClipVelocity;
		}

		[Serializable]
		public struct ShapeProxy
		{
			public ShapeProxy(Vector2 point)
			{
				this.m_Vertices = new PhysicsShape.ShapeArray
				{
					vertex0 = point
				};
				this.m_Count = 1;
				this.m_Radius = 0f;
			}

			public ShapeProxy(CircleGeometry circleGeometry)
			{
				bool flag = !circleGeometry.isValid;
				if (flag)
				{
					throw new ArgumentException("circleGeometry", "Circle Geometry is not valid.");
				}
				this.m_Vertices = new PhysicsShape.ShapeArray
				{
					vertex0 = circleGeometry.center
				};
				this.m_Count = 1;
				this.m_Radius = circleGeometry.radius;
			}

			public ShapeProxy(CapsuleGeometry capsuleGeometry)
			{
				bool flag = !capsuleGeometry.isValid;
				if (flag)
				{
					throw new ArgumentException("capsuleGeometry", "Capsule Geometry is not valid.");
				}
				this.m_Vertices = new PhysicsShape.ShapeArray
				{
					vertex0 = capsuleGeometry.center1,
					vertex1 = capsuleGeometry.center2
				};
				this.m_Count = 2;
				this.m_Radius = capsuleGeometry.radius;
			}

			public ShapeProxy(PolygonGeometry polygonGeometry)
			{
				bool flag = !polygonGeometry.isValid;
				if (flag)
				{
					throw new ArgumentException("polygonGeometry", "Polygon Geometry is not valid.");
				}
				this.m_Vertices = polygonGeometry.vertices;
				this.m_Count = polygonGeometry.count;
				this.m_Radius = polygonGeometry.radius;
			}

			public ShapeProxy(SegmentGeometry segmentGeometry)
			{
				bool flag = !segmentGeometry.isValid;
				if (flag)
				{
					throw new ArgumentException("segmentGeometry", "Segment Geometry is not valid.");
				}
				this.m_Vertices = new PhysicsShape.ShapeArray
				{
					vertex0 = segmentGeometry.point1,
					vertex1 = segmentGeometry.point2
				};
				this.m_Count = 2;
				this.m_Radius = 0f;
			}

			public ShapeProxy(ChainSegmentGeometry chainSegmentGeometry)
			{
				bool flag = !chainSegmentGeometry.isValid;
				if (flag)
				{
					throw new ArgumentException("chainSegmentGeometry", "Chain Segment Geometry is not valid.");
				}
				this.m_Vertices = new PhysicsShape.ShapeArray
				{
					vertex0 = chainSegmentGeometry.segment.point1,
					vertex1 = chainSegmentGeometry.segment.point2
				};
				this.m_Count = 2;
				this.m_Radius = 0f;
			}

			public unsafe CircleGeometry circleGeometry
			{
				get
				{
					bool flag = this.m_Count == 1;
					if (flag)
					{
						return new CircleGeometry
						{
							center = *this.m_Vertices[0],
							radius = this.m_Radius
						};
					}
					throw new InvalidOperationException("Expected a vertex count of 1.");
				}
			}

			public unsafe CapsuleGeometry capsuleGeometry
			{
				get
				{
					bool flag = this.m_Count == 2;
					if (flag)
					{
						return new CapsuleGeometry
						{
							center1 = *this.m_Vertices[0],
							center2 = *this.m_Vertices[1],
							radius = this.m_Radius
						};
					}
					throw new InvalidOperationException("Expected a vertex count of 2.");
				}
			}

			public unsafe PolygonGeometry polygonGeometry
			{
				get
				{
					fixed (Vector2* ptr = &this.m_Vertices.m_Vertex0)
					{
						Vector2* ptr2 = ptr;
						return PolygonGeometry.Create(new ReadOnlySpan<Vector2>((void*)ptr2, this.m_Count), this.m_Radius);
					}
				}
			}

			public unsafe SegmentGeometry segmentGeometry
			{
				get
				{
					bool flag = this.m_Count == 2;
					if (flag)
					{
						return new SegmentGeometry
						{
							point1 = *this.m_Vertices[0],
							point2 = *this.m_Vertices[1]
						};
					}
					throw new InvalidOperationException("Expected a vertex count of 2.");
				}
			}

			public PhysicsShape.ShapeArray vertices
			{
				readonly get
				{
					return this.m_Vertices;
				}
				set
				{
					this.m_Vertices = value;
				}
			}

			public int count
			{
				readonly get
				{
					return this.m_Count;
				}
				set
				{
					this.m_Count = Mathf.Max(1, value);
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

			[SerializeField]
			private PhysicsShape.ShapeArray m_Vertices;

			[SerializeField]
			[Min(1f)]
			private int m_Count;

			[SerializeField]
			[Min(0f)]
			private float m_Radius;
		}
	}
}
