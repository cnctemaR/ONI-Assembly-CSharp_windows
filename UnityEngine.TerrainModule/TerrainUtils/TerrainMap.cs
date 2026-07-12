using System;
using System.Collections.Generic;

namespace UnityEngine.TerrainUtils
{
	public class TerrainMap
	{
		public Terrain GetTerrain(int tileX, int tileZ)
		{
			Terrain terrain = null;
			this.m_terrainTiles.TryGetValue(new TerrainTileCoord(tileX, tileZ), out terrain);
			return terrain;
		}

		public static TerrainMap CreateFromConnectedNeighbors(Terrain originTerrain, Predicate<Terrain> filter = null, bool fullValidation = true)
		{
			bool flag = originTerrain == null;
			TerrainMap terrainMap;
			if (flag)
			{
				terrainMap = null;
			}
			else
			{
				bool flag2 = originTerrain.terrainData == null;
				if (flag2)
				{
					terrainMap = null;
				}
				else
				{
					TerrainMap terrainMap2 = new TerrainMap();
					Queue<TerrainMap.QueueElement> queue = new Queue<TerrainMap.QueueElement>();
					queue.Enqueue(new TerrainMap.QueueElement(0, 0, originTerrain));
					int num = Terrain.activeTerrains.Length;
					while (queue.Count > 0)
					{
						TerrainMap.QueueElement queueElement = queue.Dequeue();
						bool flag3 = filter == null || filter(queueElement.terrain);
						if (flag3)
						{
							bool flag4 = terrainMap2.TryToAddTerrain(queueElement.tileX, queueElement.tileZ, queueElement.terrain);
							if (flag4)
							{
								bool flag5 = terrainMap2.m_terrainTiles.Count > num;
								if (flag5)
								{
									break;
								}
								bool flag6 = queueElement.terrain.leftNeighbor != null;
								if (flag6)
								{
									queue.Enqueue(new TerrainMap.QueueElement(queueElement.tileX - 1, queueElement.tileZ, queueElement.terrain.leftNeighbor));
								}
								bool flag7 = queueElement.terrain.bottomNeighbor != null;
								if (flag7)
								{
									queue.Enqueue(new TerrainMap.QueueElement(queueElement.tileX, queueElement.tileZ - 1, queueElement.terrain.bottomNeighbor));
								}
								bool flag8 = queueElement.terrain.rightNeighbor != null;
								if (flag8)
								{
									queue.Enqueue(new TerrainMap.QueueElement(queueElement.tileX + 1, queueElement.tileZ, queueElement.terrain.rightNeighbor));
								}
								bool flag9 = queueElement.terrain.topNeighbor != null;
								if (flag9)
								{
									queue.Enqueue(new TerrainMap.QueueElement(queueElement.tileX, queueElement.tileZ + 1, queueElement.terrain.topNeighbor));
								}
							}
						}
					}
					if (fullValidation)
					{
						terrainMap2.Validate();
					}
					terrainMap = terrainMap2;
				}
			}
			return terrainMap;
		}

		public static TerrainMap CreateFromPlacement(Terrain originTerrain, Predicate<Terrain> filter = null, bool fullValidation = true)
		{
			bool flag = Terrain.activeTerrains == null || Terrain.activeTerrains.Length == 0 || originTerrain == null;
			TerrainMap terrainMap;
			if (flag)
			{
				terrainMap = null;
			}
			else
			{
				bool flag2 = originTerrain.terrainData == null;
				if (flag2)
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
					bool flag3 = filter == null;
					if (flag3)
					{
						filter = (Terrain x) => x.groupingID == groupID;
					}
					terrainMap = TerrainMap.CreateFromPlacement(new Vector2(x3, z), new Vector2(x2, z2), filter, fullValidation);
				}
			}
			return terrainMap;
		}

		public static TerrainMap CreateFromPlacement(Vector2 gridOrigin, Vector2 gridSize, Predicate<Terrain> filter = null, bool fullValidation = true)
		{
			bool flag = Terrain.activeTerrains == null || Terrain.activeTerrains.Length == 0;
			TerrainMap terrainMap;
			if (flag)
			{
				terrainMap = null;
			}
			else
			{
				TerrainMap terrainMap2 = new TerrainMap();
				float num = 1f / gridSize.x;
				float num2 = 1f / gridSize.y;
				foreach (Terrain terrain in Terrain.activeTerrains)
				{
					bool flag2 = terrain.terrainData == null;
					if (!flag2)
					{
						bool flag3 = filter == null || filter(terrain);
						if (flag3)
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
				terrainMap = ((terrainMap2.m_terrainTiles.Count > 0) ? terrainMap2 : null);
			}
			return terrainMap;
		}

		public Dictionary<TerrainTileCoord, Terrain> terrainTiles
		{
			get
			{
				return this.m_terrainTiles;
			}
		}

		public TerrainMap()
		{
			this.m_errorCode = TerrainMapStatusCode.OK;
			this.m_terrainTiles = new Dictionary<TerrainTileCoord, Terrain>();
		}

		private void AddTerrainInternal(int x, int z, Terrain terrain)
		{
			bool flag = this.m_terrainTiles.Count == 0;
			if (flag)
			{
				this.m_patchSize = terrain.terrainData.size;
			}
			else
			{
				bool flag2 = terrain.terrainData.size != this.m_patchSize;
				if (flag2)
				{
					this.m_errorCode |= TerrainMapStatusCode.SizeMismatch;
				}
			}
			this.m_terrainTiles.Add(new TerrainTileCoord(x, z), terrain);
		}

		private bool TryToAddTerrain(int tileX, int tileZ, Terrain terrain)
		{
			bool flag = false;
			bool flag2 = terrain != null;
			if (flag2)
			{
				Terrain terrain2 = this.GetTerrain(tileX, tileZ);
				bool flag3 = terrain2 != null;
				if (flag3)
				{
					bool flag4 = terrain2 != terrain;
					if (flag4)
					{
						this.m_errorCode |= TerrainMapStatusCode.Overlapping;
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
			bool flag = terrain != null;
			if (flag)
			{
				Terrain terrain2 = this.GetTerrain(tileX - 1, tileZ);
				Terrain terrain3 = this.GetTerrain(tileX + 1, tileZ);
				Terrain terrain4 = this.GetTerrain(tileX, tileZ + 1);
				Terrain terrain5 = this.GetTerrain(tileX, tileZ - 1);
				bool flag2 = terrain2;
				if (flag2)
				{
					bool flag3 = !Mathf.Approximately(terrain.transform.position.x, terrain2.transform.position.x + terrain2.terrainData.size.x) || !Mathf.Approximately(terrain.transform.position.z, terrain2.transform.position.z);
					if (flag3)
					{
						this.m_errorCode |= TerrainMapStatusCode.EdgeAlignmentMismatch;
					}
				}
				bool flag4 = terrain3;
				if (flag4)
				{
					bool flag5 = !Mathf.Approximately(terrain.transform.position.x + terrain.terrainData.size.x, terrain3.transform.position.x) || !Mathf.Approximately(terrain.transform.position.z, terrain3.transform.position.z);
					if (flag5)
					{
						this.m_errorCode |= TerrainMapStatusCode.EdgeAlignmentMismatch;
					}
				}
				bool flag6 = terrain4;
				if (flag6)
				{
					bool flag7 = !Mathf.Approximately(terrain.transform.position.x, terrain4.transform.position.x) || !Mathf.Approximately(terrain.transform.position.z + terrain.terrainData.size.z, terrain4.transform.position.z);
					if (flag7)
					{
						this.m_errorCode |= TerrainMapStatusCode.EdgeAlignmentMismatch;
					}
				}
				bool flag8 = terrain5;
				if (flag8)
				{
					bool flag9 = !Mathf.Approximately(terrain.transform.position.x, terrain5.transform.position.x) || !Mathf.Approximately(terrain.transform.position.z, terrain5.transform.position.z + terrain5.terrainData.size.z);
					if (flag9)
					{
						this.m_errorCode |= TerrainMapStatusCode.EdgeAlignmentMismatch;
					}
				}
			}
		}

		private TerrainMapStatusCode Validate()
		{
			foreach (TerrainTileCoord terrainTileCoord in this.m_terrainTiles.Keys)
			{
				this.ValidateTerrain(terrainTileCoord.tileX, terrainTileCoord.tileZ);
			}
			return this.m_errorCode;
		}

		private Vector3 m_patchSize;

		private TerrainMapStatusCode m_errorCode;

		private Dictionary<TerrainTileCoord, Terrain> m_terrainTiles;

		private struct QueueElement
		{
			public QueueElement(int tileX, int tileZ, Terrain terrain)
			{
				this.tileX = tileX;
				this.tileZ = tileZ;
				this.terrain = terrain;
			}

			public readonly int tileX;

			public readonly int tileZ;

			public readonly Terrain terrain;
		}
	}
}
