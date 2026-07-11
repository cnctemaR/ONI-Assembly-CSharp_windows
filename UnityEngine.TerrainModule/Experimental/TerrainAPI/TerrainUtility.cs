using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.TerrainAPI
{
	public static class TerrainUtility
	{
		internal static bool HasValidTerrains()
		{
			return Terrain.activeTerrains != null && Terrain.activeTerrains.Length > 0;
		}

		internal static void ClearConnectivity()
		{
			foreach (Terrain terrain in Terrain.activeTerrains)
			{
				terrain.SetNeighbors(null, null, null, null);
			}
		}

		internal static TerrainUtility.TerrainGroups CollectTerrains(bool onlyAutoConnectedTerrains = true)
		{
			TerrainUtility.TerrainGroups terrainGroups;
			if (!TerrainUtility.HasValidTerrains())
			{
				terrainGroups = null;
			}
			else
			{
				TerrainUtility.TerrainGroups terrainGroups2 = new TerrainUtility.TerrainGroups();
				Terrain[] activeTerrains = Terrain.activeTerrains;
				for (int i = 0; i < activeTerrains.Length; i++)
				{
					Terrain t = activeTerrains[i];
					if (!onlyAutoConnectedTerrains || t.allowAutoConnect)
					{
						if (!terrainGroups2.ContainsKey(t.groupingID))
						{
							TerrainUtility.TerrainMap terrainMap = TerrainUtility.TerrainMap.CreateFromPlacement(t, (Terrain x) => x.groupingID == t.groupingID && (!onlyAutoConnectedTerrains || x.allowAutoConnect), true);
							if (terrainMap != null)
							{
								terrainGroups2.Add(t.groupingID, terrainMap);
							}
						}
					}
				}
				terrainGroups = ((terrainGroups2.Count == 0) ? null : terrainGroups2);
			}
			return terrainGroups;
		}

		[RequiredByNativeCode]
		public static void AutoConnect()
		{
			if (TerrainUtility.HasValidTerrains())
			{
				TerrainUtility.ClearConnectivity();
				TerrainUtility.TerrainGroups terrainGroups = TerrainUtility.CollectTerrains(true);
				if (terrainGroups != null)
				{
					foreach (KeyValuePair<int, TerrainUtility.TerrainMap> keyValuePair in terrainGroups)
					{
						int key = keyValuePair.Key;
						TerrainUtility.TerrainMap value = keyValuePair.Value;
						foreach (KeyValuePair<TerrainUtility.TerrainMap.TileCoord, Terrain> keyValuePair2 in value.m_terrainTiles)
						{
							TerrainUtility.TerrainMap.TileCoord key2 = keyValuePair2.Key;
							Terrain terrain = value.GetTerrain(key2.tileX, key2.tileZ);
							Terrain terrain2 = value.GetTerrain(key2.tileX - 1, key2.tileZ);
							Terrain terrain3 = value.GetTerrain(key2.tileX + 1, key2.tileZ);
							Terrain terrain4 = value.GetTerrain(key2.tileX, key2.tileZ + 1);
							Terrain terrain5 = value.GetTerrain(key2.tileX, key2.tileZ - 1);
							terrain.SetNeighbors(terrain2, terrain4, terrain3, terrain5);
						}
					}
				}
			}
		}

		public class TerrainMap
		{
			public TerrainMap()
			{
				this.m_errorCode = TerrainUtility.TerrainMap.ErrorCode.OK;
				this.m_terrainTiles = new Dictionary<TerrainUtility.TerrainMap.TileCoord, Terrain>();
			}

			public Terrain GetTerrain(int tileX, int tileZ)
			{
				Terrain terrain = null;
				this.m_terrainTiles.TryGetValue(new TerrainUtility.TerrainMap.TileCoord(tileX, tileZ), out terrain);
				return terrain;
			}

			public static TerrainUtility.TerrainMap CreateFromPlacement(Terrain originTerrain, TerrainUtility.TerrainMap.TerrainFilter filter = null, bool fullValidation = true)
			{
				TerrainUtility.TerrainMap terrainMap;
				if (Terrain.activeTerrains == null || Terrain.activeTerrains.Length == 0 || originTerrain == null)
				{
					terrainMap = null;
				}
				else if (originTerrain.terrainData == null)
				{
					terrainMap = null;
				}
				else
				{
					int groupID = originTerrain.groupingID;
					float x3 = originTerrain.transform.position.x;
					float z = originTerrain.transform.position.z;
					float x2 = originTerrain.terrainData.size.x;
					float z2 = originTerrain.terrainData.size.z;
					if (filter == null)
					{
						filter = (Terrain x) => x.groupingID == groupID;
					}
					terrainMap = TerrainUtility.TerrainMap.CreateFromPlacement(new Vector2(x3, z), new Vector2(x2, z2), filter, fullValidation);
				}
				return terrainMap;
			}

			public static TerrainUtility.TerrainMap CreateFromPlacement(Vector2 gridOrigin, Vector2 gridSize, TerrainUtility.TerrainMap.TerrainFilter filter = null, bool fullValidation = true)
			{
				TerrainUtility.TerrainMap terrainMap;
				if (Terrain.activeTerrains == null || Terrain.activeTerrains.Length == 0)
				{
					terrainMap = null;
				}
				else
				{
					TerrainUtility.TerrainMap terrainMap2 = new TerrainUtility.TerrainMap();
					float num = 1f / gridSize.x;
					float num2 = 1f / gridSize.y;
					foreach (Terrain terrain in Terrain.activeTerrains)
					{
						if (!(terrain.terrainData == null))
						{
							if (filter == null || filter(terrain))
							{
								Vector3 position = terrain.transform.position;
								int num3 = Mathf.RoundToInt((position.x - gridOrigin.x) * num);
								int num4 = Mathf.RoundToInt((position.z - gridOrigin.y) * num2);
								terrainMap2.TryToAddTerrain(num3, num4, terrain);
							}
						}
					}
					if (fullValidation)
					{
						terrainMap2.Validate();
					}
					terrainMap = ((terrainMap2.m_terrainTiles.Count <= 0) ? null : terrainMap2);
				}
				return terrainMap;
			}

			private void AddTerrainInternal(int x, int z, Terrain terrain)
			{
				if (this.m_terrainTiles.Count == 0)
				{
					this.m_patchSize = terrain.terrainData.size;
				}
				else if (terrain.terrainData.size != this.m_patchSize)
				{
					this.m_errorCode |= TerrainUtility.TerrainMap.ErrorCode.SizeMismatch;
				}
				this.m_terrainTiles.Add(new TerrainUtility.TerrainMap.TileCoord(x, z), terrain);
			}

			private bool TryToAddTerrain(int tileX, int tileZ, Terrain terrain)
			{
				bool flag = false;
				if (terrain != null)
				{
					Terrain terrain2 = this.GetTerrain(tileX, tileZ);
					if (terrain2 != null)
					{
						if (terrain2 != terrain)
						{
							this.m_errorCode |= TerrainUtility.TerrainMap.ErrorCode.Overlapping;
						}
					}
					else
					{
						this.AddTerrainInternal(tileX, tileZ, terrain);
						flag = true;
					}
				}
				return flag;
			}

			private void ValidateTerrain(int tileX, int tileZ)
			{
				Terrain terrain = this.GetTerrain(tileX, tileZ);
				if (terrain != null)
				{
					Terrain terrain2 = this.GetTerrain(tileX - 1, tileZ);
					Terrain terrain3 = this.GetTerrain(tileX + 1, tileZ);
					Terrain terrain4 = this.GetTerrain(tileX, tileZ + 1);
					Terrain terrain5 = this.GetTerrain(tileX, tileZ - 1);
					if (terrain2)
					{
						if (!Mathf.Approximately(terrain.transform.position.x, terrain2.transform.position.x + terrain2.terrainData.size.x) || !Mathf.Approximately(terrain.transform.position.z, terrain2.transform.position.z))
						{
							this.m_errorCode |= TerrainUtility.TerrainMap.ErrorCode.EdgeAlignmentMismatch;
						}
					}
					if (terrain3)
					{
						if (!Mathf.Approximately(terrain.transform.position.x + terrain.terrainData.size.x, terrain3.transform.position.x) || !Mathf.Approximately(terrain.transform.position.z, terrain3.transform.position.z))
						{
							this.m_errorCode |= TerrainUtility.TerrainMap.ErrorCode.EdgeAlignmentMismatch;
						}
					}
					if (terrain4)
					{
						if (!Mathf.Approximately(terrain.transform.position.x, terrain4.transform.position.x) || !Mathf.Approximately(terrain.transform.position.z + terrain.terrainData.size.z, terrain4.transform.position.z))
						{
							this.m_errorCode |= TerrainUtility.TerrainMap.ErrorCode.EdgeAlignmentMismatch;
						}
					}
					if (terrain5)
					{
						if (!Mathf.Approximately(terrain.transform.position.x, terrain5.transform.position.x) || !Mathf.Approximately(terrain.transform.position.z, terrain5.transform.position.z + terrain5.terrainData.size.z))
						{
							this.m_errorCode |= TerrainUtility.TerrainMap.ErrorCode.EdgeAlignmentMismatch;
						}
					}
				}
			}

			private TerrainUtility.TerrainMap.ErrorCode Validate()
			{
				foreach (TerrainUtility.TerrainMap.TileCoord tileCoord in this.m_terrainTiles.Keys)
				{
					this.ValidateTerrain(tileCoord.tileX, tileCoord.tileZ);
				}
				return this.m_errorCode;
			}

			private Vector3 m_patchSize;

			public TerrainUtility.TerrainMap.ErrorCode m_errorCode;

			public Dictionary<TerrainUtility.TerrainMap.TileCoord, Terrain> m_terrainTiles;

			public delegate bool TerrainFilter(Terrain terrain);

			public struct TileCoord
			{
				public TileCoord(int tileX, int tileZ)
				{
					this.tileX = tileX;
					this.tileZ = tileZ;
				}

				public readonly int tileX;

				public readonly int tileZ;
			}

			public enum ErrorCode
			{
				OK,
				Overlapping,
				SizeMismatch = 4,
				EdgeAlignmentMismatch = 8
			}
		}

		public class TerrainGroups : Dictionary<int, TerrainUtility.TerrainMap>
		{
		}
	}
}
