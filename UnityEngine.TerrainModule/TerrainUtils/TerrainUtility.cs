using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.TerrainUtils
{
	[MovedFrom("UnityEngine.Experimental.TerrainAPI")]
	public static class TerrainUtility
	{
		internal static bool ValidTerrainsExist()
		{
			return Terrain.activeTerrains != null && Terrain.activeTerrains.Length != 0;
		}

		internal static void ClearConnectivity()
		{
			foreach (Terrain terrain in Terrain.activeTerrains)
			{
				bool allowAutoConnect = terrain.allowAutoConnect;
				if (allowAutoConnect)
				{
					terrain.SetNeighbors(null, null, null, null);
				}
			}
		}

		internal static Dictionary<int, TerrainMap> CollectTerrains(bool onlyAutoConnectedTerrains = true)
		{
			bool flag = !TerrainUtility.ValidTerrainsExist();
			Dictionary<int, TerrainMap> dictionary;
			if (flag)
			{
				dictionary = null;
			}
			else
			{
				Dictionary<int, TerrainMap> dictionary2 = new Dictionary<int, TerrainMap>();
				Terrain[] activeTerrains = Terrain.activeTerrains;
				for (int i = 0; i < activeTerrains.Length; i++)
				{
					Terrain t = activeTerrains[i];
					bool flag2 = onlyAutoConnectedTerrains && !t.allowAutoConnect;
					if (!flag2)
					{
						bool flag3 = !dictionary2.ContainsKey(t.groupingID);
						if (flag3)
						{
							TerrainMap terrainMap = TerrainMap.CreateFromPlacement(t, (Terrain x) => x.groupingID == t.groupingID && (!onlyAutoConnectedTerrains || x.allowAutoConnect), true);
							bool flag4 = terrainMap != null;
							if (flag4)
							{
								dictionary2.Add(t.groupingID, terrainMap);
							}
						}
					}
				}
				dictionary = ((dictionary2.Count != 0) ? dictionary2 : null);
			}
			return dictionary;
		}

		[RequiredByNativeCode]
		public static void AutoConnect()
		{
			bool flag = !TerrainUtility.ValidTerrainsExist();
			if (!flag)
			{
				TerrainUtility.ClearConnectivity();
				Dictionary<int, TerrainMap> dictionary = TerrainUtility.CollectTerrains(true);
				bool flag2 = dictionary == null;
				if (!flag2)
				{
					foreach (KeyValuePair<int, TerrainMap> keyValuePair in dictionary)
					{
						TerrainMap value = keyValuePair.Value;
						foreach (KeyValuePair<TerrainTileCoord, Terrain> keyValuePair2 in value.terrainTiles)
						{
							TerrainTileCoord key = keyValuePair2.Key;
							Terrain terrain = value.GetTerrain(key.tileX, key.tileZ);
							Terrain terrain2 = value.GetTerrain(key.tileX - 1, key.tileZ);
							Terrain terrain3 = value.GetTerrain(key.tileX + 1, key.tileZ);
							Terrain terrain4 = value.GetTerrain(key.tileX, key.tileZ + 1);
							Terrain terrain5 = value.GetTerrain(key.tileX, key.tileZ - 1);
							terrain.SetNeighbors(terrain2, terrain4, terrain3, terrain5);
						}
					}
				}
			}
		}
	}
}
