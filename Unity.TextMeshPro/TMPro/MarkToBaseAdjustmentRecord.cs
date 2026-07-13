using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public struct MarkToBaseAdjustmentRecord
	{
		public uint baseGlyphID
		{
			get
			{
				return this.m_BaseGlyphID;
			}
			set
			{
				this.m_BaseGlyphID = value;
			}
		}

		public GlyphAnchorPoint baseGlyphAnchorPoint
		{
			get
			{
				return this.m_BaseGlyphAnchorPoint;
			}
			set
			{
				this.m_BaseGlyphAnchorPoint = value;
			}
		}

		public uint markGlyphID
		{
			get
			{
				return this.m_MarkGlyphID;
			}
			set
			{
				this.m_MarkGlyphID = value;
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

		[SerializeField]
		private uint m_BaseGlyphID;

		[SerializeField]
		private GlyphAnchorPoint m_BaseGlyphAnchorPoint;

		[SerializeField]
		private uint m_MarkGlyphID;

		[SerializeField]
		private MarkPositionAdjustment m_MarkPositionAdjustment;
	}
}
