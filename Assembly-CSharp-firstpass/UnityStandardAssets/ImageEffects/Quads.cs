using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	internal class Quads
	{
		private static bool HasMeshes()
		{
			bool flag;
			if (Quads.meshes == null)
			{
				flag = false;
			}
			else
			{
				foreach (Mesh mesh in Quads.meshes)
				{
					if (null == mesh)
					{
						return false;
					}
				}
				flag = true;
			}
			return flag;
		}

		public static void Cleanup()
		{
			if (Quads.meshes != null)
			{
				for (int i = 0; i < Quads.meshes.Length; i++)
				{
					if (null != Quads.meshes[i])
					{
						global::UnityEngine.Object.DestroyImmediate(Quads.meshes[i]);
						Quads.meshes[i] = null;
					}
				}
				Quads.meshes = null;
			}
		}

		public static Mesh[] GetMeshes(int totalWidth, int totalHeight)
		{
			Mesh[] array;
			if (Quads.HasMeshes() && Quads.currentQuads == totalWidth * totalHeight)
			{
				array = Quads.meshes;
			}
			else
			{
				int num = 10833;
				int num2 = totalWidth * totalHeight;
				Quads.currentQuads = num2;
				int num3 = Mathf.CeilToInt(1f * (float)num2 / (1f * (float)num));
				Quads.meshes = new Mesh[num3];
				int num4 = 0;
				for (int i = 0; i < num2; i += num)
				{
					int num5 = Mathf.FloorToInt((float)Mathf.Clamp(num2 - i, 0, num));
					Quads.meshes[num4] = Quads.GetMesh(num5, i, totalWidth, totalHeight);
					num4++;
				}
				array = Quads.meshes;
			}
			return array;
		}

		private static Mesh GetMesh(int triCount, int triOffset, int totalWidth, int totalHeight)
		{
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
			Vector3[] array = new Vector3[triCount * 4];
			Vector2[] array2 = new Vector2[triCount * 4];
			Vector2[] array3 = new Vector2[triCount * 4];
			int[] array4 = new int[triCount * 6];
			for (int i = 0; i < triCount; i++)
			{
				int num = i * 4;
				int num2 = i * 6;
				int num3 = triOffset + i;
				float num4 = Mathf.Floor((float)(num3 % totalWidth)) / (float)totalWidth;
				float num5 = Mathf.Floor((float)(num3 / totalWidth)) / (float)totalHeight;
				Vector3 vector = new Vector3(num4 * 2f - 1f, num5 * 2f - 1f, 1f);
				array[num] = vector;
				array[num + 1] = vector;
				array[num + 2] = vector;
				array[num + 3] = vector;
				array2[num] = new Vector2(0f, 0f);
				array2[num + 1] = new Vector2(1f, 0f);
				array2[num + 2] = new Vector2(0f, 1f);
				array2[num + 3] = new Vector2(1f, 1f);
				array3[num] = new Vector2(num4, num5);
				array3[num + 1] = new Vector2(num4, num5);
				array3[num + 2] = new Vector2(num4, num5);
				array3[num + 3] = new Vector2(num4, num5);
				array4[num2] = num;
				array4[num2 + 1] = num + 1;
				array4[num2 + 2] = num + 2;
				array4[num2 + 3] = num + 1;
				array4[num2 + 4] = num + 2;
				array4[num2 + 5] = num + 3;
			}
			mesh.vertices = array;
			mesh.triangles = array4;
			mesh.uv = array2;
			mesh.uv2 = array3;
			return mesh;
		}

		private static Mesh[] meshes;

		private static int currentQuads = 0;
	}
}
