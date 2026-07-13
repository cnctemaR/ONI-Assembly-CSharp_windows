using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements
{
	internal struct MeshWriteDataInterface
	{
		public static MeshWriteDataInterface FromMeshWriteData(MeshWriteData data)
		{
			return new MeshWriteDataInterface
			{
				vertices = new IntPtr(data.m_Vertices.GetUnsafePtr<Vertex>()),
				indices = new IntPtr(data.m_Indices.GetUnsafePtr<ushort>()),
				vertexCount = data.m_Vertices.Length,
				indexCount = data.m_Indices.Length
			};
		}

		public IntPtr vertices;

		public IntPtr indices;

		public int vertexCount;

		public int indexCount;
	}
}
