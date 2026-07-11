using System;
using System.Collections.Generic;

namespace Satsuma
{
	public abstract class Dfs
	{
		private protected IGraph Graph { protected get; private set; }

		private protected int Level { protected get; private set; }

		public void Run(IGraph graph, IEnumerable<Node> roots = null)
		{
			this.Graph = graph;
			Dfs.Direction direction;
			this.Start(out direction);
			if (direction != Dfs.Direction.Undirected)
			{
				if (direction != Dfs.Direction.Forward)
				{
					this.arcFilter = ArcFilter.Backward;
				}
				else
				{
					this.arcFilter = ArcFilter.Forward;
				}
			}
			else
			{
				this.arcFilter = ArcFilter.All;
			}
			this.traversed = new HashSet<Node>();
			foreach (Node node in (roots ?? this.Graph.Nodes()))
			{
				if (!this.traversed.Contains(node))
				{
					this.Level = 0;
					if (!this.Traverse(node, Arc.Invalid))
					{
						break;
					}
				}
			}
			this.traversed = null;
			this.StopSearch();
		}

		private bool Traverse(Node node, Arc arc)
		{
			this.traversed.Add(node);
			if (!this.NodeEnter(node, arc))
			{
				return false;
			}
			foreach (Arc arc2 in this.Graph.Arcs(node, this.arcFilter))
			{
				if (!(arc2 == arc))
				{
					Node node2 = this.Graph.Other(arc2, node);
					if (this.traversed.Contains(node2))
					{
						if (!this.BackArc(node, arc2))
						{
							return false;
						}
					}
					else
					{
						this.Level++;
						if (!this.Traverse(node2, arc2))
						{
							return false;
						}
						this.Level--;
					}
				}
			}
			return this.NodeExit(node, arc);
		}

		protected abstract void Start(out Dfs.Direction direction);

		protected virtual bool NodeEnter(Node node, Arc arc)
		{
			return true;
		}

		protected virtual bool NodeExit(Node node, Arc arc)
		{
			return true;
		}

		protected virtual bool BackArc(Node node, Arc arc)
		{
			return true;
		}

		protected virtual void StopSearch()
		{
		}

		private HashSet<Node> traversed;

		private ArcFilter arcFilter;

		public enum Direction
		{
			Undirected,
			Forward,
			Backward
		}
	}
}
