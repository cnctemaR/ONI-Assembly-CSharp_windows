using System;

namespace MIConvexHull
{
	internal class ObjectManager
	{
		public ObjectManager(ConvexHullAlgorithm hull)
		{
			this.Dimension = hull.NumOfDimensions;
			this.Hull = hull;
			this.FacePool = hull.FacePool;
			this.FacePoolSize = 0;
			this.FacePoolCapacity = hull.FacePool.Length;
			this.FreeFaceIndices = new IndexBuffer();
			this.EmptyBufferStack = new SimpleList<IndexBuffer>();
			this.DeferredFaceStack = new SimpleList<DeferredFace>();
		}

		public void DepositFace(int faceIndex)
		{
			ConvexFaceInternal convexFaceInternal = this.FacePool[faceIndex];
			int[] adjacentFaces = convexFaceInternal.AdjacentFaces;
			for (int i = 0; i < adjacentFaces.Length; i++)
			{
				adjacentFaces[i] = -1;
			}
			this.FreeFaceIndices.Push(faceIndex);
		}

		private void ReallocateFacePool()
		{
			ConvexFaceInternal[] array = new ConvexFaceInternal[2 * this.FacePoolCapacity];
			bool[] array2 = new bool[2 * this.FacePoolCapacity];
			Array.Copy(this.FacePool, array, this.FacePoolCapacity);
			Buffer.BlockCopy(this.Hull.AffectedFaceFlags, 0, array2, 0, this.FacePoolCapacity);
			this.FacePoolCapacity = 2 * this.FacePoolCapacity;
			this.Hull.FacePool = array;
			this.FacePool = array;
			this.Hull.AffectedFaceFlags = array2;
		}

		private int CreateFace()
		{
			int facePoolSize = this.FacePoolSize;
			ConvexFaceInternal convexFaceInternal = new ConvexFaceInternal(this.Dimension, facePoolSize, this.GetVertexBuffer());
			this.FacePoolSize++;
			if (this.FacePoolSize > this.FacePoolCapacity)
			{
				this.ReallocateFacePool();
			}
			this.FacePool[facePoolSize] = convexFaceInternal;
			return facePoolSize;
		}

		public int GetFace()
		{
			if (this.FreeFaceIndices.Count > 0)
			{
				return this.FreeFaceIndices.Pop();
			}
			return this.CreateFace();
		}

		public void DepositConnector(FaceConnector connector)
		{
			if (this.ConnectorStack == null)
			{
				connector.Next = null;
				this.ConnectorStack = connector;
			}
			else
			{
				connector.Next = this.ConnectorStack;
				this.ConnectorStack = connector;
			}
		}

		public FaceConnector GetConnector()
		{
			if (this.ConnectorStack == null)
			{
				return new FaceConnector(this.Dimension);
			}
			FaceConnector connectorStack = this.ConnectorStack;
			this.ConnectorStack = this.ConnectorStack.Next;
			connectorStack.Next = null;
			return connectorStack;
		}

		public void DepositVertexBuffer(IndexBuffer buffer)
		{
			buffer.Clear();
			this.EmptyBufferStack.Push(buffer);
		}

		public IndexBuffer GetVertexBuffer()
		{
			return (this.EmptyBufferStack.Count == 0) ? new IndexBuffer() : this.EmptyBufferStack.Pop();
		}

		public void DepositDeferredFace(DeferredFace face)
		{
			this.DeferredFaceStack.Push(face);
		}

		public DeferredFace GetDeferredFace()
		{
			return (this.DeferredFaceStack.Count == 0) ? new DeferredFace() : this.DeferredFaceStack.Pop();
		}

		private readonly int Dimension;

		private FaceConnector ConnectorStack;

		private readonly SimpleList<DeferredFace> DeferredFaceStack;

		private readonly SimpleList<IndexBuffer> EmptyBufferStack;

		private ConvexFaceInternal[] FacePool;

		private int FacePoolSize;

		private int FacePoolCapacity;

		private readonly IndexBuffer FreeFaceIndices;

		private readonly ConvexHullAlgorithm Hull;
	}
}
