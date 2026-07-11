using System;
using System.Collections.Generic;
using System.Linq;

namespace TMPro
{
	[Serializable]
	public class KerningTable
	{
		public KerningTable()
		{
			this.kerningPairs = new List<KerningPair>();
		}

		public void AddKerningPair()
		{
			if (this.kerningPairs.Count == 0)
			{
				this.kerningPairs.Add(new KerningPair(0U, 0U, 0f));
				return;
			}
			uint firstGlyph = this.kerningPairs.Last<KerningPair>().firstGlyph;
			uint secondGlyph = this.kerningPairs.Last<KerningPair>().secondGlyph;
			float xOffset = this.kerningPairs.Last<KerningPair>().xOffset;
			this.kerningPairs.Add(new KerningPair(firstGlyph, secondGlyph, xOffset));
		}

		public int AddKerningPair(uint first, uint second, float offset)
		{
			if (this.kerningPairs.FindIndex((KerningPair item) => item.firstGlyph == first && item.secondGlyph == second) == -1)
			{
				this.kerningPairs.Add(new KerningPair(first, second, offset));
				return 0;
			}
			return -1;
		}

		public int AddGlyphPairAdjustmentRecord(uint first, GlyphValueRecord firstAdjustments, uint second, GlyphValueRecord secondAdjustments)
		{
			if (this.kerningPairs.FindIndex((KerningPair item) => item.firstGlyph == first && item.secondGlyph == second) == -1)
			{
				this.kerningPairs.Add(new KerningPair(first, firstAdjustments, second, secondAdjustments));
				return 0;
			}
			return -1;
		}

		public void RemoveKerningPair(int left, int right)
		{
			int num = this.kerningPairs.FindIndex((KerningPair item) => (ulong)item.firstGlyph == (ulong)((long)left) && (ulong)item.secondGlyph == (ulong)((long)right));
			if (num != -1)
			{
				this.kerningPairs.RemoveAt(num);
			}
		}

		public void RemoveKerningPair(int index)
		{
			this.kerningPairs.RemoveAt(index);
		}

		public void SortKerningPairs()
		{
			if (this.kerningPairs.Count > 0)
			{
				this.kerningPairs = (from s in this.kerningPairs
					orderby s.firstGlyph, s.secondGlyph
					select s).ToList<KerningPair>();
			}
		}

		public List<KerningPair> kerningPairs;
	}
}
