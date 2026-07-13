using System;
using System.ComponentModel;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsBodyDefinition
	{
		public PhysicsBodyDefinition()
		{
			this = PhysicsBodyDefinition.defaultDefinition;
		}

		public PhysicsBodyDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.PhysicsBody_GetDefaultDefinition(useSettings);
		}

		public static PhysicsBodyDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsBody_GetDefaultDefinition(true);
			}
		}

		public PhysicsBody.BodyType type
		{
			readonly get
			{
				return this.m_BodyType;
			}
			set
			{
				this.m_BodyType = value;
			}
		}

		public PhysicsBody.BodyConstraints constraints
		{
			readonly get
			{
				return this.m_BodyConstraints;
			}
			set
			{
				this.m_BodyConstraints = value;
			}
		}

		public PhysicsBody.TransformWriteMode transformWriteMode
		{
			readonly get
			{
				return this.m_TransformWriteMode;
			}
			set
			{
				this.m_TransformWriteMode = value;
			}
		}

		public Vector2 position
		{
			readonly get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		public PhysicsRotate rotation
		{
			readonly get
			{
				return this.m_Rotation;
			}
			set
			{
				this.m_Rotation = value;
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

		public float gravityScale
		{
			readonly get
			{
				return this.m_GravityScale;
			}
			set
			{
				this.m_GravityScale = value;
			}
		}

		public float sleepThreshold
		{
			readonly get
			{
				return this.m_SleepThreshold;
			}
			set
			{
				this.m_SleepThreshold = Mathf.Max(0f, value);
			}
		}

		public bool fastRotationAllowed
		{
			readonly get
			{
				return this.m_FastRotationAllowed;
			}
			set
			{
				this.m_FastRotationAllowed = value;
			}
		}

		public bool fastCollisionsAllowed
		{
			readonly get
			{
				return this.m_FastCollisionsAllowed;
			}
			set
			{
				this.m_FastCollisionsAllowed = value;
			}
		}

		public bool sleepingAllowed
		{
			readonly get
			{
				return this.m_SleepingAllowed;
			}
			set
			{
				this.m_SleepingAllowed = value;
			}
		}

		public bool awake
		{
			readonly get
			{
				return this.m_Awake;
			}
			set
			{
				this.m_Awake = value;
			}
		}

		public bool enabled
		{
			readonly get
			{
				return this.m_Enabled;
			}
			set
			{
				this.m_Enabled = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("PhysicsBodyDefinition.bodyType has been deprecated. Please use PhysicsBodyDefinition.type instead.", false)]
		public RigidbodyType2D bodyType
		{
			readonly get
			{
				return (RigidbodyType2D)this.m_BodyType;
			}
			set
			{
				this.m_BodyType = (PhysicsBody.BodyType)value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("PhysicsBodyDefinition.bodyConstraints has been deprecated. Please use PhysicsBodyDefinition.constraints instead.", false)]
		public RigidbodyConstraints2D bodyConstraints
		{
			readonly get
			{
				return (RigidbodyConstraints2D)this.m_BodyConstraints;
			}
			set
			{
				this.m_BodyConstraints = (PhysicsBody.BodyConstraints)value;
			}
		}

		[SerializeField]
		private PhysicsBody.BodyType m_BodyType;

		[SerializeField]
		private PhysicsBody.BodyConstraints m_BodyConstraints;

		[SerializeField]
		private PhysicsBody.TransformWriteMode m_TransformWriteMode;

		[SerializeField]
		private Vector2 m_Position;

		[SerializeField]
		private PhysicsRotate m_Rotation;

		[SerializeField]
		private Vector2 m_LinearVelocity;

		[SerializeField]
		private float m_AngularVelocity;

		[SerializeField]
		[Min(0f)]
		private float m_LinearDamping;

		[SerializeField]
		[Min(0f)]
		private float m_AngularDamping;

		[SerializeField]
		private float m_GravityScale;

		[SerializeField]
		[Min(0f)]
		private float m_SleepThreshold;

		[SerializeField]
		private bool m_FastRotationAllowed;

		[SerializeField]
		private bool m_FastCollisionsAllowed;

		[SerializeField]
		private bool m_SleepingAllowed;

		[SerializeField]
		private bool m_Awake;

		[SerializeField]
		private bool m_Enabled;
	}
}
