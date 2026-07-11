using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class ConnectedComponents
	{
		public IGraph Graph { get; private set; }

		public int Count { get; private set; }

		public List<HashSet<Node>> Components { get; private set; }

		public ConnectedComponents(IGraph graph, ConnectedComponents.Flags flags = ConnectedComponents.Flags.None)
		{
			this.Graph = graph;
			if ((flags & ConnectedComponents.Flags.CreateComponents) != ConnectedComponents.Flags.None)
			{
				this.Components = new List<HashSet<Node>>();
			}
			new ConnectedComponents.MyDfs
			{
				Parent = this
			}.Run(graph, null);
		}

		[Flags]
		public enum Flags
		{
			None = 0,
			CreateComponents = 1
		}

		private class MyDfs : Dfs
		{
			protected override void Start(out Dfs.Direction direction)
			{
				direction = Dfs.Direction.Undirected;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					ConnectedComponents parent = this.Parent;
					int count = parent.Count;
					parent.Count = count + 1;
					if (this.Parent.Components != null)
					{
						this.Parent.Components.Add(new HashSet<Node> { node });
					}
				}
				else if (this.Parent.Components != null)
				{
					this.Parent.Components[this.Parent.Count - 1].Add(node);
				}
				return true;
			}

			public ConnectedComponents Parent;
		}
	}
}
