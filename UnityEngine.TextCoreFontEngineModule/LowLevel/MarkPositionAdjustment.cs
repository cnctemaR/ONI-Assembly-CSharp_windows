using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
	[UsedByNativeCode]
	[Serializable]
	internal struct MarkPositionAdjustment
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

		[NativeName("xCoordinate")]
		[SerializeField]
		private float m_XPositionAdjustment;

		[NativeName("yCoordinate")]
		[SerializeField]
		private float m_YPositionAdjustment;
	}
}
