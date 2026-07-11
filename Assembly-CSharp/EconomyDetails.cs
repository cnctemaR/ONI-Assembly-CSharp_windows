using System;
using System.Collections.Generic;
using System.IO;
using Klei.AI;
using ProcGen;
using TUNING;
using UnityEngine;

public class EconomyDetails
{
	public EconomyDetails()
	{
		this.massResourceType = new EconomyDetails.Resource.Type("Mass", "kg");
		this.energyResourceType = new EconomyDetails.Resource.Type("Energy", "joules");
		this.timeResourceType = new EconomyDetails.Resource.Type("Time", "seconds");
		this.attributeResourceType = new EconomyDetails.Resource.Type("Attribute", "units");
		this.caloriesResourceType = new EconomyDetails.Resource.Type("Calories", "cal");
		this.amountResourceType = new EconomyDetails.Resource.Type("Amount", "units");
		this.buildingTransformationType = new EconomyDetails.Transformation.Type("Building");
		this.foodTransformationType = new EconomyDetails.Transformation.Type("Food");
		this.plantTransformationType = new EconomyDetails.Transformation.Type("Plant");
		this.creatureTransformationType = new EconomyDetails.Transformation.Type("Creature");
		this.dupeTransformationType = new EconomyDetails.Transformation.Type("Duplicant");
		this.referenceTransformationType = new EconomyDetails.Transformation.Type("Reference");
		this.effectTransformationType = new EconomyDetails.Transformation.Type("Effect");
		this.geyserActivePeriodTransformationType = new EconomyDetails.Transformation.Type("GeyserActivePeriod");
		this.geyserLifetimeTransformationType = new EconomyDetails.Transformation.Type("GeyserLifetime");
		this.energyResource = this.CreateResource(TagManager.Create("Energy"), this.energyResourceType);
		this.heatResource = this.CreateResource(TagManager.Create("Heat"), this.energyResourceType);
		this.duplicantTimeResource = this.CreateResource(TagManager.Create("DupeTime"), this.timeResourceType);
		this.caloriesResource = this.CreateResource(new Tag(Db.Get().Amounts.Calories.deltaAttribute.Id), this.amountResourceType);
		foreach (Element element in ElementLoader.elements)
		{
			this.CreateResource(element);
		}
		this.GatherStartingBiomeAmounts();
		foreach (KPrefabID kprefabID in Assets.Prefabs)
		{
			this.CreateTransformation(kprefabID, kprefabID.PrefabTag);
			if (kprefabID.GetComponent<GeyserConfigurator>() != null)
			{
				this.CreateTransformation(kprefabID, kprefabID.PrefabTag + "_ActiveOnly");
			}
		}
		foreach (Effect effect in Db.Get().effects.resources)
		{
			this.CreateTransformation(effect);
		}
		EconomyDetails.Transformation transformation = new EconomyDetails.Transformation(TagManager.Create("Duplicant"), this.dupeTransformationType, 1f);
		transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Oxygen), -0.1f));
		transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.CarbonDioxide), 0.1f * Assets.GetPrefab(MinionConfig.ID).GetComponent<OxygenBreather>().O2toCO2conversion));
		transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.duplicantTimeResource, 0.875f));
		transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.caloriesResource, -1666.6666f));
		transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), this.amountResourceType), 0.16666667f));
		this.transformations.Add(transformation);
		EconomyDetails.Transformation transformation2 = new EconomyDetails.Transformation(TagManager.Create("Electrolysis"), this.referenceTransformationType, 1f);
		transformation2.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Oxygen), 1.7777778f));
		transformation2.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Hydrogen), 0.22222222f));
		transformation2.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Water), -2f));
		this.transformations.Add(transformation2);
		EconomyDetails.Transformation transformation3 = new EconomyDetails.Transformation(TagManager.Create("MethaneCombustion"), this.referenceTransformationType, 1f);
		transformation3.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Methane), -1f));
		transformation3.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Oxygen), -4f));
		transformation3.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.CarbonDioxide), 2.75f));
		transformation3.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Water), 2.25f));
		this.transformations.Add(transformation3);
		EconomyDetails.Transformation transformation4 = new EconomyDetails.Transformation(TagManager.Create("CoalCombustion"), this.referenceTransformationType, 1f);
		transformation4.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Carbon), -1f));
		transformation4.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.Oxygen), -2.6666667f));
		transformation4.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(GameTags.CarbonDioxide), 3.6666667f));
		this.transformations.Add(transformation4);
	}

	private static void WriteProduct(StreamWriter o, string a, string b, string c)
	{
		o.Write(string.Concat(new string[] { "\"=PRODUCT(", a, ", ", b, ", ", c, ")\"" }));
	}

	public void DumpTransformations(EconomyDetails.Scenario scenario, StreamWriter o)
	{
		List<EconomyDetails.Resource> used_resources = new List<EconomyDetails.Resource>();
		foreach (EconomyDetails.Transformation transformation in this.transformations)
		{
			if (scenario.IncludesTransformation(transformation))
			{
				foreach (EconomyDetails.Transformation.Delta delta in transformation.deltas)
				{
					if (!used_resources.Contains(delta.resource))
					{
						used_resources.Add(delta.resource);
					}
				}
			}
		}
		used_resources.Sort((EconomyDetails.Resource x, EconomyDetails.Resource y) => x.tag.Name.CompareTo(y.tag.Name));
		List<EconomyDetails.Ratio> list = new List<EconomyDetails.Ratio>();
		list.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Algae), this.GetResource(GameTags.Oxygen), false));
		list.Add(new EconomyDetails.Ratio(this.energyResource, this.GetResource(GameTags.Oxygen), false));
		list.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Oxygen), this.energyResource, false));
		list.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Water), this.GetResource(GameTags.Oxygen), false));
		list.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.DirtyWater), this.caloriesResource, false));
		list.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Water), this.caloriesResource, false));
		list.Add(new EconomyDetails.Ratio(this.GetResource(GameTags.Fertilizer), this.caloriesResource, false));
		list.Add(new EconomyDetails.Ratio(this.energyResource, this.CreateResource(new Tag(Db.Get().Amounts.Stress.deltaAttribute.Id), this.amountResourceType), true));
		list.RemoveAll((EconomyDetails.Ratio x) => !used_resources.Contains(x.input) || !used_resources.Contains(x.output));
		o.Write("Id");
		o.Write(",Count");
		o.Write(",Type");
		o.Write(",Time(s)");
		int num = 4;
		foreach (EconomyDetails.Resource resource in used_resources)
		{
			o.Write(string.Concat(new string[]
			{
				", ",
				resource.tag.Name,
				"(",
				resource.type.unit,
				")"
			}));
			num++;
		}
		o.Write(",MassDelta");
		foreach (EconomyDetails.Ratio ratio in list)
		{
			o.Write(string.Concat(new string[]
			{
				", ",
				ratio.output.tag.Name,
				"(",
				ratio.output.type.unit,
				")/",
				ratio.input.tag.Name,
				"(",
				ratio.input.type.unit,
				")"
			}));
			num++;
		}
		string text = "B";
		o.Write("\n");
		int num2 = 1;
		this.transformations.Sort((EconomyDetails.Transformation x, EconomyDetails.Transformation y) => x.tag.Name.CompareTo(y.tag.Name));
		for (int i = 0; i < this.transformations.Count; i++)
		{
			EconomyDetails.Transformation transformation2 = this.transformations[i];
			if (scenario.IncludesTransformation(transformation2))
			{
				num2++;
			}
		}
		string text2 = "B" + (num2 + 4).ToString();
		int num3 = 1;
		for (int j = 0; j < this.transformations.Count; j++)
		{
			EconomyDetails.Transformation transformation3 = this.transformations[j];
			if (scenario.IncludesTransformation(transformation3))
			{
				if (transformation3.tag == new Tag(EconomyDetails.debugTag))
				{
					int num4 = 0;
					num4++;
				}
				num3++;
				o.Write("\"" + transformation3.tag.Name + "\"");
				o.Write("," + scenario.GetCount(transformation3.tag).ToString());
				o.Write(",\"" + transformation3.type.id + "\"");
				o.Write(",\"" + transformation3.timeInSeconds.ToString("0.00") + "\"");
				string text3 = text + num3.ToString();
				float num5 = 0f;
				bool flag = false;
				foreach (EconomyDetails.Resource resource2 in used_resources)
				{
					float num6 = 0f;
					foreach (EconomyDetails.Transformation.Delta delta2 in transformation3.deltas)
					{
						if (delta2.resource.tag == resource2.tag)
						{
							num6 += delta2.amount;
							if (delta2.resource.type == this.massResourceType)
							{
								flag = true;
								num5 += num6;
							}
						}
					}
					o.Write(",");
					if (num6 != 0f)
					{
						num6 /= transformation3.timeInSeconds;
						EconomyDetails.WriteProduct(o, text3, num6.ToString("0.00000"), text2);
					}
				}
				o.Write(",");
				if (flag)
				{
					num5 /= transformation3.timeInSeconds;
					EconomyDetails.WriteProduct(o, text3, num5.ToString("0.00000"), text2);
				}
				foreach (EconomyDetails.Ratio ratio2 in list)
				{
					o.Write(", ");
					EconomyDetails.Transformation.Delta delta3 = transformation3.GetDelta(ratio2.input);
					EconomyDetails.Transformation.Delta delta4 = transformation3.GetDelta(ratio2.output);
					if (delta4 != null && delta3 != null && delta3.amount < 0f && (delta4.amount > 0f || ratio2.allowNegativeOutput))
					{
						o.Write(delta4.amount / Mathf.Abs(delta3.amount));
					}
				}
				o.Write("\n");
			}
		}
		int num7 = 4;
		for (int k = 0; k < num; k++)
		{
			if (k >= num7 && k < num7 + used_resources.Count)
			{
				string text4 = ((char)(65 + k % 26)).ToString();
				int num8 = Mathf.FloorToInt((float)k / 26f);
				if (num8 > 0)
				{
					text4 = ((char)(65 + num8 - 1)).ToString() + text4;
				}
				o.Write(string.Concat(new string[]
				{
					"\"=SUM(",
					text4,
					"2: ",
					text4,
					num2.ToString(),
					")\""
				}));
			}
			o.Write(",");
		}
		string text5 = "B" + (num2 + 5).ToString();
		o.Write("\n");
		o.Write("\nTiming:");
		o.Write("\nTimeInSeconds:," + scenario.timeInSeconds);
		o.Write("\nSecondsPerCycle:," + 600f.ToString());
		o.Write("\nCycles:,=" + text2 + "/" + text5);
	}

	public EconomyDetails.Resource CreateResource(Tag tag, EconomyDetails.Resource.Type resource_type)
	{
		foreach (EconomyDetails.Resource resource in this.resources)
		{
			if (resource.tag == tag)
			{
				return resource;
			}
		}
		EconomyDetails.Resource resource2 = new EconomyDetails.Resource(tag, resource_type);
		this.resources.Add(resource2);
		return resource2;
	}

	public EconomyDetails.Resource CreateResource(Element element)
	{
		return this.CreateResource(element.tag, this.massResourceType);
	}

	public EconomyDetails.Transformation CreateTransformation(Effect effect)
	{
		EconomyDetails.Transformation transformation = new EconomyDetails.Transformation(new Tag(effect.Id), this.effectTransformationType, 1f);
		foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
		{
			EconomyDetails.Resource resource = this.CreateResource(new Tag(attributeModifier.AttributeId), this.attributeResourceType);
			transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, attributeModifier.Value));
		}
		this.transformations.Add(transformation);
		return transformation;
	}

	public EconomyDetails.Transformation GetTransformation(Tag tag)
	{
		foreach (EconomyDetails.Transformation transformation in this.transformations)
		{
			if (transformation.tag == tag)
			{
				return transformation;
			}
		}
		return null;
	}

	public EconomyDetails.Transformation CreateTransformation(KPrefabID prefab_id, Tag tag)
	{
		if (tag == new Tag(EconomyDetails.debugTag))
		{
			int num = 0;
			num++;
		}
		ElementConverter component = prefab_id.GetComponent<ElementConverter>();
		EnergyConsumer component2 = prefab_id.GetComponent<EnergyConsumer>();
		ElementConsumer component3 = prefab_id.GetComponent<ElementConsumer>();
		BuildingElementEmitter component4 = prefab_id.GetComponent<BuildingElementEmitter>();
		Generator component5 = prefab_id.GetComponent<Generator>();
		EnergyGenerator component6 = prefab_id.GetComponent<EnergyGenerator>();
		ManualGenerator component7 = prefab_id.GetComponent<ManualGenerator>();
		ManualDeliveryKG[] components = prefab_id.GetComponents<ManualDeliveryKG>();
		StateMachineController component8 = prefab_id.GetComponent<StateMachineController>();
		Edible component9 = prefab_id.GetComponent<Edible>();
		Crop component10 = prefab_id.GetComponent<Crop>();
		Uprootable component11 = prefab_id.GetComponent<Uprootable>();
		Recipe recipe = RecipeManager.Get().recipes.Find((Recipe r) => r.Result == prefab_id.PrefabTag);
		List<FertilizationMonitor.Def> list = null;
		List<IrrigationMonitor.Def> list2 = null;
		GeyserConfigurator component12 = prefab_id.GetComponent<GeyserConfigurator>();
		Toilet component13 = prefab_id.GetComponent<Toilet>();
		FlushToilet component14 = prefab_id.GetComponent<FlushToilet>();
		RelaxationPoint component15 = prefab_id.GetComponent<RelaxationPoint>();
		CreatureCalorieMonitor.Def def = prefab_id.gameObject.GetDef<CreatureCalorieMonitor.Def>();
		if (component8 != null)
		{
			list = component8.GetDefs<FertilizationMonitor.Def>();
			list2 = component8.GetDefs<IrrigationMonitor.Def>();
		}
		EconomyDetails.Transformation transformation = null;
		float num2 = 1f;
		if (component9 != null)
		{
			transformation = new EconomyDetails.Transformation(tag, this.foodTransformationType, num2);
		}
		else if (component != null || component2 != null || component3 != null || component4 != null || component5 != null || component6 != null || component11 != null || component12 != null || component13 != null || component14 != null || component15 != null || def != null)
		{
			if (component11 != null || component10 != null)
			{
				if (component10 != null)
				{
					num2 = component10.cropVal.cropDuration;
				}
				transformation = new EconomyDetails.Transformation(tag, this.plantTransformationType, num2);
			}
			else if (def != null)
			{
				transformation = new EconomyDetails.Transformation(tag, this.creatureTransformationType, num2);
			}
			else if (component12 != null)
			{
				GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration = new GeyserConfigurator.GeyserInstanceConfiguration
				{
					typeId = component12.presetType,
					rateRoll = 0.5f,
					iterationLengthRoll = 0.5f,
					iterationPercentRoll = 0.5f,
					yearLengthRoll = 0.5f,
					yearPercentRoll = 0.5f
				};
				if (tag.Name.Contains("_ActiveOnly"))
				{
					float iterationLength = geyserInstanceConfiguration.GetIterationLength();
					transformation = new EconomyDetails.Transformation(tag, this.geyserActivePeriodTransformationType, iterationLength);
				}
				else
				{
					float yearLength = geyserInstanceConfiguration.GetYearLength();
					transformation = new EconomyDetails.Transformation(tag, this.geyserLifetimeTransformationType, yearLength);
				}
			}
			else
			{
				if (component13 != null || component14 != null)
				{
					num2 = 600f;
				}
				transformation = new EconomyDetails.Transformation(tag, this.buildingTransformationType, num2);
			}
		}
		if (transformation != null)
		{
			if (component != null && component.consumedElements != null)
			{
				foreach (ElementConverter.ConsumedElement consumedElement in component.consumedElements)
				{
					EconomyDetails.Resource resource = this.CreateResource(consumedElement.tag, this.massResourceType);
					transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource, -consumedElement.massConsumptionRate));
				}
				if (component.outputElements != null)
				{
					foreach (ElementConverter.OutputElement outputElement in component.outputElements)
					{
						Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
						EconomyDetails.Resource resource2 = this.CreateResource(element.tag, this.massResourceType);
						transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource2, outputElement.massGenerationRate));
					}
				}
			}
			if (component3 != null && component6 == null && (component == null || prefab_id.GetComponent<AlgaeHabitat>() != null))
			{
				EconomyDetails.Resource resource3 = this.GetResource(ElementLoader.FindElementByHash(component3.elementToConsume).tag);
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource3, -component3.consumptionRate));
			}
			if (component2 != null)
			{
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.energyResource, -component2.WattsNeededWhenActive));
			}
			if (component4 != null)
			{
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(component4.element), component4.emitRate));
			}
			if (component5 != null)
			{
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.energyResource, component5.GetComponent<Building>().Def.GeneratorWattageRating));
			}
			if (component6 != null)
			{
				if (component6.formula.inputs != null)
				{
					foreach (EnergyGenerator.InputItem inputItem in component6.formula.inputs)
					{
						transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(inputItem.tag), -inputItem.consumptionRate));
					}
				}
				if (component6.formula.outputs != null)
				{
					foreach (EnergyGenerator.OutputItem outputItem in component6.formula.outputs)
					{
						transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(outputItem.element), outputItem.creationRate));
					}
				}
			}
			if (GameComps.StructureTemperatures.Has(prefab_id.gameObject))
			{
				BuildingDef def2 = prefab_id.GetComponent<BuildingComplete>().Def;
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.heatResource, def2.SelfHeatKilowattsWhenActive + def2.ExhaustKilowattsWhenActive));
			}
			if (component7)
			{
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.duplicantTimeResource, -1f));
			}
			if (component9)
			{
				EdiblesManager.FoodInfo foodInfo = component9.FoodInfo;
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.caloriesResource, foodInfo.CaloriesPerUnit));
			}
			if (component10 != null)
			{
				EconomyDetails.Resource resource4 = this.CreateResource(TagManager.Create(component10.cropVal.cropId), this.amountResourceType);
				float num3 = (float)component10.cropVal.numProduced;
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource4, num3));
				GameObject prefab = Assets.GetPrefab(new Tag(component10.cropVal.cropId));
				if (prefab != null)
				{
					Edible component16 = prefab.GetComponent<Edible>();
					if (component16 != null)
					{
						transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.caloriesResource, component16.FoodInfo.CaloriesPerUnit * num3));
					}
				}
			}
			if (recipe != null)
			{
				EconomyDetails.Resource resource5;
				foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
				{
					resource5 = this.CreateResource(ingredient.tag, this.amountResourceType);
					transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource5, -ingredient.amount));
				}
				resource5 = this.CreateResource(recipe.Result, this.amountResourceType);
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource5, recipe.OutputUnits));
			}
			if (components != null)
			{
				for (int m = 0; m < components.Length; m++)
				{
					transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.duplicantTimeResource, -0.1f * transformation.timeInSeconds));
				}
			}
			if (list != null && list.Count > 0)
			{
				foreach (FertilizationMonitor.Def def3 in list)
				{
					foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in def3.consumedElements)
					{
						EconomyDetails.Resource resource6 = this.CreateResource(consumeInfo.tag, this.massResourceType);
						transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource6, -consumeInfo.massConsumptionRate * transformation.timeInSeconds));
					}
				}
			}
			if (list2 != null && list2.Count > 0)
			{
				foreach (IrrigationMonitor.Def def4 in list2)
				{
					foreach (PlantElementAbsorber.ConsumeInfo consumeInfo2 in def4.consumedElements)
					{
						EconomyDetails.Resource resource7 = this.CreateResource(consumeInfo2.tag, this.massResourceType);
						transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource7, -consumeInfo2.massConsumptionRate * transformation.timeInSeconds));
					}
				}
			}
			if (component12 != null)
			{
				GeyserConfigurator.GeyserInstanceConfiguration geyserInstanceConfiguration2 = new GeyserConfigurator.GeyserInstanceConfiguration
				{
					typeId = component12.presetType,
					rateRoll = 0.5f,
					iterationLengthRoll = 0.5f,
					iterationPercentRoll = 0.5f,
					yearLengthRoll = 0.5f,
					yearPercentRoll = 0.5f
				};
				if (tag.Name.Contains("_ActiveOnly"))
				{
					float num5 = geyserInstanceConfiguration2.GetMassPerCycle() / 600f * geyserInstanceConfiguration2.GetIterationLength();
					transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(geyserInstanceConfiguration2.GetElement().CreateTag(), this.massResourceType), num5));
				}
				else
				{
					float num6 = geyserInstanceConfiguration2.GetMassPerCycle() / 600f * geyserInstanceConfiguration2.GetYearLength() * geyserInstanceConfiguration2.GetYearPercent();
					transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(geyserInstanceConfiguration2.GetElement().CreateTag(), this.massResourceType), num6));
				}
			}
			if (component13 != null)
			{
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), this.amountResourceType), -0.16666667f));
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(SimHashes.Dirt), -component13.solidWastePerUse.mass));
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(component13.solidWastePerUse.elementID), component13.solidWastePerUse.mass));
			}
			if (component14 != null)
			{
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(new Tag(Db.Get().Amounts.Bladder.deltaAttribute.Id), this.amountResourceType), -0.16666667f));
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(SimHashes.Water), -component14.massConsumedPerUse));
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.GetResource(SimHashes.DirtyWater), component14.massEmittedPerUse));
			}
			if (component15 != null)
			{
				Effect effect = component15.CreateEffect();
				foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
				{
					EconomyDetails.Resource resource8 = this.CreateResource(new Tag(attributeModifier.AttributeId), this.attributeResourceType);
					transformation.AddDelta(new EconomyDetails.Transformation.Delta(resource8, attributeModifier.Value));
				}
			}
			if (def != null)
			{
				this.CollectDietTransformations(prefab_id);
			}
			this.transformations.Add(transformation);
		}
		return transformation;
	}

	private void CollectDietTransformations(KPrefabID prefab_id)
	{
		Trait trait = Db.Get().traits.Get(prefab_id.GetComponent<Modifiers>().initialTraits[0]);
		CreatureCalorieMonitor.Def def = prefab_id.gameObject.GetDef<CreatureCalorieMonitor.Def>();
		WildnessMonitor.Def def2 = prefab_id.gameObject.GetDef<WildnessMonitor.Def>();
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.AddRange(trait.SelfModifiers);
		list.AddRange(def2.tameEffect.SelfModifiers);
		float num = 0f;
		float num2 = 0f;
		foreach (AttributeModifier attributeModifier in list)
		{
			if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.maxAttribute.Id)
			{
				num = attributeModifier.Value;
			}
			if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
			{
				num2 = attributeModifier.Value;
			}
		}
		foreach (Diet.Info info in def.diet.infos)
		{
			foreach (Tag tag in info.consumedTags)
			{
				float num3 = Mathf.Abs(num / num2);
				float num4 = num / info.caloriesPerKg;
				float num5 = num4 * info.producedConversionRate;
				EconomyDetails.Transformation transformation = new EconomyDetails.Transformation(new Tag(prefab_id.PrefabTag.Name + "Diet" + tag.Name), this.creatureTransformationType, num3);
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(tag, this.massResourceType), -num4));
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.CreateResource(new Tag(info.producedElement.ToString()), this.massResourceType), num5));
				transformation.AddDelta(new EconomyDetails.Transformation.Delta(this.caloriesResource, num));
				this.transformations.Add(transformation);
			}
		}
	}

	private static void CollectDietScenarios(List<EconomyDetails.Scenario> scenarios)
	{
		EconomyDetails.Scenario scenario = new EconomyDetails.Scenario("diets/all", 0, null);
		foreach (KPrefabID kprefabID in Assets.Prefabs)
		{
			CreatureCalorieMonitor.Def def = kprefabID.gameObject.GetDef<CreatureCalorieMonitor.Def>();
			if (def != null)
			{
				EconomyDetails.Scenario scenario2 = new EconomyDetails.Scenario("diets/" + kprefabID.name, 0, null);
				foreach (Diet.Info info in def.diet.infos)
				{
					foreach (Tag tag in info.consumedTags)
					{
						Tag tag2 = kprefabID.PrefabTag.Name + "Diet" + tag.Name;
						scenario2.AddEntry(new EconomyDetails.Scenario.Entry(tag2, 1));
						scenario.AddEntry(new EconomyDetails.Scenario.Entry(tag2, 1));
					}
				}
				scenarios.Add(scenario2);
			}
		}
		scenarios.Add(scenario);
	}

	public void GatherStartingBiomeAmounts()
	{
		for (int i = 0; i < Grid.CellCount; i++)
		{
			if (global::World.Instance.zoneRenderData.worldZoneTypes[i] == SubWorld.ZoneType.Sandstone)
			{
				Element element = Grid.Element[i];
				float num = 0f;
				this.startingBiomeAmounts.TryGetValue(element, out num);
				this.startingBiomeAmounts[element] = num + Grid.Mass[i];
				this.startingBiomeCellCount++;
			}
		}
	}

	public EconomyDetails.Resource GetResource(SimHashes element)
	{
		return this.GetResource(ElementLoader.FindElementByHash(element).tag);
	}

	public EconomyDetails.Resource GetResource(Tag tag)
	{
		foreach (EconomyDetails.Resource resource in this.resources)
		{
			if (resource.tag == tag)
			{
				return resource;
			}
		}
		return null;
	}

	private float GetDupeBreathingPerSecond(EconomyDetails details)
	{
		EconomyDetails.Transformation transformation = details.GetTransformation(TagManager.Create("Duplicant"));
		return transformation.GetDelta(details.GetResource(GameTags.Oxygen)).amount;
	}

	private EconomyDetails.BiomeTransformation CreateBiomeTransformationFromTransformation(EconomyDetails details, Tag transformation_tag, Tag input_resource_tag, Tag output_resource_tag)
	{
		EconomyDetails.Resource resource = details.GetResource(input_resource_tag);
		EconomyDetails.Resource resource2 = details.GetResource(output_resource_tag);
		EconomyDetails.Transformation transformation = details.GetTransformation(transformation_tag);
		float num = transformation.GetDelta(resource2).amount / -transformation.GetDelta(resource).amount;
		float num2 = this.GetDupeBreathingPerSecond(details) * 600f;
		return new EconomyDetails.BiomeTransformation((transformation_tag.Name + input_resource_tag.Name + "Cycles").ToTag(), resource, num / -num2);
	}

	private static void DumpEconomyDetails()
	{
		global::Debug.Log("Starting Economy Details Dump...");
		EconomyDetails details = new EconomyDetails();
		List<EconomyDetails.Scenario> list = new List<EconomyDetails.Scenario>();
		EconomyDetails.Scenario scenario = new EconomyDetails.Scenario("default", 1, (EconomyDetails.Transformation t) => true);
		list.Add(scenario);
		EconomyDetails.Scenario scenario2 = new EconomyDetails.Scenario("all_buildings", 1, (EconomyDetails.Transformation t) => t.type == details.buildingTransformationType);
		list.Add(scenario2);
		EconomyDetails.Scenario scenario3 = new EconomyDetails.Scenario("all_plants", 1, (EconomyDetails.Transformation t) => t.type == details.plantTransformationType);
		list.Add(scenario3);
		EconomyDetails.Scenario scenario4 = new EconomyDetails.Scenario("all_creatures", 1, (EconomyDetails.Transformation t) => t.type == details.creatureTransformationType);
		list.Add(scenario4);
		EconomyDetails.Scenario scenario5 = new EconomyDetails.Scenario("all_stress", 1, (EconomyDetails.Transformation t) => t.GetDelta(details.GetResource(new Tag(Db.Get().Amounts.Stress.deltaAttribute.Id))) != null);
		list.Add(scenario5);
		EconomyDetails.Scenario scenario6 = new EconomyDetails.Scenario("all_foods", 1, (EconomyDetails.Transformation t) => t.type == details.foodTransformationType);
		list.Add(scenario6);
		EconomyDetails.Scenario scenario7 = new EconomyDetails.Scenario("geysers/geysers_active_period_only", 1, (EconomyDetails.Transformation t) => t.type == details.geyserActivePeriodTransformationType);
		list.Add(scenario7);
		EconomyDetails.Scenario scenario8 = new EconomyDetails.Scenario("geyser/geysers_whole_lifetime", 1, (EconomyDetails.Transformation t) => t.type == details.geyserLifetimeTransformationType);
		list.Add(scenario8);
		EconomyDetails.Scenario scenario9 = new EconomyDetails.Scenario("oxygen/algae_distillery", 0, null);
		scenario9.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("AlgaeDistillery"), 3));
		scenario9.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("AlgaeHabitat"), 22));
		scenario9.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Duplicant"), 9));
		scenario9.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("WaterPurifier"), 1));
		list.Add(scenario9);
		EconomyDetails.Scenario scenario10 = new EconomyDetails.Scenario("oxygen/algae_habitat_electrolyzer", 0, null);
		scenario10.AddEntry(new EconomyDetails.Scenario.Entry("AlgaeHabitat", 1));
		scenario10.AddEntry(new EconomyDetails.Scenario.Entry("Duplicant", 1));
		scenario10.AddEntry(new EconomyDetails.Scenario.Entry("Electrolyzer", 1));
		list.Add(scenario10);
		EconomyDetails.Scenario scenario11 = new EconomyDetails.Scenario("oxygen/electrolyzer", 0, null);
		scenario11.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Electrolyzer"), 1));
		scenario11.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 1));
		scenario11.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Duplicant"), 9));
		scenario11.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1));
		scenario11.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("GasPump"), 1));
		list.Add(scenario11);
		EconomyDetails.Scenario scenario12 = new EconomyDetails.Scenario("purifiers/methane_generator", 0, null);
		scenario12.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("MethaneGenerator"), 1));
		scenario12.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("FertilizerMaker"), 3));
		scenario12.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Electrolyzer"), 1));
		scenario12.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("GasPump"), 1));
		scenario12.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 2));
		scenario12.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1));
		scenario12.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("PrickleFlower"), 0));
		list.Add(scenario12);
		EconomyDetails.Scenario scenario13 = new EconomyDetails.Scenario("purifiers/water_purifier", 0, null);
		scenario13.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("WaterPurifier"), 1));
		scenario13.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Compost"), 2));
		scenario13.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Electrolyzer"), 1));
		scenario13.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 2));
		scenario13.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("GasPump"), 1));
		scenario13.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("HydrogenGenerator"), 1));
		scenario13.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("PrickleFlower"), 29));
		list.Add(scenario13);
		EconomyDetails.Scenario scenario14 = new EconomyDetails.Scenario("energy/petroleum_generator", 0, null);
		scenario14.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("PetroleumGenerator"), 1));
		scenario14.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("OilRefinery"), 1));
		scenario14.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("WaterPurifier"), 1));
		scenario14.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 1));
		scenario14.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("GasPump"), 1));
		scenario14.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("CO2Scrubber"), 1));
		scenario14.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("MethaneGenerator"), 1));
		list.Add(scenario14);
		EconomyDetails.Scenario scenario15 = new EconomyDetails.Scenario("energy/coal_generator", 0, (EconomyDetails.Transformation t) => t.tag.Name.Contains("Hatch"));
		scenario15.AddEntry(new EconomyDetails.Scenario.Entry("Generator", 1));
		list.Add(scenario15);
		EconomyDetails.Scenario scenario16 = new EconomyDetails.Scenario("waste/outhouse", 0, null);
		scenario16.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Outhouse"), 1));
		scenario16.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Compost"), 1));
		list.Add(scenario16);
		EconomyDetails.Scenario scenario17 = new EconomyDetails.Scenario("stress/massage_table", 0, null);
		scenario17.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("MassageTable"), 1));
		scenario17.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("ManualGenerator"), 1));
		list.Add(scenario17);
		EconomyDetails.Scenario scenario18 = new EconomyDetails.Scenario("waste/flush_toilet", 0, null);
		scenario18.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("FlushToilet"), 1));
		scenario18.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("WaterPurifier"), 1));
		scenario18.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("LiquidPump"), 1));
		scenario18.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("FertilizerMaker"), 1));
		list.Add(scenario18);
		EconomyDetails.CollectDietScenarios(list);
		foreach (EconomyDetails.Transformation transformation in details.transformations)
		{
			EconomyDetails.Transformation transformation_iter = transformation;
			EconomyDetails.Scenario scenario19 = new EconomyDetails.Scenario("transformations/" + transformation.tag.Name, 1, (EconomyDetails.Transformation t) => transformation_iter == t);
			list.Add(scenario19);
		}
		foreach (EconomyDetails.Transformation transformation2 in details.transformations)
		{
			EconomyDetails.Scenario scenario20 = new EconomyDetails.Scenario("transformation_groups/" + transformation2.tag.Name, 0, null);
			scenario20.AddEntry(new EconomyDetails.Scenario.Entry(transformation2.tag, 1));
			foreach (EconomyDetails.Transformation transformation3 in details.transformations)
			{
				bool flag = false;
				foreach (EconomyDetails.Transformation.Delta delta in transformation2.deltas)
				{
					if (delta.resource.type != details.energyResourceType)
					{
						foreach (EconomyDetails.Transformation.Delta delta2 in transformation3.deltas)
						{
							if (delta.resource == delta2.resource)
							{
								scenario20.AddEntry(new EconomyDetails.Scenario.Entry(transformation3.tag, 0));
								flag = true;
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
			list.Add(scenario20);
		}
		foreach (EdiblesManager.FoodInfo foodInfo in FOOD.FOOD_TYPES_LIST)
		{
			EconomyDetails.Scenario scenario21 = new EconomyDetails.Scenario("food/" + foodInfo.Id, 0, null);
			Tag tag2 = TagManager.Create(foodInfo.Id);
			scenario21.AddEntry(new EconomyDetails.Scenario.Entry(tag2, 1));
			scenario21.AddEntry(new EconomyDetails.Scenario.Entry(TagManager.Create("Duplicant"), 1));
			List<Tag> list2 = new List<Tag>();
			list2.Add(tag2);
			while (list2.Count > 0)
			{
				Tag tag = list2[0];
				list2.RemoveAt(0);
				Recipe recipe = RecipeManager.Get().recipes.Find((Recipe a) => a.Result == tag);
				if (recipe != null)
				{
					foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
					{
						scenario21.AddEntry(new EconomyDetails.Scenario.Entry(ingredient.tag, 1));
						list2.Add(ingredient.tag);
					}
				}
				foreach (KPrefabID kprefabID in Assets.Prefabs)
				{
					Crop component = kprefabID.GetComponent<Crop>();
					if (component != null && component.cropVal.cropId == tag.Name)
					{
						scenario21.AddEntry(new EconomyDetails.Scenario.Entry(kprefabID.PrefabTag, 1));
						list2.Add(kprefabID.PrefabTag);
					}
				}
			}
			list.Add(scenario21);
		}
		if (!Directory.Exists("assets/Tuning/Economy"))
		{
			Directory.CreateDirectory("assets/Tuning/Economy");
		}
		foreach (EconomyDetails.Scenario scenario22 in list)
		{
			string text = "assets/Tuning/Economy/" + scenario22.name + ".csv";
			if (!Directory.Exists(global::System.IO.Path.GetDirectoryName(text)))
			{
				Directory.CreateDirectory(global::System.IO.Path.GetDirectoryName(text));
			}
			using (StreamWriter streamWriter = new StreamWriter(text))
			{
				details.DumpTransformations(scenario22, streamWriter);
			}
		}
		float dupeBreathingPerSecond = details.GetDupeBreathingPerSecond(details);
		List<EconomyDetails.BiomeTransformation> list3 = new List<EconomyDetails.BiomeTransformation>();
		list3.Add(details.CreateBiomeTransformationFromTransformation(details, "MineralDeoxidizer".ToTag(), GameTags.Algae, GameTags.Oxygen));
		list3.Add(details.CreateBiomeTransformationFromTransformation(details, "AlgaeHabitat".ToTag(), GameTags.Algae, GameTags.Oxygen));
		list3.Add(details.CreateBiomeTransformationFromTransformation(details, "AlgaeHabitat".ToTag(), GameTags.Water, GameTags.Oxygen));
		list3.Add(details.CreateBiomeTransformationFromTransformation(details, "Electrolyzer".ToTag(), GameTags.Water, GameTags.Oxygen));
		list3.Add(new EconomyDetails.BiomeTransformation("StartingOxygenCycles".ToTag(), details.GetResource(GameTags.Oxygen), 1f / -(dupeBreathingPerSecond * 600f)));
		list3.Add(new EconomyDetails.BiomeTransformation("StartingOxyliteCycles".ToTag(), details.CreateResource(GameTags.OxyRock, details.massResourceType), 1f / -(dupeBreathingPerSecond * 600f)));
		string text2 = "assets/Tuning/Economy/biomes/starting_amounts.csv";
		if (!Directory.Exists(global::System.IO.Path.GetDirectoryName(text2)))
		{
			Directory.CreateDirectory(global::System.IO.Path.GetDirectoryName(text2));
		}
		using (StreamWriter streamWriter2 = new StreamWriter(text2))
		{
			streamWriter2.Write("Resource,Amount");
			foreach (EconomyDetails.BiomeTransformation biomeTransformation in list3)
			{
				streamWriter2.Write("," + biomeTransformation.tag);
			}
			streamWriter2.Write("\n");
			streamWriter2.Write("Cells, " + details.startingBiomeCellCount + "\n");
			foreach (KeyValuePair<Element, float> keyValuePair in details.startingBiomeAmounts)
			{
				streamWriter2.Write(keyValuePair.Key.id.ToString() + ", " + keyValuePair.Value.ToString());
				foreach (EconomyDetails.BiomeTransformation biomeTransformation2 in list3)
				{
					streamWriter2.Write(",");
					float num = biomeTransformation2.Transform(keyValuePair.Key, keyValuePair.Value);
					if (num > 0f)
					{
						streamWriter2.Write(num);
					}
				}
				streamWriter2.Write("\n");
			}
		}
		global::Debug.Log("Completed economy details dump!!");
	}

	private List<EconomyDetails.Transformation> transformations = new List<EconomyDetails.Transformation>();

	private List<EconomyDetails.Resource> resources = new List<EconomyDetails.Resource>();

	public Dictionary<Element, float> startingBiomeAmounts = new Dictionary<Element, float>();

	public int startingBiomeCellCount;

	public EconomyDetails.Resource energyResource;

	public EconomyDetails.Resource heatResource;

	public EconomyDetails.Resource duplicantTimeResource;

	public EconomyDetails.Resource caloriesResource;

	public EconomyDetails.Resource.Type massResourceType;

	public EconomyDetails.Resource.Type energyResourceType;

	public EconomyDetails.Resource.Type timeResourceType;

	public EconomyDetails.Resource.Type attributeResourceType;

	public EconomyDetails.Resource.Type caloriesResourceType;

	public EconomyDetails.Resource.Type amountResourceType;

	public EconomyDetails.Transformation.Type buildingTransformationType;

	public EconomyDetails.Transformation.Type foodTransformationType;

	public EconomyDetails.Transformation.Type plantTransformationType;

	public EconomyDetails.Transformation.Type creatureTransformationType;

	public EconomyDetails.Transformation.Type dupeTransformationType;

	public EconomyDetails.Transformation.Type referenceTransformationType;

	public EconomyDetails.Transformation.Type effectTransformationType;

	private const string GEYSER_ACTIVE_SUFFIX = "_ActiveOnly";

	public EconomyDetails.Transformation.Type geyserActivePeriodTransformationType;

	public EconomyDetails.Transformation.Type geyserLifetimeTransformationType;

	private static string debugTag = "CO2Scrubber";

	public class Resource
	{
		public Resource(Tag tag, EconomyDetails.Resource.Type type)
		{
			this.tag = tag;
			this.type = type;
		}

		public Tag tag { get; private set; }

		public EconomyDetails.Resource.Type type { get; private set; }

		public class Type
		{
			public Type(string id, string unit)
			{
				this.id = id;
				this.unit = unit;
			}

			public string id { get; private set; }

			public string unit { get; private set; }
		}
	}

	public class BiomeTransformation
	{
		public BiomeTransformation(Tag tag, EconomyDetails.Resource resource, float ratio)
		{
			this.tag = tag;
			this.resource = resource;
			this.ratio = ratio;
		}

		public Tag tag { get; private set; }

		public EconomyDetails.Resource resource { get; private set; }

		public float ratio { get; private set; }

		public float Transform(Element element, float amount)
		{
			if (this.resource.tag == element.tag)
			{
				return this.ratio * amount;
			}
			return 0f;
		}
	}

	public class Ratio
	{
		public Ratio(EconomyDetails.Resource input, EconomyDetails.Resource output, bool allow_negative_output)
		{
			this.input = input;
			this.output = output;
			this.allowNegativeOutput = allow_negative_output;
		}

		public EconomyDetails.Resource input { get; private set; }

		public EconomyDetails.Resource output { get; private set; }

		public bool allowNegativeOutput { get; private set; }
	}

	public class Scenario
	{
		public Scenario(string name, int default_count, Func<EconomyDetails.Transformation, bool> filter)
		{
			this.name = name;
			this.defaultCount = default_count;
			this.filter = filter;
			this.timeInSeconds = 600f;
		}

		public string name { get; private set; }

		public int defaultCount { get; private set; }

		public float timeInSeconds { get; set; }

		public void AddEntry(EconomyDetails.Scenario.Entry entry)
		{
			this.entries.Add(entry);
		}

		public int GetCount(Tag tag)
		{
			foreach (EconomyDetails.Scenario.Entry entry in this.entries)
			{
				if (entry.tag == tag)
				{
					return entry.count;
				}
			}
			return this.defaultCount;
		}

		public bool IncludesTransformation(EconomyDetails.Transformation transformation)
		{
			if (this.filter != null && this.filter(transformation))
			{
				return true;
			}
			foreach (EconomyDetails.Scenario.Entry entry in this.entries)
			{
				if (entry.tag == transformation.tag)
				{
					return true;
				}
			}
			return false;
		}

		private Func<EconomyDetails.Transformation, bool> filter;

		private List<EconomyDetails.Scenario.Entry> entries = new List<EconomyDetails.Scenario.Entry>();

		public class Entry
		{
			public Entry(Tag tag, int count)
			{
				this.tag = tag;
				this.count = count;
			}

			public Tag tag { get; private set; }

			public int count { get; private set; }
		}
	}

	public class Transformation
	{
		public Transformation(Tag tag, EconomyDetails.Transformation.Type type, float time_in_seconds)
		{
			this.tag = tag;
			this.type = type;
			this.timeInSeconds = time_in_seconds;
		}

		public Tag tag { get; private set; }

		public EconomyDetails.Transformation.Type type { get; private set; }

		public float timeInSeconds { get; private set; }

		public void AddDelta(EconomyDetails.Transformation.Delta delta)
		{
			this.deltas.Add(delta);
		}

		public EconomyDetails.Transformation.Delta GetDelta(EconomyDetails.Resource resource)
		{
			foreach (EconomyDetails.Transformation.Delta delta in this.deltas)
			{
				if (delta.resource == resource)
				{
					return delta;
				}
			}
			return null;
		}

		public List<EconomyDetails.Transformation.Delta> deltas = new List<EconomyDetails.Transformation.Delta>();

		public class Delta
		{
			public Delta(EconomyDetails.Resource resource, float amount)
			{
				this.resource = resource;
				this.amount = amount;
			}

			public EconomyDetails.Resource resource { get; private set; }

			public float amount { get; set; }
		}

		public class Type
		{
			public Type(string id)
			{
				this.id = id;
			}

			public string id { get; private set; }
		}
	}
}
