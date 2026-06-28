using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class TopologicalOrder
	{
		public IGraph Graph { get; private set; }

		public bool Acyclic { get; private set; }

		public List<Node> Order { get; private set; }

		public TopologicalOrder(IGraph graph, TopologicalOrder.Flags flags = TopologicalOrder.Flags.None)
		{
			this.Graph = graph;
			if ((flags & TopologicalOrder.Flags.CreateOrder) != TopologicalOrder.Flags.None)
			{
				this.Order = new List<Node>();
			}
			new TopologicalOrder.MyDfs
			{
				Parent = this
			}.Run(graph, null);
		}

		[Flags]
		public enum Flags
		{
			None = 0,
			CreateOrder = 1
		}

		private class MyDfs : Dfs
		{
			protected override void Start(out Dfs.Direction direction)
			{
				direction = Dfs.Direction.Forward;
				this.Parent.Acyclic = true;
				this.exited = new HashSet<Node>();
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc != Arc.Invalid && base.Graph.IsEdge(arc))
				{
					this.Parent.Acyclic = false;
					return false;
				}
				return true;
			}

			protected override bool NodeExit(Node node, Arc arc)
			{
				if (this.Parent.Order != null)
				{
					this.Parent.Order.Add(node);
				}
				this.exited.Add(node);
				return true;
			}

			protected override bool BackArc(Node node, Arc arc)
			{
				Node node2 = base.Graph.Other(arc, node);
				if (!this.exited.Contains(node2))
				{
					this.Parent.Acyclic = false;
					return false;
				}
				return true;
			}

			protected override void StopSearch()
			{
				if (this.Parent.Order != null)
				{
					if (this.Parent.Acyclic)
					{
						this.Parent.Order.Reverse();
						return;
					}
					this.Parent.Order.Clear();
				}
			}

			public TopologicalOrder Parent;

			private HashSet<Node> exited;
		}
	}
}
