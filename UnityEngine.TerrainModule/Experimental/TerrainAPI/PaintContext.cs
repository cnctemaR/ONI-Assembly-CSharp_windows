using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.TerrainAPI
{
	public class PaintContext
	{
		public PaintContext(Terrain terrain, RectInt pixelRect, int targetTextureWidth, int targetTextureHeight)
		{
			this.originTerrain = terrain;
			this.pixelRect = pixelRect;
			this.targetTextureWidth = targetTextureWidth;
			this.targetTextureHeight = targetTextureHeight;
			TerrainData terrainData = terrain.terrainData;
			this.pixelSize = new Vector2(terrainData.size.x / ((float)targetTextureWidth - 1f), terrainData.size.z / ((float)targetTextureHeight - 1f));
			this.FindTerrainTiles();
			this.ClipTerrainTiles();
		}

		public Terrain originTerrain { get; }

		public RectInt pixelRect { get; }

		public int targetTextureWidth { get; }

		public int targetTextureHeight { get; }

		public Vector2 pixelSize { get; }

		public RenderTexture sourceRenderTexture
		{
			get
			{
				return this.m_SourceRenderTexture;
			}
		}

		public RenderTexture destinationRenderTexture
		{
			get
			{
				return this.m_DestinationRenderTexture;
			}
		}

		public RenderTexture oldRenderTexture
		{
			get
			{
				return this.m_OldRenderTexture;
			}
		}

		public int terrainCount
		{
			get
			{
				return this.m_TerrainTiles.Count;
			}
		}

		public Terrain GetTerrain(int terrainIndex)
		{
			return this.m_TerrainTiles[terrainIndex].terrain;
		}

		public RectInt GetClippedPixelRectInTerrainPixels(int terrainIndex)
		{
			return this.m_TerrainTiles[terrainIndex].clippedLocalPixels;
		}

		public RectInt GetClippedPixelRectInRenderTexturePixels(int terrainIndex)
		{
			return this.m_TerrainTiles[terrainIndex].clippedPCPixels;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static event Action<PaintContext.TerrainTile, PaintContext.ToolAction, string> onTerrainTileBeforePaint;

		public static PaintContext CreateFromBounds(Terrain terrain, Rect boundsInTerrainSpace, int inputTextureWidth, int inputTextureHeight, int extraBorderPixels = 0)
		{
			return new PaintContext(terrain, TerrainPaintUtility.CalcPixelRectFromBounds(terrain, boundsInTerrainSpace, inputTextureWidth, inputTextureHeight, extraBorderPixels), inputTextureWidth, inputTextureHeight);
		}

		internal void FindTerrainTiles()
		{
			this.m_TerrainTiles = new List<PaintContext.TerrainTile>();
			Terrain leftNeighbor = this.originTerrain.leftNeighbor;
			Terrain rightNeighbor = this.originTerrain.rightNeighbor;
			Terrain topNeighbor = this.originTerrain.topNeighbor;
			Terrain bottomNeighbor = this.originTerrain.bottomNeighbor;
			bool flag = this.pixelRect.x < 0;
			bool flag2 = this.pixelRect.xMax > this.targetTextureWidth - 1;
			bool flag3 = this.pixelRect.yMax > this.targetTextureHeight - 1;
			bool flag4 = this.pixelRect.y < 0;
			if (flag && flag2)
			{
				Debug.LogWarning("PaintContext pixelRect is too large!  It should touch a maximum of 2 Terrains horizontally.");
				flag2 = false;
			}
			if (flag3 && flag4)
			{
				Debug.LogWarning("PaintContext pixelRect is too large!  It should touch a maximum of 2 Terrains vertically.");
				flag4 = false;
			}
			PaintContext.TerrainTile terrainTile = new PaintContext.TerrainTile(this.originTerrain, 0, 0);
			this.m_TerrainTiles.Add(terrainTile);
			Terrain terrain = null;
			Terrain terrain2 = null;
			Terrain terrain3 = null;
			int num = 0;
			int num2 = 0;
			if (flag)
			{
				num = -1;
				terrain = leftNeighbor;
			}
			else if (flag2)
			{
				num = 1;
				terrain = rightNeighbor;
			}
			if (flag3)
			{
				num2 = 1;
				terrain2 = topNeighbor;
			}
			else if (flag4)
			{
				num2 = -1;
				terrain2 = bottomNeighbor;
			}
			if (terrain)
			{
				terrainTile = new PaintContext.TerrainTile(terrain, num * (this.targetTextureWidth - 1), 0);
				this.m_TerrainTiles.Add(terrainTile);
				if (flag3 && terrain.topNeighbor)
				{
					terrain3 = terrain.topNeighbor;
				}
				else if (flag4 && terrain.bottomNeighbor)
				{
					terrain3 = terrain.bottomNeighbor;
				}
			}
			if (terrain2)
			{
				terrainTile = new PaintContext.TerrainTile(terrain2, 0, num2 * (this.targetTextureHeight - 1));
				this.m_TerrainTiles.Add(terrainTile);
				if (flag && terrain2.leftNeighbor)
				{
					terrain3 = terrain2.leftNeighbor;
				}
				else if (flag2 && terrain2.rightNeighbor)
				{
					terrain3 = terrain2.rightNeighbor;
				}
			}
			if (terrain3 != null)
			{
				terrainTile = new PaintContext.TerrainTile(terrain3, num * (this.targetTextureWidth - 1), num2 * (this.targetTextureHeight - 1));
				this.m_TerrainTiles.Add(terrainTile);
			}
		}

		internal void ClipTerrainTiles()
		{
			for (int i = 0; i < this.m_TerrainTiles.Count; i++)
			{
				PaintContext.TerrainTile terrainTile = this.m_TerrainTiles[i];
				terrainTile.clippedLocalPixels = default(RectInt);
				terrainTile.clippedLocalPixels.x = Mathf.Max(0, this.pixelRect.x - terrainTile.tileOriginPixels.x);
				terrainTile.clippedLocalPixels.y = Mathf.Max(0, this.pixelRect.y - terrainTile.tileOriginPixels.y);
				terrainTile.clippedLocalPixels.xMax = Mathf.Min(this.targetTextureWidth, this.pixelRect.xMax - terrainTile.tileOriginPixels.x);
				terrainTile.clippedLocalPixels.yMax = Mathf.Min(this.targetTextureHeight, this.pixelRect.yMax - terrainTile.tileOriginPixels.y);
				terrainTile.clippedPCPixels = new RectInt(terrainTile.clippedLocalPixels.x + terrainTile.tileOriginPixels.x - this.pixelRect.x, terrainTile.clippedLocalPixels.y + terrainTile.tileOriginPixels.y - this.pixelRect.y, terrainTile.clippedLocalPixels.width, terrainTile.clippedLocalPixels.height);
			}
		}

		public void CreateRenderTargets(RenderTextureFormat colorFormat)
		{
			this.m_SourceRenderTexture = RenderTexture.GetTemporary(this.pixelRect.width, this.pixelRect.height, 0, colorFormat, RenderTextureReadWrite.Linear);
			this.m_DestinationRenderTexture = RenderTexture.GetTemporary(this.pixelRect.width, this.pixelRect.height, 0, colorFormat, RenderTextureReadWrite.Linear);
			this.m_SourceRenderTexture.wrapMode = TextureWrapMode.Clamp;
			this.m_SourceRenderTexture.filterMode = FilterMode.Point;
			this.m_OldRenderTexture = RenderTexture.active;
		}

		public void Cleanup(bool restoreRenderTexture = true)
		{
			if (restoreRenderTexture)
			{
				RenderTexture.active = this.m_OldRenderTexture;
			}
			RenderTexture.ReleaseTemporary(this.m_SourceRenderTexture);
			RenderTexture.ReleaseTemporary(this.m_DestinationRenderTexture);
			this.m_SourceRenderTexture = null;
			this.m_DestinationRenderTexture = null;
			this.m_OldRenderTexture = null;
		}

		public void GatherHeightmap()
		{
			Material blitMaterial = TerrainPaintUtility.GetBlitMaterial();
			RenderTexture.active = this.sourceRenderTexture;
			GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
			GL.PushMatrix();
			GL.LoadPixelMatrix(0f, (float)this.pixelRect.width, 0f, (float)this.pixelRect.height);
			for (int i = 0; i < this.m_TerrainTiles.Count; i++)
			{
				PaintContext.TerrainTile terrainTile = this.m_TerrainTiles[i];
				if (terrainTile.clippedLocalPixels.width != 0 && terrainTile.clippedLocalPixels.height != 0)
				{
					Texture heightmapTexture = terrainTile.terrain.terrainData.heightmapTexture;
					if (heightmapTexture.width != this.targetTextureWidth || heightmapTexture.height != this.targetTextureHeight)
					{
						Debug.LogWarning("PaintContext heightmap operations must use the same resolution for all Terrains - mismatched Terrains are ignored.", terrainTile.terrain);
					}
					else
					{
						FilterMode filterMode = heightmapTexture.filterMode;
						heightmapTexture.filterMode = FilterMode.Point;
						blitMaterial.SetTexture("_MainTex", heightmapTexture);
						blitMaterial.SetPass(0);
						TerrainPaintUtility.DrawQuad(terrainTile.clippedPCPixels, terrainTile.clippedLocalPixels, heightmapTexture);
						heightmapTexture.filterMode = filterMode;
					}
				}
			}
			GL.PopMatrix();
			RenderTexture.active = this.oldRenderTexture;
		}

		public void ScatterHeightmap(string editorUndoName)
		{
			Material blitMaterial = TerrainPaintUtility.GetBlitMaterial();
			for (int i = 0; i < this.m_TerrainTiles.Count; i++)
			{
				PaintContext.TerrainTile terrainTile = this.m_TerrainTiles[i];
				if (terrainTile.clippedLocalPixels.width != 0 && terrainTile.clippedLocalPixels.height != 0)
				{
					RenderTexture heightmapTexture = terrainTile.terrain.terrainData.heightmapTexture;
					if (heightmapTexture.width != this.targetTextureWidth || heightmapTexture.height != this.targetTextureHeight)
					{
						Debug.LogWarning("PaintContext heightmap operations must use the same resolution for all Terrains - mismatched Terrains are ignored.", terrainTile.terrain);
					}
					else
					{
						if (PaintContext.onTerrainTileBeforePaint != null)
						{
							PaintContext.onTerrainTileBeforePaint(terrainTile, PaintContext.ToolAction.PaintHeightmap, editorUndoName);
						}
						RenderTexture.active = heightmapTexture;
						GL.PushMatrix();
						GL.LoadPixelMatrix(0f, (float)heightmapTexture.width, 0f, (float)heightmapTexture.height);
						this.destinationRenderTexture.filterMode = FilterMode.Point;
						blitMaterial.SetTexture("_MainTex", this.destinationRenderTexture);
						blitMaterial.SetPass(0);
						TerrainPaintUtility.DrawQuad(terrainTile.clippedLocalPixels, terrainTile.clippedPCPixels, this.destinationRenderTexture);
						GL.PopMatrix();
						terrainTile.terrain.terrainData.UpdateDirtyRegion(terrainTile.clippedLocalPixels.x, terrainTile.clippedLocalPixels.y, terrainTile.clippedLocalPixels.width, terrainTile.clippedLocalPixels.height, !terrainTile.terrain.drawInstanced);
						PaintContext.OnTerrainPainted(terrainTile, PaintContext.ToolAction.PaintHeightmap);
					}
				}
			}
		}

		public void GatherNormals()
		{
			RenderTexture normalmapTexture = this.originTerrain.normalmapTexture;
			Material blitMaterial = TerrainPaintUtility.GetBlitMaterial();
			RenderTexture.active = this.sourceRenderTexture;
			GL.Clear(false, true, new Color(0.5f, 0.5f, 0.5f, 0.5f));
			GL.PushMatrix();
			GL.LoadPixelMatrix(0f, (float)this.pixelRect.width, 0f, (float)this.pixelRect.height);
			for (int i = 0; i < this.m_TerrainTiles.Count; i++)
			{
				PaintContext.TerrainTile terrainTile = this.m_TerrainTiles[i];
				if (terrainTile.clippedLocalPixels.width != 0 && terrainTile.clippedLocalPixels.height != 0)
				{
					Texture normalmapTexture2 = terrainTile.terrain.normalmapTexture;
					if (normalmapTexture2.width != this.targetTextureWidth || normalmapTexture2.height != this.targetTextureHeight)
					{
						Debug.LogWarning("PaintContext normalmap operations must use the same resolution for all Terrains - mismatched Terrains are ignored.", terrainTile.terrain);
					}
					else
					{
						FilterMode filterMode = normalmapTexture2.filterMode;
						normalmapTexture2.filterMode = FilterMode.Point;
						blitMaterial.SetTexture("_MainTex", normalmapTexture2);
						blitMaterial.SetPass(0);
						TerrainPaintUtility.DrawQuad(terrainTile.clippedPCPixels, terrainTile.clippedLocalPixels, normalmapTexture2);
						normalmapTexture2.filterMode = filterMode;
					}
				}
			}
			GL.PopMatrix();
			RenderTexture.active = this.oldRenderTexture;
		}

		public void GatherAlphamap(TerrainLayer inputLayer, bool addLayerIfDoesntExist = true)
		{
			if (!(inputLayer == null))
			{
				int num = TerrainPaintUtility.FindTerrainLayerIndex(this.originTerrain, inputLayer);
				if (num == -1 && addLayerIfDoesntExist)
				{
					num = TerrainPaintUtility.AddTerrainLayer(this.originTerrain, inputLayer);
				}
				RenderTexture.active = this.sourceRenderTexture;
				GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
				GL.PushMatrix();
				GL.LoadPixelMatrix(0f, (float)this.pixelRect.width, 0f, (float)this.pixelRect.height);
				Vector4[] array = new Vector4[]
				{
					new Vector4(1f, 0f, 0f, 0f),
					new Vector4(0f, 1f, 0f, 0f),
					new Vector4(0f, 0f, 1f, 0f),
					new Vector4(0f, 0f, 0f, 1f)
				};
				Material copyTerrainLayerMaterial = TerrainPaintUtility.GetCopyTerrainLayerMaterial();
				for (int i = 0; i < this.m_TerrainTiles.Count; i++)
				{
					PaintContext.TerrainTile terrainTile = this.m_TerrainTiles[i];
					if (terrainTile.clippedLocalPixels.width != 0 && terrainTile.clippedLocalPixels.height != 0)
					{
						int num2 = TerrainPaintUtility.FindTerrainLayerIndex(terrainTile.terrain, inputLayer);
						if (num2 == -1)
						{
							if (!addLayerIfDoesntExist)
							{
								terrainTile.clippedLocalPixels.width = 0;
								terrainTile.clippedLocalPixels.height = 0;
								terrainTile.clippedPCPixels.width = 0;
								terrainTile.clippedPCPixels.height = 0;
								goto IL_0313;
							}
							num2 = TerrainPaintUtility.AddTerrainLayer(terrainTile.terrain, inputLayer);
						}
						terrainTile.mapIndex = num2 >> 2;
						terrainTile.channelIndex = num2 & 3;
						Texture terrainAlphaMapChecked = TerrainPaintUtility.GetTerrainAlphaMapChecked(terrainTile.terrain, terrainTile.mapIndex);
						if (terrainAlphaMapChecked.width != this.targetTextureWidth || terrainAlphaMapChecked.height != this.targetTextureHeight)
						{
							Debug.LogWarning(string.Concat(new object[] { "PaintContext alphamap operations must use the same resolution for all Terrains - mismatched Terrains are ignored. (", terrainAlphaMapChecked.width, " x ", terrainAlphaMapChecked.height, ") != (", this.targetTextureWidth, " x ", this.targetTextureHeight, ")" }), terrainTile.terrain);
						}
						else
						{
							FilterMode filterMode = terrainAlphaMapChecked.filterMode;
							terrainAlphaMapChecked.filterMode = FilterMode.Point;
							copyTerrainLayerMaterial.SetVector("_LayerMask", array[terrainTile.channelIndex]);
							copyTerrainLayerMaterial.SetTexture("_MainTex", terrainAlphaMapChecked);
							copyTerrainLayerMaterial.SetPass(0);
							TerrainPaintUtility.DrawQuad(terrainTile.clippedPCPixels, terrainTile.clippedLocalPixels, terrainAlphaMapChecked);
							terrainAlphaMapChecked.filterMode = filterMode;
						}
					}
					IL_0313:;
				}
				GL.PopMatrix();
				RenderTexture.active = this.oldRenderTexture;
			}
		}

		public void ScatterAlphamap(string editorUndoName)
		{
			Vector4[] array = new Vector4[]
			{
				new Vector4(1f, 0f, 0f, 0f),
				new Vector4(0f, 1f, 0f, 0f),
				new Vector4(0f, 0f, 1f, 0f),
				new Vector4(0f, 0f, 0f, 1f)
			};
			Material copyTerrainLayerMaterial = TerrainPaintUtility.GetCopyTerrainLayerMaterial();
			for (int i = 0; i < this.m_TerrainTiles.Count; i++)
			{
				PaintContext.TerrainTile terrainTile = this.m_TerrainTiles[i];
				if (terrainTile.clippedLocalPixels.width != 0 && terrainTile.clippedLocalPixels.height != 0)
				{
					if (PaintContext.onTerrainTileBeforePaint != null)
					{
						PaintContext.onTerrainTileBeforePaint(terrainTile, PaintContext.ToolAction.PaintTexture, editorUndoName);
					}
					RenderTexture temporary = RenderTexture.GetTemporary(new RenderTextureDescriptor(this.destinationRenderTexture.width, this.destinationRenderTexture.height, RenderTextureFormat.ARGB32)
					{
						sRGB = false,
						useMipMap = false,
						autoGenerateMips = false
					});
					RenderTexture.active = temporary;
					RectInt clippedPCPixels = terrainTile.clippedPCPixels;
					Rect rect = new Rect((float)clippedPCPixels.x / (float)this.pixelRect.width, (float)clippedPCPixels.y / (float)this.pixelRect.height, (float)clippedPCPixels.width / (float)this.pixelRect.width, (float)clippedPCPixels.height / (float)this.pixelRect.height);
					this.destinationRenderTexture.filterMode = FilterMode.Point;
					int mapIndex = terrainTile.mapIndex;
					int channelIndex = terrainTile.channelIndex;
					Texture2D texture2D = terrainTile.terrain.terrainData.alphamapTextures[mapIndex];
					for (int j = 0; j < terrainTile.terrain.terrainData.alphamapTextureCount; j++)
					{
						Texture2D texture2D2 = terrainTile.terrain.terrainData.alphamapTextures[j];
						if (texture2D2.width != this.targetTextureWidth || texture2D2.height != this.targetTextureHeight)
						{
							Debug.LogWarning("PaintContext alphamap operations must use the same resolution for all Terrains - mismatched Terrains are ignored.", terrainTile.terrain);
						}
						else
						{
							Rect rect2 = new Rect((float)terrainTile.clippedLocalPixels.x / (float)texture2D2.width, (float)terrainTile.clippedLocalPixels.y / (float)texture2D2.height, (float)terrainTile.clippedLocalPixels.width / (float)texture2D2.width, (float)terrainTile.clippedLocalPixels.height / (float)texture2D2.height);
							copyTerrainLayerMaterial.SetTexture("_MainTex", this.destinationRenderTexture);
							copyTerrainLayerMaterial.SetTexture("_OldAlphaMapTexture", this.sourceRenderTexture);
							copyTerrainLayerMaterial.SetTexture("_OriginalTargetAlphaMap", texture2D);
							copyTerrainLayerMaterial.SetTexture("_AlphaMapTexture", texture2D2);
							copyTerrainLayerMaterial.SetVector("_LayerMask", (j != mapIndex) ? Vector4.zero : array[channelIndex]);
							copyTerrainLayerMaterial.SetVector("_OriginalTargetAlphaMask", array[channelIndex]);
							copyTerrainLayerMaterial.SetPass(1);
							GL.PushMatrix();
							GL.LoadPixelMatrix(0f, (float)temporary.width, 0f, (float)temporary.height);
							GL.Begin(7);
							GL.Color(new Color(1f, 1f, 1f, 1f));
							GL.MultiTexCoord2(0, rect.x, rect.y);
							GL.MultiTexCoord2(1, rect2.x, rect2.y);
							GL.Vertex3((float)clippedPCPixels.x, (float)clippedPCPixels.y, 0f);
							GL.MultiTexCoord2(0, rect.x, rect.yMax);
							GL.MultiTexCoord2(1, rect2.x, rect2.yMax);
							GL.Vertex3((float)clippedPCPixels.x, (float)clippedPCPixels.yMax, 0f);
							GL.MultiTexCoord2(0, rect.xMax, rect.yMax);
							GL.MultiTexCoord2(1, rect2.xMax, rect2.yMax);
							GL.Vertex3((float)clippedPCPixels.xMax, (float)clippedPCPixels.yMax, 0f);
							GL.MultiTexCoord2(0, rect.xMax, rect.y);
							GL.MultiTexCoord2(1, rect2.xMax, rect2.y);
							GL.Vertex3((float)clippedPCPixels.xMax, (float)clippedPCPixels.y, 0f);
							GL.End();
							GL.PopMatrix();
							if (TerrainPaintUtility.paintTextureUsesCopyTexture)
							{
								RenderTexture temporary2 = RenderTexture.GetTemporary(new RenderTextureDescriptor(texture2D2.width, texture2D2.height, RenderTextureFormat.ARGB32)
								{
									sRGB = false,
									useMipMap = true,
									autoGenerateMips = false
								});
								if (!temporary2.IsCreated())
								{
									temporary2.Create();
								}
								Graphics.CopyTexture(texture2D2, 0, 0, temporary2, 0, 0);
								Graphics.CopyTexture(temporary, 0, 0, clippedPCPixels.x, clippedPCPixels.y, clippedPCPixels.width, clippedPCPixels.height, temporary2, 0, 0, terrainTile.clippedLocalPixels.x, terrainTile.clippedLocalPixels.y);
								temporary2.GenerateMips();
								Graphics.CopyTexture(temporary2, texture2D2);
								RenderTexture.ReleaseTemporary(temporary2);
							}
							else
							{
								GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
								if (graphicsDeviceType == GraphicsDeviceType.Metal || graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
								{
									texture2D2.ReadPixels(new Rect((float)clippedPCPixels.x, (float)clippedPCPixels.y, (float)clippedPCPixels.width, (float)clippedPCPixels.height), terrainTile.clippedLocalPixels.x, terrainTile.clippedLocalPixels.y);
								}
								else
								{
									texture2D2.ReadPixels(new Rect((float)clippedPCPixels.x, (float)(temporary.height - clippedPCPixels.y - clippedPCPixels.height), (float)clippedPCPixels.width, (float)clippedPCPixels.height), terrainTile.clippedLocalPixels.x, terrainTile.clippedLocalPixels.y);
								}
								texture2D2.Apply();
							}
						}
					}
					RenderTexture.active = null;
					RenderTexture.ReleaseTemporary(temporary);
					PaintContext.OnTerrainPainted(terrainTile, PaintContext.ToolAction.PaintTexture);
				}
			}
		}

		private static void OnTerrainPainted(PaintContext.TerrainTile tile, PaintContext.ToolAction action)
		{
			for (int i = 0; i < PaintContext.s_PaintedTerrain.Count; i++)
			{
				if (tile.terrain == PaintContext.s_PaintedTerrain[i].terrain)
				{
					PaintContext.s_PaintedTerrain[i].action |= action;
					return;
				}
			}
			PaintContext.s_PaintedTerrain.Add(new PaintContext.PaintedTerrain
			{
				terrain = tile.terrain,
				action = action
			});
		}

		public static void ApplyDelayedActions()
		{
			int i = 0;
			while (i < PaintContext.s_PaintedTerrain.Count)
			{
				PaintContext.PaintedTerrain paintedTerrain = PaintContext.s_PaintedTerrain[i];
				if ((paintedTerrain.action & PaintContext.ToolAction.PaintHeightmap) != PaintContext.ToolAction.None)
				{
					paintedTerrain.terrain.ApplyDelayedHeightmapModification();
				}
				if ((paintedTerrain.action & PaintContext.ToolAction.PaintTexture) != PaintContext.ToolAction.None)
				{
					TerrainData terrainData = paintedTerrain.terrain.terrainData;
					if (!(terrainData == null))
					{
						terrainData.SetBaseMapDirty();
						if (TerrainPaintUtility.paintTextureUsesCopyTexture)
						{
							RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(terrainData.alphamapResolution, terrainData.alphamapResolution, RenderTextureFormat.ARGB32)
							{
								sRGB = false,
								useMipMap = false,
								autoGenerateMips = false
							};
							RenderTexture temporary = RenderTexture.GetTemporary(renderTextureDescriptor);
							for (int j = 0; j < terrainData.alphamapTextureCount; j++)
							{
								Graphics.Blit(terrainData.alphamapTextures[j], temporary);
								terrainData.alphamapTextures[j].ReadPixels(new Rect(0f, 0f, (float)renderTextureDescriptor.width, (float)renderTextureDescriptor.height), 0, 0, true);
							}
							RenderTexture.ReleaseTemporary(temporary);
						}
					}
				}
				IL_0106:
				i++;
				continue;
				goto IL_0106;
			}
			PaintContext.s_PaintedTerrain.Clear();
		}

		private List<PaintContext.TerrainTile> m_TerrainTiles;

		private RenderTexture m_SourceRenderTexture;

		private RenderTexture m_DestinationRenderTexture;

		private RenderTexture m_OldRenderTexture;

		private static List<PaintContext.PaintedTerrain> s_PaintedTerrain = new List<PaintContext.PaintedTerrain>();

		internal class TerrainTile
		{
			public TerrainTile()
			{
			}

			public TerrainTile(Terrain newTerrain, int tileOriginPixelsX, int tileOriginPixelsY)
			{
				this.terrain = newTerrain;
				this.tileOriginPixels = new Vector2Int(tileOriginPixelsX, tileOriginPixelsY);
			}

			public Terrain terrain;

			public Vector2Int tileOriginPixels;

			public RectInt clippedLocalPixels;

			public RectInt clippedPCPixels;

			public int mapIndex;

			public int channelIndex;
		}

		[Flags]
		internal enum ToolAction
		{
			None = 0,
			PaintHeightmap = 1,
			PaintTexture = 2
		}

		private class PaintedTerrain
		{
			public Terrain terrain;

			public PaintContext.ToolAction action;
		}
	}
}
