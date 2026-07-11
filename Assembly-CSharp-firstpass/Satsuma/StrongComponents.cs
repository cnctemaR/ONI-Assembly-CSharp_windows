using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class StrongComponents
	{
		public StrongComponents(IGraph graph, StrongComponents.Flags flags = StrongComponents.Flags.None)
		{
			this.Graph = graph;
			if ((flags & StrongComponents.Flags.CreateComponents) != StrongComponents.Flags.None)
			{
				this.Components = new List<HashSet<Node>>();
			}
			StrongComponents.ForwardDfs forwardDfs = new StrongComponents.ForwardDfs();
			forwardDfs.Run(graph, null);
			StrongComponents.BackwardDfs backwardDfs = new StrongComponents.BackwardDfs
			{
				Parent = this
			};
			backwardDfs.Run(graph, forwardDfs.ReverseExitOrder);
		}

		public IGraph Graph { get; private set; }

		public int Count { get; private set; }

		public List<HashSet<Node>> Components { get; private set; }

		[Flags]
		public enum Flags
		{
			None = 0,
			CreateComponents = 1
		}

		private class ForwardDfs : Dfs
		{
			protected override void Start(out Dfs.Direction direction)
			{
				direction = Dfs.Direction.Forward;
				this.ReverseExitOrder = new List<Node>();
			}

			protected override bool NodeExit(Node node, Arc arc)
			{
				this.ReverseExitOrder.Add(node);
				return true;
			}

			protected override void StopSearch()
			{
				this.ReverseExitOrder.Reverse();
			}

			public List<Node> ReverseExitOrder;
		}

		private class BackwardDfs : Dfs
		{
			protected override void Start(out Dfs.Direction direction)
			{
				direction = Dfs.Direction.Backward;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					this.Parent.Count++;
					if (this.Parent.Components != null)
					{
						this.Parent.Components.Add(new HashSet<Node> { node });
					}
				}
				else if (this.Parent.Components != null)
				{
					this.Parent.Components[this.Parent.Components.Count - 1].Add(node);
				}
				return true;
			}

			public StrongComponents Parent;
		}
	}
}
