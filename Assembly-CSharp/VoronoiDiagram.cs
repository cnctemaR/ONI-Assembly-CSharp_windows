using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay;
using Delaunay.Geo;
using Klei;
using KSerialization;
using UnityEngine;

public class VoronoiDiagram
{
	public VoronoiDiagram()
	{
		this.diagram = null;
	}

	public VoronoiDiagram(Rect bounds, HashSet<VoronoiDiagram.Site> sites)
	{
		this.bounds = bounds;
		this.ids = new List<uint>();
		this.points = new List<Vector2>();
		this.weights = new List<float>();
		HashSet<VoronoiDiagram.Site>.Enumerator enumerator = sites.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			VoronoiDiagram.Site site = enumerator.Current;
			this.ids.Add(site.id);
			this.points.Add(site.position);
			this.weights.Add(site.weight);
			num++;
		}
		this.MakeVD();
	}

	public Voronoi diagram { get; private set; }

	private void MakeVD()
	{
		this.diagram = new Voronoi(this.points, this.ids, this.weights, this.bounds);
	}

	public int GetIdxForNode(global::Klei.Node node)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			if (this.ids[i] == (uint)node.node.Id)
			{
				return i;
			}
		}
		return -1;
	}

	public List<uint> GetNodeIdsForTopEdgeCells()
	{
		List<uint> list = new List<uint>();
		for (int i = 0; i < this.points.Count; i++)
		{
			if (this.IsTopEdgeCell(i))
			{
				list.Add(this.ids[i]);
			}
		}
		return list;
	}

	public bool IsTopEdgeCell(int cell)
	{
		if (cell < 0 || cell >= this.points.Count)
		{
			return false;
		}
		List<Vector2> list = this.diagram.Region(this.points[cell]);
		if (list.Count == 0)
		{
			return false;
		}
		Vector2 vector = list[0];
		for (int i = 1; i < list.Count; i++)
		{
			Vector2 vector2 = list[i];
			if (vector.y == vector2.y && vector2.y == this.bounds.height)
			{
				return true;
			}
			vector = vector2;
		}
		return vector.y == list[0].y && list[0].y == this.bounds.height;
	}

	public void OnDrawGizmos()
	{
		if ((VoronoiDiagram.drawOptions & VoronoiDiagram.DebugFlags.Points) != (VoronoiDiagram.DebugFlags)0 && this.points != null)
		{
			for (int i = 0; i < this.points.Count; i++)
			{
				DebugExtension.DebugPoint(this.points[i], Color.red, 1f, 0f, true);
			}
		}
		if ((VoronoiDiagram.drawOptions & VoronoiDiagram.DebugFlags.TopEdge) != (VoronoiDiagram.DebugFlags)0 && this.points != null)
		{
			for (int j = 0; j < this.points.Count; j++)
			{
				if (this.IsTopEdgeCell(j))
				{
					List<Vector2> list = this.diagram.Region(this.points[j]);
					Vector2 vector = list[0];
					for (int k = 1; k < list.Count; k++)
					{
						Vector2 vector2 = list[k];
						if (vector.y == vector2.y && vector2.y == this.bounds.height)
						{
							Debug.DrawLine(vector, vector2, Color.red);
						}
						else
						{
							Debug.DrawLine(vector, vector2, Color.blue);
						}
						vector = vector2;
					}
					if (vector.y == list[0].y && list[0].y == this.bounds.height)
					{
						Debug.DrawLine(vector, list[0], Color.red);
					}
					else
					{
						Debug.DrawLine(vector, list[0], Color.blue);
					}
				}
			}
		}
		if ((VoronoiDiagram.drawOptions & VoronoiDiagram.DebugFlags.Site) != (VoronoiDiagram.DebugFlags)0 && this.points != null && this.siteIndex >= 0 && this.siteIndex < this.points.Count)
		{
			List<Vector2> list2 = this.diagram.Region(this.points[this.siteIndex]);
			if (list2.Count > 0)
			{
				Vector2 vector3 = list2[0];
				for (int l = 1; l < list2.Count; l++)
				{
					Vector2 vector4 = list2[l];
					if (vector3.y == vector4.y && vector4.y == this.bounds.height)
					{
						Debug.DrawLine(vector3, vector4, Color.blue);
					}
					else
					{
						Debug.DrawLine(vector3, vector4, Color.yellow);
					}
					vector3 = vector4;
				}
				if (vector3.y == list2[0].y && list2[0].y == this.bounds.height)
				{
					Debug.DrawLine(vector3, list2[0], Color.blue);
				}
				else
				{
					Debug.DrawLine(vector3, list2[0], Color.yellow);
				}
			}
		}
		if ((VoronoiDiagram.drawOptions & VoronoiDiagram.DebugFlags.Border) != (VoronoiDiagram.DebugFlags)0)
		{
			DebugExtension.DebugRect(this.bounds, Color.yellow, 0f, true);
		}
	}

	private List<Vector2> points;

	private List<float> weights;

	private Rect bounds;

	private List<uint> ids = new List<uint>();

	public int siteIndex;

	[EnumFlags]
	public static VoronoiDiagram.DebugFlags drawOptions;

	[SerializationConfig(MemberSerialization.OptIn)]
	public class Site
	{
		public Site()
		{
			this.neighbours = new HashSet<KeyValuePair<uint, int>>();
		}

		public Site(uint id, Vector2 pos, float weight = 1f)
		{
			this.id = id;
			this.position = pos;
			this.weight = weight;
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			this.neighbours = new HashSet<KeyValuePair<uint, int>>();
		}

		[Serialize]
		public uint id;

		[Serialize]
		public float weight;

		[Serialize]
		public Vector2 position;

		[Serialize]
		public Polygon poly;

		[Serialize]
		public HashSet<KeyValuePair<uint, int>> neighbours;
	}

	[Flags]
	public enum DebugFlags
	{
		Points = 1,
		Border = 16,
		Site = 32,
		TopEdge = 64
	}
}
