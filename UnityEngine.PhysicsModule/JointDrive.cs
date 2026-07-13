using System;

namespace UnityEngine
{
	public struct JointDrive
	{
		public float positionSpring
		{
			get
			{
				return this.m_PositionSpring;
			}
			set
			{
				this.m_PositionSpring = value;
			}
		}

		public float positionDamper
		{
			get
			{
				return this.m_PositionDamper;
			}
			set
			{
				this.m_PositionDamper = value;
			}
		}

		public float maximumForce
		{
			get
			{
				return this.m_MaximumForce;
			}
			set
			{
				this.m_MaximumForce = value;
			}
		}

		public bool useAcceleration
		{
			get
			{
				return this.m_UseAcceleration == 1;
			}
			set
			{
				this.m_UseAcceleration = (value ? 1 : 0);
			}
		}

		private float m_PositionSpring;

		private float m_PositionDamper;

		private float m_MaximumForce;

		private int m_UseAcceleration;
	}
}
