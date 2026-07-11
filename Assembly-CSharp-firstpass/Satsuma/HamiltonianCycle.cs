using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class HamiltonianCycle
	{
		public HamiltonianCycle(IGraph graph)
		{
			this.Graph = graph;
			this.Cycle = null;
			this.Run();
		}

		public IGraph Graph { get; private set; }

		public IPath Cycle { get; private set; }

		private void Run()
		{
			Func<Node, Node, double> func = (Node u, Node v) => (double)((!this.Graph.Arcs(u, v, ArcFilter.Forward).Any<Arc>()) ? 10 : 1);
			IEnumerable<Node> enumerable = null;
			double num = (double)this.Graph.NodeCount();
			InsertionTsp<Node> insertionTsp = new InsertionTsp<Node>(this.Graph.Nodes(), func, TspSelectionRule.Farthest);
			insertionTsp.Run();
			if (insertionTsp.TourCost == num)
			{
				enumerable = insertionTsp.Tour;
			}
			else
			{
				Opt2Tsp<Node> opt2Tsp = new Opt2Tsp<Node>(func, insertionTsp.Tour, new double?(insertionTsp.TourCost));
				opt2Tsp.Run();
				if (opt2Tsp.TourCost == num)
				{
					enumerable = opt2Tsp.Tour;
				}
			}
			if (enumerable == null)
			{
				this.Cycle = null;
			}
			else
			{
				Path path = new Path(this.Graph);
				if (enumerable.Any<Node>())
				{
					Node node = Node.Invalid;
					foreach (Node node2 in enumerable)
					{
						if (node == Node.Invalid)
						{
							path.Begin(node2);
						}
						else
						{
							path.AddLast(this.Graph.Arcs(node, node2, ArcFilter.Forward).First<Arc>());
						}
						node = node2;
					}
					path.AddLast(this.Graph.Arcs(node, enumerable.First<Node>(), ArcFilter.Forward).First<Arc>());
				}
				this.Cycle = path;
			}
		}
	}
}
