using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Bfs
	{
		public Bfs(IGraph graph)
		{
			this.Graph = graph;
			this.parentArc = new Dictionary<Node, Arc>();
			this.level = new Dictionary<Node, int>();
			this.queue = new Queue<Node>();
		}

		public IGraph Graph { get; private set; }

		public void AddSource(Node node)
		{
			if (this.Reached(node))
			{
				return;
			}
			this.parentArc[node] = Arc.Invalid;
			this.level[node] = 0;
			this.queue.Enqueue(node);
		}

		public bool Step(Func<Node, bool> isTarget, out Node reachedTargetNode)
		{
			reachedTargetNode = Node.Invalid;
			if (this.queue.Count == 0)
			{
				return false;
			}
			Node node = this.queue.Dequeue();
			int num = this.level[node] + 1;
			foreach (Arc arc in this.Graph.Arcs(node, ArcFilter.Forward))
			{
				Node node2 = this.Graph.Other(arc, node);
				if (!this.parentArc.ContainsKey(node2))
				{
					this.queue.Enqueue(node2);
					this.level[node2] = num;
					this.parentArc[node2] = arc;
					if (isTarget != null && isTarget(node2))
					{
						reachedTargetNode = node2;
						return false;
					}
				}
			}
			return true;
		}

		public void Run()
		{
			Node node;
			while (this.Step(null, out node))
			{
			}
		}

		public Node RunUntilReached(Node target)
		{
			if (this.Reached(target))
			{
				return target;
			}
			Node node2;
			while (this.Step((Node node) => node == target, out node2))
			{
			}
			return node2;
		}

		public Node RunUntilReached(Func<Node, bool> isTarget)
		{
			Node node = this.ReachedNodes.FirstOrDefault<Node>(isTarget);
			if (node != Node.Invalid)
			{
				return node;
			}
			while (this.Step(isTarget, out node))
			{
			}
			return node;
		}

		public bool Reached(Node x)
		{
			return this.parentArc.ContainsKey(x);
		}

		public IEnumerable<Node> ReachedNodes
		{
			get
			{
				return this.parentArc.Keys;
			}
		}

		public int GetLevel(Node node)
		{
			int num;
			return (!this.level.TryGetValue(node, out num)) ? (-1) : num;
		}

		public Arc GetParentArc(Node node)
		{
			Arc arc;
			return (!this.parentArc.TryGetValue(node, out arc)) ? Arc.Invalid : arc;
		}

		public IPath GetPath(Node node)
		{
			if (!this.Reached(node))
			{
				return null;
			}
			Path path = new Path(this.Graph);
			path.Begin(node);
			for (;;)
			{
				Arc arc = this.GetParentArc(node);
				if (arc == Arc.Invalid)
				{
					break;
				}
				path.AddFirst(arc);
				node = this.Graph.Other(arc, node);
			}
			return path;
		}

		private readonly Dictionary<Node, Arc> parentArc;

		private readonly Dictionary<Node, int> level;

		private readonly Queue<Node> queue;
	}
}
