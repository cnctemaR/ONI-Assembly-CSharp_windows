using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace ProcGenGame
{
	public class TemplateSpawning
	{
		public static List<KeyValuePair<Vector2I, TemplateContainer>> DetermineTemplatesForWorld(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, ref List<WorldTrait> placedStoryTraits, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 0f, WorldGenProgressStages.Stages.PlaceTemplates);
			List<KeyValuePair<Vector2I, TemplateContainer>> list = new List<KeyValuePair<Vector2I, TemplateContainer>>();
			TemplateSpawning.s_poiPadding = settings.GetIntSetting("POIPadding");
			TemplateSpawning.s_minProgressPercent = 0f;
			TemplateSpawning.s_maxProgressPercent = 0.33f;
			TemplateSpawning.SpawnStartingTemplate(settings, terrainCells, ref list, ref placedPOIBounds, isRunningDebugGen, successCallbackFn);
			TemplateSpawning.s_minProgressPercent = TemplateSpawning.s_maxProgressPercent;
			TemplateSpawning.s_maxProgressPercent = 0.66f;
			TemplateSpawning.SpawnTemplatesFromTemplateRules(settings, terrainCells, myRandom, ref list, ref placedPOIBounds, isRunningDebugGen, successCallbackFn);
			TemplateSpawning.s_minProgressPercent = TemplateSpawning.s_maxProgressPercent;
			TemplateSpawning.s_maxProgressPercent = 1f;
			TemplateSpawning.SpawnStoryTraitTemplates(settings, terrainCells, myRandom, ref list, ref placedPOIBounds, ref placedStoryTraits, isRunningDebugGen, successCallbackFn);
			successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 1f, WorldGenProgressStages.Stages.PlaceTemplates);
			return list;
		}

		private static float ProgressPercent(float stagePercent)
		{
			return MathUtil.ReRange(stagePercent, 0f, 1f, TemplateSpawning.s_minProgressPercent, TemplateSpawning.s_maxProgressPercent);
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
			RectInt templateBounds = template.GetTemplateBounds(keyValuePair.Key, TemplateSpawning.s_poiPadding);
			if (TemplateSpawning.IsPOIOverlappingBounds(placedPOIBounds, templateBounds))
			{
				string text = "TemplateSpawning: Starting template overlaps world boundaries in world '" + settings.world.filePath + "'";
				DebugUtil.DevLogError(text);
				if (!isRunningDebugGen)
				{
					throw new Exception(text);
				}
			}
			successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, TemplateSpawning.ProgressPercent(1f), WorldGenProgressStages.Stages.PlaceTemplates);
			templateSpawnTargets.Add(keyValuePair);
			placedPOIBounds.Add(templateBounds);
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
				string text;
				List<KeyValuePair<Vector2I, TemplateContainer>> list2;
				if (!TemplateSpawning.ApplyTemplateRule(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, templateSpawnRules, ref hashSet, out text, out list2))
				{
					DebugUtil.LogErrorArgs(new object[] { text });
					if (!isRunningDebugGen)
					{
						throw new TemplateSpawningException(text, UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE);
					}
				}
			}
		}

		private static void SpawnStoryTraitTemplates(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, ref List<WorldTrait> placedStoryTraits, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			Queue<WorldTrait> queue = new Queue<WorldTrait>(settings.GetStoryTraitCandiates());
			int count = queue.Count;
			List<WorldTrait> list = new List<WorldTrait>();
			HashSet<string> hashSet = new HashSet<string>();
			while (queue.Count > 0 && list.Count < count)
			{
				WorldTrait worldTrait = queue.Dequeue();
				bool flag = false;
				List<KeyValuePair<Vector2I, TemplateContainer>> list2 = new List<KeyValuePair<Vector2I, TemplateContainer>>();
				string text = "";
				List<global::ProcGen.World.TemplateSpawnRules> list3 = new List<global::ProcGen.World.TemplateSpawnRules>();
				list3.AddRange(worldTrait.additionalWorldTemplateRules);
				list3.Sort((global::ProcGen.World.TemplateSpawnRules a, global::ProcGen.World.TemplateSpawnRules b) => b.priority.CompareTo(a.priority));
				foreach (global::ProcGen.World.TemplateSpawnRules templateSpawnRules in list3)
				{
					flag = TemplateSpawning.ApplyTemplateRule(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, templateSpawnRules, ref hashSet, out text, out list2);
					if (!flag)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					placedStoryTraits.Add(worldTrait);
					list.Add(worldTrait);
					settings.ApplyStoryTrait(worldTrait);
					DebugUtil.LogArgs(new object[] { "Applied story trait '" + worldTrait.filePath + "'" });
				}
				else
				{
					using (List<KeyValuePair<Vector2I, TemplateContainer>>.Enumerator enumerator2 = list2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<Vector2I, TemplateContainer> partialTemplate = enumerator2.Current;
							templateSpawnTargets.RemoveAll((KeyValuePair<Vector2I, TemplateContainer> x) => x.Key == partialTemplate.Key);
							hashSet.Remove(partialTemplate.Value.name);
							placedPOIBounds.RemoveAll((RectInt bound) => bound.center == partialTemplate.Key);
						}
					}
					DebugUtil.LogArgs(new object[] { string.Concat(new string[] { "Cannot place story trait '", worldTrait.filePath, "' error='", text, "'" }) });
				}
			}
		}

		private static bool ApplyTemplateRule(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, global::ProcGen.World.TemplateSpawnRules rule, ref HashSet<string> usedTemplates, out string errorMessage, out List<KeyValuePair<Vector2I, TemplateContainer>> newTemplateSpawnTargets)
		{
			newTemplateSpawnTargets = new List<KeyValuePair<Vector2I, TemplateContainer>>();
			int i = 0;
			while (i < rule.times)
			{
				ListPool<string, TemplateSpawning>.PooledList pooledList = ListPool<string, TemplateSpawning>.Allocate();
				if (!rule.allowDuplicates)
				{
					using (List<string>.Enumerator enumerator = rule.names.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string text = enumerator.Current;
							if (!usedTemplates.Contains(text))
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
						goto IL_00BA;
					}
					goto IL_00AD;
				}
				goto IL_00AD;
				IL_00BA:
				pooledList.ShuffleSeeded<string>(myRandom.RandomSource());
				if (pooledList.Count == 0)
				{
					pooledList.Recycle();
				}
				else
				{
					int num = 0;
					int num2 = 0;
					switch (rule.listRule)
					{
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeOne:
						num = 1;
						num2 = 1;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSome:
						num = rule.someCount;
						num2 = rule.someCount;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSomeTryMore:
						num = rule.someCount;
						num2 = rule.someCount + rule.moreCount;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll:
						num = pooledList.Count;
						num2 = pooledList.Count;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.TryOne:
						num2 = 1;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.TrySome:
						num2 = rule.someCount;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.TryAll:
						num2 = pooledList.Count;
						break;
					}
					string text2 = "";
					foreach (string text3 in pooledList)
					{
						if (num2 <= 0)
						{
							break;
						}
						TemplateContainer template = TemplateCache.GetTemplate(text3);
						if (template != null)
						{
							bool flag = num > 0;
							TerrainCell terrainCell = TemplateSpawning.FindTargetForTemplate(template, rule, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, flag, settings);
							if (terrainCell != null)
							{
								KeyValuePair<Vector2I, TemplateContainer> keyValuePair = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x + rule.overrideOffset.x, (int)terrainCell.poly.Centroid().y + rule.overrideOffset.y), template);
								templateSpawnTargets.Add(keyValuePair);
								newTemplateSpawnTargets.Add(keyValuePair);
								placedPOIBounds.Add(template.GetTemplateBounds(keyValuePair.Key, TemplateSpawning.s_poiPadding));
								terrainCell.node.templateTag = text3.ToTag();
								terrainCell.node.tags.Add(text3.ToTag());
								terrainCell.node.tags.Add(WorldGenTags.POI);
								usedTemplates.Add(text3);
								num2--;
								num--;
							}
							else
							{
								text2 = text2 + "\n    - " + text3;
							}
						}
					}
					pooledList.Recycle();
					if (num > 0)
					{
						string text4 = string.Join(", ", settings.GetWorldTraitIDs());
						string text5 = string.Join(", ", settings.GetStoryTraitIDs());
						errorMessage = string.Concat(new string[]
						{
							"TemplateSpawning: Guaranteed placement failure on ",
							settings.world.filePath,
							"\n",
							string.Format("    listRule={0} someCount={1} moreCount={2} count={3}\n", new object[] { rule.listRule, rule.someCount, rule.moreCount, pooledList.Count }),
							"    Could not place templates:",
							text2,
							"\n    world traits=",
							text4,
							"\n    story traits=",
							text5
						});
						return false;
					}
				}
				i++;
				continue;
				IL_00AD:
				pooledList.AddRange(rule.names);
				goto IL_00BA;
			}
			errorMessage = "";
			return true;
		}

		private static TerrainCell FindTargetForTemplate(TemplateContainer template, global::ProcGen.World.TemplateSpawnRules rule, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool guarantee, WorldGenSettings settings)
		{
			List<TerrainCell> list;
			if (!rule.useRelaxedFiltering)
			{
				list = terrainCells.FindAll(delegate(TerrainCell tc)
				{
					tc.LogInfo("Filtering", template.name, 0f);
					return tc.IsSafeToSpawnPOI(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
				});
			}
			else
			{
				list = terrainCells.FindAll(delegate(TerrainCell tc)
				{
					tc.LogInfo("Filtering Relaxed (replace features)", template.name, 0f);
					return tc.IsSafeToSpawnPOIRelaxed(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
				});
			}
			TemplateSpawning.RemoveOverlappingPOIs(ref list, ref terrainCells, ref placedPOIBounds, template, settings, rule.allowExtremeTemperatureOverlap, rule.overrideOffset);
			if (list.Count == 0)
			{
				if (guarantee && !rule.useRelaxedFiltering)
				{
					DebugUtil.LogWarningArgs(new object[] { "Could not place " + template.name + " using normal rules, trying relaxed" });
					list = terrainCells.FindAll(delegate(TerrainCell tc)
					{
						tc.LogInfo("Filtering Relaxed", template.name, 0f);
						return tc.IsSafeToSpawnPOIRelaxed(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
					});
					TemplateSpawning.RemoveOverlappingPOIs(ref list, ref terrainCells, ref placedPOIBounds, template, settings, rule.allowExtremeTemperatureOverlap, rule.overrideOffset);
				}
				if (list.Count == 0)
				{
					return null;
				}
			}
			list.ShuffleSeeded<TerrainCell>(myRandom.RandomSource());
			return list[list.Count - 1];
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
			Vector2 vector = 2f * Vector2.one * (float)TemplateSpawning.s_poiPadding;
			Vector2 vector2 = 2f * Vector2.one * 3f;
			Rect rect = new Rect(paddedTemplateBounds.position, paddedTemplateBounds.size - vector + vector2);
			Temperature temperature = SettingsCache.temperatures[subworld.temperatureRange];
			foreach (TerrainCell terrainCell in allCells)
			{
				SubWorld subWorld = settings.GetSubWorld(terrainCell.node.GetSubworld());
				Temperature temperature2 = SettingsCache.temperatures[subWorld.temperatureRange];
				if (subWorld.temperatureRange != subworld.temperatureRange)
				{
					float num = Mathf.Min(temperature.min, temperature2.min);
					float num2 = Mathf.Max(temperature.max, temperature2.max) - num;
					bool flag = rect.Overlaps(terrainCell.poly.bounds);
					bool flag2 = num2 > 100f;
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
				RectInt templateBounds = container.GetTemplateBounds(terrainCell.poly.Centroid() + poiOffset, TemplateSpawning.s_poiPadding);
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
				bool flag3;
				bool flag2 = TemplateSpawning.DoesCellMatchFilter(cell, allowedCellsFilter, out flag3);
				if (flag3)
				{
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
			}
			cell.LogInfo("> Final match", flag ? "true" : "false", 0f);
			return flag;
		}

		private static bool DoesCellMatchFilter(TerrainCell cell, global::ProcGen.World.AllowedCellsFilter filter, out bool applied)
		{
			applied = true;
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
				bool flag = cell.distancesToTags.ContainsKey(filter.tag.ToTag());
				global::Debug.Assert(flag || filter.optional, "DistanceFromTag is missing tag " + filter.tag + ", consider marking the filter optional.");
				if (flag)
				{
					int num = cell.DistanceToTag(filter.tag);
					return num >= filter.minDistance && num <= filter.maxDistance;
				}
				applied = false;
				return true;
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

		private static float s_minProgressPercent;

		private static float s_maxProgressPercent;

		private static int s_poiPadding;

		private const int TEMPERATURE_PADDING = 3;

		private const float EXTREME_POI_OVERLAP_TEMPERATURE_RANGE = 100f;
	}
}
