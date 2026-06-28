using System;
using Delaunay.Utils;
using UnityEngine;

namespace Delaunay
{
	internal sealed class HalfedgePriorityQueue : Delaunay.Utils.IDisposable
	{
		public HalfedgePriorityQueue(float ymin, float deltay, int sqrt_nsites)
		{
			this._ymin = ymin;
			this._deltay = deltay;
			this._hashsize = 4 * sqrt_nsites;
			this.Initialize();
		}

		public void Dispose()
		{
			for (int i = 0; i < this._hashsize; i++)
			{
				this._hash[i].Dispose();
				this._hash[i] = null;
			}
			this._hash = null;
		}

		private void Initialize()
		{
			this._count = 0;
			this._minBucket = 0;
			this._hash = new Halfedge[this._hashsize];
			for (int i = 0; i < this._hashsize; i++)
			{
				this._hash[i] = Halfedge.CreateDummy();
				this._hash[i].nextInPriorityQueue = null;
			}
		}

		public void Insert(Halfedge halfEdge)
		{
			int num = this.Bucket(halfEdge);
			if (num < this._minBucket)
			{
				this._minBucket = num;
			}
			Halfedge halfedge = this._hash[num];
			Halfedge nextInPriorityQueue;
			while ((nextInPriorityQueue = halfedge.nextInPriorityQueue) != null && (halfEdge.ystar > nextInPriorityQueue.ystar || (halfEdge.ystar == nextInPriorityQueue.ystar && halfEdge.vertex.x > nextInPriorityQueue.vertex.x)))
			{
				halfedge = nextInPriorityQueue;
			}
			halfEdge.nextInPriorityQueue = halfedge.nextInPriorityQueue;
			halfedge.nextInPriorityQueue = halfEdge;
			this._count++;
		}

		public void Remove(Halfedge halfEdge)
		{
			int num = this.Bucket(halfEdge);
			if (halfEdge.vertex != null)
			{
				Halfedge halfedge = this._hash[num];
				while (halfedge.nextInPriorityQueue != halfEdge)
				{
					halfedge = halfedge.nextInPriorityQueue;
				}
				halfedge.nextInPriorityQueue = halfEdge.nextInPriorityQueue;
				this._count--;
				halfEdge.vertex = null;
				halfEdge.nextInPriorityQueue = null;
				halfEdge.Dispose();
			}
		}

		private int Bucket(Halfedge halfEdge)
		{
			int num = (int)((halfEdge.ystar - this._ymin) / this._deltay * (float)this._hashsize);
			if (num < 0)
			{
				num = 0;
			}
			if (num >= this._hashsize)
			{
				num = this._hashsize - 1;
			}
			return num;
		}

		private bool IsEmpty(int bucket)
		{
			return this._hash[bucket].nextInPriorityQueue == null;
		}

		private void AdjustMinBucket()
		{
			while (this._minBucket < this._hashsize - 1 && this.IsEmpty(this._minBucket))
			{
				this._minBucket++;
			}
		}

		public bool Empty()
		{
			return this._count == 0;
		}

		public Vector2 Min()
		{
			this.AdjustMinBucket();
			Halfedge nextInPriorityQueue = this._hash[this._minBucket].nextInPriorityQueue;
			return new Vector2(nextInPriorityQueue.vertex.x, nextInPriorityQueue.ystar);
		}

		public Halfedge ExtractMin()
		{
			Halfedge nextInPriorityQueue = this._hash[this._minBucket].nextInPriorityQueue;
			this._hash[this._minBucket].nextInPriorityQueue = nextInPriorityQueue.nextInPriorityQueue;
			this._count--;
			nextInPriorityQueue.nextInPriorityQueue = null;
			return nextInPriorityQueue;
		}

		private Halfedge[] _hash;

		private int _count;

		private int _minBucket;

		private int _hashsize;

		private float _ymin;

		private float _deltay;
	}
}
