using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsDistanceJoint : IPhysicsJoint, IEquatable<PhysicsDistanceJoint>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsJoint(PhysicsDistanceJoint joint)
		{
			return joint.m_Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsDistanceJoint(PhysicsJoint joint)
		{
			return new PhysicsDistanceJoint(joint);
		}

		private PhysicsDistanceJoint(PhysicsJoint physicsJoint)
		{
			bool flag = physicsJoint.jointType > PhysicsJoint.JointType.DistanceJoint;
			if (flag)
			{
				throw new InvalidCastException(string.Format("The joint must be of type {0} but is of type {1}.", "DistanceJoint", physicsJoint.jointType));
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

		public bool Equals(PhysicsDistanceJoint other)
		{
			return this.m_Id.Equals(other);
		}

		public static bool operator ==(PhysicsDistanceJoint lhs, PhysicsDistanceJoint rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsDistanceJoint lhs, PhysicsDistanceJoint rhs)
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

		public static PhysicsDistanceJoint Create(PhysicsWorld world, PhysicsDistanceJointDefinition definition)
		{
			return PhysicsLowLevelScripting2D.DistanceJoint_Create(world, definition);
		}

		public static void DestroyBatch(ReadOnlySpan<PhysicsJoint> joints)
		{
			PhysicsJoint.DestroyBatch(joints);
		}

		public float distance
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetDistance(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetDistance(this, value);
			}
		}

		public float currentDistance
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetCurrentDistance(this);
			}
		}

		public bool enableSpring
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetEnableSpring(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetEnableSpring(this, value);
			}
		}

		public float springFrequency
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetSpringFrequency(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetSpringFrequency(this, value);
			}
		}

		public float springDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetSpringDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetSpringDamping(this, value);
			}
		}

		public float springLowerForce
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetSpringLowerForce(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetSpringLowerForce(this, value);
			}
		}

		public float springUpperForce
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetSpringUpperForce(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetSpringUpperForce(this, value);
			}
		}

		public bool enableMotor
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetEnableMotor(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetEnableMotor(this, value);
			}
		}

		public float motorSpeed
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetMotorSpeed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetMotorSpeed(this, value);
			}
		}

		public float maxMotorForce
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetMaxMotorForce(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetMaxMotorForce(this, value);
			}
		}

		public float currentMotorForce
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetCurrentMotorForce(this);
			}
		}

		public bool enableLimit
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetEnableLimit(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetEnableLimit(this, value);
			}
		}

		public float minDistanceLimit
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetMinDistanceLimit(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetMinDistanceLimit(this, value);
			}
		}

		public float maxDistanceLimit
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetMaxDistanceLimit(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.DistanceJoint_SetMaxDistanceLimit(this, value);
			}
		}

		private readonly PhysicsJoint m_Id;
	}
}
