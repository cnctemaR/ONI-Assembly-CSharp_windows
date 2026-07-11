using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore
{
	[Serializable]
	internal class KerningTable
	{
		public KerningTable()
		{
			this.kerningPairs = new List<KerningPair>();
		}

		public int AddGlyphPairAdjustmentRecord(uint first, GlyphValueRecord firstAdjustments, uint second, GlyphValueRecord secondAdjustments)
		{
			int num = this.kerningPairs.FindIndex((KerningPair item) => item.firstGlyph == first && item.secondGlyph == second);
			bool flag = num == -1;
			int num2;
			if (flag)
			{
				this.kerningPairs.Add(new KerningPair(first, firstAdjustments, second, secondAdjustments));
				num2 = 0;
			}
			else
			{
				num2 = -1;
			}
			return num2;
		}

		public void RemoveKerningPair(int left, int right)
		{
			int num = this.kerningPairs.FindIndex((KerningPair item) => (ulong)item.firstGlyph == (ulong)((long)left) && (ulong)item.secondGlyph == (ulong)((long)right));
			bool flag = num != -1;
			if (flag)
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
			bool flag = this.kerningPairs.Count > 0;
			if (flag)
			{
				this.kerningPairs = (from s in this.kerningPairs
					orderby s.firstGlyph, s.secondGlyph
					select s).ToList<KerningPair>();
			}
		}

		public List<KerningPair> kerningPairs;
	}
}
