using System;
using System.Collections.Generic;
using ClipperLib;
using Delaunay.Geo;
using Klei;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class VoronoiNode
{
	public VoronoiNode()
	{
		this.type = VoronoiNode.NodeType.Unknown;
	}

	public VoronoiNode(VoronoiNode.NodeType type)
	{
		this.type = type;
		this.tags = new TagSet();
	}

	protected VoronoiNode(VoronoiDiagram.Site site, VoronoiNode.NodeType type, VoronoiTree parent)
	{
		this.tags = new TagSet();
		this.site = site;
		this.type = type;
		this.parent = parent;
	}

	public VoronoiTree parent { get; private set; }

	public void SetParent(VoronoiTree newParent)
	{
		this.parent = newParent;
	}

	public VoronoiNode GetNeighbour(uint id)
	{
		foreach (KeyValuePair<uint, int> keyValuePair in this.site.neighbours)
		{
			if (keyValuePair.Key == id)
			{
				return this.GetSibling(id);
			}
		}
		return null;
	}

	public List<VoronoiNode> GetNeighbors()
	{
		List<VoronoiNode> list = new List<VoronoiNode>();
		HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = this.site.neighbours.GetEnumerator();
		while (enumerator.MoveNext())
		{
			List<VoronoiNode> list2 = list;
			KeyValuePair<uint, int> keyValuePair = enumerator.Current;
			list2.Add(this.GetSibling(keyValuePair.Key));
		}
		return list;
	}

	public List<KeyValuePair<VoronoiNode, LineSegment>> GetNeighborsByEdge()
	{
		List<KeyValuePair<VoronoiNode, LineSegment>> list = new List<KeyValuePair<VoronoiNode, LineSegment>>();
		for (int i = 0; i < this.site.poly.Vertices.Count; i++)
		{
			if (this.site.neighbours != null)
			{
				LineSegment edge = this.site.poly.GetEdge(i);
				VoronoiNode voronoiNode = null;
				foreach (KeyValuePair<uint, int> keyValuePair in this.site.neighbours)
				{
					if (keyValuePair.Value == i)
					{
						HashSet<KeyValuePair<uint, int>>.Enumerator enumerator;
						KeyValuePair<uint, int> keyValuePair2 = enumerator.Current;
						voronoiNode = this.GetSibling(keyValuePair2.Key);
					}
				}
				if (voronoiNode != null)
				{
					list.Add(new KeyValuePair<VoronoiNode, LineSegment>(voronoiNode, edge));
				}
			}
		}
		return list;
	}

	public VoronoiNode GetSibling(uint siteId)
	{
		return this.parent.GetChildByID(siteId);
	}

	public List<VoronoiNode> GetSiblings()
	{
		List<VoronoiNode> list = new List<VoronoiNode>();
		for (int i = 0; i < this.parent.ChildCount(); i++)
		{
			VoronoiNode child = this.parent.GetChild(i);
			if (child != this)
			{
				list.Add(child);
			}
		}
		return list;
	}

	public bool ComputeNode(List<VoronoiDiagram.Site> sites)
	{
		if (this.site.poly == null || sites == null || sites.Count == 0)
		{
			this.visited = VoronoiNode.VisitedType.MissingData;
			return false;
		}
		this.visited = VoronoiNode.VisitedType.VisitedSuccess;
		List<Vector2> list = null;
		List<Vector2> list2 = new List<Vector2>();
		for (int i = 0; i < sites.Count; i++)
		{
			list2.Add(sites[i].position);
		}
		int num = 0;
		for (int j = 0; j < sites.Count; j++)
		{
			if (!this.site.poly.Contains(sites[j].position))
			{
				if (list == null)
				{
					list = PointGenerator.GetRandomPoints(this.site.poly, 5f, 1f, list2, PointGenerator.SampleBehaviour.PoissonDisk, true, true, true);
				}
				if (num >= list.Count - 1)
				{
					list2.AddRange(list);
					list = PointGenerator.GetRandomPoints(this.site.poly, 0.5f, 0.5f, list2, PointGenerator.SampleBehaviour.PoissonDisk, true, true, true);
					num = 0;
				}
				if (list.Count == 0)
				{
					sites[j].position = sites[0].position + Vector2.one * WorldGen.RandomValue();
				}
				else
				{
					sites[j].position = list[num++];
				}
			}
		}
		HashSet<Vector2> hashSet = new HashSet<Vector2>();
		for (int k = 0; k < sites.Count; k++)
		{
			if (hashSet.Contains(sites[k].position))
			{
				this.visited = VoronoiNode.VisitedType.Error;
				sites[k].position += new Vector2(WorldGen.RandomRange(0f, 1f), WorldGen.RandomRange(0f, 1f));
			}
			hashSet.Add(sites[k].position);
			sites[k].poly = null;
		}
		if (sites.Count == 1)
		{
			sites[0].poly = this.site.poly;
			sites[0].position = sites[0].poly.Centroid();
			return true;
		}
		HashSet<VoronoiDiagram.Site> hashSet2 = new HashSet<VoronoiDiagram.Site>();
		for (int l = 0; l < sites.Count; l++)
		{
			hashSet2.Add(new VoronoiDiagram.Site(sites[l].id, sites[l].position, sites[l].weight));
		}
		hashSet2.Add(new VoronoiDiagram.Site(VoronoiNode.maxIndex + 1U, new Vector2(this.site.poly.bounds.xMin - 500f, this.site.poly.bounds.yMin + this.site.poly.bounds.height / 2f), 1f));
		hashSet2.Add(new VoronoiDiagram.Site(VoronoiNode.maxIndex + 2U, new Vector2(this.site.poly.bounds.xMax + 500f, this.site.poly.bounds.yMin + this.site.poly.bounds.height / 2f), 1f));
		hashSet2.Add(new VoronoiDiagram.Site(VoronoiNode.maxIndex + 3U, new Vector2(this.site.poly.bounds.xMin + this.site.poly.bounds.width / 2f, this.site.poly.bounds.yMin - 500f), 1f));
		hashSet2.Add(new VoronoiDiagram.Site(VoronoiNode.maxIndex + 4U, new Vector2(this.site.poly.bounds.xMin + this.site.poly.bounds.width / 2f, this.site.poly.bounds.yMax + 500f), 1f));
		Rect rect = new Rect(this.site.poly.bounds.xMin - 500f, this.site.poly.bounds.yMin - 500f, this.site.poly.bounds.width + 500f, this.site.poly.bounds.height + 500f);
		VoronoiDiagram voronoiDiagram = new VoronoiDiagram(rect, hashSet2);
		for (int m = 0; m < sites.Count; m++)
		{
			if (sites[m].id <= VoronoiNode.maxIndex)
			{
				List<Vector2> list3 = voronoiDiagram.diagram.Region(sites[m].position);
				if (list3 == null)
				{
					if (this.type != VoronoiNode.NodeType.Leaf)
					{
						this.visited = VoronoiNode.VisitedType.Error;
						return false;
					}
				}
				else
				{
					Polygon polygon = new Polygon(list3).Clip(this.site.poly, ClipType.ctIntersection);
					if (polygon == null || polygon.Vertices.Count < 3)
					{
						if (this.type != VoronoiNode.NodeType.Leaf)
						{
							this.visited = VoronoiNode.VisitedType.Error;
							return false;
						}
					}
					else
					{
						sites[m].poly = polygon;
					}
				}
			}
		}
		for (int n = 0; n < sites.Count; n++)
		{
			if (sites[n].id <= VoronoiNode.maxIndex)
			{
				HashSet<uint> hashSet3 = voronoiDiagram.diagram.NeighborSitesIDsForSite(sites[n].position);
				VoronoiNode.FilterNeighbours(sites[n], hashSet3, sites);
				sites[n].position = sites[n].poly.Centroid();
			}
		}
		return true;
	}

	private static void FilterNeighbours(VoronoiDiagram.Site home, HashSet<uint> neighbours, List<VoronoiDiagram.Site> sites)
	{
		if (home == null)
		{
			Debug.LogError("FilterNeighbours home == null");
		}
		HashSet<KeyValuePair<uint, int>> hashSet = new HashSet<KeyValuePair<uint, int>>();
		HashSet<uint>.Enumerator niter = neighbours.GetEnumerator();
		while (niter.MoveNext())
		{
			VoronoiDiagram.Site site = sites.Find((VoronoiDiagram.Site s) => s.id == niter.Current);
			if (site != null)
			{
				if (site.poly == null)
				{
					Debug.LogError("FilterNeighbours neighbour.poly == null");
				}
				int num = -1;
				Polygon.Commonality commonality = home.poly.SharesEdge(site.poly, ref num);
				if (commonality == Polygon.Commonality.Edge)
				{
					hashSet.Add(new KeyValuePair<uint, int>(niter.Current, num));
				}
			}
		}
		home.neighbours = hashSet;
	}

	protected static Color GetColour(int depth, VoronoiNode.NodeType nt, uint id, VoronoiNode.VisitedType visited)
	{
		Color color = Color.magenta;
		if ((VoronoiNode.drawOptions & VoronoiNode.DebugFlags.ColourByType) != (VoronoiNode.DebugFlags)0)
		{
			if (nt != VoronoiNode.NodeType.Internal)
			{
				if (nt != VoronoiNode.NodeType.Leaf)
				{
					color = Color.red;
				}
				else
				{
					color = Color.yellow;
				}
			}
			else
			{
				color = Color.green;
			}
		}
		else if ((VoronoiNode.drawOptions & VoronoiNode.DebugFlags.ColourByDepth) != (VoronoiNode.DebugFlags)0)
		{
			if (depth == -1)
			{
				color = Color.black;
			}
			else if (depth == -2)
			{
				color = Color.white;
			}
			else if (depth == -3)
			{
				color = Color.yellow;
			}
			else
			{
				ColourHSV colourHSV = new ColourHSV((float)depth / (float)VoronoiNode.maxDepth * 360f, 1f, 1f);
				color = colourHSV.ToColor();
			}
		}
		else if ((VoronoiNode.drawOptions & VoronoiNode.DebugFlags.ColourById) != (VoronoiNode.DebugFlags)0)
		{
			ColourHSV colourHSV2 = new ColourHSV(id / VoronoiNode.maxIndex * 360f, 1f, 1f);
			color = colourHSV2.ToColor();
		}
		else if ((VoronoiNode.drawOptions & VoronoiNode.DebugFlags.ColourByVisited) != (VoronoiNode.DebugFlags)0)
		{
			switch (visited + 2)
			{
			case VoronoiNode.VisitedType.NotVisited:
				color = Color.yellow;
				break;
			case VoronoiNode.VisitedType.VisitedSuccess:
				color = Color.red;
				break;
			case (VoronoiNode.VisitedType)2:
				color = Color.blue;
				break;
			default:
				color = Color.green;
				break;
			}
		}
		return color;
	}

	public void Reset(List<VoronoiDiagram.Site> sites = null)
	{
		this.visited = VoronoiNode.VisitedType.NotVisited;
		if (sites != null)
		{
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			for (int i = 0; i < sites.Count; i++)
			{
				if (hashSet.Contains(sites[i].position))
				{
					this.visited = VoronoiNode.VisitedType.Error;
					return;
				}
				hashSet.Add(sites[i].position);
			}
		}
	}

	public void SetTags(TagSet originalTags)
	{
		this.tags = new TagSet(originalTags);
	}

	public void AddTag(Tag tag)
	{
		if (this.tags == null)
		{
			this.tags = new TagSet();
		}
		this.tags.Add(tag);
	}

	public void AddTagToNeighbors(Tag tag)
	{
		foreach (KeyValuePair<uint, int> keyValuePair in this.site.neighbours)
		{
			this.GetNeighbour(keyValuePair.Key).AddTag(tag);
		}
	}

	public void PushTagsToSiteNode()
	{
		Node node;
		if (this.tags.Contains(WorldGenTags.Overworld))
		{
			node = WorldGen.WorldLayout.overworldGraph.FindNodeByID(this.site.id);
		}
		else
		{
			node = WorldGen.WorldLayout.localGraph.FindNodeByID(this.site.id);
		}
		if (node != null)
		{
			node.tags.Union(this.tags);
		}
	}

	public virtual VoronoiTree Split(VoronoiNode.SplitType splitType = (VoronoiNode.SplitType)0, TagSet dontCopyTags = null, TagSet moveTags = null, VoronoiNode.NodeTypeOverride typeOverride = null)
	{
		return null;
	}

	public virtual void Draw(int depth = 0)
	{
		if (depth > VoronoiNode.maxDepth || this.site.poly == null)
		{
			return;
		}
		bool flag = this.type == VoronoiNode.NodeType.Internal && (VoronoiNode.drawOptions & VoronoiNode.DebugFlags.Internal) != (VoronoiNode.DebugFlags)0;
		if (!(flag | (this.type == VoronoiNode.NodeType.Leaf && (VoronoiNode.drawOptions & VoronoiNode.DebugFlags.Leaf) != (VoronoiNode.DebugFlags)0)))
		{
			return;
		}
		Color color = VoronoiNode.GetColour(depth, this.type, this.site.id, this.visited);
		if ((VoronoiNode.drawOptions & VoronoiNode.DebugFlags.ColourByWinding) != (VoronoiNode.DebugFlags)0)
		{
			Winding winding = this.site.poly.Winding();
			Winding winding2 = winding;
			if (winding2 != Winding.CLOCKWISE)
			{
				if (winding2 != Winding.COUNTERCLOCKWISE)
				{
					color = Color.red;
				}
				else
				{
					color = Color.green;
				}
			}
			else
			{
				color = Color.blue;
			}
		}
		if ((VoronoiNode.drawOptions & VoronoiNode.DebugFlags.ColourByConvex) != (VoronoiNode.DebugFlags)0)
		{
			color = ((!this.site.poly.IsConvex()) ? Color.red : Color.green);
		}
		if ((VoronoiNode.drawOptions & VoronoiNode.DebugFlags.ColourBySiteType) != (VoronoiNode.DebugFlags)0)
		{
			Node node = WorldGen.WorldLayout.localGraph.FindNodeByID(this.site.id);
			if (node != null)
			{
				color = Graph.GetColourForNodeType(node.type);
			}
		}
		else if ((VoronoiNode.drawOptions & VoronoiNode.DebugFlags.ColourByTNLocation) != (VoronoiNode.DebugFlags)0)
		{
			Node node2 = WorldGen.WorldLayout.localGraph.FindNode((Node n) => n.position == this.site.position);
			if (node2 != null)
			{
				color = Color.green;
			}
			else
			{
				node2 = WorldGen.WorldLayout.overworldGraph.FindNode((Node n) => n.position == this.site.position);
				if (node2 != null)
				{
					color = Color.blue;
				}
				else
				{
					color = Color.red;
				}
			}
		}
		this.site.poly.DebugDraw(color, (VoronoiNode.drawOptions & VoronoiNode.DebugFlags.Centroid) != (VoronoiNode.DebugFlags)0, 1f, 0.9f);
	}

	[EnumFlags]
	public static VoronoiNode.DebugFlags drawOptions;

	public static int maxDepth;

	public static uint maxIndex;

	[Serialize]
	public VoronoiNode.NodeType type;

	public VoronoiNode.VisitedType visited;

	[Serialize]
	public VoronoiDiagram.Site site;

	[Serialize]
	public TagSet tags;

	[Flags]
	public enum DebugFlags
	{
		Internal = 1,
		Leaf = 2,
		Site = 4,
		Centroid = 8,
		ColourByType = 16,
		ColourByDepth = 32,
		ColourById = 64,
		ColourByChildCount = 128,
		ColourByVisited = 256,
		ColourByWinding = 512,
		ColourByConvex = 1024,
		ColourBySiteType = 2048,
		ColourByTNLocation = 4096
	}

	public enum NodeType
	{
		Unknown,
		Internal,
		Leaf
	}

	public enum VisitedType
	{
		MissingData = -2,
		Error,
		NotVisited,
		VisitedSuccess
	}

	public enum SplitType
	{
		KeepParentAsCentroid = 1,
		ChildrenDuplicateParent,
		ChildrenChosenFromLayer = 4
	}

	public delegate string NodeTypeOverride(Vector2 position);
}
