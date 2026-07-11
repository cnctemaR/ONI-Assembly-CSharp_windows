using System;
using System.Collections.Generic;
using Delaunay.Geo;
using Delaunay.LR;
using UnityEngine;

namespace Delaunay
{
	public sealed class Site : ICoord, IComparable
	{
		private Site(Vector2 p, uint index, float weight, uint color)
		{
			this.Init(p, index, weight, color);
		}

		public uint color { get; private set; }

		public float weight { get; private set; }

		internal List<Edge> edges
		{
			get
			{
				return this._edges;
			}
		}

		public float x
		{
			get
			{
				return this._coord.x;
			}
		}

		internal float y
		{
			get
			{
				return this._coord.y;
			}
		}

		public Vector2 Coord
		{
			get
			{
				return this._coord;
			}
		}

		public float Dist(ICoord p)
		{
			return Vector2.Distance(p.Coord, this._coord);
		}

		public override string ToString()
		{
			return "Site " + this._siteIndex.ToString() + ": " + this.Coord.ToString();
		}

		public static Site Create(Vector2 p, uint index, float weight, uint color)
		{
			if (Site._pool.Count > 0)
			{
				return Site._pool.Pop().Init(p, index, weight, color);
			}
			return new Site(p, index, weight, color);
		}

		internal static void SortSites(List<Site> sites)
		{
			sites.Sort();
		}

		public int CompareTo(object obj)
		{
			Site site = (Site)obj;
			int num = Voronoi.CompareByYThenX(this, site);
			if (num == -1)
			{
				if (this._siteIndex > site._siteIndex)
				{
					uint num2 = this._siteIndex;
					this._siteIndex = site._siteIndex;
					site._siteIndex = num2;
				}
			}
			else if (num == 1 && site._siteIndex > this._siteIndex)
			{
				uint num2 = site._siteIndex;
				site._siteIndex = this._siteIndex;
				this._siteIndex = num2;
			}
			return num;
		}

		private static bool CloseEnough(Vector2 p0, Vector2 p1)
		{
			return Vector2.Distance(p0, p1) < Site.EPSILON;
		}

		private Site Init(Vector2 p, uint index, float weight, uint color)
		{
			this.scaled_weight = -1f;
			this._coord = p;
			this._siteIndex = index;
			this.weight = weight;
			this.color = color;
			this._edges = new List<Edge>();
			this._region = null;
			return this;
		}

		private void Move(Vector2 p)
		{
			this.Clear();
			this._coord = p;
		}

		public void Dispose()
		{
			this.Clear();
			Site._pool.Push(this);
		}

		private void Clear()
		{
			if (this._edges != null)
			{
				this._edges.Clear();
				this._edges = null;
			}
			if (this._edgeOrientations != null)
			{
				this._edgeOrientations.Clear();
				this._edgeOrientations = null;
			}
			if (this._region != null)
			{
				this._region.Clear();
				this._region = null;
			}
		}

		public void AddEdge(Edge edge)
		{
			this._edges.Add(edge);
		}

		public Vector2 GetClosestPt(Vector2 p)
		{
			Vector2 normalized = (p - this._coord).normalized;
			return this._coord + normalized * this.weight;
		}

		public Edge NearestEdge()
		{
			this._edges.Sort((Edge a, Edge b) => Edge.CompareSitesDistances(a, b));
			return this._edges[0];
		}

		public List<Site> NeighborSites()
		{
			if (this._edges == null || this._edges.Count == 0)
			{
				return new List<Site>();
			}
			if (this._edgeOrientations == null)
			{
				this.ReorderEdges();
			}
			List<Site> list = new List<Site>();
			for (int i = 0; i < this._edges.Count; i++)
			{
				Edge edge = this._edges[i];
				list.Add(this.NeighborSite(edge));
			}
			return list;
		}

		private Site NeighborSite(Edge edge)
		{
			if (this == edge.leftSite)
			{
				return edge.rightSite;
			}
			if (this == edge.rightSite)
			{
				return edge.leftSite;
			}
			return null;
		}

		internal List<Vector2> Region(Rect clippingBounds)
		{
			if (this._edges == null || this._edges.Count == 0)
			{
				return new List<Vector2>();
			}
			if (this._edgeOrientations == null)
			{
				this.ReorderEdges();
				this._region = this.ClipToBounds(clippingBounds);
				if (new Polygon(this._region).Winding() == Winding.CLOCKWISE)
				{
					this._region.Reverse();
				}
			}
			return this._region;
		}

		internal List<Vector2> Region(Polygon clippingBounds)
		{
			if (this._edges == null || this._edges.Count == 0)
			{
				return new List<Vector2>();
			}
			if (this._edgeOrientations == null)
			{
				this.ReorderEdges();
				this._region = this.ClipToBounds(clippingBounds);
				if (new Polygon(this._region).Winding() == Winding.CLOCKWISE)
				{
					this._region.Reverse();
				}
			}
			return this._region;
		}

		private void ReorderEdges()
		{
			EdgeReorderer edgeReorderer = new EdgeReorderer(this._edges, VertexOrSite.VERTEX);
			this._edges = edgeReorderer.edges;
			this._edgeOrientations = edgeReorderer.edgeOrientations;
			edgeReorderer.Dispose();
		}

		private List<Vector2> ClipToBounds(Rect bounds)
		{
			List<Vector2> list = new List<Vector2>();
			int count = this._edges.Count;
			int num = 0;
			while (num < count && !this._edges[num].visible)
			{
				num++;
			}
			if (num == count)
			{
				return new List<Vector2>();
			}
			Edge edge = this._edges[num];
			Side side = this._edgeOrientations[num];
			if (edge.clippedEnds[side] == null)
			{
				global::Debug.LogError("XXX: Null detected when there should be a Vector2!");
			}
			if (edge.clippedEnds[SideHelper.Other(side)] == null)
			{
				global::Debug.LogError("XXX: Null detected when there should be a Vector2!");
			}
			list.Add(edge.clippedEnds[side].Value);
			list.Add(edge.clippedEnds[SideHelper.Other(side)].Value);
			for (int i = num + 1; i < count; i++)
			{
				edge = this._edges[i];
				if (edge.visible)
				{
					this.Connect(list, i, bounds, false);
				}
			}
			this.Connect(list, num, bounds, true);
			return list;
		}

		private List<Vector2> ClipToBounds(Polygon bounds)
		{
			return this.ClipToBounds(bounds.bounds);
		}

		private void Connect(List<Vector2> points, int j, Rect bounds, bool closingUp = false)
		{
			Vector2 vector = points[points.Count - 1];
			Edge edge = this._edges[j];
			Side side = this._edgeOrientations[j];
			if (edge.clippedEnds[side] == null)
			{
				global::Debug.LogError("XXX: Null detected when there should be a Vector2!");
			}
			Vector2 value = edge.clippedEnds[side].Value;
			if (!Site.CloseEnough(vector, value))
			{
				if (vector.x != value.x && vector.y != value.y)
				{
					int num = BoundsCheck.Check(vector, bounds);
					int num2 = BoundsCheck.Check(value, bounds);
					if ((num & BoundsCheck.RIGHT) != 0)
					{
						float num3 = bounds.xMax;
						if ((num2 & BoundsCheck.BOTTOM) != 0)
						{
							float num4 = bounds.yMax;
							points.Add(new Vector2(num3, num4));
						}
						else if ((num2 & BoundsCheck.TOP) != 0)
						{
							float num4 = bounds.yMin;
							points.Add(new Vector2(num3, num4));
						}
						else if ((num2 & BoundsCheck.LEFT) != 0)
						{
							float num4;
							if (vector.y - bounds.y + value.y - bounds.y < bounds.height)
							{
								num4 = bounds.yMin;
							}
							else
							{
								num4 = bounds.yMax;
							}
							points.Add(new Vector2(num3, num4));
							points.Add(new Vector2(bounds.xMin, num4));
						}
					}
					else if ((num & BoundsCheck.LEFT) != 0)
					{
						float num3 = bounds.xMin;
						if ((num2 & BoundsCheck.BOTTOM) != 0)
						{
							float num4 = bounds.yMax;
							points.Add(new Vector2(num3, num4));
						}
						else if ((num2 & BoundsCheck.TOP) != 0)
						{
							float num4 = bounds.yMin;
							points.Add(new Vector2(num3, num4));
						}
						else if ((num2 & BoundsCheck.RIGHT) != 0)
						{
							float num4;
							if (vector.y - bounds.y + value.y - bounds.y < bounds.height)
							{
								num4 = bounds.yMin;
							}
							else
							{
								num4 = bounds.yMax;
							}
							points.Add(new Vector2(num3, num4));
							points.Add(new Vector2(bounds.xMax, num4));
						}
					}
					else if ((num & BoundsCheck.TOP) != 0)
					{
						float num4 = bounds.yMin;
						if ((num2 & BoundsCheck.RIGHT) != 0)
						{
							float num3 = bounds.xMax;
							points.Add(new Vector2(num3, num4));
						}
						else if ((num2 & BoundsCheck.LEFT) != 0)
						{
							float num3 = bounds.xMin;
							points.Add(new Vector2(num3, num4));
						}
						else if ((num2 & BoundsCheck.BOTTOM) != 0)
						{
							float num3;
							if (vector.x - bounds.x + value.x - bounds.x < bounds.width)
							{
								num3 = bounds.xMin;
							}
							else
							{
								num3 = bounds.xMax;
							}
							points.Add(new Vector2(num3, num4));
							points.Add(new Vector2(num3, bounds.yMax));
						}
					}
					else if ((num & BoundsCheck.BOTTOM) != 0)
					{
						float num4 = bounds.yMax;
						if ((num2 & BoundsCheck.RIGHT) != 0)
						{
							float num3 = bounds.xMax;
							points.Add(new Vector2(num3, num4));
						}
						else if ((num2 & BoundsCheck.LEFT) != 0)
						{
							float num3 = bounds.xMin;
							points.Add(new Vector2(num3, num4));
						}
						else if ((num2 & BoundsCheck.TOP) != 0)
						{
							float num3;
							if (vector.x - bounds.x + value.x - bounds.x < bounds.width)
							{
								num3 = bounds.xMin;
							}
							else
							{
								num3 = bounds.xMax;
							}
							points.Add(new Vector2(num3, num4));
							points.Add(new Vector2(num3, bounds.yMin));
						}
					}
				}
				if (closingUp)
				{
					return;
				}
				points.Add(value);
			}
			if (edge.clippedEnds[SideHelper.Other(side)] == null)
			{
				global::Debug.LogError("XXX: Null detected when there should be a Vector2!");
			}
			Vector2 value2 = edge.clippedEnds[SideHelper.Other(side)].Value;
			if (!Site.CloseEnough(points[0], value2))
			{
				points.Add(value2);
			}
		}

		private static Stack<Site> _pool = new Stack<Site>();

		private static readonly float EPSILON = 0.005f;

		private Vector2 _coord;

		public float scaled_weight;

		private uint _siteIndex;

		private List<Edge> _edges;

		private List<Side> _edgeOrientations;

		private List<Vector2> _region;
	}
}
