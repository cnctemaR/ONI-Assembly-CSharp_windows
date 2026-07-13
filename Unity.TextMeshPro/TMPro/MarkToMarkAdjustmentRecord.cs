using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public struct MarkToMarkAdjustmentRecord
	{
		public uint baseMarkGlyphID
		{
			get
			{
				return this.m_BaseMarkGlyphID;
			}
			set
			{
				this.m_BaseMarkGlyphID = value;
			}
		}

		public GlyphAnchorPoint baseMarkGlyphAnchorPoint
		{
			get
			{
				return this.m_BaseMarkGlyphAnchorPoint;
			}
			set
			{
				this.m_BaseMarkGlyphAnchorPoint = value;
			}
		}

		public uint combiningMarkGlyphID
		{
			get
			{
				return this.m_CombiningMarkGlyphID;
			}
			set
			{
				this.m_CombiningMarkGlyphID = value;
			}
		}

		public MarkPositionAdjustment combiningMarkPositionAdjustment
		{
			get
			{
				return this.m_CombiningMarkPositionAdjustment;
			}
			set
			{
				this.m_CombiningMarkPositionAdjustment = value;
			}
		}

		[SerializeField]
		private uint m_BaseMarkGlyphID;

		[SerializeField]
		private GlyphAnchorPoint m_BaseMarkGlyphAnchorPoint;

		[SerializeField]
		private uint m_CombiningMarkGlyphID;

		[SerializeField]
		private MarkPositionAdjustment m_CombiningMarkPositionAdjustment;
	}
}
