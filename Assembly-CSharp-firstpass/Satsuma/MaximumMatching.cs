using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class MaximumMatching : IClearable
	{
		public MaximumMatching(IGraph graph, Func<Node, bool> isRed)
		{
			this.Graph = graph;
			this.IsRed = isRed;
			this.matching = new Matching(this.Graph);
			this.unmatchedRedNodes = new HashSet<Node>();
			this.Clear();
		}

		public IGraph Graph { get; private set; }

		public Func<Node, bool> IsRed { get; private set; }

		public IMatching Matching
		{
			get
			{
				return this.matching;
			}
		}

		public void Clear()
		{
			this.matching.Clear();
			this.unmatchedRedNodes.Clear();
			foreach (Node node in this.Graph.Nodes())
			{
				if (this.IsRed(node))
				{
					this.unmatchedRedNodes.Add(node);
				}
			}
		}

		public int GreedyGrow(int maxImprovements = 2147483647)
		{
			int num = 0;
			List<Node> list = new List<Node>();
			foreach (Node node in this.unmatchedRedNodes)
			{
				foreach (Arc arc in this.Graph.Arcs(node, ArcFilter.All))
				{
					Node node2 = this.Graph.Other(arc, node);
					if (!this.matching.HasNode(node2))
					{
						this.matching.Enable(arc, true);
						list.Add(node);
						num++;
						if (num >= maxImprovements)
						{
							goto IL_00CE;
						}
						break;
					}
				}
			}
			IL_00CE:
			foreach (Node node3 in list)
			{
				this.unmatchedRedNodes.Remove(node3);
			}
			return num;
		}

		public void Add(Arc arc)
		{
			if (this.matching.HasArc(arc))
			{
				return;
			}
			this.matching.Enable(arc, true);
			Node node = this.Graph.U(arc);
			this.unmatchedRedNodes.Remove((!this.IsRed(node)) ? this.Graph.V(arc) : node);
		}

		private Node Traverse(Node node)
		{
			Arc arc = this.matching.MatchedArc(node);
			if (this.IsRed(node))
			{
				foreach (Arc arc2 in this.Graph.Arcs(node, ArcFilter.All))
				{
					if (arc2 != arc)
					{
						Node node2 = this.Graph.Other(arc2, node);
						if (!this.parentArc.ContainsKey(node2))
						{
							this.parentArc[node2] = arc2;
							if (!this.matching.HasNode(node2))
							{
								return node2;
							}
							Node node3 = this.Traverse(node2);
							if (node3 != Node.Invalid)
							{
								return node3;
							}
						}
					}
				}
			}
			else
			{
				Node node4 = this.Graph.Other(arc, node);
				if (!this.parentArc.ContainsKey(node4))
				{
					this.parentArc[node4] = arc;
					Node node5 = this.Traverse(node4);
					if (node5 != Node.Invalid)
					{
						return node5;
					}
				}
			}
			return Node.Invalid;
		}

		public void Run()
		{
			List<Node> list = new List<Node>();
			this.parentArc = new Dictionary<Node, Arc>();
			foreach (Node node in this.unmatchedRedNodes)
			{
				this.parentArc.Clear();
				this.parentArc[node] = Arc.Invalid;
				Node node2 = this.Traverse(node);
				if (!(node2 == Node.Invalid))
				{
					for (;;)
					{
						Arc arc = this.parentArc[node2];
						Node node3 = this.Graph.Other(arc, node2);
						Arc arc2 = ((!(node3 == node)) ? this.parentArc[node3] : Arc.Invalid);
						if (arc2 != Arc.Invalid)
						{
							this.matching.Enable(arc2, false);
						}
						this.matching.Enable(arc, true);
						if (arc2 == Arc.Invalid)
						{
							break;
						}
						node2 = this.Graph.Other(arc2, node3);
					}
					list.Add(node);
				}
			}
			this.parentArc = null;
			foreach (Node node4 in list)
			{
				this.unmatchedRedNodes.Remove(node4);
			}
		}

		private readonly Matching matching;

		private readonly HashSet<Node> unmatchedRedNodes;

		private Dictionary<Node, Arc> parentArc;
	}
}
