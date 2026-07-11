using System;
using TUNING;
using UnityEngine;

public class CargoBayConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "CargoBay";
		int num = 5;
		int num2 = 5;
		string text2 = "rocket_storage_solid_kanim";
		int num3 = 1000;
		float num4 = 60f;
		string[] array = new string[]
		{
			"BuildableRaw",
			SimHashes.Steel.ToString()
		};
		float[] array2 = new float[]
		{
			ROCKETRY.CARGO_CONTAINER_MASS.STATIC_MASS,
			ROCKETRY.CARGO_CONTAINER_MASS.STATIC_MASS
		};
		string[] array3 = array;
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.BuildingAttachPoint;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array2, array3, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier, 0.2f);
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.Invincible = true;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		buildingDef.OutputConduitType = ConduitType.Solid;
		buildingDef.UtilityOutputOffset = new CellOffset(0, 3);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		CargoBay cargoBay = go.AddOrGet<CargoBay>();
		cargoBay.storage = go.AddOrGet<Storage>();
		cargoBay.storageType = CargoBay.CargoType.solids;
		cargoBay.storage.capacityKg = 1000f;
		cargoBay.storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<RocketModule>().SetBGKAnim(Assets.GetAnim("rocket_storage_solid_bg_kanim"));
		EntityTemplates.ExtendBuildingToRocketModule(go);
		go.AddOrGet<SolidConduitDispenser>();
	}

	public const string ID = "CargoBay";
}
