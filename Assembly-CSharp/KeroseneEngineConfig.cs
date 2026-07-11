using System;
using TUNING;
using UnityEngine;

public class KeroseneEngineConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "KeroseneEngine";
		int num = 7;
		int num2 = 5;
		string text2 = "rocket_petroleum_engine_kanim";
		int num3 = 1000;
		float num4 = 60f;
		float[] engine_MASS_SMALL = BUILDINGS.ROCKETRY_MASS_KG.ENGINE_MASS_SMALL;
		string[] array = new string[] { SimHashes.Steel.ToString() };
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloorOrBuildingAttachPoint;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, engine_MASS_SMALL, array, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier, 0.2f);
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ViewMode = SimViewMode.None;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = false;
		buildingDef.CanMove = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[]
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
		BuildingTemplates.DoPostConfigure(go);
		RocketEngine rocketEngine = go.AddOrGet<RocketEngine>();
		rocketEngine.thrustAmount = (float)ROCKETRY.MODULE_THRUST_SCORE.ENGINES.MEDIUM;
		rocketEngine.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
		EntityTemplates.ExtendEntityToRocketModule(go);
		go.AddOrGet<RocketModule>();
	}

	public const string ID = "KeroseneEngine";
}
