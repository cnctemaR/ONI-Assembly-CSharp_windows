using System;
using System.Collections.Generic;
using Delaunay.Geo;
using KSerialization;
using Satsuma;
using Satsuma.Drawing;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Graph<N, A> where N : Node, new() where A : Arc, new()
	{
		public List<N> nodes
		{
			get
			{
				return this.nodeList;
			}
		}

		public List<A> arcs
		{
			get
			{
				return this.arcList;
			}
		}

		public CustomGraph baseGraph { get; private set; }

		public void SetSeed(int seed)
		{
			this.myRandom = new SeededRandom(seed);
		}

		public Graph(int seed)
		{
			this.SetSeed(seed);
			this.nodeList = new List<N>();
			this.arcList = new List<A>();
			this.baseGraph = new CustomGraph();
		}

		public N AddNode(string type, Vector2 position = default(Vector2))
		{
			N n = new N();
			n.SetNode(this.baseGraph.AddNode());
			n.SetType(type);
			n.SetPosition(position);
			this.nodeList.Add(n);
			return n;
		}

		public void Remove(N n)
		{
			this.baseGraph.DeleteNode(n.node);
			this.nodes.Remove(n);
		}

		public A AddArc(N nodeA, N nodeB, string type)
		{
			Arc arc = this.baseGraph.AddArc(nodeA.node, nodeB.node, Directedness.Undirected);
			A a = new A();
			a.SetArc(arc);
			a.SetType(type);
			this.arcList.Add(a);
			return a;
		}

		public N FindNodeByID(uint id)
		{
			return this.nodeList.Find((N node) => node.node.Id == (long)((ulong)id));
		}

		public A FindArcByID(uint id)
		{
			return this.arcList.Find((A arc) => arc.arc.Id == (long)((ulong)id));
		}

		public N FindNode(Predicate<N> pred)
		{
			return this.nodeList.Find(pred);
		}

		public A FindArc(Predicate<A> pred)
		{
			return this.arcList.Find(pred);
		}

		public List<A> GetArcs(N node0)
		{
			List<A> list = new List<A>();
			using (IEnumerator<Arc> enumerator = this.baseGraph.Arcs(node0.node, ArcFilter.All).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Arc sarc = enumerator.Current;
					list.Add(this.arcList.Find((A a) => a.arc == sarc));
				}
			}
			return list;
		}

		public A GetArc(N node0, N node1)
		{
			IEnumerator<Arc> enumerator = this.baseGraph.Arcs(node0.node, node1.node, ArcFilter.All).GetEnumerator();
			if (enumerator.MoveNext())
			{
				Arc sarc = enumerator.Current;
				return this.arcList.Find((A a) => a.arc == sarc);
			}
			return default(A);
		}

		public List<N> GetNodes(A arc)
		{
			Node u = this.baseGraph.U(arc.arc);
			Node v = this.baseGraph.V(arc.arc);
			return new List<N>
			{
				this.nodeList.Find((N n) => n.node == u),
				this.nodeList.Find((N n) => n.node == v)
			};
		}

		public int GetDistanceToTagSetFromNode(N node, TagSet tagset)
		{
			List<N> nodesWithAtLeastOneTag = this.GetNodesWithAtLeastOneTag(tagset);
			if (nodesWithAtLeastOneTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(this.baseGraph, (Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithAtLeastOneTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithAtLeastOneTag[i].node);
				}
				dijkstra.RunUntilFixed(node.node);
				return (int)dijkstra.GetDistance(node.node);
			}
			return -1;
		}

		public int GetDistanceToTagFromNode(N node, Tag tag)
		{
			List<N> nodesWithTag = this.GetNodesWithTag(tag);
			if (nodesWithTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(this.baseGraph, (Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithTag[i].node);
				}
				dijkstra.RunUntilFixed(node.node);
				return (int)dijkstra.GetDistance(node.node);
			}
			return -1;
		}

		public Dictionary<uint, int> GetDistanceToTag(Tag tag)
		{
			List<N> nodesWithTag = this.GetNodesWithTag(tag);
			if (nodesWithTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(this.baseGraph, (Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithTag[i].node);
				}
				Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
				for (int j = 0; j < this.nodes.Count; j++)
				{
					dijkstra.RunUntilFixed(this.nodes[j].node);
					dictionary[(uint)this.nodes[j].node.Id] = (int)dijkstra.GetDistance(this.nodes[j].node);
				}
				return dictionary;
			}
			return null;
		}

		public List<N> GetNodesWithAtLeastOneTag(TagSet tagset)
		{
			return this.nodeList.FindAll((N node) => node.tags.ContainsOne(tagset));
		}

		public List<N> GetNodesWithTag(Tag tag)
		{
			return this.nodeList.FindAll((N node) => node.tags.Contains(tag));
		}

		public List<A> GetArcsWithTag(Tag tag)
		{
			return this.arcList.FindAll((A arc) => arc.tags.Contains(tag));
		}

		public static PointD GetForceForBoundry(PointD particle, Polygon bounds)
		{
			Vector2 vector = new Vector2((float)particle.X, (float)particle.Y);
			List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>> edgesWithinDistance = bounds.GetEdgesWithinDistance(vector, float.MaxValue);
			double num = 0.0;
			double num2 = 0.0;
			for (int i = 0; i < edgesWithinDistance.Count; i++)
			{
				KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>> keyValuePair = edgesWithinDistance[i];
				MathUtil.Pair<Vector2, Vector2> value = keyValuePair.Value;
				float second = keyValuePair.Key.Second;
				double num3 = (double)keyValuePair.Key.First;
				Vector2 vector2 = value.First + (value.Second - value.First) * second;
				PointD pointD = new PointD((double)vector2.x, (double)vector2.y);
				double num4 = 1.0 / (num3 * num3);
				num += (particle.X - pointD.X) / num3 * num4;
				num2 += (particle.Y - pointD.Y) / num3 * num4;
			}
			if (bounds.Contains(vector))
			{
				return new PointD(num, num2);
			}
			return new PointD(-num, -num2);
		}

		public PointD GetPositionForNode(Node node)
		{
			Node node2 = this.nodeList.Find((N n) => n.node == node);
			return new PointD((double)node2.position.x, (double)node2.position.y);
		}

		public void SetInitialNodePositions(Polygon bounds)
		{
			List<Vector2> list = PointGenerator.GetRandomPoints(bounds, 50f, 0f, null, PointGenerator.SampleBehaviour.PoissonDisk, true, this.myRandom, true, true);
			int num = 0;
			for (int i = 0; i < this.nodeList.Count; i++)
			{
				if (num == list.Count - 1)
				{
					list = PointGenerator.GetRandomPoints(bounds, 10f, 20f, list, PointGenerator.SampleBehaviour.PoissonDisk, true, this.myRandom, true, true);
					num = 0;
				}
				this.nodeList[i].SetPosition(list[num++]);
			}
		}

		public bool Layout(Polygon bounds = null)
		{
			bool flag = false;
			int num = 0;
			Vector2 vector = default(Vector2);
			Func<Node, PointD> <>9__0;
			Func<PointD, PointD> <>9__1;
			while (!flag && num < 100)
			{
				flag = true;
				Func<Node, PointD> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Node n) => this.GetPositionForNode(n));
				}
				Func<Node, PointD> func2 = func;
				IGraph baseGraph = this.baseGraph;
				int num2 = num;
				ForceDirectedLayout forceDirectedLayout = new ForceDirectedLayout(baseGraph, func2, num2);
				ForceDirectedLayout forceDirectedLayout2 = forceDirectedLayout;
				Func<PointD, PointD> func3;
				if ((func3 = <>9__1) == null)
				{
					func3 = (<>9__1 = (PointD point) => Graph<N, A>.GetForceForBoundry(point, bounds));
				}
				forceDirectedLayout2.ExternalForce = func3;
				forceDirectedLayout.Run(0.01);
				IEnumerator<Node> enumerator = this.baseGraph.Nodes().GetEnumerator();
				int num3 = 0;
				while (enumerator.MoveNext())
				{
					Node node = enumerator.Current;
					Node node2 = this.nodeList.Find((N n) => n.node == node);
					if (node2 != null)
					{
						vector.x = (float)forceDirectedLayout.NodePositions[node].X;
						vector.y = (float)forceDirectedLayout.NodePositions[node].Y;
						if (!bounds.Contains(vector))
						{
							flag = false;
							global::Debug.LogWarning("Re-doing layout - cell was off map");
							break;
						}
						node2.SetPosition(vector);
					}
					if (!flag)
					{
						break;
					}
					num3++;
				}
				num++;
			}
			if (num >= 10)
			{
				global::Debug.LogWarning("Re-ran layout " + num.ToString() + " times");
			}
			return flag;
		}

		[Serialize]
		public List<N> nodeList;

		[Serialize]
		public List<A> arcList;

		private SeededRandom myRandom;
	}
}
