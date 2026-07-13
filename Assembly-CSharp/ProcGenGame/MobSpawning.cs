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
		public static Dictionary<int, string> PlaceFeatureAmbientMobs(WorldGenSettings settings, TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells, bool isDebug, ref HashSet<int> alreadyOccupiedCells)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			Cell node = tc.node;
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
					List<int> mobPossibleSpawnPoints = MobSpawning.GetMobPossibleSpawnPoints(mob, availableSpawnCellsFeature, cells, alreadyOccupiedCells, rnd);
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
						MobSpawning.SpawnCountMobs(mob, tag2, num2, mobPossibleSpawnPoints, tc, ref dictionary, ref alreadyOccupiedCells);
					}
				}
			}
			return dictionary;
		}

		public static Dictionary<int, string> PlaceBiomeAmbientMobs(WorldGenSettings settings, TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells, bool isDebug, ref HashSet<int> alreadyOccupiedCells)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			Cell node = tc.node;
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
					List<int> mobPossibleSpawnPoints = MobSpawning.GetMobPossibleSpawnPoints(mob, list2, cells, alreadyOccupiedCells, rnd);
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
						MobSpawning.SpawnCountMobs(mob, tag2, num3, mobPossibleSpawnPoints, tc, ref dictionary, ref alreadyOccupiedCells);
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
			int num2 = 0;
			while (num2 - num < count && num2 < mobPossibleSpawnPoints.Count)
			{
				int num3 = mobPossibleSpawnPoints[num2];
				int num4 = ((mobData.location == Mob.Location.Ceiling) ? (-1) : 1);
				bool flag = false;
				for (int i = 0; i < mobData.width + mobData.paddingX * 2; i++)
				{
					for (int j = 0; j < mobData.height; j++)
					{
						int num5 = MobSpawning.MobWidthOffset(Grid.OffsetCell(num3, 0, j * num4), i);
						if (alreadyOccupiedCells.Contains(num5))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (flag)
				{
					num++;
				}
				else
				{
					for (int k = 0; k < mobData.width + mobData.paddingX * 2; k++)
					{
						for (int l = 0; l < mobData.height; l++)
						{
							int num6 = MobSpawning.MobWidthOffset(Grid.OffsetCell(num3, 0, l * num4), k);
							alreadyOccupiedCells.Add(num6);
						}
					}
					tc.AddMob(new KeyValuePair<int, Tag>(num3, mobPrefab));
					spawnedMobs.Add(num3, mobPrefab.Name);
				}
				num2++;
			}
		}

		public static int MobWidthOffset(int occupiedCell, int widthIterator)
		{
			return Grid.OffsetCell(occupiedCell, (widthIterator % 2 == 0) ? (-(widthIterator / 2)) : (widthIterator / 2 + widthIterator % 2), 0);
		}

		private static bool IsSuitableMobSpawnPoint(int cell, Mob mob, Sim.Cell[] cells, ref HashSet<int> alreadyOccupiedCells)
		{
			int num = ((mob.location == Mob.Location.Ceiling || mob.location == Mob.Location.LiquidCeiling) ? (-1) : 1);
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			int num2 = mob.width + mob.paddingX * 2;
			int num3 = num2 / 2 - mob.width - mob.paddingX + 1;
			CellOffset cellOffset = new CellOffset(num3 - 1, (num < 0) ? 1 : mob.height);
			CellOffset cellOffset2 = cellOffset + new CellOffset(mob.width + 1, -(mob.height + 1));
			if (!Grid.IsCellOffsetValid(cell, cellOffset) || !Grid.IsCellOffsetValid(cell, cellOffset2))
			{
				return false;
			}
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < mob.height; j++)
				{
					int num4 = MobSpawning.MobWidthOffset(Grid.OffsetCell(cell, 0, j * num), i);
					if (!Grid.IsValidCell(num4))
					{
						return false;
					}
					if (alreadyOccupiedCells.Contains(num4))
					{
						return false;
					}
				}
			}
			Element element = ElementLoader.elements[(int)cells[cell].elementIdx];
			Element element2 = ElementLoader.elements[(int)cells[Grid.CellAbove(cell)].elementIdx];
			switch (mob.location)
			{
			case Mob.Location.Floor:
			{
				bool flag = true;
				for (int k = 0; k < mob.height; k++)
				{
					for (int l = 0; l < num2; l++)
					{
						int num5 = Grid.OffsetCell(cell, 0, k);
						num5 = MobSpawning.MobWidthOffset(num5, l);
						Element element3 = ElementLoader.elements[(int)cells[num5].elementIdx];
						Element element4 = ElementLoader.elements[(int)cells[Grid.CellAbove(num5)].elementIdx];
						Element element5 = ElementLoader.elements[(int)cells[Grid.CellBelow(num5)].elementIdx];
						flag = flag && MobSpawning.isNaturalCavity(num5);
						flag = flag && !element3.IsSolid;
						flag = flag && !element3.IsLiquid;
						if (k == 0 && l < mob.width)
						{
							flag = flag && element5.IsSolid;
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
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'Ceiling' and is used for mob " + mob.name);
				bool flag2 = true;
				for (int m = 0; m < mob.height; m++)
				{
					for (int n = 0; n < mob.width; n++)
					{
						int num6 = Grid.OffsetCell(cell, 0, -m);
						num6 = MobSpawning.MobWidthOffset(num6, n);
						Element element6 = ElementLoader.elements[(int)cells[num6].elementIdx];
						Element element7 = ElementLoader.elements[(int)cells[Grid.CellAbove(num6)].elementIdx];
						if (m == 0)
						{
							flag2 = flag2 && element7.IsSolid;
						}
						flag2 = flag2 && MobSpawning.isNaturalCavity(num6);
						flag2 = flag2 && !element6.IsSolid;
						flag2 = flag2 && !element6.IsLiquid;
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
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'Air' and is used for mob " + mob.name);
				return !element.IsSolid && !element2.IsSolid && !element.IsLiquid;
			case Mob.Location.Solid:
			{
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'Solid' and is used for mob " + mob.name);
				for (int num7 = 0; num7 < mob.width; num7++)
				{
					for (int num8 = 0; num8 < mob.height; num8++)
					{
						int num9 = MobSpawning.MobWidthOffset(Grid.OffsetCell(cell, 0, num8 * num), num7);
						if (MobSpawning.isNaturalCavity(num9) || !ElementLoader.elements[(int)cells[num9].elementIdx].IsSolid)
						{
							return false;
						}
					}
				}
				return true;
			}
			case Mob.Location.Water:
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'Water' and is used for mob " + mob.name);
				return (element.id == SimHashes.Water || element.id == SimHashes.DirtyWater) && (element2.id == SimHashes.Water || element2.id == SimHashes.DirtyWater);
			case Mob.Location.Surface:
			{
				bool flag3 = true;
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'Surface' and is used for mob " + mob.name);
				for (int num10 = 0; num10 < mob.width; num10++)
				{
					int num11 = MobSpawning.MobWidthOffset(cell, num10);
					Element element8 = ElementLoader.elements[(int)cells[num11].elementIdx];
					Element element9 = ElementLoader.elements[(int)cells[Grid.CellBelow(num11)].elementIdx];
					flag3 = flag3 && element8.id == SimHashes.Vacuum;
					flag3 = flag3 && element9.IsSolid;
				}
				return flag3;
			}
			case Mob.Location.LiquidFloor:
			{
				bool flag4 = true;
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'LiquidFloor' and is used for mob " + mob.name);
				for (int num12 = 0; num12 < mob.height; num12++)
				{
					for (int num13 = 0; num13 < mob.width; num13++)
					{
						int num14 = Grid.OffsetCell(cell, 0, num12);
						num14 = MobSpawning.MobWidthOffset(num14, num13);
						Element element10 = ElementLoader.elements[(int)cells[num14].elementIdx];
						Element element11 = ElementLoader.elements[(int)cells[Grid.CellAbove(num14)].elementIdx];
						Element element12 = ElementLoader.elements[(int)cells[Grid.CellBelow(num14)].elementIdx];
						flag4 = flag4 && MobSpawning.isNaturalCavity(cell);
						flag4 = flag4 && !element10.IsSolid;
						if (num12 == 0)
						{
							flag4 = flag4 && element10.IsLiquid;
							flag4 = flag4 && element12.IsSolid;
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
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'AnyFloor' and is used for mob " + mob.name);
				for (int num15 = 0; num15 < mob.height; num15++)
				{
					for (int num16 = 0; num16 < mob.width; num16++)
					{
						int num17 = Grid.OffsetCell(cell, 0, num15);
						num17 = MobSpawning.MobWidthOffset(num17, num16);
						Element element13 = ElementLoader.elements[(int)cells[num17].elementIdx];
						Element element14 = ElementLoader.elements[(int)cells[Grid.CellAbove(num17)].elementIdx];
						Element element15 = ElementLoader.elements[(int)cells[Grid.CellBelow(num17)].elementIdx];
						flag5 = flag5 && MobSpawning.isNaturalCavity(cell);
						flag5 = flag5 && !element13.IsSolid;
						if (num15 == 0)
						{
							flag5 = flag5 && element15.IsSolid;
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
			case Mob.Location.LiquidCeiling:
			{
				bool flag6 = true;
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'LiquidCeiling' and is used for mob " + mob.name);
				for (int num18 = 0; num18 < mob.height; num18++)
				{
					for (int num19 = 0; num19 < mob.width; num19++)
					{
						int num20 = Grid.OffsetCell(cell, 0, -num18);
						num20 = MobSpawning.MobWidthOffset(num20, num19);
						Element element16 = ElementLoader.elements[(int)cells[num20].elementIdx];
						Element element17 = ElementLoader.elements[(int)cells[Grid.CellAbove(num20)].elementIdx];
						if (num18 == 0)
						{
							flag6 = flag6 && element17.IsSolid;
						}
						flag6 = flag6 && MobSpawning.isNaturalCavity(num20);
						flag6 = flag6 && element16.IsLiquid;
						flag6 = flag6 && !element16.IsSolid;
						if (!flag6)
						{
							break;
						}
					}
					if (!flag6)
					{
						break;
					}
				}
				return flag6;
			}
			case Mob.Location.Liquid:
			{
				bool flag7 = true;
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'Liquid' and is used for mob " + mob.name);
				for (int num21 = 0; num21 < mob.height; num21++)
				{
					for (int num22 = 0; num22 < mob.width; num22++)
					{
						int num23 = Grid.OffsetCell(cell, 0, num21);
						num23 = MobSpawning.MobWidthOffset(num23, num22);
						Element element18 = ElementLoader.elements[(int)cells[num23].elementIdx];
						flag7 = flag7 && element18.IsLiquid;
						if (num21 == mob.height - 1)
						{
							Element element19 = ElementLoader.elements[(int)cells[Grid.CellAbove(num23)].elementIdx];
							flag7 = flag7 && element19.IsLiquid;
						}
					}
				}
				return flag7;
			}
			case Mob.Location.EntombedFloorPeek:
			{
				bool flag8 = false;
				bool flag9 = true;
				global::Debug.Assert(mob.paddingX == 0, "Mob paddingX not implemented yet for rule 'EntombedFloorPeek' and is used for mob " + mob.name);
				for (int num24 = 0; num24 < mob.height; num24++)
				{
					for (int num25 = 0; num25 < mob.width; num25++)
					{
						int num26 = Grid.OffsetCell(cell, 0, num24);
						num26 = MobSpawning.MobWidthOffset(num26, num25);
						Element element20 = ElementLoader.elements[(int)cells[num26].elementIdx];
						Element element21 = ElementLoader.elements[(int)cells[Grid.CellBelow(num26)].elementIdx];
						flag8 = flag8 || !element20.IsSolid;
						if (num24 == 0)
						{
							flag9 = flag9 && element21.IsSolid;
						}
						if (!flag9)
						{
							break;
						}
					}
					if (!flag9)
					{
						break;
					}
				}
				return flag9 && flag8;
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
