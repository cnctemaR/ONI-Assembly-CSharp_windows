using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsIgnoreJointDefinition
	{
		public PhysicsIgnoreJointDefinition()
		{
			this = PhysicsLowLevelScripting2D.IgnorePhysicsJoint_GetDefaultDefinition();
		}

		public static PhysicsIgnoreJointDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.IgnorePhysicsJoint_GetDefaultDefinition();
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

		[SerializeField]
		private PhysicsBody m_BodyA;

		[SerializeField]
		private PhysicsBody m_BodyB;
	}
}
