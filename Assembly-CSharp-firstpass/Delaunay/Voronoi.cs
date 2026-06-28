using System;
using System.Collections.Generic;
using Delaunay.Geo;
using Delaunay.LR;
using Delaunay.Utils;
using UnityEngine;

namespace Delaunay
{
	public sealed class Voronoi : Delaunay.Utils.IDisposable
	{
		public Voronoi(List<Vector2> points, List<uint> colors, List<float> weights, Rect plotBounds)
		{
			this._plotBounds = plotBounds;
			this.min_weight = float.MaxValue;
			this.max_weight = float.MinValue;
			this._sites = new SiteList();
			this._sitesIndexedByLocation = new Dictionary<Vector2, Site>();
			this._triangles = new List<Triangle>();
			this._edges = new List<Edge>();
			this.AddSites(points, colors, weights);
			float num = this.max_weight - this.min_weight;
			if (num > 0f)
			{
				this._sites.ScaleWeight(1f + num);
			}
			this.FortunesAlgorithm();
		}

		public Rect plotBounds
		{
			get
			{
				return this._plotBounds;
			}
		}

		public void Dispose()
		{
			if (this._sites != null)
			{
				this._sites.Dispose();
				this._sites = null;
			}
			if (this._triangles != null)
			{
				int num = this._triangles.Count;
				for (int i = 0; i < num; i++)
				{
					this._triangles[i].Dispose();
				}
				this._triangles.Clear();
				this._triangles = null;
			}
			if (this._edges != null)
			{
				int num = this._edges.Count;
				for (int i = 0; i < num; i++)
				{
					this._edges[i].Dispose();
				}
				this._edges.Clear();
				this._edges = null;
			}
			this._sitesIndexedByLocation = null;
		}

		private void AddSites(List<Vector2> points, List<uint> colors, List<float> weights)
		{
			this.weightSum = 0f;
			for (int i = 0; i < points.Count; i++)
			{
				this.AddSite(points[i], (colors == null) ? 0U : colors[i], i, (weights != null) ? weights[i] : 1f);
			}
		}

		private void AddSite(Vector2 p, uint color, int index, float weight = 1f)
		{
			if (!this._sitesIndexedByLocation.ContainsKey(p))
			{
				Site site = Site.Create(p, (uint)index, weight, color);
				this.min_weight = Mathf.Min(this.min_weight, weight);
				this.max_weight = Mathf.Max(this.max_weight, weight);
				this._sitesIndexedByLocation[p] = site;
				this._sites.Add(site);
				this.weightSum += site.weight;
			}
		}

		public Site GetSiteByLocation(Vector2 p)
		{
			return this._sitesIndexedByLocation[p];
		}

		public List<Edge> Edges()
		{
			return this._edges;
		}

		public List<Vector2> Region(Vector2 p)
		{
			Site site = this._sitesIndexedByLocation[p];
			List<Vector2> list;
			if (site == null)
			{
				list = new List<Vector2>();
			}
			else
			{
				list = site.Region(this._plotBounds);
			}
			return list;
		}

		public List<Vector2> NeighborSitesForSite(Vector2 coord)
		{
			List<Vector2> list = new List<Vector2>();
			Site site = this._sitesIndexedByLocation[coord];
			List<Vector2> list2;
			if (site == null)
			{
				list2 = list;
			}
			else
			{
				List<Site> list3 = site.NeighborSites();
				for (int i = 0; i < list3.Count; i++)
				{
					Site site2 = list3[i];
					list.Add(site2.Coord);
				}
				list2 = list;
			}
			return list2;
		}

		public HashSet<uint> NeighborSitesIDsForSite(Vector2 coord)
		{
			HashSet<uint> hashSet = new HashSet<uint>();
			Site site = this._sitesIndexedByLocation[coord];
			HashSet<uint> hashSet2;
			if (site == null)
			{
				hashSet2 = hashSet;
			}
			else
			{
				List<Site> list = site.NeighborSites();
				for (int i = 0; i < list.Count; i++)
				{
					hashSet.Add(list[i].color);
				}
				hashSet2 = hashSet;
			}
			return hashSet2;
		}

		public List<uint> ListNeighborSitesIDsForSite(Vector2 coord)
		{
			List<uint> list = new List<uint>();
			Site site = this._sitesIndexedByLocation[coord];
			List<uint> list2;
			if (site == null)
			{
				list2 = list;
			}
			else
			{
				List<Site> list3 = site.NeighborSites();
				for (int i = 0; i < list3.Count; i++)
				{
					list.Add(list3[i].color);
				}
				list2 = list;
			}
			return list2;
		}

		public List<Circle> Circles()
		{
			return this._sites.Circles();
		}

		public List<LineSegment> VoronoiBoundaryForSite(Vector2 coord)
		{
			return DelaunayHelpers.VisibleLineSegments(DelaunayHelpers.SelectEdgesForSitePoint(coord, this._edges));
		}

		public List<LineSegment> DelaunayLinesForSite(Vector2 coord)
		{
			return DelaunayHelpers.DelaunayLinesForEdges(DelaunayHelpers.SelectEdgesForSitePoint(coord, this._edges));
		}

		public List<LineSegment> VoronoiDiagram()
		{
			return DelaunayHelpers.VisibleLineSegments(this._edges);
		}

		public List<LineSegment> DelaunayTriangulation()
		{
			return DelaunayHelpers.DelaunayLinesForEdges(DelaunayHelpers.SelectNonIntersectingEdges(this._edges));
		}

		public List<LineSegment> Hull()
		{
			return DelaunayHelpers.DelaunayLinesForEdges(this.HullEdges());
		}

		private List<Edge> HullEdges()
		{
			return this._edges.FindAll((Edge edge) => edge.IsPartOfConvexHull());
		}

		public List<Vector2> HullPointsInOrder()
		{
			List<Edge> list = this.HullEdges();
			List<Vector2> list2 = new List<Vector2>();
			List<Vector2> list3;
			if (list.Count == 0)
			{
				list3 = list2;
			}
			else
			{
				EdgeReorderer edgeReorderer = new EdgeReorderer(list, VertexOrSite.SITE);
				list = edgeReorderer.edges;
				List<Side> edgeOrientations = edgeReorderer.edgeOrientations;
				edgeReorderer.Dispose();
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					Edge edge = list[i];
					Side side = edgeOrientations[i];
					list2.Add(edge.Site(side).Coord);
				}
				list3 = list2;
			}
			return list3;
		}

		public List<LineSegment> SpanningTree(KruskalType type = KruskalType.MINIMUM)
		{
			List<Edge> list = DelaunayHelpers.SelectNonIntersectingEdges(this._edges);
			List<LineSegment> list2 = DelaunayHelpers.DelaunayLinesForEdges(list);
			return DelaunayHelpers.Kruskal(list2, type);
		}

		public List<List<Vector2>> Regions()
		{
			return this._sites.Regions(this._plotBounds);
		}

		public List<uint> SiteColors()
		{
			return this._sites.SiteColors();
		}

		public List<Vector2> SiteCoords()
		{
			return this._sites.SiteCoords();
		}

		private void FortunesAlgorithm()
		{
			Vector2 vector = Vector2.zero;
			Rect sitesBounds = this._sites.GetSitesBounds();
			int num = (int)Mathf.Sqrt((float)(this._sites.Count + 4));
			HalfedgePriorityQueue halfedgePriorityQueue = new HalfedgePriorityQueue(sitesBounds.y, sitesBounds.height, num);
			EdgeList edgeList = new EdgeList(sitesBounds.x, sitesBounds.width, num);
			List<Halfedge> list = new List<Halfedge>();
			List<Vertex> list2 = new List<Vertex>();
			this.fortunesAlgorithm_bottomMostSite = this._sites.Next();
			Site site = this._sites.Next();
			for (;;)
			{
				if (!halfedgePriorityQueue.Empty())
				{
					vector = halfedgePriorityQueue.Min();
				}
				if (site != null && (halfedgePriorityQueue.Empty() || Voronoi.CompareByYThenX(site, vector) < 0))
				{
					Halfedge halfedge = edgeList.EdgeListLeftNeighbor(site.Coord);
					Halfedge halfedge2 = halfedge.edgeListRightNeighbor;
					Site site2 = this.FortunesAlgorithm_rightRegion(halfedge);
					Edge edge = Edge.CreateBisectingEdge(site2, site);
					this._edges.Add(edge);
					Halfedge halfedge3 = Halfedge.Create(edge, new Side?(Side.LEFT));
					list.Add(halfedge3);
					edgeList.Insert(halfedge, halfedge3);
					Vertex vertex;
					if ((vertex = Vertex.Intersect(halfedge, halfedge3)) != null)
					{
						list2.Add(vertex);
						halfedgePriorityQueue.Remove(halfedge);
						halfedge.vertex = vertex;
						halfedge.ystar = vertex.y + site.Dist(vertex);
						halfedgePriorityQueue.Insert(halfedge);
					}
					halfedge = halfedge3;
					halfedge3 = Halfedge.Create(edge, new Side?(Side.RIGHT));
					list.Add(halfedge3);
					edgeList.Insert(halfedge, halfedge3);
					if ((vertex = Vertex.Intersect(halfedge3, halfedge2)) != null)
					{
						list2.Add(vertex);
						halfedge3.vertex = vertex;
						halfedge3.ystar = vertex.y + site.Dist(vertex);
						halfedgePriorityQueue.Insert(halfedge3);
					}
					site = this._sites.Next();
				}
				else
				{
					if (halfedgePriorityQueue.Empty())
					{
						break;
					}
					Halfedge halfedge = halfedgePriorityQueue.ExtractMin();
					Halfedge edgeListLeftNeighbor = halfedge.edgeListLeftNeighbor;
					Halfedge halfedge2 = halfedge.edgeListRightNeighbor;
					Halfedge edgeListRightNeighbor = halfedge2.edgeListRightNeighbor;
					Site site2 = this.FortunesAlgorithm_leftRegion(halfedge);
					Site site3 = this.FortunesAlgorithm_rightRegion(halfedge2);
					Vertex vertex2 = halfedge.vertex;
					vertex2.SetIndex();
					Edge edge2 = halfedge.edge;
					Side? leftRight = halfedge.leftRight;
					edge2.SetVertex(leftRight.Value, vertex2);
					Edge edge3 = halfedge2.edge;
					Side? leftRight2 = halfedge2.leftRight;
					edge3.SetVertex(leftRight2.Value, vertex2);
					edgeList.Remove(halfedge);
					halfedgePriorityQueue.Remove(halfedge2);
					edgeList.Remove(halfedge2);
					Side side = Side.LEFT;
					if (site2.y > site3.y)
					{
						Site site4 = site2;
						site2 = site3;
						site3 = site4;
						side = Side.RIGHT;
					}
					Edge edge = Edge.CreateBisectingEdge(site2, site3);
					this._edges.Add(edge);
					Halfedge halfedge3 = Halfedge.Create(edge, new Side?(side));
					list.Add(halfedge3);
					edgeList.Insert(edgeListLeftNeighbor, halfedge3);
					edge.SetVertex(SideHelper.Other(side), vertex2);
					Vertex vertex;
					if ((vertex = Vertex.Intersect(edgeListLeftNeighbor, halfedge3)) != null)
					{
						list2.Add(vertex);
						halfedgePriorityQueue.Remove(edgeListLeftNeighbor);
						edgeListLeftNeighbor.vertex = vertex;
						edgeListLeftNeighbor.ystar = vertex.y + site2.Dist(vertex);
						halfedgePriorityQueue.Insert(edgeListLeftNeighbor);
					}
					if ((vertex = Vertex.Intersect(halfedge3, edgeListRightNeighbor)) != null)
					{
						list2.Add(vertex);
						halfedge3.vertex = vertex;
						halfedge3.ystar = vertex.y + site2.Dist(vertex);
						halfedgePriorityQueue.Insert(halfedge3);
					}
				}
			}
			halfedgePriorityQueue.Dispose();
			edgeList.Dispose();
			for (int i = 0; i < list.Count; i++)
			{
				Halfedge halfedge4 = list[i];
				halfedge4.ReallyDispose();
			}
			list.Clear();
			for (int j = 0; j < this._edges.Count; j++)
			{
				Edge edge = this._edges[j];
				edge.ClipVertices(this._plotBounds);
			}
			for (int k = 0; k < list2.Count; k++)
			{
				Vertex vertex = list2[k];
				vertex.Dispose();
			}
			list2.Clear();
		}

		private Site FortunesAlgorithm_leftRegion(Halfedge he)
		{
			Edge edge = he.edge;
			Site site;
			if (edge == null)
			{
				site = this.fortunesAlgorithm_bottomMostSite;
			}
			else
			{
				Edge edge2 = edge;
				Side? leftRight = he.leftRight;
				site = edge2.Site(leftRight.Value);
			}
			return site;
		}

		private Site FortunesAlgorithm_rightRegion(Halfedge he)
		{
			Edge edge = he.edge;
			Site site;
			if (edge == null)
			{
				site = this.fortunesAlgorithm_bottomMostSite;
			}
			else
			{
				Edge edge2 = edge;
				Side? leftRight = he.leftRight;
				site = edge2.Site(SideHelper.Other(leftRight.Value));
			}
			return site;
		}

		public static int CompareByYThenX(Site s1, Site s2)
		{
			int num;
			if (s1.y < s2.y)
			{
				num = -1;
			}
			else if (s1.y > s2.y)
			{
				num = 1;
			}
			else if (s1.x < s2.x)
			{
				num = -1;
			}
			else if (s1.x > s2.x)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static int CompareByYThenX(Site s1, Vector2 s2)
		{
			int num;
			if (s1.y < s2.y)
			{
				num = -1;
			}
			else if (s1.y > s2.y)
			{
				num = 1;
			}
			else if (s1.x < s2.x)
			{
				num = -1;
			}
			else if (s1.x > s2.x)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		private SiteList _sites;

		private Dictionary<Vector2, Site> _sitesIndexedByLocation;

		private List<Triangle> _triangles;

		private List<Edge> _edges;

		private float min_weight;

		private float max_weight;

		private Rect _plotBounds;

		private float weightSum;

		private Site fortunesAlgorithm_bottomMostSite;
	}
}
