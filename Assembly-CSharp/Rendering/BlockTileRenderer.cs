using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering
{
	public class BlockTileRenderer : MonoBehaviour
	{
		public BlockTileRenderer()
		{
			this.forceRebuild = false;
		}

		public bool ForceRebuild
		{
			get
			{
				return this.forceRebuild;
			}
		}

		public void FreeResources()
		{
			foreach (KeyValuePair<KeyValuePair<BuildingDef, bool>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
			{
				if (keyValuePair.Value != null)
				{
					keyValuePair.Value.FreeResources();
				}
			}
			this.renderInfo.Clear();
		}

		private static bool MatchesDef(GameObject go, BuildingDef def)
		{
			return go != null && go.GetComponent<Building>().Def == def;
		}

		public virtual BlockTileRenderer.Bits GetConnectionBits(int x, int y, int query_layer)
		{
			BlockTileRenderer.Bits bits = (BlockTileRenderer.Bits)0;
			GameObject gameObject = Grid.Objects[y * Grid.WidthInCells + x, query_layer];
			BuildingDef buildingDef = ((!(gameObject != null)) ? null : gameObject.GetComponent<Building>().Def);
			if (y > 0)
			{
				int num = (y - 1) * Grid.WidthInCells + x;
				if (x > 0 && BlockTileRenderer.MatchesDef(Grid.Objects[num - 1, query_layer], buildingDef))
				{
					bits |= BlockTileRenderer.Bits.DownLeft;
				}
				if (BlockTileRenderer.MatchesDef(Grid.Objects[num, query_layer], buildingDef))
				{
					bits |= BlockTileRenderer.Bits.Down;
				}
				if (x < Grid.WidthInCells - 1 && BlockTileRenderer.MatchesDef(Grid.Objects[num + 1, query_layer], buildingDef))
				{
					bits |= BlockTileRenderer.Bits.DownRight;
				}
			}
			int num2 = y * Grid.WidthInCells + x;
			if (x > 0 && BlockTileRenderer.MatchesDef(Grid.Objects[num2 - 1, query_layer], buildingDef))
			{
				bits |= BlockTileRenderer.Bits.Left;
			}
			if (x < Grid.WidthInCells - 1 && BlockTileRenderer.MatchesDef(Grid.Objects[num2 + 1, query_layer], buildingDef))
			{
				bits |= BlockTileRenderer.Bits.Right;
			}
			if (y < Grid.HeightInCells - 1)
			{
				int num3 = (y + 1) * Grid.WidthInCells + x;
				if (x > 0 && BlockTileRenderer.MatchesDef(Grid.Objects[num3 - 1, query_layer], buildingDef))
				{
					bits |= BlockTileRenderer.Bits.UpLeft;
				}
				if (BlockTileRenderer.MatchesDef(Grid.Objects[num3, query_layer], buildingDef))
				{
					bits |= BlockTileRenderer.Bits.Up;
				}
				if (x < Grid.WidthInCells + 1 && BlockTileRenderer.MatchesDef(Grid.Objects[num3 + 1, query_layer], buildingDef))
				{
					bits |= BlockTileRenderer.Bits.UpRight;
				}
			}
			return bits;
		}

		private bool IsDecorConnectable(GameObject src, GameObject target)
		{
			if (src != null && target != null)
			{
				IBlockTileInfo component = src.GetComponent<IBlockTileInfo>();
				IBlockTileInfo component2 = target.GetComponent<IBlockTileInfo>();
				if (component != null && component2 != null)
				{
					return component.GetBlockTileConnectorID() == component2.GetBlockTileConnectorID();
				}
			}
			return false;
		}

		public virtual BlockTileRenderer.Bits GetDecorConnectionBits(int x, int y, int query_layer)
		{
			BlockTileRenderer.Bits bits = (BlockTileRenderer.Bits)0;
			GameObject gameObject = Grid.Objects[y * Grid.WidthInCells + x, query_layer];
			if (y > 0)
			{
				int num = (y - 1) * Grid.WidthInCells + x;
				if (x > 0 && Grid.Objects[num - 1, query_layer] != null)
				{
					bits |= BlockTileRenderer.Bits.DownLeft;
				}
				if (Grid.Objects[num, query_layer] != null)
				{
					bits |= BlockTileRenderer.Bits.Down;
				}
				if (x < Grid.WidthInCells - 1 && Grid.Objects[num + 1, query_layer] != null)
				{
					bits |= BlockTileRenderer.Bits.DownRight;
				}
			}
			int num2 = y * Grid.WidthInCells + x;
			if (x > 0 && this.IsDecorConnectable(gameObject, Grid.Objects[num2 - 1, query_layer]))
			{
				bits |= BlockTileRenderer.Bits.Left;
			}
			if (x < Grid.WidthInCells - 1 && this.IsDecorConnectable(gameObject, Grid.Objects[num2 + 1, query_layer]))
			{
				bits |= BlockTileRenderer.Bits.Right;
			}
			if (y < Grid.HeightInCells - 1)
			{
				int num3 = (y + 1) * Grid.WidthInCells + x;
				if (x > 0 && Grid.Objects[num3 - 1, query_layer] != null)
				{
					bits |= BlockTileRenderer.Bits.UpLeft;
				}
				if (Grid.Objects[num3, query_layer] != null)
				{
					bits |= BlockTileRenderer.Bits.Up;
				}
				if (x < Grid.WidthInCells + 1 && Grid.Objects[num3 + 1, query_layer] != null)
				{
					bits |= BlockTileRenderer.Bits.UpRight;
				}
			}
			return bits;
		}

		public void LateUpdate()
		{
			GridArea visibleArea = GridVisibleArea.GetVisibleArea();
			Vector2I vector2I = new Vector2I(visibleArea.Min.x / 16, visibleArea.Min.y / 16);
			Vector2I vector2I2 = new Vector2I((visibleArea.Max.x + 16 - 1) / 16, (visibleArea.Max.y + 16 - 1) / 16);
			foreach (BlockTileRenderer.RenderInfo renderInfo in this.renderInfo.Values)
			{
				for (int i = vector2I.y; i < vector2I2.y; i++)
				{
					for (int j = vector2I.x; j < vector2I2.x; j++)
					{
						renderInfo.Rebuild(this, j, i, MeshUtil.vertices, MeshUtil.uvs, MeshUtil.indices, MeshUtil.colours);
						renderInfo.Render(j, i);
					}
				}
			}
		}

		public Color GetCellColour(int cell, SimHashes element)
		{
			Color white;
			if (cell == this.selectedCell)
			{
				white = this.selectColour;
			}
			else if (cell == this.invalidPlaceCell && element == SimHashes.Void)
			{
				white = this.invalidPlaceColour;
			}
			else if (cell == this.highlightCell)
			{
				white = this.highlightColour;
			}
			else
			{
				white = Color.white;
			}
			return white;
		}

		public static Vector2I GetChunkIdx(int cell)
		{
			Vector2I vector2I = Grid.CellToXY(cell);
			return new Vector2I(vector2I.x / 16, vector2I.y / 16);
		}

		public void AddBlock(int renderLayer, BuildingDef def, SimHashes element, int cell)
		{
			KeyValuePair<BuildingDef, bool> keyValuePair = new KeyValuePair<BuildingDef, bool>(def, element != SimHashes.Void);
			BlockTileRenderer.RenderInfo renderInfo;
			if (!this.renderInfo.TryGetValue(keyValuePair, out renderInfo))
			{
				renderInfo = new BlockTileRenderer.RenderInfo(this, (int)def.TileLayer, renderLayer, def, element);
				this.renderInfo[keyValuePair] = renderInfo;
			}
			renderInfo.AddCell(cell);
		}

		public void RemoveBlock(BuildingDef def, SimHashes element, int cell)
		{
			KeyValuePair<BuildingDef, bool> keyValuePair = new KeyValuePair<BuildingDef, bool>(def, element != SimHashes.Void);
			BlockTileRenderer.RenderInfo renderInfo;
			if (this.renderInfo.TryGetValue(keyValuePair, out renderInfo))
			{
				renderInfo.RemoveCell(cell);
			}
		}

		public void Rebuild(ObjectLayer layer, int cell)
		{
			foreach (KeyValuePair<KeyValuePair<BuildingDef, bool>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
			{
				if (keyValuePair.Key.Key.TileLayer == layer)
				{
					keyValuePair.Value.MarkDirty(cell);
				}
			}
		}

		public void SelectCell(int cell, bool enabled)
		{
			this.UpdateCellStatus(ref this.selectedCell, cell, enabled);
		}

		public void HighlightCell(int cell, bool enabled)
		{
			this.UpdateCellStatus(ref this.highlightCell, cell, enabled);
		}

		public void SetInvalidPlaceCell(int cell, bool enabled)
		{
			this.UpdateCellStatus(ref this.invalidPlaceCell, cell, enabled);
		}

		private void UpdateCellStatus(ref int cell_status, int cell, bool enabled)
		{
			if (enabled)
			{
				if (cell != cell_status)
				{
					if (cell_status != -1)
					{
						foreach (KeyValuePair<KeyValuePair<BuildingDef, bool>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
						{
							keyValuePair.Value.MarkDirtyIfOccupied(cell_status);
						}
					}
					cell_status = cell;
					foreach (KeyValuePair<KeyValuePair<BuildingDef, bool>, BlockTileRenderer.RenderInfo> keyValuePair2 in this.renderInfo)
					{
						keyValuePair2.Value.MarkDirtyIfOccupied(cell_status);
					}
				}
			}
			else if (cell_status == cell)
			{
				foreach (KeyValuePair<KeyValuePair<BuildingDef, bool>, BlockTileRenderer.RenderInfo> keyValuePair3 in this.renderInfo)
				{
					keyValuePair3.Value.MarkDirty(cell_status);
				}
				cell_status = -1;
			}
		}

		[SerializeField]
		private bool forceRebuild = false;

		[SerializeField]
		private Color highlightColour = new Color(1.25f, 1.25f, 1.25f, 1f);

		[SerializeField]
		private Color selectColour = new Color(1.5f, 1.5f, 1.5f, 1f);

		[SerializeField]
		private Color invalidPlaceColour = Color.red;

		private const int chunkEdgeSize = 16;

		protected Dictionary<KeyValuePair<BuildingDef, bool>, BlockTileRenderer.RenderInfo> renderInfo = new Dictionary<KeyValuePair<BuildingDef, bool>, BlockTileRenderer.RenderInfo>();

		private int selectedCell = -1;

		private int highlightCell = -1;

		private int invalidPlaceCell = -1;

		[Flags]
		public enum Bits
		{
			UpLeft = 128,
			Up = 64,
			UpRight = 32,
			Left = 16,
			Right = 8,
			DownLeft = 4,
			Down = 2,
			DownRight = 1
		}

		protected class RenderInfo
		{
			public RenderInfo(BlockTileRenderer renderer, int queryLayer, int renderLayer, BuildingDef def, SimHashes element)
			{
				this.queryLayer = queryLayer;
				this.renderLayer = renderLayer;
				this.rootPosition = new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer));
				this.element = element;
				this.material = new Material(def.BlockTileMaterial);
				if (def.SceneLayer == Grid.SceneLayer.TileMain)
				{
					this.material.renderQueue = RenderQueues.BlockTiles;
				}
				this.material.DisableKeyword("ENABLE_SHINE");
				if (element != SimHashes.Void)
				{
					this.material.SetTexture("_MainTex", def.BlockTileAtlas.texture);
					this.material.name = def.BlockTileAtlas.name + "Mat";
					if (def.BlockTileShineAtlas != null)
					{
						this.material.SetTexture("_SpecularTex", def.BlockTileShineAtlas.texture);
						this.material.EnableKeyword("ENABLE_SHINE");
					}
				}
				else
				{
					this.material.SetTexture("_MainTex", def.BlockTilePlaceAtlas.texture);
					this.material.name = def.BlockTilePlaceAtlas.name + "Mat";
				}
				int num = Grid.WidthInCells / 16;
				int num2 = Grid.HeightInCells / 16;
				this.meshChunks = new Mesh[num, num2];
				this.dirtyChunks = new bool[num, num2];
				for (int i = 0; i < num2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						this.dirtyChunks[j, i] = true;
					}
				}
				BlockTileDecorInfo blockTileDecorInfo = ((element != SimHashes.Void) ? def.DecorBlockTileInfo : def.DecorPlaceBlockTileInfo);
				if (blockTileDecorInfo)
				{
					this.decorRenderInfo = new BlockTileRenderer.DecorRenderInfo(num, num2, queryLayer, def, blockTileDecorInfo);
				}
				string name = def.BlockTileAtlas.items[0].name;
				int length = name.Length;
				int num3 = length - 4;
				int num4 = num3 - 8;
				int num5 = num4 - 1;
				int num6 = num5 - 8;
				this.atlasInfo = new BlockTileRenderer.RenderInfo.AtlasInfo[def.BlockTileAtlas.items.Length];
				for (int k = 0; k < this.atlasInfo.Length; k++)
				{
					TextureAtlas.Item item = def.BlockTileAtlas.items[k];
					string text = item.name.Substring(num6, 8);
					string text2 = item.name.Substring(num4, 8);
					int num7 = Convert.ToInt32(text, 2);
					int num8 = Convert.ToInt32(text2, 2);
					this.atlasInfo[k].requiredConnections = (BlockTileRenderer.Bits)num7;
					this.atlasInfo[k].forbiddenConnections = (BlockTileRenderer.Bits)num8;
					this.atlasInfo[k].uvBox = item.uvBox;
					this.atlasInfo[k].name = item.name;
				}
				this.trimUVSize = new Vector2(64f / (float)def.BlockTileAtlas.texture.width, 64f / (float)def.BlockTileAtlas.texture.height);
			}

			public void FreeResources()
			{
				global::UnityEngine.Object.DestroyImmediate(this.material);
				this.material = null;
				this.atlasInfo = null;
				for (int i = 0; i < this.meshChunks.GetLength(0); i++)
				{
					for (int j = 0; j < this.meshChunks.GetLength(1); j++)
					{
						if (this.meshChunks[i, j] != null)
						{
							global::UnityEngine.Object.DestroyImmediate(this.meshChunks[i, j]);
							this.meshChunks[i, j] = null;
						}
					}
				}
				this.meshChunks = null;
				this.decorRenderInfo = null;
				this.occupiedCells.Clear();
			}

			public void AddCell(int cell)
			{
				int num = 0;
				this.occupiedCells.TryGetValue(cell, out num);
				this.occupiedCells[cell] = num + 1;
				this.MarkDirty(cell);
			}

			public void RemoveCell(int cell)
			{
				int num = 0;
				this.occupiedCells.TryGetValue(cell, out num);
				if (num > 1)
				{
					this.occupiedCells[cell] = num - 1;
				}
				else
				{
					this.occupiedCells.Remove(cell);
				}
				this.MarkDirty(cell);
			}

			public void MarkDirty(int cell)
			{
				Vector2I chunkIdx = BlockTileRenderer.GetChunkIdx(cell);
				this.dirtyChunks[chunkIdx.x, chunkIdx.y] = true;
			}

			public void MarkDirtyIfOccupied(int cell)
			{
				if (this.occupiedCells.ContainsKey(cell))
				{
					this.MarkDirty(cell);
				}
			}

			public void Render(int x, int y)
			{
				if (this.meshChunks[x, y] != null)
				{
					Graphics.DrawMesh(this.meshChunks[x, y], this.rootPosition, Quaternion.identity, this.material, this.renderLayer);
				}
				if (this.decorRenderInfo != null)
				{
					this.decorRenderInfo.Render(x, y, this.rootPosition - new Vector3(0f, 0f, 0.5f), this.renderLayer);
				}
			}

			public void Rebuild(BlockTileRenderer renderer, int chunk_x, int chunk_y, List<Vector3> vertices, List<Vector2> uvs, List<int> indices, List<Color> colours)
			{
				if (this.dirtyChunks[chunk_x, chunk_y] || renderer.ForceRebuild)
				{
					this.dirtyChunks[chunk_x, chunk_y] = false;
					vertices.Clear();
					uvs.Clear();
					indices.Clear();
					colours.Clear();
					for (int i = chunk_y * 16; i < chunk_y * 16 + 16; i++)
					{
						for (int j = chunk_x * 16; j < chunk_x * 16 + 16; j++)
						{
							int num = i * Grid.WidthInCells + j;
							if (this.occupiedCells.ContainsKey(num))
							{
								BlockTileRenderer.Bits connectionBits = renderer.GetConnectionBits(j, i, this.queryLayer);
								for (int k = 0; k < this.atlasInfo.Length; k++)
								{
									bool flag = (this.atlasInfo[k].requiredConnections & connectionBits) == this.atlasInfo[k].requiredConnections;
									bool flag2 = (this.atlasInfo[k].forbiddenConnections & connectionBits) != (BlockTileRenderer.Bits)0;
									if (flag && !flag2)
									{
										Color cellColour = renderer.GetCellColour(num, this.element);
										this.AddVertexInfo(this.atlasInfo[k], this.trimUVSize, j, i, connectionBits, cellColour, vertices, uvs, indices, colours);
										break;
									}
								}
							}
						}
					}
					Mesh mesh = this.meshChunks[chunk_x, chunk_y];
					if (vertices.Count > 0)
					{
						if (mesh == null)
						{
							mesh = new Mesh();
							mesh.name = "BlockTile";
							this.meshChunks[chunk_x, chunk_y] = mesh;
						}
						mesh.Clear();
						mesh.SetVertices(vertices);
						mesh.SetUVs(0, uvs);
						mesh.SetColors(colours);
						mesh.SetTriangles(indices, 0);
					}
					else if (mesh != null)
					{
						this.meshChunks[chunk_x, chunk_y] = null;
					}
					if (this.decorRenderInfo != null)
					{
						this.decorRenderInfo.Rebuild(renderer, this.occupiedCells, chunk_x, chunk_y, 16, vertices, uvs, colours, indices, this.element);
					}
				}
			}

			private void AddVertexInfo(BlockTileRenderer.RenderInfo.AtlasInfo atlas_info, Vector2 uv_trim_size, int x, int y, BlockTileRenderer.Bits connection_bits, Color color, List<Vector3> vertices, List<Vector2> uvs, List<int> indices, List<Color> colours)
			{
				Vector2 vector = new Vector2((float)x, (float)y);
				Vector2 vector2 = vector + new Vector2(1f, 1f);
				Vector2 vector3 = new Vector2(atlas_info.uvBox.x, atlas_info.uvBox.w);
				Vector2 vector4 = new Vector2(atlas_info.uvBox.z, atlas_info.uvBox.y);
				if ((connection_bits & BlockTileRenderer.Bits.Left) == (BlockTileRenderer.Bits)0)
				{
					vector.x -= 0.25f;
				}
				else
				{
					vector3.x += uv_trim_size.x;
				}
				if ((connection_bits & BlockTileRenderer.Bits.Right) == (BlockTileRenderer.Bits)0)
				{
					vector2.x += 0.25f;
				}
				else
				{
					vector4.x -= uv_trim_size.x;
				}
				if ((connection_bits & BlockTileRenderer.Bits.Up) == (BlockTileRenderer.Bits)0)
				{
					vector2.y += 0.25f;
				}
				else
				{
					vector4.y -= uv_trim_size.y;
				}
				if ((connection_bits & BlockTileRenderer.Bits.Down) == (BlockTileRenderer.Bits)0)
				{
					vector.y -= 0.25f;
				}
				else
				{
					vector3.y += uv_trim_size.y;
				}
				int count = vertices.Count;
				vertices.Add(vector);
				vertices.Add(new Vector2(vector2.x, vector.y));
				vertices.Add(vector2);
				vertices.Add(new Vector2(vector.x, vector2.y));
				uvs.Add(vector3);
				uvs.Add(new Vector2(vector4.x, vector3.y));
				uvs.Add(vector4);
				uvs.Add(new Vector2(vector3.x, vector4.y));
				indices.Add(count);
				indices.Add(count + 1);
				indices.Add(count + 2);
				indices.Add(count);
				indices.Add(count + 2);
				indices.Add(count + 3);
				colours.Add(color);
				colours.Add(color);
				colours.Add(color);
				colours.Add(color);
			}

			private BlockTileRenderer.RenderInfo.AtlasInfo[] atlasInfo = null;

			private bool[,] dirtyChunks;

			private int queryLayer;

			private Material material;

			private int renderLayer;

			private Mesh[,] meshChunks;

			private BlockTileRenderer.DecorRenderInfo decorRenderInfo;

			private Vector2 trimUVSize;

			private Vector3 rootPosition;

			private Dictionary<int, int> occupiedCells = new Dictionary<int, int>();

			private SimHashes element;

			private const float core_size = 256f;

			private const float trim_size = 64f;

			private const float cell_size = 1f;

			private const float world_trim_size = 0.25f;

			private struct AtlasInfo
			{
				public BlockTileRenderer.Bits requiredConnections;

				public BlockTileRenderer.Bits forbiddenConnections;

				public Vector4 uvBox;

				public string name;
			}
		}

		private class DecorRenderInfo
		{
			public DecorRenderInfo(int num_x_chunks, int num_y_chunks, int query_layer, BuildingDef def, BlockTileDecorInfo decorInfo)
			{
				this.decorInfo = decorInfo;
				this.queryLayer = query_layer;
				this.material = new Material(def.BlockTileMaterial);
				if (def.SceneLayer == Grid.SceneLayer.TileMain)
				{
					this.material.renderQueue = RenderQueues.BlockTiles;
				}
				this.material.SetTexture("_MainTex", decorInfo.atlas.texture);
				if (decorInfo.atlasSpec != null)
				{
					this.material.SetTexture("_SpecularTex", decorInfo.atlasSpec.texture);
					this.material.EnableKeyword("ENABLE_SHINE");
				}
				else
				{
					this.material.DisableKeyword("ENABLE_SHINE");
				}
				this.meshChunks = new Mesh[num_x_chunks, num_y_chunks];
			}

			public void FreeResources()
			{
				this.decorInfo = null;
				global::UnityEngine.Object.DestroyImmediate(this.material);
				this.material = null;
				for (int i = 0; i < this.meshChunks.GetLength(0); i++)
				{
					for (int j = 0; j < this.meshChunks.GetLength(1); j++)
					{
						if (this.meshChunks[i, j] != null)
						{
							global::UnityEngine.Object.DestroyImmediate(this.meshChunks[i, j]);
							this.meshChunks[i, j] = null;
						}
					}
				}
				this.meshChunks = null;
				this.triangles.Clear();
			}

			public void Render(int x, int y, Vector3 position, int renderLayer)
			{
				if (this.meshChunks[x, y] != null)
				{
					Graphics.DrawMesh(this.meshChunks[x, y], position, Quaternion.identity, this.material, renderLayer);
				}
			}

			public void Rebuild(BlockTileRenderer renderer, Dictionary<int, int> occupiedCells, int chunk_x, int chunk_y, int chunkEdgeSize, List<Vector3> vertices, List<Vector2> uvs, List<Color> colours, List<int> indices, SimHashes element)
			{
				vertices.Clear();
				uvs.Clear();
				this.triangles.Clear();
				colours.Clear();
				indices.Clear();
				for (int i = chunk_y * chunkEdgeSize; i < chunk_y * chunkEdgeSize + chunkEdgeSize; i++)
				{
					for (int j = chunk_x * chunkEdgeSize; j < chunk_x * chunkEdgeSize + chunkEdgeSize; j++)
					{
						int num = i * Grid.WidthInCells + j;
						if (occupiedCells.ContainsKey(num))
						{
							Color cellColour = renderer.GetCellColour(num, element);
							BlockTileRenderer.Bits decorConnectionBits = renderer.GetDecorConnectionBits(j, i, this.queryLayer);
							this.AddDecor(j, i, decorConnectionBits, cellColour, vertices, uvs, this.triangles, colours);
						}
					}
				}
				if (vertices.Count > 0)
				{
					Mesh mesh = this.meshChunks[chunk_x, chunk_y];
					if (mesh == null)
					{
						mesh = new Mesh();
						mesh.name = "DecorRender";
						this.meshChunks[chunk_x, chunk_y] = mesh;
					}
					this.triangles.Sort((BlockTileRenderer.DecorRenderInfo.TriangleInfo a, BlockTileRenderer.DecorRenderInfo.TriangleInfo b) => a.sortOrder.CompareTo(b.sortOrder));
					for (int k = 0; k < this.triangles.Count; k++)
					{
						indices.Add(this.triangles[k].i0);
						indices.Add(this.triangles[k].i1);
						indices.Add(this.triangles[k].i2);
					}
					mesh.Clear();
					mesh.SetVertices(vertices);
					mesh.SetUVs(0, uvs);
					mesh.SetColors(colours);
					mesh.SetTriangles(indices, 0);
				}
				else
				{
					this.meshChunks[chunk_x, chunk_y] = null;
				}
			}

			private void AddDecor(int x, int y, BlockTileRenderer.Bits connection_bits, Color colour, List<Vector3> vertices, List<Vector2> uvs, List<BlockTileRenderer.DecorRenderInfo.TriangleInfo> triangles, List<Color> colours)
			{
				for (int i = 0; i < this.decorInfo.decor.Length; i++)
				{
					BlockTileDecorInfo.Decor decor = this.decorInfo.decor[i];
					if (decor.variants != null && decor.variants.Length != 0)
					{
						bool flag = (connection_bits & decor.requiredConnections) == decor.requiredConnections;
						bool flag2 = (connection_bits & decor.forbiddenConnections) != (BlockTileRenderer.Bits)0;
						if (flag && !flag2)
						{
							float num = PerlinSimplexNoise.noise((float)(i + x + connection_bits) * BlockTileRenderer.DecorRenderInfo.simplex_scale.x, (float)(i + y + connection_bits) * BlockTileRenderer.DecorRenderInfo.simplex_scale.y);
							if (num >= decor.probabilityCutoff)
							{
								int num2 = (int)((float)(decor.variants.Length - 1) * num);
								int count = vertices.Count;
								Vector3 vector = new Vector3((float)x, (float)y, -1f) + decor.variants[num2].offset;
								foreach (Vector3 vector2 in decor.variants[num2].atlasItem.vertices)
								{
									vertices.Add(vector2 + vector);
									colours.Add(colour);
								}
								uvs.AddRange(decor.variants[num2].atlasItem.uvs);
								int[] indices = decor.variants[num2].atlasItem.indices;
								for (int k = 0; k < indices.Length; k += 3)
								{
									triangles.Add(new BlockTileRenderer.DecorRenderInfo.TriangleInfo
									{
										sortOrder = decor.sortOrder,
										i0 = indices[k] + count,
										i1 = indices[k + 1] + count,
										i2 = indices[k + 2] + count
									});
								}
							}
						}
					}
				}
			}

			private int queryLayer;

			private BlockTileDecorInfo decorInfo;

			private Mesh[,] meshChunks;

			private Material material;

			private List<BlockTileRenderer.DecorRenderInfo.TriangleInfo> triangles = new List<BlockTileRenderer.DecorRenderInfo.TriangleInfo>();

			private static Vector2 simplex_scale = new Vector2(92.41f, 87.16f);

			private struct TriangleInfo
			{
				public int sortOrder;

				public int i0;

				public int i1;

				public int i2;
			}
		}
	}
}
