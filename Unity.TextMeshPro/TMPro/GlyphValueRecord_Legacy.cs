using System;
using UnityEngine.TextCore.LowLevel;

namespace TMPro
{
	[Serializable]
	public struct GlyphValueRecord_Legacy
	{
		internal GlyphValueRecord_Legacy(GlyphValueRecord valueRecord)
		{
			this.xPlacement = valueRecord.xPlacement;
			this.yPlacement = valueRecord.yPlacement;
			this.xAdvance = valueRecord.xAdvance;
			this.yAdvance = valueRecord.yAdvance;
		}

		public static GlyphValueRecord_Legacy operator +(GlyphValueRecord_Legacy a, GlyphValueRecord_Legacy b)
		{
			GlyphValueRecord_Legacy glyphValueRecord_Legacy;
			glyphValueRecord_Legacy.xPlacement = a.xPlacement + b.xPlacement;
			glyphValueRecord_Legacy.yPlacement = a.yPlacement + b.yPlacement;
			glyphValueRecord_Legacy.xAdvance = a.xAdvance + b.xAdvance;
			glyphValueRecord_Legacy.yAdvance = a.yAdvance + b.yAdvance;
			return glyphValueRecord_Legacy;
		}

		public float xPlacement;

		public float yPlacement;

		public float xAdvance;

		public float yAdvance;
	}
}
