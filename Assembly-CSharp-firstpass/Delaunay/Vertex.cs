using System;
using System.Collections.Generic;
using Delaunay.LR;
using UnityEngine;

namespace Delaunay
{
	public sealed class Vertex : ICoord
	{
		private static Vertex Create(float x, float y)
		{
			if (float.IsNaN(x) || float.IsNaN(y))
			{
				return Vertex.VERTEX_AT_INFINITY;
			}
			if (Vertex._pool.Count > 0)
			{
				return Vertex._pool.Pop().Init(x, y);
			}
			return new Vertex(x, y);
		}

		public Vector2 Coord
		{
			get
			{
				return this._coord;
			}
		}

		public int vertexIndex
		{
			get
			{
				return this._vertexIndex;
			}
		}

		public Vertex(float x, float y)
		{
			this.Init(x, y);
		}

		private Vertex Init(float x, float y)
		{
			this._coord = new Vector2(x, y);
			return this;
		}

		public void Dispose()
		{
			Vertex._pool.Push(this);
		}

		public void SetIndex()
		{
			this._vertexIndex = Vertex._nvertices++;
		}

		public override string ToString()
		{
			return "Vertex (" + this._vertexIndex.ToString() + ")";
		}

		public static Vertex Intersect(Halfedge halfedge0, Halfedge halfedge1)
		{
			Edge edge = halfedge0.edge;
			Edge edge2 = halfedge1.edge;
			if (edge == null || edge2 == null)
			{
				return null;
			}
			if (edge.rightSite == edge2.rightSite)
			{
				return null;
			}
			float num = edge.a * edge2.b - edge.b * edge2.a;
			if (-1E-10 < (double)num && (double)num < 1E-10)
			{
				return null;
			}
			float num2 = (edge.c * edge2.b - edge2.c * edge.b) / num;
			float num3 = (edge2.c * edge.a - edge.c * edge2.a) / num;
			Halfedge halfedge2;
			Edge edge3;
			if (Voronoi.CompareByYThenX(edge.rightSite, edge2.rightSite) < 0)
			{
				halfedge2 = halfedge0;
				edge3 = edge;
			}
			else
			{
				halfedge2 = halfedge1;
				edge3 = edge2;
			}
			bool flag = num2 >= edge3.rightSite.x;
			if (flag)
			{
				Side? side = halfedge2.leftRight;
				Side side2 = Side.LEFT;
				if ((side.GetValueOrDefault() == side2) & (side != null))
				{
					goto IL_011B;
				}
			}
			if (!flag)
			{
				Side? side = halfedge2.leftRight;
				Side side2 = Side.RIGHT;
				if ((side.GetValueOrDefault() == side2) & (side != null))
				{
					goto IL_011B;
				}
			}
			return Vertex.Create(num2, num3);
			IL_011B:
			return null;
		}

		public float x
		{
			get
			{
				return this._coord.x;
			}
		}

		public float y
		{
			get
			{
				return this._coord.y;
			}
		}

		public static readonly Vertex VERTEX_AT_INFINITY = new Vertex(float.NaN, float.NaN);

		private static Stack<Vertex> _pool = new Stack<Vertex>();

		private static int _nvertices = 0;

		private Vector2 _coord;

		private int _vertexIndex;
	}
}
