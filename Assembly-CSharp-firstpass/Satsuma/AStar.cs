using System;

namespace Satsuma
{
	public sealed class AStar
	{
		public AStar(IGraph graph, Func<Arc, double> cost, Func<Node, double> heuristic)
		{
			this.Graph = graph;
			this.Cost = cost;
			this.Heuristic = heuristic;
			this.dijkstra = new Dijkstra(this.Graph, (Arc arc) => this.Cost(arc) - this.Heuristic(this.Graph.U(arc)) + this.Heuristic(this.Graph.V(arc)), DijkstraMode.Sum);
		}

		public IGraph Graph { get; private set; }

		public Func<Arc, double> Cost { get; private set; }

		public Func<Node, double> Heuristic { get; private set; }

		private Node CheckTarget(Node node)
		{
			if (node != Node.Invalid && this.Heuristic(node) != 0.0)
			{
				throw new ArgumentException("Heuristic is nonzero for a target");
			}
			return node;
		}

		public void AddSource(Node node)
		{
			this.dijkstra.AddSource(node, this.Heuristic(node));
		}

		public Node RunUntilReached(Node target)
		{
			return this.CheckTarget(this.dijkstra.RunUntilFixed(target));
		}

		public Node RunUntilReached(Func<Node, bool> isTarget)
		{
			return this.CheckTarget(this.dijkstra.RunUntilFixed(isTarget));
		}

		public double GetDistance(Node node)
		{
			this.CheckTarget(node);
			return (!this.dijkstra.Fixed(node)) ? double.PositiveInfinity : this.dijkstra.GetDistance(node);
		}

		public IPath GetPath(Node node)
		{
			this.CheckTarget(node);
			if (!this.dijkstra.Fixed(node))
			{
				return null;
			}
			return this.dijkstra.GetPath(node);
		}

		private Dijkstra dijkstra;
	}
}
