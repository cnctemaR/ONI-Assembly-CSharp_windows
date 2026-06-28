using System;
using TUNING;
using UnityEngine;

public class SuitLockerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SuitLocker";
		int num = 1;
		int num2 = 3;
		string text2 = "changingarea_kanim";
		int num3 = 30;
		float num4 = 30f;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, refined_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.PreventIdlingInFrontOfBuilding = true;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "SuitLocker");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		SuitLocker suitLocker = go.AddOrGet<SuitLocker>();
		suitLocker.OutfitTags = new Tag[] { GameTags.Suit };
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.capacityKG = 200f;
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.tags = new Tag[]
		{
			new Tag("SuitLocker"),
			new Tag("SuitMarker")
		};
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 400f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "SuitLocker";
}
