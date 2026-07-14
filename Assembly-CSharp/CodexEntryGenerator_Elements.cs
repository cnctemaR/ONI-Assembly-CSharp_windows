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
	private static Tag WaterTag
	{
		get
		{
			return ElementLoader.FindElementByHash(SimHashes.Water).tag;
		}
	}

	private static Tag DirtyWaterTag
	{
		get
		{
			return ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		}
	}

	public static Dictionary<string, CodexEntry> GenerateEntries()
	{
		CodexEntryGenerator_Elements.<>c__DisplayClass13_0 CS$<>8__locals1;
		CS$<>8__locals1.entriesElements = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary2 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary3 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary4 = new Dictionary<string, CodexEntry>();
		Dictionary<string, CodexEntry> dictionary5 = new Dictionary<string, CodexEntry>();
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|13_0(CodexEntryGenerator_Elements.ELEMENTS_SOLIDS_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, Assets.GetSprite("ui_elements-solid"), dictionary, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|13_0(CodexEntryGenerator_Elements.ELEMENTS_LIQUIDS_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, Assets.GetSprite("ui_elements-liquids"), dictionary2, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|13_0(CodexEntryGenerator_Elements.ELEMENTS_GASES_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, Assets.GetSprite("ui_elements-gases"), dictionary3, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|13_0(CodexEntryGenerator_Elements.ELEMENTS_OTHER_ID, UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, Assets.GetSprite("ui_elements-other"), dictionary4, ref CS$<>8__locals1);
		CodexEntryGenerator_Elements.<GenerateEntries>g__AddCategoryEntry|13_0(CodexEntryGenerator_Elements.ELEMENT_TYPES, UI.CODEX.CATEGORYNAMES.ELEMENTTYPES, Assets.GetSprite("ui_element_poperties"), dictionary5, ref CS$<>8__locals1);
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

	private static void AddPlantFiberInfo(ref HashSet<ElementUsage> inSet, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap madeMap, GameObject prefabOfProducer, GameObject prefabForPlayerFacing, Crop crop, Func<Tag, float, bool, string> customFormatting = null)
	{
		PlantFiberProducer plantFiberProducer;
		if (!prefabOfProducer.TryGetComponent<PlantFiberProducer>(out plantFiberProducer))
		{
			return;
		}
		CodexEntryGenerator_Elements.ConversionEntry conversionEntry = new CodexEntryGenerator_Elements.ConversionEntry();
		conversionEntry.title = prefabForPlayerFacing.GetProperName();
		conversionEntry.prefab = prefabForPlayerFacing;
		conversionEntry.inSet = inSet;
		conversionEntry.outSet.Add(new ElementUsage("PlantFiber", plantFiberProducer.amount / crop.cropVal.cropDuration, true, customFormatting));
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

	private static void CheckPrefab(GameObject prefab, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap madeMap)
	{
		HashSet<ElementUsage> hashSet = new HashSet<ElementUsage>();
		HashSet<ElementUsage> hashSet2 = new HashSet<ElementUsage>();
		List<ElementConverter> list = new List<ElementConverter>();
		List<ElementConverter> list2 = new List<ElementConverter>();
		List<ElementConverter> list3 = new List<ElementConverter>();
		CodexEntryGenerator_Elements.PartitionElementConverters(prefab, list, list2, list3);
		CodexEntryGenerator_Elements.CollectSharedConversionIO(prefab, hashSet, hashSet2, list);
		CodexEntryGenerator_Elements.RegisterConversionEntries(prefab, hashSet, hashSet2, usedMap, madeMap, list2);
		CodexEntryGenerator_Elements.AddIndependentConversionEntries(prefab, hashSet, usedMap, madeMap, list3);
	}

	private static void PartitionElementConverters(GameObject prefab, List<ElementConverter> outputOnlyConverters, List<ElementConverter> withInputsConverters, List<ElementConverter> categoryConverters)
	{
		IEnumerable<ElementConverter> components = prefab.GetComponents<ElementConverter>();
		foreach (ElementConverter elementConverter in (components ?? Enumerable.Empty<ElementConverter>()))
		{
			if (elementConverter.inputIsCategory)
			{
				categoryConverters.Add(elementConverter);
			}
			else if (elementConverter.consumedElements != null && elementConverter.consumedElements.Length != 0)
			{
				withInputsConverters.Add(elementConverter);
			}
			else
			{
				outputOnlyConverters.Add(elementConverter);
			}
		}
	}

	private static void VerifyClaimedByproduct(GameObject prefab, List<ElementConverter> withInputsConverters, List<ElementConverter> categoryConverters)
	{
		IConverterByproduct defImplementingInterface = prefab.GetDefImplementingInterface<IConverterByproduct>();
		if (defImplementingInterface == null)
		{
			return;
		}
		if (defImplementingInterface.ByproductRate <= 0f)
		{
			return;
		}
		bool flag = false;
		foreach (ElementConverter elementConverter in withInputsConverters)
		{
			ElementUsage elementUsage;
			if (CodexEntryGenerator_Elements.TryGetByproductUsage(defImplementingInterface, elementConverter, out elementUsage))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			foreach (ElementConverter elementConverter2 in categoryConverters)
			{
				ElementUsage elementUsage;
				if (CodexEntryGenerator_Elements.TryGetByproductUsage(defImplementingInterface, elementConverter2, out elementUsage))
				{
					flag = true;
					break;
				}
			}
		}
		DebugUtil.DevAssert(flag, "IConverterByproduct has no associated ElementConverter", null);
	}

	private static void RegisterConversionEntries(GameObject prefab, HashSet<ElementUsage> inSet, HashSet<ElementUsage> outSet, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap madeMap, List<ElementConverter> withInputsConverters)
	{
		IrrigationMonitor.Def def = prefab.GetDef<IrrigationMonitor.Def>();
		IConverterByproduct defImplementingInterface = prefab.GetDefImplementingInterface<IConverterByproduct>();
		if (withInputsConverters.Count == 0)
		{
			CodexEntryGenerator_Elements.RegisterIrrigationOrSingleEntry(prefab, inSet, outSet, usedMap, madeMap, def);
			return;
		}
		foreach (ElementConverter elementConverter in withInputsConverters)
		{
			HashSet<ElementUsage> hashSet = new HashSet<ElementUsage>(inSet);
			foreach (ElementConverter.ConsumedElement consumedElement in elementConverter.consumedElements)
			{
				hashSet.Add(new ElementUsage(consumedElement.Tag, consumedElement.MassConsumptionRate, true));
			}
			HashSet<ElementUsage> hashSet2 = new HashSet<ElementUsage>(outSet);
			IEnumerable<ElementConverter.OutputElement> outputElements = elementConverter.outputElements;
			foreach (ElementConverter.OutputElement outputElement in (outputElements ?? Enumerable.Empty<ElementConverter.OutputElement>()))
			{
				Tag tag = ElementLoader.FindElementByHash(outputElement.elementHash).tag;
				hashSet2.Add(new ElementUsage(tag, outputElement.massGenerationRate, true));
			}
			ElementUsage elementUsage;
			if (CodexEntryGenerator_Elements.TryGetByproductUsage(defImplementingInterface, elementConverter, out elementUsage))
			{
				hashSet2.Add(elementUsage);
			}
			CodexEntryGenerator_Elements.RegisterIrrigationOrSingleEntry(prefab, hashSet, hashSet2, usedMap, madeMap, def);
		}
	}

	private static void RegisterIrrigationOrSingleEntry(GameObject prefab, HashSet<ElementUsage> inSet, HashSet<ElementUsage> outSet, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap madeMap, IrrigationMonitor.Def irrigation)
	{
		if (irrigation != null)
		{
			foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in irrigation.consumedElements)
			{
				CodexEntryGenerator_Elements.RegisterSingleEntry(prefab, new HashSet<ElementUsage>(inSet)
				{
					new ElementUsage(consumeInfo.tag, consumeInfo.massConsumptionRate, true)
				}, outSet, usedMap, madeMap);
			}
			return;
		}
		CodexEntryGenerator_Elements.RegisterSingleEntry(prefab, inSet, outSet, usedMap, madeMap);
	}

	private static void RegisterSingleEntry(GameObject prefab, HashSet<ElementUsage> inSet, HashSet<ElementUsage> outSet, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap madeMap)
	{
		CodexEntryGenerator_Elements.ConversionEntry conversionEntry = new CodexEntryGenerator_Elements.ConversionEntry();
		conversionEntry.title = prefab.GetProperName();
		conversionEntry.prefab = prefab;
		conversionEntry.inSet = inSet;
		conversionEntry.outSet = outSet;
		if (inSet.Count > 0 && outSet.Count > 0)
		{
			usedMap.Add(prefab.PrefabID(), conversionEntry);
		}
		foreach (ElementUsage elementUsage in inSet)
		{
			usedMap.Add(elementUsage.tag, conversionEntry);
		}
		foreach (ElementUsage elementUsage2 in outSet)
		{
			madeMap.Add(elementUsage2.tag, conversionEntry);
		}
		Crop component = prefab.GetComponent<Crop>();
		if (component != null && prefab.GetComponent<IPlantConsumeEntities>() == null)
		{
			CodexEntryGenerator_Elements.AddPlantFiberInfo(ref inSet, usedMap, madeMap, prefab, prefab, component, null);
		}
	}

	private static bool TryGetByproductUsage(IConverterByproduct byproduct, ElementConverter conv, out ElementUsage usage)
	{
		usage = null;
		if (byproduct == null || byproduct.ByproductRate <= 0f)
		{
			return false;
		}
		IEnumerable<ElementConverter.ConsumedElement> consumedElements = conv.consumedElements;
		using (IEnumerator<ElementConverter.ConsumedElement> enumerator = (consumedElements ?? Enumerable.Empty<ElementConverter.ConsumedElement>()).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Tag == byproduct.ByproductAssociatedInputTag)
				{
					usage = new ElementUsage(byproduct.ByproductTag, byproduct.ByproductRate, byproduct.ByproductIsContinuous);
					return true;
				}
			}
		}
		return false;
	}

	private static void CollectSharedConversionIO(GameObject prefab, HashSet<ElementUsage> inSet, HashSet<ElementUsage> outSet, List<ElementConverter> outputOnlyConverters)
	{
		EnergyGenerator component = prefab.GetComponent<EnergyGenerator>();
		if (component)
		{
			IEnumerable<EnergyGenerator.InputItem> inputs = component.formula.inputs;
			foreach (EnergyGenerator.InputItem inputItem in (inputs ?? Enumerable.Empty<EnergyGenerator.InputItem>()))
			{
				inSet.Add(new ElementUsage(inputItem.tag, inputItem.consumptionRate, true));
			}
			IEnumerable<EnergyGenerator.OutputItem> outputs = component.formula.outputs;
			foreach (EnergyGenerator.OutputItem outputItem in (outputs ?? Enumerable.Empty<EnergyGenerator.OutputItem>()))
			{
				Tag tag = ElementLoader.FindElementByHash(outputItem.element).tag;
				outSet.Add(new ElementUsage(tag, outputItem.creationRate, true));
			}
		}
		foreach (ElementConverter elementConverter in outputOnlyConverters)
		{
			IEnumerable<ElementConverter.OutputElement> outputElements = elementConverter.outputElements;
			foreach (ElementConverter.OutputElement outputElement in (outputElements ?? Enumerable.Empty<ElementConverter.OutputElement>()))
			{
				Tag tag2 = ElementLoader.FindElementByHash(outputElement.elementHash).tag;
				outSet.Add(new ElementUsage(tag2, outputElement.massGenerationRate, true));
			}
		}
		IEnumerable<ElementConsumer> components = prefab.GetComponents<ElementConsumer>();
		foreach (ElementConsumer elementConsumer in (components ?? Enumerable.Empty<ElementConsumer>()))
		{
			if (!elementConsumer.storeOnConsume)
			{
				Tag tag3 = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag;
				inSet.Add(new ElementUsage(tag3, elementConsumer.consumptionRate, true));
			}
		}
		FertilizationMonitor.Def def = prefab.GetDef<FertilizationMonitor.Def>();
		if (def != null)
		{
			foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in def.consumedElements)
			{
				inSet.Add(new ElementUsage(consumeInfo.tag, consumeInfo.massConsumptionRate, true));
			}
		}
		Crop component2 = prefab.GetComponent<Crop>();
		if (component2 != null && prefab.GetComponent<IPlantConsumeEntities>() == null)
		{
			outSet.Add(new ElementUsage(component2.cropId, (float)component2.cropVal.numProduced / component2.cropVal.cropDuration, true));
		}
		FlushToilet component3 = prefab.GetComponent<FlushToilet>();
		if (component3)
		{
			inSet.Add(new ElementUsage(CodexEntryGenerator_Elements.WaterTag, component3.massConsumedPerUse, false));
			outSet.Add(new ElementUsage(CodexEntryGenerator_Elements.DirtyWaterTag, component3.massEmittedPerUse, false));
		}
		HandSanitizer component4 = prefab.GetComponent<HandSanitizer>();
		if (component4)
		{
			Tag tag4 = ElementLoader.FindElementByHash(component4.consumedElement).tag;
			inSet.Add(new ElementUsage(tag4, component4.massConsumedPerUse, false));
			if (component4.outputElement != SimHashes.Vacuum)
			{
				Tag tag5 = ElementLoader.FindElementByHash(component4.outputElement).tag;
				outSet.Add(new ElementUsage(tag5, component4.massConsumedPerUse, false));
			}
		}
	}

	private static void AddIndependentConversionEntries(GameObject prefab, HashSet<ElementUsage> inSet, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap madeMap, List<ElementConverter> categoryConverters)
	{
		Crop component = prefab.GetComponent<Crop>();
		IPlantConsumeEntities component2 = prefab.GetComponent<IPlantConsumeEntities>();
		foreach (ElementConverter elementConverter in categoryConverters)
		{
			List<CodexEntryGenerator_Elements.ConversionEntry> list = new List<CodexEntryGenerator_Elements.ConversionEntry>();
			IEnumerable<ElementConverter.ConsumedElement> consumedElements = elementConverter.consumedElements;
			using (IEnumerator<ElementConverter.ConsumedElement> enumerator2 = (consumedElements ?? Enumerable.Empty<ElementConverter.ConsumedElement>()).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					ElementConverter.ConsumedElement c2 = enumerator2.Current;
					foreach (Element element in ElementLoader.FindElements((Element e) => e.HasTag(c2.Tag)))
					{
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
				}
			}
			IEnumerable<ElementConverter.OutputElement> outputElements = elementConverter.outputElements;
			foreach (ElementConverter.OutputElement outputElement in (outputElements ?? Enumerable.Empty<ElementConverter.OutputElement>()))
			{
				ElementUsage elementUsage = new ElementUsage(ElementLoader.FindElementByHash(outputElement.elementHash).tag, outputElement.massGenerationRate, true);
				foreach (CodexEntryGenerator_Elements.ConversionEntry conversionEntry in list)
				{
					conversionEntry.outSet.Add(elementUsage);
				}
			}
			ElementUsage elementUsage2;
			if (CodexEntryGenerator_Elements.TryGetByproductUsage(prefab.GetDefImplementingInterface<IConverterByproduct>(), elementConverter, out elementUsage2))
			{
				foreach (CodexEntryGenerator_Elements.ConversionEntry conversionEntry2 in list)
				{
					conversionEntry2.outSet.Add(elementUsage2);
				}
			}
			foreach (CodexEntryGenerator_Elements.ConversionEntry conversionEntry3 in list)
			{
				if (conversionEntry3.inSet.Count > 0 && conversionEntry3.outSet.Count > 0)
				{
					usedMap.Add(prefab.PrefabID(), conversionEntry3);
				}
				foreach (ElementUsage elementUsage3 in conversionEntry3.inSet)
				{
					usedMap.Add(elementUsage3.tag, conversionEntry3);
				}
				foreach (ElementUsage elementUsage4 in conversionEntry3.outSet)
				{
					madeMap.Add(elementUsage4.tag, conversionEntry3);
				}
			}
		}
		IPlantBranchGrower defImplementingInterface = prefab.GetDefImplementingInterface<IPlantBranchGrower>();
		if (defImplementingInterface != null)
		{
			GameObject prefab2 = Assets.GetPrefab(defImplementingInterface.GetPlantBranchPrefabName());
			if (prefab2 != null)
			{
				Crop component3 = prefab2.GetComponent<Crop>();
				if (component3 != null && (component == null || component3.cropId != component.cropId || component3.cropVal.numProduced != component.cropVal.numProduced))
				{
					CodexEntryGenerator_Elements.ConversionEntry conversionEntry4 = new CodexEntryGenerator_Elements.ConversionEntry();
					conversionEntry4.title = prefab2.GetProperName();
					conversionEntry4.prefab = prefab;
					usedMap.Add(prefab.PrefabID(), conversionEntry4);
					conversionEntry4.inSet = new HashSet<ElementUsage>();
					IrrigationMonitor.Def def = prefab.GetDef<IrrigationMonitor.Def>();
					if (def != null)
					{
						foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in def.consumedElements)
						{
							conversionEntry4.inSet.Add(new ElementUsage(consumeInfo.tag, consumeInfo.massConsumptionRate, true));
						}
					}
					FertilizationMonitor.Def def2 = prefab.GetDef<FertilizationMonitor.Def>();
					if (def2 != null)
					{
						foreach (PlantElementAbsorber.ConsumeInfo consumeInfo2 in def2.consumedElements)
						{
							conversionEntry4.inSet.Add(new ElementUsage(consumeInfo2.tag, consumeInfo2.massConsumptionRate, true));
						}
					}
					conversionEntry4.outSet = new HashSet<ElementUsage>();
					int branchCount = defImplementingInterface.GetMaxBranchCount();
					conversionEntry4.outSet.Add(new ElementUsage(component3.cropId, (float)component3.cropVal.numProduced / component3.cropVal.cropDuration, true, (Tag t, float a, bool b) => GameUtil.GetFormattedBranchGrowerPlantProductionValuePerCycle(t, a, branchCount, true)));
					madeMap.Add(component3.cropId, conversionEntry4);
					CodexEntryGenerator_Elements.AddPlantFiberInfo(ref inSet, usedMap, madeMap, prefab2, prefab, component3, (Tag t, float a, bool b) => GameUtil.GetFormattedBranchGrowerPlantPlantFiberProductionValuePerCycle(t, a, branchCount, true));
				}
			}
		}
		if (component2 != null)
		{
			List<KPrefabID> prefabsOfPossiblePrey = component2.GetPrefabsOfPossiblePrey();
			List<string> list2 = new List<string>();
			foreach (KPrefabID kprefabID in prefabsOfPossiblePrey)
			{
				CreatureBrain component4 = kprefabID.GetComponent<CreatureBrain>();
				Tag tag3 = ((component4 == null) ? kprefabID.PrefabID() : component4.species);
				string text = tag3.ProperName();
				if (!list2.Contains(text))
				{
					CodexEntryGenerator_Elements.ConversionEntry conversionEntry5 = new CodexEntryGenerator_Elements.ConversionEntry();
					conversionEntry5.title = component2.GetConsumableEntitiesCategoryName() + ": " + text;
					conversionEntry5.prefab = prefab;
					conversionEntry5.inSet.Add(new ElementUsage(tag3, (component == null) ? 1f : (1f / component.cropVal.cropDuration), component != null, (Tag t, float amount, bool c) => GameUtil.GetFormattedUnits(amount, c ? GameUtil.TimeSlice.PerCycle : GameUtil.TimeSlice.None, true, "")));
					if (component != null)
					{
						conversionEntry5.outSet.Add(new ElementUsage(component.cropId, (float)component.cropVal.numProduced / component.cropVal.cropDuration, true));
						madeMap.Add(component.cropId, conversionEntry5);
					}
					usedMap.Add(prefab.PrefabID(), conversionEntry5);
					list2.Add(text);
				}
			}
		}
		ScaleGrowthMonitor.Def def3 = prefab.GetDef<ScaleGrowthMonitor.Def>();
		if (def3 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry6 = new CodexEntryGenerator_Elements.ConversionEntry();
			GameObject prefab3 = Assets.GetPrefab(prefab.GetComponent<KPrefabID>().HasTag(GameTags.SwimmingCreature) ? "UnderwaterShearingStation" : "ShearingStation");
			conversionEntry6.title = prefab3.GetProperName();
			conversionEntry6.prefab = prefab3;
			conversionEntry6.inSet = new HashSet<ElementUsage>();
			conversionEntry6.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
			usedMap.Add(prefab.PrefabID(), conversionEntry6);
			usedMap.Add(prefab3.PrefabID(), conversionEntry6);
			conversionEntry6.outSet = new HashSet<ElementUsage>();
			conversionEntry6.outSet.Add(new ElementUsage(def3.itemDroppedOnShear, def3.dropMass, false));
			madeMap.Add(def3.itemDroppedOnShear, conversionEntry6);
		}
		WellFedShearable.Def def4 = prefab.GetDef<WellFedShearable.Def>();
		if (def4 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry7 = new CodexEntryGenerator_Elements.ConversionEntry();
			GameObject prefab4 = Assets.GetPrefab(prefab.GetComponent<KPrefabID>().HasTag(GameTags.SwimmingCreature) ? "UnderwaterShearingStation" : "ShearingStation");
			conversionEntry7.title = prefab4.GetProperName();
			conversionEntry7.prefab = prefab4;
			conversionEntry7.inSet = new HashSet<ElementUsage>();
			conversionEntry7.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
			usedMap.Add(prefab.PrefabID(), conversionEntry7);
			usedMap.Add(prefab4.PrefabID(), conversionEntry7);
			conversionEntry7.outSet = new HashSet<ElementUsage>();
			conversionEntry7.outSet.Add(new ElementUsage(def4.itemDroppedOnShear, def4.dropMass, false));
			madeMap.Add(def4.itemDroppedOnShear, conversionEntry7);
		}
		FertilityShearable.Def def5 = prefab.GetDef<FertilityShearable.Def>();
		if (def5 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry8 = new CodexEntryGenerator_Elements.ConversionEntry();
			GameObject prefab5 = Assets.GetPrefab("UnderwaterMilkingStation");
			conversionEntry8.title = prefab5.GetProperName();
			conversionEntry8.prefab = prefab5;
			conversionEntry8.inSet = new HashSet<ElementUsage>
			{
				new ElementUsage(prefab.PrefabID(), 1f, false)
			};
			usedMap.Add(prefab.PrefabID(), conversionEntry8);
			usedMap.Add(prefab5.PrefabID(), conversionEntry8);
			Tag tag2 = def5.milkElement.CreateTag();
			conversionEntry8.outSet = new HashSet<ElementUsage>
			{
				new ElementUsage(tag2, def5.dropMass, false)
			};
			madeMap.Add(tag2, conversionEntry8);
		}
		MilkProductionMonitor.Def def6 = prefab.GetDef<MilkProductionMonitor.Def>();
		if (def6 != null)
		{
			string text2 = (prefab.GetComponent<KPrefabID>().HasTag(GameTags.SwimmingCreature) ? "UnderwaterMilkingStation" : "MilkingStation");
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry9 = new CodexEntryGenerator_Elements.ConversionEntry();
			GameObject prefab6 = Assets.GetPrefab(text2);
			conversionEntry9.title = prefab6.GetProperName();
			conversionEntry9.prefab = prefab6;
			conversionEntry9.inSet = new HashSet<ElementUsage>();
			conversionEntry9.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
			usedMap.Add(prefab.PrefabID(), conversionEntry9);
			usedMap.Add(prefab6.PrefabID(), conversionEntry9);
			conversionEntry9.outSet = new HashSet<ElementUsage>();
			conversionEntry9.outSet.Add(new ElementUsage(def6.element.CreateTag(), def6.Capacity, false));
			madeMap.Add(def6.element.CreateTag(), conversionEntry9);
		}
		MoistureMonitor.Def def7 = prefab.GetDef<MoistureMonitor.Def>();
		if (def7 != null)
		{
			string text3 = CODEX.HEADERS.SECRETED.Replace("{Creature}", prefab.GetProperName());
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry10 = CodexEntryGenerator_Elements.SimpleConversionBase(usedMap, prefab, text3);
			string id = Db.Get().Amounts.Mucus.deltaAttribute.Id;
			float num = 0f;
			foreach (AttributeModifier attributeModifier in Db.Get().traits.Get(prefab.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
			{
				if (attributeModifier.AttributeId == id)
				{
					num = attributeModifier.Value;
					break;
				}
			}
			float num2 = num + def7.GetMaxModification();
			ElementUsage elementUsage5 = new ElementUsage(def7.lubricant.CreateTag(), num2, true);
			elementUsage5.customFormating = (Tag tag, float amount, bool continous) => string.Format(CODEX.FORMAT_STRINGS.SECRETED, GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			ElementUsage elementUsage6 = elementUsage5;
			conversionEntry10.outSet.Add(elementUsage6);
			madeMap.Add(def7.lubricant.CreateTag(), conversionEntry10);
		}
		MoltDropperMonitor.Def def8 = prefab.GetDef<MoltDropperMonitor.Def>();
		if (def8 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry11 = CodexEntryGenerator_Elements.SimpleConversionBase(usedMap, prefab, CODEX.HEADERS.MOLTED.Replace("{Creature}", prefab.GetProperName()));
			ElementUsage elementUsage7 = new ElementUsage(def8.onGrowDropID, def8.massToDrop / 600f, true);
			elementUsage7.customFormating = (Tag tag, float amount, bool continous) => CODEX.FORMAT_STRINGS.MOLTED.Replace("{Amount}", GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			ElementUsage elementUsage8 = elementUsage7;
			conversionEntry11.outSet.Add(elementUsage8);
			madeMap.Add(def8.onGrowDropID, conversionEntry11);
		}
		Butcherable component5 = prefab.GetComponent<Butcherable>();
		if (component5 != null)
		{
			CodexEntryGenerator_Elements.ConversionEntry conversionEntry12 = new CodexEntryGenerator_Elements.ConversionEntry();
			conversionEntry12.title = prefab.GetProperName();
			conversionEntry12.prefab = prefab;
			usedMap.Add(prefab.PrefabID(), conversionEntry12);
			conversionEntry12.outSet = new HashSet<ElementUsage>();
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			foreach (KeyValuePair<string, float> keyValuePair in component5.drops)
			{
				float num3;
				dictionary.TryGetValue(keyValuePair.Key, out num3);
				dictionary[keyValuePair.Key] = num3 + Assets.GetPrefab(keyValuePair.Key).GetComponent<PrimaryElement>().Mass * keyValuePair.Value;
			}
			foreach (KeyValuePair<string, float> keyValuePair2 in dictionary)
			{
				string text4;
				float num4;
				keyValuePair2.Deconstruct(out text4, out num4);
				string text5 = text4;
				float num5 = num4;
				conversionEntry12.outSet.Add(new ElementUsage(text5, num5, false));
				madeMap.Add(text5, conversionEntry12);
			}
		}
	}

	private static void AddDietConversions(GameObject prefab, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap madeMap)
	{
		Diet diet = null;
		CreatureCalorieMonitor.Def def = prefab.GetDef<CreatureCalorieMonitor.Def>();
		if (def != null)
		{
			diet = def.diet;
		}
		else
		{
			BeehiveCalorieMonitor.Def def2 = prefab.GetDef<BeehiveCalorieMonitor.Def>();
			if (def2 != null)
			{
				diet = def2.diet;
			}
		}
		if (diet == null)
		{
			return;
		}
		float num = 0f;
		foreach (AttributeModifier attributeModifier in Db.Get().traits.Get(prefab.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
		{
			if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
			{
				num = attributeModifier.Value;
			}
		}
		foreach (Diet.Info info in diet.infos)
		{
			foreach (Tag tag in info.consumedTags)
			{
				float num2 = -num / info.caloriesPerKg;
				float num3 = num2 * info.producedConversionRate;
				bool flag = diet.IsConsumedTagAbleToBeEatenDirectly(tag);
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
						float num4 = diet.AvailableCaloriesInPrey(tag);
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
				conversionEntry.title = prefab.GetProperName();
				conversionEntry.prefab = prefab;
				conversionEntry.inSet.Add(elementUsage);
				conversionEntry.outSet.Add(new ElementUsage(info.producedElement, num3, true));
				usedMap.Add(tag, conversionEntry);
				madeMap.Add(info.producedElement, conversionEntry);
			}
		}
	}

	public static CodexEntryGenerator_Elements.ElementEntryContext GetElementEntryContext()
	{
		if (CodexEntryGenerator_Elements.contextInstance != null)
		{
			return CodexEntryGenerator_Elements.contextInstance;
		}
		CodexEntryGenerator_Elements.CodexElementMap codexElementMap = new CodexEntryGenerator_Elements.CodexElementMap();
		CodexEntryGenerator_Elements.CodexElementMap codexElementMap2 = new CodexEntryGenerator_Elements.CodexElementMap();
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
					CodexEntryGenerator_Elements.CheckPrefab(buildingDef.BuildingComplete, codexElementMap, codexElementMap2);
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
				CodexEntryGenerator_Elements.CheckPrefab(gameObject2, codexElementMap, codexElementMap2);
			}
		}
		List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<CreatureBrain>();
		foreach (GameObject gameObject3 in prefabsWithComponent)
		{
			if (gameObject3.GetDef<BabyMonitor.Def>() == null)
			{
				CodexEntryGenerator_Elements.CheckPrefab(gameObject3, codexElementMap, codexElementMap2);
			}
		}
		foreach (GameObject gameObject4 in prefabsWithComponent)
		{
			if (gameObject4.GetDef<BabyMonitor.Def>() == null)
			{
				CodexEntryGenerator_Elements.AddDietConversions(gameObject4, codexElementMap, codexElementMap2);
			}
		}
		CodexEntryGenerator_Elements.contextInstance = new CodexEntryGenerator_Elements.ElementEntryContext
		{
			usedMap = codexElementMap,
			madeMap = codexElementMap2
		};
		return CodexEntryGenerator_Elements.contextInstance;
	}

	private static CodexEntryGenerator_Elements.ConversionEntry SimpleConversionBase(CodexEntryGenerator_Elements.CodexElementMap usedMap, GameObject prefab, string title = null)
	{
		CodexEntryGenerator_Elements.ConversionEntry conversionEntry = new CodexEntryGenerator_Elements.ConversionEntry
		{
			title = ((title == null) ? prefab.GetProperName() : title),
			prefab = prefab,
			inSet = new HashSet<ElementUsage>()
		};
		usedMap.Add(prefab.PrefabID(), conversionEntry);
		return conversionEntry;
	}

	[CompilerGenerated]
	internal static void <GenerateEntries>g__AddCategoryEntry|13_0(string categoryId, string name, Sprite icon, Dictionary<string, CodexEntry> entries, ref CodexEntryGenerator_Elements.<>c__DisplayClass13_0 A_4)
	{
		CodexEntry codexEntry = CodexEntryGenerator.GenerateCategoryEntry(categoryId, name, entries, icon, true, true, null);
		codexEntry.parentId = CodexEntryGenerator_Elements.ELEMENTS_ID;
		codexEntry.category = CodexEntryGenerator_Elements.ELEMENTS_ID;
		A_4.entriesElements.Add(categoryId, codexEntry);
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
