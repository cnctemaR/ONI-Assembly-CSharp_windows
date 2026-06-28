using System;

namespace MIConvexHull
{
	internal sealed class FaceConnector
	{
		public FaceConnector(int dimension)
		{
			this.Vertices = new int[dimension - 1];
		}

		public void Update(ConvexFaceInternal face, int edgeIndex, int dim)
		{
			this.Face = face;
			this.EdgeIndex = edgeIndex;
			uint num = 23U;
			int[] vertices = face.Vertices;
			int num2 = 0;
			for (int i = 0; i < edgeIndex; i++)
			{
				this.Vertices[num2++] = vertices[i];
				num += 31U * num + (uint)vertices[i];
			}
			for (int i = edgeIndex + 1; i < vertices.Length; i++)
			{
				this.Vertices[num2++] = vertices[i];
				num += 31U * num + (uint)vertices[i];
			}
			this.HashCode = num;
		}

		public static bool AreConnectable(FaceConnector a, FaceConnector b, int dim)
		{
			bool flag;
			if (a.HashCode != b.HashCode)
			{
				flag = false;
			}
			else
			{
				int[] vertices = a.Vertices;
				int[] vertices2 = b.Vertices;
				for (int i = 0; i < vertices.Length; i++)
				{
					if (vertices[i] != vertices2[i])
					{
						return false;
					}
				}
				flag = true;
			}
			return flag;
		}

		public static void Connect(FaceConnector a, FaceConnector b)
		{
			a.Face.AdjacentFaces[a.EdgeIndex] = b.Face.Index;
			b.Face.AdjacentFaces[b.EdgeIndex] = a.Face.Index;
		}

		public int EdgeIndex;

		public ConvexFaceInternal Face;

		public uint HashCode;

		public FaceConnector Next;

		public FaceConnector Previous;

		public int[] Vertices;
	}
}
