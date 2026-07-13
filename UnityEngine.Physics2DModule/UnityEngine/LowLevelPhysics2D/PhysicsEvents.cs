using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.LowLevelPhysics2D
{
	[RequiredByNativeCode(GenerateProxy = true)]
	public readonly struct PhysicsEvents
	{
		public static event PhysicsEvents.PreSimulateEventHandler PreSimulate
		{
			add
			{
				PhysicsEvents.s_PreSimulate += value;
			}
			remove
			{
				PhysicsEvents.s_PreSimulate -= value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event PhysicsEvents.PreSimulateEventHandler s_PreSimulate;

		public static event PhysicsEvents.PreSimulateEventHandler PostSimulate
		{
			add
			{
				PhysicsEvents.s_PostSimulate += value;
			}
			remove
			{
				PhysicsEvents.s_PostSimulate -= value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event PhysicsEvents.PreSimulateEventHandler s_PostSimulate;

		[RequiredByNativeCode]
		private static void InvokePreSimulate(PhysicsWorld world, float deltaTime)
		{
			try
			{
				PhysicsEvents.PreSimulateEventHandler preSimulateEventHandler = PhysicsEvents.s_PreSimulate;
				if (preSimulateEventHandler != null)
				{
					preSimulateEventHandler(world, deltaTime);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
			finally
			{
			}
		}

		[RequiredByNativeCode]
		private static void InvokePostSimulate(PhysicsWorld world, float deltaTime)
		{
			try
			{
				PhysicsEvents.PreSimulateEventHandler preSimulateEventHandler = PhysicsEvents.s_PostSimulate;
				if (preSimulateEventHandler != null)
				{
					preSimulateEventHandler(world, deltaTime);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
			finally
			{
			}
		}

		[RequiredByNativeCode]
		private static bool SendContactFilterCallback(object callbackTarget, PhysicsEvents.ContactFilterEvent contactFilterEvent)
		{
			PhysicsCallbacks.IContactFilterCallback contactFilterCallback = callbackTarget as PhysicsCallbacks.IContactFilterCallback;
			bool flag = contactFilterCallback != null;
			return !flag || contactFilterCallback.OnContactFilter2D(contactFilterEvent);
		}

		[RequiredByNativeCode]
		private static bool SendPreSolveCallback(object callbackTarget, PhysicsEvents.PreSolveEvent preSolveEvent)
		{
			PhysicsCallbacks.IPreSolveCallback preSolveCallback = callbackTarget as PhysicsCallbacks.IPreSolveCallback;
			bool flag = preSolveCallback != null;
			return !flag || preSolveCallback.OnPreSolve2D(preSolveEvent);
		}

		[RequiredByNativeCode]
		private static void SendBodyUpdateCallbacks(PhysicsWorld world)
		{
			world.SendBodyUpdateCallbacks();
		}

		[RequiredByNativeCode]
		private static void SendContactCallbacks(PhysicsWorld world)
		{
			world.SendContactCallbacks();
		}

		[RequiredByNativeCode]
		private static void SendTriggerCallbacks(PhysicsWorld world)
		{
			world.SendTriggerCallbacks();
		}

		[RequiredByNativeCode]
		private static void SendJointThresholdCallbacks(PhysicsWorld world)
		{
			world.SendJointThresholdCallbacks();
		}

		public readonly struct BodyUpdateEvent
		{
			public PhysicsTransform transform
			{
				get
				{
					return this.m_Transform;
				}
			}

			public PhysicsBody body
			{
				get
				{
					return this.m_Body;
				}
			}

			public bool fellAsleep
			{
				get
				{
					return this.m_FellAsleep;
				}
			}

			public override string ToString()
			{
				return string.Format("BodyEvent: transform={0}, body={1}, fellAsleep={2}", this.transform, this.body, this.fellAsleep);
			}

			private readonly IntPtr m_UserData;

			private readonly PhysicsTransform m_Transform;

			private readonly PhysicsBody m_Body;

			private readonly bool m_FellAsleep;
		}

		public readonly struct TriggerBeginEvent
		{
			public PhysicsShape triggerShape
			{
				get
				{
					return this.m_TriggerShape;
				}
			}

			public PhysicsShape visitorShape
			{
				get
				{
					return this.m_VisitorShape;
				}
			}

			public override string ToString()
			{
				return string.Format("TriggerBeginEvent: triggerShape={0}, visitorShape={1}", this.triggerShape, this.visitorShape);
			}

			private readonly PhysicsShape m_TriggerShape;

			private readonly PhysicsShape m_VisitorShape;
		}

		public readonly struct TriggerEndEvent
		{
			public PhysicsShape triggerShape
			{
				get
				{
					return this.m_TriggerShape;
				}
			}

			public PhysicsShape visitorShape
			{
				get
				{
					return this.m_VisitorShape;
				}
			}

			public override string ToString()
			{
				return string.Format("TriggerEndEvent: triggerShape={0}, visitorShape={1}", this.triggerShape, this.visitorShape);
			}

			private readonly PhysicsShape m_TriggerShape;

			private readonly PhysicsShape m_VisitorShape;
		}

		public readonly struct ContactBeginEvent
		{
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

			public PhysicsShape.ContactId contactId
			{
				get
				{
					return this.m_ContactId;
				}
			}

			public override string ToString()
			{
				return string.Format("ContactBeginEvent: shapeA={0}, shapeB={1}, Id={2}", this.shapeA, this.shapeB, this.contactId);
			}

			private readonly PhysicsShape m_ShapeA;

			private readonly PhysicsShape m_ShapeB;

			private readonly PhysicsShape.ContactId m_ContactId;
		}

		public readonly struct ContactEndEvent
		{
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

			public PhysicsShape.ContactId contactId
			{
				get
				{
					return this.m_ContactId;
				}
			}

			public override string ToString()
			{
				return string.Format("ContactEndEvent: shapeA={0}, shapeB={1}, Id={2}", this.shapeA, this.shapeB, this.contactId);
			}

			private readonly PhysicsShape m_ShapeA;

			private readonly PhysicsShape m_ShapeB;

			private readonly PhysicsShape.ContactId m_ContactId;
		}

		public readonly struct ContactHitEvent
		{
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

			public PhysicsShape.ContactId contactId
			{
				get
				{
					return this.m_ContactId;
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

			public float approachSpeed
			{
				get
				{
					return this.m_ApproachSpeed;
				}
			}

			public override string ToString()
			{
				return string.Format("ContactHitEvent: shapeA={0}, shapeB={1}, point={2}, approachSpeed={3}", new object[] { this.shapeA, this.shapeB, this.point, this.approachSpeed });
			}

			private readonly PhysicsShape m_ShapeA;

			private readonly PhysicsShape m_ShapeB;

			private readonly PhysicsShape.ContactId m_ContactId;

			private readonly Vector2 m_Point;

			private readonly Vector2 m_Normal;

			private readonly float m_ApproachSpeed;
		}

		public readonly struct ContactFilterEvent
		{
			public PhysicsWorld physicsWorld
			{
				get
				{
					return this.m_PhysicsWorld;
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

			public override string ToString()
			{
				return string.Format("ContactFilterEvent: physicwWorld={0}, shapeA={1}, shapeB={2}", this.physicsWorld, this.shapeA, this.shapeB);
			}

			private readonly PhysicsWorld m_PhysicsWorld;

			private readonly PhysicsShape m_ShapeA;

			private readonly PhysicsShape m_ShapeB;
		}

		public readonly struct PreSolveEvent
		{
			public PhysicsWorld physicsWorld
			{
				get
				{
					return this.m_PhysicsWorld;
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

			public override string ToString()
			{
				return string.Format("PreSolveEvent: physicwWorld={0}, shapeA={1}, shapeB={2}, point={3}, normal={4}", new object[] { this.physicsWorld, this.shapeA, this.shapeB, this.point, this.normal });
			}

			private readonly PhysicsWorld m_PhysicsWorld;

			private readonly PhysicsShape m_ShapeA;

			private readonly PhysicsShape m_ShapeB;

			private readonly Vector2 m_Point;

			private readonly Vector2 m_Normal;
		}

		public readonly struct JointThresholdEvent
		{
			public PhysicsJoint joint
			{
				get
				{
					return this.m_Joint;
				}
			}

			public override string ToString()
			{
				return string.Format("JointEvent: joint={0}", this.joint);
			}

			private readonly PhysicsJoint m_Joint;

			private readonly IntPtr m_UserData;
		}

		public delegate void PreSimulateEventHandler(PhysicsWorld world, float deltaTime);

		public delegate void PostSimulateEventHandler(PhysicsWorld world, float deltaTime);
	}
}
