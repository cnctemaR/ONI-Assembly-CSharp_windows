using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Path : IPath, IGraph, IArcLookup, IClearable
	{
		public IGraph Graph { get; private set; }

		public Node FirstNode { get; private set; }

		public Node LastNode { get; private set; }

		public Path(IGraph graph)
		{
			this.Graph = graph;
			this.nextArc = new Dictionary<Node, Arc>();
			this.prevArc = new Dictionary<Node, Arc>();
			this.arcs = new HashSet<Arc>();
			this.Clear();
		}

		public void Clear()
		{
			this.FirstNode = Node.Invalid;
			this.LastNode = Node.Invalid;
			this.nodeCount = 0;
			this.nextArc.Clear();
			this.prevArc.Clear();
			this.arcs.Clear();
			this.edgeCount = 0;
		}

		public void Begin(Node node)
		{
			if (this.nodeCount > 0)
			{
				throw new InvalidOperationException("Path not empty.");
			}
			this.nodeCount = 1;
			this.LastNode = node;
			this.FirstNode = node;
		}

		public void AddFirst(Arc arc)
		{
			Node node = this.U(arc);
			Node node2 = this.V(arc);
			Node node3 = ((node == this.FirstNode) ? node2 : node);
			if ((node != this.FirstNode && node2 != this.FirstNode) || this.nextArc.ContainsKey(node3) || this.prevArc.ContainsKey(this.FirstNode))
			{
				throw new ArgumentException("Arc not valid or path is a cycle.");
			}
			if (node3 != this.LastNode)
			{
				this.nodeCount++;
			}
			this.nextArc[node3] = arc;
			this.prevArc[this.FirstNode] = arc;
			if (!this.arcs.Contains(arc))
			{
				this.arcs.Add(arc);
				if (this.IsEdge(arc))
				{
					this.edgeCount++;
				}
			}
			this.FirstNode = node3;
		}

		public void AddLast(Arc arc)
		{
			Node node = this.U(arc);
			Node node2 = this.V(arc);
			Node node3 = ((node == this.LastNode) ? node2 : node);
			if ((node != this.LastNode && node2 != this.LastNode) || this.nextArc.ContainsKey(this.LastNode) || this.prevArc.ContainsKey(node3))
			{
				throw new ArgumentException("Arc not valid or path is a cycle.");
			}
			if (node3 != this.FirstNode)
			{
				this.nodeCount++;
			}
			this.nextArc[this.LastNode] = arc;
			this.prevArc[node3] = arc;
			if (!this.arcs.Contains(arc))
			{
				this.arcs.Add(arc);
				if (this.IsEdge(arc))
				{
					this.edgeCount++;
				}
			}
			this.LastNode = node3;
		}

		public void Reverse()
		{
			Node firstNode = this.FirstNode;
			this.FirstNode = this.LastNode;
			this.LastNode = firstNode;
			Dictionary<Node, Arc> dictionary = this.nextArc;
			this.nextArc = this.prevArc;
			this.prevArc = dictionary;
		}

		public Arc NextArc(Node node)
		{
			Arc arc;
			if (!this.nextArc.TryGetValue(node, out arc))
			{
				return Arc.Invalid;
			}
			return arc;
		}

		public Arc PrevArc(Node node)
		{
			Arc arc;
			if (!this.prevArc.TryGetValue(node, out arc))
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
			Node i = this.FirstNode;
			if (!(i == Node.Invalid))
			{
				do
				{
					yield return i;
					Arc arc = this.NextArc(i);
					if (arc == Arc.Invalid)
					{
						break;
					}
					i = this.Graph.Other(arc, i);
				}
				while (!(i == this.FirstNode));
			}
			yield break;
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

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.ArcsHelper(u, filter);
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return from arc in this.Arcs(u, filter)
				where this.Other(arc, u) == v
				select arc;
		}

		public int NodeCount()
		{
			return this.nodeCount;
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
			return this.Arcs(u, filter).Count<Arc>();
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return this.Arcs(u, v, filter).Count<Arc>();
		}

		public bool HasNode(Node node)
		{
			return this.prevArc.ContainsKey(node) || (node != Node.Invalid && node == this.FirstNode);
		}

		public bool HasArc(Arc arc)
		{
			return this.arcs.Contains(arc);
		}

		private int nodeCount;

		private Dictionary<Node, Arc> nextArc;

		private Dictionary<Node, Arc> prevArc;

		private HashSet<Arc> arcs;

		private int edgeCount;
	}
}
