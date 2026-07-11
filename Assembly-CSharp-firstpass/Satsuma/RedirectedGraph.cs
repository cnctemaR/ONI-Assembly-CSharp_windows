using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class RedirectedGraph : IGraph, IArcLookup
	{
		public RedirectedGraph(IGraph graph, Func<Arc, RedirectedGraph.Direction> getDirection)
		{
			this.graph = graph;
			this.getDirection = getDirection;
		}

		public Node U(Arc arc)
		{
			return (this.getDirection(arc) != RedirectedGraph.Direction.Backward) ? this.graph.U(arc) : this.graph.V(arc);
		}

		public Node V(Arc arc)
		{
			return (this.getDirection(arc) != RedirectedGraph.Direction.Backward) ? this.graph.V(arc) : this.graph.U(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return this.getDirection(arc) == RedirectedGraph.Direction.Edge;
		}

		public IEnumerable<Node> Nodes()
		{
			return this.graph.Nodes();
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return (filter != ArcFilter.All) ? (from x in this.graph.Arcs(ArcFilter.All)
				where this.getDirection(x) == RedirectedGraph.Direction.Edge
				select x) : this.graph.Arcs(ArcFilter.All);
		}

		private IEnumerable<Arc> FilterArcs(Node u, IEnumerable<Arc> arcs, ArcFilter filter)
		{
			switch (filter)
			{
			case ArcFilter.All:
				return arcs;
			case ArcFilter.Edge:
				return arcs.Where<Arc>((Arc x) => this.getDirection(x) == RedirectedGraph.Direction.Edge);
			case ArcFilter.Forward:
				return arcs.Where<Arc>(delegate(Arc x)
				{
					RedirectedGraph.Direction direction = this.getDirection(x);
					if (direction != RedirectedGraph.Direction.Forward)
					{
						return direction != RedirectedGraph.Direction.Backward || this.V(x) == u;
					}
					return this.U(x) == u;
				});
			default:
				return arcs.Where<Arc>(delegate(Arc x)
				{
					RedirectedGraph.Direction direction2 = this.getDirection(x);
					if (direction2 != RedirectedGraph.Direction.Forward)
					{
						return direction2 != RedirectedGraph.Direction.Backward || this.U(x) == u;
					}
					return this.V(x) == u;
				});
			}
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.FilterArcs(u, this.graph.Arcs(u, ArcFilter.All), filter);
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return this.FilterArcs(u, this.graph.Arcs(u, v, ArcFilter.All), filter);
		}

		public int NodeCount()
		{
			return this.graph.NodeCount();
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return (filter != ArcFilter.All) ? this.Arcs(filter).Count<Arc>() : this.graph.ArcCount(ArcFilter.All);
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
			return this.graph.HasNode(node);
		}

		public bool HasArc(Arc arc)
		{
			return this.graph.HasArc(arc);
		}

		private IGraph graph;

		private Func<Arc, RedirectedGraph.Direction> getDirection;

		public enum Direction
		{
			Forward,
			Backward,
			Edge
		}
	}
}
