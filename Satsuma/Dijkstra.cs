using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Dijkstra
	{
		public IGraph Graph { get; private set; }

		public Func<Arc, double> Cost { get; private set; }

		public DijkstraMode Mode { get; private set; }

		public double NullCost { get; private set; }

		public Dijkstra(IGraph graph, Func<Arc, double> cost, DijkstraMode mode)
		{
			this.Graph = graph;
			this.Cost = cost;
			this.Mode = mode;
			this.NullCost = ((mode == DijkstraMode.Sum) ? 0.0 : double.NegativeInfinity);
			this.distance = new Dictionary<Node, double>();
			this.parentArc = new Dictionary<Node, Arc>();
			this.priorityQueue = new PriorityQueue<Node, double>();
		}

		private void ValidateCost(double c)
		{
			if (this.Mode == DijkstraMode.Sum && c < 0.0)
			{
				throw new InvalidOperationException("Invalid cost: " + c);
			}
		}

		public void AddSource(Node node)
		{
			this.AddSource(node, this.NullCost);
		}

		public void AddSource(Node node, double nodeCost)
		{
			if (this.Reached(node))
			{
				throw new InvalidOperationException("Cannot add a reached node as a source.");
			}
			this.ValidateCost(nodeCost);
			this.parentArc[node] = Arc.Invalid;
			this.priorityQueue[node] = nodeCost;
		}

		public Node Step()
		{
			if (this.priorityQueue.Count == 0)
			{
				return Node.Invalid;
			}
			double num;
			Node node = this.priorityQueue.Peek(out num);
			this.priorityQueue.Pop();
			if (double.IsPositiveInfinity(num))
			{
				return Node.Invalid;
			}
			this.distance[node] = num;
			foreach (Arc arc in this.Graph.Arcs(node, ArcFilter.Forward))
			{
				Node node2 = this.Graph.Other(arc, node);
				if (!this.Fixed(node2))
				{
					double num2 = this.Cost(arc);
					this.ValidateCost(num2);
					double num3 = ((this.Mode == DijkstraMode.Sum) ? (num + num2) : Math.Max(num, num2));
					double positiveInfinity;
					if (!this.priorityQueue.TryGetPriority(node2, out positiveInfinity))
					{
						positiveInfinity = double.PositiveInfinity;
					}
					if (num3 < positiveInfinity)
					{
						this.priorityQueue[node2] = num3;
						this.parentArc[node2] = arc;
					}
				}
			}
			return node;
		}

		public void Run()
		{
			while (this.Step() != Node.Invalid)
			{
			}
		}

		public Node RunUntilFixed(Node target)
		{
			if (this.Fixed(target))
			{
				return target;
			}
			Node node;
			do
			{
				node = this.Step();
			}
			while (!(node == Node.Invalid) && !(node == target));
			return node;
		}

		public Node RunUntilFixed(Func<Node, bool> isTarget)
		{
			Node node = this.FixedNodes.FirstOrDefault<Node>(isTarget);
			if (node != Node.Invalid)
			{
				return node;
			}
			do
			{
				node = this.Step();
			}
			while (!(node == Node.Invalid) && !isTarget(node));
			return node;
		}

		public bool Reached(Node node)
		{
			return this.parentArc.ContainsKey(node);
		}

		public IEnumerable<Node> ReachedNodes
		{
			get
			{
				return this.parentArc.Keys;
			}
		}

		public bool Fixed(Node node)
		{
			return this.distance.ContainsKey(node);
		}

		public IEnumerable<Node> FixedNodes
		{
			get
			{
				return this.distance.Keys;
			}
		}

		public double GetDistance(Node node)
		{
			double num;
			if (!this.distance.TryGetValue(node, out num))
			{
				return double.PositiveInfinity;
			}
			return num;
		}

		public Arc GetParentArc(Node node)
		{
			Arc arc;
			if (!this.parentArc.TryGetValue(node, out arc))
			{
				return Arc.Invalid;
			}
			return arc;
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

		private readonly Dictionary<Node, double> distance;

		private readonly Dictionary<Node, Arc> parentArc;

		private readonly PriorityQueue<Node, double> priorityQueue;
	}
}
