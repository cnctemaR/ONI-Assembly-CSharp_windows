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
				throw new ArgumentException("Invalid node count: " + nodeCount.ToString());
			}
			long num = (long)nodeCount * (long)(nodeCount - 1);
			if (!this.Directed)
			{
				num /= 2L;
			}
			if (num > 2147483647L)
			{
				throw new ArgumentException("Too many nodes: " + nodeCount.ToString());
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
			int num;
			for (int i = 0; i < this.nodeCount; i = num + 1)
			{
				yield return this.GetNode(i);
				num = i;
			}
			yield break;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (this.Directed)
			{
				int num;
				for (int i = 0; i < this.nodeCount; i = num + 1)
				{
					for (int j = 0; j < this.nodeCount; j = num + 1)
					{
						if (i != j)
						{
							yield return this.GetArcInternal(i, j);
						}
						num = j;
					}
					num = i;
				}
			}
			else
			{
				int num;
				for (int i = 0; i < this.nodeCount; i = num + 1)
				{
					for (int j = i + 1; j < this.nodeCount; j = num + 1)
					{
						yield return this.GetArcInternal(i, j);
						num = j;
					}
					num = i;
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
					yield break;
				}
				if (filter != ArcFilter.Forward)
				{
					foreach (Node node in this.Nodes())
					{
						if (node != u)
						{
							yield return this.GetArc(node, u);
						}
					}
					IEnumerator<Node> enumerator = null;
				}
			}
			if (!this.Directed || filter != ArcFilter.Backward)
			{
				foreach (Node node2 in this.Nodes())
				{
					if (node2 != u)
					{
						yield return this.GetArc(u, node2);
					}
				}
				IEnumerator<Node> enumerator = null;
			}
			yield break;
			yield break;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (this.Directed)
			{
				if (filter == ArcFilter.Edge)
				{
					yield break;
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
			if (filter == ArcFilter.All)
			{
				return 2 * (this.nodeCount - 1);
			}
			if (filter != ArcFilter.Edge)
			{
				return this.nodeCount - 1;
			}
			return 0;
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (!this.Directed)
			{
				return 1;
			}
			if (filter == ArcFilter.All)
			{
				return 2;
			}
			if (filter != ArcFilter.Edge)
			{
				return 1;
			}
			return 0;
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
