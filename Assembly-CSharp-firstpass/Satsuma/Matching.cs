using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Matching : IMatching, IGraph, IArcLookup, IClearable
	{
		public IGraph Graph { get; private set; }

		public Matching(IGraph graph)
		{
			this.Graph = graph;
			this.matchedArc = new Dictionary<Node, Arc>();
			this.arcs = new HashSet<Arc>();
			this.Clear();
		}

		public void Clear()
		{
			this.matchedArc.Clear();
			this.arcs.Clear();
			this.edgeCount = 0;
		}

		public void Enable(Arc arc, bool enabled)
		{
			if (enabled == this.arcs.Contains(arc))
			{
				return;
			}
			Node node = this.Graph.U(arc);
			Node node2 = this.Graph.V(arc);
			if (enabled)
			{
				if (node == node2)
				{
					throw new ArgumentException("Matchings cannot have loop arcs.");
				}
				if (this.matchedArc.ContainsKey(node))
				{
					string text = "Node is already matched: ";
					Node node3 = node;
					throw new ArgumentException(text + node3.ToString());
				}
				if (this.matchedArc.ContainsKey(node2))
				{
					string text2 = "Node is already matched: ";
					Node node3 = node2;
					throw new ArgumentException(text2 + node3.ToString());
				}
				this.matchedArc[node] = arc;
				this.matchedArc[node2] = arc;
				this.arcs.Add(arc);
				if (this.Graph.IsEdge(arc))
				{
					this.edgeCount++;
					return;
				}
			}
			else
			{
				this.matchedArc.Remove(node);
				this.matchedArc.Remove(node2);
				this.arcs.Remove(arc);
				if (this.Graph.IsEdge(arc))
				{
					this.edgeCount--;
				}
			}
		}

		public Arc MatchedArc(Node node)
		{
			Arc arc;
			if (!this.matchedArc.TryGetValue(node, out arc))
			{
				return Arc.Invalid;
			}
			return arc;
		}

		public Node U(Arc arc)
		{
			return this.Graph.U(arc);
		}

		public Node V(Arc arc)
		{
			return this.Graph.V(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return this.Graph.IsEdge(arc);
		}

		public IEnumerable<Node> Nodes()
		{
			return this.matchedArc.Keys;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (filter == ArcFilter.All)
			{
				return this.arcs;
			}
			if (this.edgeCount == 0)
			{
				return Enumerable.Empty<Arc>();
			}
			return this.arcs.Where<Arc>((Arc arc) => this.IsEdge(arc));
		}

		private bool YieldArc(Node u, ArcFilter filter, Arc arc)
		{
			return filter == ArcFilter.All || this.IsEdge(arc) || (filter == ArcFilter.Forward && this.U(arc) == u) || (filter == ArcFilter.Backward && this.V(arc) == u);
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			Arc arc = this.MatchedArc(u);
			if (arc != Arc.Invalid && this.YieldArc(u, filter, arc))
			{
				yield return arc;
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (u != v)
			{
				Arc arc = this.MatchedArc(u);
				if (arc != Arc.Invalid && arc == this.MatchedArc(v) && this.YieldArc(u, filter, arc))
				{
					yield return arc;
				}
			}
			yield break;
		}

		public int NodeCount()
		{
			return this.matchedArc.Count;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (filter != ArcFilter.All)
			{
				return this.edgeCount;
			}
			return this.arcs.Count;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			Arc arc = this.MatchedArc(u);
			if (!(arc != Arc.Invalid) || !this.YieldArc(u, filter, arc))
			{
				return 0;
			}
			return 1;
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (!(u != v))
			{
				return 0;
			}
			Arc arc = this.MatchedArc(u);
			if (!(arc != Arc.Invalid) || !(arc == this.MatchedArc(v)) || !this.YieldArc(u, filter, arc))
			{
				return 0;
			}
			return 1;
		}

		public bool HasNode(Node node)
		{
			return this.Graph.HasNode(node) && this.matchedArc.ContainsKey(node);
		}

		public bool HasArc(Arc arc)
		{
			return this.Graph.HasArc(arc) && this.arcs.Contains(arc);
		}

		private readonly Dictionary<Node, Arc> matchedArc;

		private readonly HashSet<Arc> arcs;

		private int edgeCount;
	}
}
