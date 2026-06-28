using System;
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
			}
			else
			{
				mesh.Clear();
			}
			mesh.name = "TMPro";
			this.mesh = mesh;
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

		public void ResizeMeshInfo(int size)
		{
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
				this.normals[0 + num4] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[1 + num4] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[2 + num4] = TMP_MeshInfo.s_DefaultNormal;
				this.normals[3 + num4] = TMP_MeshInfo.s_DefaultNormal;
				this.tangents[0 + num4] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[1 + num4] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[2 + num4] = TMP_MeshInfo.s_DefaultTangent;
				this.tangents[3 + num4] = TMP_MeshInfo.s_DefaultTangent;
				this.triangles[0 + num5] = 0 + num4;
				this.triangles[1 + num5] = 1 + num4;
				this.triangles[2 + num5] = 2 + num4;
				this.triangles[3 + num5] = 2 + num4;
				this.triangles[4 + num5] = 3 + num4;
				this.triangles[5 + num5] = 0 + num4;
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
