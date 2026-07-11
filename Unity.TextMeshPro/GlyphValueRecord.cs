using System;

namespace TMPro
{
	[Serializable]
	public struct GlyphValueRecord
	{
		public static GlyphValueRecord operator +(GlyphValueRecord a, GlyphValueRecord b)
		{
			GlyphValueRecord glyphValueRecord;
			glyphValueRecord.xPlacement = a.xPlacement + b.xPlacement;
			glyphValueRecord.yPlacement = a.yPlacement + b.yPlacement;
			glyphValueRecord.xAdvance = a.xAdvance + b.xAdvance;
			glyphValueRecord.yAdvance = a.yAdvance + b.yAdvance;
			return glyphValueRecord;
		}

		public float xPlacement;

		public float yPlacement;

		public float xAdvance;

		public float yAdvance;
	}
}
