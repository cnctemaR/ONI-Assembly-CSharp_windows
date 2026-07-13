using System;
using System.Collections.Generic;

namespace ClipperLib
{
	internal class MyIntersectNodeSort : IComparer<IntersectNode>
	{
		public int Compare(IntersectNode node1, IntersectNode node2)
		{
			long num = node2.Pt.Y - node1.Pt.Y;
			bool flag = num > 0L;
			int num2;
			if (flag)
			{
				num2 = 1;
			}
			else
			{
				bool flag2 = num < 0L;
				if (flag2)
				{
					num2 = -1;
				}
				else
				{
					num2 = 0;
				}
			}
			return num2;
		}
	}
}
