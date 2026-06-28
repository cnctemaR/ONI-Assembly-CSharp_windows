using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Preflow : IFlow<double>
	{
		public IGraph Graph { get; private set; }

		public Func<Arc, double> Capacity { get; private set; }

		public Node Source { get; private set; }

		public Node Target { get; private set; }

		public double FlowSize { get; private set; }

		public double Error { get; private set; }

		public Preflow(IGraph graph, Func<Arc, double> capacity, Node source, Node target)
		{
			this.Graph = graph;
			this.Capacity = capacity;
			this.Source = source;
			this.Target = target;
			this.flow = new Dictionary<Arc, double>();
			Dijkstra dijkstra = new Dijkstra(this.Graph, (Arc a) => -this.Capacity(a), DijkstraMode.Maximum);
			dijkstra.AddSource(this.Source);
			dijkstra.RunUntilFixed(this.Target);
			double num = -dijkstra.GetDistance(this.Target);
			if (double.IsPositiveInfinity(num))
			{
				this.FlowSize = double.PositiveInfinity;
				this.Error = 0.0;
				Node node = this.Target;
				Node node2 = Node.Invalid;
				while (node != this.Source)
				{
					Arc parentArc = dijkstra.GetParentArc(node);
					this.flow[parentArc] = double.PositiveInfinity;
					node2 = this.Graph.Other(parentArc, node);
					node = node2;
				}
				return;
			}
			if (double.IsNegativeInfinity(num))
			{
				num = 0.0;
			}
			this.U = (double)this.Graph.ArcCount(ArcFilter.All) * num;
			double num2 = 0.0;
			foreach (Arc arc in this.Graph.Arcs(this.Source, ArcFilter.Forward))
			{
				if (this.Graph.Other(arc, this.Source) != this.Source)
				{
					num2 += this.Capacity(arc);
					if (num2 > this.U)
					{
						break;
					}
				}
			}
			this.U = Math.Min(this.U, num2);
			double num3 = 0.0;
			foreach (Arc arc2 in this.Graph.Arcs(this.Target, ArcFilter.Backward))
			{
				if (this.Graph.Other(arc2, this.Target) != this.Target)
				{
					num3 += this.Capacity(arc2);
					if (num3 > this.U)
					{
						break;
					}
				}
			}
			this.U = Math.Min(this.U, num3);
			Supergraph supergraph = new Supergraph(this.Graph);
			Node node3 = supergraph.AddNode();
			this.artificialArc = supergraph.AddArc(node3, this.Source, Directedness.Directed);
			this.CapacityMultiplier = Utils.LargestPowerOfTwo(9.223372036854776E+18 / this.U);
			if (this.CapacityMultiplier == 0.0)
			{
				this.CapacityMultiplier = 1.0;
			}
			IntegerPreflow integerPreflow = new IntegerPreflow(supergraph, new Func<Arc, long>(this.IntegralCapacity), node3, this.Target);
			this.FlowSize = (double)integerPreflow.FlowSize / this.CapacityMultiplier;
			this.Error = (double)this.Graph.ArcCount(ArcFilter.All) / this.CapacityMultiplier;
			foreach (KeyValuePair<Arc, long> keyValuePair in integerPreflow.NonzeroArcs)
			{
				this.flow[keyValuePair.Key] = (double)keyValuePair.Value / this.CapacityMultiplier;
			}
		}

		private long IntegralCapacity(Arc arc)
		{
			return (long)(this.CapacityMultiplier * ((arc == this.artificialArc) ? this.U : Math.Min(this.U, this.Capacity(arc))));
		}

		public IEnumerable<KeyValuePair<Arc, double>> NonzeroArcs
		{
			get
			{
				return this.flow.Where<KeyValuePair<Arc, double>>((KeyValuePair<Arc, double> kv) => kv.Value != 0.0);
			}
		}

		public double Flow(Arc arc)
		{
			double num;
			this.flow.TryGetValue(arc, out num);
			return num;
		}

		private Dictionary<Arc, double> flow;

		private Arc artificialArc;

		private double U;

		private double CapacityMultiplier;
	}
}
