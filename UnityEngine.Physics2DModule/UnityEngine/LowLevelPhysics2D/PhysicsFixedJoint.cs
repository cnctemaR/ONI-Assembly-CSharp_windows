using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsFixedJoint : IPhysicsJoint, IEquatable<PhysicsFixedJoint>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsJoint(PhysicsFixedJoint joint)
		{
			return joint.m_Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsFixedJoint(PhysicsJoint joint)
		{
			return new PhysicsFixedJoint(joint);
		}

		public PhysicsFixedJoint(PhysicsJoint physicsJoint)
		{
			bool flag = physicsJoint.jointType != PhysicsJoint.JointType.FixedJoint;
			if (flag)
			{
				throw new InvalidCastException(string.Format("The joint must be of type {0} but is of type {1}.", "FixedJoint", physicsJoint.jointType));
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

		public bool Equals(PhysicsFixedJoint other)
		{
			return this.m_Id.Equals(other);
		}

		public static bool operator ==(PhysicsFixedJoint lhs, PhysicsFixedJoint rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PhysicsFixedJoint lhs, PhysicsFixedJoint rhs)
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

		public static PhysicsFixedJoint Create(PhysicsWorld world, PhysicsFixedJointDefinition definition)
		{
			return PhysicsLowLevelScripting2D.FixedJoint_Create(world, definition);
		}

		public static void DestroyBatch(ReadOnlySpan<PhysicsJoint> joints)
		{
			PhysicsJoint.DestroyBatch(joints);
		}

		public float linearFrequency
		{
			get
			{
				return PhysicsLowLevelScripting2D.FixedJoint_GetLinearFrequency(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.FixedJoint_SetLinearFrequency(this, value);
			}
		}

		public float linearDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.FixedJoint_GetLinearDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.FixedJoint_SetLinearDamping(this, value);
			}
		}

		public float angularFrequency
		{
			get
			{
				return PhysicsLowLevelScripting2D.FixedJoint_GetAngularFrequency(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.FixedJoint_SetAngularFrequency(this, value);
			}
		}

		public float angularDamping
		{
			get
			{
				return PhysicsLowLevelScripting2D.FixedJoint_GetAngularDamping(this);
			}
			set
			{
				PhysicsLowLevelScripting2D.FixedJoint_SetAngularDamping(this, value);
			}
		}

		private readonly PhysicsJoint m_Id;
	}
}
