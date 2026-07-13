using System;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsCallbacks
	{
		public interface IBodyUpdateCallback
		{
			void OnBodyUpdate2D(PhysicsEvents.BodyUpdateEvent bodyUpdateEvent);
		}

		public interface IContactFilterCallback
		{
			bool OnContactFilter2D(PhysicsEvents.ContactFilterEvent contactFilterEvent);
		}

		public interface IPreSolveCallback
		{
			bool OnPreSolve2D(PhysicsEvents.PreSolveEvent preSolveEvent);
		}

		public interface ITriggerCallback
		{
			void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent);

			void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent);
		}

		public interface IContactCallback
		{
			void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent);

			void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent);
		}

		public interface IJointThresholdCallback
		{
			void OnJointThreshold2D(PhysicsEvents.JointThresholdEvent thresholdEvent);
		}

		public readonly struct BodyUpdateCallbackTargets : IDisposable
		{
			public ReadOnlySpan<PhysicsCallbacks.BodyUpdateCallbackTargets.BodyUpdateTarget> bodyUpdateCallbackTargets
			{
				get
				{
					return this.m_BodyUpdateCallbackTargets.ToReadOnlySpan<PhysicsCallbacks.BodyUpdateCallbackTargets.BodyUpdateTarget>();
				}
			}

			public void Dispose()
			{
				this.m_BodyUpdateCallbackTargets.Dispose();
			}

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_BodyUpdateCallbackTargets;

			public readonly struct BodyUpdateTarget
			{
				public PhysicsEvents.BodyUpdateEvent bodyUpdateEvent
				{
					get
					{
						return this.m_BodyUpdateEvent;
					}
				}

				public PhysicsCallbacks.IBodyUpdateCallback bodyTarget
				{
					get
					{
						bool isValid = this.m_BodyUpdateEvent.body.isValid;
						PhysicsCallbacks.IBodyUpdateCallback bodyUpdateCallback;
						if (isValid)
						{
							bodyUpdateCallback = this.m_BodyUpdateEvent.body.callbackTarget as PhysicsCallbacks.IBodyUpdateCallback;
						}
						else
						{
							bodyUpdateCallback = null;
						}
						return bodyUpdateCallback;
					}
				}

				private readonly PhysicsEvents.BodyUpdateEvent m_BodyUpdateEvent;
			}
		}

		public readonly struct TriggerCallbackTargets : IDisposable
		{
			public ReadOnlySpan<PhysicsCallbacks.TriggerCallbackTargets.TriggerBeginTarget> BeginCallbackTargets
			{
				get
				{
					return this.m_BeginCallbackTargets.ToReadOnlySpan<PhysicsCallbacks.TriggerCallbackTargets.TriggerBeginTarget>();
				}
			}

			public ReadOnlySpan<PhysicsCallbacks.TriggerCallbackTargets.TriggerEndTarget> EndCallbackTargets
			{
				get
				{
					return this.m_EndCallbackTargets.ToReadOnlySpan<PhysicsCallbacks.TriggerCallbackTargets.TriggerEndTarget>();
				}
			}

			public void Dispose()
			{
				this.m_BeginCallbackTargets.Dispose();
				this.m_EndCallbackTargets.Dispose();
			}

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_BeginCallbackTargets;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_EndCallbackTargets;

			public readonly struct TriggerBeginTarget
			{
				public PhysicsEvents.TriggerBeginEvent beginEvent
				{
					get
					{
						return this.m_BeginEvent;
					}
				}

				public PhysicsCallbacks.ITriggerCallback triggerShapeTarget
				{
					get
					{
						bool isValid = this.m_BeginEvent.triggerShape.isValid;
						PhysicsCallbacks.ITriggerCallback triggerCallback;
						if (isValid)
						{
							triggerCallback = this.m_BeginEvent.triggerShape.callbackTarget as PhysicsCallbacks.ITriggerCallback;
						}
						else
						{
							triggerCallback = null;
						}
						return triggerCallback;
					}
				}

				public PhysicsCallbacks.ITriggerCallback visitorShapeTarget
				{
					get
					{
						bool isValid = this.m_BeginEvent.visitorShape.isValid;
						PhysicsCallbacks.ITriggerCallback triggerCallback;
						if (isValid)
						{
							triggerCallback = this.m_BeginEvent.visitorShape.callbackTarget as PhysicsCallbacks.ITriggerCallback;
						}
						else
						{
							triggerCallback = null;
						}
						return triggerCallback;
					}
				}

				private readonly PhysicsEvents.TriggerBeginEvent m_BeginEvent;
			}

			public readonly struct TriggerEndTarget
			{
				public PhysicsEvents.TriggerEndEvent endEvent
				{
					get
					{
						return this.m_EndEvent;
					}
				}

				public PhysicsCallbacks.ITriggerCallback triggerShapeTarget
				{
					get
					{
						bool isValid = this.m_EndEvent.triggerShape.isValid;
						PhysicsCallbacks.ITriggerCallback triggerCallback;
						if (isValid)
						{
							triggerCallback = this.m_EndEvent.triggerShape.callbackTarget as PhysicsCallbacks.ITriggerCallback;
						}
						else
						{
							triggerCallback = null;
						}
						return triggerCallback;
					}
				}

				public PhysicsCallbacks.ITriggerCallback visitorShapeTarget
				{
					get
					{
						bool isValid = this.m_EndEvent.visitorShape.isValid;
						PhysicsCallbacks.ITriggerCallback triggerCallback;
						if (isValid)
						{
							triggerCallback = this.m_EndEvent.visitorShape.callbackTarget as PhysicsCallbacks.ITriggerCallback;
						}
						else
						{
							triggerCallback = null;
						}
						return triggerCallback;
					}
				}

				private readonly PhysicsEvents.TriggerEndEvent m_EndEvent;
			}
		}

		public readonly struct ContactCallbackTargets : IDisposable
		{
			public ReadOnlySpan<PhysicsCallbacks.ContactCallbackTargets.ContactBeginTarget> BeginCallbackTargets
			{
				get
				{
					return this.m_BeginCallbackTargets.ToReadOnlySpan<PhysicsCallbacks.ContactCallbackTargets.ContactBeginTarget>();
				}
			}

			public ReadOnlySpan<PhysicsCallbacks.ContactCallbackTargets.ContactEndTarget> EndCallbackTargets
			{
				get
				{
					return this.m_EndCallbackTargets.ToReadOnlySpan<PhysicsCallbacks.ContactCallbackTargets.ContactEndTarget>();
				}
			}

			public void Dispose()
			{
				this.m_BeginCallbackTargets.Dispose();
				this.m_EndCallbackTargets.Dispose();
			}

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_BeginCallbackTargets;

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_EndCallbackTargets;

			public readonly struct ContactBeginTarget
			{
				public PhysicsEvents.ContactBeginEvent beginEvent
				{
					get
					{
						return this.m_BeginEvent;
					}
				}

				public PhysicsCallbacks.IContactCallback shapeTargetA
				{
					get
					{
						bool isValid = this.m_BeginEvent.shapeA.isValid;
						PhysicsCallbacks.IContactCallback contactCallback;
						if (isValid)
						{
							contactCallback = this.m_BeginEvent.shapeA.callbackTarget as PhysicsCallbacks.IContactCallback;
						}
						else
						{
							contactCallback = null;
						}
						return contactCallback;
					}
				}

				public PhysicsCallbacks.IContactCallback shapeTargetB
				{
					get
					{
						bool isValid = this.m_BeginEvent.shapeB.isValid;
						PhysicsCallbacks.IContactCallback contactCallback;
						if (isValid)
						{
							contactCallback = this.m_BeginEvent.shapeB.callbackTarget as PhysicsCallbacks.IContactCallback;
						}
						else
						{
							contactCallback = null;
						}
						return contactCallback;
					}
				}

				private readonly PhysicsEvents.ContactBeginEvent m_BeginEvent;
			}

			public readonly struct ContactEndTarget
			{
				public PhysicsEvents.ContactEndEvent endEvent
				{
					get
					{
						return this.m_EndEvent;
					}
				}

				public PhysicsCallbacks.IContactCallback shapeTargetA
				{
					get
					{
						bool isValid = this.m_EndEvent.shapeA.isValid;
						PhysicsCallbacks.IContactCallback contactCallback;
						if (isValid)
						{
							contactCallback = this.m_EndEvent.shapeA.callbackTarget as PhysicsCallbacks.IContactCallback;
						}
						else
						{
							contactCallback = null;
						}
						return contactCallback;
					}
				}

				public PhysicsCallbacks.IContactCallback shapeTargetB
				{
					get
					{
						bool isValid = this.m_EndEvent.shapeB.isValid;
						PhysicsCallbacks.IContactCallback contactCallback;
						if (isValid)
						{
							contactCallback = this.m_EndEvent.shapeB.callbackTarget as PhysicsCallbacks.IContactCallback;
						}
						else
						{
							contactCallback = null;
						}
						return contactCallback;
					}
				}

				private readonly PhysicsEvents.ContactEndEvent m_EndEvent;
			}
		}

		public readonly struct JointThresholdCallbackTargets : IDisposable
		{
			public ReadOnlySpan<PhysicsCallbacks.JointThresholdCallbackTargets.JointThresholdTarget> jointThresholdCallbackTargets
			{
				get
				{
					return this.m_JointThresholdCallbackTargets.ToReadOnlySpan<PhysicsCallbacks.JointThresholdCallbackTargets.JointThresholdTarget>();
				}
			}

			public void Dispose()
			{
				this.m_JointThresholdCallbackTargets.Dispose();
			}

			private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_JointThresholdCallbackTargets;

			public readonly struct JointThresholdTarget
			{
				public PhysicsEvents.JointThresholdEvent jointThresholdEvent
				{
					get
					{
						return this.m_JointThresholdEvent;
					}
				}

				public PhysicsCallbacks.IJointThresholdCallback jointTarget
				{
					get
					{
						bool isValid = this.m_JointThresholdEvent.joint.isValid;
						PhysicsCallbacks.IJointThresholdCallback jointThresholdCallback;
						if (isValid)
						{
							jointThresholdCallback = this.m_JointThresholdEvent.joint.callbackTarget as PhysicsCallbacks.IJointThresholdCallback;
						}
						else
						{
							jointThresholdCallback = null;
						}
						return jointThresholdCallback;
					}
				}

				private readonly PhysicsEvents.JointThresholdEvent m_JointThresholdEvent;
			}
		}
	}
}
