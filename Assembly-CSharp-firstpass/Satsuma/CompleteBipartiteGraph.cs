using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class CompleteBipartiteGraph : IGraph, IArcLookup
	{
		public int RedNodeCount { get; private set; }

		public int BlueNodeCount { get; private set; }

		public bool Directed { get; private set; }

		public CompleteBipartiteGraph(int redNodeCount, int blueNodeCount, Directedness directedness)
		{
			if (redNodeCount < 0 || blueNodeCount < 0)
			{
				throw new ArgumentException("Invalid node count: " + redNodeCount.ToString() + ";" + blueNodeCount.ToString());
			}
			if ((long)redNodeCount + (long)blueNodeCount > 2147483647L || (long)redNodeCount * (long)blueNodeCount > 2147483647L)
			{
				throw new ArgumentException("Too many nodes: " + redNodeCount.ToString() + ";" + blueNodeCount.ToString());
			}
			this.RedNodeCount = redNodeCount;
			this.BlueNodeCount = blueNodeCount;
			this.Directed = directedness == Directedness.Directed;
		}

		public Node GetRedNode(int index)
		{
			return new Node(1L + (long)index);
		}

		public Node GetBlueNode(int index)
		{
			return new Node(1L + (long)this.RedNodeCount + (long)index);
		}

		public bool IsRed(Node node)
		{
			return node.Id <= (long)this.RedNodeCount;
		}

		public Arc GetArc(Node u, Node v)
		{
			bool flag = this.IsRed(u);
			bool flag2 = this.IsRed(v);
			if (flag == flag2)
			{
				return Arc.Invalid;
			}
			if (flag2)
			{
				Node node = u;
				u = v;
				v = node;
			}
			int num = (int)(u.Id - 1L);
			int num2 = (int)(v.Id - (long)this.RedNodeCount - 1L);
			return new Arc(1L + (long)num2 * (long)this.RedNodeCount + (long)num);
		}

		public Node U(Arc arc)
		{
			return new Node(1L + (arc.Id - 1L) % (long)this.RedNodeCount);
		}

		public Node V(Arc arc)
		{
			return new Node(1L + (long)this.RedNodeCount + (arc.Id - 1L) / (long)this.RedNodeCount);
		}

		public bool IsEdge(Arc arc)
		{
			return !this.Directed;
		}

		public IEnumerable<Node> Nodes(CompleteBipartiteGraph.Color color)
		{
			if (color != CompleteBipartiteGraph.Color.Red)
			{
				if (color == CompleteBipartiteGraph.Color.Blue)
				{
					int num;
					for (int i = 0; i < this.BlueNodeCount; i = num + 1)
					{
						yield return this.GetBlueNode(i);
						num = i;
					}
				}
			}
			else
			{
				int num;
				for (int i = 0; i < this.RedNodeCount; i = num + 1)
				{
					yield return this.GetRedNode(i);
					num = i;
				}
			}
			yield break;
		}

		public IEnumerable<Node> Nodes()
		{
			int num;
			for (int i = 0; i < this.RedNodeCount; i = num + 1)
			{
				yield return this.GetRedNode(i);
				num = i;
			}
			for (int i = 0; i < this.BlueNodeCount; i = num + 1)
			{
				yield return this.GetBlueNode(i);
				num = i;
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (this.Directed && filter == ArcFilter.Edge)
			{
				yield break;
			}
			int num;
			for (int i = 0; i < this.RedNodeCount; i = num + 1)
			{
				for (int j = 0; j < this.BlueNodeCount; j = num + 1)
				{
					yield return this.GetArc(this.GetRedNode(i), this.GetBlueNode(j));
					num = j;
				}
				num = i;
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			bool flag = this.IsRed(u);
			if (this.Directed && (filter == ArcFilter.Edge || (filter == ArcFilter.Forward && !flag) || (filter == ArcFilter.Backward && flag)))
			{
				yield break;
			}
			if (flag)
			{
				int num;
				for (int i = 0; i < this.BlueNodeCount; i = num + 1)
				{
					yield return this.GetArc(u, this.GetBlueNode(i));
					num = i;
				}
			}
			else
			{
				int num;
				for (int i = 0; i < this.RedNodeCount; i = num + 1)
				{
					yield return this.GetArc(this.GetRedNode(i), u);
					num = i;
				}
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			Arc arc = this.GetArc(u, v);
			if (arc != Arc.Invalid && this.ArcCount(u, filter) > 0)
			{
				yield return arc;
			}
			yield break;
		}

		public int NodeCount()
		{
			return this.RedNodeCount + this.BlueNodeCount;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (this.Directed && filter == ArcFilter.Edge)
			{
				return 0;
			}
			return this.RedNodeCount * this.BlueNodeCount;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			bool flag = this.IsRed(u);
			if (this.Directed && (filter == ArcFilter.Edge || (filter == ArcFilter.Forward && !flag) || (filter == ArcFilter.Backward && flag)))
			{
				return 0;
			}
			if (!flag)
			{
				return this.RedNodeCount;
			}
			return this.BlueNodeCount;
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (this.IsRed(u) == this.IsRed(v))
			{
				return 0;
			}
			if (this.ArcCount(u, filter) <= 0)
			{
				return 0;
			}
			return 1;
		}

		public bool HasNode(Node node)
		{
			return node.Id >= 1L && node.Id <= (long)(this.RedNodeCount + this.BlueNodeCount);
		}

		public bool HasArc(Arc arc)
		{
			return arc.Id >= 1L && arc.Id <= (long)(this.RedNodeCount * this.BlueNodeCount);
		}

		public enum Color
		{
			Red,
			Blue
		}
	}
}
