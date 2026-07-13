using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsRelativeJoint : IPhysicsJoint, IEquatable<PhysicsRelativeJoint>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsJoint(PhysicsRelativeJoint joint)
		{
			return joint.m_Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsRelativeJoint(PhysicsJoint joint)
		{
			return new PhysicsRelativeJoint(joint);
		}

		public PhysicsRelativeJoint(PhysicsJoint physicsJoint)
		{
			bool flag = physicsJoint.jointType != PhysicsJoint.JointType.RelativeJoint;
			if (flag)
			{
				throw new InvalidCastException(string.Format("The joint must be of type {0} but is of type {1}.", "RelativeJoint", physicsJoint.jointType));
			}
			this.m_Id = physicsJoint;
		}

		public override string ToString()
		{
			return this.m_Id.ToString();
		}

		public override bool Equals(object obj)
		{
			return this.m_Id.Equals(obj);
		}

		public bool Equals(PhysicsRelativeJoint other)
		{
			return this.m_Id.Equals(other);
		}

		public static bool operator ==(PhysicsRelativeJoint lhs, PhysicsRelativeJoint rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsRelativeJoint lhs, PhysicsRelativeJoint rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return this.m_Id.GetHashCode();
		}

		public bool Destroy(int ownerKey = 0)
		{
			return this.m_Id.Destroy(ownerKey);
		}

		public bool isValid
		{
			get
			{
				return this.m_Id.isValid;
			}
		}

		public PhysicsWorld world
		{
			get
			{
				return this.m_Id.world;
			}
		}

		public PhysicsJoint.JointType jointType
		{
			get
			{
				return this.m_Id.jointType;
			}
		}

		public PhysicsBody bodyA
		{
			get
			{
				return this.m_Id.bodyA;
			}
		}

		public PhysicsBody bodyB
		{
			get
			{
				return this.m_Id.bodyB;
			}
		}

		public PhysicsTransform localAnchorA
		{
			get
			{
				return this.m_Id.localAnchorA;
			}
			set
			{
				this.m_Id.localAnchorA = value;
			}
		}

		public PhysicsTransform localAnchorB
		{
			get
			{
				return this.m_Id.localAnchorB;
			}
			set
			{
				this.m_Id.localAnchorB = value;
			}
		}

		public float forceThreshold
		{
			get
			{
				return this.m_Id.forceThreshold;
			}
			set
			{
				this.m_Id.forceThreshold = value;
			}
		}

		public float torqueThreshold
		{
			get
			{
				return this.m_Id.torqueThreshold;
			}
			set
			{
				this.m_Id.torqueThreshold = value;
			}
		}

		public bool collideConnected
		{
			get
			{
				return this.m_Id.collideConnected;
			}
			set
			{
				this.m_Id.collideConnected = value;
			}
		}

		public float tuningFrequency
		{
			get
			{
				return this.m_Id.tuningFrequency;
			}
			set
			{
				this.m_Id.tuningFrequency = value;
			}
		}

		public float tuningDamping
		{
			get
			{
				return this.m_Id.tuningDamping;
			}
			set
			{
				this.m_Id.tuningDamping = value;
			}
		}

		public float drawScale
		{
			get
			{
				return this.m_Id.drawScale;
			}
			set
			{
				this.m_Id.drawScale = value;
			}
		}

		public void WakeBodies()
		{
			this.m_Id.WakeBodies();
		}

		public Vector2 currentConstraintForce
		{
			get
			{
				return this.m_Id.currentConstraintForce;
			}
		}

		public float currentConstraintTorque
		{
			get
			{
				return this.m_Id.currentConstraintTorque;
			}
		}

		public float currentLinearSeparationError
		{
			get
			{
				return this.m_Id.currentLinearSeparationError;
			}
		}

		public float currentAngularSeparationError
		{
			get
			{
				return this.m_Id.currentAngularSeparationError;
			}
		}

		public int SetOwner(Object owner)
		{
			return this.m_Id.SetOwner(owner);
		}

		public Object GetOwner()
		{
			return this.m_Id.GetOwner();
		}

		public bool isOwned
		{
			get
			{
				return this.m_Id.isOwned;
			}
		}

		public object callbackTarget
		{
			get
			{
				return this.m_Id.callbackTarget;
			}
			set
			{
				this.m_Id.callbackTarget = value;
			}
		}

		public PhysicsUserData userData
		{
			get
			{
				return this.m_Id.userData;
			}
			set
			{
				this.m_Id.userData = value;
			}
		}

		public void Draw()
		{
			this.m_Id.Draw();
		}

		public static PhysicsRelativeJoint Create(PhysicsWorld world, PhysicsRelativeJointDefinition definition)
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_Create(world, definition);
		}

		public static void DestroyBatch(ReadOnlySpan<PhysicsJoint> joints)
		{
			PhysicsJoint.DestroyBatch(joints);
		}

		public Vector2 linearVelocity
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetLinearVelocity(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetLinearVelocity(this, value);
			}
		}

		public float angularVelocity
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetAngularVelocity(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetAngularVelocity(this, value);
			}
		}

		public float maxForce
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetMaxForce(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetMaxForce(this, value);
			}
		}

		public float maxTorque
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetMaxTorque(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetMaxTorque(this, value);
			}
		}

		public float springLinearFrequency
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringLinearFrequency(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetSpringLinearFrequency(this, value);
			}
		}

		public float springAngularFrequency
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringAngularFrequency(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetSpringAngularFrequency(this, value);
			}
		}

		public float springLinearDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringLinearDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetSpringLinearDamping(this, value);
			}
		}

		public float springAngularDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringAngularDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetSpringAngularDamping(this, value);
			}
		}

		public float springMaxForce
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringMaxForce(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetSpringMaxForce(this, value);
			}
		}

		public float springMaxTorque
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringMaxTorque(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.RelativeJoint_SetSpringMaxTorque(this, value);
			}
		}

		private readonly PhysicsJoint m_Id;
	}
}
