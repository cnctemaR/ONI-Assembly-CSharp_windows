using System;
using System.Collections.Generic;
using Delaunay.Geo;
using Delaunay.LR;
using UnityEngine;

namespace Delaunay
{
	public sealed class Edge
	{
		private Edge()
		{
			this._edgeIndex = Edge._nedges++;
			this.Init();
		}

		public static Edge CreateBisectingEdge(Site site0, Site site1)
		{
			Vector2 coord = site1.Coord;
			Vector2 coord2 = site0.Coord;
			float num = coord2.x - coord.x;
			float num2 = coord2.y - coord.y;
			float num3 = ((num <= 0f) ? (-num) : num);
			float num4 = ((num2 <= 0f) ? (-num2) : num2);
			float num5 = coord.x * num + coord.y * num2 + (num * num + num2 * num2) * 0.5f;
			float num6;
			float num7;
			if (num3 > num4)
			{
				num6 = 1f;
				num7 = num2 / num;
				num5 /= num;
			}
			else
			{
				num7 = 1f;
				num6 = num / num2;
				num5 /= num2;
			}
			Edge edge = Edge.Create();
			edge.leftSite = site0;
			edge.rightSite = site1;
			site0.AddEdge(edge);
			site1.AddEdge(edge);
			edge._leftVertex = null;
			edge._rightVertex = null;
			edge.a = num6;
			edge.b = num7;
			edge.c = num5;
			return edge;
		}

		private static Edge Create()
		{
			Edge edge;
			if (Edge._pool.Count > 0)
			{
				edge = Edge._pool.Pop();
				edge.Init();
			}
			else
			{
				edge = new Edge();
			}
			return edge;
		}

		public LineSegment DelaunayLine()
		{
			return new LineSegment(new Vector2?(this.leftSite.Coord), new Vector2?(this.rightSite.Coord));
		}

		public LineSegment VoronoiEdge()
		{
			LineSegment lineSegment;
			if (!this.visible)
			{
				lineSegment = new LineSegment(null, null);
			}
			else
			{
				lineSegment = new LineSegment(this._clippedVertices[Side.LEFT], this._clippedVertices[Side.RIGHT]);
			}
			return lineSegment;
		}

		public Vertex leftVertex
		{
			get
			{
				return this._leftVertex;
			}
		}

		public Vertex rightVertex
		{
			get
			{
				return this._rightVertex;
			}
		}

		public Vertex Vertex(Side leftRight)
		{
			return (leftRight != Side.LEFT) ? this._rightVertex : this._leftVertex;
		}

		public void SetVertex(Side leftRight, Vertex v)
		{
			if (leftRight == Side.LEFT)
			{
				this._leftVertex = v;
			}
			else
			{
				this._rightVertex = v;
			}
		}

		public bool IsPartOfConvexHull()
		{
			return this._leftVertex == null || this._rightVertex == null;
		}

		public float SitesDistance()
		{
			return Vector2.Distance(this.leftSite.Coord, this.rightSite.Coord) + (this.leftSite.weight + this.rightSite.weight) * (this.leftSite.weight + this.rightSite.weight);
		}

		public static int CompareSitesDistances_MAX(Edge edge0, Edge edge1)
		{
			float num = edge0.SitesDistance();
			float num2 = edge1.SitesDistance();
			int num3;
			if (num < num2)
			{
				num3 = 1;
			}
			else if (num > num2)
			{
				num3 = -1;
			}
			else
			{
				num3 = 0;
			}
			return num3;
		}

		public static int CompareSitesDistances(Edge edge0, Edge edge1)
		{
			return -Edge.CompareSitesDistances_MAX(edge0, edge1);
		}

		public Dictionary<Side, Vector2?> clippedEnds
		{
			get
			{
				return this._clippedVertices;
			}
		}

		public bool visible
		{
			get
			{
				return this._clippedVertices != null;
			}
		}

		public Site leftSite
		{
			get
			{
				return this._sites[Side.LEFT];
			}
			set
			{
				this._sites[Side.LEFT] = value;
			}
		}

		public Site rightSite
		{
			get
			{
				return this._sites[Side.RIGHT];
			}
			set
			{
				this._sites[Side.RIGHT] = value;
			}
		}

		public Site Site(Side leftRight)
		{
			return this._sites[leftRight];
		}

		public void Dispose()
		{
			this._leftVertex = null;
			this._rightVertex = null;
			if (this._clippedVertices != null)
			{
				this._clippedVertices[Side.LEFT] = null;
				this._clippedVertices[Side.RIGHT] = null;
				this._clippedVertices = null;
			}
			this._sites[Side.LEFT] = null;
			this._sites[Side.RIGHT] = null;
			this._sites = null;
			Edge._pool.Push(this);
		}

		private void Init()
		{
			this._sites = new Dictionary<Side, Site>();
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Edge ",
				this._edgeIndex.ToString(),
				"; sites ",
				this._sites[Side.LEFT].ToString(),
				", ",
				this._sites[Side.RIGHT].ToString(),
				"; endVertices ",
				(this._leftVertex == null) ? "null" : this._leftVertex.vertexIndex.ToString(),
				", ",
				(this._rightVertex == null) ? "null" : this._rightVertex.vertexIndex.ToString(),
				"::"
			});
		}

		public void ClipVertices(Rect bounds)
		{
			float xMin = bounds.xMin;
			float yMin = bounds.yMin;
			float xMax = bounds.xMax;
			float yMax = bounds.yMax;
			Vertex vertex;
			Vertex vertex2;
			if ((double)this.a == 1.0 && (double)this.b >= 0.0)
			{
				vertex = this._rightVertex;
				vertex2 = this._leftVertex;
			}
			else
			{
				vertex = this._leftVertex;
				vertex2 = this._rightVertex;
			}
			float num;
			float num2;
			float num3;
			float num4;
			if ((double)this.a == 1.0)
			{
				num = yMin;
				if (vertex != null && vertex.y > yMin)
				{
					num = vertex.y;
				}
				if (num > yMax)
				{
					return;
				}
				num2 = this.c - this.b * num;
				num3 = yMax;
				if (vertex2 != null && vertex2.y < yMax)
				{
					num3 = vertex2.y;
				}
				if (num3 < yMin)
				{
					return;
				}
				num4 = this.c - this.b * num3;
				if ((num2 > xMax && num4 > xMax) || (num2 < xMin && num4 < xMin))
				{
					return;
				}
				if (num2 > xMax)
				{
					num2 = xMax;
					num = (this.c - num2) / this.b;
				}
				else if (num2 < xMin)
				{
					num2 = xMin;
					num = (this.c - num2) / this.b;
				}
				if (num4 > xMax)
				{
					num4 = xMax;
					num3 = (this.c - num4) / this.b;
				}
				else if (num4 < xMin)
				{
					num4 = xMin;
					num3 = (this.c - num4) / this.b;
				}
			}
			else
			{
				num2 = xMin;
				if (vertex != null && vertex.x > xMin)
				{
					num2 = vertex.x;
				}
				if (num2 > xMax)
				{
					return;
				}
				num = this.c - this.a * num2;
				num4 = xMax;
				if (vertex2 != null && vertex2.x < xMax)
				{
					num4 = vertex2.x;
				}
				if (num4 < xMin)
				{
					return;
				}
				num3 = this.c - this.a * num4;
				if ((num > yMax && num3 > yMax) || (num < yMin && num3 < yMin))
				{
					return;
				}
				if (num > yMax)
				{
					num = yMax;
					num2 = (this.c - num) / this.a;
				}
				else if (num < yMin)
				{
					num = yMin;
					num2 = (this.c - num) / this.a;
				}
				if (num3 > yMax)
				{
					num3 = yMax;
					num4 = (this.c - num3) / this.a;
				}
				else if (num3 < yMin)
				{
					num3 = yMin;
					num4 = (this.c - num3) / this.a;
				}
			}
			this._clippedVertices = new Dictionary<Side, Vector2?>();
			if (vertex == this._leftVertex)
			{
				this._clippedVertices[Side.LEFT] = new Vector2?(new Vector2(num2, num));
				this._clippedVertices[Side.RIGHT] = new Vector2?(new Vector2(num4, num3));
			}
			else
			{
				this._clippedVertices[Side.RIGHT] = new Vector2?(new Vector2(num2, num));
				this._clippedVertices[Side.LEFT] = new Vector2?(new Vector2(num4, num3));
			}
		}

		public void ClipVertices(Polygon bounds)
		{
			LineSegment lineSegment = new LineSegment(null, null);
			bool flag = (double)this.a == 1.0 && (double)this.b >= 0.0;
			if (flag)
			{
				lineSegment.p0 = new Vector2?(this._rightVertex.Coord);
				lineSegment.p1 = new Vector2?(this._leftVertex.Coord);
			}
			else
			{
				lineSegment.p0 = new Vector2?(this._leftVertex.Coord);
				lineSegment.p1 = new Vector2?(this._rightVertex.Coord);
			}
			LineSegment lineSegment2 = new LineSegment(null, null);
			bounds.ClipSegment(lineSegment, ref lineSegment2);
			this._clippedVertices = new Dictionary<Side, Vector2?>();
			if (!flag)
			{
				this._clippedVertices[Side.LEFT] = lineSegment2.p0;
				this._clippedVertices[Side.RIGHT] = lineSegment2.p1;
			}
			else
			{
				this._clippedVertices[Side.RIGHT] = lineSegment2.p0;
				this._clippedVertices[Side.LEFT] = lineSegment2.p1;
			}
		}

		private static Stack<Edge> _pool = new Stack<Edge>();

		private static int _nedges = 0;

		public static readonly Edge DELETED = new Edge();

		public float a;

		public float b;

		public float c;

		private Vertex _leftVertex;

		private Vertex _rightVertex;

		private Dictionary<Side, Vector2?> _clippedVertices;

		private Dictionary<Side, Site> _sites;

		private int _edgeIndex;
	}
}
