using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Prim<TCost> where TCost : IComparable<TCost>
	{
		public IGraph Graph { get; private set; }

		public Func<Arc, TCost> Cost { get; private set; }

		public HashSet<Arc> Forest { get; private set; }

		public Prim(IGraph graph, Func<Arc, TCost> cost)
		{
			this.Graph = graph;
			this.Cost = cost;
			this.Forest = new HashSet<Arc>();
			this.Run();
		}

		private void Run()
		{
			this.Forest.Clear();
			PriorityQueue<Node, TCost> priorityQueue = new PriorityQueue<Node, TCost>();
			HashSet<Node> hashSet = new HashSet<Node>();
			Dictionary<Node, Arc> dictionary = new Dictionary<Node, Arc>();
			ConnectedComponents connectedComponents = new ConnectedComponents(this.Graph, ConnectedComponents.Flags.CreateComponents);
			foreach (HashSet<Node> hashSet2 in connectedComponents.Components)
			{
				Node node = hashSet2.First<Node>();
				hashSet.Add(node);
				foreach (Arc arc in this.Graph.Arcs(node, ArcFilter.All))
				{
					Node node2 = this.Graph.Other(arc, node);
					dictionary[node2] = arc;
					priorityQueue[node2] = this.Cost(arc);
				}
			}
			while (priorityQueue.Count != 0)
			{
				Node node3 = priorityQueue.Peek();
				priorityQueue.Pop();
				hashSet.Add(node3);
				this.Forest.Add(dictionary[node3]);
				foreach (Arc arc2 in this.Graph.Arcs(node3, ArcFilter.All))
				{
					Node node4 = this.Graph.Other(arc2, node3);
					if (!hashSet.Contains(node4))
					{
						TCost tcost = this.Cost(arc2);
						if (tcost.CompareTo(priorityQueue[node4]) < 0)
						{
							priorityQueue[node4] = tcost;
							dictionary[node4] = arc2;
						}
					}
				}
			}
		}
	}
}
