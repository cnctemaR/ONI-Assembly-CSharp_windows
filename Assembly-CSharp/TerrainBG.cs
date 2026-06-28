using System;
using UnityEngine;

public class TerrainBG : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.layer = LayerMask.NameToLayer("Default");
		this.worldPlane = this.CreateWorldPlane("WorldPlane");
		this.gasPlane = this.CreateGasPlane("GasPlane");
		this.backgroundMaterial = global::UnityEngine.Object.Instantiate<Material>(this.backgroundMaterial);
	}

	public Mesh CreateGasPlane(string name)
	{
		Mesh mesh = new Mesh();
		mesh.name = name;
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		array = new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3((float)Grid.WidthInCells, 0f, 0f),
			new Vector3(0f, Grid.HeightInMeters, 0f),
			new Vector3(Grid.WidthInMeters, Grid.HeightInMeters, 0f)
		};
		array2 = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		array3 = new int[] { 0, 2, 1, 1, 2, 3 };
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.bounds = new Bounds(new Vector3((float)Grid.WidthInCells * 0.5f, (float)Grid.HeightInCells * 0.5f, 0f), new Vector3((float)Grid.WidthInCells, (float)Grid.HeightInCells, 0f));
		return mesh;
	}

	public Mesh CreateWorldPlane(string name)
	{
		Mesh mesh = new Mesh();
		mesh.name = name;
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		array = new Vector3[]
		{
			new Vector3((float)(-(float)Grid.WidthInCells), (float)(-(float)Grid.HeightInCells), 0f),
			new Vector3((float)Grid.WidthInCells * 2f, (float)(-(float)Grid.HeightInCells), 0f),
			new Vector3((float)(-(float)Grid.WidthInCells), Grid.HeightInMeters * 2f, 0f),
			new Vector3(Grid.WidthInMeters * 2f, Grid.HeightInMeters * 2f, 0f)
		};
		array2 = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		array3 = new int[] { 0, 2, 1, 1, 2, 3 };
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.bounds = new Bounds(new Vector3((float)Grid.WidthInCells * 0.5f, (float)Grid.HeightInCells * 0.5f, 0f), new Vector3((float)Grid.WidthInCells, (float)Grid.HeightInCells, 0f));
		return mesh;
	}

	private void LateUpdate()
	{
		if (this.doDraw)
		{
			this.backgroundMaterial.renderQueue = RenderQueues.Background;
			for (int i = 0; i < Lighting.Instance.Settings.BackgroundLayers; i++)
			{
				if (i >= Lighting.Instance.Settings.BackgroundLayers - 1)
				{
					float num = (float)i / (float)(Lighting.Instance.Settings.BackgroundLayers - 1);
					float num2 = Mathf.Lerp(1f, Lighting.Instance.Settings.BackgroundDarkening, num);
					float num3 = Mathf.Lerp(1f, Lighting.Instance.Settings.BackgroundUVScale, num);
					float num4 = 1f;
					if (i == Lighting.Instance.Settings.BackgroundLayers - 1)
					{
						num4 = 0f;
					}
					MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
					Vector3 vector = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Background));
					materialPropertyBlock.SetVector("_BackWallParameters", new Vector4(num2, Lighting.Instance.Settings.BackgroundClip, num3, num4));
					Graphics.DrawMesh(this.worldPlane, vector, Quaternion.identity, this.backgroundMaterial, this.layer, null, 0, materialPropertyBlock);
				}
			}
			this.gasMaterial.renderQueue = RenderQueues.Gas;
			Graphics.DrawMesh(this.gasPlane, Vector3.zero, Quaternion.identity, this.gasMaterial, this.layer, null, 0, null);
		}
	}

	public Material backgroundMaterial;

	public Material gasMaterial;

	public bool doDraw = true;

	private Mesh worldPlane;

	private Mesh gasPlane;

	private int layer;
}
