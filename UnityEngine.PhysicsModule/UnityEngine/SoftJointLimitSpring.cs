using System;

namespace UnityEngine
{
	public struct SoftJointLimitSpring
	{
		public float spring
		{
			get
			{
				return this.m_Spring;
			}
			set
			{
				this.m_Spring = value;
			}
		}

		public float damper
		{
			get
			{
				return this.m_Damper;
			}
			set
			{
				this.m_Damper = value;
			}
		}

		private float m_Spring;

		private float m_Damper;
	}
}
