using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsRelativeJointDefinition
	{
		public PhysicsRelativeJointDefinition()
		{
			this = PhysicsRelativeJointDefinition.defaultDefinition;
		}

		public PhysicsRelativeJointDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.RelativeJoint_GetDefaultDefinition(useSettings);
		}

		public static PhysicsRelativeJointDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.RelativeJoint_GetDefaultDefinition(true);
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

		public Vector2 linearVelocity
		{
			readonly get
			{
				return this.m_LinearVelocity;
			}
			set
			{
				this.m_LinearVelocity = value;
			}
		}

		public float angularVelocity
		{
			readonly get
			{
				return this.m_AngularVelocity;
			}
			set
			{
				this.m_AngularVelocity = value;
			}
		}

		public float maxForce
		{
			readonly get
			{
				return this.m_MaxForce;
			}
			set
			{
				this.m_MaxForce = Mathf.Max(0f, value);
			}
		}

		public float maxTorque
		{
			readonly get
			{
				return this.m_MaxTorque;
			}
			set
			{
				this.m_MaxTorque = Mathf.Max(0f, value);
			}
		}

		public float springLinearFrequency
		{
			readonly get
			{
				return this.m_SpringLinearFrequency;
			}
			set
			{
				this.m_SpringLinearFrequency = Mathf.Max(0f, value);
			}
		}

		public float springAngularFrequency
		{
			readonly get
			{
				return this.m_SpringAngularFrequency;
			}
			set
			{
				this.m_SpringAngularFrequency = Mathf.Max(0f, value);
			}
		}

		public float springLinearDamping
		{
			readonly get
			{
				return this.m_SpringLinearDamping;
			}
			set
			{
				this.m_SpringLinearDamping = Mathf.Max(0f, value);
			}
		}

		public float springAngularDamping
		{
			readonly get
			{
				return this.m_SpringAngularDamping;
			}
			set
			{
				this.m_SpringAngularDamping = Mathf.Max(0f, value);
			}
		}

		public float springMaxForce
		{
			readonly get
			{
				return this.m_SpringMaxForce;
			}
			set
			{
				this.m_SpringMaxForce = Mathf.Max(0f, value);
			}
		}

		public float springMaxTorque
		{
			readonly get
			{
				return this.m_SpringMaxTorque;
			}
			set
			{
				this.m_SpringMaxTorque = Mathf.Max(0f, value);
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
		private Vector2 m_LinearVelocity;

		[SerializeField]
		private float m_AngularVelocity;

		[Min(0f)]
		[SerializeField]
		private float m_MaxForce;

		[SerializeField]
		[Min(0f)]
		private float m_MaxTorque;

		[SerializeField]
		[Min(0f)]
		private float m_SpringLinearFrequency;

		[SerializeField]
		[Min(0f)]
		private float m_SpringAngularFrequency;

		[SerializeField]
		[Min(0f)]
		private float m_SpringLinearDamping;

		[Min(0f)]
		[SerializeField]
		private float m_SpringAngularDamping;

		[Min(0f)]
		[SerializeField]
		private float m_SpringMaxForce;

		[Min(0f)]
		[SerializeField]
		private float m_SpringMaxTorque;

		[Min(0f)]
		[SerializeField]
		private float m_ForceThreshold;

		[Min(0f)]
		[SerializeField]
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
