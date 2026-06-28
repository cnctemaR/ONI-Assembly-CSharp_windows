using System;
using Delaunay.Utils;
using UnityEngine;

namespace Delaunay
{
	internal sealed class EdgeList : Delaunay.Utils.IDisposable
	{
		public EdgeList(float xmin, float deltax, int sqrt_nsites)
		{
			this._xmin = xmin;
			this._deltax = deltax;
			this._hashsize = 2 * sqrt_nsites;
			this._hash = new Halfedge[this._hashsize];
			this._leftEnd = Halfedge.CreateDummy();
			this._rightEnd = Halfedge.CreateDummy();
			this._leftEnd.edgeListLeftNeighbor = null;
			this._leftEnd.edgeListRightNeighbor = this._rightEnd;
			this._rightEnd.edgeListLeftNeighbor = this._leftEnd;
			this._rightEnd.edgeListRightNeighbor = null;
			this._hash[0] = this._leftEnd;
			this._hash[this._hashsize - 1] = this._rightEnd;
		}

		public Halfedge leftEnd
		{
			get
			{
				return this._leftEnd;
			}
		}

		public Halfedge rightEnd
		{
			get
			{
				return this._rightEnd;
			}
		}

		public void Dispose()
		{
			Halfedge halfedge = this._leftEnd;
			while (halfedge != this._rightEnd)
			{
				Halfedge halfedge2 = halfedge;
				halfedge = halfedge.edgeListRightNeighbor;
				halfedge2.Dispose();
			}
			this._leftEnd = null;
			this._rightEnd.Dispose();
			this._rightEnd = null;
			for (int i = 0; i < this._hashsize; i++)
			{
				this._hash[i] = null;
			}
			this._hash = null;
		}

		public void Insert(Halfedge lb, Halfedge newHalfedge)
		{
			newHalfedge.edgeListLeftNeighbor = lb;
			newHalfedge.edgeListRightNeighbor = lb.edgeListRightNeighbor;
			lb.edgeListRightNeighbor.edgeListLeftNeighbor = newHalfedge;
			lb.edgeListRightNeighbor = newHalfedge;
		}

		public void Remove(Halfedge halfEdge)
		{
			halfEdge.edgeListLeftNeighbor.edgeListRightNeighbor = halfEdge.edgeListRightNeighbor;
			halfEdge.edgeListRightNeighbor.edgeListLeftNeighbor = halfEdge.edgeListLeftNeighbor;
			halfEdge.edge = Edge.DELETED;
			halfEdge.edgeListLeftNeighbor = (halfEdge.edgeListRightNeighbor = null);
		}

		public Halfedge EdgeListLeftNeighbor(Vector2 p)
		{
			int num = (int)((p.x - this._xmin) / this._deltax * (float)this._hashsize);
			if (num < 0)
			{
				num = 0;
			}
			if (num >= this._hashsize)
			{
				num = this._hashsize - 1;
			}
			Halfedge halfedge = this.GetHash(num);
			if (halfedge == null)
			{
				int num2 = 1;
				while ((halfedge = this.GetHash(num - num2)) == null)
				{
					if ((halfedge = this.GetHash(num + num2)) != null)
					{
						break;
					}
					num2++;
				}
			}
			if (halfedge == this.leftEnd || (halfedge != this.rightEnd && halfedge.IsLeftOf(p)))
			{
				do
				{
					halfedge = halfedge.edgeListRightNeighbor;
				}
				while (halfedge != this.rightEnd && halfedge.IsLeftOf(p));
				halfedge = halfedge.edgeListLeftNeighbor;
			}
			else
			{
				do
				{
					halfedge = halfedge.edgeListLeftNeighbor;
				}
				while (halfedge != this.leftEnd && !halfedge.IsLeftOf(p));
			}
			if (num > 0 && num < this._hashsize - 1)
			{
				this._hash[num] = halfedge;
			}
			return halfedge;
		}

		private Halfedge GetHash(int b)
		{
			Halfedge halfedge;
			if (b < 0 || b >= this._hashsize)
			{
				halfedge = null;
			}
			else
			{
				Halfedge halfedge2 = this._hash[b];
				if (halfedge2 != null && halfedge2.edge == Edge.DELETED)
				{
					this._hash[b] = null;
					halfedge = null;
				}
				else
				{
					halfedge = halfedge2;
				}
			}
			return halfedge;
		}

		private float _deltax;

		private float _xmin;

		private int _hashsize;

		private Halfedge[] _hash;

		private Halfedge _leftEnd;

		private Halfedge _rightEnd;
	}
}
