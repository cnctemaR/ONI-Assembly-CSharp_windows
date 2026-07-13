using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[UsedByNativeCode]
	[Serializable]
	internal struct MarkAdjustmentRecord
	{
		public GlyphAnchorPoint anchorPosition
		{
			get
			{
				return this.m_AnchorPoint;
			}
			set
			{
				this.m_AnchorPoint = value;
			}
		}

		public MarkPositionAdjustment markPositionAdjustment
		{
			get
			{
				return this.m_MarkPositionAdjustment;
			}
			set
			{
				this.m_MarkPositionAdjustment = value;
			}
		}

		[NativeName("anchorPoint")]
		[SerializeField]
		private GlyphAnchorPoint m_AnchorPoint;

		[SerializeField]
		[NativeName("markPositionAdjustment")]
		private MarkPositionAdjustment m_MarkPositionAdjustment;
	}
}
