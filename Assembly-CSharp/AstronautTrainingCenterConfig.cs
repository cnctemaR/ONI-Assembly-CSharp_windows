using System;
using TUNING;
using UnityEngine;

public class AstronautTrainingCenterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "AstronautTrainingCenter";
		int num = 5;
		int num2 = 5;
		string text2 = "centrifuge_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.PowerInputOffset = new CellOffset(-2, 0);
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
		AstronautTrainingCenter astronautTrainingCenter = go.AddOrGet<AstronautTrainingCenter>();
		astronautTrainingCenter.workTime = float.PositiveInfinity;
		astronautTrainingCenter.requiredRolePerk = RoleManager.rolePerks.CanTrainToBeAstronaut.id;
		astronautTrainingCenter.daysToMasterRole = 10f;
		astronautTrainingCenter.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_centrifuge_kanim") };
		astronautTrainingCenter.workLayer = Grid.SceneLayer.BuildingFront;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "AstronautTrainingCenter";
}
