using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace ProcGenGame
{
	public class TemplateSpawning
	{
		public static List<TemplateSpawning.TemplateSpawner> DetermineTemplatesForWorld(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, ref List<WorldTrait> placedStoryTraits, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 0f, WorldGenProgressStages.Stages.PlaceTemplates);
			List<TemplateSpawning.TemplateSpawner> list = new List<TemplateSpawning.TemplateSpawner>();
			TemplateSpawning.s_poiPadding = settings.GetIntSetting("POIPadding");
			TemplateSpawning.ValidateTemplateRules(settings);
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

		private static void SpawnStartingTemplate(WorldGenSettings settings, List<TerrainCell> terrainCells, ref List<TemplateSpawning.TemplateSpawner> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			TerrainCell terrainCell = terrainCells.Find((TerrainCell tc) => tc.node.tags.Contains(WorldGenTags.StartLocation));
			if (settings.world.startingBaseTemplate.IsNullOrWhiteSpace())
			{
				return;
			}
			TemplateContainer template = TemplateCache.GetTemplate(settings.world.startingBaseTemplate);
			Vector2I vector2I = new Vector2I((int)terrainCell.poly.Centroid().x, (int)terrainCell.poly.Centroid().y);
			RectInt templateBounds = template.GetTemplateBounds(vector2I, TemplateSpawning.s_poiPadding);
			TemplateSpawning.TemplateSpawner templateSpawner = new TemplateSpawning.TemplateSpawner(vector2I, templateBounds, template, terrainCell);
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
			templateSpawnTargets.Add(templateSpawner);
			placedPOIBounds.Add(templateBounds);
		}

		private static void SpawnTemplatesFromTemplateRules(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<TemplateSpawning.TemplateSpawner> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
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
			List<TemplateSpawning.TemplateSpawner> list2 = new List<TemplateSpawning.TemplateSpawner>();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (global::ProcGen.World.TemplateSpawnRules templateSpawnRules in list)
			{
				successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, TemplateSpawning.ProgressPercent((float)num++ / num2), WorldGenProgressStages.Stages.PlaceTemplates);
				string text;
				if (!TemplateSpawning.ApplyTemplateRule(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, templateSpawnRules, ref hashSet, out text, ref list2))
				{
					DebugUtil.LogErrorArgs(new object[] { text });
					if (!isRunningDebugGen)
					{
						throw new WorldgenException(text, UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE);
					}
				}
			}
		}

		private static void SpawnStoryTraitTemplates(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<TemplateSpawning.TemplateSpawner> templateSpawnTargets, ref List<RectInt> placedPOIBounds, ref List<WorldTrait> placedStoryTraits, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
		{
			Queue<WorldTrait> queue = new Queue<WorldTrait>(settings.GetStoryTraitCandiates());
			int count = queue.Count;
			List<WorldTrait> list = new List<WorldTrait>();
			HashSet<string> hashSet = new HashSet<string>();
			while (queue.Count > 0 && list.Count < count)
			{
				WorldTrait worldTrait = queue.Dequeue();
				bool flag = false;
				List<TemplateSpawning.TemplateSpawner> list2 = new List<TemplateSpawning.TemplateSpawner>();
				string text = "";
				List<global::ProcGen.World.TemplateSpawnRules> list3 = new List<global::ProcGen.World.TemplateSpawnRules>();
				list3.AddRange(worldTrait.additionalWorldTemplateRules);
				list3.Sort((global::ProcGen.World.TemplateSpawnRules a, global::ProcGen.World.TemplateSpawnRules b) => b.priority.CompareTo(a.priority));
				foreach (global::ProcGen.World.TemplateSpawnRules templateSpawnRules in list3)
				{
					flag = TemplateSpawning.ApplyTemplateRule(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, templateSpawnRules, ref hashSet, out text, ref list2);
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
					foreach (TemplateSpawning.TemplateSpawner templateSpawner in list2)
					{
						TemplateSpawning.RemoveTemplate(templateSpawner, settings, terrainCells, ref templateSpawnTargets, ref placedPOIBounds);
						hashSet.Remove(templateSpawner.container.name);
					}
					if (DlcManager.FeatureClusterSpaceEnabled())
					{
						DebugUtil.LogArgs(new object[] { string.Concat(new string[] { "Cannot place story trait on '", worldTrait.filePath, "' and will try another world. error='", text, "'." }) });
					}
					else
					{
						DebugUtil.LogArgs(new object[] { string.Concat(new string[] { "Cannot place story trait '", worldTrait.filePath, "' error='", text, "'" }) });
					}
				}
			}
		}

		private static void RemoveTemplate(TemplateSpawning.TemplateSpawner toRemove, WorldGenSettings settings, List<TerrainCell> terrainCells, ref List<TemplateSpawning.TemplateSpawner> templateSpawnTargets, ref List<RectInt> placedPOIBounds)
		{
			TemplateSpawning.UpdateNodeTags(toRemove.terrainCell.node, toRemove.container.name, true);
			templateSpawnTargets.Remove(toRemove);
			placedPOIBounds.RemoveAll((RectInt bound) => bound.center == toRemove.position);
		}

		private static void ValidateTemplateRules(List<global::ProcGen.World.TemplateSpawnRules> rules, string context)
		{
			foreach (global::ProcGen.World.TemplateSpawnRules templateSpawnRules in rules)
			{
				foreach (string text in templateSpawnRules.names)
				{
					DebugUtil.DevAssert(TemplateCache.TemplateExists(text), string.Format(string.Concat(new string[] { "TemplateSpawning: Missing template '", text, "' in '", context, "'" }), Array.Empty<object>()), null);
				}
			}
		}

		private static void ValidateTemplateRules(WorldGenSettings settings)
		{
			TemplateSpawning.ValidateTemplateRules(settings.world.worldTemplateRules, settings.world.filePath);
			foreach (WeightedSubworldName weightedSubworldName in settings.world.subworldFiles)
			{
				SubWorld subWorld = settings.GetSubWorld(weightedSubworldName.name);
				if (subWorld.subworldTemplateRules != null)
				{
					TemplateSpawning.ValidateTemplateRules(subWorld.subworldTemplateRules, subWorld.name);
				}
			}
			foreach (WorldTrait worldTrait in settings.GetStoryTraitCandiates())
			{
				TemplateSpawning.ValidateTemplateRules(worldTrait.additionalWorldTemplateRules, "Story Trait Candidates");
			}
		}

		private static bool ApplyTemplateRule(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<TemplateSpawning.TemplateSpawner> templateSpawnTargets, ref List<RectInt> placedPOIBounds, global::ProcGen.World.TemplateSpawnRules rule, ref HashSet<string> usedTemplates, out string errorMessage, ref List<TemplateSpawning.TemplateSpawner> newTemplateSpawnTargets)
		{
			TemplateSpawning.<>c__DisplayClass14_0 CS$<>8__locals1 = new TemplateSpawning.<>c__DisplayClass14_0();
			CS$<>8__locals1.rule = rule;
			CS$<>8__locals1.candidates = ListPool<string, TemplateSpawning>.Allocate();
			for (int i = 0; i < CS$<>8__locals1.rule.times; i++)
			{
				CS$<>8__locals1.candidates.Clear();
				foreach (string text in CS$<>8__locals1.rule.names)
				{
					if ((CS$<>8__locals1.rule.allowDuplicates || !usedTemplates.Contains(text)) && TemplateCache.TemplateExists(text))
					{
						CS$<>8__locals1.candidates.Add(text);
					}
				}
				CS$<>8__locals1.candidates.ShuffleSeeded<string>(myRandom.RandomSource());
				if (CS$<>8__locals1.candidates.Count != 0)
				{
					int num = 0;
					if (CS$<>8__locals1.rule.listRule == global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeRange || CS$<>8__locals1.rule.listRule == global::ProcGen.World.TemplateSpawnRules.ListRule.TryRange)
					{
						num = myRandom.RandomRange(CS$<>8__locals1.rule.range.x, CS$<>8__locals1.rule.range.y);
					}
					int num2 = 0;
					int num3 = 0;
					switch (CS$<>8__locals1.rule.listRule)
					{
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeOne:
						num2 = 1;
						num3 = 1;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSome:
						num2 = CS$<>8__locals1.rule.someCount;
						num3 = CS$<>8__locals1.rule.someCount;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSomeTryMore:
						num2 = CS$<>8__locals1.rule.someCount;
						num3 = CS$<>8__locals1.rule.someCount + CS$<>8__locals1.rule.moreCount;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll:
						num2 = CS$<>8__locals1.candidates.Count;
						num3 = CS$<>8__locals1.candidates.Count;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeRange:
						num2 = num;
						num3 = num;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.TryOne:
						num3 = 1;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.TrySome:
						num3 = CS$<>8__locals1.rule.someCount;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.TryRange:
						num3 = num;
						break;
					case global::ProcGen.World.TemplateSpawnRules.ListRule.TryAll:
						num3 = CS$<>8__locals1.candidates.Count;
						break;
					}
					string text2 = "";
					foreach (string text3 in CS$<>8__locals1.candidates)
					{
						if (num3 <= 0)
						{
							break;
						}
						TemplateContainer template = TemplateCache.GetTemplate(text3);
						if (template != null)
						{
							bool flag = num2 > 0;
							Vector2I vector2I = Vector2I.zero;
							TerrainCell terrainCell;
							if (CS$<>8__locals1.rule.overridePlacement != Vector2I.minusone)
							{
								vector2I = CS$<>8__locals1.rule.overridePlacement;
								Predicate<TerrainCell> predicate;
								if ((predicate = CS$<>8__locals1.<>9__1) == null)
								{
									predicate = (CS$<>8__locals1.<>9__1 = (TerrainCell x) => x.poly.Contains(CS$<>8__locals1.rule.overridePlacement));
								}
								terrainCell = terrainCells.Find(predicate);
								if (num2 > 0 && terrainCell.node.templateTag != Tag.Invalid)
								{
									errorMessage = string.Format("Tried to place '{0}' at ({1},{2}) using overridePlacement but '{3}' is already there.", new object[]
									{
										text3,
										vector2I.x,
										vector2I.y,
										terrainCell.node.templateTag
									});
									return CS$<>8__locals1.<ApplyTemplateRule>g__ExitFn|0(false);
								}
							}
							else
							{
								terrainCell = TemplateSpawning.FindTargetForTemplate(template, CS$<>8__locals1.rule, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, flag, settings);
								if (terrainCell != null)
								{
									vector2I = new Vector2I((int)terrainCell.poly.Centroid().x + CS$<>8__locals1.rule.overrideOffset.x, (int)terrainCell.poly.Centroid().y + CS$<>8__locals1.rule.overrideOffset.y);
								}
							}
							if (terrainCell != null)
							{
								RectInt templateBounds = template.GetTemplateBounds(vector2I, TemplateSpawning.s_poiPadding);
								TemplateSpawning.TemplateSpawner templateSpawner = new TemplateSpawning.TemplateSpawner(vector2I, templateBounds, template, terrainCell);
								templateSpawnTargets.Add(templateSpawner);
								newTemplateSpawnTargets.Add(templateSpawner);
								placedPOIBounds.Add(templateBounds);
								TemplateSpawning.UpdateNodeTags(terrainCell.node, text3, false);
								usedTemplates.Add(text3);
								num3--;
								num2--;
							}
							else
							{
								text2 = text2 + "\n    - " + text3;
							}
						}
					}
					if (num2 > 0)
					{
						string text4 = string.Join(", ", settings.GetWorldTraitIDs());
						string text5 = string.Join(", ", settings.GetStoryTraitIDs());
						errorMessage = string.Concat(new string[]
						{
							"TemplateSpawning: Guaranteed placement failure on ",
							settings.world.filePath,
							"\n",
							string.Format("    listRule={0} someCount={1} moreCount={2} count={3}\n", new object[]
							{
								CS$<>8__locals1.rule.listRule,
								CS$<>8__locals1.rule.someCount,
								CS$<>8__locals1.rule.moreCount,
								CS$<>8__locals1.candidates.Count
							}),
							"    Could not place templates:",
							text2,
							"\n    world traits=",
							text4,
							"\n    story traits=",
							text5
						});
						return CS$<>8__locals1.<ApplyTemplateRule>g__ExitFn|0(false);
					}
				}
				IL_04BD:;
			}
			errorMessage = "";
			return CS$<>8__locals1.<ApplyTemplateRule>g__ExitFn|0(true);
		}

		private static void UpdateNodeTags(Node node, string template, bool remove = false)
		{
			Tag tag = template.ToTag();
			if (remove)
			{
				node.templateTag = Tag.Invalid;
				node.tags.Remove(tag);
				node.tags.Remove(WorldGenTags.POI);
				return;
			}
			node.templateTag = tag;
			node.tags.Add(template.ToTag());
			node.tags.Add(WorldGenTags.POI);
		}

		private static TerrainCell FindTargetForTemplate(TemplateContainer template, global::ProcGen.World.TemplateSpawnRules rule, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<TemplateSpawning.TemplateSpawner> templateSpawnTargets, ref List<RectInt> placedPOIBounds, bool guarantee, WorldGenSettings settings)
		{
			List<TerrainCell> list;
			if (rule.allowNearStart)
			{
				list = terrainCells.FindAll(delegate(TerrainCell tc)
				{
					tc.LogInfo("Filtering Near Start", template.name, 0f);
					return tc.IsSafeToSpawnPOINearStart(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
				});
			}
			else if (!rule.useRelaxedFiltering)
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
			if (list.Count == 0 && guarantee)
			{
				if (rule.allowNearStart && rule.useRelaxedFiltering)
				{
					DebugUtil.LogWarningArgs(new object[] { "Could not place " + template.name + " using normal rules, trying relaxed near start" });
					list = terrainCells.FindAll(delegate(TerrainCell tc)
					{
						tc.LogInfo("Filtering Near Start Relaxed", template.name, 0f);
						return tc.IsSafeToSpawnPOINearStartRelaxed(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
					});
					TemplateSpawning.RemoveOverlappingPOIs(ref list, ref terrainCells, ref placedPOIBounds, template, settings, rule.allowExtremeTemperatureOverlap, rule.overrideOffset);
				}
				else if (!rule.useRelaxedFiltering)
				{
					DebugUtil.LogWarningArgs(new object[] { "Could not place " + template.name + " using normal rules, trying relaxed" });
					list = terrainCells.FindAll(delegate(TerrainCell tc)
					{
						tc.LogInfo("Filtering Relaxed", template.name, 0f);
						return tc.IsSafeToSpawnPOIRelaxed(terrainCells, true) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
					});
					TemplateSpawning.RemoveOverlappingPOIs(ref list, ref terrainCells, ref placedPOIBounds, template, settings, rule.allowExtremeTemperatureOverlap, rule.overrideOffset);
				}
			}
			if (list.Count == 0)
			{
				return null;
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

		public static bool DoesCellMatchFilters(TerrainCell cell, List<global::ProcGen.World.AllowedCellsFilter> filters)
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
				Tag tag = filter.tag.ToTag();
				bool flag = cell.distancesToTags.ContainsKey(tag);
				if (!flag && tag == WorldGenTags.AtStart && !filter.ignoreIfMissingTag)
				{
					DebugUtil.DevLogError("DistanceFromTag was used on a world without an AtStart tag, use ignoreIfMissingTag to skip it.");
				}
				else
				{
					global::Debug.Assert(flag || filter.ignoreIfMissingTag, "DistanceFromTag is missing tag " + filter.tag + ", use ignoreIfMissingTag to skip it.");
				}
				if (flag)
				{
					int num = cell.DistanceToTag(tag);
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

		public class TemplateSpawner
		{
			public TemplateSpawner(Vector2I position, RectInt bounds, TemplateContainer container, TerrainCell terrainCell)
			{
				this.position = position;
				this.container = container;
				this.terrainCell = terrainCell;
				this.bounds = bounds;
			}

			public Vector2I position;

			public TemplateContainer container;

			public TerrainCell terrainCell;

			public RectInt bounds;
		}
	}
}
