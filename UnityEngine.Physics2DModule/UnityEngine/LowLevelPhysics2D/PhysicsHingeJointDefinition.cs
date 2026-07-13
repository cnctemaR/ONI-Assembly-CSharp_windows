using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsHingeJointDefinition
	{
		public PhysicsHingeJointDefinition()
		{
			this = PhysicsHingeJointDefinition.defaultDefinition;
		}

		public PhysicsHingeJointDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.HingeJoint_GetDefaultDefinition(useSettings);
		}

		public static PhysicsHingeJointDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.HingeJoint_GetDefaultDefinition(true);
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

		public float springTargetAngle
		{
			readonly get
			{
				return this.m_SpringTargetAngle;
			}
			set
			{
				this.m_SpringTargetAngle = value;
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

		public float lowerAngleLimit
		{
			readonly get
			{
				return this.m_LowerAngleLimit;
			}
			set
			{
				this.m_LowerAngleLimit = Mathf.Clamp(value, -178f, 178f);
			}
		}

		public float upperAngleLimit
		{
			readonly get
			{
				return this.m_UpperAngleLimit;
			}
			set
			{
				this.m_UpperAngleLimit = Mathf.Clamp(value, -178f, 178f);
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

		[SerializeField]
		private float m_SpringTargetAngle;

		[SerializeField]
		[Min(0f)]
		private float m_SpringFrequency;

		[SerializeField]
		[Min(0f)]
		private float m_SpringDamping;

		[SerializeField]
		private bool m_EnableMotor;

		[SerializeField]
		private float m_MotorSpeed;

		[Min(0f)]
		[SerializeField]
		private float m_MaxMotorTorque;

		[SerializeField]
		private bool m_EnableLimit;

		[Range(-178f, 178f)]
		[SerializeField]
		private float m_LowerAngleLimit;

		[SerializeField]
		[Range(-178f, 178f)]
		private float m_UpperAngleLimit;

		[SerializeField]
		[Min(0f)]
		private float m_ForceThreshold;

		[SerializeField]
		[Min(0f)]
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
