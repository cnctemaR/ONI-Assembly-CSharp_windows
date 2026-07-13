using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TMPro
{
	[Serializable]
	public class KerningPair
	{
		public uint firstGlyph
		{
			get
			{
				return this.m_FirstGlyph;
			}
			set
			{
				this.m_FirstGlyph = value;
			}
		}

		public GlyphValueRecord_Legacy firstGlyphAdjustments
		{
			get
			{
				return this.m_FirstGlyphAdjustments;
			}
		}

		public uint secondGlyph
		{
			get
			{
				return this.m_SecondGlyph;
			}
			set
			{
				this.m_SecondGlyph = value;
			}
		}

		public GlyphValueRecord_Legacy secondGlyphAdjustments
		{
			get
			{
				return this.m_SecondGlyphAdjustments;
			}
		}

		public bool ignoreSpacingAdjustments
		{
			get
			{
				return this.m_IgnoreSpacingAdjustments;
			}
		}

		public KerningPair()
		{
			this.m_FirstGlyph = 0U;
			this.m_FirstGlyphAdjustments = default(GlyphValueRecord_Legacy);
			this.m_SecondGlyph = 0U;
			this.m_SecondGlyphAdjustments = default(GlyphValueRecord_Legacy);
		}

		public KerningPair(uint left, uint right, float offset)
		{
			this.firstGlyph = left;
			this.m_SecondGlyph = right;
			this.xOffset = offset;
		}

		public KerningPair(uint firstGlyph, GlyphValueRecord_Legacy firstGlyphAdjustments, uint secondGlyph, GlyphValueRecord_Legacy secondGlyphAdjustments)
		{
			this.m_FirstGlyph = firstGlyph;
			this.m_FirstGlyphAdjustments = firstGlyphAdjustments;
			this.m_SecondGlyph = secondGlyph;
			this.m_SecondGlyphAdjustments = secondGlyphAdjustments;
		}

		internal void ConvertLegacyKerningData()
		{
			this.m_FirstGlyphAdjustments.xAdvance = this.xOffset;
		}

		[FormerlySerializedAs("AscII_Left")]
		[SerializeField]
		private uint m_FirstGlyph;

		[SerializeField]
		private GlyphValueRecord_Legacy m_FirstGlyphAdjustments;

		[FormerlySerializedAs("AscII_Right")]
		[SerializeField]
		private uint m_SecondGlyph;

		[SerializeField]
		private GlyphValueRecord_Legacy m_SecondGlyphAdjustments;

		[FormerlySerializedAs("XadvanceOffset")]
		public float xOffset;

		internal static KerningPair empty = new KerningPair(0U, default(GlyphValueRecord_Legacy), 0U, default(GlyphValueRecord_Legacy));

		[SerializeField]
		private bool m_IgnoreSpacingAdjustments;
	}
}
