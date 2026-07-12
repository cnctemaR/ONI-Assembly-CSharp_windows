using System;
using TUNING;
using UnityEngine;

public class OxygenMaskLockerConfig : IBuildingConfig
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "OxygenMaskLocker";
		int num = 1;
		int num2 = 2;
		string text2 = "oxygen_mask_locker_kanim";
		int num3 = 30;
		float num4 = 30f;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float[] array = new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		};
		string[] array2 = raw_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
		buildingDef.PreventIdleTraversalPastBuilding = true;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "OxygenMaskLocker");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<SuitLocker>().OutfitTags = new Tag[] { GameTags.OxygenMask };
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.capacityKG = 30f;
		go.AddOrGet<AnimTileable>().tags = new Tag[]
		{
			new Tag("OxygenMaskLocker"),
			new Tag("OxygenMaskMarker")
		};
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}

	public const string ID = "OxygenMaskLocker";
}
