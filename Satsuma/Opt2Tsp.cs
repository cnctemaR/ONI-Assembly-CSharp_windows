using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Opt2Tsp<TNode> : ITsp<TNode>
	{
		public Func<TNode, TNode, double> Cost { get; private set; }

		public IEnumerable<TNode> Tour
		{
			get
			{
				return this.tour;
			}
		}

		public double TourCost { get; private set; }

		public Opt2Tsp(Func<TNode, TNode, double> cost, IEnumerable<TNode> tour, double? tourCost)
		{
			this.Cost = cost;
			this.tour = tour.ToList<TNode>();
			double? num = tourCost;
			this.TourCost = ((num != null) ? num.GetValueOrDefault() : TspUtils.GetTourCost<TNode>(tour, cost));
		}

		public bool Step()
		{
			bool flag = false;
			for (int i = 0; i < this.tour.Count - 3; i++)
			{
				int j = i + 2;
				int num = this.tour.Count - ((i == 0) ? 2 : 1);
				while (j < num)
				{
					double num2 = this.Cost(this.tour[i], this.tour[j]) + this.Cost(this.tour[i + 1], this.tour[j + 1]) - (this.Cost(this.tour[i], this.tour[i + 1]) + this.Cost(this.tour[j], this.tour[j + 1]));
					if (num2 < 0.0)
					{
						this.TourCost += num2;
						this.tour.Reverse(i + 1, j - i);
						flag = true;
					}
					j++;
				}
			}
			return flag;
		}

		public void Run()
		{
			while (this.Step())
			{
			}
		}

		private List<TNode> tour;
	}
}
