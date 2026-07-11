using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class CodexEntryGenerator
{
	public static Dictionary<string, CodexEntry> GenerateBuildingEntries()
	{
		string text = "BUILD_CATEGORY_";
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (PlanScreen.PlanInfo planInfo in global::TUNING.BUILDINGS.PLANORDER)
		{
			string text2 = CodexCache.FormatLinkID(text + planInfo.category.ToString());
			Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
			for (int j = 0; j < (planInfo.data as string[]).Length; j++)
			{
				BuildingDef buildingDef = Assets.GetBuildingDef((planInfo.data as string[])[j]);
				List<ContentContainer> list = new List<ContentContainer>();
				CodexEntryGenerator.GenerateTitleContainers(buildingDef.Name, list);
				CodexEntryGenerator.GenerateImageContainers(buildingDef.GetUISprite("ui", false), list);
				CodexEntryGenerator.GenerateBuildingDescriptionContainers(buildingDef, list);
				CodexEntryGenerator.GenerateFabricatorContainers(buildingDef.BuildingComplete, list);
				CodexEntryGenerator.GenerateReceptacleContainers(buildingDef.BuildingComplete, list);
				CodexEntry codexEntry = new CodexEntry(text2, list, Strings.Get("STRINGS.BUILDINGS.PREFABS." + (planInfo.data as string[])[j].ToUpper() + ".NAME"));
				codexEntry.icon = buildingDef.GetUISprite("ui", false);
				codexEntry.parentId = text2;
				CodexCache.AddEntry((planInfo.data as string[])[j], codexEntry, null);
				dictionary2.Add(codexEntry.id, codexEntry);
			}
			CategoryEntry categoryEntry = CodexEntryGenerator.GenerateCategoryEntry(CodexCache.FormatLinkID(text2), Strings.Get("STRINGS.UI.BUILDCATEGORIES." + planInfo.category.ToString().ToUpper() + ".NAME"), dictionary2, null);
			categoryEntry.parentId = "BUILDINGS";
			categoryEntry.category = "BUILDINGS";
			categoryEntry.icon = Assets.GetSprite(PlanScreen.IconNameMap[planInfo.category]);
			dictionary.Add(text2, categoryEntry);
		}
		CodexEntryGenerator.PopulateCategoryEntries(dictionary);
		return dictionary;
	}

	public static void GeneratePageNotFound()
	{
		List<ContentContainer> list = new List<ContentContainer>();
		ContentContainer contentContainer = new ContentContainer();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				CODEX.PAGENOTFOUND.TITLE
			},
			{ "style", "title" }
		});
		contentContainer.content.Add(codexWidget);
		CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				CODEX.PAGENOTFOUND.SUBTITLE
			},
			{ "style", "subtitle" }
		});
		contentContainer.content.Add(codexWidget2);
		CodexWidget codexWidget3 = new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>());
		contentContainer.content.Add(codexWidget3);
		CodexWidget codexWidget4 = new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
		{
			{ "spriteName", "outhouseMessage" },
			{ "preferredWidth", "312" },
			{ "preferredHeight", "312" }
		});
		contentContainer.content.Add(codexWidget4);
		list.Add(contentContainer);
		CodexCache.AddEntry("PageNotFound", new CodexEntry("ROOT", list, CODEX.PAGENOTFOUND.TITLE)
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
			CodexEntry codexEntry = new CodexEntry("CREATURES", new List<ContentContainer>
			{
				new ContentContainer(new List<CodexWidget>
				{
					new CodexWidget(CodexWidget.ContentType.Spacer),
					new CodexWidget(CodexWidget.ContentType.Spacer)
				}, ContentContainer.ContentLayout.Vertical)
			}, name);
			codexEntry.parentId = "CREATURES";
			CodexCache.AddEntry(speciesTag.ToString(), codexEntry, null);
			results.Add(speciesTag.ToString(), codexEntry);
			foreach (GameObject gameObject in brains)
			{
				if (gameObject.GetDef<BabyMonitor.Def>() == null)
				{
					Sprite sprite = null;
					GameObject gameObject2 = Assets.TryGetPrefab(gameObject.PrefabID() + "Baby");
					if (gameObject2 != null)
					{
						sprite = Def.GetUISprite(gameObject2, "ui", false).first;
					}
					CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
					if (!(component.species != speciesTag))
					{
						List<ContentContainer> list = new List<ContentContainer>();
						string symbolPrefix = component.symbolPrefix;
						Sprite first = Def.GetUISprite(gameObject, symbolPrefix + "ui", false).first;
						if (sprite)
						{
							CodexEntryGenerator.GenerateImageContainers(new Sprite[] { first, sprite }, list, ContentContainer.ContentLayout.Horizontal);
						}
						else
						{
							CodexEntryGenerator.GenerateImageContainers(first, list);
						}
						CodexEntryGenerator.GenerateCreatureDescriptionContainers(gameObject, list);
						SubEntry subEntry = new SubEntry(component.PrefabID().ToString(), speciesTag.ToString(), list, component.GetProperName());
						subEntry.icon = first;
						subEntry.iconColor = Color.white;
						codexEntry.subEntries.Add(subEntry);
					}
				}
			}
		};
		action(GameTags.Creatures.Species.PuftSpecies, global::STRINGS.CREATURES.FAMILY.PUFT);
		action(GameTags.Creatures.Species.PacuSpecies, global::STRINGS.CREATURES.FAMILY.PACU);
		action(GameTags.Creatures.Species.OilFloaterSpecies, global::STRINGS.CREATURES.FAMILY.OILFLOATER);
		action(GameTags.Creatures.Species.LightBugSpecies, global::STRINGS.CREATURES.FAMILY.LIGHTBUG);
		action(GameTags.Creatures.Species.HatchSpecies, global::STRINGS.CREATURES.FAMILY.HATCH);
		action(GameTags.Creatures.Species.GlomSpecies, global::STRINGS.CREATURES.FAMILY.GLOM);
		action(GameTags.Creatures.Species.DreckoSpecies, global::STRINGS.CREATURES.FAMILY.DRECKO);
		action(GameTags.Creatures.Species.MooSpecies, global::STRINGS.CREATURES.FAMILY.MOO);
		return results;
	}

	public static Dictionary<string, CodexEntry> GeneratePlantEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Harvestable>();
		prefabsWithComponent.AddRange(Assets.GetPrefabsWithComponent<WiltCondition>());
		foreach (GameObject gameObject in prefabsWithComponent)
		{
			if (!dictionary.ContainsKey(gameObject.PrefabID().ToString()))
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
		foreach (EdiblesManager.FoodInfo foodInfo in FOOD.FOOD_TYPES_LIST)
		{
			if (!(Assets.GetPrefab(foodInfo.Id).GetComponent<IncubatableEgg>() != null))
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
			TechItem techItem = tech.unlockedItems[0];
			if (techItem == null)
			{
				Output.LogError(new object[] { "Unknown tech:", tech.Name });
			}
			codexEntry.icon = techItem.getUISprite("ui", false);
			codexEntry.parentId = "TECH";
			CodexCache.AddEntry(tech.Id, codexEntry, null);
			dictionary.Add(tech.Id, codexEntry);
		}
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateRoleEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			Sprite sprite = Assets.GetSprite(roleConfig.hat);
			CodexEntryGenerator.GenerateTitleContainers(roleConfig.name, list);
			CodexEntryGenerator.GenerateImageContainers(sprite, list);
			CodexEntryGenerator.GenerateGenericDescriptionContainers(roleConfig.description, list);
			CodexEntryGenerator.GenerateRelatedRoleContainers(roleConfig, list);
			CodexEntryGenerator.GenerateRoleRequirementsAndPerksContainers(roleConfig, list);
			CodexEntry codexEntry = new CodexEntry("ROLES", list, roleConfig.name);
			codexEntry.parentId = "ROLES";
			codexEntry.icon = sprite;
			CodexCache.AddEntry(roleConfig.id, codexEntry, null);
			dictionary.Add(roleConfig.id, codexEntry);
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
					List<CodexWidget> list2 = new List<CodexWidget>();
					string text = gameObject.PrefabID().ToString();
					text = text.Remove(0, 14).ToUpper();
					list2.Add(new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
					{
						{
							"stringKey",
							"STRINGS.CREATURES.SPECIES.GEYSER." + text + ".DESC"
						},
						{ "style", "body" }
					}));
					list2.Add(new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
					{
						{
							"string",
							UI.CODEX.GEYSERS.DESC
						},
						{ "style", "body" }
					}));
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
		Action<Element, List<ContentContainer>> action = delegate(Element element, List<ContentContainer> containers)
		{
			if (element.highTempTransition != null || element.lowTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<CodexWidget>
				{
					new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
					{
						{
							"string",
							CODEX.HEADERS.ELEMENTTRANSITIONS
						},
						{ "style", "subtitle" }
					}),
					new CodexWidget(CodexWidget.ContentType.DividerLine)
				}, ContentContainer.ContentLayout.Vertical));
			}
			if (element.highTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<CodexWidget>
				{
					new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
					{
						{ "preferredWidth", "32" },
						{ "preferredHeight", "32" }
					}, new Dictionary<string, object> { 
					{
						"coloredSprite",
						Def.GetUISprite(element.highTempTransition, "ui", false)
					} }),
					new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
					{
						{
							"string",
							(element.highTempTransition == null) ? string.Empty : string.Concat(new string[]
							{
								element.highTempTransition.name,
								" (",
								element.highTempTransition.GetStateString(),
								")  (",
								GameUtil.GetFormattedTemperature(element.highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true),
								")"
							})
						},
						{ "style", "body" }
					})
				}, ContentContainer.ContentLayout.Horizontal));
			}
			if (element.lowTempTransition != null)
			{
				containers.Add(new ContentContainer(new List<CodexWidget>
				{
					new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
					{
						{ "preferredWidth", "32" },
						{ "preferredHeight", "32" }
					}, new Dictionary<string, object> { 
					{
						"coloredSprite",
						Def.GetUISprite(element.lowTempTransition, "ui", false)
					} }),
					new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
					{
						{
							"string",
							(element.lowTempTransition == null) ? string.Empty : string.Concat(new string[]
							{
								element.lowTempTransition.name,
								" (",
								element.lowTempTransition.GetStateString(),
								")  (",
								GameUtil.GetFormattedTemperature(element.lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true),
								")"
							})
						},
						{ "style", "body" }
					})
				}, ContentContainer.ContentLayout.Horizontal));
			}
			containers.Add(new ContentContainer(new List<CodexWidget>
			{
				new CodexWidget(CodexWidget.ContentType.Spacer),
				new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						element.FullDescription(true)
					},
					{ "style", "body" }
				}),
				new CodexWidget(CodexWidget.ContentType.Spacer)
			}, ContentContainer.ContentLayout.Vertical));
		};
		string text7;
		foreach (Element element2 in ElementLoader.elements)
		{
			if (!element2.disabled)
			{
				List<ContentContainer> list = new List<ContentContainer>();
				string text6 = element2.name + " (" + element2.GetStateString() + ")";
				CodexEntryGenerator.GenerateTitleContainers(text6, list);
				Tuple<Sprite, Color> uisprite = Def.GetUISprite(element2, "ui", false);
				CodexEntryGenerator.GenerateImageContainers(new Tuple<Sprite, Color>[] { Def.GetUISprite(element2, "ui", false) }, list, ContentContainer.ContentLayout.Horizontal);
				action(element2, list);
				text7 = element2.id.ToString();
				string text8;
				Dictionary<string, CodexEntry> dictionary6;
				if (element2.IsSolid)
				{
					text8 = text2;
					dictionary6 = dictionary2;
				}
				else if (element2.IsLiquid)
				{
					text8 = text3;
					dictionary6 = dictionary3;
				}
				else if (element2.IsGas)
				{
					text8 = text4;
					dictionary6 = dictionary4;
				}
				else
				{
					text8 = text5;
					dictionary6 = dictionary5;
				}
				CodexEntry codexEntry = new CodexEntry(text8, list, text6);
				codexEntry.parentId = text8;
				codexEntry.icon = uisprite.first;
				codexEntry.iconColor = uisprite.second;
				CodexCache.AddEntry(text7, codexEntry, null);
				dictionary6.Add(text7, codexEntry);
			}
		}
		text7 = text2;
		CodexEntry codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, dictionary2, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text7, codexEntry2);
		text7 = text3;
		codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, dictionary3, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text7, codexEntry2);
		text7 = text4;
		codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, dictionary4, null);
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text7, codexEntry2);
		text7 = text5;
		codexEntry2 = CodexEntryGenerator.GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, dictionary5, Assets.GetSprite("overlay_heatflow"));
		codexEntry2.parentId = text;
		codexEntry2.category = text;
		dictionary.Add(text7, codexEntry2);
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
				CodexCache.AddEntry(disease.Id, codexEntry, null);
			}
		}
		return dictionary;
	}

	public static CategoryEntry GenerateCategoryEntry(string id, string name, Dictionary<string, CodexEntry> entries, Sprite icon = null)
	{
		List<ContentContainer> list = new List<ContentContainer>();
		CodexEntryGenerator.GenerateTitleContainers(name, list);
		List<CodexEntry> list2 = new List<CodexEntry>();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in entries)
		{
			list2.Add(keyValuePair.Value);
			if (icon == null)
			{
				icon = keyValuePair.Value.icon;
			}
		}
		CategoryEntry categoryEntry = new CategoryEntry("Root", list, name, list2);
		categoryEntry.icon = icon;
		CodexCache.AddEntry(id, categoryEntry, null);
		return categoryEntry;
	}

	public static void PopulateCategoryEntries(Dictionary<string, CodexEntry> categoryEntries)
	{
		List<CategoryEntry> list = new List<CategoryEntry>();
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in categoryEntries)
		{
			list.Add(keyValuePair.Value as CategoryEntry);
		}
		CodexEntryGenerator.PopulateCategoryEntries(list);
	}

	public static void PopulateCategoryEntries(List<CategoryEntry> categoryEntries)
	{
		foreach (CategoryEntry categoryEntry in categoryEntries)
		{
			List<ContentContainer> contentContainers = categoryEntry.contentContainers;
			List<CodexEntry> list = new List<CodexEntry>();
			foreach (CodexEntry codexEntry in categoryEntry.entriesInCategory)
			{
				list.Add(codexEntry);
			}
			list.Sort((CodexEntry a, CodexEntry b) => UI.StripLinkFormatting(a.name).CompareTo(UI.StripLinkFormatting(b.name)));
			foreach (CodexEntry codexEntry2 in list)
			{
				ContentContainer contentContainer = new ContentContainer(new List<CodexWidget>(), ContentContainer.ContentLayout.Horizontal);
				if (codexEntry2.icon != null)
				{
					CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
					{
						{ "preferredHeight", "48" },
						{ "preferredWidth", "48" }
					});
					codexWidget.objectProperties.Add("coloredSprite", new Tuple<Sprite, Color>(codexEntry2.icon, codexEntry2.iconColor));
					contentContainer.content.Add(codexWidget);
				}
				CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{ "string", codexEntry2.name },
					{ "style", "body" }
				});
				contentContainer.content.Add(codexWidget2);
				contentContainers.Add(contentContainer);
			}
		}
	}

	private static void GenerateTitleContainers(string name, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{ "string", name },
			{ "style", "title" }
		});
		list.Add(codexWidget);
		CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>());
		list.Add(codexWidget2);
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GeneratePrerequisiteTechContainers(Tech tech, List<ContentContainer> containers)
	{
		if (tech.requiredTech == null || tech.requiredTech.Count == 0)
		{
			return;
		}
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				CODEX.HEADERS.PREREQUISITE_TECH
			},
			{ "style", "subtitle" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>()));
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		foreach (Tech tech2 in tech.requiredTech)
		{
			CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", tech2.Name },
				{ "style", "body" }
			});
			list.Add(codexWidget2);
		}
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateRelatedRoleContainers(RoleConfig role, List<ContentContainer> containers)
	{
		bool flag = false;
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				CODEX.HEADERS.PREREQUISITE_ROLES
			},
			{ "style", "subtitle" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>()));
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		foreach (RoleAssignmentRequirement roleAssignmentRequirement in role.requirements)
		{
			if (roleAssignmentRequirement is PreviousRoleAssignmentRequirement)
			{
				CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						Game.Instance.roleManager.GetRole((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID).name
					},
					{ "style", "body" }
				});
				list.Add(codexWidget2);
				flag = true;
			}
		}
		if (flag)
		{
			list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
			containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		}
		bool flag2 = false;
		List<CodexWidget> list2 = new List<CodexWidget>();
		CodexWidget codexWidget3 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				CODEX.HEADERS.UNLOCK_ROLES
			},
			{ "style", "subtitle" }
		});
		list2.Add(codexWidget3);
		list2.Add(new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>()));
		list2.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			foreach (RoleAssignmentRequirement roleAssignmentRequirement2 in roleConfig.requirements)
			{
				if (roleAssignmentRequirement2 is PreviousRoleAssignmentRequirement && (roleAssignmentRequirement2 as PreviousRoleAssignmentRequirement).previousRoleID == role.id)
				{
					CodexWidget codexWidget4 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
					{
						{ "string", roleConfig.name },
						{ "style", "body" }
					});
					list2.Add(codexWidget4);
					flag2 = true;
				}
			}
		}
		if (flag2)
		{
			list2.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
		}
	}

	private static void GenerateRoleRequirementsAndPerksContainers(RoleConfig role, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				CODEX.HEADERS.ROLE_PERKS
			},
			{ "style", "subtitle" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>()));
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		foreach (RolePerk rolePerk in role.perks)
		{
			CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", rolePerk.description },
				{ "style", "body" }
			});
			list.Add(codexWidget2);
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateUnlockContainers(Tech tech, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				CODEX.HEADERS.TECH_UNLOCKS
			},
			{ "style", "subtitle" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>()));
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		foreach (TechItem techItem in tech.unlockedItems)
		{
			List<CodexWidget> list2 = new List<CodexWidget>();
			list2.Add(new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
			{
				{ "preferredWidth", "64" },
				{ "preferredHeight", "64" }
			})
			{
				objectProperties = { 
				{
					"sprite",
					techItem.getUISprite("ui", false)
				} }
			});
			CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", techItem.Name },
				{ "style", "body" }
			});
			list2.Add(codexWidget2);
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
		containers.Add(new ContentContainer(new List<CodexWidget>
		{
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					CODEX.HEADERS.RECIPE
				},
				{ "style", "subtitle" }
			}),
			new CodexWidget(CodexWidget.ContentType.Spacer),
			new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>())
		}, ContentContainer.ContentLayout.Vertical));
		Func<Recipe, List<ContentContainer>> func = delegate(Recipe rec)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			foreach (Recipe.Ingredient ingredient in rec.Ingredients)
			{
				GameObject prefab = Assets.GetPrefab(ingredient.tag);
				if (prefab != null)
				{
					list.Add(new ContentContainer(new List<CodexWidget>
					{
						new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
						{
							{ "preferredWidth", "64" },
							{ "preferredHeight", "64" }
						}, new Dictionary<string, object> { 
						{
							"coloredSprite",
							Def.GetUISprite(prefab, "ui", false)
						} }),
						new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
						{
							{
								"string",
								string.Format(UI.CODEX.RECIPE_ITEM, Assets.GetPrefab(ingredient.tag).GetProperName(), ingredient.amount, (ElementLoader.GetElement(ingredient.tag) != null) ? UI.UNITSUFFIXES.MASS.KILOGRAM.text : string.Empty)
							},
							{ "style", "body" }
						})
					}, ContentContainer.ContentLayout.Horizontal));
				}
			}
			return list;
		};
		containers.AddRange(func(recipe));
		GameObject gameObject = ((recipe.fabricators != null) ? Assets.GetPrefab(recipe.fabricators[0]) : null);
		if (gameObject != null)
		{
			containers.Add(new ContentContainer(new List<CodexWidget>
			{
				new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						UI.CODEX.RECIPE_FABRICATOR_HEADER
					},
					{ "style", "subtitle" }
				}),
				new CodexWidget(CodexWidget.ContentType.DividerLine)
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(new ContentContainer(new List<CodexWidget>
			{
				new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
				{
					{ "preferredWidth", "64" },
					{ "preferredHeight", "64" }
				}, new Dictionary<string, object> { 
				{
					"sprite",
					Def.GetUISpriteFromMultiObjectAnim(gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false)
				} }),
				new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string> { 
				{
					"string",
					string.Format(UI.CODEX.RECIPE_FABRICATOR, recipe.FabricationTime, gameObject.GetProperName())
				} })
			}, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GenerateUsedInRecipeContainers(Tag prefabID, List<ContentContainer> containers)
	{
		List<Recipe> list = new List<Recipe>();
		foreach (Recipe recipe in RecipeManager.Get().recipes)
		{
			foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
			{
				if (ingredient.tag == prefabID)
				{
					list.Add(recipe);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		containers.Add(new ContentContainer(new List<CodexWidget>
		{
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					CODEX.HEADERS.USED_IN_RECIPES
				},
				{ "style", "subtitle" }
			}),
			new CodexWidget(CodexWidget.ContentType.Spacer),
			new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>())
		}, ContentContainer.ContentLayout.Vertical));
		foreach (Recipe recipe2 in list)
		{
			GameObject prefab = Assets.GetPrefab(recipe2.Result);
			containers.Add(new ContentContainer(new List<CodexWidget>
			{
				new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
				{
					{ "preferredWidth", "64" },
					{ "preferredHeight", "64" }
				}, new Dictionary<string, object> { 
				{
					"coloredSprite",
					Def.GetUISprite(prefab, "ui", false)
				} }),
				new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						prefab.GetProperName()
					},
					{ "style", "body" }
				})
			}, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GeneratePlantDescriptionContainers(GameObject plant, List<ContentContainer> containers)
	{
		SeedProducer component = plant.GetComponent<SeedProducer>();
		GameObject prefab = Assets.GetPrefab(component.seedInfo.seedId);
		containers.Add(new ContentContainer(new List<CodexWidget>
		{
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					CODEX.HEADERS.HATCHESFROMEGG
				},
				{ "style", "subtitle" }
			}),
			new CodexWidget(CodexWidget.ContentType.DividerLine)
		}, ContentContainer.ContentLayout.Vertical));
		containers.Add(new ContentContainer(new List<CodexWidget>
		{
			new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
			{
				{ "preferredHeight", "48" },
				{ "preferredWidth", "48" }
			}, new Dictionary<string, object> { 
			{
				"coloredSprite",
				Def.GetUISprite(prefab, "ui", false)
			} }),
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					prefab.GetProperName()
				},
				{ "style", "body" }
			})
		}, ContentContainer.ContentLayout.Horizontal));
		List<CodexWidget> list = new List<CodexWidget>();
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				UI.CODEX.DETAILS
			},
			{ "style", "subtitle" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.DividerLine));
		InfoDescription component2 = Assets.GetPrefab(plant.PrefabID()).GetComponent<InfoDescription>();
		if (component2 != null)
		{
			CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", component2.description },
				{ "style", "body" }
			});
			list.Add(codexWidget2);
		}
		string text = string.Empty;
		List<Descriptor> plantRequirementDescriptors = GameUtil.GetPlantRequirementDescriptors(plant);
		if (plantRequirementDescriptors.Count > 0)
		{
			text += plantRequirementDescriptors[0].text;
			for (int i = 1; i < plantRequirementDescriptors.Count; i++)
			{
				text = text + "\n    • " + plantRequirementDescriptors[i].text;
			}
			CodexWidget codexWidget3 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", text },
				{ "style", "body" }
			});
			list.Add(codexWidget3);
			list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		}
		text = string.Empty;
		List<Descriptor> plantEffectDescriptors = GameUtil.GetPlantEffectDescriptors(plant);
		if (plantEffectDescriptors.Count > 0)
		{
			text += plantEffectDescriptors[0].text;
			for (int j = 1; j < plantEffectDescriptors.Count; j++)
			{
				text = text + "\n    • " + plantEffectDescriptors[j].text;
			}
			CodexWidget codexWidget4 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", text },
				{ "style", "body" }
			});
			list.Add(codexWidget4);
			list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static CodexWidget GetIconWidget(object entity)
	{
		return new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
		{
			{ "preferredHeight", "32" },
			{ "preferredWidth", "32" }
		}, new Dictionary<string, object> { 
		{
			"coloredSprite",
			Def.GetUISprite(entity, "ui", false)
		} });
	}

	private static void GenerateCreatureDescriptionContainers(GameObject creature, List<ContentContainer> containers)
	{
		CreatureCalorieMonitor.Def def = creature.GetDef<CreatureCalorieMonitor.Def>();
		if (def != null)
		{
			List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag((creature.PrefabID() + "Egg").ToTag());
			if (prefabsWithTag != null && prefabsWithTag.Count > 0)
			{
				containers.Add(new ContentContainer(new List<CodexWidget>
				{
					new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
					{
						{
							"string",
							CODEX.HEADERS.HATCHESFROMEGG
						},
						{ "style", "subtitle" }
					}),
					new CodexWidget(CodexWidget.ContentType.DividerLine)
				}, ContentContainer.ContentLayout.Vertical));
				foreach (GameObject gameObject in prefabsWithTag)
				{
					containers.Add(new ContentContainer(new List<CodexWidget>
					{
						CodexEntryGenerator.GetIconWidget(gameObject),
						new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
						{
							{
								"string",
								gameObject.GetProperName()
							},
							{ "style", "body" }
						})
					}, ContentContainer.ContentLayout.Horizontal));
				}
			}
			TemperatureVulnerable component = creature.GetComponent<TemperatureVulnerable>();
			containers.Add(new ContentContainer(new List<CodexWidget>
			{
				new CodexWidget(CodexWidget.ContentType.Spacer),
				new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						CODEX.HEADERS.COMFORTRANGE
					},
					{ "style", "subtitle" }
				}),
				new CodexWidget(CodexWidget.ContentType.DividerLine),
				new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						"    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.COMFORT_RANGE, GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true) + " - " + GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true))
					},
					{ "style", "body" }
				}),
				new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						"    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.NON_LETHAL_RANGE, GameUtil.GetFormattedTemperature(component.internalTemperatureLethal_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true) + " - " + GameUtil.GetFormattedTemperature(component.internalTemperatureLethal_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true))
					},
					{ "style", "body" }
				}),
				new CodexWidget(CodexWidget.ContentType.Spacer)
			}, ContentContainer.ContentLayout.Vertical));
			List<Tag> list = new List<Tag>();
			if (def.diet.infos.Length > 0)
			{
				if (list.Count == 0)
				{
					containers.Add(new ContentContainer(new List<CodexWidget>
					{
						new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
						{
							{
								"string",
								CODEX.HEADERS.DIET
							},
							{ "style", "subtitle" }
						}),
						new CodexWidget(CodexWidget.ContentType.DividerLine)
					}, ContentContainer.ContentLayout.Vertical));
				}
				ContentContainer contentContainer = new ContentContainer();
				contentContainer.contentLayout = ContentContainer.ContentLayout.Grid;
				contentContainer.content = new List<CodexWidget>();
				foreach (Diet.Info info in def.diet.infos)
				{
					if (info.consumedTags.Count != 0)
					{
						foreach (Tag tag in info.consumedTags)
						{
							Element element = ElementLoader.FindElementByHash(ElementLoader.GetElementID(tag));
							GameObject gameObject2 = null;
							if (element.id == SimHashes.Vacuum || element.id == SimHashes.Void)
							{
								gameObject2 = Assets.GetPrefab(tag);
								if (gameObject2 == null)
								{
									continue;
								}
							}
							if (element != null && gameObject2 == null)
							{
								if (!list.Contains(element.tag))
								{
									list.Add(element.tag);
									contentContainer.content.Add(new CodexWidget(CodexWidget.ContentType.LabelWithIcon, new Dictionary<string, string>
									{
										{
											"string",
											"    " + element.name
										},
										{ "style", "body" }
									}, new Dictionary<string, object> { 
									{
										"coloredSprite",
										Def.GetUISprite(element.substance, "ui", false)
									} }));
								}
							}
							else if (gameObject2 != null)
							{
								if (!list.Contains(gameObject2.PrefabID()))
								{
									list.Add(gameObject2.PrefabID());
									contentContainer.content.Add(new CodexWidget(CodexWidget.ContentType.LabelWithIcon, new Dictionary<string, string>
									{
										{
											"string",
											"    " + gameObject2.GetProperName()
										},
										{ "style", "body" }
									}, new Dictionary<string, object> { 
									{
										"coloredSprite",
										Def.GetUISprite(gameObject2, "ui", false)
									} }));
								}
							}
						}
					}
				}
				containers.Add(contentContainer);
			}
			bool flag = false;
			if (def.diet != null)
			{
				foreach (Diet.Info info2 in def.diet.infos)
				{
					if (info2.producedElement != null)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					ContentContainer contentContainer2 = new ContentContainer();
					contentContainer2.contentLayout = ContentContainer.ContentLayout.Grid;
					contentContainer2.content = new List<CodexWidget>();
					ContentContainer contentContainer3 = new ContentContainer(new List<CodexWidget>
					{
						new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
						{
							{
								"string",
								CODEX.HEADERS.PRODUCES
							},
							{ "style", "subtitle" }
						}),
						new CodexWidget(CodexWidget.ContentType.DividerLine)
					}, ContentContainer.ContentLayout.Vertical);
					containers.Add(contentContainer3);
					List<Tag> list2 = new List<Tag>();
					for (int k = 0; k < def.diet.infos.Length; k++)
					{
						if (def.diet.infos[k].producedElement != Tag.Invalid)
						{
							if (!list2.Contains(def.diet.infos[k].producedElement))
							{
								list2.Add(def.diet.infos[k].producedElement);
								contentContainer2.content.Add(new CodexWidget(CodexWidget.ContentType.LabelWithIcon, new Dictionary<string, string>
								{
									{
										"string",
										"• " + def.diet.infos[k].producedElement.ProperName()
									},
									{ "style", "body" }
								}, new Dictionary<string, object> { 
								{
									"coloredSprite",
									Def.GetUISprite(def.diet.infos[k].producedElement, "ui", false)
								} }));
							}
						}
					}
					containers.Add(contentContainer2);
				}
			}
		}
	}

	private static void GenerateDiseaseDescriptionContainers(Disease disease, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		foreach (Descriptor descriptor in disease.GetQualitativeDescriptors())
		{
			CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", descriptor.text },
				{ "style", "body" }
			});
			list.Add(codexWidget);
		}
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		foreach (Descriptor descriptor2 in disease.GetQuantitativeDescriptors())
		{
			CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", descriptor2.text },
				{ "style", "body" }
			});
			list.Add(codexWidget2);
		}
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateFoodDescriptionContainers(EdiblesManager.FoodInfo food, List<ContentContainer> containers)
	{
		containers.Add(new ContentContainer(new List<CodexWidget>
		{
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"stringKey",
					"STRINGS.ITEMS.FOOD." + food.ConsumableId.ToUpper() + ".DESC"
				},
				{ "style", "body" }
			}),
			new CodexWidget(CodexWidget.ContentType.Spacer),
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					string.Format(UI.CODEX.FOOD.QUALITY, GameUtil.GetFormattedFoodQuality(food.Quality))
				},
				{ "style", "body" }
			}),
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					string.Format(UI.CODEX.FOOD.CALORIES, GameUtil.GetFormattedCalories(food.CaloriesPerUnit, GameUtil.TimeSlice.None, true))
				},
				{ "style", "body" }
			}),
			new CodexWidget(CodexWidget.ContentType.Spacer),
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					(!food.CanRot) ? UI.CODEX.FOOD.NON_PERISHABLE.ToString() : string.Format(UI.CODEX.FOOD.SPOILPROPERTIES, GameUtil.GetFormattedTemperature(food.PreserveTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedCycles(food.SpoilTime, "F1"))
				},
				{ "style", "body" }
			}),
			new CodexWidget(CodexWidget.ContentType.Spacer)
		}, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateTechDescriptionContainers(Tech tech, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"stringKey",
				"STRINGS.RESEARCH.TECHS." + tech.Id.ToUpper() + ".DESC"
			},
			{ "style", "body" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateGenericDescriptionContainers(string description, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{ "string", description },
			{ "style", "body" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateBuildingDescriptionContainers(BuildingDef def, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"stringKey",
				"STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".EFFECT"
			},
			{ "style", "body" }
		});
		list.Add(codexWidget);
		CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"stringKey",
				"STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".DESC"
			},
			{ "style", "body" }
		});
		list.Add(codexWidget2);
		Tech tech = Db.Get().TechItems.LookupGroupForID(def.PrefabID);
		if (tech != null)
		{
			CodexWidget codexWidget3 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, tech.Name)
				},
				{ "style", "body" }
			});
			list.Add(codexWidget3);
		}
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def);
		List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
		if (effectDescriptors.Count > 0)
		{
			CodexWidget codexWidget4 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					CODEX.HEADERS.BUILDINGEFFECTS
				},
				{ "style", "subtitle" }
			});
			list.Add(codexWidget4);
			list.Add(new CodexWidget(CodexWidget.ContentType.DividerLine));
			foreach (Descriptor descriptor in effectDescriptors)
			{
				CodexWidget codexWidget5 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{ "string", descriptor.text },
					{ "style", "body" }
				});
				list.Add(codexWidget5);
			}
			list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		}
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			CodexWidget codexWidget6 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					CODEX.HEADERS.BUILDINGREQUIREMENTS
				},
				{ "style", "subtitle" }
			});
			list.Add(codexWidget6);
			list.Add(new CodexWidget(CodexWidget.ContentType.DividerLine));
			foreach (Descriptor descriptor2 in requirementDescriptors)
			{
				CodexWidget codexWidget7 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{ "string", descriptor2.text },
					{ "style", "body" }
				});
				list.Add(codexWidget7);
			}
			list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		}
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateImageContainers(Sprite[] sprites, List<ContentContainer> containers, ContentContainer.ContentLayout layout)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		foreach (Sprite sprite in sprites)
		{
			if (!(sprite == null))
			{
				list.Add(new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
				{
					{ "preferredWidth", "128" },
					{ "preferredHeight", "128" }
				})
				{
					objectProperties = { { "sprite", sprite } }
				});
			}
		}
		containers.Add(new ContentContainer(list, layout));
	}

	private static void GenerateImageContainers(Tuple<Sprite, Color>[] sprites, List<ContentContainer> containers, ContentContainer.ContentLayout layout)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		foreach (Tuple<Sprite, Color> tuple in sprites)
		{
			if (tuple != null)
			{
				list.Add(new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
				{
					{ "preferredWidth", "128" },
					{ "preferredHeight", "128" }
				})
				{
					objectProperties = { { "coloredSprite", tuple } }
				});
			}
		}
		containers.Add(new ContentContainer(list, layout));
	}

	private static void GenerateImageContainers(Sprite sprite, List<ContentContainer> containers)
	{
		containers.Add(new ContentContainer(new List<CodexWidget>
		{
			new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
			{
				{ "preferredWidth", "128" },
				{ "preferredHeight", "128" }
			})
			{
				objectProperties = { { "sprite", sprite } }
			}
		}, ContentContainer.ContentLayout.Vertical));
	}

	public static void CreateUnlockablesContentContainer(SubEntry subentry)
	{
		subentry.lockedContentContainer = new ContentContainer(new List<CodexWidget>
		{
			new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					CODEX.HEADERS.SECTION_UNLOCKABLES
				},
				{ "style", "subtitle" }
			}),
			new CodexWidget(CodexWidget.ContentType.DividerLine)
		}, ContentContainer.ContentLayout.Vertical)
		{
			showBeforeGeneratedContent = false
		};
	}

	private static void GenerateFabricatorContainers(GameObject entity, List<ContentContainer> containers)
	{
		Fabricator component = entity.GetComponent<Fabricator>();
		if (component == null)
		{
			return;
		}
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{ "stringKey", "STRINGS.CODEX.HEADERS.FABRICATIONS" },
			{ "style", "subtitle" }
		});
		list.Add(codexWidget);
		CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>());
		list.Add(codexWidget2);
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		foreach (Recipe recipe in component.GetRecipes())
		{
			List<CodexWidget> list2 = new List<CodexWidget>();
			CodexWidget codexWidget3 = new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
			{
				{ "preferredWidth", "64" },
				{ "preferredHeight", "64" }
			});
			GameObject prefab = Assets.GetPrefab(recipe.Result);
			codexWidget3.objectProperties.Add("coloredSprite", Def.GetUISprite(prefab, "ui", false));
			list2.Add(codexWidget3);
			CodexWidget codexWidget4 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{
					"string",
					prefab.GetProperName()
				},
				{ "style", "body" }
			});
			list2.Add(codexWidget4);
			containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Horizontal));
		}
	}

	private static void GenerateReceptacleContainers(GameObject entity, List<ContentContainer> containers)
	{
		SingleEntityReceptacle plot = entity.GetComponent<SingleEntityReceptacle>();
		if (plot == null)
		{
			return;
		}
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{ "stringKey", "STRINGS.CODEX.HEADERS.RECEPTACLE" },
			{ "style", "subtitle" }
		});
		list.Add(codexWidget);
		CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>());
		list.Add(codexWidget2);
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
		foreach (Tag tag in plot.possibleDepositObjectTags)
		{
			List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(tag);
			if (plot.rotatable == null)
			{
				prefabsWithTag.RemoveAll(delegate(GameObject go)
				{
					IReceptacleDirection component = go.GetComponent<IReceptacleDirection>();
					return component != null && component.Direction != plot.Direction;
				});
			}
			foreach (GameObject gameObject in prefabsWithTag)
			{
				List<CodexWidget> list2 = new List<CodexWidget>();
				CodexWidget codexWidget3 = new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
				{
					{ "preferredWidth", "64" },
					{ "preferredHeight", "64" }
				});
				Sprite first = Def.GetUISprite(gameObject, "ui", false).first;
				codexWidget3.objectProperties.Add("sprite", first);
				list2.Add(codexWidget3);
				CodexWidget codexWidget4 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						gameObject.GetProperName()
					},
					{ "style", "body" }
				});
				list2.Add(codexWidget4);
				containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Horizontal));
			}
		}
	}
}
