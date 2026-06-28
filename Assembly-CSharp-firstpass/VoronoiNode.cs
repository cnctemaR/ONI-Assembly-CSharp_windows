using System;
using System.Collections.Generic;
using ClipperLib;
using Delaunay.Geo;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class VoronoiNode
{
	public VoronoiNode()
	{
		this.type = VoronoiNode.NodeType.Unknown;
		this.log = new LoggerSSF("VoronoiNode", 35);
	}

	public VoronoiNode(VoronoiNode.NodeType type)
	{
		this.type = type;
		this.tags = new TagSet();
		this.log = new LoggerSSF("VoronoiNode", 35);
	}

	protected VoronoiNode(VoronoiDiagram.Site site, VoronoiNode.NodeType type, VoronoiTree parent)
	{
		this.tags = new TagSet();
		this.site = site;
		this.type = type;
		this.parent = parent;
		this.log = new LoggerSSF("VoronoiNode", 35);
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
		if (this.site.neighbours != null)
		{
			HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = this.site.neighbours.GetEnumerator();
			while (enumerator.MoveNext())
			{
				List<VoronoiNode> list2 = list;
				KeyValuePair<uint, int> keyValuePair = enumerator.Current;
				list2.Add(this.GetSibling(keyValuePair.Key));
			}
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

	public bool ComputeNode(List<VoronoiDiagram.Site> sites, int seed)
	{
		if (this.site.poly == null || sites == null || sites.Count == 0)
		{
			this.visited = VoronoiNode.VisitedType.MissingData;
			return false;
		}
		this.visited = VoronoiNode.VisitedType.VisitedSuccess;
		SeededRandom seededRandom = new SeededRandom(seed);
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
					list = PointGenerator.GetRandomPoints(this.site.poly, 5f, 1f, list2, PointGenerator.SampleBehaviour.PoissonDisk, true, seededRandom, true, true);
				}
				if (num >= list.Count - 1)
				{
					list2.AddRange(list);
					list = PointGenerator.GetRandomPoints(this.site.poly, 0.5f, 0.5f, list2, PointGenerator.SampleBehaviour.PoissonDisk, true, seededRandom, true, true);
					num = 0;
				}
				if (list.Count == 0)
				{
					sites[j].position = sites[0].position + Vector2.one * seededRandom.RandomValue();
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
				sites[k].position += new Vector2((float)seededRandom.RandomRange(0, 1), (float)seededRandom.RandomRange(0, 1));
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
			global::Debug.LogError("FilterNeighbours home == null", null);
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
					global::Debug.LogError("FilterNeighbours neighbour.poly == null", null);
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

	public virtual VoronoiTree Split(VoronoiNode.SplitCommand cmd = null)
	{
		return null;
	}

	public static int maxDepth;

	public static uint maxIndex;

	[Serialize]
	public VoronoiNode.NodeType type;

	public VoronoiNode.VisitedType visited;

	public LoggerSSF log;

	[Serialize]
	public VoronoiDiagram.Site site;

	[Serialize]
	public TagSet tags;

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

	public class SplitCommand
	{
		public VoronoiNode.SplitCommand.SplitType splitType;

		public TagSet dontCopyTags;

		public TagSet moveTags;

		public int minChildCount = 2;

		public VoronoiNode.SplitCommand.NodeTypeOverride typeOverride;

		public Action<VoronoiTree, VoronoiNode.SplitCommand> SplitFunction;

		public enum SplitType
		{
			KeepParentAsCentroid = 1,
			ChildrenDuplicateParent,
			ChildrenChosenFromLayer = 4
		}

		public delegate string NodeTypeOverride(Vector2 position);
	}
}
