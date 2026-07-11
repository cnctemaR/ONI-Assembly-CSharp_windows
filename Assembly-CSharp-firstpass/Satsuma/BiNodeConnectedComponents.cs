using System;
using System.Collections.Generic;

namespace Satsuma
{
	public class BiNodeConnectedComponents
	{
		public BiNodeConnectedComponents(IGraph graph, BiNodeConnectedComponents.Flags flags = BiNodeConnectedComponents.Flags.None)
		{
			this.Graph = graph;
			if ((flags & BiNodeConnectedComponents.Flags.CreateComponents) != BiNodeConnectedComponents.Flags.None)
			{
				this.Components = new List<HashSet<Node>>();
			}
			if ((flags & BiNodeConnectedComponents.Flags.CreateCutvertices) != BiNodeConnectedComponents.Flags.None)
			{
				this.Cutvertices = new Dictionary<Node, int>();
			}
			new BiNodeConnectedComponents.BlockDfs
			{
				Parent = this
			}.Run(graph, null);
		}

		public IGraph Graph { get; private set; }

		public int Count { get; private set; }

		public List<HashSet<Node>> Components { get; private set; }

		public Dictionary<Node, int> Cutvertices { get; private set; }

		[Flags]
		public enum Flags
		{
			None = 0,
			CreateComponents = 1,
			CreateCutvertices = 2
		}

		private class BlockDfs : LowpointDfs
		{
			protected override void Start(out Dfs.Direction direction)
			{
				base.Start(out direction);
				if (this.Parent.Components != null)
				{
					this.blockStack = new Stack<Node>();
				}
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (!base.NodeEnter(node, arc))
				{
					return false;
				}
				if (this.Parent.Cutvertices != null && arc == Arc.Invalid)
				{
					this.Parent.Cutvertices[node] = -1;
				}
				if (this.Parent.Components != null)
				{
					this.blockStack.Push(node);
				}
				this.oneNodeComponent = arc == Arc.Invalid;
				return true;
			}

			protected override bool NodeExit(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					if (this.oneNodeComponent)
					{
						this.Parent.Count++;
						if (this.Parent.Components != null)
						{
							this.Parent.Components.Add(new HashSet<Node> { node });
						}
					}
					if (this.Parent.Cutvertices != null && this.Parent.Cutvertices[node] == 0)
					{
						this.Parent.Cutvertices.Remove(node);
					}
					if (this.Parent.Components != null)
					{
						this.blockStack.Clear();
					}
				}
				else
				{
					Node node2 = base.Graph.Other(arc, node);
					if (this.lowpoint[node] >= base.Level - 1)
					{
						if (this.Parent.Cutvertices != null)
						{
							int num;
							this.Parent.Cutvertices[node2] = ((!this.Parent.Cutvertices.TryGetValue(node2, out num)) ? 0 : num) + 1;
						}
						this.Parent.Count++;
						if (this.Parent.Components != null)
						{
							HashSet<Node> hashSet = new HashSet<Node>();
							Node node3;
							do
							{
								node3 = this.blockStack.Pop();
								hashSet.Add(node3);
							}
							while (!(node3 == node));
							hashSet.Add(node2);
							this.Parent.Components.Add(hashSet);
						}
					}
				}
				return base.NodeExit(node, arc);
			}

			public BiNodeConnectedComponents Parent;

			private Stack<Node> blockStack;

			private bool oneNodeComponent;
		}
	}
}
