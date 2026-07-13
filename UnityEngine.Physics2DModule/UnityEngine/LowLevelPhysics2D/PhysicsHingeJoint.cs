using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsHingeJoint : IPhysicsJoint, IEquatable<PhysicsHingeJoint>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsJoint(PhysicsHingeJoint joint)
		{
			return joint.m_Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsHingeJoint(PhysicsJoint joint)
		{
			return new PhysicsHingeJoint(joint);
		}

		public PhysicsHingeJoint(PhysicsJoint physicsJoint)
		{
			bool flag = physicsJoint.jointType != PhysicsJoint.JointType.HingeJoint;
			if (flag)
			{
				throw new InvalidCastException(string.Format("The joint must be of type {0} but is of type {1}.", "HingeJoint", physicsJoint.jointType));
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

		public bool Equals(PhysicsHingeJoint other)
		{
			return this.m_Id.Equals(other);
		}

		public static bool operator ==(PhysicsHingeJoint lhs, PhysicsHingeJoint rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsHingeJoint lhs, PhysicsHingeJoint rhs)
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

		public static PhysicsHingeJoint Create(PhysicsWorld world, PhysicsHingeJointDefinition definition)
		{
			return PhysicsLowLevelScripting2D.HingeJoint_Create(world, definition);
		}

		public static void DestroyBatch(ReadOnlySpan<PhysicsJoint> joints)
		{
			PhysicsJoint.DestroyBatch(joints);
		}

		public bool enableSpring
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetEnableSpring(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetEnableSpring(this, value);
			}
		}

		public float springFrequency
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetSpringFrequency(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetSpringFrequency(this, value);
			}
		}

		public float springDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetSpringDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetSpringDamping(this, value);
			}
		}

		public float springTargetAngle
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetSpringTargetAngle(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetSpringTargetAngle(this, value);
			}
		}

		public float angle
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetAngle(this);
			}
		}

		public bool enableMotor
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetEnableMotor(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetEnableMotor(this, value);
			}
		}

		public float motorSpeed
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetMotorSpeed(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetMotorSpeed(this, value);
			}
		}

		public float maxMotorTorque
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetMaxMotorTorque(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetMaxMotorTorque(this, value);
			}
		}

		public float currentMotorTorque
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetCurrentMotorTorque(this);
			}
		}

		public bool enableLimit
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetEnableLimit(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetEnableLimit(this, value);
			}
		}

		public float lowerAngleLimit
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetLowerLimit(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetLowerLimit(this, value);
			}
		}

		public float upperAngleLimit
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetUpperLimit(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.HingeJoint_SetUpperLimit(this, value);
			}
		}

		private readonly PhysicsJoint m_Id;
	}
}
