using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsDistanceJointDefinition
	{
		public PhysicsDistanceJointDefinition()
		{
			this = PhysicsDistanceJointDefinition.defaultDefinition;
		}

		public PhysicsDistanceJointDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.DistanceJoint_GetDefaultDefinition(useSettings);
		}

		public static PhysicsDistanceJointDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.DistanceJoint_GetDefaultDefinition(true);
			}
		}

		public PhysicsBody bodyA
		{
			readonly get
			{
				return this.m_BodyA;
			}
			set
			{
				this.m_BodyA = value;
			}
		}

		public PhysicsBody bodyB
		{
			readonly get
			{
				return this.m_BodyB;
			}
			set
			{
				this.m_BodyB = value;
			}
		}

		public PhysicsTransform localAnchorA
		{
			readonly get
			{
				return this.m_LocalAnchorA;
			}
			set
			{
				this.m_LocalAnchorA = value;
			}
		}

		public PhysicsTransform localAnchorB
		{
			readonly get
			{
				return this.m_LocalAnchorB;
			}
			set
			{
				this.m_LocalAnchorB = value;
			}
		}

		public float distance
		{
			readonly get
			{
				return this.m_Distance;
			}
			set
			{
				this.m_Distance = Math.Max(float.Epsilon, value);
			}
		}

		public bool enableSpring
		{
			readonly get
			{
				return this.m_EnableSpring;
			}
			set
			{
				this.m_EnableSpring = value;
			}
		}

		public float springFrequency
		{
			readonly get
			{
				return this.m_SpringFrequency;
			}
			set
			{
				this.m_SpringFrequency = Mathf.Max(0f, value);
			}
		}

		public float springDamping
		{
			readonly get
			{
				return this.m_SpringDamping;
			}
			set
			{
				this.m_SpringDamping = Mathf.Max(0f, value);
			}
		}

		public float springLowerForce
		{
			readonly get
			{
				return this.m_SpringLowerForce;
			}
			set
			{
				this.m_SpringLowerForce = value;
			}
		}

		public float springUpperForce
		{
			readonly get
			{
				return this.m_SpringUpperForce;
			}
			set
			{
				this.m_SpringUpperForce = value;
			}
		}

		public bool enableMotor
		{
			readonly get
			{
				return this.m_EnableMotor;
			}
			set
			{
				this.m_EnableMotor = value;
			}
		}

		public float motorSpeed
		{
			readonly get
			{
				return this.m_MotorSpeed;
			}
			set
			{
				this.m_MotorSpeed = value;
			}
		}

		public float maxMotorForce
		{
			readonly get
			{
				return this.m_MaxMotorForce;
			}
			set
			{
				this.m_MaxMotorForce = value;
			}
		}

		public bool enableLimit
		{
			readonly get
			{
				return this.m_EnableLimit;
			}
			set
			{
				this.m_EnableLimit = value;
			}
		}

		public float minDistanceLimit
		{
			readonly get
			{
				return this.m_MinDistanceLimit;
			}
			set
			{
				this.m_MinDistanceLimit = Mathf.Max(0f, value);
			}
		}

		public float maxDistanceLimit
		{
			readonly get
			{
				return this.m_MaxDistanceLimit;
			}
			set
			{
				this.m_MaxDistanceLimit = Mathf.Max(0f, value);
			}
		}

		public float forceThreshold
		{
			readonly get
			{
				return this.m_ForceThreshold;
			}
			set
			{
				this.m_ForceThreshold = Mathf.Max(0f, value);
			}
		}

		public float torqueThreshold
		{
			readonly get
			{
				return this.m_TorqueThreshold;
			}
			set
			{
				this.m_TorqueThreshold = Mathf.Max(0f, value);
			}
		}

		public float tuningFrequency
		{
			readonly get
			{
				return this.m_TuningFrequency;
			}
			set
			{
				this.m_TuningFrequency = Mathf.Clamp(value, 0f, 1000f);
			}
		}

		public float tuningDamping
		{
			readonly get
			{
				return this.m_TuningDamping;
			}
			set
			{
				this.m_TuningDamping = Mathf.Clamp(value, 0f, 10f);
			}
		}

		public float drawScale
		{
			readonly get
			{
				return this.m_DrawScale;
			}
			set
			{
				this.m_DrawScale = Mathf.Clamp(value, 0.001f, 10f);
			}
		}

		public bool collideConnected
		{
			readonly get
			{
				return this.m_CollideConnected;
			}
			set
			{
				this.m_CollideConnected = value;
			}
		}

		[SerializeField]
		private PhysicsBody m_BodyA;

		[SerializeField]
		private PhysicsBody m_BodyB;

		[SerializeField]
		private PhysicsTransform m_LocalAnchorA;

		[SerializeField]
		private PhysicsTransform m_LocalAnchorB;

		[Min(1E-45f)]
		[SerializeField]
		private float m_Distance;

		[SerializeField]
		private bool m_EnableSpring;

		[SerializeField]
		[Min(0f)]
		private float m_SpringFrequency;

		[Min(0f)]
		[SerializeField]
		private float m_SpringDamping;

		[SerializeField]
		private float m_SpringLowerForce;

		[SerializeField]
		private float m_SpringUpperForce;

		[SerializeField]
		private bool m_EnableMotor;

		[SerializeField]
		private float m_MotorSpeed;

		[SerializeField]
		private float m_MaxMotorForce;

		[SerializeField]
		private bool m_EnableLimit;

		[SerializeField]
		[Min(0f)]
		private float m_MinDistanceLimit;

		[Min(0f)]
		[SerializeField]
		private float m_MaxDistanceLimit;

		[Min(0f)]
		[SerializeField]
		private float m_ForceThreshold;

		[Min(0f)]
		[SerializeField]
		private float m_TorqueThreshold;

		[SerializeField]
		[Range(0f, 1000f)]
		private float m_TuningFrequency;

		[Range(0f, 10f)]
		[SerializeField]
		private float m_TuningDamping;

		[Range(0.001f, 10f)]
		[SerializeField]
		private float m_DrawScale;

		[SerializeField]
		private bool m_CollideConnected;
	}
}
