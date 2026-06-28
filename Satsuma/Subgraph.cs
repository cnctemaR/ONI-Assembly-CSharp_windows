using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Subgraph : IGraph, IArcLookup
	{
		public Subgraph(IGraph graph)
		{
			this.graph = graph;
			this.EnableAllNodes(true);
			this.EnableAllArcs(true);
		}

		public void EnableAllNodes(bool enabled)
		{
			this.defaultNodeEnabled = enabled;
			this.nodeExceptions.Clear();
		}

		public void EnableAllArcs(bool enabled)
		{
			this.defaultArcEnabled = enabled;
			this.arcExceptions.Clear();
		}

		public void Enable(Node node, bool enabled)
		{
			bool flag = this.defaultNodeEnabled != enabled;
			if (flag)
			{
				this.nodeExceptions.Add(node);
				return;
			}
			this.nodeExceptions.Remove(node);
		}

		public void Enable(Arc arc, bool enabled)
		{
			bool flag = this.defaultArcEnabled != enabled;
			if (flag)
			{
				this.arcExceptions.Add(arc);
				return;
			}
			this.arcExceptions.Remove(arc);
		}

		public bool IsEnabled(Node node)
		{
			return this.defaultNodeEnabled ^ this.nodeExceptions.Contains(node);
		}

		public bool IsEnabled(Arc arc)
		{
			return this.defaultArcEnabled ^ this.arcExceptions.Contains(arc);
		}

		public Node U(Arc arc)
		{
			return this.graph.U(arc);
		}

		public Node V(Arc arc)
		{
			return this.graph.V(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return this.graph.IsEdge(arc);
		}

		private IEnumerable<Node> NodesInternal()
		{
			foreach (Node node in this.graph.Nodes())
			{
				if (this.IsEnabled(node))
				{
					yield return node;
				}
			}
			yield break;
		}

		public IEnumerable<Node> Nodes()
		{
			if (this.nodeExceptions.Count != 0)
			{
				return this.NodesInternal();
			}
			if (this.defaultNodeEnabled)
			{
				return this.graph.Nodes();
			}
			return Enumerable.Empty<Node>();
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			foreach (Arc arc in this.graph.Arcs(filter))
			{
				if (this.IsEnabled(arc) && this.IsEnabled(this.graph.U(arc)) && this.IsEnabled(this.graph.V(arc)))
				{
					yield return arc;
				}
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (this.IsEnabled(u))
			{
				foreach (Arc arc in this.graph.Arcs(u, filter))
				{
					if (this.IsEnabled(arc) && this.IsEnabled(this.graph.Other(arc, u)))
					{
						yield return arc;
					}
				}
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (this.IsEnabled(u) && this.IsEnabled(v))
			{
				foreach (Arc arc in this.graph.Arcs(u, v, filter))
				{
					if (this.IsEnabled(arc))
					{
						yield return arc;
					}
				}
			}
			yield break;
		}

		public int NodeCount()
		{
			if (!this.defaultNodeEnabled)
			{
				return this.nodeExceptions.Count;
			}
			return this.graph.NodeCount() - this.nodeExceptions.Count;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (this.nodeExceptions.Count != 0 || filter != ArcFilter.All)
			{
				return this.Arcs(filter).Count<Arc>();
			}
			if (!this.defaultNodeEnabled)
			{
				return 0;
			}
			if (!this.defaultArcEnabled)
			{
				return this.arcExceptions.Count;
			}
			return this.graph.ArcCount(ArcFilter.All) - this.arcExceptions.Count;
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
			return this.graph.HasNode(node) && this.IsEnabled(node);
		}

		public bool HasArc(Arc arc)
		{
			return this.graph.HasArc(arc) && this.IsEnabled(arc);
		}

		private IGraph graph;

		private bool defaultNodeEnabled;

		private HashSet<Node> nodeExceptions = new HashSet<Node>();

		private bool defaultArcEnabled;

		private HashSet<Arc> arcExceptions = new HashSet<Arc>();
	}
}
