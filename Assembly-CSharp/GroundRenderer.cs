using System;
using System.Collections.Generic;
using ProcGen;
using UnityEngine;
using UnityEngine.Rendering;

public class GroundRenderer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ShaderReloader.Register(new global::System.Action(this.OnShadersReloaded));
		this.OnShadersReloaded();
		this.masks.Initialize();
		SubWorld.ZoneType[] array = (SubWorld.ZoneType[])Enum.GetValues(typeof(SubWorld.ZoneType));
		this.biomeMasks = new GroundMasks.BiomeMaskData[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			SubWorld.ZoneType zoneType = array[i];
			this.biomeMasks[i] = this.GetBiomeMask(zoneType);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.size = new Vector2I((Grid.WidthInCells + 16 - 1) / 16, (Grid.HeightInCells + 16 - 1) / 16);
		this.dirtyChunks = new bool[this.size.x, this.size.y];
		this.worldChunks = new GroundRenderer.WorldChunk[this.size.x, this.size.y];
		for (int i = 0; i < this.size.y; i++)
		{
			for (int j = 0; j < this.size.x; j++)
			{
				this.worldChunks[j, i] = new GroundRenderer.WorldChunk(j, i);
				this.dirtyChunks[j, i] = true;
			}
		}
	}

	public void Render(Vector2I vis_min, Vector2I vis_max)
	{
		if (!base.enabled)
		{
			return;
		}
		int num = LayerMask.NameToLayer("World");
		Vector2I vector2I = new Vector2I(vis_min.x / 16, vis_min.y / 16);
		Vector2I vector2I2 = new Vector2I((vis_max.x + 16 - 1) / 16, (vis_max.y + 16 - 1) / 16);
		for (int i = vector2I.y; i < vector2I2.y; i++)
		{
			for (int j = vector2I.x; j < vector2I2.x; j++)
			{
				GroundRenderer.WorldChunk worldChunk = this.worldChunks[j, i];
				if (this.dirtyChunks[j, i] || GroundRenderer.forceVisibleRebuild)
				{
					this.dirtyChunks[j, i] = false;
					worldChunk.Rebuild(this.biomeMasks, this.elementMaterials);
				}
				worldChunk.Render(num);
			}
		}
		this.RebuildDirtyChunks();
	}

	private void RebuildDirtyChunks()
	{
		for (int i = 0; i < this.dirtyChunks.GetLength(1); i++)
		{
			for (int j = 0; j < this.dirtyChunks.GetLength(0); j++)
			{
				if (this.dirtyChunks[j, i])
				{
					this.dirtyChunks[j, i] = false;
					this.worldChunks[j, i].Rebuild(this.biomeMasks, this.elementMaterials);
				}
			}
		}
	}

	public void MarkDirty(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = new Vector2I(vector2I.x / 16, vector2I.y / 16);
		this.dirtyChunks[vector2I2.x, vector2I2.y] = true;
		bool flag = vector2I.x % 16 == 0 && vector2I2.x > 0;
		bool flag2 = vector2I.x % 16 == 15 && vector2I2.x < this.size.x - 1;
		bool flag3 = vector2I.y % 16 == 0 && vector2I2.y > 0;
		bool flag4 = vector2I.y % 16 == 15 && vector2I2.y < this.size.y - 1;
		if (flag)
		{
			this.dirtyChunks[vector2I2.x - 1, vector2I2.y] = true;
			if (flag3)
			{
				this.dirtyChunks[vector2I2.x - 1, vector2I2.y - 1] = true;
			}
			if (flag4)
			{
				this.dirtyChunks[vector2I2.x - 1, vector2I2.y + 1] = true;
			}
		}
		if (flag3)
		{
			this.dirtyChunks[vector2I2.x, vector2I2.y - 1] = true;
		}
		if (flag4)
		{
			this.dirtyChunks[vector2I2.x, vector2I2.y + 1] = true;
		}
		if (flag2)
		{
			this.dirtyChunks[vector2I2.x + 1, vector2I2.y] = true;
			if (flag3)
			{
				this.dirtyChunks[vector2I2.x + 1, vector2I2.y - 1] = true;
			}
			if (flag4)
			{
				this.dirtyChunks[vector2I2.x + 1, vector2I2.y + 1] = true;
			}
		}
	}

	private Vector2I GetChunkIdx(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		return new Vector2I(vector2I.x / 16, vector2I.y / 16);
	}

	private GroundMasks.BiomeMaskData GetBiomeMask(SubWorld.ZoneType zone_type)
	{
		GroundMasks.BiomeMaskData biomeMaskData = null;
		string text = zone_type.ToString().ToLower();
		foreach (KeyValuePair<string, GroundMasks.BiomeMaskData> keyValuePair in this.masks.biomeMasks)
		{
			string key = keyValuePair.Key;
			if (text == key)
			{
				biomeMaskData = keyValuePair.Value;
				break;
			}
		}
		return biomeMaskData;
	}

	private void InitOpaqueMaterial(Material material, Element element)
	{
		material.name = element.id.ToString() + "_opaque";
		material.renderQueue = RenderQueues.WorldOpaque;
		material.EnableKeyword("OPAQUE");
		material.DisableKeyword("ALPHA");
		this.ConfigureMaterialShine(material);
		material.SetInt("_SrcAlpha", 1);
		material.SetInt("_DstAlpha", 0);
		material.SetInt("_ZWrite", 1);
		material.SetTexture("_AlphaTestMap", Texture2D.whiteTexture);
	}

	private void InitAlphaMaterial(Material material, Element element)
	{
		material.name = element.id.ToString() + "_alpha";
		material.renderQueue = RenderQueues.WorldTransparent;
		material.EnableKeyword("ALPHA");
		material.DisableKeyword("OPAQUE");
		this.ConfigureMaterialShine(material);
		material.SetTexture("_AlphaTestMap", this.masks.maskAtlas.texture);
		material.SetInt("_SrcAlpha", 5);
		material.SetInt("_DstAlpha", 10);
		material.SetInt("_ZWrite", 0);
	}

	private void ConfigureMaterialShine(Material material)
	{
		Texture texture = material.GetTexture("_ShineMask");
		if (texture != null)
		{
			material.DisableKeyword("MATTE");
			material.EnableKeyword("SHINY");
		}
		else
		{
			material.EnableKeyword("MATTE");
			material.DisableKeyword("SHINY");
		}
	}

	[ContextMenu("Reload Shaders")]
	public void OnShadersReloaded()
	{
		this.FreeMaterials();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid)
			{
				if (element.substance.material == null)
				{
					Output.LogError(new object[] { element.name, "must have material associated with it in the substance table" });
				}
				Material material = new Material(element.substance.material);
				this.InitOpaqueMaterial(material, element);
				Material material2 = new Material(material);
				this.InitAlphaMaterial(material2, element);
				GroundRenderer.Materials materials = new GroundRenderer.Materials(material, material2);
				this.elementMaterials[element.id] = materials;
			}
		}
		if (this.worldChunks != null)
		{
			for (int i = 0; i < this.dirtyChunks.GetLength(1); i++)
			{
				for (int j = 0; j < this.dirtyChunks.GetLength(0); j++)
				{
					this.dirtyChunks[j, i] = true;
				}
			}
			GroundRenderer.WorldChunk[,] array = this.worldChunks;
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int k = 0; k < length; k++)
			{
				for (int l = 0; l < length2; l++)
				{
					GroundRenderer.WorldChunk worldChunk = array[k, l];
					worldChunk.Clear();
					worldChunk.Rebuild(this.biomeMasks, this.elementMaterials);
				}
			}
		}
	}

	public void FreeResources()
	{
		this.FreeMaterials();
		this.elementMaterials.Clear();
		this.elementMaterials = null;
		if (this.worldChunks != null)
		{
			GroundRenderer.WorldChunk[,] array = this.worldChunks;
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					GroundRenderer.WorldChunk worldChunk = array[i, j];
					worldChunk.FreeResources();
				}
			}
			this.worldChunks = null;
		}
	}

	private void FreeMaterials()
	{
		foreach (GroundRenderer.Materials materials in this.elementMaterials.Values)
		{
			global::UnityEngine.Object.Destroy(materials.opaque);
			global::UnityEngine.Object.Destroy(materials.alpha);
		}
		this.elementMaterials.Clear();
	}

	[SerializeField]
	private GroundMasks masks;

	private GroundMasks.BiomeMaskData[] biomeMasks;

	private Dictionary<SimHashes, GroundRenderer.Materials> elementMaterials = new Dictionary<SimHashes, GroundRenderer.Materials>();

	private bool[,] dirtyChunks;

	private GroundRenderer.WorldChunk[,] worldChunks;

	private const int ChunkEdgeSize = 16;

	private Vector2I size;

	private static bool forceVisibleRebuild;

	[Serializable]
	private struct Materials
	{
		public Materials(Material opaque, Material alpha)
		{
			this.opaque = opaque;
			this.alpha = alpha;
		}

		public Material opaque;

		public Material alpha;
	}

	private class ElementChunk
	{
		public ElementChunk(SimHashes element, Dictionary<SimHashes, GroundRenderer.Materials> materials)
		{
			this.element = element;
			GroundRenderer.Materials materials2 = materials[element];
			this.alpha = new GroundRenderer.ElementChunk.RenderData(materials2.alpha);
			this.opaque = new GroundRenderer.ElementChunk.RenderData(materials2.opaque);
			this.Clear();
		}

		public void Clear()
		{
			this.opaque.Clear();
			this.alpha.Clear();
			this.tileCount = 0;
		}

		public void AddOpaqueQuad(int x, int y, GroundMasks.UVData uvs)
		{
			this.opaque.AddQuad(x, y, uvs);
			this.tileCount++;
		}

		public void AddAlphaQuad(int x, int y, GroundMasks.UVData uvs)
		{
			this.alpha.AddQuad(x, y, uvs);
			this.tileCount++;
		}

		public void Build()
		{
			this.opaque.Build();
			this.alpha.Build();
		}

		public void Render(int layer, int element_idx)
		{
			float num = Grid.GetLayerZ(Grid.SceneLayer.Ground);
			num -= 0.0001f * (float)element_idx;
			this.opaque.Render(new Vector3(0f, 0f, num), layer);
			this.alpha.Render(new Vector3(0f, 0f, num), layer);
		}

		public void FreeResources()
		{
			this.alpha.FreeResources();
			this.opaque.FreeResources();
			this.alpha = null;
			this.opaque = null;
		}

		public SimHashes element;

		private GroundRenderer.ElementChunk.RenderData alpha;

		private GroundRenderer.ElementChunk.RenderData opaque;

		public int tileCount;

		private class RenderData
		{
			public RenderData(Material material)
			{
				this.material = material;
				this.mesh = new Mesh();
				this.mesh.MarkDynamic();
				this.mesh.name = "ElementChunk";
				this.pos = new List<Vector3>();
				this.uv = new List<Vector2>();
				this.indices = new List<int>();
			}

			public void ClearMesh()
			{
				if (this.mesh != null)
				{
					this.mesh.Clear();
					global::UnityEngine.Object.DestroyImmediate(this.mesh);
					this.mesh = null;
				}
			}

			public void Clear()
			{
				if (this.mesh != null)
				{
					this.mesh.Clear();
				}
				if (this.pos != null)
				{
					this.pos.Clear();
				}
				if (this.uv != null)
				{
					this.uv.Clear();
				}
				if (this.indices != null)
				{
					this.indices.Clear();
				}
			}

			public void FreeResources()
			{
				this.ClearMesh();
				this.Clear();
				this.pos = null;
				this.uv = null;
				this.indices = null;
				this.material = null;
			}

			public void Build()
			{
				this.mesh.SetVertices(this.pos);
				this.mesh.SetUVs(0, this.uv);
				this.mesh.SetTriangles(this.indices, 0);
			}

			public void AddQuad(int x, int y, GroundMasks.UVData uvs)
			{
				int count = this.pos.Count;
				this.indices.Add(count);
				this.indices.Add(count + 1);
				this.indices.Add(count + 3);
				this.indices.Add(count);
				this.indices.Add(count + 3);
				this.indices.Add(count + 2);
				this.pos.Add(new Vector3((float)x + -0.5f, (float)y + -0.5f, 0f));
				this.pos.Add(new Vector3((float)x + 1f + -0.5f, (float)y + -0.5f, 0f));
				this.pos.Add(new Vector3((float)x + -0.5f, (float)y + 1f + -0.5f, 0f));
				this.pos.Add(new Vector3((float)x + 1f + -0.5f, (float)y + 1f + -0.5f, 0f));
				this.uv.Add(uvs.bl);
				this.uv.Add(uvs.br);
				this.uv.Add(uvs.tl);
				this.uv.Add(uvs.tr);
			}

			public void Render(Vector3 position, int layer)
			{
				if (this.pos.Count != 0)
				{
					Graphics.DrawMesh(this.mesh, position, Quaternion.identity, this.material, layer, null, 0, null, ShadowCastingMode.Off, false, null, false);
				}
			}

			public Material material;

			public Mesh mesh;

			public List<Vector3> pos;

			public List<Vector2> uv;

			public List<int> indices;
		}
	}

	private struct WorldChunk
	{
		public WorldChunk(int x, int y)
		{
			this.chunkX = x;
			this.chunkY = y;
			this.elementChunks = new List<GroundRenderer.ElementChunk>();
		}

		public void Clear()
		{
			this.elementChunks.Clear();
		}

		private static void InsertSorted(Element element, Element[] array, int size)
		{
			int num = (int)element.id;
			for (int i = 0; i < size; i++)
			{
				Element element2 = array[i];
				if (element2.id > (SimHashes)num)
				{
					array[i] = element;
					element = element2;
					num = (int)element2.id;
				}
			}
			array[size] = element;
		}

		public void Rebuild(GroundMasks.BiomeMaskData[] biomeMasks, Dictionary<SimHashes, GroundRenderer.Materials> materials)
		{
			foreach (GroundRenderer.ElementChunk elementChunk in this.elementChunks)
			{
				elementChunk.Clear();
			}
			Vector2I vector2I = new Vector2I(this.chunkX * 16, this.chunkY * 16);
			Vector2I vector2I2 = new Vector2I(Math.Min(Grid.WidthInCells, (this.chunkX + 1) * 16), Math.Min(Grid.HeightInCells, (this.chunkY + 1) * 16));
			for (int i = vector2I.y; i < vector2I2.y; i++)
			{
				int num = Math.Max(0, i - 1);
				int num2 = i;
				for (int j = vector2I.x; j < vector2I2.x; j++)
				{
					int num3 = Math.Max(0, j - 1);
					int num4 = j;
					int num5 = num * Grid.WidthInCells + num3;
					int num6 = num * Grid.WidthInCells + num4;
					int num7 = num2 * Grid.WidthInCells + num3;
					int num8 = num2 * Grid.WidthInCells + num4;
					GroundRenderer.WorldChunk.elements[0] = Grid.Element[num5];
					GroundRenderer.WorldChunk.elements[1] = Grid.Element[num6];
					GroundRenderer.WorldChunk.elements[2] = Grid.Element[num7];
					GroundRenderer.WorldChunk.elements[3] = Grid.Element[num8];
					GroundRenderer.WorldChunk.substances[0] = ((!Grid.RenderedByWorld[num5] || !GroundRenderer.WorldChunk.elements[0].IsSolid) ? (-1) : GroundRenderer.WorldChunk.elements[0].substance.idx);
					GroundRenderer.WorldChunk.substances[1] = ((!Grid.RenderedByWorld[num6] || !GroundRenderer.WorldChunk.elements[1].IsSolid) ? (-1) : GroundRenderer.WorldChunk.elements[1].substance.idx);
					GroundRenderer.WorldChunk.substances[2] = ((!Grid.RenderedByWorld[num7] || !GroundRenderer.WorldChunk.elements[2].IsSolid) ? (-1) : GroundRenderer.WorldChunk.elements[2].substance.idx);
					GroundRenderer.WorldChunk.substances[3] = ((!Grid.RenderedByWorld[num8] || !GroundRenderer.WorldChunk.elements[3].IsSolid) ? (-1) : GroundRenderer.WorldChunk.elements[3].substance.idx);
					GroundRenderer.WorldChunk.uniqueElements[0] = GroundRenderer.WorldChunk.elements[0];
					GroundRenderer.WorldChunk.InsertSorted(GroundRenderer.WorldChunk.elements[1], GroundRenderer.WorldChunk.uniqueElements, 1);
					GroundRenderer.WorldChunk.InsertSorted(GroundRenderer.WorldChunk.elements[2], GroundRenderer.WorldChunk.uniqueElements, 2);
					GroundRenderer.WorldChunk.InsertSorted(GroundRenderer.WorldChunk.elements[3], GroundRenderer.WorldChunk.uniqueElements, 3);
					int num9 = -1;
					int num10 = i * Grid.WidthInCells + j;
					int biomeIdx = GroundRenderer.WorldChunk.GetBiomeIdx(num10);
					GroundMasks.BiomeMaskData biomeMaskData = biomeMasks[biomeIdx];
					for (int k = 0; k < GroundRenderer.WorldChunk.uniqueElements.Length; k++)
					{
						Element element = GroundRenderer.WorldChunk.uniqueElements[k];
						if (element.IsSolid)
						{
							int idx = element.substance.idx;
							if (idx != num9)
							{
								num9 = idx;
								int num11 = (((GroundRenderer.WorldChunk.substances[2] < idx) ? 0 : 1) << 3) | (((GroundRenderer.WorldChunk.substances[3] < idx) ? 0 : 1) << 2) | (((GroundRenderer.WorldChunk.substances[0] < idx) ? 0 : 1) << 1) | (((GroundRenderer.WorldChunk.substances[1] < idx) ? 0 : 1) << 0);
								if (num11 > 0)
								{
									GroundMasks.UVData[] variationUVs = biomeMaskData.tiles[num11].variationUVs;
									float staticRandom = GroundRenderer.WorldChunk.GetStaticRandom(j, i);
									int num12 = Mathf.Min(variationUVs.Length - 1, (int)((float)variationUVs.Length * staticRandom));
									GroundMasks.UVData uvdata = variationUVs[num12 % variationUVs.Length];
									GroundRenderer.ElementChunk elementChunk2 = this.GetElementChunk(element.id, materials);
									if (num11 == 15)
									{
										elementChunk2.AddOpaqueQuad(j, i, uvdata);
									}
									else
									{
										elementChunk2.AddAlphaQuad(j, i, uvdata);
									}
								}
							}
						}
					}
				}
			}
			foreach (GroundRenderer.ElementChunk elementChunk3 in this.elementChunks)
			{
				elementChunk3.Build();
			}
			for (int l = this.elementChunks.Count - 1; l >= 0; l--)
			{
				GroundRenderer.ElementChunk elementChunk4 = this.elementChunks[l];
				if (elementChunk4.tileCount == 0)
				{
					int num13 = this.elementChunks.Count - 1;
					this.elementChunks[l] = this.elementChunks[num13];
					this.elementChunks.RemoveAt(num13);
				}
			}
		}

		private GroundRenderer.ElementChunk GetElementChunk(SimHashes elementID, Dictionary<SimHashes, GroundRenderer.Materials> materials)
		{
			GroundRenderer.ElementChunk elementChunk = null;
			for (int i = 0; i < this.elementChunks.Count; i++)
			{
				if (this.elementChunks[i].element == elementID)
				{
					elementChunk = this.elementChunks[i];
					break;
				}
			}
			if (elementChunk == null)
			{
				elementChunk = new GroundRenderer.ElementChunk(elementID, materials);
				this.elementChunks.Add(elementChunk);
			}
			return elementChunk;
		}

		private static int GetBiomeIdx(int cell)
		{
			if (!Grid.IsValidCell(cell))
			{
				return 0;
			}
			return (int)global::World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
		}

		private static float GetStaticRandom(int x, int y)
		{
			return PerlinSimplexNoise.noise((float)x * GroundRenderer.WorldChunk.NoiseScale.x, (float)y * GroundRenderer.WorldChunk.NoiseScale.y);
		}

		public void Render(int layer)
		{
			for (int i = 0; i < this.elementChunks.Count; i++)
			{
				GroundRenderer.ElementChunk elementChunk = this.elementChunks[i];
				elementChunk.Render(layer, ElementLoader.FindElementByHash(elementChunk.element).substance.idx);
			}
		}

		public void FreeResources()
		{
			foreach (GroundRenderer.ElementChunk elementChunk in this.elementChunks)
			{
				elementChunk.FreeResources();
			}
			this.elementChunks.Clear();
			this.elementChunks = null;
		}

		public readonly int chunkX;

		public readonly int chunkY;

		private List<GroundRenderer.ElementChunk> elementChunks;

		private static Element[] elements = new Element[4];

		private static Element[] uniqueElements = new Element[4];

		private static int[] substances = new int[4];

		private static Vector2 NoiseScale = new Vector3(1f, 1f);
	}
}
