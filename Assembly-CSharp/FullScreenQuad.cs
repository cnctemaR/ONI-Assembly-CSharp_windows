using System;
using UnityEngine;

public class FullScreenQuad
{
	public FullScreenQuad(string name, Camera camera, bool invert = false)
	{
		this.Camera = camera;
		this.Layer = LayerMask.NameToLayer("ForceDraw");
		this.Mesh = new Mesh();
		this.Mesh.name = name;
		this.Mesh.vertices = new Vector3[]
		{
			new Vector3(-1f, -1f, 0f),
			new Vector3(-1f, 1f, 0f),
			new Vector3(1f, -1f, 0f),
			new Vector3(1f, 1f, 0f)
		};
		float num = 1f;
		float num2 = 0f;
		if (invert)
		{
			num = 0f;
			num2 = 1f;
		}
		this.Mesh.uv = new Vector2[]
		{
			new Vector2(0f, num2),
			new Vector2(0f, num),
			new Vector2(1f, num2),
			new Vector2(1f, num)
		};
		this.Mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
		this.Mesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
		this.Material = new Material(Shader.Find("Klei/PostFX/FullScreen"));
		this.Camera.cullingMask = this.Camera.cullingMask | LayerMask.GetMask(new string[] { "ForceDraw" });
	}

	public void Draw(Texture texture)
	{
		this.Material.mainTexture = texture;
		Graphics.DrawMesh(this.Mesh, Vector3.zero, Quaternion.identity, this.Material, this.Layer, this.Camera, 0, null, false, false);
	}

	private Mesh Mesh;

	private Camera Camera;

	private Material Material;

	private int Layer;
}
