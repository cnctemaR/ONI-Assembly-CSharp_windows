using System;
using System.ComponentModel;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsBody : IEquatable<PhysicsBody>
	{
		public override string ToString()
		{
			return this.isValid ? string.Format("type={0}, index={1}, world={2}, generation={3}", new object[] { this.type, this.m_Index1, this.m_World0, this.m_Generation }) : "<INVALID>";
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(PhysicsBody other)
		{
			return this.m_Index1 == other.m_Index1 && this.m_World0 == other.m_World0 && this.m_Generation == other.m_Generation;
		}

		public static bool operator ==(PhysicsBody lhs, PhysicsBody rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsBody lhs, PhysicsBody rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, ushort, ushort>(this.m_Index1, this.m_World0, this.m_Generation);
		}

		public static PhysicsBody Create(PhysicsWorld world)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_Create(world, PhysicsBodyDefinition.defaultDefinition);
		}

		public static PhysicsBody Create(PhysicsWorld world, PhysicsBodyDefinition definition)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_Create(world, definition);
		}

		public unsafe static NativeArray<PhysicsBody> CreateBatch(PhysicsWorld world, PhysicsBodyDefinition definition, int bodyCount, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_CreateBatch(world, new ReadOnlySpan<PhysicsBodyDefinition>((void*)(&definition), 1), bodyCount, allocator).ToNativeArray<PhysicsBody>();
		}

		public static NativeArray<PhysicsBody> CreateBatch(PhysicsWorld world, ReadOnlySpan<PhysicsBodyDefinition> definitions, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_CreateBatch(world, definitions, definitions.Length, allocator).ToNativeArray<PhysicsBody>();
		}

		public bool Destroy(int ownerKey = 0)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_Destroy(this, ownerKey);
		}

		public static void DestroyBatch(ReadOnlySpan<PhysicsBody> bodies)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_DestroyBatch(bodies);
		}

		public static void SetBatchVelocity(ReadOnlySpan<PhysicsBody.BatchVelocity> batch)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetBatchVelocity(batch);
		}

		public static void SetBatchForce(ReadOnlySpan<PhysicsBody.BatchForce> batch)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetBatchForce(batch);
		}

		public static void SetBatchImpulse(ReadOnlySpan<PhysicsBody.BatchImpulse> batch)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetBatchImpulse(batch);
		}

		public static void SetBatchTransform(ReadOnlySpan<PhysicsBody.BatchTransform> batch)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetBatchTransform(batch);
		}

		public PhysicsBodyDefinition definition
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_ReadDefinition(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_WriteDefinition(this, value, false);
			}
		}

		public bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_IsValid(this);
			}
		}

		public PhysicsWorld world
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetWorld(this);
			}
		}

		public PhysicsBody.BodyType type
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetBodyType(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetBodyType(this, value);
			}
		}

		public PhysicsBody.BodyConstraints constraints
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetBodyConstraints(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetBodyConstraints(this, value);
			}
		}

		public Vector2 position
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetPosition(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetPosition(this, value);
			}
		}

		public PhysicsRotate rotation
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetRotation(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetRotation(this, value);
			}
		}

		public PhysicsTransform transform
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetTransform(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetTransform(this, value);
			}
		}

		public void SetTransformTarget(PhysicsTransform transform, float deltaTime)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetTransformTarget(this, transform, deltaTime);
		}

		public void GetPositionAndRotation3D(Transform transform, PhysicsWorld.TransformWriteMode transformWriteMode, PhysicsWorld.TransformPlane transformPlane, out Vector3 position, out Quaternion rotation)
		{
			bool flag = transform == null;
			if (flag)
			{
				throw new ArgumentNullException("transform", "Transform cannot be NULL.");
			}
			PhysicsTransform transform2 = this.transform;
			PhysicsWorld world = this.world;
			switch (transformWriteMode)
			{
			case PhysicsWorld.TransformWriteMode.Fast2D:
				position = PhysicsMath.ToPosition3D(transform2.position, transform.position, transformPlane);
				rotation = PhysicsMath.ToRotationFast3D(transform2.rotation.angle, transformPlane);
				return;
			case PhysicsWorld.TransformWriteMode.Slow3D:
			{
				Vector3 vector;
				Quaternion quaternion;
				transform.GetPositionAndRotation(out vector, out quaternion);
				position = PhysicsMath.ToPosition3D(transform2.position, vector, transformPlane);
				rotation = PhysicsMath.ToRotationSlow3D(transform2.rotation.angle, quaternion, transformPlane);
				return;
			}
			}
			throw new InvalidOperationException("Invalid Transform Write Mode.");
		}

		public bool SetAndWriteTransform(PhysicsTransform transform)
		{
			this.transform = transform;
			Transform transformObject = this.transformObject;
			bool flag = transformObject == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				PhysicsWorld world = this.world;
				PhysicsWorld.TransformWriteMode transformWriteMode = world.transformWriteMode;
				PhysicsWorld.TransformPlane transformPlane = world.transformPlane;
				PhysicsWorld.TransformWriteMode transformWriteMode2 = transformWriteMode;
				PhysicsWorld.TransformWriteMode transformWriteMode3 = transformWriteMode2;
				if (transformWriteMode3 != PhysicsWorld.TransformWriteMode.Off)
				{
					if (transformWriteMode3 - PhysicsWorld.TransformWriteMode.Fast2D > 1)
					{
						throw new InvalidOperationException("Invalid Transform Write Mode.");
					}
					Vector3 vector;
					Quaternion quaternion;
					this.GetPositionAndRotation3D(this.transformObject, transformWriteMode, transformPlane, out vector, out quaternion);
					transformObject.SetPositionAndRotation(vector, quaternion);
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		public Vector2 GetLocalPoint(Vector2 worldPoint)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetLocalPoint(this, worldPoint);
		}

		public Vector2 GetWorldPoint(Vector2 localPoint)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetWorldPoint(this, localPoint);
		}

		public Vector2 GetLocalVector(Vector2 worldVector)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetLocalVector(this, worldVector);
		}

		public Vector2 GetWorldVector(Vector2 localVector)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetWorldVector(this, localVector);
		}

		public Vector2 GetLocalPointVelocity(Vector2 localPoint)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetLocalPointVelocity(this, localPoint);
		}

		public Vector2 GetWorldPointVelocity(Vector2 worldPoint)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetWorldPointVelocity(this, worldPoint);
		}

		public Vector2 linearVelocity
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetLinearVelocity(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetLinearVelocity(this, value);
			}
		}

		public float angularVelocity
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetAngularVelocity(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetAngularVelocity(this, value);
			}
		}

		public float mass
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetMass(this);
			}
		}

		public float rotationalInertia
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetRotationalInertia(this);
			}
		}

		public Vector2 localCenterOfMass
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetLocalCenterOfMass(this);
			}
		}

		public Vector2 worldCenterOfMass
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetWorldCenterOfMass(this);
			}
		}

		public PhysicsBody.MassConfiguration massConfiguration
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetMassConfiguration(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetMassConfiguration(this, value);
			}
		}

		public void ApplyMassFromShapes()
		{
			PhysicsLowLevelScripting2D.PhysicsBody_ApplyMassFromShapes(this);
		}

		public float linearDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetLinearDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetLinearDamping(this, value);
			}
		}

		public float angularDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetAngularDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetAngularDamping(this, value);
			}
		}

		public float gravityScale
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetGravityScale(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetGravityScale(this, value);
			}
		}

		public bool awake
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetAwake(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetAwake(this, value);
			}
		}

		public bool sleepingAllowed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetSleepingAllowed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetSleepingAllowed(this, value);
			}
		}

		public float sleepThreshold
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetSleepThreshold(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetSleepThreshold(this, value);
			}
		}

		public bool enabled
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetEnabled(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetEnabled(this, value);
			}
		}

		public bool fastRotationAllowed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetFastRotationAllowed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetFastRotationAllowed(this, value);
			}
		}

		public bool fastCollisionsAllowed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetFastCollisionsAllowed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetFastCollisionsAllowed(this, value);
			}
		}

		public void ApplyForce(Vector2 force, Vector2 point, bool wake = true)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_ApplyForce(this, force, point, wake);
		}

		public void ApplyForceToCenter(Vector2 force, bool wake = true)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_ApplyForceToCenter(this, force, wake);
		}

		public void ApplyTorque(float torque, bool wake = true)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_ApplyTorque(this, torque, wake);
		}

		public void ApplyLinearImpulse(Vector2 impulse, Vector2 point, bool wake = true)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_ApplyLinearImpulse(this, impulse, point, wake);
		}

		public void ApplyLinearImpulseToCenter(Vector2 impulse, bool wake = true)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_ApplyLinearImpulseToCenter(this, impulse, wake);
		}

		public void ApplyAngularImpulse(float impulse, bool wake = true)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_ApplyAngularImpulse(this, impulse, wake);
		}

		public void ClearForces()
		{
			PhysicsLowLevelScripting2D.PhysicsBody_ClearForces(this);
		}

		public void WakeTouching()
		{
			PhysicsLowLevelScripting2D.PhysicsBody_WakeTouching(this);
		}

		public void SetContactEvents(bool contactEvents)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetContactEvents(this, contactEvents);
		}

		public void SetHitEvents(bool hitEvents)
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetHitEvents(this, hitEvents);
		}

		public int SetOwner(Object owner)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_SetOwner(this, owner);
		}

		public Object GetOwner()
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetOwner(this);
		}

		public bool isOwned
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_IsOwned(this);
			}
		}

		public object callbackTarget
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetCallbackTarget(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetCallbackTarget(this, value);
			}
		}

		public PhysicsUserData userData
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetUserData(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetUserData(this, value);
			}
		}

		public Transform transformObject
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetTransformObject(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetTransformObject(this, value);
			}
		}

		public PhysicsBody.TransformWriteMode transformWriteMode
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetTransformWriteMode(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsBody_SetTransformWriteMode(this, value);
			}
		}

		public int shapeCount
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetShapeCount(this);
			}
		}

		public NativeArray<PhysicsShape> GetShapes(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetShapes(this, allocator).ToNativeArray<PhysicsShape>();
		}

		public int jointCount
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetJointCount(this);
			}
		}

		public NativeArray<PhysicsJoint> GetJoints(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetJoints(this, allocator).ToNativeArray<PhysicsJoint>();
		}

		public NativeArray<PhysicsShape.Contact> GetContacts(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetContacts(this, allocator).ToNativeArray<PhysicsShape.Contact>();
		}

		public PhysicsAABB GetAABB()
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_CalculateAABB(this);
		}

		public PhysicsShape CreateShape(CircleGeometry geometry)
		{
			return PhysicsShape.CreateShape(this, geometry);
		}

		public PhysicsShape CreateShape(CircleGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsShape.CreateShape(this, geometry, definition);
		}

		public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<CircleGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
		}

		public PhysicsShape CreateShape(PolygonGeometry geometry)
		{
			return PhysicsShape.CreateShape(this, geometry);
		}

		public PhysicsShape CreateShape(PolygonGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsShape.CreateShape(this, geometry, definition);
		}

		public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<PolygonGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
		}

		public PhysicsShape CreateShape(CapsuleGeometry geometry)
		{
			return PhysicsShape.CreateShape(this, geometry);
		}

		public PhysicsShape CreateShape(CapsuleGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsShape.CreateShape(this, geometry, definition);
		}

		public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<CapsuleGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
		}

		public PhysicsShape CreateShape(SegmentGeometry geometry)
		{
			return PhysicsShape.CreateShape(this, geometry);
		}

		public PhysicsShape CreateShape(SegmentGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsShape.CreateShape(this, geometry, definition);
		}

		public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<SegmentGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
		}

		public PhysicsShape CreateShape(ChainSegmentGeometry geometry)
		{
			return PhysicsShape.CreateShape(this, geometry);
		}

		public PhysicsShape CreateShape(ChainSegmentGeometry geometry, PhysicsShapeDefinition definition)
		{
			return PhysicsShape.CreateShape(this, geometry, definition);
		}

		public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<ChainSegmentGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
		{
			return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
		}

		public PhysicsChain CreateChain(ChainGeometry geometry, PhysicsChainDefinition definition)
		{
			return PhysicsChain.Create(this, geometry, definition);
		}

		public void Draw()
		{
			PhysicsLowLevelScripting2D.PhysicsBody_Draw(this);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("PhysicsBody.bodyType has been deprecated. Please use PhysicsBody.type instead.", false)]
		public RigidbodyType2D bodyType
		{
			get
			{
				return (RigidbodyType2D)this.type;
			}
			set
			{
				this.type = (PhysicsBody.BodyType)value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("PhysicsBody.bodyConstraints has been deprecated. Please use PhysicsBody.constraints instead.", false)]
		public RigidbodyConstraints2D bodyConstraints
		{
			get
			{
				return (RigidbodyConstraints2D)this.constraints;
			}
			set
			{
				this.constraints = (PhysicsBody.BodyConstraints)value;
			}
		}

		private readonly int m_Index1;

		private readonly ushort m_World0;

		private readonly ushort m_Generation;

		public enum BodyType
		{
			Dynamic,
			Kinematic,
			Static
		}

		[Flags]
		public enum BodyConstraints
		{
			None = 0,
			PositionX = 1,
			PositionY = 2,
			Rotation = 4,
			Position = 3,
			All = 7
		}

		public enum TransformWriteMode
		{
			Current,
			Interpolate,
			Extrapolate,
			Off
		}

		public struct TransformWriteTween
		{
			public PhysicsBody body
			{
				readonly get
				{
					return this.m_Body;
				}
				set
				{
					this.m_Body = value;
				}
			}

			public PhysicsBody.TransformWriteMode transformWriteMode
			{
				readonly get
				{
					return this.m_TransformWriteMode;
				}
				set
				{
					this.m_TransformWriteMode = value;
				}
			}

			public PhysicsTransform physicsTransform
			{
				readonly get
				{
					return this.m_PhysicsTransform;
				}
				set
				{
					this.m_PhysicsTransform = value;
				}
			}

			public Vector2 linearVelocity
			{
				readonly get
				{
					return this.m_LinearVelocity;
				}
				set
				{
					this.m_LinearVelocity = value;
				}
			}

			public float angularVelocity
			{
				readonly get
				{
					return this.m_AngularVelocity;
				}
				set
				{
					this.m_AngularVelocity = value;
				}
			}

			public Vector3 positionFrom
			{
				readonly get
				{
					return this.m_PositionFrom;
				}
				set
				{
					this.m_PositionFrom = value;
				}
			}

			public Quaternion rotationFrom
			{
				readonly get
				{
					return this.m_RotationFrom;
				}
				set
				{
					this.m_RotationFrom = value;
				}
			}

			private PhysicsBody m_Body;

			private PhysicsBody.TransformWriteMode m_TransformWriteMode;

			private PhysicsTransform m_PhysicsTransform;

			private Vector2 m_LinearVelocity;

			private float m_AngularVelocity;

			private Vector3 m_PositionFrom;

			private Quaternion m_RotationFrom;
		}

		[Serializable]
		public struct MassConfiguration
		{
			public float mass
			{
				readonly get
				{
					return this.m_Mass;
				}
				set
				{
					this.m_Mass = value;
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

			public float rotationalInertia
			{
				readonly get
				{
					return this.m_RotationalInertia;
				}
				set
				{
					this.m_RotationalInertia = value;
				}
			}

			[SerializeField]
			private float m_Mass;

			[SerializeField]
			private Vector2 m_Center;

			[SerializeField]
			private float m_RotationalInertia;
		}

		public struct BatchVelocity
		{
			public BatchVelocity(PhysicsBody physicsBody)
			{
				this.m_PhysicsBody = physicsBody;
				this.m_LinearVelocity = default(Vector2);
				this.m_AngularVelocity = 0f;
				this.m_UseLinearVelocity = false;
				this.m_UseAngularVelocity = false;
			}

			public PhysicsBody physicsBody
			{
				readonly get
				{
					return this.m_PhysicsBody;
				}
				set
				{
					this.m_PhysicsBody = value;
				}
			}

			public Vector2 linearVelocity
			{
				readonly get
				{
					return this.m_LinearVelocity;
				}
				set
				{
					this.m_LinearVelocity = value;
					this.m_UseLinearVelocity = true;
				}
			}

			public float angularVelocity
			{
				readonly get
				{
					return this.m_AngularVelocity;
				}
				set
				{
					this.m_AngularVelocity = value;
					this.m_UseAngularVelocity = true;
				}
			}

			private PhysicsBody m_PhysicsBody;

			private Vector2 m_LinearVelocity;

			private float m_AngularVelocity;

			private bool m_UseLinearVelocity;

			private bool m_UseAngularVelocity;
		}

		public struct BatchForce
		{
			public BatchForce(PhysicsBody physicsBody)
			{
				this.m_PhysicsBody = physicsBody;
				this.m_LinearForce = default(Vector2);
				this.m_LinearForcePosition = default(Vector2);
				this.m_Torque = 0f;
				this.m_WakeBody = false;
				this.m_UseLinearForce = false;
				this.m_UseLinearForcePosition = false;
				this.m_UseTorque = false;
			}

			public PhysicsBody physicsBody
			{
				readonly get
				{
					return this.m_PhysicsBody;
				}
				set
				{
					this.m_PhysicsBody = value;
				}
			}

			public void ApplyForce(Vector2 force, Vector2 point, bool wake = true)
			{
				this.m_LinearForce = force;
				this.m_LinearForcePosition = point;
				this.m_WakeBody = wake;
				this.m_UseLinearForce = true;
				this.m_UseLinearForcePosition = true;
			}

			public void ApplyForceToCenter(Vector2 force, bool wake = true)
			{
				this.m_LinearForce = force;
				this.m_WakeBody = wake;
				this.m_UseLinearForce = true;
				this.m_UseLinearForcePosition = false;
			}

			public void ApplyTorque(float torque, bool wake = true)
			{
				this.m_Torque = torque;
				this.m_WakeBody = wake;
				this.m_UseTorque = true;
			}

			private PhysicsBody m_PhysicsBody;

			private Vector2 m_LinearForce;

			private Vector2 m_LinearForcePosition;

			private float m_Torque;

			private bool m_WakeBody;

			private bool m_UseLinearForce;

			private bool m_UseLinearForcePosition;

			private bool m_UseTorque;
		}

		public struct BatchImpulse
		{
			public BatchImpulse(PhysicsBody physicsBody)
			{
				this.m_PhysicsBody = physicsBody;
				this.m_LinearImpulse = default(Vector2);
				this.m_LinearImpulsePosition = default(Vector2);
				this.m_AngularImpulse = 0f;
				this.m_WakeBody = false;
				this.m_UseLinearImpulse = false;
				this.m_UseLinearImpulsePosition = false;
				this.m_UseAngularImpulse = false;
			}

			public PhysicsBody physicsBody
			{
				readonly get
				{
					return this.m_PhysicsBody;
				}
				set
				{
					this.m_PhysicsBody = value;
				}
			}

			public void ApplyLinearImpulse(Vector2 impulse, Vector2 point, bool wake = true)
			{
				this.m_LinearImpulse = impulse;
				this.m_LinearImpulsePosition = point;
				this.m_WakeBody = wake;
				this.m_UseLinearImpulse = true;
				this.m_UseLinearImpulsePosition = true;
			}

			public void ApplyLinearImpulseToCenter(Vector2 impulse, bool wake = true)
			{
				this.m_LinearImpulse = impulse;
				this.m_WakeBody = wake;
				this.m_UseLinearImpulse = true;
				this.m_UseLinearImpulsePosition = false;
			}

			public void ApplyAngularImpulse(float impulse, bool wake = true)
			{
				this.m_AngularImpulse = impulse;
				this.m_WakeBody = true;
				this.m_UseAngularImpulse = true;
			}

			private PhysicsBody m_PhysicsBody;

			private Vector2 m_LinearImpulse;

			private Vector2 m_LinearImpulsePosition;

			private float m_AngularImpulse;

			private bool m_WakeBody;

			private bool m_UseLinearImpulse;

			private bool m_UseLinearImpulsePosition;

			private bool m_UseAngularImpulse;
		}

		public struct BatchTransform
		{
			public BatchTransform(PhysicsBody physicsBody)
			{
				this.m_PhysicsBody = physicsBody;
				this.m_PhysicsTransform = default(PhysicsTransform);
				this.m_UsePosition = false;
				this.m_UseRotation = false;
			}

			public PhysicsBody physicsBody
			{
				readonly get
				{
					return this.m_PhysicsBody;
				}
				set
				{
					this.m_PhysicsBody = value;
				}
			}

			public Vector2 position
			{
				readonly get
				{
					return this.m_PhysicsTransform.position;
				}
				set
				{
					this.m_PhysicsTransform.position = value;
					this.m_UsePosition = true;
				}
			}

			public PhysicsRotate rotation
			{
				readonly get
				{
					return this.m_PhysicsTransform.rotation;
				}
				set
				{
					this.m_PhysicsTransform.rotation = value;
					this.m_UseRotation = true;
				}
			}

			public PhysicsTransform transform
			{
				readonly get
				{
					return this.m_PhysicsTransform;
				}
				set
				{
					this.m_PhysicsTransform = value;
					this.m_UsePosition = (this.m_UseRotation = true);
				}
			}

			private PhysicsBody m_PhysicsBody;

			private PhysicsTransform m_PhysicsTransform;

			private bool m_UsePosition;

			private bool m_UseRotation;
		}
	}
}
