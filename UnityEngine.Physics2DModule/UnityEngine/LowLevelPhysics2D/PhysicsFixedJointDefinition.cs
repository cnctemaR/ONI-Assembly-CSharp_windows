using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsFixedJointDefinition
	{
		public PhysicsFixedJointDefinition()
		{
			this = PhysicsFixedJointDefinition.defaultDefinition;
		}

		public PhysicsFixedJointDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.FixedJoint_GetDefaultDefinition(useSettings);
		}

		public static PhysicsFixedJointDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.FixedJoint_GetDefaultDefinition(true);
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

		public float linearFrequency
		{
			readonly get
			{
				return this.m_LinearFrequency;
			}
			set
			{
				this.m_LinearFrequency = Mathf.Max(0f, value);
			}
		}

		public float linearDamping
		{
			readonly get
			{
				return this.m_LinearDamping;
			}
			set
			{
				this.m_LinearDamping = Mathf.Max(0f, value);
			}
		}

		public float angularFrequency
		{
			readonly get
			{
				return this.m_AngularFrequency;
			}
			set
			{
				this.m_AngularFrequency = Mathf.Max(0f, value);
			}
		}

		public float angularDamping
		{
			readonly get
			{
				return this.m_AngularDamping;
			}
			set
			{
				this.m_AngularDamping = Mathf.Max(0f, value);
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
		[Min(0f)]
		private float m_LinearFrequency;

		[SerializeField]
		[Min(0f)]
		private float m_LinearDamping;

		[SerializeField]
		[Min(0f)]
		private float m_AngularFrequency;

		[Min(0f)]
		[SerializeField]
		private float m_AngularDamping;

		[SerializeField]
		[Min(0f)]
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

		[SerializeField]
		[Range(0.001f, 10f)]
		private float m_DrawScale;

		[SerializeField]
		private bool m_CollideConnected;
	}
}
