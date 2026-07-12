using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei;
using Klei.AI;
using ProcGen;
using ProcGenGame;
using STRINGS;
using TUNING;
using UnityEngine;

public static class CodexEntryGenerator
{
	public static Dictionary<string, CodexEntry> GenerateBuildingEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (PlanScreen.PlanInfo planInfo in global::TUNING.BUILDINGS.PLANORDER)
		{
			CodexEntryGenerator.GenerateEntriesForBuildingsInCategory(planInfo, CodexEntryGenerator.categoryPrefx, ref dictionary);
		}
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			CodexEntryGenerator.GenerateDLC1RocketryEntries();
		}
		CodexEntryGenerator.PopulateCategoryEntries(dictionary);
		return dictionary;
	}

	private static void GenerateEntriesForBuildingsInCategory(PlanScreen.PlanInfo category, string categoryPrefx, ref Dictionary<string, CodexEntry> categoryEntries)
	{
		string text = HashCache.Get().Get(category.category);
		string text2 = CodexCache.FormatLinkID(categoryPrefx + text);
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (KeyValuePair<string, string> keyValuePair in category.buildingAndSubcategoryData)
		{
			CodexEntry codexEntry = CodexEntryGenerator.GenerateSingleBuildingEntry(Assets.GetBuildingDef(keyValuePair.Key), text2);
			if (codexEntry != null)
			{
				dictionary.Add(codexEntry.id, codexEntry);
			}
		}
		if (dictionary.Count == 0)
		{
			return;
		}
		CategoryEntry categoryEntry = CodexEntryGenerator.GenerateCategoryEntry(CodexCache.FormatLinkID(text2), Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text.ToUpper() + ".NAME"), dictionary, null, true, true, null);
		categoryEntry.parentId = "BUILDINGS";
		categoryEntry.category = "BUILDINGS";
		categoryEntry.icon = Assets.GetSprite(PlanScreen.IconNameMap[text]);
		categoryEntries.Add(text2, categoryEntry);
	}

	private static CodexEntry GenerateSingleBuildingEntry(BuildingDef def, string categoryEntryID)
	{
		if (def.DebugOnly)
		{
			return null;
		}
		List<ContentContainer> list = new List<ContentContainer>();
		List<ICodexWidget> list2 = new List<ICodexWidget>();
		list2.Add(new CodexText(def.Name, CodexTextStyle.Title, null));
		Tech tech = Db.Get().Techs.TryGetTechForTechItem(def.PrefabID);
		if (tech != null)
		{
			list2.Add(new CodexLabelWithIcon(tech.Name, CodexTextStyle.Body, new global::Tuple<Sprite, Color>(Assets.GetSprite("research_type_alpha_icon"), Color.white)));
		}
		list2.Add(new CodexDividerLine());
		list.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
		CodexEntryGenerator.GenerateImageContainers(def.GetUISprite("ui", false), list);
		CodexEntryGenerator.GenerateBuildingDescriptionContainers(def, list);
		CodexEntryGenerator.GenerateFabricatorContainers(def.BuildingComplete, list);
		CodexEntryGenerator.GenerateReceptacleContainers(def.BuildingComplete, list);
		CodexEntry codexEntry = new CodexEntry(categoryEntryID, list, Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".NAME"));
		codexEntry.icon = def.GetUISprite("ui", false);
		codexEntry.parentId = categoryEntryID;
		CodexCache.AddEntry(def.PrefabID, codexEntry, null);
		return codexEntry;
	}

	private static void GenerateDLC1RocketryEntries()
	{
		PlanScreen.PlanInfo planInfo = global::TUNING.BUILDINGS.PLANORDER.Find((PlanScreen.PlanInfo match) => match.category == new HashedString("Rocketry"));
		foreach (string text in SelectModuleSideScreen.moduleButtonSortOrder)
		{
			string text2 = HashCache.Get().Get(planInfo.category);
			string text3 = CodexCache.FormatLinkID(CodexEntryGenerator.categoryPrefx + text2);
			BuildingDef buildingDef = Assets.GetBuildingDef(text);
			CodexEntry codexEntry = CodexEntryGenerator.GenerateSingleBuildingEntry(buildingDef, text3);
			List<ICodexWidget> list = new List<ICodexWidget>();
			list.Add(new CodexSpacer());
			list.Add(new CodexText(UI.CLUSTERMAP.ROCKETS.MODULE_STATS.NAME_HEADER, CodexTextStyle.Subtitle, null));
			list.Add(new CodexSpacer());
			list.Add(new CodexText(UI.CLUSTERMAP.ROCKETS.SPEED.TOOLTIP, CodexTextStyle.Body, null));
			RocketModuleCluster component = buildingDef.BuildingComplete.GetComponent<RocketModuleCluster>();
			float burden = component.performanceStats.Burden;
			float enginePower = component.performanceStats.EnginePower;
			RocketEngineCluster component2 = buildingDef.BuildingComplete.GetComponent<RocketEngineCluster>();
			if (component2 != null)
			{
				list.Add(new CodexText("    • " + UI.CLUSTERMAP.ROCKETS.MAX_HEIGHT.NAME_MAX_SUPPORTED + component2.maxHeight.ToString(), CodexTextStyle.Body, null));
			}
			list.Add(new CodexText("    • " + UI.CLUSTERMAP.ROCKETS.MAX_HEIGHT.NAME_RAW + buildingDef.HeightInCells.ToString(), CodexTextStyle.Body, null));
			if (burden != 0f)
			{
				list.Add(new CodexText("    • " + UI.CLUSTERMAP.ROCKETS.BURDEN_MODULE.NAME + burden.ToString(), CodexTextStyle.Body, null));
			}
			if (enginePower != 0f)
			{
				list.Add(new CodexText("    • " + UI.CLUSTERMAP.ROCKETS.POWER_MODULE.NAME + enginePower.ToString(), CodexTextStyle.Body, null));
			}
			ContentContainer contentContainer = new ContentContainer(list, ContentContainer.ContentLayout.Vertical);
			codexEntry.AddContentContainer(contentContainer);
		}
	}

	public static void GeneratePageNotFound()
	{
		CodexCache.AddEntry("PageNotFound", new CodexEntry("ROOT", new List<ContentContainer>
		{
			new ContentContainer
			{
				content = 
				{
					new CodexText(CODEX.PAGENOTFOUND.TITLE, CodexTextStyle.Title, null),
					new CodexText(CODEX.PAGENOTFOUND.SUBTITLE, CodexTextStyle.Subtitle, null),
					new CodexDividerLine(),
					new CodexImage(312, 312, Assets.GetSprite("outhouseMessage"))
				}
			}
		}, CODEX.PAGENOTFOUND.TITLE)
		{
			searchOnly = true
		}, null);
	}

	public static Dictionary<string, CodexEntry> GenerateCreatureEntries()
	{
		Dictionary<string, CodexEntry> results = new Dictionary<string, CodexEntry>();
		List<GameObject> brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
		Action<Tag, string> action = delegate(Tag speciesTag, string name)
		{
			bool flag = false;
			List<ContentContainer> list = new List<ContentContainer>();
			CodexEntry codexEntry = new CodexEntry("CREATURES", list, name);
			foreach (GameObject gameObject in brains)
			{
				if (gameObject.GetDef<BabyMonitor.Def>() == null)
				{
					Sprite sprite = null;
					GameObject gameObject2 = Assets.TryGetPrefab(gameObject.PrefabID().ToString() + "Baby");
					if (gameObject2 != null)
					{
						sprite = Def.GetUISprite(gameObject2, "ui", false).first;
					}
					CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
					if (!(component.species != speciesTag))
					{
						if (!flag)
						{
							flag = true;
							list.Add(new ContentContainer(new List<ICodexWidget>
							{
								new CodexSpacer(),
								new CodexSpacer()
							}, ContentContainer.ContentLayout.Vertical));
							codexEntry.parentId = "CREATURES";
							CodexCache.AddEntry(speciesTag.ToString(), codexEntry, null);
							results.Add(speciesTag.ToString(), codexEntry);
						}
						List<ContentContainer> list2 = new List<ContentContainer>();
						string symbolPrefix = component.symbolPrefix;
						Sprite first = Def.GetUISprite(gameObject, symbolPrefix + "ui", false).first;
						if (sprite)
						{
							CodexEntryGenerator.GenerateImageContainers(new Sprite[] { first, sprite }, list2, ContentContainer.ContentLayout.Horizontal);
						}
						else
						{
							CodexEntryGenerator.GenerateImageContainers(first, list2);
						}
						CodexEntryGenerator.GenerateCreatureDescriptionContainers(gameObject, list2);
						SubEntry subEntry = new SubEntry(component.PrefabID().ToString(), speciesTag.ToString(), list2, component.GetProperName());
						subEntry.icon = first;
						subEntry.iconColor = Color.white;
						codexEntry.subEntries.Add(subEntry);
					}
				}
			}
		};
		action(GameTags.Creatures.Species.PuftSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.PUFTSPECIES);
		action(GameTags.Creatures.Species.PacuSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.PACUSPECIES);
		action(GameTags.Creatures.Species.OilFloaterSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.OILFLOATERSPECIES);
		action(GameTags.Creatures.Species.LightBugSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.LIGHTBUGSPECIES);
		action(GameTags.Creatures.Species.HatchSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.HATCHSPECIES);
		action(GameTags.Creatures.Species.GlomSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.GLOMSPECIES);
		action(GameTags.Creatures.Species.DreckoSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.DRECKOSPECIES);
		action(GameTags.Creatures.Species.MooSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.MOOSPECIES);
		action(GameTags.Creatures.Species.MoleSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.MOLESPECIES);
		action(GameTags.Creatures.Species.SquirrelSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.SQUIRRELSPECIES);
		action(GameTags.Creatures.Species.CrabSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.CRABSPECIES);
		action(GameTags.Robots.Models.ScoutRover, global::STRINGS.CREATURES.FAMILY_PLURAL.SCOUTROVER);
		action(GameTags.Creatures.Species.StaterpillarSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.STATERPILLARSPECIES);
		action(GameTags.Creatures.Species.BeetaSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.BEETASPECIES);
		action(GameTags.Creatures.Species.DivergentSpecies, global::STRINGS.CREATURES.FAMILY_PLURAL.DIVERGENTSPECIES);
		action(GameTags.Robots.Models.SweepBot, global::STRINGS.CREATURES.FAMILY_PLURAL.SWEEPBOT);
		return results;
	}

	public static Dictionary<string, CodexEntry> GeneratePlantEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Harvestable>();
		prefabsWithComponent.AddRange(Assets.GetPrefabsWithComponent<WiltCondition>());
		foreach (GameObject gameObject in prefabsWithComponent)
		{
			if (!dictionary.ContainsKey(gameObject.PrefabID().ToString()) && !(gameObject.GetComponent<BudUprootedMonitor>() != null))
			{
				List<ContentContainer> list = new List<ContentContainer>();
				Sprite first = Def.GetUISprite(gameObject, "ui", false).first;
				CodexEntryGenerator.GenerateImageContainers(first, list);
				CodexEntryGenerator.GeneratePlantDescriptionContainers(gameObject, list);
				CodexEntry codexEntry = new CodexEntry("PLANTS", list, gameObject.GetProperName());
				codexEntry.parentId = "PLANTS";
				codexEntry.icon = first;
				CodexCache.AddEntry(gameObject.PrefabID().ToString(), codexEntry, null);
				dictionary.Add(gameObject.PrefabID().ToString(), codexEntry);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateFoodEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (EdiblesManager.FoodInfo foodInfo in EdiblesManager.GetAllFoodTypes())
		{
			GameObject prefab = Assets.GetPrefab(foodInfo.Id);
			if (!prefab.HasTag(GameTags.DeprecatedContent) && !prefab.HasTag(GameTags.IncubatableEgg))
			{
				List<ContentContainer> list = new List<ContentContainer>();
				CodexEntryGenerator.GenerateTitleContainers(foodInfo.Name, list);
				Sprite first = Def.GetUISprite(foodInfo.ConsumableId, "ui", false).first;
				CodexEntryGenerator.GenerateImageContainers(first, list);
				CodexEntryGenerator.GenerateFoodDescriptionContainers(foodInfo, list);
				CodexEntryGenerator.GenerateRecipeContainers(foodInfo.ConsumableId.ToTag(), list);
				CodexEntryGenerator.GenerateUsedInRecipeContainers(foodInfo.ConsumableId.ToTag(), list);
				CodexEntry codexEntry = new CodexEntry("FOOD", list, foodInfo.Name);
				codexEntry.icon = first;
				codexEntry.parentId = "FOOD";
				CodexCache.AddEntry(foodInfo.Id, codexEntry, null);
				dictionary.Add(foodInfo.Id, codexEntry);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateTechEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Tech tech in Db.Get().Techs.resources)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			CodexEntryGenerator.GenerateTitleContainers(tech.Name, list);
			CodexEntryGenerator.GenerateTechDescriptionContainers(tech, list);
			CodexEntryGenerator.GeneratePrerequisiteTechContainers(tech, list);
			CodexEntryGenerator.GenerateUnlockContainers(tech, list);
			CodexEntry codexEntry = new CodexEntry("TECH", list, tech.Name);
			TechItem techItem = ((tech.unlockedItems.Count != 0) ? tech.unlockedItems[0] : null);
			codexEntry.icon = ((techItem == null) ? null : techItem.getUISprite("ui", false));
			codexEntry.parentId = "TECH";
			CodexCache.AddEntry(tech.Id, codexEntry, null);
			dictionary.Add(tech.Id, codexEntry);
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateRoleEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Skill skill in Db.Get().Skills.resources)
		{
			if (!skill.deprecated)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				Sprite sprite = Assets.GetSprite(skill.hat);
				CodexEntryGenerator.GenerateTitleContainers(skill.Name, list);
				CodexEntryGenerator.GenerateImageContainers(sprite, list);
				CodexEntryGenerator.GenerateGenericDescriptionContainers(skill.description, list);
				CodexEntryGenerator.GenerateSkillRequirementsAndPerksContainers(skill, list);
				CodexEntryGenerator.GenerateRelatedSkillContainers(skill, list);
				CodexEntry codexEntry = new CodexEntry("ROLES", list, skill.Name);
				codexEntry.parentId = "ROLES";
				codexEntry.icon = sprite;
				CodexCache.AddEntry(skill.Id, codexEntry, null);
				dictionary.Add(skill.Id, codexEntry);
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateGeyserEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Geyser>();
		if (prefabsWithComponent != null)
		{
			foreach (GameObject gameObject in prefabsWithComponent)
			{
				if (!gameObject.GetComponent<KPrefabID>().HasTag(GameTags.DeprecatedContent))
				{
					List<ContentContainer> list = new List<ContentContainer>();
					CodexEntryGenerator.GenerateTitleContainers(gameObject.GetProperName(), list);
					Sprite first = Def.GetUISprite(gameObject, "ui", false).first;
					CodexEntryGenerator.GenerateImageContainers(first, list);
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					string text = gameObject.PrefabID().ToString().ToUpper();
					string text2 = "GENERICGEYSER_";
					if (text.StartsWith(text2))
					{
						text.Remove(0, text2.Length);
					}
					list2.Add(new CodexText(UI.CODEX.GEYSERS.DESC, CodexTextStyle.Body, null));
					ContentContainer contentContainer = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
					list.Add(contentContainer);
					CodexEntry codexEntry = new CodexEntry("GEYSERS", list, gameObject.GetProperName());
					codexEntry.icon = first;
					codexEntry.parentId = "GEYSERS";
					codexEntry.id = gameObject.PrefabID().ToString();
					CodexCache.AddEntry(codexEntry.id, codexEntry, null);
					dictionary.Add(codexEntry.id, codexEntry);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateEquipmentEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Equippable>();
		if (prefabsWithComponent != null)
		{
			foreach (GameObject gameObject in prefabsWithComponent)
			{
				bool flag = false;
				Equippable component = gameObject.GetComponent<Equippable>();
				if (component.def.AdditionalTags != null)
				{
					Tag[] additionalTags = component.def.AdditionalTags;
					for (int i = 0; i < additionalTags.Length; i++)
					{
						if (additionalTags[i] == GameTags.DeprecatedContent)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag && !component.hideInCodex)
				{
					List<ContentContainer> list = new List<ContentContainer>();
					CodexEntryGenerator.GenerateTitleContainers(gameObject.GetProperName(), list);
					Sprite first = Def.GetUISprite(gameObject, "ui", false).first;
					CodexEntryGenerator.GenerateImageContainers(first, list);
					List<ICodexWidget> list2 = new List<ICodexWidget>();
					string text = gameObject.PrefabID().ToString();
					list2.Add(new CodexText(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + text.ToUpper() + ".DESC"), CodexTextStyle.Body, null));
					ContentContainer contentContainer = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
					list.Add(contentContainer);
					CodexEntry codexEntry = new CodexEntry("EQUIPMENT", list, gameObject.GetProperName());
					codexEntry.icon = first;
					codexEntry.parentId = "EQUIPMENT";
					codexEntry.id = gameObject.PrefabID().ToString();
					CodexCache.AddEntry(codexEntry.id, codexEntry, null);
					dictionary.Add(codexEntry.id, codexEntry);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateBiomeEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		ListPool<YamlIO.Error, WorldGen>.PooledList pooledList = ListPool<YamlIO.Error, WorldGen>.Allocate();
		Application.streamingAssetsPath + "/worldgen/worlds/";
		Application.streamingAssetsPath + "/worldgen/biomes/";
		Application.streamingAssetsPath + "/worldgen/subworlds/";
		WorldGen.LoadSettings(false);
		Dictionary<string, List<WeightedSubworldName>> dictionary2 = new Dictionary<string, List<WeightedSubworldName>>();
		foreach (KeyValuePair<string, ClusterLayout> keyValuePair in SettingsCache.clusterLayouts.clusterCache)
		{
			ClusterLayout value = keyValuePair.Value;
			string filePath = value.filePath;
			foreach (WorldPlacement worldPlacement in value.worldPlacements)
			{
				foreach (WeightedSubworldName weightedSubworldName in SettingsCache.worlds.GetWorldData(worldPlacement.world).subworldFiles)
				{
					string text = weightedSubworldName.name.Substring(weightedSubworldName.name.LastIndexOf("/"));
					string text2 = weightedSubworldName.name.Substring(0, weightedSubworldName.name.Length - text.Length);
					text2 = text2.Substring(text2.LastIndexOf("/") + 1);
					if (!(text2 == "subworlds"))
					{
						if (!dictionary2.ContainsKey(text2))
						{
							dictionary2.Add(text2, new List<WeightedSubworldName> { weightedSubworldName });
						}
						else
						{
							dictionary2[text2].Add(weightedSubworldName);
						}
					}
				}
			}
		}
		foreach (KeyValuePair<string, List<WeightedSubworldName>> keyValuePair2 in dictionary2)
		{
			string text3 = CodexCache.FormatLinkID(keyValuePair2.Key);
			global::Tuple<Sprite, Color> tuple = null;
			string text4 = Strings.Get("STRINGS.SUBWORLDS." + text3.ToUpper() + ".NAME");
			if (text4.Contains("MISSING"))
			{
				text4 = text3 + " (missing string key)";
			}
			List<ContentContainer> list = new List<ContentContainer>();
			CodexEntryGenerator.GenerateTitleContainers(text4, list);
			string text5 = "biomeIcon" + char.ToUpper(text3[0]).ToString() + text3.Substring(1).ToLower();
			Sprite sprite = Assets.GetSprite(text5);
			if (sprite != null)
			{
				tuple = new global::Tuple<Sprite, Color>(sprite, Color.white);
			}
			else
			{
				global::Debug.LogWarning("Missing codex biome icon: " + text5);
			}
			string text6 = Strings.Get("STRINGS.SUBWORLDS." + text3.ToUpper() + ".DESC");
			string text7 = Strings.Get("STRINGS.SUBWORLDS." + text3.ToUpper() + ".UTILITY");
			ContentContainer contentContainer = new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(string.IsNullOrEmpty(text6) ? "Basic description of the biome." : text6, CodexTextStyle.Body, null),
				new CodexSpacer(),
				new CodexText(string.IsNullOrEmpty(text7) ? "Description of the biomes utility." : text7, CodexTextStyle.Body, null),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical);
			list.Add(contentContainer);
			Dictionary<string, float> dictionary3 = new Dictionary<string, float>();
			ContentContainer contentContainer2 = new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(UI.CODEX.SUBWORLDS.ELEMENTS, CodexTextStyle.Subtitle, null),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical);
			list.Add(contentContainer2);
			ContentContainer contentContainer3 = new ContentContainer();
			contentContainer3.contentLayout = ContentContainer.ContentLayout.Vertical;
			contentContainer3.content = new List<ICodexWidget>();
			list.Add(contentContainer3);
			foreach (WeightedSubworldName weightedSubworldName2 in keyValuePair2.Value)
			{
				SubWorld subWorld = SettingsCache.subworlds[weightedSubworldName2.name];
				foreach (WeightedBiome weightedBiome in SettingsCache.subworlds[weightedSubworldName2.name].biomes)
				{
					foreach (ElementGradient elementGradient in SettingsCache.biomes.BiomeBackgroundElementBandConfigurations[weightedBiome.name])
					{
						if (dictionary3.ContainsKey(elementGradient.content))
						{
							dictionary3[elementGradient.content] = dictionary3[elementGradient.content] + elementGradient.bandSize;
						}
						else
						{
							if (ElementLoader.FindElementByName(elementGradient.content) == null)
							{
								global::Debug.LogError("Biome " + weightedBiome.name + " contains non-existent element " + elementGradient.content);
							}
							dictionary3.Add(elementGradient.content, elementGradient.bandSize);
						}
					}
				}
				foreach (Feature feature in subWorld.features)
				{
					foreach (KeyValuePair<string, ElementChoiceGroup<WeightedSimHash>> keyValuePair3 in SettingsCache.GetCachedFeature(feature.type).ElementChoiceGroups)
					{
						foreach (WeightedSimHash weightedSimHash in keyValuePair3.Value.choices)
						{
							if (dictionary3.ContainsKey(weightedSimHash.element))
							{
								dictionary3[weightedSimHash.element] = dictionary3[weightedSimHash.element] + 1f;
							}
							else
							{
								dictionary3.Add(weightedSimHash.element, 1f);
							}
						}
					}
				}
			}
			foreach (KeyValuePair<string, float> keyValuePair4 in dictionary3.OrderBy<KeyValuePair<string, float>, float>(delegate(KeyValuePair<string, float> pair)
			{
				KeyValuePair<string, float> keyValuePair5 = pair;
				return keyValuePair5.Value;
			}))
			{
				Element element = ElementLoader.FindElementByName(keyValuePair4.Key);
				if (tuple == null)
				{
					tuple = Def.GetUISprite(element.substance, "ui", false);
				}
				contentContainer3.content.Add(new CodexIndentedLabelWithIcon(element.name, CodexTextStyle.Body, Def.GetUISprite(element.substance, "ui", false)));
			}
			List<Tag> list2 = new List<Tag>();
			ContentContainer contentContainer4 = new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(UI.CODEX.SUBWORLDS.PLANTS, CodexTextStyle.Subtitle, null),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical);
			list.Add(contentContainer4);
			ContentContainer contentContainer5 = new ContentContainer();
			contentContainer5.contentLayout = ContentContainer.ContentLayout.Vertical;
			contentContainer5.content = new List<ICodexWidget>();
			list.Add(contentContainer5);
			foreach (WeightedSubworldName weightedSubworldName3 in keyValuePair2.Value)
			{
				foreach (WeightedBiome weightedBiome2 in SettingsCache.subworlds[weightedSubworldName3.name].biomes)
				{
					if (weightedBiome2.tags != null)
					{
						foreach (string text8 in weightedBiome2.tags)
						{
							if (!list2.Contains(text8))
							{
								GameObject gameObject = Assets.TryGetPrefab(text8);
								if (gameObject != null && (gameObject.GetComponent<Harvestable>() != null || gameObject.GetComponent<SeedProducer>() != null))
								{
									list2.Add(text8);
									contentContainer5.content.Add(new CodexIndentedLabelWithIcon(gameObject.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject, "ui", false)));
								}
							}
						}
					}
				}
				foreach (Feature feature2 in SettingsCache.subworlds[weightedSubworldName3.name].features)
				{
					foreach (MobReference mobReference in SettingsCache.GetCachedFeature(feature2.type).internalMobs)
					{
						Tag tag = mobReference.type.ToTag();
						if (!list2.Contains(tag))
						{
							GameObject gameObject2 = Assets.TryGetPrefab(tag);
							if (gameObject2 != null && (gameObject2.GetComponent<Harvestable>() != null || gameObject2.GetComponent<SeedProducer>() != null))
							{
								list2.Add(tag);
								contentContainer5.content.Add(new CodexIndentedLabelWithIcon(gameObject2.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject2, "ui", false)));
							}
						}
					}
				}
			}
			if (list2.Count == 0)
			{
				contentContainer5.content.Add(new CodexIndentedLabelWithIcon(UI.CODEX.SUBWORLDS.NONE, CodexTextStyle.Body, new global::Tuple<Sprite, Color>(Assets.GetSprite("inspectorUI_cannot_build"), Color.red)));
			}
			List<Tag> list3 = new List<Tag>();
			ContentContainer contentContainer6 = new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(UI.CODEX.SUBWORLDS.CRITTERS, CodexTextStyle.Subtitle, null),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical);
			list.Add(contentContainer6);
			ContentContainer contentContainer7 = new ContentContainer();
			contentContainer7.contentLayout = ContentContainer.ContentLayout.Vertical;
			contentContainer7.content = new List<ICodexWidget>();
			list.Add(contentContainer7);
			foreach (WeightedSubworldName weightedSubworldName4 in keyValuePair2.Value)
			{
				foreach (WeightedBiome weightedBiome3 in SettingsCache.subworlds[weightedSubworldName4.name].biomes)
				{
					if (weightedBiome3.tags != null)
					{
						foreach (string text9 in weightedBiome3.tags)
						{
							if (!list3.Contains(text9))
							{
								GameObject gameObject3 = Assets.TryGetPrefab(text9);
								if (gameObject3 != null && gameObject3.HasTag(GameTags.Creature))
								{
									list3.Add(text9);
									contentContainer7.content.Add(new CodexIndentedLabelWithIcon(gameObject3.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject3, "ui", false)));
								}
							}
						}
					}
				}
				foreach (Feature feature3 in SettingsCache.subworlds[weightedSubworldName4.name].features)
				{
					foreach (MobReference mobReference2 in SettingsCache.GetCachedFeature(feature3.type).internalMobs)
					{
						Tag tag2 = mobReference2.type.ToTag();
						if (!list3.Contains(tag2))
						{
							GameObject gameObject4 = Assets.TryGetPrefab(tag2);
							if (gameObject4 != null && gameObject4.HasTag(GameTags.Creature))
							{
								list3.Add(tag2);
								contentContainer7.content.Add(new CodexIndentedLabelWithIcon(gameObject4.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject4, "ui", false)));
							}
						}
					}
				}
			}
			if (list3.Count == 0)
			{
				contentContainer7.content.Add(new CodexIndentedLabelWithIcon(UI.CODEX.SUBWORLDS.NONE, CodexTextStyle.Body, new global::Tuple<Sprite, Color>(Assets.GetSprite("inspectorUI_cannot_build"), Color.red)));
			}
			string text10 = "BIOME" + text3;
			CodexEntry codexEntry = new CodexEntry("BIOMES", list, text10);
			codexEntry.name = text4;
			codexEntry.parentId = "BIOMES";
			codexEntry.icon = tuple.first;
			codexEntry.iconColor = tuple.second;
			CodexCache.AddEntry(text10, codexEntry, null);
			dictionary.Add(text10, codexEntry);
		}
		if (Application.isPlaying)
		{
			Global.Instance.modManager.HandleErrors(pooledList);
		}
		else
		{
			foreach (YamlIO.Error error in pooledList)
			{
				YamlIO.LogError(error, false);
			}
		}
		pooledList.Recycle();
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateElementEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary3 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary4 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary5 = new Dictionary<string, CodexEntry>();
		string text = CodexCache.FormatLinkID("ELEMENTS");
		string text2 = CodexCache.FormatLinkID("ELEMENTS_SOLID");
		string text3 = CodexCache.FormatLinkID("ELEMENTS_LIQUID");
		string text4 = CodexCache.FormatLinkID("ELEMENTS_GAS");
		string text5 = CodexCache.FormatLinkID("ELEMENTS_OTHER");
		string text6 = CodexCache.FormatLinkID("ELEMENTS_CLASSES");
		CodexEntryGenerator.CodexElementMap usedMap = new CodexEntryGenerator.CodexElementMap();
		CodexEntryGenerator.CodexElementMap madeMap = new CodexEntryGenerator.CodexElementMap();
		Tag waterTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		Tag dirtyWaterTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		Action<GameObject, CodexEntryGenerator.CodexElementMap, CodexEntryGenerator.CodexElementMap> action = delegate(GameObject prefab, CodexEntryGenerator.CodexElementMap usedMap, CodexEntryGenerator.CodexElementMap made)
		{
			HashSet<ElementUsage> hashSet2 = new HashSet<ElementUsage>();
			HashSet<ElementUsage> hashSet3 = new HashSet<ElementUsage>();
			EnergyGenerator component = prefab.GetComponent<EnergyGenerator>();
			if (component)
			{
				IEnumerable<EnergyGenerator.InputItem> inputs = component.formula.inputs;
				foreach (EnergyGenerator.InputItem inputItem in (inputs ?? Enumerable.Empty<EnergyGenerator.InputItem>()))
				{
					hashSet2.Add(new ElementUsage(inputItem.tag, inputItem.consumptionRate, true));
				}
				IEnumerable<EnergyGenerator.OutputItem> outputs = component.formula.outputs;
				foreach (EnergyGenerator.OutputItem outputItem in (outputs ?? Enumerable.Empty<EnergyGenerator.OutputItem>()))
				{
					Tag tag2 = ElementLoader.FindElementByHash(outputItem.element).tag;
					hashSet3.Add(new ElementUsage(tag2, outputItem.creationRate, true));
				}
			}
			IEnumerable<ElementConverter> components = prefab.GetComponents<ElementConverter>();
			foreach (ElementConverter elementConverter in (components ?? Enumerable.Empty<ElementConverter>()))
			{
				IEnumerable<ElementConverter.ConsumedElement> consumedElements = elementConverter.consumedElements;
				foreach (ElementConverter.ConsumedElement consumedElement in (consumedElements ?? Enumerable.Empty<ElementConverter.ConsumedElement>()))
				{
					hashSet2.Add(new ElementUsage(consumedElement.tag, consumedElement.massConsumptionRate, true));
				}
				IEnumerable<ElementConverter.OutputElement> outputElements = elementConverter.outputElements;
				foreach (ElementConverter.OutputElement outputElement in (outputElements ?? Enumerable.Empty<ElementConverter.OutputElement>()))
				{
					Tag tag3 = ElementLoader.FindElementByHash(outputElement.elementHash).tag;
					hashSet3.Add(new ElementUsage(tag3, outputElement.massGenerationRate, true));
				}
			}
			IEnumerable<ElementConsumer> components2 = prefab.GetComponents<ElementConsumer>();
			foreach (ElementConsumer elementConsumer in (components2 ?? Enumerable.Empty<ElementConsumer>()))
			{
				if (!elementConsumer.storeOnConsume)
				{
					Tag tag4 = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag;
					hashSet2.Add(new ElementUsage(tag4, elementConsumer.consumptionRate, true));
				}
			}
			IrrigationMonitor.Def def = prefab.GetDef<IrrigationMonitor.Def>();
			if (def != null)
			{
				foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in def.consumedElements)
				{
					hashSet2.Add(new ElementUsage(consumeInfo.tag, consumeInfo.massConsumptionRate, true));
				}
			}
			FertilizationMonitor.Def def2 = prefab.GetDef<FertilizationMonitor.Def>();
			if (def2 != null)
			{
				foreach (PlantElementAbsorber.ConsumeInfo consumeInfo2 in def2.consumedElements)
				{
					hashSet2.Add(new ElementUsage(consumeInfo2.tag, consumeInfo2.massConsumptionRate, true));
				}
			}
			FlushToilet component2 = prefab.GetComponent<FlushToilet>();
			if (component2)
			{
				hashSet2.Add(new ElementUsage(waterTag, component2.massConsumedPerUse, false));
				hashSet3.Add(new ElementUsage(dirtyWaterTag, component2.massEmittedPerUse, false));
			}
			HandSanitizer component3 = prefab.GetComponent<HandSanitizer>();
			if (component3)
			{
				Tag tag5 = ElementLoader.FindElementByHash(component3.consumedElement).tag;
				hashSet2.Add(new ElementUsage(tag5, component3.massConsumedPerUse, false));
				if (component3.outputElement != SimHashes.Vacuum)
				{
					Tag tag6 = ElementLoader.FindElementByHash(component3.outputElement).tag;
					hashSet3.Add(new ElementUsage(tag6, component3.massConsumedPerUse, false));
				}
			}
			CodexEntryGenerator.ConversionEntry conversionEntry2 = new CodexEntryGenerator.ConversionEntry();
			conversionEntry2.title = prefab.GetProperName();
			conversionEntry2.prefab = prefab;
			conversionEntry2.inSet = hashSet2;
			conversionEntry2.outSet = hashSet3;
			foreach (ElementUsage elementUsage in hashSet2)
			{
				usedMap.Add(elementUsage.tag, conversionEntry2);
			}
			foreach (ElementUsage elementUsage2 in hashSet3)
			{
				madeMap.Add(elementUsage2.tag, conversionEntry2);
			}
		};
		foreach (PlanScreen.PlanInfo planInfo in global::TUNING.BUILDINGS.PLANORDER)
		{
			foreach (KeyValuePair<string, string> keyValuePair in planInfo.buildingAndSubcategoryData)
			{
				BuildingDef buildingDef = Assets.GetBuildingDef(keyValuePair.Key);
				if (buildingDef == null)
				{
					global::Debug.LogError("Building def for id " + keyValuePair.Key + " is null");
				}
				if (!buildingDef.Deprecated)
				{
					action(buildingDef.BuildingComplete, usedMap, madeMap);
				}
			}
		}
		HashSet<GameObject> hashSet = new HashSet<GameObject>(Assets.GetPrefabsWithComponent<Harvestable>());
		foreach (GameObject gameObject in Assets.GetPrefabsWithComponent<WiltCondition>())
		{
			hashSet.Add(gameObject);
		}
		foreach (GameObject gameObject2 in hashSet)
		{
			if (!(gameObject2.GetComponent<BudUprootedMonitor>() != null))
			{
				action(gameObject2, usedMap, madeMap);
			}
		}
		foreach (GameObject gameObject3 in Assets.GetPrefabsWithComponent<CreatureBrain>())
		{
			if (gameObject3.GetDef<BabyMonitor.Def>() == null)
			{
				action(gameObject3, usedMap, madeMap);
			}
		}
		foreach (KeyValuePair<Tag, Diet> keyValuePair2 in DietManager.CollectDiets(null))
		{
			GameObject gameObject4 = Assets.GetPrefab(keyValuePair2.Key).gameObject;
			if (gameObject4.GetDef<BabyMonitor.Def>() == null)
			{
				float num = 0f;
				foreach (AttributeModifier attributeModifier in Db.Get().traits.Get(gameObject4.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
				{
					if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
					{
						num = attributeModifier.Value;
					}
				}
				foreach (Diet.Info info in keyValuePair2.Value.infos)
				{
					float num2 = -num / info.caloriesPerKg;
					float num3 = num2 * info.producedConversionRate;
					foreach (Tag tag in info.consumedTags)
					{
						CodexEntryGenerator.ConversionEntry conversionEntry = new CodexEntryGenerator.ConversionEntry();
						conversionEntry.title = gameObject4.GetProperName();
						conversionEntry.prefab = gameObject4;
						conversionEntry.inSet = new HashSet<ElementUsage>();
						conversionEntry.inSet.Add(new ElementUsage(tag, num2, true));
						conversionEntry.outSet = new HashSet<ElementUsage>();
						conversionEntry.outSet.Add(new ElementUsage(info.producedElement, num3, true));
						usedMap.Add(tag, conversionEntry);
						madeMap.Add(info.producedElement, conversionEntry);
					}
				}
			}
		}
		int i;
		Action<Element, List<ContentContainer>> action2 = delegate(Element element, List<ContentContainer> containers)
		{
			if (element.highTempTransition != null || element.lowTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexText(CODEX.HEADERS.ELEMENTTRANSITIONS, CodexTextStyle.Subtitle, null),
					new CodexDividerLine()
				}, ContentContainer.ContentLayout.Vertical));
			}
			if (element.highTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(32, 32, Def.GetUISprite(element.highTempTransition, "ui", false)),
					new CodexText((element.highTempTransition != null) ? string.Concat(new string[]
					{
						element.highTempTransition.name,
						" (",
						element.highTempTransition.GetStateString(),
						")  (",
						GameUtil.GetFormattedTemperature(element.highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false),
						")"
					}) : "", CodexTextStyle.Body, null)
				}, ContentContainer.ContentLayout.Horizontal));
			}
			if (element.lowTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(32, 32, Def.GetUISprite(element.lowTempTransition, "ui", false)),
					new CodexText((element.lowTempTransition != null) ? string.Concat(new string[]
					{
						element.lowTempTransition.name,
						" (",
						element.lowTempTransition.GetStateString(),
						")  (",
						GameUtil.GetFormattedTemperature(element.lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false),
						")"
					}) : "", CodexTextStyle.Body, null)
				}, ContentContainer.ContentLayout.Horizontal));
			}
			List<ICodexWidget> list4 = new List<ICodexWidget>();
			List<ICodexWidget> list5 = new List<ICodexWidget>();
			Func<ComplexRecipe.RecipeElement, bool> <>9__2;
			Func<ComplexRecipe.RecipeElement, bool> <>9__3;
			foreach (ComplexRecipe complexRecipe in ComplexRecipeManager.Get().recipes)
			{
				IEnumerable<ComplexRecipe.RecipeElement> ingredients = complexRecipe.ingredients;
				Func<ComplexRecipe.RecipeElement, bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (ComplexRecipe.RecipeElement i) => i.material == element.tag);
				}
				if (ingredients.Any<ComplexRecipe.RecipeElement>(func))
				{
					list4.Add(new CodexRecipePanel(complexRecipe));
				}
				IEnumerable<ComplexRecipe.RecipeElement> results = complexRecipe.results;
				Func<ComplexRecipe.RecipeElement, bool> func2;
				if ((func2 = <>9__3) == null)
				{
					func2 = (<>9__3 = (ComplexRecipe.RecipeElement i) => i.material == element.tag);
				}
				if (results.Any<ComplexRecipe.RecipeElement>(func2))
				{
					list5.Add(new CodexRecipePanel(complexRecipe));
				}
			}
			List<CodexEntryGenerator.ConversionEntry> list6;
			if (usedMap.map.TryGetValue(element.tag, out list6))
			{
				foreach (CodexEntryGenerator.ConversionEntry conversionEntry3 in list6)
				{
					list4.Add(new CodexConversionPanel(conversionEntry3.title, conversionEntry3.inSet.ToArray<ElementUsage>(), conversionEntry3.outSet.ToArray<ElementUsage>(), conversionEntry3.prefab));
				}
			}
			List<CodexEntryGenerator.ConversionEntry> list7;
			if (madeMap.map.TryGetValue(element.tag, out list7))
			{
				foreach (CodexEntryGenerator.ConversionEntry conversionEntry4 in list7)
				{
					list5.Add(new CodexConversionPanel(conversionEntry4.title, conversionEntry4.inSet.ToArray<ElementUsage>(), conversionEntry4.outSet.ToArray<ElementUsage>(), conversionEntry4.prefab));
				}
			}
			ContentContainer contentContainer = new ContentContainer(list4, ContentContainer.ContentLayout.Vertical);
			ContentContainer contentContainer2 = new ContentContainer(list5, ContentContainer.ContentLayout.Vertical);
			if (list4.Count > 0)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer(),
					new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTCONSUMEDBY, contentContainer)
				}, ContentContainer.ContentLayout.Vertical));
				containers.Add(contentContainer);
			}
			if (list5.Count > 0)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer(),
					new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTPRODUCEDBY, contentContainer2)
				}, ContentContainer.ContentLayout.Vertical));
				containers.Add(contentContainer2);
			}
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(element.FullDescription(true), CodexTextStyle.Body, null),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical));
		};
		string text8;
		foreach (Element element3 in ElementLoader.elements)
		{
			if (!element3.disabled)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				string text7 = element3.name + " (" + element3.GetStateString() + ")";
				global::Tuple<Sprite, Color> tuple = Def.GetUISprite(element3, "ui", false);
				if (tuple.first == null)
				{
					if (element3.id == SimHashes.Void)
					{
						text7 = element3.name;
						tuple = new global::Tuple<Sprite, Color>(Assets.GetSprite("ui_elements-void"), Color.white);
					}
					else if (element3.id == SimHashes.Vacuum)
					{
						text7 = element3.name;
						tuple = new global::Tuple<Sprite, Color>(Assets.GetSprite("ui_elements-vacuum"), Color.white);
					}
				}
				CodexEntryGenerator.GenerateTitleContainers(text7, list);
				CodexEntryGenerator.GenerateImageContainers(new global::Tuple<Sprite, Color>[] { tuple }, list, ContentContainer.ContentLayout.Horizontal);
				action2(element3, list);
				text8 = element3.id.ToString();
				string text9;
				Dictionary<string, CodexEntry> dictionary6;
				if (element3.IsSolid)
				{
					text9 = text2;
					dictionary6 = dictionary2;
				}
				else if (element3.IsLiquid)
				{
					text9 = text3;
					dictionary6 = dictionary3;
				}
				else if (element3.IsGas)
				{
					text9 = text4;
					dictionary6 = dictionary4;
				}
				else
				{
					text9 = text5;
					dictionary6 = dictionary5;
				}
				CodexEntry codexEntry = new CodexEntry(text9, list, text7);
				codexEntry.parentId = text9;
				codexEntry.icon = tuple.first;
				codexEntry.iconColor = tuple.second;
				CodexCache.AddEntry(text8, codexEntry, null);
				dictionary6.Add(text8, codexEntry);
			}
		}
		text8 = text2;
		CodexEntry codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text8, UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, dictionary2, Assets.GetSprite("ui_elements-solid"), true, true, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text8, codexEntry2);
		text8 = text3;
		codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text8, UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, dictionary3, Assets.GetSprite("ui_elements-liquids"), true, true, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text8, codexEntry2);
		text8 = text4;
		codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text8, UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, dictionary4, Assets.GetSprite("ui_elements-gases"), true, true, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text8, codexEntry2);
		text8 = text5;
		codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text8, UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, dictionary5, Assets.GetSprite("ui_elements-other"), true, true, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text8, codexEntry2);
		Sprite sprite = Assets.GetSprite("ui_elements_classes");
		var anon = new <>f__AnonymousType1<Tag, bool, bool, string>[]
		{
			new
			{
				tag = GameTags.IceOre,
				checkPrefabs = false,
				solidOnly = false,
				spriteName = "ui_ice"
			},
			new
			{
				tag = GameTags.RefinedMetal,
				checkPrefabs = false,
				solidOnly = true,
				spriteName = "ui_refined_metal"
			},
			new
			{
				tag = GameTags.Filter,
				checkPrefabs = false,
				solidOnly = false,
				spriteName = "ui_filtration_medium"
			},
			new
			{
				tag = GameTags.Compostable,
				checkPrefabs = true,
				solidOnly = false,
				spriteName = "ui_compostable"
			},
			new
			{
				tag = GameTags.CombustibleLiquid,
				checkPrefabs = false,
				solidOnly = false,
				spriteName = "ui_combustible_liquids"
			}
		};
		Dictionary<string, CodexEntry> dictionary7 = new Dictionary<string, CodexEntry>();
		var anon2 = anon;
		i = 0;
		while (i < anon2.Length)
		{
			var anon3 = anon2[i];
			string text10 = anon3.tag.ToString();
			string text11 = Strings.Get("STRINGS.MISC.TAGS." + text10.ToUpper());
			List<ContentContainer> list2 = new List<ContentContainer>();
			CodexEntryGenerator.GenerateTitleContainers(text11, list2);
			list2.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(Strings.Get("STRINGS.MISC.TAGS." + text10.ToUpper() + "_DESC"), CodexTextStyle.Body, null),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical));
			List<ICodexWidget> list3 = new List<ICodexWidget>();
			if (anon3.checkPrefabs)
			{
				using (List<GameObject>.Enumerator enumerator3 = Assets.GetPrefabsWithTag(anon3.tag).GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						GameObject gameObject5 = enumerator3.Current;
						if (!gameObject5.HasTag(GameTags.DeprecatedContent))
						{
							list3.Add(new CodexIndentedLabelWithIcon(gameObject5.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject5, "ui", false)));
						}
					}
					goto IL_098A;
				}
				goto IL_08BF;
			}
			goto IL_08BF;
			IL_098A:
			list2.Add(new ContentContainer(list3, ContentContainer.ContentLayout.GridTwoColumn));
			CodexEntry codexEntry3 = new CodexEntry(text6, list2, text11);
			codexEntry3.parentId = text6;
			codexEntry3.icon = Assets.GetSprite(anon3.spriteName);
			CodexCache.AddEntry(CodexCache.FormatLinkID(text10), codexEntry3, null);
			dictionary7.Add(text10, codexEntry3);
			i++;
			continue;
			IL_08BF:
			foreach (Element element2 in ElementLoader.elements)
			{
				if (!element2.disabled && (!anon3.solidOnly || element2.IsSolid))
				{
					bool flag = element2.materialCategory == anon3.tag;
					Tag[] oreTags = element2.oreTags;
					for (int j = 0; j < oreTags.Length; j++)
					{
						if (oreTags[j] == anon3.tag)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						list3.Add(new CodexIndentedLabelWithIcon(element2.name, CodexTextStyle.Body, Def.GetUISprite(element2.substance, "ui", false)));
					}
				}
			}
			goto IL_098A;
		}
		codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text6, UI.CODEX.CATEGORYNAMES.ELEMENTSCLASSES, dictionary7, sprite, true, true, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text6, codexEntry2);
		CodexEntryGenerator.PopulateCategoryEntries(dictionary);
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateDiseaseEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Disease disease in Db.Get().Diseases.resources)
		{
			if (!disease.Disabled)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				CodexEntryGenerator.GenerateTitleContainers(disease.Name, list);
				CodexEntryGenerator.GenerateDiseaseDescriptionContainers(disease, list);
				CodexEntry codexEntry = new CodexEntry("DISEASE", list, disease.Name);
				codexEntry.parentId = "DISEASE";
				dictionary.Add(disease.Id, codexEntry);
				codexEntry.icon = Assets.GetSprite("overlay_disease");
				CodexCache.AddEntry(disease.Id, codexEntry, null);
			}
		}
		return dictionary;
	}

	public static CategoryEntry GenerateCategoryEntry(string id, string name, Dictionary<string, CodexEntry> entries, Sprite icon = null, bool largeFormat = true, bool sort = true, string overrideHeader = null)
	{
		List<ContentContainer> list = new List<ContentContainer>();
		CodexEntryGenerator.GenerateTitleContainers((overrideHeader == null) ? name : overrideHeader, list);
		List<CodexEntry> list2 = new List<CodexEntry>();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in entries)
		{
			list2.Add(keyValuePair.Value);
			if (icon == null)
			{
				icon = keyValuePair.Value.icon;
			}
		}
		CategoryEntry categoryEntry = new CategoryEntry("Root", list, name, list2, largeFormat, sort);
		categoryEntry.icon = icon;
		CodexCache.AddEntry(id, categoryEntry, null);
		return categoryEntry;
	}

	public static Dictionary<string, CodexEntry> GenerateTutorialNotificationEntries()
	{
		CodexEntry codexEntry = new CodexEntry("MISCELLANEOUSTIPS", new List<ContentContainer>
		{
			new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical)
		}, Strings.Get("STRINGS.UI.CODEX.CATEGORYNAMES.MISCELLANEOUSTIPS"));
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		for (int i = 0; i < 20; i++)
		{
			TutorialMessage tutorialMessage = (TutorialMessage)Tutorial.Instance.TutorialMessage((Tutorial.TutorialMessages)i, false);
			if (tutorialMessage != null && DlcManager.IsDlcListValidForCurrentContent(tutorialMessage.DLCIDs))
			{
				if (!string.IsNullOrEmpty(tutorialMessage.videoClipId))
				{
					List<ContentContainer> list = new List<ContentContainer>();
					CodexEntryGenerator.GenerateTitleContainers(tutorialMessage.GetTitle(), list);
					CodexVideo codexVideo = new CodexVideo();
					codexVideo.videoName = tutorialMessage.videoClipId;
					codexVideo.overlayName = tutorialMessage.videoOverlayName;
					codexVideo.overlayTexts = new List<string>
					{
						tutorialMessage.videoTitleText,
						VIDEOS.TUTORIAL_HEADER
					};
					list.Add(new ContentContainer(new List<ICodexWidget> { codexVideo }, ContentContainer.ContentLayout.Vertical));
					list.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexText(tutorialMessage.GetMessageBody(), CodexTextStyle.Body, tutorialMessage.GetTitle())
					}, ContentContainer.ContentLayout.Vertical));
					CodexEntry codexEntry2 = new CodexEntry("Videos", list, UI.FormatAsLink(tutorialMessage.GetTitle(), "videos_" + i.ToString()));
					codexEntry2.icon = Assets.GetSprite("codexVideo");
					CodexCache.AddEntry("videos_" + i.ToString(), codexEntry2, null);
					dictionary.Add(codexEntry2.id, codexEntry2);
				}
				else
				{
					List<ContentContainer> list2 = new List<ContentContainer>();
					CodexEntryGenerator.GenerateTitleContainers(tutorialMessage.GetTitle(), list2);
					list2.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexText(tutorialMessage.GetMessageBody(), CodexTextStyle.Body, tutorialMessage.GetTitle())
					}, ContentContainer.ContentLayout.Vertical));
					list2.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexSpacer(),
						new CodexSpacer()
					}, ContentContainer.ContentLayout.Vertical));
					SubEntry subEntry = new SubEntry("MISCELLANEOUSTIPS" + i.ToString(), "MISCELLANEOUSTIPS", list2, tutorialMessage.GetTitle());
					codexEntry.subEntries.Add(subEntry);
				}
			}
		}
		CodexCache.AddEntry("MISCELLANEOUSTIPS", codexEntry, null);
		return dictionary;
	}

	public static void PopulateCategoryEntries(Dictionary<string, CodexEntry> categoryEntries)
	{
		List<CategoryEntry> list = new List<CategoryEntry>();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in categoryEntries)
		{
			list.Add(keyValuePair.Value as CategoryEntry);
		}
		CodexEntryGenerator.PopulateCategoryEntries(list, null);
	}

	public static void PopulateCategoryEntries(List<CategoryEntry> categoryEntries, Comparison<CodexEntry> comparison = null)
	{
		foreach (CategoryEntry categoryEntry in categoryEntries)
		{
			List<ContentContainer> contentContainers = categoryEntry.contentContainers;
			List<CodexEntry> list = new List<CodexEntry>();
			foreach (CodexEntry codexEntry in categoryEntry.entriesInCategory)
			{
				list.Add(codexEntry);
			}
			if (categoryEntry.sort)
			{
				if (comparison == null)
				{
					list.Sort((CodexEntry a, CodexEntry b) => UI.StripLinkFormatting(a.name).CompareTo(UI.StripLinkFormatting(b.name)));
				}
				else
				{
					list.Sort(comparison);
				}
			}
			if (categoryEntry.largeFormat)
			{
				ContentContainer contentContainer = new ContentContainer(new List<ICodexWidget>(), ContentContainer.ContentLayout.Grid);
				foreach (CodexEntry codexEntry2 in list)
				{
					contentContainer.content.Add(new CodexLabelWithLargeIcon(codexEntry2.name, CodexTextStyle.BodyWhite, new global::Tuple<Sprite, Color>((codexEntry2.icon != null) ? codexEntry2.icon : Assets.GetSprite("unknown"), codexEntry2.iconColor), codexEntry2.id));
				}
				if (categoryEntry.showBeforeGeneratedCategoryLinks)
				{
					contentContainers.Add(contentContainer);
				}
				else
				{
					ContentContainer contentContainer2 = contentContainers[contentContainers.Count - 1];
					contentContainers.RemoveAt(contentContainers.Count - 1);
					contentContainers.Insert(0, contentContainer2);
					contentContainers.Insert(1, contentContainer);
					contentContainers.Insert(2, new ContentContainer(new List<ICodexWidget>
					{
						new CodexSpacer()
					}, ContentContainer.ContentLayout.Vertical));
				}
			}
			else
			{
				ContentContainer contentContainer3 = new ContentContainer(new List<ICodexWidget>(), ContentContainer.ContentLayout.Vertical);
				foreach (CodexEntry codexEntry3 in list)
				{
					if (codexEntry3.icon == null)
					{
						contentContainer3.content.Add(new CodexText(codexEntry3.name, CodexTextStyle.Body, null));
					}
					else
					{
						contentContainer3.content.Add(new CodexLabelWithIcon(codexEntry3.name, CodexTextStyle.Body, new global::Tuple<Sprite, Color>(codexEntry3.icon, codexEntry3.iconColor), 64, 48));
					}
				}
				if (categoryEntry.showBeforeGeneratedCategoryLinks)
				{
					contentContainers.Add(contentContainer3);
				}
				else
				{
					ContentContainer contentContainer4 = contentContainers[contentContainers.Count - 1];
					contentContainers.RemoveAt(contentContainers.Count - 1);
					contentContainers.Insert(0, contentContainer4);
					contentContainers.Insert(1, contentContainer3);
				}
			}
		}
	}

	private static void GenerateTitleContainers(string name, List<ContentContainer> containers)
	{
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(name, CodexTextStyle.Title, null),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical));
	}

	private static void GeneratePrerequisiteTechContainers(Tech tech, List<ContentContainer> containers)
	{
		if (tech.requiredTech == null || tech.requiredTech.Count == 0)
		{
			return;
		}
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(CODEX.HEADERS.PREREQUISITE_TECH, CodexTextStyle.Subtitle, null));
		list.Add(new CodexDividerLine());
		list.Add(new CodexSpacer());
		foreach (Tech tech2 in tech.requiredTech)
		{
			list.Add(new CodexText(tech2.Name, CodexTextStyle.Body, null));
		}
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateSkillRequirementsAndPerksContainers(Skill skill, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText codexText = new CodexText(CODEX.HEADERS.ROLE_PERKS, CodexTextStyle.Subtitle, null);
		CodexText codexText2 = new CodexText(CODEX.HEADERS.ROLE_PERKS_DESC, CodexTextStyle.Body, null);
		list.Add(codexText);
		list.Add(new CodexDividerLine());
		list.Add(codexText2);
		list.Add(new CodexSpacer());
		foreach (SkillPerk skillPerk in skill.perks)
		{
			CodexText codexText3 = new CodexText(skillPerk.Name, CodexTextStyle.Body, null);
			list.Add(codexText3);
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		list.Add(new CodexSpacer());
	}

	private static void GenerateRelatedSkillContainers(Skill skill, List<ContentContainer> containers)
	{
		bool flag = false;
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText codexText = new CodexText(CODEX.HEADERS.PREREQUISITE_ROLES, CodexTextStyle.Subtitle, null);
		list.Add(codexText);
		list.Add(new CodexDividerLine());
		list.Add(new CodexSpacer());
		foreach (string text in skill.priorSkills)
		{
			CodexText codexText2 = new CodexText(Db.Get().Skills.Get(text).Name, CodexTextStyle.Body, null);
			list.Add(codexText2);
			flag = true;
		}
		if (flag)
		{
			list.Add(new CodexSpacer());
			containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		}
		bool flag2 = false;
		List<ICodexWidget> list2 = new List<ICodexWidget>();
		CodexText codexText3 = new CodexText(CODEX.HEADERS.UNLOCK_ROLES, CodexTextStyle.Subtitle, null);
		CodexText codexText4 = new CodexText(CODEX.HEADERS.UNLOCK_ROLES_DESC, CodexTextStyle.Body, null);
		list2.Add(codexText3);
		list2.Add(new CodexDividerLine());
		list2.Add(codexText4);
		list2.Add(new CodexSpacer());
		foreach (Skill skill2 in Db.Get().Skills.resources)
		{
			if (!skill2.deprecated)
			{
				using (List<string>.Enumerator enumerator = skill2.priorSkills.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == skill.Id)
						{
							CodexText codexText5 = new CodexText(skill2.Name, CodexTextStyle.Body, null);
							list2.Add(codexText5);
							flag2 = true;
						}
					}
				}
			}
		}
		if (flag2)
		{
			list2.Add(new CodexSpacer());
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
		}
	}

	private static void GenerateUnlockContainers(Tech tech, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText codexText = new CodexText(CODEX.HEADERS.TECH_UNLOCKS, CodexTextStyle.Subtitle, null);
		list.Add(codexText);
		list.Add(new CodexDividerLine());
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		foreach (TechItem techItem in tech.unlockedItems)
		{
			List<ICodexWidget> list2 = new List<ICodexWidget>();
			CodexImage codexImage = new CodexImage(64, 64, techItem.getUISprite("ui", false));
			list2.Add(codexImage);
			CodexText codexText2 = new CodexText(techItem.Name, CodexTextStyle.Body, null);
			list2.Add(codexText2);
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GenerateRecipeContainers(Tag prefabID, List<ContentContainer> containers)
	{
		Recipe recipe = null;
		foreach (Recipe recipe2 in RecipeManager.Get().recipes)
		{
			if (recipe2.Result == prefabID)
			{
				recipe = recipe2;
				break;
			}
		}
		if (recipe == null)
		{
			return;
		}
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(CODEX.HEADERS.RECIPE, CodexTextStyle.Subtitle, null),
			new CodexSpacer(),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical));
		Func<Recipe, List<ContentContainer>> func = delegate(Recipe rec)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			foreach (Recipe.Ingredient ingredient in rec.Ingredients)
			{
				GameObject prefab = Assets.GetPrefab(ingredient.tag);
				if (prefab != null)
				{
					list.Add(new ContentContainer(new List<ICodexWidget>
					{
						new CodexImage(64, 64, Def.GetUISprite(prefab, "ui", false)),
						new CodexText(string.Format(UI.CODEX.RECIPE_ITEM, Assets.GetPrefab(ingredient.tag).GetProperName(), ingredient.amount, (ElementLoader.GetElement(ingredient.tag) == null) ? "" : UI.UNITSUFFIXES.MASS.KILOGRAM.text), CodexTextStyle.Body, null)
					}, ContentContainer.ContentLayout.Horizontal));
				}
			}
			return list;
		};
		containers.AddRange(func(recipe));
		GameObject gameObject = ((recipe.fabricators == null) ? null : Assets.GetPrefab(recipe.fabricators[0]));
		if (gameObject != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(UI.CODEX.RECIPE_FABRICATOR_HEADER, CodexTextStyle.Subtitle, null),
				new CodexDividerLine()
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexImage(64, 64, Def.GetUISpriteFromMultiObjectAnim(gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false, "")),
				new CodexText(string.Format(UI.CODEX.RECIPE_FABRICATOR, recipe.FabricationTime, gameObject.GetProperName()), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GenerateUsedInRecipeContainers(Tag prefabID, List<ContentContainer> containers)
	{
		List<Recipe> list = new List<Recipe>();
		foreach (Recipe recipe in RecipeManager.Get().recipes)
		{
			using (List<Recipe.Ingredient>.Enumerator enumerator2 = recipe.Ingredients.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.tag == prefabID)
					{
						list.Add(recipe);
					}
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(CODEX.HEADERS.USED_IN_RECIPES, CodexTextStyle.Subtitle, null),
			new CodexSpacer(),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical));
		foreach (Recipe recipe2 in list)
		{
			GameObject prefab = Assets.GetPrefab(recipe2.Result);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexImage(64, 64, Def.GetUISprite(prefab, "ui", false)),
				new CodexText(prefab.GetProperName(), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GeneratePlantDescriptionContainers(GameObject plant, List<ContentContainer> containers)
	{
		SeedProducer component = plant.GetComponent<SeedProducer>();
		if (component != null)
		{
			GameObject prefab = Assets.GetPrefab(component.seedInfo.seedId);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexText(CODEX.HEADERS.GROWNFROMSEED, CodexTextStyle.Subtitle, null),
				new CodexDividerLine()
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexImage(48, 48, Def.GetUISprite(prefab, "ui", false)),
				new CodexText(prefab.GetProperName(), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Horizontal));
		}
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexSpacer());
		list.Add(new CodexText(UI.CODEX.DETAILS, CodexTextStyle.Subtitle, null));
		list.Add(new CodexDividerLine());
		InfoDescription component2 = Assets.GetPrefab(plant.PrefabID()).GetComponent<InfoDescription>();
		if (component2 != null)
		{
			list.Add(new CodexText(component2.description, CodexTextStyle.Body, null));
		}
		string text = "";
		List<Descriptor> plantRequirementDescriptors = GameUtil.GetPlantRequirementDescriptors(plant);
		if (plantRequirementDescriptors.Count > 0)
		{
			text += plantRequirementDescriptors[0].text;
			for (int i = 1; i < plantRequirementDescriptors.Count; i++)
			{
				text = text + "\n    • " + plantRequirementDescriptors[i].text;
			}
			list.Add(new CodexText(text, CodexTextStyle.Body, null));
			list.Add(new CodexSpacer());
		}
		text = "";
		List<Descriptor> plantEffectDescriptors = GameUtil.GetPlantEffectDescriptors(plant);
		if (plantEffectDescriptors.Count > 0)
		{
			text += plantEffectDescriptors[0].text;
			for (int j = 1; j < plantEffectDescriptors.Count; j++)
			{
				text = text + "\n    • " + plantEffectDescriptors[j].text;
			}
			CodexText codexText = new CodexText(text, CodexTextStyle.Body, null);
			list.Add(codexText);
			list.Add(new CodexSpacer());
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static ICodexWidget GetIconWidget(object entity)
	{
		return new CodexImage(32, 32, Def.GetUISprite(entity, "ui", false));
	}

	private static void GenerateCreatureDescriptionContainers(GameObject creature, List<ContentContainer> containers)
	{
		RobotBatteryMonitor.Def def = creature.GetDef<RobotBatteryMonitor.Def>();
		if (def != null)
		{
			Amount batteryAmount = Db.Get().Amounts.Get(def.batteryAmountId);
			float value = Db.Get().traits.Get(creature.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers.Find((AttributeModifier match) => match.AttributeId == batteryAmount.maxAttribute.Id).Value;
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.INTERNALBATTERY, CodexTextStyle.Subtitle, null),
				new CodexText("    • " + string.Format(CODEX.ROBOT_DESCRIPTORS.BATTERY.CAPACITY, value), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Vertical));
		}
		if (creature.GetDef<StorageUnloadMonitor.Def>() != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.INTERNALSTORAGE, CodexTextStyle.Subtitle, null),
				new CodexText("    • " + string.Format(CODEX.ROBOT_DESCRIPTORS.STORAGE.CAPACITY, creature.GetComponents<Storage>()[1].Capacity()), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Vertical));
		}
		List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag((creature.PrefabID().ToString() + "Egg").ToTag());
		if (prefabsWithTag != null && prefabsWithTag.Count > 0)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.HATCHESFROMEGG, CodexTextStyle.Subtitle, null)
			}, ContentContainer.ContentLayout.Vertical));
			foreach (GameObject gameObject in prefabsWithTag)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexIndentedLabelWithIcon(gameObject.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject, "ui", false))
				}, ContentContainer.ContentLayout.Horizontal));
			}
		}
		TemperatureVulnerable component = creature.GetComponent<TemperatureVulnerable>();
		if (component != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.COMFORTRANGE, CodexTextStyle.Subtitle, null),
				new CodexText("    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.COMFORT_RANGE, GameUtil.GetFormattedTemperature(component.TemperatureWarningLow, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(component.TemperatureWarningHigh, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), CodexTextStyle.Body, null),
				new CodexText("    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.NON_LETHAL_RANGE, GameUtil.GetFormattedTemperature(component.TemperatureLethalLow, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(component.TemperatureLethalHigh, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Vertical));
		}
		new List<Tag>();
		CreatureCalorieMonitor.Def def2 = creature.GetDef<CreatureCalorieMonitor.Def>();
		BeehiveCalorieMonitor.Def def3 = creature.GetDef<BeehiveCalorieMonitor.Def>();
		Diet.Info[] array = null;
		if (def2 != null)
		{
			array = def2.diet.infos;
		}
		else if (def3 != null)
		{
			array = def3.diet.infos;
		}
		if (array != null && array.Length != 0)
		{
			float num = 0f;
			foreach (AttributeModifier attributeModifier in Db.Get().traits.Get(creature.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
			{
				if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
				{
					num = attributeModifier.Value;
				}
			}
			List<ICodexWidget> list = new List<ICodexWidget>();
			foreach (Diet.Info info in array)
			{
				if (info.consumedTags.Count != 0)
				{
					foreach (Tag tag in info.consumedTags)
					{
						Element element = ElementLoader.FindElementByHash(ElementLoader.GetElementID(tag));
						if ((element.id != SimHashes.Vacuum && element.id != SimHashes.Void) || !(Assets.GetPrefab(tag) == null))
						{
							float num2 = -num / info.caloriesPerKg;
							float num3 = num2 * info.producedConversionRate;
							list.Add(new CodexConversionPanel(tag.ProperName(), tag, num2, true, info.producedElement, num3, true, creature));
						}
					}
				}
			}
			ContentContainer contentContainer = new ContentContainer(list, ContentContainer.ContentLayout.Vertical);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexCollapsibleHeader(CODEX.HEADERS.DIET, contentContainer)
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(contentContainer);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical));
		}
	}

	private static void GenerateDiseaseDescriptionContainers(Disease disease, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexSpacer());
		StringEntry stringEntry = null;
		if (Strings.TryGet("STRINGS.DUPLICANTS.DISEASES." + disease.Id.ToUpper() + ".DESC", out stringEntry))
		{
			list.Add(new CodexText(stringEntry.String, CodexTextStyle.Body, null));
			list.Add(new CodexSpacer());
		}
		foreach (Descriptor descriptor in disease.GetQuantitativeDescriptors())
		{
			list.Add(new CodexText(descriptor.text, CodexTextStyle.Body, null));
		}
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateFoodDescriptionContainers(EdiblesManager.FoodInfo food, List<ContentContainer> containers)
	{
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(food.Description, CodexTextStyle.Body, null),
			new CodexSpacer(),
			new CodexText(string.Format(UI.CODEX.FOOD.QUALITY, GameUtil.GetFormattedFoodQuality(food.Quality)), CodexTextStyle.Body, null),
			new CodexText(string.Format(UI.CODEX.FOOD.CALORIES, GameUtil.GetFormattedCalories(food.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), CodexTextStyle.Body, null),
			new CodexSpacer(),
			new CodexText(food.CanRot ? string.Format(UI.CODEX.FOOD.SPOILPROPERTIES, GameUtil.GetFormattedTemperature(food.RotTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(food.PreserveTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedCycles(food.SpoilTime, "F1", false)) : UI.CODEX.FOOD.NON_PERISHABLE.ToString(), CodexTextStyle.Body, null),
			new CodexSpacer()
		}, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateTechDescriptionContainers(Tech tech, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText codexText = new CodexText(Strings.Get("STRINGS.RESEARCH.TECHS." + tech.Id.ToUpper() + ".DESC"), CodexTextStyle.Body, null);
		list.Add(codexText);
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateGenericDescriptionContainers(string description, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexText codexText = new CodexText(description, CodexTextStyle.Body, null);
		list.Add(codexText);
		list.Add(new CodexSpacer());
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateBuildingDescriptionContainers(BuildingDef def, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		list.Add(new CodexText(Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".EFFECT"), CodexTextStyle.Body, null));
		list.Add(new CodexSpacer());
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def.BuildingComplete, false);
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			list.Add(new CodexText(CODEX.HEADERS.BUILDINGREQUIREMENTS, CodexTextStyle.Subtitle, null));
			foreach (Descriptor descriptor in requirementDescriptors)
			{
				list.Add(new CodexTextWithTooltip("    " + descriptor.text, descriptor.tooltipText, CodexTextStyle.Body));
			}
			list.Add(new CodexSpacer());
		}
		List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
		if (effectDescriptors.Count > 0)
		{
			list.Add(new CodexText(CODEX.HEADERS.BUILDINGEFFECTS, CodexTextStyle.Subtitle, null));
			foreach (Descriptor descriptor2 in effectDescriptors)
			{
				list.Add(new CodexTextWithTooltip("    " + descriptor2.text, descriptor2.tooltipText, CodexTextStyle.Body));
			}
			list.Add(new CodexSpacer());
		}
		KPrefabID component = def.BuildingComplete.GetComponent<KPrefabID>();
		Pair<Tag, string>[] array = new Pair<Tag, string>[]
		{
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.IndustrialMachinery, CODEX.BUILDING_TYPE.INDUSTRIAL_MACHINERY),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.RecBuilding, ROOMS.CRITERIA.REC_BUILDING.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.Clinic, ROOMS.CRITERIA.CLINIC.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.WashStation, ROOMS.CRITERIA.WASH_STATION.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.AdvancedWashStation, ROOMS.CRITERIA.ADVANCED_WASH_STATION.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.Toilet, ROOMS.CRITERIA.TOILET.NAME),
			new Pair<Tag, string>(RoomConstraints.ConstraintTags.FlushToilet, ROOMS.CRITERIA.FLUSH_TOILET.NAME),
			new Pair<Tag, string>(GameTags.Decoration, ROOMS.CRITERIA.DECORATIVE_ITEM.NAME)
		};
		bool flag = false;
		foreach (Pair<Tag, string> pair in array)
		{
			if (component.HasTag(pair.first))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			list.Add(new CodexText(CODEX.HEADERS.BUILDINGTYPE, CodexTextStyle.Subtitle, null));
			foreach (Pair<Tag, string> pair2 in array)
			{
				if (component.HasTag(pair2.first))
				{
					list.Add(new CodexText("    " + pair2.second, CodexTextStyle.Body, null));
				}
			}
			list.Add(new CodexSpacer());
		}
		list.Add(new CodexText("<i>" + Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".DESC") + "</i>", CodexTextStyle.Body, null));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateImageContainers(Sprite[] sprites, List<ContentContainer> containers, ContentContainer.ContentLayout layout)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		foreach (Sprite sprite in sprites)
		{
			if (!(sprite == null))
			{
				CodexImage codexImage = new CodexImage(128, 128, sprite);
				list.Add(codexImage);
			}
		}
		containers.Add(new ContentContainer(list, layout));
	}

	private static void GenerateImageContainers(global::Tuple<Sprite, Color>[] sprites, List<ContentContainer> containers, ContentContainer.ContentLayout layout)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		foreach (global::Tuple<Sprite, Color> tuple in sprites)
		{
			if (tuple != null)
			{
				CodexImage codexImage = new CodexImage(128, 128, tuple);
				list.Add(codexImage);
			}
		}
		containers.Add(new ContentContainer(list, layout));
	}

	private static void GenerateImageContainers(Sprite sprite, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		CodexImage codexImage = new CodexImage(128, 128, sprite);
		list.Add(codexImage);
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	public static void CreateUnlockablesContentContainer(SubEntry subentry)
	{
		subentry.lockedContentContainer = new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(CODEX.HEADERS.SECTION_UNLOCKABLES, CodexTextStyle.Subtitle, null),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical)
		{
			showBeforeGeneratedContent = false
		};
	}

	private static void GenerateFabricatorContainers(GameObject entity, List<ContentContainer> containers)
	{
		ComplexFabricator component = entity.GetComponent<ComplexFabricator>();
		if (component == null)
		{
			return;
		}
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexSpacer(),
			new CodexText(Strings.Get("STRINGS.CODEX.HEADERS.FABRICATIONS"), CodexTextStyle.Subtitle, null),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical));
		List<ICodexWidget> list = new List<ICodexWidget>();
		foreach (ComplexRecipe complexRecipe in component.GetRecipes())
		{
			list.Add(new CodexRecipePanel(complexRecipe));
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateReceptacleContainers(GameObject entity, List<ContentContainer> containers)
	{
		SingleEntityReceptacle plot = entity.GetComponent<SingleEntityReceptacle>();
		if (plot == null)
		{
			return;
		}
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(Strings.Get("STRINGS.CODEX.HEADERS.RECEPTACLE"), CodexTextStyle.Subtitle, null),
			new CodexDividerLine()
		}, ContentContainer.ContentLayout.Vertical));
		Tag[] possibleDepositObjectTags = plot.possibleDepositObjectTags;
		Predicate<GameObject> <>9__0;
		for (int i = 0; i < possibleDepositObjectTags.Length; i++)
		{
			List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(possibleDepositObjectTags[i]);
			if (plot.rotatable == null)
			{
				List<GameObject> list = prefabsWithTag;
				Predicate<GameObject> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = delegate(GameObject go)
					{
						IReceptacleDirection component = go.GetComponent<IReceptacleDirection>();
						return component != null && component.Direction != plot.Direction;
					});
				}
				list.RemoveAll(predicate);
			}
			foreach (GameObject gameObject in prefabsWithTag)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexImage(64, 64, Def.GetUISprite(gameObject, "ui", false).first),
					new CodexText(gameObject.GetProperName(), CodexTextStyle.Body, null)
				}, ContentContainer.ContentLayout.Horizontal));
			}
		}
	}

	private static string categoryPrefx = "BUILD_CATEGORY_";

	private class ConversionEntry
	{
		public string title;

		public GameObject prefab;

		public HashSet<ElementUsage> inSet = new HashSet<ElementUsage>();

		public HashSet<ElementUsage> outSet = new HashSet<ElementUsage>();
	}

	private class CodexElementMap
	{
		public void Add(Tag t, CodexEntryGenerator.ConversionEntry ce)
		{
			List<CodexEntryGenerator.ConversionEntry> list;
			if (this.map.TryGetValue(t, out list))
			{
				list.Add(ce);
				return;
			}
			this.map[t] = new List<CodexEntryGenerator.ConversionEntry> { ce };
		}

		public Dictionary<Tag, List<CodexEntryGenerator.ConversionEntry>> map = new Dictionary<Tag, List<CodexEntryGenerator.ConversionEntry>>();
	}
}
