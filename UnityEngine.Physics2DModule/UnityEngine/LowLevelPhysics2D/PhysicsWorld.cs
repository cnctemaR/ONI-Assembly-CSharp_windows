using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsWorld : IEquatable<PhysicsWorld>
	{
		[Obsolete("PhysicsWorld.simulationMode has been deprecated. Please use PhysicsWorld.simulationType instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SimulationMode2D simulationMode
		{
			get
			{
				return (SimulationMode2D)this.simulationType;
			}
			set
			{
				this.simulationType = (PhysicsWorld.SimulationType)value;
			}
		}

		public override string ToString()
		{
			return this.isValid ? string.Format("index={0}, generation={1}", this.m_Index1, this.m_Generation) : "<INVALID>";
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(PhysicsWorld other)
		{
			return this.m_Index1 == other.m_Index1 && this.m_Generation == other.m_Generation;
		}

		public static bool operator ==(PhysicsWorld lhs, PhysicsWorld rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsWorld lhs, PhysicsWorld rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<ushort, ushort>(this.m_Index1, this.m_Generation);
		}

		public static bool safetyLocksEnabled
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsGlobal_GetSafetyLocksEnabled();
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsGlobal_SetSafetyLocksEnabled(value);
			}
		}

		public static bool bypassLowLevel
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsGlobal_GetBypassLowLevel();
			}
		}

		public static bool isRenderingAllowed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsGlobal_IsRenderingAllowed();
			}
		}

		public static int worldCount
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetWorldCount();
			}
		}

		public static int concurrentSimulations
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsGlobal_GetConcurrentSimulations();
			}
		}

		public static float lengthUnitsPerMeter
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsGlobal_GetLengthUnitsPerMeter();
			}
		}

		public static bool useFullLayers
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsGlobal_GetUseFullLayers();
			}
		}

		public static float hugeWorldExtent
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetHugeWorldExtent();
			}
		}

		public static float linearSlop
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetLinearSlop();
			}
		}

		public static float speculativeContactDistance
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetSpeculativeContactDistance();
			}
		}

		public static float aabbMargin
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetAABBMargin();
			}
		}

		public static float bodyMaxRotation
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyMaxRotation();
			}
		}

		public static float bodyTimeToSleep
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyTimeToSleep();
			}
		}

		public static PhysicsWorld defaultWorld
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultWorld();
			}
		}

		public static NativeArray<PhysicsWorld> GetWorlds(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetWorlds(allocator).ToNativeArray<PhysicsWorld>();
		}

		public NativeArray<PhysicsBody> GetBodies(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodies(this, allocator).ToNativeArray<PhysicsBody>();
		}

		public NativeArray<PhysicsJoint> GetJoints(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetJoints(this, allocator).ToNativeArray<PhysicsJoint>();
		}

		public static PhysicsWorld Create()
		{
			return PhysicsWorld.Create(PhysicsWorldDefinition.defaultDefinition);
		}

		public static PhysicsWorld Create(PhysicsWorldDefinition definition)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_Create(definition);
		}

		public bool Destroy(int ownerKey = 0)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_Destroy(this, ownerKey);
		}

		public PhysicsWorldDefinition definition
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_ReadDefinition(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_WriteDefinition(this, value, false);
			}
		}

		public int SetOwner(Object owner)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_SetOwner(this, owner);
		}

		public Object GetOwner()
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetOwner(this);
		}

		public bool isOwned
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_IsOwned(this);
			}
		}

		public PhysicsUserData userData
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetUserData(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetUserData(this, value);
			}
		}

		public void Reset()
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_Reset(this);
		}

		public bool isValid
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_IsValid(this);
			}
		}

		public bool isEmpty
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_IsEmpty(this);
			}
		}

		public bool isDefaultWorld
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_IsDefaultWorld(this);
			}
		}

		public bool paused
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetPaused(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetPaused(this, value);
			}
		}

		public bool sleepingAllowed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetSleepingAllowed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetSleepingAllowed(this, value);
			}
		}

		public bool continuousAllowed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContinuousAllowed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetContinuousAllowed(this, value);
			}
		}

		public bool contactFilterCallbacks
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactFilterCallbacks(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetContactFilterCallbacks(this, value);
			}
		}

		public bool preSolveCallbacks
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetPreSolveCallbacks(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetPreSolveCallbacks(this, value);
			}
		}

		public bool autoBodyUpdateCallbacks
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetAutoBodyUpdateCallbacks(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetAutoBodyUpdateCallbacks(this, value);
			}
		}

		public bool autoContactCallbacks
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetAutoContactCallbacks(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetAutoContactCallbacks(this, value);
			}
		}

		public bool autoTriggerCallbacks
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetAutoTriggerCallbacks(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetAutoTriggerCallbacks(this, value);
			}
		}

		public bool autoJointThresholdCallbacks
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetAutoJointThresholdCallbacks(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetAutoJointThresholdCallbacks(this, value);
			}
		}

		public bool warmStartingAllowed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetWarmStartingAllowed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetWarmStartingAllowed(this, value);
			}
		}

		public float bounceThreshold
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetBounceThreshold(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetBounceThreshold(this, value);
			}
		}

		public float contactHitEventThreshold
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactHitEventThreshold(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetContactHitEventThreshold(this, value);
			}
		}

		public float contactFrequency
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactFrequency(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetContactFrequency(this, value);
			}
		}

		public float contactDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetContactDamping(this, value);
			}
		}

		public float contactSpeed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactSpeed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetContactSpeed(this, value);
			}
		}

		public float maximumLinearSpeed
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetMaximumLinearSpeed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetMaximumLinearSpeed(this, value);
			}
		}

		public Vector2 gravity
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetGravity(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetGravity(this, value);
			}
		}

		public int simulationWorkers
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetSimulationWorkers(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetSimulationWorkers(this, value);
			}
		}

		public PhysicsWorld.SimulationType simulationType
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetSimulationType(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetSimulationType(this, value);
			}
		}

		public int simulationSubSteps
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetSimulationSubSteps(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetSimulationSubSteps(this, value);
			}
		}

		public double lastSimulationTimestamp
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetLastSimulationTimestamp(this);
			}
		}

		public float lastSimulationDeltaTime
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetLastSimulationDeltaTime(this);
			}
		}

		public PhysicsWorld.TransformPlane transformPlane
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetTransformPlane(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetTransformPlane(this, value);
			}
		}

		public PhysicsWorld.TransformWriteMode transformWriteMode
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetTransformWriteMode(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetTransformWriteMode(this, value);
			}
		}

		public bool transformTweening
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetTransformTweening(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetTransformTweening(this, value);
			}
		}

		internal void ClearTransformWriteTweens()
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_ClearTransformWriteTweens(this);
		}

		internal void SetTransformWriteTweens(ReadOnlySpan<PhysicsBody.TransformWriteTween> transformWriteTweens)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetTransformWriteTweens(this, transformWriteTweens);
		}

		public NativeArray<PhysicsBody.TransformWriteTween> GetTransformWriteTweens()
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetTransformWriteTweens(this).ToNativeArray<PhysicsBody.TransformWriteTween>();
		}

		public void Simulate(float deltaTime)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_Simulate(this, deltaTime, PhysicsWorld.SimulationType.Script);
		}

		public static void Simulate(ReadOnlySpan<PhysicsWorld> worlds, float deltaTime)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SimulateBatch(worlds, deltaTime, PhysicsWorld.SimulationType.Script);
		}

		public void Explode(PhysicsWorld.ExplosionDefinition definition)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_Explode(this, definition);
		}

		public NativeArray<PhysicsUserData> GetBodyUpdateUserData(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyUpdateUserData(this, allocator).ToNativeArray<PhysicsUserData>();
		}

		public ReadOnlySpan<PhysicsEvents.BodyUpdateEvent> bodyUpdateEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyUpdateEvents(this).ToReadOnlySpan<PhysicsEvents.BodyUpdateEvent>();
			}
		}

		public ReadOnlySpan<PhysicsEvents.TriggerBeginEvent> triggerBeginEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetTriggerBeginEvents(this).ToReadOnlySpan<PhysicsEvents.TriggerBeginEvent>();
			}
		}

		public ReadOnlySpan<PhysicsEvents.TriggerEndEvent> triggerEndEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetTriggerEndEvents(this).ToReadOnlySpan<PhysicsEvents.TriggerEndEvent>();
			}
		}

		public ReadOnlySpan<PhysicsEvents.ContactBeginEvent> contactBeginEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactBeginEvents(this).ToReadOnlySpan<PhysicsEvents.ContactBeginEvent>();
			}
		}

		public ReadOnlySpan<PhysicsEvents.ContactEndEvent> contactEndEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactEndEvents(this).ToReadOnlySpan<PhysicsEvents.ContactEndEvent>();
			}
		}

		public ReadOnlySpan<PhysicsEvents.ContactHitEvent> contactHitEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactHitEvents(this).ToReadOnlySpan<PhysicsEvents.ContactHitEvent>();
			}
		}

		public ReadOnlySpan<PhysicsEvents.JointThresholdEvent> jointThresholdEvents
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetJointThresholdEvents(this).ToReadOnlySpan<PhysicsEvents.JointThresholdEvent>();
			}
		}

		public void SendAllCallbacks()
		{
			this.SendBodyUpdateCallbacks();
			this.SendTriggerCallbacks();
			this.SendContactCallbacks();
			this.SendJointThresholdCallbacks();
		}

		public unsafe void SendBodyUpdateCallbacks()
		{
			using (PhysicsCallbacks.BodyUpdateCallbackTargets bodyUpdateCallbackTargets = PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyUpdateCallbackTargets(this, Allocator.Temp))
			{
				ReadOnlySpan<PhysicsCallbacks.BodyUpdateCallbackTargets.BodyUpdateTarget> bodyUpdateCallbackTargets2 = bodyUpdateCallbackTargets.bodyUpdateCallbackTargets;
				for (int i = 0; i < bodyUpdateCallbackTargets2.Length; i++)
				{
					PhysicsCallbacks.BodyUpdateCallbackTargets.BodyUpdateTarget bodyUpdateTarget = *bodyUpdateCallbackTargets2[i];
					PhysicsCallbacks.IBodyUpdateCallback bodyTarget = bodyUpdateTarget.bodyTarget;
					if (bodyTarget != null)
					{
						bodyTarget.OnBodyUpdate2D(bodyUpdateTarget.bodyUpdateEvent);
					}
				}
			}
		}

		public unsafe void SendContactCallbacks()
		{
			using (PhysicsCallbacks.ContactCallbackTargets contactCallbackTargets = PhysicsLowLevelScripting2D.PhysicsWorld_GetContactCallbackTargets(this, Allocator.Temp))
			{
				ReadOnlySpan<PhysicsCallbacks.ContactCallbackTargets.ContactBeginTarget> beginCallbackTargets = contactCallbackTargets.BeginCallbackTargets;
				for (int i = 0; i < beginCallbackTargets.Length; i++)
				{
					PhysicsCallbacks.ContactCallbackTargets.ContactBeginTarget contactBeginTarget = *beginCallbackTargets[i];
					PhysicsCallbacks.IContactCallback shapeTargetA = contactBeginTarget.shapeTargetA;
					if (shapeTargetA != null)
					{
						shapeTargetA.OnContactBegin2D(contactBeginTarget.beginEvent);
					}
					PhysicsCallbacks.IContactCallback shapeTargetB = contactBeginTarget.shapeTargetB;
					if (shapeTargetB != null)
					{
						shapeTargetB.OnContactBegin2D(contactBeginTarget.beginEvent);
					}
				}
				ReadOnlySpan<PhysicsCallbacks.ContactCallbackTargets.ContactEndTarget> endCallbackTargets = contactCallbackTargets.EndCallbackTargets;
				for (int j = 0; j < endCallbackTargets.Length; j++)
				{
					PhysicsCallbacks.ContactCallbackTargets.ContactEndTarget contactEndTarget = *endCallbackTargets[j];
					PhysicsCallbacks.IContactCallback shapeTargetA2 = contactEndTarget.shapeTargetA;
					if (shapeTargetA2 != null)
					{
						shapeTargetA2.OnContactEnd2D(contactEndTarget.endEvent);
					}
					PhysicsCallbacks.IContactCallback shapeTargetB2 = contactEndTarget.shapeTargetB;
					if (shapeTargetB2 != null)
					{
						shapeTargetB2.OnContactEnd2D(contactEndTarget.endEvent);
					}
				}
			}
		}

		public unsafe void SendTriggerCallbacks()
		{
			using (PhysicsCallbacks.TriggerCallbackTargets triggerCallbackTargets = PhysicsLowLevelScripting2D.PhysicsWorld_GetTriggerCallbackTargets(this, Allocator.Temp))
			{
				ReadOnlySpan<PhysicsCallbacks.TriggerCallbackTargets.TriggerBeginTarget> beginCallbackTargets = triggerCallbackTargets.BeginCallbackTargets;
				for (int i = 0; i < beginCallbackTargets.Length; i++)
				{
					PhysicsCallbacks.TriggerCallbackTargets.TriggerBeginTarget triggerBeginTarget = *beginCallbackTargets[i];
					PhysicsCallbacks.ITriggerCallback triggerShapeTarget = triggerBeginTarget.triggerShapeTarget;
					if (triggerShapeTarget != null)
					{
						triggerShapeTarget.OnTriggerBegin2D(triggerBeginTarget.beginEvent);
					}
					PhysicsCallbacks.ITriggerCallback visitorShapeTarget = triggerBeginTarget.visitorShapeTarget;
					if (visitorShapeTarget != null)
					{
						visitorShapeTarget.OnTriggerBegin2D(triggerBeginTarget.beginEvent);
					}
				}
				ReadOnlySpan<PhysicsCallbacks.TriggerCallbackTargets.TriggerEndTarget> endCallbackTargets = triggerCallbackTargets.EndCallbackTargets;
				for (int j = 0; j < endCallbackTargets.Length; j++)
				{
					PhysicsCallbacks.TriggerCallbackTargets.TriggerEndTarget triggerEndTarget = *endCallbackTargets[j];
					PhysicsCallbacks.ITriggerCallback triggerShapeTarget2 = triggerEndTarget.triggerShapeTarget;
					if (triggerShapeTarget2 != null)
					{
						triggerShapeTarget2.OnTriggerEnd2D(triggerEndTarget.endEvent);
					}
					PhysicsCallbacks.ITriggerCallback visitorShapeTarget2 = triggerEndTarget.visitorShapeTarget;
					if (visitorShapeTarget2 != null)
					{
						visitorShapeTarget2.OnTriggerEnd2D(triggerEndTarget.endEvent);
					}
				}
			}
		}

		public unsafe void SendJointThresholdCallbacks()
		{
			using (PhysicsCallbacks.JointThresholdCallbackTargets jointThresholdCallbackTargets = PhysicsLowLevelScripting2D.PhysicsWorld_GetJointThresholdCallbackTargets(this, Allocator.Temp))
			{
				ReadOnlySpan<PhysicsCallbacks.JointThresholdCallbackTargets.JointThresholdTarget> jointThresholdCallbackTargets2 = jointThresholdCallbackTargets.jointThresholdCallbackTargets;
				for (int i = 0; i < jointThresholdCallbackTargets2.Length; i++)
				{
					PhysicsCallbacks.JointThresholdCallbackTargets.JointThresholdTarget jointThresholdTarget = *jointThresholdCallbackTargets2[i];
					PhysicsCallbacks.IJointThresholdCallback jointTarget = jointThresholdTarget.jointTarget;
					if (jointTarget != null)
					{
						jointTarget.OnJointThreshold2D(jointThresholdTarget.jointThresholdEvent);
					}
				}
			}
		}

		public PhysicsCallbacks.BodyUpdateCallbackTargets GetBodyUpdateCallbackTargets(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyUpdateCallbackTargets(this, allocator);
		}

		public PhysicsCallbacks.TriggerCallbackTargets GetTriggerCallbackTargets(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetTriggerCallbackTargets(this, allocator);
		}

		public PhysicsCallbacks.ContactCallbackTargets GetContactCallbackTargets(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactCallbackTargets(this, allocator);
		}

		public PhysicsCallbacks.JointThresholdCallbackTargets GetJointThresholdCallbackTargets(Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetJointThresholdCallbackTargets(this, allocator);
		}

		public bool TestOverlapAABB(PhysicsAABB aabb, PhysicsQuery.QueryFilter filter)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_TestOverlapAABB(this, aabb, filter);
		}

		public bool TestOverlapShape(PhysicsShape shape, PhysicsQuery.QueryFilter filter)
		{
			PhysicsShape.ShapeType shapeType = shape.shapeType;
			if (!true)
			{
			}
			bool flag;
			switch (shapeType)
			{
			case PhysicsShape.ShapeType.Circle:
				flag = this.TestOverlapGeometry(shape.circleGeometry.Transform(shape.body.transform), filter);
				break;
			case PhysicsShape.ShapeType.Capsule:
				flag = this.TestOverlapGeometry(shape.capsuleGeometry.Transform(shape.body.transform), filter);
				break;
			case PhysicsShape.ShapeType.Segment:
				flag = this.TestOverlapGeometry(shape.segmentGeometry.Transform(shape.body.transform), filter);
				break;
			case PhysicsShape.ShapeType.Polygon:
				flag = this.TestOverlapGeometry(shape.polygonGeometry.Transform(shape.body.transform), filter);
				break;
			case PhysicsShape.ShapeType.ChainSegment:
				flag = this.TestOverlapGeometry(shape.chainSegmentGeometry.Transform(shape.body.transform), filter);
				break;
			default:
				throw new ArgumentException("Invalid shape type used.", "shape");
			}
			if (!true)
			{
			}
			return flag;
		}

		public bool TestOverlapShapeProxy(PhysicsShape.ShapeProxy shapeProxy, PhysicsQuery.QueryFilter filter)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_TestOverlapShapeProxy(this, shapeProxy, filter);
		}

		public bool TestOverlapPoint(Vector2 point, PhysicsQuery.QueryFilter filter)
		{
			return this.TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(point), filter);
		}

		public bool TestOverlapGeometry(CircleGeometry geometry, PhysicsQuery.QueryFilter filter)
		{
			return this.TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
		}

		public bool TestOverlapGeometry(CapsuleGeometry geometry, PhysicsQuery.QueryFilter filter)
		{
			return this.TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
		}

		public bool TestOverlapGeometry(PolygonGeometry geometry, PhysicsQuery.QueryFilter filter)
		{
			return this.TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
		}

		public bool TestOverlapGeometry(SegmentGeometry geometry, PhysicsQuery.QueryFilter filter)
		{
			return this.TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
		}

		public bool TestOverlapGeometry(ChainSegmentGeometry geometry, PhysicsQuery.QueryFilter filter)
		{
			return this.TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapAABB(PhysicsAABB aabb, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_OverlapAABB(this, aabb, filter, allocator).ToNativeArray<PhysicsQuery.WorldOverlapResult>();
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapShape(PhysicsShape shape, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			PhysicsShape.ShapeType shapeType = shape.shapeType;
			if (!true)
			{
			}
			NativeArray<PhysicsQuery.WorldOverlapResult> nativeArray;
			switch (shapeType)
			{
			case PhysicsShape.ShapeType.Circle:
				nativeArray = this.OverlapGeometry(shape.circleGeometry.Transform(shape.body.transform), filter, allocator);
				break;
			case PhysicsShape.ShapeType.Capsule:
				nativeArray = this.OverlapGeometry(shape.capsuleGeometry.Transform(shape.body.transform), filter, allocator);
				break;
			case PhysicsShape.ShapeType.Segment:
				nativeArray = this.OverlapGeometry(shape.segmentGeometry.Transform(shape.body.transform), filter, allocator);
				break;
			case PhysicsShape.ShapeType.Polygon:
				nativeArray = this.OverlapGeometry(shape.polygonGeometry.Transform(shape.body.transform), filter, allocator);
				break;
			case PhysicsShape.ShapeType.ChainSegment:
				nativeArray = this.OverlapGeometry(shape.chainSegmentGeometry.segment.Transform(shape.body.transform), filter, allocator);
				break;
			default:
				throw new ArgumentException("Invalid shape type used.", "shape");
			}
			if (!true)
			{
			}
			return nativeArray;
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapShapeProxy(PhysicsShape.ShapeProxy shapeProxy, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_OverlapShapeProxy(this, shapeProxy, filter, allocator).ToNativeArray<PhysicsQuery.WorldOverlapResult>();
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapPoint(Vector2 point, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			return this.OverlapShapeProxy(new PhysicsShape.ShapeProxy(point), filter, allocator);
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(CircleGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			return this.OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(CapsuleGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			return this.OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(PolygonGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			return this.OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(SegmentGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			return this.OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
		}

		public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(ChainSegmentGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
		{
			return this.OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
		}

		public NativeArray<PhysicsQuery.WorldCastResult> CastRay(PhysicsQuery.CastRayInput input, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_CastRay(this, input, filter, castMode, allocator).ToNativeArray<PhysicsQuery.WorldCastResult>();
		}

		public NativeArray<PhysicsQuery.WorldCastResult> CastShape(PhysicsShape shape, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
		{
			PhysicsShape.ShapeType shapeType = shape.shapeType;
			if (!true)
			{
			}
			NativeArray<PhysicsQuery.WorldCastResult> nativeArray;
			switch (shapeType)
			{
			case PhysicsShape.ShapeType.Circle:
				nativeArray = this.CastGeometry(shape.circleGeometry.Transform(shape.body.transform), translation, filter, castMode, allocator);
				goto IL_00C1;
			case PhysicsShape.ShapeType.Capsule:
				nativeArray = this.CastGeometry(shape.capsuleGeometry.Transform(shape.body.transform), translation, filter, castMode, allocator);
				goto IL_00C1;
			case PhysicsShape.ShapeType.Polygon:
				nativeArray = this.CastGeometry(shape.polygonGeometry.Transform(shape.body.transform), translation, filter, castMode, allocator);
				goto IL_00C1;
			}
			throw new ArgumentException("Invalid shape type used for cast.", "shape");
			IL_00C1:
			if (!true)
			{
			}
			return nativeArray;
		}

		public NativeArray<PhysicsQuery.WorldCastResult> CastShapeProxy(PhysicsShape.ShapeProxy shapeProxy, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_CastShapeProxy(this, shapeProxy, translation, filter, castMode, allocator).ToNativeArray<PhysicsQuery.WorldCastResult>();
		}

		public PhysicsQuery.WorldMoverResult CastMover(PhysicsQuery.WorldMoverInput input)
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_CastMover(this, input);
		}

		public NativeArray<PhysicsQuery.WorldCastResult> CastGeometry(CircleGeometry geometry, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
		{
			return this.CastShapeProxy(new PhysicsShape.ShapeProxy(geometry), translation, filter, castMode, allocator);
		}

		public NativeArray<PhysicsQuery.WorldCastResult> CastGeometry(CapsuleGeometry geometry, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
		{
			return this.CastShapeProxy(new PhysicsShape.ShapeProxy(geometry), translation, filter, castMode, allocator);
		}

		public NativeArray<PhysicsQuery.WorldCastResult> CastGeometry(PolygonGeometry geometry, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
		{
			return this.CastShapeProxy(new PhysicsShape.ShapeProxy(geometry), translation, filter, castMode, allocator);
		}

		public PhysicsBody CreateBody()
		{
			return PhysicsBody.Create(this);
		}

		public PhysicsBody CreateBody(PhysicsBodyDefinition definition)
		{
			return PhysicsBody.Create(this, definition);
		}

		public NativeArray<PhysicsBody> CreateBodyBatch(PhysicsBodyDefinition definition, int bodyCount, Allocator allocator = Allocator.Temp)
		{
			return PhysicsBody.CreateBatch(this, definition, bodyCount, allocator);
		}

		public NativeArray<PhysicsBody> CreateBodyBatch(ReadOnlySpan<PhysicsBodyDefinition> definitions, Allocator allocator = Allocator.Temp)
		{
			return PhysicsBody.CreateBatch(this, definitions, allocator);
		}

		public static void DestroyBodyBatch(ReadOnlySpan<PhysicsBody> bodies)
		{
			PhysicsBody.DestroyBatch(bodies);
		}

		public static void DestroyShapeBatch(ReadOnlySpan<PhysicsShape> shapes, bool updateBodyMass)
		{
			PhysicsShape.DestroyBatch(shapes, updateBodyMass);
		}

		public static void DestroyJointBatch(ReadOnlySpan<PhysicsJoint> joints)
		{
			PhysicsJoint.DestroyBatch(joints);
		}

		public PhysicsDistanceJoint CreateJoint(PhysicsDistanceJointDefinition definition)
		{
			return PhysicsDistanceJoint.Create(this, definition);
		}

		public PhysicsRelativeJoint CreateJoint(PhysicsRelativeJointDefinition definition)
		{
			return PhysicsRelativeJoint.Create(this, definition);
		}

		public PhysicsIgnoreJoint CreateJoint(PhysicsIgnoreJointDefinition definition)
		{
			return PhysicsIgnoreJoint.Create(this, definition);
		}

		public PhysicsSliderJoint CreateJoint(PhysicsSliderJointDefinition definition)
		{
			return PhysicsSliderJoint.Create(this, definition);
		}

		public PhysicsHingeJoint CreateJoint(PhysicsHingeJointDefinition definition)
		{
			return PhysicsHingeJoint.Create(this, definition);
		}

		public PhysicsFixedJoint CreateJoint(PhysicsFixedJointDefinition definition)
		{
			return PhysicsFixedJoint.Create(this, definition);
		}

		public PhysicsWheelJoint CreateJoint(PhysicsWheelJointDefinition definition)
		{
			return PhysicsWheelJoint.Create(this, definition);
		}

		public int awakeBodyCount
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetAwakeBodyCount(this);
			}
		}

		public PhysicsWorld.WorldCounters counters
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetCounters(this);
			}
		}

		public static PhysicsWorld.WorldCounters globalCounters
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetGlobalCounters();
			}
		}

		public PhysicsWorld.WorldProfile profile
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetProfile(this);
			}
		}

		public static PhysicsWorld.WorldProfile globalProfile
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetGlobalProfile();
			}
		}

		public PhysicsWorld.DrawOptions drawOptions
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawOptions(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawOptions(this, value);
			}
		}

		public PhysicsWorld.DrawFillOptions drawFillOptions
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawFillOptions(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawFillOptions(this, value);
			}
		}

		public float drawThickness
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawThickness(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawThickness(this, value);
			}
		}

		public float drawFillAlpha
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawFillAlpha(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawFillAlpha(this, value);
			}
		}

		public float drawPointScale
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawPointScale(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawPointScale(this, value);
			}
		}

		public float drawNormalScale
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawNormalScale(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawNormalScale(this, value);
			}
		}

		public float drawImpulseScale
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawImpulseScale(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawImpulseScale(this, value);
			}
		}

		public int drawCapacity
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawCapacity(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawCapacity(this, value);
			}
		}

		public PhysicsWorld.DrawColors drawColors
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawColors(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawColors(this, value);
			}
		}

		public float elementDepth
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetElementDepth(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.PhysicsWorld_SetElementDepth(this, value);
			}
		}

		public void SetElementDepth3D(Vector3 position)
		{
			this.elementDepth = PhysicsMath.GetTranslationIgnoredAxis(position, this.transformPlane);
		}

		public void ClearDraw()
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_ClearDraw(this, false, true);
		}

		public void DrawGeometry(CircleGeometry geometry, PhysicsTransform transform, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.All)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawCircleGeometry(this, geometry, transform, color, lifetime, drawFillOptions);
		}

		public void DrawGeometry(ReadOnlySpan<CircleGeometry> geometry, PhysicsTransform transform, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.All)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawCircleGeometrySpan(this, geometry, transform, color, lifetime, drawFillOptions);
		}

		public void DrawGeometry(CapsuleGeometry geometry, PhysicsTransform transform, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.All)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawCapsuleGeometry(this, geometry, transform, color, lifetime, drawFillOptions);
		}

		public void DrawGeometry(ReadOnlySpan<CapsuleGeometry> geometry, PhysicsTransform transform, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.All)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawCapsuleGeometrySpan(this, geometry, transform, color, lifetime, drawFillOptions);
		}

		public void DrawGeometry(PolygonGeometry geometry, PhysicsTransform transform, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.All)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawPolygonGeometry(this, geometry, transform, color, lifetime, drawFillOptions);
		}

		public void DrawGeometry(ReadOnlySpan<PolygonGeometry> geometry, PhysicsTransform transform, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.All)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawPolygonGeometrySpan(this, geometry, transform, color, lifetime, drawFillOptions);
		}

		public void DrawGeometry(SegmentGeometry geometry, PhysicsTransform transform, Color color, float lifetime = 0f)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawSegmentGeometry(this, geometry, transform, color, lifetime);
		}

		public void DrawGeometry(ReadOnlySpan<SegmentGeometry> geometry, PhysicsTransform transform, Color color, float lifetime = 0f)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawSegmentGeometrySpan(this, geometry, transform, color, lifetime);
		}

		public void DrawBox(PhysicsTransform transform, Vector2 size, float radius, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.Outline)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawBox(this, transform, size, radius, color, lifetime, drawFillOptions);
		}

		public void DrawCircle(Vector2 center, float radius, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.Outline)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawCircle(this, center, radius, color, lifetime, drawFillOptions);
		}

		public void DrawCapsule(PhysicsTransform transform, Vector2 center1, Vector2 center2, float radius, Color color, float lifetime = 0f, PhysicsWorld.DrawFillOptions drawFillOptions = PhysicsWorld.DrawFillOptions.Outline)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawCapsule(this, transform, center1, center2, radius, color, lifetime, drawFillOptions);
		}

		public void DrawPoint(Vector2 position, float radius, Color color, float lifetime = 0f)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawPoint(this, position, radius, color, lifetime);
		}

		public void DrawLine(Vector2 point0, Vector2 point1, Color color, float lifetime = 0f)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawLine(this, point0, point1, color, lifetime);
		}

		public void DrawLineStrip(PhysicsTransform transform, ReadOnlySpan<Vector2> vertices, bool loop, Color color, float lifetime = 0f)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawLineStrip(this, transform, vertices, loop, color, lifetime);
		}

		public void DrawTransformAxis(PhysicsTransform transform, float scale, float lifetime = 0f)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawTransformAxis(this, transform, scale, lifetime);
		}

		internal void Draw(PhysicsAABB drawAABB)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_Draw(this, drawAABB);
		}

		internal static void DrawAllWorlds(PhysicsAABB drawAABB)
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_DrawAllWorlds(drawAABB);
		}

		internal PhysicsWorld.DrawResults drawResults
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawResults(this);
			}
		}

		internal readonly ushort m_Index1;

		private readonly ushort m_Generation;

		public enum SimulationType
		{
			FixedUpdate,
			Update,
			Script
		}

		public enum TransformWriteMode
		{
			Off,
			Fast2D,
			Slow3D
		}

		public enum TransformPlane
		{
			XY,
			XZ,
			ZY
		}

		[Serializable]
		public struct ExplosionDefinition
		{
			public static PhysicsWorld.ExplosionDefinition defaultDefinition
			{
				get
				{
					return PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultExplosionDefinition();
				}
			}

			public ExplosionDefinition()
			{
				this = PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultExplosionDefinition();
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

			public Vector2 position
			{
				readonly get
				{
					return this.m_Position;
				}
				set
				{
					this.m_Position = value;
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

			public float falloff
			{
				readonly get
				{
					return this.m_Falloff;
				}
				set
				{
					this.m_Falloff = Mathf.Max(0f, value);
				}
			}

			public float impulsePerLength
			{
				readonly get
				{
					return this.m_ImpulsePerLength;
				}
				set
				{
					this.m_ImpulsePerLength = value;
				}
			}

			[SerializeField]
			private PhysicsMask m_HitCategories;

			[SerializeField]
			private Vector2 m_Position;

			[Min(0f)]
			[SerializeField]
			private float m_Radius;

			[SerializeField]
			[Min(0f)]
			private float m_Falloff;

			[SerializeField]
			private float m_ImpulsePerLength;
		}

		[Serializable]
		public struct WorldCounters
		{
			public int bodyCount
			{
				readonly get
				{
					return this.m_BodyCount;
				}
				set
				{
					this.m_BodyCount = value;
				}
			}

			public int shapeCount
			{
				readonly get
				{
					return this.m_ShapeCount;
				}
				set
				{
					this.m_ShapeCount = value;
				}
			}

			public int contactCount
			{
				readonly get
				{
					return this.m_ContactCount;
				}
				set
				{
					this.m_ContactCount = value;
				}
			}

			public int jointCount
			{
				readonly get
				{
					return this.m_JointCount;
				}
				set
				{
					this.m_JointCount = value;
				}
			}

			public int islandCount
			{
				readonly get
				{
					return this.m_IslandCount;
				}
				set
				{
					this.m_IslandCount = value;
				}
			}

			public int stackUsed
			{
				readonly get
				{
					return this.m_StackUsed;
				}
				set
				{
					this.m_StackUsed = value;
				}
			}

			public int memoryUsed
			{
				readonly get
				{
					return this.m_MemoryUsed;
				}
				set
				{
					this.m_MemoryUsed = value;
				}
			}

			public int staticBroadphaseHeight
			{
				readonly get
				{
					return this.m_StaticBroadphaseHeight;
				}
				set
				{
					this.m_StaticBroadphaseHeight = value;
				}
			}

			public int broadphaseHeight
			{
				readonly get
				{
					return this.m_BroadphaseHeight;
				}
				set
				{
					this.m_BroadphaseHeight = value;
				}
			}

			public int taskCount
			{
				readonly get
				{
					return this.m_TaskCount;
				}
				set
				{
					this.m_TaskCount = value;
				}
			}

			public static PhysicsWorld.WorldCounters Add(PhysicsWorld.WorldCounters countersA, PhysicsWorld.WorldCounters countersB)
			{
				return new PhysicsWorld.WorldCounters
				{
					bodyCount = countersA.bodyCount + countersB.bodyCount,
					shapeCount = countersA.shapeCount + countersB.shapeCount,
					contactCount = countersA.contactCount + countersB.contactCount,
					jointCount = countersA.jointCount + countersB.jointCount,
					islandCount = countersA.islandCount + countersB.islandCount,
					stackUsed = countersA.stackUsed + countersB.stackUsed,
					memoryUsed = countersA.memoryUsed + countersB.memoryUsed,
					staticBroadphaseHeight = countersA.staticBroadphaseHeight + countersB.staticBroadphaseHeight,
					broadphaseHeight = countersA.broadphaseHeight + countersB.broadphaseHeight,
					taskCount = countersA.taskCount + countersB.taskCount
				};
			}

			public static PhysicsWorld.WorldCounters Maximum(PhysicsWorld.WorldCounters countersA, PhysicsWorld.WorldCounters countersB)
			{
				return new PhysicsWorld.WorldCounters
				{
					bodyCount = Mathf.Max(countersA.bodyCount, countersB.bodyCount),
					shapeCount = Mathf.Max(countersA.shapeCount, countersB.shapeCount),
					contactCount = Mathf.Max(countersA.contactCount, countersB.contactCount),
					jointCount = Mathf.Max(countersA.jointCount, countersB.jointCount),
					islandCount = Mathf.Max(countersA.islandCount, countersB.islandCount),
					stackUsed = Mathf.Max(countersA.stackUsed, countersB.stackUsed),
					memoryUsed = Mathf.Max(countersA.memoryUsed, countersB.memoryUsed),
					staticBroadphaseHeight = Mathf.Max(countersA.staticBroadphaseHeight, countersB.staticBroadphaseHeight),
					broadphaseHeight = Mathf.Max(countersA.broadphaseHeight, countersB.broadphaseHeight),
					taskCount = Mathf.Max(countersA.taskCount, countersB.taskCount)
				};
			}

			[SerializeField]
			private int m_BodyCount;

			[SerializeField]
			private int m_ShapeCount;

			[SerializeField]
			private int m_ContactCount;

			[SerializeField]
			private int m_JointCount;

			[SerializeField]
			private int m_IslandCount;

			[SerializeField]
			private int m_StackUsed;

			[SerializeField]
			private int m_StaticBroadphaseHeight;

			[SerializeField]
			private int m_BroadphaseHeight;

			[SerializeField]
			private int m_MemoryUsed;

			[SerializeField]
			private int m_TaskCount;

			[FixedBuffer(typeof(int), 24)]
			private PhysicsWorld.WorldCounters.<m_ColorCounts>e__FixedBuffer m_ColorCounts;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 96)]
			public struct <m_ColorCounts>e__FixedBuffer
			{
				public int FixedElementField;
			}
		}

		[Serializable]
		public struct WorldProfile
		{
			public float simulationStep
			{
				readonly get
				{
					return this.m_SimulationStep;
				}
				set
				{
					this.m_SimulationStep = value;
				}
			}

			public float contactPairs
			{
				readonly get
				{
					return this.m_ContactPairs;
				}
				set
				{
					this.m_ContactPairs = value;
				}
			}

			public float contactUpdates
			{
				readonly get
				{
					return this.m_ContactUpdates;
				}
				set
				{
					this.m_ContactUpdates = value;
				}
			}

			public float solving
			{
				readonly get
				{
					return this.m_Solving;
				}
				set
				{
					this.m_Solving = value;
				}
			}

			public float prepareStages
			{
				readonly get
				{
					return this.m_PrepareStages;
				}
				set
				{
					this.m_PrepareStages = value;
				}
			}

			public float solveConstraints
			{
				readonly get
				{
					return this.m_SolveConstraints;
				}
				set
				{
					this.m_SolveConstraints = value;
				}
			}

			public float prepareConstraints
			{
				readonly get
				{
					return this.m_PrepareConstraints;
				}
				set
				{
					this.m_PrepareConstraints = value;
				}
			}

			public float integrateVelocities
			{
				readonly get
				{
					return this.m_IntegrateVelocities;
				}
				set
				{
					this.m_IntegrateVelocities = value;
				}
			}

			public float warmStarting
			{
				readonly get
				{
					return this.m_WarmStarting;
				}
				set
				{
					this.m_WarmStarting = value;
				}
			}

			public float solveImpulses
			{
				readonly get
				{
					return this.m_SolveImpulses;
				}
				set
				{
					this.m_SolveImpulses = value;
				}
			}

			public float integrateTransforms
			{
				readonly get
				{
					return this.m_IntegrateTransforms;
				}
				set
				{
					this.m_IntegrateTransforms = value;
				}
			}

			public float relaxImpulses
			{
				readonly get
				{
					return this.m_RelaxImpulses;
				}
				set
				{
					this.m_RelaxImpulses = value;
				}
			}

			public float applyBounciness
			{
				readonly get
				{
					return this.m_ApplyBounciness;
				}
				set
				{
					this.m_ApplyBounciness = value;
				}
			}

			public float storeImpulses
			{
				readonly get
				{
					return this.m_StoreImpulses;
				}
				set
				{
					this.m_StoreImpulses = value;
				}
			}

			public float splitIslands
			{
				readonly get
				{
					return this.m_SplitIslands;
				}
				set
				{
					this.m_SplitIslands = value;
				}
			}

			public float bodyTransforms
			{
				readonly get
				{
					return this.m_BodyTransforms;
				}
				set
				{
					this.m_BodyTransforms = value;
				}
			}

			public float fastTriggers
			{
				readonly get
				{
					return this.m_FastTriggers;
				}
				set
				{
					this.m_FastTriggers = value;
				}
			}

			public float jointEvents
			{
				readonly get
				{
					return this.m_JointEvents;
				}
				set
				{
					this.m_JointEvents = value;
				}
			}

			public float hitEvents
			{
				readonly get
				{
					return this.m_HitEvents;
				}
				set
				{
					this.m_HitEvents = value;
				}
			}

			public float broadphaseUpdates
			{
				readonly get
				{
					return this.m_BroadphaseUpdates;
				}
				set
				{
					this.m_BroadphaseUpdates = value;
				}
			}

			public float solveContinuous
			{
				readonly get
				{
					return this.m_SolveContinuous;
				}
				set
				{
					this.m_SolveContinuous = value;
				}
			}

			public float sleepIslands
			{
				readonly get
				{
					return this.m_SleepIslands;
				}
				set
				{
					this.m_SleepIslands = value;
				}
			}

			public float updateTriggers
			{
				readonly get
				{
					return this.m_UpdateTriggers;
				}
				set
				{
					this.m_UpdateTriggers = value;
				}
			}

			public float writeTransforms
			{
				readonly get
				{
					return this.m_WriteTransforms;
				}
				set
				{
					this.m_WriteTransforms = value;
				}
			}

			public static PhysicsWorld.WorldProfile Add(PhysicsWorld.WorldProfile profileA, PhysicsWorld.WorldProfile profileB)
			{
				return new PhysicsWorld.WorldProfile
				{
					simulationStep = profileA.simulationStep + profileB.simulationStep,
					contactPairs = profileA.contactPairs + profileB.contactPairs,
					contactUpdates = profileA.contactUpdates + profileB.contactUpdates,
					solving = profileA.solving + profileB.solving,
					prepareStages = profileA.prepareStages + profileB.prepareStages,
					solveConstraints = profileA.solveConstraints + profileB.solveConstraints,
					prepareConstraints = profileA.prepareConstraints + profileB.prepareConstraints,
					integrateVelocities = profileA.integrateVelocities + profileB.integrateVelocities,
					warmStarting = profileA.warmStarting + profileB.warmStarting,
					solveImpulses = profileA.solveImpulses + profileB.solveImpulses,
					integrateTransforms = profileA.integrateTransforms + profileB.integrateTransforms,
					relaxImpulses = profileA.relaxImpulses + profileB.relaxImpulses,
					applyBounciness = profileA.applyBounciness + profileB.applyBounciness,
					storeImpulses = profileA.storeImpulses + profileB.storeImpulses,
					splitIslands = profileA.splitIslands + profileB.splitIslands,
					bodyTransforms = profileA.bodyTransforms + profileB.bodyTransforms,
					fastTriggers = profileA.fastTriggers + profileB.fastTriggers,
					jointEvents = profileA.jointEvents + profileB.jointEvents,
					hitEvents = profileA.hitEvents + profileB.hitEvents,
					broadphaseUpdates = profileA.broadphaseUpdates + profileB.broadphaseUpdates,
					solveContinuous = profileA.solveContinuous + profileB.solveContinuous,
					sleepIslands = profileA.sleepIslands + profileB.sleepIslands,
					updateTriggers = profileA.updateTriggers + profileB.updateTriggers,
					writeTransforms = profileA.writeTransforms + profileB.writeTransforms
				};
			}

			public static PhysicsWorld.WorldProfile Maximum(PhysicsWorld.WorldProfile profileA, PhysicsWorld.WorldProfile profileB)
			{
				return new PhysicsWorld.WorldProfile
				{
					simulationStep = Mathf.Max(profileA.simulationStep, profileB.simulationStep),
					contactPairs = Mathf.Max(profileA.contactPairs, profileB.contactPairs),
					contactUpdates = Mathf.Max(profileA.contactUpdates, profileB.contactUpdates),
					solving = Mathf.Max(profileA.solving, profileB.solving),
					prepareStages = Mathf.Max(profileA.prepareStages, profileB.prepareStages),
					solveConstraints = Mathf.Max(profileA.solveConstraints, profileB.solveConstraints),
					prepareConstraints = Mathf.Max(profileA.prepareConstraints, profileB.prepareConstraints),
					integrateVelocities = Mathf.Max(profileA.integrateVelocities, profileB.integrateVelocities),
					warmStarting = Mathf.Max(profileA.warmStarting, profileB.warmStarting),
					solveImpulses = Mathf.Max(profileA.solveImpulses, profileB.solveImpulses),
					integrateTransforms = Mathf.Max(profileA.integrateTransforms, profileB.integrateTransforms),
					relaxImpulses = Mathf.Max(profileA.relaxImpulses, profileB.relaxImpulses),
					applyBounciness = Mathf.Max(profileA.applyBounciness, profileB.applyBounciness),
					storeImpulses = Mathf.Max(profileA.storeImpulses, profileB.storeImpulses),
					splitIslands = Mathf.Max(profileA.splitIslands, profileB.splitIslands),
					bodyTransforms = Mathf.Max(profileA.bodyTransforms, profileB.bodyTransforms),
					fastTriggers = Mathf.Max(profileA.fastTriggers, profileB.fastTriggers),
					jointEvents = Mathf.Max(profileA.jointEvents, profileB.jointEvents),
					hitEvents = Mathf.Max(profileA.hitEvents, profileB.hitEvents),
					broadphaseUpdates = Mathf.Max(profileA.broadphaseUpdates, profileB.broadphaseUpdates),
					solveContinuous = Mathf.Max(profileA.solveContinuous, profileB.solveContinuous),
					sleepIslands = Mathf.Max(profileA.sleepIslands, profileB.sleepIslands),
					updateTriggers = Mathf.Max(profileA.updateTriggers, profileB.updateTriggers),
					writeTransforms = Mathf.Max(profileA.writeTransforms, profileB.writeTransforms)
				};
			}

			[SerializeField]
			private float m_SimulationStep;

			[SerializeField]
			private float m_ContactPairs;

			[SerializeField]
			private float m_ContactUpdates;

			[SerializeField]
			private float m_Solving;

			[SerializeField]
			private float m_PrepareStages;

			[SerializeField]
			private float m_SolveConstraints;

			[SerializeField]
			private float m_PrepareConstraints;

			[SerializeField]
			private float m_IntegrateVelocities;

			[SerializeField]
			private float m_WarmStarting;

			[SerializeField]
			private float m_SolveImpulses;

			[SerializeField]
			private float m_IntegrateTransforms;

			[SerializeField]
			private float m_RelaxImpulses;

			[SerializeField]
			private float m_ApplyBounciness;

			[SerializeField]
			private float m_StoreImpulses;

			[SerializeField]
			private float m_SplitIslands;

			[SerializeField]
			private float m_BodyTransforms;

			[SerializeField]
			private float m_FastTriggers;

			[SerializeField]
			private float m_JointEvents;

			[SerializeField]
			private float m_HitEvents;

			[SerializeField]
			private float m_BroadphaseUpdates;

			[SerializeField]
			private float m_SolveContinuous;

			[SerializeField]
			private float m_SleepIslands;

			[SerializeField]
			private float m_UpdateTriggers;

			[SerializeField]
			private float m_WriteTransforms;
		}

		[Flags]
		public enum DrawOptions
		{
			Off = 0,
			SelectedBodies = 1,
			SelectedShapes = 2,
			SelectedShapeBounds = 4,
			SelectedJoints = 8,
			AllBodies = 16,
			AllShapes = 32,
			AllShapeBounds = 64,
			AllJoints = 128,
			AllContactPoints = 256,
			AllContactNormal = 512,
			AllContactImpulse = 1024,
			AllContactFriction = 2048,
			AllCustom = 4096,
			AllSolverIslands = 8192,
			DefaultAll = 4256,
			DefaultSelected = 4106
		}

		[Flags]
		public enum DrawFillOptions
		{
			Interior = 1,
			Outline = 2,
			Orientation = 4,
			All = 7
		}

		internal readonly struct DrawResults
		{
			public override string ToString()
			{
				return string.Format("PolygonGeometry:{0}, CircleGeometry:{1}, CapsuleGeometry:{2}, Line:{3}, Point: {4}", new object[] { this.m_PolygonGeometryElements, this.m_CircleGeometryElements, this.m_CapsuleGeometryElements, this.m_LineElements, this.m_PointElements });
			}

			public NativeArray<PhysicsWorld.DrawResults.PolygonGeometryElement> polygonGeometryArray
			{
				get
				{
					return this.m_PolygonGeometryElements.ToNativeArray<PhysicsWorld.DrawResults.PolygonGeometryElement>();
				}
			}

			public NativeArray<PhysicsWorld.DrawResults.CircleGeometryElement> circleGeometryArray
			{
				get
				{
					return this.m_CircleGeometryElements.ToNativeArray<PhysicsWorld.DrawResults.CircleGeometryElement>();
				}
			}

			public NativeArray<PhysicsWorld.DrawResults.CapsuleGeometryElement> capsuleGeometryArray
			{
				get
				{
					return this.m_CapsuleGeometryElements.ToNativeArray<PhysicsWorld.DrawResults.CapsuleGeometryElement>();
				}
			}

			public NativeArray<PhysicsWorld.DrawResults.LineElement> lineArray
			{
				get
				{
					return this.m_LineElements.ToNativeArray<PhysicsWorld.DrawResults.LineElement>();
				}
			}

			public NativeArray<PhysicsWorld.DrawResults.PointElement> pointArray
			{
				get
				{
					return this.m_PointElements.ToNativeArray<PhysicsWorld.DrawResults.PointElement>();
				}
			}

			public Span<PhysicsWorld.DrawResults.PolygonGeometryElement> polygonGeometrySpan
			{
				get
				{
					return this.m_PolygonGeometryElements.ToSpan<PhysicsWorld.DrawResults.PolygonGeometryElement>();
				}
			}

			public Span<PhysicsWorld.DrawResults.CircleGeometryElement> circleGeometrySpan
			{
				get
				{
					return this.m_CircleGeometryElements.ToSpan<PhysicsWorld.DrawResults.CircleGeometryElement>();
				}
			}

			public Span<PhysicsWorld.DrawResults.CapsuleGeometryElement> capsuleGeometrySpan
			{
				get
				{
					return this.m_CapsuleGeometryElements.ToSpan<PhysicsWorld.DrawResults.CapsuleGeometryElement>();
				}
			}

			public Span<PhysicsWorld.DrawResults.LineElement> lineSpan
			{
				get
				{
					return this.m_LineElements.ToSpan<PhysicsWorld.DrawResults.LineElement>();
				}
			}

			public Span<PhysicsWorld.DrawResults.PointElement> pointSpan
			{
				get
				{
					return this.m_PointElements.ToSpan<PhysicsWorld.DrawResults.PointElement>();
				}
			}

			internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_PolygonGeometryElements;

			internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_CircleGeometryElements;

			internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_CapsuleGeometryElements;

			internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_LineElements;

			internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_PointElements;

			public readonly struct PolygonGeometryElement
			{
				public static int Size()
				{
					return 112;
				}

				public readonly PhysicsTransform transform;

				public readonly Vector2 p0;

				public readonly Vector2 p1;

				public readonly Vector2 p2;

				public readonly Vector2 p3;

				public readonly Vector2 p4;

				public readonly Vector2 p5;

				public readonly Vector2 p6;

				public readonly Vector2 p7;

				public readonly int count;

				public readonly float radius;

				public readonly float elementDepth;

				public readonly PhysicsWorld.DrawFillOptions drawFillOptions;

				public readonly Color color;
			}

			public readonly struct CircleGeometryElement
			{
				public static int Size()
				{
					return 44;
				}

				public readonly PhysicsTransform transform;

				public readonly float radius;

				public readonly float elementDepth;

				public readonly PhysicsWorld.DrawFillOptions drawFillOptions;

				public readonly Color color;
			}

			public readonly struct CapsuleGeometryElement
			{
				public static int Size()
				{
					return 48;
				}

				public readonly PhysicsTransform transform;

				public readonly float radius;

				public readonly float length;

				public readonly float elementDepth;

				public readonly PhysicsWorld.DrawFillOptions drawFillOptions;

				public readonly Color color;
			}

			public readonly struct LineElement
			{
				public static int Size()
				{
					return 40;
				}

				public readonly PhysicsTransform transform;

				public readonly float length;

				public readonly float elementDepth;

				public readonly Color color;
			}

			public readonly struct PointElement
			{
				public static int Size()
				{
					return 32;
				}

				public readonly Vector2 position;

				public readonly float radius;

				public readonly float elementDepth;

				public readonly Color color;
			}
		}

		[Serializable]
		public struct DrawColors
		{
			public Color transformAxisX;

			public Color transformAxisY;

			public Color bodyBad;

			public Color bodyDisabled;

			public Color bodyAwake;

			public Color bodyStatic;

			public Color bodyKinematic;

			public Color bodyTimeOfImpactEvent;

			public Color bodyFastCollisions;

			public Color bodyMovingFast;

			public Color bodySpeedCapped;

			public Color shapeTrigger;

			public Color shapeOther;

			public Color shapeBounds;

			public Color contactSpeculative;

			public Color contactAdded;

			public Color contactPersisted;

			public Color contactNormal;

			public Color contactImpulse;

			public Color contactFriction;

			public Color solverIsland;

			private readonly PhysicsWorld.DrawColors.ConstraintGraphArray m_ConstraintGraph;

			private struct ConstraintGraphArray
			{
				public unsafe ref Color this[int index]
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get
					{
						bool flag = index >= 0 && index < 24;
						if (flag)
						{
							fixed (Color* ptr = &this.graphConstraint0)
							{
								Color* ptr2 = ptr;
								return ref ptr2[index];
							}
						}
						throw new IndexOutOfRangeException(string.Format("{0} must be in the range [0, {1}]", index, 23));
					}
				}

				public Color graphConstraint0;

				public Color graphConstraint1;

				public Color graphConstraint2;

				public Color graphConstraint3;

				public Color graphConstraint4;

				public Color graphConstraint5;

				public Color graphConstraint6;

				public Color graphConstraint7;

				public Color graphConstraint8;

				public Color graphConstraint9;

				public Color graphConstraint10;

				public Color graphConstraint11;

				public Color graphConstraint12;

				public Color graphConstraint13;

				public Color graphConstraint14;

				public Color graphConstraint15;

				public Color graphConstraint16;

				public Color graphConstraint17;

				public Color graphConstraint18;

				public Color graphConstraint19;

				public Color graphConstraint20;

				public Color graphConstraint21;

				public Color graphConstraint22;

				public Color graphConstraint23;
			}
		}
	}
}
