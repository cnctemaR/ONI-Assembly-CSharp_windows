using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class CompleteGraph : IGraph, IArcLookup
	{
		public bool Directed { get; private set; }

		public CompleteGraph(int nodeCount, Directedness directedness)
		{
			this.nodeCount = nodeCount;
			this.Directed = directedness == Directedness.Directed;
			if (nodeCount < 0)
			{
				throw new ArgumentException("Invalid node count: " + nodeCount);
			}
			long num = (long)nodeCount * (long)(nodeCount - 1);
			if (!this.Directed)
			{
				num /= 2L;
			}
			if (num > 2147483647L)
			{
				throw new ArgumentException("Too many nodes: " + nodeCount);
			}
		}

		public Node GetNode(int index)
		{
			return new Node(1L + (long)index);
		}

		public int GetNodeIndex(Node node)
		{
			return (int)(node.Id - 1L);
		}

		public Arc GetArc(Node u, Node v)
		{
			int num = this.GetNodeIndex(u);
			int num2 = this.GetNodeIndex(v);
			if (num == num2)
			{
				return Arc.Invalid;
			}
			if (!this.Directed && num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return this.GetArcInternal(num, num2);
		}

		private Arc GetArcInternal(int x, int y)
		{
			return new Arc(1L + (long)y * (long)this.nodeCount + (long)x);
		}

		public Node U(Arc arc)
		{
			return new Node(1L + (arc.Id - 1L) % (long)this.nodeCount);
		}

		public Node V(Arc arc)
		{
			return new Node(1L + (arc.Id - 1L) / (long)this.nodeCount);
		}

		public bool IsEdge(Arc arc)
		{
			return !this.Directed;
		}

		public IEnumerable<Node> Nodes()
		{
			for (int i = 0; i < this.nodeCount; i++)
			{
				yield return this.GetNode(i);
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (this.Directed)
			{
				for (int i = 0; i < this.nodeCount; i++)
				{
					for (int j = 0; j < this.nodeCount; j++)
					{
						if (i != j)
						{
							yield return this.GetArcInternal(i, j);
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < this.nodeCount; k++)
				{
					for (int l = k + 1; l < this.nodeCount; l++)
					{
						yield return this.GetArcInternal(k, l);
					}
				}
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (this.Directed)
			{
				if (filter == ArcFilter.Edge)
				{
					goto IL_017B;
				}
				if (filter != ArcFilter.Forward)
				{
					foreach (Node w in this.Nodes())
					{
						if (w != u)
						{
							yield return this.GetArc(w, u);
						}
					}
				}
			}
			if (!this.Directed || filter != ArcFilter.Backward)
			{
				foreach (Node w2 in this.Nodes())
				{
					if (w2 != u)
					{
						yield return this.GetArc(u, w2);
					}
				}
			}
			IL_017B:
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (this.Directed)
			{
				if (filter == ArcFilter.Edge)
				{
					goto IL_00B4;
				}
				if (filter != ArcFilter.Forward)
				{
					yield return this.GetArc(v, u);
				}
			}
			if (!this.Directed || filter != ArcFilter.Backward)
			{
				yield return this.GetArc(u, v);
			}
			IL_00B4:
			yield break;
		}

		public int NodeCount()
		{
			return this.nodeCount;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			int num = this.nodeCount * (this.nodeCount - 1);
			if (!this.Directed)
			{
				num /= 2;
			}
			return num;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (!this.Directed)
			{
				return this.nodeCount - 1;
			}
			switch (filter)
			{
			case ArcFilter.All:
				return 2 * (this.nodeCount - 1);
			case ArcFilter.Edge:
				return 0;
			default:
				return this.nodeCount - 1;
			}
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (!this.Directed)
			{
				return 1;
			}
			switch (filter)
			{
			case ArcFilter.All:
				return 2;
			case ArcFilter.Edge:
				return 0;
			default:
				return 1;
			}
		}

		public bool HasNode(Node node)
		{
			return node.Id >= 1L && node.Id <= (long)this.nodeCount;
		}

		public bool HasArc(Arc arc)
		{
			Node node = this.V(arc);
			if (!this.HasNode(node))
			{
				return false;
			}
			Node node2 = this.U(arc);
			return this.Directed || node2.Id < node.Id;
		}

		private readonly int nodeCount;
	}
}
