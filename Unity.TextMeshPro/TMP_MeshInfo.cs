using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	public struct TMP_MeshInfo
	{
		public TMP_MeshInfo(Mesh mesh, int size)
		{
			if (mesh == null)
			{
				mesh = new Mesh();
				mesh.name = "TMP_MeshInfo";
			}
			else
			{
				mesh.Clear();
			}
			this.mesh = mesh;
			size = Mathf.Min(size, 16383);
			int num = size * 4;
			int num2 = size * 6;
			this.vertexCount = 0;
			this.vertices = new Vector3[num];
			this.uvs0 = new Vector2[num];
			this.uvs2 = new Vector2[num];
			this.colors32 = new Color32[num];
			this.normals = new Vector3[num];
			this.tangents = new Vector4[num];
			this.triangles = new int[num2];
			int num3 = 0;
			int num4 = 0;
			while (num4 / 4 < size)
			{
				for (int i = 0; i < 4; i++)
				{
					this.vertices[num4 + i] = Vector3.zero;
					this.uvs0[num4 + i] = Vector2.zero;
					this.uvs2[num4 + i] = Vector2.zero;
					this.colors32[num4 + i] = TMP_MeshInfo.s_DefaultColor;
					this.normals[num4 + i] = TMP_MeshInfo.s_DefaultNormal;
					this.tangents[num4 + i] = TMP_MeshInfo.s_DefaultTangent;
				}
				this.triangles[num3] = num4;
				this.triangles[num3 + 1] = num4 + 1;
				this.triangles[num3 + 2] = num4 + 2;
				this.triangles[num3 + 3] = num4 + 2;
				this.triangles[num3 + 4] = num4 + 3;
				this.triangles[num3 + 5] = num4;
				num4 += 4;
				num3 += 6;
			}
			this.mesh.vertices = this.vertices;
			this.mesh.normals = this.normals;
			this.mesh.tangents = this.tangents;
			this.mesh.triangles = this.triangles;
			this.mesh.bounds = new Bounds(Vector3.zero, new Vector3(3840f, 2160f, 0f));
		}

		public TMP_MeshInfo(Mesh mesh, int size, bool isVolumetric)
		{
			if (mesh == null)
			{
				mesh = new Mesh();
				mesh.name = "TMP_MeshInfo";
			}
			else
			{
				mesh.Clear();
			}
			this.mesh = mesh;
			int num = ((!isVolumetric) ? 4 : 8);
			int num2 = ((!isVolumetric) ? 6 : 36);
			size = Mathf.Min(size, 65532 / num);
			int num3 = size * num;
			int num4 = size * num2;
			this.vertexCount = 0;
			this.vertices = new Vector3[num3];
			this.uvs0 = new Vector2[num3];
			this.uvs2 = new Vector2[num3];
			this.colors32 = new Color32[num3];
			this.normals = new Vector3[num3];
			this.tangents = new Vector4[num3];
			this.triangles = new int[num4];
			int num5 = 0;
			int num6 = 0;
			while (num5 / num < size)
			{
				for (int i = 0; i < num; i++)
				{
					this.vertices[num5 + i] = Vector3.zero;
					this.uvs0[num5 + i] = Vector2.zero;
					this.uvs2[num5 + i] = Vector2.zero;
					this.colors32[num5 + i] = TMP_MeshInfo.s_DefaultColor;
					this.normals[num5 + i] = TMP_MeshInfo.s_DefaultNormal;
					this.tangents[num5 + i] = TMP_MeshInfo.s_DefaultTangent;
				}
				this.triangles[num6] = num5;
				this.triangles[num6 + 1] = num5 + 1;
				this.triangles[num6 + 2] = num5 + 2;
				this.triangles[num6 + 3] = num5 + 2;
				this.triangles[num6 + 4] = num5 + 3;
				this.triangles[num6 + 5] = num5;
				if (isVolumetric)
				{
					this.triangles[num6 + 6] = num5 + 4;
					this.triangles[num6 + 7] = num5 + 5;
					this.triangles[num6 + 8] = num5 + 1;
					this.triangles[num6 + 9] = num5 + 1;
					this.triangles[num6 + 10] = num5;
					this.triangles[num6 + 11] = num5 + 4;
					this.triangles[num6 + 12] = num5 + 3;
					this.triangles[num6 + 13] = num5 + 2;
					this.triangles[num6 + 14] = num5 + 6;
					this.triangles[num6 + 15] = num5 + 6;
					this.triangles[num6 + 16] = num5 + 7;
					this.triangles[num6 + 17] = num5 + 3;
					this.triangles[num6 + 18] = num5 + 1;
					this.triangles[num6 + 19] = num5 + 5;
					this.triangles[num6 + 20] = num5 + 6;
					this.triangles[num6 + 21] = num5 + 6;
					this.triangles[num6 + 22] = num5 + 2;
					this.triangles[num6 + 23] = num5 + 1;
					this.triangles[num6 + 24] = num5 + 4;
					this.triangles[num6 + 25] = num5;
					this.triangles[num6 + 26] = num5 + 3;
					this.triangles[num6 + 27] = num5 + 3;
					this.triangles[num6 + 28] = num5 + 7;
					this.triangles[num6 + 29] = num5 + 4;
					this.triangles[num6 + 30] = num5 + 7;
					this.triangles[num6 + 31] = num5 + 6;
					this.triangles[num6 + 32] = num5 + 5;
					this.triangles[num6 + 33] = num5 + 5;
					this.triangles[num6 + 34] = num5 + 4;
					this.triangles[num6 + 35] = num5 + 7;
				}
				num5 += num;
				num6 += num2;
			}
			this.mesh.vertices = this.vertices;
			this.mesh.normals = this.normals;
			this.mesh.tangents = this.tangents;
			this.mesh.triangles = this.triangles;
			this.mesh.bounds = new Bounds(Vector3.zero, new Vector3(3840f, 2160f, 64f));
		}

		public void ResizeMeshInfo(int size)
		{
			size = Mathf.Min(size, 16383);
			int num = size * 4;
			int num2 = size * 6;
			int num3 = this.vertices.Length / 4;
			Array.Resize<Vector3>(ref this.vertices, num);
			Array.Resize<Vector3>(ref this.normals, num);
			Array.Resize<Vector4>(ref this.tangents, num);
			Array.Resize<Vector2>(ref this.uvs0, num);
			Array.Resize<Vector2>(ref this.uvs2, num);
			Array.Resize<Color32>(ref this.colors32, num);
			Array.Resize<int>(ref this.triangles, num2);
			if (size <= num3)
			{
				this.mesh.triangles = this.triangles;
				this.mesh.vertices = this.vertices;
				this.mesh.normals = this.normals;
				this.mesh.tangents = this.tangents;
				return;
			}
			for (int i = num3; i < size; i++)
			{
				int num4 = i * 4;
				int num5 = i * 6;
				this.normals[num4] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[1 + num4] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[2 + num4] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[3 + num4] = TMP_MeshInfo.s_DefaultNormal;
				this.tangents[num4] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[1 + num4] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[2 + num4] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[3 + num4] = TMP_MeshInfo.s_DefaultTangent;
				this.triangles[num5] = num4;
				this.triangles[1 + num5] = 1 + num4;
				this.triangles[2 + num5] = 2 + num4;
				this.triangles[3 + num5] = 2 + num4;
				this.triangles[4 + num5] = 3 + num4;
				this.triangles[5 + num5] = num4;
			}
			this.mesh.vertices = this.vertices;
			this.mesh.normals = this.normals;
			this.mesh.tangents = this.tangents;
			this.mesh.triangles = this.triangles;
		}

		public void ResizeMeshInfo(int size, bool isVolumetric)
		{
			int num = ((!isVolumetric) ? 4 : 8);
			int num2 = ((!isVolumetric) ? 6 : 36);
			size = Mathf.Min(size, 65532 / num);
			int num3 = size * num;
			int num4 = size * num2;
			int num5 = this.vertices.Length / num;
			Array.Resize<Vector3>(ref this.vertices, num3);
			Array.Resize<Vector3>(ref this.normals, num3);
			Array.Resize<Vector4>(ref this.tangents, num3);
			Array.Resize<Vector2>(ref this.uvs0, num3);
			Array.Resize<Vector2>(ref this.uvs2, num3);
			Array.Resize<Color32>(ref this.colors32, num3);
			Array.Resize<int>(ref this.triangles, num4);
			if (size <= num5)
			{
				this.mesh.triangles = this.triangles;
				this.mesh.vertices = this.vertices;
				this.mesh.normals = this.normals;
				this.mesh.tangents = this.tangents;
				return;
			}
			for (int i = num5; i < size; i++)
			{
				int num6 = i * num;
				int num7 = i * num2;
				this.normals[num6] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[1 + num6] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[2 + num6] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[3 + num6] = TMP_MeshInfo.s_DefaultNormal;
				this.tangents[num6] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[1 + num6] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[2 + num6] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[3 + num6] = TMP_MeshInfo.s_DefaultTangent;
				if (isVolumetric)
				{
					this.normals[4 + num6] = TMP_MeshInfo.s_DefaultNormal;
					this.normals[5 + num6] = TMP_MeshInfo.s_DefaultNormal;
					this.normals[6 + num6] = TMP_MeshInfo.s_DefaultNormal;
					this.normals[7 + num6] = TMP_MeshInfo.s_DefaultNormal;
					this.tangents[4 + num6] = TMP_MeshInfo.s_DefaultTangent;
					this.tangents[5 + num6] = TMP_MeshInfo.s_DefaultTangent;
					this.tangents[6 + num6] = TMP_MeshInfo.s_DefaultTangent;
					this.tangents[7 + num6] = TMP_MeshInfo.s_DefaultTangent;
				}
				this.triangles[num7] = num6;
				this.triangles[1 + num7] = 1 + num6;
				this.triangles[2 + num7] = 2 + num6;
				this.triangles[3 + num7] = 2 + num6;
				this.triangles[4 + num7] = 3 + num6;
				this.triangles[5 + num7] = num6;
				if (isVolumetric)
				{
					this.triangles[num7 + 6] = num6 + 4;
					this.triangles[num7 + 7] = num6 + 5;
					this.triangles[num7 + 8] = num6 + 1;
					this.triangles[num7 + 9] = num6 + 1;
					this.triangles[num7 + 10] = num6;
					this.triangles[num7 + 11] = num6 + 4;
					this.triangles[num7 + 12] = num6 + 3;
					this.triangles[num7 + 13] = num6 + 2;
					this.triangles[num7 + 14] = num6 + 6;
					this.triangles[num7 + 15] = num6 + 6;
					this.triangles[num7 + 16] = num6 + 7;
					this.triangles[num7 + 17] = num6 + 3;
					this.triangles[num7 + 18] = num6 + 1;
					this.triangles[num7 + 19] = num6 + 5;
					this.triangles[num7 + 20] = num6 + 6;
					this.triangles[num7 + 21] = num6 + 6;
					this.triangles[num7 + 22] = num6 + 2;
					this.triangles[num7 + 23] = num6 + 1;
					this.triangles[num7 + 24] = num6 + 4;
					this.triangles[num7 + 25] = num6;
					this.triangles[num7 + 26] = num6 + 3;
					this.triangles[num7 + 27] = num6 + 3;
					this.triangles[num7 + 28] = num6 + 7;
					this.triangles[num7 + 29] = num6 + 4;
					this.triangles[num7 + 30] = num6 + 7;
					this.triangles[num7 + 31] = num6 + 6;
					this.triangles[num7 + 32] = num6 + 5;
					this.triangles[num7 + 33] = num6 + 5;
					this.triangles[num7 + 34] = num6 + 4;
					this.triangles[num7 + 35] = num6 + 7;
				}
			}
			this.mesh.vertices = this.vertices;
			this.mesh.normals = this.normals;
			this.mesh.tangents = this.tangents;
			this.mesh.triangles = this.triangles;
		}

		public void Clear()
		{
			if (this.vertices == null)
			{
				return;
			}
			Array.Clear(this.vertices, 0, this.vertices.Length);
			this.vertexCount = 0;
			if (this.mesh != null)
			{
				this.mesh.vertices = this.vertices;
			}
		}

		public void Clear(bool uploadChanges)
		{
			if (this.vertices == null)
			{
				return;
			}
			Array.Clear(this.vertices, 0, this.vertices.Length);
			this.vertexCount = 0;
			if (uploadChanges && this.mesh != null)
			{
				this.mesh.vertices = this.vertices;
			}
		}

		public void ClearUnusedVertices()
		{
			int num = this.vertices.Length - this.vertexCount;
			if (num > 0)
			{
				Array.Clear(this.vertices, this.vertexCount, num);
			}
		}

		public void ClearUnusedVertices(int startIndex)
		{
			int num = this.vertices.Length - startIndex;
			if (num > 0)
			{
				Array.Clear(this.vertices, startIndex, num);
			}
		}

		public void ClearUnusedVertices(int startIndex, bool updateMesh)
		{
			int num = this.vertices.Length - startIndex;
			if (num > 0)
			{
				Array.Clear(this.vertices, startIndex, num);
			}
			if (updateMesh && this.mesh != null)
			{
				this.mesh.vertices = this.vertices;
			}
		}

		public void SortGeometry(VertexSortingOrder order)
		{
			if (order != VertexSortingOrder.Normal && order == VertexSortingOrder.Reverse)
			{
				int num = this.vertexCount / 4;
				for (int i = 0; i < num; i++)
				{
					int num2 = i * 4;
					int num3 = (num - i - 1) * 4;
					if (num2 < num3)
					{
						this.SwapVertexData(num2, num3);
					}
				}
			}
		}

		public void SortGeometry(IList<int> sortingOrder)
		{
			int count = sortingOrder.Count;
			if (count * 4 > this.vertices.Length)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				int j;
				for (j = sortingOrder[i]; j < i; j = sortingOrder[j])
				{
				}
				if (j != i)
				{
					this.SwapVertexData(j * 4, i * 4);
				}
			}
		}

		public void SwapVertexData(int src, int dst)
		{
			Vector3 vector = this.vertices[dst];
			this.vertices[dst] = this.vertices[src];
			this.vertices[src] = vector;
			vector = this.vertices[dst + 1];
			this.vertices[dst + 1] = this.vertices[src + 1];
			this.vertices[src + 1] = vector;
			vector = this.vertices[dst + 2];
			this.vertices[dst + 2] = this.vertices[src + 2];
			this.vertices[src + 2] = vector;
			vector = this.vertices[dst + 3];
			this.vertices[dst + 3] = this.vertices[src + 3];
			this.vertices[src + 3] = vector;
			Vector2 vector2 = this.uvs0[dst];
			this.uvs0[dst] = this.uvs0[src];
			this.uvs0[src] = vector2;
			vector2 = this.uvs0[dst + 1];
			this.uvs0[dst + 1] = this.uvs0[src + 1];
			this.uvs0[src + 1] = vector2;
			vector2 = this.uvs0[dst + 2];
			this.uvs0[dst + 2] = this.uvs0[src + 2];
			this.uvs0[src + 2] = vector2;
			vector2 = this.uvs0[dst + 3];
			this.uvs0[dst + 3] = this.uvs0[src + 3];
			this.uvs0[src + 3] = vector2;
			vector2 = this.uvs2[dst];
			this.uvs2[dst] = this.uvs2[src];
			this.uvs2[src] = vector2;
			vector2 = this.uvs2[dst + 1];
			this.uvs2[dst + 1] = this.uvs2[src + 1];
			this.uvs2[src + 1] = vector2;
			vector2 = this.uvs2[dst + 2];
			this.uvs2[dst + 2] = this.uvs2[src + 2];
			this.uvs2[src + 2] = vector2;
			vector2 = this.uvs2[dst + 3];
			this.uvs2[dst + 3] = this.uvs2[src + 3];
			this.uvs2[src + 3] = vector2;
			Color32 color = this.colors32[dst];
			this.colors32[dst] = this.colors32[src];
			this.colors32[src] = color;
			color = this.colors32[dst + 1];
			this.colors32[dst + 1] = this.colors32[src + 1];
			this.colors32[src + 1] = color;
			color = this.colors32[dst + 2];
			this.colors32[dst + 2] = this.colors32[src + 2];
			this.colors32[src + 2] = color;
			color = this.colors32[dst + 3];
			this.colors32[dst + 3] = this.colors32[src + 3];
			this.colors32[src + 3] = color;
		}

		private static readonly Color32 s_DefaultColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		private static readonly Vector3 s_DefaultNormal = new Vector3(0f, 0f, -1f);

		private static readonly Vector4 s_DefaultTangent = new Vector4(-1f, 0f, 0f, 1f);

		public Mesh mesh;

		public int vertexCount;

		public Vector3[] vertices;

		public Vector3[] normals;

		public Vector4[] tangents;

		public Vector2[] uvs0;

		public Vector2[] uvs2;

		public Color32[] colors32;

		public int[] triangles;
	}
}
