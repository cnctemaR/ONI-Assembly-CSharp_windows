using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MetalRefineryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MetalRefinery";
		int num = 3;
		int num2 = 4;
		string text2 = "metalrefinery_kanim";
		int num3 = 30;
		float num4 = 60f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] all_MINERALS = MATERIALS.ALL_MINERALS;
		float num5 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_MINERALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 1200f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 1);
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		LiquidCooledRefinery liquidCooledRefinery = go.AddOrGet<LiquidCooledRefinery>();
		BuildingTemplates.CreateRefineryStorage(go, liquidCooledRefinery);
		liquidCooledRefinery.coolantTag = MetalRefineryConfig.COOLANT_TAG;
		liquidCooledRefinery.minCoolantMass = 400f;
		liquidCooledRefinery.outStorage.capacityKg = 2000f;
		liquidCooledRefinery.thermalFudge = 0.8f;
		liquidCooledRefinery.inStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
		liquidCooledRefinery.buildStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
		liquidCooledRefinery.outStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
		liquidCooledRefinery.outputOffset = new Vector3(1f, 0.5f);
		liquidCooledRefinery.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_metalrefinery_kanim") };
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.capacityKG = 800f;
		conduitConsumer.storage = liquidCooledRefinery.inStorage;
		conduitConsumer.alwaysConsume = true;
		conduitConsumer.forceAlwaysSatisfied = true;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.storage = liquidCooledRefinery.outStorage;
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = null;
		conduitDispenser.alwaysDispense = true;
		List<Element> list = ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.Metal));
		foreach (Element element in list)
		{
			Element highTempTransition = element.highTempTransition;
			Element lowTempTransition = highTempTransition.lowTempTransition;
			if (lowTempTransition != element)
			{
				new RefinementRecipe
				{
					material = element.tag,
					amount = 100f,
					time = 40f,
					description = string.Format(global::STRINGS.BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, lowTempTransition.name, element.name),
					fabricators = new List<Tag> { TagManager.Create("MetalRefinery", null) }
				}.AddResult(lowTempTransition.tag, 100f);
			}
		}
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveStoppableController.Instance instance = new PoweredActiveStoppableController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "MetalRefinery";

	private const float INPUT_KG = 100f;

	private const float LIQUID_COOLED_HEAT_PORTION = 0.8f;

	private static readonly Tag COOLANT_TAG = GameTags.Liquid;

	private const float COOLANT_MASS = 400f;

	private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate
	};
}
