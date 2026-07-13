using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public struct MarkPositionAdjustment
	{
		public float xPositionAdjustment
		{
			get
			{
				return this.m_XPositionAdjustment;
			}
			set
			{
				this.m_XPositionAdjustment = value;
			}
		}

		public float yPositionAdjustment
		{
			get
			{
				return this.m_YPositionAdjustment;
			}
			set
			{
				this.m_YPositionAdjustment = value;
			}
		}

		public MarkPositionAdjustment(float x, float y)
		{
			this.m_XPositionAdjustment = x;
			this.m_YPositionAdjustment = y;
		}

		[SerializeField]
		private float m_XPositionAdjustment;

		[SerializeField]
		private float m_YPositionAdjustment;
	}
}
