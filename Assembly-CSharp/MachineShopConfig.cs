using System;
using TUNING;
using UnityEngine;

public class MachineShopConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MachineShop";
		int num = 4;
		int num2 = 2;
		string text2 = "machineshop_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.Deprecated = true;
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.MachineShop);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "MachineShop";

	public static readonly Tag MATERIAL_FOR_TINKER = GameTags.RefinedMetal;

	public const float MASS_PER_TINKER = 5f;

	public static readonly string ROLE_PERK = "IncreaseMachinery";
}
