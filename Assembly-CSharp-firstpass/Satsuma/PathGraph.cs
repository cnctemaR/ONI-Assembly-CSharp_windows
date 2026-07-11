using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class PathGraph : IPath, IGraph, IArcLookup
	{
		public Node FirstNode
		{
			get
			{
				if (this.nodeCount <= 0)
				{
					return Node.Invalid;
				}
				return new Node(1L);
			}
		}

		public Node LastNode
		{
			get
			{
				if (this.nodeCount <= 0)
				{
					return Node.Invalid;
				}
				return new Node((long)(this.isCycle ? 1 : this.nodeCount));
			}
		}

		public PathGraph(int nodeCount, PathGraph.Topology topology, Directedness directedness)
		{
			this.nodeCount = nodeCount;
			this.isCycle = topology == PathGraph.Topology.Cycle;
			this.directed = directedness == Directedness.Directed;
		}

		public Node GetNode(int index)
		{
			return new Node(1L + (long)index);
		}

		public int GetNodeIndex(Node node)
		{
			return (int)(node.Id - 1L);
		}

		public Arc NextArc(Node node)
		{
			if (!this.isCycle && node.Id == (long)this.nodeCount)
			{
				return Arc.Invalid;
			}
			return new Arc(node.Id);
		}

		public Arc PrevArc(Node node)
		{
			if (node.Id != 1L)
			{
				return new Arc(node.Id - 1L);
			}
			if (!this.isCycle)
			{
				return Arc.Invalid;
			}
			return new Arc((long)this.nodeCount);
		}

		public Node U(Arc arc)
		{
			return new Node(arc.Id);
		}

		public Node V(Arc arc)
		{
			return new Node((arc.Id == (long)this.nodeCount) ? 1L : (arc.Id + 1L));
		}

		public bool IsEdge(Arc arc)
		{
			return !this.directed;
		}

		public IEnumerable<Node> Nodes()
		{
			int num;
			for (int i = 1; i <= this.nodeCount; i = num + 1)
			{
				yield return new Node((long)i);
				num = i;
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (this.directed && filter == ArcFilter.Edge)
			{
				yield break;
			}
			int i = 1;
			int j = this.ArcCountInternal();
			while (i <= j)
			{
				yield return new Arc((long)i);
				int num = i;
				i = num + 1;
			}
			yield break;
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

		private int ArcCountInternal()
		{
			if (this.nodeCount == 0)
			{
				return 0;
			}
			if (!this.isCycle)
			{
				return this.nodeCount - 1;
			}
			return this.nodeCount;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (!this.directed || filter != ArcFilter.Edge)
			{
				return this.ArcCountInternal();
			}
			return 0;
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
			return node.Id >= 1L && node.Id <= (long)this.nodeCount;
		}

		public bool HasArc(Arc arc)
		{
			return arc.Id >= 1L && arc.Id <= (long)this.ArcCountInternal();
		}

		private readonly int nodeCount;

		private readonly bool isCycle;

		private readonly bool directed;

		public enum Topology
		{
			Path,
			Cycle
		}
	}
}
