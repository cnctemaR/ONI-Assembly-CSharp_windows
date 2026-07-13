using System;

namespace TMPro
{
	public struct GlyphPairKey
	{
		public GlyphPairKey(uint firstGlyphIndex, uint secondGlyphIndex)
		{
			this.firstGlyphIndex = firstGlyphIndex;
			this.secondGlyphIndex = secondGlyphIndex;
			this.key = (secondGlyphIndex << 16) | firstGlyphIndex;
		}

		internal GlyphPairKey(TMP_GlyphPairAdjustmentRecord record)
		{
			this.firstGlyphIndex = record.firstAdjustmentRecord.glyphIndex;
			this.secondGlyphIndex = record.secondAdjustmentRecord.glyphIndex;
			this.key = (this.secondGlyphIndex << 16) | this.firstGlyphIndex;
		}

		public uint firstGlyphIndex;

		public uint secondGlyphIndex;

		public uint key;
	}
}
