using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsWheelJointDefinition
	{
		public PhysicsWheelJointDefinition()
		{
			this = PhysicsWheelJointDefinition.defaultDefinition;
		}

		public PhysicsWheelJointDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.WheelJoint_GetDefaultDefinition(useSettings);
		}

		public static PhysicsWheelJointDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.WheelJoint_GetDefaultDefinition(true);
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

		[Min(0f)]
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

		[Min(0f)]
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

		[Min(0f)]
		public float maxMotorTorque
		{
			readonly get
			{
				return this.m_MaxMotorTorque;
			}
			set
			{
				this.m_MaxMotorTorque = Mathf.Max(0f, value);
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

		public float lowerTranslationLimit
		{
			readonly get
			{
				return this.m_LowerTranslationLimit;
			}
			set
			{
				this.m_LowerTranslationLimit = value;
			}
		}

		public float upperTranslationLimit
		{
			readonly get
			{
				return this.m_UpperTranslationLimit;
			}
			set
			{
				this.m_UpperTranslationLimit = value;
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

		[SerializeField]
		private bool m_EnableSpring;

		[Min(0f)]
		[SerializeField]
		private float m_SpringFrequency;

		[SerializeField]
		[Min(0f)]
		private float m_SpringDamping;

		[SerializeField]
		private bool m_EnableMotor;

		[SerializeField]
		private float m_MotorSpeed;

		[SerializeField]
		[Min(0f)]
		private float m_MaxMotorTorque;

		[SerializeField]
		private bool m_EnableLimit;

		[SerializeField]
		private float m_LowerTranslationLimit;

		[SerializeField]
		private float m_UpperTranslationLimit;

		[Min(0f)]
		[SerializeField]
		private float m_ForceThreshold;

		[SerializeField]
		[Min(0f)]
		private float m_TorqueThreshold;

		[Range(0f, 1000f)]
		[SerializeField]
		private float m_TuningFrequency;

		[SerializeField]
		[Range(0f, 10f)]
		private float m_TuningDamping;

		[SerializeField]
		[Range(0.001f, 10f)]
		private float m_DrawScale;

		[SerializeField]
		private bool m_CollideConnected;
	}
}
