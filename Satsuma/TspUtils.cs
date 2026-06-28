using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public static class TspUtils
	{
		public static double GetTourCost<TNode>(IEnumerable<TNode> tour, Func<TNode, TNode, double> cost)
		{
			double num = 0.0;
			if (tour.Any<TNode>())
			{
				TNode tnode = tour.First<TNode>();
				foreach (TNode tnode2 in tour.Skip<TNode>(1))
				{
					num += cost(tnode, tnode2);
					tnode = tnode2;
				}
			}
			return num;
		}
	}
}
