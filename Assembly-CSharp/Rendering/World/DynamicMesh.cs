using System;
using UnityEngine;

namespace Rendering.World
{
	public class DynamicMesh
	{
		public DynamicMesh(string name, Bounds bounds)
		{
			this.Name = name;
			this.Bounds = bounds;
		}

		public void Reserve(int vertex_count, int triangle_count)
		{
			if (vertex_count > this.VertexCount)
			{
				this.SetUVs = true;
			}
			else
			{
				this.SetUVs = false;
			}
			if (this.TriangleCount != triangle_count)
			{
				this.SetTriangles = true;
			}
			else
			{
				this.SetTriangles = false;
			}
			int num = (int)Mathf.Ceil((float)triangle_count / (float)DynamicMesh.TrianglesPerMesh);
			if (num != this.Meshes.Length)
			{
				this.Meshes = new DynamicSubMesh[num];
				for (int i = 0; i < this.Meshes.Length; i++)
				{
					int num2 = -i * DynamicMesh.VerticesPerMesh;
					this.Meshes[i] = new DynamicSubMesh(this.Name, this.Bounds, num2);
				}
				this.SetUVs = true;
				this.SetTriangles = true;
			}
			for (int j = 0; j < this.Meshes.Length; j++)
			{
				if (j == this.Meshes.Length - 1)
				{
					this.Meshes[j].Reserve(vertex_count % DynamicMesh.VerticesPerMesh, triangle_count % DynamicMesh.TrianglesPerMesh);
				}
				else
				{
					this.Meshes[j].Reserve(DynamicMesh.VerticesPerMesh, DynamicMesh.TrianglesPerMesh);
				}
			}
			this.VertexCount = vertex_count;
			this.TriangleCount = triangle_count;
		}

		public void Commit()
		{
			foreach (DynamicSubMesh dynamicSubMesh in this.Meshes)
			{
				dynamicSubMesh.Commit();
			}
			this.TriangleMeshIdx = 0;
			this.UVMeshIdx = 0;
			this.VertexMeshIdx = 0;
		}

		public void AddTriangle(int triangle)
		{
			DynamicSubMesh dynamicSubMesh = this.Meshes[this.TriangleMeshIdx];
			if (dynamicSubMesh.AreTrianglesFull())
			{
				dynamicSubMesh = this.Meshes[++this.TriangleMeshIdx];
			}
			this.Meshes[this.TriangleMeshIdx].AddTriangle(triangle);
		}

		public void AddUV(Vector2 uv)
		{
			DynamicSubMesh dynamicSubMesh = this.Meshes[this.UVMeshIdx];
			if (dynamicSubMesh.AreUVsFull())
			{
				dynamicSubMesh = this.Meshes[++this.UVMeshIdx];
			}
			dynamicSubMesh.AddUV(uv);
		}

		public void AddVertex(Vector3 vertex)
		{
			DynamicSubMesh dynamicSubMesh = this.Meshes[this.VertexMeshIdx];
			if (dynamicSubMesh.AreVerticesFull())
			{
				dynamicSubMesh = this.Meshes[++this.VertexMeshIdx];
			}
			dynamicSubMesh.AddVertex(vertex);
		}

		public void Render(Vector3 position, Quaternion rotation, Material material, int layer, MaterialPropertyBlock property_block)
		{
			foreach (DynamicSubMesh dynamicSubMesh in this.Meshes)
			{
				dynamicSubMesh.Render(position, rotation, material, layer, property_block);
			}
		}

		private static int TrianglesPerMesh = 65004;

		private static int VerticesPerMesh = 4 * DynamicMesh.TrianglesPerMesh / 6;

		public bool SetUVs;

		public bool SetTriangles;

		public string Name;

		public Bounds Bounds;

		public DynamicSubMesh[] Meshes = new DynamicSubMesh[0];

		private int VertexCount;

		private int TriangleCount;

		private int VertexIdx;

		private int UVIdx;

		private int TriangleIdx;

		private int TriangleMeshIdx;

		private int VertexMeshIdx;

		private int UVMeshIdx;
	}
}
