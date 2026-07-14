using System;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("KMonoBehaviour/scripts/WaterCubes")]
public class WaterCubes : KMonoBehaviour
{
	public static WaterCubes Instance { get; private set; }

	public static void DestroyInstance()
	{
		WaterCubes.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		WaterCubes.Instance = this;
		WaterCubes.MOLTEN_METAL_COLOR = this.material.GetColor("_MoltenMetalColor");
	}

	public void Init()
	{
		this.cubes = Util.NewGameObject(base.gameObject, "WaterCubes");
		GameObject gameObject = new GameObject();
		gameObject.name = "WaterCubesMesh";
		gameObject.transform.parent = this.cubes.transform;
		this.material.renderQueue = RenderQueues.Liquid;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		this.waterRenderer = gameObject.AddComponent<MeshRenderer>();
		this.waterRenderer.sharedMaterial = this.material;
		this.waterRenderer.shadowCastingMode = ShadowCastingMode.Off;
		this.waterRenderer.receiveShadows = false;
		this.waterRenderer.lightProbeUsage = LightProbeUsage.Off;
		this.waterRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		this.waterRenderer.sharedMaterial.SetTexture("_MainTex2", this.waveTexture);
		meshFilter.sharedMesh = this.CreateNewMesh();
		this.waterRenderer.gameObject.layer = LayerMask.NameToLayer("Water");
		this.waterRenderer.gameObject.transform.parent = base.transform;
		this.waterRenderer.gameObject.transform.SetPosition(new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Liquid)));
		if (this.liquidShaderProperties != null)
		{
			this.liquidShaderProperties.ApplyToMaterial(this.material);
		}
	}

	private void LateUpdate()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
		this.material.SetVector("_CursorWorldPosition", vector);
		if (this.liquidShaderProperties != null)
		{
			this.liquidShaderProperties.ApplyToMaterial(this.material);
		}
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

	public MeshRenderer waterRenderer;

	public LiquidShaderProperties liquidShaderProperties;

	public static Color MOLTEN_METAL_COLOR = Color.white;

	private GameObject cubes;
}
