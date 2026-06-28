using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EggIncubatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "EggIncubator";
		int num = 2;
		int num2 = 3;
		string text2 = "incubator_kanim";
		int num3 = 30;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER0, none, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.OverheatTemperature = 363.15f;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.SetDefaultStoredItemModifiers(EggIncubatorConfig.IncubatorStorage);
		EggIncubator eggIncubator = go.AddOrGet<EggIncubator>();
		eggIncubator.AddDepositTag(GameTags.Egg);
		eggIncubator.SetWorkTime(5f);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "EggIncubator";

	public static readonly List<Storage.StoredItemModifier> IncubatorStorage = new List<Storage.StoredItemModifier> { Storage.StoredItemModifier.Preserve };
}
