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
				this.kerningPairs.Add(new KerningPair(0, 0, 0f));
			}
			else
			{
				int ascII_Left = this.kerningPairs.Last<KerningPair>().AscII_Left;
				int ascII_Right = this.kerningPairs.Last<KerningPair>().AscII_Right;
				float xadvanceOffset = this.kerningPairs.Last<KerningPair>().XadvanceOffset;
				this.kerningPairs.Add(new KerningPair(ascII_Left, ascII_Right, xadvanceOffset));
			}
		}

		public int AddKerningPair(int left, int right, float offset)
		{
			int num = this.kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == left && item.AscII_Right == right);
			int num2;
			if (num == -1)
			{
				this.kerningPairs.Add(new KerningPair(left, right, offset));
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
			int num = this.kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == left && item.AscII_Right == right);
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
					orderby s.AscII_Left, s.AscII_Right
					select s).ToList<KerningPair>();
			}
		}

		public List<KerningPair> kerningPairs;
	}
}
