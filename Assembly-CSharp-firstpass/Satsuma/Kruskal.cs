using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Kruskal<TCost> where TCost : IComparable<TCost>
	{
		public IGraph Graph { get; private set; }

		public Func<Arc, TCost> Cost { get; private set; }

		public Func<Node, int> MaxDegree { get; private set; }

		public HashSet<Arc> Forest { get; private set; }

		public Dictionary<Node, int> Degree { get; private set; }

		public Kruskal(IGraph graph, Func<Arc, TCost> cost, Func<Node, int> maxDegree = null)
		{
			this.Graph = graph;
			this.Cost = cost;
			this.MaxDegree = maxDegree;
			this.Forest = new HashSet<Arc>();
			this.Degree = new Dictionary<Node, int>();
			foreach (Node node in this.Graph.Nodes())
			{
				this.Degree[node] = 0;
			}
			List<Arc> list = this.Graph.Arcs(ArcFilter.All).ToList<Arc>();
			list.Sort(delegate(Arc a, Arc b)
			{
				TCost tcost = this.Cost(a);
				return tcost.CompareTo(this.Cost(b));
			});
			this.arcEnumerator = list.GetEnumerator();
			this.arcsToGo = this.Graph.NodeCount() - new ConnectedComponents(this.Graph, ConnectedComponents.Flags.None).Count;
			this.components = new DisjointSet<Node>();
		}

		public bool Step()
		{
			if (this.arcsToGo <= 0 || this.arcEnumerator == null || !this.arcEnumerator.MoveNext())
			{
				this.arcEnumerator = null;
				return false;
			}
			this.AddArc(this.arcEnumerator.Current);
			return true;
		}

		public void Run()
		{
			while (this.Step())
			{
			}
		}

		public bool AddArc(Arc arc)
		{
			Node node = this.Graph.U(arc);
			if (this.MaxDegree != null && this.Degree[node] >= this.MaxDegree(node))
			{
				return false;
			}
			DisjointSetSet<Node> disjointSetSet = this.components.WhereIs(node);
			Node node2 = this.Graph.V(arc);
			if (this.MaxDegree != null && this.Degree[node2] >= this.MaxDegree(node2))
			{
				return false;
			}
			DisjointSetSet<Node> disjointSetSet2 = this.components.WhereIs(node2);
			if (disjointSetSet == disjointSetSet2)
			{
				return false;
			}
			this.Forest.Add(arc);
			this.components.Union(disjointSetSet, disjointSetSet2);
			Dictionary<Node, int> degree = this.Degree;
			Node node3 = node;
			int num = degree[node3];
			degree[node3] = num + 1;
			Dictionary<Node, int> degree2 = this.Degree;
			node3 = node2;
			num = degree2[node3];
			degree2[node3] = num + 1;
			this.arcsToGo--;
			return true;
		}

		private IEnumerator<Arc> arcEnumerator;

		private int arcsToGo;

		private DisjointSet<Node> components;
	}
}
