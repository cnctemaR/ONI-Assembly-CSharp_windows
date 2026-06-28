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
				CodexEntryGenerator.GenerateImageContainers(buildingDef.GetUISprite("ui"), list);
				CodexEntryGenerator.GenerateBuildingDescriptionContainers(buildingDef, list);
				CodexEntryGenerator.GenerateFabricatorContainers(buildingDef.BuildingComplete, list);
				CodexEntryGenerator.GenerateReceptacleContainers(buildingDef.BuildingComplete, list);
				CodexEntry codexEntry = new CodexEntry(text2, list, Strings.Get("STRINGS.BUILDINGS.PREFABS." + (planInfo.data as string[])[j].ToUpper() + ".NAME"));
				codexEntry.icon = buildingDef.GetUISprite("ui");
				codexEntry.parentId = text2;
				CodexCache.AddEntry((planInfo.data as string[])[j], codexEntry, null);
				dictionary2.Add(codexEntry.id, codexEntry);
			}
			CategoryEntry categoryEntry = CodexEntryGenerator.GenerateCategoryEntry(CodexCache.FormatLinkID(text2), Strings.Get("STRINGS.UI.BUILDCATEGORIES." + planInfo.category.ToString().ToUpper() + ".NAME"), dictionary2);
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
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Trappable>();
		prefabsWithComponent.AddRange(Assets.GetPrefabsWithTag("CreatureBrain"));
		prefabsWithComponent.Add(Assets.GetPrefab("Puft".ToTag()));
		foreach (GameObject gameObject in prefabsWithComponent)
		{
			if (!dictionary.ContainsKey(gameObject.PrefabID().ToString()))
			{
				List<ContentContainer> list = new List<ContentContainer>();
				Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui");
				CodexEntryGenerator.GenerateImageContainers(uispriteFromMultiObjectAnim, list);
				CodexEntryGenerator.GenerateCreatureDescriptionContainers(gameObject, list);
				CodexEntry codexEntry = new CodexEntry("CREATURES", list, gameObject.GetProperName());
				codexEntry.icon = uispriteFromMultiObjectAnim;
				codexEntry.parentId = "CREATURES";
				CodexCache.AddEntry(gameObject.PrefabID().ToString(), codexEntry, null);
				dictionary.Add(gameObject.PrefabID().ToString(), codexEntry);
			}
		}
		return dictionary;
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
				Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui");
				CodexEntryGenerator.GenerateImageContainers(uispriteFromMultiObjectAnim, list);
				CodexEntryGenerator.GeneratePlantDescriptionContainers(gameObject, list);
				CodexEntry codexEntry = new CodexEntry("PLANTS", list, gameObject.GetProperName());
				codexEntry.parentId = "PLANTS";
				codexEntry.icon = uispriteFromMultiObjectAnim;
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
			List<ContentContainer> list = new List<ContentContainer>();
			CodexEntryGenerator.GenerateTitleContainers(foodInfo.Name, list);
			Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(Assets.GetPrefab(foodInfo.ConsumableId).GetComponent<KBatchedAnimController>().AnimFiles[0], "ui");
			CodexEntryGenerator.GenerateImageContainers(uispriteFromMultiObjectAnim, list);
			CodexEntryGenerator.GenerateFoodDescriptionContainers(foodInfo, list);
			CodexEntryGenerator.GenerateUsedInRecipeContainers(foodInfo.ConsumableId.ToTag(), list);
			CodexEntry codexEntry = new CodexEntry("FOOD", list, foodInfo.Name);
			codexEntry.icon = uispriteFromMultiObjectAnim;
			codexEntry.parentId = "FOOD";
			CodexCache.AddEntry(foodInfo.Id, codexEntry, null);
			dictionary.Add(foodInfo.Id, codexEntry);
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
			codexEntry.icon = tech.unlockedItems[0].getUISprite("ui");
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
				if (!gameObject.HasTag(GameTags.DeprecatedContent))
				{
					GeyserConfigurator.GeyserInstanceConfiguration configuration = gameObject.GetComponent<Geyser>().configuration;
					List<ContentContainer> list = new List<ContentContainer>();
					CodexEntryGenerator.GenerateTitleContainers(gameObject.GetProperName(), list);
					Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui");
					CodexEntryGenerator.GenerateImageContainers(uispriteFromMultiObjectAnim, list);
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
					codexEntry.icon = uispriteFromMultiObjectAnim;
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
		string text7;
		foreach (Element element in ElementLoader.elements)
		{
			List<ContentContainer> list = new List<ContentContainer>();
			string text6 = element.name + " (" + element.GetStateString() + ")";
			CodexEntryGenerator.GenerateTitleContainers(text6, list);
			Sprite sprite = null;
			if (element.IsSolid)
			{
				sprite = Def.GetUISpriteFromMultiObjectAnim(element.substance.anim, "ui");
				CodexEntryGenerator.GenerateImageContainers(sprite, list);
			}
			CodexEntryGenerator.GenerateElementDescriptionContainers(element, list);
			if (element.IsSolid)
			{
				text7 = element.id.ToString();
				CodexEntry codexEntry = new CodexEntry(text2, list, text6);
				codexEntry.icon = sprite;
				codexEntry.parentId = text2;
				CodexCache.AddEntry(text7, codexEntry, null);
				dictionary2.Add(text7, codexEntry);
			}
			else if (element.IsLiquid)
			{
				text7 = element.id.ToString();
				CodexEntry codexEntry2 = new CodexEntry(text3, list, text6);
				codexEntry2.parentId = text3;
				CodexCache.AddEntry(text7, codexEntry2, null);
				dictionary3.Add(text7, codexEntry2);
			}
			else if (element.IsGas)
			{
				text7 = element.id.ToString();
				CodexEntry codexEntry3 = new CodexEntry(text4, list, text6);
				codexEntry3.parentId = text4;
				CodexCache.AddEntry(text7, codexEntry3, null);
				dictionary4.Add(text7, codexEntry3);
			}
			else
			{
				text7 = element.id.ToString();
				CodexEntry codexEntry4 = new CodexEntry(text5, list, text6);
				codexEntry4.parentId = text5;
				CodexCache.AddEntry(text7, codexEntry4, null);
				dictionary5.Add(text7, codexEntry4);
			}
		}
		text7 = text2;
		CodexEntry codexEntry5 = CodexEntryGenerator.GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, dictionary2);
		codexEntry5.parentId = text;
		codexEntry5.category = text;
		dictionary.Add(text7, codexEntry5);
		text7 = text3;
		codexEntry5 = CodexEntryGenerator.GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, dictionary3);
		codexEntry5.parentId = text;
		codexEntry5.category = text;
		dictionary.Add(text7, codexEntry5);
		text7 = text4;
		codexEntry5 = CodexEntryGenerator.GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, dictionary4);
		codexEntry5.parentId = text;
		codexEntry5.category = text;
		dictionary.Add(text7, codexEntry5);
		text7 = text5;
		codexEntry5 = CodexEntryGenerator.GenerateCategoryEntry(text7, UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, dictionary5);
		codexEntry5.parentId = text;
		codexEntry5.category = text;
		dictionary.Add(text7, codexEntry5);
		CodexEntryGenerator.PopulateCategoryEntries(dictionary);
		return dictionary;
	}

	public static Dictionary<string, CodexEntry> GenerateDiseaseEntries()
	{
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		foreach (Disease disease in Db.Get().Diseases)
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

	public static CategoryEntry GenerateCategoryEntry(string id, string name, Dictionary<string, CodexEntry> entries)
	{
		List<ContentContainer> list = new List<ContentContainer>();
		CodexEntryGenerator.GenerateTitleContainers(name, list);
		List<CodexEntry> list2 = new List<CodexEntry>();
		Sprite sprite = null;
		foreach (KeyValuePair<string, CodexEntry> keyValuePair in entries)
		{
			list2.Add(keyValuePair.Value);
			if (sprite == null)
			{
				sprite = keyValuePair.Value.icon;
			}
		}
		CategoryEntry categoryEntry = new CategoryEntry("Root", list, name, list2);
		categoryEntry.icon = sprite;
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
					codexWidget.objectProperties.Add("sprite", codexEntry2.icon);
					contentContainer.content.Add(codexWidget);
				}
				CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						"• " + codexEntry2.name
					},
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
					techItem.getUISprite("ui")
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

	private static void GenerateUsedInRecipeContainers(Tag prefabID, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				CODEX.HEADERS.USED_IN_RECIPES
			},
			{ "style", "subtitle" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.DividerLine, new Dictionary<string, string>());
		list.Add(codexWidget2);
		bool flag = false;
		foreach (Recipe recipe in RecipeManager.Get().recipes)
		{
			foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
			{
				if (ingredient.tag == prefabID)
				{
					if (!flag)
					{
						containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
						flag = true;
					}
					List<CodexWidget> list2 = new List<CodexWidget>();
					CodexWidget codexWidget3 = new CodexWidget(CodexWidget.ContentType.Image, new Dictionary<string, string>
					{
						{ "preferredWidth", "64" },
						{ "preferredHeight", "64" }
					});
					GameObject prefab = Assets.GetPrefab(recipe.Result);
					codexWidget3.objectProperties.Add("sprite", Def.GetUISpriteFromMultiObjectAnim(prefab.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui"));
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
		}
	}

	private static void GeneratePlantDescriptionContainers(GameObject plant, List<ContentContainer> containers)
	{
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
		InfoDescription component = Assets.GetPrefab(plant.PrefabID()).GetComponent<InfoDescription>();
		if (component != null)
		{
			CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
			{
				{ "string", component.description },
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

	private static void GenerateCreatureDescriptionContainers(GameObject creature, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		List<CodexWidget> list2 = new List<CodexWidget>();
		List<Element> list3 = new List<Element>();
		CreatureCalorieMonitor.Def def = creature.GetDef<CreatureCalorieMonitor.Def>();
		if (def != null)
		{
			foreach (Diet.Info info in def.diet.infos)
			{
				List<Tag> tagsVerySlow = info.consumedTagBits.GetTagsVerySlow();
				for (int j = 0; j < tagsVerySlow.Count; j++)
				{
					if (ElementLoader.GetElementID(tagsVerySlow[j]) != SimHashes.Vacuum && ElementLoader.GetElementID(tagsVerySlow[j]) != SimHashes.Void)
					{
						Element element = ElementLoader.FindElementByHash(ElementLoader.GetElementID(tagsVerySlow[j]));
						if (!list3.Contains(element))
						{
							CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
							{
								{
									"string",
									"• " + element.name
								},
								{ "style", "body" }
							});
							list3.Add(element);
							list2.Add(codexWidget);
						}
					}
				}
			}
			if (def.diet != null && def.diet.infos[0].producedElement != SimHashes.Vacuum && def.diet.infos[0].producedElement != SimHashes.Void)
			{
				CodexWidget codexWidget2 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						"• " + ElementLoader.FindElementByHash(def.diet.infos[0].producedElement).name
					},
					{ "style", "body" }
				});
				list.Insert(0, codexWidget2);
				list.Insert(0, new CodexWidget(CodexWidget.ContentType.DividerLine));
				CodexWidget codexWidget3 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						CODEX.HEADERS.PRODUCES
					},
					{ "style", "subtitle" }
				});
				list.Insert(0, codexWidget3);
				list.Insert(0, new CodexWidget(CodexWidget.ContentType.Spacer));
				containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
			}
			if (list3.Count > 0)
			{
				list2.Insert(0, new CodexWidget(CodexWidget.ContentType.DividerLine));
				CodexWidget codexWidget4 = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
				{
					{
						"string",
						CODEX.HEADERS.DIET
					},
					{ "style", "subtitle" }
				});
				list2.Insert(0, codexWidget4);
				containers.Add(new ContentContainer(list2, ContentContainer.ContentLayout.Vertical));
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
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"stringKey",
				"STRINGS.ITEMS.FOOD." + food.ConsumableId.ToUpper() + ".DESC"
			},
			{ "style", "body" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
	}

	private static void GenerateElementDescriptionContainers(Element element, List<ContentContainer> containers)
	{
		List<CodexWidget> list = new List<CodexWidget>();
		CodexWidget codexWidget = new CodexWidget(CodexWidget.ContentType.Text, new Dictionary<string, string>
		{
			{
				"string",
				element.FullDescription(true)
			},
			{ "style", "body" }
		});
		list.Add(codexWidget);
		list.Add(new CodexWidget(CodexWidget.ContentType.Spacer));
		containers.Add(new ContentContainer(list, ContentContainer.ContentLayout.Vertical));
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
			codexWidget3.objectProperties.Add("sprite", Def.GetUISpriteFromMultiObjectAnim(prefab.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui"));
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
				Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(gameObject.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui");
				codexWidget3.objectProperties.Add("sprite", uispriteFromMultiObjectAnim);
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
