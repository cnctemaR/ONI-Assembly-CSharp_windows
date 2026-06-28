using System;

namespace MIConvexHull
{
	internal sealed class ConvexFaceInternal
	{
		public ConvexFaceInternal(int dimension, int index, IndexBuffer beyondList)
		{
			this.Index = index;
			this.AdjacentFaces = new int[dimension];
			this.VerticesBeyond = beyondList;
			this.Normal = new double[dimension];
			this.Vertices = new int[dimension];
		}

		public int[] AdjacentFaces;

		public int FurthestVertex;

		public int Index;

		public bool InList;

		public bool IsNormalFlipped;

		public ConvexFaceInternal Next;

		public double[] Normal;

		public double Offset;

		public ConvexFaceInternal Previous;

		public int Tag;

		public int[] Vertices;

		public IndexBuffer VerticesBeyond;
	}
}
