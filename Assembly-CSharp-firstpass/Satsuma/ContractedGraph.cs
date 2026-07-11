using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class ContractedGraph : IGraph, IArcLookup
	{
		public ContractedGraph(IGraph graph)
		{
			this.graph = graph;
			this.nodeGroups = new DisjointSet<Node>();
			this.Reset();
		}

		public void Reset()
		{
			this.nodeGroups.Clear();
			this.unionCount = 0;
		}

		public Node Merge(Node u, Node v)
		{
			DisjointSetSet<Node> disjointSetSet = this.nodeGroups.WhereIs(u);
			DisjointSetSet<Node> disjointSetSet2 = this.nodeGroups.WhereIs(v);
			if (disjointSetSet.Equals(disjointSetSet2))
			{
				return disjointSetSet.Representative;
			}
			this.unionCount++;
			return this.nodeGroups.Union(disjointSetSet, disjointSetSet2).Representative;
		}

		public Node Contract(Arc arc)
		{
			return this.Merge(this.graph.U(arc), this.graph.V(arc));
		}

		public Node U(Arc arc)
		{
			return this.nodeGroups.WhereIs(this.graph.U(arc)).Representative;
		}

		public Node V(Arc arc)
		{
			return this.nodeGroups.WhereIs(this.graph.V(arc)).Representative;
		}

		public bool IsEdge(Arc arc)
		{
			return this.graph.IsEdge(arc);
		}

		public IEnumerable<Node> Nodes()
		{
			foreach (Node node in this.graph.Nodes())
			{
				if (this.nodeGroups.WhereIs(node).Representative == node)
				{
					yield return node;
				}
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return this.graph.Arcs(filter);
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			DisjointSetSet<Node> x = this.nodeGroups.WhereIs(u);
			foreach (Node node in this.nodeGroups.Elements(x))
			{
				foreach (Arc arc in this.graph.Arcs(node, filter))
				{
					bool loop = this.U(arc) == this.V(arc);
					if (!loop || (filter != ArcFilter.All && !this.IsEdge(arc)) || this.graph.U(arc) == node)
					{
						yield return arc;
					}
				}
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			foreach (Arc arc in this.Arcs(u, filter))
			{
				if (this.Other(arc, u) == v)
				{
					yield return arc;
				}
			}
			yield break;
		}

		public int NodeCount()
		{
			return this.graph.NodeCount() - this.unionCount;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return this.graph.ArcCount(filter);
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.Arcs(u, filter).Count<Arc>();
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return this.Arcs(u, v, filter).Count<Arc>();
		}

		public bool HasNode(Node node)
		{
			return node == this.nodeGroups.WhereIs(node).Representative;
		}

		public bool HasArc(Arc arc)
		{
			return this.graph.HasArc(arc);
		}

		private IGraph graph;

		private DisjointSet<Node> nodeGroups;

		private int unionCount;
	}
}
