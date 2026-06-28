using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public class Supergraph : IBuildableGraph, IDestroyableGraph, IClearable, IGraph, IArcLookup
	{
		public Supergraph(IGraph graph)
		{
			this.graph = graph;
			this.nodeAllocator = new Supergraph.NodeAllocator
			{
				Parent = this
			};
			this.arcAllocator = new Supergraph.ArcAllocator
			{
				Parent = this
			};
			this.nodes = new HashSet<Node>();
			this.arcs = new HashSet<Arc>();
			this.arcProperties = new Dictionary<Arc, Supergraph.ArcProperties>();
			this.edges = new HashSet<Arc>();
			this.nodeArcs_All = new Dictionary<Node, List<Arc>>();
			this.nodeArcs_Edge = new Dictionary<Node, List<Arc>>();
			this.nodeArcs_Forward = new Dictionary<Node, List<Arc>>();
			this.nodeArcs_Backward = new Dictionary<Node, List<Arc>>();
		}

		public void Clear()
		{
			this.nodeAllocator.Rewind();
			this.arcAllocator.Rewind();
			this.nodes.Clear();
			this.arcs.Clear();
			this.arcProperties.Clear();
			this.edges.Clear();
			this.nodeArcs_All.Clear();
			this.nodeArcs_Edge.Clear();
			this.nodeArcs_Forward.Clear();
			this.nodeArcs_Backward.Clear();
		}

		public Node AddNode()
		{
			if (this.NodeCount() == 2147483647)
			{
				throw new InvalidOperationException("Error: too many nodes!");
			}
			Node node = new Node(this.nodeAllocator.Allocate());
			this.nodes.Add(node);
			return node;
		}

		public Arc AddArc(Node u, Node v, Directedness directedness)
		{
			if (this.ArcCount(ArcFilter.All) == 2147483647)
			{
				throw new InvalidOperationException("Error: too many arcs!");
			}
			Arc arc = new Arc(this.arcAllocator.Allocate());
			this.arcs.Add(arc);
			bool flag = directedness == Directedness.Undirected;
			this.arcProperties[arc] = new Supergraph.ArcProperties(u, v, flag);
			Utils.MakeEntry<Node, List<Arc>>(this.nodeArcs_All, u).Add(arc);
			Utils.MakeEntry<Node, List<Arc>>(this.nodeArcs_Forward, u).Add(arc);
			Utils.MakeEntry<Node, List<Arc>>(this.nodeArcs_Backward, v).Add(arc);
			if (flag)
			{
				this.edges.Add(arc);
				Utils.MakeEntry<Node, List<Arc>>(this.nodeArcs_Edge, u).Add(arc);
			}
			if (v != u)
			{
				Utils.MakeEntry<Node, List<Arc>>(this.nodeArcs_All, v).Add(arc);
				if (flag)
				{
					Utils.MakeEntry<Node, List<Arc>>(this.nodeArcs_Edge, v).Add(arc);
					Utils.MakeEntry<Node, List<Arc>>(this.nodeArcs_Forward, v).Add(arc);
					Utils.MakeEntry<Node, List<Arc>>(this.nodeArcs_Backward, u).Add(arc);
				}
			}
			return arc;
		}

		public bool DeleteNode(Node node)
		{
			if (!this.nodes.Remove(node))
			{
				return false;
			}
			Func<Arc, bool> func = (Arc a) => this.U(a) == node || this.V(a) == node;
			Utils.RemoveAll<Arc>(this.arcs, func);
			Utils.RemoveAll<Arc>(this.edges, func);
			Utils.RemoveAll<Arc, Supergraph.ArcProperties>(this.arcProperties, func);
			this.nodeArcs_All.Remove(node);
			this.nodeArcs_Edge.Remove(node);
			this.nodeArcs_Forward.Remove(node);
			this.nodeArcs_Backward.Remove(node);
			return true;
		}

		public bool DeleteArc(Arc arc)
		{
			if (!this.arcs.Remove(arc))
			{
				return false;
			}
			Supergraph.ArcProperties arcProperties = this.arcProperties[arc];
			this.arcProperties.Remove(arc);
			Utils.RemoveLast<Arc>(this.nodeArcs_All[arcProperties.U], arc);
			Utils.RemoveLast<Arc>(this.nodeArcs_Forward[arcProperties.U], arc);
			Utils.RemoveLast<Arc>(this.nodeArcs_Backward[arcProperties.V], arc);
			if (arcProperties.IsEdge)
			{
				this.edges.Remove(arc);
				Utils.RemoveLast<Arc>(this.nodeArcs_Edge[arcProperties.U], arc);
			}
			if (arcProperties.V != arcProperties.U)
			{
				Utils.RemoveLast<Arc>(this.nodeArcs_All[arcProperties.V], arc);
				if (arcProperties.IsEdge)
				{
					Utils.RemoveLast<Arc>(this.nodeArcs_Edge[arcProperties.V], arc);
					Utils.RemoveLast<Arc>(this.nodeArcs_Forward[arcProperties.V], arc);
					Utils.RemoveLast<Arc>(this.nodeArcs_Backward[arcProperties.U], arc);
				}
			}
			return true;
		}

		public Node U(Arc arc)
		{
			Supergraph.ArcProperties arcProperties;
			if (this.arcProperties.TryGetValue(arc, out arcProperties))
			{
				return arcProperties.U;
			}
			return this.graph.U(arc);
		}

		public Node V(Arc arc)
		{
			Supergraph.ArcProperties arcProperties;
			if (this.arcProperties.TryGetValue(arc, out arcProperties))
			{
				return arcProperties.V;
			}
			return this.graph.V(arc);
		}

		public bool IsEdge(Arc arc)
		{
			Supergraph.ArcProperties arcProperties;
			if (this.arcProperties.TryGetValue(arc, out arcProperties))
			{
				return arcProperties.IsEdge;
			}
			return this.graph.IsEdge(arc);
		}

		private HashSet<Arc> ArcsInternal(ArcFilter filter)
		{
			if (filter != ArcFilter.All)
			{
				return this.edges;
			}
			return this.arcs;
		}

		private List<Arc> ArcsInternal(Node v, ArcFilter filter)
		{
			List<Arc> list;
			switch (filter)
			{
			case ArcFilter.All:
				this.nodeArcs_All.TryGetValue(v, out list);
				break;
			case ArcFilter.Edge:
				this.nodeArcs_Edge.TryGetValue(v, out list);
				break;
			case ArcFilter.Forward:
				this.nodeArcs_Forward.TryGetValue(v, out list);
				break;
			default:
				this.nodeArcs_Backward.TryGetValue(v, out list);
				break;
			}
			return list ?? Supergraph.EmptyArcList;
		}

		public IEnumerable<Node> Nodes()
		{
			if (this.graph != null)
			{
				return this.nodes.Concat<Node>(this.graph.Nodes());
			}
			return this.nodes;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (this.graph != null)
			{
				return this.ArcsInternal(filter).Concat<Arc>(this.graph.Arcs(filter));
			}
			return this.ArcsInternal(filter);
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (this.graph == null || this.nodes.Contains(u))
			{
				return this.ArcsInternal(u, filter);
			}
			return this.ArcsInternal(u, filter).Concat<Arc>(this.graph.Arcs(u, filter));
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			foreach (Arc arc in this.ArcsInternal(u, filter))
			{
				if (this.Other(arc, u) == v)
				{
					yield return arc;
				}
			}
			if (this.graph != null && !this.nodes.Contains(u) && !this.nodes.Contains(v))
			{
				foreach (Arc arc2 in this.graph.Arcs(u, v, filter))
				{
					yield return arc2;
				}
			}
			yield break;
		}

		public int NodeCount()
		{
			return this.nodes.Count + ((this.graph == null) ? 0 : this.graph.NodeCount());
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return this.ArcsInternal(filter).Count + ((this.graph == null) ? 0 : this.graph.ArcCount(filter));
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.ArcsInternal(u, filter).Count + ((this.graph == null || this.nodes.Contains(u)) ? 0 : this.graph.ArcCount(u, filter));
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			int num = 0;
			foreach (Arc arc in this.ArcsInternal(u, filter))
			{
				if (this.Other(arc, u) == v)
				{
					num++;
				}
			}
			return num + ((this.graph == null || this.nodes.Contains(u) || this.nodes.Contains(v)) ? 0 : this.graph.ArcCount(u, v, filter));
		}

		public bool HasNode(Node node)
		{
			return this.nodes.Contains(node) || (this.graph != null && this.graph.HasNode(node));
		}

		public bool HasArc(Arc arc)
		{
			return this.arcs.Contains(arc) || (this.graph != null && this.graph.HasArc(arc));
		}

		private IGraph graph;

		private Supergraph.NodeAllocator nodeAllocator;

		private Supergraph.ArcAllocator arcAllocator;

		private HashSet<Node> nodes;

		private HashSet<Arc> arcs;

		private Dictionary<Arc, Supergraph.ArcProperties> arcProperties;

		private HashSet<Arc> edges;

		private Dictionary<Node, List<Arc>> nodeArcs_All;

		private Dictionary<Node, List<Arc>> nodeArcs_Edge;

		private Dictionary<Node, List<Arc>> nodeArcs_Forward;

		private Dictionary<Node, List<Arc>> nodeArcs_Backward;

		private static readonly List<Arc> EmptyArcList = new List<Arc>();

		private class NodeAllocator : IdAllocator
		{
			protected override bool IsAllocated(long id)
			{
				return this.Parent.HasNode(new Node(id));
			}

			public Supergraph Parent;
		}

		private class ArcAllocator : IdAllocator
		{
			protected override bool IsAllocated(long id)
			{
				return this.Parent.HasArc(new Arc(id));
			}

			public Supergraph Parent;
		}

		private class ArcProperties
		{
			public Node U { get; private set; }

			public Node V { get; private set; }

			public bool IsEdge { get; private set; }

			public ArcProperties(Node u, Node v, bool isEdge)
			{
				this.U = u;
				this.V = v;
				this.IsEdge = isEdge;
			}
		}
	}
}
