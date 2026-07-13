using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class CodexEntryGenerator_Elements
{
	public static Dictionary<string, CodexEntry> GenerateEntries()
	{
		CodexEntryGenerator_Elements.<>c__DisplayClass9_0 CS$<>8__locals1;
		CS$<>8__locals1.entriesElements = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary3 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary4 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary5 = new Dictionary<string, CodexEntry>();
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|9_0(CodexEntryGenerator_Elements.ELEMENTS_SOLIDS_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, Assets.GetSprite("ui_elements-solid"), dictionary, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|9_0(CodexEntryGenerator_Elements.ELEMENTS_LIQUIDS_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, Assets.GetSprite("ui_elements-liquids"), dictionary2, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|9_0(CodexEntryGenerator_Elements.ELEMENTS_GASES_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, Assets.GetSprite("ui_elements-gases"), dictionary3, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|9_0(CodexEntryGenerator_Elements.ELEMENTS_OTHER_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, Assets.GetSprite("ui_elements-other"), dictionary4, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|9_0(CodexEntryGenerator_Elements.ELEMENT_TYPES, UI.CODEX.CATEGORYNAMES.ELEMENTTYPES, Assets.GetSprite("ui_element_poperties"), dictionary5, ref CS$<>8__locals1);
		foreach (Element element in ElementLoader.elements)
		{
			if (!element.disabled)
			{
				bool flag = false;
				Tag[] oreTags = element.oreTags;
				for (int i = 0; i < oreTags.Length; i++)
				{
					if (oreTags[i] == GameTags.HideFromCodex)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					global::Tuple<Sprite, Color> tuple = Def.GetUISprite(element, "ui", false);
					if (tuple.first == null)
					{
						if (element.id == SimHashes.Void)
						{
							tuple = new global::Tuple<Sprite, Color>(Assets.GetSprite("ui_elements-void"), Color.white);
						}
						else if (element.id == SimHashes.Vacuum)
						{
							tuple = new global::Tuple<Sprite, Color>(Assets.GetSprite("ui_elements-vacuum"), Color.white);
						}
					}
					List<ContentContainer> list = new List<ContentContainer>();
					CodexEntryGenerator.GenerateTitleContainers(element.name, list);
					CodexEntryGenerator.GenerateImageContainers(new global::Tuple<Sprite, Color>[] { tuple }, list, ContentContainer.ContentLayout.Horizontal);
					CodexEntryGenerator_Elements.GenerateElementDescriptionContainers(element, list);
					string text;
					Dictionary<string, CodexEntry> dictionary6;
					if (element.IsSolid)
					{
						text = CodexEntryGenerator_Elements.ELEMENTS_SOLIDS_ID;
						dictionary6 = dictionary;
					}
					else if (element.IsLiquid)
					{
						text = CodexEntryGenerator_Elements.ELEMENTS_LIQUIDS_ID;
						dictionary6 = dictionary2;
					}
					else if (element.IsGas)
					{
						text = CodexEntryGenerator_Elements.ELEMENTS_GASES_ID;
						dictionary6 = dictionary3;
					}
					else
					{
						text = CodexEntryGenerator_Elements.ELEMENTS_OTHER_ID;
						dictionary6 = dictionary4;
					}
					string text2 = element.id.ToString();
					CodexEntry codexEntry = new CodexEntry(text, list, element.name);
					codexEntry.parentId = text;
					codexEntry.icon = tuple.first;
					codexEntry.iconColor = tuple.second;
					CodexCache.AddEntry(text2, codexEntry, null);
					dictionary6.Add(text2, codexEntry);
				}
			}
		}
		string text3 = "IceBellyPoop";
		GameObject gameObject = Assets.TryGetPrefab(text3);
		if (gameObject != null)
		{
			string elements_SOLIDS_ID = CodexEntryGenerator_Elements.ELEMENTS_SOLIDS_ID;
			Dictionary<string, CodexEntry> dictionary7 = dictionary;
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			InfoDescription component2 = gameObject.GetComponent<InfoDescription>();
			string properName = gameObject.GetProperName();
			string description = component2.description;
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(gameObject, "ui", false);
			List<ContentContainer> list2 = new List<ContentContainer>();
			CodexEntryGenerator.GenerateTitleContainers(properName, list2);
			CodexEntryGenerator.GenerateImageContainers(new global::Tuple<Sprite, Color>[] { uisprite }, list2, ContentContainer.ContentLayout.Horizontal);
			CodexEntryGenerator_Elements.GenerateMadeAndUsedContainers(component.PrefabTag, list2);
			list2.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(description, CodexTextStyle.Body, null),
				new CodexSpacer()
			}, ContentContainer.ContentLayout.Vertical));
			CodexEntry codexEntry2 = new CodexEntry(elements_SOLIDS_ID, list2, properName);
			codexEntry2.parentId = elements_SOLIDS_ID;
			codexEntry2.icon = uisprite.first;
			codexEntry2.iconColor = uisprite.second;
			CodexCache.AddEntry(text3, codexEntry2, null);
			dictionary7.Add(text3, codexEntry2);
		}
		CodexEntryGenerator.PopulateCategoryEntries(CS$<>8__locals1.entriesElements);
		return CS$<>8__locals1.entriesElements;
	}

	public static void GenerateElementDescriptionContainers(Element element, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		List<ICodexWidget> list2 = new List<ICodexWidget>();
		if (element.sublimateId != (SimHashes)0 || element.HasTag(GameTags.Sublimating))
		{
			list.Add(new CodexTemperatureTransitionPanel(element, (element.offGasPercentage != 0f) ? CodexTemperatureTransitionPanel.TransitionType.OFFGASS : CodexTemperatureTransitionPanel.TransitionType.SUBLIMATE));
		}
		if (element.highTempTransition != null)
		{
			list.Add(new CodexTemperatureTransitionPanel(element, CodexTemperatureTransitionPanel.TransitionType.HEAT));
		}
		if (element.lowTempTransition != null)
		{
			list.Add(new CodexTemperatureTransitionPanel(element, CodexTemperatureTransitionPanel.TransitionType.COOL));
		}
		foreach (Element element2 in ElementLoader.elements)
		{
			if (!element2.disabled)
			{
				if (element2.highTempTransition == element || ElementLoader.FindElementByHash(element2.highTempTransitionOreID) == element)
				{
					list2.Add(new CodexTemperatureTransitionPanel(element2, CodexTemperatureTransitionPanel.TransitionType.HEAT));
				}
				if (element2.lowTempTransition == element || ElementLoader.FindElementByHash(element2.lowTempTransitionOreID) == element)
				{
					list2.Add(new CodexTemperatureTransitionPanel(element2, CodexTemperatureTransitionPanel.TransitionType.COOL));
				}
				if (element2.sublimateId == element.id || element2.HasTag(GameTags.Sublimating))
				{
					bool flag = element2.sublimateId == element.id;
					if (element2.sublimateId != element.id)
					{
						GameObject prefab = Assets.GetPrefab(element2.id.CreateTag());
						if (prefab != null)
						{
							Sublimates component = prefab.GetComponent<Sublimates>();
							flag = component != null && component.info.sublimatedElement == element.id;
						}
					}
					if (flag)
					{
						list2.Add(new CodexTemperatureTransitionPanel(element2, (element2.offGasPercentage != 0f) ? CodexTemperatureTransitionPanel.TransitionType.OFFGASS : CodexTemperatureTransitionPanel.TransitionType.SUBLIMATE));
					}
				}
			}
		}
		if (list.Count > 0)
		{
			ContentContainer contentContainer = new ContentContainer(list, ContentContainer.ContentLayout.Vertical);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTTRANSITIONSTO, contentContainer)
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(contentContainer);
		}
		if (list2.Count > 0)
		{
			ContentContainer contentContainer2 = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTTRANSITIONSFROM, contentContainer2)
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(contentContainer2);
		}
		CodexEntryGenerator_Elements.GenerateMadeAndUsedContainers(element.tag, containers);
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexSpacer(),
			new CodexText(element.FullDescription(true), CodexTextStyle.Body, null),
			new CodexSpacer()
		}, ContentContainer.ContentLayout.Vertical));
	}

	public static void GenerateMadeAndUsedContainers(Tag tag, List<ContentContainer> containers)
	{
		List<ICodexWidget> used = new List<ICodexWidget>();
		List<ICodexWidget> made = new List<ICodexWidget>();
		Func<ComplexRecipe.RecipeElement, bool> <>9__3;
		Func<ComplexRecipe.RecipeElement, bool> <>9__4;
		foreach (ComplexRecipe complexRecipe in ComplexRecipeManager.Get().recipes)
		{
			if (Game.IsCorrectDlcActiveForCurrentSave(complexRecipe) && !complexRecipe.IsAnyProductDeprecated())
			{
				IEnumerable<ComplexRecipe.RecipeElement> ingredients = complexRecipe.ingredients;
				Func<ComplexRecipe.RecipeElement, bool> func;
				if ((func = <>9__3) == null)
				{
					func = (<>9__3 = (ComplexRecipe.RecipeElement i) => i.material == tag);
				}
				if (ingredients.Any<ComplexRecipe.RecipeElement>(func))
				{
					used.Add(new CodexRecipePanel(complexRecipe, false));
				}
				IEnumerable<ComplexRecipe.RecipeElement> results = complexRecipe.results;
				Func<ComplexRecipe.RecipeElement, bool> func2;
				if ((func2 = <>9__4) == null)
				{
					func2 = (<>9__4 = (ComplexRecipe.RecipeElement i) => i.material == tag);
				}
				if (results.Any<ComplexRecipe.RecipeElement>(func2))
				{
					made.Add(new CodexRecipePanel(complexRecipe, true));
				}
			}
		}
		List<CodexEntryGenerator_Elements.ConversionEntry> list;
		if (CodexEntryGenerator_Elements.GetElementEntryContext().usedMap.map.TryGetValue(tag, out list))
		{
			foreach (CodexEntryGenerator_Elements.ConversionEntry conversionEntry in list)
			{
				used.Add(new CodexConversionPanel(conversionEntry.title, conversionEntry.inSet.ToArray<ElementUsage>(), conversionEntry.outSet.ToArray<ElementUsage>(), conversionEntry.prefab, conversionEntry.aidIcon1));
			}
		}
		List<CodexEntryGenerator_Elements.ConversionEntry> list2;
		if (CodexEntryGenerator_Elements.GetElementEntryContext().madeMap.map.TryGetValue(tag, out list2))
		{
			foreach (CodexEntryGenerator_Elements.ConversionEntry conversionEntry2 in list2)
			{
				made.Add(new CodexConversionPanel(conversionEntry2.title, conversionEntry2.inSet.ToArray<ElementUsage>(), conversionEntry2.outSet.ToArray<ElementUsage>(), conversionEntry2.prefab, conversionEntry2.aidIcon1));
			}
		}
		List<ManualCodexConversionRegistry.ManualConversionEntry> conversionsForGivenConverter = ManualCodexConversionRegistry.GetConversionsForGivenConverter(tag);
		if (conversionsForGivenConverter != null)
		{
			conversionsForGivenConverter.ForEach(delegate(ManualCodexConversionRegistry.ManualConversionEntry ce)
			{
				List<ICodexWidget> used3 = used;
				string headerDescription = ce.headerDescription;
				object obj;
				if (ce.input == null)
				{
					obj = null;
				}
				else
				{
					(obj = new ElementUsage[1])[0] = new ElementUsage(ce.input.first, ce.input.second, false, ce.inputCustomFormating);
				}
				object obj2;
				if (ce.output == null)
				{
					obj2 = null;
				}
				else
				{
					(obj2 = new ElementUsage[1])[0] = new ElementUsage(ce.output.first, ce.output.second, false, ce.outputCustomFormating);
				}
				used3.Add(new CodexConversionPanel(headerDescription, obj, obj2, Assets.GetPrefab(tag)));
			});
		}
		List<ManualCodexConversionRegistry.ManualConversionEntry> producersForGivenOutput = ManualCodexConversionRegistry.GetProducersForGivenOutput(tag);
		if (producersForGivenOutput != null)
		{
			producersForGivenOutput.ForEach(delegate(ManualCodexConversionRegistry.ManualConversionEntry ce)
			{
				List<ICodexWidget> made2 = made;
				string headerDescription2 = ce.headerDescription;
				object obj3;
				if (ce.input == null)
				{
					obj3 = null;
				}
				else
				{
					(obj3 = new ElementUsage[1])[0] = new ElementUsage(ce.input.first, ce.input.second, false, ce.inputCustomFormating);
				}
				object obj4;
				if (ce.output == null)
				{
					obj4 = null;
				}
				else
				{
					(obj4 = new ElementUsage[1])[0] = new ElementUsage(ce.output.first, ce.output.second, false, ce.outputCustomFormating);
				}
				made2.Add(new CodexConversionPanel(headerDescription2, obj3, obj4, Assets.GetPrefab(ce.converter.first)));
			});
		}
		List<ManualCodexConversionRegistry.ManualConversionEntry> consumersForGivenInput = ManualCodexConversionRegistry.GetConsumersForGivenInput(tag);
		if (consumersForGivenInput != null)
		{
			consumersForGivenInput.ForEach(delegate(ManualCodexConversionRegistry.ManualConversionEntry ce)
			{
				List<ICodexWidget> used2 = used;
				string headerDescription3 = ce.headerDescription;
				object obj5;
				if (ce.input == null)
				{
					obj5 = null;
				}
				else
				{
					(obj5 = new ElementUsage[1])[0] = new ElementUsage(ce.input.first, ce.input.second, false, ce.inputCustomFormating);
				}
				object obj6;
				if (ce.output == null)
				{
					obj6 = null;
				}
				else
				{
					(obj6 = new ElementUsage[1])[0] = new ElementUsage(ce.output.first, ce.output.second, false, ce.outputCustomFormating);
				}
				used2.Add(new CodexConversionPanel(headerDescription3, obj5, obj6, Assets.GetPrefab(ce.converter.first)));
			});
		}
		ContentContainer contentContainer = new ContentContainer(used, ContentContainer.ContentLayout.Vertical);
		ContentContainer contentContainer2 = new ContentContainer(made, ContentContainer.ContentLayout.Vertical);
		if (used.Count > 0)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTCONSUMEDBY, contentContainer)
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(contentContainer);
		}
		if (made.Count > 0)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTPRODUCEDBY, contentContainer2)
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(contentContainer2);
		}
	}

	public static CodexEntryGenerator_Elements.ElementEntryContext GetElementEntryContext()
	{
		if (CodexEntryGenerator_Elements.contextInstance == null)
		{
			CodexEntryGenerator_Elements.CodexElementMap codexElementMap = new CodexEntryGenerator_Elements.CodexElementMap();
			CodexEntryGenerator_Elements.<>c__DisplayClass13_0 CS$<>8__locals1;
			CS$<>8__locals1.madeMap = new CodexEntryGenerator_Elements.CodexElementMap();
			CodexEntryGenerator_Elements.<>c__DisplayClass13_1 CS$<>8__locals2;
			CS$<>8__locals2.waterTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
			CS$<>8__locals2.dirtyWaterTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
			foreach (PlanScreen.PlanInfo planInfo in global::TUNING.BUILDINGS.PLANORDER)
			{
				foreach (KeyValuePair<string, string> keyValuePair in planInfo.buildingAndSubcategoryData)
				{
					BuildingDef buildingDef = Assets.GetBuildingDef(keyValuePair.Key);
					if (buildingDef == null)
					{
						global::Debug.LogError("Building def for id " + keyValuePair.Key + " is null");
					}
					if (!buildingDef.Deprecated && !buildingDef.BuildingComplete.HasTag(GameTags.DevBuilding))
					{
						CodexEntryGenerator_Elements.<GetElementEntryContext>g__CheckPrefab|13_1(buildingDef.BuildingComplete, codexElementMap, CS$<>8__locals1.madeMap, ref CS$<>8__locals1, ref CS$<>8__locals2);
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
				if (!gameObject2.HasTag(GameTags.HideFromCodex))
				{
					CodexEntryGenerator_Elements.<GetElementEntryContext>g__CheckPrefab|13_1(gameObject2, codexElementMap, CS$<>8__locals1.madeMap, ref CS$<>8__locals1, ref CS$<>8__locals2);
				}
			}
			foreach (GameObject gameObject3 in Assets.GetPrefabsWithComponent<CreatureBrain>())
			{
				if (gameObject3.GetDef<BabyMonitor.Def>() == null)
				{
					CodexEntryGenerator_Elements.<GetElementEntryContext>g__CheckPrefab|13_1(gameObject3, codexElementMap, CS$<>8__locals1.madeMap, ref CS$<>8__locals1, ref CS$<>8__locals2);
				}
			}
			foreach (KeyValuePair<Tag, Diet> keyValuePair2 in DietManager.CollectSaveDiets(null))
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
					Diet value = keyValuePair2.Value;
					foreach (Diet.Info info in value.infos)
					{
						foreach (Tag tag in info.consumedTags)
						{
							float num2 = -num / info.caloriesPerKg;
							float num3 = num2 * info.producedConversionRate;
							bool flag = value.IsConsumedTagAbleToBeEatenDirectly(tag);
							ElementUsage elementUsage = null;
							if (flag)
							{
								if (info.foodType == Diet.Info.FoodType.EatPlantDirectly)
								{
									elementUsage = new ElementUsage(tag, num2, true, new Func<Tag, float, bool, string>(GameUtil.GetFormattedDirectPlantConsumptionValuePerCycle));
								}
								else if (info.foodType == Diet.Info.FoodType.EatPlantStorage)
								{
									elementUsage = new ElementUsage(tag, num2, true, new Func<Tag, float, bool, string>(GameUtil.GetFormattedPlantStorageConsumptionValuePerCycle));
								}
								else if (info.foodType == Diet.Info.FoodType.EatPrey || info.foodType == Diet.Info.FoodType.EatButcheredPrey)
								{
									float num4 = value.AvailableCaloriesInPrey(tag);
									num2 = -num / num4;
									num3 = num2 * info.producedConversionRate * num4 / info.caloriesPerKg;
									elementUsage = new ElementUsage(tag, num2, true, new Func<Tag, float, bool, string>(GameUtil.GetFormattedPreyConsumptionValuePerCycle));
								}
							}
							else
							{
								elementUsage = new ElementUsage(tag, num2, true);
							}
							CodexEntryGenerator_Elements.ConversionEntry conversionEntry = new CodexEntryGenerator_Elements.ConversionEntry();
							conversionEntry.title = gameObject4.GetProperName();
							conversionEntry.prefab = gameObject4;
							conversionEntry.inSet = new HashSet<ElementUsage>();
							conversionEntry.inSet.Add(elementUsage);
							conversionEntry.outSet = new HashSet<ElementUsage>();
							conversionEntry.outSet.Add(new ElementUsage(info.producedElement, num3, true));
							codexElementMap.Add(tag, conversionEntry);
							CS$<>8__locals1.madeMap.Add(info.producedElement, conversionEntry);
						}
					}
				}
			}
			CodexEntryGenerator_Elements.contextInstance = new CodexEntryGenerator_Elements.ElementEntryContext
			{
				usedMap = codexElementMap,
				madeMap = CS$<>8__locals1.madeMap
			};
		}
		return CodexEntryGenerator_Elements.contextInstance;
	}

	[CompilerGenerated]
	internal static void <GenerateEntries>g__AddCategoryEntry|9_0(string categoryId, string name, Sprite icon, Dictionary<string, CodexEntry> entries, ref CodexEntryGenerator_Elements.<>c__DisplayClass9_0 A_4)
	{
		CodexEntry codexEntry = CodexEntryGenerator.GenerateCategoryEntry(categoryId, name, entries, icon, true, true, null);
		codexEntry.parentId = CodexEntryGenerator_Elements.ELEMENTS_ID;
		codexEntry.category = CodexEntryGenerator_Elements.ELEMENTS_ID;
		A_4.entriesElements.Add(categoryId, codexEntry);
	}

	[CompilerGenerated]
	internal static void <GetElementEntryContext>g__AddPlantFiberInfo|13_0(ref HashSet<ElementUsage> inSet, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap madeMap, GameObject prefabOfProducer, GameObject prefabForPlayerFacing, Crop crop, Func<Tag, float, bool, string> customFormating = null)
	{
		PlantFiberProducer component = prefabOfProducer.GetComponent<PlantFiberProducer>();
		if (component != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry = new CodexEntryGenerator_Elements.ConversionEntry();
			conversionEntry.title = prefabForPlayerFacing.GetProperName();
			conversionEntry.prefab = prefabForPlayerFacing;
			conversionEntry.inSet = inSet;
			conversionEntry.outSet.Add(new ElementUsage("PlantFiber", component.amount / crop.cropVal.cropDuration, true, customFormating));
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry2 = conversionEntry;
			CodexConversionPanel.IconSettings iconSettings = new CodexConversionPanel.IconSettings();
			iconSettings.spriteName = "skillbadge_role_farming3";
			iconSettings.tooltip = CODEX.MISC.TIP_ICON.FARMING3_SKILL.TOOLTIP;
			iconSettings.onClickActions = delegate
			{
				ManagementMenu.Instance.OpenSkills(null);
			};
			conversionEntry2.aidIcon1 = iconSettings;
			usedMap.Add(prefabForPlayerFacing.PrefabID(), conversionEntry);
			madeMap.Add("PlantFiber", conversionEntry);
		}
	}

	[CompilerGenerated]
	internal static void <GetElementEntryContext>g__CheckPrefab|13_1(GameObject prefab, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap made, ref CodexEntryGenerator_Elements.<>c__DisplayClass13_0 A_3, ref CodexEntryGenerator_Elements.<>c__DisplayClass13_1 A_4)
	{
		HashSet<ElementUsage> hashSet = new HashSet<ElementUsage>();
		HashSet<ElementUsage> hashSet2 = new HashSet<ElementUsage>();
		EnergyGenerator component = prefab.GetComponent<EnergyGenerator>();
		if (component)
		{
			IEnumerable<EnergyGenerator.InputItem> inputs = component.formula.inputs;
			foreach (EnergyGenerator.InputItem inputItem in (inputs ?? Enumerable.Empty<EnergyGenerator.InputItem>()))
			{
				hashSet.Add(new ElementUsage(inputItem.tag, inputItem.consumptionRate, true));
			}
			IEnumerable<EnergyGenerator.OutputItem> outputs = component.formula.outputs;
			foreach (EnergyGenerator.OutputItem outputItem in (outputs ?? Enumerable.Empty<EnergyGenerator.OutputItem>()))
			{
				Tag tag = ElementLoader.FindElementByHash(outputItem.element).tag;
				hashSet2.Add(new ElementUsage(tag, outputItem.creationRate, true));
			}
		}
		IEnumerable<ElementConverter> components = prefab.GetComponents<ElementConverter>();
		foreach (ElementConverter elementConverter in (components ?? Enumerable.Empty<ElementConverter>()))
		{
			List<CodexEntryGenerator_Elements.ConversionEntry> list = new List<CodexEntryGenerator_Elements.ConversionEntry>();
			IEnumerable<ElementConverter.ConsumedElement> consumedElements = elementConverter.consumedElements;
			using (IEnumerator<ElementConverter.ConsumedElement> enumerator4 = (consumedElements ?? Enumerable.Empty<ElementConverter.ConsumedElement>()).GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					ElementConverter.ConsumedElement c2 = enumerator4.Current;
					if (elementConverter.inputIsCategory)
					{
						using (List<Element>.Enumerator enumerator5 = ElementLoader.FindElements((Element e) => e.HasTag(c2.Tag)).GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								Element element = enumerator5.Current;
								list.Add(new CodexEntryGenerator_Elements.ConversionEntry
								{
									title = prefab.GetProperName(),
									prefab = prefab,
									inSet = 
									{
										new ElementUsage(element.tag, c2.MassConsumptionRate, true)
									}
								});
							}
							continue;
						}
					}
					hashSet.Add(new ElementUsage(c2.Tag, c2.MassConsumptionRate, true));
				}
			}
			IEnumerable<ElementConverter.OutputElement> outputElements = elementConverter.outputElements;
			foreach (ElementConverter.OutputElement outputElement in (outputElements ?? Enumerable.Empty<ElementConverter.OutputElement>()))
			{
				ElementUsage elementUsage = new ElementUsage(ElementLoader.FindElementByHash(outputElement.elementHash).tag, outputElement.massGenerationRate, true);
				if (elementConverter.inputIsCategory)
				{
					using (List<CodexEntryGenerator_Elements.ConversionEntry>.Enumerator enumerator7 = list.GetEnumerator())
					{
						while (enumerator7.MoveNext())
						{
							CodexEntryGenerator_Elements.ConversionEntry conversionEntry = enumerator7.Current;
							conversionEntry.outSet.Add(elementUsage);
						}
						continue;
					}
				}
				hashSet2.Add(elementUsage);
			}
			foreach (CodexEntryGenerator_Elements.ConversionEntry conversionEntry2 in list)
			{
				if (conversionEntry2.inSet.Count > 0 && conversionEntry2.outSet.Count > 0)
				{
					usedMap.Add(prefab.PrefabID(), conversionEntry2);
				}
				foreach (ElementUsage elementUsage2 in conversionEntry2.inSet)
				{
					usedMap.Add(elementUsage2.tag, conversionEntry2);
				}
				foreach (ElementUsage elementUsage3 in conversionEntry2.outSet)
				{
					A_3.madeMap.Add(elementUsage3.tag, conversionEntry2);
				}
			}
		}
		IEnumerable<ElementConsumer> components2 = prefab.GetComponents<ElementConsumer>();
		foreach (ElementConsumer elementConsumer in (components2 ?? Enumerable.Empty<ElementConsumer>()))
		{
			if (!elementConsumer.storeOnConsume)
			{
				Tag tag2 = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag;
				hashSet.Add(new ElementUsage(tag2, elementConsumer.consumptionRate, true));
			}
		}
		IrrigationMonitor.Def def = prefab.GetDef<IrrigationMonitor.Def>();
		if (def != null)
		{
			foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in def.consumedElements)
			{
				hashSet.Add(new ElementUsage(consumeInfo.tag, consumeInfo.massConsumptionRate, true));
			}
		}
		FertilizationMonitor.Def def2 = prefab.GetDef<FertilizationMonitor.Def>();
		if (def2 != null)
		{
			foreach (PlantElementAbsorber.ConsumeInfo consumeInfo2 in def2.consumedElements)
			{
				hashSet.Add(new ElementUsage(consumeInfo2.tag, consumeInfo2.massConsumptionRate, true));
			}
		}
		IPlantConsumeEntities component2 = prefab.GetComponent<IPlantConsumeEntities>();
		Crop component3 = prefab.GetComponent<Crop>();
		if (component3 != null && component2 == null)
		{
			hashSet2.Add(new ElementUsage(component3.cropId, (float)component3.cropVal.numProduced / component3.cropVal.cropDuration, true));
		}
		FlushToilet component4 = prefab.GetComponent<FlushToilet>();
		if (component4)
		{
			hashSet.Add(new ElementUsage(A_4.waterTag, component4.massConsumedPerUse, false));
			hashSet2.Add(new ElementUsage(A_4.dirtyWaterTag, component4.massEmittedPerUse, false));
		}
		HandSanitizer component5 = prefab.GetComponent<HandSanitizer>();
		if (component5)
		{
			Tag tag3 = ElementLoader.FindElementByHash(component5.consumedElement).tag;
			hashSet.Add(new ElementUsage(tag3, component5.massConsumedPerUse, false));
			if (component5.outputElement != SimHashes.Vacuum)
			{
				Tag tag4 = ElementLoader.FindElementByHash(component5.outputElement).tag;
				hashSet2.Add(new ElementUsage(tag4, component5.massConsumedPerUse, false));
			}
		}
		CodexEntryGenerator_Elements.ConversionEntry conversionEntry3 = new CodexEntryGenerator_Elements.ConversionEntry();
		conversionEntry3.title = prefab.GetProperName();
		conversionEntry3.prefab = prefab;
		conversionEntry3.inSet = hashSet;
		conversionEntry3.outSet = hashSet2;
		if (hashSet.Count > 0 && hashSet2.Count > 0)
		{
			usedMap.Add(prefab.PrefabID(), conversionEntry3);
		}
		foreach (ElementUsage elementUsage4 in hashSet)
		{
			usedMap.Add(elementUsage4.tag, conversionEntry3);
		}
		foreach (ElementUsage elementUsage5 in hashSet2)
		{
			A_3.madeMap.Add(elementUsage5.tag, conversionEntry3);
		}
		if (component3 != null && component2 == null)
		{
			CodexEntryGenerator_Elements.<GetElementEntryContext>g__AddPlantFiberInfo|13_0(ref hashSet, usedMap, A_3.madeMap, prefab, prefab, component3, null);
		}
		IPlantBranchGrower defImplementingInterface = prefab.GetDefImplementingInterface<IPlantBranchGrower>();
		if (defImplementingInterface != null)
		{
			GameObject prefab2 = Assets.GetPrefab(defImplementingInterface.GetPlantBranchPrefabName());
			if (prefab2 != null)
			{
				Crop component6 = prefab2.GetComponent<Crop>();
				if (component6 != null && (component3 == null || component6.cropId != component3.cropId || component6.cropVal.numProduced != component3.cropVal.numProduced))
				{
					CodexEntryGenerator_Elements.ConversionEntry conversionEntry4 = new CodexEntryGenerator_Elements.ConversionEntry();
					conversionEntry4.title = prefab2.GetProperName();
					conversionEntry4.prefab = prefab;
					usedMap.Add(prefab.PrefabID(), conversionEntry4);
					conversionEntry4.inSet = new HashSet<ElementUsage>();
					IrrigationMonitor.Def def3 = prefab.GetDef<IrrigationMonitor.Def>();
					if (def3 != null)
					{
						foreach (PlantElementAbsorber.ConsumeInfo consumeInfo3 in def3.consumedElements)
						{
							conversionEntry4.inSet.Add(new ElementUsage(consumeInfo3.tag, consumeInfo3.massConsumptionRate, true));
						}
					}
					FertilizationMonitor.Def def4 = prefab.GetDef<FertilizationMonitor.Def>();
					if (def4 != null)
					{
						foreach (PlantElementAbsorber.ConsumeInfo consumeInfo4 in def4.consumedElements)
						{
							conversionEntry4.inSet.Add(new ElementUsage(consumeInfo4.tag, consumeInfo4.massConsumptionRate, true));
						}
					}
					conversionEntry4.outSet = new HashSet<ElementUsage>();
					int branchCount = defImplementingInterface.GetMaxBranchCount();
					conversionEntry4.outSet.Add(new ElementUsage(component6.cropId, (float)component6.cropVal.numProduced / component6.cropVal.cropDuration, true, (Tag t, float a, bool b) => GameUtil.GetFormattedBranchGrowerPlantProductionValuePerCycle(t, a, branchCount, true)));
					A_3.madeMap.Add(component6.cropId, conversionEntry4);
					CodexEntryGenerator_Elements.<GetElementEntryContext>g__AddPlantFiberInfo|13_0(ref hashSet, usedMap, A_3.madeMap, prefab2, prefab, component6, (Tag t, float a, bool b) => GameUtil.GetFormattedBranchGrowerPlantPlantFiberProductionValuePerCycle(t, a, branchCount, true));
				}
			}
		}
		if (component2 != null)
		{
			List<KPrefabID> prefabsOfPossiblePrey = component2.GetPrefabsOfPossiblePrey();
			List<string> list2 = new List<string>();
			foreach (KPrefabID kprefabID in prefabsOfPossiblePrey)
			{
				CreatureBrain component7 = kprefabID.GetComponent<CreatureBrain>();
				Tag tag5 = ((component7 == null) ? kprefabID.PrefabID() : component7.species);
				string text = tag5.ProperName();
				if (!list2.Contains(text))
				{
					CodexEntryGenerator_Elements.ConversionEntry conversionEntry5 = new CodexEntryGenerator_Elements.ConversionEntry();
					conversionEntry5.title = component2.GetConsumableEntitiesCategoryName() + ": " + text;
					conversionEntry5.prefab = prefab;
					conversionEntry5.inSet.Add(new ElementUsage(tag5, (component3 == null) ? 1f : (1f / component3.cropVal.cropDuration), component3 != null, (Tag t, float amount, bool c) => GameUtil.GetFormattedUnits(amount, c ? GameUtil.TimeSlice.PerCycle : GameUtil.TimeSlice.None, true, "")));
					if (component3 != null)
					{
						conversionEntry5.outSet.Add(new ElementUsage(component3.cropId, (float)component3.cropVal.numProduced / component3.cropVal.cropDuration, true));
						A_3.madeMap.Add(component3.cropId, conversionEntry5);
					}
					usedMap.Add(prefab.PrefabID(), conversionEntry5);
					list2.Add(text);
				}
			}
		}
		ScaleGrowthMonitor.Def def5 = prefab.GetDef<ScaleGrowthMonitor.Def>();
		if (def5 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry6 = new CodexEntryGenerator_Elements.ConversionEntry();
			conversionEntry6.title = Assets.GetPrefab("ShearingStation").GetProperName();
			conversionEntry6.prefab = Assets.GetPrefab("ShearingStation");
			conversionEntry6.inSet = new HashSet<ElementUsage>();
			conversionEntry6.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
			usedMap.Add(prefab.PrefabID(), conversionEntry6);
			conversionEntry6.outSet = new HashSet<ElementUsage>();
			conversionEntry6.outSet.Add(new ElementUsage(def5.itemDroppedOnShear, def5.dropMass, false));
			A_3.madeMap.Add(def5.itemDroppedOnShear, conversionEntry6);
		}
		WellFedShearable.Def def6 = prefab.GetDef<WellFedShearable.Def>();
		if (def6 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry7 = new CodexEntryGenerator_Elements.ConversionEntry();
			conversionEntry7.title = Assets.GetPrefab("ShearingStation").GetProperName();
			conversionEntry7.prefab = Assets.GetPrefab("ShearingStation");
			conversionEntry7.inSet = new HashSet<ElementUsage>();
			conversionEntry7.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
			usedMap.Add(prefab.PrefabID(), conversionEntry7);
			conversionEntry7.outSet = new HashSet<ElementUsage>();
			conversionEntry7.outSet.Add(new ElementUsage(def6.itemDroppedOnShear, def6.dropMass, false));
			A_3.madeMap.Add(def6.itemDroppedOnShear, conversionEntry7);
		}
		MilkProductionMonitor.Def def7 = prefab.GetDef<MilkProductionMonitor.Def>();
		if (def7 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry8 = new CodexEntryGenerator_Elements.ConversionEntry();
			GameObject prefab3 = Assets.GetPrefab("MilkingStation");
			conversionEntry8.title = prefab3.GetProperName();
			conversionEntry8.prefab = prefab3;
			conversionEntry8.inSet = new HashSet<ElementUsage>();
			conversionEntry8.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
			usedMap.Add(prefab.PrefabID(), conversionEntry8);
			conversionEntry8.outSet = new HashSet<ElementUsage>();
			conversionEntry8.outSet.Add(new ElementUsage(def7.element.CreateTag(), def7.Capacity, false));
			A_3.madeMap.Add(def7.element.CreateTag(), conversionEntry8);
		}
		Butcherable component8 = prefab.GetComponent<Butcherable>();
		if (component8 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry9 = new CodexEntryGenerator_Elements.ConversionEntry();
			conversionEntry9.title = prefab.GetProperName();
			conversionEntry9.prefab = prefab;
			usedMap.Add(prefab.PrefabID(), conversionEntry9);
			conversionEntry9.outSet = new HashSet<ElementUsage>();
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			foreach (KeyValuePair<string, float> keyValuePair in component8.drops)
			{
				float num;
				dictionary.TryGetValue(keyValuePair.Key, out num);
				dictionary[keyValuePair.Key] = num + Assets.GetPrefab(keyValuePair.Key).GetComponent<PrimaryElement>().Mass * keyValuePair.Value;
			}
			foreach (KeyValuePair<string, float> keyValuePair2 in dictionary)
			{
				string text2;
				float num2;
				keyValuePair2.Deconstruct(out text2, out num2);
				string text3 = text2;
				float num3 = num2;
				conversionEntry9.outSet.Add(new ElementUsage(text3, num3, false));
				A_3.madeMap.Add(text3, conversionEntry9);
			}
		}
	}

	public static string ELEMENTS_ID = CodexCache.FormatLinkID("ELEMENTS");

	public static string ELEMENTS_SOLIDS_ID = CodexCache.FormatLinkID("ELEMENTS_SOLID");

	public static string ELEMENTS_LIQUIDS_ID = CodexCache.FormatLinkID("ELEMENTS_LIQUID");

	public static string ELEMENTS_GASES_ID = CodexCache.FormatLinkID("ELEMENTS_GAS");

	public static string ELEMENTS_OTHER_ID = CodexCache.FormatLinkID("ELEMENTS_OTHER");

	public static string ELEMENT_TYPES = CodexCache.FormatLinkID("ELEMENTTYPES");

	private static CodexEntryGenerator_Elements.ElementEntryContext contextInstance;

	public class ConversionEntry
	{
		public string title;

		public GameObject prefab;

		public HashSet<ElementUsage> inSet = new HashSet<ElementUsage>();

		public HashSet<ElementUsage> outSet = new HashSet<ElementUsage>();

		public CodexConversionPanel.IconSettings aidIcon1;
	}

	public class CodexElementMap
	{
		public void Add(Tag t, CodexEntryGenerator_Elements.ConversionEntry ce)
		{
			List<CodexEntryGenerator_Elements.ConversionEntry> list;
			if (this.map.TryGetValue(t, out list))
			{
				list.Add(ce);
				return;
			}
			this.map[t] = new List<CodexEntryGenerator_Elements.ConversionEntry> { ce };
		}

		public Dictionary<Tag, List<CodexEntryGenerator_Elements.ConversionEntry>> map = new Dictionary<Tag, List<CodexEntryGenerator_Elements.ConversionEntry>>();
	}

	public class ElementEntryContext
	{
		public CodexEntryGenerator_Elements.CodexElementMap madeMap = new CodexEntryGenerator_Elements.CodexElementMap();

		public CodexEntryGenerator_Elements.CodexElementMap usedMap = new CodexEntryGenerator_Elements.CodexElementMap();
	}
}
