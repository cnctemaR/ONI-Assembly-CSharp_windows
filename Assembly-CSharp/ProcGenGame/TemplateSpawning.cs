using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace ProcGenGame
{
	public class TemplateSpawning
	{
		public static List<KeyValuePair<Vector2I, TemplateContainer>> DetermineTemplatesForWorld(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 0f, WorldGenProgressStages.Stages.PlaceTemplates);
			List<KeyValuePair<Vector2I, TemplateContainer>> list = new List<KeyValuePair<Vector2I, TemplateContainer>>();
			TemplateSpawning.m_poiPadding = settings.GetIntSetting("POIPadding");
			TemplateSpawning.minProgressPercent = 0f;
			TemplateSpawning.maxProgressPercent = 0.33f;
			TemplateSpawning.SpawnStartingTemplate(settings, terrainCells, ref list, ref placedPOIBounds, isRunningDebugGen, successCallbackFn);
			TemplateSpawning.minProgressPercent = TemplateSpawning.maxProgressPercent;
			TemplateSpawning.maxProgressPercent = 0.66f;
			TemplateSpawning.SpawnTemplatesFromTemplateRules(settings, terrainCells, myRandom, ref list, ref placedPOIBounds, isRunningDebugGen, successCallbackFn);
			TemplateSpawning.minProgressPercent = TemplateSpawning.maxProgressPercent;
			TemplateSpawning.maxProgressPercent = 1f;
			TemplateSpawning.SpawnFeatureTemplates(settings, terrainCells, myRandom, ref list, ref placedPOIBounds, successCallbackFn);
			successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 1f, WorldGenProgressStages.Stages.PlaceTemplates);
			return list;
		}

		private static float ProgressPercent(float stagePercent)
		{
			return MathUtil.ReRange(stagePercent, 0f, 1f, TemplateSpawning.minProgressPercent, TemplateSpawning.maxProgressPercent);
		}

		private static void SpawnStartingTemplate(WorldGenSettings settings, List<TerrainCell> terrainCells, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			TerrainCell terrainCell = terrainCells.Find((TerrainCell tc) => tc.node.tags.Contains(WorldGenTags.StartLocation));
			if (settings.world.startingBaseTemplate.IsNullOrWhiteSpace())
			{
				return;
			}
			TemplateContainer template = TemplateCache.GetTemplate(settings.world.startingBaseTemplate);
			KeyValuePair<Vector2I, TemplateContainer> keyValuePair = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x, (int)terrainCell.poly.Centroid().y), template);
			RectInt templateBounds = template.GetTemplateBounds(keyValuePair.Key, TemplateSpawning.m_poiPadding);
			if (TemplateSpawning.IsPOIOverlappingBounds(placedPOIBounds, templateBounds))
			{
				string text = "TemplateSpawning: Starting template overlaps world boundaries in world '" + settings.world.filePath + "'";
				DebugUtil.DevLogError(text);
				if (!isRunningDebugGen)
				{
					throw new Exception(text);
				}
			}
			templateSpawnTargets.Add(keyValuePair);
			placedPOIBounds.Add(templateBounds);
		}

		private static void SpawnFeatureTemplates(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			int num = 0;
			float num2 = (float)settings.world.subworldFiles.Count;
			foreach (WeightedSubworldName weightedSubworldName in settings.world.subworldFiles)
			{
				successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, TemplateSpawning.ProgressPercent((float)num++ / num2), WorldGenProgressStages.Stages.PlaceTemplates);
				SubWorld subWorld = settings.GetSubWorld(weightedSubworldName.name);
				if (subWorld.featureTemplates != null && subWorld.featureTemplates.Count > 0)
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, int> keyValuePair in subWorld.featureTemplates)
					{
						for (int i = 0; i < keyValuePair.Value; i++)
						{
							if (TemplateCache.TemplateExists(keyValuePair.Key))
							{
								list.Add(keyValuePair.Key);
							}
							else
							{
								DebugUtil.DevLogError(string.Format("TemplateSpawning: Template does not exist '{0}' in world '{1}'", keyValuePair.Value, settings.world.filePath));
							}
						}
					}
					list.ShuffleSeeded<string>(myRandom.RandomSource());
					List<TerrainCell> list2 = terrainCells.FindAll((TerrainCell tc) => tc.node.tags.Contains(subWorld.name.ToTag()));
					list2.ShuffleSeeded<TerrainCell>(myRandom.RandomSource());
					foreach (TerrainCell terrainCell in list2)
					{
						if (list.Count == 0)
						{
							break;
						}
						if (terrainCell.IsSafeToSpawnFeatureTemplate(true))
						{
							string text = list[list.Count - 1];
							list.RemoveAt(list.Count - 1);
							TemplateContainer template = TemplateCache.GetTemplate(text);
							if (template != null)
							{
								RectInt templateBounds = template.GetTemplateBounds(terrainCell.poly.Centroid(), TemplateSpawning.m_poiPadding);
								if (TemplateSpawning.IsPOIOverlappingBounds(placedPOIBounds, templateBounds))
								{
									DebugUtil.LogArgs(new object[] { " -> Cannot place here" });
									break;
								}
								if (TemplateSpawning.IsPOIOverlappingHighTemperatureDelta(templateBounds, subWorld, ref terrainCells, settings))
								{
									DebugUtil.LogArgs(new object[] { " -> Cannot place here" });
									break;
								}
								KeyValuePair<Vector2I, TemplateContainer> keyValuePair2 = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x, (int)terrainCell.poly.Centroid().y), template);
								templateSpawnTargets.Add(keyValuePair2);
								placedPOIBounds.Add(template.GetTemplateBounds(keyValuePair2.Key, TemplateSpawning.m_poiPadding));
								terrainCell.node.tags.Add(text.ToTag());
								terrainCell.node.tags.Add(WorldGenTags.POI);
							}
						}
					}
				}
			}
		}

		private static void SpawnTemplatesFromTemplateRules(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			List<global::ProcGen.World.TemplateSpawnRules> list = new List<global::ProcGen.World.TemplateSpawnRules>();
			if (settings.world.worldTemplateRules != null)
			{
				list.AddRange(settings.world.worldTemplateRules);
			}
			foreach (WeightedSubworldName weightedSubworldName in settings.world.subworldFiles)
			{
				SubWorld subWorld = settings.GetSubWorld(weightedSubworldName.name);
				if (subWorld.subworldTemplateRules != null)
				{
					list.AddRange(subWorld.subworldTemplateRules);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			int num = 0;
			float num2 = (float)list.Count;
			list.Sort((global::ProcGen.World.TemplateSpawnRules a, global::ProcGen.World.TemplateSpawnRules b) => b.priority.CompareTo(a.priority));
			HashSet<string> hashSet = new HashSet<string>();
			foreach (global::ProcGen.World.TemplateSpawnRules templateSpawnRules in list)
			{
				successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, TemplateSpawning.ProgressPercent((float)num++ / num2), WorldGenProgressStages.Stages.PlaceTemplates);
				int i = 0;
				while (i < templateSpawnRules.times)
				{
					ListPool<string, TemplateSpawning>.PooledList pooledList = ListPool<string, TemplateSpawning>.Allocate();
					if (!templateSpawnRules.allowDuplicates)
					{
						using (List<string>.Enumerator enumerator3 = templateSpawnRules.names.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								string text = enumerator3.Current;
								if (!hashSet.Contains(text))
								{
									if (!TemplateCache.TemplateExists(text))
									{
										DebugUtil.DevLogError(string.Concat(new string[]
										{
											"TemplateSpawning: Missing template '",
											text,
											"' in world '",
											settings.world.filePath,
											"'"
										}));
									}
									else
									{
										pooledList.Add(text);
									}
								}
							}
							goto IL_01A8;
						}
						goto IL_019A;
					}
					goto IL_019A;
					IL_01A8:
					pooledList.ShuffleSeeded<string>(myRandom.RandomSource());
					if (pooledList.Count == 0)
					{
						pooledList.Recycle();
					}
					else
					{
						int num3 = 0;
						int num4 = 0;
						switch (templateSpawnRules.listRule)
						{
						case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeOne:
							num3 = 1;
							num4 = 1;
							break;
						case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSome:
							num3 = templateSpawnRules.someCount;
							num4 = templateSpawnRules.someCount;
							break;
						case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSomeTryMore:
							num3 = templateSpawnRules.someCount;
							num4 = templateSpawnRules.someCount + templateSpawnRules.moreCount;
							break;
						case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll:
							num3 = pooledList.Count;
							num4 = pooledList.Count;
							break;
						case global::ProcGen.World.TemplateSpawnRules.ListRule.TryOne:
							num4 = 1;
							break;
						case global::ProcGen.World.TemplateSpawnRules.ListRule.TrySome:
							num4 = templateSpawnRules.someCount;
							break;
						case global::ProcGen.World.TemplateSpawnRules.ListRule.TryAll:
							num4 = pooledList.Count;
							break;
						}
						string text2 = "";
						foreach (string text3 in pooledList)
						{
							if (num4 <= 0)
							{
								break;
							}
							bool flag = num3 > 0;
							if (TemplateSpawning.FindTargetForTemplate(text3, templateSpawnRules, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, flag, settings))
							{
								hashSet.Add(text3);
								num4--;
								num3--;
							}
							else
							{
								text2 = text2 + "\n    - " + text3;
							}
						}
						if (num3 > 0)
						{
							string text4 = string.Join(", ", settings.GetTraitIDs());
							string text5 = string.Concat(new string[]
							{
								"TemplateSpawning: Guaranteed placement failiure on ",
								settings.world.filePath,
								"\n",
								string.Format("    listRule={0} someCount={1} moreCount={2} count={3}\n", new object[] { templateSpawnRules.listRule, templateSpawnRules.someCount, templateSpawnRules.moreCount, pooledList.Count }),
								"    Could not place templates:",
								text2,
								"\n    world traits=",
								text4
							});
							DebugUtil.LogErrorArgs(new object[] { text5 });
							if (!isRunningDebugGen)
							{
								throw new Exception(text5);
							}
						}
						pooledList.Recycle();
					}
					i++;
					continue;
					IL_019A:
					pooledList.AddRange(templateSpawnRules.names);
					goto IL_01A8;
				}
			}
		}

		private static bool FindTargetForTemplate(string template, global::ProcGen.World.TemplateSpawnRules rule, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool guarantee, WorldGenSettings settings)
		{
			TemplateContainer template2 = TemplateCache.GetTemplate(template);
			if (template2 == null)
			{
				return false;
			}
			List<TerrainCell> list;
			if (!rule.useRelaxedFiltering)
			{
				list = terrainCells.FindAll(delegate(TerrainCell tc)
				{
					tc.LogInfo("Filtering", template, 0f);
					return tc.IsSafeToSpawnPOI(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
				});
			}
			else
			{
				list = terrainCells.FindAll(delegate(TerrainCell tc)
				{
					tc.LogInfo("Filtering Relaxed (allowReplace)", template, 0f);
					return tc.IsSafeToSpawnPOIRelaxed(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
				});
			}
			TemplateSpawning.RemoveOverlappingPOIs(ref list, ref terrainCells, ref placedPOIBounds, template2, settings, rule.allowExtremeTemperatureOverlap, rule.overrideOffset);
			if (list.Count == 0)
			{
				if (guarantee && !rule.useRelaxedFiltering)
				{
					DebugUtil.LogWarningArgs(new object[] { "Could not place " + template + " using normal rules, trying relaxed" });
					list = terrainCells.FindAll(delegate(TerrainCell tc)
					{
						tc.LogInfo("Filtering Relaxed", template, 0f);
						return tc.IsSafeToSpawnPOIRelaxed(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
					});
					TemplateSpawning.RemoveOverlappingPOIs(ref list, ref terrainCells, ref placedPOIBounds, template2, settings, rule.allowExtremeTemperatureOverlap, rule.overrideOffset);
				}
				if (list.Count == 0)
				{
					return false;
				}
			}
			list.ShuffleSeeded<TerrainCell>(myRandom.RandomSource());
			TerrainCell terrainCell = list[list.Count - 1];
			KeyValuePair<Vector2I, TemplateContainer> keyValuePair = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x + rule.overrideOffset.x, (int)terrainCell.poly.Centroid().y + rule.overrideOffset.y), template2);
			templateSpawnTargets.Add(keyValuePair);
			placedPOIBounds.Add(template2.GetTemplateBounds(keyValuePair.Key, TemplateSpawning.m_poiPadding));
			terrainCell.node.templateTag = template.ToTag();
			terrainCell.node.tags.Add(template.ToTag());
			terrainCell.node.tags.Add(WorldGenTags.POI);
			return true;
		}

		private static bool IsPOIOverlappingBounds(List<RectInt> placedPOIBounds, RectInt templateBounds)
		{
			foreach (RectInt rectInt in placedPOIBounds)
			{
				if (templateBounds.Overlaps(rectInt))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsPOIOverlappingHighTemperatureDelta(RectInt paddedTemplateBounds, SubWorld subworld, ref List<TerrainCell> allCells, WorldGenSettings settings)
		{
			Vector2 vector = 2f * Vector2.one * (float)TemplateSpawning.m_poiPadding;
			Vector2 vector2 = 2f * Vector2.one * 3f;
			Rect rect = new Rect(paddedTemplateBounds.position, paddedTemplateBounds.size - vector + vector2);
			Temperature temperature = SettingsCache.temperatures[subworld.temperatureRange];
			for (int i = 0; i < allCells.Count; i++)
			{
				TerrainCell terrainCell = allCells[i];
				SubWorld subWorld = settings.GetSubWorld(terrainCell.node.GetSubworld());
				Temperature temperature2 = SettingsCache.temperatures[subWorld.temperatureRange];
				if (subWorld.temperatureRange != subworld.temperatureRange)
				{
					float num = Mathf.Min(temperature.min, temperature2.min);
					float num2 = Mathf.Max(temperature.max, temperature2.max) - num;
					bool flag = rect.Overlaps(terrainCell.poly.bounds);
					bool flag2 = num2 > TemplateSpawning.EXTREME_POI_OVERLAP_TEMPERATURE_RANGE;
					if (flag && flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void RemoveOverlappingPOIs(ref List<TerrainCell> filteredTerrainCells, ref List<TerrainCell> allCells, ref List<RectInt> placedPOIBounds, TemplateContainer container, WorldGenSettings settings, bool allowExtremeTemperatureOverlap, Vector2 poiOffset)
		{
			for (int i = filteredTerrainCells.Count - 1; i >= 0; i--)
			{
				TerrainCell terrainCell = filteredTerrainCells[i];
				int num = i;
				SubWorld subWorld = settings.GetSubWorld(terrainCell.node.GetSubworld());
				RectInt templateBounds = container.GetTemplateBounds(terrainCell.poly.Centroid() + poiOffset, TemplateSpawning.m_poiPadding);
				bool flag = false;
				if (TemplateSpawning.IsPOIOverlappingBounds(placedPOIBounds, templateBounds))
				{
					terrainCell.LogInfo("-> Removed due to overlapping POIs", "", 0f);
					flag = true;
				}
				else if (!allowExtremeTemperatureOverlap && TemplateSpawning.IsPOIOverlappingHighTemperatureDelta(templateBounds, subWorld, ref allCells, settings))
				{
					terrainCell.LogInfo("-> Removed due to overlapping extreme temperature delta", "", 0f);
					flag = true;
				}
				if (flag)
				{
					filteredTerrainCells.RemoveAt(num);
				}
			}
		}

		private static bool DoesCellMatchFilters(TerrainCell cell, List<global::ProcGen.World.AllowedCellsFilter> filters)
		{
			bool flag = false;
			foreach (global::ProcGen.World.AllowedCellsFilter allowedCellsFilter in filters)
			{
				bool flag2 = TemplateSpawning.DoesCellMatchFilter(cell, allowedCellsFilter);
				switch (allowedCellsFilter.command)
				{
				case global::ProcGen.World.AllowedCellsFilter.Command.Clear:
					flag = false;
					break;
				case global::ProcGen.World.AllowedCellsFilter.Command.Replace:
					flag = flag2;
					break;
				case global::ProcGen.World.AllowedCellsFilter.Command.UnionWith:
					flag = flag2 || flag;
					break;
				case global::ProcGen.World.AllowedCellsFilter.Command.IntersectWith:
					flag = flag2 && flag;
					break;
				case global::ProcGen.World.AllowedCellsFilter.Command.ExceptWith:
				case global::ProcGen.World.AllowedCellsFilter.Command.SymmetricExceptWith:
					if (flag2)
					{
						flag = false;
					}
					break;
				case global::ProcGen.World.AllowedCellsFilter.Command.All:
					flag = true;
					break;
				}
				cell.LogInfo("-> DoesCellMatchFilter " + allowedCellsFilter.command.ToString(), flag2 ? "1" : "0", (float)(flag ? 1 : 0));
			}
			cell.LogInfo("> Final match", flag ? "true" : "false", 0f);
			return flag;
		}

		private static bool DoesCellMatchFilter(TerrainCell cell, global::ProcGen.World.AllowedCellsFilter filter)
		{
			if (!TemplateSpawning.ValidateFilter(filter))
			{
				return false;
			}
			if (filter.tagcommand == global::ProcGen.World.AllowedCellsFilter.TagCommand.Default)
			{
				if (filter.subworldNames != null && filter.subworldNames.Count > 0)
				{
					foreach (string text in filter.subworldNames)
					{
						if (cell.node.tags.Contains(text))
						{
							return true;
						}
					}
					return false;
				}
				if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
				{
					foreach (SubWorld.ZoneType zoneType in filter.zoneTypes)
					{
						if (cell.node.tags.Contains(zoneType.ToString()))
						{
							return true;
						}
					}
					return false;
				}
				if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
				{
					foreach (Temperature.Range range in filter.temperatureRanges)
					{
						if (cell.node.tags.Contains(range.ToString()))
						{
							return true;
						}
					}
					return false;
				}
				return true;
			}
			switch (filter.tagcommand)
			{
			case global::ProcGen.World.AllowedCellsFilter.TagCommand.Default:
				return true;
			case global::ProcGen.World.AllowedCellsFilter.TagCommand.AtTag:
				return cell.node.tags.Contains(filter.tag);
			case global::ProcGen.World.AllowedCellsFilter.TagCommand.NotAtTag:
				return !cell.node.tags.Contains(filter.tag);
			case global::ProcGen.World.AllowedCellsFilter.TagCommand.DistanceFromTag:
			{
				int num = cell.DistanceToTag(filter.tag);
				return num >= filter.minDistance && num <= filter.maxDistance;
			}
			}
			return true;
		}

		private static bool ValidateFilter(global::ProcGen.World.AllowedCellsFilter filter)
		{
			if (filter.command == global::ProcGen.World.AllowedCellsFilter.Command.All)
			{
				return true;
			}
			int num = 0;
			if (filter.tagcommand != global::ProcGen.World.AllowedCellsFilter.TagCommand.Default)
			{
				num++;
			}
			if (filter.subworldNames != null && filter.subworldNames.Count > 0)
			{
				num++;
			}
			if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
			{
				num++;
			}
			if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
			{
				num++;
			}
			if (num != 1)
			{
				string text = "BAD ALLOWED CELLS FILTER in FEATURE RULES!";
				text += "\nA filter can only specify one of `tagcommand`, `subworldNames`, `zoneTypes`, or `temperatureRanges`.";
				text += "\nFound a filter with the following:";
				if (filter.tagcommand != global::ProcGen.World.AllowedCellsFilter.TagCommand.Default)
				{
					text += "\ntagcommand:\n\t";
					text += filter.tagcommand.ToString();
					text += "\ntag:\n\t";
					text += filter.tag;
				}
				if (filter.subworldNames != null && filter.subworldNames.Count > 0)
				{
					text += "\nsubworldNames:\n\t";
					text += string.Join(", ", filter.subworldNames);
				}
				if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
				{
					text += "\nzoneTypes:\n";
					text += string.Join<SubWorld.ZoneType>(", ", filter.zoneTypes);
				}
				if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
				{
					text += "\ntemperatureRanges:\n";
					text += string.Join<Temperature.Range>(", ", filter.temperatureRanges);
				}
				global::Debug.LogError(text);
				return false;
			}
			return true;
		}

		private static float minProgressPercent;

		private static float maxProgressPercent;

		private static int m_poiPadding;

		private const int TEMPERATURE_PADDING = 3;

		private static float EXTREME_POI_OVERLAP_TEMPERATURE_RANGE = 100f;
	}
}
