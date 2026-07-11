using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace ProcGenGame
{
	public static class MobSpawning
	{
		public static Dictionary<int, string> PlaceAmbientMobs(TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			Node node = tc.node;
			HashSet<int> alreadyOccupiedCells = new HashSet<int>();
			List<Tag> list = new List<Tag>();
			bool flag = false;
			int num = 0;
			if (node.tags == null || node.biomeSpecificTags == null)
			{
				tc.LogInfo("PlaceAmbientMobs", "No tags", (float)node.node.Id);
				return null;
			}
			foreach (Tag tag in node.biomeSpecificTags)
			{
				if (WorldGen.Settings.mobs.HasMob(tag.Name) && WorldGen.Settings.mobs.GetMob(tag.Name) != null)
				{
					list.Add(tag);
					num++;
					flag = true;
				}
			}
			if (!flag)
			{
				tc.LogInfo("PlaceAmbientMobs", "No biome MOBS", (float)node.node.Id);
				return null;
			}
			List<int> availableSpawnCells = tc.GetAvailableSpawnCells();
			tc.LogInfo("PlaceAmbientMobs", "possibleSpawnPoints", (float)availableSpawnCells.Count);
			for (int i = availableSpawnCells.Count - 1; i > 0; i--)
			{
				int num2 = availableSpawnCells[i];
				if (ElementLoader.elements[(int)cells[num2].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[(int)cells[num2].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(num2))
				{
					availableSpawnCells.RemoveAt(i);
				}
			}
			tc.LogInfo("mob spawns", "Id:" + node.node.Id + " possible cells", (float)availableSpawnCells.Count);
			if (availableSpawnCells.Count == 0)
			{
				if (WorldGen.isRunningDebugGen)
				{
					global::Debug.LogWarning("No where to put mobs possibleSpawnPoints [" + tc.node.node.Id + "]", null);
				}
				return null;
			}
			int num3 = 0;
			while (num3 < MobSettings.AmbientMobDensity && availableSpawnCells.Count > 0)
			{
				list.ShuffleSeeded<Tag>(rnd.RandomSource());
				for (int j = 0; j < list.Count; j++)
				{
					if (!WorldGen.Settings.mobs.GetMobTags().Contains(list[j]))
					{
						global::Debug.LogError("Missing sample description for tag [" + list[j].Name + "]", null);
					}
					else
					{
						Mob mob = WorldGen.Settings.mobs.MobLookupTable[list[j].Name];
						List<int> list2 = availableSpawnCells.FindAll((int cell) => MobSpawning.isSuitableMobSpawnPoint(cell, mob, cells, bgTemp, dc, ref alreadyOccupiedCells));
						if (list2.Count == 0)
						{
							if (WorldGen.isRunningDebugGen)
							{
								global::Debug.LogWarning(string.Concat(new object[]
								{
									"No SuitableMobSpawnPoint to put mobs mobPossibleSpawnPoints [",
									list[j].Name,
									"] [",
									tc.node.node.Id,
									"]"
								}), null);
							}
						}
						else
						{
							list2.ShuffleSeeded<int>(rnd.RandomSource());
							tc.LogInfo("\t\tpossible", string.Concat(new object[]
							{
								list[j].ToString(),
								" mps: ",
								list2.Count,
								" ps:"
							}), (float)availableSpawnCells.Count);
							float num4 = mob.density.GetRandomValueWithinRange(rnd);
							if (num4 > 1f)
							{
								if (WorldGen.isRunningDebugGen)
								{
									global::Debug.LogWarning("Got a mob density greater than 1.0 for " + list[j].Name + ". Probably using density as spacing!", null);
								}
								num4 = 1f;
							}
							int num5 = Mathf.RoundToInt((float)list2.Count * num4);
							tc.LogInfo("\t\tcount", list[j].ToString(), (float)num5);
							Tag tag2 = ((mob.prefabName != null) ? new Tag(mob.prefabName) : list[j]);
							int num6 = 0;
							while (num6 < num5 && list2.Count != 0)
							{
								int num7 = list2[0];
								for (int k = 0; k < mob.width; k++)
								{
									for (int l = 0; l < mob.height; l++)
									{
										int num8 = MobSpawning.MobWidthOffset(num7, k);
										alreadyOccupiedCells.Add(num8);
										if (list2.Contains(num8))
										{
											list2.Remove(num8);
										}
										if (availableSpawnCells.Contains(num8))
										{
											availableSpawnCells.Remove(num8);
										}
									}
								}
								tc.AddMob(new KeyValuePair<int, Tag>(num7, tag2));
								dictionary.Add(num7, tag2.Name);
								num6++;
							}
						}
					}
				}
				num3++;
			}
			return dictionary;
		}

		public static int MobWidthOffset(int occupiedCell, int widthIterator)
		{
			return Grid.OffsetCell(occupiedCell, (widthIterator % 2 != 0) ? (widthIterator / 2 + widthIterator % 2) : (-(widthIterator / 2)), 0);
		}

		private static bool isSuitableMobSpawnPoint(int cell, Mob mob, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, ref HashSet<int> alreadyOccupiedCells)
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
			}
			return MobSpawning.isNaturalCavity(cell) && !Grid.Solid[cell];
		}

		public static bool isNaturalCavity(int cell)
		{
			return MobSpawning.NaturalCavities != null && MobSpawning.allNaturalCavityCells.Contains(cell);
		}

		public static void DetectNaturalCavities(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLD.key, 0.8f, WorldGenProgressStages.Stages.DetectNaturalCavities);
			HashSet<int> invalidCells = new HashSet<int>();
			for (int i = 0; i < WorldGen.TerrainCells.Count; i++)
			{
				TerrainCell terrainCell = WorldGen.TerrainCells[i];
				float num = (float)i / (float)WorldGen.TerrainCells.Count * 100f;
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
