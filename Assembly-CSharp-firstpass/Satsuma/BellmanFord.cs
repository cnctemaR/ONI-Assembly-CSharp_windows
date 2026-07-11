using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class BellmanFord
	{
		public IGraph Graph { get; private set; }

		public Func<Arc, double> Cost { get; private set; }

		public IPath NegativeCycle { get; private set; }

		public BellmanFord(IGraph graph, Func<Arc, double> cost, IEnumerable<Node> sources)
		{
			this.Graph = graph;
			this.Cost = cost;
			this.distance = new Dictionary<Node, double>();
			this.parentArc = new Dictionary<Node, Arc>();
			foreach (Node node in sources)
			{
				this.distance[node] = 0.0;
				this.parentArc[node] = Arc.Invalid;
			}
			this.Run();
		}

		private void Run()
		{
			for (int i = this.Graph.NodeCount(); i > 0; i--)
			{
				foreach (Arc arc in this.Graph.Arcs(ArcFilter.All))
				{
					Node node = this.Graph.U(arc);
					Node node2 = this.Graph.V(arc);
					double num = this.GetDistance(node);
					double num2 = this.GetDistance(node2);
					double num3 = this.Cost(arc);
					if (this.Graph.IsEdge(arc))
					{
						if (num > num2)
						{
							Node node3 = node;
							node = node2;
							node2 = node3;
							double num4 = num;
							num = num2;
							num2 = num4;
						}
						if (!double.IsPositiveInfinity(num) && num3 < 0.0)
						{
							Path path = new Path(this.Graph);
							path.Begin(node);
							path.AddLast(arc);
							path.AddLast(arc);
							this.NegativeCycle = path;
							return;
						}
					}
					if (num + num3 < num2)
					{
						this.distance[node2] = num + num3;
						this.parentArc[node2] = arc;
						if (i == 0)
						{
							Node node4 = node;
							for (int j = this.Graph.NodeCount() - 1; j > 0; j--)
							{
								node4 = this.Graph.Other(this.parentArc[node4], node4);
							}
							Path path2 = new Path(this.Graph);
							path2.Begin(node4);
							Node node5 = node4;
							do
							{
								Arc arc2 = this.parentArc[node5];
								path2.AddFirst(arc2);
								node5 = this.Graph.Other(arc2, node5);
							}
							while (!(node5 == node4));
							this.NegativeCycle = path2;
							return;
						}
					}
				}
			}
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

		public double GetDistance(Node node)
		{
			if (this.NegativeCycle != null)
			{
				throw new InvalidOperationException("A negative cycle was found.");
			}
			double num;
			if (!this.distance.TryGetValue(node, out num))
			{
				return double.PositiveInfinity;
			}
			return num;
		}

		public Arc GetParentArc(Node node)
		{
			if (this.NegativeCycle != null)
			{
				throw new InvalidOperationException("A negative cycle was found.");
			}
			Arc arc;
			if (!this.parentArc.TryGetValue(node, out arc))
			{
				return Arc.Invalid;
			}
			return arc;
		}

		public IPath GetPath(Node node)
		{
			if (this.NegativeCycle != null)
			{
				throw new InvalidOperationException("A negative cycle was found.");
			}
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

		private const string NegativeCycleMessage = "A negative cycle was found.";

		private readonly Dictionary<Node, double> distance;

		private readonly Dictionary<Node, Arc> parentArc;
	}
}
