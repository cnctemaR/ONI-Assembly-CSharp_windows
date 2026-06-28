using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class MinimumCostMatching
	{
		public IGraph Graph { get; private set; }

		public Func<Node, bool> IsRed { get; private set; }

		public Func<Arc, double> Cost { get; private set; }

		public int MinimumMatchingSize { get; private set; }

		public int MaximumMatchingSize { get; private set; }

		public IMatching Matching { get; private set; }

		public MinimumCostMatching(IGraph graph, Func<Node, bool> isRed, Func<Arc, double> cost, int minimumMatchingSize = 0, int maximumMatchingSize = 2147483647)
		{
			this.Graph = graph;
			this.IsRed = isRed;
			this.Cost = cost;
			this.MinimumMatchingSize = minimumMatchingSize;
			this.MaximumMatchingSize = maximumMatchingSize;
			this.Run();
		}

		private void Run()
		{
			RedirectedGraph redirectedGraph = new RedirectedGraph(this.Graph, delegate(Arc x)
			{
				if (!this.IsRed(this.Graph.U(x)))
				{
					return RedirectedGraph.Direction.Backward;
				}
				return RedirectedGraph.Direction.Forward;
			});
			Supergraph supergraph = new Supergraph(redirectedGraph);
			Node node = supergraph.AddNode();
			Node node2 = supergraph.AddNode();
			foreach (Node node3 in this.Graph.Nodes())
			{
				if (this.IsRed(node3))
				{
					supergraph.AddArc(node, node3, Directedness.Directed);
				}
				else
				{
					supergraph.AddArc(node3, node2, Directedness.Directed);
				}
			}
			Arc reflow = supergraph.AddArc(node2, node, Directedness.Directed);
			NetworkSimplex networkSimplex = new NetworkSimplex(supergraph, (Arc x) => (long)((x == reflow) ? this.MinimumMatchingSize : 0), (Arc x) => (long)((x == reflow) ? this.MaximumMatchingSize : 1), null, delegate(Arc x)
			{
				if (!this.Graph.HasArc(x))
				{
					return 0.0;
				}
				return this.Cost(x);
			});
			networkSimplex.Run();
			if (networkSimplex.State == SimplexState.Optimal)
			{
				Matching matching = new Matching(this.Graph);
				foreach (Arc arc in networkSimplex.UpperBoundArcs.Concat<Arc>(from kv in networkSimplex.Forest
					where kv.Value == 1L
					select kv.Key))
				{
					if (this.Graph.HasArc(arc))
					{
						matching.Enable(arc, true);
					}
				}
				this.Matching = matching;
			}
		}
	}
}
