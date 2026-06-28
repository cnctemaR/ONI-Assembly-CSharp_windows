using System;
using System.Collections.Generic;
using Delaunay.LR;
using UnityEngine;

namespace Delaunay
{
	public sealed class Vertex : ICoord
	{
		public Vertex(float x, float y)
		{
			this.Init(x, y);
		}

		private static Vertex Create(float x, float y)
		{
			Vertex vertex;
			if (float.IsNaN(x) || float.IsNaN(y))
			{
				vertex = Vertex.VERTEX_AT_INFINITY;
			}
			else if (Vertex._pool.Count > 0)
			{
				vertex = Vertex._pool.Pop().Init(x, y);
			}
			else
			{
				vertex = new Vertex(x, y);
			}
			return vertex;
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
			return "Vertex (" + this._vertexIndex + ")";
		}

		public static Vertex Intersect(Halfedge halfedge0, Halfedge halfedge1)
		{
			Edge edge = halfedge0.edge;
			Edge edge2 = halfedge1.edge;
			Vertex vertex;
			if (edge == null || edge2 == null)
			{
				vertex = null;
			}
			else if (edge.rightSite == edge2.rightSite)
			{
				vertex = null;
			}
			else
			{
				float num = edge.a * edge2.b - edge.b * edge2.a;
				if (-1E-10 < (double)num && (double)num < 1E-10)
				{
					vertex = null;
				}
				else
				{
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
					if ((flag && halfedge2.leftRight == Side.LEFT) || (!flag && halfedge2.leftRight == Side.RIGHT))
					{
						vertex = null;
					}
					else
					{
						vertex = Vertex.Create(num2, num3);
					}
				}
			}
			return vertex;
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
