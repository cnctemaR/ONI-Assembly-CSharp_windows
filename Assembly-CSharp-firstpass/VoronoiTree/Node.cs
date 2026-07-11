using System;
using System.Collections.Generic;
using ClipperLib;
using Delaunay.Geo;
using KSerialization;
using UnityEngine;

namespace VoronoiTree
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Node
	{
		public Node()
		{
			this.type = Node.NodeType.Unknown;
			this.log = new LoggerSSF("VoronoiNode", 35);
		}

		public Node(Node.NodeType type)
		{
			this.type = type;
			this.tags = new TagSet();
			this.log = new LoggerSSF("VoronoiNode", 35);
		}

		protected Node(Diagram.Site site, Node.NodeType type, Tree parent)
		{
			this.tags = new TagSet();
			this.site = site;
			this.type = type;
			this.parent = parent;
			this.log = new LoggerSSF("VoronoiNode", 35);
		}

		public Tree parent { get; private set; }

		public void SetParent(Tree newParent)
		{
			this.parent = newParent;
		}

		public Node GetNeighbour(uint id)
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

		public int DistanceToTag(int dist, int maxDist, TagSet targetTags, HashSet<Node> nvis)
		{
			if (nvis.Contains(this))
			{
				return -1;
			}
			nvis.Add(this);
			if (this.tags.ContainsOne(targetTags))
			{
				return dist;
			}
			if (maxDist == 0)
			{
				return -1;
			}
			maxDist--;
			dist++;
			List<Node> neighbors = this.GetNeighbors();
			int num = -1;
			for (int i = 0; i < neighbors.Count; i++)
			{
				if (!nvis.Contains(neighbors[i]))
				{
					int num2 = neighbors[i].DistanceToTag(dist, maxDist, targetTags, nvis);
					if (num2 != -1)
					{
						if (num == -1 || num2 < num)
						{
							num = num2;
						}
					}
				}
			}
			return num;
		}

		public List<Node> GetNeighbors()
		{
			List<Node> list = new List<Node>();
			if (this.site.neighbours != null)
			{
				HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = this.site.neighbours.GetEnumerator();
				while (enumerator.MoveNext())
				{
					List<Node> list2 = list;
					KeyValuePair<uint, int> keyValuePair = enumerator.Current;
					list2.Add(this.GetSibling(keyValuePair.Key));
				}
			}
			return list;
		}

		public List<KeyValuePair<Node, LineSegment>> GetNeighborsByEdge()
		{
			List<KeyValuePair<Node, LineSegment>> list = new List<KeyValuePair<Node, LineSegment>>();
			for (int i = 0; i < this.site.poly.Vertices.Count; i++)
			{
				if (this.site.neighbours != null)
				{
					LineSegment edge = this.site.poly.GetEdge(i);
					Node node = null;
					foreach (KeyValuePair<uint, int> keyValuePair in this.site.neighbours)
					{
						if (keyValuePair.Value == i)
						{
							HashSet<KeyValuePair<uint, int>>.Enumerator enumerator;
							KeyValuePair<uint, int> keyValuePair2 = enumerator.Current;
							node = this.GetSibling(keyValuePair2.Key);
						}
					}
					if (node != null)
					{
						list.Add(new KeyValuePair<Node, LineSegment>(node, edge));
					}
				}
			}
			return list;
		}

		public Node GetSibling(uint siteId)
		{
			return this.parent.GetChildByID(siteId);
		}

		public List<Node> GetSiblings()
		{
			List<Node> list = new List<Node>();
			for (int i = 0; i < this.parent.ChildCount(); i++)
			{
				Node child = this.parent.GetChild(i);
				if (child != this)
				{
					list.Add(child);
				}
			}
			return list;
		}

		public void PlaceSites(List<Diagram.Site> sites, int seed)
		{
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
					this.visited = Node.VisitedType.Error;
					sites[k].position += new Vector2((float)seededRandom.RandomRange(0, 1), (float)seededRandom.RandomRange(0, 1));
				}
				hashSet.Add(sites[k].position);
				sites[k].poly = null;
			}
		}

		public bool ComputeNode(List<Diagram.Site> sites)
		{
			if (this.site.poly == null || sites == null || sites.Count == 0)
			{
				this.visited = Node.VisitedType.MissingData;
				return false;
			}
			this.visited = Node.VisitedType.VisitedSuccess;
			if (sites.Count == 1)
			{
				sites[0].poly = this.site.poly;
				sites[0].position = sites[0].poly.Centroid();
				return true;
			}
			HashSet<Diagram.Site> hashSet = new HashSet<Diagram.Site>();
			for (int i = 0; i < sites.Count; i++)
			{
				hashSet.Add(new Diagram.Site(sites[i].id, sites[i].position, sites[i].weight));
			}
			hashSet.Add(new Diagram.Site(Node.maxIndex + 1U, new Vector2(this.site.poly.bounds.xMin - 500f, this.site.poly.bounds.yMin + this.site.poly.bounds.height / 2f), 1f));
			hashSet.Add(new Diagram.Site(Node.maxIndex + 2U, new Vector2(this.site.poly.bounds.xMax + 500f, this.site.poly.bounds.yMin + this.site.poly.bounds.height / 2f), 1f));
			hashSet.Add(new Diagram.Site(Node.maxIndex + 3U, new Vector2(this.site.poly.bounds.xMin + this.site.poly.bounds.width / 2f, this.site.poly.bounds.yMin - 500f), 1f));
			hashSet.Add(new Diagram.Site(Node.maxIndex + 4U, new Vector2(this.site.poly.bounds.xMin + this.site.poly.bounds.width / 2f, this.site.poly.bounds.yMax + 500f), 1f));
			Rect rect = new Rect(this.site.poly.bounds.xMin - 500f, this.site.poly.bounds.yMin - 500f, this.site.poly.bounds.width + 500f, this.site.poly.bounds.height + 500f);
			Diagram diagram = new Diagram(rect, hashSet);
			for (int j = 0; j < sites.Count; j++)
			{
				if (sites[j].id <= Node.maxIndex)
				{
					List<Vector2> list = diagram.diagram.Region(sites[j].position);
					if (list == null)
					{
						if (this.type != Node.NodeType.Leaf)
						{
							this.visited = Node.VisitedType.Error;
							return false;
						}
					}
					else
					{
						Polygon polygon = new Polygon(list).Clip(this.site.poly, ClipType.ctIntersection);
						if (polygon == null || polygon.Vertices.Count < 3)
						{
							if (this.type != Node.NodeType.Leaf)
							{
								this.visited = Node.VisitedType.Error;
								return false;
							}
						}
						else
						{
							sites[j].poly = polygon;
						}
					}
				}
			}
			for (int k = 0; k < sites.Count; k++)
			{
				if (sites[k].id <= Node.maxIndex)
				{
					HashSet<uint> hashSet2 = diagram.diagram.NeighborSitesIDsForSite(sites[k].position);
					Node.FilterNeighbours(sites[k], hashSet2, sites);
					sites[k].position = sites[k].poly.Centroid();
				}
			}
			return true;
		}

		public bool ComputeNodePD(List<Diagram.Site> sites, int maxIters = 500, float threashold = 0.2f)
		{
			if (this.site.poly == null || sites == null || sites.Count == 0)
			{
				this.visited = Node.VisitedType.MissingData;
				return false;
			}
			this.visited = Node.VisitedType.VisitedSuccess;
			List<Site> list = new List<Site>();
			for (int i = 0; i < sites.Count; i++)
			{
				Site site = new Site(sites[i].id, sites[i].position, sites[i].weight);
				list.Add(site);
			}
			PowerDiagram powerDiagram = new PowerDiagram(this.site.poly, list);
			powerDiagram.ComputeVD();
			powerDiagram.ComputePowerDiagram(maxIters, threashold);
			for (int j = 0; j < sites.Count; j++)
			{
				sites[j].poly = list[j].poly;
				if (sites[j].poly == null)
				{
					global::Debug.LogErrorFormat("Site [{0}] at index [{1}]: Poly shouldnt be null here ever", new object[]
					{
						sites[j].id,
						j
					});
				}
				HashSet<uint> hashSet = new HashSet<uint>();
				for (int k = 0; k < list[j].neighbours.Count; k++)
				{
					if (!list[j].neighbours[k].dummy)
					{
						hashSet.Add((uint)list[j].neighbours[k].id);
					}
				}
				sites[j].position = sites[j].poly.Centroid();
			}
			return true;
		}

		private static void FilterNeighbours(Diagram.Site home, HashSet<uint> neighbours, List<Diagram.Site> sites)
		{
			if (home == null)
			{
				global::Debug.LogError("FilterNeighbours home == null");
			}
			HashSet<KeyValuePair<uint, int>> hashSet = new HashSet<KeyValuePair<uint, int>>();
			HashSet<uint>.Enumerator niter = neighbours.GetEnumerator();
			while (niter.MoveNext())
			{
				Diagram.Site site = sites.Find((Diagram.Site s) => s.id == niter.Current);
				if (site != null)
				{
					if (site.poly == null)
					{
						global::Debug.LogError("FilterNeighbours neighbour.poly == null");
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

		public void Reset(List<Diagram.Site> sites = null)
		{
			this.visited = Node.VisitedType.NotVisited;
			if (sites != null)
			{
				HashSet<Vector2> hashSet = new HashSet<Vector2>();
				for (int i = 0; i < sites.Count; i++)
				{
					if (hashSet.Contains(sites[i].position))
					{
						this.visited = Node.VisitedType.Error;
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

		public virtual Tree Split(Node.SplitCommand cmd = null)
		{
			return null;
		}

		public static int maxDepth;

		public static uint maxIndex;

		[Serialize]
		public Node.NodeType type;

		public Node.VisitedType visited;

		public LoggerSSF log;

		[Serialize]
		public Diagram.Site site;

		[Serialize]
		public TagSet tags;

		public Dictionary<Tag, int> minDistaceToTag = new Dictionary<Tag, int>();

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
			public Node.SplitCommand.SplitType splitType;

			public TagSet dontCopyTags;

			public TagSet moveTags;

			public int minChildCount = 2;

			public Node.SplitCommand.NodeTypeOverride typeOverride;

			public Action<Tree, Node.SplitCommand> SplitFunction;

			public enum SplitType
			{
				KeepParentAsCentroid = 1,
				ChildrenDuplicateParent,
				ChildrenChosenFromLayer = 4
			}

			public delegate string NodeTypeOverride(Vector2 position);
		}
	}
}
