using System;
using TUNING;
using UnityEngine;

public class MicrobeMusherConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MicrobeMusher", 2, 3, "microbemusher_kanim", 1200f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, null);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.OperatingKilowatts = 2f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		MicrobeMusher microbeMusher = go.AddOrGet<MicrobeMusher>();
		microbeMusher.mushbarSpawnOffset = new Vector3(1f, 0f, 0f);
		microbeMusher.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_musher_kanim") };
		BuildingTemplates.CreateFabricatorStorage(go, microbeMusher);
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

	public const string ID = "MicrobeMusher";
}
