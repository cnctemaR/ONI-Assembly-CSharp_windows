using System;
using System.Collections.Generic;

namespace ClipperLib
{
	public class MyIntersectNodeSort : IComparer<IntersectNode>
	{
		public int Compare(IntersectNode node1, IntersectNode node2)
		{
			long num = node2.Pt.Y - node1.Pt.Y;
			int num2;
			if (num > 0L)
			{
				num2 = 1;
			}
			else if (num < 0L)
			{
				num2 = -1;
			}
			else
			{
				num2 = 0;
			}
			return num2;
		}
	}
}
