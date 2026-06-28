using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class RockCrusherConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "RockCrusher";
		int num = 4;
		int num2 = 4;
		string text2 = "rockrefinery_kanim";
		float num3 = 200f;
		int num4 = 30;
		float num5 = 60f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.OperatingKilowatts = 16f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Refinery refinery = go.AddOrGet<Refinery>();
		BuildingTemplates.CreateRefineryStorage(go, refinery);
		refinery.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_rockrefinery_kanim") };
		Tag tag = SimHashes.Sand.CreateTag();
		List<Element> list = ElementLoader.elements.FindAll((Element e) => e.HasTag(GameTags.Crushable));
		foreach (Element element in list)
		{
			new RefinementRecipe
			{
				material = element.tag,
				amount = 100f,
				time = 40f,
				description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, element.name, tag.ProperName()),
				fabricators = new List<Tag> { TagManager.Create("RockCrusher", null) }
			}.AddResult(tag, 100f);
		}
		List<Element> list2 = ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.Metal));
		foreach (Element element2 in list2)
		{
			Element highTempTransition = element2.highTempTransition;
			Element lowTempTransition = highTempTransition.lowTempTransition;
			if (lowTempTransition != element2)
			{
				new RefinementRecipe
				{
					material = element2.tag,
					amount = 100f,
					time = 40f,
					description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.METAL_RECIPE_DESCRIPTION, lowTempTransition.name, element2.name),
					fabricators = new List<Tag> { TagManager.Create("RockCrusher", null) }
				}.AddResult(lowTempTransition.tag, 50f).AddResult(tag, 50f);
			}
		}
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "RockCrusher";

	private const float INPUT_KG = 100f;

	private const float METAL_ORE_EFFICIENCY = 0.5f;
}
