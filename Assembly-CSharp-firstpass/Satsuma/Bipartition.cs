using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class Bipartition
	{
		public Bipartition(IGraph graph, Bipartition.Flags flags = Bipartition.Flags.None)
		{
			this.Graph = graph;
			if ((flags & Bipartition.Flags.CreateRedNodes) != Bipartition.Flags.None)
			{
				this.RedNodes = new HashSet<Node>();
			}
			if ((flags & Bipartition.Flags.CreateBlueNodes) != Bipartition.Flags.None)
			{
				this.BlueNodes = new HashSet<Node>();
			}
			new Bipartition.MyDfs
			{
				Parent = this
			}.Run(graph, null);
		}

		public IGraph Graph { get; private set; }

		public bool Bipartite { get; private set; }

		public HashSet<Node> RedNodes { get; private set; }

		public HashSet<Node> BlueNodes { get; private set; }

		[Flags]
		public enum Flags
		{
			None = 0,
			CreateRedNodes = 1,
			CreateBlueNodes = 2
		}

		private class MyDfs : Dfs
		{
			protected override void Start(out Dfs.Direction direction)
			{
				direction = Dfs.Direction.Undirected;
				this.Parent.Bipartite = true;
				this.redNodes = this.Parent.RedNodes ?? new HashSet<Node>();
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if ((base.Level & 1) == 0)
				{
					this.redNodes.Add(node);
				}
				else if (this.Parent.BlueNodes != null)
				{
					this.Parent.BlueNodes.Add(node);
				}
				return true;
			}

			protected override bool BackArc(Node node, Arc arc)
			{
				Node node2 = base.Graph.Other(arc, node);
				if (this.redNodes.Contains(node) == this.redNodes.Contains(node2))
				{
					this.Parent.Bipartite = false;
					if (this.Parent.RedNodes != null)
					{
						this.Parent.RedNodes.Clear();
					}
					if (this.Parent.BlueNodes != null)
					{
						this.Parent.BlueNodes.Clear();
					}
					return false;
				}
				return true;
			}

			public Bipartition Parent;

			private HashSet<Node> redNodes;
		}
	}
}
