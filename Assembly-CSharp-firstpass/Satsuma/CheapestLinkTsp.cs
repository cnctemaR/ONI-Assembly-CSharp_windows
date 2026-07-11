using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class CheapestLinkTsp<TNode> : ITsp<TNode>
	{
		public IList<TNode> Nodes { get; private set; }

		public Func<TNode, TNode, double> Cost { get; private set; }

		public IEnumerable<TNode> Tour
		{
			get
			{
				return this.tour;
			}
		}

		public double TourCost { get; private set; }

		public CheapestLinkTsp(IList<TNode> nodes, Func<TNode, TNode, double> cost)
		{
			this.Nodes = nodes;
			this.Cost = cost;
			this.tour = new List<TNode>();
			this.Run();
		}

		private void Run()
		{
			CompleteGraph graph = new CompleteGraph(this.Nodes.Count, Directedness.Undirected);
			Func<Arc, double> func = (Arc arc) => this.Cost(this.Nodes[graph.GetNodeIndex(graph.U(arc))], this.Nodes[graph.GetNodeIndex(graph.V(arc))]);
			Kruskal<double> kruskal = new Kruskal<double>(graph, func, (Node _) => 2);
			kruskal.Run();
			Dictionary<Node, Arc> dictionary = new Dictionary<Node, Arc>();
			Dictionary<Node, Arc> dictionary2 = new Dictionary<Node, Arc>();
			foreach (Arc arc4 in kruskal.Forest)
			{
				Node node = graph.U(arc4);
				(dictionary.ContainsKey(node) ? dictionary2 : dictionary)[node] = arc4;
				Node node2 = graph.V(arc4);
				(dictionary.ContainsKey(node2) ? dictionary2 : dictionary)[node2] = arc4;
			}
			foreach (Node node3 in graph.Nodes())
			{
				if (kruskal.Degree[node3] == 1)
				{
					Arc arc2 = Arc.Invalid;
					Node node4 = node3;
					for (;;)
					{
						this.tour.Add(this.Nodes[graph.GetNodeIndex(node4)]);
						if (arc2 != Arc.Invalid && kruskal.Degree[node4] == 1)
						{
							break;
						}
						Arc arc3 = dictionary[node4];
						arc2 = ((arc3 != arc2) ? arc3 : dictionary2[node4]);
						node4 = graph.Other(arc2, node4);
					}
					this.tour.Add(this.Nodes[graph.GetNodeIndex(node3)]);
					break;
				}
			}
			this.TourCost = TspUtils.GetTourCost<TNode>(this.tour, this.Cost);
		}

		private List<TNode> tour;
	}
}
