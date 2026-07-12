using System;
using UnityEngine;

public static class StampToolPreviewUtil
{
	public static Material MakeMaterial(Texture texture)
	{
		Material material = new Material(Shader.Find("Sprites/Default"));
		material.SetTexture("_MainTex", texture);
		return material;
	}

	public static void MakeQuad(out GameObject gameObject, out MeshRenderer meshRenderer, float mesh_size, Vector4? uvBox = null)
	{
		gameObject = new GameObject();
		gameObject.layer = LayerMask.NameToLayer("Place");
		float num = mesh_size / 2f;
		float num2 = mesh_size / 2f;
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[]
		{
			new Vector3(-num, -num2, 0f),
			new Vector3(num, -num2, 0f),
			new Vector3(-num, num2, 0f),
			new Vector3(num, num2, 0f)
		};
		mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
		mesh.normals = new Vector3[]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		Mesh mesh2 = mesh;
		Vector2[] array2;
		if (uvBox != null)
		{
			Vector2[] array = new Vector2[4];
			array[0] = new Vector2(uvBox.Value.x, uvBox.Value.w);
			array[1] = new Vector2(uvBox.Value.z, uvBox.Value.w);
			array[2] = new Vector2(uvBox.Value.x, uvBox.Value.y);
			array2 = array;
			array[3] = new Vector2(uvBox.Value.z, uvBox.Value.y);
		}
		else
		{
			Vector2[] array3 = new Vector2[4];
			array3[0] = new Vector2(0f, 0f);
			array3[1] = new Vector2(1f, 0f);
			array3[2] = new Vector2(0f, 1f);
			array2 = array3;
			array3[3] = new Vector2(1f, 1f);
		}
		mesh2.uv = array2;
		Mesh mesh3 = mesh;
		gameObject.AddComponent<MeshFilter>().mesh = mesh3;
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
	}

	public static readonly Color COLOR_OK = Color.white;

	public static readonly Color COLOR_ERROR = Color.red;

	public const float SOLID_VIS_ALPHA = 1f;

	public const float LIQUID_VIS_ALPHA = 1f;

	public const float GAS_VIS_ALPHA = 1f;

	public const float BACKGROUND_ALPHA = 1f;
}
