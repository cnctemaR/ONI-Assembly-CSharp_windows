using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class MapGraph : Graph<Cell, Edge>
	{
		public List<Corner> corners
		{
			get
			{
				return this.cornerList;
			}
		}

		public MapGraph(int seed)
			: base(seed)
		{
			this.cornerList = new List<Corner>();
		}

		public Edge GetEdge(Cell site0, Cell site1)
		{
			return base.GetArc(site0, site1);
		}

		public Edge AddEdge(Cell site0, Cell site1, Corner corner0, Corner corner1)
		{
			Edge edge = base.AddArc(site0, site1, "Edge");
			edge.SetCorners(corner0, corner1);
			return edge;
		}

		public Edge AddOrGetEdge(Cell site0, Cell site1, Corner corner0, Corner corner1)
		{
			Edge edge = base.GetArc(site0, site1);
			if (edge != null)
			{
				return edge;
			}
			edge = base.AddArc(site0, site1, "Edge");
			edge.SetCorners(corner0, corner1);
			return edge;
		}

		public Corner AddOrGetCorner(Vector2 position)
		{
			Corner corner = this.cornerList.Find(delegate(Corner c)
			{
				Vector2 vector = c.position - position;
				return vector.x < 1f && vector.x > -1f && vector.y < 1f && vector.y > -1f;
			});
			if (corner == null)
			{
				corner = new Corner(position);
				this.cornerList.Add(corner);
			}
			return corner;
		}

		public List<Edge> GetEdgesWithTag(Tag tag)
		{
			return base.GetArcsWithTag(tag);
		}

		public void ClearEdgesAndCorners()
		{
			foreach (Edge edge in this.arcList)
			{
				base.baseGraph.DeleteArc(edge.arc);
			}
			this.arcList.Clear();
			this.cornerList.Clear();
		}

		public void ClearTags()
		{
			foreach (Cell cell in this.nodeList)
			{
				cell.tags.Clear();
			}
			foreach (Edge edge in this.arcList)
			{
				edge.tags.Clear();
			}
		}

		public void Validate()
		{
			for (int i = 0; i < this.nodeList.Count; i++)
			{
				for (int j = 0; j < this.nodeList.Count; j++)
				{
					if (j != i)
					{
						if (this.nodeList[i] == this.nodeList[j])
						{
							global::Debug.LogError("Duplicate cell (instance)");
							return;
						}
						if (this.nodeList[i].position == this.nodeList[j].position)
						{
							global::Debug.LogError("Duplicate cell (position)");
							return;
						}
						if (this.nodeList[i].node == this.nodeList[j].node)
						{
							global::Debug.LogError("Duplicate cell (node)");
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
							global::Debug.LogError("Duplicate corner (instance)");
							return;
						}
						if (this.cornerList[k].position == this.cornerList[l].position)
						{
							global::Debug.LogError("Duplicate corner (position)");
							return;
						}
					}
				}
			}
			for (int m = 0; m < this.arcList.Count; m++)
			{
				for (int n = 0; n < this.arcList.Count; n++)
				{
					if (n != m)
					{
						Edge edge = this.arcList[m];
						Edge edge2 = this.arcList[n];
						if (edge == edge2)
						{
							global::Debug.LogError("Duplicate edge (instance)");
							return;
						}
						if (edge.arc == edge2.arc)
						{
							global::Debug.LogError(string.Concat(new string[]
							{
								"Duplicate EDGE [",
								edge.arc.ToString(),
								"] & [",
								edge2.arc.ToString(),
								"] - (ARC)"
							}));
							return;
						}
						if (edge.corner0 == edge2.corner0 && edge.corner1 == edge2.corner1)
						{
							global::Debug.LogError("Duplicate edge (corner same order)");
							return;
						}
						if (edge.corner0 == edge2.corner1 && edge.corner1 == edge2.corner0)
						{
							global::Debug.LogError("Duplicate edge (corner different order)");
							return;
						}
						List<Cell> nodes = base.GetNodes(edge);
						List<Cell> nodes2 = base.GetNodes(edge2);
						if (nodes[0] == nodes2[0] && nodes[1] == nodes2[1])
						{
							global::Debug.LogError("Duplicate edge (site same order)");
							return;
						}
						if (nodes[0] == nodes2[1] && nodes[1] == nodes2[0])
						{
							global::Debug.LogError("Duplicate Edge (site differnt order)");
							return;
						}
						if (nodes[0].node == nodes2[0].node && nodes[1].node == nodes2[1].node)
						{
							global::Debug.LogError("Duplicate edge (site node same order)");
							return;
						}
						if (nodes[0].node == nodes2[1].node && nodes[1].node == nodes2[0].node)
						{
							global::Debug.LogError("Duplicate edge (site node differnt order)");
							return;
						}
					}
				}
			}
		}

		[Serialize]
		public List<Corner> cornerList;
	}
}
