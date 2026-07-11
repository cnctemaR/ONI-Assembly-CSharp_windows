using System;
using System.Collections.Generic;

namespace Satsuma.Drawing
{
	public sealed class ForceDirectedLayout
	{
		public ForceDirectedLayout(IGraph graph, Func<Node, PointD> initialPositions = null, int seed = -1)
		{
			this.Graph = graph;
			this.NodePositions = new Dictionary<Node, PointD>();
			this.SpringForce = (double d) => 2.0 * Math.Log(d);
			this.ElectricForce = (double d) => 1.0 / (d * d);
			this.ExternalForce = null;
			this.TemperatureAttenuation = 0.95;
			this.Initialize(initialPositions, seed);
		}

		public IGraph Graph { get; private set; }

		public Dictionary<Node, PointD> NodePositions { get; private set; }

		public Func<double, double> SpringForce { get; set; }

		public Func<double, double> ElectricForce { get; set; }

		public Func<PointD, PointD> ExternalForce { get; set; }

		public double Temperature { get; set; }

		public double TemperatureAttenuation { get; set; }

		public void Initialize(Func<Node, PointD> initialPositions = null, int seed = -1)
		{
			if (initialPositions == null)
			{
				Random r;
				if (seed == -1)
				{
					r = new Random();
				}
				else
				{
					r = new Random(seed);
				}
				initialPositions = (Node node) => new PointD(r.NextDouble(), r.NextDouble());
			}
			foreach (Node node2 in this.Graph.Nodes())
			{
				this.NodePositions[node2] = initialPositions(node2);
			}
			this.Temperature = 0.2;
		}

		public void Step()
		{
			Dictionary<Node, PointD> dictionary = new Dictionary<Node, PointD>();
			foreach (Node node in this.Graph.Nodes())
			{
				PointD pointD = this.NodePositions[node];
				double num = 0.0;
				double num2 = 0.0;
				foreach (Arc arc in this.Graph.Arcs(node, ArcFilter.All))
				{
					PointD pointD2 = this.NodePositions[this.Graph.Other(arc, node)];
					double num3 = pointD.Distance(pointD2);
					double num4 = this.Temperature * this.SpringForce(num3);
					num += (pointD2.X - pointD.X) / num3 * num4;
					num2 += (pointD2.Y - pointD.Y) / num3 * num4;
				}
				foreach (Node node2 in this.Graph.Nodes())
				{
					if (!(node2 == node))
					{
						PointD pointD3 = this.NodePositions[node2];
						double num5 = pointD.Distance(pointD3);
						double num6 = this.Temperature * this.ElectricForce(num5);
						num += (pointD.X - pointD3.X) / num5 * num6;
						num2 += (pointD.Y - pointD3.Y) / num5 * num6;
					}
				}
				if (this.ExternalForce != null)
				{
					PointD pointD4 = this.ExternalForce(pointD);
					num += this.Temperature * pointD4.X;
					num2 += this.Temperature * pointD4.Y;
				}
				dictionary[node] = new PointD(num, num2);
			}
			foreach (Node node3 in this.Graph.Nodes())
			{
				Dictionary<Node, PointD> nodePositions;
				Node node4;
				(nodePositions = this.NodePositions)[node4 = node3] = nodePositions[node4] + dictionary[node3];
			}
			this.Temperature *= this.TemperatureAttenuation;
		}

		public void Run(double minimumTemperature = 0.01)
		{
			while (this.Temperature > minimumTemperature)
			{
				this.Step();
			}
		}

		public const double DefaultStartingTemperature = 0.2;

		public const double DefaultMinimumTemperature = 0.01;

		public const double DefaultTemperatureAttenuation = 0.95;
	}
}
