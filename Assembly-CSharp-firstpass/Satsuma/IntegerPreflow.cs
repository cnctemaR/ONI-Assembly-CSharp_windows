using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class IntegerPreflow : IFlow<long>
	{
		public IntegerPreflow(IGraph graph, Func<Arc, long> capacity, Node source, Node target)
		{
			this.Graph = graph;
			this.Capacity = capacity;
			this.Source = source;
			this.Target = target;
			this.flow = new Dictionary<Arc, long>();
			this.excess = new Dictionary<Node, long>();
			this.label = new Dictionary<Node, long>();
			this.active = new PriorityQueue<Node, long>();
			this.Run();
			this.excess = null;
			this.label = null;
			this.active = null;
		}

		public IGraph Graph { get; private set; }

		public Func<Arc, long> Capacity { get; private set; }

		public Node Source { get; private set; }

		public Node Target { get; private set; }

		public long FlowSize { get; private set; }

		private void Run()
		{
			foreach (Node node in this.Graph.Nodes())
			{
				this.label[node] = (long)((!(node == this.Source)) ? 0 : (-(long)this.Graph.NodeCount()));
				this.excess[node] = 0L;
			}
			long num = 0L;
			foreach (Arc arc in this.Graph.Arcs(this.Source, ArcFilter.Forward))
			{
				Node node2 = this.Graph.Other(arc, this.Source);
				if (!(node2 == this.Source))
				{
					long num2 = ((!(this.Graph.U(arc) == this.Source)) ? (-this.Capacity(arc)) : this.Capacity(arc));
					if (num2 != 0L)
					{
						this.flow[arc] = num2;
						num2 = Math.Abs(num2);
						checked
						{
							num += num2;
						}
						Dictionary<Node, long> dictionary;
						Node node3;
						(dictionary = this.excess)[node3 = node2] = dictionary[node3] + num2;
						if (node2 != this.Target)
						{
							this.active[node2] = 0L;
						}
					}
				}
			}
			this.excess[this.Source] = -num;
			while (this.active.Count > 0)
			{
				long num3;
				Node node4 = this.active.Peek(out num3);
				this.active.Pop();
				long num4 = this.excess[node4];
				long num5 = long.MinValue;
				foreach (Arc arc2 in this.Graph.Arcs(node4, ArcFilter.All))
				{
					Node node5 = this.Graph.U(arc2);
					Node node6 = this.Graph.V(arc2);
					if (!(node5 == node6))
					{
						Node node7 = ((!(node4 == node5)) ? node5 : node6);
						bool flag = this.Graph.IsEdge(arc2);
						long num6;
						this.flow.TryGetValue(arc2, out num6);
						long num7 = this.Capacity(arc2);
						long num8 = ((!flag) ? 0L : (-this.Capacity(arc2)));
						if (node5 == node4)
						{
							if (num6 != num7)
							{
								long num9 = this.label[node7];
								if (num9 <= num3)
								{
									num5 = Math.Max(num5, num9 - 1L);
								}
								else
								{
									long num10 = (long)Math.Min((ulong)num4, (ulong)(num7 - num6));
									this.flow[arc2] = num6 + num10;
									Dictionary<Node, long> dictionary;
									Node node8;
									(dictionary = this.excess)[node8 = node6] = dictionary[node8] + num10;
									if (node6 != this.Source && node6 != this.Target)
									{
										this.active[node6] = this.label[node6];
									}
									num4 -= num10;
									if (num4 == 0L)
									{
										break;
									}
								}
							}
						}
						else if (num6 != num8)
						{
							long num11 = this.label[node7];
							if (num11 <= num3)
							{
								num5 = Math.Max(num5, num11 - 1L);
							}
							else
							{
								long num12 = (long)Math.Min((ulong)num4, (ulong)(num6 - num8));
								this.flow[arc2] = num6 - num12;
								Dictionary<Node, long> dictionary;
								Node node9;
								(dictionary = this.excess)[node9 = node5] = dictionary[node9] + num12;
								if (node5 != this.Source && node5 != this.Target)
								{
									this.active[node5] = this.label[node5];
								}
								num4 -= num12;
								if (num4 == 0L)
								{
									break;
								}
							}
						}
					}
				}
				this.excess[node4] = num4;
				if (num4 > 0L)
				{
					if (num5 == -9223372036854775808L)
					{
						throw new InvalidOperationException("Internal error.");
					}
					PriorityQueue<Node, long> priorityQueue = this.active;
					Node node10 = node4;
					long num13;
					num3 = (num13 = num5);
					this.label[node4] = num13;
					priorityQueue[node10] = num13;
				}
			}
			this.FlowSize = 0L;
			foreach (Arc arc3 in this.Graph.Arcs(this.Source, ArcFilter.All))
			{
				Node node11 = this.Graph.U(arc3);
				Node node12 = this.Graph.V(arc3);
				if (!(node11 == node12))
				{
					long num14;
					if (this.flow.TryGetValue(arc3, out num14))
					{
						if (node11 == this.Source)
						{
							this.FlowSize += num14;
						}
						else
						{
							this.FlowSize -= num14;
						}
					}
				}
			}
		}

		public IEnumerable<KeyValuePair<Arc, long>> NonzeroArcs
		{
			get
			{
				return this.flow.Where<KeyValuePair<Arc, long>>((KeyValuePair<Arc, long> kv) => kv.Value != 0L);
			}
		}

		public long Flow(Arc arc)
		{
			long num;
			this.flow.TryGetValue(arc, out num);
			return num;
		}

		private readonly Dictionary<Arc, long> flow;

		private readonly Dictionary<Node, long> excess;

		private readonly Dictionary<Node, long> label;

		private readonly PriorityQueue<Node, long> active;
	}
}
