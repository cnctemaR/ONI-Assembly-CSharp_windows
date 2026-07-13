using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
	[UsedByNativeCode]
	[Serializable]
	internal struct MarkToMarkAdjustmentRecord
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
		[NativeName("baseMarkGlyphID")]
		private uint m_BaseMarkGlyphID;

		[SerializeField]
		[NativeName("baseMarkAnchor")]
		private GlyphAnchorPoint m_BaseMarkGlyphAnchorPoint;

		[NativeName("combiningMarkGlyphID")]
		[SerializeField]
		private uint m_CombiningMarkGlyphID;

		[SerializeField]
		[NativeName("combiningMarkPositionAdjustment")]
		private MarkPositionAdjustment m_CombiningMarkPositionAdjustment;
	}
}
