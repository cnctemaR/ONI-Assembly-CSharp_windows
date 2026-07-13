using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TerrainBG")]
public class TerrainBG : KMonoBehaviour
{
	public bool LargeImpactorFragmentsVisible
	{
		get
		{
			return ClusterManager.Instance.activeWorld != null && ClusterManager.Instance.activeWorld.largeImpactorFragments == FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.ALLOWED && SaveGame.Instance.ColonyAchievementTracker.largeImpactorState == ColonyAchievementTracker.LargeImpactorState.Defeated;
		}
	}

	public float LargeImpactorBackgroundScale
	{
		get
		{
			return SaveGame.Instance.ColonyAchievementTracker.LargeImpactorBackgroundScale;
		}
	}

	protected override void OnPrefabInit()
	{
		TerrainBG.preventLargeImpactorFragmentsFromProgressing = false;
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.layer = LayerMask.NameToLayer("Default");
		this.noiseVolume = this.CreateTexture3D(32);
		this.starsPlane = this.CreateStarsPlane("StarsPlane");
		this.northernLightsPlane = this.CreateNorthernLightsPlane("NorthernLightsPlane");
		this.largeImpactorDefeatedPlane = this.CreateGridSizePlane("LargeImpactorDefeatedPlane");
		this.worldPlane = this.CreateWorldPlane("WorldPlane");
		this.gasPlane = this.CreateGasPlane("GasPlane");
		this.propertyBlocks = new MaterialPropertyBlock[Lighting.Instance.Settings.BackgroundLayers];
		for (int i = 0; i < this.propertyBlocks.Length; i++)
		{
			this.propertyBlocks[i] = new MaterialPropertyBlock();
		}
		this.LargeImpactorEntryProgress = (float)(this.LargeImpactorFragmentsVisible ? 1 : (-1));
		this.largeImpactorFragmentsMaterial.SetFloat("_EntryProgress", this.LargeImpactorEntryProgress);
		this.largeImpactorFragmentsMaterial.SetFloat("_LargeImpactorScale", this.LargeImpactorBackgroundScale);
	}

	private Texture3D CreateTexture3D(int size)
	{
		Color32[] array = new Color32[size * size * size];
		Texture3D texture3D = new Texture3D(size, size, size, TextureFormat.RGBA32, true);
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				for (int k = 0; k < size; k++)
				{
					Color32 color = new Color32((byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255));
					array[i + j * size + k * size * size] = color;
				}
			}
		}
		texture3D.SetPixels32(array);
		texture3D.Apply();
		return texture3D;
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

	public Mesh CreateStarsPlane(string name)
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
		Vector2 vector = new Vector2((float)Grid.WidthInCells, 2f * (float)Grid.HeightInCells);
		mesh.bounds = new Bounds(new Vector3(0.5f * vector.x, 0.5f * vector.y, 0f), new Vector3(vector.x, vector.y, 0f));
		return mesh;
	}

	public Mesh CreateNorthernLightsPlane(string name)
	{
		Mesh mesh = new Mesh();
		mesh.name = name;
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		float num2 = 1f;
		float num3 = this.northernLightSkySize * 0.5f;
		array = new Vector3[]
		{
			new Vector3(-num2, -num3, 0f),
			new Vector3(num2, -num3, 0f),
			new Vector3(-num2, num3, 0f),
			new Vector3(num2, num3, 0f)
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
		return mesh;
	}

	public Mesh CreateGridSizePlane(string name)
	{
		Mesh mesh = new Mesh();
		mesh.name = name;
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		float num2 = (float)Mathf.Max(Grid.WidthInCells, Grid.HeightInCells) / 2f;
		array = new Vector3[]
		{
			new Vector3(-num2, -num2, 0f),
			new Vector3(num2, -num2, 0f),
			new Vector3(-num2, num2, 0f),
			new Vector3(num2, num2, 0f)
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
		return mesh;
	}

	private void LateUpdate()
	{
		if (!this.doDraw)
		{
			return;
		}
		Material material = this.starsMaterial_surface;
		if (ClusterManager.Instance.activeWorld.IsModuleInterior)
		{
			Clustercraft component = ClusterManager.Instance.activeWorld.GetComponent<Clustercraft>();
			if (component.Status != Clustercraft.CraftStatus.InFlight)
			{
				material = this.starsMaterial_surface;
			}
			else if (ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(component.Location, EntityLayer.Asteroid) != null)
			{
				material = this.starsMaterial_orbit;
			}
			else
			{
				material = this.starsMaterial_space;
			}
		}
		material.renderQueue = RenderQueues.Stars;
		material.SetTexture("_NoiseVolume", this.noiseVolume);
		Vector3 vector = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Background) + 1f);
		Graphics.DrawMesh(this.starsPlane, vector, Quaternion.identity, material, this.layer);
		if (this.LargeImpactorFragmentsVisible)
		{
			Vector3 vector2 = new Vector3(CameraController.Instance.transform.position.x, CameraController.Instance.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.Background) + 0.85f);
			if (!TerrainBG.preventLargeImpactorFragmentsFromProgressing && this.LargeImpactorEntryProgress < 1f)
			{
				if (this.LargeImpactorEntryProgress < 0f)
				{
					this.LargeImpactorEntryProgress = 0f;
					this.largeImpactorFragmentsMaterial.SetFloat("_LargeImpactorScale", this.LargeImpactorBackgroundScale);
				}
				this.LargeImpactorEntryProgress += Time.unscaledDeltaTime / 1.2f;
				this.LargeImpactorEntryProgress = Mathf.Clamp01(this.LargeImpactorEntryProgress);
				this.largeImpactorFragmentsMaterial.SetFloat("_EntryProgress", this.LargeImpactorEntryProgress);
			}
			this.largeImpactorFragmentsMaterial.SetFloat("_UnscaledTime", Time.timeSinceLevelLoad);
			Graphics.DrawMesh(this.largeImpactorDefeatedPlane, vector2, Quaternion.identity, this.largeImpactorFragmentsMaterial, this.layer);
		}
		if (ClusterManager.Instance.activeWorld != null && ClusterManager.Instance.activeWorld.northernlights > 0)
		{
			Vector3 vector3 = new Vector3(CameraController.Instance.transform.position.x, CameraController.Instance.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.Background) + 0.8f);
			Graphics.DrawMesh(this.northernLightsPlane, vector3, Quaternion.identity, this.northernLightMaterial_ceres, this.layer);
		}
		this.backgroundMaterial.renderQueue = RenderQueues.Backwall;
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
				MaterialPropertyBlock materialPropertyBlock = this.propertyBlocks[i];
				materialPropertyBlock.SetVector("_BackWallParameters", new Vector4(num2, Lighting.Instance.Settings.BackgroundClip, num3, num4));
				Vector3 vector4 = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Background));
				Graphics.DrawMesh(this.worldPlane, vector4, Quaternion.identity, this.backgroundMaterial, this.layer, null, 0, materialPropertyBlock);
			}
		}
		this.gasMaterial.renderQueue = RenderQueues.Gas;
		Vector3 vector5 = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Gas));
		Graphics.DrawMesh(this.gasPlane, vector5, Quaternion.identity, this.gasMaterial, this.layer);
		Vector3 vector6 = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.GasFront));
		Graphics.DrawMesh(this.gasPlane, vector6, Quaternion.identity, this.gasMaterial, this.layer);
	}

	public Material northernLightMaterial_ceres;

	public Material largeImpactorFragmentsMaterial;

	public Material starsMaterial_surface;

	public Material starsMaterial_orbit;

	public Material starsMaterial_space;

	public Material backgroundMaterial;

	public Material gasMaterial;

	public bool doDraw = true;

	[SerializeField]
	private Texture3D noiseVolume;

	private Mesh starsPlane;

	private Mesh northernLightsPlane;

	private Mesh largeImpactorDefeatedPlane;

	private Mesh worldPlane;

	private Mesh gasPlane;

	private int layer;

	private float northernLightSkySize = 2f;

	public static bool preventLargeImpactorFragmentsFromProgressing;

	public const float LargeImpactorFragmentsEntryEffectDuration = 1.2f;

	private float LargeImpactorEntryProgress = -1f;

	private MaterialPropertyBlock[] propertyBlocks;
}
