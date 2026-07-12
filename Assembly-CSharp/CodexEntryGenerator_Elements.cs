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
		CodexEntryGenerator_Elements.<>c__DisplayClass8_0 CS$<>8__locals1;
		CS$<>8__locals1.entriesElements = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary3 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary4 = new Dictionary<string, CodexEntry>();
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|8_0(CodexEntryGenerator_Elements.ELEMENTS_SOLIDS_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, Assets.GetSprite("ui_elements-solid"), dictionary, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|8_0(CodexEntryGenerator_Elements.ELEMENTS_LIQUIDS_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, Assets.GetSprite("ui_elements-liquids"), dictionary2, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|8_0(CodexEntryGenerator_Elements.ELEMENTS_GASES_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, Assets.GetSprite("ui_elements-gases"), dictionary3, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|8_0(CodexEntryGenerator_Elements.ELEMENTS_OTHER_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, Assets.GetSprite("ui_elements-other"), dictionary4, ref CS$<>8__locals1);
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
					Dictionary<string, CodexEntry> dictionary5;
					if (element.IsSolid)
					{
						text = CodexEntryGenerator_Elements.ELEMENTS_SOLIDS_ID;
						dictionary5 = dictionary;
					}
					else if (element.IsLiquid)
					{
						text = CodexEntryGenerator_Elements.ELEMENTS_LIQUIDS_ID;
						dictionary5 = dictionary2;
					}
					else if (element.IsGas)
					{
						text = CodexEntryGenerator_Elements.ELEMENTS_GASES_ID;
						dictionary5 = dictionary3;
					}
					else
					{
						text = CodexEntryGenerator_Elements.ELEMENTS_OTHER_ID;
						dictionary5 = dictionary4;
					}
					string text2 = element.id.ToString();
					CodexEntry codexEntry = new CodexEntry(text, list, element.name);
					codexEntry.parentId = text;
					codexEntry.icon = tuple.first;
					codexEntry.iconColor = tuple.second;
					CodexCache.AddEntry(text2, codexEntry, null);
					dictionary5.Add(text2, codexEntry);
				}
			}
		}
		string text3 = "IceBellyPoop";
		GameObject gameObject = Assets.TryGetPrefab(text3);
		if (gameObject != null)
		{
			string elements_SOLIDS_ID = CodexEntryGenerator_Elements.ELEMENTS_SOLIDS_ID;
			Dictionary<string, CodexEntry> dictionary6 = dictionary;
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
			dictionary6.Add(text3, codexEntry2);
		}
		CodexEntryGenerator.PopulateCategoryEntries(CS$<>8__locals1.entriesElements);
		return CS$<>8__locals1.entriesElements;
	}

	public static void GenerateElementDescriptionContainers(Element element, List<ContentContainer> containers)
	{
		List<ICodexWidget> list = new List<ICodexWidget>();
		List<ICodexWidget> list2 = new List<ICodexWidget>();
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
		List<ICodexWidget> list = new List<ICodexWidget>();
		List<ICodexWidget> list2 = new List<ICodexWidget>();
		Func<ComplexRecipe.RecipeElement, bool> <>9__0;
		Func<ComplexRecipe.RecipeElement, bool> <>9__1;
		foreach (ComplexRecipe complexRecipe in ComplexRecipeManager.Get().recipes)
		{
			IEnumerable<ComplexRecipe.RecipeElement> ingredients = complexRecipe.ingredients;
			Func<ComplexRecipe.RecipeElement, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (ComplexRecipe.RecipeElement i) => i.material == tag);
			}
			if (ingredients.Any<ComplexRecipe.RecipeElement>(func))
			{
				list.Add(new CodexRecipePanel(complexRecipe, false));
			}
			IEnumerable<ComplexRecipe.RecipeElement> results = complexRecipe.results;
			Func<ComplexRecipe.RecipeElement, bool> func2;
			if ((func2 = <>9__1) == null)
			{
				func2 = (<>9__1 = (ComplexRecipe.RecipeElement i) => i.material == tag);
			}
			if (results.Any<ComplexRecipe.RecipeElement>(func2))
			{
				list2.Add(new CodexRecipePanel(complexRecipe, true));
			}
		}
		List<CodexEntryGenerator_Elements.ConversionEntry> list3;
		if (CodexEntryGenerator_Elements.GetElementEntryContext().usedMap.map.TryGetValue(tag, out list3))
		{
			foreach (CodexEntryGenerator_Elements.ConversionEntry conversionEntry in list3)
			{
				list.Add(new CodexConversionPanel(conversionEntry.title, conversionEntry.inSet.ToArray<ElementUsage>(), conversionEntry.outSet.ToArray<ElementUsage>(), conversionEntry.prefab));
			}
		}
		List<CodexEntryGenerator_Elements.ConversionEntry> list4;
		if (CodexEntryGenerator_Elements.GetElementEntryContext().madeMap.map.TryGetValue(tag, out list4))
		{
			foreach (CodexEntryGenerator_Elements.ConversionEntry conversionEntry2 in list4)
			{
				list2.Add(new CodexConversionPanel(conversionEntry2.title, conversionEntry2.inSet.ToArray<ElementUsage>(), conversionEntry2.outSet.ToArray<ElementUsage>(), conversionEntry2.prefab));
			}
		}
		ContentContainer contentContainer = new ContentContainer(list, ContentContainer.ContentLayout.Vertical);
		ContentContainer contentContainer2 = new ContentContainer(list2, ContentContainer.ContentLayout.Vertical);
		if (list.Count > 0)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexCollapsibleHeader(CODEX.HEADERS.ELEMENTCONSUMEDBY, contentContainer)
			}, ContentContainer.ContentLayout.Vertical));
			containers.Add(contentContainer);
		}
		if (list2.Count > 0)
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
			CodexEntryGenerator_Elements.<>c__DisplayClass12_0 CS$<>8__locals1;
			CS$<>8__locals1.madeMap = new CodexEntryGenerator_Elements.CodexElementMap();
			CodexEntryGenerator_Elements.<>c__DisplayClass12_1 CS$<>8__locals2;
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
						CodexEntryGenerator_Elements.<GetElementEntryContext>g__CheckPrefab|12_0(buildingDef.BuildingComplete, codexElementMap, CS$<>8__locals1.madeMap, ref CS$<>8__locals1, ref CS$<>8__locals2);
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
					CodexEntryGenerator_Elements.<GetElementEntryContext>g__CheckPrefab|12_0(gameObject2, codexElementMap, CS$<>8__locals1.madeMap, ref CS$<>8__locals1, ref CS$<>8__locals2);
				}
			}
			foreach (GameObject gameObject3 in Assets.GetPrefabsWithComponent<CreatureBrain>())
			{
				if (gameObject3.GetDef<BabyMonitor.Def>() == null)
				{
					CodexEntryGenerator_Elements.<GetElementEntryContext>g__CheckPrefab|12_0(gameObject3, codexElementMap, CS$<>8__locals1.madeMap, ref CS$<>8__locals1, ref CS$<>8__locals2);
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
						float num2 = -num / info.caloriesPerKg;
						float num3 = num2 * info.producedConversionRate;
						foreach (Tag tag in info.consumedTags)
						{
							ElementUsage elementUsage = (value.IsConsumedTagAbleToBeEatenDirectly(tag) ? new ElementUsage(tag, num2, true, new Func<Tag, float, bool, string>(GameUtil.GetFormattedPlantConsumptionValuePerCycle)) : new ElementUsage(tag, num2, true));
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
	internal static void <GenerateEntries>g__AddCategoryEntry|8_0(string categoryId, string name, Sprite icon, Dictionary<string, CodexEntry> entries, ref CodexEntryGenerator_Elements.<>c__DisplayClass8_0 A_4)
	{
		CodexEntry codexEntry = CodexEntryGenerator.GenerateCategoryEntry(categoryId, name, entries, icon, true, true, null);
		codexEntry.parentId = CodexEntryGenerator_Elements.ELEMENTS_ID;
		codexEntry.category = CodexEntryGenerator_Elements.ELEMENTS_ID;
		A_4.entriesElements.Add(categoryId, codexEntry);
	}

	[CompilerGenerated]
	internal static void <GetElementEntryContext>g__CheckPrefab|12_0(GameObject prefab, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap made, ref CodexEntryGenerator_Elements.<>c__DisplayClass12_0 A_3, ref CodexEntryGenerator_Elements.<>c__DisplayClass12_1 A_4)
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
			IEnumerable<ElementConverter.ConsumedElement> consumedElements = elementConverter.consumedElements;
			foreach (ElementConverter.ConsumedElement consumedElement in (consumedElements ?? Enumerable.Empty<ElementConverter.ConsumedElement>()))
			{
				hashSet.Add(new ElementUsage(consumedElement.Tag, consumedElement.MassConsumptionRate, true));
			}
			IEnumerable<ElementConverter.OutputElement> outputElements = elementConverter.outputElements;
			foreach (ElementConverter.OutputElement outputElement in (outputElements ?? Enumerable.Empty<ElementConverter.OutputElement>()))
			{
				Tag tag2 = ElementLoader.FindElementByHash(outputElement.elementHash).tag;
				hashSet2.Add(new ElementUsage(tag2, outputElement.massGenerationRate, true));
			}
		}
		IEnumerable<ElementConsumer> components2 = prefab.GetComponents<ElementConsumer>();
		foreach (ElementConsumer elementConsumer in (components2 ?? Enumerable.Empty<ElementConsumer>()))
		{
			if (!elementConsumer.storeOnConsume)
			{
				Tag tag3 = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag;
				hashSet.Add(new ElementUsage(tag3, elementConsumer.consumptionRate, true));
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
		Crop component2 = prefab.GetComponent<Crop>();
		if (component2 != null)
		{
			hashSet2.Add(new ElementUsage(component2.cropId, (float)component2.cropVal.numProduced / component2.cropVal.cropDuration, true));
		}
		FlushToilet component3 = prefab.GetComponent<FlushToilet>();
		if (component3)
		{
			hashSet.Add(new ElementUsage(A_4.waterTag, component3.massConsumedPerUse, false));
			hashSet2.Add(new ElementUsage(A_4.dirtyWaterTag, component3.massEmittedPerUse, false));
		}
		HandSanitizer component4 = prefab.GetComponent<HandSanitizer>();
		if (component4)
		{
			Tag tag4 = ElementLoader.FindElementByHash(component4.consumedElement).tag;
			hashSet.Add(new ElementUsage(tag4, component4.massConsumedPerUse, false));
			if (component4.outputElement != SimHashes.Vacuum)
			{
				Tag tag5 = ElementLoader.FindElementByHash(component4.outputElement).tag;
				hashSet2.Add(new ElementUsage(tag5, component4.massConsumedPerUse, false));
			}
		}
		if (prefab.IsPrefabID("Moo"))
		{
			hashSet.Add(new ElementUsage("GasGrass", MooConfig.DAYS_PLANT_GROWTH_EATEN_PER_CYCLE, false));
			hashSet2.Add(new ElementUsage(ElementLoader.FindElementByHash(SimHashes.Milk).tag, MooTuning.MILK_PER_CYCLE, false));
		}
		CodexEntryGenerator_Elements.ConversionEntry conversionEntry = new CodexEntryGenerator_Elements.ConversionEntry();
		conversionEntry.title = prefab.GetProperName();
		conversionEntry.prefab = prefab;
		conversionEntry.inSet = hashSet;
		conversionEntry.outSet = hashSet2;
		if (hashSet.Count > 0 && hashSet2.Count > 0)
		{
			usedMap.Add(prefab.PrefabID(), conversionEntry);
		}
		foreach (ElementUsage elementUsage in hashSet)
		{
			usedMap.Add(elementUsage.tag, conversionEntry);
		}
		foreach (ElementUsage elementUsage2 in hashSet2)
		{
			A_3.madeMap.Add(elementUsage2.tag, conversionEntry);
		}
		PlantBranchGrower.Def def3 = prefab.GetDef<PlantBranchGrower.Def>();
		if (def3 != null)
		{
			GameObject prefab2 = Assets.GetPrefab(def3.BRANCH_PREFAB_NAME);
			if (prefab2 != null)
			{
				Crop component5 = prefab2.GetComponent<Crop>();
				if (component5 != null && (component2 == null || component5.cropId != component2.cropId || component5.cropVal.numProduced != component2.cropVal.numProduced))
				{
					CodexEntryGenerator_Elements.ConversionEntry conversionEntry2 = new CodexEntryGenerator_Elements.ConversionEntry();
					conversionEntry2.title = prefab2.GetProperName();
					conversionEntry2.prefab = prefab;
					usedMap.Add(prefab.PrefabID(), conversionEntry2);
					conversionEntry2.inSet = new HashSet<ElementUsage>();
					conversionEntry2.outSet = new HashSet<ElementUsage>();
					conversionEntry2.outSet.Add(new ElementUsage(component5.cropId, (float)component5.cropVal.numProduced, false));
					A_3.madeMap.Add(component5.cropId, conversionEntry2);
				}
			}
		}
		ScaleGrowthMonitor.Def def4 = prefab.GetDef<ScaleGrowthMonitor.Def>();
		if (def4 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry3 = new CodexEntryGenerator_Elements.ConversionEntry();
			conversionEntry3.title = Assets.GetPrefab("ShearingStation").GetProperName();
			conversionEntry3.prefab = Assets.GetPrefab("ShearingStation");
			conversionEntry3.inSet = new HashSet<ElementUsage>();
			conversionEntry3.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
			usedMap.Add(prefab.PrefabID(), conversionEntry3);
			conversionEntry3.outSet = new HashSet<ElementUsage>();
			conversionEntry3.outSet.Add(new ElementUsage(def4.itemDroppedOnShear, def4.dropMass, false));
			A_3.madeMap.Add(def4.itemDroppedOnShear, conversionEntry3);
		}
		WellFedShearable.Def def5 = prefab.GetDef<WellFedShearable.Def>();
		if (def5 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry4 = new CodexEntryGenerator_Elements.ConversionEntry();
			conversionEntry4.title = Assets.GetPrefab("ShearingStation").GetProperName();
			conversionEntry4.prefab = Assets.GetPrefab("ShearingStation");
			conversionEntry4.inSet = new HashSet<ElementUsage>();
			conversionEntry4.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
			usedMap.Add(prefab.PrefabID(), conversionEntry4);
			conversionEntry4.outSet = new HashSet<ElementUsage>();
			conversionEntry4.outSet.Add(new ElementUsage(def5.itemDroppedOnShear, def5.dropMass, false));
			A_3.madeMap.Add(def5.itemDroppedOnShear, conversionEntry4);
		}
		Butcherable component6 = prefab.GetComponent<Butcherable>();
		if (component6 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry5 = new CodexEntryGenerator_Elements.ConversionEntry();
			conversionEntry5.title = prefab.GetProperName();
			conversionEntry5.prefab = prefab;
			usedMap.Add(prefab.PrefabID(), conversionEntry5);
			conversionEntry5.outSet = new HashSet<ElementUsage>();
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			foreach (string text in component6.drops)
			{
				float num;
				dictionary.TryGetValue(text, out num);
				dictionary[text] = num + Assets.GetPrefab(text).GetComponent<PrimaryElement>().Mass;
			}
			foreach (KeyValuePair<string, float> keyValuePair in dictionary)
			{
				string text2;
				float num2;
				keyValuePair.Deconstruct<string, float>(out text2, out num2);
				string text3 = text2;
				float num3 = num2;
				conversionEntry5.outSet.Add(new ElementUsage(text3, num3, false));
				A_3.madeMap.Add(text3, conversionEntry5);
			}
		}
	}

	public static string ELEMENTS_ID = CodexCache.FormatLinkID("ELEMENTS");

	public static string ELEMENTS_SOLIDS_ID = CodexCache.FormatLinkID("ELEMENTS_SOLID");

	public static string ELEMENTS_LIQUIDS_ID = CodexCache.FormatLinkID("ELEMENTS_LIQUID");

	public static string ELEMENTS_GASES_ID = CodexCache.FormatLinkID("ELEMENTS_GAS");

	public static string ELEMENTS_OTHER_ID = CodexCache.FormatLinkID("ELEMENTS_OTHER");

	private static CodexEntryGenerator_Elements.ElementEntryContext contextInstance;

	public class ConversionEntry
	{
		public string title;

		public GameObject prefab;

		public HashSet<ElementUsage> inSet = new HashSet<ElementUsage>();

		public HashSet<ElementUsage> outSet = new HashSet<ElementUsage>();
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
