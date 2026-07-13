using System;

namespace UnityEngine
{
	public struct JointTranslationLimits2D
	{
		public float min
		{
			get
			{
				return this.m_LowerTranslation;
			}
			set
			{
				this.m_LowerTranslation = value;
			}
		}

		public float max
		{
			get
			{
				return this.m_UpperTranslation;
			}
			set
			{
				this.m_UpperTranslation = value;
			}
		}

		private float m_LowerTranslation;

		private float m_UpperTranslation;
	}
}
