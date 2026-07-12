using System;
using System.Collections.Generic;
using ProcGen;
using ProcGen.Map;
using STRINGS;
using UnityEngine;

namespace ProcGenGame
{
	public static class MobSpawning
	{
		public static Dictionary<int, string> PlaceFeatureAmbientMobs(WorldGenSettings settings, TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells, bool isDebug)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			Cell node = tc.node;
			HashSet<int> hashSet = new HashSet<int>();
			FeatureSettings featureSettings = null;
			foreach (Tag tag in node.featureSpecificTags)
			{
				if (settings.HasFeature(tag.Name))
				{
					featureSettings = settings.GetFeature(tag.Name);
					break;
				}
			}
			if (featureSettings == null)
			{
				return dictionary;
			}
			if (featureSettings.internalMobs == null || featureSettings.internalMobs.Count == 0)
			{
				return dictionary;
			}
			List<int> availableSpawnCellsFeature = tc.GetAvailableSpawnCellsFeature();
			tc.LogInfo("PlaceFeatureAmbientMobs", "possibleSpawnPoints", (float)availableSpawnCellsFeature.Count);
			for (int i = availableSpawnCellsFeature.Count - 1; i > 0; i--)
			{
				int num = availableSpawnCellsFeature[i];
				if (ElementLoader.elements[(int)cells[num].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[(int)cells[num].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(num))
				{
					availableSpawnCellsFeature.RemoveAt(i);
				}
			}
			tc.LogInfo("mob spawns", "Id:" + node.NodeId.ToString() + " possible cells", (float)availableSpawnCellsFeature.Count);
			if (availableSpawnCellsFeature.Count == 0)
			{
				if (isDebug)
				{
					global::Debug.LogWarning("No where to put mobs possibleSpawnPoints [" + tc.node.NodeId.ToString() + "]");
				}
				return null;
			}
			foreach (MobReference mobReference in featureSettings.internalMobs)
			{
				Mob mob = settings.GetMob(mobReference.type);
				if (mob == null)
				{
					global::Debug.LogError("Missing mob description for internal mob [" + mobReference.type + "]");
				}
				else
				{
					List<int> mobPossibleSpawnPoints = MobSpawning.GetMobPossibleSpawnPoints(mob, availableSpawnCellsFeature, cells, hashSet, rnd);
					if (mobPossibleSpawnPoints.Count == 0)
					{
						if (isDebug)
						{
						}
					}
					else
					{
						tc.LogInfo("\t\tpossible", mobReference.type + " mps: " + mobPossibleSpawnPoints.Count.ToString() + " ps:", (float)availableSpawnCellsFeature.Count);
						int num2 = Mathf.RoundToInt(mobReference.count.GetRandomValueWithinRange(rnd));
						tc.LogInfo("\t\tcount", mobReference.type, (float)num2);
						Tag tag2 = ((mob.prefabName == null) ? new Tag(mobReference.type) : new Tag(mob.prefabName));
						MobSpawning.SpawnCountMobs(mob, tag2, num2, mobPossibleSpawnPoints, tc, ref dictionary, ref hashSet);
					}
				}
			}
			return dictionary;
		}

		public static Dictionary<int, string> PlaceBiomeAmbientMobs(WorldGenSettings settings, TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells, bool isDebug)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			Cell node = tc.node;
			HashSet<int> hashSet = new HashSet<int>();
			List<Tag> list = new List<Tag>();
			if (node.biomeSpecificTags == null)
			{
				tc.LogInfo("PlaceBiomeAmbientMobs", "No tags", (float)node.NodeId);
				return null;
			}
			foreach (Tag tag in node.biomeSpecificTags)
			{
				if (settings.HasMob(tag.Name) && settings.GetMob(tag.Name) != null)
				{
					list.Add(tag);
				}
			}
			if (list.Count <= 0)
			{
				tc.LogInfo("PlaceBiomeAmbientMobs", "No biome MOBS", (float)node.NodeId);
				return null;
			}
			List<int> list2 = (node.tags.Contains(WorldGenTags.PreventAmbientMobsInFeature) ? tc.GetAvailableSpawnCellsBiome() : tc.GetAvailableSpawnCellsAll());
			tc.LogInfo("PlaceBiomAmbientMobs", "possibleSpawnPoints", (float)list2.Count);
			for (int i = list2.Count - 1; i > 0; i--)
			{
				int num = list2[i];
				if (ElementLoader.elements[(int)cells[num].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[(int)cells[num].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(num))
				{
					list2.RemoveAt(i);
				}
			}
			tc.LogInfo("mob spawns", "Id:" + node.NodeId.ToString() + " possible cells", (float)list2.Count);
			if (list2.Count == 0)
			{
				if (isDebug)
				{
					global::Debug.LogWarning("No where to put mobs possibleSpawnPoints [" + tc.node.NodeId.ToString() + "]");
				}
				return null;
			}
			list.ShuffleSeeded<Tag>(rnd.RandomSource());
			for (int j = 0; j < list.Count; j++)
			{
				Mob mob = settings.GetMob(list[j].Name);
				if (mob == null)
				{
					global::Debug.LogError("Missing sample description for tag [" + list[j].Name + "]");
				}
				else
				{
					List<int> mobPossibleSpawnPoints = MobSpawning.GetMobPossibleSpawnPoints(mob, list2, cells, hashSet, rnd);
					if (mobPossibleSpawnPoints.Count == 0)
					{
						if (isDebug)
						{
						}
					}
					else
					{
						tc.LogInfo("\t\tpossible", list[j].ToString() + " mps: " + mobPossibleSpawnPoints.Count.ToString() + " ps:", (float)list2.Count);
						float num2 = mob.density.GetRandomValueWithinRange(rnd) * MobSettings.AmbientMobDensity;
						if (num2 > 1f)
						{
							if (isDebug)
							{
								global::Debug.LogWarning("Got a mob density greater than 1.0 for " + list[j].Name + ". Probably using density as spacing!");
							}
							num2 = 1f;
						}
						tc.LogInfo("\t\tdensity:", "", num2);
						int num3 = Mathf.RoundToInt((float)mobPossibleSpawnPoints.Count * num2);
						tc.LogInfo("\t\tcount", list[j].ToString(), (float)num3);
						Tag tag2 = ((mob.prefabName == null) ? list[j] : new Tag(mob.prefabName));
						MobSpawning.SpawnCountMobs(mob, tag2, num3, mobPossibleSpawnPoints, tc, ref dictionary, ref hashSet);
					}
				}
			}
			return dictionary;
		}

		private static List<int> GetMobPossibleSpawnPoints(Mob mob, List<int> possibleSpawnPoints, Sim.Cell[] cells, HashSet<int> alreadyOccupiedCells, SeededRandom rnd)
		{
			List<int> list = possibleSpawnPoints.FindAll((int cell) => MobSpawning.IsSuitableMobSpawnPoint(cell, mob, cells, ref alreadyOccupiedCells));
			list.ShuffleSeeded<int>(rnd.RandomSource());
			return list;
		}

		public static void SpawnCountMobs(Mob mobData, Tag mobPrefab, int count, List<int> mobPossibleSpawnPoints, TerrainCell tc, ref Dictionary<int, string> spawnedMobs, ref HashSet<int> alreadyOccupiedCells)
		{
			int num = 0;
			while (num < count && num < mobPossibleSpawnPoints.Count)
			{
				int num2 = mobPossibleSpawnPoints[num];
				for (int i = 0; i < mobData.width; i++)
				{
					for (int j = 0; j < mobData.height; j++)
					{
						int num3 = MobSpawning.MobWidthOffset(num2, i);
						alreadyOccupiedCells.Add(num3);
					}
				}
				tc.AddMob(new KeyValuePair<int, Tag>(num2, mobPrefab));
				spawnedMobs.Add(num2, mobPrefab.Name);
				num++;
			}
		}

		public static int MobWidthOffset(int occupiedCell, int widthIterator)
		{
			return Grid.OffsetCell(occupiedCell, (widthIterator % 2 == 0) ? (-(widthIterator / 2)) : (widthIterator / 2 + widthIterator % 2), 0);
		}

		private static bool IsSuitableMobSpawnPoint(int cell, Mob mob, Sim.Cell[] cells, ref HashSet<int> alreadyOccupiedCells)
		{
			for (int i = 0; i < mob.width; i++)
			{
				for (int j = 0; j < mob.height; j++)
				{
					int num = MobSpawning.MobWidthOffset(cell, i);
					if (!Grid.IsValidCell(num) || !Grid.IsValidCell(Grid.CellAbove(num)) || !Grid.IsValidCell(Grid.CellBelow(num)))
					{
						return false;
					}
					if (alreadyOccupiedCells.Contains(num))
					{
						return false;
					}
				}
			}
			Element element = ElementLoader.elements[(int)cells[cell].elementIdx];
			Element element2 = ElementLoader.elements[(int)cells[Grid.CellAbove(cell)].elementIdx];
			Element element3 = ElementLoader.elements[(int)cells[Grid.CellBelow(cell)].elementIdx];
			switch (mob.location)
			{
			case Mob.Location.Floor:
			{
				bool flag = true;
				for (int k = 0; k < mob.height; k++)
				{
					for (int l = 0; l < mob.width; l++)
					{
						int num2 = Grid.OffsetCell(cell, l, k);
						Element element4 = ElementLoader.elements[(int)cells[num2].elementIdx];
						Element element5 = ElementLoader.elements[(int)cells[Grid.CellAbove(num2)].elementIdx];
						Element element6 = ElementLoader.elements[(int)cells[Grid.CellBelow(num2)].elementIdx];
						flag = flag && MobSpawning.isNaturalCavity(num2);
						flag = flag && !element4.IsSolid;
						flag = flag && !element4.IsLiquid;
						flag = flag && !element5.IsSolid;
						if (k == 0)
						{
							flag = flag && element6.IsSolid;
						}
						if (!flag)
						{
							break;
						}
					}
					if (!flag)
					{
						break;
					}
				}
				return flag;
			}
			case Mob.Location.Ceiling:
			{
				bool flag2 = true;
				for (int m = 0; m < mob.height; m++)
				{
					for (int n = 0; n < mob.width; n++)
					{
						int num3 = Grid.OffsetCell(cell, n, m);
						Element element7 = ElementLoader.elements[(int)cells[num3].elementIdx];
						Element element8 = ElementLoader.elements[(int)cells[Grid.CellAbove(num3)].elementIdx];
						Element element9 = ElementLoader.elements[(int)cells[Grid.CellBelow(num3)].elementIdx];
						flag2 = flag2 && MobSpawning.isNaturalCavity(num3);
						flag2 = flag2 && !element7.IsSolid;
						flag2 = flag2 && !element7.IsLiquid;
						flag2 = flag2 && !element9.IsSolid;
						if (m == mob.height - 1)
						{
							flag2 = flag2 && element8.IsSolid;
						}
						if (!flag2)
						{
							break;
						}
					}
					if (!flag2)
					{
						break;
					}
				}
				return flag2;
			}
			case Mob.Location.Air:
				return !element.IsSolid && !element2.IsSolid && !element.IsLiquid;
			case Mob.Location.Solid:
				return !MobSpawning.isNaturalCavity(cell) && element.IsSolid;
			case Mob.Location.Water:
				return (element.id == SimHashes.Water || element.id == SimHashes.DirtyWater) && (element2.id == SimHashes.Water || element2.id == SimHashes.DirtyWater);
			case Mob.Location.Surface:
			{
				bool flag3 = true;
				for (int num4 = 0; num4 < mob.width; num4++)
				{
					int num5 = MobSpawning.MobWidthOffset(cell, num4);
					Element element10 = ElementLoader.elements[(int)cells[num5].elementIdx];
					Element element11 = ElementLoader.elements[(int)cells[Grid.CellBelow(num5)].elementIdx];
					flag3 = flag3 && element10.id == SimHashes.Vacuum;
					flag3 = flag3 && element11.IsSolid;
				}
				return flag3;
			}
			case Mob.Location.LiquidFloor:
			{
				bool flag4 = true;
				for (int num6 = 0; num6 < mob.height; num6++)
				{
					for (int num7 = 0; num7 < mob.width; num7++)
					{
						int num8 = Grid.OffsetCell(cell, num7, num6);
						Element element12 = ElementLoader.elements[(int)cells[num8].elementIdx];
						Element element13 = ElementLoader.elements[(int)cells[Grid.CellAbove(num8)].elementIdx];
						Element element14 = ElementLoader.elements[(int)cells[Grid.CellBelow(num8)].elementIdx];
						flag4 = flag4 && MobSpawning.isNaturalCavity(cell);
						flag4 = flag4 && !element12.IsSolid;
						flag4 = flag4 && !element13.IsSolid;
						if (num6 == 0)
						{
							flag4 = flag4 && element12.IsLiquid;
							flag4 = flag4 && element14.IsSolid;
						}
						if (!flag4)
						{
							break;
						}
					}
					if (!flag4)
					{
						break;
					}
				}
				return flag4;
			}
			case Mob.Location.AnyFloor:
			{
				bool flag5 = true;
				for (int num9 = 0; num9 < mob.height; num9++)
				{
					for (int num10 = 0; num10 < mob.width; num10++)
					{
						int num11 = Grid.OffsetCell(cell, num10, num9);
						Element element15 = ElementLoader.elements[(int)cells[num11].elementIdx];
						Element element16 = ElementLoader.elements[(int)cells[Grid.CellAbove(num11)].elementIdx];
						Element element17 = ElementLoader.elements[(int)cells[Grid.CellBelow(num11)].elementIdx];
						flag5 = flag5 && MobSpawning.isNaturalCavity(cell);
						flag5 = flag5 && !element15.IsSolid;
						flag5 = flag5 && !element16.IsSolid;
						if (num9 == 0)
						{
							flag5 = flag5 && element17.IsSolid;
						}
						if (!flag5)
						{
							break;
						}
					}
					if (!flag5)
					{
						break;
					}
				}
				return flag5;
			}
			}
			return MobSpawning.isNaturalCavity(cell) && !element.IsSolid;
		}

		public static bool isNaturalCavity(int cell)
		{
			return MobSpawning.NaturalCavities != null && MobSpawning.allNaturalCavityCells.Contains(cell);
		}

		public static void DetectNaturalCavities(List<TerrainCell> terrainCells, WorldGen.OfflineCallbackFunction updateProgressFn, Sim.Cell[] cells)
		{
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLD.key, 0f, WorldGenProgressStages.Stages.DetectNaturalCavities);
			MobSpawning.NaturalCavities.Clear();
			MobSpawning.allNaturalCavityCells.Clear();
			HashSet<int> invalidCells = new HashSet<int>();
			Func<int, bool> <>9__0;
			for (int i = 0; i < terrainCells.Count; i++)
			{
				TerrainCell terrainCell = terrainCells[i];
				float num = (float)i / (float)terrainCells.Count;
				updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, num, WorldGenProgressStages.Stages.DetectNaturalCavities);
				MobSpawning.NaturalCavities.Add(terrainCell, new List<HashSet<int>>());
				invalidCells.Clear();
				List<int> allCells = terrainCell.GetAllCells();
				for (int j = 0; j < allCells.Count; j++)
				{
					int num2 = allCells[j];
					if (!ElementLoader.elements[(int)cells[num2].elementIdx].IsSolid && !invalidCells.Contains(num2))
					{
						int num3 = num2;
						Func<int, bool> func;
						if ((func = <>9__0) == null)
						{
							func = (<>9__0 = delegate(int checkCell)
							{
								Element element = ElementLoader.elements[(int)cells[checkCell].elementIdx];
								return !invalidCells.Contains(checkCell) && !element.IsSolid;
							});
						}
						HashSet<int> hashSet = GameUtil.FloodCollectCells(num3, func, 300, invalidCells, true);
						if (hashSet != null && hashSet.Count > 0)
						{
							MobSpawning.NaturalCavities[terrainCell].Add(hashSet);
							MobSpawning.allNaturalCavityCells.UnionWith(hashSet);
						}
					}
				}
			}
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, 1f, WorldGenProgressStages.Stages.DetectNaturalCavities);
		}

		public static Dictionary<TerrainCell, List<HashSet<int>>> NaturalCavities = new Dictionary<TerrainCell, List<HashSet<int>>>();

		public static HashSet<int> allNaturalCavityCells = new HashSet<int>();
	}
}
