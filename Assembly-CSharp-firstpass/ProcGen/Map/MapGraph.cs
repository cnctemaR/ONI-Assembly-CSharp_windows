using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using Satsuma;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class MapGraph : Graph
	{
		public MapGraph(int seed)
			: base(seed)
		{
			this.cellList = new List<Cell>();
			this.cornerList = new List<Corner>();
			this.edgeList = new List<Edge>();
		}

		public List<Cell> cells
		{
			get
			{
				return this.cellList;
			}
		}

		public List<Corner> corners
		{
			get
			{
				return this.cornerList;
			}
		}

		public List<Edge> edges
		{
			get
			{
				return this.edgeList;
			}
		}

		public Edge GetEdge(Corner corner0, Corner corner1, bool createOK = true)
		{
			Edge edge = this.edgeList.Find((Edge e) => (e.corner0 == corner0 || e.corner0 == corner1) && (e.corner1 == corner0 || e.corner1 == corner1));
			if (edge != null)
			{
				return edge;
			}
			if (!createOK)
			{
				global::Debug.LogWarning("Cant create Edge but no edge found", null);
				return null;
			}
			Arc arc = base.baseGraph.AddArc(corner0.node, corner1.node, Directedness.Undirected);
			edge = new Edge(arc, corner0, corner1);
			this.arcList.Add(edge);
			this.edgeList.Add(edge);
			return edge;
		}

		public Edge GetEdge(Corner corner0, Corner corner1, Cell site0, Cell site1, bool createOK = true)
		{
			Edge edge = this.edgeList.Find((Edge e) => (e.corner0 == corner0 || e.corner0 == corner1) && (e.corner1 == corner0 || e.corner1 == corner1));
			if (edge != null)
			{
				return edge;
			}
			if (!createOK)
			{
				global::Debug.LogWarning("Cant create Edge but no edge found", null);
				return null;
			}
			Arc arc = base.baseGraph.AddArc(corner0.node, corner1.node, Directedness.Undirected);
			edge = new Edge(arc, corner0, corner1, site0, site1);
			this.arcList.Add(edge);
			this.edgeList.Add(edge);
			return edge;
		}

		public Corner GetCorner(Vector2 position, bool createOK = true)
		{
			position = new Vector2((float)((int)position.x), (float)((int)position.y));
			Corner corner = this.cornerList.Find((Corner c) => c.position == position);
			if (corner == null)
			{
				if (!createOK)
				{
					global::Debug.LogWarning("Cant create Corner but no corner found", null);
					return null;
				}
				corner = new Corner(base.baseGraph.AddNode());
				this.nodeList.Add(corner);
				corner.SetPosition(position);
				this.cornerList.Add(corner);
			}
			return corner;
		}

		public Cell GetCell(Node node)
		{
			return this.cellList.Find((Cell c) => c.node == node);
		}

		public Cell GetCell(Vector2 position)
		{
			position = new Vector2((float)((int)position.x), (float)((int)position.y));
			return this.cellList.Find((Cell c) => c.position == position);
		}

		public Cell GetCell(Vector2 position, Node node, bool createOK = true)
		{
			position = new Vector2((float)((int)position.x), (float)((int)position.y));
			Cell cell = this.cellList.Find((Cell c) => c.position == position);
			if (cell == null)
			{
				if (!createOK)
				{
					global::Debug.LogWarning("Cant create Cell but no cell found", null);
					return null;
				}
				cell = this.cellList.Find((Cell c) => c.node == node);
				if (cell == null)
				{
					cell = new Cell(node);
					cell.SetPosition(position);
					this.cellList.Add(cell);
				}
				else
				{
					global::Debug.LogWarning("GetCell Same node [" + node.Id + "] differnt position!", null);
				}
			}
			return cell;
		}

		public List<Edge> GetEdgesWithTag(Tag tag)
		{
			List<Edge> list = new List<Edge>();
			for (int i = 0; i < this.edgeList.Count; i++)
			{
				if (this.edgeList[i].tags.Contains(tag))
				{
					list.Add(this.edgeList[i]);
				}
			}
			return list;
		}

		public void Remove(Edge n)
		{
			n.site0.Remove(n);
			n.site1.Remove(n);
			this.edges.Remove(n);
		}

		public void Validate()
		{
			for (int i = 0; i < this.cellList.Count; i++)
			{
				for (int j = 0; j < this.cellList.Count; j++)
				{
					if (j != i)
					{
						if (this.cellList[i] == this.cellList[j])
						{
							global::Debug.LogError("Duplicate cell (class)", null);
							return;
						}
						if (this.cellList[i].position == this.cellList[j].position)
						{
							global::Debug.LogError("Duplicate cell (position)", null);
							return;
						}
						if (this.cellList[i].node == this.cellList[j].node)
						{
							global::Debug.LogError("Duplicate cell (node)", null);
							return;
						}
					}
				}
			}
			for (int k = 0; k < this.cornerList.Count; k++)
			{
				for (int l = 0; l < this.cornerList.Count; l++)
				{
					if (l != k)
					{
						if (this.cornerList[k] == this.cornerList[l])
						{
							global::Debug.LogError("Duplicate corner (class)", null);
							return;
						}
						if (this.cornerList[k].position == this.cornerList[l].position)
						{
							global::Debug.LogError("Duplicate corner (position)", null);
							return;
						}
						if (this.cornerList[k].node == this.cornerList[l].node)
						{
							global::Debug.LogError("Duplicate corner (node)", null);
							return;
						}
					}
				}
			}
			for (int m = 0; m < this.edgeList.Count; m++)
			{
				for (int n = 0; n < this.edgeList.Count; n++)
				{
					if (n != m)
					{
						Edge edge = this.edgeList[m];
						Edge edge2 = this.edgeList[n];
						if (edge == edge2)
						{
							global::Debug.LogError("Duplicate edge (class)", null);
							return;
						}
						if (edge.arc == edge2.arc)
						{
							global::Debug.LogError(string.Concat(new object[]
							{
								"Duplicate EDGE [",
								edge.arc,
								"] & [",
								edge2.arc,
								"] - (ARC) [",
								edge.site0.node.Id,
								"] &  [",
								edge.site1.node.Id,
								"]"
							}), null);
							return;
						}
						if (edge.corner0 == edge2.corner0 && edge.corner1 == edge2.corner1)
						{
							global::Debug.LogError("Duplicate edge (corner same order)", null);
							return;
						}
						if (edge.corner0 == edge2.corner1 && edge.corner1 == edge2.corner0)
						{
							global::Debug.LogError("Duplicate edge (corner different order)", null);
							return;
						}
						if (edge.site0 != edge.site1)
						{
							if (edge2.site0 != edge2.site1)
							{
								if (edge.site0 == edge2.site0 && edge.site1 == edge2.site1)
								{
									global::Debug.LogError("Duplicate edge (site same order)", null);
									return;
								}
								if (edge.site0 == edge2.site1 && edge.site1 == edge2.site0)
								{
									global::Debug.LogError(string.Concat(new object[]
									{
										"Duplicate Edge [",
										edge.arc.Id,
										"] -> [",
										edge.corner0.node.Id,
										"<-->",
										edge.corner1.node.Id,
										"] sites: [",
										edge.site0.node.Id,
										" -- ",
										edge.site1.node.Id,
										"] and [",
										edge2.arc.Id,
										"] -> [",
										edge2.corner0.node.Id,
										"<-->",
										edge2.corner1.node.Id,
										"] sites: [",
										edge2.site0.node.Id,
										" -- ",
										edge2.site1.node.Id,
										"] - (site differnt order)"
									}), null);
									global::Debug.Log(string.Concat(new object[]
									{
										"CE 0: ",
										edge.corner0.position,
										" 1: ",
										edge.corner1.position
									}), null);
									global::Debug.Log(string.Concat(new object[]
									{
										"OE 0: ",
										edge2.corner0.position,
										" 1: ",
										edge2.corner1.position
									}), null);
									global::Debug.Log(string.Concat(new object[]
									{
										"Sites C 0: ",
										edge.site0.position,
										" 1: ",
										edge.site1.position
									}), null);
									DebugExtension.DebugCircle2d(edge.site0.position, Color.red, 1f, 15f, true, 4f);
									DebugExtension.DebugCircle2d(edge.site1.position, Color.magenta, 2f, 15f, true, 4f);
									global::Debug.Log(string.Concat(new object[]
									{
										"Sites O 0: ",
										edge2.site0.position,
										" 1: ",
										edge2.site1.position
									}), null);
									DebugExtension.DebugCircle2d(edge2.site0.position, Color.green, 3f, 15f, true, 4f);
									DebugExtension.DebugCircle2d(edge2.site1.position, Color.cyan, 4f, 15f, true, 4f);
								}
								else
								{
									if (edge.site0.node == edge2.site0.node && edge.site1.node == edge2.site1.node)
									{
										global::Debug.LogError("Duplicate edge (site node same order)", null);
										return;
									}
									if (edge.site1.node == edge2.site0.node && edge.site0.node == edge2.site1.node)
									{
										global::Debug.LogError("Duplicate edge (site node differnt order)", null);
										return;
									}
								}
							}
						}
					}
				}
			}
		}

		[OnDeserialized]
		internal new void OnDeserializedMethod()
		{
			try
			{
				base.OnDeserializedMethod();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				global::Debug.Log("Error deserialising " + message + "\n" + stackTrace, null);
			}
		}

		[Serialize]
		public List<Cell> cellList;

		[Serialize]
		public List<Corner> cornerList;

		[Serialize]
		public List<Edge> edgeList;
	}
}
