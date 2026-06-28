using System;
using UnityEngine;

public class Pillars : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		Shader shader = Shader.Find("Klei/Pillar");
		this.material = new Material(shader);
		this.layer = LayerMask.NameToLayer("Default");
	}

	private void CreateMesh()
	{
	}

	private void LateUpdate()
	{
		if (!this.doDraw)
		{
			return;
		}
		if (!this.settings.Equals(this.currentSettings))
		{
			this.currentSettings = this.settings;
			this.Regenerate();
		}
		this.Render();
	}

	private void Render()
	{
		Vector3 vector = new Vector3(0f, 0f, this.currentSettings.depth);
		Graphics.DrawMesh(this.mesh, vector, Quaternion.identity, this.material, this.layer, null, 0);
	}

	private void GeneratePillars()
	{
		this.mesh = new Mesh();
		this.mesh.name = base.name;
		int num = (int)((float)Grid.WidthInCells / this.currentSettings.spacing);
		int num2 = (int)((float)Grid.HeightInCells / this.currentSettings.scale.y);
		int num3 = num * num2;
		int num4 = 4;
		int num5 = 6;
		Vector3[] array = new Vector3[num4 * num3];
		Vector2[] array2 = new Vector2[num4 * num3];
		Vector2[] array3 = new Vector2[num4 * num3];
		int[] array4 = new int[num5 * num3];
		Vector2 scale = this.currentSettings.scale;
		int num6 = 0;
		int num7 = 0;
		for (int i = 0; i < num; i++)
		{
			float num8 = (float)(i + 1) * this.currentSettings.spacing;
			for (int j = 0; j < num2; j++)
			{
				float num9 = (float)j * this.currentSettings.scale.y;
				Vector3 vector = new Vector3(num8, num9, 0f);
				float num10 = 0f;
				if (global::UnityEngine.Random.value > 0.5f)
				{
					num10 = 1f;
				}
				array[num6] = vector + new Vector3(-0.5f * scale.x, 0f, 0f);
				array[num6 + 1] = vector + new Vector3(0.5f * scale.x, 0f, 0f);
				array[num6 + 2] = vector + new Vector3(-0.5f * scale.x, scale.y, 0f);
				array[num6 + 3] = vector + new Vector3(0.5f * scale.x, scale.y, 0f);
				array2[num6] = new Vector2(0f, 0f);
				array2[num6 + 1] = new Vector2(1f, 0f);
				array2[num6 + 2] = new Vector2(0f, 1f);
				array2[num6 + 3] = new Vector2(1f, 1f);
				array3[num6] = new Vector2(num10, 0f);
				array3[num6 + 1] = new Vector2(num10, 0f);
				array3[num6 + 2] = new Vector2(num10, 0f);
				array3[num6 + 3] = new Vector2(num10, 0f);
				array4[num7++] = num6;
				array4[num7++] = num6 + 2;
				array4[num7++] = num6 + 1;
				array4[num7++] = num6 + 1;
				array4[num7++] = num6 + 2;
				array4[num7++] = num6 + 3;
				num6 += num4;
			}
		}
		this.mesh.vertices = array;
		this.mesh.uv = array2;
		this.mesh.uv2 = array3;
		this.mesh.triangles = array4;
		this.mesh.bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3((float)Grid.WidthInCells * 2f, (float)Grid.HeightInCells * 2f, 0f));
		this.material.SetVector("_Parallax", new Vector4(this.currentSettings.parallax, 0f, 0f, 0f));
	}

	private void Regenerate()
	{
		this.material.SetTexture("_MainTex", this.pillarTextures[0]);
		this.material.SetTexture("_MainTex2", this.pillarTextures[1]);
		this.CreateMesh();
		this.GeneratePillars();
	}

	public Texture2D[] pillarTextures;

	private Material material;

	private Mesh mesh;

	private int layer;

	public bool doDraw = true;

	public Pillars.Settings settings;

	private Pillars.Settings currentSettings;

	public class Pillar
	{
		public Vector3 position;
	}

	[Serializable]
	public struct Settings
	{
		public float spacing;

		public float parallax;

		public Vector2 scale;

		public float depth;
	}
}
