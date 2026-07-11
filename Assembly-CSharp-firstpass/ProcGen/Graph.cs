using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using Satsuma;
using Satsuma.Drawing;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Graph
	{
		public Graph(int seed)
		{
			this.SetSeed(seed);
			this.nodeList = new List<Node>();
			this.arcList = new List<Arc>();
			this.baseGraph = new CustomGraph();
		}

		public List<Node> nodes
		{
			get
			{
				return this.nodeList;
			}
		}

		public List<Arc> arcs
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

		public Node AddNode(string type)
		{
			Node node = new Node(this.baseGraph.AddNode(), type);
			this.nodeList.Add(node);
			return node;
		}

		public void Remove(Node n)
		{
			this.baseGraph.DeleteNode(n.node);
			this.nodes.Remove(n);
		}

		public Arc AddArc(Node nodeA, Node nodeB, string type)
		{
			Arc arc = this.baseGraph.AddArc(nodeA.node, nodeB.node, Directedness.Undirected);
			Arc arc2 = new Arc(arc, type);
			this.arcList.Add(arc2);
			return arc2;
		}

		public Node FindNodeByID(uint id)
		{
			return this.nodeList.Find((Node node) => node.node.Id == (long)((ulong)id));
		}

		public Arc FindArcByID(uint id)
		{
			return this.arcList.Find((Arc arc) => arc.arc.Id == (long)((ulong)id));
		}

		public Node FindNode(Predicate<Node> pred)
		{
			return this.nodeList.Find(pred);
		}

		public Arc FindArc(Predicate<Arc> pred)
		{
			return this.arcList.Find(pred);
		}

		public int GetDistanceToTagSetFromNode(Node node, TagSet tagset)
		{
			List<Node> nodesWithAtLeastOneTag = this.GetNodesWithAtLeastOneTag(tagset);
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

		public int GetDistanceToTagFromNode(Node node, Tag tag)
		{
			List<Node> nodesWithTag = this.GetNodesWithTag(tag);
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
			List<Node> nodesWithTag = this.GetNodesWithTag(tag);
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

		public List<Node> GetNodesWithAtLeastOneTag(TagSet tagset)
		{
			return this.nodeList.FindAll((Node node) => node.tags.ContainsOne(tagset));
		}

		public List<Node> GetNodesWithTag(Tag tag)
		{
			return this.nodeList.FindAll((Node node) => node.tags.Contains(tag));
		}

		public List<Arc> GetArcsWithTag(Tag tag)
		{
			return this.arcList.FindAll((Arc arc) => arc.tags.Contains(tag));
		}

		[OnDeserialized]
		internal void OnDeserializedMethod()
		{
			try
			{
				for (int i = 0; i < this.nodeList.Count; i++)
				{
					Node node = new Node(this.baseGraph.AddNode(), this.nodeList[i].type);
					node.SetPosition(this.nodeList[i].position);
					this.nodeList[i] = node;
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				global::Debug.Log("Error deserialising " + message + "\n" + stackTrace);
			}
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
			Node node2 = this.nodeList.Find((Node n) => n.node == node);
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
			while (!flag && num < 100)
			{
				flag = true;
				Func<Node, PointD> func = (Node n) => this.GetPositionForNode(n);
				CustomGraph baseGraph = this.baseGraph;
				int num2 = num;
				ForceDirectedLayout forceDirectedLayout = new ForceDirectedLayout(baseGraph, func, num2);
				forceDirectedLayout.ExternalForce = (PointD point) => Graph.GetForceForBoundry(point, bounds);
				forceDirectedLayout.Run(0.01);
				IEnumerator<Node> enumerator = this.baseGraph.Nodes().GetEnumerator();
				int num3 = 0;
				while (enumerator.MoveNext())
				{
					Node node = enumerator.Current;
					Node node2 = this.nodeList.Find((Node n) => n.node == node);
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
				global::Debug.LogWarning("Re-ran layout " + num + " times");
			}
			return flag;
		}

		[Serialize]
		public List<Node> nodeList;

		[Serialize]
		public List<Arc> arcList;

		private SeededRandom myRandom;
	}
}
