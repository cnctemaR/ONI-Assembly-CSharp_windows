using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using KSerialization;
using Satsuma;
using Satsuma.Drawing;
using UnityEngine;

namespace Klei
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Graph
	{
		public Graph()
		{
			this.nodeList = new List<Node>();
			this.arcList = new List<Arc>();
			this.baseGraph = new CustomGraph();
			if (Graph.nodeTypes == null)
			{
				Graph.nodeTypes = new List<string>();
			}
			if (Graph.arcTypes == null)
			{
				Graph.arcTypes = new List<string>();
			}
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

		public Node AddNode(string type)
		{
			this.AddType(type, Graph.nodeTypes);
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
			this.AddType(type, Graph.arcTypes);
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

		[OnDeserialized]
		internal void OnDeserializedMethod()
		{
			try
			{
				for (int i = 0; i < this.nodeList.Count; i++)
				{
					this.AddType(this.nodeList[i].type, Graph.nodeTypes);
					Node node = new Node(this.baseGraph.AddNode(), this.nodeList[i].type);
					node.position = this.nodeList[i].position;
					this.nodeList[i] = node;
				}
				for (int j = 0; j < this.arcList.Count; j++)
				{
					this.AddType(this.arcList[j].type, Graph.arcTypes);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				Debug.Log("Error deserialising " + ex.Message);
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
			List<Vector2> list = PointGenerator.GetRandomPoints(bounds, 50f, 0f, null, PointGenerator.SampleBehaviour.PoissonDisk, true, true, true);
			int num = 0;
			for (int i = 0; i < this.nodeList.Count; i++)
			{
				if (num == list.Count - 1)
				{
					list = PointGenerator.GetRandomPoints(bounds, 10f, 20f, list, PointGenerator.SampleBehaviour.PoissonDisk, true, true, true);
					num = 0;
				}
				this.nodeList[i].position = list[num++];
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
				int num2 = WorldGen.GlobalWorldSeed + num;
				ForceDirectedLayout forceDirectedLayout = new ForceDirectedLayout(this.baseGraph, func, num2);
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
							Debug.LogWarning("Re-doing layout - cell was off map");
							break;
						}
						node2.position = vector;
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
				Debug.LogWarning("Re-ran layout " + num + " times");
			}
			return flag;
		}

		private void AddType(string type, List<string> types)
		{
			if (!types.Contains(type))
			{
				types.Add(type);
			}
		}

		private static Color GetColourForType(List<string> types, string type)
		{
			if (types == null || types.Count == 0)
			{
				return Color.red;
			}
			int num = types.FindIndex(0, (string t) => t == type);
			if (num < 0)
			{
				return Color.red;
			}
			ColourHSV colourHSV = new ColourHSV((float)num / (float)types.Count * 360f, 1f, 1f);
			return colourHSV.ToColor();
		}

		public static Color GetColourForNodeType(string type)
		{
			return Graph.GetColourForType(Graph.nodeTypes, type);
		}

		public static Color GetColourForArcType(string type)
		{
			return Graph.GetColourForType(Graph.arcTypes, type);
		}

		protected virtual void DrawNodes()
		{
			for (int i = 0; i < this.nodeList.Count; i++)
			{
				Node node = this.nodeList[i];
				DebugExtension.DebugPoint(node.position, ((this.drawOptions & Graph.DebugFlags.NodeType) == (Graph.DebugFlags)0) ? Color.green : Graph.GetColourForNodeType(node.type), 1f, 0f, true);
			}
		}

		protected virtual void DrawArcs()
		{
			if (this.nodeList == null)
			{
				Debug.LogWarning("nodeList is null");
				return;
			}
			if (this.arcList == null)
			{
				Debug.LogWarning("arcList is null");
				return;
			}
			if (this.baseGraph == null)
			{
				Debug.LogWarning("baseGraph is null");
				return;
			}
			for (int i = 0; i < this.arcList.Count; i++)
			{
				Arc a = this.arcList[i];
				if (a == null)
				{
					Debug.LogWarning("Arc [" + i + "] is null");
				}
				else
				{
					Node node = this.nodeList.Find((Node n) => n.node == this.baseGraph.U(a.arc));
					Node node2 = this.nodeList.Find((Node n) => n.node == this.baseGraph.V(a.arc));
					Debug.DrawLine(node.position, node2.position, ((this.drawOptions & Graph.DebugFlags.ArcType) == (Graph.DebugFlags)0) ? Color.red : Graph.GetColourForArcType(a.type));
				}
			}
		}

		public void Draw()
		{
			if ((this.drawOptions & Graph.DebugFlags.DrawArcs) != (Graph.DebugFlags)0)
			{
				this.DrawArcs();
			}
			if ((this.drawOptions & Graph.DebugFlags.DrawNodes) != (Graph.DebugFlags)0)
			{
				this.DrawNodes();
			}
		}

		[Serialize]
		public List<Node> nodeList;

		[Serialize]
		public List<Arc> arcList;

		[EnumFlags]
		public Graph.DebugFlags drawOptions;

		private static List<string> nodeTypes;

		private static List<string> arcTypes;

		[Flags]
		public enum DebugFlags
		{
			DrawNodes = 1,
			NodeType = 2,
			NodeId = 4,
			DrawArcs = 8,
			ArcType = 16,
			ArcId = 32
		}
	}
}
