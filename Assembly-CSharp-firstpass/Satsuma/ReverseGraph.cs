using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class ReverseGraph : IGraph, IArcLookup
	{
		public ReverseGraph(IGraph graph)
		{
			this.graph = graph;
		}

		public static ArcFilter Reverse(ArcFilter filter)
		{
			if (filter == ArcFilter.Forward)
			{
				return ArcFilter.Backward;
			}
			if (filter == ArcFilter.Backward)
			{
				return ArcFilter.Forward;
			}
			return filter;
		}

		public Node U(Arc arc)
		{
			return this.graph.V(arc);
		}

		public Node V(Arc arc)
		{
			return this.graph.U(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return this.graph.IsEdge(arc);
		}

		public IEnumerable<Node> Nodes()
		{
			return this.graph.Nodes();
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return this.graph.Arcs(filter);
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.graph.Arcs(u, ReverseGraph.Reverse(filter));
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return this.graph.Arcs(u, v, ReverseGraph.Reverse(filter));
		}

		public int NodeCount()
		{
			return this.graph.NodeCount();
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return this.graph.ArcCount(filter);
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.graph.ArcCount(u, ReverseGraph.Reverse(filter));
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return this.graph.ArcCount(u, v, ReverseGraph.Reverse(filter));
		}

		public bool HasNode(Node node)
		{
			return this.graph.HasNode(node);
		}

		public bool HasArc(Arc arc)
		{
			return this.graph.HasArc(arc);
		}

		private IGraph graph;
	}
}
