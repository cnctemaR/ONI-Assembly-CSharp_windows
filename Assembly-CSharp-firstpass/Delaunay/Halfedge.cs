using System;
using System.Collections.Generic;
using Delaunay.LR;
using Delaunay.Utils;
using UnityEngine;

namespace Delaunay
{
	public sealed class Halfedge : Delaunay.Utils.IDisposable
	{
		public Halfedge(Edge edge = null, Side? lr = null)
		{
			this.Init(edge, lr);
		}

		public static Halfedge Create(Edge edge, Side? lr)
		{
			Halfedge halfedge;
			if (Halfedge._pool.Count > 0)
			{
				halfedge = Halfedge._pool.Pop().Init(edge, lr);
			}
			else
			{
				halfedge = new Halfedge(edge, lr);
			}
			return halfedge;
		}

		public static Halfedge CreateDummy()
		{
			return Halfedge.Create(null, null);
		}

		private Halfedge Init(Edge edge, Side? lr)
		{
			this.edge = edge;
			this.leftRight = lr;
			this.nextInPriorityQueue = null;
			this.vertex = null;
			return this;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Halfedge (leftRight: ",
				this.leftRight.ToString(),
				"; vertex: ",
				this.vertex.ToString(),
				")"
			});
		}

		public void Dispose()
		{
			if (this.edgeListLeftNeighbor == null && this.edgeListRightNeighbor == null)
			{
				if (this.nextInPriorityQueue == null)
				{
					this.edge = null;
					this.leftRight = null;
					this.vertex = null;
					Halfedge._pool.Push(this);
				}
			}
		}

		public void ReallyDispose()
		{
			this.edgeListLeftNeighbor = null;
			this.edgeListRightNeighbor = null;
			this.nextInPriorityQueue = null;
			this.edge = null;
			this.leftRight = null;
			this.vertex = null;
			Halfedge._pool.Push(this);
		}

		internal bool IsLeftOf(Vector2 p)
		{
			Vector2 coord = this.edge.rightSite.Coord;
			bool flag = p.x > coord.x;
			bool flag2;
			if (flag && this.leftRight == Side.LEFT)
			{
				flag2 = true;
			}
			else if (!flag && this.leftRight == Side.RIGHT)
			{
				flag2 = false;
			}
			else
			{
				bool flag4;
				if ((double)this.edge.a == 1.0)
				{
					float num = p.y - coord.y;
					float num2 = p.x - coord.x;
					bool flag3 = false;
					if ((!flag && (double)this.edge.b < 0.0) || (flag && (double)this.edge.b >= 0.0))
					{
						flag4 = num >= this.edge.b * num2;
						flag3 = flag4;
					}
					else
					{
						flag4 = p.x + p.y * this.edge.b > this.edge.c;
						if ((double)this.edge.b < 0.0)
						{
							flag4 = !flag4;
						}
						if (!flag4)
						{
							flag3 = true;
						}
					}
					if (!flag3)
					{
						float num3 = coord.x - this.edge.leftSite.x;
						flag4 = (double)(this.edge.b * (num2 * num2 - num * num)) < (double)(num3 * num) * (1.0 + 2.0 * (double)num2 / (double)num3 + (double)(this.edge.b * this.edge.b));
						if ((double)this.edge.b < 0.0)
						{
							flag4 = !flag4;
						}
					}
				}
				else
				{
					float num4 = this.edge.c - this.edge.a * p.x;
					float num5 = p.y - num4;
					float num6 = p.x - coord.x;
					float num7 = num4 - coord.y;
					flag4 = num5 * num5 > num6 * num6 + num7 * num7;
				}
				flag2 = ((!(this.leftRight == Side.LEFT)) ? (!flag4) : flag4);
			}
			return flag2;
		}

		private static Stack<Halfedge> _pool = new Stack<Halfedge>();

		public Halfedge edgeListLeftNeighbor;

		public Halfedge edgeListRightNeighbor;

		public Halfedge nextInPriorityQueue;

		public Edge edge;

		public Side? leftRight;

		public Vertex vertex;

		public float ystar;
	}
}
