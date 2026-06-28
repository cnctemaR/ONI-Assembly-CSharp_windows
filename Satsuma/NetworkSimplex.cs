using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class NetworkSimplex : IClearable
	{
		public IGraph Graph { get; private set; }

		public Func<Arc, long> LowerBound { get; private set; }

		public Func<Arc, long> UpperBound { get; private set; }

		public Func<Node, long> Supply { get; private set; }

		public Func<Arc, double> Cost { get; private set; }

		public SimplexState State { get; private set; }

		public NetworkSimplex(IGraph graph, Func<Arc, long> lowerBound = null, Func<Arc, long> upperBound = null, Func<Node, long> supply = null, Func<Arc, double> cost = null)
		{
			this.Graph = graph;
			Func<Arc, long> func = lowerBound;
			if (lowerBound == null)
			{
				func = (Arc x) => 0L;
			}
			this.LowerBound = func;
			Func<Arc, long> func2 = upperBound;
			if (upperBound == null)
			{
				func2 = (Arc x) => long.MaxValue;
			}
			this.UpperBound = func2;
			Func<Node, long> func3 = supply;
			if (supply == null)
			{
				func3 = (Node x) => 0L;
			}
			this.Supply = func3;
			Func<Arc, double> func4 = cost;
			if (cost == null)
			{
				func4 = (Arc x) => 1.0;
			}
			this.Cost = func4;
			this.Epsilon = 1.0;
			foreach (Arc arc in graph.Arcs(ArcFilter.All))
			{
				double num = Math.Abs(this.Cost(arc));
				if (num > 0.0 && num < this.Epsilon)
				{
					this.Epsilon = num;
				}
			}
			this.Epsilon *= 1E-12;
			this.Clear();
		}

		public long Flow(Arc arc)
		{
			if (this.Saturated.Contains(arc))
			{
				return this.UpperBound(arc);
			}
			long num;
			if (this.Tree.TryGetValue(arc, out num))
			{
				return num;
			}
			num = this.LowerBound(arc);
			if (num != -9223372036854775808L)
			{
				return num;
			}
			return 0L;
		}

		public IEnumerable<KeyValuePair<Arc, long>> Forest
		{
			get
			{
				return this.Tree.Where<KeyValuePair<Arc, long>>((KeyValuePair<Arc, long> kv) => this.Graph.HasArc(kv.Key));
			}
		}

		public IEnumerable<Arc> UpperBoundArcs
		{
			get
			{
				return this.Saturated;
			}
		}

		public void Clear()
		{
			Dictionary<Node, long> dictionary = new Dictionary<Node, long>();
			foreach (Node node in this.Graph.Nodes())
			{
				dictionary[node] = this.Supply(node);
			}
			this.Saturated = new HashSet<Arc>();
			foreach (Arc arc in this.Graph.Arcs(ArcFilter.All))
			{
				this.LowerBound(arc);
				long num = this.UpperBound(arc);
				if (num < 9223372036854775807L)
				{
					this.Saturated.Add(arc);
				}
				long num2 = this.Flow(arc);
				Dictionary<Node, long> dictionary2;
				Node node2;
				(dictionary2 = dictionary)[node2 = this.Graph.U(arc)] = dictionary2[node2] - num2;
				Dictionary<Node, long> dictionary3;
				Node node3;
				(dictionary3 = dictionary)[node3 = this.Graph.V(arc)] = dictionary3[node3] + num2;
			}
			this.Potential = new Dictionary<Node, double>();
			this.MyGraph = new Supergraph(this.Graph);
			this.ArtificialNode = this.MyGraph.AddNode();
			this.Potential[this.ArtificialNode] = 0.0;
			this.ArtificialArcs = new HashSet<Arc>();
			Dictionary<Node, Arc> dictionary4 = new Dictionary<Node, Arc>();
			foreach (Node node4 in this.Graph.Nodes())
			{
				long num3 = dictionary[node4];
				Arc arc2 = ((num3 > 0L) ? this.MyGraph.AddArc(node4, this.ArtificialNode, Directedness.Directed) : this.MyGraph.AddArc(this.ArtificialNode, node4, Directedness.Directed));
				this.Potential[node4] = (double)((num3 > 0L) ? (-1) : 1);
				this.ArtificialArcs.Add(arc2);
				dictionary4[node4] = arc2;
			}
			this.Tree = new Dictionary<Arc, long>();
			this.TreeSubgraph = new Subgraph(this.MyGraph);
			this.TreeSubgraph.EnableAllArcs(false);
			foreach (KeyValuePair<Node, Arc> keyValuePair in dictionary4)
			{
				this.Tree[keyValuePair.Value] = Math.Abs(dictionary[keyValuePair.Key]);
				this.TreeSubgraph.Enable(keyValuePair.Value, true);
			}
			this.State = SimplexState.FirstPhase;
			this.EnteringArcEnumerator = this.MyGraph.Arcs(ArcFilter.All).GetEnumerator();
			this.EnteringArcEnumerator.MoveNext();
		}

		private long ActualLowerBound(Arc arc)
		{
			if (!this.ArtificialArcs.Contains(arc))
			{
				return this.LowerBound(arc);
			}
			return 0L;
		}

		private long ActualUpperBound(Arc arc)
		{
			if (!this.ArtificialArcs.Contains(arc))
			{
				return this.UpperBound(arc);
			}
			if (this.State != SimplexState.FirstPhase)
			{
				return 0L;
			}
			return long.MaxValue;
		}

		private double ActualCost(Arc arc)
		{
			if (this.ArtificialArcs.Contains(arc))
			{
				return 1.0;
			}
			if (this.State != SimplexState.FirstPhase)
			{
				return this.Cost(arc);
			}
			return 0.0;
		}

		private static ulong MySubtract(long a, long b)
		{
			if (a == 9223372036854775807L || b == -9223372036854775808L)
			{
				return ulong.MaxValue;
			}
			return (ulong)(a - b);
		}

		public void Step()
		{
			if (this.State != SimplexState.FirstPhase && this.State != SimplexState.SecondPhase)
			{
				return;
			}
			Arc arc = this.EnteringArcEnumerator.Current;
			Arc arc2 = Arc.Invalid;
			double num = double.NaN;
			bool flag = false;
			Arc arc3;
			bool flag2;
			double num2;
			for (;;)
			{
				arc3 = this.EnteringArcEnumerator.Current;
				if (!this.Tree.ContainsKey(arc3))
				{
					flag2 = this.Saturated.Contains(arc3);
					num2 = this.ActualCost(arc3) - (this.Potential[this.MyGraph.V(arc3)] - this.Potential[this.MyGraph.U(arc3)]);
					if ((num2 < -this.Epsilon && !flag2) || (num2 > this.Epsilon && (flag2 || this.ActualLowerBound(arc3) == -9223372036854775808L)))
					{
						break;
					}
				}
				if (!this.EnteringArcEnumerator.MoveNext())
				{
					this.EnteringArcEnumerator = this.MyGraph.Arcs(ArcFilter.All).GetEnumerator();
					this.EnteringArcEnumerator.MoveNext();
				}
				if (this.EnteringArcEnumerator.Current == arc)
				{
					goto IL_011B;
				}
			}
			arc2 = arc3;
			num = num2;
			flag = flag2;
			IL_011B:
			if (arc2 == Arc.Invalid)
			{
				if (this.State == SimplexState.FirstPhase)
				{
					this.State = SimplexState.SecondPhase;
					foreach (Arc arc4 in this.ArtificialArcs)
					{
						if (this.Flow(arc4) > 0L)
						{
							this.State = SimplexState.Infeasible;
							break;
						}
					}
					if (this.State == SimplexState.SecondPhase)
					{
						new NetworkSimplex.RecalculatePotentialDfs
						{
							Parent = this
						}.Run(this.TreeSubgraph, null);
						return;
					}
				}
				else
				{
					this.State = SimplexState.Optimal;
				}
				return;
			}
			Node node = this.MyGraph.U(arc2);
			Node node2 = this.MyGraph.V(arc2);
			List<Arc> list = new List<Arc>();
			List<Arc> list2 = new List<Arc>();
			IPath path = this.TreeSubgraph.FindPath(node2, node, Dfs.Direction.Undirected);
			foreach (Node node3 in path.Nodes())
			{
				Arc arc5 = path.NextArc(node3);
				((this.MyGraph.U(arc5) == node3) ? list : list2).Add(arc5);
			}
			ulong num3 = ((num < 0.0) ? NetworkSimplex.MySubtract(this.ActualUpperBound(arc2), this.Flow(arc2)) : NetworkSimplex.MySubtract(this.Flow(arc2), this.ActualLowerBound(arc2)));
			Arc arc6 = arc2;
			bool flag3 = !flag;
			foreach (Arc arc7 in list)
			{
				ulong num4 = ((num < 0.0) ? NetworkSimplex.MySubtract(this.ActualUpperBound(arc7), this.Tree[arc7]) : NetworkSimplex.MySubtract(this.Tree[arc7], this.ActualLowerBound(arc7)));
				if (num4 < num3)
				{
					num3 = num4;
					arc6 = arc7;
					flag3 = num < 0.0;
				}
			}
			foreach (Arc arc8 in list2)
			{
				ulong num5 = ((num > 0.0) ? NetworkSimplex.MySubtract(this.ActualUpperBound(arc8), this.Tree[arc8]) : NetworkSimplex.MySubtract(this.Tree[arc8], this.ActualLowerBound(arc8)));
				if (num5 < num3)
				{
					num3 = num5;
					arc6 = arc8;
					flag3 = num > 0.0;
				}
			}
			long num6 = 0L;
			if (num3 != 0UL)
			{
				if (num3 == 18446744073709551615UL)
				{
					this.State = SimplexState.Unbounded;
					return;
				}
				num6 = (long)((num < 0.0) ? num3 : (-(long)num3));
				foreach (Arc arc9 in list)
				{
					Dictionary<Arc, long> tree;
					Arc arc10;
					(tree = this.Tree)[arc10 = arc9] = tree[arc10] + num6;
				}
				foreach (Arc arc11 in list2)
				{
					Dictionary<Arc, long> tree2;
					Arc arc12;
					(tree2 = this.Tree)[arc12 = arc11] = tree2[arc12] - num6;
				}
			}
			if (!(arc6 == arc2))
			{
				this.Tree.Remove(arc6);
				this.TreeSubgraph.Enable(arc6, false);
				if (flag3)
				{
					this.Saturated.Add(arc6);
				}
				double num7 = this.ActualCost(arc2) - (this.Potential[node2] - this.Potential[node]);
				if (num7 != 0.0)
				{
					new NetworkSimplex.UpdatePotentialDfs
					{
						Parent = this,
						Diff = num7
					}.Run(this.TreeSubgraph, new Node[] { node2 });
				}
				this.Tree[arc2] = this.Flow(arc2) + num6;
				if (flag)
				{
					this.Saturated.Remove(arc2);
				}
				this.TreeSubgraph.Enable(arc2, true);
				return;
			}
			if (flag)
			{
				this.Saturated.Remove(arc2);
				return;
			}
			this.Saturated.Add(arc2);
		}

		public void Run()
		{
			while (this.State == SimplexState.FirstPhase || this.State == SimplexState.SecondPhase)
			{
				this.Step();
			}
		}

		private double Epsilon;

		private Supergraph MyGraph;

		private Node ArtificialNode;

		private HashSet<Arc> ArtificialArcs;

		private Dictionary<Arc, long> Tree;

		private Subgraph TreeSubgraph;

		private HashSet<Arc> Saturated;

		private Dictionary<Node, double> Potential;

		private IEnumerator<Arc> EnteringArcEnumerator;

		private class RecalculatePotentialDfs : Dfs
		{
			protected override void Start(out Dfs.Direction direction)
			{
				direction = Dfs.Direction.Undirected;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					this.Parent.Potential[node] = 0.0;
				}
				else
				{
					Node node2 = this.Parent.MyGraph.Other(arc, node);
					this.Parent.Potential[node] = this.Parent.Potential[node2] + ((node == this.Parent.MyGraph.V(arc)) ? this.Parent.ActualCost(arc) : (-this.Parent.ActualCost(arc)));
				}
				return true;
			}

			public NetworkSimplex Parent;
		}

		private class UpdatePotentialDfs : Dfs
		{
			protected override void Start(out Dfs.Direction direction)
			{
				direction = Dfs.Direction.Undirected;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				Dictionary<Node, double> potential;
				(potential = this.Parent.Potential)[node] = potential[node] + this.Diff;
				return true;
			}

			public NetworkSimplex Parent;

			public double Diff;
		}
	}
}
