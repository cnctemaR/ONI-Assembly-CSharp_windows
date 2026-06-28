using System;
using UnityEngine;
using UnityEngine.Rendering;

public class WaterCubes : KMonoBehaviour
{
	public static WaterCubes Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		WaterCubes.Instance = this;
	}

	public void Init()
	{
		this.cubes = Util.NewGameObject(base.gameObject, "WaterCubes");
		GameObject gameObject = new GameObject();
		gameObject.name = "WaterCubesMesh";
		gameObject.transform.parent = this.cubes.transform;
		this.material.renderQueue = RenderQueues.Liquid;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = this.material;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		meshRenderer.lightProbeUsage = LightProbeUsage.Off;
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		meshRenderer.sharedMaterial.SetTexture("_MainTex2", this.waveTexture);
		meshFilter.sharedMesh = this.CreateNewMesh();
		meshRenderer.gameObject.layer = 0;
		meshRenderer.gameObject.transform.parent = base.transform;
		meshRenderer.gameObject.transform.position = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Liquid));
	}

	private Mesh CreateNewMesh()
	{
		Mesh mesh = new Mesh();
		mesh.name = "WaterCubes";
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		Vector3[] array3 = new Vector3[num];
		Vector4[] array4 = new Vector4[num];
		int[] array5 = new int[6];
		float layerZ = Grid.GetLayerZ(Grid.SceneLayer.Liquid);
		array = new Vector3[]
		{
			new Vector3(0f, 0f, layerZ),
			new Vector3((float)Grid.WidthInCells, 0f, layerZ),
			new Vector3(0f, Grid.HeightInMeters, layerZ),
			new Vector3(Grid.WidthInMeters, Grid.HeightInMeters, layerZ)
		};
		array2 = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		array3 = new Vector3[]
		{
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f)
		};
		array4 = new Vector4[]
		{
			new Vector4(0f, 1f, 0f, -1f),
			new Vector4(0f, 1f, 0f, -1f),
			new Vector4(0f, 1f, 0f, -1f),
			new Vector4(0f, 1f, 0f, -1f)
		};
		array5 = new int[] { 0, 2, 1, 1, 2, 3 };
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.uv2 = array2;
		mesh.normals = array3;
		mesh.tangents = array4;
		mesh.triangles = array5;
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, 0f));
		return mesh;
	}

	public Material material;

	public Texture2D waveTexture;

	private GameObject cubes;
}
