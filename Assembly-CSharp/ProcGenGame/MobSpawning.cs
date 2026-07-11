using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace ProcGenGame
{
	public static class MobSpawning
	{
		public static Dictionary<int, string> PlaceFeatureAmbientMobs(WorldGenSettings settings, TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells, bool isDebug)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			Node node = tc.node;
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
			tc.LogInfo("mob spawns", "Id:" + node.node.Id + " possible cells", (float)availableSpawnCellsFeature.Count);
			if (availableSpawnCellsFeature.Count == 0)
			{
				if (isDebug)
				{
					global::Debug.LogWarning("No where to put mobs possibleSpawnPoints [" + tc.node.node.Id + "]");
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
						tc.LogInfo("\t\tpossible", string.Concat(new object[] { mobReference.type, " mps: ", mobPossibleSpawnPoints.Count, " ps:" }), (float)availableSpawnCellsFeature.Count);
						int num2 = Mathf.RoundToInt(mobReference.count.GetRandomValueWithinRange(rnd));
						tc.LogInfo("\t\tcount", mobReference.type, (float)num2);
						Tag tag2 = ((mob.prefabName != null) ? new Tag(mob.prefabName) : new Tag(mobReference.type));
						MobSpawning.SpawnCountMobs(mob, tag2, num2, mobPossibleSpawnPoints, tc, ref dictionary, ref hashSet);
					}
				}
			}
			return dictionary;
		}

		public static Dictionary<int, string> PlaceBiomeAmbientMobs(WorldGenSettings settings, TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells, bool isDebug)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			Node node = tc.node;
			HashSet<int> hashSet = new HashSet<int>();
			List<Tag> list = new List<Tag>();
			if (node.biomeSpecificTags == null)
			{
				tc.LogInfo("PlaceBiomeAmbientMobs", "No tags", (float)node.node.Id);
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
				tc.LogInfo("PlaceBiomeAmbientMobs", "No biome MOBS", (float)node.node.Id);
				return null;
			}
			List<int> list2 = ((!node.tags.Contains(WorldGenTags.PreventAmbientMobsInFeature)) ? tc.GetAvailableSpawnCellsAll() : tc.GetAvailableSpawnCellsBiome());
			tc.LogInfo("PlaceBiomAmbientMobs", "possibleSpawnPoints", (float)list2.Count);
			for (int i = list2.Count - 1; i > 0; i--)
			{
				int num = list2[i];
				if (ElementLoader.elements[(int)cells[num].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[(int)cells[num].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(num))
				{
					list2.RemoveAt(i);
				}
			}
			tc.LogInfo("mob spawns", "Id:" + node.node.Id + " possible cells", (float)list2.Count);
			if (list2.Count == 0)
			{
				if (isDebug)
				{
					global::Debug.LogWarning("No where to put mobs possibleSpawnPoints [" + tc.node.node.Id + "]");
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
						tc.LogInfo("\t\tpossible", string.Concat(new object[]
						{
							list[j].ToString(),
							" mps: ",
							mobPossibleSpawnPoints.Count,
							" ps:"
						}), (float)list2.Count);
						float num2 = mob.density.GetRandomValueWithinRange(rnd) * MobSettings.AmbientMobDensity;
						if (num2 > 1f)
						{
							if (isDebug)
							{
								global::Debug.LogWarning("Got a mob density greater than 1.0 for " + list[j].Name + ". Probably using density as spacing!");
							}
							num2 = 1f;
						}
						tc.LogInfo("\t\tdensity:", string.Empty, num2);
						int num3 = Mathf.RoundToInt((float)mobPossibleSpawnPoints.Count * num2);
						tc.LogInfo("\t\tcount", list[j].ToString(), (float)num3);
						Tag tag2 = ((mob.prefabName != null) ? new Tag(mob.prefabName) : list[j]);
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
			return Grid.OffsetCell(occupiedCell, (widthIterator % 2 != 0) ? (widthIterator / 2 + widthIterator % 2) : (-(widthIterator / 2)), 0);
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
			switch (mob.location)
			{
			case Mob.Location.Floor:
				return MobSpawning.isNaturalCavity(cell) && !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellBelow(cell)] && !Grid.IsLiquid(cell);
			case Mob.Location.Ceiling:
				return MobSpawning.isNaturalCavity(cell) && !Grid.Solid[cell] && Grid.Solid[Grid.CellAbove(cell)] && !Grid.Solid[Grid.CellBelow(cell)] && !Grid.IsLiquid(cell);
			case Mob.Location.Air:
				return !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && !Grid.IsLiquid(cell);
			case Mob.Location.Solid:
				return !MobSpawning.isNaturalCavity(cell) && Grid.Solid[cell];
			case Mob.Location.Water:
				return (Grid.Element[cell].id == SimHashes.Water || Grid.Element[cell].id == SimHashes.DirtyWater) && (Grid.Element[Grid.CellAbove(cell)].id == SimHashes.Water || Grid.Element[Grid.CellAbove(cell)].id == SimHashes.DirtyWater);
			case Mob.Location.Surface:
			{
				bool flag = true;
				for (int k = 0; k < mob.width; k++)
				{
					int num2 = MobSpawning.MobWidthOffset(cell, k);
					flag = flag && Grid.Element[num2].id == SimHashes.Vacuum;
					flag = flag && Grid.Solid[Grid.CellBelow(num2)];
				}
				return flag;
			}
			case Mob.Location.LiquidFloor:
				return MobSpawning.isNaturalCavity(cell) && !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellBelow(cell)] && Grid.IsLiquid(cell);
			case Mob.Location.AnyFloor:
				return MobSpawning.isNaturalCavity(cell) && !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellBelow(cell)];
			}
			return MobSpawning.isNaturalCavity(cell) && !Grid.Solid[cell];
		}

		public static bool isNaturalCavity(int cell)
		{
			return MobSpawning.NaturalCavities != null && MobSpawning.allNaturalCavityCells.Contains(cell);
		}

		public static void DetectNaturalCavities(List<TerrainCell> terrainCells, WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLD.key, 0.8f, WorldGenProgressStages.Stages.DetectNaturalCavities);
			HashSet<int> invalidCells = new HashSet<int>();
			for (int i = 0; i < terrainCells.Count; i++)
			{
				TerrainCell terrainCell = terrainCells[i];
				float num = (float)i / (float)terrainCells.Count * 100f;
				updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, num, WorldGenProgressStages.Stages.DetectNaturalCavities);
				MobSpawning.NaturalCavities.Add(terrainCell, new List<HashSet<int>>());
				invalidCells.Clear();
				List<int> allCells = terrainCell.GetAllCells();
				for (int j = 0; j < allCells.Count; j++)
				{
					int num2 = allCells[j];
					if (!Grid.Solid[num2] && !invalidCells.Contains(num2))
					{
						HashSet<int> hashSet = GameUtil.FloodCollectCells(num2, (int checkCell) => !invalidCells.Contains(checkCell) && !Grid.Solid[checkCell], 300, invalidCells, true);
						if (hashSet != null && hashSet.Count > 0)
						{
							MobSpawning.NaturalCavities[terrainCell].Add(hashSet);
							MobSpawning.allNaturalCavityCells.UnionWith(hashSet);
						}
					}
				}
			}
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, 100f, WorldGenProgressStages.Stages.DetectNaturalCavities);
		}

		public static Dictionary<TerrainCell, List<HashSet<int>>> NaturalCavities = new Dictionary<TerrainCell, List<HashSet<int>>>();

		public static HashSet<int> allNaturalCavityCells = new HashSet<int>();
	}
}
